using ServersDataAggregation.Common.Model;
using ServersDataAggregation.Query.Games.Quake2.Packets;
using System.Collections;
using System.Net.Sockets;
using System.Text;

namespace ServersDataAggregation.Query.Games.Quake2;

class Quake2 : IServerInfoProvider
{
    private const string Q2_SETTING_HOSTNAME = "hostname";
    private const string Q2_SETTING_MAP = "mapname";
    private const string Q2_SETTING_VERSION = "version";
    private const string Q2_SETTING_MAXPLAYERS = "maxclients";
    private const string Q2_SETTING_MOD = "gamedir";

    private string _address;
    private int _port;

    #region IServerInfoProvider Members

    public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
    {
        _address = pServerAddress;
        _port = pServerPort;

        UdpUtility udp = new UdpUtility(pServerAddress, pServerPort);
        Q2ServerStatus q2Server = new Q2ServerStatus();

        byte[] receivedBytes = udp.SendBytes(Q2ServerStatus.StatusRequest);

        if (receivedBytes == null || receivedBytes.Length == 0)
            throw new SocketException((int)SocketError.NoData);

        q2Server.ParseBytes(receivedBytes);

        ServerSnapshot info = GetServerInfo(q2Server);
        info.Port = udp.RemotePort;
        info.IpAddress = udp.RemoteIpAddress;
        return info;
    }

    #endregion

    private ServerSnapshot GetServerInfo(Q2ServerStatus pStatus)
    {
        ServerSnapshot sInfo = new ServerSnapshot();
        try
        {
            if (pStatus.ServerSettings.Contains(Q2_SETTING_HOSTNAME)) sInfo.ServerName = pStatus.ServerSettings[Q2_SETTING_HOSTNAME].ToString();
            if (pStatus.ServerSettings.Contains(Q2_SETTING_MAXPLAYERS)) sInfo.MaxPlayerCount = int.Parse(pStatus.ServerSettings[Q2_SETTING_MAXPLAYERS].ToString());
            if (pStatus.ServerSettings.Contains(Q2_SETTING_MAP)) sInfo.Map = pStatus.ServerSettings[Q2_SETTING_MAP].ToString();
            if (pStatus.ServerSettings.Contains(Q2_SETTING_MOD)) sInfo.Mod = pStatus.ServerSettings[Q2_SETTING_MOD].ToString();
            if (pStatus.ServerSettings.Contains(Q2_SETTING_VERSION)) sInfo.ServerVersion = pStatus.ServerSettings[Q2_SETTING_VERSION].ToString();
        }
        catch
        {
            throw new FormatException("Q2 Server data was not in correct format");
        }

        try
        {
            sInfo.Players = pStatus.CurrentPlayers.Select(playerInfo => new PlayerSnapshot()
                {
                    Name = playerInfo.PlayerName,
                    Ping = int.Parse(playerInfo.Ping),
                    Frags = int.Parse(playerInfo.Frags)
                })
                .ToArray();
        }
        catch
        {
            throw new FormatException("Q2 Player data was not in correct format");
        }

        var serverSettings = new List<ServerSetting>();
        foreach (DictionaryEntry entry in pStatus.ServerSettings)
        {
            serverSettings.Add(new ServerSetting(entry.Key.ToString(), entry.Value.ToString()));
        }
        sInfo.ServerSettings = serverSettings.ToArray();

        return sInfo;
    }
}
using ServersDataAggregation.Common.Model;
using ServersDataAggregation.Query.Games.Common;
using ServersDataAggregation.Query.Games.Quake3.Packets;
using System.Collections;
using System.Net.Sockets;

namespace ServersDataAggregation.Query.Games.Quake3;

internal class Quake3 : IServerInfoProvider
{
    private const string Q3_SETTING_SV_HOSTNAME = "sv_hostname";
    private const string Q3_SETTING_HOSTNAME = "hostname";
    private const string Q3_SETTING_MAP = "mapname";
    private const string Q3_SETTING_VERSION = "version";
    private const string Q3_SETTING_MAXPLAYERS = "sv_maxclients";
    private const string Q3_SETTING_GAMENAME = "gamename";
    private const string Q3_SETTING_MOD = "modname";

    #region IServerInfoProvider Members

    public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
    {
        UdpUtility udp = new UdpUtility(pServerAddress, pServerPort);
        Q3ServerStatus q3Server = new Q3ServerStatus();

        byte[] receivedBytes = udp.SendBytes(Q3ServerStatus.StatusRequest);

        if (receivedBytes == null || receivedBytes.Length == 0)
            throw new SocketException((int)SocketError.NoData);

        q3Server.ParseBytes(receivedBytes);

        ServerSnapshot info = GetServerInfo(q3Server);
        info.Port = udp.RemotePort;
        info.IpAddress = udp.RemoteIpAddress;
        return info;
    }

    #endregion

    private ServerSnapshot GetServerInfo(Q3ServerStatus pStatus)
    {
        ServerSnapshot sInfo = new ServerSnapshot();
        try
        {
            if (pStatus.ServerSettings.Contains(Q3_SETTING_HOSTNAME)) sInfo.ServerName = pStatus.ServerSettings[Q3_SETTING_HOSTNAME].ToString();
            if (pStatus.ServerSettings.Contains(Q3_SETTING_SV_HOSTNAME)) sInfo.ServerName = pStatus.ServerSettings[Q3_SETTING_SV_HOSTNAME].ToString();
            if (pStatus.ServerSettings.Contains(Q3_SETTING_MAXPLAYERS)) sInfo.MaxPlayerCount = int.Parse(pStatus.ServerSettings[Q3_SETTING_MAXPLAYERS].ToString());
            if (pStatus.ServerSettings.Contains(Q3_SETTING_MAP)) sInfo.Map = pStatus.ServerSettings[Q3_SETTING_MAP].ToString();
            if (pStatus.ServerSettings.Contains(Q3_SETTING_MOD)) 
                sInfo.Mod = pStatus.ServerSettings[Q3_SETTING_MOD].ToString();
            else if(pStatus.ServerSettings.Contains(Q3_SETTING_GAMENAME))
                sInfo.Mod = pStatus.ServerSettings[Q3_SETTING_GAMENAME].ToString();
            if (pStatus.ServerSettings.Contains(Q3_SETTING_MOD)) sInfo.Mod = pStatus.ServerSettings[Q3_SETTING_MOD].ToString();
            if (pStatus.ServerSettings.Contains(Q3_SETTING_VERSION)) sInfo.ServerVersion = pStatus.ServerSettings[Q3_SETTING_VERSION].ToString();

        }
        catch
        {
            throw new FormatException("Q3 Server data was not in correct format");
        }

        try
        {

            sInfo.Players = pStatus.CurrentPlayers.Select(playerInfo => new PlayerSnapshot()
            {
                NameRaw = playerInfo.PlayerNameBytes,
                Name = NameUtils.PlayerBytesToString(playerInfo.PlayerNameBytes),
                Ping = int.Parse(playerInfo.Ping),
                Frags = int.Parse(playerInfo.Frags),
                
            })
                .ToArray();
        }
        catch
        {
            throw new FormatException("Q3 Player data was not in correct format");
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

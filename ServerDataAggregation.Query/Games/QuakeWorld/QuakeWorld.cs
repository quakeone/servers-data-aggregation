using System.Text;
using ServersDataAggregation.Query.Games.QuakeWorld.Packets;
using System.Collections;
using System.Net.Sockets;
using ServersDataAggregation.Common.Model;
using ServersDataAggregation.Query.Games.NetQuake.Packets;
using ServersDataAggregation.Query.Games.Common;

namespace ServersDataAggregation.Query.Games.QuakeWorld;

public class QuakeWorld : IServerInfoProvider
{
    private const string QW_SETTING_HOSTNAME = "hostname";
    private const string QW_SETTING_MAP = "map";
    private const string QW_SETTING_VERSION = "*version";
    private const string QW_SETTING_MAXPLAYERS = "maxclients";
    private const string QW_SETTING_MOD = "*gamedir";
    private ServerParameters _serverParams;

    public QuakeWorld(ServerParameters parameters)
    {
        _serverParams = parameters;
    }

    #region IServerInfoProvider Members

    public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
    {
        UdpUtility udp = new UdpUtility(pServerAddress, pServerPort);
        QWServerStatus qwServer = new QWServerStatus();

        byte[] receivedBytes = udp.SendBytes(qwServer.StatusRequest);

        if (receivedBytes == null || receivedBytes.Length == 0)
            throw new SocketException((int)SocketError.NoData);

        qwServer.ParseBytes(receivedBytes, _serverParams);

        ServerSnapshot info = GetServerInfo(qwServer);
        info.Port = udp.RemotePort;
        info.IpAddress = udp.RemoteIpAddress;
        return info;
    }

    #endregion

    private ServerSnapshot GetServerInfo(QWServerStatus pStatus)
    {
        ServerSnapshot sInfo = new ServerSnapshot();
        try
        {
            if (pStatus.ServerSettings.Contains(QW_SETTING_HOSTNAME)) sInfo.ServerName = pStatus.ServerSettings[QW_SETTING_HOSTNAME].ToString();
            if (pStatus.ServerSettings.Contains(QW_SETTING_MAXPLAYERS)) sInfo.MaxPlayerCount = int.Parse(pStatus.ServerSettings[QW_SETTING_MAXPLAYERS].ToString());
            if (pStatus.ServerSettings.Contains(QW_SETTING_MAP)) sInfo.Map = pStatus.ServerSettings[QW_SETTING_MAP].ToString();
            if (pStatus.ServerSettings.Contains(QW_SETTING_MOD)) sInfo.Mod = pStatus.ServerSettings[QW_SETTING_MOD].ToString();
            if (pStatus.ServerSettings.Contains(QW_SETTING_VERSION)) sInfo.ServerVersion = pStatus.ServerSettings[QW_SETTING_VERSION].ToString();
        }
        catch
        {
            throw new FormatException("QW Server data was not in correct format");
        }

        try
        {
            sInfo.Players = pStatus.CurrentPlayers.Select(playerInfo => new PlayerSnapshot()
            {
                FeatureFlags = PlayerSnapshotFeatureFlags.Clothes,
                SkinName = playerInfo.SkinName.Replace("\"", ""),
                ShirtColor = int.Parse(playerInfo.ShirtColor),
                PantColor = int.Parse(playerInfo.PantColor),
                Number = (int)playerInfo.PlayerNumber,
                Name = NameUtils.PlayerBytesToString(playerInfo.PlayerBytes),
                NameRaw = playerInfo.PlayerBytes,
                PlayTime = playerInfo.PlayMins == null ? new TimeSpan(0) : TimeSpan.FromMinutes(int.Parse(playerInfo.PlayMins)),
                Ping = int.Parse(playerInfo.Ping),
                Frags = int.Parse(playerInfo.Frags)
            })
            .ToArray();
        }
        catch
        {
            throw new FormatException("QW Player data was not in correct format");
        }

        var serverSettings = new List<ServerSetting>();
        foreach (DictionaryEntry entry in pStatus.ServerSettings)
        {
            serverSettings.Add(new ServerSetting(entry.Key.ToString(), entry.Value.ToString()));
        }
        sInfo.ServerSettings = serverSettings.ToArray();
        sInfo = MatchParamsHelper.DeriveParams(sInfo);

        return sInfo;
    }


    //private string ColorTranslator(int pQuakeColorNum)
    //{
    //    switch (pQuakeColorNum)
    //    {
    //        case 0:
    //            return "White";
    //        case 1:
    //            return "Brown";
    //        case 2:
    //            return "Blue";
    //        case 3:
    //            return "Green";
    //        case 4:
    //            return "Red";
    //        case 5:
    //            return "Tan";
    //        case 6:
    //            return "Pink";
    //        case 7:
    //            return "LightBrown";
    //        case 8:
    //            return "Purple";
    //        case 9:
    //            return "Violet";
    //        case 10:
    //            return "LightViolet";
    //        case 11:
    //            return "Teal";
    //        case 12:
    //            return "Yellow";
    //        case 13:
    //            return "Blue";
    //        default:
    //            return "Unknown";
    //    }
    //}
}

using System.Text;
using ServersDataAggregation.Query.Games.QuakeWorld.Packets;
using System.Collections;
using System.Net.Sockets;
using ServersDataAggregation.Common.Model;

namespace ServersDataAggregation.Query.Games.QuakeWorld;

public class QuakeWorld : IServerInfoProvider
{
    private const string QW_SETTING_HOSTNAME = "hostname";
    private const string QW_SETTING_MAP = "map";
    private const string QW_SETTING_VERSION = "*version";
    private const string QW_SETTING_MAXPLAYERS = "maxclients";
    private const string QW_SETTING_MOD = "*gamedir";
    
    #region IServerInfoProvider Members

    public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
    {
        UdpUtility udp = new UdpUtility(pServerAddress, pServerPort);
        QWServerStatus qwServer = new QWServerStatus();

        byte[] receivedBytes = udp.SendBytes(qwServer.StatusRequest);

        if (receivedBytes == null || receivedBytes.Length == 0)
            throw new SocketException((int)SocketError.NoData);

        qwServer.ParseBytes(receivedBytes);

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
                PlayerName = Encoding.UTF8.GetString(playerInfo.PlayerBytes),
                PlayTime = TimeSpan.FromMinutes(int.Parse(playerInfo.PlayMins)),
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

        return sInfo;
    }

    //public string PlayerBytesToString(byte[] pPlayerName)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    foreach (byte b in pPlayerName)
    //    {
    //        sb.Append(ConvertByteToChar(b));
    //    }
    //    return sb.ToString();
    //}

    //public char ConvertByteToChar(byte pByte)
    //{
    //    if (pByte >= 0x20 && pByte < 0x80)
    //        return (char)pByte;
    //    if (pByte >= 0xA0 && pByte <= 0xFF)
    //        return (char)(pByte - 128);

    //    switch (pByte)
    //    {
    //        case 0x0d:
    //        case 0x8d:
    //            return '>';
    //        case 0x00:
    //        case 0x05:
    //        case 0x0e:
    //        case 0x0f:
    //        case 0x0c:
    //        case 0x85:
    //        case 0x8e:
    //        case 0x8f:
    //        case 0x9c:
    //            return (char)183; // Middle dot, product of sign
    //        case 0x10:
    //        case 0x90:
    //            return '[';
    //        case 0x11:
    //        case 0x91:
    //            return ']';
    //        case 0x12:
    //        case 0x92:
    //            return '0';
    //        case 0x13:
    //        case 0x93:
    //            return '1';
    //        case 0x14:
    //        case 0x94:
    //            return '2';
    //        case 0x15:
    //        case 0x95:
    //            return '3';
    //        case 0x96:
    //        case 0x16:
    //            return '4';
    //        case 0x17:
    //        case 0x97:
    //            return '5';
    //        case 0x18:
    //        case 0x98:
    //            return '6';
    //        case 0x19:
    //        case 0x99:
    //            return '7';
    //        case 0x1A:
    //        case 0x9A:
    //            return '8';
    //        case 0x1B:
    //        case 0x9B:
    //            return '9';
    //    }
    //    return ' ';
    //}

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

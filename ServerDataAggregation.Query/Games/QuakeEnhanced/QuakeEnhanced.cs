using System.Text;
using ServersDataAggregation.Query.Games.NetQuake.Packets;
using ServersDataAggregation.Common.Model;
using ServersDataAggregation.Query.Dtls;
using ServersDataAggregation.Common.Enums;

namespace ServersDataAggregation.Query.Games.QuakeEnhanced;

public class QuakeEnhanced : IServerInfoProvider
{
    private byte[] _pskId = Encoding.UTF8.GetBytes("id-quake-ex-dtls");
    private ServerParameters _serverParams;

    private byte getHexValue(char hex)
    {
        int val = (int)hex;
        return (byte)(val - (val < 58 ? 48 : (val < 97 ? 55 : 87)));
    }

    private INetCommunicate GetNetUtility(string pServerAddress, int pServerPort, ServerParameters serverParams)
    {
        if (!string.IsNullOrEmpty(serverParams.Engine) && serverParams.Engine.ToLower() == "fte")
        {
            return new UdpUtility(pServerAddress, pServerPort);
        } 
        else
        {
            var psk = Environment.GetEnvironmentVariable("QE_PSK");
            if (string.IsNullOrEmpty(psk))
            {
                throw new ArgumentException("Quake Enhanced needs a PSK for traffic encryption");
            }

            return new DtlsUtility(StringToBytes(psk), _pskId, pServerAddress, pServerPort);
        }
    }

    // I'm sure there's a one liner for this...
    private byte[] StringToBytes ( string byteString)
    {
        var byteLength = byteString.Length / 2;
        byte[] bytes = new byte[byteLength];

        var charArray = byteString.ToCharArray();
        for(int i = 0,
            j = 0; i < byteLength; i++, j += 2)
        {
            bytes[i] = (byte)(getHexValue(charArray[j]) << 4 | getHexValue(charArray[j + 1]));
        }
        return bytes;
    }

    public QuakeEnhanced(ServerParameters parameters)
    {
        _serverParams = parameters;
    }

    public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
    {
        var net = GetNetUtility(pServerAddress, pServerPort, _serverParams);
        ServerSnapshot serverInfo = new ServerSnapshot();

        byte[] bytesReceived = net.SendBytes(new ServerInfoRequest().GetPacket());
        if (bytesReceived == null)
            throw new Exception("Communications exception");

        ServerInfoReply? serverInfoReply = GetReplyPacket(bytesReceived) as ServerInfoReply;

        if (serverInfoReply == null)
            throw new Exception("Communications exception");

        var players = new List<PlayerSnapshot>();
        var serverSnapshot = new ServerSnapshot()
        {
            Port = pServerPort,
            IpAddress = serverInfoReply.Address,
            ServerName = serverInfoReply.HostName,
            Map = serverInfoReply.MapName,
            MaxPlayerCount = (int)serverInfoReply.MaxPlayers,
            ServerVersion = serverInfoReply.GameProtocol.ToString(),
        };

        string ruleName = string.Empty;
        var serverRules = new List<ServerSetting>();

        do
        {
            bytesReceived = net.SendBytes(new RuleInfoRequest(ruleName).GetPacket());
            RuleInfoReply? rulesInfoReply = GetReplyPacket(bytesReceived) as RuleInfoReply;

            if (rulesInfoReply == null)
                throw new Exception("Communications exception");

            if (string.IsNullOrEmpty(rulesInfoReply.RuleName))
                break;

            serverRules.Add(new ServerSetting(rulesInfoReply.RuleName, rulesInfoReply.RuleValue));
            ruleName = rulesInfoReply.RuleName;

        } while (!string.IsNullOrEmpty(ruleName.Trim()));

        int playerCount = serverInfoReply.CurrentPlayers;

        for (int i = 0, j = 0; i < playerCount; i++, j++)
        {
            try
            { 
                bytesReceived = net.SendBytes(new PlayerInfoRequest(j).GetPacket()); 
            }
            catch
            {
                Console.WriteLine("My player hack isn't working ..");
                continue;
            }

            PlayerInfoReply? playerInfoReply = GetReplyPacket(bytesReceived) as PlayerInfoReply;

            if (playerInfoReply == null)
                throw new Exception("Communications exception");
            if (playerInfoReply.PlayerName == null || playerInfoReply.PlayerName.Length == 0)
                break;

            if (playerInfoReply.Address == "") i--; // this is a hack to account for the playerCount issue when bots are on the server.
            if (playerInfoReply.PlayerName == null || playerInfoReply.PlayerName.Length == 0)
                break;

            players.Add(CreatePlayerSnapshot(playerInfoReply));
        }

        serverInfo.Port = pServerPort;
        serverInfo.IpAddress = pServerAddress; // udp.RemoteIpAddress;
        serverSnapshot.Players = players.ToArray();
        serverInfo.ServerSettings = serverRules.ToArray();

        return serverInfo;
    }

    private ReplyPacket? GetReplyPacket(byte[] pBytes)
    {
        byte controlByte = ReplyPacket.GetControlByte(pBytes);
        ReplyPacket? reply = null;

        switch (controlByte)
        {
            case QuakeNetworkPacket.CCREP_SERVER_INFO:
                reply = new ServerInfoReply();
                reply.SetPacket(pBytes);
                return reply;

            case QuakeNetworkPacket.CCREP_RULE_INFO:
                reply = new RuleInfoReply();
                reply.SetPacket(pBytes);
                return reply;

            case QuakeNetworkPacket.CCREP_PLAYER_INFO:
                reply = new PlayerInfoReply();
                reply.SetPacket(pBytes);
                return reply;
        }
        return reply;
    }

    private PlayerSnapshot CreatePlayerSnapshot(PlayerInfoReply pReplyPacket)
    {
        return new PlayerSnapshot
        {
            FeatureFlags = PlayerSnapshotFeatureFlags.Clothes | PlayerSnapshotFeatureFlags.PlayerType,
            PlayerName = Encoding.UTF8.GetString(pReplyPacket.PlayerName),
            PlayerType = pReplyPacket.Address == "LOCAL" ?
                    PlayerType.Host :
                    pReplyPacket.Address == "" ? PlayerType.Bot : PlayerType.Normal,
            Frags = (short)pReplyPacket.FragCount,
            PantColor = (int)pReplyPacket.PantColor,
            ShirtColor = (int)pReplyPacket.ShirtColor,
            PlayTime = TimeSpan.FromSeconds(pReplyPacket.PlayTime)
        };
    }
}

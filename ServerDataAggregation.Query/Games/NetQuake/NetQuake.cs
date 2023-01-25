using ServersDataAggregation.Common.Model;
using ServersDataAggregation.Query.Games.Common;
using ServersDataAggregation.Query.Games.NetQuake.Packets;
using System.Text;

namespace ServersDataAggregation.Query.Games.NetQuake;

public class NetQuake : IServerInfoProvider
{
    static byte[] UNCONNECTED_NAME = Encoding.ASCII.GetBytes("unconnected");
    public ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort)
    {
        var udp = new UdpUtility(pServerAddress, pServerPort);
        // serverInfo.Players = new PlayerSnapshot[];

        byte[] bytesReceived = udp.SendBytes(new ServerInfoRequest().GetPacket());
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
            ServerVersion = serverInfoReply.GameProtocol.ToString()
        };

        string ruleName = string.Empty;
        var serverRules = new List<ServerSetting>();
        do
        {
            bytesReceived = udp.SendBytes(new RuleInfoRequest(ruleName).GetPacket());
            RuleInfoReply? rulesInfoReply = GetReplyPacket(bytesReceived) as RuleInfoReply;

            if (rulesInfoReply == null)
                throw new Exception("Communications exception");

            if (string.IsNullOrEmpty(rulesInfoReply.RuleName))
                break;

            serverRules.Add(new ServerSetting(rulesInfoReply.RuleName, rulesInfoReply.RuleValue));
            ruleName = rulesInfoReply.RuleName;

        } while (!string.IsNullOrEmpty(ruleName.Trim()));

        int playerCount = serverInfoReply.CurrentPlayers;

        for (int i = 0; i < playerCount; i++)
        {
            bytesReceived = udp.SendBytes(new PlayerInfoRequest(i).GetPacket());
            PlayerInfoReply? playerInfoReply = GetReplyPacket(bytesReceived) as PlayerInfoReply;

            if (playerInfoReply == null)
                throw new Exception("Communications exception");

            if (playerInfoReply.PlayerName == null || playerInfoReply.PlayerName.Length == 0)
                break;

            if (playerInfoReply.PlayerName.SequenceEqual(NetQuake.UNCONNECTED_NAME))
                continue;

            players.Add(CreatePlayerSnapshot(playerInfoReply));
        }
        
        serverSnapshot.Port = udp.RemotePort;
        serverSnapshot.IpAddress = udp.RemoteIpAddress;
        serverSnapshot.Players = players.ToArray();
        serverSnapshot.ServerSettings = serverRules.ToArray();
        var modMode = ModModeHelper.DeriveModMode(serverSnapshot.ServerSettings);
        if (modMode != null)
        {
            serverSnapshot.Mode = modMode.Mode;
            serverSnapshot.Mod = modMode.Mod;
        }

        return serverSnapshot;
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
        var playerSnapshot = new PlayerSnapshot
        {
            NameRaw = pReplyPacket.PlayerName,
            Name = NameUtils.PlayerBytesToString(pReplyPacket.PlayerName),
            FeatureFlags = PlayerSnapshotFeatureFlags.Clothes
        };

        // Frags are -99 (observer)
        if (pReplyPacket.FragCount > 65000)
            playerSnapshot.Frags = -99;
        else
            playerSnapshot.Frags = pReplyPacket.FragCount;
        playerSnapshot.Number = (int)pReplyPacket.PlayerNumber;
        playerSnapshot.PantColor = (int)pReplyPacket.PantColor;
        playerSnapshot.ShirtColor = (int)pReplyPacket.ShirtColor;
        playerSnapshot.PlayTime = TimeSpan.FromSeconds(pReplyPacket.PlayTime);

        return playerSnapshot;
    }

}


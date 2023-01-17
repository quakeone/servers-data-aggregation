namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal class PlayerInfoRequest : RequestPacket
{
    int _playerNum;

    protected override int TotalSize
    {
        get { return base.Size + 1; }
    }

    internal PlayerInfoRequest(int pPlayerNum)
    {
        PacketType = QuakeNetworkPacket.PACKET_CONTROL;
        Command = QuakeNetworkPacket.CCREQ_PLAYER_INFO;
        _playerNum = pPlayerNum;
    }

    internal override byte[] GetPacket()
    {
        byte[] bytes = new byte[TotalSize];
        bytes[base.Size] = (byte)_playerNum;
        InternalGetPacket(bytes);
        return bytes;
    }
}

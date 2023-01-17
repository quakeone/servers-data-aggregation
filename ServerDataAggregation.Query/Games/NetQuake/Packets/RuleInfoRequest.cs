namespace ServersDataAggregation.Query.Games.NetQuake.Packets;
internal class RuleInfoRequest : RequestPacket
{
    private string _rulename;

    internal RuleInfoRequest()
        : this(string.Empty)
    {
    }

    internal RuleInfoRequest(string pRuleName)
    {
        PacketType = QuakeNetworkPacket.PACKET_CONTROL;
        Command = QuakeNetworkPacket.CCREQ_RULE_INFO;
        _rulename = pRuleName;
    }

    internal override byte[] GetPacket()
    {
        byte[] packetByte = new byte[this.Size];
        InternalGetPacket(packetByte);
        return packetByte;
    }

    protected override int TotalSize
    {
        get { return this.Size; }
    }

    protected new int Size
    {
        get
        {
            return base.Size + _rulename.Length + 1; // 1 byte for string null terminator
        }
    }

    protected override void InternalGetPacket(byte[] pBytes)
    {
        Packet.SetStringInBytes(pBytes, _rulename, base.Size);

        base.InternalGetPacket(pBytes);
    }
}

namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal class RuleInfoReply : ReplyPacket
{
    internal string RuleName { get; private set; }
    internal string RuleValue { get; private set; }

    protected override int TotalSize
    {
        get { throw new NotImplementedException(); }
    }
    protected new int Size
    {
        get { throw new NotImplementedException(); }
    }

    public RuleInfoReply()
    {
    }

    internal override void SetPacket(byte[] pBytes)
    {
        int byteOffset = base.Size;

        // This will be larger if contains a valid rule.
        if (pBytes.Length > base.Size)
        {
            RuleName = Packet.GetNullTerminatedString(pBytes, byteOffset);
            byteOffset += RuleName.Length + 1;
            RuleValue = Packet.GetNullTerminatedString(pBytes, byteOffset);
            byteOffset += RuleValue.Length + 1;
        }

        base.InternalSetPacket(pBytes);
    }
}

namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal class PlayerInfoReply : ReplyPacket
{
    internal byte PlayerNumber { get; set; }
    internal byte[] PlayerName { get; set; }
    internal string Address { get; set; }
    internal byte ShirtColor { get; set; }
    internal byte PantColor { get; set; }
    internal int PlayTime { get; set; }
    internal int FragCount { get; set; }

    internal override void SetPacket(byte[] pBytes)
    {
        int byteCounter = this.Size;
        PlayerNumber = pBytes[byteCounter++];

        PlayerName = Packet.GetNullTerminatedBytes(pBytes, byteCounter);
        byteCounter += PlayerName.Length + 1;

        byte colors = pBytes[byteCounter++];
        ShirtColor = (byte)((colors >> 4) & 0x0F);
        PantColor = (byte)((colors) & 0x0F);

        // Next three are unused
        byteCounter += 3;

        FragCount = (int)((pBytes[byteCounter + 1] << 8) | pBytes[byteCounter]);
        byteCounter += 4;

        PlayTime = (int)((pBytes[byteCounter + 1] << 8) | pBytes[byteCounter]);
        byteCounter += 4;

        Address = Packet.GetNullTerminatedString(pBytes, byteCounter);
    }

    protected override int TotalSize
    {
        get { throw new NotImplementedException(); }
    }
}

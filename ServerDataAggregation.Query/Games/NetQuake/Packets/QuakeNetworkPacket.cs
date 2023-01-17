namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal abstract class QuakeNetworkPacket : Packet
{
    protected const ushort PACKET_CONTROL = 0x8000;

    internal const byte CCREP_ACCEPT = 0x81;
    internal const byte CCREP_REJECT = 0x82;
    internal const byte CCREP_SERVER_INFO = 0x83;
    internal const byte CCREP_PLAYER_INFO = 0x84;
    internal const byte CCREP_RULE_INFO = 0x85;

    internal const byte CCREQ_SERVER_INFO = 0x02;
    internal const byte CCREQ_PLAYER_INFO = 0x03;
    internal const byte CCREQ_RULE_INFO = 0x04; 
        

    private const int PACKET_SIZE = 4;

    protected ushort PacketLen;
    protected ushort PacketType;

    protected new int Size
    {
        get { return PACKET_SIZE; }
    }
    protected override void InternalGetPacket(byte[] pBytes)
    {
        if (pBytes.Length < PACKET_SIZE)
            throw new Exception("Invalid byte size");

        int byteCounter = 0;

        byte[] byteType = BitConverter.GetBytes(PacketType);
        byte[] byteLen = BitConverter.GetBytes(this.TotalSize);

        pBytes[byteCounter++] = byteType[1];
        pBytes[byteCounter++] = byteType[0];
        pBytes[byteCounter++] = byteLen[1];
        pBytes[byteCounter++] = byteLen[0];
    }

    protected override void InternalSetPacket(byte[] pBytes)
    {
        if (pBytes.Length < PACKET_SIZE)
            throw new Exception("Invalid byte size");

        int byteCounter = 0;

        PacketType = BitConverter.ToUInt16(pBytes, byteCounter);
        byteCounter += 2;
        PacketLen = BitConverter.ToUInt16(pBytes, byteCounter);
    }
}

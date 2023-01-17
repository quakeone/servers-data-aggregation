namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal abstract class RequestPacket : QuakeNetworkPacket
{
    private const int PACKET_SIZE = 1;

    protected byte Command;

    protected new int Size
    {
        get
        {
            return base.Size + PACKET_SIZE;
        }
    }

    protected override void InternalGetPacket(byte[] pBytes)
    {
        pBytes[base.Size] = Command;
        base.InternalGetPacket(pBytes);
    }

    internal abstract byte[] GetPacket();
}
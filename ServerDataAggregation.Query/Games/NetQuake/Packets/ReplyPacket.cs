
namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal abstract class ReplyPacket : QuakeNetworkPacket
{
    public const int CONTROL_BYTE_OFFSET = 4;
    private const int PACKET_SIZE = 1;

    protected byte Command;

    public static byte GetControlByte(byte[] pBytes)
    {
        if (pBytes.Length <= CONTROL_BYTE_OFFSET)
            throw new Exception("Not a valid Reply");

        return pBytes[CONTROL_BYTE_OFFSET];
    }

    protected new int Size
    {
        get
        {
            return base.Size + PACKET_SIZE;
        }
    }

    protected override void InternalSetPacket(byte[] pBytes)
    {
        Command = pBytes[base.Size];
        base.InternalSetPacket(pBytes);
    }

    internal abstract void SetPacket(byte[] pBytes);
}

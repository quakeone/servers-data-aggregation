using System.Text;

namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal abstract class Packet
{
    /// <summary>
    /// Size of Subpacket
    /// </summary>
    protected int Size { get; private set; }
    /// <summary>
    /// Total size of entire packet
    /// </summary>
    protected abstract int TotalSize { get; }
    /// <summary>
    /// Subpacket calls in order to apply to byte array
    /// </summary>
    /// <param name="pBytes"></param>
    protected abstract void InternalGetPacket(byte[] pBytes);
    /// <summary>
    /// Subpacket calls in order to interpret byte array
    /// </summary>
    /// <param name="pBytes"></param>
    protected abstract void InternalSetPacket(byte[] pBytes);

    protected static string GetNullTerminatedString(byte[] pBytes, int pOffset)
    {
        int thisOffset = pOffset;
        var sb = new StringBuilder();
            
        for (int i = 0; pBytes[thisOffset] != 0x00; i++, thisOffset++)
        {
            sb.Append((char)pBytes[thisOffset]);
        }

        return sb.ToString();
    }

    protected static byte[] GetNullTerminatedBytes(byte[] pBytes, int pOffset)
    {
        int thisOffset = pOffset;
        int length = 0;

        while (thisOffset < pBytes.Length && 
            pBytes[thisOffset] != 0x00)
        {
            length++;
            thisOffset++;
        }

        byte[] bytes = new byte[length];
        thisOffset = pOffset;

        for (int i = 0; pBytes[thisOffset] != 0x00; i++, thisOffset++)
        {
            bytes[i] = pBytes[thisOffset];
        }

        return bytes;
    }

    protected static void SetStringInBytes(byte[] pBytes, string pString, int pOffset)
    {
        Encoding.ASCII.GetBytes(pString, 0, pString.Length, pBytes, pOffset);
    }
}



using System.Text;

namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal class ServerInfoRequest : RequestPacket
{
    private const string SERVER_GAME = "QUAKE";
    private const byte SERVER_GAME_VERSION = 0x03;

    public ServerInfoRequest()
    {
        PacketType = QuakeNetworkPacket.PACKET_CONTROL;
        Command = QuakeNetworkPacket.CCREQ_SERVER_INFO;
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
            return base.Size + SERVER_GAME.Length + 2; // 1 byte for string null terminator, 1 byte for Server Version
        }
    }

    protected override void InternalGetPacket(byte[] pBytes)
    {

        byte[] byteGame = Encoding.ASCII.GetBytes(SERVER_GAME);

        int byteOffset = base.Size;
        for (int i = 0; i < byteGame.Length; i++, byteOffset++)
        {
            pBytes[byteOffset] = byteGame[i];
        }

        // + 1 to skip over the string terminating null character.
        pBytes[byteOffset + 1] = SERVER_GAME_VERSION;

        base.InternalGetPacket(pBytes);
    }
}

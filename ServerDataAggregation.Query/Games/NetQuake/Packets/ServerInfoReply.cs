namespace ServersDataAggregation.Query.Games.NetQuake.Packets;

internal class ServerInfoReply : ReplyPacket
{
    internal string Address;
    internal string HostName;
    internal string MapName;
    internal byte CurrentPlayers;
    internal byte MaxPlayers;
    internal byte GameProtocol;

    protected override int TotalSize
    {
        get { return 0; }
    }
    protected new int Size
    {
        get { throw new NotImplementedException(); }
    }


    protected override void InternalSetPacket(byte[] pBytes)
    {
        base.InternalSetPacket(pBytes);
    }

    internal override void SetPacket(byte[] pBytes)
    {
        int byteOffset = base.Size;

        Address = Packet.GetNullTerminatedString(pBytes, byteOffset);
        byteOffset += Address.Length + 1;
        HostName = Packet.GetNullTerminatedString(pBytes, byteOffset);
        byteOffset += HostName.Length + 1;
        MapName = Packet.GetNullTerminatedString(pBytes, byteOffset);
        byteOffset += MapName.Length + 1;

        CurrentPlayers = pBytes[byteOffset++];
        MaxPlayers = pBytes[byteOffset++];
        GameProtocol = pBytes[byteOffset++];

        base.InternalSetPacket(pBytes);
    }
}

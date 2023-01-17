namespace ServersDataAggregation.Query.Games.Quake2.Packets;

internal class Q2PlayerStatus
{
    internal byte[] PlayerNameBytes { get; set; }
    internal string PlayerName { get; set; }
    internal string Ping { get; set; }
    internal string Frags { get; set; }
}

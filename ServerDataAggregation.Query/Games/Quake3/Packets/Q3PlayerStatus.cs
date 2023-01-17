namespace ServersDataAggregation.Query.Games.Quake3.Packets;

internal class Q3PlayerStatus
{
    internal byte[] PlayerNameBytes { get; set; }
    internal string PlayerName { get; set; }
    internal string Ping { get; set; }
    internal string Frags { get; set; }
}

namespace ServersDataAggregation.Query.Games.QuakeWorld.Packets;

internal class QWPlayerStatus
{
    internal string Address { get; set; }
    internal byte[] PlayerBytes { get; set; }
    internal int PlayerNumber { get; set; }
    internal string SkinName { get; set; }
    internal string? PlayMins { get; set; }
    internal string Ping { get; set; }
    internal string Frags { get; set; }
    internal string ShirtColor { get; set; }
    internal string PantColor { get; set; }
}

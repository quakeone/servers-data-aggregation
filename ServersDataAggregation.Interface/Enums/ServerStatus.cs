namespace ServersDataAggregation.Common
{
    /// <summary>
    /// Enum for describing the Status of a Game Server
    /// </summary>
    public enum ServerStatus
    {
        Running,
        NotResponding,
        NotFound,
        QueryError
    }
}

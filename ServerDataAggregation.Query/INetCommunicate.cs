namespace ServersDataAggregation.Query;

public interface INetCommunicate
{
    /// <summary>
    /// Basic send/receive method
    /// </summary>
    /// <param name="pBytes"></param>
    /// <returns></returns>
    byte[] SendBytes(byte[] pBytes);
    string RemoteIpAddress { get; }
    int RemotePort { get; }
}


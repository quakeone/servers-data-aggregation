using ServersDataAggregation.Common.Model;

namespace ServersDataAggregation.Query;

public interface IServerInfoProvider
{
    ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort);
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ServerDataAggregation.Persistence;
using ServerDataAggregation.Persistence.Models;

namespace ServersDataAggregation.Service.Tasks;

public class SynchronizeServers
{
    private Boolean ServerMatch (Server A, Server B) => B.Address == A.Address && B.Port == A.Port && B.GameId == A.GameId;

    private Server[] Synchronize(Server[] currentServers, Server[] maybeAdditional)
    {
        return maybeAdditional.Where(maybeAdd =>
            currentServers.All(current => !ServerMatch(current, maybeAdd))
        ).ToArray();
    }

    private async Task<Server[]> SynchronizeQSB(Server[] currentServers)
    {
        var qsbService = new Services.QSBApp.Service();
        var servers = await qsbService.GetServers();

        return Synchronize(currentServers, servers);
    }

    public async Task Execute()
    {
        using (var context = new PersistenceContext())
        {
            var currentServers = await context.Servers.ToArrayAsync();
            var newServers = await SynchronizeQSB(currentServers);
            if (newServers.Length > 0)
            {
                context.Servers.AddRange(newServers);
                await context.SaveChangesAsync();
            }
        }
    }
}

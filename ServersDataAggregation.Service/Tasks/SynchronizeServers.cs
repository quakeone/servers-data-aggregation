using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ServerDataAggregation.Persistence;
using ServerDataAggregation.Persistence.Models;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;

namespace ServersDataAggregation.Service.Tasks;

public class SynchronizeServers
{
    private string? GetIP(string address)
    {
        IPAddress[] ips;

        ips = Dns.GetHostAddresses(address);
        var ip = ips.FirstOrDefault(ip =>
        {
            var ipArr = ip.GetAddressBytes();
            return ipArr[0] != 172 && ipArr[0] != 192 && ipArr[0] != 10;
        });

        if (ip == null)
        {
            ip = ips[0];
        }
        return ip != null ? ip.ToString() : null;
    }

    private bool ServerMatch (Server A, Server B) => B.Address == A.Address && B.Port == A.Port;

    private Server[] Synchronize(ServerState[] currentServers, Server[] maybeAdditional)
    {
        // Find servers from this source that haven't been added yet
        var uniquelyAddedFromSource = maybeAdditional.Where(maybeAdd =>
            !currentServers.Any(existing =>
                existing.ServerDefinition.Source == maybeAdd.Source && ServerMatch(existing.ServerDefinition, maybeAdd)));

        // Filter if it's been added by another source (match on resolved IP)
        // We don't want to add a server that already exists with the same IP.
        // ie - manually added via DNS and master server returning same instance but IP.
        return uniquelyAddedFromSource
            .Where(newServer =>
            {
                var newIp = GetIP(newServer.Address);
                return newIp == null || !currentServers.Any(s => s.IpAddress == newIp && s.ServerDefinition.Port == newServer.Port);
            })
            .ToArray();
    }

    private async Task<Server[]> SynchronizeQSB(ServerState[] currentServers)
    {
        var qsbService = new Services.QSBApp.Service();
        var servers = await qsbService.GetServers();

        return Synchronize(currentServers, servers);
    }
    private Server[] SynchronizeDPMaster(ServerState[] currentServers)
    {
        var dpMasterService = new Services.DpMaster.Service();
        var servers = dpMasterService.GetFTEServers();

        return Synchronize(currentServers, servers);
    }

    public async Task Execute()
    {
        using (var context = new PersistenceContext())
        {
            var currentServers = await context.ServerState
                .Include(ss => ss.ServerDefinition)
                .ToArrayAsync();

            var newServers = new List<Server>();
            newServers.AddRange(await SynchronizeQSB(currentServers));
            newServers.AddRange(SynchronizeDPMaster(currentServers));
            var distinctNewServers = newServers.DistinctBy(s => $"{s.Address}:{s.Port}");

            if (newServers.Count > 0)
            {
                context.Servers.AddRange(distinctNewServers);
                await context.SaveChangesAsync();
            }
        }
    }
}

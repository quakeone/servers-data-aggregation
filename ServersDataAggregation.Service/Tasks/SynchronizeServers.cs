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
        IPAddress[] ips =new IPAddress[0];
        try
        {
            ips = Dns.GetHostAddresses(address);
        }
        catch { }
        var ip = ips.FirstOrDefault(ip =>
        {
            var ipArr = ip.GetAddressBytes();
            return ipArr[0] != 172 && ipArr[0] != 192 && ipArr[0] != 10;
        });

        if (ip == null && ips.Count() > 0)
        {
            ip = ips[0];
        }
        return ip != null ? ip.ToString() : null;
    }

    private bool ServerMatch (Server A, Server B) => B.Address == A.Address && B.Port == A.Port;

    private Server[] Synchronize(ServerState[] currentServers, Server[] fromSource)
    {
        if (fromSource.Length == 0)
        {
            return new Server[0];
        }

        // Find servers from this source that haven't been added yet
        var uniquelyAddedFromSource = fromSource.Where(maybeAdd =>
            !currentServers.Any(existing => ServerMatch(existing.ServerDefinition, maybeAdd)));

        var toEnable = currentServers.Where(existing => 
            !existing.ServerDefinition.Active &&
            fromSource.Any(sourceServer => 
                sourceServer.Active && // Only enable if the source is also active
                ServerMatch(existing.ServerDefinition, sourceServer))
        );


        foreach (var server in toEnable)
        {
            server.ServerDefinition.Active = true;
        }

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
    private Server[] SynchronizeFTEMaster(ServerState[] currentServers)
    {
        var dpMasterService = new Services.DpMaster.Service();
        var servers = dpMasterService.GetFTEServers();

        return Synchronize(currentServers, servers);
    }
    private Server[] SynchronizeDPMaster(ServerState[] currentServers)
    {
        var dpMasterService = new Services.DpMaster.Service();
        var servers = dpMasterService.GetDPServers();

        return Synchronize(currentServers, servers);
    }
    private Server[] SynchronizeQWMaster(ServerState[] currentServers)
    {
        var qwMasterService = new Services.QWMaster.Service();
        var servers = qwMasterService.GetQWServers();

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
            //newServers.AddRange(SynchronizeQWMaster(currentServers));
            newServers.AddRange(await SynchronizeQSB(currentServers));
            newServers.AddRange(SynchronizeFTEMaster(currentServers));
            newServers.AddRange(SynchronizeDPMaster(currentServers));
            //newServers.AddRange(await SynchronizeQServersNet(currentServers));

            var distinctNewServers = newServers.DistinctBy(s => $"{s.Address}:{s.Port}");

            if (distinctNewServers.Count() > 0)
            {
                context.Servers.AddRange(distinctNewServers);
            }
            await context.SaveChangesAsync();
        }
    }
}

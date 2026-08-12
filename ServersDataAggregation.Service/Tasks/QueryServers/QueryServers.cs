using Microsoft.EntityFrameworkCore;
using ServerDataAggregation.Persistence;
using ServerDataAggregation.Persistence.Models;
using ServersDataAggregation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ServersDataAggregation.Service.Tasks.QueryServers;

public class QueryServers
{
    // UDP-bound work, not CPU-bound: intentionally far above the core count.
    private const int MAX_CONCURRENT_QUERIES = 300;

    private async Task EnsureAllServerStateExists(PersistenceContext context)
    {
        var query = from server in context.Set<Server>()
            join serverState in context.Set<ServerState>()
                on server.ServerId equals serverState.ServerDefinition.ServerId into grouping
            from serverState in grouping.DefaultIfEmpty()
            where serverState == null
            select server ;

        var noState = await query.ToArrayAsync();
        if (noState.Length > 0) {
            await context.ServerState.AddRangeAsync(noState.Select(server => new ServerState {
                ServerDefinition = server,
                MatchInfo = new MatchInfo() { }
            }));
            try {
                await context.SaveChangesAsync();
            }
            catch(Exception ex) {

            }
        }
    }
    private async Task<ServerState[]> FindQueryableServers()
    {
        using(var context = new PersistenceContext())
        {
            await EnsureAllServerStateExists(context);
            
            var candidates = await context.ServerState
                .Include(s => s.ServerDefinition)
                .Where(serverState =>
                serverState.ServerDefinition.Active &&
                (serverState.LastQuery == null || serverState.LastQuery < DateTime.UtcNow.AddSeconds(-serverState.ServerDefinition.QueryInterval))
            ).ToArrayAsync();

            return candidates.Where(candidate =>
            {
                if (candidate.LastQuery != null && candidate.LastQueryResult > 0)
                {
                    // Server query issue
                    switch ((ServerStatus)candidate.LastQueryResult)
                    {
                        // Connected to the server, no response however
                        case ServerStatus.NotResponding:
                            if (candidate.FailedQueryAttempts > 500)
                                return candidate.LastQuery < DateTime.UtcNow.AddHours(-24);
                            if (candidate.FailedQueryAttempts > 100)
                                return candidate.LastQuery < DateTime.UtcNow.AddHours(-6);
                            if (candidate.FailedQueryAttempts > 20)
                                return candidate.LastQuery < DateTime.UtcNow.AddHours(-1);
                            if (candidate.FailedQueryAttempts > 3)
                                return candidate.LastQuery < DateTime.UtcNow.AddMinutes(-5);
                            break;
                        case ServerStatus.NotFound:
                            // Couldn't connect or find the server
                            if (candidate.FailedQueryAttempts > 500)
                                return candidate.LastQuery < DateTime.UtcNow.AddHours(-24);
                            if (candidate.FailedQueryAttempts > 100)
                                return candidate.LastQuery < DateTime.UtcNow.AddHours(-6);
                            if (candidate.FailedQueryAttempts > 3)
                                return candidate.LastQuery < DateTime.UtcNow.AddHours(-1);
                            break;
                        case ServerStatus.QueryError:
                            // Couldn't parse the reply
                            if (candidate.FailedQueryAttempts > 100)
                                return candidate.LastQuery < DateTime.UtcNow.AddHours(-6);
                            if (candidate.FailedQueryAttempts > 3)
                                return candidate.LastQuery < DateTime.UtcNow.AddMinutes(-5);
                            break;
                    }
                }

                return true;
            }).ToArray();
        }
    }

    /// <summary>
    /// Perform a query and process query result
    /// </summary>
    /// <param name="pServer"></param>
    /// <returns></returns>
    public Task DoQuery(ServerState serverState)
    { 
        return Task.Run(() => new QueryServer(serverState).DoQuery());
    }

    /// <summary>
    /// Perform all necessary queries for servers needing it
    /// Queries are performed asynchronously
    /// </summary>
    public async Task Execute()
    {
        var serversToQuery = await FindQueryableServers();

        using var gate = new SemaphoreSlim(MAX_CONCURRENT_QUERIES);

        var queryTasks = serversToQuery.Select(async serverState =>
        {
            await gate.WaitAsync();
            try
            {
                await new QueryServer(serverState).DoQuery();
            }
            catch (Exception ex)
            {
                Logging.LogError(ex, $"Unhandled error querying {serverState.ServerDefinition}");
            }
            finally
            {
                gate.Release();
            }
        });

        await Task.WhenAll(queryTasks);
    }
}
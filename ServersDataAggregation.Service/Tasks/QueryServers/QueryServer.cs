//using ServerDataAggregation.Persistence;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Npgsql.Replication;
using ServerDataAggregation.Persistence;
using ServerDataAggregation.Persistence.Models;
using ServersDataAggregation.Common;
using ServersDataAggregation.Common.Model;
using ServersDataAggregation.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Tasks.QueryServers;

internal class QueryServer
{
    /// <summary>
    /// If server is not responding, retry this many times
    /// </summary>
    const int SERVER_NOT_RESPONDING_RETRY_COUNT = 3;

    public ServerState _serverState { get; private set; }

    internal QueryServer(ServerState serverState)
    {
        _serverState = serverState;
    }

    private Tuple<Common.Model.ServerSnapshot?, ServerStatus> GetSnapshot()
    {
        int pRetryCount = 0;
        ServerStatus status = ServerStatus.Running;
        ServerDataAggregation.Persistence.Models.Server server = _serverState.ServerDefinition;
        do
        {
            string retryString = "";
            if (pRetryCount > 0)
            {
                Thread.Sleep(1000);
                retryString = " (Retry #" + pRetryCount + ")";
            }
            Logging.LogTrace("Querying " + server.Address + ":" + server.Port.ToString() + retryString);
            try
            {
                return new Tuple<Common.Model.ServerSnapshot?, ServerStatus>(
                    ServerInterface.GetServerInfo(server.Address.Trim(), server.Port, (Game)server.GameId, server.Parameters),
                    status);
            }
            catch (ServerNotRespondingException)
            {
                status = ServerStatus.NotResponding;
            }
            catch (ServerNotFoundException)
            {
                status = ServerStatus.NotFound;
            }
            catch (ServerQueryParseException ex)
            {
                Logging.LogWarning($"{server} query exception " + ex.ToString());
                status = ServerStatus.QueryError;
            }

            // Retry logic:
        } while (pRetryCount++ < SERVER_NOT_RESPONDING_RETRY_COUNT
            && status == ServerStatus.NotResponding);

        return new Tuple<Common.Model.ServerSnapshot?, ServerStatus>(null, status);
    }

    private async Task FailedQuery(ServerStatus status)
    {
        using (var context = new PersistenceContext())
        {
            context.ServerState.Attach(_serverState);

            _serverState.LastQuery = DateTime.UtcNow;
            _serverState.LastQueryResult = (int)status;
            _serverState.FailedQueryAttempts++;

            await context.SaveChangesAsync();
        }
    }

    private async Task SuccessQuery(Common.Model.ServerSnapshot snapshot)
    {

        var stateUpdate = new StateUpdate(_serverState, snapshot);
        await stateUpdate.PerformUpdate();
    }

    public async Task DoQuery ()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        var snapshotResult = GetSnapshot();

        stopWatch.Stop();

        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
            
        if (snapshotResult.Item2 == ServerStatus.Running)
        {
            Debug.WriteLine($"Successful query on {_serverState} in {ts.Seconds}.{ts.Milliseconds} seconds");
            await SuccessQuery(snapshotResult.Item1);
        }
        else
        {
            Debug.WriteLine($"Failed query on {_serverState} in {ts.Seconds}.{ts.Milliseconds} seconds");
            await FailedQuery(snapshotResult.Item2);
        }
    }
}

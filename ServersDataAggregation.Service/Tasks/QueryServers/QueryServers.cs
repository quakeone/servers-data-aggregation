﻿using Microsoft.EntityFrameworkCore;
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
    private async Task<Server[]> FindQueryableServers()
    {
        using(var context = new PersistenceContext())
        {
            var candidates = await context.Servers.Where(server =>
                server.Active &&
                (server.LastQuery == null || server.LastQuery < DateTime.UtcNow.AddSeconds(-server.QueryInterval))
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
                            if (candidate.FailedQueryAttempts > 10)
                                return candidate.LastQuery > DateTime.UtcNow.AddMinutes(-1);
                            break;
                        case ServerStatus.NotFound:
                            // Couldn't connect or find the server
                            if (candidate.FailedQueryAttempts > 10)
                                return candidate.LastQuery > DateTime.UtcNow.AddHours(-10);
                            break;
                        case ServerStatus.QueryError:
                            // Couldn't find the server
                            if (candidate.FailedQueryAttempts > 10)
                                return candidate.LastQuery > DateTime.UtcNow.AddHours(-1);
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
    public Task DoQuery(Server server)
    { 
        return Task.Run(() => new QueryServer(server).DoQuery());
    }

    /// <summary>
    /// Perform all necessary queries for servers needing it
    /// Queries are performed asynchronously
    /// </summary>
    public async Task Execute()
    {
        var serversToQuery = await FindQueryableServers();

        await Task.WhenAll(serversToQuery.AsParallel().Select(server => Task.Run(() => new QueryServer(server).DoQuery())));
    }

    ///// <summary>
    ///// Processes a server Query Response, and handles grunt work of keeping the cache up to date
    ///// </summary>
    ///// <param name="pQuery"></param>
    //public void ProcessQueryResult(ServerQuery pQuery, IDataSession pDataSession)
    //{
    //    ServerActivity activity = ServerCache.Cache[pQuery.DbServer.ServerId];

    //    // Reload server data in case anything changed
    //    var gameServer = pDataSession.GetServer(pQuery.DbServer.ServerId);

    //    gameServer.LastQuery = DateTime.UtcNow;

    //    if (pQuery.ServerSnapshot.Status != ServerStatus.Running)
    //    {
    //        if (activity != null && activity.ServerSnapshot != null)
    //            activity.UpdateStatus(pQuery.ServerSnapshot.Status);

    //        // Use old DB object to report on the DNS actually used (in case it changed)
    //        System.Diagnostics.Debug.WriteLine("Query to " + pQuery.DbServer.DNS + ":" + pQuery.DbServer.Port.ToString() + " failed");

    //        // Server query issue
    //        switch (pQuery.ServerSnapshot.Status)
    //        {
    //            // Connected to the server, no response however
    //            case ServerStatus.NotResponding:
    //                if (gameServer.FailedQueryAttempts > 10)
    //                    gameServer.NextQuery = DateTime.UtcNow.AddMinutes(1);
    //                break;
    //            case ServerStatus.NotFound:
    //                // Couldn't connect or find the server
    //                if (gameServer.FailedQueryAttempts > 10)
    //                    gameServer.NextQuery = DateTime.UtcNow.AddHours(10);
    //                break;
    //            case ServerStatus.QueryError:
    //                // Couldn't find the server
    //                if (gameServer.FailedQueryAttempts > 10)
    //                    gameServer.NextQuery = DateTime.UtcNow.AddHours(10);
    //                break;
    //        }

    //        gameServer.FailedQueryAttempts++;
    //        gameServer.QueryResult = (int)pQuery.ServerSnapshot.Status;
    //        pDataSession.AddUpdateServer(gameServer);
    //        return;
    //    }

    //    // Server is running
    //    ActivityInstanceSynch activitySynchronizer = new ActivityInstanceSynch(pDataSession, pQuery.DbServer);

    //    if (activity == null)
    //    {
    //        activity = activitySynchronizer.CreateNewActivity(pQuery.ServerSnapshot);
    //        activity.UpdateStatus(pQuery.ServerSnapshot.Status);
    //    }
    //    else
    //    {
    //        activity.UpdateStatus(pQuery.ServerSnapshot.Status);
    //        activitySynchronizer.SynchExistingActivity(activity, pQuery.ServerSnapshot);
    //    }

    //    pQuery.DbServer.LastQuerySuccess = DateTime.UtcNow;
    //    pQuery.DbServer.FailedQueryAttempts = 0;
    //    pQuery.DbServer.QueryResult = (int)pQuery.ServerSnapshot.Status;

    //    pQuery.DbServer.NextQuery = null; // This is awkward. I do this to make the query only run off LastQuery + QueryInterval.

    //    pDataSession.AddUpdateServerDataObject(new SnapshotInstance(activity).GetServerDataObject());
    //    pDataSession.AddUpdateServer(pQuery.DbServer);
    //}
    //public async Task Execute()
    //{
    //}

}
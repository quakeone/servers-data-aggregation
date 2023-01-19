//using ServerDataAggregation.Persistence;
using ServerDataAggregation.Persistence;
using ServerDataAggregation.Persistence.Models;
using ServersDataAggregation.Common;
using ServersDataAggregation.Query;
using System;
using System.Collections.Generic;
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

    public Server _server { get; private set; }

    internal QueryServer(Server server)
    {
        _server = server;
    }

    private Tuple<Common.Model.ServerSnapshot?, ServerStatus> GetSnapshot()
    {
        int pRetryCount = 0;
        ServerStatus status = ServerStatus.Running;
        do
        {
            string retryString = "";
            if (pRetryCount > 0)
            {
                Thread.Sleep(1000);
                retryString = " (Retry #" + pRetryCount + ")";
            }
            System.Diagnostics.Debug.WriteLine("Querying " + _server.Address + ":" + _server.Port.ToString() + retryString);
            try
            {
                return new Tuple<Common.Model.ServerSnapshot?, ServerStatus>(
                    ServerInterface.GetServerInfo(_server.Address.Trim(), _server.Port, (Game)_server.GameId, _server.Parameters),
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
            catch (ServerQueryParseException)
            {
                status = ServerStatus.QueryError;
            }

            // Retry logic:
        } while (pRetryCount++ < SERVER_NOT_RESPONDING_RETRY_COUNT
            && status == ServerStatus.NotResponding);

        return new Tuple<Common.Model.ServerSnapshot?, ServerStatus>(null, status);
    }

    public void DoQuery ()
    {
        var now = DateTime.UtcNow;
        var snapshotResult = GetSnapshot();
        using (var context = new PersistenceContext())
        {
            context.Servers.Attach(_server);
            _server.LastQuery = now;
            _server.LastQueryResult = (int)snapshotResult.Item2;

            var snapshot = snapshotResult.Item1;

            if (snapshot == null)
            {
                _server.FailedQueryAttempts++;
            }
            else 
            {
                _server.LastQuerySuccess = now;
                context.ServerSnapshots.Add(new ServerSnapshot
                {
                    ServerId = _server.ServerId,
                    Hostname = snapshot.ServerName,
                    Map = snapshot.Map,
                    IpAddress = snapshot.IpAddress,
                    ServerSettings = JsonSerializer.Serialize(snapshot.ServerSettings),
                    TimeStamp = DateTime.UtcNow,
                    MaxPlayers = snapshot.MaxPlayerCount,
                    Mod = snapshot.Mod,
                    Mode = snapshot.Mode,
                    Players = snapshot.Players.Select(player => new PlayerSnapshot
                    {
                        Frags = player.Frags,
                        ShirtColor = player.ShirtColor,
                        PantColor = player.PantColor,
                        Skin = player.SkinName,
                        Model = player.ModelName,
                        Name = player.PlayerName,
                        Ping = player.Ping,
                        PlayTime = player.PlayTime
                    }).ToList()
                });
            }
            context.SaveChanges();
        }
    }
}

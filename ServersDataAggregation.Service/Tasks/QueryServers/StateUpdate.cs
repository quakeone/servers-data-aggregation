using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerDataAggregation.Persistence;
using ServerDataAggregation.Persistence.Models;
using ServersDataAggregation.Common;
using ServersDataAggregation.Service.Services.IpApi;
using System.Text;
using System.Text.Json;
using Db = ServerDataAggregation.Persistence.Models;

namespace ServersDataAggregation.Service.Tasks.QueryServers
{
    internal class StateUpdate
    {
        private ServerState _serverState;
        private Common.Model.ServerSnapshot _snapshot;

        public StateUpdate(ServerState serverState, Common.Model.ServerSnapshot snapshot)
        {
            _serverState = serverState;
            _snapshot = snapshot;
        }

        private Db.ServerSnapshot CreateServerSnapshot(Common.Model.ServerSnapshot serverSnapshot)
        {
            return new Db.ServerSnapshot
            {
                ServerId = _serverState.ServerDefinition.ServerId,
                Hostname = _snapshot.ServerName,
                Map = _snapshot.Map,
                IpAddress = _snapshot.IpAddress,
                ServerSettings = JsonSerializer.Serialize(_snapshot.ServerSettings),
                TimeStamp = DateTime.UtcNow,
                MaxPlayers = _snapshot.MaxPlayerCount,
                Mod = _snapshot.Mod,
                Mode = _snapshot.Mode,
                Players = _snapshot.Players.Select(player => new ServerDataAggregation.Persistence.Models.PlayerSnapshot
                {
                    Frags = player.Frags,
                    ShirtColor = player.ShirtColor,
                    PantColor = player.PantColor,
                    Skin = player.SkinName,
                    Model = player.ModelName,
                    Name = player.Name,
                    NameRaw = Convert.ToBase64String(player.NameRaw),
                    Number = player.Number,
                    Ping = player.Ping,
                    PlayTime = player.PlayTime
                }).ToList()
            };
        }
        private int CalcTotalFrags(
            Common.Model.PlayerSnapshot currentPlayerSnapshot,
            Db.PlayerSnapshot? prevPlayerSnapshot,
            PlayerState? currentPlayerState) {
            // Assume observer
            if (currentPlayerSnapshot.Frags == -99)
            {
                return currentPlayerState?.TotalFrags ?? 0;
            }
            if (prevPlayerSnapshot == null || currentPlayerState == null)
            {
                return currentPlayerSnapshot.Frags;
            }
            if (currentPlayerSnapshot.Frags > prevPlayerSnapshot.Frags)
            {
                return currentPlayerState.TotalFrags + (currentPlayerSnapshot.Frags - prevPlayerSnapshot.Frags);
            }
            return currentPlayerState.TotalFrags;
        }

        private async Task<ServerSnapshot?> GetPreviousSnapshot(PersistenceContext context)
        {
            var timeAllowance = DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0));
            return await context.ServerSnapshots
                                .Where(s =>
                                    s.ServerId == _serverState.ServerDefinition.ServerId
                                    && s.TimeStamp > timeAllowance
                                )
                                .OrderByDescending(t => t.TimeStamp)
                                .FirstOrDefaultAsync();
        }

        public async Task UpdateServerState(Db.ServerSnapshot? prevSnapshot, PersistenceContext context)
        {
            var snapshot = CreateServerSnapshot(_snapshot);
            context.ServerSnapshots.Add(snapshot);

            if (prevSnapshot == null)
            {
                prevSnapshot = snapshot;
            }

            if (_serverState.IpAddress != _snapshot.IpAddress)
            {
                _serverState.IpAddress = _snapshot.IpAddress;
                var ipResult = await new Services.IpApi.Service().GetResult(_snapshot.IpAddress);
                if (ipResult.status == "success")
                {
                    _serverState.ServerDefinition.Country = ipResult.countryCode;
                    _serverState.ServerDefinition.Locality = ipResult.regionName;
                }
            }
            _serverState.TimeStamp = DateTime.UtcNow;
            _serverState.Map = _snapshot.Map;
            _serverState.Mod = _snapshot.Mod;
            _serverState.Mode = _snapshot.Mode;
            _serverState.Hostname = _snapshot.ServerName;
            _serverState.MaxPlayers = _snapshot.MaxPlayerCount;
            _serverState.Fraglimit = _snapshot.Fraglimit;
            _serverState.Timelimit = _snapshot.Timelimit;
            _serverState.MatchStatus = (int)_snapshot.MatchStatus;
            _serverState.ServerSettings = JsonSerializer.Serialize(_snapshot.ServerSettings);
            _serverState.Players = _snapshot.Players.Select(player =>
            {
                var nameRaw = Convert.ToBase64String(player.NameRaw);
                var prevPlayerState = _serverState.Players?.FirstOrDefault(p =>  p.NameRaw == nameRaw);
                var prevPlayerSnap = prevSnapshot.Players.FirstOrDefault(p => p.NameRaw == nameRaw);

                var joinTime = prevPlayerState == null || prevPlayerState.JoinTime == null
                        ? (DateTime.UtcNow - player.PlayTime)
                        : prevPlayerState.JoinTime;

                return new PlayerState
                {
                    Frags = player.Frags,
                    ShirtColor = player.ShirtColor,
                    PantColor = player.PantColor,
                    TotalFrags = CalcTotalFrags(player, prevPlayerSnap, prevPlayerState),
                    Skin = player.SkinName,
                    Model = player.ModelName,
                    Name = player.Name,
                    NameRaw = nameRaw,
                    Number = player.Number,
                    Ping = player.Ping,
                    JoinTime = joinTime,
                    Type = (int)player.PlayerType
                };
            }).ToList();
        }

        public async Task PerformUpdate()
        {
            using(var context = new PersistenceContext())
            {
                context.Attach(_serverState);

                _serverState.LastQuery = DateTime.UtcNow;
                _serverState.LastQueryResult = (int)ServerStatus.Running;
                _serverState.FailedQueryAttempts = 0;

                var prevSnapshot = await GetPreviousSnapshot(context);

                await UpdateServerState(prevSnapshot, context);

                await new MatchState(_serverState, prevSnapshot)
                    .ProcessMatch(context);

                await context.SaveChangesAsync();
            }
        }
    }
}

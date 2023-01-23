using Microsoft.Extensions.Logging;
using ServerDataAggregation.Persistence.Models;
using ServerDataAggregation.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServersDataAggregation.Service.Tasks.QueryServers
{
    internal class MatchState
    {

        // Definitions:
        // Idle: Player  On server for very long time w/o score
        // Active Player: Non Observer, Non Host (or idle for a very long time?)

        // match start conditions:
        // No Match
        // Active(Snapshot.players) >= 2

        // match end conditions:
        // Match exists
        // Match.mod != snapshot.mod
        // Match.mode != snapshot.mode
        // Match.currentMap != snapshot.currentMap
        // Active(Snapshot.players) < 2

        const int PLAYER_IDLE_ALLOWANCE_SECONDS = 1800; // 30 mins

        private ServerState _serverState;
        private ILogger<StateUpdate> _logger;

        public MatchState(ServerState serverState)
        {
            _logger = LoggerFactory.Create(options => { }).CreateLogger<StateUpdate>();
            _serverState = serverState;
        }

        private void ServerDebug(string message)
        {
            _logger.LogDebug($"{_serverState.ServerDefinition}: ${message}");
        }

        private T SnapshotPlayerValue<T>(
            Func<PlayerSnapshot, T> getter, IEnumerable<PlayerSnapshot> playerSnapshots)
        {
            var tryIt = playerSnapshots
                    .Select(getter)
                    .GroupBy(s => s)
                    .MaxBy(g => g.Count())
                    .FirstOrDefault();
            return tryIt;
            //return playerSnapshots
            //    .Aggregate(new Dictionary<T, int>() { { default, 0 } }, (dict, snapshot) =>
            //    {
            //        T val = getter(snapshot);
            //        if (val == null) { return dict; }
            //        if (!dict.ContainsKey(val))
            //        {
            //            dict.Add(val, 1);
            //        }
            //        else
            //        {
            //            dict[val]++;
            //        }
            //        return dict;
            //    })
            //    .Aggregate((keyValMax, keyVal) => keyValMax.Value < keyVal.Value ? keyVal : keyValMax)
            //.Key;
        }

        /// <summary>
        ///  Dictionary of Player.Number PlayerState/PlayerMatch for comparison
        /// </summary>
        /// <param name="playerMatches"></param>
        /// <param name="playerStates"></param>
        /// <returns></returns>
        private MatchPlayerState[] GetMatchPlayerState(
            IEnumerable<PlayerMatch> playerMatches, IEnumerable<PlayerState> playerStates)
        {
            var list = new List<MatchPlayerState>();
            for (int i = 0; i < _serverState.MaxPlayers; i++)
            {
                var match = playerMatches.FirstOrDefault(m => m.Number == i);
                var state = playerStates.FirstOrDefault(m => m.Number == i);

                if (match != null || state != null)
                {
                    list.Add(new MatchPlayerState { Number = i, match = match, state = state });
                }
            }
            return list.ToArray();
        }

        private bool IsActivePlayer(PlayerState player)
        {
            if (player.Frags == -99)
            {
                return false;
            }
            var playerUptime = (DateTime.UtcNow - player.JoinTime).TotalSeconds;
            if (playerUptime < PLAYER_IDLE_ALLOWANCE_SECONDS)
            {
                return true;
            }
            var fragRatio = player.TotalFrags / playerUptime;
            return fragRatio < (3 / 3600); // 3 frags an hour to be labeled as "active" seems reasonable enough.
        }

        private bool IsStartMatch()
        {
            var activePlayers = _serverState.Players
                .Where(IsActivePlayer)
                .ToArray();

            if (activePlayers.Length > 1)
            {
                ServerDebug($"Match Start Detected - {activePlayers} activePlayers");
                return true;
            }
            return false;
        }

        private bool IsEndMatch(ServerMatch currentMatch, IEnumerable<MatchPlayerState> playerMatches)
        {
            var activePlayers = _serverState.Players
                .Where(IsActivePlayer)
                .ToArray();

            var fragResetCount = playerMatches.Count(p => p.IsFragReset);

            if (fragResetCount > 1)
            {
                ServerDebug($"Ending Match - Frag Reset count is more than 1:  {fragResetCount}");
                return true;
            }
            if (activePlayers.Length < 2)
            {
                ServerDebug($"Ending Match - {activePlayers} activePlayers");
                return true;
            }
            if (currentMatch.Map != _serverState.Map)
            {
                ServerDebug($"Ending Match - Map {currentMatch.Map} changed to {_serverState.Map}");
                return true;
            }
            if (currentMatch.Mod != _serverState.Mod)
            {
                ServerDebug($"Ending Match - Mod {currentMatch.Mod} changed to {_serverState.Mod}");
                return true;
            }
            if (currentMatch.Mode != _serverState.Mode)
            {
                ServerDebug($"Ending Match - Mode {currentMatch.Mode} changed to {_serverState.Mode}");
                return true;
            }
            return false;
        }

        private PlayerMatch CreateNewPlayerMatch(PlayerState player) => new PlayerMatch
        {
            Frags = player.Frags,
            PantColor = player.PantColor,
            ShirtColor = player.ShirtColor,
            Skin = player.Skin,
            Model = player.Model,
            Name = player.Name,
            Number = player.Number,
            PlayerMatchStart = DateTime.UtcNow
        };

        private ServerMatch CreateServerMatch(ServerState serverState, IEnumerable<PlayerState> playerState) => new ServerMatch
        {
            Server = serverState.ServerDefinition,
            MatchStart = DateTime.UtcNow,
            Map = serverState.Map,
            Mod = serverState.Mod,
            Mode = serverState.Mode,
            PlayerMatches = playerState.Select(CreateNewPlayerMatch).ToList()
        };

        internal async Task ProcessMatch(PersistenceContext context)
        {
            Debug.Assert(_serverState.Players != null, $"{_serverState.ServerDefinition}: Players array should always exist here.");

            var match = await context
                .ServerMatches
                .Include(sm => sm.PlayerMatches)
                .FirstOrDefaultAsync(sm => sm.Server.ServerId == _serverState.ServerDefinition.ServerId);
            if (match != null)
            {
                var matchStateDiffs = GetMatchPlayerState(match.PlayerMatches, _serverState.Players);

                if (IsEndMatch(match, matchStateDiffs))
                {
                    var matchHistory = await context.ServerSnapshots
                        .Where(ss => ss.TimeStamp > match.MatchStart && ss.ServerId == match.Server.ServerId)
                        .ToListAsync();

                    // get past snapshots for data aggr
                    foreach (var playerMatch in match.PlayerMatches)
                    {
                        var playerHistory = matchHistory.SelectMany(s => s.Players.Where(p => p.Number == playerMatch.Number));
                        playerMatch.ShirtColor = SnapshotPlayerValue(s => s.ShirtColor, playerHistory);
                        playerMatch.PantColor = SnapshotPlayerValue(s => s.ShirtColor, playerHistory);
                        playerMatch.Skin = SnapshotPlayerValue(s => s.Skin, playerHistory);
                        playerMatch.Model = SnapshotPlayerValue(s => s.Model, playerHistory);
                        playerMatch.PlayerMatchEnd = DateTime.UtcNow;
                    }
                    match.MatchEnd = DateTime.UtcNow;

                    match = null;
                }
                else
                {
                    // continuing match, perform update.

                }
            }


            if (match == null && IsStartMatch())
            {
                await context.ServerMatches.AddAsync(CreateServerMatch(_serverState, _serverState.Players));
            }
        }
    }
}

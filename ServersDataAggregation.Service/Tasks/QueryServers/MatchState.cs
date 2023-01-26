using Microsoft.Extensions.Logging;
using ServerDataAggregation.Persistence.Models;
using ServerDataAggregation.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ServersDataAggregation.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Db = ServerDataAggregation.Persistence.Models;

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

        const int MATCH_LENGTH_ALLOWANCE_SECONDS = 60; // 1 min
        const int PLAYER_IDLE_ALLOWANCE_SECONDS = 1800; // 30 mins

        private ServerState _serverState;

        public MatchState(ServerState serverState)
        {
            _serverState = serverState;
        }

        private void ServerDebug(string message)
        {
            Logging.LogDebug($"{_serverState.ServerDefinition}: {message}");
        }

        private T SnapshotPlayerValue<T>(
            Func<Db.PlayerSnapshot, T> getter, IEnumerable<Db.PlayerSnapshot> playerSnapshots)
        {
            return playerSnapshots
                .Select(getter)
                .GroupBy(s => s)
                .MaxBy(g => g.Count())
                .FirstOrDefault();
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
            // Player matchup logic.
            // Iterate in order
            var list = new List<MatchPlayerState>();
            var statesWip = playerStates.ToList();
            foreach(var playerMatch in playerMatches.Where(pm => pm.PlayerMatchEnd == null))
            {
                var stateCheck = statesWip.FirstOrDefault(s => s.NameRaw == playerMatch.NameRaw);
                list.Add(new MatchPlayerState { match = playerMatch, state = stateCheck });
                if (stateCheck != null)
                {
                    statesWip.Remove(stateCheck);
                }
            }
            list.AddRange(statesWip.Select(playerState => new MatchPlayerState { state = playerState }));
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

            var playersWithFrags = activePlayers.Count(ap => ap.Frags > 0);

            if (activePlayers.Length > 1 && playersWithFrags > 0)
            {
                ServerDebug($"Match Start Detected - {activePlayers.Length} activePlayers and {playersWithFrags} with non-zero frags");
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
                ServerDebug($"Ending Match - {activePlayers.Length} activePlayers");
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

        private bool IsDiscardable(ServerMatch currentMatch)
        {
            var length = DateTime.UtcNow - currentMatch.MatchStart;
            var totalFrags = currentMatch.PlayerMatches.Aggregate(0, (count, pm) => pm.Frags == -99 ? 0 : count + pm.Frags);

            if (length.TotalSeconds < MATCH_LENGTH_ALLOWANCE_SECONDS)
            {
                ServerDebug($"Discarding Match - Length was {Math.Floor(length.TotalSeconds)}secs (threshold is {MATCH_LENGTH_ALLOWANCE_SECONDS}secs)");
                return true;
            }

            if (totalFrags < 1)
            {
                ServerDebug($"Discarding Match - Total Frags was {totalFrags} (threshold is {1})");
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
            PlayerType = player.PlayerType,
            Name = player.Name,
            NameRaw = player.NameRaw,
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

        private PlayerMatchProgress CreatePlayerMatchPorgress (ServerMatch match, PlayerMatch playerMatch) => new PlayerMatchProgress 
        {
            PlayerMatch = playerMatch,
            ServerMatch = match,
            Timestamp = DateTime.UtcNow,
            Frags = playerMatch.Frags,
            PantColor = playerMatch.PantColor,
            ShirtColor = playerMatch.ShirtColor,
            Skin = playerMatch.Skin,
            Model = playerMatch.Model
        };

        // synchronize match players
        private void UpdateMatch(ServerMatch match, MatchPlayerState[] matchDiffs) {
            // update existing, remove not found
            foreach (var diff in matchDiffs)
            {
                if (diff.match == null)
                {
                    match.PlayerMatches.Add(CreateNewPlayerMatch(diff.state));
                }
                else if (diff.state == null)
                {
                    diff.match.PlayerMatchEnd = DateTime.UtcNow;
                } 
                else
                {
                    diff.match.Frags = diff.state.Frags;
                    diff.match.ShirtColor = diff.state.ShirtColor;
                    diff.match.PantColor = diff.state.PantColor;
                    diff.match.Skin = diff.state.Skin;
                    diff.match.Model = diff.state.Model;
                    // If leaves and comes back, reset the end state.
                    diff.match.PlayerMatchEnd = null;
                }
            }
        }

        internal async Task ProcessMatch(PersistenceContext context)
        {
            Debug.Assert(_serverState.Players != null, $"{_serverState.ServerDefinition}: Players array should always exist here.");

            var match = await context
                .ServerMatches
                .Include(sm => sm.PlayerMatches)
                .FirstOrDefaultAsync(sm => 
                    sm.Server.ServerId == _serverState.ServerDefinition.ServerId
                    && sm.MatchEnd == null);

            if (match != null)
            {
                var matchStateDiffs = GetMatchPlayerState(match.PlayerMatches, _serverState.Players);

                // update

                if (!IsEndMatch(match, matchStateDiffs) ) {
                    UpdateMatch(match, matchStateDiffs);

                    if (match.PlayerMatches.Count > 0) {
                        await context.PlayerMatchProgresses.AddRangeAsync(
                            match.PlayerMatches.Select(playerMatch => 
                                CreatePlayerMatchPorgress(match, playerMatch)));
                    }
                } 
                else
                {
                    // discard match if junk
                    if (IsDiscardable(match))
                    {
                        context.ServerMatches.Remove(match);
                    } 
                    else
                    {
                        var matchHistory = await context.ServerSnapshots
                            .Where(ss => ss.TimeStamp > match.MatchStart && ss.ServerId == match.Server.ServerId)
                            .ToListAsync();
                        
                        var playerMatches = match.PlayerMatches
                            .GroupBy(pm => pm.NameRaw)
                            .Where(g => g.Count() == 1)
                            .Select(g => g.FirstOrDefault());
                        
                        // get past snapshots for data aggr
                        foreach (var playerMatch in playerMatches)
                        {
                            // If there's dupe entries, don't bother.
                            var playerHistory = matchHistory.SelectMany(s => s.Players.Where(p => p.NameRaw == playerMatch.NameRaw));
                            if (playerHistory.Count() > 0)
                            { 
                                playerMatch.ShirtColor = SnapshotPlayerValue(s => s.ShirtColor, playerHistory);
                                playerMatch.PantColor = SnapshotPlayerValue(s => s.ShirtColor, playerHistory);
                                playerMatch.Skin = SnapshotPlayerValue(s => s.Skin, playerHistory);
                                playerMatch.Model = SnapshotPlayerValue(s => s.Model, playerHistory);
                            }
                            if (playerMatch.PlayerMatchEnd == null)
                            {
                                playerMatch.PlayerMatchEnd = DateTime.UtcNow;
                            }
                        }
                        match.MatchEnd = DateTime.UtcNow;
                    }

                    match = null;
                }
            }


            if (match == null && IsStartMatch())
            {
                var serverMatch = CreateServerMatch(_serverState, _serverState.Players);
                await context.ServerMatches.AddAsync(serverMatch);

                if (serverMatch.PlayerMatches.Count > 0) {
                    await context.PlayerMatchProgresses.AddRangeAsync(
                        serverMatch.PlayerMatches.Select(playerMatch => 
                            CreatePlayerMatchPorgress(serverMatch, playerMatch)));
                }
            }
        }
    }
}

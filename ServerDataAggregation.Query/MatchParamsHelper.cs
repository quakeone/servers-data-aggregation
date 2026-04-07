using ServersDataAggregation.Common.Enums;
using ServersDataAggregation.Common.Model;
using FlagStatus = ServersDataAggregation.Common.Model.FlagStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query
{
    public static class MatchParamsHelper
    {

        public static ServerSnapshot DeriveParams(ServerSnapshot snapshot)
        {
            int number;
            var settings = snapshot.ServerSettings;
            var matchInfo = new MatchInfo();

            var testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "timelimit");
            if (testRule != null)
            {
                var value = testRule.Value.ToLower();
                var split = value.Split(new string[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                snapshot.Timelimit = int.TryParse(split[0], out number) ? number : 0;

                if (ModModeHelper.SupportsTimedMatch(snapshot.Mod, snapshot.Mode))
                {
                    if (split[0].StartsWith("final score"))
                    {
                        snapshot.MatchStatus = MatchStatus.WaitingForTeam;
                    }
                    else if (split[0].StartsWith("sudden death"))
                    {
                        snapshot.MatchStatus = MatchStatus.MatchInProgress;
                        matchInfo.IsSuddenDeath = true;
                    }
                    else
                    {
                        if (split.Length > 1)
                        {
                            var secondHalf = split[1].ToLower();
                            if (secondHalf.Contains("waiting for")) {
                                snapshot.MatchStatus = MatchStatus.WaitingForTeam;
                            }
                            else if (secondHalf.Contains("match starting"))
                            {
                                snapshot.MatchStatus = MatchStatus.MatchStarting;
                            }
                            else
                            {
                                snapshot.MatchStatus = MatchStatus.MatchInProgress;
                            }
                        }
                        else
                        {
                            snapshot.MatchStatus = MatchStatus.MatchInProgress;
                            matchInfo.MatchTimeRemainingMin = snapshot.Timelimit;
                        }
                    }
                }
            }

            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "fraglimit");
            if (testRule != null)
            {
                var value = testRule.Value.ToLower();
                var split = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                snapshot.Fraglimit = int.TryParse(split[0], out number) ? number : 0;
            }

            // KTX uses "status" serverinfo for match state
            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "status");
            if (testRule != null)
            {
                if (ModModeHelper.SupportsTimedMatch(snapshot.Mod, snapshot.Mode))
                {
                    var statusValue = testRule.Value.ToLower();
                    if (statusValue.Contains("min left"))
                    {
                        snapshot.MatchStatus = MatchStatus.MatchInProgress;
                        var minPart = statusValue.Split(' ')[0];
                        if (int.TryParse(minPart, out var minLeft))
                        {
                            matchInfo.MatchTimeRemainingMin = minLeft;
                        }
                    }
                    else if (statusValue == "countdown" || statusValue == "forcestart")
                    {
                        snapshot.MatchStatus = MatchStatus.MatchStarting;
                    }
                    else
                    {
                        snapshot.MatchStatus = MatchStatus.WaitingForTeam;
                    }
                }
            }

            // matchtime (total match length)
            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "matchtime");
            if (testRule != null && int.TryParse(testRule.Value, out number))
            {
                matchInfo.MatchLengthMin = number;
            }

            // round info (CA modes)
            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "round");
            if (testRule != null && int.TryParse(testRule.Value, out number))
            {
                matchInfo.Round = number;
            }
            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "roundtotal");
            if (testRule != null && int.TryParse(testRule.Value, out number))
            {
                matchInfo.RoundTotal = number;
            }

            // CTF flag statuses
            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "red flag");
            if (testRule != null)
            {
                matchInfo.RedFlagStatus = ParseFlagStatus(testRule.Value);
            }
            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "blue flag");
            if (testRule != null)
            {
                matchInfo.BlueFlagStatus = ParseFlagStatus(testRule.Value);
            }

            // team scores (teamscore1:<color>, teamscore2:<color>)
            foreach (var setting in settings)
            {
                var key = setting.Setting.ToLower();
                if (key.StartsWith("teamscore1:"))
                {
                    var color = setting.Setting.Substring("teamscore1:".Length).Trim();
                    if (int.TryParse(setting.Value, out number))
                    {
                        matchInfo.Team1 = new TeamScore { Color = color, Score = number };
                    }
                }
                else if (key.StartsWith("teamscore2:"))
                {
                    var color = setting.Setting.Substring("teamscore2:".Length).Trim();
                    if (int.TryParse(setting.Value, out number))
                    {
                        matchInfo.Team2 = new TeamScore { Color = color, Score = number };
                    }
                }
            }

            matchInfo.Status = snapshot.MatchStatus;
            snapshot.MatchInfo = matchInfo;

            return snapshot;
        }

        private static FlagStatus? ParseFlagStatus(string value)
        {
            return value.ToLower() switch
            {
                "at base" => FlagStatus.AtBase,
                "dropped" => FlagStatus.Dropped,
                "carried" => FlagStatus.Carried,
                _ => null
            };
        }
    }
}

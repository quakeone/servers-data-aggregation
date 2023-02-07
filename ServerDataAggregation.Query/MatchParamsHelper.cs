using ServersDataAggregation.Common.Model;
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
            var testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "timelimit");
            if (testRule != null)
            {
                var value = testRule.Value.ToLower();
                var split = value.Split(new string[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                snapshot.Timelimit = int.TryParse(split[0], out number) ? number : 0;

                if (ModModeHelper.SupportsTimedMatch(snapshot.Mode))
                {
                    if (split[0].StartsWith("final score"))
                    {
                        snapshot.MatchStatus = Common.Enums.MatchStatus.WaitingForTeam;
                    } 
                    else
                    {
                        if (split.Length > 1)
                        {
                            var secondHalf = split[1].ToLower();
                            if (secondHalf.Contains("waiting for teams")) {
                                snapshot.MatchStatus = Common.Enums.MatchStatus.WaitingForTeam;
                            } 
                            else if (secondHalf.Contains("match starting"))
                            {
                                snapshot.MatchStatus = Common.Enums.MatchStatus.MatchStarting;
                            }
                            else
                            {
                                snapshot.MatchStatus = Common.Enums.MatchStatus.MatchInProgress;
                            }
                        }
                        else
                        {
                            snapshot.MatchStatus = Common.Enums.MatchStatus.MatchInProgress;
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
            testRule = settings.FirstOrDefault(setting => setting.Setting.ToLower() == "status");
            if (testRule != null)
            {
                if (ModModeHelper.SupportsTimedMatch(snapshot.Mode))
                {
                    
                    if (testRule.Value.Contains("min left"))
                    {
                        snapshot.MatchStatus = Common.Enums.MatchStatus.MatchInProgress;
                    } 
                    else
                    {
                        snapshot.MatchStatus = Common.Enums.MatchStatus.WaitingForTeam;
                    }
                }
            }

            return snapshot;
        }
    }
}

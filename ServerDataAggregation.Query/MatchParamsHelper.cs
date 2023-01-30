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

                    if (split.Length > 1)
                    {
                        snapshot.MatchStatus = split[1].ToLower() == "waiting for teams"
                            ? Common.Enums.MatchStatus.WaitingForTeam
                            : Common.Enums.MatchStatus.MatchInProgress;
                    } 
                    else
                    {
                        snapshot.MatchStatus = Common.Enums.MatchStatus.MatchInProgress;
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

            return snapshot;
        }
    }
}

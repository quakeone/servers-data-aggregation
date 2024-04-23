using ServerDataAggregation.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Tasks.QueryServers
{
    public class MatchPlayerState
    {
        public PlayerMatch? match { get; set; }
        public PlayerState? state { get; set; }

        // This is far from perfect
        // if frags has dropped more than 2
        // and match.frags is non-zero
        // and state.frags is close to zero
        public bool IsFragReset
        {
            get
            {
                if (match != null && state != null)
                {
                    if (Math.Abs(match.Frags) > 0 && Math.Abs(state.Frags) < 2
                        && Math.Abs(state.Frags) < (Math.Abs(match.Frags) - 1))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsNew
        {
            get { return match == null && state != null; }
        }

        public bool HasLeft
        {
            get { return match != null && state == null; }
        }
    }
}

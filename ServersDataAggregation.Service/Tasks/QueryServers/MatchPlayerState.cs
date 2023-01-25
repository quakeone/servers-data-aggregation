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
        // and match.frags > 0
        // and state.frags < 3
        public bool IsFragReset
        {
            get
            {
                if (match != null && state != null)
                {
                    if (match.Frags > 0 && state.Frags < 3
                        && state.Frags < (match.Frags - 1))
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

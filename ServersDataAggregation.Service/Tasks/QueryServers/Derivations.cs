using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Tasks.QueryServers
{
    internal class Derivations
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
    }
}

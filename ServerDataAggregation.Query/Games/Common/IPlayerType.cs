using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSB.GameServerInterface.Games.Common
{
    public enum PlayerType 
    { 
        Normal,
        Host,
        Bot
    }
    public interface IPlayerType
    {
        PlayerType PlayerType { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    class Game
    {
        public List<Player> PlayerList { get; set; }
        public int ConnectingPlayers { get; set; }
        public int TargetNumber { get; set; }
        public List<Move> MoveList { get; set; }
        public GameState GameState { get; set; }
    }
}

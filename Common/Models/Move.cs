using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Common.Models
{



    class Move
    {
        public int Id { get; set; }
        public Player TargetPlayer { get; set; }
        public DateTime MoveDateTime { get; set; }

        public int CoordenadasX{ get; set; }
        public int CoordenadasY{ get; set; }
    }
}

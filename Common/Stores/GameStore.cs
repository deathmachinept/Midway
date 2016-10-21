using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Stores
{
    class GameStore
    {
        private static GameStore _instance;
        public Game Game { get; set; }

        private GameStore()
        {
            Game = new Game();
        }

        public static GameStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameStore();
                }
                return _instance;
            }
        }
    }
}

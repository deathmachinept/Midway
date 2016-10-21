using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Stores
{
    class PlayerStore
    {
        private static PlayerStore _instance;
        public Player Player { get; set; }

        private PlayerStore()
        {
            Player = new Player();
        }

        public static PlayerStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerStore();
                }
                return _instance;
            }
        }

    }
}

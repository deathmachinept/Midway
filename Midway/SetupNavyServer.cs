using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midway
{
    class SetupNavyServer
    {



        private int points;
        private bool noPoints;
        private int shipType;
        private bool realnumber;
        private int[,] ships = new int[4, 2];

        List<Ships> gameServerShipList = new List<Ships>();



        public SetupNavyServer()
        {
            points = 1000;

            ships[0, 0] = 1;
            ships[0, 1] = 100;

            ships[1, 0] = 2;
            ships[1, 1] = 150;

            ships[2, 0] = 3;
            ships[2, 1] = 250; 
            
            ships[3, 0] = 4;
            ships[3, 1] = 300;

        }

        //destroyer - Low HP, Defense agaisn't carriers, Fastest
        //cruiser - Destroyer killers, moderate defense agaisn't AA and Navy
        //Battleships - Kills in one shot destroyer if it hits, slow
        //Carrier - Medium HP, Slowest, 
        public void chooseSetup()
        {

            while (!noPoints)
            {
                Console.WriteLine("Escolhe Navio :\n 1 - Destroyer 100 \n 2 - Cruise 150 \n 3 - Battleship 200 \n 4 - Carrier ");
                realnumber = int.TryParse(Console.ReadLine(), out shipType);
                if (realnumber)
                {
                    if (shipType > 0 || shipType < 5)
                    {
                        //switch (shipType)
                        //{
                        //    case 1:
                        //        points = points ;
                        //}
                    }
                    else
                    {
                        Console.WriteLine("Numero inválido tente outra vez!");
                    }
                }
            }
        }
    }
}

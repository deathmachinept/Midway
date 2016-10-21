using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace ServerEmpty
{
    public enum gameState
    {
        setup,
        battle,
        endGame,
    }

    class Gmae
    {
        public static char[,] board;
        public char[,] boardPlayers;
        public int Jogador1Budget, Jogador2Budget;
        public int playerID;
        private List<Ships> gameServerShipList;
        private List<Ships> player1ShipList = new List<Ships>();
        private List<Ships> player2ShipList = new List<Ships>();
        private int shipPlayerId1, shipPlayerId2;
        private Ships tempShip;
        private gameState estado;

        void initialize()
        {
            gameServerShipList = new List<Ships>();
            player1ShipList = new List<Ships>();
            player2ShipList = new List<Ships>();
            shipPlayerId1 = 100;
            shipPlayerId2 = 200;
            estado = gameState.setup;
        }

        static void Main (string[] args)
        {
            initialize();


            while (estado == gameState.setup)
            {
                while (Jogador1Budget > 50)
                {
                    Console.WriteLine("Player 1 - Choose one ship to add to your army? \n 0 - Destroyer 1 - Cruiser 2 - Battleship 3 - Carrier");
                    int idShip = int.Parse(Console.ReadLine());

                    switch (idShip)
                    {
                        case 0:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.destroyer, 1, shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 1:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.cruiser, 1, shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 2:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.battleship, 1, shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 3:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.carrier, 1, shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                    }
                }

                while (Jogador2Budget > 50)
                {
                    Console.WriteLine("Player - 2 Choose one ship to add to your army? \n 0 - Destroyer 1 - Cruiser 2 - Battleship 3 - Carrier");
                    int idShip = int.Parse(Console.ReadLine());

                    switch (idShip)
                    {

                        case 0:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.destroyer, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 1:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.cruiser, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 2:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.battleship, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 3:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.carrier, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;

                    }
                }
                estado = gameState.battle;
            }

            if (estado == gameState.battle)
            {
                int idJogador;
                foreach (Ships shipCollection in gameServerShipList)
                {
                    idJogador = shipCollection.getShipPlayerID;
                    if (idJogador == 1)
                    {
                        player1ShipList.Add(shipCollection);
                    }
                    else if (idJogador == 2)
                    {
                        player2ShipList.Add(shipCollection);
                    }
                }
                Console.WriteLine("Imprimir Ship List player 1 ");
                Console.ReadLine();

                foreach (Ships shipCollection in player1ShipList)
                {
                    Console.WriteLine("Ship ID " + shipCollection.getShipID + "Ship Type: " + shipCollection.getName + "HP: " + shipCollection.ChangeHp);
                }
                Console.WriteLine("Imprimir Ship List player 2 ");
                Console.ReadLine();
                foreach (Ships shipCollection in player2ShipList)
                {
                    Console.WriteLine("Ship ID " + shipCollection.getShipID + "Ship Type: " + shipCollection.getName + "HP: " + shipCollection.ChangeHp);
                }
            }

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerMidway
{
    public enum gameState
    {
        setup,
        battle,
        turnoplay1,
        turnoplay2,
        endGame
    }
    class NovoJogo
    {

        private string[,] boardPlayers = new string[20,11];
        private int Jogador1Budget, Jogador2Budget;
        private List<Ships> player1ShipList = new List<Ships>();
        private List<Ships> player2ShipList = new List<Ships>();
        private gameState estadoJogoServidor;
        private JogadorServer jogador1, jogador2;
        private bool flag;
        private Ships DestroyedShip;

        public NovoJogo(JogadorServer jog1, JogadorServer jog2 )
        {
            this.estadoJogoServidor = gameState.setup;
            this.jogador1 = jog1;
            this.jogador2 = jog2;
            IniciarTabuleiro();
        }

        public gameState GetSetEstadoLobbyGame
        {
            get { return this.estadoJogoServidor; }
            set { this.estadoJogoServidor = value; }
        }

        public JogadorServer getJogador1
        {
            get { return this.jogador1; }
        }

        public JogadorServer getJogador2
        {
            get { return this.jogador2; }
        }

        public JogadorServer retornaJogadorInverso(TcpClient jogadorActual)
        {
            if(jogador1.JogadorTcp == jogadorActual)
            {
                return jogador2;
            }
            else if(jogador2.JogadorTcp == jogadorActual)
            {
               return jogador1;
            }

            return null;
        }


        public string[,] GetBoardPlayers
        {
            get { return this.boardPlayers; }
            set {  this.boardPlayers = value; }
        }

        void IniciarTabuleiro()
        {
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    boardPlayers[x, y] = " ";

                }
            }
        }

        //DD destroyer CR Cruiser CA Carrier BS Battleship
        public bool checkCoordinates(int[,] coordenada, int size)
        {
            int[,] coordenadaArray = coordenada;
            int coordenadaCheckX;
            int coordenadaCheckY;
            for (int i = 0; i < size; i++)
            {

                coordenadaCheckX = coordenadaArray[i,0];
                coordenadaCheckY = coordenadaArray[i,1];
                if (boardPlayers[coordenadaCheckX, coordenadaCheckY] != " ")
                {
                    Console.WriteLine("Tem diferente de espaco " + boardPlayers[coordenadaCheckX, coordenadaCheckY]+" ");
                    return false;
                }
                else if( i == size-1 )
                {
                    Console.WriteLine("Nao tem nada " + size);

                    return true;
                }

            }
            return false;
        }


        public bool AddShipToPlayerList(Ships insertShip)
        {
            int contarSize = insertShip.getSize;
            typeShips tipoNavio = insertShip.getID;
            int[,] coordenadaShip = insertShip.GetSetCoordenadas;
            int coordenadaX;
            int coordenadaY;

            if (checkCoordinates(coordenadaShip, contarSize))
            {
                Console.WriteLine("Check coordinates True");
                if (tipoNavio == typeShips.destroyer)
                {
                    Console.WriteLine("destroyer");
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i,0];
                        coordenadaY = coordenadaShip[i,1];
                        boardPlayers[coordenadaX, coordenadaY] = "DD";
                        Console.WriteLine(boardPlayers[coordenadaX, coordenadaY]);

                        return true;
                    }
                }
                if (tipoNavio == typeShips.battleship)
                {
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i,0];
                        coordenadaY = coordenadaShip[i,1];
                        boardPlayers[coordenadaX, coordenadaY] = "BS";
                        return true;
                    }
                }
                if (tipoNavio == typeShips.carrier)
                {
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i,0];
                        coordenadaY = coordenadaShip[i,1];
                        boardPlayers[coordenadaX, coordenadaY] = "CA";
                        return true;

                    }
                }
                if (tipoNavio == typeShips.cruiser)
                {
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i,0];
                        coordenadaY = coordenadaShip[i,1];
                        boardPlayers[coordenadaX, coordenadaY] = "CR";
                    }
                    return true;
                }
            }
            Console.WriteLine("Check coordinates False");
            return false;
        }

        public bool AddShip(int playerID, Ships ship)
        {
            Console.WriteLine("Player ID " + playerID + " " + ship.getSize);
            for (int i = 0; i < ship.getSize; i++)
            {
                Console.WriteLine("Coordenada X " + ship.GetSetCoordenadas[i, 0]);
                Console.WriteLine("Coordenada Y " + ship.GetSetCoordenadas[i, 1]);

            }

            if (playerID == 1)
            {
                if (jogador1.Budget >= ship.Cost) {
                    if (AddShipToPlayerList(ship)) //true
                    {
                        Ships tempShip = new Ships(ship.getID);
                        Console.WriteLine(" Count List " + player1ShipList.Count);
                        player1ShipList.Add(tempShip);

                        jogador1.Budget -= tempShip.Cost;
                        Console.WriteLine("Jogador 1 budget server " + jogador1.Budget + " Count List "+player1ShipList.Count);
                        return true;
                    }
                    return false;
                }
            }else if (playerID == 2)
            {
                if (AddShipToPlayerList(ship)) //true
                {
                    Ships tempShip = new Ships(ship.getID);
                    player2ShipList.Add(tempShip);
                    jogador2.Budget -= tempShip.Cost;
                    Console.WriteLine("Jogador 2 budget server" + jogador1.Budget);

                    return true;
                }
            }

                //Enviar mensagem de erro na insercao
            return false;
        }

        public Ships FindShipOnPlayer(int findX, int findY, out int playerID)
        {
            int[,] GetCoordenadas;
            int CoordenadaX, CoordenadaY;
            int size;

            foreach (Ships procurarShip in player1ShipList)
            {
                size = procurarShip.getSize;
                GetCoordenadas = procurarShip.GetSetCoordenadas;
                for (int i = 0; i < size; i++)
                {
                    CoordenadaX = GetCoordenadas[i,0];
                    CoordenadaY = GetCoordenadas[i,1];
                    if (CoordenadaX == findX && CoordenadaY == findY)
                    { // retornar navio
                        playerID = 1;
                        return procurarShip;
                    }
                }
            }
            foreach (Ships procurarShip in player2ShipList)
            {
                size = procurarShip.getSize;
                GetCoordenadas = procurarShip.GetSetCoordenadas;
                for (int i = 0; i < size; i++)
                {
                    CoordenadaX = GetCoordenadas[i,0];
                    CoordenadaY = GetCoordenadas[i,1];
                    if (CoordenadaX == findX && CoordenadaY == findY)
                    { // retornar navio
                        playerID = 2;
                        return procurarShip;
                    }
                }
            }
            playerID = 0;
            return null;
        }

        public bool Player1Perdeu()
        {
            if (player1ShipList.Count == 0)
            {
                return true;
            }
            return false;
        }

        public bool Player2Perdeu()
        {
            if (player2ShipList.Count == 0)
            {
                return true;
            }
            return false;
        }


        public bool battleTiro(int x, int y, int damage, out bool navioEstaDestruido, out TcpClient playerIDTcp)
        {
            string type;
            Ships searchResultShips;
            int[,] CoordenadasSearch;
            int size;
            int HP;
            int plyID;
            playerIDTcp = null;
            damage = 0;
            navioEstaDestruido = false;
            if(boardPlayers[x, y] != " ")
            {
                type = boardPlayers[x, y];
                searchResultShips = FindShipOnPlayer(x, y,out plyID);
                if (searchResultShips == null)
                {
                    return false;
                    
                }
                else
                {
                    size = searchResultShips.getSize;
                    HP = searchResultShips.getHP;
                    HP = HP - damage;

                    if (HP <= 0)
                    {
                        if (plyID == 1)
                        {
                            CoordenadasSearch = searchResultShips.GetSetCoordenadas;
                            for (int l = 0; l < size; l++)
                            {
                                boardPlayers[CoordenadasSearch[l,0], CoordenadasSearch[l,1]] = " ";
                            }
                            DestroyedShip = searchResultShips;
                            playerIDTcp = jogador1.JogadorTcp;
                            player1ShipList.Remove(searchResultShips);
                        }else if (plyID == 2)
                        {
                            CoordenadasSearch = searchResultShips.GetSetCoordenadas;
                            for (int l = 0; l < size; l++) //corre as celulas associadas ao navio e eliminda do board
                            {
                                boardPlayers[CoordenadasSearch[l,0], CoordenadasSearch[l,1]] = " ";
                            }
                            DestroyedShip = searchResultShips;
                            playerIDTcp = jogador2.JogadorTcp;
                            player2ShipList.Remove(searchResultShips);
                        }
                        navioEstaDestruido = true;
                    }
                    else
                    {
                        searchResultShips.getHP = HP;

                    }
                    return true;
                }

            }
            return false;
            //escuta commandos de jogo do jogador1
        }
    }
}

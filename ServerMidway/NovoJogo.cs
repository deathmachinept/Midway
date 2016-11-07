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
        endGame
    }
    class NovoJogo
    {

        private char[,] board;
        private string[,] boardPlayers = new string[20,11];
        private int Jogador1Budget, Jogador2Budget;
        private List<Ships> player1ShipList = new List<Ships>();
        private List<Ships> player2ShipList = new List<Ships>();
        private gameState estadoJogoServidor;
        private JogadorServer jogador1, jogador2;
        private bool flag;

        public NovoJogo(JogadorServer jog1, JogadorServer jog2 )
        {
            this.estadoJogoServidor = gameState.setup;
            this.jogador1 = jog1;
            this.jogador2 = jog2;
            IniciarTabuleiro();
        }

        void IniciarTabuleiro()
        {
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; x < 11; x++)
                {
                    boardPlayers[x, y] = " ";
                }
            }
        }

        //DD destroyer CR Cruiser CA Carrier BS Battleship
        bool checkCoordinates(int[][] coordenada, int size)
        {
            int[][] coordenadaArray = coordenada;
            int coordenadaCheckX;
            int coordenadaCheckY;
            for (int i = 0; i < size; i++)
            {
                coordenadaCheckX = coordenadaArray[i][0];
                coordenadaCheckY = coordenadaArray[i][1];
                if (boardPlayers[coordenadaCheckX, coordenadaCheckY] != " ")
                {
                    return false;
                }
                else if( i == size-1 )
                {
                    return true;
                }

            }
            return false;
        }


        bool AddShipToPlayerList(Ships insertShip)
        {
            int contarSize = insertShip.getSize;
            typeShip tipoNavio = insertShip.getID;
            int[][] coordenadaShip = insertShip.GetSetCoordenadas;
            int coordenadaX;
            int coordenadaY;

            if (checkCoordinates(coordenadaShip, contarSize))
            {
                if (tipoNavio == typeShip.destroyer)
                {
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i][0];
                        coordenadaY = coordenadaShip[i][1];
                        boardPlayers[coordenadaX, coordenadaY] = "DD";
                        return true;
                    }
                }
                if (tipoNavio == typeShip.battleship)
                {
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i][0];
                        coordenadaY = coordenadaShip[i][1];
                        boardPlayers[coordenadaX, coordenadaY] = "BS";
                        return true;
                    }
                }
                if (tipoNavio == typeShip.carrier)
                {
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i][0];
                        coordenadaY = coordenadaShip[i][1];
                        boardPlayers[coordenadaX, coordenadaY] = "CA";
                        return true;

                    }
                }
                if (tipoNavio == typeShip.cruiser)
                {
                    for (int i = 0; i < contarSize; i++)
                    {
                        coordenadaX = coordenadaShip[i][0];
                        coordenadaY = coordenadaShip[i][1];
                        boardPlayers[coordenadaX, coordenadaY] = "CR";
                    }
                    return true;
                }
            }
            return false;
        }

        bool AddShip(typeShip tipoNavio, int playerID, Ships Ship)
        {
            if (playerID == 1)
            {
                if (AddShipToPlayerList(Ship)) //true
                {
                    Ships tempShip = new Ships(tipoNavio);
                    player1ShipList.Add(tempShip);
                    return true;
                }

            }else if (playerID == 2)
            {
                if (AddShipToPlayerList(Ship)) //true
                {
                    Ships tempShip = new Ships(tipoNavio);
                    player2ShipList.Add(tempShip);
                    return true;
                }
            }

                //Enviar mensagem de erro na insercao
            return false;
        }

        Ships FindShipOnPlayer(int findX, int findY, out int playerID)
        {
            int[][] GetCoordenadas;
            int CoordenadaX, CoordenadaY;
            int size;

            foreach (Ships procurarShip in player1ShipList)
            {
                size = procurarShip.getSize;
                GetCoordenadas = procurarShip.GetSetCoordenadas;
                for (int i = 0; i < size; i++)
                {
                    CoordenadaX = GetCoordenadas[i][0];
                    CoordenadaY = GetCoordenadas[i][1];
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
                    CoordenadaX = GetCoordenadas[i][0];
                    CoordenadaY = GetCoordenadas[i][1];
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

        bool battleTiro(int x, int y, int size,int damage)
        {
            string type;
            Ships searchResultShips;
            int[][] CoordenadasSearch;
            int HP;
            int plyID;
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
                    HP = searchResultShips.getHP;
                    HP = HP - damage;
                    if (HP <= 0)
                    {
                        if (plyID == 1)
                        {
                            CoordenadasSearch = searchResultShips.GetSetCoordenadas;
                            for (int l = 0; l < size; l++)
                            {
                                boardPlayers[CoordenadasSearch[l][0], CoordenadasSearch[l][1]] = " ";
                            }
                            player1ShipList.Remove(searchResultShips);
                        }else if (plyID == 2)
                        {
                            CoordenadasSearch = searchResultShips.GetSetCoordenadas;
                            for (int l = 0; l < size; l++) //corre as celulas associadas ao navio e eliminda do board
                            {
                                boardPlayers[CoordenadasSearch[l][0], CoordenadasSearch[l][1]] = " ";
                            }
                            player2ShipList.Remove(searchResultShips);
                        }

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

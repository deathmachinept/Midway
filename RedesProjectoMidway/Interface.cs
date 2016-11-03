using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedesProjectoMidway
{
    class Interface
    {
        private Button cruiserButton;
        private Texture2D cruiseIcon;
        private List<ShipButton> NaviosButtons;
        private List<ShipGridButtonPlayer> ListasdeNaviosVisiveis;
        private List<ShipsClient> Jogador1Navios;
        private List<ShipsClient> Jogador2Navios;
        private PoolAvailableShip iniciarAvailableShips;

        private List<ShipsClient> NaviosPoolTemporario;


        private int ScreenWidth, ScreenHeight;
        private int IdJogador;
        public int cellSize, drawStartWidth, drawStartHeight;
        private GraphicsDevice graphicsUI;
        private ContentManager pastaRoot;
        public Interface(GraphicsDevice graphics, ContentManager content, int screenWidth, int screenHeight, int playerID, int cellDimension, int startDrawWidth, int startDrawHeight)
        {

            IdJogador = playerID;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            this.graphicsUI = graphics;
            this.pastaRoot = content;
            this.cellSize =cellDimension;
            this.drawStartWidth = startDrawWidth;
            this.drawStartHeight = startDrawHeight;
            Console.WriteLine(drawStartWidth + " " + startDrawHeight);
            iniciarAvailableShips = new PoolAvailableShip(pastaRoot);

            NaviosButtons = new List<ShipButton>(); // botoes de inferface
            Jogador1Navios = new List<ShipsClient>();
            Jogador2Navios = new List<ShipsClient>();
            ListasdeNaviosVisiveis = new List<ShipGridButtonPlayer>();
            InserirBotoes(iniciarAvailableShips.getAvailableShipsList);

        }




        public void InserirBotoes(List<ShipsClient> naviosDisponiveis)
        {
            int i = 0;
            foreach (ShipsClient ships in naviosDisponiveis)
            {

                if (ships.getID == typeShip.cruiser)
                {
                    i += ScreenWidth / 7;
                    ShipButton tempButtonLista = new ShipButton(this.graphicsUI, ships.getTextureShip, new Vector2(i, 25f), new Vector2(0.25f, 0.25f), typeShip.cruiser);
                    tempButtonLista.changeTexture2D = pastaRoot.Load<Texture2D>("JapaneseCruise");
                    NaviosButtons.Add(tempButtonLista);
                }
                else if (ships.getID == typeShip.destroyer)
                {
                    i += ScreenWidth / 7;
                    ShipButton tempButtonLista = new ShipButton(this.graphicsUI, ships.getTextureShip, new Vector2(i, 25f), new Vector2(0.25f, 0.25f), typeShip.destroyer);
                    tempButtonLista.changeTexture2D = pastaRoot.Load<Texture2D>("JapaneseCruise");
                    NaviosButtons.Add(tempButtonLista);
                }
                else if (ships.getID == typeShip.battleship)
                {
                    i += ScreenWidth / 7;
                    ShipButton tempButtonLista = new ShipButton(this.graphicsUI, ships.getTextureShip, new Vector2(i, 25f), new Vector2(0.25f, 0.25f), typeShip.battleship);
                    tempButtonLista.changeTexture2D = pastaRoot.Load<Texture2D>("JapaneseCruise");
                    NaviosButtons.Add(tempButtonLista);
                }
                else
                {
                    i += ScreenWidth / 7;
                    ShipButton tempButtonLista = new ShipButton(this.graphicsUI, ships.getTextureShip, new Vector2(i, 25f), new Vector2(0.25f, 0.25f), typeShip.carrier);
                    tempButtonLista.changeTexture2D = pastaRoot.Load<Texture2D>("JapaneseCruise");
                    NaviosButtons.Add(tempButtonLista);
                }

            }

            Console.WriteLine(NaviosButtons.Count);
        }

        public void Update(MouseState mouseState, GameTime time)
        {
            foreach (ShipButton btn in NaviosButtons)
            {
                btn.Update(mouseState, time);
                if (btn.devoGerarNovoButton)
                {
                    //NaviosPoolTemporario = iniciarAvailableShips.getAvailableShipsList;
                    typeShip tipo = btn.getShipType;
                    foreach (ShipsClient findType in iniciarAvailableShips.getAvailableShipsList)
                    {
                        if (findType.getID == tipo)
                        {
                            if (IdJogador == 1)
                            {
                                if (mouseState.X > drawStartWidth || mouseState.Y > drawStartHeight)
                                {
                                    if(mouseState.X < drawStartWidth*20 || mouseState.Y < drawStartHeight*11)
                                    {
                                        ShipGridButtonPlayer tempAddButton = new ShipGridButtonPlayer(graphicsUI, findType.getTextureShip,
    new Vector2(btn.obterRectangle.X, btn.obterRectangle.Y), new Vector2(0.25f, 0.25f), tipo, 1); // get rectangle button position

                                        //esta a meter navio na coordenada do ponto do rectangulo
                                        findType.GetSetCoordenadas = new Vector2((int)((btn.obterRectangle.X + cellSize / 2 - drawStartWidth) / cellSize), (int)((btn.obterRectangle.Y + cellSize / 2 - drawStartHeight) / cellSize));
                                        Console.WriteLine("Coordenadas Inseridas " + findType.GetSetCoordenadas);
                                        Jogador1Navios.Add(findType);
                                        ListasdeNaviosVisiveis.Add(tempAddButton);
                                        btn.devoGerarNovoButton = false;
                                        //Console.WriteLine("navio " + findType.GetSetCoordenadas);
                                    }
                                    // e maior que a primeira celula
                                }

                            }
                            else
                            {
                                Jogador2Navios.Add(findType);
                                ShipGridButtonPlayer tempAddButton = new ShipGridButtonPlayer(graphicsUI, findType.getTextureShip,
                                    new Vector2(btn.obterRectangle.X, btn.obterRectangle.Y), new Vector2(0.25f, 0.25f), tipo, 2); // get rectangle button position
                                ListasdeNaviosVisiveis.Add(tempAddButton);
                                btn.devoGerarNovoButton = false;

                            }
                        }
                    }
                }
            }

            foreach (ShipGridButtonPlayer btnGrid in ListasdeNaviosVisiveis)
            {
                btnGrid.Actulizar(mouseState, time);
            }

            //if (mouseState.RightButton == ButtonState.Pressed)
            //{
            //    ListasdeNaviosVisiveis.Clear();
            //}
        }


        public void Draw(SpriteBatch spritebatch, SpriteFont fonte)
        {
            foreach (ShipButton btn in NaviosButtons)
            {
                btn.Draw(spritebatch);
            }


            foreach (ShipGridButtonPlayer btnGrid in ListasdeNaviosVisiveis)
            {
                btnGrid.Draw(spritebatch);
                //Console.WriteLine("@Imprimindo");
            }


        }
    }
}

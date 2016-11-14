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
using Common;
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
        private List<ShipsClient> JogadorNavios;
        private PoolAvailableShip iniciarAvailableShips;

        public ShipsClient TempNavioPorInserir;
        public bool naoHaDinheiroParaNavio = false;
        private List<ShipsClient> NaviosPoolTemporario;
        private NetworkStream NetworkStreamInterface;

        private int ScreenWidth, ScreenHeight;
        public int IdJogador, jogadorBudget;
        public int cellSize, drawStartWidth, drawStartHeight;
        private GraphicsDevice graphicsUI;
        private ContentManager pastaRoot;
        private Point[] procurarCoordenadas = new Point[4];
        public NetworkStream streamCliente;
        private Ships novoShips;
        private MensagemShip novaMensage;
        private Mensagem mensagemDisparo;

        public bool temMensagemaEnviar = false, NavioDestuido = false, temMensagemDeDisparo = false;
        public ShipGridButtonPlayer botaoAguardaConfirmacao;
        private int[,] PointToCoordenadasShips; 



        public Interface(GraphicsDevice graphics, ContentManager content, int screenWidth, int screenHeight, int playerID, int cellDimension, int startDrawWidth, int startDrawHeight, int budget, NetworkStream clientStream)
        {

            IdJogador = playerID;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            this.graphicsUI = graphics;
            this.pastaRoot = content;
            this.cellSize = cellDimension;
            this.drawStartWidth = startDrawWidth;
            this.drawStartHeight = startDrawHeight;
            this.jogadorBudget = budget;
            this.streamCliente = clientStream;
            Console.WriteLine(drawStartWidth + " " + startDrawHeight);
            iniciarAvailableShips = new PoolAvailableShip(pastaRoot);

            NaviosButtons = new List<ShipButton>(); // botoes de inferface
            JogadorNavios = new List<ShipsClient>();
            ListasdeNaviosVisiveis = new List<ShipGridButtonPlayer>();
            InserirBotoes(iniciarAvailableShips.getAvailableShipsList);

        }

        public void DestruirNavio(int coordenadaX, int coordenadaY)
        {
            Point coordenadasEmPoint = new Point(coordenadaX,coordenadaY);
            Point Pixeis = convertCoordenadaEmPixel(new Point(coordenadaX, coordenadaY));
            bool foundShip;

            foreach (ShipGridButtonPlayer btnGrid in ListasdeNaviosVisiveis)
            {

                if (btnGrid.obterRectangle.Contains(Pixeis))
                {
                    ListasdeNaviosVisiveis.Remove(btnGrid);
                };

            }
            foreach (ShipsClient naviosJogador in JogadorNavios)
            {
                int size = naviosJogador.getSize;
                for (int i = 0; i < size; i++)
                {
                    if (naviosJogador.GetSetCoordenadas[i] == coordenadasEmPoint)
                    {
                        foundShip = true;
                        JogadorNavios.Remove(naviosJogador);
                        NavioDestuido = true;
                        break;
                    }
                }
            }

        }

        public MensagemShip getMessagem_A_Enviar
        {
            get { return this.novaMensage; }
        }

        public Mensagem getMessagemDisparo
        {
            get { return this.mensagemDisparo; }
        }

        public ShipGridButtonPlayer getLastButton
        {
            get { return this.botaoAguardaConfirmacao; }
        }

        public bool RemoveButton(ShipGridButtonPlayer RemoveTempButton)
        {
            foreach (ShipGridButtonPlayer FindButton in ListasdeNaviosVisiveis)
            {
                if (FindButton == RemoveTempButton)
                {
                    ListasdeNaviosVisiveis.Remove(RemoveTempButton);
                }
                return true;
            }
            return false;
        }


        public void AddShipToPlayer(bool inserir)
        {
            if(inserir)
            JogadorNavios.Add(TempNavioPorInserir);
            else
            {
                TempNavioPorInserir = null;
            }
        }

        public ShipsClient GetShipTemp
        {
            get { return this.TempNavioPorInserir; }
            set { this.TempNavioPorInserir = value; }
        }

        public void InserirBotoes(List<ShipsClient> naviosDisponiveis)
        {
            int i = 0;
            foreach (ShipsClient ships in naviosDisponiveis)
            {

                if (ships.getTipo == typeShip.cruiser)
                {
                    i += ScreenWidth / 7;
                    ShipButton tempButtonLista = new ShipButton(this.graphicsUI, ships.getTextureShip, new Vector2(i, 25f), new Vector2(0.25f, 0.25f), typeShip.cruiser);
                    tempButtonLista.changeTexture2D = pastaRoot.Load<Texture2D>("JapaneseCruise");
                    NaviosButtons.Add(tempButtonLista);
                }
                else if (ships.getTipo == typeShip.destroyer)
                {
                    i += ScreenWidth / 7;
                    ShipButton tempButtonLista = new ShipButton(this.graphicsUI, ships.getTextureShip, new Vector2(i, 25f), new Vector2(0.25f, 0.25f), typeShip.destroyer);
                    tempButtonLista.changeTexture2D = pastaRoot.Load<Texture2D>("JapaneseCruise");
                    NaviosButtons.Add(tempButtonLista);
                }
                else if (ships.getTipo == typeShip.battleship)
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

        public NetworkStream getSetStream
        {
            get { return this.NetworkStreamInterface; }
            set { this.NetworkStreamInterface = value; }
        }


        public int updateBudget
        {
            get { return this.jogadorBudget; }
            set { this.jogadorBudget = value; }
        }

        public Point convertPixelEmCoordenada(int width, int height)
        {
            Point coordenadasTabuleiro = new Point((int)((width + this.cellSize / 2 - this.drawStartWidth) / this.cellSize),
                                                    (int)((height + this.cellSize / 2 - this.drawStartHeight) / this.cellSize));
            return coordenadasTabuleiro;
        }

        public Point convertPixelEmCoordenadaTopLeft(int width, int height)
        {
            Point coordenadasTabuleiro = new Point((int)(((width - this.drawStartWidth) - (this.cellSize / 2)) / this.cellSize) - 1, (int)(((height - this.drawStartHeight) / this.cellSize) - 1));
            return coordenadasTabuleiro;
        }

        public Point convertCoordenadaEmPixel(Point coordenada)
        {
            Point PixeisCoordenadas = new Point((int)(coordenada.X * cellSize + this.cellSize / 2) + drawStartWidth, (int)(coordenada.Y * this.cellSize + this.cellSize / 2 + drawStartHeight));

            return PixeisCoordenadas;
            //origem no centro
        }

        public Point convertCoordenadaEmPixel(Point coordenada, int size)
        {

            Point PixeisCoordenadas = new Point((int)((coordenada.X * cellSize + ((this.cellSize / 2) * size)) + drawStartWidth), (int)(coordenada.Y * this.cellSize + ((this.cellSize / 2))) + drawStartHeight);
            Console.WriteLine("Coordenadas Pixeis " + PixeisCoordenadas);
            return PixeisCoordenadas;
            //origem no centro
        }

        public void Update(MouseState mouseState, GameTime time, gameState estadoDeJogo)
        {
            if (estadoDeJogo == gameState.setup)
            {
                foreach (ShipButton btn in NaviosButtons)
                {
                    btn.Update(mouseState, time);

                    if (btn.devoGerarNovoButton)
                    {
                        //NaviosPoolTemporario = iniciarAvailableShips.getAvailableShipsList;
                        typeShip tipo = btn.getShipType;

                        #region criar navio no mapa e enviar para server

                        foreach (ShipsClient findType in iniciarAvailableShips.getAvailableShipsList)
                        {
                            if (findType.getTipo == tipo)
                            {

                                    if (mouseState.X > drawStartWidth || mouseState.Y > drawStartHeight)
                                    {
                                        if (mouseState.X < drawStartWidth*20 || mouseState.Y < drawStartHeight*11)
                                        {
                                            TempNavioPorInserir = findType;
                                            int tamanhoShip = TempNavioPorInserir.getSize;
                                            int offsetX = mouseState.X - btn.obterRectangle.X;
                                            int offsetY = mouseState.Y - btn.obterRectangle.Y;
                                            PointToCoordenadasShips = new int[tamanhoShip, 2];
                                            Point tempoPointToFillArray = convertPixelEmCoordenada(btn.obterRectangle.X,
                                                btn.obterRectangle.Y);
                                            Point[] inserirCoordenadasShipCliente = new Point[tamanhoShip];


                                            Console.WriteLine("TempPointFill " + tempoPointToFillArray + " " +
                                                              TempNavioPorInserir.getSize);
                                            for (int i = 0; i < TempNavioPorInserir.getSize; i++)
                                            {
                                                inserirCoordenadasShipCliente[i] = new Point(
                                                    tempoPointToFillArray.X + i,
                                                    tempoPointToFillArray.Y);
                                                PointToCoordenadasShips[i, 0] = (int) tempoPointToFillArray.X + i;
                                                PointToCoordenadasShips[i, 1] = (int) tempoPointToFillArray.Y;

                                            }

                                            TempNavioPorInserir.GetSetCoordenadas = inserirCoordenadasShipCliente;

                                            Point test =
                                                convertCoordenadaEmPixel(TempNavioPorInserir.GetSetCoordenadas[0],
                                                    TempNavioPorInserir.getSize);

                                            Console.WriteLine("Reconversao " +
                                                              convertPixelEmCoordenada(btn.obterRectangle.X,
                                                                  btn.obterRectangle.Y));

                                            ShipGridButtonPlayer tempAddButton = new ShipGridButtonPlayer(graphicsUI,
                                                TempNavioPorInserir.getTextureShip,
                                                new Vector2(test.X, test.Y),
                                                new Vector2(0.25f, 0.25f), tipo, IdJogador);

                                            Console.WriteLine("POINT Coordenadas Inseridas " +
                                                              TempNavioPorInserir.GetSetCoordenadas[0] + " pos 1 " +
                                                              TempNavioPorInserir.GetSetCoordenadas[1]);
                                            Console.WriteLine("Coordenadas Inseridas " + PointToCoordenadasShips[0, 0] +
                                                              " pos 1 " + PointToCoordenadasShips[1, 0]);


                                            botaoAguardaConfirmacao = tempAddButton;
                                            int index = (int) tempAddButton.getShipType;
                                            Common.typeShips tipoShipCommon;

                                            switch (index)
                                            {
                                                case 0:
                                                    tipoShipCommon = Common.typeShips.destroyer;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novoShips.GetSetCoordenadas = PointToCoordenadasShips;

                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);

                                                    temMensagemaEnviar = true;
                                                    break;
                                                case 1:
                                                    tipoShipCommon = Common.typeShips.cruiser;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novoShips.GetSetCoordenadas = PointToCoordenadasShips;
                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);

                                                    temMensagemaEnviar = true;
                                                    break;
                                                case 2:
                                                    tipoShipCommon = Common.typeShips.battleship;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novoShips.GetSetCoordenadas = PointToCoordenadasShips;
                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);

                                                    temMensagemaEnviar = true;

                                                    break;
                                                case 3:
                                                    tipoShipCommon = Common.typeShips.carrier;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novoShips.GetSetCoordenadas = PointToCoordenadasShips;

                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);

                                                    temMensagemaEnviar = true;

                                                    break;
                                            }
                                            ListasdeNaviosVisiveis.Add(tempAddButton);
                                            btn.devoGerarNovoButton = false;

                                        }
                                        // e maior que a primeira celula
                                    }

                            }
                        }

                        #endregion
                    }
                }
             //For Ship button interface

            foreach (ShipGridButtonPlayer btnGrid in ListasdeNaviosVisiveis)
            {
                btnGrid.Actulizar(mouseState, time);
                if (btnGrid.novoMovimento == true)
                {
                    //se houver movimento em botão, buscar o rectangulo anterior antes do movimento do rato
                    //converter a posicao dos pixeis em coordenadas e procurar as coordenadas do navio
                    Console.WriteLine("Old Rectangulo : " + btnGrid.GetOldRectangle.X + btnGrid.GetOldRectangle.Y);
                    Console.WriteLine("Novo Rectangulo : " + btnGrid.GetOldRectangle.X + btnGrid.GetOldRectangle.Y);

                    Point tempPoint = convertPixelEmCoordenadaTopLeft(btnGrid.GetOldRectangle.X,
                        btnGrid.GetOldRectangle.Y);
                    Console.WriteLine("Houve Movimento Coordenadas antigas " + tempPoint.X + " , " + tempPoint.Y);
                    foreach (ShipsClient var in JogadorNavios)
                    {
                        //Corre todos os navios do jogador

                        //para cada navio ver as suas coordenadas 
                        procurarCoordenadas = var.GetSetCoordenadas;
                        for (int i = 0; i < var.getSize; i++)
                        {
                            //
                            if (procurarCoordenadas[i] == tempPoint)
                            {
                                Point novaCoordenada = convertPixelEmCoordenada(btnGrid.obterRectangle.X,
                                    btnGrid.obterRectangle.Y);
                                for (int l = 0; l < var.getSize; l++)
                                {

                                    var.GetSetCoordenadas[l] = new Point(novaCoordenada.X + l, novaCoordenada.Y);
                                }
                                Point novoRectangulo = convertCoordenadaEmPixel(novaCoordenada, var.getSize);
                                btnGrid.novaEscalaRectangulo(novoRectangulo, new Vector2(0.25f, 0.25f));
                                btnGrid.novoMovimento = false;
                                break;
                            }
                        }
                    }
                }
            }
        } // end estado de jogo setup
            else if (estadoDeJogo == gameState.battle)
            {
                foreach (ShipGridButtonPlayer btnGrid in ListasdeNaviosVisiveis)
                {
                    btnGrid.ActulizarBattle(mouseState, time);
                    if (btnGrid.disparoPronto)
                    {
                        int damageNavio = iniciarAvailableShips.getShipType(btnGrid.getShipType).getDamage;

                        mensagemDisparo = new Mensagem(mensagemStateClient.tiro);
                        Point coordenadaDisparo = convertPixelEmCoordenada(btnGrid.posicaoDestinoAtaqueX,
                            btnGrid.posicaoDestinoAtaqueY);
                        mensagemDisparo.CoordenadaX = coordenadaDisparo.X;
                        mensagemDisparo.CoordenadaY = coordenadaDisparo.Y;
                        mensagemDisparo.damage = damageNavio;
                        btnGrid.disparoPronto = false;
                        temMensagemDeDisparo = true;

                    }

                }
            }
    }


        public void Draw(SpriteBatch spritebatch, SpriteFont fonte, gameState estadoCliente)
        {
            if (estadoCliente == gameState.setup)
            {
                foreach (ShipButton btn in NaviosButtons)
                {
                    btn.Draw(spritebatch);
                }
            }


            foreach (ShipGridButtonPlayer btnGrid in ListasdeNaviosVisiveis)
            {
                btnGrid.Draw(spritebatch);
                //Console.WriteLine("@Imprimindo");
            }
        }
    }
}
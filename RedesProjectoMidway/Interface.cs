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
        private List<ShipsClient> Jogador1Navios;
        private List<ShipsClient> Jogador2Navios;
        private PoolAvailableShip iniciarAvailableShips;

        private List<ShipsClient> NaviosPoolTemporario;
        private NetworkStream NetworkStreamInterface;

        private int ScreenWidth, ScreenHeight;
        private int IdJogador, jogadorBudget;
        public int cellSize, drawStartWidth, drawStartHeight;
        private GraphicsDevice graphicsUI;
        private ContentManager pastaRoot;
        private Point[] procurarCoordenadas = new Point[4];
        public NetworkStream streamCliente;
        private Ships novoShips;
        private MensagemShip novaMensage;

        public Interface(GraphicsDevice graphics, ContentManager content, int screenWidth, int screenHeight, int playerID, int cellDimension, int startDrawWidth, int startDrawHeight, int budget, NetworkStream clientStream)
        {

            IdJogador = playerID;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            this.graphicsUI = graphics;
            this.pastaRoot = content;
            this.cellSize =cellDimension;
            this.drawStartWidth = startDrawWidth;
            this.drawStartHeight = startDrawHeight;
            this.jogadorBudget = budget;
            this.streamCliente = clientStream;
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

        public void SendMessageToServer(Mensagem message)
        {
            try
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = message.ByteMessage();

                streamCliente.Write(buffer, 0, buffer.Length);
                streamCliente.Flush();
            }
            catch
            {
                //Debug = "Erro no envio de mensagem para o servidor!";
            }
        }

        public int updateBudget
        {
            get { return this.jogadorBudget; }
            set { this.jogadorBudget = value; }
        }

        public Point convertPixelEmCoordenada(int width, int height)
        {
            Point coordenadasTabuleiro = new Point((int)((width + this.cellSize/2 - this.drawStartWidth)/this.cellSize),
                                                    (int)((height + this.cellSize/2 - this.drawStartHeight)/this.cellSize));
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

        public Point convertCoordenadaEmPixel(Point coordenada,int size)
        {

            Point PixeisCoordenadas = new Point((int)((coordenada.X * cellSize + ((this.cellSize / 2)*size)) + drawStartWidth), (int)(coordenada.Y * this.cellSize + ((this.cellSize / 2))) + drawStartHeight);
            Console.WriteLine("Coordenadas Pixeis "+PixeisCoordenadas);
            return PixeisCoordenadas;
            //origem no centro
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
                        if (findType.getTipo == tipo)
                        {
                            if (jogadorBudget >= findType.Cost)
                            {
                                if (IdJogador == 1)
                                {
                                    if (mouseState.X > drawStartWidth || mouseState.Y > drawStartHeight)
                                    {
                                        if (mouseState.X < drawStartWidth*20 || mouseState.Y < drawStartHeight*11)
                                        {
                                            int tamanhoShip = findType.getSize;
                                            int offsetX = mouseState.X - btn.obterRectangle.X;
                                            int offsetY = mouseState.Y - btn.obterRectangle.Y;

                                            jogadorBudget = jogadorBudget - findType.Cost;
                                            Point tempoPointToFillArray = convertPixelEmCoordenada(btn.obterRectangle.X, btn.obterRectangle.Y);
                                            for (int i = 0; i < findType.getSize; i++)
                                            {
                                                findType.GetSetCoordenadas[i] = convertPixelEmCoordenada(btn.obterRectangle.X+i, btn.obterRectangle.Y);
                                            }
                                            Point test = convertCoordenadaEmPixel(findType.GetSetCoordenadas[0],findType.getSize);

                                            Console.WriteLine("Reconversao "+convertPixelEmCoordenada(btn.obterRectangle.X, btn.obterRectangle.Y) );

                                            ShipGridButtonPlayer tempAddButton = new ShipGridButtonPlayer(graphicsUI,
                                            findType.getTextureShip,
                                            new Vector2(test.X,test.Y),
                                            new Vector2(0.25f, 0.25f), tipo, 1);

                                            //tempAddButton.obterRectangle = new Rectangle(test.X, test.Y, 
                                            //    (int)(findType.getTextureShip.Width*0.25f),  (int)(findType.getTextureShip.Height));
                                            Console.WriteLine("Coordenadas Inseridas " + findType.GetSetCoordenadas[0]);

                                            Jogador1Navios.Add(findType);
                                            int index = (int)tempAddButton.getShipType;
                                            Common.typeShip tipoShipCommon;
                                    
                                            switch (index)
                                            {
                                                case 0:
                                                    tipoShipCommon = Common.typeShip.destroyer;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                                    Console.WriteLine("Devia ter enviado a mensagem");
                                                    SendMessageToServer(novaMensage);

                                                    break;
                                                case 1:
                                                    tipoShipCommon = Common.typeShip.cruiser;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                                    SendMessageToServer(novaMensage);
                                                    break;
                                                case 2:
                                                    tipoShipCommon = Common.typeShip.battleship;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                                    SendMessageToServer(novaMensage);
                                                    break;
                                                case 3:
                                                    tipoShipCommon = Common.typeShip.carrier;
                                                    novoShips = new Ships(tipoShipCommon);
                                                    novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                                    SendMessageToServer(novaMensage);
                                                    break;
                                            }
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
                                    ShipGridButtonPlayer tempAddButton = new ShipGridButtonPlayer(graphicsUI,
                                        findType.getTextureShip,
                                        new Vector2(btn.obterRectangle.X, btn.obterRectangle.Y),
                                        new Vector2(0.25f, 0.25f), tipo, 2); // get rectangle button position
                                    ListasdeNaviosVisiveis.Add(tempAddButton);

                                    int index = (int)tempAddButton.getShipType;
                                    Common.typeShip tipoShipCommon;
                                    Ships novoShips;
                                    MensagemShip novaMensage;
                                    
                                    switch (index)
                                    {
                                        case 0:
                                            tipoShipCommon = Common.typeShip.destroyer;
                                            novoShips = new Ships(tipoShipCommon);
                                            novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                            SendMessageToServer(novaMensage);
                                            break;
                                        case 1:
                                            tipoShipCommon = Common.typeShip.cruiser;
                                            novoShips = new Ships(tipoShipCommon);
                                            novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                            SendMessageToServer(novaMensage);
                                            break;
                                        case 2:
                                            tipoShipCommon = Common.typeShip.battleship;
                                            novoShips = new Ships(tipoShipCommon);
                                            novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                            SendMessageToServer(novaMensage);
                                            break;
                                        case 3:
                                            tipoShipCommon = Common.typeShip.carrier;
                                            novoShips = new Ships(tipoShipCommon);
                                            novaMensage = new MensagemShip(mensagemStateClient.ship, novoShips);
                                            SendMessageToServer(novaMensage);
                                            break;
                                    }
                                    btn.devoGerarNovoButton = false;

                                }
                            }
                        }
                    }
                }
            }

            foreach (ShipGridButtonPlayer btnGrid in ListasdeNaviosVisiveis)
            {
                btnGrid.Actulizar(mouseState, time);
                if (btnGrid.novoMovimento == true)
                {
                    //se houver movimento em botão, buscar o rectangulo anterior antes do movimento do rato
                    //converter a posicao dos pixeis em coordenadas e procurar as coordenadas do navio
                    Console.WriteLine("Old Rectangulo : "+btnGrid.GetOldRectangle.X + btnGrid.GetOldRectangle.Y);
                    Console.WriteLine("Novo Rectangulo : " + btnGrid.GetOldRectangle.X + btnGrid.GetOldRectangle.Y);

                    Point tempPoint = convertPixelEmCoordenadaTopLeft(btnGrid.GetOldRectangle.X, btnGrid.GetOldRectangle.Y);
                    Console.WriteLine("Houve Movimento Coordenadas antigas " + tempPoint.X +" , "+ tempPoint.Y);
                    foreach (ShipsClient var in Jogador1Navios)
                    {
                        //Corre todos os navios do jogador

                        //para cada navio ver as suas coordenadas 
                        procurarCoordenadas = var.GetSetCoordenadas;
                            for (int i = 0; i < var.getSize; i++)
                            {
                                //
                                if (procurarCoordenadas[i] == tempPoint)
                                {
                                    Point novaCoordenada = convertPixelEmCoordenada(btnGrid.obterRectangle.X,btnGrid.obterRectangle.Y);
                                    for (int l = 0; l < var.getSize; l++)
                                    {

                                        var.GetSetCoordenadas[l] = new Point(novaCoordenada.X+l, novaCoordenada.Y);
                                    }
                                    Point novoRectangulo = convertCoordenadaEmPixel(novaCoordenada, var.getSize);
                                    btnGrid.novaEscalaRectangulo(novoRectangulo,new Vector2(0.25f,0.25f));
                                    btnGrid.novoMovimento = false;
                                    break;
                                }
                            }
                      }
                  }
                }
        
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

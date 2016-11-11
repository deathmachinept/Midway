using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;




namespace RedesProjectoMidway
{
    public enum gameState
    {
        wait,
        setup,
        battle,
        endGame,
    }


    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public gameState estadoCliente;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private PoolAvailableShip listadeTiposdeNavios;

        private SpriteFont FontParaTitulos;
        private int timer;
        private Texture2D water, cruise;
        private int screenWidth, screenHeight;

        TcpListener tcpListener;
        Thread listenThread;
        Thread clientThread;
        Thread processar;
        Thread broadcast;
        private int port = 7777;
        private int playerID;


        private Texture2D waterTexture2D;
        Vector2 scaleSprite = new Vector2(0.2f, 0.2f);
        float scaleImagens = 0.50f;

        private int DrawStartWidth, DrawStartHeight, EndDrawWidth, EndDrawHeight;
        private int cellSize = 64;
        private int widthBoard = 20, heightBoard = 11;
        public static string[,] board;
        public int Jogador1Budget = 500, Jogador2Budget = 500;
        private bool leftMouseClique = false;
        private Vector2 posicaoEmClique;

        private Vector2 ScaleImagem = new Vector2(0.5f, 0.5f);

        private TcpClient client;
        private NetworkStream clientStream;
        private Thread waitForServerMessages, processMessage;
        private Tuple<TcpClient, NetworkStream> clientAndStream;

        private List<InterfaceCelula> ListaGrid;
        private List<Button> ListaButton;

        private Interface interfacedeJogInterface;
        private bool iniciarInterface = false, once = false, insercaoShip = false, aguardaConfirmacao = false, onceMessage = false;

        private String MensagemServer = " ", Debug = " ", Debug2 = " ", Debug3 = " ";
        private int cmd; float timerForMessageAction = 0f, TimeSinceWaiting = 0f;

        private List<ShipsClient> listaTesteNavio;
        private string[,] testeBoard;

        private Mensagem EnviarMensageJson;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;



            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;



        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //graphics.PreferredBackBufferWidth = this.GraphicsDevice.DisplayMode.Width;
            //graphics.PreferredBackBufferHeight = this.graphics.GraphicsDevice.DisplayMode.Height;
            //graphics.IsFullScreen = true;
            estadoCliente = gameState.wait;
            once = false;
            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;
            //graphics.ApplyChanges();
            FontParaTitulos = Content.Load<SpriteFont>("FontTitulos");
            int UnidadeStartWidth = (int)((screenWidth / cellSize)); //22
            int UnidadeStartHeight = (int)((screenHeight / cellSize));
            DrawStartWidth = (int)(UnidadeStartWidth + cellSize / 2);
            DrawStartHeight = (int)(UnidadeStartHeight + cellSize / 2);


            board = new string[widthBoard, heightBoard];
            IsMouseVisible = true;

            base.Initialize();
        }

        #region redes
        private void ConnectToServer(string servidor, int porto)
        {

            client = new TcpClient();
            try
            {

                client.Connect(servidor, porto);
                clientStream = client.GetStream();

                clientAndStream = Tuple.Create<TcpClient, NetworkStream>(client, clientStream);

                //Cria thread para comunicacao com servidor
                waitForServerMessages = new Thread(new ParameterizedThreadStart(ReceiveServerMessage));
                waitForServerMessages.IsBackground = true;
                waitForServerMessages.Start(clientAndStream);
            }
            catch (Exception e)
            {
                Debug = "Error a No server connection";
            }

        }

        private void ReceiveServerMessage(object clientAndStream)
        {
            byte[] receiveBuffer = new byte[10025];
            while (true)
            {
                Tuple<TcpClient, NetworkStream> tupleClient = (Tuple<TcpClient, NetworkStream>)clientAndStream;

                NetworkStream networkStream = tupleClient.Item2;
                TcpClient client = tupleClient.Item1;

                while (true)
                {
                    try
                    {
                        var bytesRead = networkStream.Read(receiveBuffer, 0, (int)client.ReceiveBufferSize);
                        if (bytesRead == 0)
                        {
                            Debug = "Perdeu Player";
                            // Read returns 0 if the client closes the connection
                            break;
                        }

                        string mensagem = System.Text.Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);

                        //cria nova thread para processar o conteudo da mensagem
                        processMessage = new Thread(new ParameterizedThreadStart(ProcessServerMessage));
                        processMessage.IsBackground = true;
                        processMessage.Start(mensagem);

                    }
                    catch (Exception ex)
                    {
                        Debug = "ReceivePortMessages: " + ex.ToString();
                        break;
                    }
                }
            }
        }

        public void SendMessageToServer(Mensagem message)
        {
            try
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = message.ByteMessage();

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
            catch
            {
                Debug = "Erro no envio de mensagem para o servidor!";
            }
        }

        private void ProcessServerMessage(object obj)
        {

            string mensagem = (string)obj;

            Mensagem mensagemJson = JsonConvert.DeserializeObject<Mensagem>(mensagem);

            if (mensagemJson.estadoMensagem == mensagemStateClient.id)
            {
                string player_ID = mensagemJson.getMessageString;
                Debug3 = player_ID;
                playerID = Int32.Parse(player_ID);
                iniciarInterface = true;
                estadoCliente = gameState.setup;

            }
            if (mensagemJson.estadoMensagem == mensagemStateClient.confirmacao)
            {
                Console.WriteLine("RECEBER MSG!!");
                onceMessage = true;
                if (mensagemJson.confirmacao)
                {
                    Console.WriteLine("RECEBER Afirmativa!!");
                    insercaoShip = true;
                }
                else
                {
                    insercaoShip = false;

                }
            }

            if (mensagemJson.estadoMensagem == mensagemStateClient.budgetJogador2)
            {
                Console.WriteLine("Buget 2!");
                Jogador2Budget = Jogador2Budget - mensagemJson.budget2;

            }
            if (mensagemJson.estadoMensagem == mensagemStateClient.reinicia)
            {
                estadoCliente = gameState.wait;
                iniciarInterface = true;
            }
            if (mensagemJson.estadoMensagem == mensagemStateClient.info)
            {

                Debug2 = mensagemJson.getMessageString;
                iniciarInterface = true;
            }
            //string[] messages = mensagem.Split(';');
            //foreach (string var in messages)
            //{
            //    Console.WriteLine(var);
            //

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        ///
        #endregion

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            water = Content.Load<Texture2D>("Water");
            cruise = Content.Load<Texture2D>("JapaneseCruise");
            //ListaGrid = new List<InterfaceCelula>();
            ListaButton = new List<Button>();




            for (int x = 0; x < widthBoard; x++)
            {
                for (int y = 0; y < heightBoard; y++)
                {
                    Button tempButton = new Button(graphics.GraphicsDevice, water, new Vector2(x * cellSize + (DrawStartWidth), y * cellSize + (DrawStartHeight)), new Vector2(0.25f, 0.25f));
                    ListaButton.Add(tempButton);
                    //Console.WriteLine("pos "+tempButton.changePositionB);
                }
            }
            //Console.WriteLine("Coordenadas " + new Vector2( cellSize + DrawStartWidth, cellSize + DrawStartHeight));



            // TODO: use this.Content to load your game content here
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (iniciarInterface == true)
            {
                interfacedeJogInterface = new Interface(graphics.GraphicsDevice, Content, screenWidth, screenHeight,playerID, cellSize, DrawStartWidth, DrawStartHeight, Jogador1Budget, clientStream);
                iniciarInterface = false;
                once = true;
            }

            KeyboardState state = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                posicaoEmClique = new Vector2(mouseState.X, mouseState.Y);
            }
            timer = timer + 10;
            if (state.IsKeyDown(Keys.Left) && timer > 500)
            {
                timer = 0;
                ConnectToServer("localhost", 7777);
            }
            if (state.IsKeyDown(Keys.Right) && timer > 1000)
            {
                timer = 0;
                Mensagem novaMsg = new Mensagem(mensagemStateClient.info);
                Console.WriteLine(novaMsg.estadoMensagem);
                novaMsg.inserirMensagem("A mensagem chegou?!");
                SendMessageToServer(novaMsg);


            }


            if (estadoCliente == gameState.setup && once == true)
            {
                interfacedeJogInterface.Update(mouseState, gameTime);

                if (onceMessage && aguardaConfirmacao)
                {
                    onceMessage = false;
                    TimeSinceWaiting = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Console.WriteLine("float " + TimeSinceWaiting);

                }
                else if (aguardaConfirmacao && TimeSinceWaiting!= 0)
                {
                    timerForMessageAction += (float)gameTime.ElapsedGameTime.TotalSeconds - TimeSinceWaiting;
                    Console.WriteLine("float " + timerForMessageAction);

                }

                if (aguardaConfirmacao && insercaoShip == true)
                {
                    Console.WriteLine("Insercao de navio");
                    interfacedeJogInterface.jogadorBudget = interfacedeJogInterface.jogadorBudget -
                                                            interfacedeJogInterface.GetShipTemp.Cost;
                    interfacedeJogInterface.AddShipToPlayer(true);
                    insercaoShip = false;
                    aguardaConfirmacao = false;
                    timerForMessageAction = 0f;
                    Jogador1Budget = interfacedeJogInterface.jogadorBudget;

                }
                else if (aguardaConfirmacao && timerForMessageAction>5000f)
                {
                    Console.WriteLine("Nao chegou a tempo, timerforMessageAction"+timerForMessageAction);
                    interfacedeJogInterface.AddShipToPlayer(false);
                    interfacedeJogInterface.RemoveButton(interfacedeJogInterface.getLastButton);
                    aguardaConfirmacao = false;
                    timerForMessageAction = 0f;
                }


                if (interfacedeJogInterface.temMensagemaEnviar == true)
                {
                    timerForMessageAction = 1000;
                    aguardaConfirmacao = true;
                    MensagemShip temporaria = interfacedeJogInterface.getMessagem_A_Enviar;
                    SendMessageToServer(temporaria);
                    interfacedeJogInterface.temMensagemaEnviar = false;
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();



            //for (int x = 0; x < widthBoard; x++)


            foreach (Button objCelula in ListaButton)
            {
                objCelula.Draw(spriteBatch);

            }
            if (estadoCliente == gameState.setup && once == true)
            {
                interfacedeJogInterface.Draw(spriteBatch, FontParaTitulos);
            }
            spriteBatch.DrawString(FontParaTitulos, "Jogador 1 ", new Vector2(15f, 15f), Color.Black);
            spriteBatch.DrawString(FontParaTitulos, "Budget: " + Jogador1Budget, new Vector2(15f, 45f), Color.Black);

            spriteBatch.DrawString(FontParaTitulos, "Jogador 2 ", new Vector2(screenWidth - 100f, 15f), Color.Black);
            spriteBatch.DrawString(FontParaTitulos, "Budget: " + Jogador2Budget, new Vector2(screenWidth - 100f, 45f), Color.Black);

            //spriteBatch.DrawString(FontParaTitulos, "Posicao Rato: " + posicaoEmClique, new Vector2((float)screenWidth/2, 15f), Color.White);
            spriteBatch.DrawString(FontParaTitulos, MensagemServer, new Vector2((float)screenWidth / 2, 15f), Color.White);
            spriteBatch.DrawString(FontParaTitulos, Debug, new Vector2((float)screenWidth / 2, 25f), Color.White);
            spriteBatch.DrawString(FontParaTitulos, Debug2, new Vector2((float)screenWidth / 2, 35f), Color.White);
            spriteBatch.DrawString(FontParaTitulos, Debug3, new Vector2((float)screenWidth / 2, 60f), Color.Purple);


            spriteBatch.End();
            base.Draw(gameTime);


        }
    }
}
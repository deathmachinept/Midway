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


        private Texture2D waterTexture2D;
        Vector2 scaleSprite = new Vector2(0.2f,0.2f);
        float scaleImagens = 0.50f;

        private int DrawStartWidth, DrawStartHeight, EndDrawWidth, EndDrawHeight;
        private int cellSize = 64;
        private int widthBoard = 20, heightBoard = 11;
        public static string[,] board;
        public int Jogador1Budget = 500, Jogador2Budget = 500;
        private bool leftMouseClique = false;
        private Vector2 posicaoEmClique;

        private Vector2 ScaleImagem = new Vector2(0.5f,0.5f);

        private TcpClient client;
        private NetworkStream clientStream;
        private Thread waitForServerMessages, processMessage;
        private Tuple<TcpClient, NetworkStream> clientAndStream;

        private List<InterfaceCelula> ListaGrid;
        private List<Button> ListaButton;

        private Interface interfacedeJogInterface;
        private bool iniciarInterface = true, once = false;

        private String MensagemServer = " ", Debug = " ", Debug2 = " ";
        int cmd;

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
            estadoCliente = gameState.setup;
            once = true;
            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;
            //graphics.ApplyChanges();
            FontParaTitulos = Content.Load<SpriteFont>("FontTitulos");
            int UnidadeStartWidth = (int)((screenWidth / cellSize)); //22
            int UnidadeStartHeight = (int)((screenHeight / cellSize));
            DrawStartWidth = (int)(UnidadeStartWidth + cellSize / 2);
            DrawStartHeight = (int)(UnidadeStartHeight + cellSize / 2);


            board = new string[widthBoard,heightBoard];
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

                //thread que fica a aguardar comunicações iniciadas pelo servidor
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
                            Debug= "Perdeu Player";
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
            string mensagem = (string) obj;


            string[] messages = mensagem.Split(';');
            foreach (string var in messages)
            {
                Console.WriteLine(var);
            }

            if (messages.Length == 2)
            {
                Debug2 = mensagem;
                mensagem = messages[0];
            }
            else if (messages.Length > 2)
            {
                mensagem = messages[messages.Length-2];
                //Debug = "CAGUEI!"+mensagem;
                //Debug2 = "CAGUEI!"+mensagem;
            }

            //Debug = "Mensagem : " + mensagem;
            string commando = mensagem.Substring(0, 1);

            bool temComando = Int32.TryParse(commando, out cmd);
            Console.WriteLine("TEMO COMANDO  " + temComando + " commando ");
            //Debug = "Tem Commando " + temComando + " cmd : "+ cmd;
            if (temComando)
            {
                switch (cmd)
                {
                    case 0:
                        MensagemServer = mensagem.Substring(1);
                        break;
                    case 1:
                        iniciarInterface = true;
                        estadoCliente = gameState.setup;
                        //Debug = "Tem Commando " + temComando + " cmd : " + cmd;

                        break;
                }
            }


            //JsonTextReader reader = new JsonTextReader(new StringReader(json));
            //reader.SupportMultipleContent = false;


            //while (true)
            //{
            //    if (!reader.Read())
            //    {
            //        break;
            //    }

            //    //ATENÇÃO QUE DAQUI PARA BAIXO SÃO COISAS ESPECIFICAS DO MEU PROJETO,
            //    //PODES IGNORAR E ADAPTAR AO TEU
            //    JsonSerializer serializer = new JsonSerializer();
            //    JObject jsonObj = serializer.Deserialize<JObject>(reader);
            //    Console.WriteLine(jsonObj.ToString());

            //}

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
            listaTesteNavio = new List<ShipsClient>();
            ShipsClient testeNavio = new ShipsClient(typeShip.battleship,Content);
            listaTesteNavio.Add(testeNavio);
            ShipsClient testeNavio1 = new ShipsClient(typeShip.battleship, Content);
            listaTesteNavio.Add(testeNavio1);

            testeBoard = new string[2,2]{{"One","Test"},{"Sent","Kill"}};

            
            
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
                interfacedeJogInterface = new Interface(graphics.GraphicsDevice, Content, screenWidth, screenHeight, 1, cellSize, DrawStartWidth, DrawStartHeight,Jogador1Budget,clientStream);
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
            if (state.IsKeyDown(Keys.Left) && timer>500 )
            {
                timer = 0;
                ConnectToServer("localhost", 7777);
                //Console.WriteLine("Connected!!");
            }
            if (state.IsKeyDown(Keys.Right) && timer>1000 )
            {
                timer = 0;
                //Console.WriteLine("Tentando Enviar!");
                //SendMessageToServer("HELLO WORLD!!");
                Mensagem novaMsg = new Mensagem(mensagemStateClient.setup);
                SendMessageToServer(novaMsg);


            }
            if (state.IsKeyDown(Keys.Up) && timer > 1000)
            {
                // TODO: Add your update logic here
                Console.WriteLine("Screen Width "+screenWidth+"Screen Height " + screenHeight + "Draw sTart Wdith "+DrawStartWidth + 
                    " Draw Star Height "+ DrawStartHeight + " End Draw Star Width " + EndDrawWidth +
                                  " End Draw Star Height " +EndDrawHeight);
            }



            if (estadoCliente == gameState.setup && once == true)
            {
                interfacedeJogInterface.Update(mouseState, gameTime);
                Jogador1Budget = interfacedeJogInterface.updateBudget;
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
            spriteBatch.DrawString(FontParaTitulos, "Jogador 1 ", new Vector2(15f, 15f),Color.Black);
            spriteBatch.DrawString(FontParaTitulos, "Budget: " + Jogador1Budget, new Vector2(15f, 45f), Color.Black);

            spriteBatch.DrawString(FontParaTitulos, "Jogador 2 ", new Vector2(screenWidth-100f, 15f), Color.Black);
            spriteBatch.DrawString(FontParaTitulos, "Budget: " + Jogador1Budget, new Vector2(screenWidth-100f, 45f), Color.Black);

            //spriteBatch.DrawString(FontParaTitulos, "Posicao Rato: " + posicaoEmClique, new Vector2((float)screenWidth/2, 15f), Color.White);
            spriteBatch.DrawString(FontParaTitulos, MensagemServer, new Vector2((float)screenWidth / 2, 15f), Color.White);
            spriteBatch.DrawString(FontParaTitulos, Debug, new Vector2((float)screenWidth / 2, 25f), Color.White);
            spriteBatch.DrawString(FontParaTitulos, Debug2, new Vector2((float)screenWidth / 2, 35f), Color.White);




            spriteBatch.End();
            base.Draw(gameTime);



        }
    }
}

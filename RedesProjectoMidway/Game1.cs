using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace RedesProjectoMidway
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private int timer;
        private Texture2D water;


        TcpListener tcpListener;
        Thread listenThread;
        Thread clientThread;
        Thread processar;
        Thread broadcast;
        private int port = 7777;


        private Texture2D waterTexture2D;
        Vector2 scaleSprite;
        float scaleImagens = 0.50f;

        private int width = 20, height = 10;
        public static char[,] board;
        public char[,] boardPlayer;
        public char[,] boardOponent;
        

        private TcpClient client;
        private NetworkStream clientStream;
        private Thread waitForServerMessages, processMessage;
        private Tuple<TcpClient, NetworkStream> clientAndStream;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            board = new char[width,height];

            base.Initialize();
        }


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
                Console.WriteLine("Error a No server connection");
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
                            Console.WriteLine("Perdeu Player");
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
                        Console.WriteLine("ReceivePortMessages: " + ex.ToString());
                        break;
                    }
                }
            }
        }

        private void SendMessageToServer(string message)
        {
            try
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(message);

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
            catch
            {
                Console.WriteLine("Erro no envio de mensagem para o servidor!");
            }
        }

        private void ProcessServerMessage(object obj)
        {
            string json = (string)obj;
            Console.WriteLine(json);



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
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            water = Content.Load<Texture2D>("Water");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();
            timer = timer + 10;
            if (state.IsKeyDown(Keys.Left) && timer>500 )
            {
                timer = 0;
                ConnectToServer("localhost", 7777);
                Console.WriteLine("Connected!!");
            }
            if (state.IsKeyDown(Keys.Right) && timer>1000 )
            {
                timer = 0;
                Console.WriteLine("Tentando Enviar!");
                SendMessageToServer("HELLO WORLD!!");
                
            }
            // TODO: Add your update logic here


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            



            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    spriteBatch.Draw(water, new Vector2(x*width, y*height), scale: scaleSprite, color: Color.White);
                }
            }

            base.Draw(gameTime);
        }
    }
}

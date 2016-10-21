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


namespace Cliente_MidWay
{

    enum GameState
    {
        MainMenu,
        Options,
        Setup,
        Playing,
        GameOver,
    }



    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    

    public class ClientGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D waterTexture2D;
        Vector2 scaleSprite;
        float scaleImagens = 0.50f;

        private int width = 30 , height = 30;
        public static char[,] board;
        public char[,] boardPlayer;
        public char[,] boardOponent;

        private TcpClient client;
        private NetworkStream clientStream;
        private Thread waitForServerMessages, processMessage;
        private Tuple<TcpClient,NetworkStream> clientAndStream;

        private void ConnectToServer(string servidor, int porto)
        {
            
            client = new TcpClient();
            try
            {

                client.Connect(servidor, porto);
                clientStream = client.GetStream();

                clientAndStream = Tuple.Create<TcpClient,NetworkStream>(client, clientStream);

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



            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            reader.SupportMultipleContent = true;
            while (true)
            {
                if (!reader.Read())
                {
                    break;
                }

                //ATENÇÃO QUE DAQUI PARA BAIXO SÃO COISAS ESPECIFICAS DO MEU PROJETO,
                //PODES IGNORAR E ADAPTAR AO TEU

                JsonSerializer serializer = new JsonSerializer();
                JObject jsonObj = serializer.Deserialize<JObject>(reader);
                Console.WriteLine(jsonObj.ToString());

            }

        }


        public ClientGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //ConnectToServer("127.0.0.1", 7777);
            Console.WriteLine("Connected!!");
            // TODO: Add your initialization logic here
            //scaleSprite = new Vector2(1f * scaleImagens, 1f * scaleImagens);
            //boardPlayer = new char[width, height];
            //boardOponent = new char[width, height];
            //waterTexture2D = Content.Load<Texture2D>("Water");

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        boardPlayer[width, height] = '0';
            //    }
            //}

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {

            //        //spriteBatch.Draw(waterTexture2D, new Vector2(x * size, y * size), scale: scaleSprite, color: Color.White);
            //    }
            //}
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

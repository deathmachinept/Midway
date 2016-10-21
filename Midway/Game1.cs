using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Midway
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>

     public enum gameState
    {
        setup,
        battle,
        endGame,
    }
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
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
        public Game1()
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
            // TODO: Add your initialization logic here
            gameServerShipList = new List<Ships>();
            player1ShipList = new List<Ships>();
            player2ShipList = new List<Ships>();
            shipPlayerId1 = 100;
            shipPlayerId2 = 200;
            estado = gameState.setup;
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

            while (estado == gameState.setup) { 
                while (Jogador1Budget>50)
                {
                    Console.WriteLine("Player 1 - Choose one ship to add to your army? \n 0 - Destroyer 1 - Cruiser 2 - Battleship 3 - Carrier");
                    int idShip = int.Parse(Console.ReadLine());

                    switch (idShip)
                    {
                        case 0:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.destroyer, 1,shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 1:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.cruiser, 1,shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 2:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.battleship, 1,shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 3:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.carrier, 1,shipPlayerId1);
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
                            tempShip = new Ships(typeShip.destroyer, 2,shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: "+Jogador2Budget +"Ship type: "+ tempShip.getName);
                            break;
                        case 1:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.cruiser, 2,shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 2:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.battleship, 2,shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 3:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.carrier, 2,shipPlayerId2);
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
                    Console.WriteLine("Ship ID " +shipCollection.getShipID+ "Ship Type: "+shipCollection.getName + "HP: "+ shipCollection.ChangeHp);
                }
                Console.WriteLine("Imprimir Ship List player 2 ");
                Console.ReadLine();
                foreach (Ships shipCollection in player2ShipList)
                {
                    Console.WriteLine("Ship ID " + shipCollection.getShipID + "Ship Type: " + shipCollection.getName + "HP: " + shipCollection.ChangeHp);
                }
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

            base.Draw(gameTime);
        }
    }
}

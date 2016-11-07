using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RedesProjectoMidway
{
    class ShipGridButtonPlayer:ShipButton
    {
        private int playerID;
        private float angle;
        private Vector2 origin;
        private int deltaScrollWheelValue = 0;
        private int currentScrollWheelValue = 0;
        private int cellSize = 64;
        private bool movimentou = false;
        private Rectangle oldRectangle;

        public ShipGridButtonPlayer(GraphicsDevice graphics, Texture2D textura, Vector2 position, Vector2 escala, typeShip tipoNavio,int Id)
        : base(graphics, textura, position, escala, tipoNavio)
        {
          this.playerID = Id;
          this.origin = new Vector2(160, 31);
          this.oldRectangle = boundingBox;
        }

        public int getPlayerID
        {
            get { return this.playerID; }
        }

        public bool novoMovimento
        {
            get { return this.movimentou; }
            set { this.movimentou = value; }

        }

        public Rectangle GetOldRectangle
        {
            get { return this.oldRectangle; }

        }


        public void Actulizar(MouseState mouseState, GameTime time)
        {
            deltaScrollWheelValue = mouseState.ScrollWheelValue - currentScrollWheelValue;
            currentScrollWheelValue += deltaScrollWheelValue;

            if (boundingBox.Contains(mouseState.Position.X, mouseState.Position.Y))
                Esta_Por_Cima = true;
            else
                Esta_Por_Cima = false;

            if (Esta_Por_Cima && mouseState.LeftButton == ButtonState.Pressed && Tempo_Esperado >= 250)
            {
                if (contarClick == 0)
                {
                    Foi_Clicado = true;
                    presoRato = true;
                    Console.WriteLine("Oldrectangle " + oldRectangle.X + " "+ oldRectangle.Y);
                    contarClick = 1;
                }
                else
                {
                    contarClick = 0;
                    Foi_Clicado = true;
                    presoRato = false;

                }

                Tempo_Esperado = 0;
            }
            else
                Foi_Clicado = false;

            if (presoRato)
            {
                //criar botão temporario, ao largar addicionar a lista dos navios do jogador retirar pontos ao budget,
                //chamar a lista de navios do jogagor, addicionar nav
                //Button tempButtonLista = new Button(gdevice, texturaNavio, new Vector2(i, 25f), new Vector2(0.25f, 0.25f));

                //boundingBox.X = mouseState.X - (int)((changeTexture2D.Width * getEscalaButton.X) / 2);
                //boundingBox.Y = mouseState.Y - (int)((changeTexture2D.Height * getEscalaButton.Y) / 2);
                movimentou = false;
                boundingBox.X = (mouseState.X);
                boundingBox.Y = (mouseState.Y);


                if (deltaScrollWheelValue > 0)
                {
                    angle += 1.5708f;
                }
                else if (deltaScrollWheelValue < 0)
                {
                    angle -= 1.5708f;
                }

            }
            else if(presoRato == false && Foi_Clicado == true)
            {
                boundingBox.X = (mouseState.X);
                boundingBox.Y = (mouseState.Y);
                movimentou = true;
            }

            Tempo_Esperado += time.ElapsedGameTime.Milliseconds;

        }

        //public Point convertPixelEmCoordenada(int width, int height)
        //{
        //    Point coordenadasTabuleiro = new Point((int)((width + this.cellSize / 2 - this.drawStartWidth) / this.cellSize),
        //                                            (int)((height + this.cellSize / 2 - this.drawStartHeight) / this.cellSize));
        //    return coordenadasTabuleiro;
        //}


        public float GetDegrees
        {
            get
            {
                Double AngleD = angle;
                return (float)(AngleD*(180 / Math.PI));
            }
        }

        private double RadianToDegree(double angle)
        {
            return this.angle * (180.0 / Math.PI);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Draw(Textura, boundingBox, new Color(Color.White, 0));
            spritebatch.Draw(
         Textura,
         boundingBox,
         null,
         Color.White,
         angle,
         origin,
          SpriteEffects.None,
         1f
                );

            //spritebatch.Draw(Textura, Posicao,new Rectangle(0, 0, Textura.Width, Textura.Height) , Color.White, angle, Vector2.Zero, 1.0f, SpriteEffects.None, 1);

        }

    }
}

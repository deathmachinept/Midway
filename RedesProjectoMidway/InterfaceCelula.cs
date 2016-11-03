using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RedesProjectoMidway
{
    class InterfaceCelula
    {
        Texture2D Textura;
        private Vector2 escalaB;
        public Vector2 Posicao;
        Rectangle colliderBox;
        GraphicsDevice gdevice;
        private Point oldMouseState = new Point(0,0);

        public bool Clicked;
        public bool Hover;
        private int delay = 0;
        private float scaleB = 1;

        public InterfaceCelula(GraphicsDevice graphics, Texture2D textura, Vector2 posicao, Vector2 escalaBotao)
        {
            this.Textura = textura;
            this.Posicao = posicao;
            this.gdevice = graphics;
            this.escalaB = escalaBotao;
            this.colliderBox = new Rectangle((int)Posicao.X, (int)Posicao.Y, (int)(Textura.Width * escalaB.X), (int)(Textura.Height * escalaB.Y));

        }

        public void Update(MouseState mouseState, GameTime time)
        {

            if (mouseState.Position != oldMouseState)
            {

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    Console.WriteLine("Cliquei!!");

                    if (colliderBox.Contains(mouseState.X, mouseState.Y))
                    {
                        Clicked = true;
                    }
                    else
                        Clicked = false;
                }
                else if (delay == 1000)
                {
                    if (colliderBox.Contains(mouseState.X, mouseState.Y))
                    {
                        Hover = true;
                    }

                }
                oldMouseState = mouseState.Position;
            }

            delay += time.ElapsedGameTime.Milliseconds;
        }

        public void Draw(SpriteBatch spritebatch, SpriteFont fonte)
        {
            spritebatch.Draw(this.Textura, this.colliderBox, Color.White);
            if (Clicked)
            {
                spritebatch.DrawString(fonte, "Posicao "+Posicao, new Vector2(666f, 30f), Color.Red);
            }

        }

        public Vector2 changePositionGrid
        {
            get { return this.Posicao; }
            set { this.Posicao = value; }
        }

    }
}

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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace RedesProjectoMidway
{
    class Button
    {
        protected Texture2D Textura;
        protected Vector2 Posicao;
        //Cor a dar para as letras
        public Color Cor_Texto = Color.Gray;
        protected Rectangle boundingBox, oldBoundingBox;
        GraphicsDevice gdevice;
        private Vector2 escala;
        public bool Foi_Clicado;
        public bool Esta_Por_Cima, presoRato = false, recriarRectangulo = true;
        public int contarClick = 0;
        public float Tempo_Esperado = 0;
        private float scaleB = 1;
        private typeShip casoSejaShipButton;

        public Button(GraphicsDevice graphics, Texture2D textura, Vector2 pos, Vector2 escalaBotao)
        {
            this.Textura = textura;
            this.Posicao = pos;
            this.gdevice = graphics;
            this.escala = escalaBotao;
            this.boundingBox = new Rectangle((int)Posicao.X, (int)Posicao.Y, (int)(Textura.Width * escala.X), (int)(Textura.Height * escala.Y));
            this.oldBoundingBox = boundingBox;
        }

        public Vector2 getEscalaButton
        {
            get { return this.escala; }
            set { this.escala = value; }
        }

        public Texture2D changeTexture2D
        {
            get { return this.Textura; }
            set { this.Textura = value; }
        }


        public Vector2 changePositionB
        {
            get { return this.Posicao; }
            set { this.Posicao = value; }
        }

        public float changeScaleB
        {
            get { return this.scaleB; }
            set { this.scaleB = value; }
        }

        public virtual void Update(MouseState mouseState, GameTime time)
        {

            if (boundingBox.Contains(mouseState.Position.X, mouseState.Position.Y))
                Esta_Por_Cima = true;
            else
                Esta_Por_Cima = false;

            if (Esta_Por_Cima && mouseState.LeftButton == ButtonState.Pressed && Tempo_Esperado >= 250)
            {
                Foi_Clicado = true;
                Tempo_Esperado = 0;
            }
            else
                Foi_Clicado = false;
            Tempo_Esperado += time.ElapsedGameTime.Milliseconds;
        }

       

        protected void novaEscalaRectangulo(Point posicaoRato, Vector2 novaEscala)
        {
            this.boundingBox = new Rectangle((int)posicaoRato.X, (int)posicaoRato.Y, (int)(Textura.Width * novaEscala.X), (int)(Textura.Height * novaEscala.Y));
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(this.Textura, this.boundingBox, Color.White);
        }

        public void Draw_Botao_Territorio(SpriteBatch spritebatch)
        {
            spritebatch.Draw(this.Textura, this.boundingBox, new Color(Color.White, 0));
        }

    }
}

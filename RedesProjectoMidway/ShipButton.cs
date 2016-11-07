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
    class ShipButton:Button
    {
        private typeShip classeNavio;
        private bool gerarNovoButton;
        private Rectangle rectanguloUnidade;

        public ShipButton(GraphicsDevice graphics,Texture2D textura, Vector2 pos, Vector2 escala, typeShip tipoNavio )
            :base(graphics,textura, pos, escala)
        {
            this.classeNavio = tipoNavio;
            this.gerarNovoButton = false;
        }



        public bool devoGerarNovoButton
        {
            get { return this.gerarNovoButton; }
            set {gerarNovoButton=value; }


        }

        public Rectangle obterRectangle
        {
            get { return this.rectanguloUnidade; }
            set {  this.rectanguloUnidade = value; }
        }

        public typeShip getShipType        
        {
            get { return this.classeNavio; }
        }

         public override void Update(MouseState mouseState, GameTime time)
        {

            if (boundingBox.Contains(mouseState.Position.X, mouseState.Position.Y))
                Esta_Por_Cima = true;
            else
                Esta_Por_Cima = false;

            if (Esta_Por_Cima && mouseState.LeftButton == ButtonState.Pressed && Tempo_Esperado >= 250)
            {
                if (contarClick == 0)
                {
                    Foi_Clicado = true;
                    //Console.WriteLine("SHIP CLICKADO!!");
                    presoRato = true;
                    contarClick = 1;
                }
                else
                {
                    contarClick = 0;
                    this.gerarNovoButton = true;
                    Foi_Clicado = true;
                    presoRato = false;
                    this.rectanguloUnidade = this.boundingBox;
                    this.boundingBox = oldBoundingBox;
                    //Console.WriteLine("GerarNovo");

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

                if (recriarRectangulo)
                {
                    recriarRectangulo = false;
                    novaEscalaRectangulo(mouseState.Position,new Vector2(0.35f,0.35f));
                }
                boundingBox.X = mouseState.X - (int)((changeTexture2D.Width * getEscalaButton.X) / 2);
                boundingBox.Y = mouseState.Y - (int)((changeTexture2D.Height * getEscalaButton.Y) / 2) ;
            }

            Tempo_Esperado += time.ElapsedGameTime.Milliseconds;
        }

    }
}

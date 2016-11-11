using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace RedesProjectoMidway
{
    public enum typeShip
    {
        destroyer = 0,
        cruiser = 1,
        battleship = 2,
        carrier = 3
    }

    class ShipsClient
    {

            private int hp;
            private int speed;
            private int damage;
            private int range;
            private int points, size;
            private int IdUnidade;
            private Point[] CoordenadasNavio;
            public string name;
            [JsonIgnore]
            private ContentManager pastaRoot;
            [JsonIgnore]
            private Texture2D textureNavio;

            [JsonIgnore]
            private ContentManager pastaContent;
            private typeShip tipo;

            //private

            public ShipsClient( typeShip selectShip,  ContentManager content)
            {
                tipo = selectShip;
                pastaContent = content;
                preencherShip(tipo);
            }


            public typeShip getTipo
            {
               get { return this.tipo; }
            }

            public int getHP
            {
                get { return this.hp; }
                set { this.hp = value; }
            }
            public int setGetID
            {
                get { return this.IdUnidade; }
                set { this.IdUnidade = value; }
            }

            public int Cost
            {
                get { return this.points; }
            }

            public int getSize
            {
                get { return this.size; }
            }

            public Point[] GetSetCoordenadas
            {
                get { return this.CoordenadasNavio; }
                set { this.CoordenadasNavio=value; }
            }

            public string getName
            {
                get { return this.name; }
            }

            public Texture2D getTextureShip
            {
                get { return this.textureNavio; }
            }


            void preencherShip(typeShip tipo)
            {
                int idTipo = (int) tipo;

                switch (idTipo)
                {
                    case 0: //destroyer
                        this.hp = 100;
                        this.speed = 4;
                        this.damage = 25;
                        this.range = 2;
                        this.points = 50;
                        this.name = "Destroyer";
                        this.size = 2;

                        this.textureNavio = pastaContent.Load<Texture2D>("Cruise_Unit");
                        break;
                    case 1: //cruiser
                        this.hp = 100;
                        this.speed = 3;
                        this.damage = 35;
                        this.range = 3;
                        this.points = 80;
                        this.name = "Cruiser";
                        this.size = 3;
                        this.textureNavio = pastaContent.Load<Texture2D>("Cruise_Unit");
                        break;
                    case 2: //BattleShip
                        this.hp = 300;
                        this.speed = 2;
                        this.damage = 50;
                        this.range = 4;
                        this.points = 120;
                        this.name = "Battleship";
                        this.size = 4;
                        this.textureNavio = pastaContent.Load<Texture2D>("Cruise_Unit");
                        break;
                    case 3: //Carrier
                        this.hp = 250;
                        this.speed = 1;
                        this.damage = 10;
                        this.range = 3;
                        this.points = 150;
                        this.name = "Carrier";
                        this.size = 4;
                        this.textureNavio = pastaContent.Load<Texture2D>("Cruise_Unit");
                        break;
                }
            }
        }
    }

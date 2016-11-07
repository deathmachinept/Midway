using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum typeShip
    {
        destroyer,
        cruiser,
        battleship,
        carrier
    }


    public class Ships
    {
        private int hp;
        private int speed;
        private int damage;
        private int range;
        private int points, size;
        private int[][] CoordenadasNavio;
        public string name;
        //private ContentManager pastaRoot;
        //private Texture2D textureNavio;
        //private ContentManager pastaContent;
        private typeShip tipo;

        //private

        public Ships(typeShip selectShip)
        {
            tipo = selectShip;
            preencherShip(tipo);
        }


        public typeShip getID
        {
            get { return this.tipo; }

        }
        public int Cost
        {
            get { return this.points; }
        }

        public int getHP
        {
            get { return this.hp; }
            set { this.hp = value; }
        }

        public int getSize
        {
            get { return this.size; }
        }

        public int[][] GetSetCoordenadas
        {
            get { return this.CoordenadasNavio; }
            set { this.CoordenadasNavio = value; }
        }

        public string getName
        {
            get { return this.name; }
        }



        void preencherShip(typeShip tipo)
        {
            int idTipo = (int)tipo;

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
                    break;
                case 1: //cruiser
                    this.hp = 100;
                    this.speed = 3;
                    this.damage = 35;
                    this.range = 3;
                    this.points = 80;
                    this.name = "Cruiser";
                    this.size = 3;
                    break;
                case 2: //BattleShip
                    this.hp = 300;
                    this.speed = 2;
                    this.damage = 50;
                    this.range = 4;
                    this.points = 120;
                    this.name = "Battleship";
                    this.size = 4;
                    break;
                case 3: //Carrier
                    this.hp = 250;
                    this.speed = 1;
                    this.damage = 10;
                    this.range = 3;
                    this.points = 150;
                    this.name = "Carrier";
                    this.size = 4;
                    break;
            }
        }
    }
}

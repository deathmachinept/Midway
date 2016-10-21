using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
            public enum typeShip
        {
            destroyer,
            cruiser,
            battleship,
            carrier
        }
    class Ships
    {
            private int hp;
            private int speed;
            private int damage;
            private int range;
            private int points;
            private int shipPlayerID;
            private int shipID;
            public string name;

            private typeShip tipo;

            public Ships(typeShip selecaoTipo, int playerID, int serverShipID)
            {
                tipo = selecaoTipo;
                preencherShip(tipo);
                this.shipPlayerID = playerID;
                this.shipID = serverShipID;
            }

            public int Cost { get { return this.points; } }

            public int ChangeHp
            {
                get { return this.hp; }
                set { this.hp = value; }
            }

            public int ChangeSpeed
            {
                get { return this.speed; }
                set { this.speed = value; }
            }

            public int ChangeDamage
            {
                get { return this.speed; }
                set { this.speed = value; }
            }

            public int ChangeRange
            {
                get { return this.range; }
                set { this.range = value; }
            }

            public string getName
            {
                get { return this.name; }
            }

            public int getShipPlayerID
            {
                get { return this.shipPlayerID; }
            }

            public int getShipID
            {
                get { return this.shipID; }
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
                        break;
                    case 1: //cruiser
                        this.hp = 100;
                        this.speed = 3;
                        this.damage = 35;
                        this.range = 3;
                        this.points = 80;
                        this.name = "Cruiser";
                        break;
                    case 2: //BattleShip
                        this.hp = 300;
                        this.speed = 2;
                        this.damage = 50;
                        this.range = 4;
                        this.points = 120;
                        this.name = "Battleship";
                        break;
                    case 3: //Carrier
                        this.hp = 250;
                        this.speed = 1;
                        this.damage = 10;
                        this.range = 3;
                        this.points = 150;
                        this.name = "Carrier";
                        break;
                }
            }
        }
    }


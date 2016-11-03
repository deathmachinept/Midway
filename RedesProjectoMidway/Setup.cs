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
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace RedesProjectoMidway
{
        public enum stateOfSetup
    {
        begin,
        selectShip,
        deploy
    }

    class Setup
    {
        private List<ShipsClient> ListadeNavios;
        private int plyBudget;
        private stateOfSetup estado;
        public Setup(int playerId, int playersBudget)
        {
            estado = stateOfSetup.begin;
            ListadeNavios = new List<ShipsClient>();
        }

        public void Update(GameTime time, int shipType)
        {
            if (estado == stateOfSetup.begin)
            {
                
            }
        }

        public void SelecionarUnidade(typeShip shipType)
        {
            //ShipsClient tempShipCliente = new ShipsClient();
            //tempShipCliente
            //ListadeNavios.Add();
        }

        public void Draw(SpriteBatch spritebatch)
        {

        }
    }
}

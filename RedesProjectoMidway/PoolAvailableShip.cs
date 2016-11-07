using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace RedesProjectoMidway
{

    class PoolAvailableShip
    {
        //Classe que inicia uma lista dos varios tipos de navios existentes no jogo
        //que e depois chamada qd e perciso selecionar um tipo especifico de navio

        private List<ShipsClient> NaviosDisponiveis;
        private ContentManager pastaRoot;

        public PoolAvailableShip(ContentManager content)
        {
            NaviosDisponiveis = new List<ShipsClient>();
            pastaRoot = content;
            inserirTodosNavios();
        }

        public List<ShipsClient> getAvailableShipsList
        {
            get { return this.NaviosDisponiveis; }
        }

        public ShipsClient getShipType(typeShip obterInfodeTipoNavioShip)
        {
            foreach (ShipsClient infoNavio in NaviosDisponiveis)
            {
                if (infoNavio.getTipo == obterInfodeTipoNavioShip)
                {
                    return infoNavio;
                }
            }
            return null;
        }

        public void inserirTodosNavios()
        {
            ShipsClient tempShipLista = new ShipsClient(typeShip.destroyer, pastaRoot);
            NaviosDisponiveis.Add(tempShipLista);
            tempShipLista = new ShipsClient(typeShip.cruiser, pastaRoot);
            NaviosDisponiveis.Add(tempShipLista);
            tempShipLista = new ShipsClient(typeShip.battleship, pastaRoot);
            NaviosDisponiveis.Add(tempShipLista);
            tempShipLista = new ShipsClient(typeShip.carrier, pastaRoot);
            NaviosDisponiveis.Add(tempShipLista);
        }

    }
}

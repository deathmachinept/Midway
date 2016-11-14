using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common
{
        public enum mensagemStateClient
        {
            wait,
            id,
            info,
            reinicia,
            setup,
            ship,
            confirmacao,
            budgetJogador2,
            budgetInsuficient,
            budgetMaxed,
            startBattleClient, //server manda mensagem para cliente passar para o loop de jogo
            requestBattle,
            checkIfShipLeft,
            destruirNavio,
            waitTurno,
            ganhaste,
            perdeste,
            ingame,
            tiro,
            battle

        }
        public class Mensagem
        {
            [JsonProperty("tipoMensagemJson")] 
            public mensagemStateClient estadoMensagem;

            public string mensagem;
            public bool confirmacao;
            public int CoordenadaX, CoordenadaY;
            public int budget2;
            public int damage;
            public int idJogador;

            public Mensagem(mensagemStateClient mensagemState)
            {
                this.estadoMensagem = mensagemState;
            }

            public void inserirMensagem(string inserirMensagem)
            {
                this.mensagem = inserirMensagem;
            }
            public string getMessageString
            {
                get { return this.mensagem; }
                set { this.mensagem = value; }
            }
            public int GetSetCoordenadaX
            {
                get { return this.CoordenadaX; }
                set { this.CoordenadaX = value; }
            }

            public int GetSetCoordenadaY
            {
                get { return this.CoordenadaY; }
                set { this.CoordenadaY = value; }
            }

            public int BudgetOponent
            {
                get { return this.budget2; }
                set { this.budget2 = value; }
            }

            public bool GetSetConfirmation
            {
                get { return this.confirmacao; }
                set { this.confirmacao = value; }
            }

            public byte[] ByteMessage()
            {
                return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
            }

        }
}

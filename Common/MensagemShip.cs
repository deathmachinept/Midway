using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MensagemShip:Mensagem
    {
        public Ships Navio;
        public MensagemShip(mensagemStateClient mensagemState, Ships NovoNavio)
            : base(mensagemState)
        {
            this.Navio = NovoNavio;
        }

        public Ships GetShipObject
        {
            get { return this.Navio; }
            set { this.Navio = value; }
        }
    }
}

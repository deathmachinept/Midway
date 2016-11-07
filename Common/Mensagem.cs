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
            setup,
            ship,
            battle

        }
        public class Mensagem
        {
            [JsonProperty("tipoMensagemJson")] 
            public mensagemStateClient estadoMensagem;

            public Mensagem(mensagemStateClient mensagemState)
            {
                mensagemStateClient estadoMensagem = mensagemState;
            }

            public byte[] ByteMessage()
            {
                return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
            }

        }
}

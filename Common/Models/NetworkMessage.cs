using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace Common.Models
{
    class NetworkMessage
    {

        [JsonProperty("Message")]
        public string Message { get; set; }


        [JsonProperty("Connected")]
        public bool Connected { get; set; }
        

        [JsonProperty("Player")]
        public Player Player { get; set; }
        public NetworkInstruction NetworkInstruction { get; set; }

    }
}

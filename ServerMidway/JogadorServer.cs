using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerMidway
{
    public enum playerState
    {
        waiting,
        setup,
        playing,
        endGame
    }

    class JogadorServer
    {
        public int Budget;
        public int Side;
        public TcpClient JogadorTcp;
        public NetworkStream JogadorStream;
        public playerState Estado;
        public JogadorServer(TcpClient jogadorClient, NetworkStream streamJog)
        {
            this.Estado = playerState.waiting;
            this.JogadorTcp = jogadorClient;
            this.JogadorStream = streamJog;
            this.Budget = 500;
        }

        public int playerID
        {
            get { return this.Side; }
            set { this.Side=value; }
        }



    }
}

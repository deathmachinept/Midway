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
        playing,
        endGame
    }

    class JogadorServer
    {
        public int Budget;
        public int Side;
        public TcpClient JogadorTcp;
        public playerState Estado;
        public JogadorServer(TcpClient jogadorClient, int side)
        {
            this.Estado = playerState.waiting;
            this.JogadorTcp = jogadorClient;
            this.Side = side;
            this.Budget = 500;
        }


    }
}

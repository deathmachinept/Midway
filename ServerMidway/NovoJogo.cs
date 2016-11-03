using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMidway
{

    class NovoJogo
    {
        private enum gameState
        {
            wait,
            setup,
            battle,
            endGame,
        }
        private char[,] board;
        private char[,] boardPlayers;
        private int Jogador1Budget, Jogador2Budget;
        private List<Ships> player1ShipList = new List<Ships>();
        private List<Ships> player2ShipList = new List<Ships>();
        private gameState estadoJogoServidor;
        public NovoJogo()
        {
            estadoJogoServidor = gameState.wait;
        }
    }
}

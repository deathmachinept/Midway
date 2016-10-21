using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;


namespace Server
{
    public enum gameState
    {
        setup,
        battle,
        endGame,
    }


    class Game
    {

        TcpListener tcpListener;
        Thread listenThread;
        Thread clientThread;
        Thread processar;
        Thread broadcast;
        private int port = 7777;

        
        public char[,] board;
        public char[,] boardPlayers;
        public int Jogador1Budget, Jogador2Budget;
        public int playerID;
        private bool passTurn;
        private List<Ships> gameServerShipList;
        private List<Ships> player1ShipList = new List<Ships>();
        private List<Ships> player2ShipList = new List<Ships>();
        private int shipPlayerId1, shipPlayerId2;
        private Ships tempShip;
        private gameState estado;
        private int contar = 0;

        public void initialize()
        {
            passTurn = false;
            gameServerShipList = new List<Ships>();
            player1ShipList = new List<Ships>();
            player2ShipList = new List<Ships>();
            Jogador1Budget = 500;
            Jogador2Budget = 500;
            shipPlayerId1 = 100;
            shipPlayerId2 = 200;
            estado = gameState.setup;

            iniciarServidor();

        }

        void iniciarServidor()
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("Porta :"+port+"Começa thread");
            listenThread = new Thread(new ThreadStart(escutarClientes));
            listenThread.Start();    
        }

        private void Enviar_mensagem(string mensagem, object client)
        {
            try
            {
                TcpClient tcpClient = (TcpClient)client;
                NetworkStream clientStream = tcpClient.GetStream();
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(mensagem);
                clientStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                //Mensagem não enviada
            }
        }

        void escutarClientes()
        {
            tcpListener.Start();
            Console.WriteLine("Listening....");
            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = new TcpClient();
                client = this.tcpListener.AcceptTcpClient();
                NetworkStream clientStream = client.GetStream();
                Enviar_mensagem("Teste a Mensagem Servidor", client);
                //create a thread to handle communication
                if (contar == 2)
                {
                    LogicadeJogo();
                }
                //with connected client
                clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);

            }

        }

        void LogicadeJogo()
        {
            while (estado == gameState.setup)
            {
                while (Jogador1Budget > 50 || passTurn)
                {
                    Console.WriteLine("Player 1 - Choose one ship to add to your army? \n 0 - Destroyer 1 - Cruiser 2 - Battleship 3 - Carrier");
                    int idShip = int.Parse(Console.ReadLine());

                    switch (idShip)
                    {
                        case 0:
                            if (escolherNavio(0, shipPlayerId1))
                            {
                                Console.WriteLine("Escolha posicao X");
                                Console.ReadLine();
                                Console.WriteLine("Escolha posicao Y");
                                Console.ReadLine();
                            }
                            break;
                        case 1:
                            if (escolherNavio(1, shipPlayerId1))
                            {

                            }
                            else
                            {
                                Console.WriteLine("Deseja adquerir um Destroyer? (Y/N)");
                                String caracter = Console.ReadLine();
                                if (caracter == "Y" || caracter == "y")
                                {
                                    escolherNavio(0, shipPlayerId1);
                                }
                                else if (caracter == "N" || caracter == "n")
                                {
                                    Console.WriteLine("Passar Turno");
                                    passTurn = true;
                                }
                            };
                            break;
                        case 2:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.battleship, 1, shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 3:
                            shipPlayerId1++;
                            tempShip = new Ships(typeShip.carrier, 1, shipPlayerId1);
                            Jogador1Budget = Jogador1Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                            break;
                    }
                }

                while (Jogador2Budget > 50)
                {
                    Console.WriteLine("Player - 2 Choose one ship to add to your army? \n 0 - Destroyer 1 - Cruiser 2 - Battleship 3 - Carrier");
                    int idShip = int.Parse(Console.ReadLine());

                    switch (idShip)
                    {

                        case 0:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.destroyer, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 1:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.cruiser, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 2:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.battleship, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;
                        case 3:
                            shipPlayerId2++;
                            tempShip = new Ships(typeShip.carrier, 2, shipPlayerId2);
                            Jogador2Budget = Jogador2Budget - tempShip.Cost;
                            gameServerShipList.Add(tempShip);
                            Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
                            break;

                    }
                }
                estado = gameState.battle;
            }

            if (estado == gameState.battle)
            {
                int idJogador;
                foreach (Ships shipCollection in gameServerShipList)
                {
                    idJogador = shipCollection.getShipPlayerID;
                    if (idJogador == 1)
                    {
                        player1ShipList.Add(shipCollection);
                    }
                    else if (idJogador == 2)
                    {
                        player2ShipList.Add(shipCollection);
                    }
                }
                Console.WriteLine("Imprimir Ship List player 1 ");
                Console.ReadLine();

                foreach (Ships shipCollection in player1ShipList)
                {
                    Console.WriteLine("Ship ID " + shipCollection.getShipID + "Ship Type: " + shipCollection.getName + "HP: " + shipCollection.ChangeHp);
                }
                Console.WriteLine("Imprimir Ship List player 2 ");
                Console.ReadLine();
                foreach (Ships shipCollection in player2ShipList)
                {
                    Console.WriteLine("Ship ID " + shipCollection.getShipID + "Ship Type: " + shipCollection.getName + "HP: " + shipCollection.ChangeHp);
                }
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;
                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();

                string mensagem = encoder.GetString(message, 0, bytesRead);

                object objecto_mensagem = (object)mensagem;
                object objecto_cliente = (object)tcpClient;

                object[] parametros = { objecto_mensagem, objecto_cliente };
                object param = (object)parametros;

                processar = new Thread(new ParameterizedThreadStart(Processar_comando));
                try
                {
                    processar.Start(param);
                }
                catch
                {
                    processar.Join();
                    processar.Abort();
                }
            }
            tcpClient.Close();
        }


        //Processar Commando
        private void Processar_comando(object param)
        {

            object[] parametros = (object[])param;
            string mensagem = (string)parametros[0];
            TcpClient cliente = (TcpClient)parametros[1];
            
          }




        bool escolherNavio( int escolha, int playerID)
        {
            int playerBudget = 0;
            int oldshipPlayerId = 0;
            int shipPlayerId = 0;

            if (playerID == 1){
                playerBudget = Jogador1Budget;
                shipPlayerId = shipPlayerId1;
                oldshipPlayerId = shipPlayerId1;
            }
            else if (playerID == 2) {
                playerBudget = Jogador2Budget;
                shipPlayerId = shipPlayerId2;
                oldshipPlayerId = shipPlayerId2;
            }

            switch (escolha)
            {
                case 0:
                    shipPlayerId1++;
                    tempShip = new Ships(typeShip.destroyer, playerID, shipPlayerId);
                    playerBudget = playerBudget - tempShip.Cost;
                    gameServerShipList.Add(tempShip);
                    Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
                    return true;
                    break;
                case 1:
                    shipPlayerId1++;
                    tempShip = new Ships(typeShip.cruiser, playerID, shipPlayerId);
                    if (tempShip.Cost <= playerBudget)
                    {
                         playerBudget = playerBudget - tempShip.Cost;
                         gameServerShipList.Add(tempShip);
                            if (playerID == 1)
                                Jogador1Budget = playerBudget;
                            else if (playerID == 2)
                                Jogador2Budget = playerBudget;
                        Console.WriteLine("Budget: " + playerBudget + "Ship type: " + tempShip.getName);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Não possui os pontos necessários para escolher um "+tempShip.getName +"\n");
                        if (playerID == 1)
                            shipPlayerId1 = oldshipPlayerId;
                        else if (playerID == 2)
                            shipPlayerId2 = oldshipPlayerId;
                        return false;
                    }
                    break;
                case 2:
                    shipPlayerId1++;
                    tempShip = new Ships(typeShip.cruiser, playerID, shipPlayerId);
                    if (tempShip.Cost <= playerBudget)
                    {
                         playerBudget = playerBudget - tempShip.Cost;
                         gameServerShipList.Add(tempShip);
                            if (playerID == 1)
                                Jogador1Budget = playerBudget;
                            else if (playerID == 2)
                                Jogador2Budget = playerBudget;
                         Console.WriteLine("Budget: " + playerBudget + "Ship type: " + tempShip.getName);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Não possui os pontos necessários para escolher um "+tempShip.getName +"\n");
                        if (playerID == 1)
                            shipPlayerId1 = oldshipPlayerId;
                        else if (playerID == 2)
                            shipPlayerId2 = oldshipPlayerId;
                        return false;
                    }
                    break;
                case 3:
                    shipPlayerId1++;
                    tempShip = new Ships(typeShip.carrier, playerID, shipPlayerId);
                    if (tempShip.Cost <= playerBudget)
                    {
                         playerBudget = playerBudget - tempShip.Cost;
                         gameServerShipList.Add(tempShip);
                            if (playerID == 1)
                                Jogador1Budget = playerBudget;
                            else if (playerID == 2)
                                Jogador2Budget = playerBudget;
                         Console.WriteLine("Budget: " + playerBudget + "Ship type: " + tempShip.getName);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Não possui os pontos necessários para escolher um "+tempShip.getName +"\n");
                        if (playerID == 1)
                            shipPlayerId1 = oldshipPlayerId;
                        else if (playerID == 2)
                            shipPlayerId2 = oldshipPlayerId;
                        return false;
                    }
                    break;
            }
            return false;
        }

        public Game(){
            initialize();


           


        }


    }
}

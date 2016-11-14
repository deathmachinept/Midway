using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common;



namespace ServerMidway
{
    //public enum gameState
    //{
    //    wait,
    //    setup,
    //    battle,
    //    endGame,
    //}


    class ServerGame
    {

        TcpListener tcpListener;
        Thread listenThread;
        Thread clientThread;
        Thread processar;
        Thread broadcast;
        private Tuple<TcpClient, NetworkStream> MensagemTuple;
        private int port = 7777;
        private bool serverFull = false;


        public char[,] board;
        public char[,] boardPlayers;
        public int Jogador1Budget, Jogador2Budget;
        public int playerID;
        private bool passTurn;
        private Ships detroyerFindMinCost;
        private List<Ships> gameServerShipList;
        private List<Ships> player1ShipList = new List<Ships>();
        private List<Ships> player2ShipList = new List<Ships>();
        private List<TcpClient> listaClientes = new List<TcpClient>();
        private int shipPlayerId1, shipPlayerId2;
        private Ships tempShip;
        private gameState estado;
        private int contar = 0;
        private bool jogadorDisconnect, gerado, clienteJaExiste = false;
        private TcpClient jogadorDisconnectado;
        private Random seed;
        private int GuardarRandomNum;
        private List<JogadorServer> listaJogadoresNoServer;
        private List<NovoJogo> LobbyJogos;
        private int commando;

        public void initialize()
        {
            seed = new Random();
            jogadorDisconnect = false;
            passTurn = false;
            gameServerShipList = new List<Ships>();
            player1ShipList = new List<Ships>();
            player2ShipList = new List<Ships>();
            LobbyJogos = new List<NovoJogo>();
            listaJogadoresNoServer = new List<JogadorServer>();
            detroyerFindMinCost = new Ships(typeShips.destroyer);
            estado = gameState.setup;

            IniciarServidor();
            Console.ReadLine();
        }

        NovoJogo FindGame(TcpClient findPlayerInGame)
        {
            foreach (NovoJogo Jogo in LobbyJogos)
            {
                if (Jogo.getJogador1.JogadorTcp == findPlayerInGame ||Jogo.getJogador2.JogadorTcp == findPlayerInGame )
                {
                    return Jogo;
                }
            }
            return null;
        }

        JogadorServer FindJogadorWithTcp(TcpClient findPlayerInGame)
        {
            foreach (NovoJogo Jogo in LobbyJogos)
            {
                if (Jogo.getJogador1.JogadorTcp == findPlayerInGame )
                {
                    return Jogo.getJogador1;
                }
                if (Jogo.getJogador2.JogadorTcp == findPlayerInGame)
                {
                    return Jogo.getJogador2;
                }
            }
            return null;
        }

        void IniciarServidor()
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("Porta :" + port + "Começa thread");
            listenThread = new Thread(new ThreadStart(escutarClientes));
            listenThread.Start();
        }

        private void Enviar_mensagem(string mensagem, object client)
        {
            try
            {
                TcpClient tcpClient = (TcpClient) client;
                NetworkStream clientStream = tcpClient.GetStream();
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(mensagem);
                clientStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                Console.WriteLine("Mensagem Falhou o Envio!!");
                //Mensagem não enviada
            }
        }

        private void Enviar_mensagemJson(Mensagem mensagemObj, NetworkStream clientStream)
        {
            try
            {

                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = mensagemObj.ByteMessage();
                clientStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                Console.WriteLine("Mensagem Falhou o Envio!!");
                //Mensagem não enviada
            }
        }

        TcpClient checkExistenciaClient(TcpClient checkCliente)
        {
            foreach (TcpClient client in listaClientes)
            {
                if (client == checkCliente)
                {
                    return client;
                }
            }
            return null;
        }

        void escutarClientes()
        {
            tcpListener.Start();
            Console.WriteLine("Listening....");
            while (true)
            {
                //So entra se a placa de rede detectar 
                TcpClient client = new TcpClient();
                client = this.tcpListener.AcceptTcpClient();
                JogadorServer tempJogador;
                JogadorServer obterJogador;
                NetworkStream clientStream = client.GetStream();
                tempJogador = new JogadorServer(client, clientStream);

                foreach (JogadorServer CheckExistencia in listaJogadoresNoServer)
                {
                    if (CheckExistencia.JogadorTcp == client)
                    {
                        tempJogador = CheckExistencia;
                        clienteJaExiste = true;
                    }
                    else
                    {
                        clienteJaExiste = false;
                    }
                }
                #region cliente novo
                if (clienteJaExiste == false)
                {

                    listaJogadoresNoServer.Add(tempJogador);

                    obterJogador = listaJogadoresNoServer.Find(p => p.Estado == playerState.waiting && p != tempJogador);

                    if (obterJogador != null)
                    {

                        GuardarRandomNum = seed.Next(1, 2);

                        if (GuardarRandomNum == 1)
                        {
                            tempJogador.playerID = 2;
                            obterJogador.playerID = 1;
                        }
                        else
                        {
                            tempJogador.playerID = 1;
                            obterJogador.playerID = 2;
                        }

                        obterJogador.Estado = playerState.playing;
                        tempJogador.Estado = playerState.playing;

                        NovoJogo tempJogo = new NovoJogo(obterJogador, tempJogador);
                        LobbyJogos.Add(tempJogo);
                        Mensagem mensagemInicio = new Mensagem(mensagemStateClient.id);
                        mensagemInicio.inserirMensagem(obterJogador.playerID.ToString());
                        Enviar_mensagemJson(mensagemInicio, obterJogador.JogadorStream);
                        mensagemInicio.inserirMensagem(tempJogador.playerID.ToString());
                        Enviar_mensagemJson(mensagemInicio, tempJogador.JogadorStream);
                        Console.WriteLine("Jogo Iniciado!!");

                        serverFull = true;
                    }
                    else
                    {
                         Console.WriteLine("Jogador Connectado! ;" + tempJogador.JogadorTcp.Client.RemoteEndPoint.ToString());

                         if (serverFull)
                            {
                                Mensagem mensagem = new Mensagem(mensagemStateClient.info);
                                mensagem.mensagem = "Jogo já em execução! foi inserido na filha! ;" +
                                                    tempJogador.JogadorTcp;
                                Enviar_mensagemJson(mensagem, tempJogador.JogadorStream);
                            }
                            else
                            {
                                Mensagem mensagem = new Mensagem(mensagemStateClient.info);
                                mensagem.mensagem = "A espera de jogadores... " + tempJogador.JogadorTcp;
                                Enviar_mensagemJson(mensagem, tempJogador.JogadorStream);
                            }
                    }
                    MensagemTuple = new Tuple<TcpClient, NetworkStream>(client, clientStream);
                    //with connected client
                    clientThread = new Thread(new ParameterizedThreadStart(CommunicaodeJogador));
                    clientThread.Start(MensagemTuple);
                }
                #endregion 
                else if (clienteJaExiste == true)
                {
                    obterJogador = listaJogadoresNoServer.Find(p => p.Estado == playerState.waiting && p != tempJogador);

                    if (obterJogador != null)
                    {

                        GuardarRandomNum = seed.Next(1, 2);

                        if (GuardarRandomNum == 1)
                        {
                            tempJogador.playerID = 2;
                            obterJogador.playerID = 1;
                        }
                        else
                        {
                            tempJogador.playerID = 1;
                            obterJogador.playerID = 2;
                        }

                        obterJogador.Estado = playerState.playing;
                        tempJogador.Estado = playerState.playing;

                        NovoJogo tempJogo = new NovoJogo(obterJogador, tempJogador);
                        LobbyJogos.Add(tempJogo);
                        Mensagem mensagemInicio = new Mensagem(mensagemStateClient.id);
                        mensagemInicio.inserirMensagem(obterJogador.playerID.ToString());
                        Enviar_mensagemJson(mensagemInicio, obterJogador.JogadorStream);
                        mensagemInicio.inserirMensagem(tempJogador.playerID.ToString());
                        Enviar_mensagemJson(mensagemInicio, tempJogador.JogadorStream);
                        Console.WriteLine("Jogo Iniciado!!");

                        serverFull = true;
                    }
                    else
                    {
                        Console.WriteLine("Jogador Connectado! ;" + tempJogador.JogadorTcp.Client.RemoteEndPoint.ToString());
                        if (serverFull)
                        {
                            Mensagem mensagem = new Mensagem(mensagemStateClient.info);
                            mensagem.mensagem = "Jogo já em execução! foi inserido na filha! ;" + tempJogador.JogadorTcp.ToString();
                            Enviar_mensagemJson(mensagem, tempJogador.JogadorStream);
                        }
                        else
                        {
                            Mensagem mensagem = new Mensagem(mensagemStateClient.info);
                            mensagem.mensagem = "A espera de jogadores... " + tempJogador.JogadorTcp;
                            Enviar_mensagemJson(mensagem, tempJogador.JogadorStream);
                        }
                    }
                }
            }
        }

        void ReiniciarJogoAposDisconnectDeUmJogador(TcpClient jogadorDisconnect, NovoJogo jogoInterrupido, JogadorServer jogadorAindaEmJogo)
        {

            //Remove Jogador da lista de jogadores connectados ao servidador
            listaJogadoresNoServer.Remove(FindJogadorWithTcp(jogadorDisconnect));
            foreach (JogadorServer jogador in listaJogadoresNoServer)
            {
                if (jogador == jogadorAindaEmJogo)
                {

                    jogador.Estado = playerState.waiting;
                    jogador.playerID = seed.Next(1, 2);
                }
            }

            // Buscar o monogame do jogador ainda em jogo por no estado waiting
            if (LobbyJogos.Count > 1)
            {
                foreach (NovoJogo jogo in LobbyJogos)
                {
                    if (jogo == jogoInterrupido)
                    {
                        LobbyJogos.Remove(jogoInterrupido);
                    }
                }
            }
        }

        private void CommunicaodeJogador(object client)
        {
            Tuple<TcpClient, NetworkStream> clientMessagem = (Tuple<TcpClient, NetworkStream>) client;
            TcpClient tcpClient = clientMessagem.Item1;
            NetworkStream clientStream = clientMessagem.Item2;

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
                    Console.WriteLine("Disconnected! Erro na stream do jogador" );
                    jogadorDisconnect = true;
                    jogadorDisconnectado = tcpClient;
                    NovoJogo jogoInterrupido = FindGame(jogadorDisconnectado);
                    JogadorServer JogadorAindaEmJogo = jogoInterrupido.retornaJogadorInverso(jogadorDisconnectado);

                    Enviar_mensagemJson(new Mensagem(mensagemStateClient.reinicia), JogadorAindaEmJogo.JogadorStream);
                    ReiniciarJogoAposDisconnectDeUmJogador(jogadorDisconnectado, jogoInterrupido, JogadorAindaEmJogo);

                    break;
                }

                if (bytesRead == 0)
                {
                    Console.WriteLine("Disconnected! Nao foram enviados bytes para ler");
                    jogadorDisconnect = true;
                    jogadorDisconnectado = tcpClient;
                    NovoJogo jogoInterrupido = FindGame(jogadorDisconnectado);
                    JogadorServer JogadorAindaEmJogo = jogoInterrupido.retornaJogadorInverso(jogadorDisconnectado);


                    Enviar_mensagemJson(new Mensagem(mensagemStateClient.reinicia), JogadorAindaEmJogo.JogadorStream);
                    ReiniciarJogoAposDisconnectDeUmJogador(jogadorDisconnectado, jogoInterrupido, JogadorAindaEmJogo);
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();

                string mensagem = encoder.GetString(message, 0, bytesRead);
                string mensagem2 = mensagem;
                MensagemShip jsonObjShip;
                Mensagem jsonObj = JsonConvert.DeserializeObject<Mensagem>(mensagem);
                if (jsonObj.estadoMensagem == mensagemStateClient.ship)
                {
                    jsonObjShip = JsonConvert.DeserializeObject<MensagemShip>(mensagem2);

                    Ships ReadNavioMessage = jsonObjShip.Navio;

                    for (int i = 0; i < ReadNavioMessage.getSize; i++)
                    {
                        Console.WriteLine("Coordenada X " + ReadNavioMessage.GetSetCoordenadas[i, 0]);
                        Console.WriteLine("Coordenada Y " + ReadNavioMessage.GetSetCoordenadas[i, 1]);

                    }

                    NovoJogo jogo = FindGame(tcpClient);
                    Mensagem confirmacaoMsg = new Mensagem(mensagemStateClient.confirmacao);

                    JogadorServer jogador = (FindJogadorWithTcp(tcpClient));

                    if (jogador.Budget >= ReadNavioMessage.Cost) // Budget do jogador maior ou igual ao do navio que ele quer comprar
                    {
                        Console.WriteLine("Tem que ser falso!!");
                        if (jogo.AddShip(FindJogadorWithTcp(tcpClient).playerID, ReadNavioMessage))
                        {
                            Console.WriteLine("Enviar Mensagem!");
                            confirmacaoMsg.confirmacao = true;
                            Enviar_mensagemJson(confirmacaoMsg, clientStream);


                            Mensagem MensagemInfoOponent = new Mensagem(mensagemStateClient.budgetJogador2);
                            MensagemInfoOponent.budget2 = ReadNavioMessage.Cost;

                            JogadorServer oponente = jogo.retornaJogadorInverso(tcpClient);
                            Console.WriteLine("Enviar Mensagem budget!");
                            Enviar_mensagemJson(MensagemInfoOponent, oponente.JogadorStream);
                        }
                        else
                        {

                            confirmacaoMsg.confirmacao = false;
                            Enviar_mensagemJson(confirmacaoMsg, clientStream);
                        }
                        

                    }
                    else if (detroyerFindMinCost.Cost > jogador.Budget)
                    {
                            confirmacaoMsg.confirmacao = false;
                            Enviar_mensagemJson(confirmacaoMsg, clientStream);

                            Mensagem mensagemBudgetMaxed = confirmacaoMsg;
                            mensagemBudgetMaxed.estadoMensagem = mensagemStateClient.budgetMaxed;
                            mensagemBudgetMaxed.confirmacao = false;
                            Enviar_mensagemJson(mensagemBudgetMaxed, clientStream);

                    }

                }
                else if (jsonObj.estadoMensagem == mensagemStateClient.tiro)
                {
                    NovoJogo jogo = FindGame(tcpClient);
                    bool navioDestruido =false;
                    JogadorServer oponente = jogo.retornaJogadorInverso(tcpClient);
                    JogadorServer jogador = FindJogadorWithTcp(tcpClient);
                    TcpClient jogadorTcpDestruido;


                    if (jogo.battleTiro(jsonObj.CoordenadaX, jsonObj.CoordenadaY, jsonObj.damage, out navioDestruido, out jogadorTcpDestruido))
                    {
                        if (navioDestruido)
                        {
                            oponente = FindJogadorWithTcp(jogadorTcpDestruido);
                            Mensagem destruirNavio = new Mensagem(mensagemStateClient.destruirNavio);
                            destruirNavio.CoordenadaX = jsonObj.CoordenadaX;
                            destruirNavio.CoordenadaY = jsonObj.CoordenadaY;
                            destruirNavio.mensagem = "O teu navio nas coordenadas X " + jsonObj.CoordenadaX + " Y " +
                                                     jsonObj.CoordenadaX + " Foi Destruido! Sofrendo " + jsonObj.damage+ " Dano!";
                            Enviar_mensagemJson(destruirNavio, oponente.JogadorStream);
                            Mensagem relatorio = new Mensagem(mensagemStateClient.info);
                            relatorio.mensagem= "Acertaste num navio inimigo nas coordenadas X " + jsonObj.CoordenadaX + " Y " +
                                                     jsonObj.CoordenadaX + " Foi Destruido! Sofrendo " + jsonObj.damage+ " Dano!";
                            Enviar_mensagemJson(relatorio,clientStream);
                            Console.WriteLine("Nao Acertaste em nenhum navio!!");

                        }
                    }
                    else
                    {
                        Mensagem information = new Mensagem(mensagemStateClient.info);
                        information.mensagem = "Nao Acertaste em nenhum navio!";
                        Console.WriteLine("Nao Acertaste em nenhum navio!!");
                    };

                        Mensagem turnoAtaque = new Mensagem(mensagemStateClient.battle);
                        Mensagem turnoWait = new Mensagem(mensagemStateClient.waitTurno);

                        Enviar_mensagemJson(turnoAtaque, oponente.JogadorStream);
                        oponente.Estado = playerState.playing;
                        Enviar_mensagemJson(turnoWait, jogador.JogadorStream);
                        jogador.Estado = playerState.wait;

                }
                else if (jsonObj.estadoMensagem == mensagemStateClient.info)
                {
                    Console.WriteLine("Entrou");
                    Console.WriteLine(jsonObj.getMessageString);
                }

                else if (jsonObj.estadoMensagem == mensagemStateClient.budgetMaxed)
                {
                    Mensagem confirmacaoMsg = new Mensagem(mensagemStateClient.budgetMaxed);
                    confirmacaoMsg.confirmacao = false;
                    Enviar_mensagemJson(confirmacaoMsg, clientStream);
                }

                else if (jsonObj.estadoMensagem == mensagemStateClient.requestBattle)
                {
                    bool navioDestruido;
                    NovoJogo jogo = FindGame(tcpClient);
                    JogadorServer oponente = jogo.retornaJogadorInverso(tcpClient);
                    JogadorServer jogador = FindJogadorWithTcp(tcpClient);
                    jogador.Estado = playerState.playing;
                    if (oponente.Estado == playerState.playing || oponente.Estado == playerState.wait)
                    {
                        Mensagem mensagemInicioBattalha = new Mensagem(mensagemStateClient.battle);
                        jogo.GetSetEstadoLobbyGame = gameState.battle;
                        Mensagem mensagemSegundoJogador = new Mensagem(mensagemStateClient.waitTurno);
                        oponente.Estado = playerState.wait;
                        jogador.Estado = playerState.playing;

                        Enviar_mensagemJson(mensagemSegundoJogador, oponente.JogadorStream);
                        Enviar_mensagemJson(mensagemInicioBattalha, jogador.JogadorStream);


                    }
                    else
                    {
                        Mensagem mensagemInicioBattalha = new Mensagem(mensagemStateClient.waitTurno);
                        mensagemInicioBattalha.confirmacao = false;
                        jogo.GetSetEstadoLobbyGame = gameState.battle;
                        Enviar_mensagemJson(mensagemInicioBattalha, jogador.JogadorStream);
                    }

                }
                
                else if (jsonObj.estadoMensagem == mensagemStateClient.checkIfShipLeft)
                {
                    NovoJogo jogo = FindGame(tcpClient);
                    bool algumJogadorPerdeu = false;
                    int playerID = 0;
                    JogadorServer oponente = jogo.retornaJogadorInverso(tcpClient);
                    JogadorServer jogador = FindJogadorWithTcp(tcpClient);

                    if (jogo.Player1Perdeu())
                    {
                        algumJogadorPerdeu = true;
                        playerID = 1;
                    }
                    if (jogo.Player2Perdeu())
                    {
                        algumJogadorPerdeu = true;
                        playerID = 2;
                    }

                    if (algumJogadorPerdeu)
                    {
                        Mensagem Perdeu = new Mensagem(mensagemStateClient.perdeste);

                        Mensagem Ganhou = new Mensagem(mensagemStateClient.ganhaste);
                        if (playerID == jogador.playerID)
                        {
                            Enviar_mensagemJson(Perdeu, jogador.JogadorStream);
                            Enviar_mensagemJson(Ganhou, oponente.JogadorStream);

                        }else if (playerID == oponente.playerID)
                        {
                            Enviar_mensagemJson(Perdeu, oponente.JogadorStream);
                            Enviar_mensagemJson(Ganhou, jogador.JogadorStream);
                        }

                    }
                    else
                    {
                        //Mensagem mandaTruePoisAindaEstamEmJogo = new Mensagem(mensagemStateClient.checkIfShipLeft);
                        //mandaTruePoisAindaEstamEmJogo.confirmacao = true;
                        //Enviar_mensagemJson(mandaTruePoisAindaEstamEmJogo, clientStream);
                        Console.WriteLine("Recebeu check the navios ainda estam em jogo");
                    }

                }
                else if (jsonObj.estadoMensagem == mensagemStateClient.battle)
                {

                }

            }
            tcpClient.Close();
        }


        void LogicadeJogo()
        {
            //while (estado == gameState.setup)
            //{
            //    while (Jogador1Budget > 50 || passTurn)
            //    {
            //        Console.WriteLine("Player 1 - Choose one ship to add to your army? \n 0 - Destroyer 1 - Cruiser 2 - Battleship 3 - Carrier");
            //        int idShip = int.Parse(Console.ReadLine());

            //        switch (idShip)
            //        {
            //            case 0:
            //                if (escolherNavio(0, shipPlayerId1))
            //                {
            //                    Console.WriteLine("Escolha posicao X");
            //                    Console.ReadLine();
            //                    Console.WriteLine("Escolha posicao Y");
            //                    Console.ReadLine();
            //                }
            //                break;
            //            case 1:
            //                if (escolherNavio(1, shipPlayerId1))
            //                {

            //                }
            //                else
            //                {
            //                    Console.WriteLine("Deseja adquerir um Destroyer? (Y/N)");
            //                    String caracter = Console.ReadLine();
            //                    if (caracter == "Y" || caracter == "y")
            //                    {
            //                        escolherNavio(0, shipPlayerId1);
            //                    }
            //                    else if (caracter == "N" || caracter == "n")
            //                    {
            //                        Console.WriteLine("Passar Turno");
            //                        passTurn = true;
            //                    }
            //                };
            //                break;
            //            case 2:
            //                shipPlayerId1++;
            //                tempShip = new Ships(typeShip.battleship, 1, shipPlayerId1);
            //                Jogador1Budget = Jogador1Budget - tempShip.Cost;
            //                gameServerShipList.Add(tempShip);
            //                Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
            //                break;
            //            case 3:
            //                shipPlayerId1++;
            //                tempShip = new Ships(typeShip.carrier, 1, shipPlayerId1);
            //                Jogador1Budget = Jogador1Budget - tempShip.Cost;
            //                gameServerShipList.Add(tempShip);
            //                Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
            //                break;
            //        }
            //    }

            //    while (Jogador2Budget > 50)
            //    {
            //        Console.WriteLine("Player - 2 Choose one ship to add to your army? \n 0 - Destroyer 1 - Cruiser 2 - Battleship 3 - Carrier");
            //        int idShip = int.Parse(Console.ReadLine());

            //        switch (idShip)
            //        {

            //            case 0:
            //                shipPlayerId2++;
            //                tempShip = new Ships(typeShip.destroyer, 2, shipPlayerId2);
            //                Jogador2Budget = Jogador2Budget - tempShip.Cost;
            //                gameServerShipList.Add(tempShip);
            //                Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
            //                break;
            //            case 1:
            //                shipPlayerId2++;
            //                tempShip = new Ships(typeShip.cruiser, 2, shipPlayerId2);
            //                Jogador2Budget = Jogador2Budget - tempShip.Cost;
            //                gameServerShipList.Add(tempShip);
            //                Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
            //                break;
            //            case 2:
            //                shipPlayerId2++;
            //                tempShip = new Ships(typeShip.battleship, 2, shipPlayerId2);
            //                Jogador2Budget = Jogador2Budget - tempShip.Cost;
            //                gameServerShipList.Add(tempShip);
            //                Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
            //                break;
            //            case 3:
            //                shipPlayerId2++;
            //                tempShip = new Ships(typeShip.carrier, 2, shipPlayerId2);
            //                Jogador2Budget = Jogador2Budget - tempShip.Cost;
            //                gameServerShipList.Add(tempShip);
            //                Console.WriteLine("Budget: " + Jogador2Budget + "Ship type: " + tempShip.getName);
            //                break;

            //        }
            //    }
            //    estado = gameState.battle;
            //}

            //if (estado == gameState.battle)
            //{
            //    int idJogador;
            //    foreach (Ships shipCollection in gameServerShipList)
            //    {
            //        idJogador = shipCollection.getShipPlayerID;
            //        if (idJogador == 1)
            //        {
            //            player1ShipList.Add(shipCollection);
            //        }
            //        else if (idJogador == 2)
            //        {
            //            player2ShipList.Add(shipCollection);
            //        }
            //    }
            //    Console.WriteLine("Imprimir Ship List player 1 ");
            //    Console.ReadLine();

            //    foreach (Ships shipCollection in player1ShipList)
            //    {
            //        Console.WriteLine("Ship ID " + shipCollection.getShipID + "Ship Type: " + shipCollection.getName + "HP: " + shipCollection.ChangeHp);
            //    }
            //    Console.WriteLine("Imprimir Ship List player 2 ");
            //    Console.ReadLine();
            //    foreach (Ships shipCollection in player2ShipList)
            //    {
            //        Console.WriteLine("Ship ID " + shipCollection.getShipID + "Ship Type: " + shipCollection.getName + "HP: " + shipCollection.ChangeHp);
            //    }
            //}
        }


        bool escolherNavio(int escolha, int playerID)
        {
            //int playerBudget = 0;
            //int oldshipPlayerId = 0;
            //int shipPlayerId = 0;

            //if (playerID == 1)
            //{
            //    playerBudget = Jogador1Budget;
            //    shipPlayerId = shipPlayerId1;
            //    oldshipPlayerId = shipPlayerId1;
            //}
            //else if (playerID == 2)
            //{
            //    playerBudget = Jogador2Budget;
            //    shipPlayerId = shipPlayerId2;
            //    oldshipPlayerId = shipPlayerId2;
            //}

            //switch (escolha)
            //{
            //    case 0:
            //        shipPlayerId1++;
            //        tempShip = new Ships(typeShip.destroyer, playerID, shipPlayerId);
            //        playerBudget = playerBudget - tempShip.Cost;
            //        gameServerShipList.Add(tempShip);
            //        Console.WriteLine("Budget: " + Jogador1Budget + "Ship type: " + tempShip.getName);
            //        return true;
            //        break;
            //    case 1:
            //        shipPlayerId1++;
            //        tempShip = new Ships(typeShip.cruiser, playerID, shipPlayerId);
            //        if (tempShip.Cost <= playerBudget)
            //        {
            //            playerBudget = playerBudget - tempShip.Cost;
            //            gameServerShipList.Add(tempShip);
            //            if (playerID == 1)
            //                Jogador1Budget = playerBudget;
            //            else if (playerID == 2)
            //                Jogador2Budget = playerBudget;
            //            Console.WriteLine("Budget: " + playerBudget + "Ship type: " + tempShip.getName);
            //            return true;
            //        }
            //        else
            //        {
            //            Console.WriteLine("Não possui os pontos necessários para escolher um " + tempShip.getName + "\n");
            //            if (playerID == 1)
            //                shipPlayerId1 = oldshipPlayerId;
            //            else if (playerID == 2)
            //                shipPlayerId2 = oldshipPlayerId;
            //            return false;
            //        }
            //        break;
            //    case 2:
            //        shipPlayerId1++;
            //        tempShip = new Ships(typeShip.cruiser, playerID, shipPlayerId);
            //        if (tempShip.Cost <= playerBudget)
            //        {
            //            playerBudget = playerBudget - tempShip.Cost;
            //            gameServerShipList.Add(tempShip);
            //            if (playerID == 1)
            //                Jogador1Budget = playerBudget;
            //            else if (playerID == 2)
            //                Jogador2Budget = playerBudget;
            //            Console.WriteLine("Budget: " + playerBudget + "Ship type: " + tempShip.getName);
            //            return true;
            //        }
            //        else
            //        {
            //            Console.WriteLine("Não possui os pontos necessários para escolher um " + tempShip.getName + "\n");
            //            if (playerID == 1)
            //                shipPlayerId1 = oldshipPlayerId;
            //            else if (playerID == 2)
            //                shipPlayerId2 = oldshipPlayerId;
            //            return false;
            //        }
            //        break;
            //    case 3:
            //        shipPlayerId1++;
            //        tempShip = new Ships(typeShip.carrier, playerID, shipPlayerId);
            //        if (tempShip.Cost <= playerBudget)
            //        {
            //            playerBudget = playerBudget - tempShip.Cost;
            //            gameServerShipList.Add(tempShip);
            //            if (playerID == 1)
            //                Jogador1Budget = playerBudget;
            //            else if (playerID == 2)
            //                Jogador2Budget = playerBudget;
            //            Console.WriteLine("Budget: " + playerBudget + "Ship type: " + tempShip.getName);
            //            return true;
            //        }
            //        else
            //        {
            //            Console.WriteLine("Não possui os pontos necessários para escolher um " + tempShip.getName + "\n");
            //            if (playerID == 1)
            //                shipPlayerId1 = oldshipPlayerId;
            //            else if (playerID == 2)
            //                shipPlayerId2 = oldshipPlayerId;
            //            return false;
            //        }
            //        break;
            //}
            return false;
            //}
        }
    }
}
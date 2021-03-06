﻿namespace WebWist
{

    public class GameProcess
    {
        private static GameProcess instance;

        private WistGame.GameManager gameManager;

        ConnectedClient[] clientByPlayerIndex = null;

        WistGame.GameChangePool workingGameChanges;

        public static GameProcess Instance
        {
            get
            {
                if (GameProcess.instance == null)
                {
                    GameProcess.instance = new GameProcess();
                }

                return GameProcess.instance;
            }
        }

        private GameProcess()
        {
        }

        public void InitializeGame(int numberOfPlayer, int numberOfTurns)
        {
            this.workingGameChanges = new WistGame.GameChangePool();
            this.gameManager = new WistGame.GameManager(numberOfPlayer, numberOfTurns);

            this.clientByPlayerIndex = new ConnectedClient[numberOfPlayer];
            for (int index = 0; index < this.clientByPlayerIndex.Length; ++index)
            {
                this.clientByPlayerIndex[index] = null;
            }
        }

        public WistGame.GameManager GetGameManager()
        {
            return this.gameManager;
        }

        public bool TryRegisterClient(ConnectedClient client, int requestedIndex)
        {
            if (requestedIndex < 0 || requestedIndex >= this.clientByPlayerIndex.Length)
            {
                return false;
            }

            if (client == null)
            {
                return false;
            }

            for (int index = 0; index < this.clientByPlayerIndex.Length; ++index)
            {
                if (this.clientByPlayerIndex[index] == client)
                {
                    return false;
                }
            }

            if (this.clientByPlayerIndex[requestedIndex] != null)
            {
                return false;
            }

            this.clientByPlayerIndex[requestedIndex] = client;
            client.PlayerIndex = requestedIndex;
            return true;
        }

        public bool TryUnregisterClient(ConnectedClient client)
        {
            for (int index = 0; index < this.clientByPlayerIndex.Length; ++index)
            {
                if (this.clientByPlayerIndex[index] == client)
                {
                    this.clientByPlayerIndex[index].PlayerIndex = -1;
                    this.clientByPlayerIndex[index] = null;
                    return true;
                }
            }

            return false;
        }

        public PlayerViewUpdate GetPlayerView(int playerIndex)
        {
            WistGame.Sandbox sandbox = this.gameManager.GetSandbox();
            WistGame.Player player = sandbox.Players[playerIndex];
            PlayerViewUpdate view = new PlayerViewUpdate();
            view.PlayerIndex = playerIndex;
            view.Hand = player.Hand.ToArray();
            view.Bet = player.Bet;
            view.Score = player.Score;
            view.GameStateID = this.gameManager.GetStateID();
            view.CurrentPlayer = sandbox.CurrentPlayer;
            view.TrumpCard = sandbox.TrumpCard;

            if (view.GameStateID == WistGame.GameStateID.Betting)
            {
                view.BetFailures = this.gameManager.GetBetFailures(playerIndex);
            }

            view.OtherPlayers = new PlayerViewUpdate.Player[sandbox.NumberOfPlayers];
            for (int otherIndex = 0; otherIndex < view.OtherPlayers.Length; ++otherIndex)
            {
                ref PlayerViewUpdate.Player otherPlayer = ref view.OtherPlayers[otherIndex];
                otherPlayer.NumberOfCards = sandbox.Players[otherIndex].Hand.Count;
                otherPlayer.CurrentScore = sandbox.Players[otherIndex].Score;
                otherPlayer.Bet = sandbox.Players[otherIndex].Bet;
            }

            return view;
        }

        public string GetSandboxJson()
        {
            string result = string.Empty;

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter textWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter);

            PlayerViewUpdate playerView = this.GetPlayerView(0);

            serializer.Serialize(textWriter, playerView);
            stringWriter.Close();
            result = stringWriter.ToString();
            return result;
        }

        public void HandleMessage(ConnectedClient client, string messageString)
        {
            System.IO.StringReader stringReader = new System.IO.StringReader(messageString);
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            JSONOrder order = null;
            try
            {
                order = (JSONOrder)serializer.Deserialize(stringReader, typeof(JSONOrder));
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while parsing socket message");
                Program.ReportException(ex);
                return;
            }

            if (order == null)
            {
                System.Console.WriteLine("Error while parsing socket message into json");
                return;
            }

            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter textWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter);

            switch (order.OrderType)
            {
                case "SelectPlayerSlot":
                    {
                        if (order.PlayerIndex < 0)
                        {
                            System.Console.WriteLine("Missing PlayerIndex");
                            return;
                        }
                        
                        if (this.TryRegisterClient(client, order.PlayerIndex))
                        {
                            OrderAcknowledgement acknowledgement = new OrderAcknowledgement() { OrderID = order.OrderID, FailureFlags = WistGame.Failures.None };
                            this.SendResponseToClient(acknowledgement, client);

                            PlayerViewUpdate playerView = this.GetPlayerView(client.PlayerIndex);
                            this.SendResponseToClient(playerView, client);
                        }
                        else
                        {
                            System.Console.WriteLine($"Couldn't register to player slot {order.PlayerIndex}");
                        }

                        JSONResponse availableSlots = this.RequestAvailablePlayerSlots();
                        this.BroadCast(availableSlots);

                        break;
                    }
                case "RequestPlayerSlots":
                    {
                        JSONResponse response = this.RequestAvailablePlayerSlots();
                        this.SendResponseToClient(response, client);

                        break;
                    }

                case "PlaceBet":
                    {
                        int playerIndex = client.PlayerIndex;
                        int betValue = order.BetValue;

                        WistGame.PlaceBetOrder betOrder = new WistGame.PlaceBetOrder()
                        {
                            PlayerIndex = playerIndex,
                            BetValue = betValue,
                        };

                        this.workingGameChanges.Clear();
                        WistGame.Failures failures = this.gameManager.ProcessOrder(betOrder, this.workingGameChanges);

                        OrderAcknowledgement acknowledgement = new OrderAcknowledgement()
                        {
                            OrderID = order.OrderID,
                            FailureFlags = failures,
                        };

                        this.SendResponseToClient(acknowledgement, client);

                        if (failures == WistGame.Failures.None)
                        {
                            SandboxChanges sandboxChanges = new SandboxChanges();
                            sandboxChanges.GameChanges = this.workingGameChanges.GetGameChanges();
                            this.BroadCast(sandboxChanges);
                        }

                        break;
                    }
                case "PlayCard":
                    {
                        int playerIndex = client.PlayerIndex;
                        int cardIndex = order.CardIndex;

                        WistGame.PlayCardOrder playOrder = new WistGame.PlayCardOrder()
                        {
                            PlayerIndex = playerIndex,
                            CardIndex = cardIndex,
                        };

                        this.workingGameChanges.Clear();
                        WistGame.Failures failures = this.gameManager.ProcessOrder(playOrder, this.workingGameChanges);

                        OrderAcknowledgement acknowledgement = new OrderAcknowledgement()
                        {
                            OrderID = order.OrderID,
                            FailureFlags = failures,
                        };

                        this.SendResponseToClient(acknowledgement, client);

                        if (failures == WistGame.Failures.None)
                        {
                            SandboxChanges sandboxChanges = new SandboxChanges()
                            {
                                GameChanges = this.workingGameChanges.GetGameChanges(),
                            };

                            this.AddPlayerViewIfNecessary(sandboxChanges, client.PlayerIndex);

                            this.BroadCast(sandboxChanges);
                        }

                        break;
                    }

                default:
                    {
                        System.Console.WriteLine($"Unkown orderType : {order.OrderType}.");
                        return;
                    }
            }
        }

        private void AddPlayerViewIfNecessary(SandboxChanges sandboxChanges, int playerIndex)
        {
            for (int index = 0; index < sandboxChanges.GameChanges.Length; ++index)
            {
                if (sandboxChanges.GameChanges[index].ChangeType == WistGame.GameChange.GameChangeType.GameStateChange &&
                    sandboxChanges.GameChanges[index].GameState == WistGame.GameStateID.Initialize)
                {
                    sandboxChanges.PlayerViewUpdate = this.GetPlayerView(playerIndex);
                    return;
                }
            }
        }

        private JSONResponse RequestAvailablePlayerSlots()
        {
            AvailablePlayerSlot response = new AvailablePlayerSlot();
            response.AvaialablePlayerSlots = new bool[this.clientByPlayerIndex.Length];
            for (int index = 0; index < this.clientByPlayerIndex.Length; ++index)
            {
                response.AvaialablePlayerSlots[index] = this.clientByPlayerIndex[index] == null;
            }

            return response;
        }

        private void SendResponseToClient(JSONResponse response, ConnectedClient client)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter textWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter);
            serializer.Serialize(textWriter, response);
            stringWriter.Close();
            string message = stringWriter.ToString();
            System.Console.WriteLine($"Sending message \"{message}\".");
            client.MessageQueue.Add(message);
        }

        private void BroadCast(JSONResponse response)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter textWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter);
            serializer.Serialize(textWriter, response);
            stringWriter.Close();
            string message = stringWriter.ToString();
            WebSocketMiddleware.Broadcast(message);
        }
    }
}

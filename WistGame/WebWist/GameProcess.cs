namespace WebWist
{
    public class GameProcess
    {
        private static GameProcess instance;

        private WistGame.GameManager gameManager;

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
            this.gameManager = new WistGame.GameManager(numberOfPlayer, numberOfTurns);
        }

        public WistGame.GameManager GetGameManager()
        {
            return this.gameManager;
        }

        public WistGame.Failures PostStringOrder(string input)
        {
            string[] splitted = input.Split(' ');
            WistGame.GameOrder order = TryParseGameOrder(splitted);
            if (order != null)
            {
                WistGame.Failures failure = gameManager.ProcessOrder(order);
                System.Console.WriteLine(failure.ToString());
                return failure;
            }

            return WistGame.Failures.Unknown;
        }

        public byte[] GetSerializePlayer(int playerIndex)
        {
            WistGame.Sandbox sandbox = this.gameManager.GetSandbox();
            int numberOfPlayers = sandbox.NumberOfPlayers;

            FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);

            WistGame.Card trumpCard = sandbox.TrumpCard;
            FlatBuffers.Offset<Serialization.Card> trumpOffset = Serialization.Card.CreateCard(builder, (Serialization.Sigil)trumpCard.Sigil, trumpCard.Value);

            int cardsInHand = sandbox.Players[playerIndex].Hand.Count;
            FlatBuffers.Offset<Serialization.Card>[] handCardOffsets = new FlatBuffers.Offset<Serialization.Card>[cardsInHand];
            for (int cardIndex = 0; cardIndex < cardsInHand; ++cardIndex)
            {
                WistGame.Card card = sandbox.Players[playerIndex].Hand[cardIndex];
                handCardOffsets[cardIndex] = Serialization.Card.CreateCard(builder, (Serialization.Sigil)card.Sigil, card.Value);
            }

            var playerHandOffest = Serialization.PlayerSandbox.CreatePlayerHandVector(builder, handCardOffsets);

            int[] placedBets = new int[numberOfPlayers];
            int[] selectedCards = new int[numberOfPlayers];
            int[] numberOfCardsPerPlayers = new int[numberOfPlayers];
            for (int index = 0; index < numberOfPlayers; ++index)
            {
                placedBets[index] = sandbox.Players[index].Bet;
                selectedCards[index] = sandbox.Players[index].SelectedCard;
                numberOfCardsPerPlayers[index] = sandbox.Players[index].Hand.Count;
            }

            var placedBetOffset = Serialization.PlayerSandbox.CreatePlacedBetsVectorBlock(builder, placedBets);
            var selectedCardOffset = Serialization.PlayerSandbox.CreatePlayedCardsVectorBlock(builder, selectedCards);
            var numberOfCardsPerPlayerOffset = Serialization.PlayerSandbox.CreateNumberOfCardPerPlayersVectorBlock(builder, numberOfCardsPerPlayers);

            Serialization.PlayerSandbox.StartPlayerSandbox(builder);
            Serialization.PlayerSandbox.AddPlayerIndex(builder, playerIndex);
            Serialization.PlayerSandbox.AddNumberOfPlayers(builder, sandbox.Players.Length);
            Serialization.PlayerSandbox.AddMaxHandSize(builder, sandbox.MaxHandSize);
            Serialization.PlayerSandbox.AddCurrentTurn(builder, sandbox.CurrentTurn);
            Serialization.PlayerSandbox.AddCurrentState(builder, (Serialization.State)this.gameManager.GetStateID());
            Serialization.PlayerSandbox.AddCurrentPlayer(builder, sandbox.CurrentPlayer);

            builder.AddInt(sandbox.CurrentPlayer);
            Serialization.PlayerSandbox.AddTrumpCard(builder, trumpOffset);

            Serialization.PlayerSandbox.AddFirstFoldPlayer(builder, sandbox.FirstFoldPlayer);

            Serialization.PlayerSandbox.AddPlacedBets(builder, placedBetOffset);
            Serialization.PlayerSandbox.AddPlayedCards(builder, selectedCardOffset);
            Serialization.PlayerSandbox.AddNumberOfCardPerPlayers(builder, numberOfCardsPerPlayerOffset);

            Serialization.PlayerSandbox.AddPlayerHand(builder, playerHandOffest);

            Serialization.PlayerSandbox.EndPlayerSandbox(builder);
            return builder.SizedByteArray();
        }

        private WistGame.GameOrder TryParseGameOrder(string[] input)
        {
            if (input.Length < 4)
            {
                return null;
            }

            WistGame.GameOrder order = null;

            string stringOrder = input[2].Trim().ToLower();

            if (stringOrder == "bet")
            {
                int playerIndex;
                int betValue;
                if (!int.TryParse(input[1], out playerIndex))
                {
                    return null;
                }

                if (!int.TryParse(input[3], out betValue))
                {
                    return null;
                }

                order = new WistGame.PlaceBetOrder()
                {
                    PlayerIndex = playerIndex,
                    Bet = betValue,
                };
            }
            else if (stringOrder == "play")
            {
                int playerIndex;
                int cardIndex;
                if (!int.TryParse(input[1], out playerIndex))
                {
                    return null;
                }

                if (!int.TryParse(input[3], out cardIndex))
                {
                    return null;
                }

                order = new WistGame.PlayCardOrder()
                {
                    PlayerIndex = playerIndex,
                    CardIndex = cardIndex,
                };
            }


            return order;
        }
    }
}

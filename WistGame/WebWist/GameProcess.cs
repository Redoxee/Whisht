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

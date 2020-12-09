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
    }
}

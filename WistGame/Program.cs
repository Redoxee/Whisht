using System;

namespace WistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager gameManager = new GameManager(numberOfPlayers: 3, maxHandSize: 5);

            GameStateMachine stateMachine = new GameStateMachine();
            GameState firstState = new InitializeGameState();
            stateMachine.SetInitialState(firstState);

            bool quit = false;
            do
            {
                System.Console.WriteLine(stateMachine.GetDebugString());

                string line = System.Console.ReadLine();
                string[] splitted = line.Split(' ');

                if (splitted.Length == 0)
                {
                    continue;
                }

                GameOrder order = TryParseGameOrder(splitted);
                if (order != null)
                {
                    Failures failure = stateMachine.ProcessOrder(order);
                    System.Console.WriteLine(failure.ToString());
                }
                
                if (splitted[0].Trim().ToLower() == "quit")
                {
                    quit = true;
                }

            } while (!quit);
        }

        private static GameOrder TryParseGameOrder(string[] input)
        {
            if (input.Length < 4)
            {
                return null;
            }

            GameOrder order = null;

            if (input[2].Trim().ToLower() == "bet")
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

                order = new PlaceBetOrder()
                {
                    PlayerIndex = playerIndex,
                    Bet = betValue,
                };
            }

            return order;
        }
    }
}

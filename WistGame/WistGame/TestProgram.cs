﻿using System;

namespace WistGame
{
    class TestProgram
    {
        static void Main(string[] args)
        {
            GameChangePool gameChanges = new GameChangePool();
            GameManager gameManager = new GameManager(numberOfPlayers: 2, numberOfTurns: 3, gameChanges);

            bool quit = false;
            do
            {
                System.Console.WriteLine(gameManager.GetDebugString());

                string line = System.Console.ReadLine();
                string[] splitted = line.Split(' ');

                if (splitted.Length == 0)
                {
                    continue;
                }

                GameOrder order = TryParseGameOrder(splitted);
                if (order != null)
                {
                    Failures failure = gameManager.ProcessOrder(order, gameChanges);
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

                order = new PlaceBetOrder()
                {
                    PlayerIndex = playerIndex,
                    BetValue = betValue,
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

                order = new PlayCardOrder()
                {
                    PlayerIndex = playerIndex,
                    CardIndex = cardIndex,
                };
            }
            

            return order;
        }
    }
}

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

            stateMachine.ProcessOrder(null);
        }
    }
}

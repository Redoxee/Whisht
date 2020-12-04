using System;

namespace WistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager gameManager = new GameManager(numberOfPlayers: 3, maxHandSize: 5);

            GameStateMachine stateMachine = new GameStateMachine();
            GameState firstTurnState = new SetupTurn(stateMachine);
            stateMachine.SetInitialState(firstTurnState); ;

            stateMachine.ProcessOrder(null);

           }
    }
}

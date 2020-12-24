using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    public class GameManager
    {
        internal Sandbox Sandbox;

        internal GameStateMachine stateMachine;

        public GameManager(int numberOfPlayers, int numberOfTurns)
        {
            int maxHandSize = numberOfTurns / 2 + 1;

            this.Sandbox = new Sandbox();
            this.Sandbox.Deck = new Deck();
            this.Sandbox.Players = new Player[numberOfPlayers];
            this.Sandbox.MaxHandSize = maxHandSize;

            for (int index = 0; index < numberOfPlayers; ++index)
            {
                this.Sandbox.Players[index] = new Player(index);
            }

            this.Sandbox.CurrentPlayer = 0;
            this.Sandbox.CurrentTurn = 0;

            this.stateMachine = new GameStateMachine(this);
            GameState firstState = new InitializeGameState();
            this.stateMachine.SetInitialState(firstState);
        }

        public bool IsGameFinished()
        {
            return false;
        }

        public Sandbox GetSandbox()
        {
            return this.Sandbox;
        }

        public Failures ProcessOrder(GameOrder order)
        {
            return this.stateMachine.ProcessOrder(order);
        }

        public GameStateID GetStateID()
        {
            return this.stateMachine.GetStateID();
        }

        public Failures[] GetBetFailures(int playerIndex)
        {
            if (this.stateMachine.GetStateID() != GameStateID.Betting)
            {
                System.Console.WriteLine("Querying bet failures in the wrong state");
                return null;
            }

            return this.Sandbox.Players[playerIndex].Failures;
        }

        public string GetDebugString()
        {
            return this.stateMachine.GetDebugString();
        }
    }
}

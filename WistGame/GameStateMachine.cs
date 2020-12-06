using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    internal class GameStateMachine
    {
        private GameState currentState = null;
        private GameState nextGameState = null;
        
        public void ProcessOrder(GameOrder order)
        {
            System.Console.WriteLine($"[GameStateMachine] processing order {(order != null ? order.ToString() : "null")}.");
            if (this.currentState != null)
            {
                this.currentState.ProcessOrder(this, order);
            }

            this.ResolveNextState();
        }
        public void SetNextState(GameState nextState)
        {
            this.nextGameState = nextState;
        }

        public void SetInitialState(GameState initialState)
        {
            System.Console.WriteLine($"[GameStateMachine] Initial state {initialState.GetType().Name}.");
            this.nextGameState = initialState;
            this.ResolveNextState();
        }

        private void ResolveNextState()
        {
            int loopCounter = 0;
            const int tooManyLoop = 64;
            while (this.nextGameState != null && loopCounter < tooManyLoop)
            {
                GameState next = this.nextGameState;
                System.Console.WriteLine($"[GameStateMachine] Transitioning from state {(this.currentState != null ? this.currentState.ToString() : "null")} to state {next.GetType().Name}.");
                this.nextGameState = null;
                this.currentState = next;
                this.currentState.StartState(this);

                loopCounter++;
            }

            if (loopCounter >= tooManyLoop)
            {
                System.Console.Error.WriteLine($"[GameStateMachine] Too many loop while resolving states!");
            }
        }
    }

    internal abstract class GameState
    {
        public abstract void StartState(GameStateMachine stateMachine);

        public abstract void ProcessOrder(GameStateMachine stateMachine, GameOrder order);
    }
}

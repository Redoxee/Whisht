using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    public class GameStateMachine
    {
        private GameState currentState = null;
        private GameState nextGameState = null;
        
        public void ProcessOrder(GameOrder order)
        {
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
            this.nextGameState = initialState;
            this.ResolveNextState();
        }

        private void ResolveNextState()
        {
            while (this.nextGameState != null)
            {
                GameState next = this.nextGameState;
                this.nextGameState = null;
                this.currentState = next;
                this.currentState.StartState(this);
            }
        }
    }

    public abstract class GameState
    {
        public abstract void StartState(GameStateMachine stateMachine);

        public abstract void ProcessOrder(GameStateMachine stateMachine, GameOrder order);
    }
}

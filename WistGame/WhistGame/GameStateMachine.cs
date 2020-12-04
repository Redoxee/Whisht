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
                this.currentState.StartState();
            }
        }
    }

    public abstract class GameState
    {
        protected GameStateMachine StateMachine = null;

        public GameState(GameStateMachine stateMachine)
        {
            this.StateMachine = stateMachine;
        }

        public abstract void StartState();

        public abstract void ProcessOrder(GameStateMachine context, GameOrder order);
    }
}

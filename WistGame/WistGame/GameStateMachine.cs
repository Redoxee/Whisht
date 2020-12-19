using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    internal class GameStateMachine
    {
        internal GameManager gameManager;

        private GameState currentState = null;
        private GameState nextGameState = null;

        public GameStateMachine(GameManager manager)
        {
            this.gameManager = manager;
        }

        public Failures ProcessOrder(GameOrder order)
        {
            Failures failure = Failures.None;
            System.Console.WriteLine($"[GameStateMachine] processing order {(order != null ? order.ToString() : "null")}.");
            if (this.currentState != null)
            {
                failure |= this.currentState.ProcessOrder(this, order);
            }

            this.ResolveNextState();

            return failure;
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

        public StateID GetStateID()
        {
            if (this.currentState != null)
            {
                return this.currentState.StateID;
            }

            return StateID.Unkown;
        }

        public string GetDebugString()
        {
            if (this.currentState != null)
            {
                return this.currentState.GetDebugMessage(this);
            }

            return "No current state";
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

        public abstract Failures ProcessOrder(GameStateMachine stateMachine, GameOrder order);

        public abstract StateID StateID
        {
            get;
        }

        public virtual string GetDebugMessage(GameStateMachine stateMachine)
        {
            return this.GetType().Name;
        }
    }
}

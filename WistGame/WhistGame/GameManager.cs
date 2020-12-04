using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    class GameManager
    {
        public static GameManager Instance;

        public Sandbox Sandbox;

        public GameManager(int numberOfPlayers, int maxHandSize)
        {
            GameManager.Instance = this;
            
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
        }

        public bool IsGameFinished()
        {
            return false;
        }
    }
}

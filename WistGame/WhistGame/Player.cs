using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    class Player
    {
        public readonly int Index;

        public List<Card> Hand = new List<Card>();
        public int Bet;
        public int Score;

        public int SelectedCard;

        public Player(int index)
        {
            this.Index = index;
        }
    }
}

namespace WistGame
{
    class Sandbox
    {
        public Deck Deck;
        public Player[] Players;
        public Card FamillyCard;

        public int MaxHandSize;

        public int CurrentTurn;
        public int CurrentPlayer;

        public int NumberOfTurns 
        {
            get 
            {
                return this.MaxHandSize * 2 - 1;
            }
        }

        public int GetHandSizeForTurn(int turn)
        {
            if (turn < this.MaxHandSize)
            {
                return turn + 1;
            }
            else
            {
                return this.MaxHandSize + (this.MaxHandSize - (turn + 1));
            }
        }
    }
}

namespace WistGame
{
    public class Sandbox
    {
        public Deck Deck;
        public Player[] Players;
        public Card TrumpCard;
        public Card AskedCard;

        public PlayedCard[] PlayedCards;

        public int MaxHandSize;

        public int RemainingFoldInTurn;
        public int FirstFoldPlayer;

        public int CurrentTurn;
        public int CurrentPlayer;

        public int NumberOfTurns 
        {
            get 
            {
                return this.MaxHandSize * 2 - 1;
            }
        }

        public int NumberOfPlayers
        {
            get => this.Players.Length;
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

        public int GetCurrentHandSize()
        {
            return this.GetHandSizeForTurn(this.CurrentTurn);
        }
    }
}

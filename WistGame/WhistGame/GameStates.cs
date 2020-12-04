namespace WistGame
{
    public class SetupTurn : GameState
    {
        public SetupTurn(GameStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void StartState()
        {
            GameManager game = GameManager.Instance;
            game.Sandbox.Deck.RefillDeck();
            game.Sandbox.Deck.Shuffle();

            int handSize = game.Sandbox.GetHandSizeForTurn(game.Sandbox.CurrentTurn);

            for (int playerIndex = 0; playerIndex < game.Sandbox.Players.Length; ++playerIndex)
            {
                Player player = game.Sandbox.Players[playerIndex];
                player.Hand.Clear();
                player.SelectedCard = -1;

                for (int cardIndex = 0; cardIndex < handSize; ++cardIndex)
                {
                    player.Hand.Add(game.Sandbox.Deck.PickCard());
                }
            }

            game.Sandbox.FamillyCard = game.Sandbox.Deck.PickCard();

            this.StateMachine.SetNextState(new PlaceBet(this.StateMachine));
        }

        public override void ProcessOrder(GameStateMachine context, GameOrder order)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PlaceBet : GameState
    {
        private int nextBettingPlayer = 0;
        private int lastBettingPlayer = 0;

        public PlaceBet(GameStateMachine stateMachine) : base(stateMachine)
        { 
        }

        public override void StartState()
        {
            GameManager game = GameManager.Instance;

            this.nextBettingPlayer = 0;
            this.lastBettingPlayer = game.Sandbox.Players.Length - 1;
            
            for (int index = 0; index < game.Sandbox.Players.Length; ++index)
            {
                game.Sandbox.Players[index].Bet = 0;
            }
        }

        public override void ProcessOrder(GameStateMachine context, GameOrder order)
        {
            PlaceBetOrder betOrder = order as PlaceBetOrder;
            if (betOrder == null)
            {
                return;
            }

            GameManager game = GameManager.Instance;

            if (betOrder.Bet > game.CurrentHandSize)
            {
                return;
            }

            if (this.nextBettingPlayer == this.lastBettingPlayer)
            {
                int betCummul = 0;
                for (int index = 0; index < game.Players.Length; ++index)
                {
                    betCummul += game.Players[index].Contract;
                }

                int forbidenCummul = game.CurrentHandSize * game.Players.Length;
                if (betCummul + betOrder.Bet == forbidenCummul)
                {
                    return;
                }
            }

            game.Players[this.nextBettingPlayer].Contract = betOrder.Bet;
            this.nextBettingPlayer = (this.nextBettingPlayer + 1) % game.Players.Length;

            if (this.nextBettingPlayer == this.lastBettingPlayer)
            {
                context.nextGameState = new CardRoundState();
            }
        }

        public class PlaceBetOrder : GameOrder
        {
            public int Bet;
        }

        public class CardRoundState : GameState
        {
            public override void ProcessOrder(GameStateMachine context, GameOrder order)
            {
            }
        }
    }
}

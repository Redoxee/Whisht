namespace WistGame
{
    public class InitializeTurnState : GameState
    {
        public override void StartState(GameStateMachine stateMachine)
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
                player.Bet = -1;

                for (int cardIndex = 0; cardIndex < handSize; ++cardIndex)
                {
                    player.Hand.Add(game.Sandbox.Deck.PickCard());
                }
            }

            game.Sandbox.FamillyCard = game.Sandbox.Deck.PickCard();

            stateMachine.SetNextState(new BettingState());
        }

        public override void ProcessOrder(GameStateMachine context, GameOrder order)
        {
            throw new System.NotImplementedException();
        }
    }

    public class BettingState : GameState
    {
        private int lastBettingPlayer = 0;

        public override void StartState(GameStateMachine stateMachine)
        {
            Sandbox sandbox = GameManager.Instance.Sandbox;

            sandbox.CurrentPlayer = 0;
            this.lastBettingPlayer = sandbox.Players.Length - 1;
            
            for (int index = 0; index < sandbox.Players.Length; ++index)
            {
                sandbox.Players[index].Bet = 0;
            }
        }

        public override void ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
            PlaceBetOrder betOrder = order as PlaceBetOrder;
            if (betOrder == null)
            {
                return;
            }

            Sandbox sandbox = GameManager.Instance.Sandbox;

            if (sandbox.CurrentPlayer != betOrder.PlayerIndex)
            {
                return;
            }

            int handSize = sandbox.GetCurrentHandSize();
            if (betOrder.Bet < 0 || betOrder.Bet >= handSize)
            {
                return;
            }

            if (sandbox.CurrentPlayer == this.lastBettingPlayer)
            {
                int forbidenBet = this.GetForbidenBet();
                if (betOrder.Bet == forbidenBet)
                {
                    return;
                }
            }

            sandbox.Players[sandbox.CurrentPlayer].Bet = betOrder.Bet;
            sandbox.CurrentPlayer++;

            if (sandbox.CurrentPlayer >= sandbox.Players.Length)
            {
                stateMachine.SetNextState(new FoldState());
            }

        }

        private int GetForbidenBet()
        {
            Sandbox sandbox = GameManager.Instance.Sandbox;
            int handSize = sandbox.GetCurrentHandSize();
            int betCummul = 0;
            for (int index = 0; index < this.lastBettingPlayer; ++index)
            {
                betCummul += sandbox.Players[index].Bet;
            }

            int forbidenCummul = handSize * sandbox.Players.Length;

            return forbidenCummul - betCummul;
        }
    }

    public class PlaceBetOrder : GameOrder
    {
        public int PlayerIndex;
        public int Bet;
    }

    public class FoldState : GameState
    {
        public override void StartState(GameStateMachine stateMachine)
        {
            Sandbox sandbox = GameManager.Instance.Sandbox;
            sandbox.CurrentPlayer = 0;

            for (int index = 0; index < sandbox.Players.Length; ++index)
            {
                Player player = sandbox.Players[index];
                player.SelectedCard = -1;

                int handSize = player.Hand.Count;
                player.Failures = new Failures[handSize];
                for (int cardIndex = 0; cardIndex < handSize; ++cardIndex)
                {
                    player.Failures[cardIndex] = Failures.None;
                }
            }
        }

        public override void ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
            PlayCardOrder playCardOrder = order as PlayCardOrder;
            if (playCardOrder == null)
            {
                return;
            }

            Sandbox sandbox = GameManager.Instance.Sandbox;

            if (playCardOrder.PlayerIndex != sandbox.CurrentPlayer)
            {
                return;
            }

            Player player = sandbox.Players[playCardOrder.PlayerIndex];

            if (playCardOrder.CardIndex < 0 || playCardOrder.CardIndex >= player.Hand.Count)
            {
                return;
            }

            if (player.Failures[playCardOrder.CardIndex] != Failures.None)
            {
                return;
            }

            ref PlayedCard playedCard = ref sandbox.PlayedCards[player.Index];
            
            playedCard.PlayerIndex = player.Index;
            playedCard.Card = player.Hand[playCardOrder.CardIndex];
            player.Hand.RemoveAt(playCardOrder.CardIndex);

            if (sandbox.CurrentPlayer == 0)
            {
                sandbox.RequieredCard = playedCard.Card;

                for (int playerIndex = 0; playerIndex < sandbox.Players.Length; ++playerIndex)
                {
                    bool hasRequieredColor = false;
                    bool hasFamilyColor = false;
                    for (int cardIndex = 0; cardIndex < sandbox.Players[playerIndex].Hand.Count; ++cardIndex)
                    {
                        Sigil cardColor = sandbox.Players[playerIndex].Hand[cardIndex].Sigil;
                        hasRequieredColor |= cardColor == sandbox.RequieredCard.Sigil;
                        hasFamilyColor |= cardColor == sandbox.FamillyCard.Sigil;
                    }

                    for (int cardIndex = 0; cardIndex < sandbox.Players[playerIndex].Hand.Count; ++cardIndex)
                    {
                        Sigil cardColor = sandbox.Players[playerIndex].Hand[cardIndex].Sigil;
                        if (hasRequieredColor && cardColor != sandbox.RequieredCard.Sigil)
                        {
                            sandbox.Players[playerIndex].Failures[cardIndex] |= Failures.ColorCardAvailable;
                        }

                        if (hasFamilyColor && cardColor != sandbox.FamillyCard.Sigil)
                        {
                            sandbox.Players[playerIndex].Failures[cardIndex] |= Failures.FamilyCardAvailable;
                        }
                    }
                }
            }

            sandbox.CurrentPlayer++;

            if (sandbox.CurrentPlayer == sandbox.Players.Length)
            {
                stateMachine.SetNextState(new ResolveFoldState());
            }
        }

    }

    public class PlayCardOrder : GameOrder 
    {
        public int PlayerIndex;
        public int CardIndex;
    }

    public class ResolveFoldState : GameState
    {
        public override void StartState(GameStateMachine stateMachine)
        {
            
        }
     
        public override void ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
        }
    }
}

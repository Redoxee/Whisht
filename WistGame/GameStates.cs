namespace WistGame
{
    internal class InitializeGameState : GameState
    {
        public override void StartState(GameStateMachine stateMachine)
        {
            stateMachine.SetNextState(new InitializeTurnState());
        }

        public override Failures ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
            return Failures.WrongOrder;
        }
    }

    internal class InitializeTurnState : GameState
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

            game.Sandbox.TrumpCard = game.Sandbox.Deck.PickCard();

            stateMachine.SetNextState(new BettingState());
        }

        public override Failures ProcessOrder(GameStateMachine context, GameOrder order)
        {
            return Failures.WrongOrder;
        }
    }

    internal class BettingState : GameState
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

        public override Failures ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
            PlaceBetOrder betOrder = order as PlaceBetOrder;
            if (betOrder == null)
            {
                return Failures.WrongOrder;
            }

            Sandbox sandbox = GameManager.Instance.Sandbox;

            if (sandbox.CurrentPlayer != betOrder.PlayerIndex)
            {
                return Failures.WrongPlayer;
            }

            int handSize = sandbox.GetCurrentHandSize();
            if (betOrder.Bet < 0 || betOrder.Bet > handSize)
            {
                return Failures.BetOutOfBounds;
            }

            if (sandbox.CurrentPlayer == this.lastBettingPlayer)
            {
                int forbidenBet = this.GetForbidenBet();
                if (betOrder.Bet == forbidenBet)
                {
                    return Failures.BetValueForbiden;
                }
            }

            sandbox.Players[sandbox.CurrentPlayer].Bet = betOrder.Bet;
            sandbox.CurrentPlayer++;

            if (sandbox.CurrentPlayer >= sandbox.Players.Length)
            {
                stateMachine.SetNextState(new FoldState());
            }

            return Failures.None;
        }

        public override string GetDebugMessage()
        {
            Sandbox sandbox = GameManager.Instance.Sandbox;

            return $"{this.GetType().Name} - Waiting for bet order from player {sandbox.CurrentPlayer}, Hand : [{string.Join(",", sandbox.Players[sandbox.CurrentPlayer].Hand)}].";
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

    internal class PlaceBetOrder : GameOrder
    {
        public int PlayerIndex;
        public int Bet;
    }

    internal class FoldState : GameState
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

        public override Failures ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
            PlayCardOrder playCardOrder = order as PlayCardOrder;
            if (playCardOrder == null)
            {
                return Failures.WrongOrder;
            }

            Sandbox sandbox = GameManager.Instance.Sandbox;

            if (playCardOrder.PlayerIndex != sandbox.CurrentPlayer)
            {
                return Failures.WrongPlayer;
            }

            Player player = sandbox.Players[playCardOrder.PlayerIndex];

            if (playCardOrder.CardIndex < 0 || playCardOrder.CardIndex >= player.Hand.Count)
            {
                return Failures.CardOutOfBounds;
            }

            if (player.Failures[playCardOrder.CardIndex] != Failures.None)
            {
                return player.Failures[playCardOrder.CardIndex];
            }

            ref PlayedCard playedCard = ref sandbox.PlayedCards[player.Index];
            
            playedCard.PlayerIndex = player.Index;
            playedCard.Card = player.Hand[playCardOrder.CardIndex];
            player.Hand.RemoveAt(playCardOrder.CardIndex);

            if (sandbox.CurrentPlayer == 0)
            {
                sandbox.AskedCard = playedCard.Card;

                for (int playerIndex = 0; playerIndex < sandbox.Players.Length; ++playerIndex)
                {
                    bool hasRequieredColor = false;
                    bool hasFamilyColor = false;
                    for (int cardIndex = 0; cardIndex < sandbox.Players[playerIndex].Hand.Count; ++cardIndex)
                    {
                        Sigil cardColor = sandbox.Players[playerIndex].Hand[cardIndex].Sigil;
                        hasRequieredColor |= cardColor == sandbox.AskedCard.Sigil;
                        hasFamilyColor |= cardColor == sandbox.TrumpCard.Sigil;
                    }

                    for (int cardIndex = 0; cardIndex < sandbox.Players[playerIndex].Hand.Count; ++cardIndex)
                    {
                        Sigil cardColor = sandbox.Players[playerIndex].Hand[cardIndex].Sigil;
                        if (hasRequieredColor && cardColor != sandbox.AskedCard.Sigil)
                        {
                            sandbox.Players[playerIndex].Failures[cardIndex] |= Failures.ColorCardAvailable;
                        }

                        if (hasFamilyColor && cardColor != sandbox.TrumpCard.Sigil)
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

            return Failures.None;
        }

    }

    internal class PlayCardOrder : GameOrder 
    {
        public int PlayerIndex;
        public int CardIndex;
    }

    internal class ResolveFoldState : GameState
    {
        public override void StartState(GameStateMachine stateMachine)
        {
            Sandbox sandbox = GameManager.Instance.Sandbox;

            ref PlayedCard foldWinner = ref sandbox.PlayedCards[0];
            bool isCorrectFamily = foldWinner.Card.Sigil == sandbox.AskedCard.Sigil;
            bool isTrumpFamily = foldWinner.Card.Sigil == sandbox.TrumpCard.Sigil;

            for (int index = 1; index < sandbox.PlayedCards.Length; ++index)
            {
                ref PlayedCard otherCard = ref sandbox.PlayedCards[index];
                bool isOtherCorrectFamily = otherCard.Card.Sigil == sandbox.AskedCard.Sigil;
                bool isOtherTrumpFamily = otherCard.Card.Sigil == sandbox.TrumpCard.Sigil;

                bool isStronger = false;

                if (isOtherTrumpFamily != isTrumpFamily)
                {
                    if (isTrumpFamily)
                    {
                        continue;
                    }
                    else
                    {
                        isStronger = true;
                    }
                }

                if (!isStronger && (isOtherCorrectFamily != isCorrectFamily))
                {
                    if (isCorrectFamily)
                    {
                        continue;
                    }
                    else
                    {
                        isStronger = true;
                    }
                }

                if (!isStronger && (otherCard.Card.Value > foldWinner.Card.Value))
                {
                    isStronger = true;
                }

                if (isStronger)
                {
                    foldWinner = otherCard;
                    isCorrectFamily = isOtherCorrectFamily;
                    isTrumpFamily = isOtherTrumpFamily;
                }
            }

            sandbox.Players[foldWinner.PlayerIndex].FoldWon++;
            sandbox.RemainingFoldInTurn--;
            
            if (sandbox.RemainingFoldInTurn > 0)
            {
                stateMachine.SetNextState(new FoldState());
                return;
            }

            sandbox.CurrentTurn++;
            if (sandbox.CurrentTurn < sandbox.NumberOfTurns)
            {
                stateMachine.SetNextState(new InitializeTurnState());
                return;
            }

            stateMachine.SetNextState(new EndGameState());
        }
     
        public override Failures ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
            return Failures.WrongOrder;
        }
    }

    internal class EndGameState : GameState
    {
        public override void StartState(GameStateMachine stateMachine)
        {
        }

        public override Failures ProcessOrder(GameStateMachine stateMachine, GameOrder order)
        {
            return Failures.WrongOrder;
        }
    }
}

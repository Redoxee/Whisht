using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebWist
{
    [System.Serializable]
    public class PlayerViewUpdate : JSONResponse
    {
        public override string MessageType => "PlayerViewUpdate";

        public WistGame.GameStateID GameStateID;
        public int PlayerIndex;
        public WistGame.Card TrumpCard;
        public WistGame.Card[] Hand;
        [Newtonsoft.Json.JsonProperty("BetFailures", ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public WistGame.Failures[] BetFailures;
        public int Bet;
        public int PlayedCard;
        public int Score;
        public Player[] OtherPlayers;
        public int CurrentPlayer;

        public struct Player
        {
            public int Bet;
            public int NumberOfCards;
            public int CurrentScore;
        }
    }
}

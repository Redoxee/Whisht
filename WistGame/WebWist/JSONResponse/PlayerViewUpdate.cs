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

        public int PlayerIndex;
        public WistGame.Card[] Hand;
        public int Bet;
        public int PlayedCard;
        public int Score;
        public WistGame.StateID StateID;
    }
}

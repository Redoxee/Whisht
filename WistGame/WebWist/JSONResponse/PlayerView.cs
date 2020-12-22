using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebWist
{
    [System.Serializable]
    public class PlayerView : JSONResponse
    {
        public override string ResponseType => "PlayerView";

        public int PlayerIndex;
        public WistGame.Card[] Hand;
        public int Bet;
        public int PlayedCard;
        public int Score;
        public WistGame.StateID StateID;
    }
}

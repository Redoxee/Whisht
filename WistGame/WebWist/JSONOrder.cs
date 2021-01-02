using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebWist
{
    public class JSONOrder
    {
        public string OrderType = null;
        public int OrderID = -1;
        public int PlayerIndex = -1;
        public int BetValue = -1;
        public int CardIndex = -1;
        public bool[] AvailablePlayerIndexes = null;
    }
}

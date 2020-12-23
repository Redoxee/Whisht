using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebWist
{
    public class OrderAcknowledgement : JSONResponse
    {
        public override string MessageType => "OrderAcknowledgement";
        public int OrderID;
        public WistGame.Failures FailureFlags;
    }
}

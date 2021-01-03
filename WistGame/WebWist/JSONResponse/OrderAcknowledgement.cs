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
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public WistGame.Failures FailureFlags;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebWist
{
    public class AvailablePlayerSlot : JSONResponse
    {
        public override string ResponseType => "AvailablePlayerSlot";

        public bool[] AvaialablePlayerSlots = null;
    }
}

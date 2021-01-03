using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebWist
{
    public class SandboxChanges : JSONResponse
    {
        public override string MessageType => nameof(SandboxChanges);

        public WistGame.GameChange[] GameChanges;
        public PlayerViewUpdate PlayerViewUpdate;
    }
}

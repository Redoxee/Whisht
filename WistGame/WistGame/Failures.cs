using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WistGame
{
    [System.Flags]
    public enum Failures : ulong
    {
        None                    = 0,
        Unknown                 = 1 << 0,
        ColorCardAvailable      = 1 << 1,
        FamilyCardAvailable     = 1 << 2,
        WrongOrder              = 1 << 3,
        WrongPlayer       = 1 << 4,
        CardOutOfBounds         = 1 << 5,
        BetOutOfBounds          = 1 << 6,
        BetValueForbiden        = 1 << 7,
    }
}

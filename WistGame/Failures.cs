using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WistGame
{
    [System.Flags]
    enum Failures : ulong
    {
        None = 0,
        NeitherColorNorFamily = 1 << 0,
        ColorCardAvailable = 1 << 1,
        FamilyCardAvailable = 1 << 2,
    }
}

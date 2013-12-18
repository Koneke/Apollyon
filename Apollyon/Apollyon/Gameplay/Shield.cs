using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class Shield
    {
        int Current;
        int Max;

        public Shield(int _c, int _max)
        {
            Current = _c;
            Max = _max;
        }

        public float getPercentage()
        {
            return 100f * (float)Current / Max;
        }

        //resistances and stuff here later
    }
}

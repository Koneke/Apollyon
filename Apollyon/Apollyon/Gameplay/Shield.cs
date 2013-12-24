using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class Shield
    {
        public int Current;
        public int Max;

        public Shield(int _current, int _max)
        {
            Current = _current;
            Max = _max;
        }

        /*public float getPercentage()
        {
            return 100f * (float)Current / Max;
        }*/

        //resistances and stuff here later
        //or not? have it on the ship and this as a component? idk
    }
}

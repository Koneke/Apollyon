using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class AISimpleBrain
    {
        public List<Ship> Fleet;

        public AISimpleBrain()
        {
            Fleet = new List<Ship>();
        }

        public virtual void Tick() {} //once per tick
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class ShipComponent
    {
        public Ship ship;
        public string Name;

        public bool Active = false;
        public int Frequency; //time ticks per component tick
        int timer;

        public ShipComponent(string _name)
        {
            Name = _name;
        }

        public virtual void Tick()
        {
            timer += 1;
            if (timer == Frequency) Fire();
        }

        public virtual void Fire()
        {
            timer = 0;
        }
    }
}

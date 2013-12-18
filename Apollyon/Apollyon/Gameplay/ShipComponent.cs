using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class ShipComponent
    {
        public Ship Parent;
        public string Name;

        public bool Active = false;
        public int Frequency; //time ticks per component tick
        public int Timer;

        //weapons and similar
        public List<Ship> Targets;
        public Game.TargetingType TargetingType;

        public ShipComponent(string _name)
        {
            Name = _name;

            //choose random from the list of targets.
            //adding support for weakest, strongest etc. later on
            TargetingType = Game.TargetingType.Random;
        }

        public virtual void Tick()
        {
            if (!Active)
            {
                Timer = 0;
                return;
            }
            Timer += 1;
            if (Timer == Frequency)
            {
                Fire();
                Timer = 0;
            }
        }

        public virtual void Fire()
        {
        }
    }
}

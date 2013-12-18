using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class Weapon : ShipComponent
    {
        public int Damage;

        public Weapon(string _name) : base(_name)
        {
            Frequency = 60;
            Damage = 3;
        }

        public override void Fire() {
            Ship _target = null;
            switch (TargetingType)
            {
                case Game.TargetingType.Random:
                    _target = Targets[Game.Random.Next(0, Targets.Count)];
                    break;
                default:
                    break;
            }

            if (_target != null)
            {
                _target.Shield.Current -= Damage;
                Game.Log(Parent.Name + " dealt " + Damage +
                    " points of damage to " + _target.Name + " using " +
                    this.Name + ".");
            }
        }
    }
}

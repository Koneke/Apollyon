using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class AISimpleFighter : AISimpleBrain
    {
        List<Ship> targets;
        Dictionary<Ship, SpaceObject> fleetTargets =
            new Dictionary<Ship, SpaceObject>();
        public Faction Faction;

        public AISimpleFighter()
        {
            targets = new List<Ship>();
        }

        //brain functions

        void Target()
        {
            targets = 
                //find all ships
                Utility.QuerySpace(
                    new List<string> { "ship" } )
                .Cast<Ship>().ToList()
                //who are hostiles
                .FindAll(x => Faction.GetHostiles().Contains(x.Faction))
                //and alive
                .FindAll(x => x.Health > 0);
        }


        //rewrite/rework, a bit flaky atm
        public override void Tick()
        {
            //if we don't have a faction, we don't have hostiles
            if (Faction == null) return;

            Target();

            foreach (Ship _s in Fleet)
            {
                if (CheckShipEquipment(_s))
                {
                    float _targetRange = ShipTarget(_s);
                    ShipMove(_s, _targetRange);
                    ShipFire(_s);
                }
            }
        }

        //fleet functions

        //bool good-to-go
        bool CheckShipEquipment(Ship _s)
        {
            foreach (Item _i in _s.Inventory.FindAll(
                x => x.HasTag("weapon")))
                _i.Use();

            //equip all weapons available
            if (_s.Components.FindAll(
                x => x.Item.HasTag("weapon")).Count == 0)
                return false;

            if (targets.Count == 0)
            {
                foreach (ShipComponent _c in
                    _s.Components
                    .FindAll(x => x.Item.HasTag("weapon"))
                    .Cast<Weapon>()
                ) {
                    Weapon _w = _c as Weapon;
                    _w.Targets.Clear();
                    _w.Active = true;
                }
                return false;
            }

            return true;
        }

        //returns optim range
        float ShipTarget(Ship _s)
        {
            float _targetRange =
                //find all our weapons
                _s.Components.FindAll(
                    x => x.Item.HasTag("weapon"))
                //and select the one with the worst range
                .Cast<Weapon>().OrderBy(x => x.Range).ToList()[0].Range;

            bool _retarget = false;

            if (!fleetTargets.Keys.Contains(_s))
                _retarget = true;

            else if(fleetTargets[_s].Health <= 0)
                _retarget = true;

            if (_retarget)
            {
                //retarget closest target
                fleetTargets[_s] = targets.OrderBy(
                    x => Vector2.Distance(x.Position, _s.Position))
                    .ToList()[0];
            }

            return _targetRange;
        }

        void ShipMove(Ship _s, float _targetRange)
        {
            float _distance = Vector2.Distance(
                _s.Position,
                fleetTargets[_s].Position);

            if (
                _distance > _targetRange * 0.9f ||
                _distance < _targetRange * 0.7f
                )
            {
                double _angle = Math.Atan2(
                    fleetTargets[_s].Position.Y - _s.Position.Y,
                    fleetTargets[_s].Position.X - _s.Position.X);

                //aiming at 90% of the range so we have a bit of margin if
                //we/they move

                _s.TargetPosition =
                    new Vector2(
                        fleetTargets[_s].Position.X +
                            (float)Math.Cos(_angle) * _targetRange * 0.9f,
                        fleetTargets[_s].Position.Y +
                            (float)Math.Sin(_angle) * _targetRange * 0.9f
                        );
            }
            else
            {
                _s.Stop();
            }
        }

        void ShipFire(Ship _s)
        {
            foreach (ShipComponent _c in
                _s.Components
                .FindAll(x => x.Item.HasTag("weapon"))
                .Cast<Weapon>()
            ) {
                Weapon _w = _c as Weapon;
                _w.Targets = new List<SpaceObject> { fleetTargets[_s] };
                _w.Active = true;
            }
        }
    }
}

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

        //rewrite/rework, a bit flaky atm
        public override void Tick()
        {
            if (Faction == null) return;
            targets = targets.FindAll(x => x.Health > 0);

            if(true)
            {
                List<SpaceObject> query = Utility.QuerySpace(
                    new List<string> { "ship" }
                );
                targets = query.Cast<Ship>().ToList();
                targets = targets.FindAll(
                    x => Faction.GetHostiles().Contains(x.Faction)
                );
            }

            if (targets.Count == 0)
            {
                foreach (Ship _s in Fleet)
                {
                    foreach (ShipComponent _c in _s.Components)
                    {
                        if(_c.Item.HasTag("weapon")) {
                            _c.Targets = null;
                            _c.Active = false;
                        }
                    }
                }
                return;
            }

            foreach (Ship _s in Fleet)
            {
                if (fleetTargets.ContainsKey(_s))
                {
                    if (fleetTargets[_s].Health <= 0)
                    {
                        fleetTargets.Remove(_s);
                        continue;
                    }

                    if (
                        _s.Components.FindAll(
                            x => x.Item.HasTag("weapon")
                        ).Any(
                            x => Vector2.Distance(
                                fleetTargets[_s].Position,
                                _s.Position
                            )
                            > ((Weapon)x).Range
                        )
                    ) {
                        _s.TargetPosition = fleetTargets[_s].Position;
                    }
                    else
                    {
                        _s.TargetPosition = _s.Position;
                        _s.Stop();
                    }
                }
                else
                {
                    fleetTargets.Add(
                        _s,
                        targets.OrderBy(
                        x => Vector2.Distance(x.Position, _s.Position))
                        .Reverse().ToList()[0]);
                }
                foreach (Item _i in _s.Inventory.FindAll(x=>true))
                    if (_i.HasTag("weapon"))
                        if (_i.Component != null)
                            _i.Use();

                foreach (ShipComponent _c in _s.Components)
                {
                    if(_c.Item.HasTag("weapon")) {
                        _c.Targets = targets.Cast<SpaceObject>().ToList();
                        _c.Active = true;
                    }
                }
            }
        }
    }
}

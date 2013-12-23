using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class AISimpleMiner : AISimpleBrain
    {
        //int amtMined;
        //targets I'm keeping track to delegate my minions towards
        List<SpaceObject> targets;
        Dictionary<Ship, SpaceObject> fleetTargets;

        public AISimpleMiner()
        {
            targets = new List<SpaceObject>();
            fleetTargets = new Dictionary<Ship, SpaceObject>();
        }

        public override void Tick()
        {
            if (fleetTargets.Count == 0)
            {
                targets = Utility.QuerySpace(
                    new List<string>(){"celestial"}
                );
            }

            Fleet = Fleet.FindAll(x => x.Health > 0);

            foreach (Ship _s in Fleet)
            {
                if (
                    _s.Inventory.Count >= 50
                    )
                {
                    if (fleetTargets.ContainsKey(_s))
                    {
                        Game.Log(_s.Name + ": I'm full!");
                        fleetTargets.Remove(_s);
                    }

                    foreach (ShipComponent _c in
                        _s.Components.FindAll(x => x.Item.HasTag("miner")))
                    {
                        _c.Targets = null;
                        _c.Active = false;
                    }
                }
            }

            foreach (Ship _s in Fleet)
            {
                //equip all available miners

                foreach (Item _m in _s.Inventory.FindAll(x => x.ID == 1102)) {
                    _m.Use();
                }

                //stupid limit for testing behaviour
                if (_s.Inventory.Count >= 50) continue;
                //make if of this instead, here for now w/e
                if (targets.Count == 0) continue;

                SpaceObject target;
                if (!fleetTargets.ContainsKey(_s))
                    fleetTargets.Add(_s, targets.OrderBy(
                        x => Vector2.Distance(x.Position, _s.Position))
                        .ToList()[0]);

                target = fleetTargets[_s];

                _s.TargetPosition = target.Position;

                //if close enough, stop and start mining
                if (Vector2.Distance(_s.Position, target.Position) < 50)
                {
                    _s.Stop();

                    foreach (ShipComponent _c in
                        _s.Components.FindAll(x => x.Item.HasTag("miner")))
                    {
                        if (_c.Targets == null)
                        {
                            _c.Targets = new List<SpaceObject>() { target };
                            _c.Active = true;
                        }
                    }
                }

                if (Game.World.SpaceObjects.Any(
                    x => Vector2.Distance(x.Position, _s.Position) < 100))
                {
                    _s.Scoop();
                    //amtMined += _s.Scoop();
                }
            }
        }
    }
}

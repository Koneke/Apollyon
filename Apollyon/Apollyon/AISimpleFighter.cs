using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class AISimpleFighter : AISimpleBrain
    {
        List<Ship> targets;
        public Faction Faction;

        public AISimpleFighter()
        {
            targets = new List<Ship>();
        }

        public override void Tick()
        {
            if (Faction == null) return;
            if (targets.Count == 0)
            {
                List<SpaceObject> query = Utility.QuerySpace(
                    new List<string> { "ship" }
                );
                targets = query.Cast<Ship>().ToList();
                targets = targets.FindAll(
                    x => Faction.GetHostiles().Contains(x.Faction)
                );
            }

            foreach (Ship _s in Fleet)
            {
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

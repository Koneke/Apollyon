using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollyon
{
    class Station : SpaceObject
    {
        List<Ship> DockedShips;
        Dictionary<Ship, List<Item>> Hangars;

        public Station(
            World _world
        ) : base(_world)
        {
            DockedShips = new List<Ship>();
            Hangars = new Dictionary<Ship, List<Item>>();
        }

        public void Dock(Ship _s)
        {
            DockedShips.Add(_s);
            _s.Position = Position;
            Utility.Tag(_s, "docked");
        }

        public void Undock(Ship _s)
        {
            DockedShips.Remove(_s);
            Utility.Untag(_s, "docked");
        }
    }
}

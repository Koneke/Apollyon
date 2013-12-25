using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollyon
{
    class Station
    {
        public string Name;
        List<Ship> DockedShips;
        Dictionary<Ship, List<Item>> Hangars;

        public void Dock(Ship _s)
        {
            DockedShips.Add(_s);
        }

        public void Undock(Ship _s)
        {
            DockedShips.Remove(_s);
        }
    }
}

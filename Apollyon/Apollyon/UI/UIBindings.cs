using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    static class UIBindings
    {
        public static Dictionary<
            string, List<SpaceObject>>
        ShipLists = new Dictionary<string,List<SpaceObject>>();

        public static void Bind(string _key, List<SpaceObject> _list)
        {
            if (!ShipLists.ContainsKey(_key))
                ShipLists.Add(_key.ToLower(), _list);
            else
                ShipLists[_key] = _list;
        }

        public static List<SpaceObject> Get(string _key)
        {
            return ShipLists[_key.ToLower()];
        }
    }
}

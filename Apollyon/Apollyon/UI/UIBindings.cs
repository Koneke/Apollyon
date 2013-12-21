using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class UIBindings
    {
        public static Dictionary<
            string, List<ISpaceObject>>
        ShipLists = new Dictionary<string,List<ISpaceObject>>();

        public static void Bind(string _key, List<ISpaceObject> _list)
        {
            ShipLists.Add(_key, _list);
        }

        public static List<ISpaceObject> Get(string _key)
        {
            return ShipLists[_key];
        }
    }
}

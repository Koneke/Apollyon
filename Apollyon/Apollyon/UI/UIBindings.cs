using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class UIBindings
    {
        public static Dictionary<
            string, List<Ship>> ShipLists = new Dictionary<string,List<Ship>>();

        public static void Bind(string _key, List<Ship> _list)
        {
            ShipLists.Add(_key, _list);
        }

        public static List<Ship> Get(string _key)
        {
            return ShipLists[_key];
        }
    }
}

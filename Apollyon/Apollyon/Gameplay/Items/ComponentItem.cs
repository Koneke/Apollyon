using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class ComponentItem : Item
    {
        public ShipComponent Component;

        public ComponentItem(
            string _name = "",
            int _id = -1,
            ShipComponent _component = null,
            bool _stacking = false,
            int _count = 1
        ) : base(_name, _id, _stacking, _count) {
            Component = _component;
        }

        public override void RealUse()
        {
            Carrier.Inventory.Remove(this);
            Carrier.Components.Add(Component);
            Component.Parent = Carrier;
        }
    }
}

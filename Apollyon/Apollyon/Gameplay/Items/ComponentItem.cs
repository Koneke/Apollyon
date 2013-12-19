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
            ShipComponent _component = null,
            bool _stacking = false,
            int _count = 1
        ) : base(_name, _stacking, _count) {
            Component = _component;
        }

        public override void RealUse(Ship _user)
        {
            _user.Inventory.Remove(this);
            _user.Components.Add(Component);
            Component.Parent = _user;
        }
    }
}

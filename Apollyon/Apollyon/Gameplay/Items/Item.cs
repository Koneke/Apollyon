using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Item : SpaceObject
    {
        public override bool Visible
        {
            get { return Carrier == null; }
        }

        /*-------*/

        //switch to SpaceObject to account for containers and such
        //public Ship Carrier;
        public SpaceObject Carrier;

        public int Count;
        public Boolean Stacking;
        public int ID;

        public Item(
            string _name = "",
            int _id = -1,
            bool _inSpace = true, //debuggish, leave for now
            bool _stacking = false,
            int _count = 1
        ) {
            Name = _name;
            ID = _id;
            Stacking = _stacking;
            Count = _count;
            Carrier = null;
            Tags = new List<string>();
            Tags.Add("Item");
            Size = new Vector2(16, 16);

            if (_inSpace) {
                Game.World.SpaceObjects.Add(this); }
        }

        public override void Die()
        {
            Game.World.SpaceObjects.Remove(this);
            Game.Log(Name + " was destroyed.");
        }

        /*optional bits*/
        public ShipComponent Component;

        public void Use()
        {
            RealUse();
        }

        public virtual void RealUse()
        {
            //for now, only ships use items
            if (!Carrier.HasTag("ship")) return;
            if (Component != null)
            {
                Ship _carrier = Carrier as Ship;
                _carrier.Inventory.Remove(this);
                _carrier.Components.Add(Component);
                Component.Parent = _carrier;
            }
        }
    }
}

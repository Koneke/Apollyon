﻿using System;
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

        public Ship Carrier;

        public int Count;
        public Boolean Stacking;
        public int ID;

        public Item(
            string _name = "",
            int _id = -1,
            bool _inSpace = true,
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

        /*optional bits*/
        public ShipComponent Component;

        public void Use()
        {
            RealUse();
        }

        public virtual void RealUse()
        {
            if (Component != null)
            {
                Carrier.Inventory.Remove(this);
                Carrier.Components.Add(Component);
                Component.Parent = Carrier;
            }
        }
    }
}

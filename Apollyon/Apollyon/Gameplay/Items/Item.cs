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

        public static int ItemSpaceLife = 20000; //ms
        public DateTime Dropped;

        //switch to SpaceObject to account for containers and such
        public SpaceObject Carrier;

        public int Count;
        public int ID;

        bool rotationDirection;
        float rotationSpeed;

        public Item(
            string _name = "",
            int _id = -1,
            bool _inSpace = true, //debuggish, leave for now
            int _count = 1
        ) {
            Name = _name;
            ID = _id;
            Count = _count;
            Carrier = null;
            Tags = new List<string>();
            Tags.Add("Item");
            Size = new Vector2(16, 16);

            rotationDirection = Game.Random.NextDouble() > 0.5f;
            rotationSpeed = 0.01f + (float)Game.Random.NextDouble() * 0.01f;

            if (_inSpace) {
                Game.World.SpaceObjects.Add(this); }
        }

        public override void Update()
        {
            base.Update();

            if (
                Carrier == null
            )
            {
                Rotation += rotationSpeed * (rotationDirection ? 1 : -1);

                if (Dropped.Year == 1)
                    Dropped = DateTime.Now;
            }
            else
            {
                Dropped = new DateTime();
            }
            
            if (
                Dropped.Year != 1 &&
                (DateTime.Now - Dropped).TotalMilliseconds > ItemSpaceLife
            ) {
                Die();
            }
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
            //for now, only ships |use| items
            if (!Carrier.HasTag("ship")) return;
            if (Component != null)
            {
                Ship _carrier = Carrier as Ship;
                _carrier.Inventory.Remove(this);
                _carrier.Components.Add(Component);
                Component.Carrier = _carrier;
            }
        }
    }
}

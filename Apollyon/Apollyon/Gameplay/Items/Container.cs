using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Container : Item
    {
        public List<Item> Inventory;

        public Container(
            string _name,
            World _world
        ) : base(_name, _world)
        {
            Name = "Generic Container";
            World = _world;
            Texture = Res.Textures["generic"];
            Size = new Vector2(24, 24);

            Inventory = new List<Item>();

            Utility.Tag(this, "item");
            Utility.Tag(this, "container");

            ID = 100;
        }

        public override void Die()
        {
            while (Inventory.Count > 0)
            {
                Item _i = Inventory[0];
                DropItem(_i);
            }
        }

        //below two are clutzily ripped from ship.cs
        public void AddItem(Item _i)
        {
            _i.Carrier = this;
            Utility.Tag(_i, "carried");
            Inventory.Add(_i);
        }

        public void DropItem(Item _i)
        {
            _i.Carrier = null;
            _i.Position = Position;
            _i.Tags.Remove("carried");
            _i.Velocity = new Vector2(
                -0.5f + (float)Game.Random.NextDouble(),
                -0.5f + (float)Game.Random.NextDouble());
            Inventory.Remove(_i);
        }
    }
}

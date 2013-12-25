using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class Asteroid : Container
    {
        public Asteroid(
            string _name,
            World _world
        ) : base(_name, _world)
        {
            Name = "Asteroid";
            Texture = Res.Textures["asteroid"];
            Size = new Vector2(32, 32);
            Utility.Untag(this, "item");
            Utility.Tag(this, "celestial");
        }

        public override void Update()
        {
            Position += Velocity;
        }

        public override void Damage(AttackInfo _attack)
        {
            if (_attack.Weapon.Item.HasTag("miner"))
            {
                Item _i = ItemDatabase.Spawn(
                    _attack.Source.World,
                    ItemDatabase.Items.Find(
                    x => x.ID == 1000));
                _i
                    .SetPosition(Position)
                    .SetVelocity(
                        new Vector2(
                            -0.5f+(float)Game.Random.NextDouble(),
                            -0.5f+(float)Game.Random.NextDouble()
                        ) * 0.2f
                );
                if(_attack.Source.HasTag("ship")) {
                    Ship _s = (_attack.Source as Ship);
                    _s.AddItem(_i);
                    _i.SetVelocity(Vector2.Zero);
                }
            }
        }
    }
}

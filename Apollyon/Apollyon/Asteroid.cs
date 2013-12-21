using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class Asteroid : Container
    {
        public Asteroid()
        {
            Name = "Asteroid";
            Texture = Res.Textures["asteroid"];
            Size = new Vector2(32, 32);
            Utility.Untag(this, "item");
            Utility.Tag(this, "celestial");
        }

        public override void Damage(AttackInfo _attack)
        {
            if (_attack.Weapon.Item.HasTag("miner"))
            {
                ItemDatabase.Spawn(
                    ItemDatabase.Items.Find(
                    x => x.ID == 1000))
                    .SetPosition(Position)
                    .SetVelocity(
                        new Vector2(
                            -0.5f+(float)Game.Random.NextDouble(),
                            -0.5f+(float)Game.Random.NextDouble()
                        ) * 0.2f
                );
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Container : SpaceObject
    {
        public Container()
        {
            Name = "Generic Container";
            Texture = Res.Textures["generic"];
            Size = new Vector2(24, 24);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class SpaceItem
    {
        public Vector2 Position;
        public Texture2D Texture; //might want to move this to the item itself?
        public Item Item;

        public SpaceItem(
            Vector2 _position,
            Texture2D _texture,
            Item _item
        ) {
            Position = _position;
            Texture = _texture;
            Item = _item;
        }
    }
}

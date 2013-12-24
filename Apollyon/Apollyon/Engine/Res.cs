using System.Text;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    static class Res
    {
        public static Dictionary<string, Texture2D> Textures =
            new Dictionary<string, Texture2D>();
        public static Dictionary<string, SpriteFont> Fonts =
            new Dictionary<string, SpriteFont>();

        //make one for textures as well, easier to handle in the future this way
        public static SpriteFont GetFont(string _name)
        {
            return Fonts[_name];
        }
    }
}

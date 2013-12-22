using System.Text;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Res
    {
        //public static SpriteFont LogFont;

        public static Dictionary<string, Texture2D> Textures =
            new Dictionary<string, Texture2D>();
        public static Dictionary<string, SpriteFont> Fonts =
            new Dictionary<string, SpriteFont>();

        //make one for textures as well, easier to handle in the future this way
        public static SpriteFont GetFont(string _name)
        {
            return Fonts[_name];
        }

        /* word wrapping fr http://www.xnawiki.com/index.php/Basic_Word_Wrapping
         * because I can't be arsed with writing this again */
        public static string WrapText(
            SpriteFont spriteFont,
            string text,
            float maxLineWidth
        ) {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;
            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);
                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + " " + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }
            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Utility
    {
        public static Color MultiplyColours(Color a, Color b)
        {
            return new Color(
                a.R/255f * b.R/255f,
                a.G/255f * b.G/255f,
                a.B/255f * b.B/255f,
                a.A/255f * b.A/255f
            );
        }

        public static void DrawOutlinedRectangle(
            SpriteBatch spriteBatch,
            Rectangle _r,
            Color _c
        ) {
            spriteBatch.Begin();

            spriteBatch.Draw(
                Res.OneByOne,
                new Rectangle(
                    (int)_r.X,
                    (int)_r.Y,
                    (int)_r.Width,
                    1
                ),
                _c
            );

            spriteBatch.Draw(
                Res.OneByOne,
                new Rectangle(
                    (int)_r.X,
                    (int)_r.Y + (int)_r.Height-1,
                    (int)_r.Width,
                    1
                ),
                _c);

            spriteBatch.Draw(
                Res.OneByOne,
                new Rectangle(
                    (int)_r.X,
                    (int)_r.Y,
                    1,
                    (int)_r.Height
                ),
                _c);

            spriteBatch.Draw(
                Res.OneByOne,
                new Rectangle(
                    (int)_r.X + (int)_r.Width-1,
                    (int)_r.Y,
                    1,
                    (int)_r.Height
                ),
                _c);

            spriteBatch.End();
        }
    }
}

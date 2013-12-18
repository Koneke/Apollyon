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
            //hi my name is xna durrrrrrrrrr
            return new Color(
                a.R/255f * b.R/255f,
                a.G/255f * b.G/255f,
                a.B/255f * b.B/255f,
                a.A/255f * b.A/255f
            );
            //why is this not in by default anyways? there's operator
            //overloading in the language, and it's your own fucking langauge
            //and framework... colour multiplication is pretty cool shit yknow.
            //also, fak you i want my colours between 0 and 1
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

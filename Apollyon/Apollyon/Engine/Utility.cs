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
        public static List<ISpaceObject> QuerySpace(List<string> _tags)
        {
            List<ISpaceObject> _r = Game.World.SpaceObjects;
            foreach (string _tag in _tags)
            {
                _r = _r.FindAll(x => x.GetTags().Contains(_tag));
            }
            return _r;
        }

        //put the colour stuff somewhere else, this class is a bit messy AON 
        public static Color AddColours(Color _a, Color _b)
        {
            return new Color(
                _a.R/255f + _b.R/255f,
                _a.G/255f + _b.G/255f,
                _a.B/255f + _b.B/255f,
                _a.A/255f + _b.A/255f
            );
        }

        public static Color AddColours(Color _a, Vector4 _b)
        {
            var foo =
                _a.A / 255f + _b.W;
            Color _c = new Color(
                _a.R/255f + _b.X,
                _a.G/255f + _b.Y,
                _a.B/255f + _b.Z,
                _a.A/255f + _b.W
            );
            return _c;
        }

        public static Color MultiplyColours(Color _a, Color _b)
        {
            return new Color(
                _a.R/255f * _b.R/255f,
                _a.G/255f * _b.G/255f,
                _a.B/255f * _b.B/255f,
                _a.A/255f * _b.A/255f
            );
        }

        public static Color ScaleColour(Color _a, float _scale)
        {
            return new Color(
                _a.R * _scale,
                _a.G * _scale,
                _a.B * _scale,
                _a.A * _scale
            );
        }

        public static Color SubtractFromColour(Color _a, float _amount)
        {
            return new Color(
                _a.R - _amount,
                _a.G - _amount,
                _a.B - _amount,
                _a.A - _amount
            );
        }

        public static Color AddToColour(Color _a, float _amount)
        {
            return new Color(
                _a.R + _amount,
                _a.G + _amount,
                _a.B + _amount,
                _a.A + _amount
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

        public static void DropShadowText(
            SpriteBatch spriteBatch,
            SpriteFont _font,
            string _text,
            Vector2 _position,
            Color _colorA,
            Color _colorB)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(
                _font,
                _text,
                _position+new Vector2(1, 1),
                _colorA
            );
            spriteBatch.DrawString(
                _font,
                _text,
                _position,
                _colorB 
            );
            spriteBatch.End();
        }
    }
}

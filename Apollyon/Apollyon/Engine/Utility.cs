﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Utility
    {
        public static List<SpaceObject> QuerySpace(
            List<string> _tags,
            //any or all, any = objects having any one tag, all is obv
            bool any = false
        ) {
            List<SpaceObject> _r = Game.World.SpaceObjects;
            if (any)
            {
                //_r = _r.FindAll(x => x.Tags.Any(y => _tags.Contains(y)));
                _r.FindAll(x => _tags.Any(y => x.HasTag(y)));
            }
            else
            {
                _r = _r.FindAll(x => _tags.All(y => x.HasTag(y)));
            }

            return _r;
        }

        public static void Tag(SpaceObject _i, string _tag)
        {
            _i.Tags.Add(_tag);
            _i.Tags = _i.Tags.Distinct().ToList();
        }

        public static void Untag(SpaceObject _i, string _tag)
        {
            _i.Tags = _i.Tags.FindAll(x => !x.Equals(_tag));
            _i.Tags = _i.Tags.Distinct().ToList();
        }

        public static Vector2 RandomDirectionVector()
        {
            double _a = Game.Random.NextDouble()*Math.PI*2;
            return new Vector2(
                (float)Math.Cos(_a),
                (float)Math.Sin(_a));
        }

        /* word wrapping fr http://www.xnawiki.com/index.php/Basic_Word_Wrapping
         * because I can't be arsed with writing this again */
        public static string WrapText(
            SpriteFont spriteFont,
            string text,
            float maxLineWidth,
            bool indent = false
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
                    if(indent) sb.Append("\n" + " " + word + " ");
                    else sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }
            return sb.ToString();
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

        public static int SumColour(Color _a)
        {
            return _a.R + _a.G + _a.B + _a.A;
        }


        public static void DrawOutlinedRectangle(
            Rectangle _r,
            Color _c
        ) {
            List<Rectangle> _rectangles = new List<Rectangle>
            {
                new Rectangle(
                    (int)_r.X, (int)_r.Y,
                    (int)_r.Width, 1),
                new Rectangle(
                    (int)_r.X, (int)_r.Y + (int)_r.Height-1,
                    (int)_r.Width, 1),
                new Rectangle(
                    (int)_r.X, (int)_r.Y,
                    1, (int)_r.Height),
                new Rectangle(
                    (int)_r.X + (int)_r.Width-1, (int)_r.Y,
                    1, (int)_r.Height)
            };

            foreach (Rectangle _rectangle in _rectangles)
            {
                DrawManager.AddCall(
                    new BasicDrawCall(
                        Res.Textures["1x1"],
                        _rectangle,
                        null,
                        _c,
                        0,
                        Vector2.Zero,
                        -1f
                    )
                );
            }
        }

        public static void DropShadowText(
            string _font,
            string _text,
            Vector2 _position,
            Color _colorA,
            Color _colorB)
        {
            DrawManager.AddCall(
                new TextDrawCall(
                    _font,
                    _text,
                    _position+new Vector2(1, 1),
                    _colorA,
                    -9f
                )
            );
            DrawManager.AddCall(
                new TextDrawCall(
                    _font,
                    _text,
                    _position,
                    _colorB,
                    -9f
                )
            );
        }
    }
}

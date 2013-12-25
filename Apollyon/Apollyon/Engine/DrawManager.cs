using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    interface IDrawCall
    {
        float GetDepth();
        void Draw(SpriteBatch spriteBatch);
    }

    class BasicDrawCall : IDrawCall
    {
        public Texture2D Texture;

        public Rectangle? Rectangle;
        public Rectangle? SourceRect;

        public Vector2? Position;

        public Color Colour;
        public float Rotation;
        public Vector2 Origin;
        public float Depth;

        public float GetDepth() { return Depth; }

        public BasicDrawCall(
            Texture2D _texture,
            Rectangle? _rectangle,
            Rectangle? _sourceRect,
            Color _colour,
            float _rotation,
            Vector2 _origin,
            float _depth = 0
        ) {
            Texture = _texture;
            Rectangle = _rectangle;
            SourceRect = _sourceRect;
            Colour = _colour;
            Rotation = _rotation;
            Origin = _origin;
            Depth = _depth;
        }

        public BasicDrawCall(
            Texture2D _texture,
            Rectangle? _rectangle,
            Vector2? _position,
            Color _colour,
            float _depth = 0
        ) {
            Texture = _texture;
            Rectangle = _rectangle;
            SourceRect = null;
            Position = _position;
            Colour = _colour;
            Rotation = 0f;
            Origin = new Vector2(0, 0);
            Depth = _depth;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Rectangle.HasValue)
            {
                spriteBatch.Draw(
                    Texture,
                    Rectangle.Value,
                    SourceRect,
                    Colour,
                    Rotation,
                    Origin,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                spriteBatch.Draw(
                    Texture,
                    Position.Value,
                    Colour
                );
            }
        }
    }

    class TextDrawCall : IDrawCall
    {
        public string Font;
        public string Text;
        public Vector2 Position;
        public Color Colour;
        public float Depth;

        public float GetDepth() { return Depth; }

        public TextDrawCall(
            string _font,
            string _text,
            Vector2 _position,
            Color _colour,
            float _depth = 0
        ) {
            Font = _font;
            Text = _text;
            Position = _position;
            Colour = _colour;
            Depth = _depth;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                Res.GetFont(Font),
                Text,
                Position,
                Colour
            );
        }
    }

    static class DrawManager
    {
        public static List<IDrawCall> DrawCalls = new List<IDrawCall>();

        public static void AddCall(IDrawCall _idc)
        {
            DrawCalls.Add(_idc);
        }

        public static void Draw(SpriteBatch spriteBatch) {
            foreach(IDrawCall _idc in
                DrawCalls.OrderBy(x => x.GetDepth()).Reverse()
            ) {
                _idc.Draw(spriteBatch);
            }
            DrawCalls.Clear();
        }
    }
}

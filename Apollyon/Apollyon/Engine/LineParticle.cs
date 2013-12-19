using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class LineParticle : Particle
    {
        public Vector2 Target;
        public int Thickness;

        public LineParticle(
            Vector2 _position,
            Vector2 _target,
            int _thickness,
            Color _colour,
            int _lifeTime,
            Texture2D _texture = null,
            Vector4? _deltaColour = null,
            double _direction = 0,
            float _speed = 0,
            float _friction = 0,
            double _rotation = 0,
            float _angularVelocity = 0
        ) : base (
            _position, _texture, _colour, _lifeTime, _deltaColour, _direction,
            _speed, _friction, _rotation, _angularVelocity)
        {
            Thickness = _thickness;
            Target = _target;
        }

        public override Particle Copy()
        {
            LineParticle _p = (LineParticle)base.Copy();
            _p.Thickness = Thickness;
            _p.Target = Target;
            return _p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                Res.OneByOne,
                new Rectangle(
                    (int)Game.Camera.WorldToScreen(Position).X,
                    (int)Game.Camera.WorldToScreen(Position).Y,
                    (int)(Vector2.Distance(Position, Target)
                        *Game.Camera.GetZoom()),
                    Thickness
                ),
                null,
                Colour,
                (float)Math.Atan2(
                    Target.Y - Position.Y,
                    Target.X - Position.X
                ),
                new Vector2(0,0),
                SpriteEffects.None,
                0f
            );
            spriteBatch.End();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Particle
    {
        public static List<Particle> Particles = new List<Particle>();

        public DateTime Created;

        public Vector2 Position;
        public Texture2D Texture;
        public Color Colour;
        public int LifeTime;
        public double Direction;
        public float Speed;
        public float Friction;
        public double Rotation;
        public float AngularVelocity;
        public float Depth = -1f; //below 0 = under game, above = above game

        public virtual Particle Copy ()
        {
            Particle _p = new Particle(
                Position,
                Texture,
                Colour,
                LifeTime,
                Direction,
                Speed,
                Friction,
                Rotation,
                AngularVelocity
            );
            _p.Created = Created;
            _p.Depth = Depth;
            return _p;
        }

        public Particle(
            Vector2 _position,
            Texture2D _texture,
            Color _colour,
            int _lifeTime,
            double _direction = 0,
            float _speed = 0,
            float _friction = 0,
            double _rotation = 0,
            float _angularVelocity = 0
        ) {
            Created = DateTime.Now;
            Position = _position;
            Texture = _texture;
            Colour = _colour;
            LifeTime = _lifeTime;
            Direction = _direction;
            Speed = _speed;
            Friction = _friction;
            Rotation = _rotation;
            AngularVelocity = _angularVelocity;
        }

        public virtual void Update()
        {
            Position.X += (float)Math.Cos(Direction) * Speed;
            Position.Y += (float)Math.Sin(Direction) * Speed;
            Direction += AngularVelocity;
            Speed *= 1 - Friction;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            var _p = Game.Camera.WorldToScreen(Position);
            DrawManager.AddCall(
                new BasicDrawCall(
                    Texture,
                    new Rectangle(
                        (int)_p.X,
                        (int)_p.Y,
                        Math.Max((int)(Texture.Width *
                            Game.Camera.GetZoom()), 1),
                        Math.Max((int)(Texture.Height *
                            Game.Camera.GetZoom()), 1)
                    ),
                    null,
                    Colour,
                    (float)Rotation,
                    new Vector2(Texture.Width/2, Texture.Height/2),
                    -0.1f
                )
            );
        }
    }
}

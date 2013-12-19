using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ParticleSpawn
    {
        public Particle Particle;
        public int Count;

        List<Particle> particles;

        public ParticleSpawn(int _count, Particle _p)
        {
            Particle = _p;
            Count = _count;

            particles = new List<Particle>();
            Renew();
        }

        public ParticleSpawn Renew()
        {
            particles.Clear();
            for (int i = 0; i < Count; i++)
                particles.Add(Particle.Copy());
            return this;
        }

        public ParticleSpawn SetSpeed(float _speed)
        {
            foreach (Particle _p in particles)
            {
                _p.Speed = _speed;
            }
            return this;
        }

        public ParticleSpawn SetDirection(double _direction)
        {
            foreach (Particle _p in particles)
            {
                _p.Direction = _direction;
            }
            return this;
        }

        public ParticleSpawn SetPosition(Vector2 _amount)
        {
            foreach (Particle _p in particles)
            {
                _p.Position = _amount;
            }
            return this;
        }

        public ParticleSpawn RandomizeLifeTime(int _amount)
        {
            foreach (Particle _p in particles)
            {
                _p.LifeTime -= _amount / 2;
                _p.LifeTime += (int)(Game.Random.NextDouble() * _amount);

            }
            return this;
        }

        public ParticleSpawn RandomizeSpeed(float _amount)
        {
            foreach (Particle _p in particles)
            {
                _p.Speed -= _amount / 2f;
                _p.Speed += (float)Game.Random.NextDouble() * _amount;
                _p.Speed = Math.Max(_p.Speed, 0);
            }
            return this;
        }

        public ParticleSpawn RandomizePosition(Vector2 _amount)
        {
            foreach (Particle _p in particles)
            {
                _p.Position.X -= _amount.X / 2f;
                _p.Position.Y -= _amount.Y / 2f;
                _p.Position.X += _amount.X * (float)Game.Random.NextDouble();
                _p.Position.Y += _amount.Y * (float)Game.Random.NextDouble();
            }
            return this;
        }

        public ParticleSpawn RandomizeDirection(float _amount)
        {
            foreach (Particle _p in particles)
            {
                _p.Direction -= _amount / 2f;
                _p.Direction += (float)Game.Random.NextDouble() * _amount;
            }
            return this;
        }

        public ParticleSpawn RandomizeColor(Color _amount)
        {
            foreach (Particle _p in particles)
            {
                _p.Colour = Utility.SubtractFromColour(_p.Colour, 255*0.2f);
                _p.Colour =
                    new Color(
                        Math.Max(_p.Colour.R/255f +
                            _amount.R/255f * (float)Game.Random.NextDouble(), 0
                            ),
                        Math.Max(_p.Colour.G/255f +
                            _amount.G/255f * (float)Game.Random.NextDouble(), 0
                            ),
                        Math.Max(_p.Colour.B/255f +
                            _amount.B/255f * (float)Game.Random.NextDouble(), 0
                            ),
                        0f
                    );
            }
            return this;
        }

        public void Spawn()
        {
            Particle.Particles.AddRange(particles);
        }
    }

    class Particle
    {
        public static List<Particle> Particles = new List<Particle>();

        public DateTime Created;

        public Vector2 Position;
        public Texture2D Texture;
        public Color Colour;
        public Vector4? DeltaColour;
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
                DeltaColour,
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
            Vector4? _deltaColour = null,
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
            DeltaColour = _deltaColour;
            LifeTime = _lifeTime;
            Direction = _direction;
            Speed = _speed;
            Friction = _friction;
            Rotation = _rotation;
            AngularVelocity = _angularVelocity;
        }

        public static void UpdateParticles()
        {
        }

        public virtual void Update()
        {
            Position.X += (float)Math.Cos(Direction) * Speed;
            Position.Y += (float)Math.Sin(Direction) * Speed;
            Direction += AngularVelocity;
            Speed *= 1 - Friction;
            //Colour = Utility.AddColours(Colour, DeltaColour);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            var _p = Game.Camera.WorldToScreen(Position);
            spriteBatch.Begin();
            spriteBatch.Draw(
                Texture,
                new Rectangle(
                    (int)_p.X,
                    (int)_p.Y,
                    Math.Max((int)(Texture.Width * Game.Camera.GetZoom()), 1),
                    Math.Max((int)(Texture.Height * Game.Camera.GetZoom()), 1)
                ),
                null,
                Colour,
                (float)Rotation,
                new Vector2(Texture.Width/2, Texture.Height/2),
                SpriteEffects.None,
                0f
            );
            spriteBatch.End();
        }
    }
}

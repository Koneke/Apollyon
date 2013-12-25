using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Particle2
    {
        public static List<Particle2> Particles = new List<Particle2>();
        static DateTime last;

        public static void AddParticle(Particle2 _p)
        {
            if (Particles.Count == 0)
                last = DateTime.Now;
            Particles.Add(_p);
        }

        public static void Update()
        {
            float _dt = //dt in s
                (float)((DateTime.Now - last).TotalMilliseconds / 1000f);
            int _dtms = //dt in ms
                (int)((DateTime.Now - last).TotalMilliseconds);

            foreach (Particle2 _p in Particles)
            {
                _p.position += _p.velocity * _dt;
                _p.lifeTime -= _dtms;
            }

            Particles = Particles.FindAll(x => x.lifeTime > 0);

            last = DateTime.Now;
        }

        public static void Tick() //deltas here
        {
            foreach (Particle2 _p in Particles)
            {
                _p.ApplyFrictionDelta();
                _p.ApplyColorDelta();
                _p.ApplyRotationDelta();
                _p.ApplyScaleDelta();

                _p.velocity *= 1 - _p.friction;
            }
        }

        public static void Draw(
            SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            foreach (Particle2 _p in Particles)
            {
                Vector2 _screenPosition =
                    Game.Camera.WorldToScreen(_p.position);

                Rectangle _screenRect = new Rectangle(
                    (int)_screenPosition.X,
                    (int)_screenPosition.Y,
                    Math.Max(
                        (int)(_p.texture.Width * _p.scale *
                            Game.Camera.GetZoom()),
                        1
                    ),
                    Math.Max(
                        (int)(_p.texture.Height * _p.scale *
                            Game.Camera.GetZoom()),
                        1
                    )
                );

                DrawManager.AddCall(
                    new BasicDrawCall(
                        _p.texture,
                        _screenRect,
                        null,
                        _p.color,
                        (float)_p.rotation,
                        new Vector2(
                            _p.texture.Width / 2,
                            _p.texture.Height / 2),
                        1f
                    )
                );
                /*
                spriteBatch.Draw(
                    _p.texture,
                    _screenRect,
                    null,
                    _p.color,
                    (float)_p.rotation,
                    new Vector2(
                        _p.texture.Width / 2,
                        _p.texture.Height / 2),
                        SpriteEffects.None,
                    0f
                );*/
            }
            //spriteBatch.End();
        }

        //ACTUAL VARIED VALUES ARE ONLY GENERATED UPON USING THE GENERATE FUNCTS
        //SO, BASIC USE
        //1. FILL IN VALUES, SAVE AS TEMPLATE
        //2. CLONE AS YOU LIKE
        //3. SPAWN THE CLONES

        #region vars
        Vector2 position;
        float positionVariation;

        Vector2 velocity;
        float velocityVariation;

        float friction;
        float frictionVariation;
        float frictionDelta;

        Color color;
        Color colorVariation;
        Color colorDelta;
        //one for scaleing over time? only pure add/sub atm

        float scale;
        float scaleVariation;
        float scaleDelta;

        double rotation;
        double rotationVariation;
        double rotationDelta;

        Texture2D texture;

        int lifeTime;
        int lifeTimeVariation;
        #endregion

        #region sets
        public Particle2 SetPosition(
            Vector2 _position,
            float _positionVariation)
        {
            position = _position;
            positionVariation = _positionVariation;
            return this;
        }
        //set pos without altering variation
        public Particle2 SetPosition(Vector2 _position)
        {
            position = _position;
            return this;
        }

        public Particle2 SetVelocity(
            Vector2 _velocity,
            float _velocityVariation)
        {
            velocity = _velocity;
            velocityVariation = _velocityVariation;
            return this;
        }
        public Particle2 SetVelocity(
            Vector2 _velocity)
        {
            velocity = _velocity;
            return this;
        }

        public Particle2 SetFriction(
            float _friction,
            float _frictionVariation,
            float _frictionDelta)
        {
            friction = _friction;
            frictionVariation = _frictionVariation;
            frictionDelta = _frictionDelta;
            return this;
        }

        public Particle2 SetColour(
            Color _color,
            Color _colorVariation,
            Color _colorDelta)
        {
            color = _color;
            colorVariation = _colorVariation;
            colorDelta = _colorDelta;
            return this;
        }

        public Particle2 SetScale(
            float _scale,
            float _scaleVariation,
            float _scaleDelta)
        {
            scale = _scale;
            scaleVariation = _scaleVariation;
            scaleDelta = _scaleDelta;
            return this;
        }

        public Particle2 SetRotation(
            double _rotation,
            double _rotationVariation,
            double _rotationDelta)
        {
            rotation = _rotation;
            rotationVariation = _rotationVariation;
            rotationDelta = _rotationDelta;
            return this;
        }

        public Particle2 SetTexture(
            Texture2D _texture)
        {
            texture = _texture;
            return this;
        }

        public Particle2 SetLifeTime(
            int _lifeTime,
            int _lifeTimeVariation)
        {
            lifeTime = _lifeTime;
            lifeTimeVariation = _lifeTimeVariation;
            return this;
        }
        #endregion

        #region generate
        void GenerateSpawnPosition()
        {
            position += 
                new Vector2(
                    (float)((-positionVariation/2f) + positionVariation
                        * Game.Random.NextDouble()),
                    (float)((-positionVariation/2f) + positionVariation
                        * Game.Random.NextDouble())
                );
        }
        void GenerateSpawnVelocity()
        {
            velocity += 
                new Vector2(
                    (float)((-velocityVariation/2f) + velocityVariation
                        * Game.Random.NextDouble()),
                    (float)((-velocityVariation/2f) + velocityVariation
                        * Game.Random.NextDouble())
                );
        }
        void GenerateSpawnFriction()
        {
            friction +=  +
                (-frictionVariation / 2f) +
                (float)Game.Random.NextDouble() * frictionVariation;
        }
        void GenerateSpawnColour()
        {
            color.R +=
                (byte)(-colorVariation.R / 2f +
                colorVariation.R * Game.Random.NextDouble());
            color.G +=
                (byte)(-colorVariation.G / 2f +
                colorVariation.G * Game.Random.NextDouble());
            color.B +=
                (byte)(-colorVariation.B / 2f +
                colorVariation.B * Game.Random.NextDouble());
            color.A +=
                (byte)(-colorVariation.A / 2f +
                colorVariation.A * Game.Random.NextDouble());
        }
        void GenerateSpawnScale()
        {
            scale +=
                (-scaleVariation/2f) +
                (float)Game.Random.NextDouble() * scaleVariation;
        }
        void GenerateSpawnRotation()
        {
            rotation +=
                (-rotationVariation/2f) +
                (float)Game.Random.NextDouble() * rotationVariation;
        }
        void GenerateSpawnLifeTime()
        {
            lifeTime +=
                (int)((-lifeTimeVariation/2f) +
                (float)Game.Random.NextDouble() * lifeTimeVariation);
        }
        #endregion

        #region deltas
        public void ApplyFrictionDelta() {
            friction += frictionDelta;
        }
        public void ApplyColorDelta() {
            color.R += colorDelta.R;
            color.G += colorDelta.G;
            color.B += colorDelta.B;
            color.A += colorDelta.A;
        }
        public void ApplyRotationDelta() {
            rotation += rotationDelta;
        }
        public void ApplyScaleDelta() {
            scale += scaleDelta;
        }
        #endregion

        public Particle2 Spawn()
        {
            GenerateSpawnPosition();
            GenerateSpawnVelocity();
            GenerateSpawnFriction();
            GenerateSpawnColour();
            GenerateSpawnScale();
            GenerateSpawnRotation();
            GenerateSpawnLifeTime();
            return this;
        }

        public Particle2 Clone()
        {
            return new Particle2()
                .SetPosition(position, positionVariation)
                .SetVelocity(velocity, velocityVariation)
                .SetFriction(friction, frictionVariation, frictionDelta)
                .SetColour(color, colorVariation, colorDelta)
                .SetScale(scale, scaleVariation, scaleDelta)
                .SetRotation(rotation, rotationVariation, rotationDelta)
                .SetTexture(texture)
                .SetLifeTime(lifeTime, lifeTimeVariation);
        }
    }
}
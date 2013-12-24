using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using IrrKlang;

namespace Apollyon
{
    class Ship : SpaceObject
    {
        public override double Rotation {
            get { return Direction; } }

        public Faction Faction;

        public double Direction;
        public float Speed;
        public float MaxSpeed;
        public float Acceleration;
        public Vector2? TargetPosition;
        public float TurnSpeed = 0.01f; //rad/s

        public List<ShipComponent> Components;

        ISound engineSound = null;

        int health, maxHealth;
        public override int Health
        {
            get { return Shield.Current + health; }
            set { health = value; }
        }

        public override int MaxHealth
        {
            get { return Shield.Max + maxHealth; }
            set { maxHealth = value; }
        }

        public new Vector2 Velocity
        {
            get
            {
                return new Vector2(
                    (float)Math.Cos(Direction) * Speed,
                    (float)Math.Sin(Direction) * Speed
                    );
            }
        }

        Shield Shield;
        public override void Damage(AttackInfo _attack)
        {
            int _damage = _attack.Damage;
            if (Shield.Current > 0)
            {
                Shield.Current -= _damage;
                _damage -= Shield.Current;
                _damage = Math.Max(_damage, 0);
            }
            health -= _damage;
            if (_attack.Source != null)
            {
                Ship _attacker = _attack.Source as Ship;
                if (Faction.GetRelations(Faction, _attacker.Faction) > -1f)
                    Faction.SetRelations(Faction, _attacker.Faction, -1f);
            }
        }

        public Vector2 GetVelocity() {
            return new Vector2(
                (float)Math.Cos(Direction),
                (float)Math.Sin(Direction)) * Speed;
        }

        public List<Item> Inventory;

        public Ship(
            Vector2 _position
        ) {
            Name = ShipNameGenerator.GenerateName();
            Texture = Res.Textures["ship"];
            Position = _position;
            Size = new Vector2(32, 32);
            TargetPosition = null;
            Direction = Math.PI;
            Speed = 0;
            MaxSpeed = 3;
            Acceleration = 0.002f;

            Components = new List<ShipComponent>();
            Shield = new Shield(Game.Random.Next(50, 100), 100);
            health = maxHealth = 100;

            Inventory = new List<Item>();

            Tags = new List<string>();
            Tags.Add("ship");

            Visible = true;
            Depth = 1;

            EngineTrail =
                new Particle2()
                .SetPosition(Vector2.Zero, 4)
                //velocity is the oddball, going px/s instead of px/t
                .SetVelocity(Vector2.Zero, 50)
                .SetFriction(0.05f, 0.03f, 0.01f)
                .SetColour(
                    new Color(1f,0.4f,0f,1f),
                    new Color(0f,0.4f,0f,1f),
                    new Color(0f,0f,0f,0f))
                .SetScale(1.5f, 0.35f, 0f)
                .SetRotation(0, Math.PI*2f, 0) //add delta variation
                .SetTexture(Res.Textures["1x1"])
                .SetLifeTime(600, 350);
        }
        Particle2 EngineTrail;

        public void AddItem(Item _i)
        {
            _i.Carrier = this;
            Utility.Tag(_i, "carried");
            Inventory.Add(_i);
        }

        public void DropItem(Item _i)
        {
            _i.Carrier = null;
            _i.Position = Position;
            _i.Tags.Remove("carried");
            double _angle = Game.Random.NextDouble() * Math.PI * 2;
            _i.Velocity = new Vector2(
                (float)Math.Cos(_angle),
                (float)Math.Sin(_angle)
            ) * (float)(Game.Random.NextDouble()) * (2f/3f);
            _i.Velocity += Velocity/2f;
            Inventory.Remove(_i);
        }

        public void Stop()
        {
            this.TargetPosition = null;
        }

        //return amount scooped
        public int Scoop()
        {
            int _pre = Inventory.Count;
            foreach (SpaceObject _si in
                Game.World.SpaceObjects.FindAll(
                x => x.Tags.Contains("item")))
            {
                Item _i = (Item)_si;
                if (_i.Carrier == null)
                {
                    if (Vector2.Distance(
                            Position,
                            _si.Position
                        ) < 100)
                    {
                        AddItem(_i);
                        //break;
                    }
                }
            }
            return Inventory.Count - _pre;
        }
        
        //rewrite this as Get(string _action), send actions from elsewhere
        public void Input(
            KeyboardState ks, KeyboardState oks,
            MouseState ms, MouseState oms
            )
        {
            if (ks.IsKeyDown(Keys.X) && (!oks.IsKeyDown(Keys.X)))
                if (UIBindings.Get("Selected").Contains(this))
                {
                    health = -1;
                    Shield.Current = -1;
                }

            if (ks.IsKeyDown(Keys.S) && (!oks.IsKeyDown(Keys.S)))
                if (UIBindings.Get("Selected").Contains(this))
                    Stop();

            if (ks.IsKeyDown(Keys.Z) && (!oks.IsKeyDown(Keys.Z)))
                if (UIBindings.Get("Selected").Contains(this))
                    Scoop();
        }

        public override void Die()
        {
            Audio.PlaySoundAtPosition(
                "afx/explosion.wav",
                new Vector3(Position.X, Position.Y, 0)
            );

            while(Inventory.Count > 0)
            {
                Item _i = Inventory[0];
                DropItem(_i);
                _i.Position = Position + new Vector2(
                        Game.Random.Next(-3, 4),
                        Game.Random.Next(-3, 4));
            }

            for (int i = 0; i < 1000; i++)
                Particle2.AddParticle(
                    EngineTrail
                    .Clone()
                        .SetPosition(Position)
                        .SetFriction(0.002f,0.001f,0)
                        .SetScale(2f, 0.75f, -0.001f)
                        .SetVelocity(
                            Utility.RandomDirectionVector() * 35 +
                            GetVelocity() * 50)
                        .SetLifeTime(3000, 1200)
                    .Spawn()
                );

            for (int i = 0; i < 1000; i++)
                Particle2.AddParticle(
                    EngineTrail
                    .Clone()
                        .SetPosition(Position)
                        .SetFriction(0.002f,0.001f,0)
                        .SetScale(2.5f, 0.75f, -0.001f)
                        .SetVelocity(
                            Utility.RandomDirectionVector() * 25 +
                            GetVelocity() * 35)
                        .SetLifeTime(2000, 800)
                    .Spawn()
                );

            UIBindings.Get("All").Remove(this);
        }

        public override void Update() //goes once per tick
        {
            if (engineSound == null)
                engineSound = Audio.PlaySoundAtPosition(
                    "afx/engine2.wav", Position, true, true);

            Audio.SetSoundPosition(engineSound, Position);

            engineSound.Volume = Math.Min(
                Math.Max(Speed, 0), 1
            );

            if (Speed > 0.1f)
            {
                if (engineSound.Paused)
                    engineSound.Paused = false;
                for (int i = 0; i < 10; i++)
                    Particle2.AddParticle(
                        EngineTrail
                        .Clone()
                            .SetPosition(Position)
                            .SetVelocity(
                                Utility.RandomDirectionVector() * 50 +
                                Velocity * 30)
                        .Spawn()
                    );
            }
            else
            {
                engineSound.Paused = true;
            }

            foreach (ShipComponent _c in Components)
                _c.Tick();

            if (!TargetPosition.HasValue && Speed > 0)
            {
                Speed -= Acceleration * 5; //easier to stop for some reason
                //just feels better is a good enough reason, I guess
                Speed = Math.Max(Speed, 0);
            }
            else if (TargetPosition.HasValue)
            {

                float _d = Vector2.Distance(Position, TargetPosition.Value);

                //if (_d < 0.5f) return; //do something to stop on the spot turning

                float _magicNumber = 60;
                float _targetSpeed = _d < _magicNumber * MaxSpeed ?
                    _d / (_magicNumber * MaxSpeed) : MaxSpeed;
                if (Math.Abs(Speed - _targetSpeed) < Acceleration)
                    Speed = _targetSpeed;
                if (Speed < _targetSpeed) Speed += Acceleration;
                if (Speed > _targetSpeed) Speed -= Acceleration * 10;

                double _targetDirection = Math.Atan2(
                    TargetPosition.Value.Y - Position.Y,
                    TargetPosition.Value.X - Position.X);

                var a = _targetDirection;
                var b = Direction;

                a = a <= 0 ? a + Math.PI * 2 : a;
                b = b <= 0 ? b + Math.PI * 2 : b;
                b = b - a;
                a = 0;
                b = b <= 0 ? b + Math.PI * 2 : b;

                if (b > Math.PI)
                    Direction += TurnSpeed;
                else
                    Direction -= TurnSpeed;

                while (Direction > Math.PI * 2)
                    Direction -= Math.PI * 2;
                while (Direction < 0)
                    Direction += Math.PI * 2;
            }

            Position.X += (float)Math.Cos(Direction) * Speed;
            Position.Y += (float)Math.Sin(Direction) * Speed;
        }
    }
}

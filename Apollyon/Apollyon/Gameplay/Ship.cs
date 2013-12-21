using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Ship : SpaceObject
    {
        public override double Rotation {
            get { return Direction; } }

        public double Direction;
        public float Speed;
        public float MaxSpeed;
        public float Acceleration;
        public Vector2 TargetPosition;
        public float TurnSpeed = 0.01f; //rad/s

        public List<ShipComponent> Components;

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

        Shield Shield;
        //public override void Damage(int _damage)
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
        }

        public List<Item> Inventory;

        public Ship(
            Vector2 _position
        ) {
            Name = ShipNameGenerator.GenerateName();
            Texture = Res.Ship;
            Position = _position;
            Size = new Vector2(32, 32);
            TargetPosition = Position;
            Direction = Math.PI;
            Speed = 0;
            MaxSpeed = 3;
            Acceleration = 0.002f;

            Components = new List<ShipComponent>();
            Shield = new Shield(Game.Random.Next(50, 100), 100);
            Health = MaxHealth = 100;

            Inventory = new List<Item>();

            Tags = new List<string>();
            Tags.Add("ship");

            Visible = true;
            Depth = 1;
        }

        public void AddComponent(ShipComponent _sc)
        {
            Components.Add(_sc);
            _sc.Parent = this;
        }

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
            _i.Velocity = new Vector2(
                -0.5f + (float)Game.Random.NextDouble(),
                -0.5f + (float)Game.Random.NextDouble());
            Inventory.Remove(_i);
        }

        public void Input(
            KeyboardState ks, KeyboardState oks,
            MouseState ms, MouseState oms
            )
        {
            if (ks.IsKeyDown(Keys.X) && (!oks.IsKeyDown(Keys.X)))
            {
                if (UIBindings.Get("Selected").Contains(this))
                {
                    Shield.Current = -1;
                }
            }
            if (ks.IsKeyDown(Keys.S) && (!oks.IsKeyDown(Keys.S)))
            {
                if (UIBindings.Get("Selected").Contains(this))
                {
                    this.TargetPosition = Position+
                        new Vector2(
                            (float)Math.Cos(Direction)*Speed*15,
                            (float)Math.Sin(Direction)*Speed*15);
                }
            }
            if (ks.IsKeyDown(Keys.Z) && (!oks.IsKeyDown(Keys.Z)))
            {
                if (UIBindings.Get("Selected").Contains(this))
                {
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
                }
            }
        }

        public override void Die()
        {
            while(Inventory.Count > 0)
            {
                Item _i = Inventory[0];
                DropItem(_i);
                _i.Position = Position + new Vector2(
                        Game.Random.Next(-3, 4),
                        Game.Random.Next(-3, 4));
            }

            new ParticleSpawn(
                1000,
                new Particle(
                    new Vector2(
                        Position.X,
                        Position.Y),
                    Res.OneByOne,
                    new Color(1f, 0.4f, 0f, 1f),
                    900,
                    new Vector4(0f, 0f, 0f, -0.5f), //does not work
                //why does xna seem to drop the A of the colour above
                //completely all of a sudden..?
                    0,
                    6,
                    0.05f
                )
            )
            .RandomizeLifeTime(200)
            .RandomizeSpeed(5.5f)
            .RandomizePosition(new Vector2(8, 8))
            .RandomizeDirection((float)Math.PI*2f)
            .RandomizeColor(new Color(0f, 1f, 0f, 0f))
            .Spawn();

            UIBindings.Get("All").Remove(this);
        }

        public override void Update() //goes once per tick
        {
            if (Speed > 0.1f)
            {
                //would love to save this somewhere else and just spawn here
                new ParticleSpawn(
                    10,
                    new Particle(
                        new Vector2(
                            Position.X,
                            Position.Y),
                        Res.OneByOne,
                        new Color(1f, 0.2f, 0f, 1f),
                        300,
                        new Vector4(0f, 0f, 0f, -0.5f), //does not work
                    //why does xna seem to drop the A of the colour above
                    //completely all of a sudden..?
                        Direction + Math.PI,
                        Speed * Speed,
                        0.1f
                    )
                )
                .RandomizeSpeed(1.5f)
                .RandomizePosition(new Vector2(8, 8))
                .RandomizeDirection((float)Math.PI / 8f)
                .RandomizeColor(new Color(0f, 1f, 0f, 0f))
                .Spawn();
            }

            foreach (ShipComponent _c in Components)
            {
                _c.Tick();
            }

            float _d = Vector2.Distance(Position, TargetPosition);

            //if (_d < 0.5f) return; //do something to stop on the spot turning

            float _magicNumber = 60;
            float _targetSpeed = _d < _magicNumber * MaxSpeed ?
                _d / (_magicNumber * MaxSpeed) : MaxSpeed;
            if (Math.Abs(Speed-_targetSpeed) < Acceleration)
                Speed = _targetSpeed;
            if (Speed < _targetSpeed) Speed += Acceleration;
            if (Speed > _targetSpeed) Speed -= Acceleration*10;

            Position.X += (float)Math.Cos(Direction) * Speed;
            Position.Y += (float)Math.Sin(Direction) * Speed;

            double _targetDirection = Math.Atan2(
                TargetPosition.Y - Position.Y,
                TargetPosition.X - Position.X);

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
    }
}

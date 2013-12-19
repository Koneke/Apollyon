using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class Ship
    {
        public string Name;

        public Vector2 Position;
        public double Direction;
        public float Speed;
        public float MaxSpeed;
        public float Acceleration;
        public Vector2 TargetPosition;
        public float TurnSpeed = 0.01f; //rad/s

        public List<ShipComponent> Components;
        public Shield Shield;
        //public Armor Armor;

        public List<Item> Inventory;

        //public bool Friendly;

        public Ship()
        {
            Name = ShipNameGenerator.GenerateName();
            Position = new Vector2(0, 0);
            Direction = Math.PI;
            Speed = 0;
            MaxSpeed = 3;
            Acceleration = 0.002f;

            Components = new List<ShipComponent>();
            Shield = new Shield(Game.Random.Next(50, 100), 100);

            Inventory = new List<Item>();
        }

        public void AddComponent(ShipComponent _sc)
        {
            Components.Add(_sc);
            _sc.Parent = this;
        }

        public void Update() //goes once per tick
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

            float _d = Vector2.Distance(Position, TargetPosition);
            //Speed = 1;
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

            foreach (ShipComponent _c in Components)
            {
                _c.Tick();
            }
        }
    }
}

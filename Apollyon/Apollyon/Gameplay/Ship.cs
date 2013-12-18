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
        public Vector2 TargetPosition;
        public float TurnSpeed = 0.06f; //rad/s

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

            Components = new List<ShipComponent>();
            Shield = new Shield(Game.Random.Next(50, 100), 100);

            Inventory = new List<Item>();
        }

        public void AddComponent(ShipComponent _sc)
        {
            Components.Add(_sc);
            _sc.Parent = this;
        }

        public void Update()
        {
            //if (Speed == 0) return; //HACK DEBUG ETC.

            float _d = Vector2.Distance(Position, TargetPosition);
            Speed = 1;
            Speed = _d < 10 * Speed ? _d / (10 * Speed) : Speed;

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
                Direction += 0.05f;
            else
                Direction -= 0.05f;

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

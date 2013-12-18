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

        //public bool Friendly;

        public Ship()
        {
            Name = ShipNameGenerator.GenerateName();
            Position = new Vector2(0, 0);
            Direction = Math.PI;
            Speed = 0;

            Components = new List<ShipComponent>();
            Shield = new Shield(Game.Random.Next(50, 100), 100);
        }

        public void AddComponent(ShipComponent _sc)
        {
            Components.Add(_sc);
            _sc.Parent = this;
        }

        public void Update()
        {
            Position.X += (float)Math.Cos(Direction) * Speed;
            Position.Y += (float)Math.Sin(Direction) * Speed;

            foreach (ShipComponent _c in Components)
            {
                _c.Tick();
            }
        }
    }
}

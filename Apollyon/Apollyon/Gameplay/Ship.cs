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

        public List<Ship> Targets;
        public Game.TargetingType TargetingType;

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

            Targets = new List<Ship>();
            //choose random from the list of targets.
            //adding support for weakest, strongest etc. later on
            TargetingType = Game.TargetingType.Random;

            Components = new List<ShipComponent>();
            Shield = new Shield(Game.Random.Next(50, 100), 100);
        }

        public void Update()
        {
            //WARNING: CURRENTLY RUNNING FRAME DEPENDANT.
            //         SWITCH TO DT?

            //holy shit this entire function is a cluster fuck of ship turning
            //jesus christ
            //something something cross product of vectors
            //i heard that stuff is cool
            //this first bit is ok though, i.e. the only bit not commented out

            Position.X += (float)Math.Cos(Direction) * Speed;
            Position.Y += (float)Math.Sin(Direction) * Speed;
            foreach (ShipComponent _c in Components)
            {
                if(_c.Active) _c.Tick();
            }

            /*double _trgtDir = Math.Atan2(
                TargetPosition.Y - Position.Y,
                TargetPosition.X - Position.X);

            float _dx = (float)Math.Cos(Direction);
            float _dy = (float)Math.Sin(Direction);
            float _tx = (float)Math.Cos(_trgtDir);
            float _ty = (float)Math.Sin(_trgtDir);
            double _corrAng = Math.Atan2(_ty - _dy, _tx - _dx);
            _dx += (float)Math.Cos(_corrAng) * TurnSpeed;
            _dy += (float)Math.Sin(_corrAng) * TurnSpeed;

            Direction = Math.Atan2(_dy, _dx);*/

            //if (_trgtDir > Direction) Direction += TurnSpeed;
            //if (_trgtDir < Direction) Direction -= TurnSpeed;
        }
    }
}

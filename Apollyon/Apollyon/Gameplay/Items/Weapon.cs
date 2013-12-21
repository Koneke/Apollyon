using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class Weapon : ShipComponent
    {
        public int Damage;
        public int BeamThickness; //we don't really want this here
        //make more data driven etc., never repeated enough

        public Weapon(string _name, int _id) : base(_name, _id)
        {
            Frequency = 60;
            Damage = 3;
            BeamThickness = 1;
        }

        public override void Fire() {
            SpaceObject _target = null;

            switch (TargetingType)
            {
                case Game.TargetingType.Random:
                    _target = Targets[Game.Random.Next(0, Targets.Count)];
                    while (
                        _target.Health <= 0/* ||
                        !_target.Tags.Contains("ship"))*/
                        &&
                        Targets.Count > 0
                        )
                    {
                        Targets.Remove(_target);
                        if (Targets.Count == 0) break;
                        _target = Targets[Game.Random.Next(0, Targets.Count)];
                    }
                    break;
                default:
                    break;
            }
            if (Targets.Count == 0)
            {
                Active = false;
                return;
            }

            if (_target != null)
            {
                //((Ship)_target).Shield.Current -= Damage;
                //(_target as Ship).Damage(Damage);
                _target.Damage(Damage);
                Game.Log(Parent.Name + " dealt " + Damage +
                    " points of damage to " + _target.Name + " using " +
                    this.Name + ".");
            }

            Vector2 _hitPosition = _target.Position;
            
            double _ang = Math.Atan2(
                _target.Position.Y - Parent.Position.Y,
                _target.Position.X - Parent.Position.X
            );

            float _d = Vector2.Distance(
                Parent.Position,
                _target.Position
            ); //TODO: Change me when missing to see the shot whizz past

            double _rand = 0.07f;
            _ang -= _rand/2f;
            _ang += _rand * Game.Random.NextDouble();
            _hitPosition.X = Parent.Position.X+(float)Math.Cos(_ang) * _d;
            _hitPosition.Y = Parent.Position.Y+(float)Math.Sin(_ang) * _d;

            Particle.Particles.Add(
                new LineParticle(
                    Parent.Position,
                    _hitPosition,
                    BeamThickness,
                    new Color(1f, 0.7f, 0f, 1f),
                    100
                )
            );

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Apollyon
{
    class AttackInfo
    {
        public Weapon Weapon;
        public int Damage;
        public SpaceObject Source; //null = world/unknown

        public AttackInfo(
            Weapon _w,
            int _dmg,
            SpaceObject _src
        ) {
            Weapon = _w;
            Damage = _dmg;
            Source = _src;
        }
    }

    class Weapon : ShipComponent
    {
        public int Damage;
        public int BeamThickness; //we don't really want this here
        //make more data driven etc., never repeated enough
        public Color BeamTint; //important ok
        public int Range;

        public Weapon(string _name, int _id) : base(_name, _id)
        {
            Frequency = 60;
            Damage = 3;
            BeamThickness = 1;
            BeamTint = new Color(1f, 0.7f, 0f, 1f);
            Range = 0;
        }

        public override void Fire() {
            SpaceObject _target = null;

            Targets = Targets.FindAll(
                x => Vector2.Distance(x.Position, Carrier.Position) < Range);

            if (Targets.Count == 0)
            {
                Active = false;
                return;
            }

            switch (TargetingType)
            {
                case Game.TargetingType.Random:
                    _target = Targets[Game.Random.Next(0, Targets.Count)];
                    while (_target.Health <= 0 && Targets.Count > 0)
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
                _target.Damage(new AttackInfo(
                    this, Damage, Carrier));
            }

            Vector2 _hitPosition = _target.Position;
            
            double _ang = Math.Atan2(
                _target.Position.Y - Carrier.Position.Y,
                _target.Position.X - Carrier.Position.X
            );

            float _d = Vector2.Distance(
                Carrier.Position,
                _target.Position
            ); //TODO: Change me when missing to see the shot whizz past

            double _rand = 0.07f;
            _ang -= _rand/2f;
            _ang += _rand * Game.Random.NextDouble();
            _hitPosition.X = Carrier.Position.X+(float)Math.Cos(_ang) * _d;
            _hitPosition.Y = Carrier.Position.Y+(float)Math.Sin(_ang) * _d;

            Particle.Particles.Add(
                new LineParticle(
                    Carrier.Position,
                    _hitPosition,
                    BeamThickness,
                    BeamTint,
                    100
                )
            );

            Audio.PlaySoundAtPosition(
                "afx/explosion.wav",
                new Vector3(
                    Carrier.Position.X,
                    Carrier.Position.Y, 0)
            );
        }
    }
}

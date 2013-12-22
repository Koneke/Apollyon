using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class SpaceObject
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Size { get; set; }
        public virtual double Rotation { get; set; }
        public List<String> Tags { get; set; }
        public virtual bool Visible { get; set; }
        public float Depth { get; set; }
        public virtual int Health { get; set; }
        public virtual int MaxHealth { get; set; }

        public virtual void Damage(AttackInfo _attack) {
            Health -= _attack.Damage; }

        public virtual void Update() {
            Position += Velocity; }

        //fluent for conveniency and Vector2 is a dumb struct;
        public SpaceObject SetPosition(Vector2 _p) {
            Position = _p; return this; }

        public SpaceObject SetVelocity(Vector2 _v) {
            Velocity = _v; return this; }

        public SpaceObject()
        {
            Visible = true;
            Tags = new List<string>();
            Health = 1;
            MaxHealth = 1;
        }

        public void SetTags(List<string> _tags) {
            Tags = _tags; }

        public bool HasTag(string _tag) {
            return Tags.Contains(_tag); }

        public virtual void Die() { }
    }
}

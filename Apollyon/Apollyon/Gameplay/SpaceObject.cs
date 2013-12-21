using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    public class SpaceObject
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position;
        public Vector2 Size { get; set; }
        public virtual double Rotation { get; set; }
        public List<String> Tags { get; set; }
        public virtual bool Visible { get; set; }
        public float Depth { get; set; }

        //fluent for conveniency and Vector2 is a dumb struct;
        public SpaceObject SetPosition(Vector2 _p)
        {
            Position = _p;
            return this;
        }

        public SpaceObject()
        {
            Visible = true;
            Tags = new List<string>();
        }

        public void SetTags(List<string> _tags)
        {
            Tags = _tags;
        }
        public bool HasTag(string _tag) {
            return Tags.Contains(_tag);
        }
    }
}

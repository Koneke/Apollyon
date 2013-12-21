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
        public Vector2 Position;// { get; set; }
        public Vector2 Size { get; set; }
        public virtual double Rotation { get; set; }
        public List<String> Tags { get; set; }
        public virtual bool Visible { get; set; }
        public float Depth { get; set; }

        public SpaceObject()
        {
            Visible = true;
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

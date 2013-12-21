using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Container : ISpaceObject
    {
        public string Name;
        public string GetName() {
            return Name; }

        public Vector2 Position;
        public Vector2 GetPosition() {
            return Position; }

        public Vector2 GetSize() {
            return new Vector2(32, 32); }

        public Texture2D GetTexture() {
            return Res.Ship; }

        public List<string> Tags;
        public List<string> GetTags() {
            return Tags; }
        public void SetTags(List<string> _tags) {
            Tags = _tags; }
        public bool HasTag(string _tag) {
            return Tags.Contains(_tag.ToLower()); }

        public bool GetVisible() {
            return true; }

        public double GetRotation() {
            return 0; }

        public float GetDepth() { //0 middle, <0 behind, >0 in front
            return 1; }
    }
}

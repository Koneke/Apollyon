using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class Item : ISpaceObject
    {
        //iso impl
        public string Name;
        public string GetName() {
            return Name; }

        public Vector2 Position;
        public Vector2 GetPosition() {
            return Position; }

        public Vector2 GetSize() {
            return new Vector2(16, 16); }

        public Texture2D Texture;
        public Texture2D GetTexture() {
            return Texture; }

        public List<string> Tags;
        public List<string> GetTags() {
            return Tags; }
        public bool HasTag(string _tag) {
            return Tags.Contains(_tag.ToLower()); }

        public bool GetVisible() {
            return Carrier == null; }

        public double GetRotation() { return 0; }

        public float GetDepth() {
            return 0f; }

        /*-------*/

        public Ship Carrier;

        public int Count;
        public Boolean Stacking;
        public int ID;

        public Item(
            string _name = "",
            int _id = -1,
            bool _stacking = false,
            int _count = 1
        ) {
            Name = _name;
            ID = _id;
            Stacking = _stacking;
            Count = _count;
            Carrier = null;
            Tags = new List<string>();
            Tags.Add("Item");
        }

        /*optional bits*/
        public ShipComponent Component;

        public void Use()
        {
            RealUse();
        }

        public virtual void RealUse()
        {
            if (Component != null)
            {
                Carrier.Inventory.Remove(this);
                Carrier.Components.Add(Component);
                Component.Parent = Carrier;
            }
        }
    }
}

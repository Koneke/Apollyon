using System;
using System.Text;

namespace Apollyon
{
    class Item
    {
        public string Name;
        public int Count;
        public Boolean Stacking;

        public Item(
            string _name = "",
            bool _stacking = false,
            int _count = 1
        ) {
            Name = _name;
            Stacking = _stacking;
            Count = _count;
        }

        public virtual void Use(Ship _user)
        {
        }
    }
}

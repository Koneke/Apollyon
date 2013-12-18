using System;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    public enum KeyBindType
    {
        Press,
        Hold,
        Release
    }

    class ApWindowKeyBind
    {
        public string Action;
        public Keys Key;
        public KeyBindType BindType;
        public ApWindow Window;

        public ApWindowKeyBind(
            string _action,
            Keys _key,
            KeyBindType _type,
            ApWindow _window
        ) {
            Action = _action;
            Key = _key;
            BindType = _type;
            Window = _window;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    static class BindingsManager
    {
        public static List<ApWindowKeyBind> WindowKeyBindings =
            new List<ApWindowKeyBind>();

        static Keys[] lastKeys;

        public static void HandleInput()
        {
            HandleInput(InputManager.ks.GetPressedKeys());
        }
        public static void HandleInput(
            Keys[] _keys
        ) {
            List<Keys> _keyList = new List<Keys>(_keys);
            List<Keys> _oldKeyList = new List<Keys>(lastKeys??new Keys[]{});

            foreach(Keys _key in _keyList)
            {
                foreach (ApWindowKeyBind _b in WindowKeyBindings)
                {
                    if (_key == _b.Key)
                    {
                        if (
                            (!_oldKeyList.Contains(_key) &&
                            _b.BindType == KeyBindType.Press) ||
                            (_oldKeyList.Contains(_key) &&
                            _b.BindType == KeyBindType.Hold)
                        ) {
                            _b.Window.GetAction(_b.Action);
                        }
                    }
                }
            }

            foreach (Keys _oldKey in _oldKeyList)
            {
                foreach (ApWindowKeyBind _b in WindowKeyBindings)
                {
                    if (_oldKey == _b.Key)
                    {
                        if(
                            !_keyList.Contains(_oldKey) &&
                            _b.BindType == KeyBindType.Release
                        ) {
                            _b.Window.GetAction(_b.Action);
                        }
                    }
                }
            }

            lastKeys = _keys;
        }
    }
}

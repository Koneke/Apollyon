using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    //raw input stuff like mouse/kb
    class InputManager
    {
        public static MouseState ms, oms;
        public static KeyboardState ks, oks;
        public GameState MyState;

        public static void UpdateStart()
        {
            ks = Keyboard.GetState();
            ms = Mouse.GetState();
            Game.MouseWheelDelta =
                -(ms.ScrollWheelValue - oms.ScrollWheelValue)/120;
        }

        public static void UpdateEnd()
        {
            oks = ks;
            oms = ms;
        }

        public List<ApKeyBind> KeyBinds;

        public InputManager()
        {
            KeyBinds = new List<ApKeyBind>();
        }

        public void Bind(ApKeyBind _abk, IInputReceiver _sender)
        {
            //ugly BUT it works
            _abk.Sender = _sender;
            KeyBinds.Add(_abk);
        }

        public void Update()
        {
            List<Keys> _keyList = new List<Keys>(
                ks.GetPressedKeys());
            List<Keys> _oldKeyList = new List<Keys>(
                oks.GetPressedKeys()??new Keys[]{});

            foreach(Keys _key in _keyList)
            {
                foreach (ApKeyBind _b in KeyBinds)
                {
                    if (_key == _b.Key)
                    {
                        if (
                            (
                                //if key was just pressed
                                !_oldKeyList.Contains(_key) &&
                                _b.BindType == KeyBindType.Press
                                //or
                            ) || (
                                //if key is held
                                _oldKeyList.Contains(_key) &&
                                _b.BindType == KeyBindType.Hold
                            )
                        ) {
                            //signify binder that the input was made
                            _b.Sender.Receive(_b);
                        }
                    }
                }
            }

            foreach(Keys _key in _oldKeyList)
            {
                foreach (ApKeyBind _b in KeyBinds)
                {
                    if (_key == _b.Key)
                    {
                        if (
                                //if key was just released 
                                !_keyList.Contains(_key) &&
                                _b.BindType == KeyBindType.Release
                        ) {
                            //signify binder that the input was made
                            _b.Sender.Receive(_b);
                        }
                    }
                }
            }
        }
    }
}

using System;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    //raw input stuff like mouse/kb
    class InputManager
    {
        public static MouseState ms, oms;
        public static KeyboardState ks, oks;

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
    }
}

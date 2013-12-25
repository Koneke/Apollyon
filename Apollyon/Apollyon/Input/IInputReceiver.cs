using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    public enum KeyBindType
    {
        Press,
        Hold,
        Release
    }

    //move to its own file ok
    class ApKeyBind
    {
        public string Action;
        public Keys Key;
        public KeyBindType BindType;
        public IInputReceiver Sender;

        public ApKeyBind(
            string _action,
            Keys _key,
            KeyBindType _type
        ) {
            Action = _action;
            Key = _key;
            BindType = _type;
        }
    }

    //Whatever kind of class implementing this is free to bind keys and then
    //receive them with a switch or whatever, idc. just make bindings central
    //fuck.
    interface IInputReceiver
    {
        void Bind(ApKeyBind _akb);
        void Receive(ApKeyBind _akb);
    }
}

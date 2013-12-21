using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApOverview : ApWindow
    {
        int indent = 4; //intendent from the border in the list

        public List<string> Sources; //list of all tags looked for in space

        public ApOverview(
            int _x, int _y, int _w, int _h
            ) : base(_x, _y, _w, _h)
        {
        }

        public override void Update()
        {
        }

        public override void SpecificUILoading() { }
        public override void GetAction(string _action) { }
        public override void ActualRender(SpriteBatch spriteBatch) { }
        public override void OwnInput(MouseState ms, MouseState oms) { }
    }
}

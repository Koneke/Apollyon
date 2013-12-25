using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class WindowManager
    {
        public static List<ApWindow> Windows = new List<ApWindow>();
        public static ApWindow GetWindowByName(string _name)
        {
            return Windows.Find(x => x.Name.Equals(_name));
        }

        public static void Load()
        {
            foreach (ApWindow _w in Windows)
                _w.SpecificUILoading();
        }

        public static void RenderAll(SpriteBatch spriteBatch)
        {
            foreach (ApWindow w in Windows)
                w.Render(spriteBatch);
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (ApWindow w in Windows)
                w.Draw(spriteBatch);
        }
    }
}

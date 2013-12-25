using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApLogWindow : ApWindow
    {
        public List<string> Log;
        int indent = 4;

        public ApLogWindow(
            int _x, int _y, int _w, int _h
            )
            : base (_x, _y, _w, _h) {
                Log = new List<string>();
        }

        public override void ActualRender(SpriteBatch spriteBatch)
        {
            graphics.Clear(ApLogWindow.StandardBackground);

            spriteBatch.Begin();

            float _currentY = 0;
            for(int i = Log.Count-1; i >= 0; i--) {
                string message = Log[i];

                if (h - _currentY < 0) break;

                string _string = Utility.WrapText(
                    Res.GetFont("log font"),
                    message,
                    w-2*indent,
                    true //indent past first line
                );

                float _offs = Res.GetFont("log font").MeasureString(_string).Y;

                spriteBatch.DrawString(
                    Res.GetFont("log font"),
                    _string,
                    new Vector2(indent, h-_offs-_currentY),
                    Color.White
                );
                _currentY += _offs;
            }

            spriteBatch.End();

            DrawBorder(spriteBatch, ApWindow.StandardBorder);
        }

        public override void Receive(ApKeyBind _akb)
        {
            string _action = _akb.Action;
            switch(_action) {
                case "Clear Log":
                    Log.Clear();
                    break;
                default:
                    Log.Add("Unrecognized action \"" + _action + "\".");
                    break;
            }
        }
    }
}

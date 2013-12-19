using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApStatusWindow : ApWindow
    {
        public List<Ship> Ships;

        public ApStatusWindow(
            int _x, int _y, int _w, int _h
            )
            : base (_x, _y, _w, _h) {
        }

        public override void SpecificUILoading()
        {
            switch (xml.Element("ships").Value)
            {
                //in the future, let lists register themselves with a
                //name so that we can do something like
                //GetList("Selected Ships") or similar.

                case "ShipOverview.Selection":
                    Ships = ApUI.ShipOverview.Selection;
                    break;
                case "HostileOverview.Selection":
                    Ships = ApUI.HostileOverview.Selection;
                    break;
                default: break;
            }
        }

        public override void ActualRender(SpriteBatch spriteBatch)
        {
            graphics.Clear(
                Utility.MultiplyColours(
                    ApLogWindow.StandardBackground,
                    Tint
                )
            );

            int _sideMargin = 4;
            int _topMargin = 12; //between top bar and top, vv bottom
            //int _padding = 4; //between bars

            float _percentage = 0;
            if (Ships != null)
            {
                foreach (Ship _s in Ships)
                    _percentage += _s.Shield.getPercentage();
                _percentage /= Ships.Count;
            }

            Utility.DrawOutlinedRectangle(
                spriteBatch,
                new Rectangle(
                    _sideMargin,
                    _topMargin,
                    (int)(w - (_sideMargin * 2)),
                    (int)(h - (_topMargin * 2))
                    ),
                ApWindow.StandardBorder
            );
                    

            spriteBatch.Begin();

            spriteBatch.DrawString(
                Res.LogFont,
                "Avg. Shield",
                new Vector2(_sideMargin + 4, _topMargin + 2),
                Color.White);

            if ((Ships??new List<Ship>{}).Count != 0)
            {
                spriteBatch.Draw(
                    Res.OneByOne,
                    new Rectangle(
                        _sideMargin+1,
                        _topMargin+1,
                        (int)((w - ((_sideMargin+1) * 2)) * _percentage / 100f),
                        (int)h - ((_topMargin+1) * 2)
                    ),
                    Utility.MultiplyColours(
                        ApWindow.StandardBorder,
                        new Color(1f, 1f, 1f, 0.4f)
                    )
                );
            }

            spriteBatch.End();

            DrawBorder(spriteBatch, ApWindow.StandardBorder);
        }
    }
}

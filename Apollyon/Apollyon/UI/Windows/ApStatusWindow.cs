using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApStatusWindow : ApWindow
    {
        public string Ships; //ships to read from, e.g. "selected"

        public ApStatusWindow(
            int _x, int _y, int _w, int _h
            )
            : base (_x, _y, _w, _h) {
        }

        public override void SpecificUILoading()
        {
            Ships = xml.Element("ships").Value;
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

            float _percentage = 0;
            if (UIBindings.Get(Ships) != null)
            {
                foreach (Ship _s in
                    UIBindings.Get(Ships).FindAll(x => x.HasTag("ship")))
                    //semi hackish, mainly here to let it keep working while we
                    //implement targeting things which are not ships
                    _percentage += 100*((float)_s.Health/_s.MaxHealth);
                _percentage /= UIBindings.Get(Ships).Count;
            }

            Utility.DrawOutlinedRectangle(
                spriteBatch,
                new Rectangle(
                    (int)(x1+_sideMargin),
                    (int)(y1+_topMargin),
                    (int)(w - (_sideMargin * 2)),
                    (int)(h - (_topMargin * 2))
                    ),
                ApWindow.StandardBorder
            );
                    
            spriteBatch.Begin();

            spriteBatch.DrawString(
                Res.GetFont("log font"),
                "Average Health",
                new Vector2(_sideMargin + 4, _topMargin + 2),
                Color.White);

            if (UIBindings.Get(Ships) != null)
            {
                if (UIBindings.Get(Ships).Count != 0)
                {
                    spriteBatch.Draw(
                        Res.Textures["1x1"],
                        new Rectangle(
                            _sideMargin + 1,
                            _topMargin + 1,
                            (int)((w - ((_sideMargin + 1) * 2))
                                * _percentage / 100f),
                            (int)h - ((_topMargin + 1) * 2)
                        ),
                        Utility.MultiplyColours(
                            ApWindow.StandardBorder,
                            new Color(1f, 1f, 1f, 0.4f)
                        )
                    );
                }
            }

            spriteBatch.End();

            DrawBorder(spriteBatch, ApWindow.StandardBorder);
        }
    }
}

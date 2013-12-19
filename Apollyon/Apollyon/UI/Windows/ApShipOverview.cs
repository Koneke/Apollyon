using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApShipOverview : ApWindow
    {
        public string Ships;
        public string Selection;
        int indent = 4;

        public ApShipOverview (
            int _x, int _y, int _w, int _h
            )
            : base (_x, _y, _w, _h) {
        }

        public override void SpecificUILoading()
        {
            Ships = xml.Element("ships").Value;
            Selection = xml.Element("selection").Value;
        }
            
        public override void GetAction(string _action)
        {
            switch (_action)
            {
                case "Clear Selection":
                    UIBindings.Get(Selection).Clear();
                    break;
                default:
                    break;
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

            spriteBatch.Begin();

            float _currentY = 0;
            float _offs = Res.LogFont.MeasureString("ship").Y;

            foreach (Ship _s in
                Game.World.SpaceObjects.FindAll(
                x => x.GetTags().Contains("Ship"))
                )
            {
                if (UIBindings.Get(Selection).Contains(_s))
                {
                    spriteBatch.Draw(
                        Res.OneByOne,
                        new Rectangle(
                            0,
                            +1 + (int)_currentY,
                            (int)w,
                            -1 + (int)_offs
                        ),
                        Utility.MultiplyColours(
                            ApWindow.StandardBorder,
                            new Color(1f, 1f, 1f, 0.3f)
                        )
                    );
                }

                spriteBatch.DrawString(
                    Res.LogFont,
                    _s.Name,
                    new Vector2(
                        indent,
                        _currentY
                    ),
                    Color.White
                );
                _currentY += _offs;
            }

            spriteBatch.End();

            DrawBorder(spriteBatch, ApWindow.StandardBorder);
        }

        public override void OwnInput(MouseState ms, MouseState oms)
        {
            if (
                ms.LeftButton == ButtonState.Pressed &&
                oms.LeftButton == ButtonState.Released)
            {
                float _itemHeight = Res.LogFont.MeasureString("ship").Y;
                float _mouseY = ms.Y - y1;
                float _item = _mouseY - (_mouseY % _itemHeight);
                _item /= _itemHeight;
                int _index = (int)_item;

                List<ISpaceObject> _q =
                    Utility.QuerySpace(new List<string> { "Ship" });

                //if (_item >= UIBindings.Get(Ships).Count) {
                if (_index >= _q.Count)
                {
                    UIBindings.Get(Selection).Clear();
                    return;
                }

                /*int _shipIndex = UIBindings.Get(Ships).IndexOf(
                    UIBindings.Get(Ships)[(int)_item]
                );*/

                if (_q.Count == 0) return;

                Ship _s = (Ship)_q[_index];
                /*Ship _s = (Ship)Utility.QuerySpace(
                    new List<string> { "Ship" })[_index];*/

                if (UIBindings.Get(Selection).Contains(_s))
                {
                    UIBindings.Get(Selection).Remove(_s);
                }
                else
                {
                    UIBindings.Get(Selection).Add(_s);
                }

                /*//deselect
                if (_shipIndex != -1)
                {
                    UIBindings.Get(Selection).
                        RemoveAt(_shipIndex);
                }
                
                //select
                else
                {
                    UIBindings.Get(Selection).
                        Add(UIBindings.Get(Selection)[(int)_item]);
                }*/
            }
        }
    }
}

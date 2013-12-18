using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApShipOverview : ApWindow
    {
        public List<Ship> ShipList; //list to overview
        int indent = 4;
        public List<Ship> Selection;

        public ApShipOverview (
            int _x, int _y, int _w, int _h
            )
            : base (_x, _y, _w, _h) {

            //other list is not created here since it is actually just taking
            //a list from somewhere else to overview
            //we'll have to occasionally cleanup the selection list from items
            //not in the ordinary list, I guess.
            //feels a bit clutzy atm
            Selection = new List<Ship>();
        }

        public override void GetAction(string _action)
        {
            switch (_action)
            {
                case "Clear Selection":
                    Selection.Clear();

                    //HACK: UGLY, UGLY HACK. DO SOMETHING ABOUT IT.
                    if (this == ApUI.ShipOverview)
                    {
                        ApUI.ComponentOverview.UpdateList();
                        ApUI.Inventory.UpdateList();
                    }
                    else
                        ApUI.HostileComponentOverview.UpdateList();

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

            if (ShipList != null)
            {
                spriteBatch.Begin();

                float _currentY = 0;
                float _offs = Res.LogFont.MeasureString("ship").Y;

                foreach (Ship _s in ShipList)
                {
                    if (Selection.Contains(_s))
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
                        Selection.Contains(_s) ? Color.Yellow : Color.White
                    );
                    _currentY += _offs;
                }

                spriteBatch.End();
            }

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

                if (_item >= ShipList.Count) {
                    Selection.Clear();
                    return;
                }

                int _shipIndex = Selection.IndexOf(
                    ShipList[(int)_item]
                );

                //deselect
                if (_shipIndex != -1)
                {
                    Selection.RemoveAt(_shipIndex);
                }
                
                //select
                else
                {
                    Selection.Add(ShipList[(int)_item]);
                }

                //HACK: UGLY, UGLY HACK. DO SOMETHING ABOUT IT.
                if (this == ApUI.ShipOverview)
                {
                    ApUI.ComponentOverview.UpdateList();
                    ApUI.Inventory.UpdateList();
                }
                else
                    ApUI.HostileComponentOverview.UpdateList();
            }
        }
    }
}

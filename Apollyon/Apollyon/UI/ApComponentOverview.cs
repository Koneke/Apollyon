using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApComponentOverview : ApWindow
    {
        public List<Ship> Ships;
        public List<ShipComponent> ComponentList; //list to overview
        int indent = 4;
        public int Selection = -1;
        DateTime lastLeftClick;

        public ApComponentOverview (
            int _x, int _y, int _w, int _h
            )
            : base (_x, _y, _w, _h) {
                ComponentList = new List<ShipComponent>();
        }

        public void UpdateList()
        {
            if (Ships == null) return;
            if (Ships.Count == 0)
            {
                ComponentList.Clear();
                return;
            }

            ComponentList.Clear();
            ComponentList.AddRange(Ships[0].Components);

            var names = ComponentList.Select(s => s.Name);
            foreach (Ship _s in Ships)
            {
                names =
                    ComponentList.Select(s => s.Name);
                var _snames =
                    _s.Components.Select(s => s.Name);
                names = names.Intersect(_snames);
                names = names.ToList(); //finallize so linq doens't fuck with it
            }

            ComponentList.Clear();
            foreach (Ship _s in Ships)
            {
                foreach (ShipComponent _c in _s.Components)
                {
                    if (names.Contains(_c.Name))
                    {
                        if (ComponentList.Find(x => x.Name == _c.Name) == null)
                        {
                            ComponentList.Add(_c);
                        }
                    }
                }
            }

            /*foreach (Ship _s in Ships)
            {
                foreach (ShipComponent _c in _s.Components)
                {
                    if (ComponentList.Find(x => x.Name == _c.Name) == null)
                    {
                        ComponentList.Add(_c);
                    }
                }
            }*/
        }

        public override void GetAction(string _action)
        {
            switch (_action)
            {
                case "Clear Selection":
                    Selection = -1;
                    break;
                default:
                    break;
            }
        }

        public override void ActualRender(SpriteBatch spriteBatch)
        {
            graphics.Clear(ApLogWindow.StandardBackground);

            if (ComponentList != null && Ships != null)
            {
                spriteBatch.Begin();

                float _currentY = 0;
                float _offs = Res.LogFont.MeasureString("ship").Y;

                //foreach (ShipComponent _c in list)
                for(int i = 0; i < ComponentList.Count; i++)
                {
                    ShipComponent _c = ComponentList[i];
                    if (Selection == i)
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
                        _c.Name,
                        new Vector2(
                            indent,
                            _currentY
                        ),
                        Selection == i ? Color.Yellow : Color.White
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
                //dblclick
                float _itemHeight = Res.LogFont.MeasureString("ship").Y;
                float _mouseY = ms.Y - y1;
                float _item = _mouseY - (_mouseY % _itemHeight);
                _item /= _itemHeight;

                Selection = (int)_item;

                if (Selection >= ComponentList.Count)
                {
                    Selection = -1;
                }

                if ((DateTime.Now - lastLeftClick).Milliseconds < 200)
                {
                    if(Selection != -1)
                        Game.Log(
                            "CO : Would fire " + ComponentList[Selection].Name
                        );
                }

                lastLeftClick = DateTime.Now;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApComponentOverviewPost {
        public string Name;
        public int ID;
        public List<ShipComponent> Components;

        public ApComponentOverviewPost(
            string _name,
            int _id,
            int _count,
            List<ShipComponent> _components
            )
        {
            Name = _name;
            ID = _id;
            Components = _components;
        }

        public void Add(ShipComponent _sc)
        {
            Components.Add(_sc);
        }
    }
    
    class ApComponentOverview : ApWindow
    {
        public string Ships;
        public List<ApComponentOverviewPost> ComponentList; //list to overview
        int indent = 4;
        public int Selection = -1;
        DateTime lastLeftClick;

        public ApComponentOverview (
            int _x, int _y, int _w, int _h
            )
            : base (_x, _y, _w, _h) {
                ComponentList = new List<ApComponentOverviewPost>();
        }

        public override void SpecificUILoading()
        {
            Ships = xml.Element("ships").Value;
        }

        public void UpdateList()
        {
            if (UIBindings.Get(Ships) == null) return;
            if (UIBindings.Get(Ships).Count == 0)
            {
                ComponentList.Clear();
                return;
            }

            ComponentList.Clear();
            foreach (Ship _s in UIBindings.Get(Ships))
            {
                foreach (ShipComponent _c in _s.Components)
                {
                    if (!ComponentList.Any(x => x.ID == _c.ID))
                    {
                        ComponentList.Add(
                            new ApComponentOverviewPost(
                                _c.Name,
                                _c.ID,
                                1,
                                new List<ShipComponent>{ _c }
                            )
                        );
                    }
                    else
                    {
                        ApComponentOverviewPost _acop =
                            ComponentList.Find(x => x.ID == _c.ID);
                        _acop.Components.Add(_c);
                    }
                }
            }
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
            graphics.Clear(
                Utility.MultiplyColours(
                    ApLogWindow.StandardBackground,
                    Tint
                )
            );

            if (ComponentList != null && UIBindings.Get(Ships) != null)
            {
                spriteBatch.Begin();

                float _currentY = 0;
                float _offs = Res.LogFont.MeasureString("ship").Y;

                for(int i = 0; i < ComponentList.Count; i++)
                {
                    ApComponentOverviewPost _acop = ComponentList[i];
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

                    Color _textColour = Color.White;
                    if (_acop.Components.All(x => x.Active))
                        _textColour = Color.Yellow;
                    else if (_acop.Components.Any(x => x.Active))
                        _textColour = Color.Blue;

                    spriteBatch.DrawString(
                        Res.LogFont,
                        _acop.ID + " : " +
                        _acop.Components.Count + "x " +
                        _acop.Name,
                        new Vector2(
                            indent,
                            _currentY
                        ),
                        _textColour
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

                if (
                    (DateTime.Now - lastLeftClick).Milliseconds < 200 &&
                    //still a bit hacky, but...
                    this == WindowManager.GetWindowByName("Component Overview")
                ) {
                    if (Selection != -1)
                    {
                        if(Game.Verbose) Game.Log(
                            "CO : Would fire " +
                            ComponentList[Selection].Name +
                            " from " +
                            String.Join(
                                ", ",
                                UIBindings.Get("Selected").Select(
                                    x => x.Name)
                                ) + " to " +
                            String.Join(
                                ", ",
                                UIBindings.Get("Targeted").Select(
                                    x => x.Name)
                                ) + "."
                        );

                        if(!ComponentList[Selection].Components.
                            Any(x => x.Active))
                        {
                            if (Game.Targeted.Count > 0)
                            {
                                foreach (ShipComponent _c in
                                    ComponentList[Selection].Components)
                                {
                                    _c.Active = true;
                                    _c.Targets = new List<Ship>(
                                        UIBindings.Get("Targeted"));
                                }
                            }
                            else Game.Log("No target.");
                        }
                        else
                        {
                            foreach (ShipComponent _c in
                                ComponentList[Selection].Components)
                            {
                                _c.Active = false;
                                _c.Targets = null;
                            }
                        }
                    }
                }

                lastLeftClick = DateTime.Now;
            }
        }
    }
}

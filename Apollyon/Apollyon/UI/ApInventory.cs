using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class ApInventoryPost
    {
        public string Name;
        public List<Item> Items;

        public ApInventoryPost(
            string _name,
            List<Item> _items
        ) {
            Name = _name;
            Items = _items;
        }

        public int Count
        {
            get
            {
                int _count = 0;
                foreach (Item _i in Items)
                {
                    _count += _i.Count;
                }
                return _count;
            }
        }
    }

    class ApInventory : ApWindow
    {
        public List<Ship> Ships;
        public List<ApInventoryPost> Items; //list to overview
        public List<ApInventoryPost> Selection;
        int indent = 4;
        DateTime lastLeftClick;

        public ApInventory (
            int _x, int _y, int _w, int _h
        ) : base (_x, _y, _w, _h) {
            Items = new List<ApInventoryPost>();
            Selection = new List<ApInventoryPost>();
        }

        public void UpdateList()
        {
            Items.Clear();
            if ((Ships??new List<Ship>{}).Count == 0) return;

            foreach (Ship _s in Ships)
            {
                foreach (Item _i in _s.Inventory)
                {
                    ApInventoryPost _post = Items.Find(x => x.Name == _i.Name);
                    if (_post == null)
                    {
                        Items.Add(
                            new ApInventoryPost(
                                _i.Name,
                                new List<Item> { _i }
                            )
                        );
                    }
                    else
                    {
                        _post.Items.Add(_i);
                    }
                }
            }
        }

        public override void GetAction(string _action)
        {
            switch (_action)
            {
                case "Clear Selection":
                    Selection.Clear();
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

            if (Items != null)
            {
                spriteBatch.Begin();

                float _currentY = 0;
                float _offs = Res.LogFont.MeasureString("ship").Y;

                foreach (ApInventoryPost _i in Items)
                {
                    if (Selection.Contains(_i))
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
                        _i.Count + "x " + _i.Name,
                        new Vector2(
                            indent,
                            _currentY
                        ),
                        Ships.Count > 1 ? Color.DarkGray : Color.White
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

                float _itemHeight = Res.LogFont.MeasureString("item").Y;
                float _mouseY = ms.Y - y1;
                int _item =
                    (int)((_mouseY - (_mouseY % _itemHeight))/_itemHeight);
                //_item /= _itemHeight;

                if (_item >= Items.Count) {
                    Selection.Clear();
                    return;
                }

                if (Selection.Contains(Items[_item]))
                {
                    Selection.Remove(
                        Items[_item]
                    );
                }
                else
                {
                    Selection.Add(Items[_item]);
                }

                //dblclick
                if (
                    (DateTime.Now - lastLeftClick).Milliseconds
                    < Game.DoubleClickTime
                    && Ships.Count == 1
                ) {
                    Items[_item].Items[0].Use(Ships[0]);
                }

                lastLeftClick = DateTime.Now;
            }
        }
    }
}

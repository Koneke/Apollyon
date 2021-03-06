﻿using System;
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
        public string Ships;
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

        public override void SpecificUILoading()
        {
            Ships = xml.Element("ships").Value;
        }

        public override void Update()
        {
            UpdateList();
        }

        public void UpdateList()
        {
            Items.Clear();
            if (UIBindings.Get(Ships) == null) return;
            if (UIBindings.Get(Ships).Count == 0)
            {
                Selection.Clear();
                return;
            }

            var _iNames = Selection.Select(x => x.Name);

            foreach (Ship _s in
                UIBindings.Get(Ships).FindAll(x => x.HasTag("ship")))
                //semi hackish, mainly here to let it keep workign while we
                //implement targeting things which are not ships
            {
                foreach (Item _i in _s.Inventory)
                {
                    ApInventoryPost _post =
                        Items.Find(x => x.Items[0].ID == _i.ID);

                    if (_post == null)
                        Items.Add(
                            new ApInventoryPost(
                                _i.Name,
                                new List<Item> { _i }
                            )
                        );
                    else
                        _post.Items.Add(_i);
                }
            }

            if (UIBindings.Get(Ships).Count > 1) return;
            //fix this in future (deselecting any item not present on any
            //selected ship
            for(int i = 0; i < Selection.Count; i++)
            {
                ApInventoryPost _i = Selection[i];
                if(!_iNames.Contains(_i.Name)) {
                    Selection.Remove(_i);
                }
            }

            Items = Items.OrderBy(x => x.Name).ToList();
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
                float _offs = Res.GetFont("log font").MeasureString("ship").Y;

                foreach (ApInventoryPost _i in Items)
                {
                    if (Selection.Find(x => x.Name.Equals(_i.Name)) != null)
                    {
                        spriteBatch.Draw(
                            Res.Textures["1x1"],
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
                        Res.GetFont("log font"),
                        _i.Count + "x " + _i.Name,
                        new Vector2(
                            indent,
                            _currentY
                        ),
                        UIBindings.Get(Ships).Count > 1
                            ? Color.DarkGray : Color.White
                    );
                    _currentY += _offs;
                }

                spriteBatch.End();
            }

            DrawBorder(spriteBatch, ApWindow.StandardBorder);
        }

        public override void OwnInput(MouseState ms, MouseState oms)
        {
            if(UIBindings.Get("Selected").Count == 1) {
                if (ms.RightButton == ButtonState.Pressed &&
                    oms.RightButton == ButtonState.Released &&
                    UIBindings.Get("Selected")[0].Tags.Contains("ship")
                    )
                {
                    float _itemHeight =
                        Res.GetFont("log font").MeasureString("ship").Y;
                    float _mouseY = ms.Y - y1;
                    float _item = _mouseY - (_mouseY % _itemHeight);
                    _item /= _itemHeight;
                    if (_item < Items.Count)
                    {
                        Item _i = Items[(int)_item].Items[0];
                        Ship _s = (UIBindings.Get("Selected")[0] as Ship);
                        _s.DropItem(_s.Inventory.Find(x => x.ID == _i.ID));
                    }
                }
            }

            if (
                ms.LeftButton == ButtonState.Pressed &&
                oms.LeftButton == ButtonState.Released)
            {

                float _itemHeight =
                    Res.GetFont("log font").MeasureString("item").Y;
                float _mouseY = ms.Y - y1;
                int _item =
                    (int)((_mouseY - (_mouseY % _itemHeight))/_itemHeight);

                if (_item >= Items.Count) {
                    Selection.Clear();
                    return;
                }

                ApInventoryPost _find = 
                    Selection.Find(
                    x => x.Name.Equals(Items[_item].Name));
                if (_find != null)
                {
                    Selection.Remove(
                        _find
                    );
                }
                else
                {
                    Selection.Add(Items[_item]);
                }

                var a = DateTime.Now;
                var b = lastLeftClick;
                var c = a - b;

                //dblclick
                if (
                    (DateTime.Now - lastLeftClick).TotalMilliseconds
                    < Game.DoubleClickTime
                    && UIBindings.Get(Ships).Count == 1
                ) {
                    //use one
                    Items[_item].Items[0].Use();
                }

                lastLeftClick = DateTime.Now;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

//temp
using System.Xml;
using System.Xml.Linq;

namespace Apollyon
{
    class ApOverview : ApWindow
    {
        int indent = 4; //intendent from the border in the list

        public bool Any; //search for ANY tag or ALL tags
        public List<string> Sources; //list of all tags looked for in space
        public List<string> Filters;
        List<SpaceObject> list;

        public ApOverview(
            int _x, int _y, int _w, int _h
            ) : base(_x, _y, _w, _h)
        {
            Any = true;
            Sources = new List<string>();
            Filters = new List<string>();
        }

        public override void Update()
        {
            //list = Game.World.SpaceObjects
            list = Utility.QuerySpace(
                Sources, Any);
            list = list.FindAll(
                x => Filters.All(y => !x.HasTag(y))
            );
        }

        public override void SpecificUILoading() {
            Sources = xml.Element("tags").Elements().Select(
                x => x.Value).ToList();
            Filters = xml.Element("filters").Elements().Select(
                x => x.Value).ToList();
        }
        public override void GetAction(string _action) { }
        public override void ActualRender(
            SpriteBatch spriteBatch
        ) {
            graphics.Clear(
                Utility.MultiplyColours(
                    ApLogWindow.StandardBackground,
                    Tint
                )
            );

            float _currentY = 0;
            float _offs = Res.LogFont.MeasureString("ship").Y;

            spriteBatch.Begin();
            foreach (SpaceObject _i in list)
            {
                spriteBatch.DrawString(
                    Res.LogFont,
                    _i.Name,
                    new Vector2(indent, _currentY),
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

                float _itemHeight = Res.LogFont.MeasureString("item").Y;
                float _mouseY = ms.Y - y1;
                int _item =
                    (int)((_mouseY - (_mouseY % _itemHeight)) / _itemHeight);
                int _index = (int)_item;

                if (_index >= list.Count)
                {
                    UIBindings.Get("Selected").Clear();
                    return;
                }

                UIBindings.Get("Selected").Add(list[_index]);
            }
        }
    }
}

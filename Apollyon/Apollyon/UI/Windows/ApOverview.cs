using System;
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
        int scrollOffset = 0;
        int lastWheelValue = 0;
        int size;
        int itemHeight;

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

            itemHeight = (int)Res.GetFont("log font").MeasureString("ship").Y;
            //cast to kill ambiguity
            size = (int)Math.Floor((double)_h / itemHeight);
        }

        public override void Update()
        {
            list = Utility.QuerySpace(
                Sources, Any
            );

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
            float _offs = Res.GetFont("log font").MeasureString("ship").Y;

            spriteBatch.Begin();
            //add check so we only draw visible in the future
            for(int _i = scrollOffset; _i < list.Count; _i++)
            {
                SpaceObject _so = list[_i];
                if (UIBindings.Get("Selected").Contains(_so))
                    spriteBatch.Draw(
                        Res.Textures["1x1"],
                        new Rectangle(
                            0,
                            (int)_currentY,
                            (int)w,
                            (int)_offs),
                        Utility.MultiplyColours(
                            ApWindow.StandardBorder,
                            new Color(1f, 1f, 1f, 0.3f)
                        )
                    );

                spriteBatch.DrawString(
                    Res.GetFont("log font"),
                    _so.Name,
                    new Vector2(indent, _currentY),
                    Color.White
                );

                if (list[_i].HasTag("ship"))
                {
                    Ship _s = list[_i] as Ship;
                    Color _hostileColour = Color.Red;
                    Color _neutralColour = Color.Gray;
                    Color _friendlyColour = Color.LimeGreen;

                    Color _shipColour = _neutralColour;

                    if (Game.PlayerFaction.GetHostiles().Contains(
                        _s.Faction))
                        _shipColour = _hostileColour;

                    else if (Game.PlayerFaction.GetFriendlies().Contains(
                        _s.Faction))
                        _shipColour = _friendlyColour;

                    spriteBatch.Draw(
                        Res.Textures["1x1"],
                        new Rectangle(
                            (int)w-6,
                            (int)_currentY,
                            4,
                            (int)_offs
                        ),
                        _shipColour
                    );
                }

                _currentY += _offs;
            }

            float _scrollerSize =
                    (int)(h * Math.Min((float)size / list.Count, 1));
            float _scrollerMax = h - _scrollerSize;
            float _scrollerPos =
                _scrollerMax * ((float)scrollOffset / (list.Count-size));

            spriteBatch.Draw(
                Res.Textures["1x1"],
                new Rectangle(
                    (int)w-2,
                    (int)_scrollerPos,
                    2,
                    (int)(h*Math.Min((float)size/list.Count,1))
                ),
                Color.White
            );

            
            spriteBatch.End();

            DrawBorder(spriteBatch, ApWindow.StandardBorder);
        }
        public override void OwnInput(MouseState ms, MouseState oms)
        {
            if (ms.ScrollWheelValue != lastWheelValue)
            {
                scrollOffset += Game.MouseWheelDelta;
                lastWheelValue = ms.ScrollWheelValue;
            }

            scrollOffset = Math.Max(scrollOffset, 0);
            scrollOffset = Math.Min(
                scrollOffset,
                Math.Max(list.Count-size, 0)
            );

            if (
                (ms.LeftButton == ButtonState.Pressed &&
                oms.LeftButton == ButtonState.Released) ||
                (ms.RightButton == ButtonState.Pressed &&
                oms.RightButton == ButtonState.Released)
                )
            {
                float _itemHeight =
                    Res.GetFont("log font").MeasureString("item").Y;
                float _mouseY = ms.Y - y1;
                int _item =
                    (int)((_mouseY - (_mouseY % _itemHeight)) / _itemHeight);
                _item += scrollOffset;
                int _index = (int)_item;

                string _list =
                    (ms.LeftButton == ButtonState.Pressed &&
                    oms.LeftButton == ButtonState.Released) ?
                    "Selected" : "Targeted";
                if (_index >= list.Count)
                {
                    UIBindings.Get(_list).Clear();
                    return;
                }

                if (UIBindings.Get(_list).Contains(list[_index]))
                {
                    UIBindings.Get(_list).Remove(list[_index]);
                    return;
                }
                UIBindings.Get(_list).Add(list[_index]);
            }
        }
    }
}

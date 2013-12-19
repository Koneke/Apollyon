using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class World
    {
        
        //public Rectangle Camera;
        public Camera Camera;
        public Vector2 CameraPosition
        {
            get { return new Vector2(Camera.X, Camera.Y); }
        }
        public List<Ship> Ships;
        public List<SpaceItem> Items; //items in space

        public float Timer; //ms timer

        Rectangle boxSelection;
        bool selecting = false;

        public World()
        {
            Ships = new List<Ship>();
            Items = new List<SpaceItem>();
            Camera = new Camera();
            UIBindings.Bind("All", Ships);
        }

        public void Input(
            KeyboardState ks, KeyboardState oks,
            MouseState ms, MouseState oms)
        {
            Camera.Input(ms, oms);

            foreach (Ship _s in Ships)
            {
                _s.Input(ks, oks, ms, oms);
            }

            if (
                ms.LeftButton == ButtonState.Pressed
            ) {
                if (
                    oms.LeftButton == ButtonState.Released &&
                    !ApWindow.PointInWindow(new Point(ms.X, ms.Y))
                ) {
                    Vector2 _clickPoint = Camera.ScreenToWorld(
                        new Vector2(ms.X, ms.Y));
                    boxSelection.X = (int)_clickPoint.X;
                    boxSelection.Y = (int)_clickPoint.Y;
                    selecting = true;
                }

                if (
                    oms.LeftButton == ButtonState.Pressed &&
                    selecting
                ) {
                    Vector2 _clickPoint = Camera.ScreenToWorld(
                        new Vector2(ms.X, ms.Y));
                    boxSelection.Width = (int)(_clickPoint.X - boxSelection.X);
                    boxSelection.Height = (int)(_clickPoint.Y - boxSelection.Y);
                }
            } else if (
                ms.LeftButton == ButtonState.Released &&
                oms.LeftButton == ButtonState.Pressed &&
                selecting
            ) {
                //ApUI.ShipOverview.Selection.Clear();
                Game.Selected.Clear();
                foreach (Ship _s in Ships)
                {
                    Rectangle _box = boxSelection;

                    if (_box.Width < 0)
                    {
                        _box.X += _box.Width;
                        _box.Width *= -1;
                    }

                    if (_box.Height < 0)
                    {
                        _box.Y += _box.Height;
                        _box.Height *= -1;
                    }

                    List<Ship> _list = UIBindings.Get(
                        ((ApShipOverview)WindowManager.
                        GetWindowByName("Fleet Overview")).Ships);

                    if (_list != null)
                    {
                        if (
                            _list.Contains(_s) &&
                            _box.Contains(
                                new Point(
                                    (int)_s.Position.X,
                                    (int)_s.Position.Y
                                )
                            )
                        )
                        {
                            Game.Selected.Add(_s);
                        }
                    }
                }
                boxSelection.Width = 0;
                selecting = false;
            }

            if (
                ms.RightButton == ButtonState.Pressed &&
                oms.RightButton == ButtonState.Released &&
                !ApWindow.PointInWindow(new Point(ms.X, ms.Y))
            ) {
                if (Game.Selected.Count > 0)
                {
                    Vector2 _avgPosition = new Vector2(0, 0);

                    foreach (Ship _s in Game.Selected)
                    {
                        _avgPosition.X += _s.Position.X;
                        _avgPosition.Y += _s.Position.Y;
                    }

                    _avgPosition =
                        _avgPosition / Game.Selected.Count;

                    foreach (Ship _s in Game.Selected)
                    {
                        Vector2 _avgPositionOffset =
                            _s.Position - _avgPosition;
                        _s.TargetPosition =
                            Camera.ScreenToWorld(
                                new Vector2(ms.X, ms.Y)
                            ) + _avgPositionOffset;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            while (Timer > Game.TickTime)
            {
                foreach (Ship _s in Ships)
                {
                    _s.Update();
                }
                foreach (Ship _s in Ships.FindAll(
                    x => x.Shield.Current <= 0))
                {
                    _s.Die();
                    Game.Fleet.Remove(_s);
                }
                Ships = Ships.FindAll(x => x.Shield.Current > 0);
                Timer -= Game.TickTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Ship _s in Ships)
            {
                //selection box
                if (boxSelection.Width != 0)
                {
                    Rectangle _screenRectangle = boxSelection;

                    _screenRectangle.Offset(
                        new Point(-Camera.X, -Camera.Y));

                    if (_screenRectangle.Width < 0)
                    {
                        _screenRectangle.X += _screenRectangle.Width;
                        _screenRectangle.Width *= -1;
                    }

                    if (_screenRectangle.Height < 0)
                    {
                        _screenRectangle.Y += _screenRectangle.Height;
                        _screenRectangle.Height *= -1;
                    }

                    _screenRectangle.X =
                        (int)(_screenRectangle.X * Camera.GetZoom());
                    _screenRectangle.Y =
                        (int)(_screenRectangle.Y * Camera.GetZoom());

                    _screenRectangle.Width =
                        (int)(_screenRectangle.Width * Camera.GetZoom());
                    _screenRectangle.Height =
                        (int)(_screenRectangle.Height * Camera.GetZoom());

                    Utility.DrawOutlinedRectangle(
                        spriteBatch,
                        _screenRectangle,
                        new Color(0f, 1f, 0f, 1f)
                    );
                }

                spriteBatch.Begin();

                Vector2 _sp = Game.Camera.WorldToScreen(_s.Position);
                Rectangle _shipRect =
                    new Rectangle(
                        (int)(_sp.X),
                        (int)(_sp.Y),
                        (int)(32 * Camera.GetZoom()),
                        (int)(32 * Camera.GetZoom())
                    );

                spriteBatch.Draw(
                    Res.Ship,
                    _shipRect,
                    null,
                    Color.White,
                    (float)_s.Direction,
                    new Vector2(Res.Ship.Width / 2, Res.Ship.Height / 2),
                    SpriteEffects.None, 0f

                );

                spriteBatch.End();

                Rectangle _shipRectOffset = _shipRect;
                _shipRectOffset.Offset(
                    (int)(-16 * Camera.GetZoom()),
                    (int)(-16 * Camera.GetZoom())
                );

                if(Game.Targeted.Contains(_s)) {
                    Utility.DrawOutlinedRectangle(
                        spriteBatch,
                        _shipRectOffset,
                        new Color(1f, 0f, 0f, 0.5f)
                    );
                } else if(Game.Selected.Contains(_s)) {
                    Utility.DrawOutlinedRectangle(
                        spriteBatch,
                        _shipRectOffset,
                        new Color(0f, 1f, 0f, 0.5f)
                    );
                }

                MouseState _ms = Mouse.GetState();
                if(_shipRectOffset.Contains(new Point(_ms.X, _ms.Y))) {
                    Utility.DropShadowText(
                        spriteBatch,
                        Res.LogFont,
                        _s.Name,
                        new Vector2(
                            _ms.X - Res.LogFont.MeasureString(_s.Name).X/2 + 10,
                            _ms.Y - 20
                        ),
                        Color.Black,
                        Color.White
                    );
                }
            }

            foreach (SpaceItem _i in Items)
            {
                Rectangle _itemRect = new Rectangle(
                    (int)Game.Camera.WorldToScreen(_i.Position).X,
                    (int)Game.Camera.WorldToScreen(_i.Position).Y,
                    16,
                    16
                );

                Rectangle _itemRectOffset =
                    new Rectangle(
                        _itemRect.X, _itemRect.Y,
                        _itemRect.Width, _itemRect.Height
                    );
                _itemRectOffset.Offset(new Point(-8, -8));
                
                spriteBatch.Begin();

                spriteBatch.Draw(
                    _i.Texture,
                    _itemRect,
                    null,
                    Color.White,
                    0,
                    new Vector2(8, 8),
                    SpriteEffects.None,
                    0f
                );

                spriteBatch.End();

                MouseState _ms = Mouse.GetState();
                if(_itemRectOffset.Contains(new Point(_ms.X, _ms.Y))) {
                    Utility.DropShadowText(
                        spriteBatch,
                        Res.LogFont,
                        _i.Item.Name,
                        new Vector2(
                            _ms.X - Res.LogFont.MeasureString(
                                _i.Item.Name).X/2 + 10,
                            _ms.Y - 20
                        ),
                        Color.Black,
                        Color.White
                    );
                }
            }
        }
    }
}

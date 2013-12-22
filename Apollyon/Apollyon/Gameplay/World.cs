using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class World
    {
        public Camera Camera;
        public Vector2 CameraPosition
        {
            get { return new Vector2(Camera.X, Camera.Y); }
        }
        public List<SpaceObject> SpaceObjects;

        public float Timer; //ms timer

        Rectangle boxSelection;
        bool selecting = false;

        public World()
        {
            SpaceObjects = new List<SpaceObject>();
            Camera = new Camera();
        }

        public void Input(
            KeyboardState ks, KeyboardState oks,
            MouseState ms, MouseState oms)
        {
            Camera.Input(ms, oms);

            foreach (Ship _s in SpaceObjects.FindAll(
                x => x.Tags.Contains("ship")))
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
                //Game.Selected.Clear();
                UIBindings.Get("Selected").Clear();
                foreach (Ship _s in SpaceObjects.FindAll(
                    x => x.Tags.Contains("ship")))
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

                    List<SpaceObject> _list;

                    _list = Game.World.SpaceObjects.FindAll(
                        x => x.Tags.Contains("ship"));

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
                            UIBindings.Get("Selected").Add(_s);
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
                if (UIBindings.Get("Selected").Count > 0)
                {
                    Vector2 _avgPosition = new Vector2(0, 0);

                    foreach (Ship _s in UIBindings.Get("Selected"))
                    {
                        _avgPosition.X += _s.Position.X;
                        _avgPosition.Y += _s.Position.Y;
                    }

                    _avgPosition =
                        _avgPosition / UIBindings.Get("Selected").Count;

                    foreach (Ship _s in UIBindings.Get("Selected"))
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
                foreach(AISimpleBrain _AI in Game.AIs)
                    _AI.Tick();

                foreach(SpaceObject _so in SpaceObjects.FindAll(x=>true))
                    _so.Update();

                foreach (SpaceObject _so in SpaceObjects.FindAll(
                    x => x.Health <= 0))
                        _so.Die();

                SpaceObjects = SpaceObjects.FindAll(x => x.Health > 0);

                Timer -= Game.TickTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string _hoverText = "";
            MouseState _ms = Mouse.GetState();

            foreach (SpaceObject _so in
                SpaceObjects
                .FindAll(x => !x.HasTag("carried"))
                .OrderBy(x => x.Depth)
                )
            {
                if(!_so.Visible) continue;

                #region maindrawing
                spriteBatch.Begin();

                Vector2 _sp = Game.Camera.WorldToScreen(_so.Position);

                Rectangle _screenRect = new Rectangle(
                    (int)(_sp.X),
                    (int)(_sp.Y),
                    (int)(_so.Size.X * Camera.GetZoom()),
                    (int)(_so.Size.Y * Camera.GetZoom())
                );

                var _z = Camera.GetZoom();//debug

                spriteBatch.Draw(
                    _so.Texture??Res.Textures["generic"],
                    _screenRect,
                    null,
                    Color.White,
                    (float)_so.Rotation,
                    new Vector2(
                        //_so.Size.X / 2,
                        //_so.Size.Y / 2
                        _so.Texture.Width/2f,
                        _so.Texture.Height/2f
                        ),
                    SpriteEffects.None, 0f
                );

                spriteBatch.End();
                #endregion

                #region selectionboxes
                _screenRect.Offset(
                    //new Point(-_screenRect.Width / 2, -_screenRect.Width / 2)
                    new Point(
                        (int)(-_so.Size.X * _z / 2f),
                        (int)(-_so.Size.Y * _z / 2f))
                );

                //if (_so.Tags.Contains("ship"))
                if(true)
                {
                    bool _selected = UIBindings.Get("Selected").
                        Contains(_so);

                    bool _targeted = UIBindings.Get("Targeted").
                        Contains(_so);

                    if(_selected)
                    {
                        if(_targeted) _screenRect.Offset(new Point(1, 1));
                        Utility.DrawOutlinedRectangle(
                            spriteBatch,
                            _screenRect,
                            new Color(0f, 1f, 0f, 0.5f)
                        );
                        if(_targeted) _screenRect.Offset(new Point(-1, -1));
                    }
                    if(_targeted)
                    {
                        if(_selected) _screenRect.Offset(new Point(-1, -1));
                        Utility.DrawOutlinedRectangle(
                            spriteBatch,
                            _screenRect,
                            new Color(1f, 0f, 0f, 0.5f)
                        );
                        if(_selected) _screenRect.Offset(new Point(1, 1));
                    }
                }
                #endregion

                if (_screenRect.Contains(new Point(_ms.X, _ms.Y))) {
                    _hoverText += "\n" + _so.Name;
                }
            }

            Utility.DropShadowText(
                spriteBatch,
                Res.LogFont,
                _hoverText,
                new Vector2(
                    _ms.X -
                        Res.LogFont.MeasureString(_hoverText).X/2 + 10,
                    _ms.Y - 
                        Res.LogFont.MeasureString(_hoverText).Y
                ),
                Color.Black,
                Color.White
            );

            drawBoxSelection(spriteBatch);
        }

        //stuff like this feels like it should be in game, idk
        void drawBoxSelection(SpriteBatch spriteBatch)
        {
            //box selection drawing
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
        }
    }
}

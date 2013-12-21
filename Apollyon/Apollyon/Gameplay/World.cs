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
        public List<ISpaceObject> SpaceObjects;

        public float Timer; //ms timer

        Rectangle boxSelection;
        bool selecting = false;

        public World()
        {
            SpaceObjects = new List<ISpaceObject>();
            Camera = new Camera();
        }

        public void Input(
            KeyboardState ks, KeyboardState oks,
            MouseState ms, MouseState oms)
        {
            Camera.Input(ms, oms);

            foreach (Ship _s in SpaceObjects.FindAll(
                x => x.GetTags().Contains("ship")))
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
                Game.Selected.Clear();
                foreach (Ship _s in SpaceObjects.FindAll(
                    x => x.GetTags().Contains("ship")))
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

                    List<ISpaceObject> _list;

                    _list = Game.World.SpaceObjects.FindAll(
                        x => x.GetTags().Contains("ship"));

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
                foreach (Ship _s in SpaceObjects.FindAll(
                    x => x.GetTags().Contains("ship")))
                {
                    _s.Update();
                }
                foreach (Ship _s in SpaceObjects
                    .FindAll(x => x.GetTags().Contains("ship"))
                    .FindAll(x => ((Ship)x).Shield.Current <= 0))
                {
                    _s.Die();
                    Game.Fleet.Remove(_s);
                }
                SpaceObjects = SpaceObjects.FindAll
                    (x =>
                        !x.GetTags().Contains("ship") ||
                        ((Ship)x).Shield.Current > 0);
                Timer -= Game.TickTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string _hoverText = "";
            MouseState _ms = Mouse.GetState();

            foreach (ISpaceObject _so in
                SpaceObjects
                .FindAll(x => !x.HasTag("carried"))
                .OrderBy(x => x.GetDepth())
                )
            {
                if(!_so.GetVisible()) continue;

                #region maindrawing
                spriteBatch.Begin();

                Vector2 _sp = Game.Camera.WorldToScreen(_so.GetPosition());

                Rectangle _screenRect = new Rectangle(
                    (int)(_sp.X),
                    (int)(_sp.Y),
                    (int)(_so.GetSize().X * Camera.GetZoom()),
                    (int)(_so.GetSize().Y * Camera.GetZoom())
                );

                spriteBatch.Draw(
                    _so.GetTexture()??Res.Textures["generic"],
                    _screenRect,
                    null,
                    Color.White,
                    (float)_so.GetRotation(),
                    new Vector2(
                        _so.GetSize().X / 2,
                        _so.GetSize().Y / 2
                        ),
                    SpriteEffects.None, 0f
                );

                spriteBatch.End();
                #endregion

                #region selectionboxes
                _screenRect.Offset(
                    new Point(-_screenRect.Width / 2, -_screenRect.Width / 2)
                );

                if (_so.GetTags().Contains("ship"))
                {
                    bool _selected = UIBindings.Get("Selected").
                        Contains((Ship)_so);

                    bool _targeted = UIBindings.Get("Targeted").
                        Contains((Ship)_so);

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
                    _hoverText += "\n" + _so.GetName();
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

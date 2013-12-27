﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class World
    {
        //public Camera Camera;
        public List<SpaceObject> SpaceObjects;

        public float Timer; //ms timer

        Rectangle boxSelection;
        public bool selecting = false;
        bool selectingHostile = false;

        public World()
        {
            SpaceObjects = new List<SpaceObject>();
            //Camera = new Camera();
        }

        //CA1502
        public void Input()
        {
            Input(
                InputManager.ks,
                InputManager.oks,
                InputManager.ms,
                InputManager.oms
            );
        }

        public void Input(
            KeyboardState ks, KeyboardState oks,
            MouseState ms, MouseState oms)
        {
            //still ugly, but not as
            if(
                Game.GetState("space").IsTopState() &&
                Game.GetState("space").WindowManager.PointInWindow(
                    new Point(ms.X,ms.Y)) == null)
                Game.Camera.Input(ms, oms);

            foreach (Ship _s in SpaceObjects.FindAll(
                x => x.Tags.Contains("ship")))
                _s.Input(ks, oks, ms, oms);

            selectingHostile = ks.IsKeyDown(Keys.LeftControl);

            if (ms.LeftButton == ButtonState.Pressed) {
                if (
                    oms.LeftButton == ButtonState.Released &&
                    Game.SpaceState.WindowManager
                    .PointInWindow(new Point(ms.X, ms.Y)) == null
                ) {
                    Vector2 _clickPoint = Game.Camera.ScreenToWorld(
                        new Vector2(ms.X, ms.Y));
                    boxSelection.X = (int)_clickPoint.X;
                    boxSelection.Y = (int)_clickPoint.Y;
                    selecting = true;
                }

                if (
                    oms.LeftButton == ButtonState.Pressed &&
                    selecting
                ) {
                    Vector2 _clickPoint = Game.Camera.ScreenToWorld(
                        new Vector2(ms.X, ms.Y));
                    boxSelection.Width = (int)(_clickPoint.X - boxSelection.X);
                    boxSelection.Height = (int)(_clickPoint.Y - boxSelection.Y);
                }
            } else if (
                ms.LeftButton == ButtonState.Released &&
                oms.LeftButton == ButtonState.Pressed &&
                selecting
            ) {
                if (!ks.IsKeyDown(Keys.LeftShift)) //add to selection w/ shift
                {
                    if (!selectingHostile)
                        UIBindings.Get("Selected").Clear();
                    else
                        UIBindings.Get("Targeted").Clear();
                }

                foreach(SpaceObject _so in SpaceObjects)
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

                    _list = Game.World.SpaceObjects;

                    if (_list != null)
                    {
                        if (
                            _list.Contains(_so) &&
                            _box.Contains(
                                new Point(
                                    (int)_so.Position.X,
                                    (int)_so.Position.Y
                                )
                            )
                        )
                        {
                            string list = selectingHostile
                                ? "targeted" : "selected";
                            if (ks.IsKeyDown(Keys.LeftShift))
                            {
                                if (UIBindings.Get(list).Contains(_so))
                                    UIBindings.Get(list).Remove(_so);
                                else
                                    UIBindings.Get(list).Add(_so);
                            }
                            else
                            {
                                UIBindings.Get(list).Add(_so);
                            }
                        }
                    }
                }
                boxSelection.Width = 0;
                selecting = false;
            }

            if (
                ms.RightButton == ButtonState.Pressed &&
                oms.RightButton == ButtonState.Released &&
                Game.SpaceState.WindowManager
                .PointInWindow(new Point(ms.X, ms.Y)) == null
            ) {
                if (
                    UIBindings.Get("Selected").FindAll(
                        x => x.HasTag("ship")).Count > 0
                    )
                {
                    Vector2 _avgPosition = new Vector2(0, 0);

                    foreach (Ship _s in
                        UIBindings.Get("Selected").FindAll(
                        x => x.HasTag("ship")))
                    {
                        _avgPosition.X += _s.Position.X;
                        _avgPosition.Y += _s.Position.Y;
                    }

                    _avgPosition =
                        _avgPosition / UIBindings.Get("Selected").Count;

                    foreach (Ship _s in
                        UIBindings.Get("Selected").FindAll(
                        x => x.HasTag("ship")))
                    {
                        Vector2 _avgPositionOffset =
                            _s.Position - _avgPosition;
                        _s.TargetPosition =
                            Game.Camera.ScreenToWorld(
                                new Vector2(ms.X, ms.Y)
                            ) + _avgPositionOffset;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            UIBindings.Bind(
                "All",
                SpaceObjects.FindAll(
                    x => x.HasTag("ship") && x.Health > 0
                )
            );
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            while (Timer > Game.TickTime)
            {
                foreach(AISimpleBrain _AI in Game.AIs)
                    _AI.Tick();

                //put here because I don't have tick logic in Game1 and I'm lazy
                Particle2.Tick();

                foreach(SpaceObject _so in SpaceObjects.FindAll(x=>true))
                    _so.Update();

                foreach (SpaceObject _so in SpaceObjects.FindAll(
                    x => x.Health <= 0))
                        _so.Die();

                SpaceObjects = SpaceObjects.FindAll(x => x.Health > 0);

                //selection stuff, only if top state (because we're not box
                //selecting stuff in world in other states

                if (Game.GetState("space").IsTopState())
                {
                    List<Ship> _ships =
                        UIBindings.Get("selected").FindAll(
                            x => x.HasTag("ship"))
                        .Cast<Ship>()
                        .ToList()
                        .FindAll(
                            x => x.Faction != Game.PlayerFaction)
                        ;

                    UIBindings.Bind("selected",
                        UIBindings.Get("selected")
                        //filter out hostiles
                            .FindAll(x => !_ships.Contains(x))
                        //filter out nonships
                            .FindAll(x => x.HasTag("ship"))
                        //filter out docked ships
                            .FindAll(x => x.HasTag("!docked"))
                    );
                }

                Timer -= Game.TickTime;
            }
        }

        public void Draw()
        {
            string _hoverText = "";
            MouseState _ms = Mouse.GetState();

            foreach (SpaceObject _so in
                SpaceObjects
                //.FindAll(x => !x.HasTag("carried"))
                .FindAll(x => x.HasTag("!carried"))
                .FindAll(x => x.HasTag("!docked"))
                .OrderBy(x => x.Depth)
                )
            {
                if(!_so.Visible) continue;

                #region maindrawing
                Vector2 _sp = Game.Camera.WorldToScreen(_so.Position);

                Rectangle _screenRect = new Rectangle(
                    (int)(_sp.X),
                    (int)(_sp.Y),
                    Math.Max((int)(_so.Size.X * Game.Camera.GetZoom()),1),
                    Math.Max((int)(_so.Size.Y * Game.Camera.GetZoom()),1)
                );

                if (Game.Camera.Rectangle.Contains(
                    new Point(
                        (int)_so.Position.X,
                        (int)_so.Position.Y)
                    )
                ) {
                    DrawManager.AddCall(
                        new BasicDrawCall(
                            _so.Texture ?? Res.Textures["generic"],
                            _screenRect,
                            null,
                            Color.White,
                            (float)_so.Rotation,
                            new Vector2(
                                _so.Texture.Width / 2f,
                                _so.Texture.Height / 2f
                                ),
                            0f
                        )
                    );
                }
                #endregion

                #region selectionboxes
                _screenRect.Offset(
                    new Point(
                        (int)(-_so.Size.X * Game.Camera.GetZoom() / 2f),
                        (int)(-_so.Size.Y * Game.Camera.GetZoom() / 2f))
                );

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
                            _screenRect,
                            new Color(0f, 1f, 0f, 0.5f)
                        );
                        if(_targeted) _screenRect.Offset(new Point(-1, -1));
                    }
                    if(_targeted)
                    {
                        if(_selected) _screenRect.Offset(new Point(-1, -1));
                        Utility.DrawOutlinedRectangle(
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
                "log font",
                _hoverText,
                new Vector2(
                    _ms.X -
                        Res.GetFont("log font")
                        .MeasureString(_hoverText).X/2 + 10,
                    _ms.Y - 
                        Res.GetFont("log font")
                        .MeasureString(_hoverText).Y
                ),
                Color.Black,
                Color.White
            );

            drawBoxSelection();
        }

        //stuff like this feels like it should be in game, idk
        void drawBoxSelection()
        {
            //box selection drawing
            if (boxSelection.Width != 0)
            {
                Rectangle _screenRectangle = boxSelection;

                _screenRectangle.Offset(
                    new Point(-Game.Camera.X, -Game.Camera.Y));

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
                    (int)(_screenRectangle.X * Game.Camera.GetZoom());
                _screenRectangle.Y =
                    (int)(_screenRectangle.Y * Game.Camera.GetZoom());

                _screenRectangle.Width =
                    (int)(_screenRectangle.Width * Game.Camera.GetZoom());
                _screenRectangle.Height =
                    (int)(_screenRectangle.Height * Game.Camera.GetZoom());

                Utility.DrawOutlinedRectangle(
                    _screenRectangle,
                    selectingHostile ?
                        new Color(1f, 0f, 0f, 1f) :
                        new Color(0f, 1f, 0f, 1f)
                );
            }
        }
    }
}

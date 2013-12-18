using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class World
    {
        
        public Rectangle Camera;
        public Vector2 CameraPosition
        {
            get { return new Vector2(Camera.X, Camera.Y); }
        }
        public List<Ship> Ships;

        public float Timer; //ms timer

        Rectangle boxSelection;
        int lastScrollWheelValue;

        public World()
        {
            Ships = new List<Ship>();
            Camera = new Rectangle(
                0,
                0,
                (int)Game.ScreenSize.X,
                (int)Game.ScreenSize.Y
            );
            lastScrollWheelValue = 0;
        }

        public void Input(MouseState ms, MouseState oms)
        {
            float _zoom = (Game.ScreenSize.X / (float)Camera.Width);
            int _scrollSpeed = (int)(9f/_zoom);
            if (ms.X > Game.ScreenSize.X)
                Camera.X += _scrollSpeed;
            if (ms.Y > Game.ScreenSize.Y)
                Camera.Y += _scrollSpeed;
            if (ms.X < 0)
                Camera.X -= _scrollSpeed;
            if (ms.Y < 0)
                Camera.Y -= _scrollSpeed;

            if (
                ms.LeftButton == ButtonState.Pressed &&
                !ApWindow.PointInWindow(new Point(ms.X, ms.Y))
            ) {
                if (oms.LeftButton == ButtonState.Released)
                {
                    boxSelection.X = ms.X;
                    boxSelection.Y = ms.Y;
                }
                else
                {
                    boxSelection.Width = ms.X - boxSelection.X;
                    boxSelection.Height = ms.Y - boxSelection.Y;
                }
            }

            int _wDelta = (lastScrollWheelValue - ms.ScrollWheelValue);

            float _premsx = Camera.X + ms.X * _zoom;
            float _premsy = Camera.Y + ms.Y * _zoom;

            int _preW = Camera.Width;
            int _preH = Camera.Height;

            float _zoomSpeed = 1.1f;

            Camera.Width = (int)(Camera.Width * 1 + (_wDelta * _zoomSpeed));
            Camera.Height = (int)(Camera.Width * (9f / 16f));
            _zoom = (Game.ScreenSize.X / (float)Camera.Width);
            float _newmsx = Camera.X + ms.X * _zoom;
            float _newmsy = Camera.Y + ms.Y * _zoom;
            Camera.X -= (int)(
                (Camera.Width - _preW) * (ms.X / (Game.ScreenSize.X / 2f)) / 2f);
            Camera.Y -= (int)(
                (Camera.Height - _preH) * (ms.Y / (Game.ScreenSize.Y / 2f)) / 2f);

            lastScrollWheelValue = ms.ScrollWheelValue;
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
                Timer -= Game.TickTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Ship _s in Ships)
            {
                Point _shipPoint =
                    new Point((int)_s.Position.X, (int)_s.Position.Y);
                if (!Camera.Contains(_shipPoint))
                    continue;

                Vector2 _screenPosition = _s.Position - CameraPosition;
                //float _zoom = ((float)Camera.Width / Game.ScreenSize.X);
                float _xzoom = (Game.ScreenSize.X / (float)Camera.Width);
                float _yzoom = (Game.ScreenSize.Y / (float)Camera.Height);
                _screenPosition = _screenPosition * new Vector2(_xzoom,_yzoom);

                Rectangle _shipRect =
                    new Rectangle(
                        (int)(_screenPosition.X),
                        (int)(_screenPosition.Y),
                        (int)(32*_xzoom),
                        (int)(32*_yzoom)
                    );

                Rectangle _shipRectOffset = _shipRect;
                _shipRectOffset.Offset(
                    (int)(-16 * _xzoom),
                    (int)(-16 * _yzoom)
                );

                if(ApUI.HostileOverview.Selection.Contains(_s)) {
                    Utility.DrawOutlinedRectangle(
                        spriteBatch,
                        _shipRectOffset,
                        new Color(1f, 0f, 0f, 0.5f)
                    );
                } else if(ApUI.ShipOverview.Selection.Contains(_s)) {
                    Utility.DrawOutlinedRectangle(
                        spriteBatch,
                        _shipRectOffset,
                        new Color(0f, 1f, 0f, 0.5f)
                    );
                }

                spriteBatch.Begin();
                spriteBatch.Draw(
                    Res.Ship,
                    new Rectangle(
                        (int)(_screenPosition.X),
                        (int)(_screenPosition.Y),
                        (int)(32*_xzoom),
                        (int)(32*_yzoom)
                    ),
                    null,
                    Color.White,
                    //(float)Math.Atan2(_s.Velocity.Y, _s.Velocity.X),
                    (float)_s.Direction,
                    new Vector2(Res.Ship.Width/2, Res.Ship.Height/2),
                    SpriteEffects.None, 0f
                );
                spriteBatch.End();
            }
        }
    }
}

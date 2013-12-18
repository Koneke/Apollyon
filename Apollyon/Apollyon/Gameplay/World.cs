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

        public World()
        {
            Ships = new List<Ship>();
            Camera = new Rectangle(
                0,
                0,
                (int)Game.ScreenSize.X,
                (int)Game.ScreenSize.Y
            );
        }

        public void Input(MouseState ms, MouseState oms)
        {
            int _scrollSpeed = 6;
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
            spriteBatch.Begin();
            foreach (Ship _s in Ships)
            {
                Point _shipPoint =
                    new Point((int)_s.Position.X, (int)_s.Position.Y);
                if (!Camera.Contains(_shipPoint))
                    continue;

                Vector2 _screenPosition = _s.Position - CameraPosition;
                //float _zoom = ((float)Camera.Width / Game.ScreenSize.X);
                float _zoom = (Game.ScreenSize.X / (float)Camera.Width);
                _screenPosition = _screenPosition * _zoom;

                spriteBatch.Draw(
                    Res.Ship,
                    new Rectangle(
                        (int)(_screenPosition.X),
                        (int)(_screenPosition.Y),
                        (int)(32*_zoom),
                        (int)(32*_zoom)
                    ),
                    null,
                    Color.White,
                    //(float)Math.Atan2(_s.Velocity.Y, _s.Velocity.X),
                    (float)_s.Direction,
                    new Vector2(Res.Ship.Width/2, Res.Ship.Height/2),
                    SpriteEffects.None, 0f
                );
            }
            spriteBatch.End();
        }
    }
}

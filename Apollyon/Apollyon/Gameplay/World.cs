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

        public float Timer; //ms timer

        Rectangle boxSelection;

        public World()
        {
            Ships = new List<Ship>();
            Camera = new Camera();
        }

        public void Input(MouseState ms, MouseState oms)
        {
            Camera.Input(ms, oms);

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

            if (
                ms.RightButton == ButtonState.Pressed &&
                oms.RightButton == ButtonState.Released
            ) {
                if (ApUI.ShipOverview.Selection.Count > 0)
                {
                    Vector2 _avgPosition = new Vector2(0, 0);

                    foreach (Ship _s in ApUI.ShipOverview.Selection)
                    {
                        _avgPosition.X += _s.Position.X;
                        _avgPosition.Y += _s.Position.Y;
                    }

                    _avgPosition =
                        _avgPosition / ApUI.ShipOverview.Selection.Count;

                    foreach (Ship _s in ApUI.ShipOverview.Selection)
                    {
                        Vector2 _avgPositionOffset =
                            _s.Position - _avgPosition;
                        _s.TargetPosition =
                            Camera.ScreenToWorld(
                                new Vector2(ms.X, ms.Y)
                            ) + _avgPositionOffset;
                        var a = 1;
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
                Timer -= Game.TickTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Ship _s in Ships)
            {
                Point _shipPoint =
                    new Point((int)_s.Position.X, (int)_s.Position.Y);
                //if (!Camera.Contains(_shipPoint))
                if (!Camera.Contains(_shipPoint))
                    continue;

                Vector2 _screenPosition = _s.Position - CameraPosition;
                _screenPosition =
                    _screenPosition *
                    new Vector2(
                        Camera.GetZoom(),
                        Camera.GetZoom()
                    );

                Rectangle _shipRect =
                    new Rectangle(
                        (int)(_screenPosition.X),
                        (int)(_screenPosition.Y),
                        (int)(32 * Camera.GetZoom()),
                        (int)(32 * Camera.GetZoom())
                    );

                Rectangle _shipRectOffset = _shipRect;
                _shipRectOffset.Offset(
                    (int)(-16 * Camera.GetZoom()),
                    (int)(-16 * Camera.GetZoom())
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
                        (int)(32 * Camera.GetZoom()),
                        (int)(32 * Camera.GetZoom())
                    ),
                    null,
                    Color.White,
                    (float)_s.Direction,
                    new Vector2(Res.Ship.Width / 2, Res.Ship.Height / 2),
                    SpriteEffects.None, 0f
                );
                spriteBatch.End();
            }
        }
    }
}

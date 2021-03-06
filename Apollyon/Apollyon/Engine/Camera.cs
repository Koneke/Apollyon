﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    class Camera
    {
        public Rectangle Rectangle;

        //Vector2? mouseGrabPoint;
        Vector2? cameraStartGrab;

        public int X
        {
            get { return Rectangle.X; }
            set { Rectangle.X = value; }
        }

        public int Y
        {
            get { return Rectangle.Y; }
            set { Rectangle.Y = value; }
        }

        public Camera()
        {
            Rectangle = new Rectangle(
                0,
                0,
                (int)Game.ScreenSize.X,
                (int)Game.ScreenSize.Y
            );
        }

        public float GetZoom()
        {
            return Game.ScreenSize.X / Rectangle.Width;
        }

        public void Input(MouseState ms, MouseState oms)
        {
            if (!Game.HasFocus) return;
            float _zoom = (Game.ScreenSize.X / (float)Rectangle.Width);

            if (
                ms.MiddleButton == ButtonState.Pressed &&
                oms.MiddleButton == ButtonState.Released)
                cameraStartGrab = new Vector2(ms.X, ms.Y);

            if (
                ms.MiddleButton == ButtonState.Released &&
                oms.MiddleButton == ButtonState.Pressed)
                cameraStartGrab = null;

            if (cameraStartGrab != null)
            {
                X += (int)((cameraStartGrab.Value.X - ms.X) / GetZoom());
                Y += (int)((cameraStartGrab.Value.Y - ms.Y) / GetZoom());
                cameraStartGrab = new Vector2(ms.X, ms.Y);
            }
            else //don't zoom/pan and stuff while mm dragging
            {
                int _scrollSpeed = (int)(9f / _zoom);
                if (ms.X > Game.ScreenSize.X)
                    Rectangle.X += _scrollSpeed;
                if (ms.Y > Game.ScreenSize.Y)
                    Rectangle.Y += _scrollSpeed;
                if (ms.X < 0)
                    Rectangle.X -= _scrollSpeed;
                if (ms.Y < 0)
                    Rectangle.Y -= _scrollSpeed;


                int _preW = Rectangle.Width;
                int _preH = Rectangle.Height;

                float _zoomSpeed = 1.1f;

                Rectangle.Width =
                    (int)(Rectangle.Width * 1 +
                    (Game.MouseWheelDelta * 120 * _zoomSpeed));
                Rectangle.Height =
                    (int)(Rectangle.Width * (9f / 16f));
                _zoom = (Game.ScreenSize.X / (float)Rectangle.Width);

                //work more on this to avoid glitching "through" the world
                if (_zoom > 6)
                {
                    Rectangle.Width = (int)Game.ScreenSize.X / 6;
                    Rectangle.Height = (int)Game.ScreenSize.Y / 6;
                }

                Rectangle.X -= (int)(
                    (Rectangle.Width - _preW) *
                    (ms.X / (Game.ScreenSize.X / 2f)) / 2f);
                Rectangle.Y -= (int)(
                    (Rectangle.Height - _preH) *
                    (ms.Y / (Game.ScreenSize.Y / 2f)) / 2f);
            }
        }

        public Vector2 ScreenToWorld(Vector2 _position)
        {
            Vector2 _camPos = new Vector2(Rectangle.X, Rectangle.Y);
            return _camPos + _position / GetZoom();
        }

        public Vector2 WorldToScreen(Vector2 _position)
        {
            Vector2 _camPos = new Vector2(Rectangle.X, Rectangle.Y);
            Vector2 _offs = (_position - _camPos) * GetZoom();
            return _offs;
        }

        public void CenterOn(Vector2 _position) {
            X = (int)(_position.X - Rectangle.Width / 2f);
            Y = (int)(_position.Y - Rectangle.Height / 2f);
        }
    }
}

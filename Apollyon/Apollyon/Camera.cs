using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    class Camera
    {
        public Rectangle Rectangle;
        int lastScrollWheelValue;

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
            lastScrollWheelValue = 0;
        }

        public Boolean Contains(Point _p)
        {
            return Rectangle.Contains(_p);
        }

        public float GetZoom()
        {
            return Game.ScreenSize.X / Rectangle.Width;
        }

        public void Input(MouseState ms, MouseState oms)
        {
            float _zoom = (Game.ScreenSize.X / (float)Rectangle.Width);

            int _scrollSpeed = (int)(9f/_zoom);
            if (ms.X > Game.ScreenSize.X)
                Rectangle.X += _scrollSpeed;
            if (ms.Y > Game.ScreenSize.Y)
                Rectangle.Y += _scrollSpeed;
            if (ms.X < 0)
                Rectangle.X -= _scrollSpeed;
            if (ms.Y < 0)
                Rectangle.Y -= _scrollSpeed;

            int _wDelta = (lastScrollWheelValue - ms.ScrollWheelValue);

            float _premsx = Rectangle.X + ms.X * _zoom;
            float _premsy = Rectangle.Y + ms.Y * _zoom;

            int _preW = Rectangle.Width;
            int _preH = Rectangle.Height;

            float _zoomSpeed = 1.1f;

            Rectangle.Width = (int)(Rectangle.Width * 1 + (_wDelta * _zoomSpeed));
            Rectangle.Height = (int)(Rectangle.Width * (9f / 16f));
            _zoom = (Game.ScreenSize.X / (float)Rectangle.Width);
            float _newmsx = Rectangle.X + ms.X * _zoom;
            float _newmsy = Rectangle.Y + ms.Y * _zoom;
            Rectangle.X -= (int)(
                (Rectangle.Width - _preW) * (ms.X / (Game.ScreenSize.X / 2f)) / 2f);
            Rectangle.Y -= (int)(
                (Rectangle.Height - _preH) * (ms.Y / (Game.ScreenSize.Y / 2f)) / 2f);

            lastScrollWheelValue = ms.ScrollWheelValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    class ApWindow : IDisposable, IInputReceiver
    {
        //move stuff over towards WindowManager?
        public WindowManager MyManager;

        #region static
        protected static GraphicsDevice graphics;
        //public static bool DrawHelp;

        public static void Setup(GraphicsDevice _graphics)
        {
            graphics = _graphics;
        }

        #endregion

        public void Dispose()
        {
            Target.Dispose();
        }

        public String Name;
        public string Help;
        public Color Tint;

        public XElement xml;

        public virtual void Update() { }

        public Vector2 PointToScreen(Vector2 _point)
        {
            return new Vector2(
                _point.X + x1,
                _point.Y + y1
            );
        }

        public void Bind(ApKeyBind _apk)
        {
            MyManager.MyState.InputManager.Bind(
                _apk, this
            );
        }

        public virtual void Receive(ApKeyBind _apk)
        {
        }

        //receive an action from a key they bound earlier
        //to be overridden by subclasses
        public virtual void GetAction(string _action)
        {
        }

        public virtual void SpecificUILoading()
        {
        }

        //standard colours to keep things fairly consistent across the board
        public static Color StandardBackground =
            new Color(0.2f,0.2f,0.2f,0.3f);

        public static Color StandardBorder =
            new Color(0.4f, 0.4f, 0.4f, 1f);

        //commented out since you might just as well use Res.LogFont directly,
        //might be put in later for standards reasons though.
        //public static SpriteFont StandardFont = Res.LogFont

        //the render target used to draw the window itself.
        //this means that we can do stuff like not having to redraw the window
        //every frame, or do effects and stuff on it, and ui rescaling, and
        //good stuff like that.
        public RenderTarget2D Target;

        public bool Draggable;

        public Vector2 Position;
        public Vector2 Size;

        //general position handling properties.
        public float x1 {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float y1 {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public float x2 {
            get { return x1 + Size.X; }
            set { Position.X = value - Size.X; }
        }

        public float y2 {
            get { return y1 + Size.Y; }
            set { Position.Y = value - Size.Y; }
        }

        //w and h are currently not settable.
        //to add this, we need to recreate the render target and such.
        //doable, just not implemented yet.
        public float w {
            get { return Size.X; }
        }

        public float h {
            get { return Size.Y; }
        }

        public Rectangle Area
        {
            get {
                return new Rectangle(
                    (int) x1,
                    (int) y1,
                    (int)(x2 - x1),
                    (int)(y2 - y1)
                );
            }
        }
        
        //general selfexplanatory setup
        public ApWindow(
            float _x,
            float _y,
            int   _width,
            int   _height
        ) {
            Target = new RenderTarget2D(
                graphics,
                _width, _height
            );
            x1 = _x;
            y1 = _y;
            Size.X = _width;
            Size.Y = _height;
            Draggable = true;
            Tint = new Color(1f, 1f, 1f, 1f);
        }

        //render preparation and finishing
        public void Render(SpriteBatch spriteBatch)
        {
            RenderTargetBinding[] _previousRT = graphics.GetRenderTargets();
            graphics.SetRenderTarget(Target);
            ActualRender(spriteBatch);
            /*if (DrawHelp)
            {
                spriteBatch.Begin();
                spriteBatch.End();
            }*/
            graphics.SetRenderTargets(_previousRT);
        }

        public void DrawHelp()
        {
            //spriteBatch.Draw(
            DrawManager.AddCall(
                new BasicDrawCall(
                    Res.Textures["1x1"],
                    new Rectangle(
                        (int)x1,
                        (int)y1,
                        (int)w,
                        (int)h
                    ),
                    null,
                    Utility.MultiplyColours(
                        StandardBorder,
                        new Color(0f, 0f, 0f, 0.7f)
                    ),
                    -12f
                )
            );
            DrawManager.AddCall(
                new TextDrawCall(
                    "log font",
                    Help??"",
                    PointToScreen(new Vector2(4, 4)),
                    Color.White,
                    -13f
                )
            );
        }

        //actual rendering of the stuff in the window
        //overridden by subclasses
        public virtual void ActualRender(
            SpriteBatch spriteBatch
        ) {
            graphics.Clear(Color.Blue);
        }

        //own input handling, specific to the subclass
        //overridden by subclasses
        public virtual void OwnInput(
            MouseState ms,
            MouseState oms
        ) {
        }

        public virtual void OwnInput(
            KeyboardState ks,
            KeyboardState oks,
            MouseState ms,
            MouseState oms
        ) {
            OwnInput(ms, oms); //defaulting
        }

        //convenience function
        public void DrawBorder(SpriteBatch spriteBatch, Color _c)
        {
            Rectangle _r = Area;
            Utility.DrawOutlinedRectangle(_r, _c);
        }

        //draw the window to the screen
        public void Draw() {
            DrawManager.AddCall(
                new BasicDrawCall(
                    Target,
                    Area,
                    null,
                    Color.White,
                    -1
                )
            );
        }
    }
}

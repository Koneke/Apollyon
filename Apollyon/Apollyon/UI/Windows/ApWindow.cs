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
    class ApWindow : IDisposable
    {
        //move stuff over towards WindowManager?

        public void Dispose()
        {
            Target.Dispose();
        }

        //to be set upon initialization to the graphicsdevice in Game1.
        //required for creating the rendertargets.
        //put here to avoid having to poke in Game1 every time we want to create
        //a new target, and because it's more readable this way.
        protected static GraphicsDevice graphics;

        public String Name;
        public string Help;
        public static bool DrawHelp;
        public Color Tint;

        public XElement xml;

        public static List<ApWindow> Windows;
        public static void Setup(GraphicsDevice _graphics)
        {
            Windows = new List<ApWindow>();
            graphics = _graphics;
        }

        //keeping track of currently dragged window and such
        static ApWindow grabbed = null;
        static Vector2 grabOffset = new Vector2(0,0);

        public virtual void Update() { }
        
        //handle window dragging (if draggable)
        //pushes the window to the front as well.
        //possible todo: push window to front even if not draggable? iunno
        //possible todo: snapping windows?
        public static void Input()
        {
            Input(
                InputManager.ks,
                InputManager.oks,
                InputManager.ms,
                InputManager.oms
            );
        }
        public static void Input(
            KeyboardState ks,
            KeyboardState oks,
            MouseState ms,
            MouseState oms
        ) {
            DrawHelp = ks.IsKeyDown(Keys.H);

            if (
                ms.RightButton == ButtonState.Pressed &&
                oms.RightButton == ButtonState.Released
            ) {
                for (int i = Windows.Count - 1; i >= 0; i--)
                {
                    if (!Windows[i].Draggable) continue;

                    Point _mousePoint = new Point(ms.X, ms.Y);
                    if (Windows[i].Area.Contains(_mousePoint))
                    {
                        grabOffset.X = _mousePoint.X - Windows[i].Position.X;
                        grabOffset.Y = _mousePoint.Y - Windows[i].Position.Y;
                        ApWindow _hit = Windows[i];
                        Windows.Remove(_hit);
                        Windows.Add(_hit); //push to front
                        grabbed = _hit;
                        break;
                    }
                }
            }
            else if (ms.RightButton == ButtonState.Pressed)
            {
                if (grabbed != null)
                {
                    grabbed.x1 = ms.X - grabOffset.X;
                    grabbed.y1 = ms.Y - grabOffset.Y;

                    //sample snapping
                    grabbed.x1 -= grabbed.x1 % 10;
                    grabbed.y1 -= grabbed.y1 % 10;

                    //oob checking (and recalcing graboffset) here
                }
            }
            else
            {
                grabbed = null;
            }

            foreach (ApWindow _w in Windows)
            {
                Point _mousePoint = new Point(ms.X, ms.Y);
                if (_w.Area.Contains(_mousePoint))
                {
                    _w.OwnInput(ks, oks, ms, oms);
                }
            }
        }

        public static ApWindow PointInWindow(Point _p)
        {
            foreach (ApWindow _w in Windows)
            {
                if (_w.Area.Contains(_p)) return _w;
            }
            return null;
        }

        //stop hardbinding to keys! skip _key, keep _type, get _key/_action
        //combo from data (to allow freely remapping keys to actions). 
        public void BindKey(
            Keys _key,
            KeyBindType _type,
            string _action
        ) {
            //tells the engine that this window wants this key, and it wants it
            //for _action. this allows us to do neat stuff such as displaying
            //binds and what not. what this actually does in the engine is that
            //unless the key is hooked on a higher level, it'll keep the window
            //updated about its state.
            BindingsManager.WindowKeyBindings.Add(
                new ApWindowKeyBind(_action, _key, _type, this)
            );
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

        Vector2 Position;
        Vector2 Size;

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
            //set { Size.X = value; }
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
            if (DrawHelp)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(
                    Res.Textures["1x1"],
                    new Rectangle(0,0,(int)w,(int)h),
                    Utility.MultiplyColours(
                        StandardBorder,
                        new Color(0f, 0f, 0f, 0.7f)
                    )
                );
                spriteBatch.DrawString(
                    Res.GetFont("log font"),
                    Help??"",
                    new Vector2(4, 4),
                    Color.White
                );
                spriteBatch.End();
            }
            graphics.SetRenderTargets(_previousRT);
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
            _r.X = 0;
            _r.Y = 0;
            Utility.DrawOutlinedRectangle(spriteBatch, _r, _c);
        }

        //draw the window to the screen
        public void Draw(
            SpriteBatch spriteBatch
        ) {
            spriteBatch.Begin();
            spriteBatch.Draw(
                Target, Area, Color.White
            );
            spriteBatch.End();
        }
    }
}

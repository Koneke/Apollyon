using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class WindowManager
    {
        public List<ApWindow> Windows;
        public GameState MyState;

        public WindowManager()
        {
            Windows = new List<ApWindow>();
        }

        public ApWindow GetWindowByName(string _name)
        {
            return Windows.Find(x => x.Name.Equals(_name));
        }

        public void UpdateAll()
        {
            foreach (ApWindow _w in Windows)
                _w.Update();
        }

        //NOTICE: THESE ARE ALLOWED TO USE SPRITEBATCH, AS THEY USE THAT TO
        //RENDER THE WINDOW'S OWN TARGET, NOT TO THE SCREEN DIRECTLY.
        //THE TARGETS THEMSELVES ARE THEN DRAWN WITH DRAWMANAGER.
        //DON'T CLEAN THIS SPRITEBATCH UP, ALTEAST NOT NOW.
        public void RenderAll(SpriteBatch spriteBatch)
        {
            foreach (ApWindow w in Windows)
                w.Render(spriteBatch);
        }

        public void DrawAll()
        {
            foreach (ApWindow w in Windows)
            {
                w.Draw();
                if (DrawHelp) w.DrawHelp();
            }
        }

        Vector2 grabOffset;
        bool DrawHelp = false;
        ApWindow grabbed = null;

        public void Input()
        {
            DrawHelp = InputManager.ks.IsKeyDown(Keys.H);

            if (
                InputManager.ms.RightButton == ButtonState.Pressed &&
                InputManager.oms.RightButton == ButtonState.Released
            ) {
                for (int i = Windows.Count - 1; i >= 0; i--)
                {
                    if (!Windows[i].Draggable) continue;

                    Point _mousePoint = new Point(
                        InputManager.ms.X,
                        InputManager.ms.Y
                    );

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
            else if (
                InputManager.ms.RightButton == ButtonState.Pressed)
            {
                if (grabbed != null)
                {
                    grabbed.x1 = InputManager.ms.X - grabOffset.X;
                    grabbed.y1 = InputManager.ms.Y - grabOffset.Y;

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
                Point _mousePoint = new Point(
                    InputManager.ms.X,
                    InputManager.ms.Y
                );
                if (_w.Area.Contains(_mousePoint))
                {
                    _w.OwnInput(
                        InputManager.ks,
                        InputManager.oks,
                        InputManager.ms,
                        InputManager.oms
                    );
                }
            }
        }

        public ApWindow PointInWindow(Point _p)
        {
            foreach (ApWindow _w in Windows)
            {
                if (_w.Area.Contains(_p)) return _w;
            }
            return null;
        }

        public void Load(string _path)
        {
            IEnumerable<XElement> _elements =
                //(XElement.Load("Content/data/ui.xml")).Elements();
                (XElement.Load(_path)).Elements();

            foreach (var _e in _elements)
            {
                string _name    = _e.Element("name").Value;
                string _type    = _e.Element("type").Value;

                var _position   = _e.Element("position");

                int _x, _y, _w, _h;
                Int32.TryParse(_position.Element("x").Value, out _x);
                Int32.TryParse(_position.Element("y").Value, out _y);
                Int32.TryParse(_position.Element("w").Value, out _w);
                Int32.TryParse(_position.Element("h").Value, out _h);

                ApWindow _new = null;

                switch (_type)
                {
                    case "CombatLog":
                        _new = new ApLogWindow(_x, _y, _w, _h); break;
                    case "ComponentOverview":
                        _new = new ApComponentOverview(_x, _y, _w, _h); break;
                    case "StatusWindow":
                        _new = new ApStatusWindow(_x, _y, _w, _h); break;
                    case "Inventory":
                        _new = new ApInventory(_x, _y, _w, _h); break;
                    case "Overview":
                        _new = new ApOverview(_x, _y, _w, _h); break;
                    default:
                        throw new System.FormatException("BAD UI TYPE");
                }

                _new.Name = _name;
                _new.MyManager = this;
                if (_e.Element("help") != null)
                {
                    _new.Help =
                        Utility.WrapText(
                        Res.GetFont("log font"),
                        _e.Element("help").Value,
                        _w
                    );

                }

                if (_e.Elements("tint").Any())
                {
                    var _tint = _e.Element("tint");
                    string _rs = _tint.Element("r").Value;
                    string _gs = _tint.Element("g").Value;
                    string _bs = _tint.Element("b").Value;
                    string _as = _tint.Element("a").Value;

                    Int32 _r, _g, _b, _a;
                    Int32.TryParse(_rs, out _r);
                    Int32.TryParse(_gs, out _g);
                    Int32.TryParse(_bs, out _b);
                    Int32.TryParse(_as, out _a);
                    Color _colour = new Color(_r, _g, _b, _a);

                    _new.Tint = _colour;
                }

                var _binds      = _e.Element("bindings");
                if (_binds != null)
                {
                    foreach (var _bind in _binds.Elements())
                    {
                        string _key = _bind.Element("key").Value.ToUpper();
                        string _bindType =
                            _bind.Element("type").Value.ToLower();
                        _bindType =
                            Char.ToUpper(_bindType[0]) +
                            _bindType.Substring(1, _bindType.Length - 1);
                        string _action = _bind.Element("action").Value;

                        Keys _k = (Keys)Enum.Parse(
                            typeof(Keys),
                            _key
                        );

                        KeyBindType _t = (KeyBindType)Enum.Parse(
                            typeof(KeyBindType),
                            _bindType
                        );

                        _new.Bind(
                            new ApKeyBind(
                                _action, _k, _t
                            )
                        );
                    }
                }

                _new.xml = _e;
                Windows.Add(_new);
            }

            foreach (ApWindow _w in Windows)
                _w.SpecificUILoading();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Apollyon
{
    static class UILoader
    {
        public static void Load()
        {
            IEnumerable<XElement> _elements =
                (XElement.Load("Content/data/ui.xml")).Elements();

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

                        _new.BindKey(
                            _k, _t, _action
                        );
                    }
                }

                _new.xml = _e;
                WindowManager.Windows.Add(_new);
            }
        }
    }
}

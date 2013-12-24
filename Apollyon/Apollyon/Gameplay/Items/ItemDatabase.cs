using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class WeaponTemplate
    {
        public int Damage;
        public int Frequency;
        public int BeamThickness;
        public int Range;
        public Color BeamTint;
    }

    class ItemTemplate
    {
        public string Name;
        public int ID;
        public Texture2D Texture;
        public List<string> Tags;
        //public bool Stacking;

        public WeaponTemplate Weapon;
    }

    static class ItemDatabase
    {
        public static List<ItemTemplate> Items = new List<ItemTemplate>();

        public static void LoadData()
        {
            IEnumerable<XElement> _elements =
                (XElement.Load("Content/data/items.xml")).Elements();

            foreach (XElement _e in _elements)
            {
                ItemTemplate _template = new ItemTemplate();

                _template.Name = _e.Element("name").Value;

                Int32.TryParse(
                    _e.Element("id").Value,
                    out _template.ID);

                _template.Texture = Res.Textures[_e.Element("texture").Value];

                _template.Tags = new List<string>();
                XElement _tags = _e.Element("tags");
                foreach (XElement _tag in _tags.Elements())
                {
                    _template.Tags.Add(_tag.Value);
                }

                /*_template.Stacking = _e.Element("stacking").
                    Value.Equals("false") ?
                    false : true;*/

                XElement _weapon = _e.Element("weapon");
                if (_weapon != null)
                {
                    _template.Weapon = new WeaponTemplate();

                    Int32.TryParse(
                        _weapon.Element("damage").Value,
                        out _template.Weapon.Damage);

                    Int32.TryParse(
                        _weapon.Element("frequency").Value,
                        out _template.Weapon.Frequency);

                    Int32.TryParse(
                        _weapon.Element("beamthickness").Value,
                        out _template.Weapon.BeamThickness);

                    Int32.TryParse(
                        _weapon.Element("range").Value,
                        out _template.Weapon.Range);

                    XElement _tint = _weapon.Element("beamtint");
                    if (_tint != null)
                    {
                        string _rs = _tint.Element("r").Value;
                        string _gs = _tint.Element("g").Value;
                        string _bs = _tint.Element("b").Value;
                        string _as = _tint.Element("a").Value;

                        int _r, _g, _b, _a;
                        Int32.TryParse(_rs, out _r);
                        Int32.TryParse(_gs, out _g);
                        Int32.TryParse(_bs, out _b);
                        Int32.TryParse(_as, out _a);

                        Color _beamtint = new Color(_r, _g, _b, _a);
                        _template.Weapon.BeamTint = _beamtint;
                    }
                }

                Items.Add(_template);
            }
        }

        public static Item Spawn(ItemTemplate _template)
        {
            Item _i = new Item();

            if (_template.Weapon != null)
            {
                //_i = new ComponentItem();

            }
            /*else {
                _i = new Item();*/

            _i.Name = _template.Name;
            _i.ID = _template.ID;
            _i.Texture = _template.Texture;
            _i.Tags = new List<string>(_template.Tags);
            //_i.Stacking = _template.Stacking;

            if (_template.Weapon != null)
            {
                //ComponentItem _ci = (ComponentItem)_i;
                Weapon _w = new Weapon(
                    _template.Name,
                    _template.ID);
                _w.Damage = _template.Weapon.Damage;
                _w.Frequency = _template.Weapon.Frequency;
                _w.BeamThickness = _template.Weapon.BeamThickness;
                _w.Range = _template.Weapon.Range;
                if(Utility.SumColour(_template.Weapon.BeamTint) != 0)
                    _w.BeamTint = _template.Weapon.BeamTint;
                _w.Item = _i;
                _i.Component = _w;
                _i.Component.Item = _i; //reference upwards for the component
                //_i = _ci;
                //return _ci;
            }

            return _i;
        }
    }
}

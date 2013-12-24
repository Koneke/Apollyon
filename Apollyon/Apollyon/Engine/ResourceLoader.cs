using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Apollyon
{
    static class ResourceLoader
    {
        public static void Load(ContentManager _cm) //add path var later?
        {
            IEnumerable<XElement> _textures =
                (XElement.Load("Content/data/resources.xml"))
                .Elements("texture");

            foreach (XElement _e in _textures) 
            {
                Res.Textures.Add(
                    _e.Element("name").Value,
                    _cm.Load<Texture2D>(_e.Element("path").Value)
                );
            }

            IEnumerable<XElement> _fonts =
                (XElement.Load("Content/data/resources.xml"))
                .Elements("font");

            foreach (XElement _e in _fonts) 
            {
                Res.Fonts.Add(
                    _e.Element("name").Value,
                    _cm.Load<SpriteFont>(_e.Element("path").Value)
                );
            }
        }
    }
}

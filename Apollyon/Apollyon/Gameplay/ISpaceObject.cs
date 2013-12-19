using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    public interface ISpaceObject
    {
        string GetName();
        Texture2D GetTexture();
        Vector2 GetPosition();
        Vector2 GetSize();
        double GetRotation();
        List<string> GetTags();
        bool GetVisible();
        float GetDepth();
    }
}

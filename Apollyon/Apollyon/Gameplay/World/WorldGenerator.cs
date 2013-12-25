using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class WorldGenerator
    {
        public virtual World Generate(World _world)
        {
            throw new Exception("Not yet implemented. Use DevWorldGenerator.");
        }
    }
}

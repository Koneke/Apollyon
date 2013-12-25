using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class GameState : IGameState
    {
        public World World;

        MouseState ms, oms;
        KeyboardState ks, oks;

        public void Update(GameTime gameTime)
        {
            if (World == null) return;

            World.Input();
            World.Update(gameTime);
        }

        public void Draw()
        {
            DrawManager.AddCall(
                new BasicDrawCall(
                    Res.Textures["background"],
                    new Rectangle(
                        0,
                        0,
                        (int)Game.ScreenSize.X,
                        (int)Game.ScreenSize.Y
                    ),
                    null,
                    Color.White,
                    10f
                )
            );
            World.Draw();
        }
    }
}

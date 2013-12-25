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

        public void Draw(SpriteBatch spriteBatch)
        {
            //bg will be here later, we just need to solved removing local
            //drawing all over the goddamn place so we can have proper depth
            //sorted stuff across the entire game.
            /*spriteBatch.Begin();
            spriteBatch.Draw(
                Res.Textures["background"],
                new Rectangle(
                    0,
                    0,
                    (int)Game.ScreenSize.X,
                    (int)Game.ScreenSize.Y
                ),
                Color.White);
            spriteBatch.End();*/
            World.Draw(spriteBatch);
        }
    }
}

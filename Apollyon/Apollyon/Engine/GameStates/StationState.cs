using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    //docked in station game state
    //display market and manage ships and stuff here.
    class StationState : GameState
    {
        public Station Station;

        public override void Update(GameTime gameTime) {
            WindowManager.Input();
            WindowManager.UpdateAll();

            if (InputManager.ks.IsKeyDown(
                Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Game.ActiveStates.Remove(this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            WindowManager.RenderAll(spriteBatch);
            WindowManager.DrawAll();

            DrawManager.AddCall(
                new TextDrawCall(
                    "log font",
                    Station.Name,
                    new Vector2(0, 0),
                    Color.White,
                    -11f
                )
            );
        }

        public override void Load()
        {
            WindowManager.Load("Content/data/stationui.xml");
        }
    }
}

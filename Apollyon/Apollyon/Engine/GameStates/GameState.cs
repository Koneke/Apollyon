using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    class GameState
    {
        public WindowManager WindowManager;
        public InputManager InputManager;

        public GameState()
        {
            WindowManager = new WindowManager();
            InputManager = new InputManager();
            WindowManager.MyState = this;
            InputManager.MyState = this;
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
        public virtual void Input() { }
        public virtual void Load() { }

        public bool IsTopState()
        {
            return Game.ActiveStates[
                Game.ActiveStates.Count - 1] == this;
        }
    }
}

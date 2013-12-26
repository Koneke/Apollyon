﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Apollyon
{
    //main game state, for when we are out in space
    class SpaceState : GameState
    {
        public World World;

        public SpaceState() : base()
        {
        }

        public override void Load()
        {
            WindowManager.Load("Content/data/ui.xml");
        }

        public override void Input()
        {
            InputManager.Update();

            if (InputManager.ks.IsKeyDown(Keys.M) &&
                !InputManager.oks.IsKeyDown(Keys.M))
            {
                Audio.Mute = !Audio.Mute;
                Audio.bgm.Volume = Audio.Mute ? 0 : 0.05f;
            }

            WindowManager.Input();
            //HERE IS WHERE WE GIVE INPUT TO CAMERA, WM OR WORLD
            World.Input();
        }

        public override void Update(GameTime gameTime)
        {
            if (World == null) return;

            foreach (string _s in
                UIBindings.ShipLists.Keys.ToList().FindAll(x=>true)
            )
                UIBindings.Bind(_s, UIBindings.Get(_s).FindAll(
                    x => x.Health > 0));

            //ugly, update
            Particle.Particles =
                Particle.Particles.FindAll(
                    x => (DateTime.Now - x.Created).TotalMilliseconds
                        < x.LifeTime);
            Particle2.Update();
            foreach (Particle _p in Particle.Particles)
                _p.Update();

            WindowManager.Input();
            WindowManager.UpdateAll();

            World.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            WindowManager.RenderAll(spriteBatch);
            WindowManager.DrawAll();

            foreach (Particle _p in Particle.Particles)
                _p.Draw();
            Particle2.Draw();

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

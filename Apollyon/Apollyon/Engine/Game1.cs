using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using IrrKlang;

namespace Apollyon
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //several gamestates can be running at once, but only one can be drawn
        //at a time.
        List<IGameState> activeStates;
        IGameState drawState;

        string cwd;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            cwd = System.IO.Directory.GetCurrentDirectory();
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            Game.ScreenSize.X = graphics.PreferredBackBufferWidth;
            Game.ScreenSize.Y = graphics.PreferredBackBufferHeight;

            UIBindings.Bind("Selected", new List<SpaceObject>());
            UIBindings.Bind("Targeted", new List<SpaceObject>());
            UIBindings.Bind("All", new List<SpaceObject>());

            activeStates = new List<IGameState>();

            GameState _gs = new GameState();

            Game.World = new World();
            DevWorldGenerator _dwg = new DevWorldGenerator();
            _dwg.Generate(Game.World);

            _gs.World = Game.World;
            activeStates.Add(_gs);
            drawState = _gs;

            Game.Camera = Game.World.Camera;
            Audio.bgm = Audio.Play("mus/mAmbience.ogg", 0.05f, false, true);

            //REMEMBER: convenience while deving
            //(so i can spawn at 0,0 and still see stuff)
            Game.Camera.X -= 600;
            Game.Camera.Y -= 400;

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ResourceLoader.Load(Content);

            ApWindow.Setup(graphics.GraphicsDevice);
            UILoader.Load();

            ApWindow.Windows = WindowManager.Windows;
            WindowManager.Load();

            ItemDatabase.LoadData(); //move to load
            Audio.UpdateSettings();
            Audio.ContentRoot = cwd + "/Content/";
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Game.HasFocus = IsActive;

            InputManager.UpdateStart();
            Audio.UpdateListenerPosition();
            //use the BM more?
            BindingsManager.HandleInput();

            //clean lists to filter out dead ships
            //<later me> are we not already doing this locally?
            //most of the time, atleast? idk, check it out
            foreach (string _s in
                UIBindings.ShipLists.Keys.ToList().FindAll(x=>true)
            )
                UIBindings.Bind(_s, UIBindings.Get(_s).FindAll(
                    x => x.Health > 0));

            ApWindow.Input();

            //update all active states
            foreach (IGameState _gs in activeStates)
                _gs.Update(gameTime);

            foreach (ApWindow _w in ApWindow.Windows)
                _w.Update();

            //should be moved. switch these over from static to being
            //per world?
            Particle.Particles =
                Particle.Particles.FindAll(
                    x => (DateTime.Now - x.Created).TotalMilliseconds
                        < x.LifeTime);
            Particle2.Update();
            foreach (Particle _p in Particle.Particles)
                _p.Update();

            InputManager.UpdateEnd();
        }

        protected override void Draw(GameTime gameTime)
        {
            WindowManager.RenderAll(spriteBatch);

            graphics.GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            //to be moved asap
            spriteBatch.Begin();
            spriteBatch.Draw(
                Res.Textures["background"],
                new Rectangle(
                    0,
                    0,
                    graphics.PreferredBackBufferWidth,
                    graphics.PreferredBackBufferHeight),
                Color.White);
            spriteBatch.End();

            foreach (Particle _p in
                Particle.Particles.FindAll(x => x.Depth < 0))
                _p.Draw(spriteBatch);
            Particle2.Draw(spriteBatch);

            drawState.Draw(spriteBatch);

            foreach (Particle _p in
                Particle.Particles.FindAll(x => x.Depth > 0))
                _p.Draw(spriteBatch);

            WindowManager.DrawAll(spriteBatch);

            spriteBatch.Begin();
            DrawManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

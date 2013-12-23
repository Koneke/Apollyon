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

        MouseState ms;
        MouseState oms; //last frame's
        KeyboardState ks;
        KeyboardState oks;

        World world;

        ISoundEngine soundEngine;
        string cwd;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            soundEngine = new ISoundEngine();
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

            LoadUI();
            ItemDatabase.LoadData(); //move to load
            world = new World();
            Game.World = world;
            Game.Camera = world.Camera;

            //REMEMBER: convenience while deving
            //(so i can spawn at 0,0 and still see stuff)
            Game.Camera.X -= 600;
            Game.Camera.Y -= 400;

            //test stuff
            Ship _s = new Ship(new Vector2(100, 300));
            world.SpaceObjects.Add(_s);
            UIBindings.Get("All").Add(_s);

            _s.AddItem(ItemDatabase.Spawn(
                ItemDatabase.Items.Find(x => x.ID == 1101)));
            _s.AddItem(ItemDatabase.Spawn(
                ItemDatabase.Items.Find(x => x.ID == 1101)));
            _s.AddItem(ItemDatabase.Spawn(
                ItemDatabase.Items.Find(x => x.ID == 1101)));
            _s.AddItem(ItemDatabase.Spawn( //spawn into inventory
                ItemDatabase.Items.Find(x => x.ID == 1199)));
            ItemDatabase.Spawn( //spawn into space
                ItemDatabase.Items.Find(x => x.ID == 1100))
                .SetPosition(new Vector2(100, 100));

            AISimpleMiner _AI = new AISimpleMiner();
            Game.AIs.Add(_AI);

            _s = new Ship(new Vector2(550, 100));
            world.SpaceObjects.Add(_s);
            UIBindings.Get("All").Add(_s);
            _s.AddItem(ItemDatabase.Spawn(
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _AI.Fleet.Add(_s);

            _s = new Ship(new Vector2(100, 100));
            world.SpaceObjects.Add(_s);
            UIBindings.Get("All").Add(_s);
            _s.AddItem(ItemDatabase.Spawn(
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _s.AddItem(ItemDatabase.Spawn(
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _AI.Fleet.Add(_s);

            Asteroid _a = new Asteroid();
            _a.Position = new Vector2(400, 400);

            _a = new Asteroid();
            _a.Position = new Vector2(0, 80);

            Container _c = new Container();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ResourceLoader.Load(Content);
            Audio.UpdateSettings();
            Audio.ContentRoot = cwd + "/Content/";
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            Audio.UpdateListenerPosition();

            Game.HasFocus = IsActive;

            BindingsManager.HandleInput(ks.GetPressedKeys());

            ApWindow.Input(ks, oks, ms, oms);
            world.Input(ks, oks, ms, oms);
            world.Update(gameTime);
            foreach (ApWindow _w in ApWindow.Windows)
                _w.Update();

            Particle.Particles =
                Particle.Particles.FindAll(
                    x => (DateTime.Now - x.Created).TotalMilliseconds
                        < x.LifeTime);

            Particle2.Update();

            foreach (Particle _p in Particle.Particles)
                _p.Update();

            oms = ms;
            oks = ks;
        }

        protected override void Draw(GameTime gameTime)
        {
            foreach (ApWindow w in ApWindow.Windows)
                w.Render(spriteBatch);

            graphics.GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

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

            world.Draw(spriteBatch);

            foreach (Particle _p in
                Particle.Particles.FindAll(x => x.Depth > 0))
                _p.Draw(spriteBatch);

            foreach (ApWindow w in ApWindow.Windows)
                w.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        void LoadUI()
        {
            ApWindow.graphics = graphics.GraphicsDevice;
            ApWindow.Setup();

            UILoader foo = new UILoader();
            foo.Load();

            ApWindow.Windows = WindowManager.Windows;

            foreach (ApWindow _w in WindowManager.Windows)
                _w.SpecificUILoading();
        }
    }
}

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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            Game.ScreenSize.X = graphics.PreferredBackBufferWidth;
            Game.ScreenSize.Y = graphics.PreferredBackBufferHeight;

            Game.Selected = new List<Ship>();
            Game.Targeted = new List<Ship>();
            UIBindings.Bind("Selected", Game.Selected);
            UIBindings.Bind("Targeted", Game.Targeted);
            UIBindings.Bind("All", Game.Fleet);

            LoadUI();
            ItemDatabase.LoadData();
            world = new World();
            Game.World = world;
            Game.Camera = world.Camera;

            //test stuff
            Ship _s = new Ship(new Vector2(100, 300));
            //world.Ships.Add(_s);
            world.SpaceObjects.Add(_s);
            Game.Fleet.Add(_s);

            _s.AddComponent(new Weapon("Railgun", 1100));
            Weapon _blaster = new Weapon("Heavy Blaster", 1101);
            _blaster.Frequency = 140;
            _blaster.Damage = 7;
            _blaster.BeamThickness = 3;
            /*ComponentItem _ci = new ComponentItem(
                "Heavy Blaster",
                2002, //1000-2000 weapons
                      //2000-3000 the items for those weapons
                _blaster);
            _ci.Carrier = _s;
            _s.AddItem(_ci);*/
            _s.AddItem(
                ItemDatabase.Spawn(
                ItemDatabase.Items.Find(x => x.ID == 1100)
                )
            );

            ComponentItem _ci = new ComponentItem(
                "Railgun",
                2001,
                new Weapon("Railgun", 1100)
            );
            _ci.Position = _s.Position;
            Game.World.SpaceObjects.Add(_ci);

            _s = new Ship(new Vector2(300, 100));
            //world.Ships.Add(_s);
            world.SpaceObjects.Add(_s);
            Game.Fleet.Add(_s);
            _s.AddComponent(
                new Weapon("Railgun", 1001)
            );
        }

        protected override void LoadContent()
        {
            //data-driven pls
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Res.OneByOne = Content.Load<Texture2D>("gfx/1x1");
            Res.Background = Content.Load<Texture2D>("gfx/background");
            Res.Ship = Content.Load<Texture2D>("gfx/ship");
            Res.LogFont = Content.Load<SpriteFont>("Logfont");
            Res.Textures.Add("generic", Content.Load<Texture2D>("gfx/generic"));
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            Game.HasFocus = IsActive;

            BindingsManager.HandleInput(ks.GetPressedKeys());

            ApWindow.Input(ms, oms);
            world.Input(ks, oks, ms, oms);
            world.Update(gameTime);
            ApWindow.Update();

            Particle.Particles =
                Particle.Particles.FindAll(
                    x => (DateTime.Now - x.Created).TotalMilliseconds
                        < x.LifeTime);
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
                Res.Background,
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

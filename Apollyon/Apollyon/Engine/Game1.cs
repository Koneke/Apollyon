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

            LoadUI();
            world = new World();

            //test stuff
            Ship _s = new Ship();
            _s.Position = new Vector2(100, 300);
            _s.Direction = 0;
            _s.Speed = 1;
            _s.TargetPosition = new Vector2(300, 100);
            world.Ships.Add(_s);
            Game.Fleet.Add(_s);
            _s.AddComponent(
                new Weapon("Railgun")
            );
            Weapon _blaster = new Weapon("Heavy Blaster");
            _blaster.Frequency = 140;
            _blaster.Damage = 7;
            ComponentItem _ci = new ComponentItem(
                "Heavy Blaster",
                _blaster);
            _s.Inventory.Add(_ci);
            _s.Inventory.Add(new Item("Coal", true, 5));

            _s = new Ship();
            _s.Position = new Vector2(300, 100);
            _s.Speed = 1;
            _s.Inventory.Add(new Item("Coal", true, 5));
            _s.Inventory.Add(new Item("Railgun", true, 1));
            world.Ships.Add(_s);
            Game.Fleet.Add(_s);

            ApUI.CombatLog.Log.Add(
                "Hi! I'm a combat log!");
        }

        protected override void LoadContent()
        {
            //data-driven pls
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Res.OneByOne = Content.Load<Texture2D>("gfx/1x1");
            Res.Background = Content.Load<Texture2D>("gfx/background");
            Res.Ship = Content.Load<Texture2D>("gfx/ship");
            Res.LogFont = Content.Load<SpriteFont>("Logfont");
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
            world.Input(ms, oms);
            world.Update(gameTime);
            ApWindow.Update();

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

            world.Draw(spriteBatch);

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

            ApUI.CombatLog =
                (ApLogWindow)WindowManager.
                GetWindowByName("Combat Log");

            ApUI.ShipOverview =
                (ApShipOverview)WindowManager.
                GetWindowByName("Fleet Overview");

            ApUI.HostileOverview =
                (ApShipOverview)WindowManager.
                GetWindowByName("Hostile Overview");

            ApUI.ComponentOverview =
                (ApComponentOverview)WindowManager.
                GetWindowByName("Component Overview");

            ApUI.HostileComponentOverview =
                (ApComponentOverview)WindowManager.
                GetWindowByName("Hostile Component Overview");

            ApUI.SelectionStatus =
                (ApStatusWindow)WindowManager.
                GetWindowByName("Selection Status");

            ApUI.HostileSelectionStatus =
                (ApStatusWindow)WindowManager.
                GetWindowByName("Target Status");

            ApUI.Inventory =
                (ApInventory)WindowManager.
                GetWindowByName("Selection Inventory");

            foreach (ApWindow _w in WindowManager.Windows)
                _w.SpecificUILoading();

        }
    }
}

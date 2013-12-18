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

            ApWindow.graphics = graphics.GraphicsDevice;
            ApWindow.Setup();

            ApUI.CombatLog = 
                new ApLogWindow(
                    10, 10, 200, 200
                );
            ApWindow.Windows.Add(ApUI.CombatLog);

            ApUI.ShipOverview =
                new ApShipOverview(
                    220, 10, 200, 200
                );
            ApWindow.Windows.Add(ApUI.ShipOverview);
            ApUI.ShipOverview.list = Game.Fleet; //player fleet of ships
            ApUI.ShipOverview.BindKey(
                Keys.D,
                KeyBindType.Press,
                "Clear Selection"
            );

            ApUI.HostileOverview =
                new ApShipOverview(
                    430, 10, 200, 200
                );
            ApWindow.Windows.Add(ApUI.HostileOverview);
            ApUI.HostileOverview.Tint = new Color(1f, 0f, 0f, 1f);
            ApUI.HostileOverview.list = Game.Fleet; //player fleet of ships
            ApUI.HostileOverview.BindKey(
                Keys.F,
                KeyBindType.Press,
                "Clear Selection"
            );

            ApUI.ComponentOverview =
                new ApComponentOverview(
                    220, 220, 200, 200
                );
            ApWindow.Windows.Add(ApUI.ComponentOverview);
            ApUI.ComponentOverview.Ships = ApUI.ShipOverview.Selection;

            ApUI.HostileComponentOverview =
                new ApComponentOverview(
                    10, 310, 200, 200
                );
            ApWindow.Windows.Add(ApUI.HostileComponentOverview);
            ApUI.HostileComponentOverview.Tint = new Color(1f, 0f, 0f, 1f);
            ApUI.HostileComponentOverview.Ships =
                ApUI.HostileOverview.Selection;

            ApUI.SelectionStatus =
                new ApStatusWindow(
                    10, 220, 200, 80
                );
            ApWindow.Windows.Add(ApUI.SelectionStatus);
            ApUI.SelectionStatus.Ships = ApUI.ShipOverview.Selection;

            ApUI.HostileSelectionStatus =
                new ApStatusWindow(
                    430, 220, 200, 80
                );
            ApWindow.Windows.Add(ApUI.HostileSelectionStatus);
            ApUI.HostileSelectionStatus.Tint = new Color(1f, 0f, 0f, 1f);
            ApUI.HostileSelectionStatus.Ships = ApUI.HostileOverview.Selection;

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
            _s.AddComponent(_blaster);

            _s = new Ship();
            _s.Position = new Vector2(300, 100);
            _s.Speed = 1;
            world.Ships.Add(_s);
            Game.Fleet.Add(_s);


            ApUI.CombatLog.BindKey(Keys.C, KeyBindType.Press, "Clear log");
            ApUI.CombatLog.Log.Add(
                "Hi! I'm a combat log!");
            //end of test stuff
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Res.OneByOne = Content.Load<Texture2D>("1x1");
            Res.Background = Content.Load<Texture2D>("background");
            Res.Ship = Content.Load<Texture2D>("ship");
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

            BindingsManager.HandleInput(ks.GetPressedKeys());

            ApWindow.Input(ms, oms);
            world.Input(ms, oms);
            world.Update(gameTime);

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
    }
}

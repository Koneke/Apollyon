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
        List<GameState> activeStates;
        GameState drawState;

        SpaceState spaceState;
        StationState stationState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            drawState = Game.SpaceState = spaceState = new SpaceState();
            stationState = new StationState();

            base.Initialize();

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            Game.ScreenSize.X = graphics.PreferredBackBufferWidth;
            Game.ScreenSize.Y = graphics.PreferredBackBufferHeight;

            UIBindings.Bind("Selected", new List<SpaceObject>());
            UIBindings.Bind("Targeted", new List<SpaceObject>());
            UIBindings.Bind("All", new List<SpaceObject>());

            activeStates = new List<GameState>();

            Game.World = new World();
            DevWorldGenerator _dwg = new DevWorldGenerator();
            _dwg.Generate(Game.World);

            spaceState.World = Game.World;
            activeStates.Add(spaceState);

            foreach (GameState _gs in activeStates)
                _gs.Load();

            Game.Camera = new Camera();
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

            ItemDatabase.LoadData(); //move to load
            Audio.UpdateSettings();
            Audio.ContentRoot =
                System.IO.Directory.GetCurrentDirectory() + "/Content/";
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

            //update all active states
            foreach (GameState _gs in activeStates)
                _gs.Update(gameTime);

            InputManager.UpdateEnd();
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            drawState.Draw(spriteBatch);

            spriteBatch.Begin();
            DrawManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

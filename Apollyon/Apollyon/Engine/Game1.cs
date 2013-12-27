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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            StationState stationState;

            Game.AddState("space", new SpaceState());
            //Game.SpaceState = spaceState = new SpaceState();
            stationState = new StationState();

            base.Initialize();

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            Game.ScreenSize.X = graphics.PreferredBackBufferWidth;
            Game.ScreenSize.Y = graphics.PreferredBackBufferHeight;
            Game.Camera = new Camera();
            Game.World = (new DevWorldGenerator()).Generate(new World());

            UIBindings.Bind("Selected", new List<SpaceObject>());
            UIBindings.Bind("Targeted", new List<SpaceObject>());
            UIBindings.Bind("All", new List<SpaceObject>());

            //spaceState.World = Game.World;
            ((SpaceState)Game.GetState("space")).World = Game.World;
            Game.ActiveStates.Add(Game.GetState("space"));

            stationState.Station = new Station(Game.World);
            stationState.Station.Name = "The Spooky Sailor Cafe";
            Ship _s = new Ship(
                Vector2.Zero,
                Game.World
            );
            _s.Faction = Game.PlayerFaction;
            Game.World.SpaceObjects.Add(_s);
            stationState.Station.Dock(_s);
            Game.ActiveStates.Add(stationState);

            foreach (GameState _gs in Game.ActiveStates)
                _gs.Load();

            Audio.bgm = Audio.Play("mus/mAmbience.ogg", 0.05f, false, true);
            Audio.bgm.Volume = Audio.Mute ? 0 : 0.05f;

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

            //only take input from the front state as of now
            Game.ActiveStates[Game.ActiveStates.Count-1].Input();

            //update all active states
            foreach (GameState _gs in Game.ActiveStates.FindAll(x=>true))
                _gs.Update(gameTime);

            InputManager.UpdateEnd();
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            Game.ActiveStates[Game.ActiveStates.Count-1].Draw(spriteBatch);

            spriteBatch.Begin();
            DrawManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

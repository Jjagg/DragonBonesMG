using System;
using System.Diagnostics;
using DragonBonesMG;
using DragonBonesMG.Core;
using DragonBonesMG.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DragonBoneTest {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {

        private TextureAtlas _dragonAtlas;
        private DbArmature _dragonArmature;

        private TextureAtlas _demonAtlas;
        private DbArmature _demonArmature;

        private TextureAtlas _meshAtlas;
        private DbArmature _meshArmature;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _demonAtlas = TextureAtlas.FromJson("Content/DemonTexture.json");
            _demonAtlas.LoadContent(Content);
            _demonArmature = DragonBones.FromJson("Content/Demon.json", _demonAtlas, GraphicsDevice).Armature;

            _dragonAtlas = TextureAtlas.FromJson("Content/texture.json");
            _dragonAtlas.LoadContent(Content);
            _dragonArmature = DragonBones.FromJson("Content/Dragon.json", _dragonAtlas, GraphicsDevice).Armature;

            //_dragonArmature.PlaySound += (s, e) => Debug.WriteLine("Got sound event: " + e.Name);
            //_dragonArmature.DoAction += (s, e) => Debug.WriteLine("Got action event: " + e.Name);
            _dragonArmature.AnimationEvent +=
                delegate(object s, DbAnimationEventArgs e) {
                    Debug.WriteLine("Got animation event: " + e.Name + "; " + switched);
                    switched = !switched;
                };

            _meshAtlas = TextureAtlas.FromJson("Content/Culling.json");
            _meshAtlas.LoadContent(Content);
            _meshArmature = DragonBones.FromJson("Content/Meshes.json", _meshAtlas, GraphicsDevice).Armature;
        }

        private bool switched;

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
        }

        private KeyboardState _prevKeyboard = Keyboard.GetState();

        private float _timeScale = 1.0f;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();
            if (keyboard.IsKeyDown(Keys.OemComma) && !_prevKeyboard.IsKeyDown(Keys.OemComma))
                _timeScale = Math.Max(_timeScale - 0.1f, -1.5f);
            if (keyboard.IsKeyDown(Keys.OemPeriod) && !_prevKeyboard.IsKeyDown(Keys.OemPeriod))
                _timeScale = Math.Min(_timeScale + 0.1f, 1.5f);

            _prevKeyboard = keyboard;

            _demonArmature.TimeScale = _timeScale;
            _demonArmature.Update(gameTime.ElapsedGameTime);

            _dragonArmature.TimeScale = _timeScale;
            _dragonArmature.Update(gameTime.ElapsedGameTime);

            _meshArmature.TimeScale = _timeScale;
            _meshArmature.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _dragonArmature.Draw(spriteBatch, new Vector2(120f, 170f), 0f, new Vector2(-0.3f, 0.3f));
            _demonArmature.Draw(spriteBatch, new Vector2(400f, 270f), 0f, new Vector2(-0.5f, 0.5f));
            _meshArmature.Draw(spriteBatch, new Vector2(100f, 350f), 0f, new Vector2(-1f, 1f));
        }
    }
}
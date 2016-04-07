using System;
using DragonBonesMG;
using DragonBonesMG.Display;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DragonBoneTest {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {

        private TextureAtlas atlas;
        private DbArmature Armature { get; set; }

        private RenderTarget2D _buffer;

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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _buffer = new RenderTarget2D(GraphicsDevice, 1000, 2000);

            atlas = TextureAtlas.FromJson("texture.json");
            atlas.LoadContent(Content);

            Armature = DragonBones.ArmatureFromJson("Dragon_Test.json", atlas);
            Armature.GotoAndStop("jump", 0);
            Armature.SetTimeScale(0.1);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Armature.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            // TODO special spritebatch wrapper to which we can push and pop transformation matrices!
            GraphicsDevice.SetRenderTarget(_buffer);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Armature.Draw(spriteBatch, Matrix.CreateTranslation(400, 500, 0) *
                                       Matrix.CreateScale(0.4f, 0.4f, 1f));

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(_buffer, new Vector2(250, 100));

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
using System;
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

            atlas = TextureAtlas.FromJson("Content/Culling.json");
            atlas.LoadContent(Content);

            Armature = DragonBones.ArmatureFromJson("Content/Meshes.json", atlas);
            Armature.GotoAndPlay("morph");
            Armature.SetTimeScale(0.3);

            //Armature = DragonBones.ArmatureFromJson("Content/SolveCulling.json", atlas);
            //Armature.GotoAndPlay("tweening");
            //Armature.SetTimeScale(0.3);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
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
            GraphicsDevice.Clear(Color.Black);
            var texture = atlas.Get("Culling").RenderToTexture(spriteBatch);

            Armature.Draw(spriteBatch, Matrix.CreateTranslation(300, 200, 0), new Color(0, 0.5f, 1));

            spriteBatch.Begin(transformMatrix:
                Matrix.CreateRotationZ(MathHelper.PiOver4) * Matrix.CreateTranslation(100, 100, 0));

            atlas.Get("Culling").Draw(spriteBatch);
            spriteBatch.Draw(texture, new Vector2(100));

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
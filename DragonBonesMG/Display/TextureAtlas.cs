using System.Collections.Generic;
using System.IO;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Display {
    public class TextureAtlas : ITextureSupplier {

        public readonly string ImagePath;
        private readonly Dictionary<string, Rectangle> _textures;
        private Texture2D _texture;

        internal TextureAtlas(TextureAtlasData data) {
            ImagePath = data.ImagePath;
            _textures = new Dictionary<string, Rectangle>();
            foreach (var sub in data.SubTextures)
                _textures.Add(sub.Name, new Rectangle(sub.X, sub.Y, sub.Width, sub.Height));
        }

        public void LoadContent(ContentManager content) {
            // ContentManager doesn't like extensions
            var name = Path.GetFileNameWithoutExtension(ImagePath);
            _texture = content.Load<Texture2D>(name);
        }

        /// <summary>
        /// Get a drawable that will draw the given texture when drawn.
        /// Returns null if the given texture does not exist.
        /// </summary>
        public IDrawableDb Get(string textureName) {
            if (!_textures.ContainsKey(textureName))
                return null;
            return new TexturePart(_texture, _textures[textureName]);
        }

        /// <summary>
        /// Parse the json file at the given path and return the resulting TextureAtlas.
        /// Don't forget to load the texture using <see cref="LoadContent"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TextureAtlas FromJson(string path) {
            return new TextureAtlas(TextureAtlasData.FromJson(path));
        }
    }
}
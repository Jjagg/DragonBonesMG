using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Display {
    public class TexturePart : IDrawable {

        private readonly Texture2D _texture;
        private readonly Rectangle _bounds;

        public TexturePart(Texture2D texture, Rectangle bounds) {
            _texture = texture;
            _bounds = bounds;
        }

        /// <summary>
        /// Assumes SpriteBatch.Begin() has been called.
        /// </summary>
        public void Draw(SpriteBatch s) {
            // Use center of the texture as the origin
            s.Draw(_texture, -_bounds.Size.ToVector2() / 2f, _bounds, Color.White);
        }
    }
}
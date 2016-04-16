using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Display {
    public class DbImage : DbDisplay {

        private readonly IDrawableDb _texture;

        public DbImage(string textureName, ITextureSupplier texturer)
            : base(textureName) {
            _texture = texturer.Get(textureName);
        }

        public DbImage(IDrawableDb texture, string name = "$default")
            : base(name) {
            _texture = texture;
        }

        public override void Draw(SpriteBatch s, Matrix transform, Color parentColor) {
            // check for negative scaling to allow flipped textures
            // not doing this will make textures that are flipped once (either X or Y axis, but not both)
            // not be drawn because of culling
            // having to do this explicitly makes me sad :( better solutions welcome
            // this could be done at initialization passing the spriteeffect to this function
            var scale = transform.Scale;
            var effect = SpriteEffects.None;
            if (scale.X < 0 && scale.Y > 0) {
                effect = SpriteEffects.FlipHorizontally;
                transform = Matrix.CreateScale(-1, 1, 1) * transform;
            } else if (scale.Y < 0 && scale.X > 0) {
                effect = SpriteEffects.FlipVertically;
                transform = Matrix.CreateScale(1, -1, 1) * transform;
            }
            s.Begin(transformMatrix: transform);
            _texture.Draw(s, parentColor, effect);
            s.End();
        }
    }
}
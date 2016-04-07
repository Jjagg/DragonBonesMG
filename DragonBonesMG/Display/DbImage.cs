using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Display {
    public class DbImage {

        private DbTransform _transform;
        private readonly string _textureName;

        private ITextureSupplier _texturer;

        public DbImage(DisplayData data, ITextureSupplier texturer) {
            _transform = new DbTransform(data.Transform);
            _textureName = data.FileName;
            _texturer = texturer;
        }

        public void Draw(SpriteBatch s, Matrix transform) {
            s.Begin(transformMatrix: _transform.Matrix * transform);
            _texturer.Get(_textureName).Draw(s);
            s.End();
        }
    }
}
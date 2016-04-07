using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG {
    /// <summary>
    /// Encapsulates a transformation matrix so we can make sure angular interpolation is done
    /// correctly (with a quaternion rather than just linearly).
    /// </summary>
    public class DbTransform {

        public DbTransform(TransformData data) :
            this(data.X, data.Y, data.SkX, data.SkY) {
        }

        public DbTransform(float x, float y, float skX, float skY, float scX = 1f, float scY = 1f) {
            Matrix = Matrix.CreateScale(scX, scY, 1) *
                     Matrix.CreateRotationZ(MathHelper.ToRadians(skX)) *
                     Matrix.CreateTranslation(x, y, 0);
        }

        public Matrix Matrix { get; private set; }
    }
}
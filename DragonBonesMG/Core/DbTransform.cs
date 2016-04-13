using System.Diagnostics.Contracts;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Core {
    /// <summary>
    /// Encapsulates a transformation matrix so we can make sure angular interpolation is done
    /// correctly (with a quaternion rather than just linearly).
    /// </summary>
    public struct DbTransform {

        internal DbTransform(TransformData data) :
            this(data.X, data.Y, MathHelper.ToRadians(data.SkX), data.scX, data.scY) {
        }

        public DbTransform(float x, float y, float sk, float scX = 1f, float scY = 1f) :
            this(new Vector2(x, y), sk, new Vector2(scX, scY)) {
        }

        public DbTransform(Vector2 t, float r, Vector2 s)
            : this(t, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, r), s) {
        }

        public DbTransform(Vector2 t, Quaternion r, Vector2 s) {
            Translation = t;
            Rotation = r;
            Scale = s;
        }

        [Pure]
        public Matrix GetMatrix() {
            return Matrix.CreateScale(new Vector3(Scale, 0f)) *
                   Matrix.CreateFromQuaternion(Rotation) *
                   Matrix.CreateTranslation(new Vector3(Translation, 0f));
        }

        public Vector2 Translation { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector2 Scale { get; private set; }

        public static DbTransform Identity =>
            new DbTransform(Vector2.Zero, 0f, Vector2.One);

        public static DbTransform Combine(DbTransform t1, DbTransform t2) {
            return new DbTransform(
                t1.Translation + t2.Translation,
                t1.Rotation * t2.Rotation,
                t1.Scale * t2.Scale);
        }

        public static DbTransform Interpolate(DbTransform t1, DbTransform t2, float weight) {
            return new DbTransform(
                Vector2.Lerp(t1.Translation, t2.Translation, weight),
                Quaternion.Slerp(t1.Rotation, t2.Rotation, weight),
                Vector2.Lerp(t1.Scale, t2.Scale, weight));
        }
    }
}
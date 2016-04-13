using Microsoft.Xna.Framework;

namespace DragonBonesMG.JsonData {
    internal class TransformData {
        public float X;
        public float Y;
        public float SkX;
        public float SkY;
        public float scX = 1;
        public float scY = 1;

        public Matrix ToMatrix() {
            return Matrix.CreateScale(scX, scY, 1) *
                   Matrix.CreateRotationZ(SkX) *
                   Matrix.CreateTranslation(X, Y, 0);
        }
    }
}
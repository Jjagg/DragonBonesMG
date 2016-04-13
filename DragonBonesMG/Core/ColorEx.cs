using Microsoft.Xna.Framework;

namespace DragonBonesMG.Core {
    internal static class ColorEx {
        public static Color Multiply(Color parentColor, Color colorTransform) {
            var val = new Color(parentColor.ToVector4() * colorTransform.ToVector4());
            return val;
        }
    }
}
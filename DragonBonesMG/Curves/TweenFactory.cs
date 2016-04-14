namespace DragonBonesMG.Curves {
    public static class TweenFactory {
        public static ITweenCurve FromArray(float[] values) {
            if (values == null || values.Length < 4)
                return new LinearCurve();
            return new CubicBezier(values[0], values[1], values[2], values[3]);
        }
    }
}
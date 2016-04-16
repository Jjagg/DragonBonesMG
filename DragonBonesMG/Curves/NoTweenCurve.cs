namespace DragonBonesMG.Curves {
    public class NoTweenCurve : ITweenCurve {
        public float GetValue(float time) {
            return 0;
        }
    }
}
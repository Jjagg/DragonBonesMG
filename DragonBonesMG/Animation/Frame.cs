using DragonBonesMG.Curves;

namespace DragonBonesMG.Animation {
    public abstract class Frame {
        public int StartFrame { get; protected set; }
        public ITweenCurve TweenCurve;
    }
}
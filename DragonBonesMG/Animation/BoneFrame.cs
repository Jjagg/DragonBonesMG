using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    public struct BoneFrame {
        public int StartFrame;
        // from dragonbones.objects.AnimationData
        // frame tweenEase, [-1, 0):ease in, 0:line easing, (0, 1]:ease out, (1, 2]:ease in out
        public int? TweenEasing;
        public DbTransform Transform;
        public CurveData TweenCurve;

        // TODO check out Xna.Framework.Curve
        public BoneFrame(int startFrame, int? tweenEasing, DbTransform transform, CurveData curve) {
            StartFrame = startFrame;
            TweenEasing = tweenEasing;
            Transform = transform;
            TweenCurve = curve;
        }

    }
}
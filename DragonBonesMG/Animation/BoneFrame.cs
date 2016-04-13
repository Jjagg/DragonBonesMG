using DragonBonesMG.Core;
using DragonBonesMG.Curves;
using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    public class BoneFrame {
        public readonly int StartFrame;
        public DbTransform Transform;
        public ITweenCurve TweenCurve;

        // TODO check out Xna.Framework.Curve
        internal BoneFrame(int startFrame, BoneFrameData f) {
            StartFrame = startFrame;
            Transform = new DbTransform(f.Transform);
            var tw = f.TweenCurve;

            if (tw == null || tw.Length < 4)
                TweenCurve = new LinearCurve();
            else
                TweenCurve = new CubicBezier(tw[0], tw[1], tw[2], tw[3]);
        }

    }
}
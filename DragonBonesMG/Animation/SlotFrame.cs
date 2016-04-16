using DragonBonesMG.Curves;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    public class SlotFrame : Frame {

        public readonly int DisplayIndex;
        public readonly int? ZOrder;
        public readonly Color Color;

        internal SlotFrame(int startFrame, SlotFrameData f) {
            StartFrame = startFrame;
            DisplayIndex = f.DisplayIndex;
            ZOrder = f.Z;
            Color = f.Color;
            TweenCurve = f.TweenEasing == null ? new NoTweenCurve() : TweenFactory.FromArray(f.TweenCurve);
        }

    }
}
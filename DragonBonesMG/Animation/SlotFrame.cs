using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    internal class SlotFrame {
        public readonly int StartFrame;
        public int? TweenEasing;

        public readonly int DisplayIndex;
        public readonly int ZOrder;
        public readonly Color Color;

        public SlotFrame(int startFrame, SlotFrameData f) {
            StartFrame = startFrame;
            TweenEasing = f.TweenEasing;
            DisplayIndex = f.DisplayIndex;
            ZOrder = f.Z;
            Color = f.Color;
        }

    }
}
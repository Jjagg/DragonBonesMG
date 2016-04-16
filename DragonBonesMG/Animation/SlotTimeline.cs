using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    public class SlotTimeline : SingleTimeline {

        private readonly SlotFrame[] _frames;
        public SlotState State { get; private set; }

        // ReSharper disable once CoVariantArrayConversion
        protected override Frame[] Frames => _frames;

        public SlotTimeline(SlotFrame[] frames) {
            _frames = frames;
        }

        protected override void UpdateState() {
            var prev = _frames[FrameIndex];
            var next = _frames[(FrameIndex + 1) % _frames.Length];
            var color = Color.Lerp(prev.Color, next.Color, Weight);
            State = new SlotState(prev.DisplayIndex, prev.ZOrder, color);
        }
    }
}
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    internal class SlotTimeLine {
        private int _frameIndex;

        private readonly SlotFrame[] _frames;
        public SlotState State { get; private set; }

        public SlotTimeLine(SlotFrame[] frames) {
            _frames = frames;
        }

        public void Update(float frameTime) {
            if (_frames.Length <= 1) return;
            if (frameTime < _frames[_frameIndex].StartFrame)
                _frameIndex = 0;
            while (_frameIndex + 1 < _frames.Length &&
                   _frames[_frameIndex + 1].StartFrame <= frameTime)
                _frameIndex++;

            var prev = _frames[_frameIndex];
            var next = _frames[(_frameIndex + 1) % _frames.Length];

            // TODO map weight with curve
            var weight = (frameTime - prev.StartFrame) /
                         (next.StartFrame - prev.StartFrame);

            var color = Color.Lerp(prev.Color, next.Color, weight);
            State = new SlotState(prev.DisplayIndex, prev.ZOrder, color);
        }
    }
}
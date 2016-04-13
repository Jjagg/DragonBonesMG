using DragonBonesMG.Core;

namespace DragonBonesMG.Animation {
    /// <summary>
    /// A BoneTimeline holds the transform keyframes for a single bone.
    /// <see cref="Tween"/> holds the current transform of the bone this is the timeline of.
    /// </summary>
    public class BoneTimeline {

        // index of the first keyframe before the current time in the animation
        private int _frameIndex;

        private readonly BoneFrame[] _frames;
        public DbTransform Tween { get; private set; }

        public BoneTimeline(BoneFrame[] frames) {
            _frameIndex = 0;
            _frames = frames;
            Tween = frames.Length == 0 ? DbTransform.Identity : frames[0].Transform;
        }

        public void Update(float frameTime) {
            if (_frames.Length == 1) return;
            if (frameTime < _frames[_frameIndex].StartFrame)
                _frameIndex = 0;
            while (_frameIndex + 1 < _frames.Length &&
                   _frames[_frameIndex + 1].StartFrame <= frameTime)
                _frameIndex++;

            var prev = _frames[_frameIndex];
            var next = _frames[(_frameIndex + 1) % _frames.Length];

            var weight = prev.TweenCurve.GetValue(
                (frameTime - prev.StartFrame) /
                (next.StartFrame - prev.StartFrame));
            Tween = DbTransform.Interpolate(
                prev.Transform,
                next.Transform,
                weight);
        }

    }
}
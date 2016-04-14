using DragonBonesMG.Core;

namespace DragonBonesMG.Animation {
    /// <summary>
    /// A BoneTimeline holds the transform keyframes for a single bone.
    /// <see cref="Tween"/> holds the current transform of the bone this is the timeline of.
    /// </summary>
    public class BoneTimeline : SingleTimeline {


        private readonly BoneFrame[] _frames;
        public DbTransform Tween { get; private set; }

        public BoneTimeline(BoneFrame[] frames) {
            _frames = frames;
            Tween = frames.Length == 0 ? DbTransform.Identity : frames[0].Transform;
        }

        // ReSharper disable once CoVariantArrayConversion
        protected override Frame[] Frames => _frames;

        protected override void UpdateState() {
            var prev = _frames[FrameIndex];
            var next = _frames[(FrameIndex + 1) % _frames.Length];
            Tween = DbTransform.Interpolate(
                prev.Transform,
                next.Transform,
                Weight);
        }

    }
}
namespace DragonBonesMG.Animation {
    /// <summary>
    /// Represents a timeline for a single object. (Mesh, Bone or Slot)
    /// </summary>
    public abstract class SingleTimeline {
        protected int FrameIndex;
        protected float Weight;

        /// <summary>
        /// Note that array is covariant, but should not be. The runtime type of this arrays contents is not Frame.
        /// </summary>
        protected abstract Frame[] Frames { get; }

        public SingleTimeline() {
            FrameIndex = 0;
            Weight = 0;
        }

        public void Update(float frameTime) {
            if (Frames.Length <= 1) return;
            UpdateFrame(frameTime);
            UpdateState();
        }

        private void UpdateFrame(float frameTime) {
            if (frameTime < Frames[FrameIndex].StartFrame)
                FrameIndex = 0;
            while (FrameIndex + 1 < Frames.Length &&
                   Frames[FrameIndex + 1].StartFrame <= frameTime)
                FrameIndex++;

            var prev = Frames[FrameIndex];
            var next = Frames[(FrameIndex + 1) % Frames.Length];

            Weight = prev.TweenCurve.GetValue(
                (frameTime - prev.StartFrame) /
                (next.StartFrame - prev.StartFrame));
        }

        protected abstract void UpdateState();
    }
}
namespace DragonBonesMG.Animation {
    /// <summary>
    /// A specific moment within an <see cref="DbAnimation"/>. 
    /// This data class holds all transforms for that moment in the animation.
    /// </summary>
    internal class DbAnimationState {

        public readonly TransformTimelineState TransformState;
        public readonly SlotTimelineState SlotState;
        public readonly FFDTimelineState FFDState;

        public DbAnimationState(TransformTimelineState transformState,
            SlotTimelineState slotState, FFDTimelineState ffdState) {
            TransformState = transformState;
            SlotState = slotState;
            FFDState = ffdState;
        }
    }
}
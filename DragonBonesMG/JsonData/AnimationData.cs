using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class AnimationData {
        public string Name;
        public int Duration;
        public int PlayTimes;

        [JsonProperty(PropertyName = "frame")]
        public EventFrameData[] EventFrames;

        [JsonProperty(PropertyName = "bone")]
        public BoneTimelineData[] BoneTimelines;

        [JsonProperty(PropertyName = "slot")]
        public SlotTimelineData[] SlotTimelines;

        [JsonProperty(PropertyName = "ffd")]
        public MeshTimelineData[] MeshTimelines;
    }
}
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class AnimationData {
        public string Name;
        public int Duration;
        public int PlayTimes;

        [JsonProperty(PropertyName = "bone")]
        public TransformTimelineData[] BoneTimelines;

        [JsonProperty(PropertyName = "slot")]
        public SlotTimelineData[] SlotTimelines;

        [JsonProperty(PropertyName = "ffd")]
        public FFDTimelineData[] FfdTimelines;
    }
}
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class AnimationData {
        // TODO: when no debugging is needed anymore, make all *Data internal
        public string Name;
        public int Duration;
        public int PlayTimes;

        [JsonProperty(PropertyName = "bone")]
        public TransformTimelineData[] TransformTimelines;

        [JsonProperty(PropertyName = "slot")]
        public SlotTimeLineData[] SlotTimelines;

        [JsonProperty(PropertyName = "ffd")]
        public FFDTimelineData[] FfdTimelines;
    }
}
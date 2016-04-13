using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class FFDTimelineData {
        // TODO figure this out

        public string Name;

        public int Scale;
        public int Offset;

        [JsonProperty(PropertyName = "slot")]
        public string SlotName;

        [JsonProperty(PropertyName = "skin")]
        public string SkinName;

        [JsonProperty(PropertyName = "frame")]
        public FFDFrameData[] FFDFrames;

        [JsonProperty(PropertyName = "bone")]
        public TransformTimelineData[] TransformFrames;

    }
}
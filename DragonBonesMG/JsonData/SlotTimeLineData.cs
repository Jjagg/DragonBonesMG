using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class SlotTimelineData {
        [JsonProperty(PropertyName = "name")]
        public string SlotName;

        [JsonProperty(PropertyName = "frame")]
        public SlotFrameData[] SlotFrames;
    }
}
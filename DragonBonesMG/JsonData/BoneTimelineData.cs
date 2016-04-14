using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class BoneTimelineData {

        [JsonProperty(PropertyName = "name")]
        public string BoneName;

        [JsonProperty(PropertyName = "frame")]
        public BoneFrameData[] BoneFrames;
    }
}
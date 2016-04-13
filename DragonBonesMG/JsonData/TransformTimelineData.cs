using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class TransformTimelineData {

        [JsonProperty(PropertyName = "name")]
        public string BoneName;

        [JsonProperty(PropertyName = "frame")]
        public BoneFrameData[] BoneFrames;
    }
}
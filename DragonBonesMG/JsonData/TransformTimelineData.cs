using DragonBonesMG.Animation;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class TransformTimelineData {

        [JsonProperty(PropertyName = "name")]
        public string BoneName;

        [JsonProperty(PropertyName = "frame")]
        public BoneFrameData[] BoneFrames;
    }
}
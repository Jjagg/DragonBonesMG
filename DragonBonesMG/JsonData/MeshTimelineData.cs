using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class MeshTimelineData {

        public string Name;

        public int Scale;
        public int Offset;

        [JsonProperty(PropertyName = "slot")]
        public string SlotName;

        [JsonProperty(PropertyName = "skin")]
        public string SkinName; // TODO what's this?

        [JsonProperty(PropertyName = "frame")]
        public MeshFrameData[] MeshFrames;

    }
}
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class SlotFillData {
        [JsonProperty(PropertyName = "name")]
        public string SlotName;

        [JsonProperty(PropertyName = "display")]
        public DisplayData[] Displays;
    }
}
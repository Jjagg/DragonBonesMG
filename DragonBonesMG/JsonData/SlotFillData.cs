using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class SlotFillData {
        [JsonProperty(PropertyName = "name")]
        public string SlotName;

        [JsonProperty(PropertyName = "display")]
        public DisplayData[] Displays;
    }
}
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class SkinData {
        public string Name;

        [JsonProperty(PropertyName = "slot")]
        public SlotFillData[] SlotFills;
    }
}
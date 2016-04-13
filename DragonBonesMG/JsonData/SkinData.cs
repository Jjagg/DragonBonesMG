using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class SkinData {
        public string Name;

        [JsonProperty(PropertyName = "slot")]
        public SlotFillData[] SlotFills;
    }
}
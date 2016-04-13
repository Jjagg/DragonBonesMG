using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class IKData {
        public string Name;
        public string Target;
        public int Weight;

        [JsonProperty(PropertyName = "bone")]
        public string BoneName;

        public int Chain;
        public bool BendPositive;
    }
}
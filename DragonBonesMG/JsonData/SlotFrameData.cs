using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class SlotFrameData {
        public int Duration;
        public int DisplayIndex;
        public int? TweenEasing;
        public int? Z;
        public ColorData Color;

        [JsonProperty(PropertyName = "curve")]
        public float[] TweenCurve;
    }
}
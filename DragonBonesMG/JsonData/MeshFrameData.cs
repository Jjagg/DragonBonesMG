using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class MeshFrameData {
        public int Duration;
        public int Offset;
        public float[] Vertices;
        public int? TweenEasing;

        [JsonProperty(PropertyName = "curve")]
        public float[] TweenCurve;
    }
}
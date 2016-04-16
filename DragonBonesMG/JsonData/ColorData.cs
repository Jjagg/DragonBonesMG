using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class ColorData {

        [JsonProperty(PropertyName = "rM")]
        public float RedMul = 1;

        [JsonProperty(PropertyName = "gM")]
        public float GreenMul = 1;

        [JsonProperty(PropertyName = "bM")]
        public float BlueMul = 1;

        [JsonProperty(PropertyName = "aM")]
        public float AlphaMul = 1;


        [JsonProperty(PropertyName = "rO")]
        public int RedOffset;

        [JsonProperty(PropertyName = "gO")]
        public int GreenOffset;

        [JsonProperty(PropertyName = "bO")]
        public int BlueOffset;

        [JsonProperty(PropertyName = "aO")]
        public int AlphaOffset;

        /// <summary>
        /// Note: premultiplied alpha!
        /// </summary>
        public Color ToColor() {
            return new Color(RedMul, GreenMul, BlueMul) * AlphaMul;
        }

        /// <summary>
        /// Note: premultiplied alpha!
        /// </summary>
        public static implicit operator Color(ColorData data) {
            return data.ToColor();
        }
    }
}
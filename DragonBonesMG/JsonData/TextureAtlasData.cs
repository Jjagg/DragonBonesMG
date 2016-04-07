using System.IO;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class TextureAtlasData {
        public string Name;
        public string ImagePath;

        [JsonProperty(PropertyName = "SubTexture")]
        public SubTextureData[] SubTextures;

        public static TextureAtlasData FromJson(string path) {
            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<TextureAtlasData>(data);
        }
    }
}
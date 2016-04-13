using System.IO;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class DbData {
        public int FrameRate;
        public string Name;
        public string Version;
        public bool IsGlobal;

        [JsonProperty(PropertyName = "armature")]
        public ArmatureData[] Armatures;

        public static DbData FromJson(string path) {
            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<DbData>(data);
        }
    }
}
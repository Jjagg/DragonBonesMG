using System.IO;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class DbData {
        public int FrameRate;
        public string Name;
        public string Version;
        public bool IsGlobal;
        public ArmatureData[] Armature;

        public static DbData FromJson(string path) {
            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<DbData>(data);
        }
    }
}
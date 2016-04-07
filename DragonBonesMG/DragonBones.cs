using DragonBonesMG.Display;
using DragonBonesMG.JsonData;

namespace DragonBonesMG {
    public class DragonBones {

        public readonly string Name;
        public readonly string Version;
        public readonly bool IsGlobal;
        public readonly int FrameRate;
        public readonly DbArmature Armature;

        public DragonBones(ITextureSupplier texturer, DbData data) {
            Name = data.Name;
            Version = data.Version;
            IsGlobal = data.IsGlobal;
            FrameRate = data.FrameRate;
            Armature = new DbArmature(texturer, data.Armature[0]);
        }

        public static DbArmature ArmatureFromJson(string path, ITextureSupplier texturer) {
            var db = new DragonBones(texturer, DbData.FromJson(path));
            var arm = db.Armature;
            return arm;
        }
    }
}
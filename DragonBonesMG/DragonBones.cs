using System.Collections.Generic;
using System.IO;
using System.Linq;
using DragonBonesMG.Core;
using DragonBonesMG.Display;
using DragonBonesMG.JsonData;

namespace DragonBonesMG {
    public class DragonBones {

        public readonly string Name;
        public readonly string Version;
        public readonly bool IsGlobal;
        public readonly int FrameRate;
        public readonly List<DbArmature> Armatures;

        // pretty convenient for loading a file with just one armature, which is often the case.
        public DbArmature Armature => Armatures.FirstOrDefault();


        internal DragonBones(ITextureSupplier texturer, DbData data) {
            Name = data.Name;
            Version = data.Version;
            IsGlobal = data.IsGlobal;
            FrameRate = data.FrameRate;
            Armatures = new List<DbArmature>();
            foreach (var armatureData in data.Armatures) {
                var armature = new DbArmature(armatureData.Name, texturer, this);
                armature.Initialize(armatureData);
                Armatures.Add(armature);
            }
        }

        public DbArmature GetArmature(string name) {
            return Armatures.FirstOrDefault(a => a.Name == name);
        }

        public static DbArmature ArmatureFromJson(string path, ITextureSupplier texturer) {
            if (!File.Exists(path))
                throw new FileNotFoundException("Could not resolve the given path", path);
            var db = new DragonBones(texturer, DbData.FromJson(path));
            var arm = db.Armature;
            return arm;
        }
    }
}
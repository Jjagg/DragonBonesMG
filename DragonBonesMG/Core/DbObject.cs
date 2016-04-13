namespace DragonBonesMG.Core {
    public class DbObject {
        public string Name { get; protected set; }
        protected DbArmature Armature;
        public DbBone Parent { get; set; }

        public DbObject(string name, DbArmature armature, DbBone parent) {
            Name = name;
            Armature = armature;
            Parent = parent;
        }

        public DbObject(string name, DbArmature armature, string parent) :
            this(name, armature, parent == null ? null : armature.GetBone(parent)) {
        }
    }
}
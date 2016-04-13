using System.Collections.Generic;
using DragonBonesMG.Animation;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Core {

    public class DbBone : DbObject {

        public readonly DbTransform Origin;

        /// <summary>
        /// Transform of this bone with tween calculated in.
        /// </summary>
        public Matrix CurrentGlobalTransform { get; private set; }

        public int Length { get; }

        public readonly List<DbBone> Bones;
        public readonly List<DbSlot> Slots;
        private DbTransform _tween;

        internal DbBone(DbArmature armature, BoneData data) : base(data.Name, armature, data.Parent) {
            Origin = new DbTransform(data.Transform);
            _tween = DbTransform.Identity;
            CurrentGlobalTransform = Origin.GetMatrix();
            Length = data.Length;
            Bones = new List<DbBone>();
            Slots = new List<DbSlot>();
        }

        /// <summary>
        /// Add the given bone to this bone as its child.
        /// Also adds the bone to this bones armature for fast lookups.
        /// </summary>
        public void AddBone(DbBone bone) {
            Bones.Add(bone);
            Armature.AddBone(bone);
        }

        public void AddSlot(DbSlot slot) {
            Slots.Add(slot);
            Armature.AddSlot(slot);
        }

        // TODO: remove slots/bones

        internal void UpdateRecursive(TransformTimelineState state) {
            UpdateRecursive(state, Matrix.Identity);
        }

        internal void UpdateRecursive(TransformTimelineState state, Matrix parentTransform) {
            // get the current tween transform
            _tween = state.GetState(Name);

            // and update the current transform
            CurrentGlobalTransform = DbTransform.Combine(Origin, _tween).GetMatrix() *
                                     parentTransform;

            // and all children
            foreach (var child in Bones)
                child.UpdateRecursive(state, CurrentGlobalTransform);
        }

        internal void ResetRecursive() {
            _tween = DbTransform.Identity;
            foreach (var child in Bones)
                child.ResetRecursive();
        }
    }
}
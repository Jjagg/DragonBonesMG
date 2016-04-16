using System.Collections.Generic;
using DragonBonesMG.Animation;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Core {

    /// <summary>
    /// Basic building block of skeletal animations. A bone can have slots that inherit transform from the bone.
    /// A bone's positions is determined by position of its parent in combination with its own positions. 
    /// Bones can be updated with a TransformTimelineState to be animated.
    /// <seealso cref="DbSlot"/>
    /// <seealso cref="TransformTimelineState"/>
    /// </summary>
    public class DbBone : DbObject {

        /// <summary>
        /// Contains the original position of this bone relative to its parent.
        /// </summary>
        public readonly DbTransform Origin;

        /// <summary>
        /// Transform of this bone with tween calculated in.
        /// </summary>
        public Matrix CurrentGlobalTransform { get; private set; }

        /// <summary>
        /// The global position of this bone.
        /// </summary>
        public Vector2 Position => new Vector2(CurrentGlobalTransform.Translation.X, CurrentGlobalTransform.Translation.Y);

        /// <summary>
        /// The global rotation of this bone.
        /// </summary>
        public Quaternion Rotation => CurrentGlobalTransform.Rotation;

        /// <summary>
        /// The global scale of this bone.
        /// </summary>
        public Vector2 Scale => new Vector2(CurrentGlobalTransform.Scale.X, CurrentGlobalTransform.Scale.Y);

        /// <summary>
        /// The length of this bone. This is exported by DragonBonesPro, but not used outside the editor for now.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// All direct children of this bone.
        /// </summary>
        public readonly List<DbBone> Bones;

        /// <summary>
        /// All slots attached to this bone.
        /// </summary>
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

        /// <summary>
        /// Attach a slot to this bone.
        /// </summary>
        public void AddSlot(DbSlot slot) {
            Slots.Add(slot);
            Armature.AddSlot(slot);
        }

        // TODO: remove slots/bones

        internal void UpdateRecursive(TransformTimelineState state) {
            var parentTransform = Parent?.CurrentGlobalTransform ?? Matrix.Identity;
            // get the current tween transform
            _tween = state.GetState(Name);

            // and update the current transform
            CurrentGlobalTransform = DbTransform.Combine(Origin, _tween).GetMatrix() *
                                     parentTransform;

            // and all children
            foreach (var child in Bones)
                child.UpdateRecursive(state);
        }

        internal void ResetRecursive() {
            _tween = DbTransform.Identity;
            var parentTransform = Parent?.CurrentGlobalTransform ?? Matrix.Identity;
            CurrentGlobalTransform = Origin.GetMatrix() * parentTransform;
            foreach (var child in Bones)
                child.ResetRecursive();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DragonBonesMG.Animation;
using DragonBonesMG.Display;
using DragonBonesMG.JsonData;
using DragonBonesMG.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Core {
    // TODO Meshes, nested armature (might work, needs testing), events,
    // TODO inverse kinematics, pivot? (not sure if used in DBPro)
    // TODO general unit testing
    // DONE bone transform, color transform, basic rendering, texture atlas, tweening
    public class DbArmature : DbDisplay, IAnimatable {
        internal int FrameRate { get; private set; }

        public DbBone RootBone => Bones.Any() ? Bones[0] : null;
        // save bones flat for performance lookups
        internal readonly KeyedCollectionImpl<string, DbBone> Bones;
        // save slots flat in armature for performance, traversing the bonetree is not as fast.
        // slots have a reference to their parent bone for getting their position.
        internal readonly KeyedCollectionImpl<string, DbSlot> Slots;
        // cache slots ordered by z-order
        internal IEnumerable<DbSlot> SortedSlots;
        internal bool SlotsChanged;

        public readonly KeyedCollectionImpl<string, DbAnimation> Animations;
        private DbAnimation _currentAnimation;

        public readonly List<DbIkConstraint> IkConstraints;

        private Dictionary<string, string> DefaultActions;

        #region Initialization

        internal DbArmature(string name, ITextureSupplier texturer, DragonBones creator)
            : base(name) {
            _creator = creator;
            SetTextureSupplier(texturer);
            Bones = new KeyedCollectionImpl<string, DbBone>(b => b.Name);
            Slots = new KeyedCollectionImpl<string, DbSlot>(s => s.Name);
            Animations = new KeyedCollectionImpl<string, DbAnimation>(a => a.Name);
            IkConstraints = new List<DbIkConstraint>();
        }

        internal void Initialize(ArmatureData data) {
            FrameRate = data.FrameRate;

            if (data.Bones.Any()) {
                Bones.Add(new DbBone(this, data.Bones[0]));
                for (var i = 1; i < data.Bones.Length; i++) {
                    var bone = data.Bones[i];
                    var parentBone = Bones[bone.Parent];
                    parentBone.AddBone(new DbBone(this, bone));
                }
            }

            foreach (var slot in data.Slots) {
                var parentBone = Bones[slot.Parent];
                parentBone.AddSlot(new DbSlot(this, slot));
            }

            foreach (var fill in data.Skins[0].SlotFills) {
                var slot = Slots[fill.SlotName];
                foreach (var display in fill.Displays)
                    slot.AddDisplay(display);
            }
            SortSlots();

            foreach (var animation in data.Animations)
                Animations.Add(new DbAnimation(this, animation));

            foreach (var ik in data.InverseKinematics)
                IkConstraints.Add(new DbIkConstraint(ik)); // TODO

            ResetBones();
            // load DefaultActions into Dictionary
            DefaultActions = data.DefaultActions[0];
            if (DefaultActions.ContainsKey("gotoAndPlay") && DefaultActions["gotoAndPlay"] != "")
                _currentAnimation = Animations[DefaultActions["gotoAndPlay"]];

        }

        #endregion

        #region Bones

        public void ResetBones() {
            RootBone?.ResetRecursive();
        }

        /// <summary>
        /// Get the bone with the given name or null if it doesn't exist.
        /// </summary>
        public DbBone GetBone(string name) {
            DbBone bone;
            Bones.TryGet(name, out bone);
            return bone;
        }

        /// <summary>
        /// Add references to a bone and all its children bones/slots to this armatures 
        /// collections of bones and slots for quick lookups.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        internal void AddBone(DbBone bone) {
            IEnumerable<DbBone> bones = new[] {bone};
            while (bones.Any()) {
                Bones.AddRange(bones);
                var slots = bones.SelectMany(b => b.Slots);
                Slots.AddRange(slots);
                bones = bones.SelectMany(b => b.Bones);
            }
        }

        #endregion

        #region Slots

        /// <summary>
        /// Get the slot with the given name or null if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DbSlot GetSlot(string name) {
            DbSlot slot;
            Slots.TryGet(name, out slot);
            return slot;
        }

        /// <summary>
        /// Add a slot to this armature.
        /// </summary>
        /// <param name="slot"></param>
        public void AddSlot(DbSlot slot) {
            Slots.Add(slot);
        }

        /// <summary>
        /// Sort slots by zorder.
        /// </summary>
        private void SortSlots() {
            SortedSlots = Slots.OrderBy(s => s.ZOrder);
            SlotsChanged = false;
        }

        #endregion

        #region Update and Draw

        public void Update(TimeSpan elapsed) {
            if (IsAnimating()) {
                _currentAnimation.Update(elapsed);
                var animationState = _currentAnimation.GetCurrentState();
                // update bones
                RootBone?.UpdateRecursive(animationState.TransformState);
                // update slots
                foreach (var slot in Slots)
                    slot.Update(animationState.DisplayState);
            }
            if (SlotsChanged)
                SortSlots();
        }

        public void Draw(SpriteBatch s) {
            Draw(s, Matrix.Identity);
        }

        public void Draw(SpriteBatch s, Matrix transform) {
            Draw(s, transform, Color.White);
        }

        public override void Draw(SpriteBatch s, Matrix transform, Color colorTransform) {
            foreach (var slot in SortedSlots)
                slot.Draw(s, transform, colorTransform);
        }

        #endregion

        #region IAnimatable

        public double TimeScale { get; private set; } = 1;

        public void PlayAnimation(bool loop) {
            _currentAnimation?.Play(loop);
        }

        public void StopAnimation() {
            _currentAnimation?.Pause();
        }

        private void GotoAnimation(string animation) {
            if (!Animations.Contains(animation)) throw new ArgumentException(nameof(animation));
            _currentAnimation = Animations[animation];
            _currentAnimation.Reset();
        }

        public void GotoAndPlay(string animation, bool loop = true) {
            GotoAnimation(animation);
            PlayAnimation(loop);
        }

        public void GotoAndPlay(string animation, float time, bool loop = true) {
            GotoAnimation(animation);
            _currentAnimation?.PassTime(time);
            PlayAnimation(loop);
        }

        public void GotoAndStop(string animation, float time) {
            GotoAnimation(animation);
            _currentAnimation?.PassTime(time);
            if (_currentAnimation != null)
                RootBone?.UpdateRecursive(_currentAnimation.GetCurrentState().TransformState,
                    Matrix.Identity);
        }

        public void SetTimeScale(double value) {
            if (value < 0) throw new ArgumentException(nameof(value));
            TimeScale = value;
        }

        public bool IsAnimating() {
            return _currentAnimation != null && _currentAnimation.IsPlaying;
        }

        public bool IsDoneAnimating() {
            return _currentAnimation == null || _currentAnimation.IsComplete;
        }

        #endregion

        #region Texture Supplier

        public ITextureSupplier Texturer { get; private set; }

        internal void SetTextureSupplier(ITextureSupplier texturer) {
            Texturer = texturer;
        }

        #endregion

        #region Creator

        private readonly DragonBones _creator;

        public DragonBones GetCreator() {
            return _creator;
        }

        #endregion
    }
}
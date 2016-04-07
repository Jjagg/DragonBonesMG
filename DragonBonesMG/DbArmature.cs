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

namespace DragonBonesMG {
    // TODO Meshes, tweening, color transform, nested armature, sound events,
    // TODO inverse kinematics, pivot?
    // TODO split DbDisplayObjects
    // DONE bone updating, basic rendering, texture atlas
    public class DbArmature : IAnimatable {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public int FrameRate { get; private set; }

        public DbBone RootBone => Bones.Any() ? Bones[0] : null;
        // save bones flat for performance lookups
        internal readonly KeyedCollectionImpl<string, DbBone> Bones;
        // save slots flat in armature for performance, traversing the bonetree is not as fast.
        // slots have a reference to their parent bone for getting their position.
        internal readonly KeyedCollectionImpl<string, DbSlot> Slots;
        // cache slots ordered by z-order
        internal readonly List<DbSlot> SortedSlots;
        internal bool SlotsChanged;

        public readonly KeyedCollectionImpl<string, DbAnimation> Animations;
        private DbAnimation _currentAnimation;

        public readonly List<DbIkConstraint> IkConstraints;

        private readonly Dictionary<string, string> DefaultActions;

        #region Initialization

        public DbArmature(ITextureSupplier texturer, ArmatureData data) {
            SetTextureSupplier(texturer);
            Bones = new KeyedCollectionImpl<string, DbBone>(b => b.Name);
            Slots = new KeyedCollectionImpl<string, DbSlot>(s => s.Name);
            SortedSlots = new List<DbSlot>();
            Animations = new KeyedCollectionImpl<string, DbAnimation>(a => a.Name);
            IkConstraints = new List<DbIkConstraint>();
            Initialize(data);
            // load DefaultActions into Dictionary
            DefaultActions = data.DefaultActions[0];
            if (DefaultActions.ContainsKey("gotoAndPlay"))
                _currentAnimation = Animations[DefaultActions["gotoAndPlay"]];
            ResetBones();
        }

        private void Initialize(ArmatureData data) {
            Name = data.Name;
            Type = data.Type;
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
            SortedSlots.AddRange(Slots);
            UpdateSlots();

            foreach (var fill in data.Skins[0].SlotFills) {
                var slot = Slots[fill.SlotName];
                foreach (var display in fill.Displays)
                    slot.AddDisplay(display);
            }

            foreach (var animation in data.Animations)
                Animations.Add(new DbAnimation(this, animation));

            foreach (var ik in data.InverseKinematics)
                IkConstraints.Add(new DbIkConstraint(ik)); // TODO
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
        /// Add a bone and all its children bones/slots to this armature.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        internal void AddBone(DbBone bone) {
            IEnumerable<DbBone> bones = new[] {bone};
            IEnumerable<DbSlot> slots = new DbSlot[] {};
            while (bones.Any()) {
                Bones.AddRange(bones);
                slots = bones.SelectMany(b => b.Slots);
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
        private void UpdateSlots() {
            SortedSlots.Sort((s1, s2) => s1.ZOrder - s2.ZOrder);
            SlotsChanged = false;
        }

        #endregion

        #region Update and Draw

        public void Update(TimeSpan elapsed) {
            _currentAnimation.Update(elapsed);
            var animationState = _currentAnimation.GetCurrentState();
            if (IsAnimating())
                RootBone?.UpdateRecursive(animationState.TransformState, Matrix.Identity);
            if (SlotsChanged)
                UpdateSlots();
        }

        public void Draw(SpriteBatch s, Matrix transform) {
            // TODO fix half width offset ?
            foreach (var slot in Slots)
                slot.Draw(s, transform);
        }

        #endregion

        #region IAnimatable

        public double TimeScale { get; private set; } = 1;

        public void PlayAnimation() {
            _currentAnimation?.Play();
        }

        public void StopAnimation() {
            _currentAnimation?.Pause();
        }

        private void GotoAnimation(string animation, int playTimes) {
            if (!Animations.Contains(animation)) return;
            _currentAnimation = Animations[animation];
            _currentAnimation.PlayTimes = playTimes;
            _currentAnimation.Reset();
        }

        public void GotoAndPlay(string animation, int playTimes = -1) {
            GotoAnimation(animation, playTimes);
            PlayAnimation();
        }

        public void GotoAndPlay(string animation, float time, int playTimes = -1) {
            GotoAnimation(animation, playTimes);
            _currentAnimation?.PassTime(time);
            PlayAnimation();
        }

        public void GotoAndStop(string animation, float time, int playTimes = -1) {
            GotoAnimation(animation, playTimes);
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
    }
}
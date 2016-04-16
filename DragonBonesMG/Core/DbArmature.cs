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

    public delegate void SoundEventHandler(object sender, DbAnimationEventArgs e);

    public delegate void ActionEventHandler(object sender, DbAnimationEventArgs e);

    public delegate void AnimationEventHandler(object sender, DbAnimationEventArgs e);

    /// <summary>
    /// This is the main DragonBones entity. Armatures have a collection of bones and slots that determine
    /// what an armature looks like and how it is transformed. It also has a collection of animations that
    /// can be played through the <see cref="IAnimatable"/> interface. When using DragonBones in a game,
    /// use an entities armature like you would use its sprite or animationplayer with other animating techniques.
    /// <seealso cref="DbBone"/>
    /// <seealso cref="DbSlot"/>
    /// <seealso cref="DbAnimation"/>
    /// </summary>
    public class DbArmature : DbDisplay, IAnimatable {

        /// <summary>The framerate set in DragonBonesPro editor. Used to determine expected playback speed.</summary>
        public int FrameRate { get; private set; }

        /// <summary>The root bone in this armatures bone hierarchy.</summary>
        public DbBone RootBone => Bones.Any() ? Bones[0] : null;

        // save bones flat for performance lookups
        internal readonly KeyedCollectionImpl<string, DbBone> Bones;

        // save slots flat in armature for performance
        // slots have a reference to their parent bone for getting their position.
        internal readonly KeyedCollectionImpl<string, DbSlot> Slots;

        // cache slots ordered by z-order, resorted on update if SlotsChanged == true
        internal IEnumerable<DbSlot> SortedSlots;
        internal bool SlotsChanged;

        /// <summary>Collection of all animations for this armature.</summary>
        public readonly KeyedCollectionImpl<string, DbAnimation> Animations;

        private DbAnimation _currentAnimation;

        /// <summary>The name of the active animation of this armature.</summary>
        public string CurrentAnimation => _currentAnimation.Name;

        /// <summary>
        /// A list of inverse kinematics constraints on this armature. (Not implemented yet) TODO
        /// </summary>
        public readonly List<DbIkConstraint> IkConstraints;

        private Dictionary<string, string> DefaultActions;

        /// <summary>
        /// Get the position of this armature as a Vector2.
        /// </summary>
        public Vector2 Position => RootBone?.Position ?? Vector2.Zero;

        /// <summary>
        /// Get the rotation of this armature. TODO get only the rotation around Z
        /// </summary>
        public Quaternion Rotation => RootBone?.Rotation ?? Quaternion.Identity;

        /// <summary>
        /// Get the scale of this armature.
        /// </summary>
        public Vector2 Scale => RootBone?.Scale ?? Vector2.One;

        #region Initialization

        internal DbArmature(string name, ITextureSupplier texturer, GraphicsDevice graphics, DragonBones creator)
            : base(name) {
            Creator = creator;
            Texturer = texturer;
            GraphicsDevice = graphics;

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

            foreach (var animation in data.Animations)
                Animations.Add(new DbAnimation(this, animation));

            foreach (var ik in data.InverseKinematics)
                IkConstraints.Add(new DbIkConstraint(ik)); // TODO

            SortSlots();
            ResetBones();
            // load DefaultActions into Dictionary
            DefaultActions = data.DefaultActions[0];
            if (DefaultActions.ContainsKey("gotoAndPlay") && Animations.Contains(DefaultActions["gotoAndPlay"]))
                GotoAndPlay(DefaultActions["gotoAndPlay"]);

        }

        #endregion

        #region Bones

        /// <summary>
        /// Reset the bones of this armature to their original positions.
        /// </summary>
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

        /// <summary>
        /// Update this armature. Elapsed time will be multiplied by <see cref="TimeScale"/>,
        /// <see cref="CurrentAnimation"/> will be updated and all bones and slots will be 
        /// updated to reflect the changes in the animation.
        /// <seealso cref="DbAnimation.Update"/>
        /// </summary>
        /// <param name="elapsed">The time elapsed since the last call to update.</param>
        public void Update(TimeSpan elapsed) {
            if (IsAnimating()) {
                _currentAnimation.Update(elapsed);
                var animationState = _currentAnimation.GetCurrentState();
                // update bones
                RootBone?.UpdateRecursive(animationState.TransformState);
                // update slots
                foreach (var slot in Slots)
                    slot.Update(animationState.DisplayState, animationState.FFDState);
            }
            if (SlotsChanged)
                SortSlots();
        }

        /// <summary>
        /// Draw this armature with the given spritebatch and optionally position, rotation (in radians) and scale.
        /// </summary>
        /// <param name="s">A spritebatch</param>
        /// <param name="position">Position to draw at, Vector2.Zero when not passed.</param>
        /// <param name="rotation">Rotation of the armature in radians</param>
        /// <param name="scale">Scale of the armature along X and Y axis, Vector2.One when not passed</param>
        public void Draw(SpriteBatch s, Vector2? position = null, float rotation = 0f, Vector2? scale = null,
            Color? color = null) {
            var p = position ?? Vector2.Zero;
            var sc = scale ?? Vector2.One;
            var c = color ?? Color.White;
            Draw(s,
                Matrix.CreateScale(sc.X, sc.Y, 1f) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(p.X, p.Y, 0f),
                c);
        }

        /// <summary>
        /// Draw this armature using the given spritebatch and transforming it with the given matrix and color.
        /// </summary>
        /// <param name="s">A spritebatch</param>
        /// <param name="transform">The transformation matrix to apply</param>
        /// <param name="colorTransform">The color transformation to apply</param>
        public override void Draw(SpriteBatch s, Matrix transform, Color colorTransform) {
            foreach (var slot in SortedSlots)
                slot.Draw(s, transform, colorTransform);
        }

        #endregion

        #region IAnimatable

        /// <summary>
        /// Time multiplier for animation playback. Can also be negative for reverse playback.
        /// </summary>
        public double TimeScale { get; set; } = 1;

        /// <summary>
        /// Play the current animation if one is set.
        /// </summary>
        /// <param name="loop">If false the animation will stop after one full play, otherwise it will loop</param>
        public void PlayAnimation(bool loop) {
            _currentAnimation?.Play(loop);
        }

        /// <summary>
        /// Pause the current animation if it is set.
        /// </summary>
        public void PauseAnimation() {
            _currentAnimation?.Pause();
        }

        private void GotoAnimation(string animation) {
            if (!Animations.Contains(animation)) throw new ArgumentException(nameof(animation));
            _currentAnimation = Animations[animation];
            _currentAnimation.Reset();
        }

        /// <summary>
        /// Set the current animation to the given one and play it.
        /// </summary>
        /// <param name="animation">The name of the animation to play</param>
        /// <param name="loop">If false the animation will stop after one full play, otherwise it will loop</param>
        public void GotoAndPlay(string animation, bool loop = true) {
            GotoAnimation(animation);
            PlayAnimation(loop);
        }

        /// <summary>
        /// Set the current animation to the given one and play it.
        /// </summary>
        /// <param name="animation">The name of the animation to play</param>
        /// <param name="time">The time in the animation to start playing at.</param>
        /// <param name="loop">If false the animation will stop after one full play, otherwise it will loop</param>
        public void GotoAndPlay(string animation, float time, bool loop = true) {
            GotoAnimation(animation);
            _currentAnimation?.SetTime(time);
            PlayAnimation(loop);
        }

        /// <summary>
        /// Set the current animation to the given one and pause it at the given time.
        /// </summary>
        /// <param name="animation">The name of the animation to go to.</param>
        /// <param name="time">The time in the animation to set.</param>
        public void GotoAndStop(string animation, float time) {
            GotoAnimation(animation);
            _currentAnimation?.SetTime(time);
            if (_currentAnimation != null)
                RootBone?.UpdateRecursive(_currentAnimation.GetCurrentState().TransformState);
        }

        /// <summary>True if an animation is loaded and it is playing, false otherwise</summary>
        public bool IsAnimating() {
            return _currentAnimation != null && _currentAnimation.IsPlaying;
        }

        /// <summary>
        /// True if no animation is loaded or <see cref="DbAnimation.IsComplete"/> is true for the current animation.
        /// </summary>
        public bool IsDoneAnimating() {
            return _currentAnimation == null || _currentAnimation.IsComplete;
        }

        #endregion

        #region Texture Supplier

        /// <summary>
        /// Used to supply textures to all slots in this armature.
        /// </summary>
        public ITextureSupplier Texturer { get; private set; }

        public GraphicsDevice GraphicsDevice { get; private set; }

        #endregion

        #region Creator

        /// <summary>
        /// Get the DragonBones instance that loaded this armature.
        /// </summary>
        public DragonBones Creator { get; }

        #endregion

        #region Events

        public event ActionEventHandler DoAction;

        internal void OnAction(DbAnimationEventArgs e) {
            DoAction?.Invoke(this, e);
        }

        public event SoundEventHandler PlaySound;

        internal void OnSound(DbAnimationEventArgs e) {
            PlaySound?.Invoke(this, e);
        }

        public event AnimationEventHandler AnimationEvent;

        internal void OnAnimationEvent(DbAnimationEventArgs e) {
            AnimationEvent?.Invoke(this, e);
        }

        #endregion
    }
}
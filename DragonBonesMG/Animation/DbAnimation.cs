using System;
using System.Collections.Generic;
using DragonBonesMG.Core;
using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    public class DbAnimation {
        public string Name { get; }
        public int Duration { get; }
        public float DurationTime => Duration / (float) FrameRate;
        // Not sure if this is ever useful in a game; so not using this. Instead I use Loop below.
        public readonly int PlayTimes;
        public bool Loop { get; internal set; }
        internal int FrameRate => _armature.FrameRate;
        public float CurrentFrame { get; private set; }

        // public bool TweenEnabled { get; private set; }
        internal double TimeScale => _armature.TimeScale;

        private DbArmature _armature;

        private SortedSet<int> _keyFrames;
        private readonly TransformTimeline _transformTimeline;
        private readonly DisplayTimeline _displayTimeline;
        private readonly FFDTimeline _ffdTimeline;

        public bool IsPlaying { get; private set; }
        public bool IsComplete { get; private set; }

        // in DragonBones AS, but I'm doing it a bit differently
        //private int _currentFrameIndex;
        //private int _currentFramePosition;
        //private int _currentFrameDuration;

        #region Constructor

        internal DbAnimation(DbArmature armature, AnimationData data) {
            _armature = armature;
            Name = data.Name;
            Duration = data.Duration;
            PlayTimes = data.PlayTimes;
            Loop = PlayTimes != 0;

            // setup timelines
            _transformTimeline = new TransformTimeline(data.BoneTimelines);
            _displayTimeline = new DisplayTimeline(data.SlotTimelines);
            _ffdTimeline = new FFDTimeline(data.MeshTimelines);
        }

        #endregion

        #region Playback

        /// <summary>
        /// Play this animation.
        /// </summary>
        public void Play(bool loop) {
            IsPlaying = true;
            IsComplete = false;
            Loop = loop;
        }

        /// <summary>
        /// Pause this animation.
        /// </summary>
        public void Pause() {
            IsPlaying = false;
        }

        public void Reset() {
            IsPlaying = false;
            CurrentFrame = 0;
            IsComplete = false;
        }

        internal void PassTime(float elapsed) {
            CurrentFrame += elapsed * FrameRate;
            if (CurrentFrame < Duration) return;
            if (Loop) {
                CurrentFrame -= Duration;
            } else {
                IsPlaying = false;
                IsComplete = true;
                CurrentFrame = Duration;
            }
        }

        #endregion

        #region Update

        public void Update(TimeSpan elapsed) {
            if (!IsPlaying) return;
            PassTime((float) (TimeScale * elapsed.TotalSeconds));
            _transformTimeline.Update(CurrentFrame);
            _displayTimeline.Update(CurrentFrame);
            _ffdTimeline.Update(CurrentFrame);
        }

        #endregion

        /// <summary>
        /// Calculate a snapshot of the animation at this instant and return it.
        /// </summary>
        internal DbAnimationState GetCurrentState() {
            return new DbAnimationState(
                _transformTimeline.GetState(),
                _displayTimeline.GetState(),
                _ffdTimeline.GetState());
        }
    }
}
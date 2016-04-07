using System;
using System.Collections.Generic;
using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    public class DbAnimation {
        public string Name { get; }
        public int Duration { get; }
        public float DurationTime => Duration * FrameRate;
        public int PlayTimes { get; set; }
        public int FrameRate => _armature.FrameRate;
        private float CurrentFrame => FrameRate * _time;

        // public bool TweenEnabled { get; private set; }
        internal double TimeScale => _armature.TimeScale;

        private DbArmature _armature;

        private SortedSet<int> _keyFrames;
        private readonly TransformTimeline _transformTimeline;
        private readonly SlotTimeline _slotTimeline;
        private readonly FFDTimeline _ffdTimeline;

        public bool IsPlaying { get; private set; }
        public bool IsComplete { get; private set; }

        // current time in the animation in seconds
        private float _time;

        // in DragonBones AS, but I'm doing it a bit differently
        //private int _currentFrameIndex;
        //private int _currentFramePosition;
        //private int _currentFrameDuration;

        #region Constructor

        public DbAnimation(DbArmature armature, AnimationData data) {
            _armature = armature;
            Name = data.Name;
            Duration = data.Duration;
            PlayTimes = data.PlayTimes;

            // setup timelines
            _transformTimeline = new TransformTimeline(Duration, data.TransformTimelines);
            // TODO 
        }

        #endregion

        #region Playback

        /// <summary>
        /// Play this animation.
        /// </summary>
        public void Play() {
            IsPlaying = true;
            IsComplete = false;
        }

        /// <summary>
        /// Pause this animation.
        /// </summary>
        public void Pause() {
            IsPlaying = false;
        }

        public void Reset() {
            IsPlaying = false;
            _time = 0;
            IsComplete = false;
        }

        public void PassTime(float time) {
            _time += time;
            while (_time > DurationTime) {
                if (PlayTimes == 0) {
                    IsPlaying = false;
                    _time = DurationTime;
                    IsComplete = true;
                    break;
                }
                _time -= DurationTime;
                if (PlayTimes > 0)
                    PlayTimes--;
            }
        }

        #endregion

        #region Update

        public void Update(TimeSpan elapsed) {
            if (!IsPlaying) return;
            PassTime((float) (TimeScale * elapsed.TotalSeconds));
            float frame = CurrentFrame;
            _transformTimeline.Update(frame);
            //_slotTimeline.Update(frame); TODO
            //_ffdTimeline.Update(frame); TODO
        }

        #endregion

        /// <summary>
        /// Calculate a snapshot of the animation at this instant and return it.
        /// </summary>
        internal DbAnimationState GetCurrentState() {
            return new DbAnimationState(
                _transformTimeline.getState(),
                null,
                null);
            //_slotTimeline.getState(),
            //_ffdTimeline.getState());
        }
    }
}
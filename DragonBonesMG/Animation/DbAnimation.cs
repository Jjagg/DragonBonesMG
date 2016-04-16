using System;
using System.Collections.Generic;
using System.Linq;
using DragonBonesMG.Core;
using DragonBonesMG.JsonData;
using DragonBonesMG.Util;

namespace DragonBonesMG.Animation {
    /// <summary>
    /// DragonBones animations consist of 4 timelines (for now). A <see cref="TransformTimeline"/>,
    /// a <see cref="DisplayTimeline"/>, an <see cref="FFDTimeline"/> and an EventTimeline (as an array of <see cref="EventFrames"/>).
    /// An armatures <see cref="IAnimatable"/> methods will delegate to a DbAnimation (if one is active). 
    /// <see cref="Update"/> will update all timelines. 
    /// <see cref="GetCurrentState"/> returns a snapshot of current timeline states (except for events, those are triggered
    /// automatically when a keyframe is passed).
    /// </summary>
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
            EventFrames = new EventFrame[data.EventFrames.Length];
            var startFrame = 0;
            for (var i = 0; i < data.EventFrames.Length; i++) {
                var frame = data.EventFrames[i];
                EventFrames[i] = new EventFrame(startFrame, frame);
                startFrame += frame.Duration;
            }
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
            // check for events
            var lastFrame = CurrentFrame;
            CurrentFrame += elapsed * FrameRate;
            if (elapsed > 0) {
                // positive timescale
                if (CurrentFrame >= Duration) {
                    if (Loop) {
                        CurrentFrame -= Duration;
                    } else {
                        IsPlaying = false;
                        IsComplete = true;
                        CurrentFrame = Duration;
                    }
                }
                TriggerEvents(lastFrame, CurrentFrame);
            } else {
                // for negative timescales
                if (CurrentFrame < 0) {
                    if (Loop) {
                        CurrentFrame += Duration;
                    } else {
                        IsPlaying = false;
                        IsComplete = true;
                        CurrentFrame = 0;
                    }
                }
                TriggerEvents(CurrentFrame, lastFrame);
            }
        }

        /// <summary>
        /// Set the animation time. Used instead of <see cref="PassTime"/> when no events should be triggered.
        /// </summary>
        /// <param name="time"></param>
        internal void SetTime(float time) {
            if (time < 0) throw new ArgumentException(nameof(time) + " can not be negative");
            CurrentFrame = (time * FrameRate) % Duration;
        }

        #endregion

        #region Update

        public void Update(TimeSpan elapsed) {
            if (!IsPlaying || elapsed == TimeSpan.Zero) return;
            PassTime((float) (TimeScale * elapsed.TotalSeconds));
            _transformTimeline.Update(CurrentFrame);
            _displayTimeline.Update(CurrentFrame);
            _ffdTimeline.Update(CurrentFrame);
        }

        #endregion

        #region GetCurrentState

        /// <summary>
        /// Calculate a snapshot of the animation at this instant and return it.
        /// </summary>
        internal DbAnimationState GetCurrentState() {
            return new DbAnimationState(
                _transformTimeline.GetState(),
                _displayTimeline.GetState(),
                _ffdTimeline.GetState());
        }

        #endregion

        #region Events

        public EventFrame[] EventFrames;

        /// <summary>
        /// Trigger events between a start and end frame.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void TriggerEvents(float start, float end) {
            var range = new CyclicFloatRange(start, end);
            foreach (var frame in EventFrames.Where(e => range.Inside(e.StartFrame)))
                TriggerEvent(frame);
        }

        private void TriggerEvent(EventFrame e) {
            if (e.Action != null)
                _armature.OnAction(new DbAnimationEventArgs(e.Action));
            if (e.Event != null)
                _armature.OnAnimationEvent(new DbAnimationEventArgs(e.Event));
            if (e.Sound != null)
                _armature.OnSound(new DbAnimationEventArgs(e.Sound));
        }

        private struct CyclicFloatRange {

            public readonly float Start;
            public readonly float End;

            public CyclicFloatRange(float start, float end) {
                Start = start;
                End = end;
            }

            public bool Inside(float value) {
                if (Start <= End)
                    return Start < value && value <= End;
                return Start < value || value <= End;
            }

        }

        #endregion
    }
}
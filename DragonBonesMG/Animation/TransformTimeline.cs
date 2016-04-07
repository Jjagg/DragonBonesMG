using System.Collections.Generic;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    public class TransformTimeline {

        private int _duration;
        private float _currentFrame;

        private Dictionary<string, BoneFrame[]> _keyFrames;
        private Dictionary<string, Matrix> _boneTween;

        /// <summary>
        /// Create a new TransformTimeline given the data for all bone timelines.
        /// </summary>
        /// <param name="duration">Duration of the animation in frames.</param>
        /// <param name="datas"></param>
        public TransformTimeline(int duration, TransformTimelineData[] datas) {
            _keyFrames = new Dictionary<string, BoneFrame[]>();
            _boneTween = new Dictionary<string, Matrix>();
            _duration = duration;
            foreach (var d in datas) {
                var startFrame = 0;
                var frames = new BoneFrame[d.BoneFrames.Length];
                for (var i = 0; i < d.BoneFrames.Length; i++) {
                    var f = d.BoneFrames[i];
                    frames[i] = new BoneFrame(startFrame, f.TweenEasing,
                        new DbTransform(f.Transform), f.TweenCurve);
                    startFrame += f.Duration;
                }
                _keyFrames.Add(d.BoneName, frames);
                _boneTween.Add(d.BoneName, frames[0].Transform.Matrix); // TODO check if not empty
            }
            _currentFrame = 0f;
        }

        public void Update(float currentFrame) {
            _currentFrame = currentFrame;
            // TODO update all bones !! working here
        }

        internal TransformTimelineState getState() {
            return new TransformTimelineState(_boneTween);
        }
    }
}
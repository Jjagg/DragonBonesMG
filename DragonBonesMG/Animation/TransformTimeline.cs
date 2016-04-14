using System.Collections.Generic;
using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    /// <summary>
    /// A TransformTimeline holds a collection of BoneTimelines. />.
    /// </summary>
    public class TransformTimeline {

        private readonly Dictionary<string, BoneTimeline> _boneTimelines;

        /// <summary>
        /// Create a new TransformTimeline given the data for all bone timelines.
        /// </summary>
        /// <param name="datas"></param>
        internal TransformTimeline(BoneTimelineData[] datas) {
            _boneTimelines = new Dictionary<string, BoneTimeline>(datas.Length);
            foreach (var d in datas) {
                var startFrame = 0;
                var frames = new BoneFrame[d.BoneFrames.Length];
                for (var i = 0; i < d.BoneFrames.Length; i++) {
                    var f = d.BoneFrames[i];
                    frames[i] = new BoneFrame(startFrame, f);
                    startFrame += f.Duration;
                }
                _boneTimelines.Add(d.BoneName, new BoneTimeline(frames));
            }
        }

        public void Update(float currentFrame) {
            foreach (var tl in _boneTimelines.Values)
                tl.Update(currentFrame);
        }

        internal TransformTimelineState GetState() {
            return new TransformTimelineState(_boneTimelines);
        }
    }
}
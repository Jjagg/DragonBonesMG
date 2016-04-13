using System.Collections.Generic;
using DragonBonesMG.Core;

namespace DragonBonesMG.Animation {

    internal class TransformTimelineState {
        private readonly Dictionary<string, BoneTimeline> _timelines;

        public TransformTimelineState(Dictionary<string, BoneTimeline> boneTimelines) {
            _timelines = boneTimelines;
        }

        public DbTransform GetState(string boneName) {
            return _timelines.ContainsKey(boneName)
                ? _timelines[boneName].Tween
                : DbTransform.Identity;
        }
    }

}
using System.Collections.Generic;
using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    internal class FFDTimeline {

        private readonly Dictionary<string, MeshTimeline> _meshTimelines;

        internal FFDTimeline(MeshTimelineData[] datas) {
            _meshTimelines = new Dictionary<string, MeshTimeline>(datas.Length);
            foreach (var d in datas) {
                var startFrame = 0;
                var frames = new MeshFrame[d.MeshFrames.Length];
                for (var i = 0; i < d.MeshFrames.Length; i++) {
                    var f = d.MeshFrames[i];
                    frames[i] = new MeshFrame(startFrame, f);
                    startFrame += f.Duration;
                }
                _meshTimelines.Add(d.SlotName, new MeshTimeline(frames));
            }
        }

        public void Update(float currentFrame) {
            foreach (var tl in _meshTimelines.Values)
                tl.Update(currentFrame);
        }

        public FFDTimelineState GetState() {
            return new FFDTimelineState(_meshTimelines);
        }

    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    public class FFDTimelineState {

        private readonly Dictionary<string, MeshTimeline> _meshTimelines;

        public FFDTimelineState(Dictionary<string, MeshTimeline> meshTimelines) {
            _meshTimelines = meshTimelines;
        }

        public MeshTimeline GetVertices(string slotName) {
            return _meshTimelines.ContainsKey(slotName)
                ? _meshTimelines[slotName]
                : null;
        }
    }
}
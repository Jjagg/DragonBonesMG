using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    public class MeshTimeline : SingleTimeline {


        private readonly MeshFrame[] _frames;

        /// <summary>
        /// Offset of the vertices in this timeline
        /// </summary>
        public int Offset;

        public float[] Vertices;

        // ReSharper disable once CoVariantArrayConversion
        protected override Frame[] Frames => _frames;

        public MeshTimeline(MeshFrame[] frames) {
            _frames = frames;
            if (frames.Length > 0)
                Vertices = frames[0].Vertices;
        }

        protected override void UpdateState() {
            var prev = _frames[FrameIndex];
            var next = _frames[(FrameIndex + 1) % _frames.Length];

            if (!prev.Vertices.Any()) {
                Offset = next.Offset;
                Vertices = new float[next.Vertices.Length];
                for (var i = 0; i < Vertices.Length; i++)
                    Vertices[i] = next.Vertices[i] * Weight;
                return;
            }
            if (!next.Vertices.Any()) {
                Offset = prev.Offset;
                Vertices = new float[prev.Vertices.Length];
                var revWeight = 1 - Weight;
                for (var i = 0; i < Vertices.Length; i++)
                    Vertices[i] = prev.Vertices[i] * revWeight;
                return;
            }

            int maxOffset;
            if (prev.Offset < next.Offset) {
                Vertices = new float[prev.Vertices.Length];
                Offset = prev.Offset;
                maxOffset = next.Offset;
                Buffer.BlockCopy(prev.Vertices, 0, Vertices, 0,
                    (next.Offset - prev.Offset) * 4);
            } else {
                Vertices = new float[next.Vertices.Length];
                Offset = next.Offset;
                maxOffset = prev.Offset;
                Buffer.BlockCopy(next.Vertices, 0, Vertices, 0,
                    (prev.Offset - next.Offset) * 4);
            }
            for (int i = maxOffset - Offset; i < Vertices.Length; i++) {
                var prevIndex = i - prev.Offset;
                var nextIndex = i - next.Offset;
                Vertices[i] = MathHelper.Lerp(prev.Vertices[prevIndex],
                    next.Vertices[nextIndex], Weight);
            }
        }

    }
}
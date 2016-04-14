using System;
using DragonBonesMG.Curves;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    public class MeshFrame : Frame {
        public readonly int Offset;
        public readonly float[] Vertices;

        internal MeshFrame(int startFrame, MeshFrameData f) {
            StartFrame = startFrame;
            Offset = f.Offset;
            Vertices = f.Vertices;
            TweenCurve = TweenFactory.FromArray(f.TweenCurve);
        }

    }
}
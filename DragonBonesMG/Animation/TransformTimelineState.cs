using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {

    internal class TransformTimelineState {
        private readonly Dictionary<string, Matrix> _boneTransforms;

        public TransformTimelineState(Dictionary<string, Matrix> boneTransforms) {
            _boneTransforms = boneTransforms;
        }

        public Matrix GetTween(string boneName) {
            Debug.Assert(_boneTransforms.ContainsKey(boneName));
            return _boneTransforms[boneName];
        }
    }

}
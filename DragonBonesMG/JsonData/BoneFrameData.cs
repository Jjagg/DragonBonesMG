using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class BoneFrameData {
        public int Duration;

        // from dragonbones.objects.AnimationData
        // frame tweenEase, [-1, 0):ease in, 0:line easing, (0, 1]:ease out, (1, 2]:ease in out
        public int? TweenEasing;

        public TransformData Transform;

        [JsonProperty(PropertyName = "curve")]
        public float[] TweenCurve;

    }
}
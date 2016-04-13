using System.Collections.Generic;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class ArmatureData {
        public string Name;
        public string Type;
        public int FrameRate;
        public Dictionary<string, string>[] DefaultActions;

        [JsonProperty(PropertyName = "bone")]
        public BoneData[] Bones;

        [JsonProperty(PropertyName = "slot")]
        public SlotData[] Slots;

        [JsonProperty(PropertyName = "animation")]
        public AnimationData[] Animations;

        [JsonProperty(PropertyName = "skin")]
        public SkinData[] Skins;

        [JsonProperty(PropertyName = "ik")]
        public IKData[] InverseKinematics;

    }
}
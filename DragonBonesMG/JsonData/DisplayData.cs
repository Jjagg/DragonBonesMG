using System.Collections.Generic;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    internal class DisplayData {

        [JsonProperty(PropertyName = "name")]
        public string Name;

        public TransformData Transform;
        public Dictionary<string, string>[] DefaultActions;

        // IMAGE, MESH or ARMATURE
        public string Type;

        // MESH ONLY
        public float Width;
        public float Height;
        // not sure what this is?
        public int[] UserEdges;
        // series of x and y position of vertices on the image
        public float[] Vertices;
        public int[] Edges;
        // same as vertices, but mapped to [0, 1]
        public float[] Uvs;
        // indices!
        public short[] Triangles;
    }
}
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class DisplayData {

        [JsonProperty(PropertyName = "name")]
        public string FileName;

        // IMAGE, MESH or ARMATURE
        public string Type;
        public TransformData Transform;
        // MESH
        public float Width;
        public float Height;
        public int[] UserEdges;
        public float[] Vertices;
        public int[] Edges;
        public float[] Uvs;
        public int[] Triangles;
    }
}
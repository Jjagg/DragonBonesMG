using DragonBonesMG.Display;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Mesh {
    public class DbMesh : DbDisplay {

        private readonly IDrawableDb _drawable;
        private Texture2D _texture;

        private float[] _originalVertices;
        private short[] _indices;
        private float[] _uvs;
        private VertexPositionColorTexture[] _vertices;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private BasicEffect _effect;

        private Matrix _cameraMatrix;
        private bool _initialized = false;

        internal DbMesh(DisplayData data, ITextureSupplier texturer)
            : base(data.Name) {
            _drawable = texturer.Get(data.Name);
            _originalVertices = data.Vertices;
            _indices = data.Triangles;
            _uvs = data.Uvs;
            _vertices = new VertexPositionColorTexture[_originalVertices.Length / 2];
            // edges, userEdges?
        }

        // TODO public constructor

        /// <summary>
        /// Update this mesh.
        /// </summary>
        /// <param name="vertices"></param>
        public void Update(Vector2[] vertices) {
            // TODO update the vertices
            if (!_initialized) return;
        }

        /// <summary>
        /// Draw this mesh.
        /// </summary>
        /// <param name="s">A spritebatch.</param>
        /// <param name="transform"> A transformation matrix.</param>
        /// <param name="colorTransform">A color</param>
        public override void Draw(SpriteBatch s, Matrix transform, Color colorTransform) {
            if (!_initialized) Initialize(s);
            _effect = new BasicEffect(s.GraphicsDevice) {
                World = Matrix.Identity,
                View = Matrix.Identity,
                Projection = transform * _cameraMatrix,
                Texture = _texture,
                VertexColorEnabled = true,
                TextureEnabled = true
            };
            //_effect.AmbientLightColor = colorTransform.ToVector3();
            _indexBuffer.SetData(_indices);
            _vertexBuffer.SetData(_vertices);
            s.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            s.GraphicsDevice.Indices = _indexBuffer;

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            s.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (var pass in _effect.CurrentTechnique.Passes) {
                pass.Apply();
                s.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                    _indices.Length / 3);
            }
        }

        public void Initialize(SpriteBatch s) {
            if (_initialized) return;
            var graphicsDevice = s.GraphicsDevice;

            var vp = graphicsDevice.Viewport;
            _cameraMatrix =
                Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, 1);

            _texture = _drawable.RenderToTexture(s);

            // TODO these are all structs, so efficiency can be improved by working with pointers!
            for (int i = 0; i < _originalVertices.Length; i += 2) {
                var v = new VertexPositionColorTexture(
                    new Vector3(_originalVertices[i], _originalVertices[i + 1], 0f),
                    Color.White,
                    new Vector2(_uvs[i], _uvs[i + 1]));
                _vertices[i / 2] = v;
            }


            _indexBuffer = new IndexBuffer(graphicsDevice, typeof (short),
                _indices.Length, BufferUsage.WriteOnly);

            _vertexBuffer = new DynamicVertexBuffer(graphicsDevice,
                typeof (VertexPositionColorTexture),
                _vertices.Length, BufferUsage.WriteOnly);

            _initialized = true;
        }
    }
}
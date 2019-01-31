using System.Collections.Generic;
using ModelConverter.Calculators;
using ModelConverter.Math;

namespace ModelConverter.Model
{
    internal class Model : IModel
    {

        #region Member Variables

        private readonly List<Face> _faces = new List<Face>();
        private readonly List<TextureCoord> _textureCoords = new List<TextureCoord>();
        private readonly List<Vector> _vertexNormals = new List<Vector>();

        private readonly List<Vertex> _vertices = new List<Vertex>();

        #endregion

        #region Public Properties

        public IReadOnlyList<Vertex> Vertices => _vertices;

        public IReadOnlyList<Vector> VertexNormals => _vertexNormals;

        public IReadOnlyList<TextureCoord> TextureCoords => _textureCoords;

        public IReadOnlyList<Face> Faces => _faces;

        #endregion

        #region Public Methods

        public void AddFace(Face f)
        {
            f.Normal = f.Normal ?? FaceNormalCalculator.CalculateNormal(this, f);

            if (f.Normal.Length < 1e-5)
                return; // skipping degenerate face

            if (f.VertexIndices.Length == 3)
            {
                _faces.Add(f);
            }
            else
            {
                var triangles = Triangulator.Triangulate(this, f);
                foreach (var triangle in triangles)
                {
                    _faces.Add(CreateFaceFromIndices(f, triangle[0], triangle[1], triangle[2]));
                }
            }
        }

        public void AddTextureCoord(TextureCoord t) => _textureCoords.Add(t);

        public void AddVertex(Vertex v) => _vertices.Add(v);

        public void AddVertexNormal(Vector v) => _vertexNormals.Add(v);

        #endregion

        #region Private Methods

        private static Face CreateFaceFromIndices(Face f, int i0, int i1, int i2)
        {
            return new Face
            {
                Normal = f.Normal,
                VertexIndices = new[] { f.VertexIndices[i0], f.VertexIndices[i1], f.VertexIndices[i2] },
                NormalIndices = f.NormalIndices.Length > 0 ? new[] { f.NormalIndices[i0], f.NormalIndices[i1], f.NormalIndices[i2] } : new int[0],
                TextureCoordIndices = f.TextureCoordIndices.Length > 0 ? new[] { f.TextureCoordIndices[i0], f.TextureCoordIndices[i1], f.TextureCoordIndices[i2] } : new int[0],
            };
        }

        #endregion

    }

}
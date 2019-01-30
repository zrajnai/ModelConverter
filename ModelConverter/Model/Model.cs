using System.Collections.Generic;
using ModelConverter.Math;

namespace ModelConverter.Model
{
    internal class Model : IModel
    {
        public IReadOnlyList<Vertex> Vertices => _vertices;
        public IReadOnlyList<Vector> VertexNormals => _vertexNormals;
        public IReadOnlyList<TextureCoord> TextureCoords => _textureCoords;
        public IReadOnlyList<Face> Faces => _faces;

        private readonly List<Vertex> _vertices = new List<Vertex>();
        private readonly List<Vector> _vertexNormals = new List<Vector>();
        private readonly List<TextureCoord> _textureCoords = new List<TextureCoord>();
        private readonly List<Face> _faces = new List<Face>();

        public void AddVertex(Vertex v) => _vertices.Add(v);
        public void AddVertexNormal(Vector v) => _vertexNormals.Add(v);
        public void AddTextureCoord(TextureCoord t) => _textureCoords.Add(t);
        public void AddFace(Face f)
        {

            f.Normal = f.Normal ?? CalculateNormal(f.VertexIndices);

            if (f.VertexIndices.Length == 3)
            {
                _faces.Add(f);
            }
            else if (f.VertexIndices.Length == 4)
            {
                _faces.Add(GetFaceFromQuad(f, 0, 1, 2));
                _faces.Add(GetFaceFromQuad(f, 0, 2, 3));
            }
        }

        private static Face GetFaceFromQuad(Face f, int i0, int i1, int i2)
        {
            return new Face
            {
                Normal = f.Normal,
                VertexIndices = new[] { f.VertexIndices[i0], f.VertexIndices[i1], f.VertexIndices[i2] },
                NormalIndices = f.NormalIndices.Length > 0
                    ? new[] { f.NormalIndices[i0], f.NormalIndices[i1], f.NormalIndices[i2] }
                    : new int[0],
                TextureCoordIndices = f.TextureCoordIndices.Length > 0
                    ? new[] { f.TextureCoordIndices[i0], f.TextureCoordIndices[i1], f.TextureCoordIndices[i2] }
                    : new int[0],
            };
        }

        private Vector CalculateNormal(IList<int> vertexIndices)
        {
            // Note : supposes that the first 3 vertices are non convex!
            var v1 = Vertices[vertexIndices[0]];
            var v2 = Vertices[vertexIndices[1]];
            var v3 = Vertices[vertexIndices[2]];

            var v31 = (Vector)v3 - (Vector)v1;
            var v21 = (Vector)v2 - (Vector)v1;

            var n = (v31 % v21).Normalize();

            return new Vector(n.X, n.Y, n.Z);

        }
    }
}

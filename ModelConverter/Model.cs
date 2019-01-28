using System.Collections.Generic;

namespace ModelConverter
{
    public class Model
    {
        public IEnumerable<Vertex> Vertices => _vertices;
        public IEnumerable<VertexNormal> VertexNormals => _vertexNormals;
        public IEnumerable<TextureCoord> TextureCoords => _textureCoords;
        public IEnumerable<Face> Faces => _faces;

        private readonly List<Vertex> _vertices = new List<Vertex>();
        private readonly List<VertexNormal> _vertexNormals = new List<VertexNormal>();
        private readonly List<TextureCoord> _textureCoords = new List<TextureCoord>();
        private readonly List<Face> _faces = new List<Face>();


        public void AddVertex(Vertex v) => _vertices.Add(v);
        public void AddVertexNormal(VertexNormal v) => _vertexNormals.Add(v);
        public void AddTextureCoord(TextureCoord t) => _textureCoords.Add(t);
        public void AddFace(Face f) => _faces.Add(f);
    }
}

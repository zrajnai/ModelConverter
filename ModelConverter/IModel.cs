using System.Collections.Generic;

namespace ModelConverter
{
    public interface IModel {
        IReadOnlyList<Vertex> Vertices { get; }
        IReadOnlyList<Face> Faces { get; }
        IReadOnlyList<Vector> VertexNormals { get; }
        IReadOnlyList<TextureCoord> TextureCoords { get; }
    }
}
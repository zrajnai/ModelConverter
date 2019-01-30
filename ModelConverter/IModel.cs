using System.Collections.Generic;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter
{
    public interface IModel {
        IReadOnlyList<Vertex> Vertices { get; }
        IReadOnlyList<Face> Faces { get; }
        IReadOnlyList<Vector> VertexNormals { get; }
        IReadOnlyList<TextureCoord> TextureCoords { get; }
    }
}
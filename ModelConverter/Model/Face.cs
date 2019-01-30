using ModelConverter.Math;

namespace ModelConverter.Model
{
    public class Face
    {
        public int[] VertexIndices { get; set; }
        public int[] TextureCoordIndices { get; set; }
        public int[] NormalIndices { get; set; }
        public Vector Normal { get; set; }
    }
}
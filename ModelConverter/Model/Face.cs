using ModelConverter.Math;

namespace ModelConverter.Model
{
    public class Face
    {

        #region Public Properties

        public int[] VertexIndices { get; set; }

        public int[] TextureCoordIndices { get; set; }

        public int[] NormalIndices { get; set; }

        public Vector Normal { get; set; }

        #endregion

    }
}
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.Calculators
{
    public static class FaceNormalCalculator
    {

        #region Public Methods

        public static Vector CalculateNormal(IModel model, Face f)
        {
            var normal = new Vector(0, 0, 0);

            for (var idx0 = 0; idx0 < f.VertexIndices.Length; idx0++)
            {
                var idx1 = new CircularIterator(f.VertexIndices.Length, idx0 + 1).Current;
                var idx2 = new CircularIterator(f.VertexIndices.Length, idx1 + 1).Current;

                normal = normal + CalculateNormal(model, f.VertexIndices[idx0], f.VertexIndices[idx1], f.VertexIndices[idx2]);
            }

            return normal.Normalize();
        }

        #endregion

        #region Private Methods

        private static Vector CalculateNormal(IModel model, int i0, int i1, int i2)
        {
            var v1 = (Vector)model.Vertices[i0];
            var v2 = (Vector)model.Vertices[i1];
            var v3 = (Vector)model.Vertices[i2];

            var v31 = (v3 - v1).Normalize();
            var v21 = (v2 - v1).Normalize();

            var n = v31 % v21;

            return new Vector(n.X, n.Y, n.Z);

        }

        #endregion

    }
}
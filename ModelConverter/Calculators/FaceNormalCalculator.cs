using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.Calculators {
    public static class FaceNormalCalculator
    {
        public static Vector CalculateNormal(IModel model, Face f)
        {
            var normal = new Vector(0, 0, 0);

            for (var idx0 = 0; idx0 < f.VertexIndices.Length; idx0++)
            {
                var idx1 = (idx0 + 1 + f.VertexIndices.Length) % f.VertexIndices.Length;
                var idx2 = (idx1 + 1 + f.VertexIndices.Length) % f.VertexIndices.Length;
                normal = normal + CalculateNormal(model, f.VertexIndices[idx0], f.VertexIndices[idx1], f.VertexIndices[idx2]);
            }

            return normal.Normalize();
        }

        private static Vector CalculateNormal(IModel model, int i0, int i1, int i2)
        {
            // Note : supposes that the first 3 vertices are non convex!
            var v1 = model.Vertices[i0];
            var v2 = model.Vertices[i1];
            var v3 = model.Vertices[i2];

            var v31 = (Vector)v3 - (Vector)v1;
            var v21 = (Vector)v2 - (Vector)v1;

            var n = (v31 % v21).Normalize();

            return new Vector(n.X, n.Y, n.Z);

        }
    }
}
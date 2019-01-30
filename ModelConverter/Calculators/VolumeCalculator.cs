using System.Linq;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.Calculators
{
    public static class VolumeCalculator
    {
        public static double Calculate(IModel model)
        {
            return model.Faces.Sum(face => CalculateVolume(model, face));
        }

        private static double CalculateVolume(IModel model, Face face)
        {
            var v0 = model.Vertices[face.VertexIndices[0]];
            var v1 = model.Vertices[face.VertexIndices[1]];
            var v2 = model.Vertices[face.VertexIndices[2]];

            return VolumeOfTetrahedron((Vector)v0, (Vector)v1, (Vector)v2);
        }

        private static double VolumeOfTetrahedron(Vector p1, Vector p2, Vector p3)
        {
            var v321 = p3.X * p2.Y * p1.Z;
            var v231 = p2.X * p3.Y * p1.Z;
            var v312 = p3.X * p1.Y * p2.Z;
            var v132 = p1.X * p3.Y * p2.Z;
            var v213 = p2.X * p1.Y * p3.Z;
            var v123 = p1.X * p2.Y * p3.Z;
            return (-v321 + v231 + v312 - v132 - v213 + v123) / 6.0f;
        }
    }
}
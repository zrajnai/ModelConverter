using System.Linq;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.Calculators
{
    public static class AreaCalculator
    {
        public static double Calculate(IModel model)
        {
            return model.Faces.Sum(face => CalculateArea(model, face));
        }

        private static double CalculateArea(IModel model, Face face)
        {
            var v0 = model.Vertices[face.VertexIndices[0]];
            var v1 = model.Vertices[face.VertexIndices[1]];
            var v2 = model.Vertices[face.VertexIndices[2]];

            var v10 = (Vector)v1 - (Vector)v0;
            var v20 = (Vector)v2 - (Vector)v0;

            var area = (v10 % v20).Length * 0.5d;
            return area;
        }
    }

}
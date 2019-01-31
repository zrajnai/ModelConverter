using System.Linq;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.Calculators
{
    public static class AreaCalculator
    {

        #region Public Methods

        public static double Calculate(IModel model)
        {
            return model.Faces.Sum(face => CalculateArea(model, face));
        }

        #endregion

        #region Private Methods

        private static double CalculateArea(IModel model, Face face)
        {
            var v0 = (Vector)model.Vertices[face.VertexIndices[0]];
            var v1 = (Vector)model.Vertices[face.VertexIndices[1]];
            var v2 = (Vector)model.Vertices[face.VertexIndices[2]];

            var v10 = v1 - v0;
            var v20 = v2 - v0;

            var area = (v10 % v20).Length * 0.5d;
            return area;
        }

        #endregion

    }

}
using ModelConverter.Math;

namespace ModelConverter.Model
{
    public class Vertex
    {

        #region Public Properties

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double W { get; set; }

        #endregion

        #region Public Methods

        public static explicit operator Vertex(Vector v)
        {
            return new Vertex { X = v.X, Y = v.Y, Z = v.Z, W = 1 };
        }

        #endregion

    }
}
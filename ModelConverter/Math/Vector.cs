using ModelConverter.Model;

namespace ModelConverter.Math
{
    public class Vector
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static explicit operator Vector(Vertex v) => new Vector(v.X, v.Y, v.Z);

        public static Vector operator-(Vector v1, Vector v2) => new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

        public static Vector operator+(Vector v1, Vector v2) => new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        
        public static double operator*(Vector v1, Vector v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;

        public static Vector operator %(Vector v1, Vector v2) => new Vector(v1.Y * v2.Z - v1.Z * v2.Y, v2.X * v1.Z - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);

        public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector Normalize()
        {
            var length = Length;
            if (length < 1e-5)
                return new Vector(0,0,0);
            return new Vector(X / Length, Y / length, Z / length);
        }

    }
}
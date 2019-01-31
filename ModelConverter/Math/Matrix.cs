using System;
using ModelConverter.Model;

namespace ModelConverter.Math
{
    public struct Matrix : IEquatable<Matrix>
    {
        private const double _degreesToRadians = System.Math.PI / 180;
        private const double _epsilon = 1e-5;

        public Matrix(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33) : this(m11, m12, m13, m21, m22, m23, m31, m32, m33, 0, 0, 0) { }

        public Matrix(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33, double offsetX, double offsetY, double offsetZ) : this(m11, m12, m13, 0, m21, m22, m23, 0, m31, m32, m33, 0, offsetX, offsetY, offsetZ, 1) { }

        private Matrix(double m11,
                       double m12,
                       double m13,
                       double m14,
                       double m21,
                       double m22,
                       double m23,
                       double m24,
                       double m31,
                       double m32,
                       double m33,
                       double m34,
                       double offsetX,
                       double offsetY,
                       double offsetZ,
                       double m44)
        {
            _m11 = m11;
            _m12 = m12;
            _m13 = m13;
            _m14 = m14;
            _m21 = m21;
            _m22 = m22;
            _m23 = m23;
            _m24 = m24;
            _m31 = m31;
            _m32 = m32;
            _m33 = m33;
            _m34 = m34;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _offsetZ = offsetZ;
            _m44 = m44;
        }

        public static Matrix Identity => new Matrix(1, 0, 0, 0, 1, 0, 0, 0, 1);

        private readonly double _m11;
        private readonly double _m12;
        private readonly double _m13;
        private readonly double _m14;
        private readonly double _m21;
        private readonly double _m22;
        private readonly double _m23;
        private readonly double _m24;
        private readonly double _m31;
        private readonly double _m32;
        private readonly double _m33;
        private readonly double _m34;
        private readonly double _m44;
        private readonly double _offsetX;
        private readonly double _offsetY;
        private readonly double _offsetZ;

        public Vertex Transform(Vertex point)
        {
            return new Vertex
            {
                X = point.X * _m11 + point.Y * _m21 + point.Z * _m31 + _offsetX,
                Y = point.X * _m12 + point.Y * _m22 + point.Z * _m32 + _offsetY,
                Z = point.X * _m13 + point.Y * _m23 + point.Z * _m33 + _offsetZ,
                W = 1
            };
        }

        public Vector Transform(Vector vector)
        {
            return new Vector(
                vector.X * _m11 + vector.Y * _m21 + vector.Z * _m31,
                vector.X * _m12 + vector.Y * _m22 + vector.Z * _m32,
                vector.X * _m13 + vector.Y * _m23 + vector.Z * _m33);
        }

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return matrix1.Equals(matrix2);
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !matrix1.Equals(matrix2);
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(
                matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21 + matrix1._m13 * matrix2._m31,
                matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22 + matrix1._m13 * matrix2._m32,
                matrix1._m11 * matrix2._m13 + matrix1._m12 * matrix2._m23 + matrix1._m13 * matrix2._m33,
                matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21 + matrix1._m23 * matrix2._m31,
                matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22 + matrix1._m23 * matrix2._m32,
                matrix1._m21 * matrix2._m13 + matrix1._m22 * matrix2._m23 + matrix1._m23 * matrix2._m33,
                matrix1._m31 * matrix2._m11 + matrix1._m32 * matrix2._m21 + matrix1._m33 * matrix2._m31,
                matrix1._m31 * matrix2._m12 + matrix1._m32 * matrix2._m22 + matrix1._m33 * matrix2._m32,
                matrix1._m31 * matrix2._m13 + matrix1._m32 * matrix2._m23 + matrix1._m33 * matrix2._m33,
                matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix1._offsetZ * matrix2._m31 + matrix2._offsetX,
                matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix1._offsetZ * matrix2._m32 + matrix2._offsetY,
                matrix1._offsetX * matrix2._m13 + matrix1._offsetY * matrix2._m23 + matrix1._offsetZ * matrix2._m33 + matrix2._offsetZ);
        }

        public static Matrix Scale(double scale) => new Matrix(scale, 0, 0, 0, scale, 0, 0, 0, scale);

        public static Matrix Scale(double scaleX, double scaleY, double scaleZ) => new Matrix(scaleX, 0, 0, 0, scaleY, 0, 0, 0, scaleZ);

        public static Matrix Translate(Vector offset) => new Matrix(1, 0, 0, 0, 1, 0, 0, 0, 1, offset.X, offset.Y, offset.Z);

        public static Matrix Translate(double offsetX, double offsetY, double offsetZ) => new Matrix(1, 0, 0, 0, 1, 0, 0, 0, 1, offsetX, offsetY, offsetZ);

        public static Matrix RotationXDeg(double angle) => RotationXRad(angle * _degreesToRadians);

        public static Matrix RotationXRad(double angle)
        {
            var sin = System.Math.Sin(angle);
            var cos = System.Math.Cos(angle);
            return new Matrix(1, 0, 0, 0, cos, sin, 0, -sin, cos);
        }

        public static Matrix RotationYDeg(double angle) => RotationYRad(angle * _degreesToRadians);

        public static Matrix RotationYRad(double angle)
        {
            var sin = System.Math.Sin(angle);
            var cos = System.Math.Cos(angle);
            return new Matrix(cos, 0, -sin,
                              0, 1, 0,
                              sin, 0, cos);
        }

        public static Matrix RotationZDeg(double angle) => RotationZRad(angle * _degreesToRadians);

        public static Matrix RotationZRad(double angle)
        {
            var sin = System.Math.Sin(angle);
            var cos = System.Math.Cos(angle);
            return new Matrix(cos, sin, 0, -sin, cos, 0, 0, 0, 1);
        }

        public static Matrix RotationDeg(double xAngle, double yAngle, double zAngle) => RotationRad(xAngle * _degreesToRadians, yAngle * _degreesToRadians, zAngle * _degreesToRadians);

        public static Matrix RotationRad(double xAngle, double yAngle, double zAngle)
        {
            return RotationXRad(xAngle) * RotationYRad(yAngle) * RotationZRad(zAngle);
        }

        public static bool Equals(Matrix matrix1, Matrix matrix2)
        {
            return matrix1.Equals(matrix2);
        }

        public bool Equals(Matrix other)
        {
            const double tolerance = _epsilon;
            return
                System.Math.Abs(_m11 - other._m11) < tolerance &&
                System.Math.Abs(_m12 - other._m12) < tolerance &&
                System.Math.Abs(_m13 - other._m13) < tolerance &&
                System.Math.Abs(_m14 - other._m14) < tolerance &&
                System.Math.Abs(_m21 - other._m21) < tolerance &&
                System.Math.Abs(_m22 - other._m22) < tolerance &&
                System.Math.Abs(_m23 - other._m23) < tolerance &&
                System.Math.Abs(_m24 - other._m24) < tolerance &&
                System.Math.Abs(_m31 - other._m31) < tolerance &&
                System.Math.Abs(_m32 - other._m32) < tolerance &&
                System.Math.Abs(_m33 - other._m33) < tolerance &&
                System.Math.Abs(_m34 - other._m34) < tolerance &&
                System.Math.Abs(_offsetX - other._offsetX) < tolerance &&
                System.Math.Abs(_offsetY - other._offsetY) < tolerance &&
                System.Math.Abs(_offsetZ - other._offsetZ) < tolerance &&
                System.Math.Abs(_m44 - other._m44) < tolerance;
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix)
            {
                var matrix = (Matrix)obj;
                return Equals(matrix);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return
                _m11.GetHashCode() ^
                _m12.GetHashCode() ^
                _m13.GetHashCode() ^
                _m14.GetHashCode() ^
                _m21.GetHashCode() ^
                _m22.GetHashCode() ^
                _m23.GetHashCode() ^
                _m24.GetHashCode() ^
                _m31.GetHashCode() ^
                _m32.GetHashCode() ^
                _m33.GetHashCode() ^
                _m34.GetHashCode() ^
                _offsetX.GetHashCode() ^
                _offsetY.GetHashCode() ^
                _offsetZ.GetHashCode() ^
                _m44.GetHashCode();
        }

        public override string ToString()
        {
            return $"{_m11},{_m12},{_m13},{_m14}, {_m21},{_m22},{_m23},{_m24}, {_m31},{_m32},{_m33},{_m34}, {_offsetX},{_offsetY},{_offsetZ},{_m44}";
        }
    }
}
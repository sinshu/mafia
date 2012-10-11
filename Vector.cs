using System;

namespace Mafia
{
    /// <summary>
    /// 2次元のベクトルを表す。
    /// </summary>
    public struct Vector
    {
        public static Vector Zero = new Vector(0, 0);

        public double X;
        public double Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(double a, Vector v)
        {
            return new Vector(a * v.X, a * v.Y);
        }

        public static Vector operator *(Vector v, double a)
        {
            return new Vector(v.X * a, v.Y * a);
        }

        public static explicit operator IntVector(Vector v)
        {
            return new IntVector((int)Math.Round(v.X), (int)Math.Round(v.Y));
        }
    }
}

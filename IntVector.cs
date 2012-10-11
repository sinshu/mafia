using System;
using System.Drawing;

namespace Mafia
{
    /// <summary>
    /// 2次元のベクトルを表す。
    /// </summary>
    public struct IntVector
    {
        public int X;
        public int Y;

        public IntVector(int x, int y)
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

        public static bool operator ==(IntVector a, IntVector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(IntVector a, IntVector b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static IntVector operator +(IntVector a, IntVector b)
        {
            return new IntVector(a.X + b.X, a.Y + b.Y);
        }

        public static IntVector operator -(IntVector a, IntVector b)
        {
            return new IntVector(a.X - b.X, a.Y - b.Y);
        }

        public static IntVector operator *(int a, IntVector v)
        {
            return new IntVector(a * v.X, a * v.Y);
        }

        public static IntVector operator *(IntVector v, int a)
        {
            return new IntVector(v.X * a, v.Y * a);
        }
    }
}

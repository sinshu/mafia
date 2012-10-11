using System;

namespace Mafia
{
    /// <summary>
    /// 格子状に配置されたブロックの一つの位置を指す。
    /// </summary>
    /// <remarks>
    /// 微妙に的外れなネーミングだが気にするな。
    /// </remarks>
    public struct Dim
    {
        public int Row;
        public int Col;

        public Dim(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Dim a, Dim b)
        {
            return a.Row == b.Row && a.Col == b.Col;
        }

        public static bool operator !=(Dim a, Dim b)
        {
            return a.Row != b.Row || a.Col != b.Col;
        }
    }
}

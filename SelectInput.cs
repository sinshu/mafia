using System;

namespace Mafia
{
    /// <summary>
    /// セレクト画面中の操作。
    /// </summary>
    public struct SelectInput
    {
        public static SelectInput Empty = new SelectInput(false, false, false, false, false, false);

        public bool Left;
        public bool Up;
        public bool Right;
        public bool Down;
        public bool Ok;
        public bool GotoTitle;

        public SelectInput(bool left, bool up, bool right, bool down, bool ok, bool gotoTitle)
        {
            Left = left;
            Up = up;
            Right = right;
            Down = down;
            Ok = ok;
            GotoTitle = gotoTitle;
        }
    }
}

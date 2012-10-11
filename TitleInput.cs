using System;

namespace Mafia
{
    /// <summary>
    /// タイトル画面時の操作。
    /// </summary>
    public struct TitleInput
    {
        public static TitleInput Empty = new TitleInput(false, false, false, false);

        public bool Up;
        public bool Down;
        public bool Start;
        public bool Exit;

        public TitleInput(bool up, bool down, bool start, bool exit)
        {
            Up = up;
            Down = down;
            Start = start;
            Exit = exit;
        }
    }
}

using System;

namespace Mafia
{
    /// <summary>
    /// ÉQÅ[ÉÄíÜÇÃëÄçÏÅB
    /// </summary>
    public struct GameInput
    {
        public static GameInput Empty = new GameInput(false, false, false, false, false, false);

        public bool Left;
        public bool Up;
        public bool Right;
        public bool Down;
        public bool GotoSelect;
        public bool GotoTitle;

        public GameInput(bool left, bool up, bool right, bool down, bool gotoSelect, bool gotoTitle)
        {
            Left = left;
            Up = up;
            Right = right;
            Down = down;
            GotoSelect = gotoSelect;
            GotoTitle = gotoTitle;
        }
    }
}

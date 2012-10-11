using System;

namespace Mafia
{
    /// <summary>
    /// マリオ風アクションゲームによく見られる格子状のマップを表す。
    /// </summary>
    public class Map
    {
        public const int NONE = 0;
        public const int SPIKE_LEFT = 6660;
        public const int SPIKE_UP = 6661;
        public const int SPIKE_RIGHT = 6662;
        public const int SPIKE_DOWN = 6663;
        public const int LIFT_RETURN = 10000;
        public const int DOOR_SLIDE = 20000;

        private int numRows;
        private int numCols;
        private int[,] map;

        public Map(int numRows, int numCols)
        {
            this.numRows = numRows;
            this.numCols = numCols;
            map = new int[numRows, numCols];
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    map[row, col] = NONE;
                }
            }
        }

        public int this[int row, int col]
        {
            get
            {
                if (row < 0 || numRows <= row || col < 0 || numCols <= col)
                {
                    return NONE;
                }
                return map[row, col];
            }

            set
            {
                if (row < 0 || numRows <= row || col < 0 || numCols <= col)
                {
                    return;
                }
                map[row, col] = value;
            }
        }

        public int this[Dim d]
        {
            get
            {
                int row = d.Row;
                int col = d.Col;
                if (row < 0 || numRows <= row || col < 0 || numCols <= col)
                {
                    return NONE;
                }
                return map[row, col];
            }

            set
            {
                int row = d.Row;
                int col = d.Col;
                if (row < 0 || numRows <= row || col < 0 || numCols <= col)
                {
                    return;
                }
                map[row, col] = value;
            }
        }

        public bool IsObstacle(Thing thing, int row, int col)
        {
            if (thing is Lift)
            {
                return this[row, col] != NONE;
            }
            else if (thing is Door)
            {
                return this[row, col] != NONE && this[row, col] != DOOR_SLIDE;
            }
            return this[row, col] != NONE && this[row, col] != LIFT_RETURN;
        }

        public bool IsObstacle(Dim d)
        {
            return this[d] != NONE;
        }

        public int Width
        {
            get
            {
                return numCols * Mafia.BLOCK_WIDTH;
            }
        }

        public int Height
        {
            get
            {
                return numRows * Mafia.BLOCK_WIDTH;
            }
        }

        public void Draw(MafiaVideo video, IntVector camera)
        {
            int topRow = camera.Y / Mafia.BLOCK_WIDTH;
            int bottomRow = (camera.Y + Mafia.SCREEN_HEIGHT) / Mafia.BLOCK_WIDTH;
            int leftCol = camera.X / Mafia.BLOCK_WIDTH;
            int rightCol = (camera.X + Mafia.SCREEN_WIDTH) / Mafia.BLOCK_WIDTH;

            int numRows = Mafia.SCREEN_HEIGHT / Mafia.BLOCK_WIDTH;
            int numCols = Mafia.SCREEN_WIDTH / Mafia.BLOCK_WIDTH;

            for (int row = 0; row <= numRows; row++)
            {
                for (int col = 0; col <= numCols; col++)
                {
                    video.Draw(16, 0, 16, 16, 0, 0, col * Mafia.BLOCK_WIDTH - (camera.X / 2) % 16, row * Mafia.BLOCK_WIDTH - (camera.Y / 2) % 16);
                }
            }

            for (int row = topRow; row <= bottomRow; row++)
            {
                for (int col = leftCol; col <= rightCol; col++)
                {
                    IntVector p = new IntVector(col * Mafia.BLOCK_WIDTH - camera.X, row * Mafia.BLOCK_WIDTH - camera.Y);
                    switch (this[row, col])
                    {
                        case NONE:
                            break;
                        case Map.SPIKE_LEFT:
                            video.Draw(48, 0, 16, 16, 0, 0, p.X, p.Y);
                            break;
                        case Map.SPIKE_UP:
                            video.Draw(48, 0, 16, 16, 0, 1, p.X, p.Y);
                            break;
                        case Map.SPIKE_RIGHT:
                            video.Draw(48, 0, 16, 16, 0, 2, p.X, p.Y);
                            break;
                        case Map.SPIKE_DOWN:
                            video.Draw(48, 0, 16, 16, 0, 3, p.X, p.Y);
                            break;
                        case LIFT_RETURN:
                            break;
                        case DOOR_SLIDE:
                            video.Draw(112, 0, 16, 16, 0, 0, p.X, p.Y);
                            break;
                        default:
                            video.Draw(32, 0, 16, 16, 0, 0, p.X, p.Y);
                            break;
                    }
                }
            }
        }
    }
}

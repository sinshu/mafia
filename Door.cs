using System;

namespace Mafia
{
    public class Door : Thing
    {
        private static int WIDTH = 8;
        private static int HEIGHT = 48;

        public bool Open;

        private double closeHeight;
        private double openHeight;

        public override int Width
        {
            get
            {
                return WIDTH;
            }
        }

        public override int Height
        {
            get
            {
                return HEIGHT;
            }
        }

        public override bool IsObstacle
        {
            get
            {
                return true;
            }
        }

        public Door(GameScene game, int row, int col, bool open) : base(game, row, col)
        {
            Position.X = col * Mafia.BLOCK_WIDTH + 4;
            Position.Y = (row - 3) * Mafia.BLOCK_WIDTH;
            Open = open;
            closeHeight = row * Mafia.BLOCK_WIDTH;
            openHeight = closeHeight - 48;
        }

        public override void Initialize()
        {
            for (int i = 0; i < 24; i++)
            {
                Tick(GameInput.Empty);
            }
        }

        public override void Tick(GameInput input)
        {
            if (Open)
            {
                if (openHeight < Position.Y)
                {
                    if (Position.Y - openHeight < 2)
                    {
                        MoveVerticalTo(openHeight);
                    }
                    else
                    {
                        MoveVerticalBy(-2);
                    }
                }
            }
            else
            {
                if (Position.Y < closeHeight)
                {
                    if (closeHeight - Position.Y < 2)
                    {
                        MoveVerticalTo(closeHeight);
                    }
                    else
                    {
                        MoveVerticalBy(2);
                    }
                }
            }
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera)) return;
            IntVector p = (IntVector)Position - camera;
            video.Draw(176, 64, 8, 48, 0, 0, p.X, p.Y);
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
        }

        protected override double MoveDownBy_CannotPush(double d)
        {
            double oldY = Position.Y;
            double newY = oldY + d;
            Position.Y = newY;
            bool blocked = false;
            for (int col = LeftCol; col <= RightCol; col++)
            {
                if (Game.Map.IsObstacle(this, BottomRow, col))
                {
                    blocked = true;
                }
            }
            if (blocked)
            {
                Bottom = BottomRow * Mafia.BLOCK_WIDTH;
                newY = Position.Y;
            }
            Position.Y = oldY;
            Thing collided = null;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || thing.Top < Bottom || !(Left < thing.Right && thing.Left < Right)) continue;
                Position.Y = newY;
                if (thing.Top < Bottom)
                {
                    Bottom = thing.Top;
                    newY = Position.Y;
                    collided = thing;
                }
                Position.Y = oldY;
            }
            Position.Y = newY;
            if (collided != null && collided is Switch)
            {
                CollidBottom(collided);
                collided.CollidedTop(this);
            }
            else if (blocked)
            {
                CollidBottom(null);
            }
            return newY - oldY;
        }
    }
}

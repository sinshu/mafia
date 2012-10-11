using System;

namespace Mafia
{
    /// <summary>
    /// リフト。
    /// </summary>
    public class Lift : Thing
    {
        private static int WIDTH = 32;
        private static int HEIGHT = 16;

        private static int liftCount = 0;

        private int moveCount;

        public Lift(GameScene game, int row, int col, int behavior) : base(game, row, col)
        {
            switch (behavior)
            {
                case 0:
                    Velocity.X = -1;
                    break;
                case 1:
                    Velocity.Y = -1;
                    break;
                case 2:
                    Velocity.X = 1;
                    break;
                case 3:
                    Velocity.Y = 1;
                    break;
                case 4:
                    Velocity.X = -2;
                    break;
                case 5:
                    Velocity.Y = -2;
                    break;
                case 6:
                    Velocity.X = 2;
                    break;
                case 7:
                    Velocity.Y = 2;
                    break;
                case 8:
                    Velocity.X = -4;
                    break;
                case 9:
                    Velocity.X = 4;
                    break;

            }

            moveCount = ((liftCount++) % 16) * 2;
        }

        public override bool IsObstacle
        {
            get
            {
                return true;
            }
        }

        public override bool CanPush
        {
            get
            {
                return true;
            }
        }

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

        public override void CollidLeft(Thing thing)
        {
            if (thing != null)
            {
                if (thing.CanPush)
                {
                    Velocity.X = -Velocity.X;
                }
            }
            else
            {
                Velocity.X = -Velocity.X;
            }
        }

        public override void CollidTop(Thing thing)
        {
            if (thing != null)
            {
                if (thing.CanPush)
                {
                    Velocity.Y = -Velocity.Y;
                }
            }
            else
            {
                Velocity.Y = -Velocity.Y;
            }
        }

        public override void CollidRight(Thing thing)
        {
            if (thing != null)
            {
                if (thing.CanPush)
                {
                    Velocity.X = -Velocity.X;
                }
            }
            else
            {
                Velocity.X = -Velocity.X;
            }
        }

        public override void CollidBottom(Thing thing)
        {
            if (thing != null)
            {
                if (thing.CanPush)
                {
                    Velocity.Y = -Velocity.Y;
                }
            }
            else
            {
                Velocity.Y = -Velocity.Y;
            }
        }

        public override void Tick(GameInput input)
        {
            MoveBy(Velocity);
            moveCount = (moveCount + 1) % 32;
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera)) return;
            IntVector p = (IntVector)Position - camera;
            video.Draw(0, 96, 32, 16, 0, 0, p.X, p.Y);
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            if (!ShouldPlaySound()) return;
            if (moveCount == 0)
            {
                sound.Play(buffers.Lift, this);
            }
        }

        protected override double MoveLeftBy_CanPush(double d)
        {
            double oldX = Position.X;
            double newX = oldX + d;
            Position.X = newX;
            bool blocked = false;
            bool spiked = true;
            for (int row = TopRow; row <= BottomRow; row++)
            {
                if (Game.Map.IsObstacle(this, row, LeftCol))
                {
                    blocked = true;
                    if (Game.Map[row, LeftCol] != Map.SPIKE_RIGHT) spiked = false;
                }
            }
            if (blocked)
            {
                Left = (LeftCol + 1) * Mafia.BLOCK_WIDTH;
                newX = Position.X;
                CollidLeft(null);
                if (spiked) Damaged(null);
            }
            Position.X = oldX;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || Left < thing.Right || !(Top < thing.Bottom && thing.Top < Bottom)) continue;
                Position.X = newX;
                if (Left < thing.Right)
                {
                    if (thing is Door)
                    {
                        Left = thing.Right;
                        newX = Position.X;
                    }
                    else
                    {
                        double thing_oldX = thing.Left;
                        thing.Right = Left;
                        double thing_newX = thing.Left;
                        thing.Left = thing_oldX;
                        thing.MoveHorizontalTo(thing_newX);
                    }
                    CollidLeft(thing);
                    thing.CollidedRight(this);
                }
                Position.X = oldX;
            }
            Position.X = newX;
            return newX - oldX;
        }

        protected override double MoveRightBy_CanPush(double d)
        {
            double oldX = Position.X;
            double newX = oldX + d;
            Position.X = newX;
            bool blocked = false;
            bool spiked = true;
            for (int row = TopRow; row <= BottomRow; row++)
            {
                if (Game.Map.IsObstacle(this, row, RightCol))
                {
                    blocked = true;
                    if (Game.Map[row, RightCol] != Map.SPIKE_LEFT) spiked = false;
                }
            }
            if (blocked)
            {
                Right = RightCol * Mafia.BLOCK_WIDTH;
                newX = Position.X;
                CollidRight(null);
                if (spiked) Damaged(null);
            }
            Position.X = oldX;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || thing.Left < Right || !(Top < thing.Bottom && thing.Top < Bottom)) continue;
                Position.X = newX;
                if (thing.Left < Right)
                {
                    if (thing is Door)
                    {
                        Right = thing.Left;
                        newX = Position.X;
                    }
                    else
                    {
                        double thing_oldX = thing.Left;
                        thing.Left = Right;
                        double thing_newX = thing.Left;
                        thing.Left = thing_oldX;
                        thing.MoveHorizontalTo(thing_newX);
                    }
                    CollidRight(thing);
                    thing.CollidedLeft(this);
                }
                Position.X = oldX;
            }
            Position.X = newX;
            return newX - oldX;
        }
    }
}

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;

namespace Mafia
{
    /// <summary>
    /// 地形との当たり判定を必要とする矩形物体を表す。
    /// </summary>
    /// <remarks>
    /// 最近の物理計算エンジンって神じゃね？
    /// </remarks>
    public abstract class Thing
    {
        public GameScene Game;
        public Vector Position;
        public Vector Velocity;
        public ThingList Carry;
        public bool Removed;

        public Thing(GameScene game, Vector position)
        {
            this.Game = game;
            this.Position = position;
            Velocity = Vector.Zero;
            Removed = false;
            Carry = new ThingList();
        }

        public Thing(GameScene game, int row, int col)
        {
            this.Game = game;
            Position.X = col * Mafia.BLOCK_WIDTH;
            Position.Y = row * Mafia.BLOCK_WIDTH;
            Velocity = Vector.Zero;
            Removed = false;
            Carry = new ThingList();
        }

        public abstract int Width
        {
            get;
        }

        public abstract int Height
        {
            get;
        }

        public virtual bool IsObstacle
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanPush
        {
            get
            {
                return false;
            }
        }

        public void Remove()
        {
            Removed = true;
        }

        public virtual void Initialize()
        {
        }

        public virtual void BeforeTick()
        {
        }

        public abstract void Tick(GameInput input);

        public virtual void AfterTick()
        {
        }

        public abstract void Draw(MafiaVideo video, IntVector camera);

        public abstract void PlaySound(MafiaSound sound, MafiaBufferContainer buffers);

        public virtual void CollidLeft(Thing thing)
        {
            Velocity.X = 0;
        }

        public virtual void CollidTop(Thing thing)
        {
            Velocity.Y = 0;

        }

        public virtual void CollidRight(Thing thing)
        {
            Velocity.X = 0;
        }

        public virtual void CollidBottom(Thing thing)
        {
            Velocity.Y = 0;
        }

        public virtual void CollidedLeft(Thing thing)
        {
        }

        public virtual void CollidedTop(Thing thing)
        {
            if (!thing.CanPush && !Carry.Has(thing))
            {
                Carry.AddThing(thing);
            }
        }

        public virtual void CollidedRight(Thing thing)
        {
        }

        public virtual void CollidedBottom(Thing thing)
        {
        }

        public virtual void Damaged(Thing thing)
        {
        }

        public virtual Vector MoveBy(Vector delta)
        {
            return new Vector(MoveHorizontalBy(delta.X), MoveVerticalBy(delta.Y));
        }

        public virtual double MoveHorizontalBy(double d)
        {
            Carry.RemoveThings(IsNotCarry);
            if (CanPush)
            {
                if (d < 0)
                {
                    double t = MoveLeftBy_CanPush(d);
                    Carry.MoveHorizontalBy(t);
                    return t;
                }
                else if (d > 0)
                {
                    double t = MoveRightBy_CanPush(d);
                    Carry.MoveHorizontalBy(t);
                    return t;
                }
            }
            else
            {
                if (d < 0)
                {
                    double t = MoveLeftBy_CannotPush(d);
                    Carry.MoveHorizontalBy(t);
                    return t;
                }
                else if (d > 0)
                {
                    double t = MoveRightBy_CannotPush(d);
                    Carry.MoveHorizontalBy(t);
                    return t;
                }
            }
            return 0;
        }

        public virtual double MoveVerticalBy(double d)
        {
            Carry.RemoveThings(IsNotCarry);
            if (CanPush)
            {
                if (d < 0)
                {
                    return MoveUpBy_CanPush(d);
                }
                else if (d > 0)
                {
                    double t = MoveDownBy_CanPush(d);
                    Carry.MoveVerticalBy(t);
                    return t;
                }
            }
            else
            {
                if (d < 0)
                {
                    return MoveUpBy_CannotPush(d);
                }
                else if (d > 0)
                {
                    double t = MoveDownBy_CannotPush(d);
                    Carry.MoveVerticalBy(t);
                    return t;
                }
            }
            return 0;
        }


        public virtual void MoveTo(Vector position)
        {
            MoveHorizontalTo(position.X);
            MoveVerticalTo(position.Y);
        }

        public virtual void MoveHorizontalTo(double x)
        {
            if (CanPush)
            {
                if (x < Position.X)
                {
                    double t = MoveLeftBy_CanPush(x - Position.X);
                    Carry.MoveHorizontalBy(t);
                }
                else if (Position.X < x)
                {
                    double t = MoveRightBy_CanPush(x - Position.X);
                    Carry.MoveHorizontalBy(t);
                }
            }
            else
            {
                if (x < Position.X)
                {
                    double t = MoveLeftBy_CannotPush(x - Position.X);
                    Carry.MoveHorizontalBy(t);
                }
                else if (Position.X < x)
                {
                    double t = MoveRightBy_CannotPush(x - Position.X);
                    Carry.MoveHorizontalBy(t);
                }
            }
        }

        public virtual void MoveVerticalTo(double y)
        {
            if (CanPush)
            {
                if (y < Position.Y)
                {
                    MoveUpBy_CanPush(y - Position.Y);
                }
                else if (Position.Y < y)
                {
                    double t = MoveDownBy_CanPush(y - Position.Y);
                    Carry.MoveVerticalBy(t);
                }
            }
            else
            {
                if (y < Position.Y)
                {
                    MoveUpBy_CannotPush(y - Position.Y);
                }
                else if (Position.Y < y)
                {
                    double t = MoveDownBy_CannotPush(y - Position.Y);
                    Carry.MoveVerticalBy(t);
                }
            }
        }

        public double Left
        {
            get
            {
                return Position.X;
            }

            set
            {
                Position.X = value;
            }
        }

        public double Top
        {
            get
            {
                return Position.Y;
            }

            set
            {
                Position.Y = value;
            }
        }

        public double Right
        {
            get
            {
                return Position.X + Width;
            }

            set
            {
                Position.X = value - Width;
            }
        }

        public double Bottom
        {
            get
            {
                return Position.Y + Height;
            }

            set
            {
                Position.Y = value - Height;
            }
        }

        public Vector LeftTop
        {
            get
            {
                return Position;
            }

            set
            {
                Position = value;
            }
        }

        public Vector RightTop
        {
            get
            {
                return new Vector(Position.X + Width, Position.Y);
            }

            set
            {
                Position.X = value.X - Width;
                Position.Y = value.Y;
            }
        }

        public Vector LeftBottom
        {
            get
            {
                return new Vector(Position.X, Position.Y + Height);
            }

            set
            {
                Position.X = value.X;
                Position.Y = value.Y - Height;
            }
        }

        public Vector RightBottom
        {
            get
            {
                return new Vector(Position.X + Width, Position.Y + Height);
            }

            set
            {
                Position.X = value.X - Width;
                Position.Y = value.Y - Height;
            }
        }

        public Vector Center
        {
            get
            {
                return new Vector(Position.X + (double)Width / 2, Position.Y + (double)Height / 2);
            }

            set
            {
                Position = new Vector(value.X - (double)Width / 2, value.Y - (double)Height / 2);
            }
        }

        public Vector CenterOnScreen
        {
            get
            {
                return Center - Game.Camera;
            }
        }

        public int LeftCol
        {
            get
            {
                return (int)Math.Floor(Position.X / Mafia.BLOCK_WIDTH);
            }
        }

        public int TopRow
        {
            get
            {
                return (int)Math.Floor(Position.Y / Mafia.BLOCK_WIDTH);
            }
        }

        public int RightCol
        {
            get
            {
                return (int)Math.Ceiling((Position.X + Width) / Mafia.BLOCK_WIDTH) - 1;
            }
        }

        public int BottomRow
        {
            get
            {
                return (int)Math.Ceiling((Position.Y + Height) / Mafia.BLOCK_WIDTH) - 1;
            }
        }

        public Dim LeftTopDim
        {
            get
            {
                return new Dim(TopRow, LeftCol);
            }
        }

        public Dim RightTopDim
        {
            get
            {
                return new Dim(TopRow, RightCol);
            }
        }

        public Dim LeftBottomDim
        {
            get
            {
                return new Dim(BottomRow, LeftCol);
            }
        }

        public Dim RightBottomDim
        {
            get
            {
                return new Dim(BottomRow, RightCol);
            }
        }

        protected bool Overlapped(Thing thing)
        {
            return Left < thing.Right && thing.Left < Right && Top < thing.Bottom && thing.Top < Bottom;
        }

        protected bool Overlapped(Thing thing, int depth)
        {
            return Left + depth < thing.Right && thing.Left + depth < Right && Top + depth < thing.Bottom && thing.Top + depth < Bottom;
        }

        protected bool ShouldDraw(IntVector camera)
        {
            return Left < camera.X + Mafia.SCREEN_WIDTH && camera.X < Right && Top < camera.Y + Mafia.SCREEN_HEIGHT && camera.Y < Bottom;
        }

        protected bool ShouldDraw(IntVector camera, int left, int right, int top, int bottom)
        {
            return Left - left < camera.X + Mafia.SCREEN_WIDTH && camera.X < Right + right && Top - top < camera.Y + Mafia.SCREEN_HEIGHT && camera.Y < Bottom + bottom;
        }

        protected bool ShouldPlaySound()
        {
            return CenterOnScreen.X < Mafia.SCREEN_WIDTH + Mafia.SOUND_RANGE && -Mafia.SOUND_RANGE < CenterOnScreen.X && CenterOnScreen.Y < Mafia.SCREEN_HEIGHT + Mafia.SOUND_RANGE && -Mafia.SOUND_RANGE < CenterOnScreen.Y;
        }

        protected virtual double MoveLeftBy_CannotPush(double d)
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
            }
            Position.X = oldX;
            Thing collided = null;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || Left < thing.Right || !(Top < thing.Bottom && thing.Top < Bottom)) continue;
                Position.X = newX;
                if (Left < thing.Right)
                {
                    Left = thing.Right;
                    newX = Position.X;
                    collided = thing;
                }
                Position.X = oldX;
            }
            Position.X = newX;
            if (collided != null)
            {
                CollidLeft(collided);
                collided.CollidedRight(this);
            }
            else if (blocked)
            {
                CollidLeft(null);
                if (spiked) Damaged(null);
            }
            return newX - oldX;
        }

        protected virtual double MoveLeftBy_CanPush(double d)
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
                    double thing_oldX = thing.Left;
                    thing.Right = Left;
                    double thing_newX = thing.Left;
                    thing.Left = thing_oldX;
                    thing.MoveHorizontalTo(thing_newX);
                    CollidLeft(thing);
                    thing.CollidedRight(this);
                }
                Position.X = oldX;
            }
            Position.X = newX;
            return newX - oldX;
        }

        protected virtual double MoveUpBy_CannotPush(double d)
        {
            double oldY = Position.Y;
            double newY = oldY + d;
            Position.Y = newY;
            bool blocked = false;
            bool spiked = true;
            for (int col = LeftCol; col <= RightCol; col++)
            {
                if (Game.Map.IsObstacle(this, TopRow, col))
                {
                    blocked = true;
                    if (Game.Map[TopRow, col] != Map.SPIKE_DOWN) spiked = false;
                }
            }
            if (blocked)
            {
                Top = (TopRow + 1) * Mafia.BLOCK_WIDTH;
                newY = Position.Y;
            }
            Position.Y = oldY;
            Thing collided = null;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || Top < thing.Bottom || !(Left < thing.Right && thing.Left < Right)) continue;
                Position.Y = newY;
                if (Top < thing.Bottom)
                {
                    Top = thing.Bottom;
                    newY = Position.Y;
                    collided = thing;
                }
                Position.Y = oldY;
            }
            Position.Y = newY;
            if (collided != null)
            {
                CollidTop(collided);
                collided.CollidedBottom(this);
            }
            else if (blocked)
            {
                CollidTop(null);
                if (spiked) Damaged(null);
            }
            return newY - oldY;
        }

        protected virtual double MoveUpBy_CanPush(double d)
        {
            double oldY = Position.Y;
            double newY = oldY + d;
            Position.Y = newY;
            bool blocked = false;
            bool spiked = true;
            for (int col = LeftCol; col <= RightCol; col++)
            {
                if (Game.Map.IsObstacle(this, TopRow, col))
                {
                    blocked = true;
                    if (Game.Map[TopRow, col] != Map.SPIKE_DOWN) spiked = false;
                }
            }
            if (blocked)
            {
                Top = (TopRow + 1) * Mafia.BLOCK_WIDTH;
                newY = Position.Y;
                CollidTop(null);
                if (spiked) Damaged(null);
            }
            Position.Y = oldY;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || Top < thing.Bottom || !(Left < thing.Right && thing.Left < Right)) continue;
                Position.Y = newY;
                if (Top < thing.Bottom)
                {
                    double thing_oldY = thing.Top;
                    thing.Bottom = Top;
                    double thing_newY = thing.Top;
                    thing.Top = thing_oldY;
                    thing.MoveVerticalTo(thing_newY);
                    CollidTop(thing);
                    thing.CollidedBottom(this);
                }
                Position.Y = oldY;
            }
            Position.Y = newY;
            return newY - oldY;
        }

        protected virtual double MoveRightBy_CannotPush(double d)
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
            }
            Position.X = oldX;
            Thing collided = null;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || thing.Left < Right || !(Top < thing.Bottom && thing.Top < Bottom)) continue;
                Position.X = newX;
                if (thing.Left < Right)
                {
                    Right = thing.Left;
                    newX = Position.X;
                    collided = thing;
                }
                Position.X = oldX;
            }
            Position.X = newX;
            if (collided != null)
            {
                CollidRight(collided);
                collided.CollidedLeft(this);
            }
            else if (blocked)
            {
                CollidRight(null);
                if (spiked) Damaged(null);
            }
            return newX - oldX;
        }

        protected virtual double MoveRightBy_CanPush(double d)
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
                    double thing_oldX = thing.Left;
                    thing.Left = Right;
                    double thing_newX = thing.Left;
                    thing.Left = thing_oldX;
                    thing.MoveHorizontalTo(thing_newX);
                    CollidRight(thing);
                    thing.CollidedLeft(this);
                }
                Position.X = oldX;
            }
            Position.X = newX;
            return newX - oldX;
        }

        protected virtual double MoveDownBy_CannotPush(double d)
        {
            double oldY = Position.Y;
            double newY = oldY + d;
            Position.Y = newY;
            bool blocked = false;
            bool spiked = true;
            for (int col = LeftCol; col <= RightCol; col++)
            {
                if (Game.Map.IsObstacle(this, BottomRow, col))
                {
                    blocked = true;
                    if (Game.Map[BottomRow, col] != Map.SPIKE_UP) spiked = false;
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
            if (collided != null)
            {
                CollidBottom(collided);
                collided.CollidedTop(this);
            }
            else if (blocked)
            {
                CollidBottom(null);
                if (spiked) Damaged(null);
            }
            return newY - oldY;
        }

        protected virtual double MoveDownBy_CanPush(double d)
        {
            double oldY = Position.Y;
            double newY = oldY + d;
            Position.Y = newY;
            bool blocked = false;
            bool spiked = true;
            for (int col = LeftCol; col <= RightCol; col++)
            {
                if (Game.Map.IsObstacle(this, BottomRow, col))
                {
                    blocked = true;
                    if (Game.Map[BottomRow, col] != Map.SPIKE_UP) spiked = false;
                }
            }
            if (blocked)
            {
                Bottom = BottomRow * Mafia.BLOCK_WIDTH;
                newY = Position.Y;
                CollidBottom(null);
                if (spiked) Damaged(null);
            }
            Position.Y = oldY;
            foreach (Thing thing in Game.ThingList)
            {
                if (!thing.IsObstacle || thing.Top < Bottom || !(Left < thing.Right && thing.Left < Right)) continue;
                Position.Y = newY;
                if (thing.Top < Bottom)
                {
                    double thing_oldY = thing.Position.Y;
                    thing.Top = Bottom;
                    double thing_newY = thing.Position.Y;
                    thing.Position.Y = thing_oldY;
                    thing.MoveVerticalTo(thing_newY);
                    CollidBottom(thing);
                    thing.CollidedTop(this);
                }
                Position.Y = oldY;
            }
            Position.Y = newY;
            return newY - oldY;
        }

        protected bool IsNotCarry(Thing thing)
        {
            return !(thing.Bottom == Top && thing.Left < Right && Left < thing.Right && thing.IsObstacle);
        }
    }
}

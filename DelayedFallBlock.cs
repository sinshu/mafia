using System;

namespace Mafia
{
    public class DelayedFallBlock : Thing
    {
        private static int WIDTH = 16;
        private static int HEIGHT = 16;

        private int type;
        private bool falling;
        private int fallCount;
        private bool damaged;

        private bool fallSound;

        public DelayedFallBlock(GameScene game, int row, int col, int type) : base(game, row, col)
        {
            this.type = type;
            falling = false;
            fallCount = 0;
            damaged = false;
            fallSound = false;
        }

        public override bool IsObstacle
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
            base.CollidLeft(thing);
            if (type == Map.SPIKE_LEFT)
            {
                if (thing != null)
                {
                    thing.Damaged(this);
                }
            }
        }

        public override void CollidRight(Thing thing)
        {
            base.CollidedRight(thing);
            if (type == Map.SPIKE_RIGHT)
            {
                if (thing != null)
                {
                    thing.Damaged(this);
                }
            }
        }

        public override void CollidBottom(Thing thing)
        {
            base.CollidBottom(thing);
            if (type == Map.SPIKE_DOWN)
            {
                if (thing != null)
                {
                    thing.Damaged(this);
                }
            }
        }

        public override void CollidedLeft(Thing thing)
        {
            base.CollidedLeft(thing);
            if (type == Map.SPIKE_LEFT)
            {
                thing.Damaged(this);
            }
        }

        public override void CollidedTop(Thing thing)
        {
            base.CollidedTop(thing);
            if (type != Map.SPIKE_DOWN)
            {
                if (!falling)
                {
                    falling = true;
                    fallSound = true;
                }
            }
        }

        public override void CollidedRight(Thing thing)
        {
            base.CollidedRight(thing);
            if (type == Map.SPIKE_RIGHT)
            {
                thing.Damaged(this);
            }
        }

        public override void CollidedBottom(Thing thing)
        {
            base.CollidedBottom(thing);
            if (type == Map.SPIKE_DOWN)
            {
                thing.Damaged(this);
            }
        }

        public override void Damaged(Thing thing)
        {
            if (damaged) return;
            damaged = true;
            for (int i = 0; i < 8; i++)
            {
                Game.AddThingInGame(new Tiun(Game, Position, i, 2, 16, Tiun.MOKU));
            }
            Remove();
        }

        public override void BeforeTick()
        {
            fallSound = false;
        }

        public override void Tick(GameInput input)
        {
            if (falling)
            {
                if (fallCount < (type == Map.SPIKE_DOWN ? 16 : 8))
                {
                    fallCount++;
                }
                else
                {
                    Velocity.Y += 0.25;
                    if (Velocity.Y > 4.0)
                    {
                        Velocity.Y = 4.0;
                    }
                    MoveVerticalBy(Velocity.Y);
                }
            }
            else
            {
                if (type == Map.SPIKE_DOWN)
                {
                    if (Math.Abs(Game.Player.Position.X - Position.X) < 16)
                    {
                        int playerRow = Game.Player.TopRow;
                        int limit = 0;
                        for (int row = BottomRow; row < playerRow; row++)
                        {
                            if (limit < 16)
                            {
                                limit++;
                            }
                            else
                            {
                                return;
                            }
                            if (Game.Map.IsObstacle(this, row, LeftCol))
                            {
                                return;
                            }
                        }
                        falling = true;
                        fallSound = true;
                    }
                }
            }
            
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera)) return;
            IntVector p = (IntVector)Position - camera;
            switch (type)
            {
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
                default:
                    video.Draw(32, 0, 16, 16, 0, 0, p.X, p.Y);
                    break;
            }
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            if (fallSound)
            {
                sound.Play(buffers.Fall, this);
            }
        }
    }
}

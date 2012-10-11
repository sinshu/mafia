using System;

namespace Mafia
{
    public class Box : Thing
    {
        private const int WIDTH = 32;
        private const int HEIGHT = 32;

        public const int IN_AIR = 0;
        public const int ON_GROUND = 1;

        public int LandState;
        public int MoveCount;

        private int prevLandState;
        private bool pushed;

        public Box(GameScene game, int row, int col) : base(game, row, col)
        {
            LandState = ON_GROUND;
            prevLandState = ON_GROUND;
            MoveCount = 0;
            pushed = false;
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

        public override bool IsObstacle
        {
            get
            {
                return true;
            }
        }

        public void PushedByPlayer()
        {
            pushed = true;
        }

        public override void BeforeTick()
        {
            prevLandState = LandState;
            pushed = false;
        }

        public override void Tick(GameInput input)
        {
            Velocity.Y += 0.25;
            if (Velocity.Y > 4)
            {
                Velocity.Y = 4;
            }
            LandState = IN_AIR;
            MoveVerticalBy(Velocity.Y);
        }

        public override void AfterTick()
        {
            if (pushed)
            {
                MoveCount = (MoveCount + 1) % 8;
            }
            else
            {
                MoveCount = 0;
            }
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera)) return;
            IntVector p = (IntVector)Position - camera;
            video.Draw(144, 64, 32, 32, 0, 0, p.X, p.Y);
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            if (!ShouldPlaySound()) return;
            if (MoveCount == 1)
            {
                sound.Play(buffers.BoxMove, this);
            }
            if (LandState == ON_GROUND && prevLandState == IN_AIR)
            {
                sound.Play(buffers.BoxFall, this);
            }
        }

        public override void CollidBottom(Thing thing)
        {
            base.CollidBottom(thing);
            LandState = ON_GROUND;
        }
    }
}

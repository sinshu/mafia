using System;

namespace Mafia
{
    /// <summary>
    /// Ç®ã‡ÅB
    /// </summary>
    public class Spring : Thing
    {
        private static int WIDTH = 16;
        private static int HEIGHT = 16;

        private int animation;

        public Spring(GameScene game, int row, int col) : base(game, row, col)
        {
            animation = 0;
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

        public override void CollidedTop(Thing thing)
        {
            if (thing != null && !thing.CanPush)
            {
                thing.Velocity.Y = -4;
            }
            animation = 9;
        }

        public override void Tick(GameInput input)
        {
        }

        public override void AfterTick()
        {
            if (animation > 0)
            {
                animation--;
            }
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera, 0, 0, 16, 0)) return;
            IntVector p = (IntVector)Position - camera - new IntVector(0, 16);
            video.Draw(0, 64, 16, 32, 0, animation, p.X, p.Y);
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            if (animation == 8)
            {
                sound.Play(buffers.Spring, this);
            }
        }
    }
}

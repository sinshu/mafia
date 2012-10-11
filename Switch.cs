using System;

namespace Mafia
{
    public class Switch : Thing
    {
        private static int WIDTH = 16;
        private static int HEIGHT = 4;

        private bool pressed;

        private bool pressSound;

        public Switch(GameScene game, int row, int col) : base(game, row, col)
        {
            Position.Y = row * Mafia.BLOCK_WIDTH + 12;
            pressed = false;
            pressSound = false;
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
            if (!pressed)
            {
                Game.ToggleDoors();
                pressed = true;
                pressSound = true;
            }
        }


        public override void BeforeTick()
        {
            pressSound = false;
        }
        public override void Tick(GameInput input)
        {
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera, 0, 0, 2, 0)) return;
            IntVector p = (IntVector)Position - camera - new IntVector(0, 12);
            video.Draw(128, 0, 16, 16, 0, pressed ? 1 : 0, p.X, p.Y);
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            if (!ShouldPlaySound()) return;
            if (pressSound)
            {
                sound.Play(buffers.Switch, this);
            }
        }
    }
}

using System;

namespace Mafia
{
    /// <summary>
    /// Ç®ã‡ÅB
    /// </summary>
    public class Coin : Thing
    {
        private static int WIDTH = 16;
        private static int HEIGHT = 16;

        private int animation;

        public Coin(GameScene game, int row, int col) : base(game, row, col)
        {
            animation = 0;
        }

        public override bool IsObstacle
        {
            get
            {
                return false;
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
        }

        public override void Tick(GameInput input)
        {
            animation = (animation + 1) % 16;
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera)) return;
            IntVector p = (IntVector)Position - camera;
            video.Draw(0, 48, 16, 16, 0, animation, p.X, p.Y);
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
        }

        public void KiraKira()
        {
            for (int i = 0; i < 8; i++)
            {
                Game.AddThingInGame(new Tiun(Game, Position, i, 2, 16, Tiun.KIRA));
                Game.AddThingInGame(new Tiun(Game, Position, i, 4, 16, Tiun.KIRA));
            }
        }
    }
}

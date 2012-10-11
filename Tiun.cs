using System;

namespace Mafia
{
    /// <summary>
    /// Ã¨³ÝÃ¨³ÝÃ¨³Ý
    /// </summary>
    public class Tiun : Thing
    {
        public static int TIUN = 1;
        public static int KIRA = 2;
        public static int MOKU = 3;

        private static int WIDTH = 16;
        private static int HEIGHT = 16;

        int direction;
        int speed;
        int life;
        int type;

        public Tiun(GameScene game, Vector position, int direction, int speed, int life, int type) : base(game, position)
        {
            this.direction = direction;
            this.speed = speed;
            this.life = life;
            this.type = type;
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

        public override void Tick(GameInput input)
        {
            Position.X += speed * Math.Cos(Math.PI * direction / 4);
            Position.Y += speed * Math.Sin(Math.PI * direction / 4);
            life--;
            if (life == 0)
            {
                Remove();
            }
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera)) return;
            IntVector p = (IntVector)Position - camera;
            if (type == TIUN)
            {
                video.Draw(0, 112, 16, 16, 0, 7 - (life / 2) % 8, p.X, p.Y);
            }
            else if (type == KIRA)
            {
                video.Draw(0, 112, 16, 16, 1, 7 - (life / 2) % 8, p.X, p.Y);
            }
            else if (type == MOKU)
            {
                video.Draw(0, 112, 16, 16, 2, 7 - (life / 2) % 8, p.X, p.Y);
            }
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
        }
    }
}

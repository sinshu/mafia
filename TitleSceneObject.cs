using System;

namespace Mafia
{
    public class TitleSceneObject
    {
        public const int MAFIA = 1;
        public const int COIN = 2;

        int type;
        int animation;
        Vector position;
        Vector velocity;

        public TitleSceneObject(Random random)
        {
            Reset(random);
        }

        public void Reset(Random random)
        {
            type = random.Next(MAFIA, COIN + 1);
            if (type == MAFIA)
            {
                animation = random.Next(16);
            }
            else if (type == COIN)
            {
                animation = 0;
            }
            position.X = random.NextDouble() * (Mafia.SCREEN_WIDTH - 16);
            position.Y = Mafia.SCREEN_HEIGHT;
            velocity.X = random.NextDouble() * 4 - 2;
            velocity.Y = random.NextDouble() * -4 - 4;
        }

        public void Tick(Random random)
        {
            velocity.Y += 0.125;
            position += velocity;
            if (position.X < -16 || position.X > Mafia.SCREEN_WIDTH || position.Y > Mafia.SCREEN_HEIGHT)
            {
                Reset(random);
            }
            if (type == MAFIA)
            {
            }
            else if (type == COIN)
            {
                animation = (animation + 1) % 16;
            }
        }

        public void Draw(MafiaVideo video)
        {
            IntVector p = (IntVector)position;
            if (type == MAFIA)
            {
                video.Draw(0, 16, 16, 32, 0, animation, p.X, p.Y);
            }
            else if (type == COIN)
            {
                video.Draw(0, 48, 16, 16, 0, animation, p.X, p.Y);
            }
        }
    }
}

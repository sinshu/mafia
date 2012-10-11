using System;
using System.Drawing;

namespace Mafia
{
    /// <summary>
    /// タイトル画面。
    /// </summary>
    public class TitleScene
    {
        public const int NONE = 0;
        public const int START_GAME = 1;
        public const int EXIT = 2;

        private const int NUM_OBJECTS = 8;

        private int timer;
        private double titleWaveWidth;

        private Random random;
        private TitleSceneObject[] objects;

        public TitleScene()
        {
            timer = 0;
            titleWaveWidth = 64;
            random = new Random();
            objects = new TitleSceneObject[NUM_OBJECTS];
            for (int i = 0; i < NUM_OBJECTS; i++)
            {
                objects[i] = new TitleSceneObject(random);
            }

        }

        public int Tick(TitleInput input)
        {
            int result = NONE;
            if (input.Start)
            {
                result = START_GAME;
            }
            timer++;
            if (titleWaveWidth > 0)
            {
                titleWaveWidth -= 0.125;
            }
            else
            {
                titleWaveWidth = 0;
            }
            if (input.Exit)
            {
                result = EXIT;
            }
            for (int i = 0; i < NUM_OBJECTS; i++)
            {
                objects[i].Tick(random);
            }
            return result;
        }

        public void Draw(MafiaVideo video)
        {
            for (int row = -1; row < Mafia.SCREEN_HEIGHT / Mafia.BLOCK_WIDTH; row++)
            {
                for (int col = -1; col < Mafia.SCREEN_WIDTH / Mafia.BLOCK_WIDTH; col++)
                {
                    video.Draw(16, 0, 16, 16, 0, 0, col * Mafia.BLOCK_WIDTH, row * Mafia.BLOCK_WIDTH + ((int)Math.Round(32 * Math.Sin(Math.PI * timer / 180.0)) + 1024) % 16);
                }
            }

            for (int i = 0; i < 128; i++)
            {
                double x = Mafia.SCREEN_WIDTH / 2 - 128 + titleWaveWidth * Math.Cos(Math.PI * i / 32.0 + Math.PI * timer / 24.0 + Math.PI) + 3;
                int y = Mafia.SCREEN_HEIGHT / 2 - 64 + 3;
                video.DrawColor(0, 384, 256, 1, i, 0, (int)Math.Round(x), y + i, Color.FromArgb(128, Color.Black));
            }

            for (int i = 0; i < NUM_OBJECTS; i++)
            {
                objects[i].Draw(video);
            }

            for (int i = 0; i < 128; i++)
            {
                double x = Mafia.SCREEN_WIDTH / 2 - 128 + titleWaveWidth * Math.Cos(Math.PI * i / 32.0 + Math.PI * timer / 24.0) - 1;
                int y = Mafia.SCREEN_HEIGHT / 2 - 64 - 1;
                video.Draw(0, 384, 256, 1, i, 0, (int)Math.Round(x), y + i);
            }
        }
    }
}

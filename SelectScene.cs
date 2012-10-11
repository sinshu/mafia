using System;
using System.Drawing;

namespace Mafia
{
    /// <summary>
    /// ステージセレクト画面。
    /// </summary>
    public class SelectScene
    {
        public const int NONE = -1;
        public const int GOTO_TITLE = -2;

        Stage[] stages;
        private int stageIndex;

        private bool selectSound;

        public SelectScene(Stage[] stages, int stageIndex)
        {
            this.stages = stages;
            this.stageIndex = stageIndex;
            selectSound = false;
        }

        public int CurrentStage
        {
            get
            {
                return stageIndex;
            }

            set
            {
                stageIndex = value;
            }
        }

        public int Tick(SelectInput input)
        {
            selectSound = false;
            if (input.Left && !input.Right)
            {
                if (stageIndex == 0)
                {
                    stageIndex = stages.Length - 1;
                }
                else
                {
                    stageIndex--;
                }
                selectSound = true;
            }
            else if (input.Right && !input.Left)
            {
                stageIndex = (stageIndex + 1) % stages.Length;
                selectSound = true;
            }
            if (input.Ok)
            {
                return stageIndex;
            }
            if (input.GotoTitle)
            {
                return GOTO_TITLE;
            }
            return NONE;
        }

        public void Draw(MafiaVideo video)
        {
            video.FillScreen(Color.FromArgb(128, Color.Black));
            video.DrawStringCenter("ステージをえらんでください", 1, -63, Color.Black);
            video.DrawStringCenter("ステージをえらんでください", 0, -64, Color.White);
            if (stageIndex != 0)
            {
                video.Draw(176, 160, 8, 16, 0, 0, Mafia.SCREEN_WIDTH / 2 - 24, Mafia.SCREEN_HEIGHT / 2 - 8);
            }
            if (stageIndex != stages.Length - 1)
            {
                video.Draw(176, 160, 8, 16, 0, 1, Mafia.SCREEN_WIDTH / 2 + 16, Mafia.SCREEN_HEIGHT / 2 - 8);
            }
            DrawStageNumber(video);
            video.DrawStringCenter(stages[stageIndex].Title, 1, 25, Color.Black);
            video.DrawStringCenter(stages[stageIndex].Title, 0, 24, Color.White);
        }

        public void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            if (selectSound)
            {
                sound.Play(buffers.Ue, null);
            }
        }

        private void DrawStageNumber(MafiaVideo video)
        {
            int number = stageIndex + 1;
            if (number < 10)
            {
                video.Draw(0, 160, 16, 16, 0, number, Mafia.SCREEN_WIDTH / 2 - 8, Mafia.SCREEN_HEIGHT / 2 - 8);
            }
            else
            {
                video.Draw(0, 160, 16, 16, 0, (number / 10) % 10, Mafia.SCREEN_WIDTH / 2 - 8 - 8, Mafia.SCREEN_HEIGHT / 2 - 8);
                video.Draw(0, 160, 16, 16, 0, number % 10, Mafia.SCREEN_WIDTH / 2 - 8 + 8, Mafia.SCREEN_HEIGHT / 2 - 8);
            }
        }
    }
}

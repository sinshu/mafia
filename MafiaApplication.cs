using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;

namespace Mafia
{
    /// <summary>
    /// メインループとか。
    /// </summary>
    public class MafiaApplication
    {
        public const int TITLE_SCENE = 1;
        public const int SELECT_SCENE_TITLE = 2;
        public const int GAME_SCENE = 3;
        public const int SELECT_SCENE_GAME = 4;

        private Form form;
        private MafiaVideo video;
        private MafiaSound sound;
        private MafiaInput input;
        private Audio bgm = null;

        private TitleScene title;
        private SelectScene select;
        private GameScene game;

        private int numStages;
        private string[] stagePath;
        private Stage[] stages;
        private int currentStageIndex;

        private int state;

        private FpsTimer timer;

        public MafiaApplication(string[] stagePath)
        {
            numStages = Mafia.NUM_STAGES + stagePath.Length;
            this.stagePath = stagePath;
        }

        public void Run()
        {
            form = new Form();
            // ウィンドウサイズ可変の方が良くね？
            form.Icon = MafiaLoader.DefaultLoader.GetIcon("mafia.ico");
            form.Text = Mafia.TITLE;

            stages = new Stage[numStages];
            for (int i = 0; i < numStages; i++)
            {
                if (i < Mafia.NUM_STAGES)
                {
                    stages[i] = MafiaLoader.DefaultLoader.GetStage("stage" + i + ".stg");
                }
                else
                {
                    stages[i] = MafiaLoader.DefaultLoader.GetStage(stagePath[i - Mafia.NUM_STAGES]);
                }
            }
            if (stagePath.Length == 0)
            {
                currentStageIndex = 0;
            }
            else
            {
                currentStageIndex = Mafia.NUM_STAGES;
            }

            title = new TitleScene();
            select = new SelectScene(stages, 0);

            state = TITLE_SCENE;

            video = new MafiaVideo(form, MessageBox.Show("フルスクリーンで起動しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes);
            sound = new MafiaSound(form);
            input = new MafiaInput(form);

            try
            {
                bgm = new Audio(Application.StartupPath + "\\" + Mafia.RESOURCE_NAME + "\\mafia.mp3");
                bgm.Ending += new EventHandler(bgm_Ending);
            }
            catch
            {
            }

            form.Show();

            timer = new FpsTimer(Mafia.FPS, false);

            while (CheckState())
            {
                if (input.ShouldToggleFullscreen())
                {
                    video.ToggleFullscreen();
                }
                switch (state)
                {
                    case TITLE_SCENE:
                        // イェンドウ、nullかどうかチェックしなきゃならんとは何事か
                        if (bgm != null && bgm.Playing)
                        {
                            bgm.Stop();
                        }
                        switch (title.Tick(input.GetCurrentTitleInput()))
                        {
                            case TitleScene.NONE:
                                if (timer.ShouldRender() && video.CheckDrawable())
                                {
                                    video.Begin();
                                    video.DrawTitleScene(title);
                                    video.DrawFps(timer.GetCurrentFps());
                                    video.End();
                                    video.Present();
                                }
                                break;
                            case TitleScene.START_GAME:
                                select.CurrentStage = currentStageIndex;
                                state = SELECT_SCENE_TITLE;
                                break;
                            case TitleScene.EXIT:
                                form.Close();
                                break;
                        }
                        break;
                    case SELECT_SCENE_TITLE:
                        {
                            title.Tick(TitleInput.Empty);
                            int result = select.Tick(input.GetCurrentSelectInput());
                            switch (result)
                            {
                                case SelectScene.NONE:
                                    if (timer.ShouldRender() && video.CheckDrawable())
                                    {
                                        video.Begin();
                                        video.DrawTitleScene(title);
                                        video.DrawSelectScene(select);
                                        video.DrawFps(timer.GetCurrentFps());
                                        video.End();
                                        video.Present();
                                    }
                                    sound.PlaySelectSound(select);
                                    break;
                                case SelectScene.GOTO_TITLE:
                                    state = TITLE_SCENE;
                                    break;
                                default:
                                    state = GAME_SCENE;
                                    game = stages[currentStageIndex = result].CreateGame();
                                    break;
                            }
                        }
                        break;
                    case GAME_SCENE:
                        if (bgm != null && !bgm.Playing)
                        {
                            bgm.Play();
                        }
                        switch (game.Tick(input.GetCurrentGameInput()))
                        {
                            case GameScene.NONE:
                                if (timer.ShouldRender() && video.CheckDrawable())
                                {
                                    video.Begin();
                                    video.DrawGameScene(game);
                                    video.DrawFps(timer.GetCurrentFps());
                                    video.End();
                                    video.Present();
                                }
                                sound.PlayGameSound(game);
                                break;
                            case GameScene.RESET_GAME:
                                sound.StopSounds();
                                game = stages[currentStageIndex].CreateGame();
                                break;
                            case GameScene.CLEAR_GAME:
                                sound.StopSounds();
                                currentStageIndex = (currentStageIndex + 1) % numStages;
                                game = stages[currentStageIndex].CreateGame();
                                break;
                            case GameScene.SELECT:
                                if (timer.ShouldRender() && video.CheckDrawable())
                                {
                                    video.Begin();
                                    video.DrawGameScene(game);
                                    video.DrawFps(timer.GetCurrentFps());
                                    video.End();
                                    video.Present();
                                }
                                sound.PlayGameSound(game);
                                select.CurrentStage = currentStageIndex;
                                state = SELECT_SCENE_GAME;
                                break;
                            case GameScene.GOTO_TITLE:
                                sound.StopSounds();
                                state = TITLE_SCENE;
                                break;
                        }
                        break;
                    case SELECT_SCENE_GAME:
                        game.Tick(GameInput.Empty);
                        {
                            int result = select.Tick(input.GetCurrentSelectInput());
                            switch (result)
                            {
                                case SelectScene.NONE:
                                    if (timer.ShouldRender() && video.CheckDrawable())
                                    {
                                        video.Begin();
                                        video.DrawGameScene(game);
                                        video.DrawSelectScene(select);
                                        video.DrawFps(timer.GetCurrentFps());
                                        video.End();
                                        video.Present();
                                    }
                                    sound.PlayGameSound(game);
                                    sound.PlaySelectSound(select);
                                    break;
                                case SelectScene.GOTO_TITLE:
                                    sound.StopSounds();
                                    state = TITLE_SCENE;
                                    break;
                                default:
                                    sound.StopSounds();
                                    game = stages[currentStageIndex = result].CreateGame();
                                    state = GAME_SCENE;
                                    break;
                            }
                        }
                        break;
                }
                Application.DoEvents();
                timer.Wait();
            }

            if (bgm != null)
            {
                if (bgm.Playing)
                {
                    bgm.Stop();
                }
                bgm.Dispose();
                bgm = null;
            }
            video.Dispose();
            video = null;
            sound.Dispose();
            sound = null;
            input.Dispose();
            input = null;
        }

        void bgm_Ending(object sender, EventArgs e)
        {
            bgm.CurrentPosition = 0;
        }

        public bool CheckState()
        {
            return form.Created;
        }

        public void Dispose()
        {
            if (bgm != null)
            {
                bgm.Dispose();
                bgm = null;
            }
            if (video != null)
            {
                video.Dispose();
                video = null;
            }
            if (input != null)
            {
                input.Dispose();
                input = null;
            }
            if (form != null)
            {
                form.Dispose();
                form = null;
            }
        }

        public static void Main(string[] args)
        {
            MafiaApplication app = null;
            try
            {
                app = new MafiaApplication(args);
                app.Run();
            }
            catch (Exception e)
            {
                // MessageBox.Show("エラーが発生したらしいです＞＜\n" + e.Message);
                MessageBox.Show("エラーが発生したらしいです＞＜\n" + e.Message.Replace("。", "＞＜"), "ひぃ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                app.Dispose();
            }
        }
    }
}

using System;
using System.Drawing;

namespace Mafia
{
    /// <summary>
    /// ゲーム云々。
    /// </summary>
    public class GameScene
    {
        public const int NONE = 0;
        public const int RESET_GAME = 1;
        public const int CLEAR_GAME = 2;
        public const int SELECT = 3;
        public const int GOTO_TITLE = 4;

        private string title;
        private Map map;
        private Player player;
        private ThingList things;
        private ThingList addThings;
        private Vector camera;

        private int firstNumCoins;
        private int currentNumCoins;
        private int gameTimer;
        private int missTimer;
        private int clearTimer;

        public GameScene(string title, int numRows, int numCols)
        {
            this.title = title;
            map = new Map(numRows, numCols);
            things = new ThingList();
            addThings = new ThingList();
            currentNumCoins = 0;
            gameTimer = 0;
            missTimer = 0;
            clearTimer = 0;
        }

        public void AddPlayer(int row, int col, int direction)
        {
            player = new Player(this, row, col, direction);
            things.AddThing(player);
            Camera = player.Focus - new Vector(Mafia.SCREEN_WIDTH / 2, Mafia.SCREEN_HEIGHT / 2);
        }

        public void AddCoin(int row, int col)
        {
            things.AddThing(new Coin(this, row, col));
            firstNumCoins++;
            currentNumCoins++;
        }

        public void AddLift(int row, int col, int behavior)
        {
            things.AddThing(new Lift(this, row, col, behavior));
        }

        public void AddSpring(int row, int col)
        {
            things.AddThing(new Spring(this, row, col));
        }

        public void AddBox(int row, int col)
        {
            things.AddThing(new Box(this, row, col));
        }

        public void AddSwitch(int row, int col)
        {
            things.AddThing(new Switch(this, row, col));
        }

        public void AddDoor(int row, int col, int state)
        {
            if (state == 0)
            {
                things.AddThing(new Door(this, row, col, false));
            }
            else
            {
                things.AddThing(new Door(this, row, col, true));
            }
        }

        public void AddFallBlock(int row, int col, int type)
        {
            things.AddThing(new FallBlock(this, row, col, type));
        }

        public void AddDelayedFallBlock(int row, int col, int type)
        {
            things.AddThing(new DelayedFallBlock(this, row, col, type));
        }

        public void AddThingInGame(Thing thing)
        {
            addThings.AddThing(thing);
        }

        public void GetCoin(Coin coin)
        {
            coin.Remove();
            currentNumCoins--;
        }

        public void ToggleDoors()
        {
            foreach (Thing thing in things)
            {
                if (thing is Door)
                {
                    Door door = (Door)thing;
                    door.Open = !door.Open;
                }
            }
        }

        public int Tick(GameInput input)
        {
            if (gameTimer == 0) things.Initialize();
            int result = NONE;
            things.BeforeTick();
            things.Tick(input);
            things.AddThingList(addThings);
            addThings.Tick(input);
            addThings.Clear();
            things.AfterTick();
            Camera = (player.Focus - new Vector(Mafia.SCREEN_WIDTH / 2, Mafia.SCREEN_HEIGHT / 2)) * 0.125 + camera * 0.875;
            gameTimer++;
            if (player.Missed)
            {
                if (missTimer < 180)
                {
                    missTimer++;
                }
                else
                {
                    result = RESET_GAME;
                }
            }
            else if (currentNumCoins == 0)
            {
                if (clearTimer < 180)
                {
                    clearTimer++;
                }
                else
                {
                    result = CLEAR_GAME;
                }
            }
            if (input.GotoSelect)
            {
                result = SELECT;
            }
            if (input.GotoTitle)
            {
                result = GOTO_TITLE;
            }
            return result;
        }

        public Map Map
        {
            get
            {
                return map;
            }
        }

        public Player Player
        {
            get
            {
                return player;
            }
        }

        public ThingList ThingList
        {
            get
            {
                return things;
            }
        }

        public Vector Camera
        {
            get
            {
                return camera;
            }

            set
            {
                if (value.X < 0)
                {
                    camera.X = 0;
                }
                else if (value.X > map.Width - Mafia.SCREEN_WIDTH)
                {
                    camera.X = map.Width - Mafia.SCREEN_WIDTH;
                }
                else
                {
                    camera.X = value.X;
                }

                if (value.Y < 0)
                {
                    camera.Y = 0;
                }
                else if (value.Y > map.Height - Mafia.SCREEN_HEIGHT)
                {
                    camera.Y = map.Height - Mafia.SCREEN_HEIGHT;
                }
                else
                {
                    camera.Y = value.Y;
                }
            }
        }

        public int NumCoins
        {
            get
            {
                return currentNumCoins;
            }
        }

        public void Draw(MafiaVideo video)
        {
            map.Draw(video, (IntVector)camera);
            things.Draw(video, (IntVector)camera);
            if (!player.Missed)
            {
                if (currentNumCoins == 0)
                {
                    // video.Draw(0, 320, 128, 64, 0, 0, Mafia.SCREEN_WIDTH / 2 - 64, Mafia.SCREEN_HEIGHT / 2 - 32);
                    double waveWidth = 0;
                    if (clearTimer < 60)
                    {
                        waveWidth = (60 - clearTimer) / 2;
                    }
                    for (int i = 0; i < 64; i++)
                    {
                        double x = Mafia.SCREEN_WIDTH / 2 - 64 + waveWidth * Math.Cos(Math.PI * i / 16.0 + Math.PI * clearTimer / 12.0 + Math.PI) + 2;
                        int y = Mafia.SCREEN_HEIGHT / 2 - 32;
                        video.DrawColor(0, 320, 128, 1, i, 0, (int)Math.Round(x), y + i + 2, Color.FromArgb(128, Color.Black));
                    }
                    for (int i = 0; i < 64; i++)
                    {
                        double x = Mafia.SCREEN_WIDTH / 2 - 64 + waveWidth * Math.Cos(Math.PI * i / 16.0 + Math.PI * clearTimer / 12.0);
                        int y = Mafia.SCREEN_HEIGHT / 2 - 32;
                        video.Draw(0, 320, 128, 1, i, 0, (int)Math.Round(x), y + i);
                    }
                }
                else
                {
                    if (gameTimer < 300)
                    {
                        video.DrawStringCenter(title, 1, 1, Color.Black);
                        video.DrawStringCenter(title, 0, 0, Color.White);
                    }
                    else
                    {
                        video.DrawString(title, 9, 9, Color.Black);
                        video.DrawString(title, 8, 8, Color.White);
                    }
                    video.Draw(0, 160, 16, 16, 0, (firstNumCoins - currentNumCoins) % 10, Mafia.SCREEN_WIDTH - 48, Mafia.SCREEN_HEIGHT - 16);
                    video.Draw(0, 160, 16, 16, 0, 10, Mafia.SCREEN_WIDTH - 32, Mafia.SCREEN_HEIGHT - 16);
                    video.Draw(0, 160, 16, 16, 0, firstNumCoins % 10, Mafia.SCREEN_WIDTH - 16, Mafia.SCREEN_HEIGHT - 16);
                }
            }
        }

        public void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            things.PlaySound(sound, buffers);
        }
    }
}

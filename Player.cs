using System;

namespace Mafia
{
    /// <summary>
    /// クラス名を見れば分かると思うがプレイヤー。
    /// </summary>
    public class Player : Thing
    {
        private const int WIDTH = 16;
        private const int HEIGHT = 32;

        public const int LEFT = -1;
        public const int RIGHT = 1;

        public const int IN_AIR = 0;
        public const int ON_GROUND = 1;

        public int Direction;
        public int Animation;
        public int LandState;
        public int JumpTick;

        private Vector focus;

        private bool missed;

        private bool left;
        private bool up;
        private bool right;
        private bool down;

        private bool jumpSound;
        private bool tiunSound;
        private bool coinSound;

        public Player(GameScene game, int row, int col, int direction) : base(game, row, col)
        {
            if (direction == RIGHT)
            {
                this.Direction = RIGHT;
            }
            else
            {
                this.Direction = LEFT;
            }
            Animation = 0;
            LandState = IN_AIR;
            JumpTick = -1;
            focus = Vector.Zero;
            if (Center.X + focus.X < Mafia.SCREEN_WIDTH / 2)
            {
                focus.X = Mafia.SCREEN_WIDTH / 2 - Center.X;
            }
            else if (Center.X + focus.X > game.Map.Width - Mafia.SCREEN_WIDTH / 2)
            {
                focus.X = game.Map.Width - Mafia.SCREEN_WIDTH / 2 - Center.X;
            }
            if (Center.Y + focus.Y < Mafia.SCREEN_HEIGHT / 2)
            {
                focus.Y = Mafia.SCREEN_HEIGHT / 2 - Center.Y;
            }
            else if (Center.Y + focus.Y > game.Map.Height - Mafia.SCREEN_HEIGHT / 2)
            {
                focus.Y = game.Map.Height - Mafia.SCREEN_HEIGHT / 2 - Center.Y;
            }
            missed = false;
            left = up = right = down = false;
            jumpSound = false;
            tiunSound = false;
            coinSound = false;
        }

        public override bool IsObstacle
        {
            get
            {
                return !missed;
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

        public bool Missed
        {
            get
            {
                return missed;
            }
        }

        public override void CollidLeft(Thing thing)
        {
            if (thing == null)
            {
                Velocity.X = 0;
            }
            else if (thing is Box && LandState == ON_GROUND && thing.Carry.Count == 0 && ((Box)thing).LandState == Box.ON_GROUND)
            {
                if (Velocity.X == -2)
                {
                    thing.MoveHorizontalBy(-1);
                    double oldX = Position.X;
                    Left = thing.Right;
                    double newX = Position.X;
                    Position.X = oldX;
                    if (newX != oldX)
                    {
                        MoveHorizontalTo(newX);
                        ((Box)thing).PushedByPlayer();
                    }
                }
            }
            else
            {
                Velocity.X = 0;
            }
        }

        public override void CollidRight(Thing thing)
        {
            if (thing == null)
            {
                Velocity.X = 0;
            }
            else if (thing is Box && LandState == ON_GROUND && thing.Carry.Count == 0 && ((Box)thing).LandState == Box.ON_GROUND)
            {
                if (Velocity.X == 2)
                {
                    thing.MoveHorizontalBy(1);
                    double oldX = Position.X;
                    Right = thing.Left;
                    double newX = Position.X;
                    Position.X = oldX;
                    if (newX != oldX)
                    {
                        MoveHorizontalTo(newX);
                        ((Box)thing).PushedByPlayer();
                    }
                }
            }
            else
            {
                Velocity.X = 0;
            }
        }

        public override void CollidTop(Thing thing)
        {
            Velocity.Y = 0;
            JumpTick = -1;
        }

        public override void CollidBottom(Thing thing)
        {
            base.CollidBottom(thing);
            if (thing is Spring)
            {
                Animation = (Animation + 4) % 8;
                if (up)
                {
                    LandState = IN_AIR;
                    JumpTick = 30;
                }
            }
            else
            {
                if (!left && !right || left && right)
                {
                    Animation = 0;
                }
                LandState = ON_GROUND;
            }
        }

        public override void CollidedBottom(Thing thing)
        {
            if (!left && !right || left && right)
            {
                Animation = 0;
            }
            LandState = ON_GROUND;
        }

        public override void Damaged(Thing thing)
        {
            Animation = 0;
            if (!missed)
            {
                missed = true;
                for (int i = 0; i < 8; i++)
                {
                    Game.AddThingInGame(new Tiun(Game, Position, i, 2, 64, Tiun.TIUN));
                    Game.AddThingInGame(new Tiun(Game, Position, i, 4, 64, Tiun.TIUN));
                }
                tiunSound = true;
            }
        }

        public override void BeforeTick()
        {
            jumpSound = false;
            tiunSound = false;
            coinSound = false;
        }

        public override void Tick(GameInput input)
        {
            left = input.Left;
            up = input.Up;
            right = input.Right;
            down = input.Down;

            if (!missed)
            {
                if (input.GotoSelect) Damaged(null);

                #region 左右の動き
                {
                    if (!left && !right || left && right)
                    {
                        if (LandState == IN_AIR)
                        {
                            if (Math.Abs(Velocity.X) > 0.0625)
                            {
                                Velocity.X -= Math.Sign(Velocity.X) * 0.0625;
                            }
                            else
                            {
                                Velocity.X = 0;
                            }
                        }
                        else if (LandState == ON_GROUND)
                        {
                            if (Math.Abs(Velocity.X) > 0.25)
                            {
                                Velocity.X -= Math.Sign(Velocity.X) * 0.25;
                            }
                            else
                            {
                                Velocity.X = 0;
                            }
                        }
                    }
                    else if (left && !right)
                    {
                        Direction = LEFT;
                        if (LandState == IN_AIR)
                        {
                            Velocity.X -= 0.25;
                        }
                        else if (LandState == ON_GROUND)
                        {
                            Velocity.X -= 0.5;
                            Animation = (Animation + 1) % 8;
                        }
                    }
                    else if (right && !left)
                    {
                        Direction = RIGHT;
                        if (LandState == IN_AIR)
                        {
                            Velocity.X += 0.25;
                        }
                        else if (LandState == ON_GROUND)
                        {
                            Velocity.X += 0.5;
                            Animation = (Animation + 1) % 8;
                        }
                    }
                    if (Math.Abs(Velocity.X) > 2) Velocity.X = Math.Sign(Velocity.X) * 2;
                    MoveHorizontalBy(Velocity.X);
                }
                #endregion

                #region ジャンプ＆上下の動き
                {
                    Velocity.Y += 0.25;
                    if (Velocity.Y > 4)
                    {
                        Velocity.Y = 4;
                    }
                    if (LandState == ON_GROUND)
                    {
                        if (up)
                        {
                            if (JumpTick != -1)
                            {
                                Animation = (Animation + 4) % 8;
                                LandState = IN_AIR;
                                JumpTick = 14;
                                jumpSound = true;
                            }
                        }
                        else
                        {
                            JumpTick = 0;
                        }
                    }
                    if (LandState == IN_AIR)
                    {
                        if (JumpTick > 0)
                        {
                            if (up)
                            {
                                Velocity.Y = -4;
                                JumpTick--;
                            }
                            else
                            {
                                JumpTick = -1;
                            }
                        }
                        else
                        {
                            if (Velocity.Y < 0)
                            {
                                JumpTick = -1;
                            }
                            else if (!up)
                            {
                                JumpTick = 0;
                            }
                        }
                    }
                    LandState = IN_AIR;
                    MoveVerticalBy(Velocity.Y);
                }
                #endregion

                foreach (Thing thing in Game.ThingList)
                {
                    if (thing is Coin)
                    {
                        if (Overlapped(thing, 4))
                        {
                            ((Coin)thing).KiraKira();
                            Game.GetCoin((Coin)thing);
                            coinSound = true;
                        }
                    }
                }

                #region カメラ位置の補正
                {
                    switch (Direction)
                    {
                        case LEFT:
                            focus.X -= 0.5;
                            break;
                        case RIGHT:
                            focus.X += 0.5;
                            break;
                    }
                    if (Math.Abs(focus.X) > 64) focus.X = Math.Sign(focus.X) * 64;
                    if (Center.X + focus.X < Mafia.SCREEN_WIDTH / 2)
                    {
                        focus.X = Mafia.SCREEN_WIDTH / 2 - Center.X;
                    }
                    else if (Center.X + focus.X > Game.Map.Width - Mafia.SCREEN_WIDTH / 2)
                    {
                        focus.X = Game.Map.Width - Mafia.SCREEN_WIDTH / 2 - Center.X;
                    }
                    if (down)
                    {
                        focus.Y += 0.5;
                    }
                    else
                    {
                        focus.Y -= 0.5;
                    }
                    if (Math.Abs(focus.Y) > 32) focus.Y = Math.Sign(focus.Y) * 32;
                    if (Center.Y + focus.Y < Mafia.SCREEN_HEIGHT / 2)
                    {
                        focus.Y = Mafia.SCREEN_HEIGHT / 2 - Center.Y;
                    }
                    else if (Center.Y + focus.Y > Game.Map.Height - Mafia.SCREEN_HEIGHT / 2)
                    {
                        focus.Y = Game.Map.Height - Mafia.SCREEN_HEIGHT / 2 - Center.Y;
                    }
                }
                #endregion
            }
        }

        public override void Draw(MafiaVideo video, IntVector camera)
        {
            if (!ShouldDraw(camera)) return;
            if (missed) return;
            IntVector p = (IntVector)Position - camera;
            switch (Direction)
            {
                case LEFT:
                    video.Draw(0, 16, 16, 32, 0, Animation, p.X, p.Y);
                    break;
                case RIGHT:
                    video.Draw(128, 16, 16, 32, 0, Animation, p.X, p.Y);
                    break;
            }
        }

        public override void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            if (!ShouldPlaySound()) return;
            if (jumpSound)
            {
                sound.Play(buffers.Jump, this);
            }
            if (tiunSound)
            {
                sound.Play(buffers.Tiun, this);
            }
            if (coinSound)
            {
                sound.Play(buffers.Coin, this);
                if (Game.NumCoins > 0)
                {
                    sound.Play(buffers.Hyakuyen, this);
                }
                else
                {
                    sound.Play(buffers.Stiana, this);
                }
            }
        }

        /*
        public override double MoveHorizontalBy(double d)
        {
            Carry.RemoveThings(IsNotCarry);
            if (CanPush)
            {
                if (d < 0)
                {
                    MoveLeftBy_CanPush(d);
                }
                else if (d > 0)
                {
                    MoveRightBy_CanPush(d);
                }
            }
            else
            {
                if (d < 0)
                {
                    MoveLeftBy_CannotPush(d);
                }
                else if (d > 0)
                {
                    MoveRightBy_CannotPush(d);
                }
            }
            return 0;
        }
        */

        public Vector Focus
        {
            get
            {
                return Center + focus;
            }
        }
    }
}

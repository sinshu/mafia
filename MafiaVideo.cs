using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mafia
{
    /// <summary>
    /// 画面表示の云々を扱う。
    /// </summary>
    /// <remarks>
    /// フルスクリーン時にAlt+Tabを押しても無問題。
    /// SpriteをDeviceLost時にDisposeするとDeviceReset時にエラーが発生する。
    /// でも再生成しなくてもちゃんと動いてるっぽいので放置。
    /// </remarks>
    public class MafiaVideo : IDisposable
    {
        private Form form;
        private Device device;
        private Sprite sprite;
        private Surface backBuffer;
        private Texture offScreenImage;
        private Surface offScreenSurface;
        private Texture texture;

        private EventHandler deviceLost;
        private EventHandler deviceReset;
        private CancelEventHandler deviceResizing;

        private bool fullscreen;

        public MafiaVideo(Form form, bool fullscreen)
        {
            this.form = form;
            this.fullscreen = fullscreen;

            if (fullscreen)
            {
                form.FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                form.FormBorderStyle = FormBorderStyle.Sizable;
            }

            try
            {
                device = new Device(0, DeviceType.Hardware, form, CreateFlags.HardwareVertexProcessing, GetDefaultPresentParameters(fullscreen));
            }
            catch (InvalidCallException)
            {
                try
                {
                    device = new Device(0, DeviceType.Hardware, form, CreateFlags.SoftwareVertexProcessing, GetDefaultPresentParameters(fullscreen));
                }
                catch (InvalidCallException)
                {
                    try
                    {
                        device = new Device(0, DeviceType.Software, form, CreateFlags.SoftwareVertexProcessing, GetDefaultPresentParameters(fullscreen));
                    }
                    catch (InvalidCallException)
                    {
                        throw new Exception("Direct3Dデバイスの生成に失敗しました。");
                    }
                }
            }
            deviceLost = new EventHandler(OnDeviceLost);
            deviceReset = new EventHandler(OnDeviceReset);
            deviceResizing = new CancelEventHandler(OnDeviceResizing);
            device.DeviceLost += deviceLost;
            device.DeviceReset += deviceReset;
            device.DeviceResizing += deviceResizing;
            device.Clear(ClearFlags.Target, Color.FromArgb(212, 208, 200), 0.0f, 0);

            sprite = new Sprite(device);

            backBuffer = device.GetBackBuffer(0, 0, BackBufferType.Mono);
            offScreenImage = new Texture(device, Mafia.SCREEN_WIDTH, Mafia.SCREEN_HEIGHT, 1, Usage.RenderTarget, Manager.Adapters[0].CurrentDisplayMode.Format, Pool.Default);
            offScreenSurface = offScreenImage.GetSurfaceLevel(0);

            texture = MafiaLoader.DefaultLoader.GetTexture(device, "mafia.bmp");

            if (fullscreen)
            {
                Cursor.Hide();
            }

            // form.ClientSize = new Size(Mafia.SCREEN_WIDTH, Mafia.SCREEN_HEIGHT);
            form.ClientSize = new Size(Mafia.SCREEN_WIDTH * 2, Mafia.SCREEN_HEIGHT * 2);
        }

        public bool CheckDrawable()
        {
            int result;
            device.CheckCooperativeLevel(out result);
            switch ((ResultCode)result)
            {
                case ResultCode.Success:
                    return true;
                case ResultCode.DeviceLost:
                    return false;
                case ResultCode.DeviceNotReset:
                    device.Reset(GetDefaultPresentParameters(fullscreen));
                    return true;
            }
            return false;
        }

        public void ResetDevice(bool fullscreen)
        {
            this.fullscreen = fullscreen;
            if (fullscreen)
            {
                Cursor.Hide();
                form.FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                Cursor.Show();
                form.FormBorderStyle = FormBorderStyle.Sizable;
            }
            device.Reset(GetDefaultPresentParameters(fullscreen));
            // form.ClientSize = new Size(Mafia.SCREEN_WIDTH, Mafia.SCREEN_HEIGHT);
            form.ClientSize = new Size(Mafia.SCREEN_WIDTH * 2, Mafia.SCREEN_HEIGHT * 2);
        }

        public void ToggleFullscreen()
        {
            fullscreen = !fullscreen;
            ResetDevice(fullscreen);
        }

        public void Begin()
        {
            device.SetRenderTarget(0, offScreenSurface);
            device.BeginScene();
            sprite.Begin(SpriteFlags.AlphaBlend);
            device.SetSamplerState(0, SamplerStageStates.MagFilter, true);
            device.SetSamplerState(0, SamplerStageStates.MinFilter, true);
        }

        public void End()
        {
            sprite.End();
            device.EndScene();
            device.SetRenderTarget(0, backBuffer);
        }

        public void Present()
        {
            // device.Clear(ClearFlags.Target, Color.Black, 0.0f, 0);
            device.BeginScene();
            sprite.Begin(SpriteFlags.None);
            device.SetSamplerState(0, SamplerStageStates.MagFilter, true);
            device.SetSamplerState(0, SamplerStageStates.MinFilter, true);
            sprite.Draw2D(offScreenImage, new Rectangle(0, 0, Mafia.SCREEN_WIDTH, Mafia.SCREEN_HEIGHT), new Size(Mafia.SCREEN_WIDTH * 2, Mafia.SCREEN_HEIGHT * 2), new Point(0, 0), Color.White);
            sprite.End();
            device.EndScene();
            device.Present();

            // 描画が重いとき再現用。
            // System.Threading.Thread.Sleep(100);
        }

        public void Draw(int srcX, int srcY, int width, int height, int row, int col, int dstX, int dstY)
        {
            sprite.Draw2D(texture, new Rectangle(srcX + width * col, srcY + height * row, width, height), new Size(width, height), new Point(dstX, dstY), Color.White);
        }

        public void DrawColor(int srcX, int srcY, int width, int height, int row, int col, int dstX, int dstY, Color color)
        {
            sprite.Draw2D(texture, new Rectangle(srcX + width * col, srcY + height * row, width, height), new Size(width, height), new Point(dstX, dstY), color);
        }

        public void DrawString(string s, int x, int y, Color color)
        {
            for (int i = 0; i < s.Length; i++)
            {
                bool katakana = false;
                bool ten = false;
                bool maru = false;
                int row = 0;
                int col = 5;
                switch (s[i])
                {
                    case 'あ':
                        row = 0; col = 0; break;
                    case 'い':
                        row = 0; col = 1; break;
                    case 'う':
                        row = 0; col = 2; break;
                    case 'え':
                        row = 0; col = 3; break;
                    case 'お':
                        row = 0; col = 4; break;
                    case 'か':
                        row = 1; col = 0; break;
                    case 'き':
                        row = 1; col = 1; break;
                    case 'く':
                        row = 1; col = 2; break;
                    case 'け':
                        row = 1; col = 3; break;
                    case 'こ':
                        row = 1; col = 4; break;
                    case 'が':
                        ten = true; row = 1; col = 0; break;
                    case 'ぎ':
                        ten = true; row = 1; col = 1; break;
                    case 'ぐ':
                        ten = true; row = 1; col = 2; break;
                    case 'げ':
                        ten = true; row = 1; col = 3; break;
                    case 'ご':
                        ten = true; row = 1; col = 4; break;
                    case 'さ':
                        row = 2; col = 0; break;
                    case 'し':
                        row = 2; col = 1; break;
                    case 'す':
                        row = 2; col = 2; break;
                    case 'せ':
                        row = 2; col = 3; break;
                    case 'そ':
                        row = 2; col = 4; break;
                    case 'ざ':
                        ten = true; row = 2; col = 0; break;
                    case 'じ':
                        ten = true; row = 2; col = 1; break;
                    case 'ず':
                        ten = true; row = 2; col = 2; break;
                    case 'ぜ':
                        ten = true; row = 2; col = 3; break;
                    case 'ぞ':
                        ten = true; row = 2; col = 4; break;
                    case 'た':
                        row = 3; col = 0; break;
                    case 'ち':
                        row = 3; col = 1; break;
                    case 'つ':
                        row = 3; col = 2; break;
                    case 'っ':
                        row = 11; col = 2; break;
                    case 'て':
                        row = 3; col = 3; break;
                    case 'と':
                        row = 3; col = 4; break;
                    case 'だ':
                        ten = true; row = 3; col = 0; break;
                    case 'ぢ':
                        ten = true; row = 3; col = 1; break;
                    case 'づ':
                        ten = true; row = 3; col = 2; break;
                    case 'で':
                        ten = true; row = 3; col = 3; break;
                    case 'ど':
                        ten = true; row = 3; col = 4; break;
                    case 'な':
                        row = 4; col = 0; break;
                    case 'に':
                        row = 4; col = 1; break;
                    case 'ぬ':
                        row = 4; col = 2; break;
                    case 'ね':
                        row = 4; col = 3; break;
                    case 'の':
                        row = 4; col = 4; break;
                    case 'は':
                        row = 5; col = 0; break;
                    case 'ひ':
                        row = 5; col = 1; break;
                    case 'ふ':
                        row = 5; col = 2; break;
                    case 'へ':
                        row = 5; col = 3; break;
                    case 'ほ':
                        row = 5; col = 4; break;
                    case 'ば':
                        ten = true; row = 5; col = 0; break;
                    case 'び':
                        ten = true; row = 5; col = 1; break;
                    case 'ぶ':
                        ten = true; row = 5; col = 2; break;
                    case 'べ':
                        ten = true; row = 5; col = 3; break;
                    case 'ぼ':
                        ten = true; row = 5; col = 4; break;
                    case 'ぱ':
                        maru = true; row = 5; col = 0; break;
                    case 'ぴ':
                        maru = true; row = 5; col = 1; break;
                    case 'ぷ':
                        maru = true; row = 5; col = 2; break;
                    case 'ぺ':
                        maru = true; row = 5; col = 3; break;
                    case 'ぽ':
                        maru = true; row = 5; col = 4; break;
                    case 'ま':
                        row = 6; col = 0; break;
                    case 'み':
                        row = 6; col = 1; break;
                    case 'む':
                        row = 6; col = 2; break;
                    case 'め':
                        row = 6; col = 3; break;
                    case 'も':
                        row = 6; col = 4; break;
                    case 'や':
                        row = 7; col = 0; break;
                    case 'ゆ':
                        row = 7; col = 2; break;
                    case 'よ':
                        row = 7; col = 4; break;
                    case 'ゃ':
                        row = 12; col = 0; break;
                    case 'ゅ':
                        row = 12; col = 2; break;
                    case 'ょ':
                        row = 12; col = 4; break;
                    case 'ら':
                        row = 8; col = 0; break;
                    case 'り':
                        row = 8; col = 1; break;
                    case 'る':
                        row = 8; col = 2; break;
                    case 'れ':
                        row = 8; col = 3; break;
                    case 'ろ':
                        row = 8; col = 4; break;
                    case 'わ':
                        row = 9; col = 0; break;
                    case 'を':
                        row = 9; col = 4; break;
                    case 'ん':
                        row = 10; col = 0; break;
                    case 'ア':
                        katakana = true; row = 0; col = 0; break;
                    case 'イ':
                        katakana = true; row = 0; col = 1; break;
                    case 'ウ':
                        katakana = true; row = 0; col = 2; break;
                    case 'エ':
                        katakana = true; row = 0; col = 3; break;
                    case 'オ':
                        katakana = true; row = 0; col = 4; break;
                    case 'カ':
                        katakana = true; row = 1; col = 0; break;
                    case 'キ':
                        katakana = true; row = 1; col = 1; break;
                    case 'ク':
                        katakana = true; row = 1; col = 2; break;
                    case 'ケ':
                        katakana = true; row = 1; col = 3; break;
                    case 'コ':
                        katakana = true; row = 1; col = 4; break;
                    case 'ガ':
                        katakana = true; ten = true; row = 1; col = 0; break;
                    case 'ギ':
                        katakana = true; ten = true; row = 1; col = 1; break;
                    case 'グ':
                        katakana = true; ten = true; row = 1; col = 2; break;
                    case 'ゲ':
                        katakana = true; ten = true; row = 1; col = 3; break;
                    case 'ゴ':
                        katakana = true; ten = true; row = 1; col = 4; break;
                    case 'サ':
                        katakana = true; row = 2; col = 0; break;
                    case 'シ':
                        katakana = true; row = 2; col = 1; break;
                    case 'ス':
                        katakana = true; row = 2; col = 2; break;
                    case 'セ':
                        katakana = true; row = 2; col = 3; break;
                    case 'ソ':
                        katakana = true; row = 2; col = 4; break;
                    case 'ザ':
                        katakana = true; ten = true; row = 2; col = 0; break;
                    case 'ジ':
                        katakana = true; ten = true; row = 2; col = 1; break;
                    case 'ズ':
                        katakana = true; ten = true; row = 2; col = 2; break;
                    case 'ゼ':
                        katakana = true; ten = true; row = 2; col = 3; break;
                    case 'ゾ':
                        katakana = true; ten = true; row = 2; col = 4; break;
                    case 'タ':
                        katakana = true; row = 3; col = 0; break;
                    case 'チ':
                        katakana = true; row = 3; col = 1; break;
                    case 'ツ':
                        katakana = true; row = 3; col = 2; break;
                    case 'ッ':
                        katakana = true; row = 11; col = 2; break;
                    case 'テ':
                        katakana = true; row = 3; col = 3; break;
                    case 'ト':
                        katakana = true; row = 3; col = 4; break;
                    case 'ダ':
                        katakana = true; ten = true; row = 3; col = 0; break;
                    case 'ヂ':
                        katakana = true; ten = true; row = 3; col = 1; break;
                    case 'ヅ':
                        katakana = true; ten = true; row = 3; col = 2; break;
                    case 'デ':
                        katakana = true; ten = true; row = 3; col = 3; break;
                    case 'ド':
                        katakana = true; ten = true; row = 3; col = 4; break;
                    case 'ナ':
                        katakana = true; row = 4; col = 0; break;
                    case 'ニ':
                        katakana = true; row = 4; col = 1; break;
                    case 'ヌ':
                        katakana = true; row = 4; col = 2; break;
                    case 'ネ':
                        katakana = true; row = 4; col = 3; break;
                    case 'ノ':
                        katakana = true; row = 4; col = 4; break;
                    case 'ハ':
                        katakana = true; row = 5; col = 0; break;
                    case 'ヒ':
                        katakana = true; row = 5; col = 1; break;
                    case 'フ':
                        katakana = true; row = 5; col = 2; break;
                    case 'ヘ':
                        katakana = true; row = 5; col = 3; break;
                    case 'ホ':
                        katakana = true; row = 5; col = 4; break;
                    case 'バ':
                        katakana = true; ten = true; row = 5; col = 0; break;
                    case 'ビ':
                        katakana = true; ten = true; row = 5; col = 1; break;
                    case 'ブ':
                        katakana = true; ten = true; row = 5; col = 2; break;
                    case 'ベ':
                        katakana = true; ten = true; row = 5; col = 3; break;
                    case 'ボ':
                        katakana = true; ten = true; row = 5; col = 4; break;
                    case 'パ':
                        katakana = true; maru = true; row = 5; col = 0; break;
                    case 'ピ':
                        katakana = true; maru = true; row = 5; col = 1; break;
                    case 'プ':
                        katakana = true; maru = true; row = 5; col = 2; break;
                    case 'ペ':
                        katakana = true; maru = true; row = 5; col = 3; break;
                    case 'ポ':
                        katakana = true; maru = true; row = 5; col = 4; break;
                    case 'マ':
                        katakana = true; row = 6; col = 0; break;
                    case 'ミ':
                        katakana = true; row = 6; col = 1; break;
                    case 'ム':
                        katakana = true; row = 6; col = 2; break;
                    case 'メ':
                        katakana = true; row = 6; col = 3; break;
                    case 'モ':
                        katakana = true; row = 6; col = 4; break;
                    case 'ヤ':
                        katakana = true; row = 7; col = 0; break;
                    case 'ユ':
                        katakana = true; row = 7; col = 2; break;
                    case 'ヨ':
                        katakana = true; row = 7; col = 4; break;
                    case 'ャ':
                        katakana = true; row = 12; col = 0; break;
                    case 'ュ':
                        katakana = true; row = 12; col = 2; break;
                    case 'ョ':
                        katakana = true; row = 12; col = 4; break;
                    case 'ラ':
                        katakana = true; row = 8; col = 0; break;
                    case 'リ':
                        katakana = true; row = 8; col = 1; break;
                    case 'ル':
                        katakana = true; row = 8; col = 2; break;
                    case 'レ':
                        katakana = true; row = 8; col = 3; break;
                    case 'ロ':
                        katakana = true; row = 8; col = 4; break;
                    case 'ワ':
                        katakana = true; row = 9; col = 0; break;
                    case 'ヲ':
                        katakana = true; row = 9; col = 4; break;
                    case 'ン':
                        katakana = true; row = 10; col = 0; break;
                    case 'ー':
                        row = 10; col = 2; break;
                }
                if (ten)
                {
                    DrawColor(0, 176, 8, 8, 10, 3, x + i * 8, y - 8, color);
                }
                else if (maru)
                {
                    DrawColor(0, 176, 8, 8, 10, 4, x + i * 8, y - 8, color);
                }
                if (!katakana)
                {
                    DrawColor(0, 176, 8, 8, row, col, x + i * 8, y, color);
                }
                else
                {
                    DrawColor(64, 176, 8, 8, row, col, x + i * 8, y, color);
                }
            }
        }

        public void DrawStringCenter(string s, int offsetX, int offsetY, Color color)
        {
            DrawString(s, Mafia.SCREEN_WIDTH / 2 - s.Length * 4 + offsetX, Mafia.SCREEN_HEIGHT / 2 - 4 + offsetY, color);
        }

        public void FillScreen(Color color)
        {
            sprite.Draw2D(texture, new Rectangle(0, 0, 16, 16), new Size(Mafia.SCREEN_WIDTH, Mafia.SCREEN_HEIGHT), new Point(0, 0), color);
        }

        public void DrawTitleScene(TitleScene title)
        {
            title.Draw(this);
        }

        public void DrawSelectScene(SelectScene select)
        {
            select.Draw(this);
        }

        public void DrawGameScene(GameScene game)
        {
            game.Draw(this);
        }

        public void DrawFps(int fps)
        {
            if (fps < 0) return;
            Point p = new Point(Mafia.SCREEN_WIDTH - 16, Mafia.SCREEN_HEIGHT - 16);
            int number = fps;
            int digit = 0;
            do
            {
                // Draw(textures.Number[number % 10], p - new Size(digit * 16, 0));
                number /= 10;
                digit++;
            }
            while (number > 0);
        }

        public void Dispose()
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
            if (offScreenSurface != null)
            {
                offScreenSurface.Dispose();
                offScreenSurface = null;
            }
            if (offScreenImage != null)
            {
                offScreenImage.Dispose();
                offScreenImage = null;
            }
            if (backBuffer != null)
            {
                backBuffer.Dispose();
                backBuffer = null;
            }
            if (sprite != null)
            {
                sprite.Dispose();
                sprite = null;
            }
            if (device != null)
            {
                device.DeviceLost -= deviceLost;
                device.DeviceReset -= deviceReset;
                device.DeviceResizing -= deviceResizing;
                device.Dispose();
                device = null;
            }
        }

        private static PresentParameters GetDefaultPresentParameters(bool fullscreen)
        {
            PresentParameters pp = new PresentParameters();
            pp.BackBufferFormat = Manager.Adapters[0].CurrentDisplayMode.Format;
            // pp.BackBufferWidth = Mafia.SCREEN_WIDTH;
            // pp.BackBufferHeight = Mafia.SCREEN_HEIGHT;
            pp.BackBufferWidth = Mafia.SCREEN_WIDTH * 2;
            pp.BackBufferHeight = Mafia.SCREEN_HEIGHT * 2;
            pp.PresentationInterval = PresentInterval.One;
            pp.SwapEffect = SwapEffect.Discard;
            pp.Windowed = !fullscreen;
            pp.MultiSample = MultiSampleType.None;
            return pp;
        }

        private void OnDeviceLost(object sender, EventArgs e)
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
            if (offScreenSurface != null)
            {
                offScreenSurface.Dispose();
                offScreenSurface = null;
            }
            if (offScreenImage != null)
            {
                offScreenImage.Dispose();
                offScreenImage = null;
            }
            if (backBuffer != null)
            {
                backBuffer.Dispose();
                backBuffer = null;
            }
            sprite.OnLostDevice();
        }

        private void OnDeviceReset(object sender, EventArgs e)
        {
            backBuffer = device.GetBackBuffer(0, 0, BackBufferType.Mono);
            offScreenImage = new Texture(device, Mafia.SCREEN_WIDTH, Mafia.SCREEN_HEIGHT, 1, Usage.RenderTarget, Manager.Adapters[0].CurrentDisplayMode.Format, Pool.Default);
            offScreenSurface = offScreenImage.GetSurfaceLevel(0);
            texture = texture = MafiaLoader.DefaultLoader.GetTexture(device, "mafia.bmp");
            sprite.OnResetDevice();
        }

        private void OnDeviceResizing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

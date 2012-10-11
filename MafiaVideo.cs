using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mafia
{
    /// <summary>
    /// ��ʕ\���̉]�X�������B
    /// </summary>
    /// <remarks>
    /// �t���X�N���[������Alt+Tab�������Ă������B
    /// Sprite��DeviceLost����Dispose�����DeviceReset���ɃG���[����������B
    /// �ł��Đ������Ȃ��Ă������Ɠ����Ă���ۂ��̂ŕ��u�B
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
                        throw new Exception("Direct3D�f�o�C�X�̐����Ɏ��s���܂����B");
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

            // �`�悪�d���Ƃ��Č��p�B
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
                    case '��':
                        row = 0; col = 0; break;
                    case '��':
                        row = 0; col = 1; break;
                    case '��':
                        row = 0; col = 2; break;
                    case '��':
                        row = 0; col = 3; break;
                    case '��':
                        row = 0; col = 4; break;
                    case '��':
                        row = 1; col = 0; break;
                    case '��':
                        row = 1; col = 1; break;
                    case '��':
                        row = 1; col = 2; break;
                    case '��':
                        row = 1; col = 3; break;
                    case '��':
                        row = 1; col = 4; break;
                    case '��':
                        ten = true; row = 1; col = 0; break;
                    case '��':
                        ten = true; row = 1; col = 1; break;
                    case '��':
                        ten = true; row = 1; col = 2; break;
                    case '��':
                        ten = true; row = 1; col = 3; break;
                    case '��':
                        ten = true; row = 1; col = 4; break;
                    case '��':
                        row = 2; col = 0; break;
                    case '��':
                        row = 2; col = 1; break;
                    case '��':
                        row = 2; col = 2; break;
                    case '��':
                        row = 2; col = 3; break;
                    case '��':
                        row = 2; col = 4; break;
                    case '��':
                        ten = true; row = 2; col = 0; break;
                    case '��':
                        ten = true; row = 2; col = 1; break;
                    case '��':
                        ten = true; row = 2; col = 2; break;
                    case '��':
                        ten = true; row = 2; col = 3; break;
                    case '��':
                        ten = true; row = 2; col = 4; break;
                    case '��':
                        row = 3; col = 0; break;
                    case '��':
                        row = 3; col = 1; break;
                    case '��':
                        row = 3; col = 2; break;
                    case '��':
                        row = 11; col = 2; break;
                    case '��':
                        row = 3; col = 3; break;
                    case '��':
                        row = 3; col = 4; break;
                    case '��':
                        ten = true; row = 3; col = 0; break;
                    case '��':
                        ten = true; row = 3; col = 1; break;
                    case '��':
                        ten = true; row = 3; col = 2; break;
                    case '��':
                        ten = true; row = 3; col = 3; break;
                    case '��':
                        ten = true; row = 3; col = 4; break;
                    case '��':
                        row = 4; col = 0; break;
                    case '��':
                        row = 4; col = 1; break;
                    case '��':
                        row = 4; col = 2; break;
                    case '��':
                        row = 4; col = 3; break;
                    case '��':
                        row = 4; col = 4; break;
                    case '��':
                        row = 5; col = 0; break;
                    case '��':
                        row = 5; col = 1; break;
                    case '��':
                        row = 5; col = 2; break;
                    case '��':
                        row = 5; col = 3; break;
                    case '��':
                        row = 5; col = 4; break;
                    case '��':
                        ten = true; row = 5; col = 0; break;
                    case '��':
                        ten = true; row = 5; col = 1; break;
                    case '��':
                        ten = true; row = 5; col = 2; break;
                    case '��':
                        ten = true; row = 5; col = 3; break;
                    case '��':
                        ten = true; row = 5; col = 4; break;
                    case '��':
                        maru = true; row = 5; col = 0; break;
                    case '��':
                        maru = true; row = 5; col = 1; break;
                    case '��':
                        maru = true; row = 5; col = 2; break;
                    case '��':
                        maru = true; row = 5; col = 3; break;
                    case '��':
                        maru = true; row = 5; col = 4; break;
                    case '��':
                        row = 6; col = 0; break;
                    case '��':
                        row = 6; col = 1; break;
                    case '��':
                        row = 6; col = 2; break;
                    case '��':
                        row = 6; col = 3; break;
                    case '��':
                        row = 6; col = 4; break;
                    case '��':
                        row = 7; col = 0; break;
                    case '��':
                        row = 7; col = 2; break;
                    case '��':
                        row = 7; col = 4; break;
                    case '��':
                        row = 12; col = 0; break;
                    case '��':
                        row = 12; col = 2; break;
                    case '��':
                        row = 12; col = 4; break;
                    case '��':
                        row = 8; col = 0; break;
                    case '��':
                        row = 8; col = 1; break;
                    case '��':
                        row = 8; col = 2; break;
                    case '��':
                        row = 8; col = 3; break;
                    case '��':
                        row = 8; col = 4; break;
                    case '��':
                        row = 9; col = 0; break;
                    case '��':
                        row = 9; col = 4; break;
                    case '��':
                        row = 10; col = 0; break;
                    case '�A':
                        katakana = true; row = 0; col = 0; break;
                    case '�C':
                        katakana = true; row = 0; col = 1; break;
                    case '�E':
                        katakana = true; row = 0; col = 2; break;
                    case '�G':
                        katakana = true; row = 0; col = 3; break;
                    case '�I':
                        katakana = true; row = 0; col = 4; break;
                    case '�J':
                        katakana = true; row = 1; col = 0; break;
                    case '�L':
                        katakana = true; row = 1; col = 1; break;
                    case '�N':
                        katakana = true; row = 1; col = 2; break;
                    case '�P':
                        katakana = true; row = 1; col = 3; break;
                    case '�R':
                        katakana = true; row = 1; col = 4; break;
                    case '�K':
                        katakana = true; ten = true; row = 1; col = 0; break;
                    case '�M':
                        katakana = true; ten = true; row = 1; col = 1; break;
                    case '�O':
                        katakana = true; ten = true; row = 1; col = 2; break;
                    case '�Q':
                        katakana = true; ten = true; row = 1; col = 3; break;
                    case '�S':
                        katakana = true; ten = true; row = 1; col = 4; break;
                    case '�T':
                        katakana = true; row = 2; col = 0; break;
                    case '�V':
                        katakana = true; row = 2; col = 1; break;
                    case '�X':
                        katakana = true; row = 2; col = 2; break;
                    case '�Z':
                        katakana = true; row = 2; col = 3; break;
                    case '�\':
                        katakana = true; row = 2; col = 4; break;
                    case '�U':
                        katakana = true; ten = true; row = 2; col = 0; break;
                    case '�W':
                        katakana = true; ten = true; row = 2; col = 1; break;
                    case '�Y':
                        katakana = true; ten = true; row = 2; col = 2; break;
                    case '�[':
                        katakana = true; ten = true; row = 2; col = 3; break;
                    case '�]':
                        katakana = true; ten = true; row = 2; col = 4; break;
                    case '�^':
                        katakana = true; row = 3; col = 0; break;
                    case '�`':
                        katakana = true; row = 3; col = 1; break;
                    case '�c':
                        katakana = true; row = 3; col = 2; break;
                    case '�b':
                        katakana = true; row = 11; col = 2; break;
                    case '�e':
                        katakana = true; row = 3; col = 3; break;
                    case '�g':
                        katakana = true; row = 3; col = 4; break;
                    case '�_':
                        katakana = true; ten = true; row = 3; col = 0; break;
                    case '�a':
                        katakana = true; ten = true; row = 3; col = 1; break;
                    case '�d':
                        katakana = true; ten = true; row = 3; col = 2; break;
                    case '�f':
                        katakana = true; ten = true; row = 3; col = 3; break;
                    case '�h':
                        katakana = true; ten = true; row = 3; col = 4; break;
                    case '�i':
                        katakana = true; row = 4; col = 0; break;
                    case '�j':
                        katakana = true; row = 4; col = 1; break;
                    case '�k':
                        katakana = true; row = 4; col = 2; break;
                    case '�l':
                        katakana = true; row = 4; col = 3; break;
                    case '�m':
                        katakana = true; row = 4; col = 4; break;
                    case '�n':
                        katakana = true; row = 5; col = 0; break;
                    case '�q':
                        katakana = true; row = 5; col = 1; break;
                    case '�t':
                        katakana = true; row = 5; col = 2; break;
                    case '�w':
                        katakana = true; row = 5; col = 3; break;
                    case '�z':
                        katakana = true; row = 5; col = 4; break;
                    case '�o':
                        katakana = true; ten = true; row = 5; col = 0; break;
                    case '�r':
                        katakana = true; ten = true; row = 5; col = 1; break;
                    case '�u':
                        katakana = true; ten = true; row = 5; col = 2; break;
                    case '�x':
                        katakana = true; ten = true; row = 5; col = 3; break;
                    case '�{':
                        katakana = true; ten = true; row = 5; col = 4; break;
                    case '�p':
                        katakana = true; maru = true; row = 5; col = 0; break;
                    case '�s':
                        katakana = true; maru = true; row = 5; col = 1; break;
                    case '�v':
                        katakana = true; maru = true; row = 5; col = 2; break;
                    case '�y':
                        katakana = true; maru = true; row = 5; col = 3; break;
                    case '�|':
                        katakana = true; maru = true; row = 5; col = 4; break;
                    case '�}':
                        katakana = true; row = 6; col = 0; break;
                    case '�~':
                        katakana = true; row = 6; col = 1; break;
                    case '��':
                        katakana = true; row = 6; col = 2; break;
                    case '��':
                        katakana = true; row = 6; col = 3; break;
                    case '��':
                        katakana = true; row = 6; col = 4; break;
                    case '��':
                        katakana = true; row = 7; col = 0; break;
                    case '��':
                        katakana = true; row = 7; col = 2; break;
                    case '��':
                        katakana = true; row = 7; col = 4; break;
                    case '��':
                        katakana = true; row = 12; col = 0; break;
                    case '��':
                        katakana = true; row = 12; col = 2; break;
                    case '��':
                        katakana = true; row = 12; col = 4; break;
                    case '��':
                        katakana = true; row = 8; col = 0; break;
                    case '��':
                        katakana = true; row = 8; col = 1; break;
                    case '��':
                        katakana = true; row = 8; col = 2; break;
                    case '��':
                        katakana = true; row = 8; col = 3; break;
                    case '��':
                        katakana = true; row = 8; col = 4; break;
                    case '��':
                        katakana = true; row = 9; col = 0; break;
                    case '��':
                        katakana = true; row = 9; col = 4; break;
                    case '��':
                        katakana = true; row = 10; col = 0; break;
                    case '�[':
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

using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Windows.Forms;

namespace Mafia
{
    /// <summary>
    /// プレイヤーからの入力をうねうねする。
    /// </summary>
    public class MafiaInput : IDisposable
    {
        Form form;
        Device device;

        private bool left;
        private bool up;
        private bool right;
        private bool down;
        private bool enter;
        private bool space;
        private bool alt;
        private bool esc;

        private bool prevLeft;
        private bool prevUp;
        private bool prevRight;
        private bool prevDown;
        private bool prevEnter;
        private bool prevSpace;
        private bool prevAlt;
        private bool prevEsc;

        public MafiaInput(Form form)
        {
            this.form = form;
            device = new Device(SystemGuid.Keyboard);
            device.Acquire();
            prevLeft = prevUp = prevRight = prevDown = false;
            prevEnter = prevSpace = false;
            prevAlt = false;
            prevEsc = false;
        }

        public TitleInput GetCurrentTitleInput()
        {
            if (form.Focused)
            {
                UpdateKeys();
                return new TitleInput(up, down, space && !prevSpace, esc && !prevEsc);
            }
            return TitleInput.Empty;

        }

        public SelectInput GetCurrentSelectInput()
        {
            if (form.Focused)
            {
                UpdateKeys();
                return new SelectInput(left && !prevLeft, up && !prevUp, right && !prevRight, down && !prevDown, space && !prevSpace, esc && !prevEsc);
            }
            return SelectInput.Empty;
        }

        public GameInput GetCurrentGameInput()
        {
            if (form.Focused)
            {
                UpdateKeys();
                return new GameInput(left, up, right, down, space && !prevSpace, esc && !prevEsc);
            }
            return GameInput.Empty;
        }

        public bool ShouldToggleFullscreen()
        {
            return alt && enter && !prevEnter;
        }

        public void Dispose()
        {
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }

        private void UpdateKeys()
        {
            prevLeft = left;
            prevUp = up;
            prevRight = right;
            prevDown = down;
            prevEnter = enter;
            prevSpace = space;
            prevAlt = alt;
            prevEsc = esc;
            KeyboardState state = device.GetCurrentKeyboardState();
            left = state[Key.Left];
            up = state[Key.Up] || state[Key.A] || state[Key.S] || state[Key.Z] || state[Key.LeftShift] || state[Key.LeftControl];
            right = state[Key.Right];
            down = state[Key.Down];
            enter = state[Key.Return];
            space = state[Key.Space];
            alt = state[Key.LeftAlt] || state[Key.RightAlt];
            esc = state[Key.Escape];
        }
    }
}

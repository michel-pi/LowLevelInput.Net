using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using LowLevelInput.PInvoke;

namespace LowLevelInput.Hooks
{
    public class KeyboardHook : IDisposable
    {
        private readonly object _lock;

        private volatile bool _clearInjectedFlag;
        private volatile WindowsHook _hook;

        public bool IsInstalled => _hook == null ? false : _hook.IsInstalled;
        public bool ClearInjectedFlag { get => _clearInjectedFlag; set => _clearInjectedFlag = value; }

        public WindowsHook Hook { get => _hook; private set => _hook = value; }

        public event EventHandler<KeyboardHookEventArgs> KeyboardEvent;
        public event EventHandler<KeyboardHookEventArgs> KeyboardEventAsync;

        public KeyboardFilterCallback KeyboardFilter;

        public KeyboardHook()
        {
            _lock = new object();
        }

        public KeyboardHook(bool clearInjectedFlag) : this()
        {
            ClearInjectedFlag = clearInjectedFlag;
        }

        ~KeyboardHook()
        {
            Dispose(false);
        }

        public void Install()
        {
            lock (_lock)
            {
                if (IsInstalled) throw new InvalidOperationException("The " + nameof(KeyboardHook) + " is already installed.");

                if (_hook == null)
                {
                    _hook = new WindowsHook(WindowsHookType.LowLevelKeyboard);
                }

                _hook.HookCalled += _hook_HookCalled;

                _hook.HookFilter = _hook_filterCallback;

                _hook.Install();
            }
        }

        public void Uninstall()
        {
            lock (_lock)
            {
                if (!IsInstalled) throw new InvalidOperationException("The " + nameof(KeyboardHook) + " needs to be installed before you can uninstall it.");

                _hook.HookCalled -= _hook_HookCalled;

                _hook.HookFilter = null;

                _hook.Uninstall();
            }
        }
        
        protected virtual void OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            KeyboardEvent?.Invoke(this, new KeyboardHookEventArgs(key, state));
        }

        protected virtual void OnKeyboardEventAsync(VirtualKeyCode key, KeyState state)
        {
            Task.Factory.StartNew(() =>
            {
                KeyboardEventAsync?.Invoke(this, new KeyboardHookEventArgs(key, state));
            });
        }

        protected virtual bool OnKeyboardFilter(VirtualKeyCode key, KeyState state)
        {
            bool? filter = KeyboardFilter?.Invoke(key, state);

            bool result = filter == null ? false : (bool)filter;

            return result;
        }

        private void _hook_HookCalled(object sender, HookCalledEventArgs e)
        {
            if (e.WParam == IntPtr.Zero || e.LParam == IntPtr.Zero) return;

            if (ClearInjectedFlag)
            {
                ClearInjectedFlagHelper(e.LParam);
            }

            var msg = (WindowMessage)e.WParam.ToInt32();
            var key = (VirtualKeyCode)Marshal.ReadInt32(e.LParam);

            switch (msg)
            {
                case WindowMessage.Keydown:
                case WindowMessage.ImeKeydown:
                case WindowMessage.Syskeydown:
                    OnKeyboardEventAsync(key, KeyState.Down);
                    OnKeyboardEvent(key, KeyState.Down);
                    break;
                case WindowMessage.Keyup:
                case WindowMessage.ImeKeyup:
                case WindowMessage.Syskeyup:
                    OnKeyboardEventAsync(key, KeyState.Up);
                    OnKeyboardEvent(key, KeyState.Up);
                    break;
            }
        }

        private bool _hook_filterCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (wParam == IntPtr.Zero || lParam == IntPtr.Zero) return false;

            var msg = (WindowMessage)wParam.ToInt32();
            var key = (VirtualKeyCode)Marshal.ReadInt32(lParam);

            bool isKeyDown = false;

            switch (msg)
            {
                case WindowMessage.Keydown:
                case WindowMessage.ImeKeydown:
                case WindowMessage.Syskeydown:
                    isKeyDown = true;
                    break;
            }

            return OnKeyboardFilter(key, isKeyDown ? KeyState.Down : KeyState.Up);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (_hook != null)
                {
                    _hook.Dispose();
                }
                
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private static void ClearInjectedFlagHelper(IntPtr lParam)
        {
            if (lParam == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(lParam));

            int flags = Marshal.ReadInt32(lParam + 8);

            flags = SetBit(flags, 1, false);
            flags = SetBit(flags, 4, false);

            Marshal.WriteInt32(lParam + 8, flags);
        }

        private static int SetBit(int num, int index, bool value)
        {
            if (value)
            {
                return num | (1 << index);
            }
            else
            {
                return num & ~(1 << index);
            }
        }
    }
}

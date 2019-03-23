using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using LowLevelInput.PInvoke;

namespace LowLevelInput.Hooks
{
    public class MouseHook : IDisposable
    {
        private readonly object _lock;

        private volatile bool _clearInjectedFlag;
        private volatile WindowsHook _hook;

        public bool IsInstalled => _hook == null ? false : _hook.IsInstalled;
        public bool ClearInjectedFlag { get => _clearInjectedFlag; set => _clearInjectedFlag = value; }

        public WindowsHook Hook { get => _hook; private set => _hook = value; }

        public event EventHandler<KeyboardHookEventArgs> MouseEvent;
        public event EventHandler<KeyboardHookEventArgs> MouseEventAsync;

        public KeyboardFilterCallback MouseFilter;

        public MouseHook()
        {
            _lock = new object();
        }

        public MouseHook(bool clearInjectedFlag) : this()
        {
            ClearInjectedFlag = clearInjectedFlag;
        }

        ~MouseHook()
        {
            Dispose(false);
        }

        public void Install()
        {
            lock (_lock)
            {
                if (IsInstalled) throw new InvalidOperationException("The " + nameof(MouseHook) + " is already installed.");

                if (_hook == null)
                {
                    _hook = new WindowsHook(WindowsHookType.LowLevelMouse);
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
                if (!IsInstalled) throw new InvalidOperationException("The " + nameof(MouseHook) + " needs to be installed before you can uninstall it.");

                _hook.HookCalled -= _hook_HookCalled;

                _hook.HookFilter = null;

                _hook.Uninstall();
            }
        }

        protected virtual void OnMouseEvent(VirtualKeyCode key, KeyState state)
        {
            MouseEvent?.Invoke(this, new KeyboardHookEventArgs(key, state));
        }

        protected virtual void OnMouseEventAsync(VirtualKeyCode key, KeyState state)
        {
            Task.Factory.StartNew(() =>
            {
                MouseEventAsync?.Invoke(this, new KeyboardHookEventArgs(key, state));
            });
        }

        protected virtual bool OnMouseFilter(VirtualKeyCode key, KeyState state)
        {
            bool? filter = MouseFilter?.Invoke(key, state);

            bool result = filter == null ? false : (bool)filter;

            return result;
        }

        private void _hook_HookCalled(object sender, HookCalledEventArgs e)
        {
            if (e.WParam == IntPtr.Zero || e.LParam == IntPtr.Zero) return;
            
        }

        private bool _hook_filterCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            return false;
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
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LowLevelInput.Converters;
using LowLevelInput.PInvoke;

namespace LowLevelInput.Hooks
{
    public class MouseHook : IDisposable
    {
        private readonly object _lock;
        
        private volatile WindowsHook _hook;

        private volatile bool _captureMouseMove;
        private volatile bool _clearInjectedFlag;

        private volatile int _mouseX;
        private volatile int _mouseY;

        private Dictionary<VirtualKeyCode, KeyState> _buttonStates;
        private Dictionary<VirtualKeyCode, List<EventHandler<MouseHookEventArgs>>> _registeredMouseEvents;
        
        public bool IsInstalled => _hook == null ? false : _hook.IsInstalled;

        public bool CaptureMouseMove { get => _captureMouseMove; set => _captureMouseMove = value; }
        public bool ClearInjectedFlag { get => _clearInjectedFlag; set => _clearInjectedFlag = value; }

        public int MouseX => _mouseX;
        public int MouseY => _mouseY;

        public WindowsHook Hook { get => _hook; private set => _hook = value; }

        public event EventHandler<MouseHookEventArgs> MouseEvent;
        public event EventHandler<MouseHookEventArgs> MouseEventAsync;

        public MouseFilterCallback MouseFilter;

        public MouseHook(bool captureMouseMove = true)
        {
            _lock = new object();

            _buttonStates = new Dictionary<VirtualKeyCode, KeyState>()
            {
                { VirtualKeyCode.Lbutton, KeyState.Up },
                { VirtualKeyCode.Rbutton, KeyState.Up },
                { VirtualKeyCode.Mbutton, KeyState.Up },
                { VirtualKeyCode.Xbutton1, KeyState.Up },
                { VirtualKeyCode.Xbutton2, KeyState.Up }
            };

            _registeredMouseEvents = new Dictionary<VirtualKeyCode, List<EventHandler<MouseHookEventArgs>>>();

            CaptureMouseMove = captureMouseMove;
        }

        public MouseHook(bool captureMouseMove, bool clearInjectedFlag = false) : this(captureMouseMove)
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

        protected virtual void OnMouseEvent(MouseHookEventArgs mouse)
        {
            MouseEvent?.Invoke(this, mouse);
        }

        protected virtual void OnMouseEventAsync(MouseHookEventArgs mouse)
        {
            Task.Factory.StartNew(() =>
            {
                MouseEventAsync?.Invoke(this, mouse);
            });
        }

        protected virtual bool OnMouseFilter(MouseHookEventArgs mouse)
        {
            bool? filter = MouseFilter?.Invoke(this, mouse);

            return filter == null ? false : (bool)filter;
        }

        protected virtual void OnFireRegisteredEvents(MouseHookEventArgs mouse)
        {
            Task.Factory.StartNew(() =>
            {
                List<EventHandler<MouseHookEventArgs>> list;

                lock (_lock)
                {
                    if (!_registeredMouseEvents.ContainsKey(mouse.Button)) return;

                    list = _registeredMouseEvents[mouse.Button];
                }

                if (list == null || list.Count == 0) return;

                try
                {
                    foreach (var eventHandler in list)
                    {
                        eventHandler?.Invoke(this, mouse);
                    }
                }
                catch
                {
                    return;
                }
            });
        }
        
        private void _hook_HookCalled(object sender, HookCalledEventArgs e)
        {
            if (e.WParam == IntPtr.Zero || e.LParam == IntPtr.Zero) return;
            
        }

        private bool _hook_filterCallback(object sender, int code, IntPtr wParam, IntPtr lParam)
        {
            if (wParam == IntPtr.Zero || lParam == IntPtr.Zero) return false;

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

        private static void ClearInjectedFlagHelper(IntPtr lParam)
        {
            if (lParam == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(lParam));

            Marshal.WriteInt32(lParam, 12, 0);
        }

        private static ushort HIWORD(int number)
        {
            return (ushort)(((uint)number >> 16) & 0xFFFF);
        }
    }
}

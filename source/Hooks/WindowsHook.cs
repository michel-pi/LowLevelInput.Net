using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using LowLevelInput.Converters;
using LowLevelInput.PInvoke;

namespace LowLevelInput.Hooks
{
    public class WindowsHook : IDisposable
    {
        private const WindowMessage CustomDestroyWindowMessage = (WindowMessage)0x1337;

        private static readonly IntPtr MainModuleHandle = Process.GetCurrentProcess().MainModule.BaseAddress;

        private readonly object _lock;

        private volatile Exception _exception;

        private volatile IntPtr _hookHandle;
        
        private volatile HookProc _hookProc;
        private volatile IntPtr _hookProcAddress;

        private volatile Thread _thread;
        private volatile uint _threadId;

        private volatile bool _isInstalled;

        public bool IsInstalled { get => _isInstalled; private set => _isInstalled = value; }

        public WindowsHookType HookType { get; private set; }
        
        public event EventHandler<HookCalledEventArgs> HookCalled;

        // return if this call should be filtered
        public HookFilterCallback HookFilter;

        private WindowsHook()
        {
            _lock = new object();
            _exception = null;

            UnhookGuard.UnhookGuardEvent += UnhookGuard_UnhookGuardEvent;
        }
        
        public WindowsHook(WindowsHookType type) : this()
        {
            HookType = type;
        }

        ~WindowsHook()
        {
            Dispose(false);
        }

        public void Install()
        {
            lock (_lock)
            {
                if (IsInstalled) throw new InvalidOperationException("The " + nameof(WindowsHook) + " is already installed.");

                _thread = new Thread(WindowsHookThread)
                {
                    IsBackground = false
                };
                
                _thread.Start();

                while (!IsInstalled)
                {
                    Thread.Sleep(10);

                    if (_exception != null)
                    {
                        var tmp = _exception;

                        _exception = null;

                        throw tmp;
                    }
                }
            }
        }

        public void Uninstall()
        {
            lock (_lock)
            {
                if (!IsInstalled) throw new InvalidOperationException("Please install the " + nameof(WindowsHook) + " before calling this method.");

                _exception = null;

                User32.PostThreadMessage(_threadId, CustomDestroyWindowMessage, IntPtr.Zero, IntPtr.Zero);

                while (IsInstalled)
                {
                    Thread.Sleep(10);

                    if (_exception != null)
                    {
                        var tmp = _exception;

                        _exception = null;

                        throw tmp;
                    }
                }
            }
        }
        
        public override bool Equals(object obj)
        {
            if (obj is WindowsHook value)
            {
                return value._isInstalled == _isInstalled
                    && value._hookHandle == _hookHandle
                    && value._hookProcAddress == _hookProcAddress
                    && value._threadId == _threadId;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(WindowsHook value)
        {
            return value != null
                && value._isInstalled == _isInstalled
                && value._hookHandle == _hookHandle
                && value._hookProcAddress == _hookProcAddress
                && value._threadId == _threadId;
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                _isInstalled.GetHashCode(),
                _hookHandle.GetHashCode(),
                _hookProc.GetHashCode(),
                _threadId.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "IsInstalled", IsInstalled.ToString(),
                "WindowsHookType", HookTypeConverter.ToString(HookType));
        }

        protected virtual void OnHookCalled(int code, IntPtr wParam, IntPtr lParam)
        {
            HookCalled?.Invoke(this, new HookCalledEventArgs(code, wParam, lParam));
        }

        protected virtual bool OnHookFilter(int code, IntPtr wParam, IntPtr lParam)
        {
            bool? filter = HookFilter?.Invoke(this, new HookCalledEventArgs(code, wParam, lParam));

            bool result = filter == null ? false : (bool)filter;

            return result;
        }
        
        private void WindowsHookThread()
        {
            Thread.BeginThreadAffinity();
            
            _threadId = Kernel32.GetCurrentThreadId();

            _hookProc = WindowsHookProc;
            _hookProcAddress = Marshal.GetFunctionPointerForDelegate(_hookProc);

            _hookHandle = User32.SetWindowsHookEx((int)HookType, _hookProcAddress, MainModuleHandle, 0);

            if (_hookHandle == IntPtr.Zero)
            {
                _exception = new Exception("Failed to install " + nameof(WindowsHook) + ".");

                return;
            }

            IsInstalled = true;

            var msg = default(Message);

            while (true)
            {
                var result = User32.GetMessage(ref msg, IntPtr.Zero, 0, 0);
                
                if (result == -1)
                {
                    break;
                }
                else if (result != 0)
                {
                    if (msg.WindowMessage == CustomDestroyWindowMessage)
                    {
                        break;
                    }

                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);

                    if (msg.WindowMessage == WindowMessage.Quit)
                    {
                        break;
                    }
                }
            }

            User32.UnhookWindowsHookEx(_hookHandle);

            IsInstalled = false;

            Thread.EndThreadAffinity();
        }

        [AllowReversePInvokeCalls]
        private IntPtr WindowsHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            // contains a message
            if (code >= 0)
            {
                // some hooks may not have a message if the code is not 0
                if (OnHookFilter(code, wParam, lParam))
                {
                    return (IntPtr)(-1);
                }
                else
                {
                    OnHookCalled(code, wParam, lParam);
                }
            }
            
            return User32.CallNextHookEx(_hookHandle, code, wParam, lParam);
        }

        private void UnhookGuard_UnhookGuardEvent(object sender, UnhookGuardEventArgs e)
        {
            Dispose();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                UnhookGuard.UnhookGuardEvent -= UnhookGuard_UnhookGuardEvent;

                if (IsInstalled)
                {
                    Uninstall();
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

        public static bool Equals(WindowsHook left, WindowsHook right)
        {
            return left != null
                && left.Equals(right);
        }
    }
}

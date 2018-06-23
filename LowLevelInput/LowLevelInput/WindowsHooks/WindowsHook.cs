using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

using LowLevelInput.PInvoke.Types;
using LowLevelInput.PInvoke.Libraries;

namespace LowLevelInput.WindowsHooks
{
    /// <summary>
    /// An generic class to install WindowsHooks
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public class WindowsHook : IDisposable
    {
        private static IntPtr MainModuleHandle = Process.GetCurrentProcess().MainModule.BaseAddress;

        private IntPtr hookHandle;
        private User32.HookProc hookProc;
        private Thread hookThread;
        private uint hookThreadId;
        private object lockObject;

        private WindowsHook()
        {
            lockObject = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsHook"/> class.
        /// </summary>
        /// <param name="windowsHookType">Type of the windows hook.</param>
        public WindowsHook(WindowsHookType windowsHookType)
        {
            lockObject = new object();
            WindowsHookType = windowsHookType;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="WindowsHook"/> class.
        /// </summary>
        ~WindowsHook()
        {
            Dispose(false);
        }

        /// <summary>
        /// </summary>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        public delegate void HookCallback(IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Occurs when [on hook called].
        /// </summary>
        public event HookCallback OnHookCalled;

        /// <summary>
        /// Gets the type of the windows hook.
        /// </summary>
        /// <value>The type of the windows hook.</value>
        public WindowsHookType WindowsHookType { get; private set; }

        private IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == 0)
            {
                OnHookCalled?.Invoke(wParam, lParam);
            }

            return User32.CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        private void InitializeHookThread()
        {
            lock (lockObject)
            {
                hookThreadId = Kernel32.GetCurrentThreadId();

                hookProc = new User32.HookProc(HookProcedure);

                IntPtr methodPtr = Marshal.GetFunctionPointerForDelegate(hookProc);

                hookHandle = User32.SetWindowsHookEx((int)WindowsHookType, methodPtr, MainModuleHandle, 0);
            }

            Message msg = new Message();

            while (User32.GetMessage(ref msg, IntPtr.Zero, 0, 0) != 0)
            {
                if (msg.Msg == (uint)WindowsMessage.WM_QUIT) break;
            }

            User32.UnhookWindowsHookEx(hookHandle);
        }

        /// <summary>
        /// Installs the hook.
        /// </summary>
        /// <returns></returns>
        public bool InstallHook()
        {
            lock (lockObject)
            {
                if (hookHandle != IntPtr.Zero) return false;
                if (hookThreadId != 0) return false;

                hookThread = new Thread(InitializeHookThread)
                {
                    IsBackground = true
                };

                hookThread.Start();

                return true;
            }
        }

        /// <summary>
        /// Uninstalls the hook.
        /// </summary>
        /// <returns></returns>
        public bool UninstallHook()
        {
            lock (lockObject)
            {
                if (hookHandle == IntPtr.Zero) return false;
                if (hookThreadId == 0) return false;

                if (User32.PostThreadMessage(hookThreadId, (uint)WindowsMessage.WM_QUIT, IntPtr.Zero, IntPtr.Zero) != 0)
                {
                    try
                    {
                        hookThread.Join();
                    }
                    catch
                    {
                    }
                }

                hookHandle = IntPtr.Zero;
                hookThreadId = 0;
                hookThread = null;

                return true;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnHookCalled = null;
                }

                UninstallHook();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
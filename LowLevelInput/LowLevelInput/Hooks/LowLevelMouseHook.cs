using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using LowLevelInput.PInvoke.Types;
using LowLevelInput.WindowsHooks;

namespace LowLevelInput.Hooks
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public class LowLevelMouseHook : IDisposable
    {
        private WindowsHook hook;
        private object lockObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="LowLevelMouseHook"/> class.
        /// </summary>
        public LowLevelMouseHook()
        {
            lockObject = new object();
            CaptureMouseMove = false;
            ClearInjectedFlag = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LowLevelMouseHook"/> class.
        /// </summary>
        /// <param name="captureMouseMove">if set to <c>true</c> [capture mouse move].</param>
        public LowLevelMouseHook(bool captureMouseMove)
        {
            lockObject = new object();
            CaptureMouseMove = captureMouseMove;
            ClearInjectedFlag = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LowLevelMouseHook"/> class.
        /// </summary>
        /// <param name="captureMouseMove">if set to <c>true</c> [capture mouse move].</param>
        /// <param name="clearInjectedFlag">if set to <c>true</c> [clear injected flag].</param>
        public LowLevelMouseHook(bool captureMouseMove, bool clearInjectedFlag)
        {
            lockObject = new object();
            CaptureMouseMove = captureMouseMove;
            ClearInjectedFlag = clearInjectedFlag;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LowLevelMouseHook"/> class.
        /// </summary>
        ~LowLevelMouseHook()
        {
            Dispose(false);
        }

        /// <summary>
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="key">The key.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public delegate void MouseEventCallback(KeyState state, VirtualKeyCode key, int x, int y);

        /// <summary>
        /// Occurs when [on mouse event].
        /// </summary>
        public event MouseEventCallback OnMouseEvent;

        /// <summary>
        /// Gets or sets a value indicating whether [capture mouse move].
        /// </summary>
        /// <value><c>true</c> if [capture mouse move]; otherwise, <c>false</c>.</value>
        public bool CaptureMouseMove { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [clear injected flag].
        /// </summary>
        /// <value><c>true</c> if [clear injected flag]; otherwise, <c>false</c>.</value>
        public bool ClearInjectedFlag { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is left mouse button pressed.
        /// </summary>
        /// <value><c>true</c> if this instance is left mouse button pressed; otherwise, <c>false</c>.</value>
        public bool IsLeftMouseButtonPressed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is middle mouse button pressed.
        /// </summary>
        /// <value><c>true</c> if this instance is middle mouse button pressed; otherwise, <c>false</c>.</value>
        public bool IsMiddleMouseButtonPressed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is right mouse button pressed.
        /// </summary>
        /// <value><c>true</c> if this instance is right mouse button pressed; otherwise, <c>false</c>.</value>
        public bool IsRightMouseButtonPressed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is x button1 pressed.
        /// </summary>
        /// <value><c>true</c> if this instance is x button1 pressed; otherwise, <c>false</c>.</value>
        public bool IsXButton1Pressed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is x button2 pressed.
        /// </summary>
        /// <value><c>true</c> if this instance is x button2 pressed; otherwise, <c>false</c>.</value>
        public bool IsXButton2Pressed { get; private set; }

        private void Global_OnProcessExit()
        {
            Dispose();
        }

        private void Global_OnUnhandledException()
        {
            Dispose();
        }

        private int HIWORD(int number)
        {
            return (int)BitConverter.ToInt16(BitConverter.GetBytes(number), 2);
        }

        private void Hook_OnHookCalled(IntPtr wParam, IntPtr lParam)
        {
            if (lParam == IntPtr.Zero) return;

            IsMiddleMouseButtonPressed = false; // important to reset here

            WindowsMessage msg = (WindowsMessage)((uint)wParam.ToInt32());

            int x = Marshal.ReadInt32(lParam);
            int y = Marshal.ReadInt32(lParam + 4);

            int mouseData = Marshal.ReadInt32(lParam + 8);

            if (ClearInjectedFlag)
            {
                Marshal.WriteInt32(lParam + 12, 0);
            }

            switch (msg)
            {
                case WindowsMessage.WM_LBUTTONDBLCLK:
                case WindowsMessage.WM_NCLBUTTONDBLCLK:
                    IsLeftMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.LBUTTON, x, y);

                    IsLeftMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.LBUTTON, x, y);
                    break;

                case WindowsMessage.WM_LBUTTONDOWN:
                case WindowsMessage.WM_NCLBUTTONDOWN:
                    IsLeftMouseButtonPressed = true;
                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.LBUTTON, x, y);
                    break;

                case WindowsMessage.WM_LBUTTONUP:
                case WindowsMessage.WM_NCLBUTTONUP:
                    IsLeftMouseButtonPressed = false;
                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.LBUTTON, x, y);
                    break;

                case WindowsMessage.WM_MBUTTONDOWN:
                case WindowsMessage.WM_NCMBUTTONDOWN:
                    IsMiddleMouseButtonPressed = true;
                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.MBUTTON, x, y);
                    break;

                case WindowsMessage.WM_MBUTTONUP:
                case WindowsMessage.WM_NCMBUTTONUP:
                    IsMiddleMouseButtonPressed = false;
                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.MBUTTON, x, y);
                    break;

                case WindowsMessage.WM_MBUTTONDBLCLK:
                case WindowsMessage.WM_NCMBUTTONDBLCLK:
                    IsMiddleMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.MBUTTON, x, y);

                    IsMiddleMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.MBUTTON, x, y);
                    break;

                case WindowsMessage.WM_RBUTTONDBLCLK:
                case WindowsMessage.WM_NCRBUTTONDBLCLK:
                    IsRightMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.RBUTTON, x, y);

                    IsRightMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.RBUTTON, x, y);
                    break;

                case WindowsMessage.WM_RBUTTONDOWN:
                case WindowsMessage.WM_NCRBUTTONDOWN:
                    IsRightMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.RBUTTON, x, y);
                    break;

                case WindowsMessage.WM_RBUTTONUP:
                case WindowsMessage.WM_NCRBUTTONUP:
                    IsRightMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.RBUTTON, x, y);
                    break;

                case WindowsMessage.WM_XBUTTONDBLCLK:
                case WindowsMessage.WM_NCXBUTTONDBLCLK:
                    if (HIWORD(mouseData) == 0x1)
                    {
                        IsXButton1Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.XBUTTON1, x, y);

                        IsXButton1Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.XBUTTON1, x, y);
                    }
                    else
                    {
                        IsXButton2Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.XBUTTON2, x, y);

                        IsXButton2Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.XBUTTON2, x, y);
                    }
                    break;

                case WindowsMessage.WM_XBUTTONDOWN:
                case WindowsMessage.WM_NCXBUTTONDOWN:
                    if (HIWORD(mouseData) == 0x1)
                    {
                        IsXButton1Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.XBUTTON1, x, y);
                    }
                    else
                    {
                        IsXButton2Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.XBUTTON2, x, y);
                    }
                    break;

                case WindowsMessage.WM_XBUTTONUP:
                case WindowsMessage.WM_NCXBUTTONUP:
                    if (HIWORD(mouseData) == 0x1)
                    {
                        IsXButton1Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.XBUTTON1, x, y);
                    }
                    else
                    {
                        IsXButton2Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.XBUTTON2, x, y);
                    }
                    break;

                case WindowsMessage.WM_MOUSEWHEEL:
                case WindowsMessage.WM_MOUSEHWHEEL:
                    InvokeEventListeners(KeyState.None, VirtualKeyCode.SCROLL, HIWORD(mouseData), HIWORD(mouseData));

                    break;

                case WindowsMessage.WM_MOUSEMOVE:
                case WindowsMessage.WM_NCMOUSEMOVE:
                    if (CaptureMouseMove)
                    {
                        InvokeEventListeners(KeyState.None, VirtualKeyCode.INVALID, x, y);
                    }
                    break;
            }
        }

        private void InvokeEventListeners(KeyState state, VirtualKeyCode key, int x, int y)
        {
            Task.Factory.StartNew(() =>
            {
                OnMouseEvent?.Invoke(state, key, x, y);
            });
        }

        /// <summary>
        /// Installs the hook.
        /// </summary>
        /// <returns></returns>
        public bool InstallHook()
        {
            lock (lockObject)
            {
                if (hook != null) return false;

                hook = new WindowsHook(WindowsHookType.LowLevelMouse);
            }

            hook.OnHookCalled += Hook_OnHookCalled;

            hook.InstallHook();

            Global.OnProcessExit += Global_OnProcessExit;
            Global.OnUnhandledException += Global_OnUnhandledException;

            return true;
        }

        /// <summary>
        /// Uninstalls the hook.
        /// </summary>
        /// <returns></returns>
        public bool UninstallHook()
        {
            lock (lockObject)
            {
                if (hook == null) return false;

                Global.OnProcessExit -= Global_OnProcessExit;
                Global.OnUnhandledException -= Global_OnUnhandledException;

                hook.OnHookCalled -= Hook_OnHookCalled;

                hook.UninstallHook();

                hook.Dispose();

                hook = null;

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
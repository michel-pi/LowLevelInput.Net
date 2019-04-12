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

        private readonly Dictionary<VirtualKeyCode, KeyState> _buttonStates;
        private readonly Dictionary<VirtualKeyCode, List<EventHandler<MouseHookEventArgs>>> _registeredMouseEvents;
        
        public bool IsInstalled => _hook == null ? false : _hook.IsInstalled;

        public bool CaptureMouseMove { get => _captureMouseMove; set => _captureMouseMove = value; }
        public bool ClearInjectedFlag { get => _clearInjectedFlag; set => _clearInjectedFlag = value; }

        public int MouseX => _mouseX;
        public int MouseY => _mouseY;

        public bool IsLeftButtonDown => IsButtonDown(VirtualKeyCode.Lbutton);
        public bool IsRightButtonDown => IsButtonDown(VirtualKeyCode.Rbutton);
        public bool IsMiddleButtonDown => IsButtonDown(VirtualKeyCode.Mbutton);
        public bool IsXButton1Down => IsButtonDown(VirtualKeyCode.Xbutton1);
        public bool IsXButton2Down => IsButtonDown(VirtualKeyCode.Xbutton2);

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
                { VirtualKeyCode.Xbutton2, KeyState.Up },
                { VirtualKeyCode.Scroll, KeyState.Up }
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

        public KeyState GetButtonState(VirtualKeyCode key)
        {
            switch (key)
            {
                case VirtualKeyCode.Lbutton:
                case VirtualKeyCode.Rbutton:
                case VirtualKeyCode.Mbutton:
                case VirtualKeyCode.Xbutton1:
                case VirtualKeyCode.Xbutton2:
                case VirtualKeyCode.Scroll:
                    var state = _buttonStates[key];
                    if (state == KeyState.Pressed) _buttonStates[key] = KeyState.Up;
                    return state;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key));
            }
        }

        public VirtualKeyCode GetNextPressedButton(int timeout = -1)
        {
            var threadLock = new object();
            var button = VirtualKeyCode.Invalid;

            MouseEventAsync += localEventHandler;

            bool result = false;

            Monitor.Enter(threadLock);

            if (timeout < 0)
            {
                Monitor.Wait(threadLock);
                result = true;
            }
            else
            {
                result = Monitor.Wait(threadLock, timeout);
            }

            Monitor.Exit(threadLock);

            MouseEventAsync -= localEventHandler;

            if (result)
            {
                return button;
            }
            else
            {
                return VirtualKeyCode.Invalid;
            }

            void localEventHandler(object sender, MouseHookEventArgs e)
            {
                if (e.IsMouseMove) return;

                if (e.State == KeyState.Down)
                {
                    button = e.Button;

                    if (!Monitor.TryEnter(threadLock)) return;

                    Monitor.PulseAll(threadLock);
                    Monitor.Exit(threadLock);
                }
            }
        }

        public bool WaitForEvent(VirtualKeyCode button, KeyState state = KeyState.None, int timeout = -1)
        {
            if (button < VirtualKeyCode.Invalid || button > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(button));
            if (state < KeyState.None || state > KeyState.Pressed) throw new ArgumentOutOfRangeException(nameof(state));

            if (state == KeyState.Pressed) state = KeyState.Down;

            var threadLock = new object();

            MouseEventAsync += localEventHandler;

            bool result = false;

            Monitor.Enter(threadLock);

            if (timeout < 0)
            {
                Monitor.Wait(threadLock);
                result = true;
            }
            else
            {
                result = Monitor.Wait(threadLock, timeout);
            }

            Monitor.Exit(threadLock);

            MouseEventAsync -= localEventHandler;

            return result;

            void localEventHandler(object sender, MouseHookEventArgs e)
            {
                if (e.Button != button) return;
                if (state != e.State && state != KeyState.None) return;

                if (!Monitor.TryEnter(threadLock)) return;

                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            }
        }

        public bool WaitForEvent(int x, int y, VirtualKeyCode button, KeyState state = KeyState.None, int timeout = -1)
        {
            if (button < VirtualKeyCode.Invalid || button > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(button));
            if (state < KeyState.None || state > KeyState.Pressed) throw new ArgumentOutOfRangeException(nameof(state));

            if (state == KeyState.Pressed) state = KeyState.Down;

            var threadLock = new object();

            MouseEventAsync += localEventHandler;

            bool result = false;

            Monitor.Enter(threadLock);

            if (timeout < 0)
            {
                Monitor.Wait(threadLock);
                result = true;
            }
            else
            {
                result = Monitor.Wait(threadLock, timeout);
            }

            Monitor.Exit(threadLock);

            MouseEventAsync -= localEventHandler;

            return result;

            void localEventHandler(object sender, MouseHookEventArgs e)
            {
                if (e.Button != button) return;
                if (state != e.State && state != KeyState.None) return;
                if (e.X != x || e.Y != y) return;

                if (!Monitor.TryEnter(threadLock)) return;

                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            }
        }

        public bool WaitForEvent(int x, int y, int timeout = -1)
        {
            var threadLock = new object();

            MouseEventAsync += localEventHandler;

            bool result = false;

            Monitor.Enter(threadLock);

            if (timeout < 0)
            {
                Monitor.Wait(threadLock);
                result = true;
            }
            else
            {
                result = Monitor.Wait(threadLock, timeout);
            }

            Monitor.Exit(threadLock);

            MouseEventAsync -= localEventHandler;

            return result;

            void localEventHandler(object sender, MouseHookEventArgs e)
            {
                if (e.X != x || e.Y != y) return;

                if (!Monitor.TryEnter(threadLock)) return;

                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            }
        }

        public void RegisterMouseEvent(VirtualKeyCode button, EventHandler<MouseHookEventArgs> eventHandler)
        {
            if (button < VirtualKeyCode.Invalid || button > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(button));
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));

            lock (_lock)
            {
                if (!_registeredMouseEvents.ContainsKey(button)) _registeredMouseEvents.Add(button, new List<EventHandler<MouseHookEventArgs>>());

                _registeredMouseEvents[button].Add(eventHandler);
            }
        }

        public bool UnregisterMouseEvent(VirtualKeyCode button, EventHandler<MouseHookEventArgs> eventHandler)
        {
            if (button < VirtualKeyCode.Invalid || button > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(button));
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));

            lock (_lock)
            {
                if (!_registeredMouseEvents.ContainsKey(button)) return false;

                return _registeredMouseEvents[button].Remove(eventHandler);
            }
        }

        public void SetButtonState(VirtualKeyCode key, KeyState state)
        {
            switch (key)
            {
                case VirtualKeyCode.Lbutton:
                case VirtualKeyCode.Rbutton:
                case VirtualKeyCode.Mbutton:
                case VirtualKeyCode.Xbutton1:
                case VirtualKeyCode.Xbutton2:
                case VirtualKeyCode.Scroll:
                    try
                    {
                        _buttonStates[key] = state;
                    }
                    catch
                    {

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key));
            }
        }

        public bool IsButtonDown(VirtualKeyCode key)
        {
            switch (key)
            {
                case VirtualKeyCode.Lbutton:
                case VirtualKeyCode.Rbutton:
                case VirtualKeyCode.Mbutton:
                case VirtualKeyCode.Xbutton1:
                case VirtualKeyCode.Xbutton2:
                case VirtualKeyCode.Scroll:
                    var state = _buttonStates[key];
                    if (state == KeyState.Pressed) _buttonStates[key] = KeyState.Up;
                    return state == KeyState.Down || state == KeyState.Pressed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key));
            }
        }

        public bool IsButtonUp(VirtualKeyCode key)
        {
            switch (key)
            {
                case VirtualKeyCode.Lbutton:
                case VirtualKeyCode.Rbutton:
                case VirtualKeyCode.Mbutton:
                case VirtualKeyCode.Xbutton1:
                case VirtualKeyCode.Xbutton2:
                case VirtualKeyCode.Scroll:
                    return _buttonStates[key] == KeyState.Up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key));
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
        
        private void ResetButtonPressedStates()
        {
            try
            {
                if (_buttonStates[VirtualKeyCode.Scroll] == KeyState.Pressed) _buttonStates[VirtualKeyCode.Scroll] = KeyState.Up;
            }
            catch
            {

            }
        }

        private void _hook_HookCalled(object sender, HookCalledEventArgs e)
        {
            var eventData = GetMouseHookEventArgs(e.WParam, e.LParam);
            
            if (eventData == null)
            {
                return;
            }
            else
            {
                ResetButtonPressedStates();

                if (_clearInjectedFlag)
                {
                    ClearInjectedFlagHelper(e.LParam);
                }

                if (eventData.IsMouseMove)
                {
                    _mouseX = eventData.X;
                    _mouseY = eventData.Y;
                }

                if (eventData.State != KeyState.None && eventData.Button != VirtualKeyCode.Invalid)
                {
                    SetButtonState(eventData.Button, eventData.State);
                }
                
                if (eventData.IsMouseMove && !CaptureMouseMove)
                {
                    return;
                }
                else
                {
                    OnMouseEventAsync(eventData);
                    OnMouseEvent(eventData);
                    OnFireRegisteredEvents(eventData);
                }
            }
        }

        private bool _hook_filterCallback(object sender, HookCalledEventArgs e)
        {
            var eventData = GetMouseHookEventArgs(e.WParam, e.LParam);

            if (eventData == null)
            {
                return false;
            }
            else
            {
                return OnMouseFilter(eventData);
            }
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
        
        private static MouseHookEventArgs GetMouseHookEventArgs(IntPtr wParam, IntPtr lParam)
        {
            if (lParam == IntPtr.Zero) return null;

            VirtualKeyCode button = VirtualKeyCode.Invalid;
            KeyState state = KeyState.None;
            int mouseWheelDelta = 0;
            bool isMouseMove = false;

            var msg = (WindowMessage)(uint)wParam.ToInt32();

            int x = 0;
            int y = 0;

            switch (msg)
            {
                case WindowMessage.Lbuttondblclk:
                case WindowMessage.Nclbuttondblclk:
                    button = VirtualKeyCode.Lbutton;
                    state = KeyState.Pressed;
                    break;

                case WindowMessage.Lbuttondown:
                case WindowMessage.Nclbuttondown:
                    button = VirtualKeyCode.Lbutton;
                    state = KeyState.Down;
                    break;

                case WindowMessage.Lbuttonup:
                case WindowMessage.Nclbuttonup:
                    button = VirtualKeyCode.Lbutton;
                    state = KeyState.Up;
                    break;

                case WindowMessage.Mbuttondblclk:
                case WindowMessage.Ncmbuttondblclk:
                    button = VirtualKeyCode.Mbutton;
                    state = KeyState.Pressed;
                    break;

                case WindowMessage.Mbuttondown:
                case WindowMessage.Ncmbuttondown:
                    button = VirtualKeyCode.Mbutton;
                    state = KeyState.Down;
                    break;

                case WindowMessage.Mbuttonup:
                case WindowMessage.Ncmbuttonup:
                    button = VirtualKeyCode.Mbutton;
                    state = KeyState.Up;
                    break;

                case WindowMessage.Rbuttondblclk:
                case WindowMessage.Ncrbuttondblclk:
                    button = VirtualKeyCode.Rbutton;
                    state = KeyState.Pressed;
                    break;

                case WindowMessage.Rbuttondown:
                case WindowMessage.Ncrbuttondown:
                    button = VirtualKeyCode.Rbutton;
                    state = KeyState.Down;
                    break;

                case WindowMessage.Rbuttonup:
                case WindowMessage.Ncrbuttonup:
                    button = VirtualKeyCode.Rbutton;
                    state = KeyState.Up;
                    break;

                case WindowMessage.Xbuttondblclk:
                case WindowMessage.Ncxbuttondblclk:
                    if (HIWORD(Marshal.ReadInt32(lParam, 8)) == 0x1)
                    {
                        button = VirtualKeyCode.Xbutton1;
                        state = KeyState.Pressed;
                    }
                    else
                    {
                        button = VirtualKeyCode.Xbutton2;
                        state = KeyState.Pressed;
                    }
                    break;

                case WindowMessage.Xbuttondown:
                case WindowMessage.Ncxbuttondown:
                    if (HIWORD(Marshal.ReadInt32(lParam, 8)) == 0x1)
                    {
                        button = VirtualKeyCode.Xbutton1;
                        state = KeyState.Down;
                    }
                    else
                    {
                        button = VirtualKeyCode.Xbutton2;
                        state = KeyState.Down;
                    }
                    break;

                case WindowMessage.Xbuttonup:
                case WindowMessage.Ncxbuttonup:
                    if (HIWORD(Marshal.ReadInt32(lParam, 8)) == 0x1)
                    {
                        button = VirtualKeyCode.Xbutton1;
                        state = KeyState.Up;
                    }
                    else
                    {
                        button = VirtualKeyCode.Xbutton2;
                        state = KeyState.Up;
                    }
                    break;

                case WindowMessage.Mousewheel:
                case WindowMessage.Mousehwheel:
                    button = VirtualKeyCode.Scroll;
                    state = KeyState.Pressed;
                    mouseWheelDelta = HIWORD(Marshal.ReadInt32(lParam, 8));
                    break;

                case WindowMessage.Mousemove:
                case WindowMessage.Ncmousemove:
                    isMouseMove = true;

                    x = Marshal.ReadInt32(lParam);
                    y = Marshal.ReadInt32(lParam, 4);
                    break;
                default:
                    return null;
            }

            return isMouseMove
                ? new MouseHookEventArgs(x, y, button, state, mouseWheelDelta)
                : new MouseHookEventArgs(button, state, mouseWheelDelta);
        }

        private static void ClearInjectedFlagHelper(IntPtr lParam)
        {
            if (lParam == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(lParam));

            Marshal.WriteInt32(lParam, 12, 0);
        }

        private static short HIWORD(int number)
        {
            return (short)((number >> 16) & 0xFFFF);
        }
    }
}

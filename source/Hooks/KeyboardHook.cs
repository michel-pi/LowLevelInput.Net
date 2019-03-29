using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LowLevelInput.Converters;
using LowLevelInput.PInvoke;

namespace LowLevelInput.Hooks
{
    public class KeyboardHook : IDisposable
    {
        private readonly object _lock;

        private volatile bool _clearInjectedFlag;
        private volatile bool _capslockState;
        private volatile WindowsHook _hook;

        private Dictionary<VirtualKeyCode, KeyState> _keyStates;
        private Dictionary<VirtualKeyCode, List<EventHandler<KeyboardHookEventArgs>>> _registeredKeyboardEvents;

        public bool IsInstalled => _hook == null ? false : _hook.IsInstalled;
        public bool ClearInjectedFlag { get => _clearInjectedFlag; set => _clearInjectedFlag = value; }

        public bool Capslock { get => _capslockState; private set => _capslockState = value; }

        public bool IsShiftKeyDown => IsDown(VirtualKeyCode.Lshift) || IsDown(VirtualKeyCode.Rshift) || IsDown(VirtualKeyCode.Shift);
        public bool IsControlDown => IsDown(VirtualKeyCode.Lcontrol) || IsDown(VirtualKeyCode.Rcontrol) || IsDown(VirtualKeyCode.Control);
        public bool IsAltDown => IsDown(VirtualKeyCode.Lmenu) || IsDown(VirtualKeyCode.Rmenu) || IsDown(VirtualKeyCode.Menu);

        public bool AreLettersUppercase => Capslock ? !IsShiftKeyDown : IsShiftKeyDown;
        
        public WindowsHook Hook { get => _hook; private set => _hook = value; }

        public event EventHandler<KeyboardHookEventArgs> KeyboardEvent;
        public event EventHandler<KeyboardHookEventArgs> KeyboardEventAsync;

        public KeyboardFilterCallback KeyboardFilter;

        public KeyboardHook()
        {
            _lock = new object();
            
            _keyStates = new Dictionary<VirtualKeyCode, KeyState>();

            foreach (var key in KeyCodeConverter.KeyCodes)
            {
                _keyStates.Add(key, KeyState.Up);
            }

            _registeredKeyboardEvents = new Dictionary<VirtualKeyCode, List<EventHandler<KeyboardHookEventArgs>>>();

            Capslock = GetAsyncCapslockState();
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

                Capslock = GetAsyncCapslockState();

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

        public KeyState GetKeyState(VirtualKeyCode key)
        {
            if (key < VirtualKeyCode.Invalid || key > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(key));

            return _keyStates[key];
        }

        public void SetKeyState(VirtualKeyCode key, KeyState state)
        {
            if (key < VirtualKeyCode.Invalid || key > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(key));
            if (state < KeyState.None || state > KeyState.Pressed) throw new ArgumentOutOfRangeException(nameof(state));

            _keyStates[key] = state;
        }

        public bool IsDown(VirtualKeyCode key)
        {
            var state = GetKeyState(key);

            return state == KeyState.Down || state == KeyState.Pressed;
        }

        public bool IsUp(VirtualKeyCode key)
        {
            return GetKeyState(key) == KeyState.Up;
        }

        public bool IsPressed(VirtualKeyCode key)
        {
            return GetKeyState(key) == KeyState.Pressed;
        }

        public bool WasPressed(VirtualKeyCode key)
        {
            var state = GetKeyState(key);

            if (state == KeyState.Down || state == KeyState.Pressed)
            {
                SetKeyState(key, KeyState.Up);

                return true;
            }
            else
            {
                return false;
            }
        }

        public VirtualKeyCode GetNextPressedKey(int timeout = -1)
        {
            var threadLock = new object();
            var key = VirtualKeyCode.Invalid;

            EventHandler<KeyboardHookEventArgs> eventHandler = (object sender, KeyboardHookEventArgs e) =>
            {
                if (e.State != KeyState.Down) return;

                key = e.Key;

                if (!Monitor.TryEnter(threadLock)) return;

                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            };

            KeyboardEventAsync += eventHandler;

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

            KeyboardEventAsync -= eventHandler;

            if (result)
            {
                return key;
            }
            else
            {
                return VirtualKeyCode.Invalid;
            }
        }

        public bool WaitForEvent(VirtualKeyCode key, KeyState state = KeyState.None, int timeout = -1)
        {
            if (key < VirtualKeyCode.Invalid || key > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(key));
            if (state < KeyState.None || state > KeyState.Pressed) throw new ArgumentOutOfRangeException(nameof(state));

            if (state == KeyState.Pressed) state = KeyState.Down;

            var threadLock = new object();

            EventHandler<KeyboardHookEventArgs> eventHandler = (object sender, KeyboardHookEventArgs e) =>
            {
                if (e.Key != key) return;
                if (state != e.State && state != KeyState.None) return;

                if (!Monitor.TryEnter(threadLock)) return;

                Monitor.PulseAll(threadLock);
                Monitor.Exit(threadLock);
            };

            KeyboardEventAsync += eventHandler;

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

            KeyboardEventAsync -= eventHandler;

            return result;
        }

        public void RegisterKeyboardEvent(VirtualKeyCode key, EventHandler<KeyboardHookEventArgs> eventHandler)
        {
            if (key < VirtualKeyCode.Invalid || key > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(key));
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));

            lock (_lock)
            {
                if (!_registeredKeyboardEvents.ContainsKey(key)) _registeredKeyboardEvents.Add(key, new List<EventHandler<KeyboardHookEventArgs>>());

                _registeredKeyboardEvents[key].Add(eventHandler);
            }
        }

        public bool UnregisterKeyboardEvent(VirtualKeyCode key, EventHandler<KeyboardHookEventArgs> eventHandler)
        {
            if (key < VirtualKeyCode.Invalid || key > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(key));
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));

            lock (_lock)
            {
                if (!_registeredKeyboardEvents.ContainsKey(key)) return false;

                return _registeredKeyboardEvents[key].Remove(eventHandler);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyboardHook value)
            {
                return value._clearInjectedFlag == _clearInjectedFlag
                    && value._hook == null ? _hook == null : value._hook.Equals(_hook);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(KeyboardHook value)
        {
            return value != null
                && value._clearInjectedFlag == _clearInjectedFlag
                && value._hook == null ? _hook == null : value._hook.Equals(_hook);
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                _clearInjectedFlag.GetHashCode(),
                _hook == null ? 1 : _hook.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "IsInstalled", IsInstalled.ToString(),
                "ClearInjectedFlag", ClearInjectedFlag.ToString(),
                "Capslock", Capslock.ToString(),
                "IsShiftKeyDown", IsShiftKeyDown.ToString());
        }

        protected virtual void OnKeyboardEvent(VirtualKeyCode key, KeyState state, bool capslock, bool isShiftKeyDown)
        {
            KeyboardEvent?.Invoke(this, new KeyboardHookEventArgs(key, state, capslock, isShiftKeyDown));
        }

        protected virtual void OnKeyboardEventAsync(VirtualKeyCode key, KeyState state, bool capslock, bool isShiftKeyDown)
        {
            Task.Factory.StartNew(() =>
            {
                KeyboardEventAsync?.Invoke(this, new KeyboardHookEventArgs(key, state, capslock, isShiftKeyDown));
            });
        }

        protected virtual bool OnKeyboardFilter(VirtualKeyCode key, KeyState state)
        {
            bool? filter = KeyboardFilter?.Invoke(this, new KeyboardHookEventArgs(key, state));

            return filter == null ? false : (bool)filter;
        }

        protected virtual void OnFireRegisteredEvents(VirtualKeyCode key, KeyState state, bool capslock, bool isShiftKeyDown)
        {
            Task.Factory.StartNew(() =>
            {
                List<EventHandler<KeyboardHookEventArgs>> list;

                lock (_lock)
                {
                    if (!_registeredKeyboardEvents.ContainsKey(key)) return;

                    list = _registeredKeyboardEvents[key];
                }

                if (list == null || list.Count == 0) return;

                var args = new KeyboardHookEventArgs(key, state, capslock, isShiftKeyDown);

                try
                {
                    foreach (var eventHandler in list)
                    {
                        eventHandler?.Invoke(this, args);
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
                    _keyStates[key] = _keyStates[key] == KeyState.Down ? KeyState.Pressed : KeyState.Down;

                    OnFireRegisteredEvents(key, KeyState.Down, Capslock, IsShiftKeyDown);
                    OnKeyboardEventAsync(key, KeyState.Down, Capslock, IsShiftKeyDown);
                    OnKeyboardEvent(key, KeyState.Down, Capslock, IsShiftKeyDown);
                    break;
                case WindowMessage.Keyup:
                case WindowMessage.ImeKeyup:
                case WindowMessage.Syskeyup:
                    _keyStates[key] = KeyState.Up;

                    if (key == VirtualKeyCode.Capital)
                    {
                        Capslock = !Capslock;
                    }

                    OnFireRegisteredEvents(key, KeyState.Up, Capslock, IsShiftKeyDown);
                    OnKeyboardEventAsync(key, KeyState.Up, Capslock, IsShiftKeyDown);
                    OnKeyboardEvent(key, KeyState.Up, Capslock, IsShiftKeyDown);
                    break;
            }
        }

        private bool _hook_filterCallback(object sender, int code, IntPtr wParam, IntPtr lParam)
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

        public static bool Equals(KeyboardHook left, KeyboardHook right)
        {
            return left != null
                && left.Equals(right);
        }

        private static bool GetAsyncCapslockState()
        {
            var result = User32.GetAsyncKeyState(VirtualKeyCode.Capital);

            if ((result & 0x8000) != 0)
            {
                return true;
            }
            else if ((result & 0x0001) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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

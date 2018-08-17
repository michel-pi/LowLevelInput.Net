using System;
using System.Runtime.InteropServices;

using LowLevelInput.Hooks;
using LowLevelInput.PInvoke.Types;

namespace LowLevelInput.WindowsHooks
{
    /// <summary>
    /// 
    /// </summary>
    public static class WindowsHookFilter
    {
        /// <summary>
        /// return true if a filter should take action
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public delegate bool WindowsHookFilterEventHandler(VirtualKeyCode key, KeyState state);

        /// <summary>
        /// Occurs when [filter]. Returns true if a filter should take action
        /// </summary>
        public static event WindowsHookFilterEventHandler Filter;

        // returns true if an event needs to be filtered
        internal static bool InternalFilterEventsHelper(IntPtr wParam, IntPtr lParam)
        {
            if (wParam == IntPtr.Zero || lParam == IntPtr.Zero) return false;

            var events = Filter;

            if (events == null) return false;

            var msg = (WindowsMessage)(uint)wParam.ToInt32();

            var key = (VirtualKeyCode)Marshal.ReadInt32(lParam);

            KeyState state;

            switch (msg)
            {

                case WindowsMessage.Keydown:
                case WindowsMessage.Syskeydown:
                    state = KeyState.Down;
                    break;

                case WindowsMessage.Keyup:
                case WindowsMessage.Syskeyup:
                    state = KeyState.Up;
                    break;

                default:
                    state = KeyState.None;
                    break;
            }

            if (state == KeyState.None) return false;
            
            return events.Invoke(key, state);
        }
    }
}

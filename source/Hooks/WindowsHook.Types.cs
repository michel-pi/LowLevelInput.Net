using System;

namespace LowLevelInput.Hooks
{
    public delegate bool HookFilterCallback(object sender, int code, IntPtr wParam, IntPtr lParam);

    public class HookCalledEventArgs : EventArgs
    {
        public int Code { get; private set; }

        public IntPtr WParam { get; private set; }
        public IntPtr LParam { get; private set; }

        private HookCalledEventArgs()
        {
            throw new NotImplementedException();
        }

        public HookCalledEventArgs(int code, IntPtr wParam, IntPtr lParam)
        {
            Code = code;
            WParam = wParam;
            LParam = lParam;
        }

        public override bool Equals(object obj)
        {
            var args = obj as HookCalledEventArgs;
            
            if (args == null)
            {
                return false;
            }
            else
            {
                return args.Code == Code
                    && args.LParam == LParam
                    && args.WParam == WParam;
            }
        }

        public bool Equals(HookCalledEventArgs value)
        {
            return value != null
                && value.Code == Code
                && value.LParam == LParam
                && value.WParam == WParam;
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Code.GetHashCode(),
                WParam.GetHashCode(),
                LParam.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Code", Code.ToString(),
                "WParam", WParam.ToString(),
                "LParam", LParam.ToString()
                );
        }

        public static bool Equals(HookCalledEventArgs left, HookCalledEventArgs right)
        {
            return left != null
                && left.Equals(right);
        }
    }

    /// <summary>
    ///     An enumeration of all WindowsHook types
    /// </summary>
    public enum WindowsHookType
    {
        /// <summary>
        ///     The MSG filter
        /// </summary>
        MsgFilter = -1,

        /// <summary>
        ///     The journal record
        /// </summary>
        JournalRecord = 0,

        /// <summary>
        ///     The journal playback
        /// </summary>
        JournalPlayback = 1,

        /// <summary>
        ///     The keyboard
        /// </summary>
        Keyboard = 2,

        /// <summary>
        ///     The get message
        /// </summary>
        GetMessage = 3,

        /// <summary>
        ///     The call WND proc
        /// </summary>
        CallWndProc = 4,

        /// <summary>
        ///     The CBT
        /// </summary>
        Cbt = 5,

        /// <summary>
        ///     The system MSG filter
        /// </summary>
        SysMsgFilter = 6,

        /// <summary>
        ///     The mouse
        /// </summary>
        Mouse = 7,

        /// <summary>
        ///     The undocumented
        /// </summary>
        Undocumented = 8,

        /// <summary>
        ///     The debug
        /// </summary>
        Debug = 9,

        /// <summary>
        ///     The shell
        /// </summary>
        Shell = 10,

        /// <summary>
        ///     The foreground idle
        /// </summary>
        ForegroundIdle = 11,

        /// <summary>
        ///     The call WND proc ret
        /// </summary>
        CallWndProcRet = 12,

        /// <summary>
        ///     The low level keyboard
        /// </summary>
        LowLevelKeyboard = 13,

        /// <summary>
        ///     The low level mouse
        /// </summary>
        LowLevelMouse = 14
    }
}

using System;
using System.Collections.Generic;

using LowLevelInput.Hooks;

namespace LowLevelInput.Converters
{
    public static class HookTypeConverter
    {
        private static Dictionary<WindowsHookType, string> _hookTypeToString;
        private static Dictionary<string, WindowsHookType> _stringToHookType;

        public static IEnumerable<WindowsHookType> HookTypes
        {
            get
            {
                foreach (var pair in _hookTypeToString)
                    yield return pair.Key;
            }
        }

        static HookTypeConverter()
        {
            _hookTypeToString = new Dictionary<WindowsHookType, string>();
            _stringToHookType = new Dictionary<string, WindowsHookType>();

            _hookTypeToString.Add(WindowsHookType.MsgFilter, "MsgFilter");
            _hookTypeToString.Add(WindowsHookType.JournalRecord, "JournalRecord");
            _hookTypeToString.Add(WindowsHookType.JournalPlayback, "JournalPlayback");
            _hookTypeToString.Add(WindowsHookType.Keyboard, "Keyboard");
            _hookTypeToString.Add(WindowsHookType.GetMessage, "GetMessage");
            _hookTypeToString.Add(WindowsHookType.CallWndProc, "CallWndProc");
            _hookTypeToString.Add(WindowsHookType.Cbt, "Cbt");
            _hookTypeToString.Add(WindowsHookType.SysMsgFilter, "SysMsgFilter");
            _hookTypeToString.Add(WindowsHookType.Mouse, "Mouse");
            _hookTypeToString.Add(WindowsHookType.Undocumented, "Undocumented");
            _hookTypeToString.Add(WindowsHookType.Debug, "Debug");
            _hookTypeToString.Add(WindowsHookType.Shell, "Shell");
            _hookTypeToString.Add(WindowsHookType.ForegroundIdle, "ForegroundIdle");
            _hookTypeToString.Add(WindowsHookType.CallWndProcRet, "CallWndProcRet");
            _hookTypeToString.Add(WindowsHookType.LowLevelKeyboard, "LowLevelKeyboard");
            _hookTypeToString.Add(WindowsHookType.LowLevelMouse, "LowLevelMouse");

            foreach (var pair in _hookTypeToString)
            {
                _stringToHookType.Add(pair.Value, pair.Key);
            }
        }

        public static WindowsHookType ToHookType(string type)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));

            if (_stringToHookType.ContainsKey(type))
            {
                return _stringToHookType[type];
            }
            else
            {
                throw new FormatException(nameof(type));
            }
        }

        public static WindowsHookType ToHookType(int index)
        {
            if (index < (int)WindowsHookType.MsgFilter || index > (int)WindowsHookType.LowLevelMouse) throw new ArgumentOutOfRangeException(nameof(index));

            return (WindowsHookType)index;
        }

        public static string ToString(WindowsHookType type)
        {
            if (type < WindowsHookType.MsgFilter || type > WindowsHookType.LowLevelMouse) throw new ArgumentOutOfRangeException(nameof(type));

            if (_hookTypeToString.ContainsKey(type))
            {
                return _hookTypeToString[type];
            }
            else
            {
                throw new FormatException(nameof(type));
            }
        }

        public static string ToString(int index)
        {
            var type = (WindowsHookType)index;

            if (type < WindowsHookType.MsgFilter || type > WindowsHookType.LowLevelMouse) throw new ArgumentOutOfRangeException(nameof(index));

            if (_hookTypeToString.ContainsKey(type))
            {
                return _hookTypeToString[type];
            }
            else
            {
                throw new FormatException(nameof(type));
            }
        }
    }
}

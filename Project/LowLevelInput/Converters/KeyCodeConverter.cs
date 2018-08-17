using System.Collections.Generic;

using LowLevelInput.Hooks;

namespace LowLevelInput.Converters
{
    /// <summary>
    ///     Provides converter methods for VirtualKeyCodes
    /// </summary>
    public static class KeyCodeConverter
    {
        private static readonly string[] KeyCodeMap =
        {
            "Hotkey",
            "Lbutton",
            "Rbutton",
            "Cancel",
            "Mbutton",
            "Xbutton1",
            "Xbutton2",
            "",
            "Back",
            "Tab",
            "",
            "",
            "Clear",
            "Return",
            "",
            "",
            "Shift",
            "Control",
            "Menu",
            "Pause",
            "Capital",
            "Kana",
            "Hangul",
            "",
            "Junja",
            "Final",
            "Hanja",
            "Kanji",
            "",
            "Escape",
            "Convert",
            "Nonconvert",
            "Accept",
            "Modechange",
            "Space",
            "Prior",
            "Next",
            "End",
            "Home",
            "Left",
            "Up",
            "Right",
            "Down",
            "Select",
            "Print",
            "Execute",
            "Snapshot",
            "Insert",
            "Delete",
            "Help",
            "Zero",
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven",
            "Eight",
            "Nine",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "Lwin",
            "Rwin",
            "Apps",
            "",
            "Sleep",
            "Numpad0",
            "Numpad1",
            "Numpad2",
            "Numpad3",
            "Numpad4",
            "Numpad5",
            "Numpad6",
            "Numpad7",
            "Numpad8",
            "Numpad9",
            "Multiply",
            "Add",
            "Separator",
            "Subtract",
            "Decimal",
            "Divide",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12",
            "F13",
            "F14",
            "F15",
            "F16",
            "F17",
            "F18",
            "F19",
            "F20",
            "F21",
            "F22",
            "F23",
            "F24",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "Numlock",
            "Scroll",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "Lshift",
            "Rshift",
            "Lcontrol",
            "Rcontrol",
            "Lmenu",
            "Rmenu",
            "BrowserBack",
            "BrowserForward",
            "BrowserRefresh",
            "BrowserStop",
            "BrowserSearch",
            "BrowserFavorites",
            "BrowserHome",
            "VolumeMute",
            "VolumeDown",
            "VolumeUp",
            "MediaNextTrack",
            "MediaPrevTrack",
            "MediaStop",
            "MediaPlayPause",
            "LaunchMail",
            "LaunchMediaSelect",
            "LaunchApp1",
            "LaunchApp2",
            "",
            "",
            "Oem1",
            "OemPlus",
            "OemComma",
            "OemMinus",
            "OemPeriod",
            "Oem2",
            "Oem3",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "Oem4",
            "Oem5",
            "Oem6",
            "Oem7",
            "Oem8",
            "",
            "",
            "Oem102",
            "",
            "",
            "Processkey",
            "",
            "Packet",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "Attn",
            "Crsel",
            "Exsel",
            "Ereof",
            "Play",
            "Zoom",
            "Noname",
            "Pa1",
            "OemClear"
        };

        /// <summary>
        ///     Enumerates <c>VirtualKeyCode</c> and it's <c>string</c> representation.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<VirtualKeyCode, string>> EnumerateVirtualKeyCodes()
        {
            for (int i = 0; i < KeyCodeMap.Length; i++)
            {
                if (string.IsNullOrEmpty(KeyCodeMap[i])) continue;
                if (string.IsNullOrWhiteSpace(KeyCodeMap[i])) continue;

                yield return new KeyValuePair<VirtualKeyCode, string>((VirtualKeyCode) i, KeyCodeMap[i]);
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents a <c>VirtualKeyCode</c>.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A <see cref="System.String" /> that represents a <c>VirtualKeyCode</c>.</returns>
        public static string ToString(VirtualKeyCode code)
        {
            int index = (int) code;

            if (index < 0) return string.Empty;
            if (index >= KeyCodeMap.Length) return string.Empty;

            return KeyCodeMap[index];
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public static string ToString(int index)
        {
            if (index < 0) return "Invalid";
            if (index >= KeyCodeMap.Length) return "Invalid";

            return KeyCodeMap[index];
        }

        /// <summary>
        ///     Converts a string to it's corresponding VirtualKeyCode
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static VirtualKeyCode ToVirtualKeyCode(string name)
        {
            if (string.IsNullOrEmpty(name)) return VirtualKeyCode.Invalid;
            if (string.IsNullOrWhiteSpace(name)) return VirtualKeyCode.Invalid;

            for (int i = 0; i < KeyCodeMap.Length; i++)
                if (name == KeyCodeMap[i]) return (VirtualKeyCode) i;

            return VirtualKeyCode.Invalid;
        }

        /// <summary>
        ///     To the virtual key code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public static VirtualKeyCode ToVirtualKeyCode(int code)
        {
            if (code < 0) return VirtualKeyCode.Invalid;
            if (code >= KeyCodeMap.Length) return VirtualKeyCode.Invalid;

            return (VirtualKeyCode) code;
        }
    }
}
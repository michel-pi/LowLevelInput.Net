using System;
using System.Collections.Generic;

using LowLevelInput.Hooks;

namespace LowLevelInput.Converters
{
    public static class KeyCodeConverter
    {
        private static Dictionary<VirtualKeyCode, string> _keyCodeToString;
        private static Dictionary<string, VirtualKeyCode> _stringToKeyCode;

        public static bool SanitizeInput { get; set; }

        public static IEnumerable<VirtualKeyCode> KeyCodes
        {
            get
            {
                foreach (var pair in _keyCodeToString)
                    yield return pair.Key;
            }
        }

        static KeyCodeConverter()
        {
            SanitizeInput = true;

            _keyCodeToString = new Dictionary<VirtualKeyCode, string>();
            _stringToKeyCode = new Dictionary<string, VirtualKeyCode>();

            #region fill _keyCodeToString dictionary
            _keyCodeToString.Add(VirtualKeyCode.Hotkey, "Hotkey");
            _keyCodeToString.Add(VirtualKeyCode.Lbutton, "Lbutton");
            _keyCodeToString.Add(VirtualKeyCode.Rbutton, "Rbutton");
            _keyCodeToString.Add(VirtualKeyCode.Cancel, "Cancel");
            _keyCodeToString.Add(VirtualKeyCode.Mbutton, "Mbutton");
            _keyCodeToString.Add(VirtualKeyCode.Xbutton1, "Xbutton1");
            _keyCodeToString.Add(VirtualKeyCode.Xbutton2, "Xbutton2");
            _keyCodeToString.Add(VirtualKeyCode.Back, "Back");
            _keyCodeToString.Add(VirtualKeyCode.Tab, "Tab");
            _keyCodeToString.Add(VirtualKeyCode.Clear, "Clear");
            _keyCodeToString.Add(VirtualKeyCode.Return, "Return");
            _keyCodeToString.Add(VirtualKeyCode.Shift, "Shift");
            _keyCodeToString.Add(VirtualKeyCode.Control, "Control");
            _keyCodeToString.Add(VirtualKeyCode.Menu, "Menu");
            _keyCodeToString.Add(VirtualKeyCode.Pause, "Pause");
            _keyCodeToString.Add(VirtualKeyCode.Capital, "Capital");
            _keyCodeToString.Add(VirtualKeyCode.Kana, "Kana");
            _keyCodeToString.Add(VirtualKeyCode.Hangul, "Hangul");
            _keyCodeToString.Add(VirtualKeyCode.Junja, "Junja");
            _keyCodeToString.Add(VirtualKeyCode.Final, "Final");
            _keyCodeToString.Add(VirtualKeyCode.Hanja, "Hanja");
            _keyCodeToString.Add(VirtualKeyCode.Kanji, "Kanji");
            _keyCodeToString.Add(VirtualKeyCode.Escape, "Escape");
            _keyCodeToString.Add(VirtualKeyCode.Convert, "Convert");
            _keyCodeToString.Add(VirtualKeyCode.Nonconvert, "Nonconvert");
            _keyCodeToString.Add(VirtualKeyCode.Accept, "Accept");
            _keyCodeToString.Add(VirtualKeyCode.Modechange, "Modechange");
            _keyCodeToString.Add(VirtualKeyCode.Space, "Space");
            _keyCodeToString.Add(VirtualKeyCode.Prior, "Prior");
            _keyCodeToString.Add(VirtualKeyCode.Next, "Next");
            _keyCodeToString.Add(VirtualKeyCode.End, "End");
            _keyCodeToString.Add(VirtualKeyCode.Home, "Home");
            _keyCodeToString.Add(VirtualKeyCode.Left, "Left");
            _keyCodeToString.Add(VirtualKeyCode.Up, "Up");
            _keyCodeToString.Add(VirtualKeyCode.Right, "Right");
            _keyCodeToString.Add(VirtualKeyCode.Down, "Down");
            _keyCodeToString.Add(VirtualKeyCode.Select, "Select");
            _keyCodeToString.Add(VirtualKeyCode.Print, "Print");
            _keyCodeToString.Add(VirtualKeyCode.Execute, "Execute");
            _keyCodeToString.Add(VirtualKeyCode.Snapshot, "Snapshot");
            _keyCodeToString.Add(VirtualKeyCode.Insert, "Insert");
            _keyCodeToString.Add(VirtualKeyCode.Delete, "Delete");
            _keyCodeToString.Add(VirtualKeyCode.Help, "Help");
            _keyCodeToString.Add(VirtualKeyCode.Zero, "Zero");
            _keyCodeToString.Add(VirtualKeyCode.One, "One");
            _keyCodeToString.Add(VirtualKeyCode.Two, "Two");
            _keyCodeToString.Add(VirtualKeyCode.Three, "Three");
            _keyCodeToString.Add(VirtualKeyCode.Four, "Four");
            _keyCodeToString.Add(VirtualKeyCode.Five, "Five");
            _keyCodeToString.Add(VirtualKeyCode.Six, "Six");
            _keyCodeToString.Add(VirtualKeyCode.Seven, "Seven");
            _keyCodeToString.Add(VirtualKeyCode.Eight, "Eight");
            _keyCodeToString.Add(VirtualKeyCode.Nine, "Nine");
            _keyCodeToString.Add(VirtualKeyCode.A, "A");
            _keyCodeToString.Add(VirtualKeyCode.B, "B");
            _keyCodeToString.Add(VirtualKeyCode.C, "C");
            _keyCodeToString.Add(VirtualKeyCode.D, "D");
            _keyCodeToString.Add(VirtualKeyCode.E, "E");
            _keyCodeToString.Add(VirtualKeyCode.F, "F");
            _keyCodeToString.Add(VirtualKeyCode.G, "G");
            _keyCodeToString.Add(VirtualKeyCode.H, "H");
            _keyCodeToString.Add(VirtualKeyCode.I, "I");
            _keyCodeToString.Add(VirtualKeyCode.J, "J");
            _keyCodeToString.Add(VirtualKeyCode.K, "K");
            _keyCodeToString.Add(VirtualKeyCode.L, "L");
            _keyCodeToString.Add(VirtualKeyCode.M, "M");
            _keyCodeToString.Add(VirtualKeyCode.N, "N");
            _keyCodeToString.Add(VirtualKeyCode.O, "O");
            _keyCodeToString.Add(VirtualKeyCode.P, "P");
            _keyCodeToString.Add(VirtualKeyCode.Q, "Q");
            _keyCodeToString.Add(VirtualKeyCode.R, "R");
            _keyCodeToString.Add(VirtualKeyCode.S, "S");
            _keyCodeToString.Add(VirtualKeyCode.T, "T");
            _keyCodeToString.Add(VirtualKeyCode.U, "U");
            _keyCodeToString.Add(VirtualKeyCode.V, "V");
            _keyCodeToString.Add(VirtualKeyCode.W, "W");
            _keyCodeToString.Add(VirtualKeyCode.X, "X");
            _keyCodeToString.Add(VirtualKeyCode.Y, "Y");
            _keyCodeToString.Add(VirtualKeyCode.Z, "Z");
            _keyCodeToString.Add(VirtualKeyCode.Lwin, "Lwin");
            _keyCodeToString.Add(VirtualKeyCode.Rwin, "Rwin");
            _keyCodeToString.Add(VirtualKeyCode.Apps, "Apps");
            _keyCodeToString.Add(VirtualKeyCode.Sleep, "Sleep");
            _keyCodeToString.Add(VirtualKeyCode.Numpad0, "Numpad0");
            _keyCodeToString.Add(VirtualKeyCode.Numpad1, "Numpad1");
            _keyCodeToString.Add(VirtualKeyCode.Numpad2, "Numpad2");
            _keyCodeToString.Add(VirtualKeyCode.Numpad3, "Numpad3");
            _keyCodeToString.Add(VirtualKeyCode.Numpad4, "Numpad4");
            _keyCodeToString.Add(VirtualKeyCode.Numpad5, "Numpad5");
            _keyCodeToString.Add(VirtualKeyCode.Numpad6, "Numpad6");
            _keyCodeToString.Add(VirtualKeyCode.Numpad7, "Numpad7");
            _keyCodeToString.Add(VirtualKeyCode.Numpad8, "Numpad8");
            _keyCodeToString.Add(VirtualKeyCode.Numpad9, "Numpad9");
            _keyCodeToString.Add(VirtualKeyCode.Multiply, "Multiply");
            _keyCodeToString.Add(VirtualKeyCode.Add, "Add");
            _keyCodeToString.Add(VirtualKeyCode.Separator, "Separator");
            _keyCodeToString.Add(VirtualKeyCode.Subtract, "Subtract");
            _keyCodeToString.Add(VirtualKeyCode.Decimal, "Decimal");
            _keyCodeToString.Add(VirtualKeyCode.Divide, "Divide");
            _keyCodeToString.Add(VirtualKeyCode.F1, "F1");
            _keyCodeToString.Add(VirtualKeyCode.F2, "F2");
            _keyCodeToString.Add(VirtualKeyCode.F3, "F3");
            _keyCodeToString.Add(VirtualKeyCode.F4, "F4");
            _keyCodeToString.Add(VirtualKeyCode.F5, "F5");
            _keyCodeToString.Add(VirtualKeyCode.F6, "F6");
            _keyCodeToString.Add(VirtualKeyCode.F7, "F7");
            _keyCodeToString.Add(VirtualKeyCode.F8, "F8");
            _keyCodeToString.Add(VirtualKeyCode.F9, "F9");
            _keyCodeToString.Add(VirtualKeyCode.F10, "F10");
            _keyCodeToString.Add(VirtualKeyCode.F11, "F11");
            _keyCodeToString.Add(VirtualKeyCode.F12, "F12");
            _keyCodeToString.Add(VirtualKeyCode.F13, "F13");
            _keyCodeToString.Add(VirtualKeyCode.F14, "F14");
            _keyCodeToString.Add(VirtualKeyCode.F15, "F15");
            _keyCodeToString.Add(VirtualKeyCode.F16, "F16");
            _keyCodeToString.Add(VirtualKeyCode.F17, "F17");
            _keyCodeToString.Add(VirtualKeyCode.F18, "F18");
            _keyCodeToString.Add(VirtualKeyCode.F19, "F19");
            _keyCodeToString.Add(VirtualKeyCode.F20, "F20");
            _keyCodeToString.Add(VirtualKeyCode.F21, "F21");
            _keyCodeToString.Add(VirtualKeyCode.F22, "F22");
            _keyCodeToString.Add(VirtualKeyCode.F23, "F23");
            _keyCodeToString.Add(VirtualKeyCode.F24, "F24");
            _keyCodeToString.Add(VirtualKeyCode.Numlock, "Numlock");
            _keyCodeToString.Add(VirtualKeyCode.Scroll, "Scroll");
            _keyCodeToString.Add(VirtualKeyCode.Lshift, "Lshift");
            _keyCodeToString.Add(VirtualKeyCode.Rshift, "Rshift");
            _keyCodeToString.Add(VirtualKeyCode.Lcontrol, "Lcontrol");
            _keyCodeToString.Add(VirtualKeyCode.Rcontrol, "Rcontrol");
            _keyCodeToString.Add(VirtualKeyCode.Lmenu, "Lmenu");
            _keyCodeToString.Add(VirtualKeyCode.Rmenu, "Rmenu");
            _keyCodeToString.Add(VirtualKeyCode.BrowserBack, "BrowserBack");
            _keyCodeToString.Add(VirtualKeyCode.BrowserForward, "BrowserForward");
            _keyCodeToString.Add(VirtualKeyCode.BrowserRefresh, "BrowserRefresh");
            _keyCodeToString.Add(VirtualKeyCode.BrowserStop, "BrowserStop");
            _keyCodeToString.Add(VirtualKeyCode.BrowserSearch, "BrowserSearch");
            _keyCodeToString.Add(VirtualKeyCode.BrowserFavorites, "BrowserFavorites");
            _keyCodeToString.Add(VirtualKeyCode.BrowserHome, "BrowserHome");
            _keyCodeToString.Add(VirtualKeyCode.VolumeMute, "VolumeMute");
            _keyCodeToString.Add(VirtualKeyCode.VolumeDown, "VolumeDown");
            _keyCodeToString.Add(VirtualKeyCode.VolumeUp, "VolumeUp");
            _keyCodeToString.Add(VirtualKeyCode.MediaNextTrack, "MediaNextTrack");
            _keyCodeToString.Add(VirtualKeyCode.MediaPrevTrack, "MediaPrevTrack");
            _keyCodeToString.Add(VirtualKeyCode.MediaStop, "MediaStop");
            _keyCodeToString.Add(VirtualKeyCode.MediaPlayPause, "MediaPlayPause");
            _keyCodeToString.Add(VirtualKeyCode.LaunchMail, "LaunchMail");
            _keyCodeToString.Add(VirtualKeyCode.LaunchMediaSelect, "LaunchMediaSelect");
            _keyCodeToString.Add(VirtualKeyCode.LaunchApp1, "LaunchApp1");
            _keyCodeToString.Add(VirtualKeyCode.LaunchApp2, "LaunchApp2");
            _keyCodeToString.Add(VirtualKeyCode.Oem1, "Oem1");
            _keyCodeToString.Add(VirtualKeyCode.OemPlus, "OemPlus");
            _keyCodeToString.Add(VirtualKeyCode.OemComma, "OemComma");
            _keyCodeToString.Add(VirtualKeyCode.OemMinus, "OemMinus");
            _keyCodeToString.Add(VirtualKeyCode.OemPeriod, "OemPeriod");
            _keyCodeToString.Add(VirtualKeyCode.Oem2, "Oem2");
            _keyCodeToString.Add(VirtualKeyCode.Oem3, "Oem3");
            _keyCodeToString.Add(VirtualKeyCode.Oem4, "Oem4");
            _keyCodeToString.Add(VirtualKeyCode.Oem5, "Oem5");
            _keyCodeToString.Add(VirtualKeyCode.Oem6, "Oem6");
            _keyCodeToString.Add(VirtualKeyCode.Oem7, "Oem7");
            _keyCodeToString.Add(VirtualKeyCode.Oem8, "Oem8");
            _keyCodeToString.Add(VirtualKeyCode.Oem102, "Oem102");
            _keyCodeToString.Add(VirtualKeyCode.Processkey, "Processkey");
            _keyCodeToString.Add(VirtualKeyCode.Packet, "Packet");
            _keyCodeToString.Add(VirtualKeyCode.Attn, "Attn");
            _keyCodeToString.Add(VirtualKeyCode.Crsel, "Crsel");
            _keyCodeToString.Add(VirtualKeyCode.Exsel, "Exsel");
            _keyCodeToString.Add(VirtualKeyCode.Ereof, "Ereof");
            _keyCodeToString.Add(VirtualKeyCode.Play, "Play");
            _keyCodeToString.Add(VirtualKeyCode.Zoom, "Zoom");
            _keyCodeToString.Add(VirtualKeyCode.Noname, "Noname");
            _keyCodeToString.Add(VirtualKeyCode.Pa1, "Pa1");
            _keyCodeToString.Add(VirtualKeyCode.OemClear, "OemClear");
            _keyCodeToString.Add(VirtualKeyCode.Max, "Max");
            _keyCodeToString.Add(VirtualKeyCode.Invalid, "Invalid");
            #endregion

            foreach (var pair in _keyCodeToString)
            {
                _stringToKeyCode.Add(pair.Value, pair.Key);
            }
        }

        public static string ToString(VirtualKeyCode keyCode)
        {
            if (keyCode < VirtualKeyCode.Invalid || keyCode > VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(keyCode));

            return _keyCodeToString[keyCode];
        }

        public static string ToString(int index)
        {
            if (index < (int)VirtualKeyCode.Invalid || index > (int)VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(index));

            return _keyCodeToString[(VirtualKeyCode)index];
        }

        public static VirtualKeyCode ToKeyCode(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            if (SanitizeInput)
            {
                key = FixCharacterCasing(key);
            }

            return _stringToKeyCode[key];
        }

        public static VirtualKeyCode ToKeyCode(int index)
        {
            if (index < (int)VirtualKeyCode.Invalid || index > (int)VirtualKeyCode.Max) throw new ArgumentOutOfRangeException(nameof(index));

            return (VirtualKeyCode)index;
        }

        private static string FixCharacterCasing(string str)
        {
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException(nameof(str));

            if (char.IsLower(str[0]))
            {
                char[] chars = str.ToCharArray();
                chars[0] = char.ToUpper(chars[0]);

                return new string(chars);
            }
            else
            {
                return str;
            }
        }
    }
}

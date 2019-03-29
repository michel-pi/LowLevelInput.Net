using System;
using System.Collections.Generic;

using LowLevelInput.Hooks;

namespace LowLevelInput.Converters
{
    public static class KeyStateConverter
    {
        private static Dictionary<KeyState, string> _keyStateToString;
        private static Dictionary<string, KeyState> _stringToKeyState;

        public static bool SanitizeInput { get; set; }

        public static IEnumerable<KeyState> KeyStates
        {
            get
            {
                foreach (var pair in _keyStateToString)
                    yield return pair.Key;
            }
        }
        
        static KeyStateConverter()
        {
            SanitizeInput = true;

            _keyStateToString = new Dictionary<KeyState, string>();
            _stringToKeyState = new Dictionary<string, KeyState>();

            _keyStateToString.Add(KeyState.None, "None");
            _keyStateToString.Add(KeyState.Up, "Up");
            _keyStateToString.Add(KeyState.Down, "Down");
            _keyStateToString.Add(KeyState.Pressed, "Pressed");

            foreach (var pair in _keyStateToString)
            {
                _stringToKeyState.Add(pair.Value, pair.Key);
            }
        }

        public static KeyState ToKeyState(string state)
        {
            if (string.IsNullOrEmpty(state)) throw new ArgumentNullException(nameof(state));

            if (SanitizeInput)
            {
                state = FixCharacterCasing(state);
            }

            return _stringToKeyState[state];
        }

        public static KeyState ToKeyState(int index)
        {
            if (index < (int)KeyState.None || index > (int)KeyState.Pressed) throw new ArgumentOutOfRangeException(nameof(index));

            return (KeyState)index;
        }

        public static string ToString(KeyState state)
        {
            if (state < KeyState.None || state > KeyState.Pressed) throw new ArgumentOutOfRangeException(nameof(state));

            return _keyStateToString[state];
        }

        public static string ToString(int index)
        {
            if (index < (int)KeyState.None || index > (int)KeyState.Pressed) throw new ArgumentOutOfRangeException(nameof(index));

            return _keyStateToString[(KeyState)index];
        }

        private static string FixCharacterCasing(string str)
        {
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException(nameof(str));

            char[] chars = str.ToCharArray();

            chars[0] = char.ToUpper(chars[0]);

            if (chars.Length > 1)
            {
                for (int i = 1; i < chars.Length; i++)
                {
                    chars[i] = char.ToLower(chars[i]);
                }
            }

            return new string(chars);
        }
    }
}

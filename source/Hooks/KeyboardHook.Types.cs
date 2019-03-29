using System;
using LowLevelInput.Converters;

namespace LowLevelInput.Hooks
{
    public delegate bool KeyboardFilterCallback(object sender, KeyboardHookEventArgs keyboard);

    public class KeyboardHookEventArgs : EventArgs
    {
        public VirtualKeyCode Key { get; private set; }
        public KeyState State { get; private set; }

        public bool Capslock { get; private set; }
        public bool IsShiftKeyDown { get; private set; }

        public bool IsUppercaseLetter => Capslock ? !IsShiftKeyDown : IsShiftKeyDown;

        private KeyboardHookEventArgs()
        {
            throw new NotImplementedException();
        }

        public KeyboardHookEventArgs(VirtualKeyCode key, KeyState state)
        {
            Key = key;
            State = state;
        }

        public KeyboardHookEventArgs(VirtualKeyCode key, KeyState state, bool capslock, bool isShiftKeyDown)
        {
            Key = key;
            State = state;

            Capslock = capslock;
            IsShiftKeyDown = isShiftKeyDown;
        }

        public bool IsUp(VirtualKeyCode key)
        {
            return key == Key && State == KeyState.Up;
        }

        public bool IsDown(VirtualKeyCode key)
        {
            return key == Key && State == KeyState.Down;
        }

        public bool IsPressed(VirtualKeyCode key)
        {
            return key == Key && (State == KeyState.Down || State == KeyState.Pressed);
        }

        public override bool Equals(object obj)
        {
            var keyboard = obj as KeyboardHookEventArgs;

            if (keyboard == null)
            {
                return false;
            }
            else
            {
                return keyboard.Key == Key
                    && keyboard.State == State
                    && keyboard.Capslock == Capslock
                    && keyboard.IsShiftKeyDown == IsShiftKeyDown;
            }
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Key.GetHashCode(),
                State.GetHashCode(),
                Capslock.GetHashCode(),
                IsShiftKeyDown.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Key", KeyCodeConverter.ToString(Key),
                "State", KeyStateConverter.ToString(State),
                "IsUppercaseLetter", IsUppercaseLetter.ToString());
        }
    }
}

using System;

namespace LowLevelInput.Hooks
{
    public delegate bool KeyboardFilterCallback(VirtualKeyCode key, KeyState state);

    public class KeyboardHookEventArgs : EventArgs
    {
        public VirtualKeyCode Key { get; private set; }
        public KeyState State { get; private set; }

        private KeyboardHookEventArgs()
        {
            throw new NotImplementedException();
        }

        public KeyboardHookEventArgs(VirtualKeyCode key, KeyState state)
        {
            Key = key;
            State = state;
        }
    }
}

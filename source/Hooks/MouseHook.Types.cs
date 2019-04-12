using System;
using LowLevelInput.Converters;

namespace LowLevelInput.Hooks
{
    public delegate bool MouseFilterCallback(object sender, MouseHookEventArgs mouse);

    public class MouseHookEventArgs : EventArgs
    {
        public bool IsMouseMove { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public VirtualKeyCode Button { get; private set; }
        public KeyState State { get; private set; }

        public int MouseWheelDelta { get; private set; }
        
        private MouseHookEventArgs()
        {
            throw new NotImplementedException();
        }

        public MouseHookEventArgs(VirtualKeyCode key, KeyState state, int mouseWheelDelta = 0)
        {
            Button = key;
            State = state;

            MouseWheelDelta = mouseWheelDelta;
        }

        public MouseHookEventArgs(int x, int y, int mouseWheelDelta = 0)
        {
            IsMouseMove = true;

            X = x;
            Y = y;

            MouseWheelDelta = mouseWheelDelta;
        }

        public MouseHookEventArgs(int x, int y, VirtualKeyCode key, KeyState state, int mouseWheelDelta = 0)
        {
            IsMouseMove = true;

            X = x;
            Y = y;

            Button = key;
            State = state;

            MouseWheelDelta = mouseWheelDelta;
        }

        public bool IsButtonDown(VirtualKeyCode key)
        {
            return Button == key && State == KeyState.Down;
        }

        public override bool Equals(object obj)
        {
            if (obj is MouseHookEventArgs mouse)
            {
                return mouse.IsMouseMove == IsMouseMove
                    && mouse.X == X
                    && mouse.Y == Y
                    && mouse.Button == Button
                    && mouse.MouseWheelDelta == MouseWheelDelta;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(MouseHookEventArgs value)
        {
            return value != null
                && value.IsMouseMove == IsMouseMove
                && value.X == X
                && value.Y == Y
                && value.Button == Button
                && value.MouseWheelDelta == MouseWheelDelta;
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                IsMouseMove.GetHashCode(),
                X.GetHashCode(),
                Y.GetHashCode(),
                Button.GetHashCode(),
                State.GetHashCode(),
                MouseWheelDelta.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "IsMouseMove", IsMouseMove.ToString(),
                "X", X.ToString(),
                "Y", Y.ToString(),
                "Button", KeyCodeConverter.ToString(Button),
                "State", KeyStateConverter.ToString(State),
                "MouseWheelDelta", MouseWheelDelta.ToString());
        }

        public static bool Equals(MouseHookEventArgs left, MouseHookEventArgs right)
        {
            return left != null
                && left.Equals(right);
        }
    }
}

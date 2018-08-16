using System;

using LowLevelInput;
using LowLevelInput.Converters;
using LowLevelInput.Hooks;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // creates a new instance to capture inputs
            // also provides IsPressed, WasPressed and GetState methods
            var inputManager = new InputManager();

            // you may not need those when you use InputManager
            var keyboardHook = new LowLevelKeyboardHook();
            var mouseHook = new LowLevelMouseHook();

            // subscribe to the events offered by InputManager
            inputManager.OnKeyboardEvent += InputManager_OnKeyboardEvent;
            inputManager.OnMouseEvent += InputManager_OnMouseEvent;

            // same events as above (in case you only need a specific hook and less functionality)
            keyboardHook.OnKeyboardEvent += KeyboardHook_OnKeyboardEvent;
            mouseHook.OnMouseEvent += MouseHook_OnMouseEvent;

            // we need to initialize our classes before they fire events and are completely usable
            inputManager.Initialize();
            keyboardHook.InstallHook();
            mouseHook.InstallHook();

            // registers an event (callback) which gets fired whenever the key changes it's state
            // be sure to use this method after the InputManager is initialized
            inputManager.RegisterEvent(VirtualKeyCode.Lbutton, InputManager_KeyStateChanged);

            Console.WriteLine("Waiting for up arrow key to exit!");

            // This method will block the current thread until the up arrow key changes it's state to Down
            // There is no performance penalty (spinning loop waiting for this)
            inputManager.WaitForEvent(VirtualKeyCode.Up, KeyState.Down);

            // be sure to dispose instances you dont use anymore
            // not doing so may block windows input and let inputs appear delayed or lagging
            // these classes try dispose itself when an unhandled exception occurs or the process exits
            mouseHook.Dispose();
            keyboardHook.Dispose();
            inputManager.Dispose();
        }

        private static void MouseHook_OnMouseEvent(VirtualKeyCode key, KeyState state, int x, int y)
        {
            // same as InputManager_OnMouseEvent
        }

        private static void KeyboardHook_OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            // same as InputManager_OnKeyboardEvent
        }

        private static void InputManager_OnMouseEvent(VirtualKeyCode key, KeyState state, int x, int y)
        {
            // x and y may be 0 if there is no data
            Console.WriteLine("OnMouseEvent: " + KeyCodeConverter.ToString(key) + " - " + KeyStateConverter.ToString(state) + " - X: " + x + ", Y: " + y);
        }

        private static void InputManager_OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            Console.WriteLine("OnKeyboardEvent: " + KeyCodeConverter.ToString(key) + " - " + KeyStateConverter.ToString(state));
        }

        private static void InputManager_KeyStateChanged(VirtualKeyCode key, KeyState state)
        {
            // you may use the same callback for every key or define a new one for each
            Console.WriteLine("The key state of " + KeyCodeConverter.ToString(key) + " changed to " + KeyStateConverter.ToString(state));
        }
    }
}

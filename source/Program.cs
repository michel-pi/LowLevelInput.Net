using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using LowLevelInput.Hooks;

namespace LowLevelInput
{
    class Program
    {
        static void Main(string[] args)
        {
            KeyboardHook hook = new KeyboardHook(false);

            hook.KeyboardEventAsync += Hook_KeyboardEventAsync;
            hook.KeyboardFilter = Hook_KeyboardFilter;

            hook.Install();

            while (true)
            {
                Thread.Sleep(100);
            }

            hook.Dispose();
        }

        private static void Hook_KeyboardEventAsync(object sender, KeyboardHookEventArgs e)
        {
            Console.WriteLine(e.Key.ToString() + "\t-\t" + e.State.ToString());
        }

        private static bool Hook_KeyboardFilter(VirtualKeyCode key, KeyState state)
        {
            return key == VirtualKeyCode.A;
        }
    }
}

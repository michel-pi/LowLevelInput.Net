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
            var hook = new MouseHook(false);

            hook.MouseEventAsync += Hook_MouseEventAsync;

            hook.Install();

            Console.ReadLine();

            hook.Dispose();
        }

        private static void Hook_MouseEventAsync(object sender, MouseHookEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        //static void Main(string[] args)
        //{
        //    var hook = new KeyboardHook();

        //    hook.KeyboardEventAsync += Hook_KeyboardEventAsync;

        //    hook.Install();

        //    Console.ReadLine();

        //    hook.Dispose();
        //}

        //private static void Hook_KeyboardEventAsync(object sender, KeyboardHookEventArgs e)
        //{
        //    Console.WriteLine(e.ToString());
        //}
    }
}

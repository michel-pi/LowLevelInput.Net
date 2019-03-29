using System;
using System.Runtime.InteropServices;

using LowLevelInput.Hooks;

namespace LowLevelInput.PInvoke
{
    internal static class User32
    {
        public delegate IntPtr CallNextHookExDelegate(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);
        public static readonly CallNextHookExDelegate CallNextHookEx;

        public delegate short GetAsyncKeyStateDelegate(VirtualKeyCode key);
        public static readonly GetAsyncKeyStateDelegate GetAsyncKeyState;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int GetMessageDelegate(ref Message lpMessage, IntPtr hwnd, uint msgFilterMin, uint msgFilterMax);
        public static readonly GetMessageDelegate GetMessage;
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PostThreadMessageDelegate(uint threadId, WindowMessage msg, IntPtr wParam, IntPtr lParam);
        public static readonly PostThreadMessageDelegate PostThreadMessage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr SetWindowsHookExDelegate(int type, IntPtr hookProcedure, IntPtr hModule, uint threadId);
        public static readonly SetWindowsHookExDelegate SetWindowsHookEx;

        public delegate int UnhookWindowsHookExDelegate(IntPtr hHook);
        public static readonly UnhookWindowsHookExDelegate UnhookWindowsHookEx;

        static User32()
        {
            var library = DynamicImport.ImportLibrary("user32.dll");

            CallNextHookEx = DynamicImport.Import<CallNextHookExDelegate>(library, "CallNextHookEx");
            GetAsyncKeyState = DynamicImport.Import<GetAsyncKeyStateDelegate>(library, "GetAsyncKeyState");
            GetMessage = DynamicImport.Import<GetMessageDelegate>(library, "GetMessageW");
            PostThreadMessage = DynamicImport.Import<PostThreadMessageDelegate>(library, "PostThreadMessageW");
            SetWindowsHookEx = DynamicImport.Import<SetWindowsHookExDelegate>(library, "SetWindowsHookExW");
            UnhookWindowsHookEx = DynamicImport.Import<UnhookWindowsHookExDelegate>(library, "UnhookWindowsHookEx");
        }
    }
}

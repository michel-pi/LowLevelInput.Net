using System;

namespace LowLevelInput.PInvoke
{
    internal static class Kernel32
    {
        public delegate uint GetCurrentThreadIdDelegate();
        public static readonly GetCurrentThreadIdDelegate GetCurrentThreadId;

        static Kernel32()
        {
            var library = DynamicImport.ImportLibrary("kernel32.dll");

            GetCurrentThreadId = DynamicImport.Import<GetCurrentThreadIdDelegate>(library, "GetCurrentThreadId");
        }
    }
}

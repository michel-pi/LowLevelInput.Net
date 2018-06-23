using System;
using System.Security;

namespace LowLevelInput.PInvoke.Libraries
{
    [SuppressUnmanagedCodeSecurity()]
    internal static class Kernel32
    {
        public static GetCurrentThreadId_t GetCurrentThreadId = WinApi.GetMethod<GetCurrentThreadId_t>("kernel32.dll", "GetCurrentThreadId");

        public delegate uint GetCurrentThreadId_t();
    }
}
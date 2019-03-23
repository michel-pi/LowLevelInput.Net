using System;

namespace LowLevelInput
{
    internal class UnhookGuardEventArgs : EventArgs
    {
        public bool HasException => Exception != null;

        public Exception Exception { get; private set; }

        public UnhookGuardEventArgs()
        {
            Exception = null;
        }

        public UnhookGuardEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }

    internal static class UnhookGuard
    {
        public static event EventHandler<UnhookGuardEventArgs> UnhookGuardEvent;

        static UnhookGuard()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            UnhookGuardEvent?.Invoke(sender, new UnhookGuardEventArgs());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhookGuardEvent?.Invoke(sender, new UnhookGuardEventArgs((Exception)e.ExceptionObject));
        }
    }
}

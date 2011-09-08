using System;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class Run
    {
        public static Exception CatchingException(Action action)
        {
            Exception exception = null;

            try { action(); }
            catch (Exception ex) { exception = ex; }

            return exception;
        }
    }
}
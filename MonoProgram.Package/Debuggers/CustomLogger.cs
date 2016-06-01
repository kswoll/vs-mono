using System;
using Mono.Debugging.Client;

namespace MonoProgram.Package.Debuggers
{
    public class CustomLogger : ICustomLogger
    {
        public void LogError(string message, Exception ex)
        {
        }

        public void LogAndShowException(string message, Exception ex)
        {
        }

        public void LogMessage(string messageFormat, params object[] args)
        {
        }
/*

        public string GetNewDebuggerLogFilename()
        {
            return @"c:\temp\debugger.log";
        }
*/
    }
}
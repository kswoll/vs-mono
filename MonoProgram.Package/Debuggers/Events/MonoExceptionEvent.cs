using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoProgram.Package.Debuggers.Events
{
    public class MonoExceptionEvent: MonoStoppingEvent, IDebugExceptionEvent2
    {
        public const string IID = "51A94113-8788-4A54-AE15-08B74FF922D0";

        public bool IsUnhandled { get; set; }

        public string Description { get; private set; }

        public MonoExceptionEvent(StackFrame frame)
        {
            Description = frame?.GetAllLocals().SingleOrDefault(l => l.Name == "$exception")?.DisplayValue;
        }

        public int CanPassToDebuggee()
        {
            return VSConstants.S_OK;
        }

        public int GetException(EXCEPTION_INFO[] pExceptionInfo)
        {
            pExceptionInfo[0].bstrExceptionName = Description;
            pExceptionInfo[0].dwState = IsUnhandled ? enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT : enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE;
            pExceptionInfo[0].guidType = DebuggerGuids.CSharpLanguageService;
            return VSConstants.S_OK;
        }

        public int GetExceptionDescription(out string pbstrDescription)
        {
            pbstrDescription = Description;
            return VSConstants.S_OK;
        }

        public int PassToDebuggee(int fPass)
        {
            return VSConstants.S_OK;
        }
    }
}

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers.Events
{
    // This interface is sent by the debug engine (DE) to the session debug manager (SDM) when a program has run to completion
    // or is otherwise destroyed.
    public class MonoProgramDestroyEvent : SynchronousEvent, IDebugProgramDestroyEvent2
    {
        public const string IID = "E147E9E3-6440-4073-A7B7-A65592C714B5";

        private readonly uint exitCode;

        public MonoProgramDestroyEvent(uint exitCode)
        {
            this.exitCode = exitCode;
        }

        int IDebugProgramDestroyEvent2.GetExitCode(out uint exitCode)
        {
            exitCode = this.exitCode;
            
            return VSConstants.S_OK;
        }
    }
}
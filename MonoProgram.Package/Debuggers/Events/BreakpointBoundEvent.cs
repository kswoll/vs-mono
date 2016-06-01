using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers.Events
{
    public class BreakpointBoundEvent : AsynchronousEvent, IDebugBreakpointBoundEvent2
    {
        public const string IID = "1dddb704-cf99-4b8a-b746-dabb01dd13a0";

        private readonly MonoPendingBreakpoint pendingBreakpoint;
        private readonly MonoBoundBreakpoint boundBreakpoint;

        public BreakpointBoundEvent(MonoPendingBreakpoint pendingBreakpoint, MonoBoundBreakpoint boundBreakpoint)
        {
            this.pendingBreakpoint = pendingBreakpoint;
            this.boundBreakpoint = boundBreakpoint;
        }

        int IDebugBreakpointBoundEvent2.EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 ppEnum)
        {
            var boundBreakpoints = new IDebugBoundBreakpoint2[1];
            boundBreakpoints[0] = boundBreakpoint;
            ppEnum = new MonoBoundBreakpointsEnum(boundBreakpoints);
            return VSConstants.S_OK;
        }

        int IDebugBreakpointBoundEvent2.GetPendingBreakpoint(out IDebugPendingBreakpoint2 ppPendingBP)
        {
            ppPendingBP = pendingBreakpoint;
            return VSConstants.S_OK;
        }
    }
}
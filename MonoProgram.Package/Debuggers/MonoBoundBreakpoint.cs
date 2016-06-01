using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoBoundBreakpoint : IDebugBoundBreakpoint2
    {
        private readonly MonoEngine engine;
        private readonly uint address;
        private readonly MonoPendingBreakpoint pendingBreakpoint;
        private readonly MonoBreakpointResolution breakpointResolution;

        public MonoBoundBreakpoint(MonoEngine engine, uint address, MonoPendingBreakpoint pendingBreakpoint, MonoBreakpointResolution breakpointResolution)
        {
            this.engine = engine;
            this.address = address;
            this.pendingBreakpoint = pendingBreakpoint;
            this.breakpointResolution = breakpointResolution;
        }

        public int GetPendingBreakpoint(out IDebugPendingBreakpoint2 pendingBreakpoint)
        {
            pendingBreakpoint = this.pendingBreakpoint;
            return VSConstants.S_OK;
        }

        public int GetState(enum_BP_STATE[] state)
        {
            state[0] = enum_BP_STATE.BPS_ENABLED;
            return VSConstants.S_OK;
        }

        public int GetHitCount(out uint hitCount)
        {
            hitCount = 0;
            return VSConstants.S_OK;
        }

        public int GetBreakpointResolution(out IDebugBreakpointResolution2 breakpointResolution)
        {
            breakpointResolution = this.breakpointResolution;
            return VSConstants.S_OK;
        }

        public int Enable(int enable)
        {
            return VSConstants.S_OK;
        }

        public int SetHitCount(uint hitCount)
        {
            return VSConstants.S_OK;
        }

        public int SetCondition(BP_CONDITION bpCondition)
        {
            return VSConstants.S_OK;
        }

        public int SetPassCount(BP_PASSCOUNT bpPassCount)
        {
            return VSConstants.S_OK;
        }

        public int Delete()
        {
            return VSConstants.S_OK;
        }
    }
}
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoBoundBreakpointsEnum : Enumerator<IDebugBoundBreakpoint2, IEnumDebugBoundBreakpoints2>, IEnumDebugBoundBreakpoints2
    {
        public MonoBoundBreakpointsEnum(IDebugBoundBreakpoint2[] breakpoints) : base(breakpoints)
        {
        }

        public int Next(uint celt, IDebugBoundBreakpoint2[] rgelt, ref uint celtFetched)
        {
            return Next(celt, rgelt, out celtFetched);
        }
    }
}
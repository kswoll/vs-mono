using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoThreadEnum : Enumerator<IDebugThread2, IEnumDebugThreads2>, IEnumDebugThreads2
    {
        public MonoThreadEnum(IDebugThread2[] threads) : base(threads)
        {
        }

        public int Next(uint celt, IDebugThread2[] rgelt, ref uint celtFetched)
        {
            return Next(celt, rgelt, out celtFetched);
        }
    }
}
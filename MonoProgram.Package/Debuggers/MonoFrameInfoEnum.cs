using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoFrameInfoEnum : Enumerator<FRAMEINFO, IEnumDebugFrameInfo2>, IEnumDebugFrameInfo2
    {
        public MonoFrameInfoEnum(FRAMEINFO[] data) : base(data)
        {
        }

        public int Next(uint celt, FRAMEINFO[] rgelt, ref uint celtFetched)
        {
            return Next(celt, rgelt, out celtFetched);
        }
    }
}
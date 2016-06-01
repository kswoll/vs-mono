using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoCodeContextEnum : Enumerator<IDebugCodeContext2, IEnumDebugCodeContexts2>, IEnumDebugCodeContexts2
    {
        public MonoCodeContextEnum(IDebugCodeContext2[] codeContexts) : base(codeContexts)
        {
        }

        public int Next(uint celt, IDebugCodeContext2[] rgelt, ref uint celtFetched)
        {
            return Next(celt, rgelt, out celtFetched);
        }         
    }
}
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoModuleEnum : Enumerator<IDebugModule2, IEnumDebugModules2>, IEnumDebugModules2
    {
        public MonoModuleEnum(IDebugModule2[] modules) : base(modules)
        {
        }

        public int Next(uint celt, IDebugModule2[] rgelt, ref uint celtFetched)
        {
            return Next(celt, rgelt, out celtFetched);
        }
    }
}
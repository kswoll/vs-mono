using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoPropertyInfoEnum : Enumerator<DEBUG_PROPERTY_INFO, IEnumDebugPropertyInfo2>, IEnumDebugPropertyInfo2
    {
        public MonoPropertyInfoEnum(DEBUG_PROPERTY_INFO[] data) : base(data)
        {
        }
    }
}
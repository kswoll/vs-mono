using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoPropertyEnum : Enumerator<DEBUG_PROPERTY_INFO, IEnumDebugPropertyInfo2>, IEnumDebugPropertyInfo2
    {
        public MonoPropertyEnum(DEBUG_PROPERTY_INFO[] properties) : base(properties)
        {
        }
    }
}
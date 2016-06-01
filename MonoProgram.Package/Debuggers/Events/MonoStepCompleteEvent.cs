using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers.Events
{
    public class MonoStepCompleteEvent : MonoStoppingEvent, IDebugStepCompleteEvent2
    {
        public const string IID = "0f7f24c1-74d9-4ea6-a3ea-7edb2d81441d"; 
    }
}
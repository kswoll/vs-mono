using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers.Events
{
    public class MonoProgramCreateEvent : AsynchronousEvent, IDebugProgramCreateEvent2
    {
        public const string IID = "96CD11EE-ECD4-4E89-957E-B5D496FC4139";
        
        internal static void Send(MonoEngine engine)
        {
            MonoProgramCreateEvent eventObject = new MonoProgramCreateEvent();
            engine.Send(eventObject, IID, null);
        }
    }
}
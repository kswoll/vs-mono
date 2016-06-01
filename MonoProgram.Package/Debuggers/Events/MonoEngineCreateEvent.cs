using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers.Events
{
    public class MonoEngineCreateEvent : AsynchronousEvent, IDebugEngineCreateEvent2
    {
        public const string IID = "FE5B734C-759D-4E59-AB04-F103343BDD06";

        private readonly IDebugEngine2 engine;

        public MonoEngineCreateEvent(MonoEngine engine)
        {
            this.engine = engine;
        }

        public static void Send(MonoEngine engine)
        {
            MonoEngineCreateEvent eventObject = new MonoEngineCreateEvent(engine);
            engine.Send(eventObject, IID, null, null);
        }
        
        int IDebugEngineCreateEvent2.GetEngine(out IDebugEngine2 engine)
        {
            engine = this.engine;
            
            return VSConstants.S_OK;
        }
    }
}
using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public static class MonoCallbackExtensions
    {
        public static void Send(this IDebugEventCallback2 callback, MonoEngine engine, IDebugEvent2 eventObject, string iidEvent, IDebugProgram2 program, IDebugThread2 thread)
        {
            uint attributes; 
            var riidEvent = new Guid(iidEvent);
            eventObject.GetAttributes(out attributes);
            callback.Event(engine, null, program, thread, eventObject, ref riidEvent, attributes);
        }

        public static void Send(this IDebugEventCallback2 callback, MonoEngine engine, IDebugEvent2 eventObject, string iidEvent, IDebugThread2 thread)
        {
            callback.Send(engine, eventObject, iidEvent, engine, thread);
        }
    }
}
using System.Collections.Generic;
using Mono.Debugging.Client;

namespace MonoProgram.Package.Debuggers
{
    public class MonoBreakpointManager
    {
        public MonoEngine Engine { get; }
        public MonoPendingBreakpoint this[BreakEvent breakEvent] => breakpoints[breakEvent];
        public Catchpoint this[string exceptionName] => catchpoints[exceptionName];
        public IEnumerable<Catchpoint> Catchpoints => catchpoints.Values;
        public bool ContainsCatchpoint(string exceptionName) => catchpoints.ContainsKey(exceptionName);

        private readonly Dictionary<BreakEvent, MonoPendingBreakpoint> breakpoints = new Dictionary<BreakEvent, MonoPendingBreakpoint>();
        private readonly Dictionary<string, Catchpoint> catchpoints = new Dictionary<string, Catchpoint>();

        public MonoBreakpointManager(MonoEngine engine)
        {
            Engine = engine;
        }

        public void Add(BreakEvent breakEvent, MonoPendingBreakpoint pendingBreakpoint) 
        {
            breakpoints[breakEvent] = pendingBreakpoint;
        }

        public void Remove(BreakEvent breakEvent)
        {
            Engine.Session.Breakpoints.Remove(breakEvent);
            breakpoints.Remove(breakEvent);
        }

        public void Add(Catchpoint catchpoint)
        {
            catchpoints[catchpoint.ExceptionName] = catchpoint;
        }

        public void Remove(Catchpoint catchpoint)
        {
            catchpoints.Remove(catchpoint.ExceptionName);
        }
    }
}
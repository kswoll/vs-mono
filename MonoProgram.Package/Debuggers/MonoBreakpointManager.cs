using System.Collections.Generic;
using Mono.Debugging.Client;

namespace MonoProgram.Package.Debuggers
{
    public class MonoBreakpointManager
    {
        public MonoEngine Engine { get; }
        public MonoPendingBreakpoint this[BreakEvent breakEvent] => breakpoints[breakEvent];

        private readonly Dictionary<BreakEvent, MonoPendingBreakpoint> breakpoints = new Dictionary<BreakEvent, MonoPendingBreakpoint>();

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
    }
}
using System;
using System.Collections.Concurrent;

namespace MonoProgram.Package.Debuggers
{
    public class ConnectionManager
    {
        private static readonly ConcurrentDictionary<Tuple<string, string>, Connection> connections = new ConcurrentDictionary<Tuple<string, string>, Connection>();

        public static Connection Get(MonoDebuggerSettings settings)
        {
            return connections.GetOrAdd(Tuple.Create(settings.Host, settings.Username), _ => new Connection(settings.Host, settings.Username, settings.Password));
        }
    }
}
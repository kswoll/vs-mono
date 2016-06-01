using System.Collections.Generic;
using Mono.Debugging.Client;

namespace MonoProgram.Package.Debuggers
{
    public class MonoThreadManager
    {
        public MonoEngine Engine { get; }

        private readonly Dictionary<long, MonoThread> threads = new Dictionary<long, MonoThread>();

        public MonoThreadManager(MonoEngine engine)
        {
            Engine = engine;
        }

        public MonoThread this[ThreadInfo thread]
        {
            get
            {
                var result = threads[thread.Id];
                result.SetDebuggedThread(thread);
                return result;
            }
        }

        public void Add(ThreadInfo thread, MonoThread monoThread) 
        {
            threads[thread.Id] = monoThread;
        }

        public void Remove(ThreadInfo thread)
        {
            threads.Remove(thread.Id);
        }

        public IEnumerable<MonoThread> All => threads.Values;
    }
}
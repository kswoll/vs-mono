using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;
using MonoProgram.Package.Debuggers.Events;

namespace MonoProgram.Package.Debuggers
{
    public class MonoExpression : IDebugExpression2
    {
        public string Expression { get; }

        private MonoEngine engine;
        private MonoThread thread;
        private readonly ObjectValue value;
        private CancellationTokenSource cancellationToken;

        public MonoExpression(MonoEngine engine, MonoThread thread, string expression, ObjectValue value)
        {
            this.engine = engine;
            this.thread = thread;
            this.value = value;
            Expression = expression;
        }

        public int EvaluateAsync(enum_EVALFLAGS flags, IDebugEventCallback2 callback)
        {
            cancellationToken = new CancellationTokenSource();
            Task.Run(
                () =>
                {
                    IDebugProperty2 result;
                    EvaluateSync(flags, uint.MaxValue, callback, out result);
                    callback = new MonoCallbackWrapper(callback ?? engine.Callback);
                    callback.Send(engine, new MonoExpressionCompleteEvent(engine, thread, value, Expression), MonoExpressionCompleteEvent.IID, thread);
                }, 
                cancellationToken.Token);
            return VSConstants.S_OK;
        }

        public int Abort()
        {
            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
                cancellationToken = null;
                return VSConstants.S_OK;
            }
            else
            {
                return VSConstants.S_FALSE;
            }
        }

        public int EvaluateSync(enum_EVALFLAGS flags, uint timeout, IDebugEventCallback2 callback, out IDebugProperty2 result)
        {
            result = new MonoProperty(Expression, value);
            return VSConstants.S_OK;
        }
    }
}
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;

namespace MonoProgram.Package.Debuggers.Events
{
    public class MonoExpressionCompleteEvent : AsynchronousEvent, IDebugExpressionEvaluationCompleteEvent2
    {
        public const string IID = "C0E13A85-238A-4800-8315-D947C960A843";

        private readonly MonoEngine engine;
        private readonly MonoThread thread;
        private readonly ObjectValue value;
        private readonly string expression;
        private readonly IDebugProperty2 property;

        public MonoExpressionCompleteEvent(MonoEngine engine, MonoThread thread, ObjectValue value, string expression, IDebugProperty2 property = null)
        {
            this.engine = engine;
            this.thread = thread;
            this.value = value;
            this.expression = expression;
            this.property = property;
        }

        public int GetExpression(out IDebugExpression2 expr)
        {
            expr = new MonoExpression(engine, thread, expression, value);
            return VSConstants.S_OK;
        }

        public int GetResult(out IDebugProperty2 prop)
        {
            prop = property ?? new MonoProperty(expression, value);
            return VSConstants.S_OK;
        }
    }
}
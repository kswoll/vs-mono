using System;

namespace MonoProgram.Package.Utils
{
    public class ComponentException : Exception
    {
        public int Code { get; }

        public ComponentException(int code)
        {
            Code = code;
        }
    }
}
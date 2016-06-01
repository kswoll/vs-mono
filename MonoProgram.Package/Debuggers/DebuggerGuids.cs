using System;

namespace MonoProgram.Package.Debuggers
{
    public static class DebuggerGuids
    {
        public static readonly Guid CSharpLanguageService = new Guid("{694DD9B6-B865-4C5B-AD85-86356E9C88DC}");

        static private Guid _guidFilterRegisters = new Guid("223ae797-bd09-4f28-8241-2763bdc5f713");
        static public Guid guidFilterRegisters
        {
            get { return _guidFilterRegisters; }
        }

        static private Guid _guidFilterLocals = new Guid("b200f725-e725-4c53-b36a-1ec27aef12ef");
        static public Guid guidFilterLocals
        {
            get { return _guidFilterLocals; }
        }

        static private Guid _guidFilterAllLocals = new Guid("196db21f-5f22-45a9-b5a3-32cddb30db06");
        static public Guid guidFilterAllLocals
        {
            get { return _guidFilterAllLocals; }
        }

        static private Guid _guidFilterArgs = new Guid("804bccea-0475-4ae7-8a46-1862688ab863");
        static public Guid guidFilterArgs
        {
            get { return _guidFilterArgs; }
        }

        static private Guid _guidFilterLocalsPlusArgs = new Guid("e74721bb-10c0-40f5-807f-920d37f95419");
        static public Guid guidFilterLocalsPlusArgs
        {
            get { return _guidFilterLocalsPlusArgs; }
        }

        static private Guid _guidFilterAllLocalsPlusArgs = new Guid("939729a8-4cb0-4647-9831-7ff465240d5f");
        static public Guid guidFilterAllLocalsPlusArgs
        {
            get { return _guidFilterAllLocalsPlusArgs; }
        }
    }
}
using System;

namespace MonoProgram.Package.Debuggers
{
    public class Guids
    {
        public const string EngineId = "{452B50F7-4558-478D-B3CD-ACDC2EA6F345}";
        public const string AttachCommandGroupGuidString = "A0BC1490-8E97-4524-B94F-B70C8A21F8E3";
        public static readonly Guid AttachCommandGroupGuid = new Guid(AttachCommandGroupGuidString);
        public const uint AttachCommandId = 0x100;

    }
}
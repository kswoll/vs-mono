using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoBreakpointResolution : IDebugBreakpointResolution2
    {
        private readonly MonoEngine engine;
        private readonly uint address;
        private readonly MonoDocumentContext documentContext;

        public MonoBreakpointResolution(MonoEngine engine, uint address, MonoDocumentContext documentContext)
        {
            this.engine = engine;
            this.address = address;
            this.documentContext = documentContext;
        }

        public int GetBreakpointType(enum_BP_TYPE[] type)
        {
            type[0] = enum_BP_TYPE.BPT_CODE;
            return VSConstants.S_OK;
        }

        public int GetResolutionInfo(enum_BPRESI_FIELDS fields, BP_RESOLUTION_INFO[] resolutionInfo)
        {
	        if ((fields & enum_BPRESI_FIELDS.BPRESI_BPRESLOCATION) != 0) 
            {
                // The sample engine only supports code breakpoints.
                var location = new BP_RESOLUTION_LOCATION();
                location.bpType = (uint)enum_BP_TYPE.BPT_CODE;

                // The debugger will not QI the IDebugCodeContex2 interface returned here. We must pass the pointer
                // to IDebugCodeContex2 and not IUnknown.
                MonoMemoryAddress codeContext = new MonoMemoryAddress(engine, address, documentContext);
                location.unionmember1 = Marshal.GetComInterfaceForObject(codeContext, typeof(IDebugCodeContext2));
                resolutionInfo[0].bpResLocation = location;
                resolutionInfo[0].dwFields |= enum_BPRESI_FIELDS.BPRESI_BPRESLOCATION;

            }


            return VSConstants.S_OK;
        }
    }
}
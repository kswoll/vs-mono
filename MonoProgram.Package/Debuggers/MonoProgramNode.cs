using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoProgramNode : IDebugProgramNode2
    {
        private readonly AD_PROCESS_ID processId;

        public MonoProgramNode(AD_PROCESS_ID processId)
        {
            this.processId = processId;
        }

        public int GetProgramName(out string programName)
        {
            programName = null;
            return VSConstants.S_OK;
        }

        public int GetHostName(enum_GETHOSTNAME_TYPE hostNameType, out string hostName)
        {
            hostName = null;
            return VSConstants.S_OK;
        }

        public int GetHostPid(AD_PROCESS_ID[] hostProcessIds)
        {
            hostProcessIds[0] = processId;
            return VSConstants.S_OK;
        }

        public int GetHostMachineName_V7(out string hostMachineName)
        {
            hostMachineName = null;
            return VSConstants.E_NOTIMPL;
        }

        public int Attach_V7(IDebugProgram2 pMDMProgram, IDebugEventCallback2 pCallback, uint dwReason)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetEngineInfo(out string engineName, out Guid engineGuid)
        {
            engineName = "Mono Debug Engine";
            engineGuid = new Guid(Guids.EngineId);
            return VSConstants.S_OK;
        }

        public int DetachDebugger_V7()
        {
            return VSConstants.E_NOTIMPL;
        }
    }
}
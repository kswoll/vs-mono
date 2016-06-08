using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace MonoProgram.Package.Debuggers
{
    public class MonoModule : IDebugModule3
    {
        private readonly MonoEngine engine;

        public MonoModule(MonoEngine engine)
        {
            this.engine = engine;
        }

        public int GetInfo(enum_MODULE_INFO_FIELDS dwFields, MODULE_INFO[] pinfo)
        {
            var info = new MODULE_INFO();

            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_NAME) != 0)
            {
                info.m_bstrName = System.IO.Path.GetFileName(engine.Session.VirtualMachine.RootDomain.GetEntryAssembly().Location);
                info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_NAME;
            }
            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_URL) != 0)
            {
                info.m_bstrUrl = engine.Session.VirtualMachine.RootDomain.GetEntryAssembly().Location;
                info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_URL;
            }
            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_LOADADDRESS) != 0)
            {
                info.m_addrLoadAddress = 0;
                info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_LOADADDRESS;
            }
            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_PREFFEREDADDRESS) != 0)
            {
                info.m_addrPreferredLoadAddress = 0;
                info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_PREFFEREDADDRESS;
            }
            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_SIZE) != 0)
            {
                info.m_dwSize = 0;
                info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_SIZE;
            }
            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_LOADORDER) != 0)
            {
                info.m_dwLoadOrder = 0;
                info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_LOADORDER;
            }
            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_URLSYMBOLLOCATION) != 0)
            {
/*
                if (this.DebuggedModule.SymbolsLoaded)
                {
                    info.m_bstrUrlSymbolLocation = this.DebuggedModule.SymbolPath;
                    info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_URLSYMBOLLOCATION;
                }
*/
            }
            if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_FLAGS) != 0)
            {
                info.m_dwModuleFlags = 0;
//                if (this.DebuggedModule.SymbolsLoaded)
//                {
                    info.m_dwModuleFlags |= (enum_MODULE_FLAGS.MODULE_FLAG_SYMBOLS);
//                }

/*
                if (this.Process.Is64BitArch)
                {
                    info.m_dwModuleFlags |= enum_MODULE_FLAGS.MODULE_FLAG_64BIT;
                }
*/

                info.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_FLAGS;
            }

            pinfo[0] = info;

            return VSConstants.S_OK;
            
        }

        [Obsolete]
        public int ReloadSymbols_Deprecated(string pszUrlToSymbols, out string pbstrDebugMessage)
        {
            throw new NotImplementedException();
        }

        public int GetSymbolInfo(enum_SYMBOL_SEARCH_INFO_FIELDS dwFields, MODULE_SYMBOL_SEARCH_INFO[] pinfo)
        {
            return VSConstants.S_OK;
        }

        public int LoadSymbols()
        {
            throw new NotImplementedException();
        }

        public int IsUserCode(out int pfUser)
        {
            pfUser = 1;
            return VSConstants.S_OK;
        }

        public int SetJustMyCodeState(int fIsUserCode)
        {
            throw new NotImplementedException();
        }
    }
}
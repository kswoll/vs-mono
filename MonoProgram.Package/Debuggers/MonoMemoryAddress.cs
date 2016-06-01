using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using MonoProgram.Package.Utils;

namespace MonoProgram.Package.Debuggers
{
    public class MonoMemoryAddress : IDebugCodeContext2, IDebugCodeContext100
    {
        private readonly MonoEngine engine;
        private readonly uint address;
        private readonly MonoDocumentContext documentContext;

        public MonoMemoryAddress(MonoEngine engine, uint address, MonoDocumentContext documentContext)
        {
            this.engine = engine;
            this.address = address;
            this.documentContext = documentContext;
        }

        public int GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int GetInfo(enum_CONTEXT_INFO_FIELDS fields, CONTEXT_INFO[] info)
        {
            info[0].dwFields = 0;

            if ((fields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS) != 0)
            {
                info[0].bstrAddress = address.ToString();
                info[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS;
            }

            // Fields not supported by the sample
            if ((fields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSOFFSET) != 0) {}
            if ((fields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSABSOLUTE) != 0) {}
            if ((fields & enum_CONTEXT_INFO_FIELDS.CIF_MODULEURL) != 0) {}
            if ((fields & enum_CONTEXT_INFO_FIELDS.CIF_FUNCTION) != 0) {}
            if ((fields & enum_CONTEXT_INFO_FIELDS.CIF_FUNCTIONOFFSET) != 0) {}

            return VSConstants.S_OK;
        }

        public int Add(ulong dwCount, out IDebugMemoryContext2 newAddress)
        {
            newAddress = new MonoMemoryAddress(engine, (uint)dwCount + address, documentContext);
            return VSConstants.S_OK;
        }

        public int Subtract(ulong dwCount, out IDebugMemoryContext2 ppMemCxt)
        {
            ppMemCxt = new MonoMemoryAddress(engine, (uint)dwCount - address, documentContext);
            return VSConstants.S_OK;
        }

        public int Compare(enum_CONTEXT_COMPARE compare, IDebugMemoryContext2[] compareToItems, uint compareToLength, out uint foundIndex)
        {
            foundIndex = uint.MaxValue;

            try
            {
                enum_CONTEXT_COMPARE contextCompare = compare;

                for (uint c = 0; c < compareToLength; c++)
                {
                    MonoMemoryAddress compareTo = compareToItems[c] as MonoMemoryAddress;
                    if (compareTo == null)
                    {
                        continue;
                    }

                    if (!ReferenceEquals(this.engine, compareTo.engine))
                    {
                        continue;
                    }

                    bool result;

                    switch (contextCompare)
                    {
                        case enum_CONTEXT_COMPARE.CONTEXT_EQUAL:
                            result = address == compareTo.address;
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_LESS_THAN:
                            result = address < compareTo.address;
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_GREATER_THAN:
                            result = address > compareTo.address;
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_LESS_THAN_OR_EQUAL:
                            result = address <= compareTo.address;
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_GREATER_THAN_OR_EQUAL:
                            result = address >= compareTo.address;
                            break;

                        // The sample debug engine doesn't understand scopes or functions
                        case enum_CONTEXT_COMPARE.CONTEXT_SAME_SCOPE:
                        case enum_CONTEXT_COMPARE.CONTEXT_SAME_FUNCTION:
                            result = address == compareTo.address;
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_SAME_MODULE:
                            result = address == compareTo.address;
                            if (!result)
                            {
/*
                                DebuggedModule module = engine.DebuggedProcess.ResolveAddress(address);

                                if (module != null)
                                {
                                    result = (compareTo.address >= module.BaseAddress) &&
                                        (compareTo.address < module.BaseAddress + module.Size);
                                }
*/
                            }
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_SAME_PROCESS:
                            result = true;
                            break;

                        default:
                            // A new comparison was invented that we don't support
                            return VSConstants.E_NOTIMPL;
                    }

                    if (result)
                    {
                        foundIndex = c;
                        return VSConstants.S_OK;
                    }
                }

                return VSConstants.S_FALSE;
            }
            catch (ComponentException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }
        }

        public int GetDocumentContext(out IDebugDocumentContext2 context)
        {
            context = documentContext;
            return VSConstants.S_OK;
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            if (documentContext != null)
            {
                documentContext.GetLanguageInfo(ref pbstrLanguage, ref pguidLanguage);
                return VSConstants.S_OK;
            }
            else
            {
                return VSConstants.S_FALSE;
            }
        }

        public int GetProgram(out IDebugProgram2 program)
        {
            program = engine;
            return VSConstants.S_OK;
        }
    }
}
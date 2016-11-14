using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using MonoProgram.Package.Utils;

namespace MonoProgram.Package.Debuggers
{
    public class MonoDocumentContext : IDebugDocumentContext2
    {
        private readonly string fileName;
        private readonly TEXT_POSITION start;
        private readonly TEXT_POSITION end;
        private readonly MonoMemoryAddress address;

        public MonoDocumentContext(string fileName, TEXT_POSITION start, TEXT_POSITION end, MonoMemoryAddress address)
        {
            this.fileName = fileName;
            this.start = start;
            this.end = end;
            this.address = address;
        }

        public int GetDocument(out IDebugDocument2 document)
        {
            document = null;
            return VSConstants.E_FAIL;
        }

        public int GetName(enum_GETNAME_TYPE gnType, out string pbstrFileName)
        {
            pbstrFileName = fileName;
            return VSConstants.S_OK;
        }

        public int EnumCodeContexts(out IEnumDebugCodeContexts2 ppEnumCodeCxts)
        {
            ppEnumCodeCxts = null;
            try
            {
                var codeContexts = new MonoMemoryAddress[1];
                codeContexts[0] = address;
                ppEnumCodeCxts = new MonoCodeContextEnum(codeContexts);
                return VSConstants.S_OK;
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

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            pbstrLanguage = "C#";
            pguidLanguage = DebuggerGuids.CSharpLanguageService;
            return VSConstants.S_OK;
        }

        public int GetStatementRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
        {
            try
            {
                pBegPosition[0].dwColumn = start.dwColumn;
                pBegPosition[0].dwLine = start.dwLine;

                pEndPosition[0].dwColumn = end.dwColumn;
                pEndPosition[0].dwLine = end.dwLine;
            }
            catch (ComponentException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }

            return VSConstants.S_OK;
        }

        public int GetSourceRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
        {
            throw new NotImplementedException("This method is not implemented");
        }

        public int Compare(enum_DOCCONTEXT_COMPARE Compare, IDebugDocumentContext2[] rgpDocContextSet, uint dwDocContextSetLen, out uint pdwDocContext)
        {
            pdwDocContext = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int Seek(int nCount, out IDebugDocumentContext2 ppDocContext)
        {
            ppDocContext = null;
            return VSConstants.E_NOTIMPL;
        }
    }
}
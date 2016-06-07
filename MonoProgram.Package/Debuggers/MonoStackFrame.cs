using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;
using MonoProgram.Package.Utils;
using StackFrame = Mono.Debugging.Client.StackFrame;

namespace MonoProgram.Package.Debuggers
{
    public class MonoStackFrame : IDebugStackFrame2, IDebugExpressionContext2
    {
        private readonly MonoEngine engine;
        private readonly MonoThread thread;

        private readonly string documentName;       
        private readonly string functionName;
        private readonly bool hasSource;
        private readonly Func<StackFrame> frame;
        private int? lineNumberOverride;

        // An array of this frame's parameters
        private readonly ObjectValue[] parameters;

        // An array of this frame's locals
        private readonly ObjectValue[] locals;     

        public MonoStackFrame(MonoEngine engine, MonoThread thread, Func<StackFrame> frame)
        {
            this.engine = engine;
            this.thread = thread;
            this.frame = frame;

            var allLocals = frame().GetAllLocals(EvaluationOptions.DefaultOptions);
            parameters = frame().GetParameters(EvaluationOptions.DefaultOptions);
            locals = allLocals.Where(x => !parameters.Any(y => y.Name == x.Name)).ToArray();
            hasSource = frame().HasDebugInfo;
            functionName = frame().SourceLocation.MethodName;
            documentName = frame().SourceLocation.FileName;
        }

        public int LineNumber
        {
            get { return lineNumberOverride ?? frame().SourceLocation.Line; }
            set { lineNumberOverride = value; }
        }

        // Construct a FRAMEINFO for this stack frame with the requested information.
        public void SetFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, out FRAMEINFO frameInfo)
        {
            frameInfo = new FRAMEINFO();
            var frame = this.frame();

            // The debugger is asking for the formatted name of the function which is displayed in the callstack window.
            // There are several optional parts to this name including the module, argument types and values, and line numbers.
            // The optional information is requested by setting flags in the dwFieldSpec parameter.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME) != 0)
            {
                // If there is source information, construct a string that contains the module name, function name, and optionally argument names and values.
                if (hasSource)
                {
                    frameInfo.m_bstrFuncName = "";

                    if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_MODULE) != 0)
                    {
                        frameInfo.m_bstrFuncName = System.IO.Path.GetFileName(frame.FullModuleName) + "!";
                    }

                    frameInfo.m_bstrFuncName += functionName;

                    if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS) != 0 && parameters.Length > 0)
                    {
                        frameInfo.m_bstrFuncName += "(";
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_TYPES) != 0)
                            {
                                frameInfo.m_bstrFuncName += parameters[i].TypeName + " ";
                            }

                            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_NAMES) != 0)
                            {
                                frameInfo.m_bstrFuncName += parameters[i].Name;
                            }

                            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_VALUES) != 0)
                            {
                                frameInfo.m_bstrFuncName += "=" + parameters[i].Value;
                            }

                            if (i < parameters.Length - 1)
                            {
                                frameInfo.m_bstrFuncName += ", ";
                            }
                        }
                        frameInfo.m_bstrFuncName += ")";
                    }

                    if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_LINES) != 0)
                    {
                        frameInfo.m_bstrFuncName += " Line:" + (uint)LineNumber;
                    }
                }
                else
                {                   
                    // No source information, so only return the module name and the instruction pointer.
                     frameInfo.m_bstrFuncName = frame.AddressSpace;
                }
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FUNCNAME;
            }

            // The debugger is requesting the name of the module for this stack frame.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_MODULE) != 0)
            {
                frameInfo.m_bstrModule = frame.FullModuleName;
                frameInfo.m_dwValidFields |=  enum_FRAMEINFO_FLAGS.FIF_MODULE;
            }

            // The debugger is requesting the range of memory addresses for this frame.
            // For the sample engine, this is the contents of the frame pointer.
            if ((dwFieldSpec &  enum_FRAMEINFO_FLAGS.FIF_STACKRANGE) != 0)
            {
                frameInfo.m_addrMin = (ulong)frame.Address;
                frameInfo.m_addrMax = (ulong)frame.Address;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_STACKRANGE;
            }

            // The debugger is requesting the IDebugStackFrame2 value for this frame info.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FRAME) != 0)
            {
                frameInfo.m_pFrame = this;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FRAME;
            }
            
            // Does this stack frame of symbols loaded?
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO) != 0)
            {
                frameInfo.m_fHasDebugInfo = hasSource ? 1 : 0;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO;
            }

            // Is this frame stale?
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_STALECODE) != 0)
            {
                frameInfo.m_fStaleCode = 0;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_STALECODE;
            }

            // The debugger would like a pointer to the IDebugModule2 that contains this stack frame.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_DEBUG_MODULEP) != 0)
            {
/*
                if (module != null)
                {
                    AD7Module ad7Module = (AD7Module)module.Client;
                    Debug.Assert(ad7Module != null);
                    frameInfo.m_pModule = ad7Module;
                    frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_DEBUG_MODULEP;
                }
*/
            }
        }

        // Construct an instance of IEnumDebugPropertyInfo2 for the combined locals and parameters.
        private void CreateLocalsPlusArgsProperties(out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            elementsReturned = 0;
    
            int localsLength = 0;

            if (locals != null)
            {
                localsLength = locals.Length;
                elementsReturned += (uint)localsLength;
            }

            if (parameters != null)
            {
                elementsReturned += (uint)parameters.Length;
            }
            var propInfo = new DEBUG_PROPERTY_INFO[elementsReturned];

            if (locals != null)
            {
                for (int i = 0; i < locals.Length; i++)
                {
                    MonoProperty property = new MonoProperty(locals[i].Name, locals[i]);
                    propInfo[i] = property.ConstructDebugPropertyInfo(enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_STANDARD);
                }
            }

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {                   
                    MonoProperty property = new MonoProperty(parameters[i].Name, parameters[i]);
                    propInfo[localsLength + i] = property.ConstructDebugPropertyInfo(enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_STANDARD); 
                }
            }

            enumObject = new MonoPropertyInfoEnum(propInfo);
        }

        // Construct an instance of IEnumDebugPropertyInfo2 for the locals collection only.
        private void CreateLocalProperties(out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            elementsReturned = (uint)locals.Length;
            var propInfo = new DEBUG_PROPERTY_INFO[locals.Length];

            for (int i = 0; i < propInfo.Length; i++)
            {
                MonoProperty property = new MonoProperty(locals[i].Name, locals[i]);
                propInfo[i] = property.ConstructDebugPropertyInfo(enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_STANDARD);
            }

            enumObject = new MonoPropertyInfoEnum(propInfo);
        }

        // Construct an instance of IEnumDebugPropertyInfo2 for the parameters collection only.
        private void CreateParameterProperties(out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            elementsReturned = (uint)parameters.Length;
            var propInfo = new DEBUG_PROPERTY_INFO[parameters.Length];

            for (int i = 0; i < propInfo.Length; i++)
            {
                MonoProperty property = new MonoProperty(parameters[i].Name, parameters[i]);
                propInfo[i] = property.ConstructDebugPropertyInfo(enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_STANDARD);
            }

            enumObject = new MonoPropertyInfoEnum(propInfo);
        }

        // Creates an enumerator for properties associated with the stack frame, such as local variables.
        // The sample engine only supports returning locals and parameters. Other possible values include
        // class fields (this pointer), registers, exceptions...
        int IDebugStackFrame2.EnumProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, uint nRadix, ref Guid guidFilter, uint dwTimeout, out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            int hr;

            elementsReturned = 0;
            enumObject = null;
            
            try
            {
                if (guidFilter == DebuggerGuids.guidFilterLocalsPlusArgs ||
                        guidFilter == DebuggerGuids.guidFilterAllLocalsPlusArgs ||
                        guidFilter == DebuggerGuids.guidFilterAllLocals)        
                {
                    CreateLocalsPlusArgsProperties(out elementsReturned, out enumObject);
                    hr = VSConstants.S_OK;
                }
                else if (guidFilter == DebuggerGuids.guidFilterLocals)
                {
                    CreateLocalProperties(out elementsReturned, out enumObject);
                    hr = VSConstants.S_OK;
                }
                else if (guidFilter == DebuggerGuids.guidFilterArgs)
                {
                    CreateParameterProperties(out elementsReturned, out enumObject);
                    hr = VSConstants.S_OK;
                }
                else
                {
                    hr = VSConstants.E_NOTIMPL;
                }
            }
            catch (ComponentException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }
            
            return hr;
        }

        // Gets the code context for this stack frame. The code context represents the current instruction pointer in this stack frame.
        int IDebugStackFrame2.GetCodeContext(out IDebugCodeContext2 memoryAddress)
        {
            memoryAddress = null;

            try
            {
                memoryAddress = new MonoMemoryAddress(engine, (uint)frame().Address, null);
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

        // Gets a description of the properties of a stack frame.
        // Calling the IDebugProperty2::EnumChildren method with appropriate filters can retrieve the local variables, method parameters, registers, and "this" 
        // pointer associated with the stack frame. The debugger calls EnumProperties to obtain these values in the sample.
        int IDebugStackFrame2.GetDebugProperty(out IDebugProperty2 property)
        {
            throw new NotImplementedException();
        }

        // Gets the document context for this stack frame. The debugger will call this when the current stack frame is changed
        // and will use it to open the correct source document for this stack frame.
        int IDebugStackFrame2.GetDocumentContext(out IDebugDocumentContext2 docContext)
        {
            docContext = null;
            try
            {
                if (hasSource)
                {
                    // Assume all lines begin and end at the beginning of the line.
                    // TODO: Accurate line endings
                    var lineNumber = (uint)LineNumber;
                    TEXT_POSITION begTp = new TEXT_POSITION();
                    begTp.dwColumn = 0;
                    begTp.dwLine = lineNumber - 1;
                    TEXT_POSITION endTp = new TEXT_POSITION();
                    endTp.dwColumn = 0;
                    endTp.dwLine = lineNumber - 1;

                    docContext = new MonoDocumentContext(engine.TranslateToLocalPath(documentName), begTp, endTp, null);
                    return VSConstants.S_OK;
                }
            }
            catch (ComponentException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }

            return VSConstants.S_FALSE;
        }

        // Gets an evaluation context for expression evaluation within the current context of a stack frame and thread.
        // Generally, an expression evaluation context can be thought of as a scope for performing expression evaluation. 
        // Call the IDebugExpressionContext2::ParseText method to parse an expression and then call the resulting IDebugExpression2::EvaluateSync 
        // or IDebugExpression2::EvaluateAsync methods to evaluate the parsed expression.
        int IDebugStackFrame2.GetExpressionContext(out IDebugExpressionContext2 ppExprCxt)
        {
            ppExprCxt = this;
            return VSConstants.S_OK;
        }

        // Gets a description of the stack frame.
        int IDebugStackFrame2.GetInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, FRAMEINFO[] pFrameInfo)
        {
            try
            {
                SetFrameInfo(dwFieldSpec, out pFrameInfo[0]);

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

        // Gets the language associated with this stack frame. 
        // In this sample, all the supported stack frames are C++
        int IDebugStackFrame2.GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            pbstrLanguage = "C#";
            pguidLanguage = DebuggerGuids.CSharpLanguageService; 
            return VSConstants.S_OK;
        }

        // Gets the name of the stack frame.
        // The name of a stack frame is typically the name of the method being executed.
        int IDebugStackFrame2.GetName(out string name)
        {
            name = frame().SourceLocation.MethodName;
            return VSConstants.S_OK;
        }

        // Gets a machine-dependent representation of the range of physical addresses associated with a stack frame.
        int IDebugStackFrame2.GetPhysicalStackRange(out ulong addrMin, out ulong addrMax)
        {
            addrMin = 0;
            addrMax = 0;
            return VSConstants.S_OK;
        }

        // Gets the thread associated with a stack frame.
        int IDebugStackFrame2.GetThread(out IDebugThread2 thread)
        {
            thread = this.thread;
            return VSConstants.S_OK;
        }

        // Retrieves the name of the evaluation context. 
        // The name is the description of this evaluation context. It is typically something that can be parsed by an expression evaluator 
        // that refers to this exact evaluation context. For example, in C++ the name is as follows: 
        // "{ function-name, source-file-name, module-file-name }"
        int IDebugExpressionContext2.GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        // Parses a text-based expression for evaluation.
        // The engine sample only supports locals and parameters so the only task here is to check the names in those collections.
        int IDebugExpressionContext2.ParseText(string code,
                                                enum_PARSEFLAGS dwFlags, 
                                                uint nRadix, 
                                                out IDebugExpression2 expression, 
                                                out string error, 
                                                out uint pichError)
        {
            var frame = this.frame();
            error = null;
            pichError = 0;
            try
            {
                if (frame.ValidateExpression(code))
                {
                    var value = frame.GetExpressionValue(code, EvaluationOptions.DefaultOptions);
                    expression = new MonoExpression(engine, thread, code, value);
                    return VSConstants.S_OK;
                }
                else
                {
                    expression = null;
                    return VSConstants.S_FALSE;                    
                }
            }
            catch (Exception e)
            {
                expression = null;
                Debug.WriteLine("Unexpected exception during Attach: \r\n" + e);
                return VSConstants.S_FALSE;
            }
        }
    }
}


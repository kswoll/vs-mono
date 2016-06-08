using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;
using MonoProgram.Package.Debuggers.Events;
using MonoProgram.Package.Utils;

namespace MonoProgram.Package.Debuggers
{
    public class MonoThread : IDebugThread2, IDebugThread100
    {
        private readonly MonoEngine engine;
        const string ThreadNameString = "Sample Engine Thread";
        private string threadDisplayName;
        private uint threadFlags;
        private ThreadInfo debuggedThread;
        private int? lineNumberOverride;

        public MonoThread(MonoEngine engine, ThreadInfo debuggedThread)
        {
            this.engine = engine;
            this.debuggedThread = debuggedThread;
        }

        string GetCurrentLocation(bool fIncludeModuleName)
        {
            return debuggedThread.Location;
        }

        internal ThreadInfo GetDebuggedThread()
        {
            return debuggedThread;
        }

        internal void SetDebuggedThread(ThreadInfo value)
        {
            debuggedThread = value;
        }

        #region IDebugThread2 Members

        // Determines whether the next statement can be set to the given stack frame and code context.
        // The sample debug engine does not support set next statement, so S_FALSE is returned.
        int IDebugThread2.CanSetNextStatement(IDebugStackFrame2 stackFrame, IDebugCodeContext2 codeContext)
        {
            return VSConstants.S_FALSE;
        }

        // Retrieves a list of the stack frames for this thread.
        // For the sample engine, enumerating the stack frames requires walking the callstack in the debuggee for this thread
        // and coverting that to an implementation of IEnumDebugFrameInfo2. 
        // Real engines will most likely want to cache this information to avoid recomputing it each time it is asked for,
        // and or construct it on demand instead of walking the entire stack.
        int IDebugThread2.EnumFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, out IEnumDebugFrameInfo2 enumObject)
        {
            // Ask the lower-level to perform a stack walk on this thread
            enumObject = null;

            try
            {
                int numStackFrames = debuggedThread.Backtrace.FrameCount;
                FRAMEINFO[] frameInfoArray;

                if (numStackFrames == 0)
                {
                    // failed to walk any frames. Only return the top frame.
                    frameInfoArray = new FRAMEINFO[0];
//                    MonoStackFrame frame = new MonoStackFrame(engine, this, debuggedThread);
//                    frame.SetFrameInfo(dwFieldSpec, out frameInfoArray[0]);
                }
                else
                {
                    frameInfoArray = new FRAMEINFO[numStackFrames];

                    for (int i = 0; i < numStackFrames; i++)
                    {
                        var frame = new MonoStackFrame(engine, this, () => debuggedThread.Backtrace.GetFrame(i));
                        if (lineNumberOverride != null)
                            frame.LineNumber = lineNumberOverride.Value;
                        frame.SetFrameInfo(dwFieldSpec, out frameInfoArray[i]);
                    }
                }

                enumObject = new MonoFrameInfoEnum(frameInfoArray);
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

        // Get the name of the thread. For the sample engine, the name of the thread is always "Sample Engine Thread"
        int IDebugThread2.GetName(out string threadName)
        {
            threadName = ThreadNameString;
            return VSConstants.S_OK;
        }

        // Return the program that this thread belongs to.
        int IDebugThread2.GetProgram(out IDebugProgram2 program)
        {
            program = engine;
            return VSConstants.S_OK;
        }

        // Gets the system thread identifier.
        int IDebugThread2.GetThreadId(out uint threadId)
        {
            threadId = (uint)debuggedThread.Id;
            return VSConstants.S_OK;
        }

        // Gets properties that describe a thread.
        int IDebugThread2.GetThreadProperties(enum_THREADPROPERTY_FIELDS dwFields, THREADPROPERTIES[] propertiesArray)
        {
            try
            {
                THREADPROPERTIES props = new THREADPROPERTIES();

                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_ID) != 0)
                {
                    props.dwThreadId = (uint)debuggedThread.Id;
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_ID;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_SUSPENDCOUNT) != 0)
                {
                    // sample debug engine doesn't support suspending threads
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_SUSPENDCOUNT;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_STATE) != 0)
                {
                    props.dwThreadState = (uint)enum_THREADSTATE.THREADSTATE_RUNNING;
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_STATE;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_PRIORITY) != 0)
                {
                    props.bstrPriority = "Normal";
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_PRIORITY;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_NAME) != 0)
                {
                    props.bstrName = ThreadNameString;
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_NAME;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_LOCATION) != 0)
                {
                    props.bstrLocation = GetCurrentLocation(true);
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_LOCATION;
                }

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

        // Resume a thread.
        // This is called when the user chooses "Unfreeze" from the threads window when a thread has previously been frozen.
        int IDebugThread2.Resume(out uint suspendCount)
        {
            // The sample debug engine doesn't support suspending/resuming threads
            suspendCount = 0;
            return VSConstants.E_NOTIMPL;
        }

        // Sets the next statement to the given stack frame and code context.
        // The sample debug engine doesn't support set next statment
        int IDebugThread2.SetNextStatement(IDebugStackFrame2 stackFrame, IDebugCodeContext2 codeContext)
        {
            IDebugDocumentContext2 context;
            codeContext.GetDocumentContext(out context);
            string fileName;
            context.GetName(enum_GETNAME_TYPE.GN_FILENAME, out fileName);
            fileName = engine.TranslateToBuildServerPath(fileName);

            var startPosition = new TEXT_POSITION[1];
            var endPosition = new TEXT_POSITION[1];
            context.GetStatementRange(startPosition, endPosition);

            EventHandler<TargetEventArgs> stepFinished = null;
//            var waiter = new AutoResetEvent(false);
            stepFinished = (sender, args) =>
            {
                lineNumberOverride = null;
//                if (true || args.Thread.Backtrace.GetFrame(0).SourceLocation.Line == startPosition[0].dwLine)
//                {
                    engine.Session.TargetStopped -= stepFinished;
//                    waiter.Set();
//                    engine.Send(new MonoBreakpointEvent(new MonoBoundBreakpointsEnum(new IDebugBoundBreakpoint2[0])), MonoStepCompleteEvent.IID, engine.ThreadManager[args.Thread]);
//                }
//                else
//                {
//                    engine.Session.NextInstruction();
//                }
            };
            engine.Session.TargetStopped += stepFinished;
            engine.Session.SetNextStatement(fileName, (int)startPosition[0].dwLine, (int)startPosition[0].dwColumn + 1);
            lineNumberOverride = (int)startPosition[0].dwLine;
//            engine.Session.Stop();
//            engine.Session.NextInstruction();
//            waiter.WaitOne();

            return VSConstants.S_OK;
        }

        // suspend a thread.
        // This is called when the user chooses "Freeze" from the threads window
        int IDebugThread2.Suspend(out uint suspendCount)
        {
            // The sample debug engine doesn't support suspending/resuming threads
            suspendCount = 0;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IDebugThread100 Members

        int IDebugThread100.SetThreadDisplayName(string name)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM.
            return VSConstants.E_NOTIMPL;
        }

        int IDebugThread100.GetThreadDisplayName(out string name)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM, which calls GetThreadProperties100()
            name = "";
            return VSConstants.E_NOTIMPL;
        }

        // Returns whether this thread can be used to do function/property evaluation.
        int IDebugThread100.CanDoFuncEval()
        {
            return VSConstants.S_FALSE;
        }

        int IDebugThread100.SetFlags(uint flags)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM.
            return VSConstants.E_NOTIMPL;
        }

        int IDebugThread100.GetFlags(out uint flags)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM.
            flags = 0;
            return VSConstants.E_NOTIMPL;
        }

        int IDebugThread100.GetThreadProperties100(uint dwFields, THREADPROPERTIES100[] props)
        {
            int hRes = VSConstants.S_OK;

            // Invoke GetThreadProperties to get the VS7/8/9 properties
            THREADPROPERTIES[] props90 = new THREADPROPERTIES[1];
            enum_THREADPROPERTY_FIELDS dwFields90 = (enum_THREADPROPERTY_FIELDS)(dwFields & 0x3f);
            hRes = ((IDebugThread2)this).GetThreadProperties(dwFields90, props90);
            props[0].bstrLocation = props90[0].bstrLocation;
            props[0].bstrName = props90[0].bstrName;
            props[0].bstrPriority = props90[0].bstrPriority;
            props[0].dwFields = (uint)props90[0].dwFields;
            props[0].dwSuspendCount = props90[0].dwSuspendCount;
            props[0].dwThreadId = props90[0].dwThreadId;
            props[0].dwThreadState = props90[0].dwThreadState;

            // Populate the new fields
            if (hRes == VSConstants.S_OK && dwFields != (uint)dwFields90)
            {
                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_DISPLAY_NAME) != 0)
                {
                    // Thread display name is being requested
                    props[0].bstrDisplayName = debuggedThread.Name;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_DISPLAY_NAME;

                    // Give this display name a higher priority than the default (0)
                    // so that it will actually be displayed
                    props[0].DisplayNamePriority = 10;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_DISPLAY_NAME_PRIORITY;
                }

                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_CATEGORY) != 0)
                {
                    // Thread category is being requested
                    props[0].dwThreadCategory = 0;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_CATEGORY;
                }

                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_AFFINITY) != 0)
                {
                    // Thread cpu affinity is being requested
                    props[0].AffinityMask = 0;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_AFFINITY;
                }

                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_PRIORITY_ID) != 0)
                {
                    // Thread display name is being requested
                    props[0].priorityId = 0;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_PRIORITY_ID;
                }

            }

            return hRes;
        }

        #endregion

        #region Uncalled interface methods

        // These methods are not currently called by the Visual Studio debugger, so they don't need to be implemented

        int IDebugThread2.GetLogicalThread(IDebugStackFrame2 stackFrame, out IDebugLogicalThread2 logicalThread)
        {
            Debug.Fail("This function is not called by the debugger");

            logicalThread = null;
            return VSConstants.E_NOTIMPL;
        }

        int IDebugThread2.SetThreadName(string name)
        {
            Debug.Fail("This function is not called by the debugger");

            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}
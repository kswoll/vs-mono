using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;
using MonoProgram.Package.Debuggers.Events;
using MonoProgram.Package.Utils;

namespace MonoProgram.Package.Debuggers
{
    public class MonoPendingBreakpoint : IDebugPendingBreakpoint2
    {
        public MonoBoundBreakpoint[] BoundBreakpoints => boundBreakpoints.ToArray();

        private readonly MonoBreakpointManager breakpointManager;
        private readonly IDebugBreakpointRequest2 request;
        private readonly BP_REQUEST_INFO requestInfo; 
        private readonly List<MonoBoundBreakpoint> boundBreakpoints = new List<MonoBoundBreakpoint>();
        private Breakpoint breakpoint;
        private bool isDeleted;
        private bool isEnabled;

        public MonoPendingBreakpoint(MonoBreakpointManager breakpointManager, IDebugBreakpointRequest2 request)
        {
            this.breakpointManager = breakpointManager;
            this.request = request;

            var requestInfo = new BP_REQUEST_INFO[1];
            EngineUtils.CheckOk(request.GetRequestInfo(enum_BPREQI_FIELDS.BPREQI_ALLFIELDS, requestInfo));
            this.requestInfo = requestInfo[0];
        }

        public int CanBind(out IEnumDebugErrorBreakpoints2 error)
        {
            error = null;
            if (isDeleted || requestInfo.bpLocation.bpLocationType != (uint)enum_BP_LOCATION_TYPE.BPLT_CODE_FILE_LINE)
            {
                return VSConstants.S_FALSE;
            }

            return VSConstants.S_OK;
        }

        public int Bind()
        {
            TEXT_POSITION[] startPosition;
            TEXT_POSITION[] endPosition;
            var engine = breakpointManager.Engine;
            var documentName = engine.GetLocationInfo(requestInfo.bpLocation.unionmember2, out startPosition, out endPosition);
            documentName = engine.TranslateToBuildServerPath(documentName);

            breakpoint = engine.Session.Breakpoints.Add(documentName, (int)startPosition[0].dwLine + 1, (int)startPosition[0].dwColumn + 1);
            breakpointManager.Add(breakpoint, this);
            SetCondition(requestInfo.bpCondition);
            SetPassCount(requestInfo.bpPassCount);

            lock (boundBreakpoints)
            {
                uint address = 0;
                var breakpointResolution = new MonoBreakpointResolution(engine, address, GetDocumentContext(address));
                var boundBreakpoint = new MonoBoundBreakpoint(engine, address, this, breakpointResolution);
                boundBreakpoints.Add(boundBreakpoint);                    

                engine.Send(new MonoBreakpointBoundEvent(this, boundBreakpoint), MonoBreakpointBoundEvent.IID, null);
            }

            return VSConstants.S_OK;
        }

        public MonoDocumentContext GetDocumentContext(uint address)
        {
            TEXT_POSITION[] startPosition;
            TEXT_POSITION[] endPosition;
            var documentName = breakpointManager.Engine.GetLocationInfo(requestInfo.bpLocation.unionmember2, out startPosition, out endPosition);
            var codeContext = new MonoMemoryAddress(breakpointManager.Engine, address, null);
            
            return new MonoDocumentContext(documentName, startPosition[0], endPosition[0], codeContext);
        }

        public int GetState(PENDING_BP_STATE_INFO[] state)
        {
            if (isDeleted)
                state[0].state = (enum_PENDING_BP_STATE)enum_BP_STATE.BPS_DELETED;
            else if (isEnabled)
                state[0].state = (enum_PENDING_BP_STATE)enum_BP_STATE.BPS_ENABLED;
            else 
                state[0].state = (enum_PENDING_BP_STATE)enum_BP_STATE.BPS_DISABLED;

            return VSConstants.S_OK;
        }

        public int GetBreakpointRequest(out IDebugBreakpointRequest2 request)
        {
            request = this.request;
            return VSConstants.S_OK;
        }

        public int Virtualize(int fVirtualize)
        {
            return VSConstants.S_OK;
        }

        public int Enable(int enable)
        {
            lock (boundBreakpoints)
            {
                isEnabled = enable != 0;

                var breakpoint = this.breakpoint;
                if (breakpoint != null)
                {
                    breakpoint.Enabled = isEnabled;
                }

                foreach (var boundBreakpoint in boundBreakpoints)
                {
                    boundBreakpoint.Enable(enable);
                }                
            }

            return VSConstants.S_OK;
        }

        public int SetCondition(BP_CONDITION condition)
        {
            breakpoint.ConditionExpression = condition.bstrCondition;
            breakpoint.BreakIfConditionChanges = condition.styleCondition == enum_BP_COND_STYLE.BP_COND_WHEN_CHANGED;
                
            return VSConstants.S_OK;
        }

        public int SetPassCount(BP_PASSCOUNT passCount)
        {
            breakpoint.HitCount = (int)passCount.dwPassCount;
            switch (passCount.stylePassCount)
            {
                case enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_EQUAL:
                    breakpoint.HitCountMode = HitCountMode.EqualTo;
                    break;
                case enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_EQUAL_OR_GREATER:
                    breakpoint.HitCountMode = HitCountMode.GreaterThanOrEqualTo;
                    break;
                case enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_MOD:
                    breakpoint.HitCountMode = HitCountMode.MultipleOf;
                    break;
                default:
                    breakpoint.HitCountMode = HitCountMode.None;
                    break;
            }

            return VSConstants.S_OK;
        }

        public int EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 enumerator)
        {
            lock (boundBreakpoints)
            {
                enumerator = new MonoBoundBreakpointsEnum(boundBreakpoints.ToArray());
            }
            return VSConstants.S_OK;
        }

        public int EnumErrorBreakpoints(enum_BP_ERROR_TYPE errorType, out IEnumDebugErrorBreakpoints2 enumerator)
        {
            enumerator = null;
            return VSConstants.S_OK;
        }

        public int Delete()
        {
            if (!isDeleted)
            {
                isDeleted = true;
                if (breakpoint != null)
                    breakpointManager.Remove(breakpoint);

                lock (boundBreakpoints)
                {
                    for (var i = boundBreakpoints.Count - 1; i >= 0; i--)
                    {
                        boundBreakpoints[i].Delete();
                    }
                }
            }
            return VSConstants.S_OK;
        }
    }
}
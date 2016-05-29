using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace MonoProgram.Package.Projects
{
    public class MonoProgramProjectFlavor : FlavoredProjectBase, IVsProjectFlavorCfgProvider
    {
        private MonoProgramPackage package;
        private IVsProjectFlavorCfgProvider innerCfgProvider;

        public MonoProgramProjectFlavor(MonoProgramPackage package)
        {
            this.package = package;
        }

        protected override void SetInnerProject(IntPtr innerIUnknown)
        {
            // This line has to be called before the base invocation or you'll get an error complaining that the serviceProvider
            // must have been set first.
            serviceProvider = package;

            base.SetInnerProject(innerIUnknown);

            var objectForIUnknown = Marshal.GetObjectForIUnknown(innerIUnknown);
            innerCfgProvider = (IVsProjectFlavorCfgProvider)objectForIUnknown;
        }

        /// <summary>
        /// Release the innerVsProjectFlavorCfgProvider when closed.
        /// </summary>
        protected override void Close()
        {
            base.Close();

            if (innerCfgProvider != null)
            {
                Marshal.ReleaseComObject(innerCfgProvider);
                innerCfgProvider = null;
            }
        }

        public int CreateProjectFlavorCfg(IVsCfg baseProjectCfg, out IVsProjectFlavorCfg flavoredProjectCfg)
        {
            IVsProjectFlavorCfg cfg;
            innerCfgProvider.CreateProjectFlavorCfg(baseProjectCfg, out cfg);

            flavoredProjectCfg = new MonoProgramFlavorCfg(this, baseProjectCfg, cfg);
            return VSConstants.S_OK;
        }
    }
}
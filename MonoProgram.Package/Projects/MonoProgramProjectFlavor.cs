using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;
using MonoProgram.Package.ProgramProperties;

namespace MonoProgram.Package.Projects
{
    public class MonoProgramProjectFlavor : FlavoredProjectBase, IVsProjectFlavorCfgProvider
    {
        public MonoProgramPackage Package { get; }

        private IVsProjectFlavorCfgProvider innerCfgProvider;

        public MonoProgramProjectFlavor(MonoProgramPackage package)
        {
            Package = package;
        }

        protected override void SetInnerProject(IntPtr innerIUnknown)
        {
            // This line has to be called before the base invocation or you'll get an error complaining that the serviceProvider
            // must have been set first.
            serviceProvider = Package;

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

        /// <summary>
        ///     By overriding GetProperty method and using propId parameter containing one of
        ///     the values of the __VSHPROPID2 enumeration, we can filter, add or remove project
        ///     properties.
        ///     For example, to add a page to the configuration-dependent property pages, we
        ///     need to filter configuration-dependent property pages and then add a new page
        ///     to the existing list.
        /// </summary>
        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            if (propId == (int)__VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList)
            {
                // Get a semicolon-delimited list of clsids of the configuration-dependent
                // property pages.
                ErrorHandler.ThrowOnFailure(base.GetProperty(itemId, propId, out property));

                // Add the CustomPropertyPage property page.
                property += ';' + typeof(MonoPropertyPage).GUID.ToString("B");

                return VSConstants.S_OK;
            }

            return base.GetProperty(itemId, propId, out property);
        }
    }
}
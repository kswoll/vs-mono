using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace MonoProgram.Package.Projects
{
    public class MonoProgramFlavorCfg : IVsProjectFlavorCfg, IPersistXMLFragment, IVsDebuggableProjectCfg, IVsBuildableProjectCfg
    {
        /// <summary>
        /// This allows the property page to map a IVsCfg object (the baseConfiguration) to an actual instance of 
        /// CustomPropertyPageProjectFlavorCfg.
        /// </summary>
        private static readonly Dictionary<IVsCfg, MonoProgramFlavorCfg> mapIVsCfgToCustomPropertyPageProjectFlavorCfg = new Dictionary<IVsCfg, MonoProgramFlavorCfg>();

        private readonly MonoProgramProjectFlavor project;
        private readonly IVsCfg baseProjectCfg;
        private readonly IVsProjectFlavorCfg innerProjectFlavorCfg;
        private readonly IVsDebuggableProjectCfg baseDebugConfiguration;
//        private readonly IVsBuildableProjectCfg baseBuildConfiguration;
        private readonly Dictionary<string, string> propertiesList = new Dictionary<string, string>();

        private bool isClosed;
        private bool isDirty;

        public MonoProgramFlavorCfg(MonoProgramProjectFlavor project, IVsCfg baseProjectCfg, IVsProjectFlavorCfg innerProjectFlavorCfg)
        {
            this.project = project;
            this.baseProjectCfg = baseProjectCfg;
            this.innerProjectFlavorCfg = innerProjectFlavorCfg;

            var debugGuid = typeof(IVsDebuggableProjectCfg).GUID;
            IntPtr baseDebugConfigurationPtr;
		    innerProjectFlavorCfg.get_CfgType(ref debugGuid, out baseDebugConfigurationPtr);
		    baseDebugConfiguration = (IVsDebuggableProjectCfg)Marshal.GetObjectForIUnknown(baseDebugConfigurationPtr);

//            var buildGuid = typeof(IVsBuildableProjectCfg).GUID;
//            IntPtr baseBuildConfigurationPtr;
//            innerProjectFlavorCfg.get_CfgType(ref buildGuid, out baseBuildConfigurationPtr);
//            baseBuildConfiguration = (IVsBuildableProjectCfg)Marshal.GetObjectForIUnknown(baseBuildConfigurationPtr);
        }

        /// <summary>
        /// Provides access to a configuration interfaces such as IVsBuildableProjectCfg2 or IVsDebuggableProjectCfg.
		/// </summary>
		/// <param name="iidCfg">IID of the interface that is being asked</param>
		/// <param name="ppCfg">Object that implement the interface</param>
		/// <returns>HRESULT</returns>
        public int get_CfgType(ref Guid iidCfg, out IntPtr ppCfg)
        {
            ppCfg = IntPtr.Zero;
		    if (iidCfg == typeof(IVsDebuggableProjectCfg).GUID)
		    {
		        ppCfg = Marshal.GetComInterfaceForObject(this, typeof(IVsDebuggableProjectCfg));
		        return VSConstants.S_OK;
		    }
            if (iidCfg == typeof(IVsBuildableProjectCfg2).GUID)
            {
                ppCfg = Marshal.GetComInterfaceForObject(this, typeof(IVsBuildableProjectCfg2));
                return VSConstants.S_OK;
            }
            if (iidCfg == typeof(IVsBuildableProjectCfg).GUID)
            {
                ppCfg = Marshal.GetComInterfaceForObject(this, typeof(IVsBuildableProjectCfg));
                return VSConstants.S_OK;
            }
            if (innerProjectFlavorCfg != null)
            {
                return innerProjectFlavorCfg.get_CfgType(ref iidCfg, out ppCfg);
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Get or set a Property.
        /// </summary>
        public string this[string propertyName]
        {
            get
            {
                if (propertiesList.ContainsKey(propertyName))
                {
                    return propertiesList[propertyName];
                }
                return "";
            }
            set
            {
                // Don't do anything if there isn't any real change
                if (this[propertyName] == value)
                {
                    return;
                }

                isDirty = true;
                if (propertiesList.ContainsKey(propertyName))
                {
                    propertiesList.Remove(propertyName);
                }
                propertiesList.Add(propertyName, value);
            }
        }

        /// <summary>
        /// Closes the IVsProjectFlavorCfg object.
        /// </summary>
        /// <returns></returns>
        public int Close()
        {
            if (isClosed)
            {
                return VSConstants.E_FAIL;
            }

            isClosed = true;
            mapIVsCfgToCustomPropertyPageProjectFlavorCfg.Remove(baseProjectCfg);
            var hr = innerProjectFlavorCfg.Close();

            Marshal.ReleaseComObject(baseProjectCfg);
            Marshal.ReleaseComObject(innerProjectFlavorCfg);
            Marshal.ReleaseComObject(baseDebugConfiguration);
//            Marshal.ReleaseComObject(baseBuildConfiguration);

            return hr;
        }

        /// <summary>
        /// Implement the InitNew method to initialize the project extension properties and other build-independent data. This 
        /// method is called if there is no XML configuration data present in the project file.
        /// </summary>
        /// <param name="guidFlavor">GUID of the project subtype.</param>
        /// <param name="storage">Specifies the storage type used for persisting files. Values are taken from the 
        /// _PersistStorageType enumeration. The file type is either project file (.csproj or .vbproj) or user file 
        /// (.csproj.user or .vbproj.user).</param>
        public int InitNew(ref Guid guidFlavor, uint storage)
        {
            // Return, if it is our guid.
            if (IsMyFlavorGuid(ref guidFlavor))
            {
                return VSConstants.S_OK;
            }

            // Forward the call to inner flavor(s).
            if (innerProjectFlavorCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)innerProjectFlavorCfg).InitNew(ref guidFlavor, storage);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Implement the IsFragmentDirty method to determine whether an XML fragment has changed since it was last saved to 
        /// its current file.
        /// </summary>
        /// <param name="storage">Storage type of the file in which the XML is persisted. Values are taken from 
        /// _PersistStorageType enumeration.</param>
        /// <param name="pfDirty">Set to 1 if dirty, 0 if not</param>
        public int IsFragmentDirty(uint storage, out int pfDirty)
        {
            pfDirty = 0;
            switch (storage)
            {
                // Specifies storage file type to project file.
                case (uint)_PersistStorageType.PST_PROJECT_FILE:                   
                    if (isDirty)
                        pfDirty |= 1;
                    break;
                // Specifies storage file type to user file.
                case (uint)_PersistStorageType.PST_USER_FILE:
                    // Do not store anything in the user file.
                    break;
            }

            // Forward the call to inner flavor(s) 
            if (pfDirty == 0 && innerProjectFlavorCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)innerProjectFlavorCfg).IsFragmentDirty(storage, out pfDirty);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Implement the Load method to load the XML data from the project file.
        /// </summary>
        /// <param name="guidFlavor">GUID of the project subtype.</param>
        /// <param name="storage">Storage type of the file in which the XML is persisted. Values are taken from _PersistStorageType 
        /// enumeration.</param>
        /// <param name="pszXMLFragment">String containing the XML fragment.</param>
        public int Load(ref Guid guidFlavor, uint storage, string pszXMLFragment)
        {
            if (IsMyFlavorGuid(ref guidFlavor))
            {
                switch (storage)
                {
                    case (uint)_PersistStorageType.PST_PROJECT_FILE:
                        // Load our data from the XML fragment.
                        var doc = new XmlDocument();
                        var node = doc.CreateElement(GetType().Name);
                        node.InnerXml = pszXMLFragment;
                        if (node.FirstChild != null)
                        {
                            // Load all the properties
                            foreach (XmlNode child in node.FirstChild.ChildNodes)
                            {
                                propertiesList.Add(child.Name, child.InnerText);
                            }                            
                        }
                        break;
                    case (uint)_PersistStorageType.PST_USER_FILE:
                        // Do not store anything in the user file.
                        break;
                }
            }

            // Forward the call to inner flavor(s)
            if (innerProjectFlavorCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)innerProjectFlavorCfg).Load(ref guidFlavor, storage, pszXMLFragment);
            }

            return VSConstants.S_OK;
        }
        
        /// <summary>
        /// Implement the Save method to save the XML data in the project file.
        /// </summary>
        /// <param name="guidFlavor">GUID of the project subtype.</param>
        /// <param name="storage">Storage type of the file in which the XML is persisted. Values are taken from 
        /// _PersistStorageType enumeration.</param>
        /// <param name="pbstrXMLFragment">String containing the XML fragment.</param>
        /// <param name="fClearDirty">Indicates whether to clear the dirty flag after the save is complete. If true, the flag 
        /// should be cleared. If false, the flag should be left unchanged.</param>
        public int Save(ref Guid guidFlavor, uint storage, out string pbstrXMLFragment, int fClearDirty)
        {
            pbstrXMLFragment = null;

            if (IsMyFlavorGuid(ref guidFlavor))
            {
                switch (storage)
                {
                    case (uint)_PersistStorageType.PST_PROJECT_FILE:
                        // Create XML for our data (a string and a bool).
                        var doc = new XmlDocument();
                        var root = doc.CreateElement(GetType().Name);

                        foreach (var property in propertiesList)
                        {
                            XmlNode node = doc.CreateElement(property.Key);
                            node.AppendChild(doc.CreateTextNode(property.Value));
                            root.AppendChild(node);
                        }

                        doc.AppendChild(root);
                        // Get XML fragment representing our data
                        pbstrXMLFragment = doc.InnerXml;

                        if (fClearDirty != 0)
                            isDirty = false;
                        break;
                    case (uint)_PersistStorageType.PST_USER_FILE:
                        // Do not store anything in the user file.
                        break;
                }
            }

            // Forward the call to inner flavor(s)
            if (innerProjectFlavorCfg != null && innerProjectFlavorCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)innerProjectFlavorCfg).Save(ref guidFlavor, storage, out pbstrXMLFragment, fClearDirty);
            }

            return VSConstants.S_OK;
        }

        private bool IsMyFlavorGuid(ref Guid guidFlavor)
        {
            return guidFlavor.Equals(typeof(MonoProgramProjectFactory).GUID);
        }

	    public int get_DisplayName(out string pbstrDisplayName)
	    {
	        return baseDebugConfiguration.get_DisplayName(out pbstrDisplayName);
	    }

	    [Obsolete]
	    public int get_IsDebugOnly(out int pfIsDebugOnly)
	    {
	        return baseDebugConfiguration.get_IsDebugOnly(out pfIsDebugOnly);
	    }

        [Obsolete]
	    public int get_IsReleaseOnly(out int pfIsReleaseOnly)
	    {
	        return baseDebugConfiguration.get_IsReleaseOnly(out pfIsReleaseOnly);
	    }

        [Obsolete]
	    public int EnumOutputs(out IVsEnumOutputs ppIVsEnumOutputs)
	    {
	        return baseDebugConfiguration.EnumOutputs(out ppIVsEnumOutputs);
	    }

        [Obsolete]
	    public int OpenOutput(string szOutputCanonicalName, out IVsOutput ppIVsOutput)
	    {
	        return baseDebugConfiguration.OpenOutput(szOutputCanonicalName, out ppIVsOutput);
	    }

        [Obsolete]
	    public int get_ProjectCfgProvider(out IVsProjectCfgProvider ppIVsProjectCfgProvider)
	    {
	        return baseDebugConfiguration.get_ProjectCfgProvider(out ppIVsProjectCfgProvider);
	    }

	    public int get_BuildableProjectCfg(out IVsBuildableProjectCfg ppIVsBuildableProjectCfg)
	    {
	        return baseDebugConfiguration.get_BuildableProjectCfg(out ppIVsBuildableProjectCfg);
	    }

	    public int get_CanonicalName(out string pbstrCanonicalName)
	    {
	        return baseDebugConfiguration.get_CanonicalName(out pbstrCanonicalName);
	    }

        [Obsolete]
	    public int get_Platform(out Guid pguidPlatform)
        {
            return baseDebugConfiguration.get_Platform(out pguidPlatform);
        }

        [Obsolete]
	    public int get_IsPackaged(out int pfIsPackaged)
	    {
	        return baseDebugConfiguration.get_IsPackaged(out pfIsPackaged);
	    }

        [Obsolete]
	    public int get_IsSpecifyingOutputSupported(out int pfIsSpecifyingOutputSupported)
        {
            return baseDebugConfiguration.get_IsSpecifyingOutputSupported(out pfIsSpecifyingOutputSupported);
        }

        [Obsolete]
	    public int get_TargetCodePage(out uint puiTargetCodePage)
        {
            return baseDebugConfiguration.get_TargetCodePage(out puiTargetCodePage);
        }

        [Obsolete]
	    public int get_UpdateSequenceNumber(ULARGE_INTEGER[] puliUSN)
        {
            return baseDebugConfiguration.get_UpdateSequenceNumber(puliUSN);
        }

	    public int get_RootURL(out string pbstrRootURL)
	    {
	        return baseDebugConfiguration.get_RootURL(out pbstrRootURL);
	    }

	    public int DebugLaunch(uint grfLaunch)
	    {
	        return baseDebugConfiguration.DebugLaunch(grfLaunch);
	    }

	    public int QueryDebugLaunch(uint grfLaunch, out int pfCanLaunch)
	    {
	        return baseDebugConfiguration.QueryDebugLaunch(grfLaunch, out pfCanLaunch);
	    }

        public int get_ProjectCfg(out IVsProjectCfg ppIVsProjectCfg)
        {
            ppIVsProjectCfg = this;
            return VSConstants.S_OK;
        }

        public int AdviseBuildStatusCallback(IVsBuildStatusCallback pIVsBuildStatusCallback, out uint pdwCookie)
        {
            throw new NotImplementedException();
        }

        public int UnadviseBuildStatusCallback(uint dwCookie)
        {
            throw new NotImplementedException();
        }

        public int StartBuild(IVsOutputWindowPane pIVsOutputWindowPane, uint dwOptions)
        {
            throw new NotImplementedException();
        }

        public int StartClean(IVsOutputWindowPane pIVsOutputWindowPane, uint dwOptions)
        {
            throw new NotImplementedException();
        }

        public int StartUpToDateCheck(IVsOutputWindowPane pIVsOutputWindowPane, uint dwOptions)
        {
            throw new NotImplementedException();
        }

        public int QueryStatus(out int pfBuildDone)
        {
            throw new NotImplementedException();
        }

        public int Stop(int fSync)
        {
            throw new NotImplementedException();
        }

        public int Wait(uint dwMilliseconds, int fTickWhenMessageQNotEmpty)
        {
            throw new NotImplementedException();
        }

        public int QueryStartBuild(uint dwOptions, int[] pfSupported, int[] pfReady)
        {
            throw new NotImplementedException();
        }

        public int QueryStartClean(uint dwOptions, int[] pfSupported, int[] pfReady)
        {
//            var dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE.14.0");

            var dte = project.Package.GetGlobalService<SDTE>() as DTE2;
            var configuration = dte.Solution.SolutionBuild.ActiveConfiguration;
            var dteProject = GetDTEProject(project);
            var dir = Path.Combine(dteProject.FullName, dteProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString());

//            var builtGroup = dteProject.ConfigurationManager.ActiveConfiguration.OutputGroups.OfType<EnvDTE.OutputGroup>().First(x => x.CanonicalName == "Built");

            return VSConstants.S_OK;
        }

        public static EnvDTE.Project GetDTEProject(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as EnvDTE.Project;
        }

        public int QueryStartUpToDateCheck(uint dwOptions, int[] pfSupported, int[] pfReady)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using MonoProgram.Package.Credentials;
using MonoProgram.Package.Debuggers;
using MonoProgram.Package.ProgramProperties;
using MonoProgram.Package.Utils;
using Renci.SshNet;

namespace MonoProgram.Package.Projects
{
    public class MonoProgramFlavorCfg : IVsProjectFlavorCfg, IPersistXMLFragment, IVsDebuggableProjectCfg, IVsBuildableProjectCfg
    {
        public const string DebugCategory = "host";
        public const string BuildCategory = "build";

        /// <summary>
        /// This allows the property page to map a IVsCfg object (the baseConfiguration) to an actual instance of 
        /// CustomPropertyPageProjectFlavorCfg.
        /// </summary>
        private static readonly Dictionary<IVsCfg, MonoProgramFlavorCfg> cfgs = new Dictionary<IVsCfg, MonoProgramFlavorCfg>();
        private static readonly Dictionary<Project, MonoProgramFlavorCfg> cfgsByDteProject = new Dictionary<Project, MonoProgramFlavorCfg>();

        private readonly MonoProgramProjectFlavor project;
        private readonly IVsCfg baseProjectCfg;
        private readonly IVsProjectFlavorCfg innerProjectFlavorCfg;
        private readonly IVsDebuggableProjectCfg baseDebugConfiguration;
        private readonly Dictionary<string, string> propertiesList = new Dictionary<string, string>();
        private readonly Dictionary<uint, IVsBuildStatusCallback> callbacks = new Dictionary<uint, IVsBuildStatusCallback>();

        private uint callbackCookieCounter;
        private bool isClosed;
        private bool isDirty;

        public MonoProgramFlavorCfg(MonoProgramProjectFlavor project, IVsCfg baseProjectCfg, IVsProjectFlavorCfg innerProjectFlavorCfg)
        {
            this.project = project;
            this.baseProjectCfg = baseProjectCfg;
            this.innerProjectFlavorCfg = innerProjectFlavorCfg;
            cfgs.Add(baseProjectCfg, this);
            cfgsByDteProject[GetDTEProject(project)] = this;

            var debugGuid = typeof(IVsDebuggableProjectCfg).GUID;
            IntPtr baseDebugConfigurationPtr;
		    innerProjectFlavorCfg.get_CfgType(ref debugGuid, out baseDebugConfigurationPtr);
		    baseDebugConfiguration = (IVsDebuggableProjectCfg)Marshal.GetObjectForIUnknown(baseDebugConfigurationPtr);
        }

        internal static MonoProgramFlavorCfg GetMonoProgramFlavorCfgFromVsCfg(IVsCfg configuration)
		{
            if (cfgs.ContainsKey(configuration))
            {
                return cfgs[configuration];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(configuration), $"Cannot find configuration in {nameof(cfgs)}.");
            }
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
            cfgs.Remove(baseProjectCfg);
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

            var debugHost = this[MonoPropertyPage.DebugHostProperty];
            var debugPassword = this[MonoPropertyPage.DebugUsernameProperty];
            if (!string.IsNullOrEmpty(debugHost) && !string.IsNullOrEmpty(debugPassword))
            {
                this[MonoPropertyPage.DebugPasswordProperty] = CredentialManager.GetCredentials(DebugCategory, debugHost, debugPassword)?.Password;
            }

            var buildHost = this[MonoPropertyPage.BuildHostProperty];
            var buildUsername = this[MonoPropertyPage.BuildUsernameProperty];
            if (!string.IsNullOrEmpty(buildHost) && !string.IsNullOrEmpty(buildUsername))
            {
                this[MonoPropertyPage.BuildPasswordProperty] = CredentialManager.GetCredentials(BuildCategory, buildHost, buildUsername)?.Password;
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
                var debugHost = this[MonoPropertyPage.DebugHostProperty];
                var debugUsername = this[MonoPropertyPage.DebugUsernameProperty];
                var debugPassword = this[MonoPropertyPage.DebugPasswordProperty];
                if (!string.IsNullOrEmpty(debugHost) && !string.IsNullOrEmpty(debugUsername) && debugPassword != null)
                {
                    CredentialManager.SetCredentials(DebugCategory, debugHost, debugUsername, debugPassword);
                }

                var buildHost = this[MonoPropertyPage.BuildHostProperty];
                var buildUsername = this[MonoPropertyPage.BuildUsernameProperty];
                var buildPassword = this[MonoPropertyPage.BuildPasswordProperty];
                if (!string.IsNullOrEmpty(buildHost) && !string.IsNullOrEmpty(buildUsername) && buildPassword != null)
                {
                    CredentialManager.SetCredentials(BuildCategory, buildHost, buildUsername, buildPassword);
                }

                switch (storage)
                {
                    case (uint)_PersistStorageType.PST_PROJECT_FILE:
                        // Create XML for our data (a string and a bool).
                        var doc = new XmlDocument();
                        var root = doc.CreateElement(GetType().Name);

                        foreach (var property in propertiesList.Where(x => x.Key != MonoPropertyPage.DebugPasswordProperty))
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
            if (innerProjectFlavorCfg is IPersistXMLFragment)
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
            var dteProject = GetDTEProject(project);
            var projectFolder = Path.GetDirectoryName(dteProject.FullName);
	        var projectConfiguration = dteProject.ConfigurationManager.ActiveConfiguration;
	        var dir = Path.GetDirectoryName(Path.Combine(projectFolder, projectConfiguration.Properties.Item("OutputPath").Value.ToString()));
	        var fileName = dteProject.Properties.Item("OutputFileName").Value.ToString();
	        var outputFile = Path.Combine(dir, fileName);

	        var sourceMappings = new List<MonoSourceMapping>();
	        foreach (Project currentProject in dteProject.Collection)
	        {
	            var cfg = cfgsByDteProject[currentProject];
	            var sourceRoot = Path.GetDirectoryName(currentProject.FullName);
	            var buildRoot = cfg[MonoPropertyPage.BuildFolderProperty].NullIfEmpty() ?? ConvertToUnixPath(sourceRoot);
                sourceMappings.Add(new MonoSourceMapping(sourceRoot, buildRoot));
	        }
//	        var sourceRoot = projectFolder;
//	        var buildRoot = this[MonoPropertyPage.BuildFolderProperty].NullIfEmpty() ?? ConvertToUnixPath(sourceRoot);
            var settings = new MonoDebuggerSettings(this[MonoPropertyPage.DebugHostProperty], this[MonoPropertyPage.DebugUsernameProperty], this[MonoPropertyPage.DebugPasswordProperty], sourceMappings.ToArray());

            var debugger = (IVsDebugger4)project.Package.GetGlobalService<IVsDebugger>();
            var debugTargets = new VsDebugTargetInfo4[1];
	        debugTargets[0].LaunchFlags = grfLaunch;
            debugTargets[0].dlo = (uint)DEBUG_LAUNCH_OPERATION.DLO_CreateProcess;
            debugTargets[0].bstrExe = outputFile;
	        debugTargets[0].bstrCurDir = this[MonoPropertyPage.DebugDestinationProperty].Trim('/');     // If the user has a slash on either side, just get rid of it
	        debugTargets[0].bstrOptions = settings.ToString();
            debugTargets[0].guidLaunchDebugEngine = new Guid(Guids.EngineId);

            var processInfo = new VsDebugTargetProcessInfo[debugTargets.Length];
            debugger.LaunchDebugTargets4(1, debugTargets, processInfo);

	        return VSConstants.S_OK;
	    }

	    public int QueryDebugLaunch(uint grfLaunch, out int pfCanLaunch)
	    {
	        pfCanLaunch = 1;
	        return VSConstants.S_OK;
	    }

        public int get_ProjectCfg(out IVsProjectCfg ppIVsProjectCfg)
        {
            ppIVsProjectCfg = this;
            return VSConstants.S_OK;
        }

        private void UpdateBuildStatus(int status)
        {
            foreach (var callback in callbacks.Values.ToArray())
            {
                callback.BuildEnd(status);
            }            
        }

        public int AdviseBuildStatusCallback(IVsBuildStatusCallback callback, out uint pdwCookie)
        {
            pdwCookie = ++callbackCookieCounter;
            callbacks[callbackCookieCounter] = callback;
            return VSConstants.S_OK;
        }

        public int UnadviseBuildStatusCallback(uint dwCookie)
        {
            callbacks.Remove(dwCookie);
            return VSConstants.S_OK;
        }

        public int StartBuild(IVsOutputWindowPane outputPane, uint dwOptions)
        {
            var buildHost = this[MonoPropertyPage.BuildHostProperty];
            var dteProject = GetDTEProject(project);
            var projectFolder = Path.GetDirectoryName(dteProject.FullName);
            outputPane.Log($"Starting build of {projectFolder}...");

            // If using Windows Bash...
            if (string.IsNullOrEmpty(buildHost))
            {
                var bash = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Sysnative", "bash.exe");
                if (!File.Exists(bash))
                {
//                    outputPane.LogError(dteProject.FullName, $"Error: You must set up a build server on the 'Mono' project property page.");
                    outputPane.Log(VsLogSeverity.Error, dteProject.UniqueName, dteProject.FullName, "Error: You must set up a build server on the 'Mono' project property page.");

                    UpdateBuildStatus(0);
                    return VSConstants.S_FALSE;
                }

                var bashProjectFolder = ConvertToUnixPath(projectFolder);
                var outputFile = Path.GetTempFileName();
                var script = $@"cd ""{bashProjectFolder}""
    xbuild > ""{ConvertToUnixPath(outputFile)}""
    exit
    ".Replace("\r\n", "\n");
                var scriptFile = Path.GetTempFileName();
                var arguments = $"/C {bash} --init-file {ConvertToUnixPath(scriptFile)}";
                File.WriteAllText(scriptFile, script);

                var process = new System.Diagnostics.Process
                {
                    StartInfo = new ProcessStartInfo("CMD", arguments)
                    {
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
                outputPane.Log(File.ReadAllText(outputFile));

                try
                {
                    File.Delete(scriptFile);
                    File.Delete(outputFile);
                }
                catch (Exception e)
                {
                    outputPane.Log(e.ToString());
                }
            }
            else
            {
                outputPane.Log("Uploading project to the build server...");
                var buildUsername = this[MonoPropertyPage.BuildUsernameProperty];
                var buildPassword = this[MonoPropertyPage.BuildPasswordProperty];
                var buildFolder = this[MonoPropertyPage.BuildFolderProperty];
                using (var client = new SftpClient(buildHost, buildUsername, buildPassword))
                {
                    client.Connect();

                    client.CreateFullDirectory(buildFolder);
                    client.ChangeDirectory(buildFolder);

                    outputPane.Log($"Clearing out the contents of the build folder: {buildFolder}");
                    client.Clear();

                    // Upload project
                    var createdDirectories = new HashSet<string>();
                    var projectFile = Path.GetFileName(dteProject.FullName);
                    client.Upload(projectFolder, projectFile, createdDirectories);
                    outputPane.Log($"Uploaded {projectFile}");
                    var projectItems = new Queue<ProjectItem>(dteProject.ProjectItems.Cast<ProjectItem>());
                    while (projectItems.Any())
                    {
                        var projectItem = projectItems.Dequeue();

                        for (short i = 1; i <= projectItem.FileCount; i++)
                        {
                            var fileName = projectItem.FileNames[i];
                            if (File.Exists(fileName))
                            {
                                fileName = FileUtils.ToRelativePath(projectFolder, fileName);
                                client.Upload(projectFolder, fileName, createdDirectories);
                                outputPane.Log($"Uploaded {fileName}");                                
                            }
                        }

                        foreach (ProjectItem childItem in projectItem.ProjectItems)
                        {
                            projectItems.Enqueue(childItem);
                        }
                    }

                    using (var ssh = new SshClient(buildHost, buildUsername, buildPassword))
                    {
                        outputPane.Log("Starting xbuild to build the project");
                        ssh.Connect();
                        var exitCode = ssh.RunCommand($@"cd {buildFolder}; xbuild /p:Configuration={dteProject.ConfigurationManager.ActiveConfiguration.ConfigurationName} > xbuild.output; exitcode=$?; cat xbuild.output; rm xbuild.output; exit ""$exitcode""", outputPane, dteProject.UniqueName);
                        if (exitCode != 0)
                        {
                            UpdateBuildStatus(0);
                            return VSConstants.S_FALSE;
                        }
                    }

	                var projectConfiguration = dteProject.ConfigurationManager.ActiveConfiguration;
	                var outputFolder = Path.GetDirectoryName(Path.Combine(projectFolder, projectConfiguration.Properties.Item("OutputPath").Value.ToString()));
                    outputPane.Log($"Copying build artifacts to the output folder: {outputFolder}");

                    var buildServerOutputPath = buildFolder + "/" + FileUtils.ToRelativePath(projectFolder, outputFolder).Replace("\\", "/");
                    client.CreateFullDirectory(buildServerOutputPath);
                    client.ChangeDirectory(buildServerOutputPath);
                    foreach (var file in client.ListDirectory(".").Where(x => x.IsRegularFile))
                    {
                        using (var output = new FileStream(Path.Combine(outputFolder, file.Name), FileMode.Create, FileAccess.Write))
                        {
                            client.DownloadFile(file.FullName, output);
                            outputPane.Log($"Copied {file.Name}");
                        }
                    }

                    client.Disconnect();
                }
            }

            UpdateBuildStatus(1);
            return VSConstants.S_OK;
        }

        private static string ConvertToUnixPath(string file)
        {
            var result = file.Replace("\\", "/").Replace(":", "");
            result = char.ToLower(result[0]) + result.Substring(1);
            result = "/mnt/" + result;
            return result;
        }

        public int StartClean(IVsOutputWindowPane pIVsOutputWindowPane, uint dwOptions)
        {
            return VSConstants.S_OK;
        }

        public int StartUpToDateCheck(IVsOutputWindowPane outputPane, uint dwOptions)
        {
            outputPane.OutputString("Beginning update check!!\n");
            return VSConstants.S_FALSE;
//            return VSConstants.S_OK;
        }

        public int QueryStatus(out int pfBuildDone)
        {
            throw new NotImplementedException();
        }

        public int Stop(int fSync)
        {
            return VSConstants.S_OK;
        }

        [Obsolete]
        public int Wait(uint dwMilliseconds, int fTickWhenMessageQNotEmpty)
        {
            return VSConstants.S_OK;
        }

        public int QueryStartBuild(uint dwOptions, int[] pfSupported, int[] pfReady)
        {
            pfSupported[0] = 1;
            pfReady[0] = 1;
            return VSConstants.S_OK;
        }

        public int QueryStartClean(uint dwOptions, int[] pfSupported, int[] pfReady)
        {
            pfSupported[0] = 1;
            return VSConstants.S_OK;
        }

        public static Project GetDTEProject(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException(nameof(hierarchy));

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as Project;
        }

        public int QueryStartUpToDateCheck(uint dwOptions, int[] pfSupported, int[] pfReady)
        {
            pfSupported[0] = 0;
            pfReady[0] = 1;
            return VSConstants.S_OK;
        }
    }
}
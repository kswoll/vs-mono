using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MonoProgram.Package.ProgramProperties;
using MonoProgram.Package.Projects;

namespace MonoProgram.Package
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [ProvideObject(typeof(MonoPropertyPage), RegisterUsing = RegistrationMethod.CodeBase)]
    [ProvideProjectFactory(typeof(MonoProgramProjectFactory), "MonoProgram", null, null, null, @"..\Templates\Projects")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class MonoProgramPackage : Microsoft.VisualStudio.Shell.Package
    {
        /// <summary>
        /// MonoProgramPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "e2cea683-3e6c-4cec-ab3b-0601beae5ca5";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            RegisterProjectFactory(new MonoProgramProjectFactory(this));
        }

        public T GetGlobalService<T>()
        {
            return (T)GetService(typeof(T));
        }
    }
}

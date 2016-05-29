using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Flavor;

namespace MonoProgram.Package.Projects
{
    [Guid("C6F10C0B-C1F0-4F18-A597-8A7E9EB1A480")]
    public class MonoProgramProjectFactory : FlavoredProjectFactoryBase
    {
        private readonly MonoProgramPackage package;

        public MonoProgramProjectFactory(MonoProgramPackage package)
        {
            this.package = package;
        }

        /// <summary>
        /// Create an instance of MonoProgramProjectFlavor.  The initialization will be done later when Visual Studio calls
        /// InitalizeForOuter on it.
        /// </summary>
        /// <param name="outerProjectIUnknown"> This value points to the outer project. It is useful if there is a 
        /// Project SubType of this Project SubType.</param>
        /// <returns>A MonoProgramProjectFactory instance that has not been initialized.</returns>
        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
        {
            return new MonoProgramProjectFlavor(package);
        }
    }
}
using System;
using System.Runtime.InteropServices;
using MonoProgram.Package.PropertyPages;

namespace MonoProgram.Package.ProgramProperties
{
    [Guid("82CE8435-2B18-44C8-9582-E87FC3627E01")]
    public class MonoPropertyPage : PropertyPage
    {
        public const string HostProperty = "HostProperty";
        public const string UsernameProperty = "UsernameProperty";
        public const string PasswordProperty = "PasswordProperty";
        public const string DestinationProperty = "DestinationProperty";
        public const string SourceRootProperty = "SourceRoot";
        public const string BuildRootProperty = "BuildRoot";
        public const string BuildServerProperty = "BuildServerProperty";
        public const string BuildFolderProperty = "BuildFolderProperty";

        protected override string HelpKeyword => string.Empty;
		public override string Title => "Mono";
        protected override IPageView GetNewPageView() => new MonoPropertiesView(this);
        protected override IPropertyStore GetNewPropertyStore() => new MonoPropertiesStore();
    }
}
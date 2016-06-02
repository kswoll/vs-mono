using System.Runtime.InteropServices;
using MonoProgram.Package.PropertyPages;

namespace MonoProgram.Package.ProgramProperties
{
    [Guid("82CE8435-2B18-44C8-9582-E87FC3627E01")]
    public class MonoPropertyPage : PropertyPage
    {
        public const string DebugHostProperty = "HostProperty";
        public const string DebugUsernameProperty = "UsernameProperty";
        public const string DebugPasswordProperty = "PasswordProperty";
        public const string DebugDestinationProperty = "DestinationProperty";
        public const string BuildRootProperty = "BuildRoot";
        public const string BuildHostProperty = "BuildHostProperty";
        public const string BuildFolderProperty = "BuildFolderProperty";
        public const string BuildUsernameProperty = "BuildUsername";
        public const string BuildPasswordProperty = "BuildPassword";

        protected override string HelpKeyword => string.Empty;
		public override string Title => "Mono";
        protected override IPageView GetNewPageView() => new MonoPropertiesView(this);
        protected override IPropertyStore GetNewPropertyStore() => new MonoPropertiesStore();
    }
}
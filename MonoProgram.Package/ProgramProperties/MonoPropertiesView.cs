using System.Runtime.InteropServices;
using MonoProgram.Package.PropertyPages;

namespace MonoProgram.Package.ProgramProperties
{
    [Guid("3DE15C6B-2C7F-416C-B496-7D76A64A5247")]
    public partial class MonoPropertiesView : PageView
    {
        public const string HostProperty = "HostProperty";
        public const string UsernameProperty = "UsernameProperty";
        public const string PasswordProperty = "PasswordProperty";

		private PropertyControlTable propertyControlTable;

        public MonoPropertiesView()
        {
            InitializeComponent();
        }

        public MonoPropertiesView(IPageViewSite pageViewSite) : base(pageViewSite)
        {
            InitializeComponent();
        }

        /// <summary>
        /// This property is used to map the control on a PageView object to a property
        /// in PropertyStore object.
        /// 
        /// This property will be called in the base class's constructor, which means that
        /// the InitializeComponent has not been called and the Controls have not been
        /// initialized.
        /// </summary>
		protected override PropertyControlTable PropertyControlTable
		{
			get
			{
				if (propertyControlTable == null)
				{
					// This is the list of properties that will be persisted and their
					// assciation to the controls.
					propertyControlTable = new PropertyControlTable();

                    // This means that this CustomPropertyPageView object has not been
                    // initialized.
                    if (string.IsNullOrEmpty(base.Name))
                    {
                        InitializeComponent();
                    }

                    // Add two Property Name / Control KeyValuePairs. 
					propertyControlTable.Add(HostProperty, hostTextBox);
//                    propertyControlTable.Add(BooleanPropertyTag, chkBooleanProperty);
				}
				return propertyControlTable;
			}
		}
    }
}

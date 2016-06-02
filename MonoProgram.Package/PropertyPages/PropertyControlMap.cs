using System.Windows.Forms;

namespace MonoProgram.Package.PropertyPages
{
    public class PropertyControlMap
    {
        // The IPageViewSite Interface is implemented by the PropertyPage class.
        private readonly IPageViewSite pageViewSite;

        // The PropertyControlTable class stores the Control / Property Name KeyValuePairs. 
        // A KeyValuePair contains a Control of a PageView object, and a Property Name of
        // PropertyPage object.
        private readonly PropertyControlTable propertyControlTable;

        // The IPropertyPageUi Interface is implemented by the PageView Class.
        private readonly IPropertyPageUi propertyPageUI;

        public PropertyControlMap(IPageViewSite pageViewSite, IPropertyPageUi propertyPageUI, PropertyControlTable propertyControlTable)
        {
            this.propertyControlTable = propertyControlTable;
            this.pageViewSite = pageViewSite;
            this.propertyPageUI = propertyPageUI;
        }

        /// <summary>
        /// Initialize the Controls on a PageView Object using the Properties of a PropertyPage object.
        /// </summary>
        public void InitializeControls()
        {
            propertyPageUI.UserEditComplete -= propertyPageUI_UserEditComplete;
            foreach (var str in propertyControlTable.GetPropertyNames())
            {
                var valueForProperty = pageViewSite.GetValueForProperty(str);
                var controlFromPropertyName = propertyControlTable.GetControlFromPropertyName(str);

                propertyPageUI.SetControlValue(controlFromPropertyName, valueForProperty);
            }
            propertyPageUI.UserEditComplete += propertyPageUI_UserEditComplete;
        }

        /// <summary>
        ///     Notify the PropertyPage object that a Control value is changed.
        /// </summary>
        private void propertyPageUI_UserEditComplete(Control control, string value)
        {
            var propertyNameFromControl = propertyControlTable.GetPropertyNameFromControl(control);
            pageViewSite.PropertyChanged(propertyNameFromControl, value);
        }
    }
}
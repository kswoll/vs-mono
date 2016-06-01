using System.Windows.Forms;

namespace MonoProgram.Package.PropertyPages
{
    public interface IPropertyPageUi
    {
		event UserEditCompleteHandler UserEditComplete;
        string GetControlValue(Control control);
        void SetControlValue(Control control, string value);
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonoProgram.Package.PropertyPages
{
	public interface IPageView: IDisposable
	{
        void HideView();
        void Initialize(Control parentControl, Rectangle rectangle);
        void MoveView(Rectangle rectangle);
        int ProcessAccelerator(ref Message message);
        void RefreshPropertyValues();
        void ShowView();
        Size ViewSize { get; }
	}
}
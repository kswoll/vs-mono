using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio;

namespace MonoProgram.Package.PropertyPages
{
    public class PageView : UserControl, IPageView, IPropertyPageUi
    {
        private readonly PropertyControlMap propertyControlMap;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public PageView()
        {
        }

        public PageView(IPageViewSite pageViewSite)
        {
            propertyControlMap = new PropertyControlMap(pageViewSite, this, PropertyControlTable);
        }

        /// <summary>
        ///     This property is used to map the control on a PageView object to a property
        ///     in PropertyStore object.
        ///     This property must be overriden.
        /// </summary>
        protected virtual PropertyControlTable PropertyControlTable
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///     Occur if the value of a control changed.
        /// </summary>
        public event UserEditCompleteHandler UserEditComplete;

        /// <summary>
        ///     Raise the UserEditComplete event.
        /// </summary>
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            UserEditComplete?.Invoke(chk, chk.Checked.ToString());
        }

        /// <summary>
        ///     Raise the UserEditComplete event.
        /// </summary>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            UserEditComplete?.Invoke(tb, tb.Text);
        }

        protected virtual void OnInitialize()
        {
        }

        #region IPageView members

        /// <summary>
        ///     Make the PageView hide.
        /// </summary>
        public void HideView()
        {
            Hide();
        }

        /// <summary>
        ///     Initialize this PageView object.
        /// </summary>
        /// <param name="parentControl">
        ///     The parent control of this PageView object.
        /// </param>
        /// <param name="rectangle">
        ///     The position of this PageView object.
        /// </param>
        public virtual void Initialize(Control parentControl, Rectangle rectangle)
        {
            SetBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            Parent = parentControl;

            // Initialize the value of the Controls on this PageView object. 
            propertyControlMap.InitializeControls();

            // Register the event when the value of a Control changed.
            foreach (var control in PropertyControlTable.GetControls())
            {
                var tb = control as TextBox;
                if (tb != null)
                {
                    tb.TextChanged += TextBox_TextChanged;
                }
                else
                {
                    var chk = control as CheckBox;
                    if (chk != null)
                        chk.CheckedChanged += CheckBox_CheckedChanged;
                }
            }
            OnInitialize();
        }

        /// <summary>
        /// Move to new position.
        /// </summary>
        public void MoveView(Rectangle rectangle)
        {
            Location = new Point(rectangle.X, rectangle.Y);
            Size = new Size(rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Pass a keystroke to the property page for processing.
        /// </summary>
        public int ProcessAccelerator(ref Message keyboardMessage)
        {
            if (FromHandle(keyboardMessage.HWnd).PreProcessMessage(ref keyboardMessage))
            {
                return VSConstants.S_OK;
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        ///     Refresh the UI.
        /// </summary>
        public void RefreshPropertyValues()
        {
            propertyControlMap.InitializeControls();
        }

        /// <summary>
        ///     Show this PageView object.
        /// </summary>
        public void ShowView()
        {
            Show();
        }

        #endregion

        #region IPropertyPageUI

        /// <summary>
        ///     Get the value of a Control on this PageView object.
        /// </summary>
        public virtual string GetControlValue(Control control)
        {
            var chk = control as CheckBox;
            if (chk != null)
            {
                return chk.Checked.ToString();
            }

            var tb = control as TextBox;
            if (tb == null)
            {
                throw new ArgumentOutOfRangeException();
            }
            return tb.Text;
        }

        /// <summary>
        ///     Set the value of a Control on this PageView object.
        /// </summary>
        public virtual void SetControlValue(Control control, string value)
        {
            var chk = control as CheckBox;
            if (chk != null)
            {
                bool flag;
                if (!bool.TryParse(value, out flag))
                {
                    flag = false;
                }
                chk.Checked = flag;
            }
            else
            {
                var tb = control as TextBox;
                if (tb != null)
                {
                    tb.Text = value;
                }
            }
        }

        #endregion
    }
}
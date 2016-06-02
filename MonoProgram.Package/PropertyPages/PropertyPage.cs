using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Help = Microsoft.VisualStudio.VSHelp.Help;
using IServiceProvider = System.IServiceProvider;

namespace MonoProgram.Package.PropertyPages
{
    public abstract class PropertyPage : IPropertyPage2, IPageViewSite
    {
        /// <summary>
        /// The Property Page Title that is displayed in Visual Studio.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// The HelpKeyword if F1 is pressed. By default, it will return String.Empty.
        /// </summary>
        protected abstract string HelpKeyword { get; }

        protected abstract IPageView GetNewPageView();
        protected abstract IPropertyStore GetNewPropertyStore();

        protected IPageView myPageView;

        // The changed Property Name / Value KeyValuePair. 
        private KeyValuePair<string, string>? propertyToBePersisted;

        /// <summary>
        ///     Use a site object with this interface to set up communications between the
        ///     property frame and the property page object.
        ///     http://msdn.microsoft.com/en-us/library/ms690583(VS.85).aspx
        /// </summary>
        public IPropertyPageSite PropertyPageSite { get; set; }

        /// <summary>
        ///     A PropertyStore object is used to store the Properties of a
        ///     PropertyPage object.
        /// </summary>
        public IPropertyStore PropertyStore { get; set; }

        /// <summary>
        ///     A PageView is a UserControl that is used to display the Properties of a
        ///     PropertyPage object.
        /// </summary>
        public IPageView MyPageView
        {
            get
            {
                if (myPageView == null)
                {
                    var concretePageView = GetNewPageView();
                    MyPageView = concretePageView;
                }
                return myPageView;
            }
            set { myPageView = value; }
        }

        /// <summary>
        /// This method is called if the value of a Control changed on a PageView object.
        /// </summary>
        /// <param name="propertyName">
        /// The Property Name mapped to the Control.
        /// </param>
        /// <param name="propertyValue">
        /// The new value.
        /// </param>
        public void PropertyChanged(string propertyName, string propertyValue)
        {
            if (PropertyStore != null)
            {
                propertyToBePersisted = new KeyValuePair<string, string>(propertyName, propertyValue);

                if (PropertyPageSite != null)
                {
                    // Informs the frame that the property page managed by this site has                                                          
                    // changed its state, that is, one or more property values have been 
                    // changed in the page.
                    PropertyPageSite.OnStatusChange(Constants.PROPPAGESTATUS_DIRTY | Constants.PROPPAGESTATUS_VALIDATE);
                }
            }
        }

        /// <summary>
        ///     Get the value of a Property which is stored in the PropertyStore,
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetValueForProperty(string propertyName)
        {
            return PropertyStore?.PropertyValue(propertyName);
        }

        /// <summary>
        ///     Initialize a property page and provides the page with a pointer to the
        ///     IPropertyPageSite interface through which the property page communicates
        ///     with the property frame.
        /// </summary>
        public void SetPageSite(IPropertyPageSite pPageSite)
        {
            PropertyPageSite = pPageSite;
        }

        /// <summary>
        ///     Create the dialog box window for the property page.
        ///     The dialog box is created without a frame, caption, or system menu/controls.
        /// </summary>
        /// <param name="hWndParent">The window handle of the parent of the dialog box that is being created.</param>
        /// <param name="pRect">
        ///     The RECT structure containing the positioning information for the dialog box.
        ///     This method must create its dialog box with the placement and dimensions described by this structure.
        /// </param>
        /// <param name="bModal">Indicates whether the dialog box frame is modal (TRUE) or modeless (FALSE). </param>
        public void Activate(IntPtr hWndParent, RECT[] pRect, int bModal)
        {
            if (pRect == null || pRect.Length == 0)
            {
                throw new ArgumentNullException(nameof(pRect));
            }
            var parentControl = Control.FromHandle(hWndParent);
            var rect = pRect[0];
            MyPageView.Initialize(parentControl, Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom));
        }

        /// <summary>
        ///     Destroy the window created in IPropertyPage::Activate.
        /// </summary>
        public void Deactivate()
        {
            if (myPageView != null)
            {
                myPageView.Dispose();
                myPageView = null;
            }
        }

        /// <summary>
        ///     Retrieve information about the property page.
        /// </summary>
        public void GetPageInfo(PROPPAGEINFO[] pPageInfo)
        {
            PROPPAGEINFO proppageinfo;
            if ((pPageInfo == null) || (pPageInfo.Length == 0))
            {
                throw new ArgumentNullException(nameof(pPageInfo));
            }
            proppageinfo.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            proppageinfo.dwHelpContext = 0;
            proppageinfo.pszDocString = null;
            proppageinfo.pszHelpFile = null;
            proppageinfo.pszTitle = Title;
            proppageinfo.SIZE.cx = MyPageView.Size.Width;
            proppageinfo.SIZE.cy = MyPageView.Size.Height;
            pPageInfo[0] = proppageinfo;
        }

        /// <summary>
        ///     Provide the property page with an array of pointers to objects associated
        ///     with this property page.
        ///     When the property page receives a call to IPropertyPage::Apply, it must send
        ///     value changes to these objects through whatever interfaces are appropriate.
        ///     The property page must query for those interfaces. This method can fail if
        ///     the objects do not support the interfaces expected by the property page.
        /// </summary>
        /// <param name="cObjects">
        ///     The number of pointers in the array pointed to by ppUnk.
        ///     If this parameter is 0, the property page must release any pointers previously
        ///     passed to this method.
        /// </param>
        /// <param name="ppunk"></param>
        public void SetObjects(uint cObjects, object[] ppunk)
        {
            // If cObjects ==0 or ppunk == null, release the PropertyStore.
            if ((ppunk == null) || (cObjects == 0))
            {
                if (PropertyStore != null)
                {
                    PropertyStore.Dispose();
                    PropertyStore = null;
                }
            }
            else
            {
                // Initialize the PropertyStore using the provided objects.
                PropertyStore = GetNewPropertyStore();
                PropertyStore.Initialize(ppunk);

                // If PropertyStore is not null, which means that the PageView UI has been
                // initialized, then it needs to be refreshed.
                if (PropertyStore != null)
                {
                    MyPageView.RefreshPropertyValues();
                }
            }
        }

        /// <summary>
        ///     Make the property page dialog box visible or invisible.
        ///     If the page is made visible, the page should set the focus
        ///     to itself, specifically to the first property on the page.
        /// </summary>
        /// <param name="nCmdShow">
        ///     A command describing whether to become visible (SW_SHOW or SW_SHOWNORMAL) or
        ///     hidden (SW_HIDE). No other values are valid for this parameter.
        /// </param>
        public void Show(uint nCmdShow)
        {
            switch (nCmdShow)
            {
                case Constants.SW_HIDE:
                    MyPageView.HideView();
                    return;
                case Constants.SW_SHOW:
                case Constants.SW_SHOWNORMAL:
                    MyPageView.ShowView();
                    return;
            }
        }

        /// <summary>
        ///     Positions and resizes the property page dialog box within the frame.
        /// </summary>
        /// <param name="pRect"></param>
        public void Move(RECT[] pRect)
        {
            MyPageView.MoveView(Rectangle.FromLTRB(pRect[0].left, pRect[0].top, pRect[0].right, pRect[0].bottom));
        }

        /// <summary>
        ///     Indicate whether the property page has changed since it was activated
        ///     or since the most recent call to Apply.
        /// </summary>
        /// <returns>
        ///     This method returns S_OK to indicate that the property page has changed.
        ///     Otherwise, it returns S_FALSE.
        /// </returns>
        public int IsPageDirty()
        {
            if (!propertyToBePersisted.HasValue)
            {
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Apply the current values to the underlying objects associated with the
        ///     property page as previously passed to IPropertyPage::SetObjects.
        /// </summary>
        public void Apply()
        {
            // Save the changed value to PropertyStore.
            if (propertyToBePersisted.HasValue)
            {
                PropertyStore.Persist(propertyToBePersisted.Value.Key, propertyToBePersisted.Value.Value);
                propertyToBePersisted = null;
            }
        }

        /// <summary>
        ///     Invoke the property page help in response to an end-user request.
        /// </summary>
        /// <param name="pszHelpDir"></param>
        public void Help(string pszHelpDir)
        {
            var iPropertyPageSite = PropertyPageSite as IServiceProvider;
            if (iPropertyPageSite != null)
            {
                var service = (Help)iPropertyPageSite.GetService(typeof(Help));
                service?.DisplayTopicFromF1Keyword(HelpKeyword);
            }
        }

        /// <summary>
        ///     Pass a keystroke to the property page for processing.
        /// </summary>
        /// <param name="pMsg">
        ///     A pointer to the MSG structure describing the keystroke to be processed.
        /// </param>
        /// <returns></returns>
        public int TranslateAccelerator(MSG[] pMsg)
        {
            // Pass the message to the PageView object.
            var message = Message.Create(pMsg[0].hwnd, (int)pMsg[0].message, pMsg[0].wParam, pMsg[0].lParam);
            var hr = MyPageView.ProcessAccelerator(ref message);
            pMsg[0].lParam = message.LParam;
            pMsg[0].wParam = message.WParam;
            return hr;
        }

        /// <summary>
        ///     Specifies which field is to receive the focus when the property page is activated.
        /// </summary>
        /// <param name="DISPID">
        ///     The property that is to receive the focus.
        /// </param>
        public void EditProperty(int DISPID)
        {
        }

        /// <summary>
        ///     Applies the current values to the underlying objects associated with the
        ///     property page as previously passed to IPropertyPage::SetObjects.
        /// </summary>
        int IPropertyPage.Apply()
        {
            Apply();
            return VSConstants.S_OK;
        }
    }
}
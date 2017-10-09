using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace appCore.UI
{
    /// <summary>
    /// An extention of the MS ErrorProvider implementation,
    /// which attempts to fix a problem, where the tool tip 
    /// window disapears on click and does not easily return to
    /// being displayed when the error provider icon is revisited.
    /// </summary>
    public class ErrorProviderFixed : ErrorProvider
    {
        ErrorProviderFixManager mToolTipFix = null;

        public ErrorProviderFixed()
            : base()
        {
            mToolTipFix = new ErrorProviderFixManager(this);
        }

        public ErrorProviderFixed(ContainerControl parentControl)
            : base(parentControl)
        {
            mToolTipFix = new ErrorProviderFixManager(this);
        }

        public ErrorProviderFixed(IContainer container)
            : base(container)
        {
            mToolTipFix = new ErrorProviderFixManager(this);
        }

        protected override void Dispose(bool disposing)
        {
            mToolTipFix.Dispose();
            base.Dispose(disposing);
        }

        public new void SetError(Control control, string value)
        {
            base.SetError(control, value);
            if (control is AMTTextBox)
                ((AMTTextBox)control).ErrorIssued = !string.IsNullOrEmpty(value);
        }
    }

    public class ErrorProviderFixManager : IDisposable
    {
        private ErrorProvider mTheErrorProvider = null;
        private Timer mTmrCheckHandlesProc = null;
        private Hashtable mHashOfNativeWindows = new Hashtable();

        public void Dispose()
        {
            mTmrCheckHandlesProc.Stop();
            mTmrCheckHandlesProc.Dispose();
            mTmrCheckHandlesProc = null;
        }

        /// <summary>
        /// constructor, which will start a timer which will
        /// keep the errorProviders tooltip window up-to-date and enabled.
        ///
        /// Todo: I would like to do this without a timer (Suggestions welcome).
        /// Email me at: km@KevinMPhoto.com
        /// </summary>
        /// <param name="ep"></param>
        public ErrorProviderFixManager(ErrorProvider ep)
        {
            mTheErrorProvider = ep;

            mTmrCheckHandlesProc = new Timer();
            mTmrCheckHandlesProc.Enabled = true;
            mTmrCheckHandlesProc.Interval = 1000;
            mTmrCheckHandlesProc.Tick += new EventHandler(tmr_CheckHandels);
        }

        /// <summary>
        /// Resets the error provider, error messages.  I've noticed that when
        /// you click on an error provider, while its tool tip is displayed,
        /// the tool tip doesn't return.  It will return if the text is reset, or
        /// if the user is able to hover over anohter error provider message for
        /// that errorProvider instance.
        /// 
        /// Todo: I would like to find an easier way to fix this...
        /// Email me at: km@KevinMPhoto.com
        /// </summary>
        private void RefreshProviderErrors()
        {
            Hashtable hashRes = (Hashtable)GetFieldValue(mTheErrorProvider, "items");
            foreach (Control control in hashRes.Keys)
            {
                if (hashRes[control] == null)
                {
                    break;
                }

                if (!(bool)GetFieldValue(hashRes[control], "toolTipShown"))
                {
                    if (mTheErrorProvider.GetError(control) != null && mTheErrorProvider.GetError(control).Length > 0)
                    {
                        string str = mTheErrorProvider.GetError(control);
                        ErrorBlinkStyle prev = mTheErrorProvider.BlinkStyle;
                        mTheErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                        mTheErrorProvider.SetError(control, "");
                        mTheErrorProvider.SetError(control, str);
                        mTheErrorProvider.BlinkStyle = prev;
                    }
                }
            }
        }

        /// <summary>
        /// This method checks to see if the error provider's tooltip window has
        /// changed and if it has updates this Native window with the new handle.
        /// 
        /// If there is some sort of change it also calls the RefreshProviderErrors, which
        /// fixes the tooltip problem...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmr_CheckHandels(object sender, EventArgs e)
        {
            if (mTheErrorProvider.ContainerControl == null)
            {
                return;
            }

            if (mTheErrorProvider.ContainerControl.Visible)
            {
                Hashtable hashRes = (Hashtable)GetFieldValue(mTheErrorProvider, "windows");
                if (hashRes.Count > 0)
                {
                    foreach (Object obj in hashRes.Keys)
                    {
                        ErrorProviderNativeWindowHook hook = null;
                        if (mHashOfNativeWindows.Contains(obj))
                        {
                            hook = (ErrorProviderNativeWindowHook)mHashOfNativeWindows[obj];
                        }
                        else
                        {
                            hook = new ErrorProviderNativeWindowHook();
                            mHashOfNativeWindows[obj] = hook;
                        }

                        NativeWindow nativeWindow = GetFieldValue(hashRes[obj], "tipWindow") as NativeWindow;
                        if (nativeWindow != null && hook.Handle == IntPtr.Zero)
                        {
                            hook.AssignHandle(nativeWindow.Handle);
                        }
                    }
                }

                foreach (ErrorProviderNativeWindowHook hook in mHashOfNativeWindows.Values)
                {
                    if (hook.mBlnTrigerRefresh)
                    {
                        hook.mBlnTrigerRefresh = false;
                        RefreshProviderErrors();
                    }
                }
            }
        }

        /// <summary>
        /// A helper method, which allows us to get the value of private fields.
        /// </summary>
        /// <param name="instance">the object instance</param>
        /// <param name="name">the name of the field, which we want to get</param>
        /// <returns>the value of the private field</returns>
        private object GetFieldValue(object instance, string name)
        {
            if (instance == null) return null;

            FieldInfo fInfo = null;
            Type type = instance.GetType();
            while (fInfo == null && type != null)
            {
                fInfo = type.GetField(name, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance);
                type = type.BaseType;
            }
            if (fInfo == null)
            {
            }
            return fInfo.GetValue(instance);
        }
    }

    /// <summary>
    /// A NativeWindow, which we use to trap the WndProc messages
    /// and patch up the ErrorProvider/ToolTip bug.
    /// </summary>
    public class ErrorProviderNativeWindowHook : NativeWindow
    {
        private int mInt407Count = 0;
        internal bool mBlnTrigerRefresh = false;

        /// <summary>
        /// This is the magic.  On the 0x407 message we need to reset the 
        /// error provider; however, I can't do it directly in the WndProc; 
        /// otherwise, we could get a cross threading type exception, since
        /// this WndProc is called on a seperate thread.  The Timer will make
        /// sure the work gets done on the Main GUI thread.          
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x407)
            {
                mInt407Count++;
                if (mInt407Count > 3)  // if this occures we need to release...
                {
                    this.ReleaseHandle();
                    mBlnTrigerRefresh = true;
                }
            }
            else
            {
                mInt407Count = 0;
            }

            base.WndProc(ref m);
        }
    }
}

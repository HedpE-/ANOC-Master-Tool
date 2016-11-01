/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 01-07-2016
 * Time: 02:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace appCore.Shifts
{
    //using System.Runtime.InteropServices;
    //
    //public class WholeShiftsPanel : Panel, IMessageFilter
    //{
    //	private bool managed;
    //
    //	public WholeShiftsPanel () : this (true) {
    //	}
    //
    //	public WholeShiftsPanel (bool start) {
    //		managed = false;
    //		if (start)
    //			ManagedMouseWheelStart();
    //	}
    //
    //	protected override void Dispose (bool disposing) {
    //		if (disposing)
    //			ManagedMouseWheelStop();
    //		base.Dispose(disposing);
    //	}
    //
    //	/************************************
    //	 * IMessageFilter implementation
    //	 * *********************************/
    //	private const int WM_MOUSEWHEEL = 0x20a;
    //	// P/Invoke declarations
    //	[DllImport(""user32.dll"")]
    //	private static extern IntPtr WindowFromPoint (Point pt);
    //	[DllImport(""user32.dll"")]
    //	private static extern IntPtr SendMessage (IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    //
    //	private bool IsChild (Control ctrl) {
    //		Control loopCtrl = ctrl;
    //
    //		while (loopCtrl != null && loopCtrl != this)
    //			loopCtrl = loopCtrl.Parent;
    //
    //		return (loopCtrl == this);
    //	}
    //
    //	public bool PreFilterMessage (ref Message m) {
    //		if (m.Msg == WM_MOUSEWHEEL) {
    //			//Ensure the message was sent to a child of the current control
    ////			if (IsChild(Control.FromHandle(m.HWnd))) {
    //				// Find the control at screen position m.LParam
    //				Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
    //
    //				//Ensure control under the mouse is valid and is not the target control
    //				//otherwise we'd be trap in a loop.
    //				IntPtr hWnd = WindowFromPoint(pos);
    //				if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null) {
    //					SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
    //					return true;
    //				}
    ////			}
    //		}
    //		return false;
    //	}
    //
    //	/****************************************
    //	 * MouseWheelManagedForm specific methods
    //	 * **************************************/
    //	public void ManagedMouseWheelStart () {
    //		if (!managed) {
    //			managed = true;
    //			Application.AddMessageFilter(this);
    //		}
    //	}
    //
    //	public void ManagedMouseWheelStop () {
    //		if (managed) {
    //			managed = false;
    //			Application.RemoveMessageFilter(this);
    //		}
    //	}
    public class WholeShiftsPanel : Panel
	{
		public WholeShiftsPanel()
		{
			this.SetStyle(ControlStyles.Selectable, true);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();
			base.OnMouseDown(e);
		}

		protected override void OnEnter(EventArgs e)
		{
			this.Invalidate();
			base.OnEnter(e);
			this.Focus();
		}

		protected override void OnLeave(EventArgs e)
		{
			this.Invalidate();
			base.OnLeave(e);
			this.Hide();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (this.Focused) {
				var rc = this.ClientRectangle;
				rc.Inflate(-2, -2);
				ControlPaint.DrawFocusRectangle(e.Graphics, rc);
			}
		}
	}
}
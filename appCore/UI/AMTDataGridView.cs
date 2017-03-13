/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 12/03/2017
 * Time: 21:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTDataGridView.
	/// </summary>
	public class AMTDataGridView : DataGridView
	{
		bool alwaysVisibleVScrollBar;
		public bool AlwaysVisibleVScrollBar {
			get { return alwaysVisibleVScrollBar; }
			set {
				alwaysVisibleVScrollBar =
					VerticalScrollBar.Visible = value;
			}
		}
		
		public bool DoubleBuffer {
			get { return DoubleBuffered; }
			set { DoubleBuffered = value; }
		}
		
		public AMTDataGridView() {
			this.VerticalScrollBar.VisibleChanged += VerticalScrollBar_VisibleChanged;
		}
		
		void VerticalScrollBar_VisibleChanged(object sender, EventArgs e) {
			if(alwaysVisibleVScrollBar && !VerticalScrollBar.Visible)
				VerticalScrollBar.Visible = true;
			if(VerticalScrollBar.Visible) {
				VerticalScrollBar.Location = new System.Drawing.Point(Width - VerticalScrollBar.Width, 0);
				VerticalScrollBar.SetBounds(VerticalScrollBar.Location.X, VerticalScrollBar.Location.Y, VerticalScrollBar.Width, Height);
			}
		}
	}
}

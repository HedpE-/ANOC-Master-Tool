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
			this.VerticalScrollBar.SizeChanged += VerticalScrollBar_SizeChanged;
		}
		
		void VerticalScrollBar_VisibleChanged(object sender, EventArgs e) {
			if(alwaysVisibleVScrollBar && !VerticalScrollBar.Visible)
				VerticalScrollBar.Visible = true;
			
			if(VerticalScrollBar.Visible) {
				VerticalScrollBar.Location = new System.Drawing.Point(Width - VerticalScrollBar.Width - 1, 1);
				VerticalScrollBar.Height = Height - 2;
				VerticalScrollBar.SetBounds(VerticalScrollBar.Location.X, VerticalScrollBar.Location.Y, VerticalScrollBar.Width, VerticalScrollBar.Height);
			}
		}
		
		void VerticalScrollBar_SizeChanged(object sender, EventArgs e) {
			this.VerticalScrollBar.SizeChanged -= VerticalScrollBar_SizeChanged;
			VerticalScrollBar.Location = new System.Drawing.Point(Width - VerticalScrollBar.Width - 1, 1);
			VerticalScrollBar.Height = Height - 2;
			VerticalScrollBar.SetBounds(VerticalScrollBar.Location.X, VerticalScrollBar.Location.Y, VerticalScrollBar.Width, VerticalScrollBar.Height);
			this.VerticalScrollBar.SizeChanged += VerticalScrollBar_SizeChanged;
		}
	}
}

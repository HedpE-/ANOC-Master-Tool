﻿/*
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
		
		private bool SuppressAutoSelection { get; set; }
		
		public AMTDataGridView() {
			SuppressAutoSelection = true;
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
		
		public new /*shadowing*/ object DataSource
		{
			get
			{
				return base.DataSource;
			}
			set
			{
				SuppressAutoSelection = true;
				Form parent = this.FindForm();

				// Either the selection gets cleared on form load....
				parent.Load -= parent_Load;
				parent.Load += parent_Load;

				base.DataSource = value;

				// ...or it gets cleared straight after the DataSource is set
				ClearSelectionAndResetSuppression();
			}
		}

		protected override void OnSelectionChanged(EventArgs e)
		{
			if (SuppressAutoSelection)
				return;

			base.OnSelectionChanged(e);
		}

		void ClearSelectionAndResetSuppression()
		{
			if (this.SelectedRows.Count > 0 || this.SelectedCells.Count > 0)
			{
				this.ClearSelection();
				SuppressAutoSelection = false;
			}
		}

		void parent_Load(object sender, EventArgs e)
		{
			ClearSelectionAndResetSuppression();
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 16/12/2016
 * Time: 19:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of LoadingPanel.
	/// </summary>
	public class LoadingPanel : Panel
	{
		BackgroundWorker backgroundWorker = new BackgroundWorker();
		
		int _spinnerSize = 32;
		public int spinnerSize
		{
			get {
				return _spinnerSize;
			}
			set {
				_spinnerSize = value;
			}
		}
		
		double _opacity = 0.4;
		public double Opacity
		{
			get {
				return _opacity;
			}
			set {
				_opacity = value;
			}
		}
		
		/// <summary>
		/// Show LoadingPanel with both Threaded and Non Threaded instructions.
		/// For Threaded or Non Threaded action only, pass the other action argument as null
		/// </summary>
		/// <param name="actionThreaded"></param>
		/// <param name="actionNonThreaded"></param>
		/// <param name="showLoading"></param>
		/// <param name="parentControl"></param>
		public void Show(Action actionThreaded, Action actionNonThreaded, bool showLoading, Control parentControl) {
			darkenBackgroundForm(showLoading, parentControl);
			
			backgroundWorker.DoWork += delegate {
				if(actionThreaded != null)
					actionThreaded();
			};
			
			backgroundWorker.RunWorkerCompleted += delegate {
				if(actionNonThreaded != null)
					actionNonThreaded();
				Parent.Controls.Remove(this);
				this.Dispose();
			};
			
			backgroundWorker.RunWorkerAsync();
		}
		
		void darkenBackgroundForm(bool showLoading, Control parentControl) {
//			Form parentForm = getParentForm(parentControl);
			Form parentForm = parentControl.FindForm();
			if(!(parentControl is Form))
				parentControl.Parent.Controls.Add(this);
			else
				parentControl.Controls.Add(this);
			// take a screenshot of the form and darken it
			Bitmap bmp = new Bitmap(parentControl.ClientRectangle.Width, parentControl.ClientRectangle.Height);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.CompositingMode = CompositingMode.SourceOver;
				g.CopyFromScreen(parentControl.PointToScreen(new Point(0, 0)), new Point(0, 0), parentControl.ClientRectangle.Size);
				Color darken = Color.FromArgb((int)(255 * Opacity), Color.Black);
				using (Brush brsh = new SolidBrush(darken))
					g.FillRectangle(brsh, parentControl.ClientRectangle);
			}
			
//			Location = new Point(parentControl.PointToScreen(Point.Empty).X - parentForm.PointToScreen(Point.Empty).X,
//			                     parentControl.PointToScreen(Point.Empty).Y - parentForm.PointToScreen(Point.Empty).Y);
			
			if(!(parentControl is Form))
				Location = parentControl.Location;
			else
				Location = Point.Empty;
			
			Size = parentControl.ClientRectangle.Size;
			BackgroundImage = bmp;
			
			BringToFront();
			if(showLoading) {
				Point loc = PointToScreen(Point.Empty);
				loc.X = (parentControl.Width - spinnerSize) / 2;
				loc.Y = (parentControl.Height - spinnerSize) / 2;
//				ProgressSpinner loadingBox = new ProgressSpinner();
//				loadingBox.LoadGIFImage = Resources.spinner1;
//				loadingBox.BackColor = Color.Transparent;
//				loadingBox.Size = new Size(spinnerSize, spinnerSize);
//				loadingBox.Location = loc;
//				Controls.Add(loadingBox);
//				loadingBox.BringToFront();
//				loadingBox.Start();
				PictureBox loadingBox = new PictureBox();
				loadingBox.BackColor = Color.Transparent;
				loadingBox.Image = loadingBox.InitialImage = Resources.spinner1;
				loadingBox.Size = new Size(spinnerSize, spinnerSize);
				loadingBox.SizeMode = PictureBoxSizeMode.StretchImage;
				loadingBox.Location = loc;
				Controls.Add(loadingBox);
				loadingBox.BringToFront();
			}
		}
		
		Form getParentForm(Control control) {
			Form parentForm = null;
			if(control is Form)
				parentForm = (Form)control;
			else {
				while(parentForm == null) {
					try {
						control = control.Parent;
						if(control is Form)
							parentForm = (Form)control;
					}
					catch {}
				}
			}
			return parentForm;
		}
	}
}

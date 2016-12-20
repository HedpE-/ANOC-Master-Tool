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
		/// Show LoadingPanel with both Threaded and Non Threaded instructions
		/// For Threaded or Non Threaded action only, pass the other action argument as null
		/// </summary>
		/// <param name="actionThreaded"></param>
		/// <param name="actionNonThreaded"></param>
		/// <param name="showLoading"></param>
		/// <param name="parentControl"></param>
		public void Show(Action actionThreaded, Action actionNonThreaded, bool showLoading, Control parentControl) {
			Form parentForm = (Form)parentControl ?? getParentForm(parentControl);
			parentForm.Controls.Add(this);
			// take a screenshot of the form and darken it
			Bitmap bmp = new Bitmap(Parent.ClientRectangle.Width, Parent.ClientRectangle.Height);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.CompositingMode = CompositingMode.SourceOver;
				g.CopyFromScreen(Parent.PointToScreen(new Point(0, 0)), new Point(0, 0), Parent.ClientRectangle.Size);
				Color darken = Color.FromArgb((int)(255 * Opacity), Color.Black);
				using (Brush brsh = new SolidBrush(darken))
					g.FillRectangle(brsh, Parent.ClientRectangle);
			}

			Location = new Point(0, 0);
			Size = Parent.ClientRectangle.Size;
			BackgroundImage = bmp;
			
			BringToFront();
			if(showLoading) {
				Point loc = PointToScreen(Point.Empty);
				loc.X = (Parent.Width - spinnerSize) / 2;
				loc.Y = (Parent.Height - spinnerSize) / 2;
				PictureBox loadingBox = new PictureBox();
				loadingBox.BackColor = Color.Transparent;
				loadingBox.Image = loadingBox.InitialImage = Resources.spinner1;
				loadingBox.Size = new Size(spinnerSize, spinnerSize);
				loadingBox.SizeMode = PictureBoxSizeMode.StretchImage;
				loadingBox.Location = loc;
				Controls.Add(loadingBox);
				loadingBox.BringToFront();
			}
			
			if(actionThreaded != null)
				backgroundWorker.DoWork += delegate { actionThreaded(); };
			
			backgroundWorker.RunWorkerCompleted += delegate {
				if(actionNonThreaded != null)
					actionNonThreaded();
				Parent.Controls.Remove(this);
				this.Dispose();
			};
			
			backgroundWorker.RunWorkerAsync();
		}
		
		Form getParentForm(Control control) {
			Form parentForm = null;
			while(parentForm == null) {
				control = control.Parent;
				if(control is Form)
					parentForm = (Form)control;
			}
			return parentForm;
		}
	}
}

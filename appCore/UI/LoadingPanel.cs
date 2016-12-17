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
		
		public LoadingPanel() {
		}
		
		public void StartBackgroundWorker(Action action, bool showLoading) {
			// Resolve Parent Form if needed
//			Form parentForm = control is Form ? control as Form : Toolbox.Tools.getParentForm(control);
			// take a screenshot of the form and darken it
			Bitmap bmp = new Bitmap(Parent.ClientRectangle.Width, Parent.ClientRectangle.Height);
			using (Graphics G = Graphics.FromImage(bmp))
			{
				G.CompositingMode = CompositingMode.SourceOver;
				G.CopyFromScreen(Parent.PointToScreen(new Point(0, 0)), new Point(0, 0), Parent.ClientRectangle.Size);
				const double percent = 0.40;
				Color darken = Color.FromArgb((int)(255 * percent), Color.Black);
				using (Brush brsh = new SolidBrush(darken))
				{
					G.FillRectangle(brsh, Parent.ClientRectangle);
				}
			}

			// put the darkened screenshot into a Panel and bring it to the front:
//			using (Panel p = new Panel())
//			{
			Location = new Point(0, 0);
			Size = Parent.ClientRectangle.Size;
			BackgroundImage = bmp;
//			parentForm.Controls.Add(p);
//			BringToFront();
			
			// display your dialog somehow:
			if(showLoading) {
				Point loc = PointToScreen(Point.Empty);
				loc.X = loc.X + ((Width - 32) / 2);
				loc.Y = loc.Y + ((Height - 32) / 2);
				PictureBox loadingBox = new PictureBox();
				loadingBox.Image = Resources.spinner1;
				loadingBox.Location = loc;
				Controls.Add(loadingBox);
//					Loading.ShowLoadingForm(loc, parentForm);
			}
			
			backgroundWorker.DoWork +=  delegate { action(); };
            backgroundWorker.RunWorkerCompleted += delegate { 
				Parent.Controls.Remove(this);
				this.Dispose();
			};
            backgroundWorker.RunWorkerAsync();
		}

//        void OnDoWork(object sender, DoWorkEventArgs e)
//        {
//			action();
//        }

//        void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            // hide animation
//            this.pictureBox.Image = null;
//            // show result indication
//            if (e.Cancelled)
//            {
//                this.labelProgress.Text = "Operation cancelled by the user!";
//                this.pictureBox.Image = Properties.Resources.WarningImage;
//            }
//            else
//            {
//                if (e.Error != null)
//                {
//                    this.labelProgress.Text = "Operation failed: " + e.Error.Message;
//                    this.pictureBox.Image = Properties.Resources.ErrorImage;
//                }
//                else
//                {
//                    this.labelProgress.Text = "Operation finished successfuly!";
//                    this.pictureBox.Image = Properties.Resources.InformationImage;
//                }
//            }
//            // restore button states
//            this.buttonStart.Enabled = true;
//            this.buttonCancel.Enabled = false;
//            this.buttonError.Enabled = false;
//        }
	}
}

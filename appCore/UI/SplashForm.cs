/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 11-01-2016
 * Time: 19:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of SplashForm.
	/// </summary>
	public partial class SplashForm : Form
	{
		//Delegate for cross thread call to close
		private delegate void CloseDelegate();
		static Thread thread;
		//The type of form to be displayed as the splash screen.
		public static SplashForm splashForm;

		static System.Windows.Forms.Timer tIn = new System.Windows.Forms.Timer();
		static System.Windows.Forms.Timer tOut = new System.Windows.Forms.Timer();
		
		static bool ShowVersionDetails;

		SplashForm()
		{
			InitializeComponent();
			ExtraFormSettings();
			tOut.Enabled = false;
			tIn.Interval = 50;
			tOut.Interval = 25;
			tIn.Tick += fadeIn;
			tOut.Tick += fadeOut;
			tIn.Enabled = true;
		}
		
		static public void ShowSplashScreen(bool versionDetails)
		{
			// Make sure it is only launched once.
			if(splashForm != null)
				return;
			ShowVersionDetails = versionDetails;
			thread = new Thread(new ThreadStart(SplashForm.ShowForm));
			thread.Name = "SplashForm.ShowForm";
			thread.IsBackground = true;
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		static void ShowForm()
		{
			splashForm = new SplashForm();
			
			Application.Run(splashForm);
		}
		
		void SplashFormShown(object sender, EventArgs e)
		{
			this.BringToFront();
		}

		public static void CloseForm()
		{
			// Loop for a maximum of 200ms if the screen hasn't yet been loaded.
			for (var i = 0; i < 200; i++)
			{
				if (splashForm != null && splashForm.IsHandleCreated)
				{
					break;
				}

				Thread.Sleep(1);
			}

			// Don't try to close if it is already closed.
			// If the screen form is still null after waiting, it was most likely already closed.
			if (splashForm != null && splashForm.IsHandleCreated)
			{
				splashForm.Invoke(new Action(delegate {
				                             	tOut.Enabled = true;
				                             }));
				wait4Timer();
				splashForm.Invoke(new CloseDelegate(CloseFormInternal));
			}
			
			if (!thread.Join(TimeSpan.FromSeconds(5))) {
				try {
					thread.Abort();
				}
				catch(ThreadAbortException) { }
			}
		}

		static void CloseFormInternal()
		{
			if (splashForm == null)
				return;
			
			splashForm.Close();
			splashForm = null;
		}
		
		public static void UpdateLabelText(string text)
		{
			try {
				splashForm.BeginInvoke(new Action(delegate {
				                                  	Label lb1 = (Label)splashForm.Controls["label1"];
				                                  	lb1.Text = text + ", please wait...";
				                                  	int difference = splashForm.ClientSize.Width - lb1.Width;
				                                  	lb1.Left = difference / 2;
				                                  }));
			}
			catch {}
		}
		
		void ExtraFormSettings()
		{
			if(ShowVersionDetails) {
				var fnv = FileVersionInfo.GetVersionInfo(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\appCore.dll");
				
				// 
				// versionLabel
				// 
				Label versionLabel = new Label();
				versionLabel.AutoSize = true;
				versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				versionLabel.Location = new System.Drawing.Point(12, 126);
				versionLabel.Name = "versionLabel";
				versionLabel.Size = new System.Drawing.Size(0, 12);
				versionLabel.Text = "Version: " + fnv.FileVersion;
				// 
				// releaseDateLabel
				// 
				Label releaseDateLabel = new Label();
				releaseDateLabel.AutoSize = true;
				releaseDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				releaseDateLabel.Location = new System.Drawing.Point(12, 114);
				releaseDateLabel.Name = "releaseDateLabel";
				releaseDateLabel.Size = new System.Drawing.Size(0, 12);
				releaseDateLabel.Text = "Release date: " + File.GetLastWriteTime(fnv.FileName).ToString("dd-MM-yyyy");
				
				this.Controls.AddRange(new Control[]{
				                       	versionLabel,
				                       	releaseDateLabel
				                       });
			}
			label1.Left = (this.ClientSize.Width - label1.Width) / 2;
			pictureBox1.Left = (this.ClientSize.Width - pictureBox1.Width) / 2;
		}
		
		void fadeIn(object sender, EventArgs e)
		{
			// Fade in by increasing the opacity of the splash to 1.0
			if (this.Opacity < 1.0)
				this.Opacity += 0.075;
			else
				tIn.Enabled = false;
		}
		
		void fadeOut(object sender, EventArgs e)
		{
			if (this.Opacity > 0)
				this.Opacity -= 0.075;
			else
				tOut.Enabled = false;
		}
		
		public static void wait4Timer()
		{
			while(tIn.Enabled || tOut.Enabled) { }
		}
	}
}
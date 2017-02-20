/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 11-01-2016
 * Time: 19:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of Loading.
	/// </summary>
	public partial class Loading : Form
	{
		private delegate void CloseDelegate(); //Delegate for cross thread call to close
		static System.Drawing.Point formXY;
		static Loading loadingForm;
		static Thread thread;
		
		static int _spinnerSize = 32;		
		public static int spinnerSize
		{
			get {
				return _spinnerSize;
			}
			set {
				_spinnerSize = value;
			}
		}
		
		Loading()
		{
			InitializeComponent();
		}

		public static void ShowLoadingForm(System.Drawing.Point loc, Form owner)
		{
			// Make sure it is only launched once.
			
			if (loadingForm != null)
				return;
			//loadingForm.Owner = owner;
			formXY = loc;
			thread = new Thread(new ThreadStart(ShowLoadingForm));
			thread.Name = "ShowLoadingForm";
			thread.IsBackground = true;
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		static void ShowLoadingForm()
		{
			loadingForm = new Loading();
			loadingForm.StartPosition = FormStartPosition.Manual;
			loadingForm.Location = formXY;
			Application.Run(loadingForm);
		}

		public static void CloseLoadingForm()
		{
			// Loop for a maximum of 100ms if the screen hasn't yet been loaded.
			for (var i = 0; i < 100; i++)
			{
				if (loadingForm != null && loadingForm.IsHandleCreated)
				{
					break;
				}

				Thread.Sleep(1);
			}

			// Don't try to close if it is already closed.
			// If the screen form is still null after waiting, it was most likely already closed.
			if (loadingForm != null && loadingForm.IsHandleCreated)
			{
				loadingForm.Invoke(new CloseDelegate(CloseFormInternal));
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
			loadingForm.Close();
			loadingForm = null;
//			Control.CheckForIllegalCrossThreadCalls = false; // CAUTION::: Disable illegal CrossThread operations check
		}
//		
//		public static void UpdateLabelText(string text)
//		{
//			Label lb1 = (Label)loadingForm.Controls["label1"];
//			loadingForm.BeginInvoke(new Action(delegate {
//			                                   	lb1.Text = text + ", please wait...";
//			                                   	int labelWidth = lb1.Width;
//			                                   	int controlWidth = loadingForm.ClientSize.Width;
//			                                   	int difference = controlWidth - labelWidth;
//			                                   	lb1.Left = difference / 2;
//			                                   }));
//		}
	}
}
/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 23/11/2016
 * Time: 22:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using RestSharp;

namespace appCore.Remedy.UI
{
	/// <summary>
	/// Description of RemedyWebBrowser.
	/// </summary>
	public class RemedyWebBrowser : Form
	{
		WebBrowser internalBrowser = new WebBrowser();
		RestClient client = new RestClient();
		Uri currentUri;
		
		public RemedyWebBrowser()
		{
			AutoScaleMode = AutoScaleMode.Font;
			Text = "RemedyWebBrowser";
			Name = "RemedyWebBrowser";
			Size = internalBrowser.Size = new Size(1000, 800);
//			FormBorderStyle = FormBorderStyle.None;
//			ControlBox = false;
//			MaximizeBox = false;
//			MinimizeBox = false;
//			ShowIcon = false;
//			ShowInTaskbar = false;
			
			Controls.Add(internalBrowser);
		}

		void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			if (e.Url.AbsolutePath != "blank")
			{
				currentUri = new Uri(currentUri, e.Url.AbsolutePath);
				HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(currentUri);

				HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

				internalBrowser.DocumentStream = myResponse.GetResponseStream();
				e.Cancel = true;
			}
		}
	}
}

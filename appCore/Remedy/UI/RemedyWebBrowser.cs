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
using BMC.ARSystem;
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
			Load += Form1_Load;
		}

		void Form1_Load(object sender, EventArgs e)
		{
			Server arserver = new Server();
			arserver.Login("https://ukremprdpxy-vip.dc-dublin.de/arsys", "goncalvesr1", "RG_Nov16", "");
			
			currentUri = new Uri(@"https://ukremprdpxy-vip.dc-dublin.de/arsys/shared/login.jsp");
			
			client.BaseUrl = currentUri;
			client.Proxy = new WebProxy("http://vfukukproxy.internal.vodafone.com:8080", true);
			client.Proxy.Credentials = CredentialCache.DefaultCredentials;
			
			IRestRequest request = new RestRequest("/arsys/servlet/LoginServlet", Method.POST);
			request.AddParameter("username", "goncalvesr1");
			request.AddParameter("pwd", "RG_Nov16");
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			
			IRestResponse response = client.Execute(request);
			//WebProxy myProxy = new WebProxy("208.52.92.160:80");
			//myRequest.Proxy = myProxy;

			internalBrowser.DocumentText = response.Content;

			//            internalBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);
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

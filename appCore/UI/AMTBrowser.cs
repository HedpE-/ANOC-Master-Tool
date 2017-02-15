/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 28-07-2016
 * Time: 22:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.OI;
using appCore.Web;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Generic;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTBrowser.
	/// </summary>
	public class AMTBrowser : WebBrowser
	{
		public AMTBrowser()
		{
//			Navigating += OnNavigate;
		}
		
		void OnNavigate(object sender, WebBrowserNavigatingEventArgs e) {	
			if(!OiConnection.LoggedOn)
				OiConnection.InitiateOiConnection();
			e.Cancel = true;
			ResumeSession(e.Url, OiConnection.OICookieContainer);
		}
		
		public void ResumeSession(string url, CookieContainer container) {
			string cookie_string = string.Empty;
			List<Cookie> cookies = container.ToList();
			foreach (Cookie cookie in cookies)
			{
				cookie_string += cookie + ";";
				InternetSetCookie(url, cookie.Name, cookie.Value);
			}
			Navigate(url, "", null, "Cookie: " + cookie_string + Environment.NewLine);
		}
		
		public void ResumeSession(Uri uri, CookieContainer container) {
			ResumeSession(uri.AbsoluteUri, container);
		}
		
		public void DisplayHtml(string html)
		{
			Navigate("about:blank");
			if (Document != null)
			{
				Document.Write(string.Empty);
			}
			DocumentText = html;
		}
		
		[DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
	}
}

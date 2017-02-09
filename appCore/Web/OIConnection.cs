/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 29-07-2016
 * Time: 16:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using RestSharp;
using appCore.UI;

namespace appCore.Web
{
	/// <summary>
	/// Description of OIConnection.
	/// </summary>
	public static class OIConnection
	{
		static RestClient client;
//		static List<Parameter> requiredHeaders = new List<Parameter>();
		static string OIUsername = string.Empty;
		static string OIPassword = string.Empty;
		static Uri OiPortalUri = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/");
		static Uri OldOiPortalUri = new Uri("http://195.233.194.118/");
		public static bool LoggedOn;
		public static bool LoggedOnOldOiPortal;
		public static CookieContainer OICookieContainer {
			get;
			private set;
		}
		public static CookieContainer OldOICookieContainer {
			get;
			private set;
		}
		public static bool Available {
			get {
				return CheckAvailability(false) == HttpStatusCode.OK;
			}
			private set {}
		}
		
		static void Logon(bool OldOiPortal)
		{
			// Load OI credentials from SettingsFile
			OIUsername = Settings.SettingsFile.OIUsername;
			OIPassword = Settings.SettingsFile.OIPassword;
			if(string.IsNullOrEmpty(OIUsername) || string.IsNullOrEmpty(OIPassword)) // Request credentials if empty on SettingsFile
				RequestOiCredentials();
		Retry:
			if(!OldOiPortal) {
				OICookieContainer = new CookieContainer();
				client.CookieContainer = OICookieContainer;
			}
			else {
				OldOICookieContainer = new CookieContainer();
				client.CookieContainer = OldOICookieContainer;
			}
			client.BaseUrl = OldOiPortal ? OldOiPortalUri : OiPortalUri;
			
			IWebProxy proxy;
			try {
				proxy = Settings.CurrentUser.NetworkDomain == "internal.vodafone.com" ?
					new WebProxy("http://vfukukproxy.internal.vodafone.com:8080/", true) :
					WebRequest.GetSystemWebProxy();
			}
			catch (Exception) {
				proxy = WebRequest.GetSystemWebProxy();
			}
			
			proxy.Credentials = CredentialCache.DefaultNetworkCredentials;	
			client.Proxy = proxy;
			
//			client.Proxy = WebRequest.DefaultWebProxy;
//			client.Proxy.Credentials = CredentialCache.DefaultCredentials;
			
			string restRequest = OldOiPortal ?
				"/SSO/index.asp?url=%2FSSO%2Fsecurity%2Ephp&proxylogin=true" :
				"/sso/index.php?url=%2F";
			IRestRequest request = new RestRequest(restRequest, Method.POST);
			request.AddParameter("username", OIUsername);
			request.AddParameter("password", OIPassword.DecryptText());
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			
			IRestResponse response = client.Execute(request);
			
//			if(!OldOiPortal)
//				requiredHeaders = (List<Parameter>)response.Headers;
			
			if((!OldOiPortal && response.Content.Contains(@"<div class=""logged_in"">")) || (OldOiPortal && response.ContentLength == 167 && response.Content.Contains("<meta http-equiv=" + '"' + "refresh" + '"' + " content=" + '"' + "0;URL='http://operationalintelligence.vf-uk.corp.vodafone.com/'" + '"' + " />"))) {
				if(OldOiPortal)
					LoggedOnOldOiPortal = true;
				else
					LoggedOn = true;
			}
			else {
				if(OldOiPortal)
					LoggedOnOldOiPortal = false;
				else
					LoggedOn = false;
				
				DialogResult res = appCore.UI.FlexibleMessageBox.Show("Login failed with current OI credentials, do you want to change?\n\nIncorrect OI credentials prevents the use of this Tool's full features.","Login Failed",MessageBoxButtons.YesNo,MessageBoxIcon.Error);
				if(res == DialogResult.Yes) {
					RequestOiCredentials();
					goto Retry;
				}
			}
		}
		
		static HttpStatusCode CheckAvailability(bool OldOiPortal) {
			client.BaseUrl = OldOiPortal ? OldOiPortalUri : OiPortalUri;
			
			IWebProxy proxy;
			try {
				proxy = Settings.CurrentUser.NetworkDomain == "internal.vodafone.com" ?
					new WebProxy("http://vfukukproxy.internal.vodafone.com:8080/", true) :
					WebRequest.GetSystemWebProxy();
			}
			catch (Exception) {
				proxy = WebRequest.GetSystemWebProxy();
			}
			
			proxy.Credentials = CredentialCache.DefaultNetworkCredentials;	
			client.Proxy = proxy;
			
//			client.Proxy = WebRequest.DefaultWebProxy;
//			client.Proxy.Credentials = CredentialCache.DefaultCredentials;
			
			IRestRequest request = new RestRequest(Method.HEAD);
			IRestResponse response = client.Execute(request);
			
			return response.StatusCode;
		}
		
		static void RequestOiCredentials() {
			UI.AuthForm auth = new UI.AuthForm("OI");
			auth.StartPosition = FormStartPosition.CenterParent;
			auth.ShowDialog();
			
			if(!string.IsNullOrEmpty(auth.Username)) {
				DialogResult ans = new DialogResult();
				if(auth.Username != OIUsername) {
					OIUsername = auth.Username;
					ans = FlexibleMessageBox.Show("Stored OI Credentials: " +  OIUsername + Environment.NewLine + Environment.NewLine + "You entered different credentials, do you want to overwrite the stored information?","OI credentials",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
					if(ans == DialogResult.Yes)
						Settings.SettingsFile.OIUsername = OIUsername;
				}
				if(auth.Password != OIPassword) {
					OIPassword = auth.Password;
					if(ans == DialogResult.Yes)
						Settings.SettingsFile.OIPassword = OIPassword;
				}
			}
		}
		
		public static void InitiateOiConnection(bool OldOiPortal = false) {
			// Instantiate RestSharp client
			
			client = new RestClient();
			
			if(CheckAvailability(OldOiPortal) == HttpStatusCode.OK) { // Check server availability
				// Check Login status
				bool loginState = OldOiPortal ? LoggedOnOldOiPortal : LoggedOn;
				if(!loginState) {
					Logon(OldOiPortal);
				}
				else {
					// If already logged in, confirm on server
					client.BaseUrl = OldOiPortal ? OldOiPortalUri : OiPortalUri;
					client.CookieContainer = OICookieContainer;
					IRestRequest request = new RestRequest(Method.POST);
					request.AddHeader("Content-Type", "application/html");
					request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
					IRestResponse response = client.Execute(request);
					
					if((!OldOiPortal && response.Content.Contains("Not logged in")) || (OldOiPortal && response.ContentLength != 167 && !response.Content.Contains("<meta http-equiv=" + '"' + "refresh" + '"' + " content=" + '"' + "0;URL='http://operationalintelligence.vf-uk.corp.vodafone.com/'" + '"' + " />"))) // Login if server kicked user out
						Logon(OldOiPortal);
				}
			}
		}
		
		/// <summary>
		/// Requests data from OI API function
		/// </summary>
		/// <param name="dataToRequest">"sites", "cells"</param>
		public static string requestApiOutput(string dataToRequest)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				IRestRequest request = new RestRequest(string.Format("/api/sitelopedia/export-{0}", dataToRequest), Method.GET);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"allsites", "allcells"</param>
		public static string requestPhpOutput(string phpFile)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"inc", "crq", "alarms", "ca"</param>
		/// <param name="site">Site number</param>
		public static string requestPhpOutput(string phpFile, string site)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
				request.AddParameter("siteinput", site);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"sitevisit"</param>
		/// <param name="site">Site number</param>
		/// <param name="days">Number of days for the Site Visits query</param>
		public static string requestPhpOutput(string phpFile, string site, int days)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				if(days == 0)
					days = 90;
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
				request.AddParameter("site", site);
				request.AddParameter("type", "a");
				request.AddParameter("days", days.ToString());
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"index"</param>
		/// <param name="site">Site number</param>
		/// <param name="cell">Cell number</param>
		public static string requestPhpOutput(string phpFile, string site, string cell)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.POST);
				request.AddParameter("easting", string.Empty);
				request.AddParameter("northing", string.Empty);
				request.AddParameter("site_single", site);
				request.AddParameter("cell_id", cell);
				request.AddParameter("lac_id", string.Empty);
				request.AddParameter("location", string.Empty);
				request.AddParameter("range", string.Empty);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"enterlock"</param>
		/// <param name="site">Site number</param>
		/// <param name="cellsList">List containing Cells to lock</param>
		/// <param name="Reference">Reference used for lockdown</param>
		/// <param name="comments">Lock comments</param>
		/// <param name="ManRef">Manual reference</param>
		public static string requestPhpOutput(string phpFile, string site, List<string> cellsList, string Reference, string comments, bool ManRef) {
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
				foreach(string cell in cellsList)
					request.AddParameter("checkbox" + cell, "on");
				
				if(ManRef) {
					request.AddParameter("Ref", string.Empty);
					request.AddParameter("ManRef", Reference);
				}
				else {
					request.AddParameter("Ref", Reference);
					request.AddParameter("ManRef", string.Empty);
				}
				request.AddParameter("Comment", comments);
				request.AddParameter("Site", site);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"cellslocked"</param>
		/// <param name="site">Site number</param>
		/// <param name="cellsList">List containing Cells to unlock</param>
		/// <param name="comments">Unlock comments</param>
		public static string requestPhpOutput(string phpFile, string site, List<string> cellsList, string comments) {
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.POST);
				if(cellsList != null) {
					foreach(string cell in cellsList)
						request.AddParameter("checkbox" + cell, "on");
					request.AddParameter("Comment", comments);
				}
				request.AddParameter("SiteNo", site);
				request.AddParameter("FromTime", string.Empty);
				request.AddParameter("ToTime", string.Empty);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
//			                        	I use the system.windows.forms.webbrowser and
//
		//httprequest / response in tandem to achieve what your looking for take the below:
//
		////navigate to the a page first with the webBrowser.
//
		//add the below to the documentcomplete event
//
		////
//
		////here I extract the cookies, but unfortunately the winforms webbrowser gives the cookies as one large string,
//
		////so you have to split it inorder to build a list / collection of system.net.Cookie
//
		//var c = webBrowser1.Document.Cookie.Split(";".ToCharArray());
		////cookie split
		//            var cookiecol = new CookieCollection();
//
		//            foreach (var cook in c)
		//            {
//
		//             // cookie split again for name value pairs
//
		//                var name = cook.Trim().Split("=".ToCharArray())[0];
		//                var value = cook.Trim().Split("=".ToCharArray())[1];
		//                cookiecol.Add(new Cookie(name, value));
		//            }
		//            //I create a request
		//            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(new Uri("http://***.com.au/game_results_download.php?date="+intialdate.ToString("dd/MM/yyyy")+"&game=1&num=550&state=qld"));
//
		//            req.CookieContainer = new CookieContainer();
//
		//           //attach the cookies
//
		//           req.CookieContainer.Add(new Uri("http://***.com.au"),cookiecol);
//
		//            var resp = (HttpWebResponse)req.GetResponse();
//
		//            // pull the data the string t - is actually a CSV file. in this case in my code base. but it could be anything.
		//            string t = new StreamReader(resp.GetResponseStream(), Encoding.Default).ReadToEnd();
//
		//            /*The only way to negate the save file dialog is by with send keys. which is crap.
//
		//   The example above gets the file stream, even though I don't actually know the default name of the file that has been served.
//
		//*/
//
		//I hope this helps.
//
		//The file will typically be requested as a GET this means alittle bit of manual Browser Debug is going to go a long way to finding the url, for the download, I'd suggest firefox with firebug installed.
		
//			                        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://operationalintelligence.vf-uk.corp.vodafone.com/site/allsites.php");
//
//			                        	HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//			                        	StreamReader input = new StreamReader(response.GetResponseStream());
//
//			                        	DataSet dsTest = new DataSet();
//			                        	dsTest.ReadXml(input);
//
//			                        	siteDetailsTable = Functions.GetDataTableFromCsv(all_sites,true);
//
//			                        	string search = all_sites;
//			                        	string text;
//
//			                        	//create the constructor with post type and few data
//			                        	appCore.Tools.MyWebRequest myRequest = new Tools.MyWebRequest("http://www.yourdomain.com","POST","a=value1&b=value2");
//			                        	//show the response string on the console screen.
//			                        	Console.WriteLine(myRequest.GetResponse());
//
//			                        	using (var client = new WebClient())
//			                        	{
//			                        		text = client.DownloadString("http://operationalintelligence.vf-uk.corp.vodafone.com/site/allsites.php");
//			                        	}
	}
}

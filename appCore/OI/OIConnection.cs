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
using appCore.Web.UI;

namespace appCore.OI
{
	/// <summary>
	/// Description of OiConnection.
	/// </summary>
	public static class OiConnection
	{
		static RestClient client;
		const string VfUkProxy = "http://vfukukproxy.internal.vodafone.com:8080/";
		const string VfPtProxy = "10.74.51.1:80";
		static IWebProxy proxy;
		public static IWebProxy Proxy {
			get {
				if(proxy == null) {
					try {
						proxy = Settings.CurrentUser.NetworkDomain == "internal.vodafone.com" ?
							new WebProxy(VfUkProxy, true) :
							WebRequest.GetSystemWebProxy();
					}
					catch (Exception) {
						proxy = WebRequest.GetSystemWebProxy();
					}
					
//					proxy = WebRequest.GetSystemWebProxy(); // Bypass UK Proxy
					
					proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
				}
				return proxy;
			}
			set { proxy = value; }
		}
		
		static string OIUsername = string.Empty;
		static string OIPassword = string.Empty;
		static Uri OiPortalUri = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/");
		static bool loggedOn;
		public static bool LoggedOn {
			get { return loggedOn; }
			private set {
				loggedOn = value;
				LoggedOnCheckTimeStamp = loggedOn ? DateTime.Now : new DateTime(1, 1, 1);
				
			}
		}
		static DateTime LoggedOnCheckTimeStamp;
		static TimeSpan LogonCheckLifeTime = new TimeSpan(0, 30, 0);
		
		public static bool Available {
			get {
				if(CheckAvailability() == HttpStatusCode.OK) {
					AvailableCheckTimeStamp = DateTime.Now;
					return true;
				}
				
				AvailableCheckTimeStamp = new DateTime(1, 1, 1);
				return false;
			}
			private set { }
		}
		static DateTime AvailableCheckTimeStamp;
		static TimeSpan AvailabilityCheckLifeTime = new TimeSpan(0, 30, 0);
		
		public static CookieContainer OICookieContainer {
			get;
			private set;
		}
		
		static void Logon()
		{
			// Load OI credentials from SettingsFile
			OIUsername = Settings.SettingsFile.OIUsername;
			OIPassword = Settings.SettingsFile.OIPassword;
			if(string.IsNullOrEmpty(OIUsername) || string.IsNullOrEmpty(OIPassword)) // Request credentials if empty on SettingsFile
				RequestOiCredentials();
		Retry:
			OICookieContainer = new CookieContainer();
			client.CookieContainer = OICookieContainer;
			client.BaseUrl = OiPortalUri;
			
			client.Proxy = Proxy;
			
			string restRequest = "/sso/index.php?url=%2F";
			IRestRequest request = new RestRequest(restRequest, Method.POST);
			request.AddParameter("username", OIUsername);
			request.AddParameter("password", OIPassword.DecryptText());
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			
			IRestResponse response = client.Execute(request);
			
			if(response.Content.Contains(@"<div class=""logged_in"">"))
				LoggedOn = true;
			else {
				LoggedOn = false;
				
				DialogResult res = FlexibleMessageBox.Show("Login failed with current OI credentials, do you want to change?\n\nIncorrect OI credentials prevents the use of this Tool's full features.","Login Failed",MessageBoxButtons.YesNo,MessageBoxIcon.Error);
				if(res == DialogResult.Yes) {
					RequestOiCredentials();
					goto Retry;
				}
			}
		}
		
		static void RevalidateLoginState() {
			client.BaseUrl = OiPortalUri;
			client.Proxy = Proxy;
			client.CookieContainer = OICookieContainer;
			IRestRequest request = new RestRequest(Method.POST);
			request.AddHeader("Content-Type", "application/html");
			request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
			IRestResponse response = client.Execute(request);
			
			if(response.Content.Contains("Not logged in")) // Login if server kicked user out
				Logon();
			else
				LoggedOn = true;
		}
		
		static HttpStatusCode CheckAvailability() {
			client.BaseUrl = OiPortalUri;
			
			client.Proxy = Proxy;
			
			IRestRequest request = new RestRequest(Method.HEAD);
			IRestResponse response = client.Execute(request);
			
			return response.StatusCode;
		}
		
		static void RequestOiCredentials() {
			AuthForm auth = new AuthForm("OI");
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
		
		public static bool InitiateOiConnection() {
			// Instantiate RestSharp client
			
			client = new RestClient();
			
			// Check server availability
			bool OiAvailable = (DateTime.Now - AvailableCheckTimeStamp) > AvailabilityCheckLifeTime ?
				Available :
				true;
			
			if(OiAvailable) {
				// Check Login state
				bool loginState = LoggedOn;
				if(!loginState)
					Logon();
				else {
					if((DateTime.Now - LoggedOnCheckTimeStamp) > LogonCheckLifeTime)
						RevalidateLoginState(); // If logon check lifetime expired, confirm on server
				}
			}
			return OiAvailable;
		}
		
		/// <summary>
		/// Requests the export CSV files from OI APIs
		/// </summary>
		/// <param name="dataToRequest">"sites", "cells"</param>
		public static string requestApiOutput(string dataToRequest)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				client.Proxy = Proxy;
				IRestRequest request = new RestRequest(string.Format("/api/sitelopedia/export-{0}", dataToRequest), Method.GET);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI APIs
		/// </summary>
		/// <param name="API">"cells", "incidents", "changes", "visits", "alarms", "availability", "access"</param>
		/// <param name="site">Site number</param>
		/// <param name="bearer">Bearer</param>
		public static string requestApiOutput(string API, string site, int bearer = 4)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				client.Proxy = Proxy;
				IRestRequest request = new RestRequest(string.Format("/api/sitelopedia/get-{0}", API), Method.POST);
				request.AddParameter("siteNumbers", site);
				request.AddParameter("range", string.Empty);
				request.AddParameter("bearer", bearer);
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Requests data from OI APIs
		/// </summary>
		/// <param name="API">"cells", "incidents", "changes", "visits", "alarms", "availability", "access"</param>
		/// <param name="site">Site number</param>
		/// <param name="bearer">Bearer</param>
		public static string requestApiOutput(string API, IEnumerable<string> sitesList, int bearer = 4)
		{
			InitiateOiConnection();
			if(LoggedOn) {
//				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://operationalintelligence.vf-uk.corp.vodafone.com" + string.Format("/api/sitelopedia/get-{0}", API));
//				webRequest.Method = "POST";
//				webRequest.CookieContainer = OICookieContainer;
//
//				string request = "siteNumbers=" + string.Join(",", sitesList) + "&range=&bearer=4";
//				byte[] dataStream =  System.Text.Encoding.ASCII.GetBytes(request);
//				webRequest.ContentLength = dataStream.Length;   //Count bytes to send
//				var os = webRequest.GetRequestStream();
//				os.Write(dataStream,0,dataStream.Length);
//				os.Close();
//				WebResponse webResponse = webRequest.GetResponse();
				
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				client.Proxy = Proxy;
				client.Timeout = 300000;
				IRestRequest request = new RestRequest(string.Format("/api/sitelopedia/get-{0}", API), Method.POST);
				request.RequestFormat = DataFormat.Xml;
				request.AddParameter("siteNumbers", string.Join(",", sitesList));
				request.AddParameter("range", string.Empty);
				request.AddParameter("bearer", bearer);
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
				client.Proxy = Proxy;
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
		public static string requestPhpOutput(string phpFile, IEnumerable<string> sitesList)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				client.Proxy = Proxy;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
				request.AddParameter("siteinput", string.Join(",", sitesList));
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
				client.Proxy = Proxy;
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
				client.Proxy = Proxy;
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
				client.Proxy = Proxy;
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
				client.Proxy = Proxy;
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
				client.Proxy = Proxy;
				IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.POST);
				if(cellsList != null) {
					foreach(string cell in cellsList)
						request.AddParameter("checkbox" + cell, "on");
					request.AddParameter("Comment", comments);
				}
				if(!string.IsNullOrEmpty(site)) {
					request.AddParameter("SiteNo", site);
					request.AddParameter("FromTime", string.Empty);
					request.AddParameter("ToTime", string.Empty);
				}
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

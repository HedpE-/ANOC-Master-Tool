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
using System.Threading.Tasks;
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
                var task = new Task<bool>(() =>
                {
                    if (CheckAvailability() == HttpStatusCode.OK)
                    {
                        AvailableCheckTimeStamp = DateTime.Now;
                        return true;
                    }
                    return false;
                });
                task.Start();
                bool OiAvailability = task.Wait(TimeSpan.FromMilliseconds(7500)) && task.Result;
                if(!OiAvailability)
                    OiAvailability = task.Wait(TimeSpan.FromMilliseconds(7500)) && task.Result;

                try
                {
                    task.Dispose();
                }
                catch(Exception e)
                {
                    var t = e.Message;
                }
				
				AvailableCheckTimeStamp = new DateTime(1, 1, 1);

				return OiAvailability;
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
		
		static HttpStatusCode CheckAvailability()
        {
            if (client == null)
                client = new RestClient();

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
		/// <param name="API">"sites-information", "labels-html", "cells-html", "summary", "access", "temperature", "availability-Html", "incidents", "changes", "visits", "alarms", "touchpoint", "zenpm"</param>
		/// <param name="site">Site number</param>
		/// <param name="bearer">Bearer</param>
		public static string requestApiOutput(string API, string site, int bearer = 4)
		{
			List<string> siteList = new List<string>();
			siteList.Add(site);
			return requestApiOutput(API, siteList, bearer);
        }

        /// <summary>
        /// Requests bulk data from OI APIs
        /// </summary>
        /// <param name="API">"sites-information", "labels-html", "cells-html", "summary", "access", "temperature", "availability-Html", "incidents", "changes", "visits", "alarms", "touchpoint", "zenpm"</param>
        /// <param name="site">Site number</param>
        /// <param name="bearer">Bearer</param>
        public static string requestApiOutput(string API, IEnumerable<string> sitesList, int bearer = 4)
		{
			InitiateOiConnection();
			if(LoggedOn) {
				client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
				client.CookieContainer = OICookieContainer;
				client.Proxy = Proxy;
				client.Timeout = 300000;
				IRestRequest request = new RestRequest(string.Format("/api/sitelopedia/get-{0}", API), Method.POST);
//				request.RequestFormat = DataFormat.Xml;
				request.AddParameter("siteNumbers", string.Join(",", sitesList));
				request.AddParameter("range", string.Empty);
				request.AddParameter("bearer", bearer);
				IRestResponse response = client.Execute(request);

				return response.Content;
			}
			return string.Empty;
		}

        #region async methods

        /// <summary>
        /// Requests data from OI APIs Asynchronously
        /// </summary>
        /// <param name="API">"sites-information", "labels-html", "cells-html", "summary", "access", "temperature", "availability-Html", "incidents", "changes", "visits", "alarms", "touchpoint", "zenpm"</param>
        /// <param name="site">Site number</param>
        /// <param name="bearer">Bearer</param>
        public async static Task<string> requestApiOutputAsync(string API, string site, int bearer = 4)
        {
            List<string> siteList = new List<string>();
            siteList.Add(site);
            return await requestApiOutputAsync(API, siteList, bearer);
        }

        /// <summary>
        /// Requests bulk data from OI APIs
        /// </summary>
        /// <param name="API">"sites-information", "labels-html", "cells-html", "summary", "access", "temperature", "availability-Html", "incidents", "changes", "visits", "alarms", "touchpoint", "zenpm"</param>
        /// <param name="site">Site number</param>
        /// <param name="bearer">Bearer</param>
        public async static Task<string> requestApiOutputAsync(string API, IEnumerable<string> sitesList, int bearer = 4)
        {
            InitiateOiConnection();
            if (LoggedOn)
            {
                client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
                client.CookieContainer = OICookieContainer;
                client.Proxy = Proxy;
                client.Timeout = 300000;
                IRestRequest request = new RestRequest(string.Format("/api/sitelopedia/get-{0}", API), Method.POST);
                //				request.RequestFormat = DataFormat.Xml;
                request.AddParameter("siteNumbers", string.Join(",", sitesList));
                request.AddParameter("range", string.Empty);
                request.AddParameter("bearer", bearer);
                IRestResponse response = await Task.Run(() => client.Execute(request));

                return response.Content;
            }
            return string.Empty;
        }

        /// <summary>
        /// Requests index.php whole contents for given Site
        /// </summary>
        /// <param name="site">Site ID</param>
        public async static Task<string> requestPhpOutputAsync(string site)
        {
            InitiateOiConnection();
            if (LoggedOn)
            {
                client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
                client.CookieContainer = OICookieContainer;
                client.Proxy = Proxy;
                client.Timeout = 300000;
                IRestRequest request = new RestRequest("/site/index.php", Method.POST);
                request.AddParameter("site_single", site);
                request.AddParameter("easting", string.Empty);
                request.AddParameter("northing", string.Empty);
                request.AddParameter("cell_id", string.Empty);
                request.AddParameter("lac_id", string.Empty);
                request.AddParameter("location", string.Empty);
                request.AddParameter("range", string.Empty);
                request.AddHeader("Content-Type", "application/html");
                request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
                IRestResponse response = await Task.Run(() => client.Execute(request));

                return response.Content;
            }
            return string.Empty;
        }

        #endregion

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
		/// <param name="phpFile">"inc", "crq", "visits", "alarms", "ca"</param>
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
        public static string requestPhpOutput(string phpFile, string site, List<string> cellsList, string Reference, string comments, bool ManRef)
        {
            InitiateOiConnection();
            if (LoggedOn)
            {
                client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
                client.CookieContainer = OICookieContainer;
                client.Proxy = Proxy;
                IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
                foreach (string cell in cellsList)
                    request.AddParameter("checkbox" + cell, "on");

                if (ManRef)
                {
                    request.AddParameter("Ref", string.Empty);
                    request.AddParameter("ManRef", Reference);
                }
                else
                {
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
        /// <param name="phpFile">"enterlock"</param>
        /// <param name="site">Site number</param>
        /// <param name="cellsList">List containing Cells to lock</param>
        /// <param name="Reference">Reference used for lockdown</param>
        /// <param name="comments">Lock comments</param>
        /// <param name="ManRef">Manual reference</param>
        public async static Task<string> requestPhpOutputAsync(string phpFile, string site, List<string> cellsList, string Reference, string comments, bool ManRef)
        {
            InitiateOiConnection();
            if (LoggedOn)
            {
                client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
                client.CookieContainer = OICookieContainer;
                client.Proxy = Proxy;
                IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
                foreach (string cell in cellsList)
                    request.AddParameter("checkbox" + cell, "on");

                if (ManRef)
                {
                    request.AddParameter("Ref", string.Empty);
                    request.AddParameter("ManRef", Reference);
                }
                else
                {
                    request.AddParameter("Ref", Reference);
                    request.AddParameter("ManRef", string.Empty);
                }
                request.AddParameter("Comment", comments);
                request.AddParameter("Site", site);
                request.AddHeader("Content-Type", "application/html");
                request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
                IRestResponse response = await Task.Run(() => client.Execute(request));

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
        public static string requestPhpOutput(string phpFile, string site, List<string> cellsList, string comments)
        {
            InitiateOiConnection();
            if (LoggedOn)
            {
                client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
                client.CookieContainer = OICookieContainer;
                client.Proxy = Proxy;
                IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.POST);
                if (cellsList != null)
                {
                    foreach (string cell in cellsList)
                        request.AddParameter("checkbox" + cell, "on");
                    request.AddParameter("Comment", comments);
                }
                if (!string.IsNullOrEmpty(site))
                {
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

        /// <summary>
        /// Requests data from OI PHP files
        /// </summary>
        /// <param name="phpFile">"cellslocked"</param>
        /// <param name="site">Site number</param>
        /// <param name="cellsList">List containing Cells to unlock</param>
        /// <param name="comments">Unlock comments</param>
        public async static Task<string> requestPhpOutputAsync(string phpFile, string site, List<string> cellsList, string comments)
        {
            InitiateOiConnection();
            if (LoggedOn)
            {
                client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
                client.CookieContainer = OICookieContainer;
                client.Proxy = Proxy;
                IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.POST);
                if (cellsList != null)
                {
                    foreach (string cell in cellsList)
                        request.AddParameter("checkbox" + cell, "on");
                    request.AddParameter("Comment", comments);
                }
                if (!string.IsNullOrEmpty(site))
                {
                    request.AddParameter("SiteNo", site);
                    request.AddParameter("FromTime", string.Empty);
                    request.AddParameter("ToTime", string.Empty);
                }
                request.AddHeader("Content-Type", "application/html");
                request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
                IRestResponse response = await Task.Run(() => client.Execute(request));

                return response.Content;
            }
            return string.Empty;
        }
    }
}

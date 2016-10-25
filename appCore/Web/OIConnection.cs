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
using appCore.Web;

namespace appCore.Web
{
	/// <summary>
	/// Description of OIConnection.
	/// </summary>
	public class OIConnection
	{
		public static OIConnection Connection = null;
		
		RestClient client;
		List<Parameter> requiredHeaders = new List<Parameter>();
		CookieContainer cookies;
		static string OIUsername = string.Empty;
		static string OIPassword = string.Empty;
		public string LoggedOnUsername;
		public DateTime OICookieExpireDate;
		public CookieContainer OICookieContainer {
			get {
				return cookies;
			}
			set {
				cookies = value;
			}
		}
		
		public Cookie OICookie {
			get {
				if(OICookieContainer.Count > 0) {
					List<Cookie> cList = OICookieContainer.ToList();
					for(int i=1;i <= cList.Count;i++) {
						Cookie c = cList[i];
						if(c.Name.Contains("PHPSESSID")) {
							return c;
						}
					}
				}
				return null;
			}
		}
		
		public static HttpStatusCode EstablishConnection()
		{
			OIUsername = Settings.SettingsFile.OIUsername;
			OIPassword = Settings.SettingsFile.OIPassword;
			if(Connection == null)
				Connection = new OIConnection();
			if(string.IsNullOrEmpty(OIUsername)) {
				UI.AuthForm auth = new UI.AuthForm("OI");
				auth.StartPosition = FormStartPosition.CenterParent;
				auth.ShowDialog();
				
				if(!string.IsNullOrEmpty(auth.Username)) {
					DialogResult ans = new DialogResult();
					if(auth.Username != OIUsername) {
						OIUsername = auth.Username;
						ans = MessageBox.Show("Stored OI Credentials: " +  OIUsername + Environment.NewLine + Environment.NewLine + "You entered different credentials, do you want to overwrite the stored information?","OI credentials",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
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
			HttpStatusCode status = Connection.CheckAvailability();
			return status;
		}
		
		HttpStatusCode CheckAvailability() {
			if(client == null)
				client = new RestClient();
			
			client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/sso/index.php?url=%2F");
			client.Proxy = WebRequest.DefaultWebProxy;
			client.Proxy.Credentials = CredentialCache.DefaultCredentials;
			
			IRestRequest request = new RestRequest(Method.HEAD);
			IRestResponse response = client.Execute(request);
			if(response.StatusCode == HttpStatusCode.OK)
				Connection = this;
			return response.StatusCode;
		}
		
		public HttpStatusCode Logon()
		{
			LoggedOnUsername = string.Empty;
			
			OICookieContainer = new CookieContainer();
			
			if(client == null)
				client = new RestClient();
			
			client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
			client.Proxy = WebRequest.DefaultWebProxy;
			client.Proxy.Credentials = CredentialCache.DefaultCredentials;
			client.CookieContainer = OICookieContainer;
			IRestRequest request = new RestRequest("/sso/index.php?url=%2F", Method.POST);
			request.AddParameter("username", OIUsername);
			request.AddParameter("password", Toolbox.Tools.EncryptDecryptText("Dec", OIPassword));
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			
			IRestResponse response = client.Execute(request);
			
			requiredHeaders = (List<Parameter>)response.Headers;
			
			request = new RestRequest("/site/index.php", Method.GET);
			request.AddHeader("Content-Type", "application/html");
			foreach (var header in requiredHeaders) {
				if(header.Name == "Pragma" || header.Name == "Cache-Control" || header.Name == "Vary" || header.Name == "Expires" || header.Name == "Date")
					request.AddHeader(header.Name, header.Value.ToString());
			}
			response = client.Execute(request);
			
			if((int)response.StatusCode == 200) {
				if(response.Content.Contains(@"<div class=""logged_in"">"))
					LoggedOnUsername = OIUsername;
			}
			
			return response.StatusCode;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"allsites", "allcells"</param>
		public string requestPhpOutput(string phpFile)
		{
			client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
			IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
//			request.AddParameter("username", OIUsername, ParameterType.UrlSegment);
//			request.AddParameter("password", appCore.Toolbox.Tools.EncryptDecryptText("Dec", OIPassword), ParameterType.UrlSegment);
			request.AddHeader("Content-Type", "application/html");
			request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
//			foreach (var header in requiredHeaders) {
//				if(header.Name == "Pragma" || header.Name == "Cache-Control" || header.Name == "Vary" || header.Name == "Expires" || header.Name == "Date")
//					request.AddHeader(header.Name, header.Value.ToString());
//			}
			IRestResponse response = client.Execute(request);
			
			return response.Content;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"inc", "crq", "alarms"</param>
		/// <param name="site">Site number</param>
		public string requestPhpOutput(string phpFile, string site)
		{
			client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
			IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
			request.AddParameter("siteinput", site);
			request.AddHeader("Content-Type", "application/html");
			request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
			IRestResponse response = client.Execute(request);
			
			return response.Content;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"sitevisit"</param>
		/// <param name="site">Site number</param>
		/// <param name="days">Number of days for the Site Visits query</param>
		public string requestPhpOutput(string phpFile, string site, int days)
		{
			if(days == 0)
				days = 90;
			client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
			IRestRequest request = new RestRequest(string.Format("/site/{0}.php", phpFile), Method.GET);
			request.AddParameter("site", site);
			request.AddParameter("type", "a");
			request.AddParameter("days", days.ToString());
			request.AddHeader("Content-Type", "application/html");
			request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
			IRestResponse response = client.Execute(request);
			
			return response.Content;
		}
		
		/// <summary>
		/// Requests data from OI PHP files
		/// </summary>
		/// <param name="phpFile">"index"</param>
		/// <param name="site">Site number</param>
		/// <param name="cell">Cell number</param>
		public string requestPhpOutput(string phpFile, string site, string cell)
		{
			client.BaseUrl = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com");
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

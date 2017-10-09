/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 22-09-2017
 * Time: 10:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Threading.Tasks;
using RestSharp;
using appCore.UI;
using appCore.Web.UI;

namespace appCore.Web
{
    /// <summary>
    /// Description of ConfluenceConnection.
    /// </summary>
    public static class ConfluenceConnection
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
		
		static string Username = string.Empty;
		static string Password = string.Empty;
		static Uri PortalUri = new Uri("https://confluence.sp.vodafone.com");

		public static bool LoggedOn
        {
			get {
                if (client == null)
                    client = new RestClient();

                client.BaseUrl = PortalUri;
                client.CookieContainer = cookieContainer;
                client.Proxy = Proxy;
                
                string restRequest = "/dologin.action";

                IRestRequest request = new RestRequest(restRequest, Method.POST);

                IRestResponse response = client.Execute(request);

                return response.Headers.Where(h => h.Name == "X_AUSERNAME").Any();
            }
		}
		
		public static bool Available {
			get {
                var task = new Task<bool>(() =>
                {
                    if (CheckAvailability() == HttpStatusCode.OK)
                        return true;
                    return false;
                });
                task.Start();
                bool Availability = task.Wait(TimeSpan.FromMilliseconds(7500)) && task.Result;
                if(!Availability)
                    Availability = task.Wait(TimeSpan.FromMilliseconds(7500)) && task.Result;

                try
                {
                    task.Dispose();
                }
                catch(Exception e)
                {
                    var t = e.Message;
                }

				return Availability;
			}
			private set { }
		}
		
		public static CookieContainer cookieContainer {
			get;
			private set;
		}
		
		static void Logon()
		{
            //Username = Settings.CurrentUser.VodafoneCountry.Contains("Portugal") ? Settings.CurrentUser.Email : string.Empty;
            if(string.IsNullOrEmpty(Password))
            {
                try
                {
                    RequestConfluenceCredentials();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            Retry:
			cookieContainer = new CookieContainer();
			client.CookieContainer = cookieContainer;
			client.BaseUrl = PortalUri;
			
			client.Proxy = Proxy;
			
			string restRequest = "/dologin.action";

            IRestRequest request = new RestRequest(restRequest, Method.POST);
			request.AddParameter("os_username", Username);
			request.AddParameter("os_password", Password);
            request.AddParameter("login", "Log in");
            request.AddParameter("os_destination", "");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			
			IRestResponse response = client.Execute(request);

            if (response.Headers.Any(h => h.Value.ToString() == "AUTHENTICATED_FAILED"))
            {
				DialogResult res = FlexibleMessageBox.Show("Login failed, do you want to try again?", "Login Failed",MessageBoxButtons.YesNo,MessageBoxIcon.Error);
				if(res == DialogResult.Yes)
                {
                    try
                    {
                        RequestConfluenceCredentials();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
					goto Retry;
				}
			}
		}
		
		static HttpStatusCode CheckAvailability()
        {
            if (client == null)
                client = new RestClient();

			client.BaseUrl = PortalUri;
			
			client.Proxy = Proxy;
			
			IRestRequest request = new RestRequest(Method.HEAD);
			IRestResponse response = client.Execute(request);
			
			return response.StatusCode;
		}
		
		static void RequestConfluenceCredentials()
        {
			AuthForm auth = new AuthForm("Confluence");
            //auth.Username = Username;
			auth.StartPosition = FormStartPosition.CenterParent;

            DialogResult ans = auth.ShowDialog();

            if (ans == DialogResult.OK)
            {
                Username = auth.Username;
                Password = auth.Password;
            }
            else
                throw new Exception("Login cancelled");
		}
		
		public static void InitiateConnection() {
			// Instantiate RestSharp client
			
			client = new RestClient();

            // Check server availability
            bool available = Available;
			if(available)
            {
				// Check Login state
				if(!LoggedOn)
                {
                    try
                    {
                        Logon();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            else
                throw new Exception("Confluence unavailable");
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static string requestHtmlSource(string dataToRequest)
		{
            try
            {
                InitiateConnection();
            }
            catch (Exception e)
            {
                throw e;
            }
            if (LoggedOn) {
				client.BaseUrl = PortalUri;
				client.CookieContainer = cookieContainer;
				client.Proxy = Proxy;
				IRestRequest request = new RestRequest(dataToRequest, Method.GET);
				request.AddHeader("Content-Type", "application/html");
				request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
				IRestResponse response = client.Execute(request);
				
				return response.Content;
			}
			return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] requestImage(string dataToRequest)
        {
            try
            {
                InitiateConnection();
            }
            catch (Exception e)
            {
                throw e;
            }
            if (LoggedOn)
            {
                client.BaseUrl = PortalUri;
                client.CookieContainer = cookieContainer;
                client.Proxy = Proxy;
                IRestRequest request = new RestRequest(dataToRequest, Method.GET);
                request.AddHeader("Content-Type", "application/html");
                request.AddHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.3; Tablet PC 2.0)");
                IRestResponse response = client.Execute(request);

                return response.RawBytes;

                //using (var ms = new System.IO.MemoryStream(imageBytes))
                //{
                //    return System.Drawing.Image.FromStream(ms);
                //}
            }
            return null;
        }
    }
}

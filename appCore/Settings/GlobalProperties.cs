/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-08-2016
 * Time: 13:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.IO;

namespace appCore.Settings
{
	/// <summary>
	/// Description of GlobalProperties.
	/// </summary>
	public static class GlobalProperties
	{
		public static DateTime ApplicationStartTime;
		public static TimeSpan UpTime { get { return DateTime.Now - ApplicationStartTime; } }
		
		public static CultureInfo culture = new CultureInfo("pt-PT");
		public static DateTime dt = DateTime.Parse(DateTime.Now.ToString(), culture);
		public static DirectoryInfo ShareRootDir = new DirectoryInfo(@"\\vf-pt\fs\ANOC-UK\ANOC-UK 1st LINE\1. RAN 1st LINE\ANOC Master Tool");
        public static DirectoryInfo ShareDataDir = new DirectoryInfo(@"\\vf-pt\fs\ANOC-UK\ANOC-UK 1st LINE\1. RAN 1st LINE\AMT Data");
        public static DirectoryInfo FallbackRootDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ANOC Master Tool");

        public static DirectoryInfo AppDataRootDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ANOC Master Tool");
        public static DirectoryInfo TempFolder = new DirectoryInfo(AppDataRootDir.FullName + @"\temp");
        public static DirectoryInfo AssembliesDir = new DirectoryInfo(AppDataRootDir + @"\dll");
		
		public static readonly DirectoryInfo ShiftsDefaultLocation = new DirectoryInfo(ShareRootDir.Parent.FullName + @"\Shifts");
		public static readonly DirectoryInfo DBFilesDefaultLocation = new DirectoryInfo(ShareDataDir.FullName + @"\ANOC Master Tool");
        public static readonly DirectoryInfo ExternalResourceFilesLocation = new DirectoryInfo(DBFilesDefaultLocation.FullName + @"\resources");
		
		public static string OfficePath = string.Empty;
		
		static FileVersionInfo _assemblyFileVersionInfo;
		public static FileVersionInfo AssemblyFileVersionInfo {
			get {
				if(_assemblyFileVersionInfo == null) {
					Assembly assembly = Assembly.GetExecutingAssembly();
					_assemblyFileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
				}
				return _assemblyFileVersionInfo;
			}
			private set {}
		}

        static ShareAccess _shareHostAccess;
        public static ShareAccess shareHostAccess
        {
            get
            {
                return _shareHostAccess;
            }
            set
            {
                _shareHostAccess = value;
                //if (UserFolder.FullName != null)
                    MainForm.trayIcon.toggleShareAccess();
            }
        }

        static bool _shareAccess = true;
		public static bool shareAccess {
			get {
				return _shareAccess;
			}
			set {
				_shareAccess = value;
				//if(UserFolder.FullName != null)
					MainForm.trayIcon.toggleShareAccess();
			}
		}
		static bool _siteFinder_mainswitch = true;
		public static bool siteFinder_mainswitch {
			get {
				return _siteFinder_mainswitch;
			}
			set {
				if(_siteFinder_mainswitch == value)
					return;
				_siteFinder_mainswitch = value;
				if (!_siteFinder_mainswitch) {
					MainForm.SiteDetailsPictureBox.Visible = false;
					MainForm.TroubleshootUI.siteFinderSwitch("off");
					MainForm.FailedCRQUI.siteFinderSwitch("off");
					MainForm.UpdateUI.siteFinderSwitch("off");
				}
				else {
					MainForm.SiteDetailsPictureBox.Visible = true;
					MainForm.TroubleshootUI.siteFinderSwitch("on");
					MainForm.FailedCRQUI.siteFinderSwitch("on");
					MainForm.UpdateUI.siteFinderSwitch("on");
				}
			}
        }

        public static bool WeatherServiceEnabled { get; set; } = true;

        public static void DeployExternalAssemblies()
        {
            if (!AppDataRootDir.Exists)
            {
                AppDataRootDir.Create();
                AppDataRootDir = new DirectoryInfo(AppDataRootDir.FullName);
            }
            if(!AssembliesDir.Exists)
            {
                AssembliesDir.Create();
                AssembliesDir = new DirectoryInfo(AssembliesDir.FullName);
            }
            List<string> dlls = new List<string>();
            dlls.AddRange(new string[]{
                "RestSharp.dll",
                "Outlook.dll",
                "ICSharpCode.SharpZipLib.dll",
                "BMC.ARSystem.dll",
                "GMap.NET.WindowsForms.dll",
                "GMap.NET.Core.dll",
                "FileHelpers.dll",
                "Newtonsoft.Json.dll",
                //"GeoUk.dll",
                //"Svg2.3.0.dll,

                "EntityFramework.dll",
                "EntityFramework.SqlServer.dll",

                "SQLite.Interop.dll",

                "System.Data.SQLite.dll",
                "System.Data.SQLite.EF6.dll",
                "System.Data.SQLite.Linq.dll",
            });

            ExtractEmbeddedResource(AssembliesDir.FullName, "appCore.Assemblies", dlls);

            AppDomain.CurrentDomain.AppendPrivatePath(AssembliesDir.FullName);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
        }

        private static void ExtractEmbeddedResource(string outputDir, string resourceLocation, List<string> files)
        {
            foreach (string file in files)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
                {
                    using (FileStream fileStream = new FileStream(Path.Combine(outputDir, file), FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }

        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly MyAssembly, objExecutingAssemblies;
            string strTempAssmbPath = "";
            objExecutingAssemblies = Assembly.GetExecutingAssembly();
            AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();
            foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
            {
                if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
                {
                    strTempAssmbPath = AssembliesDir.FullName + "\\" +
                        args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
                    MyAssembly = Assembly.LoadFrom(strTempAssmbPath);
                    return MyAssembly;
                }
            }
            return null;
        }

        public static void CheckShareAccess()
        {
            //IPHostEntry host = Dns.GetHostEntry(ShareRootDir.Root.FullName.Split('\\')[2]);

            shareHostAccess = new ShareAccess();

            if (!shareHostAccess.Exists || !shareHostAccess.CanRead)
            //{
                MainForm.trayIcon.showBalloon("Network share access denied", "Access to the network share was denied!"); //Your settings file will be created on the following path:" + Environment.NewLine + Environment.NewLine + Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ANOC Master Tool\\UserSettings\\");
            //  shareAccess = false;
            //}
            //else
            //{
            //    if(!shareHostAccess.CanWrite)
            //        MainForm.trayIcon.showBalloon("Network share access without Write permissions", "Access to the network share was deteced without Write permissions");
            //}
        }

		public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
		{
			bool networkAccess = false;
			var networkAccessCheck = new Thread(() => {
			                                    	try {
			                                    		using(File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)) { }
			                                    		networkAccess = true;
			                                    	}
			                                    	catch {
			                                    		if(throwIfFails)
			                                    			throw;
			                                    	}
			                                    });
			networkAccessCheck.Name = "networkAccessCheck";
			networkAccessCheck.Start();
			if (!networkAccessCheck.Join(TimeSpan.FromSeconds(20))) {
				try {
					networkAccessCheck.Abort();
				}
				catch(ThreadAbortException) { }
				return false;
			}
			return networkAccess;
		}
		
		public static void resolveOfficePath() {
			Thread thread = new Thread(() => {
			                           	Type officeType = Type.GetTypeFromProgID("Excel.Application");
			                           	dynamic xlApp = Activator.CreateInstance(officeType);
			                           	xlApp.Visible = false;
			                           	OfficePath = xlApp.Path;
			                           	xlApp.Quit();
			                           });
			thread.Name = "resolveOfficePath";
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
	}

    public class ShareAccess
    {
        public bool Exists { get; private set; }
        public bool CanRead { get; private set; }
        public bool CanWrite { get; private set; }
        public bool FullAccess
        {
            get
            {
                return CanRead && CanWrite;
            }
        }

        public ShareAccess()
        {
            DirectoryInfo di = new DirectoryInfo(GlobalProperties.ShareRootDir.FullName);
            if (di.Exists)
            {
                Exists = true;
                try
                {
                    var acl = di.GetAccessControl();
                    if (acl != null)
                    {
                        var accessRules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));
                        if (accessRules != null)
                        {
                            foreach (FileSystemAccessRule rule in accessRules)
                            {
                                if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                                {
                                    if (rule.AccessControlType == AccessControlType.Allow)
                                        CanRead = true;
                                }

                                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                                {
                                    if (rule.AccessControlType == AccessControlType.Allow)
                                        CanWrite = true;
                                }

                                if (CanRead && CanWrite)
                                    break;
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException uae)
                {
                    if (uae.Message.Contains("read-only"))
                    {
                        // seems like it is just read-only
                        CanRead = true;
                        //CanWrite = false;
                    }
                }
            }


            //Host = host;

            //IPAddress address = host.AddressList.FirstOrDefault(i => !i.ToString().StartsWith("4"));

            //string returnMessage = string.Empty;
            //try
            //{
            //    //set the ping options, TTL 128
            //    PingOptions pingOptions = new PingOptions(128, true);
            //    //create a new ping instance
            //    Ping ping = new Ping();
            //    //32 byte buffer (create empty)
            //    byte[] buffer = new byte[32];

            //    //send the ping to the host and record the returned data.
            //    //The Send() method expects 4 items:
            //    //1) The IPAddress we are pinging
            //    //2) The timeout value
            //    //3) A buffer (our byte array)
            //    //4) PingOptions
            //    PingReply pingReply = ping.Send(address, 1000, buffer, pingOptions);

            //    //make sure we dont have a null reply
            //    if (!(pingReply == null))
            //    {
            //        switch (pingReply.Status)
            //        {
            //            case IPStatus.Success:
            //                //returnMessage = string.Format("Reply from {0}: bytes={1} time={2}ms TTL={3}", pingReply.Address, pingReply.Buffer.Length, pingReply.RoundtripTime, pingReply.Options.Ttl);
            //                CanRead = true;
            //                break;
            //            //case IPStatus.TimedOut:
            //            //returnMessage = "Connection has timed out...";
            //            //break;
            //            default:
            //                //returnMessage = string.Format("Ping failed: {0}", pingReply.Status.ToString());
            //                CanRead = CanWrite = false;
            //                return;
            //        }
            //    }
            //    else
            //    {
            //        //returnMessage = "Connection failed for an unknown reason...";
            //        CanRead = CanWrite = false;
            //        return;
            //    }
            //}
            //catch (PingException ex)
            //{
            //    //returnMessage = string.Format("Connection Error: {0}", ex.Message);
            //    CanRead = CanWrite = false;
            //    return;
            //}
            //catch (System.Net.Sockets.SocketException ex)
            //{
            //    //returnMessage = string.Format("Connection Error: {0}", ex.Message);
            //    CanRead = CanWrite = false;
            //    return;
            //}

            //CanWrite = GlobalProperties.IsDirectoryWritable(GlobalProperties.ShareRootDir.FullName);
        }
    }
}

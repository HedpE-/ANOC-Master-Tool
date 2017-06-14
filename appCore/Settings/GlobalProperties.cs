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
using System.Reflection;
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
		public static DirectoryInfo FallbackRootDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ANOC Master Tool");

        public static DirectoryInfo AppDataRootDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ANOC Master Tool");
        public static DirectoryInfo AssembliesDir = new DirectoryInfo(AppDataRootDir + @"\dll");
		
		public static readonly DirectoryInfo ShiftsDefaultLocation = new DirectoryInfo(GlobalProperties.ShareRootDir.Parent.FullName + @"\Shifts");
		public static readonly DirectoryInfo DBFilesDefaultLocation = new DirectoryInfo(GlobalProperties.ShareRootDir.FullName + @"\ANOC Master Tool");
        public static readonly DirectoryInfo WeatherPicturesLocation = new DirectoryInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\resources");
		
		public static string OfficePath = string.Empty;
		
		static FileVersionInfo _assemblyFileVersionInfo;
		public static FileVersionInfo AssemblyFileVersionInfo {
			get {
				if(_assemblyFileVersionInfo == null) {
					System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
					_assemblyFileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
				}
				return _assemblyFileVersionInfo;
			}
			private set {}
		}
		
		static bool _shareAccess = true;
		public static bool shareAccess {
			get {
				return _shareAccess;
			}
			set {
				_shareAccess = value;
				if(UserFolder.FullName != null)
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
                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
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

        public static void CheckShareAccess() {
			if(!IsDirectoryWritable(ShareRootDir.FullName)) {
				MainForm.trayIcon.showBalloon("Network share access denied","Access to the network share was denied! Your settings file will be created on the following path:" + Environment.NewLine + Environment.NewLine + Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ANOC Master Tool\\UserSettings\\");
				shareAccess = false;
			}
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
}

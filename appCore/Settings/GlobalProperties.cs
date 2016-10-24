/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-08-2016
 * Time: 13:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Reflection;

namespace appCore.Settings
{
	/// <summary>
	/// Description of GlobalProperties.
	/// </summary>
	public static class GlobalProperties
	{
		public static CultureInfo culture = new CultureInfo("pt-PT");
		public static DateTime dt = DateTime.Parse(DateTime.Now.ToString(), culture);
		public static DirectoryInfo ShareRootDir = new DirectoryInfo(@"\\vf-pt\fs\ANOC-UK\ANOC-UK 1st LINE\1. RAN 1st LINE\ANOC Master Tool");
		public static DirectoryInfo FallbackRootDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ANOC Master Tool");
		
		public static readonly DirectoryInfo ShiftsDefaultLocation = new DirectoryInfo(GlobalProperties.ShareRootDir.Parent.FullName + @"\Shifts");
		public static readonly DirectoryInfo DBFilesDefaultLocation = new DirectoryInfo(GlobalProperties.ShareRootDir.FullName + @"\ANOC Master Tool");
		
		public static bool autoUpdateDbFiles = false;
		
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
//					MainForm.OutageUI.siteFinderSwitch("off");
				}
				else {
					MainForm.SiteDetailsPictureBox.Visible = true;
					MainForm.TroubleshootUI.siteFinderSwitch("on");
					MainForm.FailedCRQUI.siteFinderSwitch("on");
					MainForm.UpdateUI.siteFinderSwitch("on");
//					MainForm.OutageUI.siteFinderSwitch("on");
				}
			}
		}
		
		public static void CheckShareAccess() {
			if(!IsDirectoryWritable(ShareRootDir.FullName)) {
				MainForm.trayIcon.showBalloon("Network share access denied","Access to the network share denied or you're outside Vodafone's network, Settings file will be created on the following path:" + Environment.NewLine + Environment.NewLine + Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ANOC Master Tool\\UserSettings\\");
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
	}
}

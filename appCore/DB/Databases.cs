/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-08-2016
 * Time: 13:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Threading;
using System.IO;
using System.Timers;
using appCore.Settings;
using appCore.Web;

namespace appCore.DB
{
	/// <summary>
	/// Description of Databases.
	/// </summary>
	public static class Databases
	{
		static FileInfo _all_sites;
		public static FileInfo all_sites {
			get {
				_all_sites = new FileInfo(_all_sites.FullName);
				return _all_sites;
			}
			private set
			{
				if (File.Exists(all_sites.FullName) && File.Exists(value.FullName))
				{
//					bool sameFile = value.FullName == _all_sites.FullName && value.Length == _all_sites.Length;
					_all_sites = value;
//					if (!sameFile || siteDetailsTable == null)
//						siteDetailsTable = _all_sites.Exists ? Tools.GetDataTableFromCsv(_all_sites, true) : null;
				}
			}
		}
		
		static FileInfo _all_cells;
		public static FileInfo all_cells {
			get {
				_all_cells = new FileInfo(_all_cells.FullName);
				return _all_cells;
			}
			private set {
				if (File.Exists(all_sites.FullName) && File.Exists(value.FullName))
				{
//					bool sameFile = value.FullName == _all_cells.FullName && value.Length == _all_cells.Length;
					_all_cells = value;
//					if (!sameFile || cellDetailsTable == null)
//						cellDetailsTable = _all_cells.Exists ? Tools.GetDataTableFromCsv(_all_cells, true) : null;
				}
			}
		}
		
		public static ShiftsFile shiftsFile;
		
		public static DataTable siteDetailsTable = null;
		public static DataTable cellDetailsTable = null;
		
		static System.Timers.Timer AutoUpdateTimer = new System.Timers.Timer(60 * 60 * 1000); //one hour in milliseconds
		
		public static void ResetAutoUpdateTimer() {
			AutoUpdateTimer.Stop();
			AutoUpdateTimer.Start();
		}
		
		public static void Initialize() {
			if((CurrentUser.UserName == "GONCARJ3" || CurrentUser.UserName == "SANTOSS2") && GlobalProperties.autoUpdateDbFiles)
				UpdateSourceDBFiles();
			
			_all_sites = new FileInfo(UserFolder.FullName + @"\all_sites.csv");
			_all_cells = new FileInfo(UserFolder.FullName + @"\all_cells.csv");
		}
		
		public static void UpdateSourceDBFiles(bool onUserFolder = false) {
//			var thread = new Thread(() => {
//			string test = CurrentUser.GetUserDetails("NetworkDomain"); // TODO: Test Domain name on VF network
			FileInfo source_allsites = onUserFolder ? new FileInfo(UserFolder.FullName + @"\all_sites.csv") :
				new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv");
			FileInfo source_allcells = onUserFolder ? new FileInfo(UserFolder.FullName + @"\all_cells.csv") :
				new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv");
			
			string response = OIConnection.requestPhpOutput("allsites");
			if(response.StartsWith("SITE,JVCO_ID,GSM900,")) {
				if(GlobalProperties.shareAccess || onUserFolder) {
					if(source_allsites.Exists) {
						if(response != File.ReadAllText(source_allsites.FullName)) {
							MainForm.trayIcon.showBalloon("Updating file", "all_sites.csv updating...");
							File.WriteAllText(source_allsites.FullName, response);
						}
					}
					else {
						MainForm.trayIcon.showBalloon("Downloading file", "Downloading all_sites.csv...");
						File.WriteAllText(source_allsites.FullName, response);
					}
				}
			}
			response = OIConnection.requestPhpOutput("allcells");
			if(response.StartsWith("SITE,JVCO_ID,CELL_ID,")) {
				if(GlobalProperties.shareAccess || onUserFolder) {
					if(source_allcells.Exists) {
						if(response != File.ReadAllText(source_allcells.FullName)) {
							MainForm.trayIcon.showBalloon("Updating file", "all_cells.csv updating...");
							File.WriteAllText(source_allcells.FullName, response);
						}
					}
					else {
						MainForm.trayIcon.showBalloon("Downloading file", "Downloading all_cells.csv...");
						File.WriteAllText(source_allcells.FullName, response);
					}
				}
			}
//			}
//			                        });
//			thread.IsBackground = true;
//			thread.Start();
		}
		
		public static void PopulateDatabases() {
			bool loadingAllSitesFinished = false;
			bool loadingAllCellsFinished = false;
			bool loadingShiftsFileFinished = false;
			Thread all_sitesThread = new Thread(() => {
			                                    	all_sites = new FileInfo(all_sites.FullName);
			                                    	loadingAllSitesFinished = true;
			                                    });
			all_sitesThread.IsBackground = true;
			all_sitesThread.Start();
			Thread all_cellsThread = new Thread(() => {
			                                    	all_cells = new FileInfo(all_cells.FullName);
			                                    	loadingAllCellsFinished = true;
			                                    });
			all_cellsThread.IsBackground = true;
			all_cellsThread.Start();
			Thread shiftsFileThread = new Thread(() => {
			                                     	shiftsFile = new ShiftsFile(DateTime.Now.Year);
			                                     	loadingShiftsFileFinished = true;
			                                     });
			shiftsFileThread.IsBackground = true;
			shiftsFileThread.Start();
			
			while(!loadingAllSitesFinished || !loadingAllCellsFinished || !loadingShiftsFileFinished) {}
			
//			LoadDBFiles(null, null);
//			AutoUpdateTimer.Elapsed += LoadDBFiles;
			ResetAutoUpdateTimer();
		}
		
		public static void LoadDBFiles(object source, ElapsedEventArgs e) {
			if(e == null) {}
//			UpdateDBFiles(all_sites, all_cells);
			bool finishedTask = false;
			var thread = new Thread(() => {
			                        	UserFolder.UpdateLocalDBFilesCopy();
			                        	finishedTask = true;
			                        });
			thread.IsBackground = true;
			thread.Start();
			while(!finishedTask) {}
			try {
				thread.Abort();
			}
			catch(ThreadAbortException) { }
		}
	}
}

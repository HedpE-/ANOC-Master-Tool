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
using System.Threading;
using System.IO;
using System.Linq;
using System.Timers;
using appCore.Settings;
using appCore.OI;

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
					_all_sites = value;
			}
		}
		static readonly string currentAllSitesHeaders = "SITE,JVCO_ID,GSM900,GSM1800,UMTS900,UMTS2100,LTE800,LTE2600,LTE2100,EASTING,NORTHING,HOST,PRIORITY,ADDRESS,TELLABSATRISK,AREA,NSN_STATUS,NOC2G,NOC3G,NOC4G,VF_REGION,CLUSTER_NAME,SPECIAL_ID,SPECIAL,SPECIAL_START,SPECIAL_END,VIP,SITE_SHARE_OPERATOR,SITE_SHARE_SITE_NO,SITE_ACCESS,SITE_TYPE,SITE_SUBTYPE,PAKNET_FITTED,VODAPAGE_FITTED,DC_STATUS,DC_TIMESTAMP,COOLING_STATUS,COOLING_TIMESTAMP,KEY_INFORMATION,EF_HEALTHANDSAFETY,SWITCH2G,SWITCH3G,SWITCH4G,DRSWITCH2G,DRSWITCH3G,DRSWITCH4G,MTX2G,MTX3G,MTX4G,IP_2G_I,IP_2G_E,IP_3G_I,IP_3G_E,IP_4G_I,IP_4G_E,VENDOR_2G,VENDOR_3G,VENDOR_4G,MTX_RELATED";
		
		static FileInfo _all_cells;
		public static FileInfo all_cells {
			get {
				_all_cells = new FileInfo(_all_cells.FullName);
				return _all_cells;
			}
			private set {
				if (File.Exists(all_sites.FullName) && File.Exists(value.FullName))
					_all_cells = value;
			}
		}
		static readonly string currentAllCellsHeaders = "SITE,JVCO_ID,CELL_ID,LAC_TAC,BSC_RNC_ID,VENDOR,ENODEB_ID,TF_SITENO,CELL_NAME,BEARER,COOS,SO_EXCLUSION,WHITE_LIST,NTQ,NOC,WBTS_BCF,LOCKED,IP_2G_I,IP_2G_E,IP_3G_I,IP_3G_E,IP_4G_I,IP_4G_E,VENDOR_2G,VENDOR_3G,VENDOR_4G";
		
		public static ShiftsFile shiftsFile;
		
		public static bool autoUpdateRemoteDbFiles = false;
		public static bool ongoingRemoteDbFilesUpdate;
		
		static System.Timers.Timer RemoteDbAutoUpdateTimer = new System.Timers.Timer((60 * 60) * 2 * 1000); // 2 hours in milliseconds
		
		public static GeoAPIs.UkCities Cities;
		
		public static void Initialize() {
			_all_sites = new FileInfo(UserFolder.FullName + @"\all_sites.csv");
			_all_cells = new FileInfo(UserFolder.FullName + @"\all_cells.csv");
			
			if((CurrentUser.UserName == "GONCARJ3" || CurrentUser.UserName == "SANTOSS2") && autoUpdateRemoteDbFiles) {
				RemoteDbAutoUpdateTimer.Elapsed += RemoteDbAutoUpdateTimer_Elapsed;
				
				Thread thread = new Thread(() => {
				                           	RemoteDbAutoUpdateTimer_Elapsed(null, null);
				                           });
				thread.Name = "Databases.Initialize_RemoteDbAutoUpdateTimer_Elapsed";
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				
				RemoteDbAutoUpdateTimer.Enabled = true;
			}
			
			WeatherCollection.Initialize(GlobalProperties.ShareRootDir.FullName + @"\ANOC Master Tool\WeatherDB.json");
		}
		
		static void RemoteDbAutoUpdateTimer_Elapsed(object sender, ElapsedEventArgs e) {
			ongoingRemoteDbFilesUpdate = true;
			UpdateSourceDBFiles();
			ongoingRemoteDbFilesUpdate = false;
			if(e != null)
				UserFolder.UpdateLocalDBFilesCopy();
		}
		
		public static void UpdateSourceDBFiles(bool onUserFolder = false) {
			FileInfo source_allsites = onUserFolder ? new FileInfo(UserFolder.FullName + @"\all_sites.csv") :
				new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv");
			FileInfo source_allcells = onUserFolder ? new FileInfo(UserFolder.FullName + @"\all_cells.csv") :
				new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv");
			
			string updLocation = onUserFolder ? "on your UserFolder." : "on the Remote folder.";
			
			List<Thread> threads = new List<Thread>();
			int finishedThreadsCount = 0;
			
			Thread thread = new Thread(() => {
//			                       	string response = OiConnection.requestPhpOutput("allsites");
			                           	string response = OiConnection.requestApiOutput("sites");
			                           	if(response.StartsWith("SITE,JVCO_ID,GSM900,")) {
			                           		if(response.Substring(0, response.IndexOf("\n")) != currentAllSitesHeaders)
			                           			MainForm.trayIcon.showBalloon("all_sites Headers changes", "Downloaded all_sites headers are different from the current Site class.");
			                           		if(GlobalProperties.shareAccess || onUserFolder) {
			                           			bool updated = false;
			                           			if(source_allsites.Exists) {
			                           				if(response != File.ReadAllText(source_allsites.FullName)) {
			                           					MainForm.trayIcon.showBalloon("Updating file", "all_sites.csv updating...");
			                           					File.WriteAllText(source_allsites.FullName, response);
			                           					updated = true;
			                           				}
			                           			}
			                           			else {
			                           				MainForm.trayIcon.showBalloon("Downloading file", "Downloading all_sites.csv...");
			                           				File.WriteAllText(source_allsites.FullName, response);
			                           				updated = true;
			                           			}
			                           			if(updated)
			                           				MainForm.trayIcon.showBalloon("Update complete", "all_sites.csv updated successfully " + updLocation);
			                           		}
			                           	}
			                           	
			                           	finishedThreadsCount++;
			                           });
			thread.Name = "UpdateSourceDBFiles_allsitesThread";
			threads.Add(thread);
			
			thread = new Thread(() => {
//			                       	string response = OiConnection.requestPhpOutput("allcells");
			                    	string response = OiConnection.requestApiOutput("cells");
			                    	if(response.StartsWith("SITE,JVCO_ID,CELL_ID,")) {
			                    		if(response.Substring(0, response.IndexOf("\n")) != currentAllCellsHeaders)
			                    			MainForm.trayIcon.showBalloon("all_cells Headers changes", "Downloaded all_cells headers are different from the current Cell class.");
			                    		if(GlobalProperties.shareAccess || onUserFolder) {
			                    			bool updated = false;
			                    			if(source_allcells.Exists) {
			                    				if(response != File.ReadAllText(source_allcells.FullName)) {
			                    					MainForm.trayIcon.showBalloon("Updating file", "all_cells.csv updating...");
			                    					File.WriteAllText(source_allcells.FullName, response);
			                    					updated = true;
			                    				}
			                    			}
			                    			else {
			                    				MainForm.trayIcon.showBalloon("Downloading file", "Downloading all_cells.csv...");
			                    				File.WriteAllText(source_allcells.FullName, response);
			                    				updated = true;
			                    			}
			                    			if(updated)
			                    				MainForm.trayIcon.showBalloon("Update complete", "all_cells.csv updated successfully " + updLocation);
			                    		}
			                    	}
			                    	
			                    	finishedThreadsCount++;
			                    });
			thread.Name = "UpdateSourceDBFiles_allcellsThread";
			threads.Add(thread);
			
			foreach(Thread th in threads) {
				th.SetApartmentState(ApartmentState.STA);
				th.Start();
			}
			
			while(finishedThreadsCount < threads.Count) { }
		}
		
		public static void PopulateDatabases() {
			all_sites = new FileInfo(all_sites.FullName);
			all_cells = new FileInfo(all_cells.FullName);
			shiftsFile = new ShiftsFile(DateTime.Now.Year);
//			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
//			st.Start();
			Cities = new GeoAPIs.UkCities();
//			st.Stop();
//			var t = st.Elapsed;
		}
	}
}

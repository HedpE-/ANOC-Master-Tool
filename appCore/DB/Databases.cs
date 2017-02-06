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
					_all_sites = value;
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
					_all_cells = value;
			}
		}
		
		public static ShiftsFile shiftsFile;
		
		static System.Timers.Timer AutoUpdateTimer = new System.Timers.Timer((60 * 60) * 2 * 1000); //2 hours in milliseconds
		
		public static void ResetAutoUpdateTimer() {
			AutoUpdateTimer.Stop();
			AutoUpdateTimer.Start();
		}
		
		public static void Initialize() {
//			if((CurrentUser.UserName == "GONCARJ3" || CurrentUser.UserName == "SANTOSS2") && GlobalProperties.autoUpdateDbFiles)
//				UpdateSourceDBFiles();
			
			_all_sites = new FileInfo(UserFolder.FullName + @"\all_sites.csv");
			_all_cells = new FileInfo(UserFolder.FullName + @"\all_cells.csv");
		}
		
		public static void UpdateSourceDBFiles(bool onUserFolder = false) {
			FileInfo source_allsites = onUserFolder ? new FileInfo(UserFolder.FullName + @"\all_sites.csv") :
				new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv");
			FileInfo source_allcells = onUserFolder ? new FileInfo(UserFolder.FullName + @"\all_cells.csv") :
				new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv");
			
			List<Thread> threads = new List<Thread>();
			int finishedThreadsCount = 0;
			
			threads.Add(new Thread(() => {
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
			                       	
			                       	finishedThreadsCount++;
			                       }));
			
			threads.Add(new Thread(() => {
			                       	string response = OIConnection.requestPhpOutput("allcells");
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
			                       	
			                       	finishedThreadsCount++;
			                       }));
			
			foreach(Thread thread in threads) {
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
			
			while(finishedThreadsCount < threads.Count) { }
			
			AutoUpdateTimer.Elapsed += LoadDBFiles;
			ResetAutoUpdateTimer();
		}
		
		public static void LoadDBFiles(object source, ElapsedEventArgs e) {
			if(e == null) {}
			UserFolder.UpdateLocalDBFilesCopy();
		}
		
		public static void PopulateDatabases() {
			List<Thread> threads = new List<Thread>();
			int finishedThreadsCount = 0;
			
			threads.Add(new Thread(() => {
			                       	all_sites = new FileInfo(all_sites.FullName);
			                       	
			                       	finishedThreadsCount++;
			                       }));
			
			threads.Add(new Thread(() => {
			                       	all_cells = new FileInfo(all_cells.FullName);
			                       	
			                       	finishedThreadsCount++;
			                       }));
			
			threads.Add(new Thread(() => {
			                       	shiftsFile = new ShiftsFile(DateTime.Now.Year);
			                       	
			                       	finishedThreadsCount++;
			                       }));
			
			foreach(Thread thread in threads) {
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
			
			while(finishedThreadsCount < threads.Count) { }
		}
	}
}

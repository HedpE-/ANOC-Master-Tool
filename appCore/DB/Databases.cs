/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-08-2016
 * Time: 13:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Data;
using System.Threading;
using System.IO;
using System.Net;
using System.Timers;
using appCore.Settings;
using appCore.Toolbox;
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
					bool sameFile = value.FullName == _all_sites.FullName && value.Length == _all_sites.Length;
					_all_sites = value;
					if (!sameFile || siteDetailsTable == null)
						siteDetailsTable = _all_sites.Exists ? Tools.GetDataTableFromCsv(_all_sites, true) : buildSitesTable();
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
					bool sameFile = value.FullName == _all_cells.FullName && value.Length == _all_cells.Length;
					_all_cells = value;
					if (!sameFile || cellDetailsTable == null)
						cellDetailsTable = _all_cells.Exists ? Tools.GetDataTableFromCsv(_all_cells, true) : buildSitesTable();
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
			if((CurrentUser.userName == "GONCARJ3" || CurrentUser.userName == "SANTOSS0") && GlobalProperties.autoUpdateDbFiles)
				UpdateSourceDBFiles();
			
			_all_sites = new FileInfo(UserFolder.FullName + @"\all_sites.csv");
			_all_cells = new FileInfo(UserFolder.FullName + @"\all_cells.csv");
		}
		
		public static void UpdateSourceDBFiles() { // string all_sites, string all_cells) {
			// UNDONE: UpdateSourceDBFiles()
//			var thread = new Thread(() => {
//			string test = CurrentUser.GetUserDetails("NetworkDomain"); // TODO: Test Domain name on VF network
			FileInfo source_allsites = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv");
			FileInfo source_allcells = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv");
//			HttpStatusCode status = HttpStatusCode.NotFound;
//			if(OIConnection.Connection == null)
//				status = OIConnection.EstablishConnection();
//			HttpStatusCode statusCode = OIConnection.Connection.Logon();
//			if(!OIConnection.Connection.LoggedOn)
//			HttpStatusCode statusCode = OIConnection2.InitiateOiConnection();
//			if(statusCode == HttpStatusCode.OK) {
				string response = OIConnection.requestPhpOutput("allsites");
				if(response.StartsWith("SITE,JVCO_ID,GSM900,")) {
					if(GlobalProperties.shareAccess) {
						if(source_allsites.Exists) {
							if(response != File.ReadAllText(source_allsites.FullName)) {
								MainForm.trayIcon.showBalloon("Updating file", "all_sites.csv updating...");
								File.WriteAllText(source_allsites.FullName, response);
							}
						}
						else {
							MainForm.trayIcon.showBalloon("Updating file", "all_sites.csv updating...");
							File.WriteAllText(source_allsites.FullName, response);
						}
					}
				}
				response = OIConnection.requestPhpOutput("allcells");
				if(response.StartsWith("SITE,JVCO_ID,CELL_ID,")) {
					if(GlobalProperties.shareAccess) {
						if(source_allcells.Exists) {
							if(response != File.ReadAllText(source_allcells.FullName)) {
								MainForm.trayIcon.showBalloon("Updating file", "all_cells.csv updating...");
								File.WriteAllText(source_allcells.FullName, response);
							}
						}
						else {
							MainForm.trayIcon.showBalloon("Updating file", "all_cells.csv updating...");
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
//			Stopwatch sw = new Stopwatch();
//			sw.Start();
//
//			bool finishedTask = false;
//			Thread thread = new Thread(() => {
//			                           	all_sites = new FileInfo(all_sites.FullName);
//			                           	all_cells = new FileInfo(all_cells.FullName);
//			                           	shiftsFile = new ShiftsFile();
//			                           	finishedTask = true;
//			                           });
//			thread.IsBackground = true;
//			thread.Start();
			
//			while(!finishedTask) {}
//
//			sw.Stop();
//			FlexibleMessageBox.Show("Elapsed=" + sw.Elapsed);
//			TimeSpan sol1Time = sw.Elapsed;
//
//			siteDetailsTable = null;
//			cellDetailsTable = null;
//
//			sw = new Stopwatch();
//			sw.Start();
			
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
			                                     	shiftsFile = new ShiftsFile();
			                                     	loadingShiftsFileFinished = true;
			                                     });
			shiftsFileThread.IsBackground = true;
			shiftsFileThread.Start();
			
			while(!loadingAllSitesFinished || !loadingAllCellsFinished || !loadingShiftsFileFinished) {}
			
//			sw.Stop();
//			System.Windows.Forms.MessageBox.Show("Old way\nElapsed = " + sol1Time + "\n\nNew stuff\nElapsed = " + sw.Elapsed);
//			FlexibleMessageBox.Show("Old way\nElapsed = " + sol1Time + "\n\nNew stuff\nElapsed = " + sw.Elapsed);
			
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
		
//		public static void RefreshDBFiles(string files) {
//			if(files == "all" || files.Contains("all_sites")) {
//				all_sites = new FileInfo(all_sites.FullName);
//				siteDetailsTable = all_sites.Exists ? Tools.GetDataTableFromCsv(all_sites, true) : buildSitesTable();
//			}
//
//			if(files == "all" || files.Contains("all_cells")) {
//				all_cells = new FileInfo(all_cells.FullName);
//				cellDetailsTable = all_cells.Exists ? Tools.GetDataTableFromCsv(all_cells, true) : buildCellsTable();
//			}
//
//			if(files == "all" || files.Contains("shifts"))
//				shiftsFile = new ShiftsFile();
//		}
		
		static DataTable buildSitesTable() {
			DataTable dt = new DataTable();
			
			dt.Columns.Add(new DataColumn("SITE"));
			dt.Columns.Add(new DataColumn("JVCO_ID"));
			dt.Columns.Add(new DataColumn("GSM900"));
			dt.Columns.Add(new DataColumn("GSM1800"));
			dt.Columns.Add(new DataColumn("UMTS900"));
			dt.Columns.Add(new DataColumn("UMTS2100"));
			dt.Columns.Add(new DataColumn("LTE800"));
			dt.Columns.Add(new DataColumn("LTE2600"));
			dt.Columns.Add(new DataColumn("EASTING"));
			dt.Columns.Add(new DataColumn("NORTHING"));
			dt.Columns.Add(new DataColumn("HOST"));
			dt.Columns.Add(new DataColumn("PRIORITY"));
			dt.Columns.Add(new DataColumn("ADDRESS"));
			dt.Columns.Add(new DataColumn("POWER_COMPANY"));
			dt.Columns.Add(new DataColumn("POWER_CONTACT"));
			dt.Columns.Add(new DataColumn("TELLABSATRISK"));
			dt.Columns.Add(new DataColumn("AREA"));
			dt.Columns.Add(new DataColumn("NSN_STATUS"));
			dt.Columns.Add(new DataColumn("NOC2G"));
			dt.Columns.Add(new DataColumn("NOC3G"));
			dt.Columns.Add(new DataColumn("NOC4G"));
			dt.Columns.Add(new DataColumn("VF_REGION"));
			dt.Columns.Add(new DataColumn("SPECIAL"));
			dt.Columns.Add(new DataColumn("SPECIAL_START"));
			dt.Columns.Add(new DataColumn("SPECIAL_END"));
			dt.Columns.Add(new DataColumn("VIP"));
			dt.Columns.Add(new DataColumn("SITE_SHARE_OPERATOR"));
			dt.Columns.Add(new DataColumn("SITE_SHARE_SITE_NO"));
			dt.Columns.Add(new DataColumn("SITE_ACCESS"));
			dt.Columns.Add(new DataColumn("SITE_TYPE"));
			dt.Columns.Add(new DataColumn("SITE_SUBTYPE"));
			dt.Columns.Add(new DataColumn("PAKNET_FITTED"));
			dt.Columns.Add(new DataColumn("VODAPAGE_FITTED"));
			dt.Columns.Add(new DataColumn("DC_STATUS"));
			dt.Columns.Add(new DataColumn("DC_TIMESTAMP"));
			dt.Columns.Add(new DataColumn("COOLING_STATUS"));
			dt.Columns.Add(new DataColumn("COOLING_TIMESTAMP"));
			dt.Columns.Add(new DataColumn("KEY_INFORMATION"));
			dt.Columns.Add(new DataColumn("EF_HEALTHANDSAFETY"));
			dt.Columns.Add(new DataColumn("SWITCH2G"));
			dt.Columns.Add(new DataColumn("SWITCH3G"));
			dt.Columns.Add(new DataColumn("SWITCH4G"));
			dt.Columns.Add(new DataColumn("DRSWITCH2G"));
			dt.Columns.Add(new DataColumn("DRSWITCH3G"));
			dt.Columns.Add(new DataColumn("DRSWITCH4G"));
			dt.Columns.Add(new DataColumn("MTX2G"));
			dt.Columns.Add(new DataColumn("MTX3G"));
			dt.Columns.Add(new DataColumn("MTX4G"));
			dt.Columns.Add(new DataColumn("MTX_RELATED"));

			return dt;
		}
		
		static DataTable buildCellsTable() {
			DataTable dt = new DataTable();
			
			dt.Columns.Add(new DataColumn("SITE"));
			dt.Columns.Add(new DataColumn("JVCO_ID"));
			dt.Columns.Add(new DataColumn("CELL_ID"));
			dt.Columns.Add(new DataColumn("LAC_TAC"));
			dt.Columns.Add(new DataColumn("BSC_RNC_ID"));
			dt.Columns.Add(new DataColumn("VENDOR"));
			dt.Columns.Add(new DataColumn("ENODEB_ID"));
			dt.Columns.Add(new DataColumn("TF_SITENO"));
			dt.Columns.Add(new DataColumn("CELL_NAME"));
			dt.Columns.Add(new DataColumn("BEARER"));
			dt.Columns.Add(new DataColumn("COOS"));
			dt.Columns.Add(new DataColumn("SO_EXCLUSION"));
			dt.Columns.Add(new DataColumn("WHITE_LIST"));
			dt.Columns.Add(new DataColumn("NTQ"));
			dt.Columns.Add(new DataColumn("NOC"));
			dt.Columns.Add(new DataColumn("WBTS_BCF"));
			dt.Columns.Add(new DataColumn("LOCKED"));
			
//			dt.Rows.Add("35","V0018305S","3726","304","BEASN","Ericsson",null,null,"G00351","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("35","V0018305S","3727","304","BEASN","Ericsson",null,null,"G00352","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("35","V0018305S","3728","304","BEASN","Ericsson",null,null,"G00353","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("35","V0018305S","33067","21481","BEASN","Ericsson",null,null,"G0035W","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("35","V0018305S","23067","21481","BEASN","Ericsson",null,null,"G0035X","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("35","V0018305S","13067","21481","BEASN","Ericsson",null,null,"G0035Y","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("35","V0018305S","63051","8","RNCCY3","Ericsson",null,null,"M00035015","3G",null,null,null,null,"TANOC","WBTS-999",null);
//			dt.Rows.Add("35","V0018305S","63050","8","RNCCY3","Ericsson",null,null,"M00035025","3G",null,null,null,null,"TANOC","WBTS-999",null);
//			dt.Rows.Add("35","V0018305S","63049","8","RNCCY3","Ericsson",null,null,"M00035035","3G",null,null,null,null,"TANOC","WBTS-999",null);
//			dt.Rows.Add("35","V0018305S","17715","21723","RNCCY3","Ericsson",null,null,"TM00035017","3G",null,null,null,null,"TANOC","WBTS-1999",null);
//			dt.Rows.Add("35","V0018305S","27715","21723","RNCCY3","Ericsson",null,null,"TM00035027","3G",null,null,null,null,"TANOC","WBTS-1999",null);
//			dt.Rows.Add("35","V0018305S","37715","21723","RNCCY3","Ericsson",null,null,"TM00035037","3G",null,null,null,null,"TANOC","WBTS-1999",null);
//			dt.Rows.Add("35","V0018305S","10","24615","XCY_CA_01","Ericsson","1489",null,"N00035010","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","20","24615","XCY_CA_01","Ericsson","1489",null,"N00035020","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","30","24615","XCY_CA_01","Ericsson","1489",null,"N00035030","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","110","16784","XCY_CA_01","Ericsson","101489",null,"TN00035110","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","120","16784","XCY_CA_01","Ericsson","101489",null,"TN00035120","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","130","16784","XCY_CA_01","Ericsson","101489",null,"TN00035130","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("125","V0018305S","33067","21481","BEASN","Ericsson",null,null,"G0125W","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("125","V0018305S","23067","21481","BEASN","Ericsson",null,null,"G0125X","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("125","V0018305S","13067","21481","BEASN","Ericsson",null,null,"G0125Y","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("136","V0018305S","3726","304","BEASN","Ericsson",null,null,"G001361","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("136","V0018305S","3727","304","BEASN","Ericsson",null,null,"G001362","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("136","V0018305S","3728","304","BEASN","Ericsson",null,null,"G001363","2G",null,null,null,null,"ANOC","BCF-505",null);
			
			return dt;
		}
	}
}

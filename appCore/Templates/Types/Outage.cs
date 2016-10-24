/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 15:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using appCore.Netcool;
using appCore.SiteFinder;
using appCore.Settings;

namespace appCore.Templates.Types
{
	/// <summary>
	/// Description of Outage.
	/// </summary>
//	[Serializable]
	public class Outage : Template
	{
		public string VFoutage;
		public string TFoutage;
		public string VFbulkCI;
		public string TFbulkCI;
		public List<string> sitesList; // sites string to get POC designations 
		public List<Site> Sites; // Para o parser guardar os sites para o bulkCI
		public List<Cell> Cells; // Cells list caught on outage alarms, these can be compared later on Outage Follow Up 
		public List<string> Locations;
		List<Alarm> OutageAlarms;
		
//		string siteids = string.Empty;
//		public string SiteIDs { get { return siteids; } protected set { siteids = value; } }
//		string serviceaffected = string.Empty;
//		public string ServiceAffected { get { return serviceaffected; } protected set { serviceaffected = value; } }
//		bool repeat_intermittent;
//		public bool Repeat_Intermittent{ get { return repeat_intermittent; } protected set { repeat_intermittent = value; } }
//		string txtype = string.Empty;
//		public string TxType { get { return txtype; } protected set { txtype = value; } }
//		string ipranportconfig = string.Empty;
//		public string IpRanPortConfig { get { return ipranportconfig; } protected set { ipranportconfig = value; } }
//		string performanceoutagedetails = string.Empty;
//		public string PerformanceOutageDetails { get { return performanceoutagedetails; } protected set { performanceoutagedetails = value; } }
//		string detailedrantroubleshoot = string.Empty;
//		public string DetailedRanTroubleshoot { get { return detailedrantroubleshoot; } protected set { detailedrantroubleshoot = value; } }
		
		public Outage(List<Site> sites) {
			Sites = sites;
			foreach (Site site in Sites) {
			Cells.AddRange(site.Cells);
			Locations.Add(site.Address);
//			VFlocationsArrayList.Add(new string[]{address[address.Length - 2].Trim(' '),siteHasVF2G.ToString(),siteHasVF3G.ToString(),siteHasVF4G.ToString()});
			}
			LogType = "Outage";
		}
		
		public Outage(Parser alarms) {
			try {
//				Outage outage = new Outage();
//				
//				parserTable = op.parse(alarms);
//				VFoutage = op.genReport(parserTable,"VF");
////				VFbulkCI = op.bulkCi(sites);
//				TFoutage = op.genReport(parserTable,"TF");
////				TFbulkCI = op.bulkCi(sites);
			}
			catch {
				MainForm.trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
				return;
			}
			
			foreach(Alarm alarm in alarms.AlarmsList) {
				// Filtro alarmes //
				
				switch (alarm.Vendor)
				{
					case Site.Vendors.ALU:
						if(alarm.Summary.Contains("UNDERLYING_RESOURCE_UNAVAILABLE: State change to Disable"))
							OutageAlarms.Add(alarm);
						break;
					case Site.Vendors.Ericsson:
						if ((alarm.Summary.Contains("CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION") && alarm.Summary.Contains("BCCH")) || alarm.Summary.Contains("UtranCell_ServiceUnavailable") || alarm.Summary.Contains("4G: Heartbeat Failure"))
							OutageAlarms.Add(alarm);
						break;
					case Site.Vendors.Huawei:
						if (alarm.Summary.Contains("Cell out of Service") || alarm.Summary.Contains("Cell Unavailable") || alarm.Summary.Contains("Local Cell Unusable") || alarm.Summary.Contains("eNodeB"))
							OutageAlarms.Add(alarm);
						break;
					case Site.Vendors.NSN:
						if (!(alarm.Summary.Contains("BCCH MISSING") || alarm.Summary.Contains("CELL FAULTY") || alarm.Summary.Contains("WCDMA CELL OUT OF USE") || alarm.Summary.Contains("P3 ENODEB: NE O&M")))
							OutageAlarms.Add(alarm);
						break;
				}
			}
			
//			fullLog = generateFullLog();
			LogType = "Outage";
		}
		
		public Outage(Outage template) {
			Toolbox.Tools.CopyProperties(this, template);
//			fullLog = generateFullLog();
			LogType = "Outage";
		}
		
		public Outage(string[] log, DateTime date) {
			LoadOutageReport(log);
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = "Outage";
		}
		
		public void LoadOutageReport(string[] log)
		{
//			int c = 0;
//			VFoutage = string.Empty;
//			TFoutage = string.Empty;
//			VFbulkCI = string.Empty;
//			TFbulkCI = string.Empty;
//			string[] strTofind = { "\r\n" };
//			string[] log = globalLogs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
//			
//			if(string.IsNullOrEmpty(log[log.Length - 1])) {
//				log = log.Where((source, index) => index != log.Length - 1).ToArray();
//			}
//			
//			// Manipulate log array to make it compatible with VF/TF new logs
//			if(Array.FindIndex(log,element => element.Contains("F Report----------")) == -1) {
//				List<string> log2 = log.ToList(); // Create new List with log array values
//				string Report = log2[1]; // Store outage report to string
//				log2.RemoveAt(1); // Remove outage report previously stored on Report string
//				string[] SplitReport = Report.Split('\n'); // Split Report string to new array
//				log2.Insert(1,"----------VF Report----------"); // Insert VF Report header to match code checks
//				log2.InsertRange(2,SplitReport); // Insert SplitReport array into list after header
//				log = log2.ToArray(); // Replace original log array with new generated List values
//				if(Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal)) > -1) // Check if log contains LTE sites
//					log[Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal))] = "----------LTE sites----------"; // Convert header to match code checks
//			}
//			
//			int VFreportIndex = Array.FindIndex(log, element => element.Equals("----------VF Report----------", StringComparison.Ordinal));
//			int VFbulkciIndex = Array.FindIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
//			int TFreportIndex = Array.FindIndex(log, element => element.Equals("----------TF Report----------", StringComparison.Ordinal));
//			int TFbulkciIndex = Array.FindLastIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
//			
//			if(VFreportIndex > -1) {
//				for(c = VFreportIndex + 1;c < VFbulkciIndex;c++) {
//					VFoutage += log[c];
//					if(c < VFbulkciIndex - 1)
//						VFoutage += Environment.NewLine;
//				}
//				
//				if(TFreportIndex == -1) {
//					TFreportIndex = log.Length;
//				}
//				
//				for(c = VFbulkciIndex + 1;c < TFreportIndex;c++) {
//					VFbulkCI += log[c];
//					if(c < TFreportIndex - 1)
//						VFbulkCI += Environment.NewLine;
//				}
//			}
//			
//			if(TFreportIndex == log.Length)
//				TFreportIndex--;
//			
//			if(log[TFreportIndex].Equals("----------TF Report----------")) {
//				for(c = TFreportIndex + 1;c < TFbulkciIndex;c++) {
//					TFoutage += log[c];
//					if(c < TFbulkciIndex - 1)
//						TFoutage += Environment.NewLine;
//				}
//				
//				for(c = TFbulkciIndex + 1;c < log.Length;c++) {
//					TFbulkCI += log[c];
//					if(c < log.Length - 1)
//						VFbulkCI += Environment.NewLine;
//				}
//			}
//			
//			if(!string.IsNullOrEmpty(VFoutage) && !string.IsNullOrEmpty(TFoutage)) {
//				tabControl1.Visible = true;
//				tabControl1.SelectTab(0);
//			}
//			else {
//				if(!string.IsNullOrEmpty(VFoutage)) {
//					tabControl1.Visible = false;
//					tabControl1.SelectTab(0);
//				}
//				else {
//					if(!string.IsNullOrEmpty(TFoutage)) {
//						tabControl1.Visible = false;
//						tabControl1.SelectTab(1);
//					}
//				}
//			}
//			TabControl4SelectedIndexChanged(null,null);
		}
	}
}
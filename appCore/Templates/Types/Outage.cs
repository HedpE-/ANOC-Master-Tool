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
using System.Linq;
using appCore.Netcool;
using appCore.SiteFinder;

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
		public List<Site> Sites = new List<Site>(); // Para o parser guardar os sites para o bulkCI
		public List<Cell> Cells = new List<Cell>(); // Cells list caught on outage alarms, these can be compared later on Outage Follow Up
		public List<string> Locations = new List<string>();
		List<Alarm> OutageAlarms = new List<Alarm>();
		
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
				Locations.Add(site.Town);
//			VFlocationsArrayList.Add(new string[]{address[address.Length - 2].Trim(' '),siteHasVF2G.ToString(),siteHasVF3G.ToString(),siteHasVF4G.ToString()});
			}
			LogType = "Outage";
		}
		
		public Outage(Parser alarms) {
			// Get LTE O&M alarms to get all affected cells
			// Take COOS alarms on the OutageAlarms list
			List<Alarm> resolved4gCoosAlarms = new List<Alarm>();
			foreach(Alarm alarm in alarms.AlarmsList) {
				if(alarm.Bearer == "4G") {
					if(alarm.OnM) {
						List<Cell> LTEcells = alarm.ParentSite.Cells.Where(s => s.Bearer == "4G").ToList();
						foreach (Cell cell in LTEcells)
							resolved4gCoosAlarms.Add(new Alarm(cell, true, alarm.LastOccurrence));
					}
				}
				else
					if(alarm.COOS)
						OutageAlarms.Add(alarm);
			}
			
			OutageAlarms.AddRange(resolved4gCoosAlarms);
			
			// TODO: Separate VF & TF cells
			// TODO: Build both reports
			
			fullLog = generateFullLog();
			LogType = "Outage";
			
//			parserTable = op.parse(alarms);
//			VFoutage = op.genReport(parserTable,"VF");
//			VFbulkCI = op.bulkCi(sites);
//			TFoutage = op.genReport(parserTable,"TF");
//			TFbulkCI = op.bulkCi(sites);
		}
		
		public Outage(Outage existingOutage) {
			Toolbox.Tools.CopyProperties(this, existingOutage);
			fullLog = generateFullLog();
			LogType = "Outage";
		}
		
		public Outage(string[] log, DateTime date) {
			LoadOutageReport(log);
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = "Outage";
		}
		
		string generateFullLog() {
			return string.Empty;
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
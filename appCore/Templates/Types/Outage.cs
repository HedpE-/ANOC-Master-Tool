﻿/*
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
using FileHelpers;

namespace appCore.Templates.Types
{
	/// <summary>
	/// Description of Outage.
	/// </summary>
	//	[Serializable]
	public class Outage : Template
	{
		public string VfOutage;
		public string VfBulkCI;
		public string TefOutage;
		public string TefBulkCI;
		
		List<string> VfSites = new List<string>();
		List<string> VfGsmCells = new List<string>();
		List<string> VfUmtsCells = new List<string>();
		List<string> VfLteCells = new List<string>();
		List<string> VfLocations = new List<string>();
		DateTime VfGsmTime = new DateTime(2500,1,1);
		DateTime VfUmtsTime = new DateTime(2500,1,1);
		DateTime VfLteTime = new DateTime(2500,1,1);
		List<string> TefSites = new List<string>();
		List<string> TefGsmCells = new List<string>();
		List<string> TefUmtsCells = new List<string>();
		List<string> TefLteCells = new List<string>();
		List<string> TefLocations = new List<string>();
		DateTime TefGsmTime = new DateTime(2500,1,1);
		DateTime TefUmtsTime = new DateTime(2500,1,1);
		DateTime TefLteTime = new DateTime(2500,1,1);
		
		List<Alarm> OutageAlarms;
		
		List<Alarm> VfAlarms {
			get {
				return OutageAlarms.FindAll(s => s.Operator == "VF");
			}
		}
		List<Alarm> TefAlarms {
			get {
				return OutageAlarms.FindAll(s => s.Operator == "TEF");
			}
		}
		
		public Outage(AlarmsParser alarms) {
			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
			st.Start();
			OutageAlarms = alarms.AlarmsList;
			
			List<Cell> LTEcells = new List<Cell>();
			
			TimeSpan t2;
			if(alarms.lteSites.Count > 0) {
				LTEcells = Finder.getCells(alarms.lteSites, "4G");
//				System.Diagnostics.Stopwatch st2 = new System.Diagnostics.Stopwatch();
//				st2.Start();
				
				foreach(Cell cell in LTEcells) {
					Alarm temp = OutageAlarms.Find(a => a.SiteId == cell.ParentSite);
					OutageAlarms.Add(new Alarm(cell, true, temp));
				}
//				st2.Stop();
//				t2 = st2.Elapsed;
			}
			string toparse = string.Empty;
			try {
				var engine = new FileHelperEngine<Alarm>();

				engine.BeforeWriteRecord += (eng, e) => {
					if(e.Record.OnM && !e.Record.COOS)
						e.SkipThisRecord = true;
				};

				toparse = engine.WriteString(OutageAlarms);
			}
			catch(FileHelpersException e) {
				var m = e.Message;
			}
			
			try {
				var engine = new FileHelperEngine<Alarm>();
				engine.AfterReadRecord += (eng, e) => {
					string temp = e.Record.RncBsc + " - " + e.Record.Element;
					string temp2 = e.Record.Bearer;
					switch (e.Record.Operator) {
						case "VF":
							if(!VfLocations.Contains(e.Record.County) && !string.IsNullOrEmpty(e.Record.County))
								VfLocations.Add(e.Record.County);
							if(!VfSites.Contains(e.Record.Location) && !string.IsNullOrEmpty(e.Record.Location))
								VfSites.Add(e.Record.Location);
							switch(e.Record.Bearer) {
								case "2G":
									if(!VfGsmCells.Contains(temp))
										VfGsmCells.Add(temp);
									if(e.Record.LastOccurrence < VfGsmTime)
										VfGsmTime = e.Record.LastOccurrence;
									break;
								case "3G":
									if(!VfUmtsCells.Contains(temp))
										VfUmtsCells.Add(temp);
									if(e.Record.LastOccurrence < VfUmtsTime)
										VfUmtsTime = e.Record.LastOccurrence;
									break;
								case "4G":
									if(!VfLteCells.Contains(temp))
										VfLteCells.Add(temp);
									if(e.Record.LastOccurrence < VfLteTime)
										VfLteTime = e.Record.LastOccurrence;
									break;
							}
							break;
						case "TEF":
							if(!TefLocations.Contains(e.Record.County) && !string.IsNullOrEmpty(e.Record.County))
								TefLocations.Add(e.Record.County);
							if(!TefSites.Contains(e.Record.Location) && !string.IsNullOrEmpty(e.Record.Location))
								TefSites.Add(e.Record.Location);
							switch(e.Record.Bearer) {
								case "2G":
									if(!TefGsmCells.Contains(temp))
										TefGsmCells.Add(temp);
									if(e.Record.LastOccurrence < TefGsmTime)
										TefGsmTime = e.Record.LastOccurrence;
									break;
								case "3G":
									if(!TefUmtsCells.Contains(temp))
										TefUmtsCells.Add(temp);
									if(e.Record.LastOccurrence < TefUmtsTime)
										TefUmtsTime = e.Record.LastOccurrence;
									break;
								case "4G":
									if(!TefLteCells.Contains(temp))
										TefLteCells.Add(temp);
									if(e.Record.LastOccurrence < TefLteTime)
										TefLteTime = e.Record.LastOccurrence;
									break;
							}
							break;
					}
				};
				
				OutageAlarms = engine.ReadStringAsList(toparse);
			}
			catch(FileHelpersException e) {
				var m = e.Message;
			}
			
			st.Stop();
			var t = st.Elapsed;
			
			generateReports();
			
			// TODO: Build both reports
			
			LogType = "Outage";
			fullLog = generateFullLog();
		}
		
		public Outage(List<Site> sites) {
			List<Site> Sites = sites;
			List<Cell> Cells = new List<Cell>();
			List<string> Locations = new List<string>();
			foreach (Site site in Sites) {
				Cells.AddRange(site.Cells);
				Locations.Add(site.Town);
//			VFlocationsArrayList.Add(new string[]{address[address.Length - 2].Trim(' '),siteHasVF2G.ToString(),siteHasVF3G.ToString(),siteHasVF4G.ToString()});
			}
			LogType = "Outage";
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
		
		void generateReports() {
			int cellTotal = VfGsmCells.Count + VfUmtsCells.Count + VfLteCells.Count;
			VfLocations.Sort();
			VfSites.Sort();
			VfGsmCells.Sort();
			VfUmtsCells.Sort();
			VfLteCells.Sort();
			VfOutage = cellTotal + "x COOS (" + VfSites.Count;
			VfOutage += VfSites.Count == 1 ? " Site)" : " Sites)";
			VfOutage += Environment.NewLine + Environment.NewLine + "Locations (" + VfLocations.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine, VfLocations) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine, VfSites);
			
			if ( VfGsmCells.Count > 0 )
				VfOutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + VfGsmCells.Count + ") Event Time - " + VfGsmTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfGsmCells);
			if ( VfUmtsCells.Count > 0)
				VfOutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + VfUmtsCells.Count + ") Event Time - " + VfUmtsTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfUmtsCells);
			if ( VfLteCells.Count > 0)
				VfOutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + VfLteCells.Count + ") Event Time - " + VfGsmTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfLteCells);
			
			foreach (string site in VfSites) {
				string tempSite = site;
				while(tempSite.Length < 4)
					tempSite = "0" + site;
				VfBulkCI += tempSite + ";";
			}
			
			cellTotal = TefGsmCells.Count + TefUmtsCells.Count + TefLteCells.Count;
			TefLocations.Sort();
			TefSites.Sort();
			TefGsmCells.Sort();
			TefUmtsCells.Sort();
			TefLteCells.Sort();
			TefOutage = cellTotal + "x COOS (" + TefSites.Count;
			TefOutage += TefSites.Count == 1 ? " Site)" : " Sites)";
			TefOutage += Environment.NewLine + Environment.NewLine + "Locations (" + TefLocations.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine, TefLocations) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine, TefSites);
			
			if ( TefGsmCells.Count > 0 )
				TefOutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + TefGsmCells.Count + ") Event Time - " + TefGsmTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, TefGsmCells);
			if ( TefUmtsCells.Count > 0)
				TefOutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + TefUmtsCells.Count + ") Event Time - " + TefUmtsTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, TefUmtsCells);
			if ( TefLteCells.Count > 0)
				TefOutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + TefLteCells.Count + ") Event Time - " + TefGsmTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, TefLteCells);
			
			foreach (string site in TefSites) {
				string tempSite = site;
				while(tempSite.Length < 4)
					tempSite = "0" + site;
				TefBulkCI += tempSite + ";";
			}
		}
		
		string generateFullLog() {
//			VFoutage = op.genReport(parserTable,"VF");
//			VFbulkCI = op.bulkCi(sites);
//			TFoutage = op.genReport(parserTable,"TF");
//			TFbulkCI = op.bulkCi(sites);
			
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
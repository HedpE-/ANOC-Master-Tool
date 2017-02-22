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
using System.Windows.Forms;
using appCore.Netcool;
using appCore.SiteFinder;
using FileHelpers;

namespace appCore.Templates.Types
{
	/// <summary>
	/// Description of Outage.
	/// </summary>
	public class Outage : Template
	{
		public string VfOutage;
		public string VfBulkCI;
		public string TefOutage;
		public string TefBulkCI;
		
		public List<Site> AffectedSites = new List<Site>();
		public List<Cell> AffectedCells = new List<Cell>();
		
		public List<string> VfSites = new List<string>();
		public List<string> VfGsmCells = new List<string>();
		public List<string> VfUmtsCells = new List<string>();
		public List<string> VfLteCells = new List<string>();
		List<string> VfLocations = new List<string>();
		DateTime VfGsmTime = new DateTime(2500,1,1);
		DateTime VfUmtsTime = new DateTime(2500,1,1);
		DateTime VfLteTime = new DateTime(2500,1,1);
		public List<string> TefSites = new List<string>();
		public List<string> TefGsmCells = new List<string>();
		public List<string> TefUmtsCells = new List<string>();
		public List<string> TefLteCells = new List<string>();
		List<string> TefLocations = new List<string>();
		DateTime TefGsmTime = new DateTime(2500,1,1);
		DateTime TefUmtsTime = new DateTime(2500,1,1);
		DateTime TefLteTime = new DateTime(2500,1,1);
		
		public int GsmCells {
			get {
				return !string.IsNullOrEmpty(VfOutage) ? VfGsmCells.Count : TefGsmCells.Count;
			}
			private set { }
		}
		
		public int UmtsCells {
			get {
				return !string.IsNullOrEmpty(VfOutage) ? VfUmtsCells.Count : TefUmtsCells.Count;
			}
			private set { }
		}
		
		public int LteCells {
			get {
				return !string.IsNullOrEmpty(VfOutage) ? VfLteCells.Count : TefLteCells.Count;
			}
			private set { }
		}
		
		public string Summary {
			get;
			private set;
		}
		
		public DateTime EventTime {
			get {
				DateTime dt;
				if(!string.IsNullOrEmpty(VfOutage)) {
					dt = new DateTime(Math.Min(VfGsmTime.Ticks, VfUmtsTime.Ticks));
					if(VfLteTime.Year < 2500)
						dt = new DateTime(Math.Min(dt.Ticks, VfLteTime.Ticks));
				}
				else {
					dt = new DateTime(Math.Min(TefGsmTime.Ticks, TefUmtsTime.Ticks));
					if(TefLteTime.Year < 2500)
						dt = new DateTime(Math.Min(dt.Ticks, TefLteTime.Ticks));
				}
				return dt;
			}
			private set { }
		}
		
		string sitesList;
		public string SitesList {
			get {
				if(string.IsNullOrEmpty(sitesList) && (VfSites.Count > 0 || TefSites.Count > 0)) {
					List<string> sites = VfSites;
					sites.AddRange(TefSites);
					sites = sites.Distinct().ToList();
					sites.Sort();
					for(int c = 0;c < sites.Count;c++) {
						string site = sites[c];
						if(site.Contains(" - ")) {
							string[] strToFind = { " - " };
							site = site.Split(strToFind, StringSplitOptions.None)[0];
						}
						sites[c] = Convert.ToInt32(site.RemoveLetters()).ToString();
					}
					sitesList = string.Join(Environment.NewLine, sites);
				}
				return sitesList;
			}
			private set { }
		}
		
		List<Alarm> OutageAlarms;
		
		public Outage() { }
		
		public Outage(AlarmsParser alarms) {
			OutageAlarms = alarms.AlarmsList;
			
			List<Cell> LTEcells = new List<Cell>();
			
			if(alarms.lteSitesOnM.Count > 0) {
				LTEcells = Finder.getCells(alarms.lteSitesOnM, "4G");
				
				foreach(Cell cell in LTEcells) {
					try {
						Alarm temp = OutageAlarms.Find(a => a.SiteId == cell.ParentSite);
						OutageAlarms.Add(new Alarm(cell, true, temp));
					} catch(Exception e) {
						var m = e.Message;
					}
				}
			}
			string toparse = string.Empty;
			try {
				var engine = new FileHelperEngine<Alarm>();

				engine.BeforeWriteRecord += (eng, e) => {
					if(!e.Record.COOS || e.Record.Summary.Contains("FREQUENT"))
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
					string temp;
					Alarm al = e.Record;
					if(e.Record.Bearer == "4G")
						temp = e.Record.Element;
					else
						temp = e.Record.RncBsc + " - " + e.Record.Element;
					string tempSite = string.IsNullOrEmpty(e.Record.POC) ? e.Record.Location : e.Record.Location + " - " + e.Record.POC;
					switch (e.Record.Operator) {
						case "VF":
							if(string.IsNullOrEmpty(e.Record.County)) {
								if(string.IsNullOrEmpty(e.Record.Town)) {
									if(!VfLocations.Contains(e.Record.ParentSite.County.ToUpper()))
										VfLocations.Add(e.Record.ParentSite.County.ToUpper());
								}
								else
									if(!VfLocations.Contains(e.Record.Town.ToUpper()))
										VfLocations.Add(e.Record.Town.ToUpper());
							}
							else
								if(!VfLocations.Contains(e.Record.County.ToUpper()))
									VfLocations.Add(e.Record.County.ToUpper());
							if(!VfSites.Contains(tempSite))
								VfSites.Add(tempSite);
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
							if(string.IsNullOrEmpty(e.Record.County)) {
								if(string.IsNullOrEmpty(e.Record.Town)) {
									if(!TefLocations.Contains(e.Record.ParentSite.County.ToUpper()))
										TefLocations.Add(e.Record.ParentSite.County.ToUpper());
								}
								else
									if(!TefLocations.Contains(e.Record.Town.ToUpper()))
										TefLocations.Add(e.Record.Town.ToUpper());
							}
							else
								if(!TefLocations.Contains(e.Record.County.ToUpper()))
									TefLocations.Add(e.Record.County.ToUpper());
							if(!TefSites.Contains(tempSite))
								TefSites.Add(tempSite);
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
			
			generateReports();
			
			LogType = "Outage";
			fullLog = generateFullLog();
		}
		
		public Outage(List<string> Sites) {
			List<Site> sites = Finder.getSites(Sites);
			List<Cell> cells = new List<Cell>();
			foreach(Site site in sites)
				cells.AddRange(site.Cells);
			
			foreach (Cell cell in cells) {
				Site tempSite = sites.Find(s => s.Id == cell.ParentSite);
				string cellString;
				string tempSiteString = cell.ParentSite;
				while(tempSiteString.Length < 5)
					tempSiteString = "0" + tempSiteString;
				tempSiteString = "RBS" + tempSiteString;
				if(cell.Bearer == "4G")
					cellString = cell.Name;
				else
					cellString = cell.BscRnc_Id + " - " + cell.Name;
				switch(cell.Operator) {
					case "VF":
						if(!VfSites.Contains(tempSiteString))
							VfSites.Add(tempSiteString);
						switch(cell.Bearer) {
							case "2G":
								if(!VfGsmCells.Contains(cellString))
									VfGsmCells.Add(cellString);
								break;
							case "3G":
								if(!VfUmtsCells.Contains(cellString))
									VfUmtsCells.Add(cellString);
								break;
							case "4G":
								if(!VfLteCells.Contains(cellString))
									VfLteCells.Add(cellString);
								break;
						}
						if(!VfLocations.Contains(tempSite.Town.ToUpper()))
							VfLocations.Add(tempSite.Town.ToUpper());
						break;
					case "TEF":
						if(!TefSites.Contains(tempSiteString))
							TefSites.Add(tempSiteString);
						switch(cell.Bearer) {
							case "2G":
								if(!TefGsmCells.Contains(cellString))
									TefGsmCells.Add(cellString);
								break;
							case "3G":
								if(!TefUmtsCells.Contains(cellString))
									TefUmtsCells.Add(cellString);
								break;
							case "4G":
								if(!TefLteCells.Contains(cellString))
									TefLteCells.Add(cellString);
								break;
						}
						if(!TefLocations.Contains(tempSite.Town.ToUpper()))
							TefLocations.Add(tempSite.Town.ToUpper());
						break;
				}
			}
						
			showIncludeListForm();
			
			generateReports();
			
			LogType = "Outage";
			fullLog = generateFullLog();
		}
		
		public Outage(Outage existingOutage) {
			Toolbox.Tools.CopyProperties(this, existingOutage);
			LogType = "Outage";
			fullLog = generateFullLog();
		}
		
		public Outage(string[] log, DateTime date) {
			LoadOutageReport(log);
			string[] time = log[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = "Outage";
		}
		
		void generateReports() {
			int cellTotal = VfGsmCells.Count + VfUmtsCells.Count + VfLteCells.Count;
			if(cellTotal > 0) {
				VfLocations.Sort();
				VfSites.Sort();
				VfGsmCells.Sort();
				VfUmtsCells.Sort();
				VfLteCells.Sort();
				VfOutage = cellTotal + "x COOS (" + VfSites.Count;
				Summary = VfOutage += VfSites.Count == 1 ? " Site)" : " Sites)";
				VfOutage += Environment.NewLine + Environment.NewLine + "Locations (" + VfLocations.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine, VfLocations) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine, VfSites);
				
				if ( VfGsmCells.Count > 0 )
					VfOutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + VfGsmCells.Count + ") Event Time - " + VfGsmTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfGsmCells);
				if ( VfUmtsCells.Count > 0)
					VfOutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + VfUmtsCells.Count + ") Event Time - " + VfUmtsTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfUmtsCells);
				if ( VfLteCells.Count > 0)
					VfOutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + VfLteCells.Count + ") Event Time - " + VfLteTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfLteCells);
				
				for(int c = 0;c < VfSites.Count;c++) {
					string[] strToFind = { " - " };
					string tempSite = VfSites[c].Split(strToFind, StringSplitOptions.None)[0];
					tempSite = Convert.ToInt32(tempSite.RemoveLetters()).ToString();
					while(tempSite.Length < 4)
						tempSite = "0" + tempSite;
					VfBulkCI += tempSite + ";";
					if(c > 0 && c % 50 == 0)
						VfBulkCI += Environment.NewLine + Environment.NewLine;
				}
			}
			
			cellTotal = TefGsmCells.Count + TefUmtsCells.Count + TefLteCells.Count;
			if(cellTotal > 0) {
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
					TefOutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + TefLteCells.Count + ") Event Time - " + TefLteTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, TefLteCells);
				
				for(int c = 0;c < TefSites.Count;c++) {
					string[] strToFind = { " - " };
					string tempSite = TefSites[c].Split(strToFind, StringSplitOptions.None)[0];
					tempSite = Convert.ToInt32(tempSite.RemoveLetters()).ToString();
					while(tempSite.Length < 4)
						tempSite = "0" + tempSite;
					TefBulkCI += tempSite + ";";
					if(c > 0 && ((c + 1) % 50) == 0)
						TefBulkCI += Environment.NewLine + Environment.NewLine;
				}
			}
		}
		
		string generateFullLog() {
			string Log = string.Empty;
			if(!string.IsNullOrEmpty(VfOutage)) {
				Log += "----------VF Report----------" + Environment.NewLine;
				Log += VfOutage + Environment.NewLine;
				Log += "-----BulkCI-----" + Environment.NewLine;
				Log += VfBulkCI;
			}
			if(!string.IsNullOrEmpty(TefOutage)) {
				if(!string.IsNullOrEmpty(VfBulkCI))
					Log += Environment.NewLine;
				Log += "----------TF Report----------" + Environment.NewLine;
				Log += TefOutage + Environment.NewLine;
				Log += "-----BulkCI-----" + Environment.NewLine;
				Log += TefBulkCI;
			}
			
			return Log;
		}
		
		public void LoadOutageReport(string[] log) {
			// Manipulate log array to make it compatible with VF/TF new logs
			if(Array.FindIndex(log,element => element.Contains("F Report----------")) == -1) {
				List<string> log2 = log.ToList(); // Create new List with log array values
				string Report = log2[1]; // Store outage report to string
				log2.RemoveAt(1); // Remove outage report previously stored on Report string
				string[] SplitReport = Report.Split('\n'); // Split Report string to new array
				log2.Insert(1,"----------VF Report----------"); // Insert VF Report header to match code checks
				log2.InsertRange(2,SplitReport); // Insert SplitReport array into list after header
				log = log2.ToArray(); // Replace original log array with new generated List values
				if(Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal)) > -1) // Check if log contains LTE sites
					log[Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal))] = "----------LTE sites----------"; // Convert header to match code checks
			}
			string[] strToFind = { " - " };
			int VfReportIndex = Array.FindIndex(log, element => element.Equals("----------VF Report----------", StringComparison.Ordinal));
			int VfBulkCiIndex = Array.FindIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
			int TefReportIndex = Array.FindIndex(log, element => element.Equals("----------TF Report----------", StringComparison.Ordinal));
			int TefBulkCiIndex = Array.FindLastIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
			
			if(VfReportIndex > -1) {
				int VfLocationsIndex = Array.FindIndex(log, element => element.StartsWith("Locations", StringComparison.Ordinal));
				int VfSitesListIndex = Array.FindIndex(log, element => element.Equals("Site List", StringComparison.Ordinal));
				int VfGsmCellsIndex = Array.FindIndex(log, element => element.StartsWith("2G Cells", StringComparison.Ordinal));
				int VfUmtsCellsIndex = Array.FindIndex(log, element => element.StartsWith("3G Cells", StringComparison.Ordinal));
				int VfLteCellsIndex = Array.FindIndex(log, element => element.StartsWith("4G Cells", StringComparison.Ordinal));
				if(VfGsmCellsIndex == -1)
					VfGsmCellsIndex = VfUmtsCellsIndex == -1 ? VfLteCellsIndex : VfUmtsCellsIndex;
				if(VfUmtsCellsIndex == -1)
					VfUmtsCellsIndex = VfLteCellsIndex == -1 ? VfBulkCiIndex : VfLteCellsIndex;
				if(VfLteCellsIndex == -1)
					VfLteCellsIndex = VfBulkCiIndex;
				
				Summary = log[VfReportIndex + 1];
				
				for(int c = VfReportIndex + 1;c < VfBulkCiIndex;c++) {
					VfOutage += log[c];
					if(c < VfBulkCiIndex - 1)
						VfOutage += Environment.NewLine;
					
					if(c > VfLocationsIndex) {
						if(c < VfSitesListIndex) {
							if(!string.IsNullOrEmpty(log[c]))
								VfLocations.Add(log[c]);
						}
						else {
							if(c < VfGsmCellsIndex) {
								if(c > VfSitesListIndex && !string.IsNullOrEmpty(log[c]))
									VfSites.Add(log[c]);
							}
							else {
								if(c > VfGsmCellsIndex) {
//										try { VfGsmTime = Convert.ToDateTime(log[c].Split(strToFind, StringSplitOptions.None)[1]); } catch { }
//									else {
									if(c < VfUmtsCellsIndex) {
										if(!string.IsNullOrEmpty(log[c]))
											VfGsmCells.Add(log[c]);
									}
									else {
										if(c > VfUmtsCellsIndex) {
//												try { VfUmtsTime = Convert.ToDateTime(log[c].Split(strToFind, StringSplitOptions.None)[1]); } catch { }
//											else {
											if(c < VfLteCellsIndex) {
												if(!string.IsNullOrEmpty(log[c]))
													VfUmtsCells.Add(log[c]);
											}
											else {
												if(c == VfLteCellsIndex)
													try { VfLteTime = Convert.ToDateTime(log[c].Split(strToFind, StringSplitOptions.None)[1]); } catch { }
												else {
													if(c < VfBulkCiIndex) {
														if(!string.IsNullOrEmpty(log[c]))
															VfLteCells.Add(log[c]);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				
				try {
					if(log[VfGsmCellsIndex].Contains("2G Cells"))
						VfGsmTime = Convert.ToDateTime(log[VfGsmCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
				} catch { }
				try {
					if(log[VfUmtsCellsIndex].Contains("3G Cells"))
						VfUmtsTime = Convert.ToDateTime(log[VfUmtsCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
				} catch { }
				try {
					if(log[VfLteCellsIndex].Contains("4G Cells"))
						VfLteTime = Convert.ToDateTime(log[VfLteCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
				} catch { }
				
				if(TefReportIndex == -1)
					TefReportIndex = log.Length;
				
				for(int c = VfBulkCiIndex + 1;c < TefReportIndex;c++) {
					VfBulkCI += log[c].Replace("\r", string.Empty).Replace("\n", string.Empty);
					if(c < TefReportIndex - 1)
						VfBulkCI += Environment.NewLine;
				}
			}
			
			if(TefReportIndex == log.Length)
				TefReportIndex = -1;
			
			if(TefReportIndex > -1) {
				int TefLocationsIndex = Array.FindLastIndex(log, element => element.StartsWith("Locations", StringComparison.Ordinal));
				int TefSitesListIndex = Array.FindLastIndex(log, element => element.Equals("Site List", StringComparison.Ordinal));
				int TefGsmCellsIndex = Array.FindLastIndex(log, element => element.StartsWith("2G Cells", StringComparison.Ordinal));
				int TefUmtsCellsIndex = Array.FindLastIndex(log, element => element.StartsWith("3G Cells", StringComparison.Ordinal));
				int TefLteCellsIndex = Array.FindLastIndex(log, element => element.StartsWith("4G Cells", StringComparison.Ordinal));
				if(TefGsmCellsIndex == -1)
					TefGsmCellsIndex = TefUmtsCellsIndex == -1 ? TefLteCellsIndex : TefUmtsCellsIndex;
				if(TefUmtsCellsIndex == -1)
					TefUmtsCellsIndex = TefLteCellsIndex == -1 ? TefBulkCiIndex : TefLteCellsIndex;
				if(TefLteCellsIndex == -1)
					TefLteCellsIndex = TefBulkCiIndex;
				
				if(VfReportIndex == -1)
					Summary = log[TefReportIndex + 1];
				
				for(int c = TefReportIndex + 1;c < TefBulkCiIndex;c++) {
					TefOutage += log[c];
					if(c < TefBulkCiIndex - 1)
						TefOutage += Environment.NewLine;
					
					if(c > TefLocationsIndex) {
						if(c < TefSitesListIndex) {
							if(!string.IsNullOrEmpty(log[c]))
								TefLocations.Add(log[c]);
						}
						else {
							if(c < TefGsmCellsIndex) {
								if(c > TefSitesListIndex && !string.IsNullOrEmpty(log[c]))
									TefSites.Add(log[c]);
							}
							else {
								if(c > TefGsmCellsIndex) {
//									try { TefGsmTime = Convert.ToDateTime(log[c].Split(strToFind, StringSplitOptions.None)[1]); } catch { }
//								else {
									if(c < TefUmtsCellsIndex) {
										if(!string.IsNullOrEmpty(log[c]))
											TefGsmCells.Add(log[c]);
									}
									else {
										if(c > TefUmtsCellsIndex) {
//											try { TefUmtsTime = Convert.ToDateTime(log[c].Split(strToFind, StringSplitOptions.None)[1]); } catch { }
//										else {
											if(c < TefLteCellsIndex) {
												if(!string.IsNullOrEmpty(log[c]))
													TefUmtsCells.Add(log[c]);
											}
											else {
												if(c > TefLteCellsIndex) {
//													try { TefLteTime = Convert.ToDateTime(log[c].Split(strToFind, StringSplitOptions.None)[1]); } catch { }
//												else {
													if(c < TefBulkCiIndex) {
														if(!string.IsNullOrEmpty(log[c]))
															TefLteCells.Add(log[c]);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				
				try {
					if(log[TefGsmCellsIndex].Contains("2G Cells"))
						TefGsmTime = Convert.ToDateTime(log[TefGsmCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
				} catch { }
				try {
					if(log[TefUmtsCellsIndex].Contains("3G Cells"))
						TefUmtsTime = Convert.ToDateTime(log[TefUmtsCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
				} catch { }
				try {
					if(log[TefLteCellsIndex].Contains("4G Cells"))
						TefLteTime = Convert.ToDateTime(log[TefLteCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
				} catch { }
				
				for(int c = TefBulkCiIndex + 1;c < log.Length;c++) {
					TefBulkCI += log[c].Replace("\r", string.Empty).Replace("\n", string.Empty);
					if(c < log.Length - 1)
						TefBulkCI += Environment.NewLine;
				}
			}
		}

		void showIncludeListForm() {
			List<string[]> includeList = new List<string[]>();
			Form form = new Form();
			using (form) {
				// 
				// cb2G
				// 
				CheckBox cb2G = new CheckBox();
				cb2G.Location = new System.Drawing.Point(3, 34);
				cb2G.Name = "cb2G";
				cb2G.Size = new System.Drawing.Size(42, 20);
				cb2G.TabIndex = 0;
				cb2G.Text = "2G";
				cb2G.Enabled = VfGsmCells.Any() || TefGsmCells.Any();
				cb2G.CheckedChanged += IncludeListForm_cbCheckedChanged;
				// 
				// cb3G
				// 
				CheckBox cb3G = new CheckBox();
				cb3G.Location = new System.Drawing.Point(3, 60);
				cb3G.Name = "cb3G";
				cb3G.Size = new System.Drawing.Size(42, 20);
				cb3G.TabIndex = 2;
				cb3G.Text = "3G";
				cb3G.Enabled = VfUmtsCells.Any() || TefUmtsCells.Any();
				cb3G.CheckedChanged += IncludeListForm_cbCheckedChanged;
				// 
				// cb4G
				// 
				CheckBox cb4G = new CheckBox();
				cb4G.Location = new System.Drawing.Point(3, 86);
				cb4G.Name = "cb4G";
				cb4G.Size = new System.Drawing.Size(42, 20);
				cb4G.TabIndex = 4;
				cb4G.Text = "4G";
				cb4G.Enabled = VfLteCells.Any() || TefLteCells.Any();
				cb4G.CheckedChanged += IncludeListForm_cbCheckedChanged;
				// 
				// continueButton
				// 
				Button continueButton = new Button();
				continueButton.Location = new System.Drawing.Point(3, 112);
				continueButton.Name = "continueButton";
				continueButton.Size = new System.Drawing.Size(221, 23);
				continueButton.TabIndex = 6;
				continueButton.Text = "Continue";
				continueButton.Click += IncludeListForm_buttonClick;
				// 
				// dtp2G
				// 
				DateTimePicker dtp2G = new DateTimePicker();
				dtp2G.Checked = false;
				dtp2G.CustomFormat = "dd/MM/yyyy HH:mm";
				dtp2G.Format = DateTimePickerFormat.Custom;
				dtp2G.Location = new System.Drawing.Point(51, 34);
				dtp2G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp2G.Name = "dtp2G";
				dtp2G.Size = new System.Drawing.Size(173, 20);
				dtp2G.TabIndex = 1;
				dtp2G.Value = DateTime.Now;
				dtp2G.Visible = false;
				// 
				// dtp3G
				// 
				DateTimePicker dtp3G = new DateTimePicker();
				dtp3G.Checked = false;
				dtp3G.CustomFormat = "dd/MM/yyyy HH:mm";
				dtp3G.Format = DateTimePickerFormat.Custom;
				dtp3G.Location = new System.Drawing.Point(51, 60);
				dtp3G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp3G.Name = "dtp3G";
				dtp3G.Size = new System.Drawing.Size(173, 20);
				dtp3G.TabIndex = 3;
				dtp3G.Value = DateTime.Now;
				dtp3G.Visible = false;
				// 
				// dtp4G
				// 
				DateTimePicker dtp4G = new DateTimePicker();
				dtp4G.Checked = false;
				dtp4G.CustomFormat = "dd/MM/yyyy HH:mm";
				dtp4G.Format = DateTimePickerFormat.Custom;
				dtp4G.Location = new System.Drawing.Point(51, 86);
				dtp4G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp4G.Name = "dtp4G";
				dtp4G.Size = new System.Drawing.Size(173, 20);
				dtp4G.TabIndex = 5;
				dtp4G.Value = DateTime.Now;
				dtp4G.Visible = false;
				// 
				// IncludeListForm_label
				// 
				Label IncludeListForm_label = new Label();
				IncludeListForm_label.Location = new System.Drawing.Point(3, 2);
				IncludeListForm_label.Name = "label";
				IncludeListForm_label.Size = new System.Drawing.Size(221, 29);
				IncludeListForm_label.Text = "Which Technologies do you wish to include?";
				IncludeListForm_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
				// 
				// Form1
				// 
				form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				form.AutoScaleMode = AutoScaleMode.Font;
				form.ClientSize = new System.Drawing.Size(228, 137);
				form.Icon = appCore.UI.Resources.MB_0001_vodafone3;
//				form.MaximizeBox = false;
				form.ControlBox = false;
				form.FormBorderStyle = FormBorderStyle.FixedSingle;
				form.Controls.Add(IncludeListForm_label);
				form.Controls.Add(dtp4G);
				form.Controls.Add(dtp3G);
				form.Controls.Add(dtp2G);
				form.Controls.Add(continueButton);
				form.Controls.Add(cb4G);
				form.Controls.Add(cb3G);
				form.Controls.Add(cb2G);
				form.Name = "IncludeListForm";
				form.Text = "Generate Outage Report";
				form.ShowDialog();
				
				if(cb2G.Checked)
					VfGsmTime = TefGsmTime = dtp2G.Value;
				else {
					VfGsmCells.Clear();
					TefGsmCells.Clear();
				}
				if(cb3G.Checked)
					VfUmtsTime = TefUmtsTime = dtp3G.Value;
				else {
					VfUmtsCells.Clear();
					TefUmtsCells.Clear();
				}
				if(cb4G.Checked)
					VfLteTime = TefLteTime = dtp4G.Value;
				else {
					VfLteCells.Clear();
					TefLteCells.Clear();
				}
			}
		}

		void IncludeListForm_cbCheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)sender;
			Form form = (Form)cb.Parent;
			
			switch(cb.Name) {
				case "cb2G":
					foreach (Control ctrl in form.Controls) {
						if(ctrl.Name == "dtp2G") {
							ctrl.Visible = cb.Checked;
							break;
						}
					}
					break;
				case "cb3G":
					foreach (Control ctrl in form.Controls) {
						if(ctrl.Name == "dtp3G") {
							ctrl.Visible = cb.Checked;
							break;
						}
					}
					break;
				default:
					foreach (Control ctrl in form.Controls) {
						if(ctrl.Name == "dtp4G") {
							ctrl.Visible = cb.Checked;
							break;
						}
					}
					break;
			}
		}

		void IncludeListForm_buttonClick(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			Form form = (Form)btn.Parent;
			if(btn.Text == "Cancel")
				form.Controls["sitesList_tb"].Text = string.Empty;
			form.Close();
		}
		
		public override string ToString()
		{
			if(string.IsNullOrEmpty(fullLog))
				fullLog = generateFullLog();
			
			return fullLog;
		}
	}
}
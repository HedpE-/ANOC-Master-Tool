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
		
		List<Site> affectedSites = new List<Site>();
		public List<Site> AffectedSites {
			get {
				if(affectedSites.Count == 0) {
					List<string> list = new List<string>();
					foreach(string siteStr in VfSites) {
						string tempSite = string.Empty;
						if(siteStr.Contains(" - "))
							tempSite = siteStr.Split(new []{ " - " }, StringSplitOptions.None)[0];
						else
							tempSite = siteStr.Split(new []{ " (" }, StringSplitOptions.None)[0];
						tempSite = Convert.ToInt32(tempSite.RemoveLetters()).ToString();
						list.Add(tempSite);
					}
					foreach(string siteStr in TefSites) {
						string tempSite = string.Empty;
						if(siteStr.Contains(" - "))
							tempSite = siteStr.Split(new []{ " - " }, StringSplitOptions.None)[0];
						else
							tempSite = siteStr.Split(new []{ " (" }, StringSplitOptions.None)[0];
						tempSite = Convert.ToInt32(tempSite.RemoveLetters()).ToString();
						list.Add(tempSite);
					}
					list = list.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
					affectedSites = DB.SitesDB.getSites(list);
				}
				
				return affectedSites;
			}
			private set { affectedSites = value; }
		}
		List<Cell> affectedCells = new List<Cell>();
		public List<Cell> AffectedCells {
			get {
				if(affectedCells.Count == 0) {
					List<string> list = new List<string>();
					list.AddRange(VfGsmCells);
					list.AddRange(VfUmtsCells);
					list.AddRange(VfLteCells);
					list.AddRange(TefGsmCells);
					list.AddRange(TefUmtsCells);
					list.AddRange(TefLteCells);
					for(int c = 0;c < list.Count;c++) {
						string[] temp = { " - " };
						temp = list[c].Split(temp, StringSplitOptions.None);
						list[c] = temp.Length > 1 ? temp[1] : temp[0];
					}

					affectedCells = DB.SitesDB.getCells(list);
				}
				
				return affectedCells;
			}
			private set
            {
                affectedCells = value;
            }
        }
        List<string> affectedLocations = new List<string>();
        public List<string> AffectedLocations
        {
            get
            {
                if (affectedLocations.Count == 0)
                {
                    foreach (string loc in VfLocations)
                    {
                        if (!affectedLocations.Contains(loc, StringComparison.OrdinalIgnoreCase))
                            affectedLocations.Add(loc);
                    }
                    foreach (string loc in TefLocations)
                    {
                        if (!affectedLocations.Contains(loc, StringComparison.OrdinalIgnoreCase))
                            affectedLocations.Add(loc);
                    }
                    affectedLocations = affectedLocations.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
                }

                return affectedLocations;
            }
            private set { affectedLocations = value; }
        }

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
		
		List<string> VfGsmAffectedSites
        {
            get
            {
                var list = AffectedCells.Filter(Cell.Filters.VF_2G);
                var sites = list.Select(c => c.ParentSite).Distinct().ToList();
                for(int c = 0;c < sites.Count;c++)
                {
                    while (sites[c].Length < 5)
                        sites[c] = "0" + sites[c];
                    sites[c] = "RBS" + sites[c];
                }
                return sites;
            }
        }
        List<string> VfUmtsAffectedSites
        {
            get
            {
                var list = AffectedCells.Filter(Cell.Filters.VF_3G);
                var sites = list.Select(c => c.ParentSite).Distinct().ToList();
                for (int c = 0; c < sites.Count; c++)
                {
                    while (sites[c].Length < 5)
                        sites[c] = "0" + sites[c];
                    sites[c] = "RBS" + sites[c];
                }
                return sites;
            }
        }
        List<string> VfLteAffectedSites
        {
            get
            {
                var list = AffectedCells.Filter(Cell.Filters.VF_4G);
                var sites = list.Select(c => c.ParentSite).Distinct().ToList();
                for (int c = 0; c < sites.Count; c++)
                {
                    while (sites[c].Length < 5)
                        sites[c] = "0" + sites[c];
                    sites[c] = "RBS" + sites[c];
                }
                return sites;
            }
        }

        List<string> _VfGsmOnlyAffectedSites;
        public List<string> VfGsmOnlyAffectedSites
        {
            get
            {
                if (_VfGsmOnlyAffectedSites == null)
                {
                    var t = VfGsmAffectedSites.Except(VfUmtsAffectedSites).ToList();
                    _VfGsmOnlyAffectedSites = t.Except(VfLteAffectedSites).ToList();
                }
                return _VfGsmOnlyAffectedSites;
            }
        }
        List<string> _VfGsmUmtsAffectedSites;
        public List<string> VfGsmUmtsAffectedSites
        {
            get
            {
                if(_VfGsmUmtsAffectedSites == null)
                {
                    var t = VfGsmAffectedSites.Intersect(VfUmtsAffectedSites).ToList();
                    _VfGsmUmtsAffectedSites = t.Except(VfLteAffectedSites).ToList();
                }
                return _VfGsmUmtsAffectedSites;
            }
        }
        List<string> _VfGsmLteAffectedSites;
        public List<string> VfGsmLteAffectedSites
        {
            get
            {
                if (_VfGsmLteAffectedSites == null)
                {
                    var t = VfGsmAffectedSites.Intersect(VfLteAffectedSites).ToList();
                    _VfGsmLteAffectedSites = t.Except(VfUmtsAffectedSites).ToList();
                }
                return _VfGsmLteAffectedSites;
            }
        }
        List<string> _VfGsmUmtsLteAffectedSites;
        public List<string> VfGsmUmtsLteAffectedSites
        {
            get
            {
                if (_VfGsmUmtsLteAffectedSites == null)
                {
                    var t = VfGsmAffectedSites.Intersect(VfUmtsAffectedSites).ToList();
                    _VfGsmUmtsLteAffectedSites = t.Intersect(VfLteAffectedSites).ToList();
                }
                return _VfGsmUmtsLteAffectedSites;
            }
        }
        List<string> _VfUmtsOnlyAffectedSites;
        public List<string> VfUmtsOnlyAffectedSites
        {
            get
            {
                if (_VfUmtsOnlyAffectedSites == null)
                {
                    var t = VfUmtsAffectedSites.Except(VfGsmAffectedSites).ToList();
                    _VfUmtsOnlyAffectedSites = t.Except(VfLteAffectedSites).ToList();
                }
                return _VfUmtsOnlyAffectedSites;
            }
        }
        List<string> _VfUmtsLteAffectedSites;
        public List<string> VfUmtsLteAffectedSites
        {
            get
            {
                if (_VfUmtsLteAffectedSites == null)
                {
                    var t = VfUmtsAffectedSites.Intersect(VfLteAffectedSites).ToList();
                    _VfUmtsLteAffectedSites = t.Except(VfGsmAffectedSites).ToList();
                }
                return _VfUmtsLteAffectedSites;
            }
        }
        List<string> _VfLteOnlyAffectedSites;
        public List<string> VfLteOnlyAffectedSites
        {
            get
            {
                if (_VfLteOnlyAffectedSites == null)
                {
                    var t = VfLteAffectedSites.Except(VfGsmAffectedSites).ToList();
                    _VfLteOnlyAffectedSites = t.Except(VfUmtsAffectedSites).ToList();
                }
                return _VfLteOnlyAffectedSites;
            }
        }

        public List<string> TefGsmAffectedSites
        {
            get
            {
                var list = AffectedCells.Filter(Cell.Filters.TF_2G);
                var sites = list.Select(c => c.ParentSite).Distinct().ToList();
                for (int c = 0; c < sites.Count; c++)
                {
                    while (sites[c].Length < 5)
                        sites[c] = "0" + sites[c];
                    sites[c] = "RBS" + sites[c];
                }
                return sites;
            }
        }
        public List<string> TefUmtsAffectedSites
        {
            get
            {
                var list = AffectedCells.Filter(Cell.Filters.TF_3G);
                var sites = list.Select(c => c.ParentSite).Distinct().ToList();
                for (int c = 0; c < sites.Count; c++)
                {
                    while (sites[c].Length < 5)
                        sites[c] = "0" + sites[c];
                    sites[c] = "RBS" + sites[c];
                }
                return sites;
            }
        }
        public List<string> TefLteAffectedSites
        {
            get
            {
                var list = AffectedCells.Filter(Cell.Filters.TF_4G);
                var sites = list.Select(c => c.ParentSite).Distinct().ToList();
                for (int c = 0; c < sites.Count; c++)
                {
                    while (sites[c].Length < 5)
                        sites[c] = "0" + sites[c];
                    sites[c] = "RBS" + sites[c];
                }
                return sites;
            }
        }

        List<string> _TefGsmOnlyAffectedSites;
        public List<string> TefGsmOnlyAffectedSites
        {
            get
            {
                if (_TefGsmOnlyAffectedSites == null)
                {
                    var t = TefGsmAffectedSites.Except(TefUmtsAffectedSites).ToList();
                    _TefGsmOnlyAffectedSites = t.Except(TefLteAffectedSites).ToList();
                }
                return _TefGsmOnlyAffectedSites;
            }
        }
        List<string> _TefGsmUmtsAffectedSites;
        public List<string> TefGsmUmtsAffectedSites
        {
            get
            {
                if (_TefGsmUmtsAffectedSites == null)
                {
                    var t = TefGsmAffectedSites.Intersect(TefUmtsAffectedSites).ToList();
                    _TefGsmUmtsAffectedSites = t.Except(TefLteAffectedSites).ToList();
                }
                return _TefGsmUmtsAffectedSites;
            }
        }
        List<string> _TefGsmLteAffectedSites;
        public List<string> TefGsmLteAffectedSites
        {
            get
            {
                if (_TefGsmLteAffectedSites == null)
                {
                    var t = TefGsmAffectedSites.Intersect(TefLteAffectedSites).ToList();
                    _TefGsmLteAffectedSites = t.Except(TefUmtsAffectedSites).ToList();
                }
                return _TefGsmLteAffectedSites;
            }
        }
        List<string> _TefGsmUmtsLteAffectedSites;
        public List<string> TefGsmUmtsLteAffectedSites
        {
            get
            {
                if (_TefGsmUmtsLteAffectedSites == null)
                {
                    var t = TefGsmAffectedSites.Intersect(TefUmtsAffectedSites).ToList();
                    _TefGsmUmtsLteAffectedSites = t.Intersect(TefLteAffectedSites).ToList();
                }
                return _TefGsmUmtsLteAffectedSites;
            }
        }
        List<string> _TefUmtsOnlyAffectedSites;
        public List<string> TefUmtsOnlyAffectedSites
        {
            get
            {
                if (_TefUmtsOnlyAffectedSites == null)
                {
                    var t = TefUmtsAffectedSites.Except(TefGsmAffectedSites).ToList();
                    _TefUmtsOnlyAffectedSites = t.Except(TefLteAffectedSites).ToList();
                }
                return _TefUmtsOnlyAffectedSites;
            }
        }
        List<string> _TefUmtsLteAffectedSites;
        public List<string> TefUmtsLteAffectedSites
        {
            get
            {
                if (_TefUmtsLteAffectedSites == null)
                {
                    var t = TefUmtsAffectedSites.Intersect(TefLteAffectedSites).ToList();
                    _TefUmtsLteAffectedSites = t.Except(TefGsmAffectedSites).ToList();
                }
                return _TefUmtsLteAffectedSites;
            }
        }
        List<string> _TefLteOnlyAffectedSites;
        public List<string> TefLteOnlyAffectedSites
        {
            get
            {
                if (_TefLteOnlyAffectedSites == null)
                {
                    var t = TefLteAffectedSites.Except(TefGsmAffectedSites).ToList();
                    _TefLteOnlyAffectedSites = t.Except(TefUmtsAffectedSites).ToList();
                }
                return _TefLteOnlyAffectedSites;
            }
        }

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
					List<string> sites = VfBulkCI.Split(';').ToList();
					sites.AddRange(TefBulkCI.Split(';'));
					for(int c = 0;c < sites.Count;c++) {
						sites[c] = sites[c].RemoveLetters();
                        while (sites[c].StartsWith("0"))
                            sites[c] = sites[c].Substring(1);
                    }
                    sites = sites.Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList();
                    sites.Sort();
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
				LTEcells = DB.SitesDB.getCells(alarms.lteSitesOnM, Bearers.LTE);
				
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
					if(e.Record.Bearer == Bearers.LTE)
						temp = e.Record.Element;
					else
						temp = e.Record.RncBsc + " - " + e.Record.Element;
					string tempSite = string.IsNullOrEmpty(e.Record.POC) ? e.Record.Location : e.Record.Location + " - " + e.Record.POC;
					switch (e.Record.Operator) {
						case Operators.Vodafone:
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
								case Bearers.GSM:
									if(!VfGsmCells.Contains(temp))
										VfGsmCells.Add(temp);
									if(e.Record.LastOccurrence < VfGsmTime)
										VfGsmTime = e.Record.LastOccurrence;
									break;
								case Bearers.UMTS:
									if(!VfUmtsCells.Contains(temp))
										VfUmtsCells.Add(temp);
									if(e.Record.LastOccurrence < VfUmtsTime)
										VfUmtsTime = e.Record.LastOccurrence;
									break;
								case Bearers.LTE:
									if(!VfLteCells.Contains(temp))
										VfLteCells.Add(temp);
									if(e.Record.LastOccurrence < VfLteTime)
										VfLteTime = e.Record.LastOccurrence;
									break;
							}
							break;
						case Operators.Telefonica:
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
								case Bearers.GSM:
									if(!TefGsmCells.Contains(temp))
										TefGsmCells.Add(temp);
									if(e.Record.LastOccurrence < TefGsmTime)
										TefGsmTime = e.Record.LastOccurrence;
									break;
								case Bearers.UMTS:
									if(!TefUmtsCells.Contains(temp))
										TefUmtsCells.Add(temp);
									if(e.Record.LastOccurrence < TefUmtsTime)
										TefUmtsTime = e.Record.LastOccurrence;
									break;
								case Bearers.LTE:
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
			
			LogType = TemplateTypes.Outage;
			fullLog = generateFullLog();
		}
		
		public Outage(List<string> Sites) {
			AffectedSites = DB.SitesDB.getSites(Sites);
			AffectedCells = new List<Cell>();
			foreach(Site site in AffectedSites)
				AffectedCells.AddRange(site.Cells);
			
			foreach (Cell cell in AffectedCells) {
				Site tempSite = AffectedSites.Find(s => s.Id == cell.ParentSite);
				string cellString;
				string tempSiteString = cell.ParentSite;
				while(tempSiteString.Length < 5)
					tempSiteString = "0" + tempSiteString;
				tempSiteString = "RBS" + tempSiteString;
				if(cell.Bearer == Bearers.GSM)
					cellString = cell.Name;
				else
					cellString = cell.BscRnc_Id + " - " + cell.Name;
				switch(cell.Operator) {
					case Operators.Vodafone:
						if(!VfSites.Contains(tempSiteString))
							VfSites.Add(tempSiteString);
						switch(cell.Bearer) {
							case Bearers.GSM:
								if(!VfGsmCells.Contains(cellString))
									VfGsmCells.Add(cellString);
								break;
							case Bearers.UMTS:
								if(!VfUmtsCells.Contains(cellString))
									VfUmtsCells.Add(cellString);
								break;
							case Bearers.LTE:
								if(!VfLteCells.Contains(cellString))
									VfLteCells.Add(cellString);
								break;
						}
						if(!VfLocations.Contains(tempSite.Town.ToUpper()))
							VfLocations.Add(tempSite.Town.ToUpper());
						break;
					case Operators.Telefonica:
						if(!TefSites.Contains(tempSiteString))
							TefSites.Add(tempSiteString);
						switch(cell.Bearer) {
							case Bearers.GSM:
								if(!TefGsmCells.Contains(cellString))
									TefGsmCells.Add(cellString);
								break;
							case Bearers.UMTS:
								if(!TefUmtsCells.Contains(cellString))
									TefUmtsCells.Add(cellString);
								break;
							case Bearers.LTE:
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
			
			LogType = TemplateTypes.Outage;
			fullLog = generateFullLog();
		}
		
		public Outage(Outage existingOutage) {
			Toolbox.Tools.CopyProperties(this, existingOutage);
			LogType = TemplateTypes.Outage;
			fullLog = generateFullLog();
		}
		
		public Outage(string[] log, DateTime date) {
			LoadOutageReport(log);
			string[] time = log[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = TemplateTypes.Outage;
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
				VfOutage += Environment.NewLine + Environment.NewLine + "Locations (" + VfLocations.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine, VfLocations) + Environment.NewLine + Environment.NewLine + "Site List";
				
				if(AffectedSites.Count > 0) {
					foreach(string siteStr in VfSites) {
						VfOutage += Environment.NewLine;
						string[] siteArr = siteStr.Split(new[]{ " - " }, StringSplitOptions.None);
						siteArr[0] = siteArr[0].RemoveLetters();
						while(siteArr[0].StartsWith("0"))
							siteArr[0] = siteArr[0].Substring(1);
						Site site = AffectedSites.Find(s => s.Id == siteArr[0]);
						List<Cell> cells = AffectedCells.FindAll(c => c.ParentSite == siteArr[0]);
						VfOutage += siteStr + " (";
						if(site.Cells.Filter(Cell.Filters.VF_2G).Count > 0) {
							//if(VfGsmAffectedSites == null)
							//	VfGsmAffectedSites = new List<string>();
							//VfGsmAffectedSites.Add(site.FullId);
							VfOutage += "2G: ";
							List<Cell> outageCells = cells.Filter(Cell.Filters.VF_2G);
							if(outageCells.Count > 0)
								VfOutage += outageCells.Count > 9 ? outageCells.Count.ToString() : "0" + outageCells.Count;
							else
								VfOutage += "UP";
						}
						if(site.Cells.Filter(Cell.Filters.VF_3G).Count > 0) {
							//if(VfUmtsAffectedSites == null)
							//	VfUmtsAffectedSites = new List<string>();
							//VfUmtsAffectedSites.Add(site.FullId);
							if(!VfOutage.EndsWith("("))
								VfOutage += " | ";
							VfOutage += "3G: ";
							List<Cell> outageCells = cells.Filter(Cell.Filters.VF_3G);
							if(outageCells.Count > 0)
								VfOutage += outageCells.Count > 9 ? outageCells.Count.ToString() : "0" + outageCells.Count;
							else
								VfOutage += "UP";
						}
						if(site.Cells.Filter(Cell.Filters.VF_4G).Count > 0) {
							//if(VfLteAffectedSites == null)
							//	VfLteAffectedSites = new List<string>();
							//VfLteAffectedSites.Add(site.FullId);
							if(!VfOutage.EndsWith("("))
								VfOutage += " | ";
							VfOutage += "4G: ";
							List<Cell> outageCells = cells.Filter(Cell.Filters.VF_4G);
							if(outageCells.Count > 0)
								VfOutage += outageCells.Count > 9 ? outageCells.Count.ToString() : "0" + outageCells.Count;
							else
								VfOutage += "UP";
						}
						VfOutage += ")";
					}
                    //foreach(string site in VfGsmAffectedSites)
                    //{
                    //    List<string> affectedTechs = new List<string>();
                    //    if(VfUmtsAffectedSites != null)
                    //    {
                    //        if (VfUmtsAffectedSites.Contains(site))
                    //            affectedTechs.Add("3G");
                    //    }
                    //    if(VfLteAffectedSites != null)
                    //    {
                    //        if (VfLteAffectedSites.Contains(site))
                    //            affectedTechs.Add("4G");
                    //    }

                    //    if(affectedTechs.Count > 0)
                    //    {
                    //        if (affectedTechs.Count == 2)
                    //        {
                    //            if (VfGsmUmtsLteAffectedSites == null)
                    //                VfGsmUmtsLteAffectedSites = new List<string>();
                    //            VfGsmUmtsLteAffectedSites.Add(site);
                    //        }
                    //        else
                    //        {
                    //            if (affectedTechs[0] == "3G")
                    //            {
                    //                if (VfGsmUmtsAffectedSites == null)
                    //                    VfGsmUmtsAffectedSites = new List<string>();
                    //                VfGsmUmtsAffectedSites.Add(site);
                    //            }
                    //            else
                    //            {
                    //                if (VfGsmLteAffectedSites == null)
                    //                    VfGsmLteAffectedSites = new List<string>();
                    //                VfGsmLteAffectedSites.Add(site);
                    //            }
                    //        }
                    //    }
                    //}
				}
				else
					VfOutage += Environment.NewLine + string.Join(Environment.NewLine, VfSites);
				
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
				TefOutage += Environment.NewLine + Environment.NewLine + "Locations (" + TefLocations.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine, TefLocations) + Environment.NewLine + Environment.NewLine + "Site List";
				
				if(AffectedSites.Count > 0) {
					foreach(string siteStr in TefSites) {
						TefOutage += Environment.NewLine;
						string[] siteArr = siteStr.Split(new[]{ " - " }, StringSplitOptions.None);
						siteArr[0] = siteArr[0].RemoveLetters();
						while(siteArr[0].StartsWith("0"))
							siteArr[0] = siteArr[0].Substring(1);
						Site site = AffectedSites.Find(s => s.Id == siteArr[0]);
						List<Cell> cells = AffectedCells.FindAll(c => c.ParentSite == siteArr[0]);
						TefOutage += siteStr + " (";
						if(site.Cells.Filter(Cell.Filters.TF_2G).Count > 0) {
							//if(TefGsmAffectedSites == null)
							//	TefGsmAffectedSites = new List<string>();
							//TefGsmAffectedSites.Add(site.FullId);
							TefOutage += "2G: ";
							List<Cell> outageCells = cells.Filter(Cell.Filters.TF_2G);
							if(outageCells.Count > 0)
								TefOutage += outageCells.Count > 9 ? outageCells.Count.ToString() : "0" + outageCells.Count;
							else
								TefOutage += "UP";
						}
						if(site.Cells.Filter(Cell.Filters.TF_3G).Count > 0) {
							//if(TefUmtsAffectedSites == null)
							//	TefUmtsAffectedSites = new List<string>();
							//TefUmtsAffectedSites.Add(site.FullId);
							if(!TefOutage.EndsWith("("))
								TefOutage += " | ";
							TefOutage += "3G: ";
							List<Cell> outageCells = cells.Filter(Cell.Filters.TF_3G);
							if(outageCells.Count > 0)
								TefOutage += outageCells.Count > 9 ? outageCells.Count.ToString() : "0" + outageCells.Count;
							else
								TefOutage += "UP";
						}
						if(site.Cells.Filter(Cell.Filters.TF_4G).Count > 0) {
							//if(TefLteAffectedSites == null)
							//	TefLteAffectedSites = new List<string>();
							//TefLteAffectedSites.Add(site.FullId);
							if(!TefOutage.EndsWith("("))
								TefOutage += " | ";
							TefOutage += "4G: ";
							List<Cell> outageCells = cells.Filter(Cell.Filters.TF_4G);
							if(outageCells.Count > 0)
								TefOutage += outageCells.Count > 9 ? outageCells.Count.ToString() : "0" + outageCells.Count;
							else
								TefOutage += "UP";
						}
						TefOutage += ")";
                    }
                    //foreach (string site in TefGsmAffectedSites)
                    //{
                    //    List<string> affectedTechs = new List<string>();
                    //    if (TefUmtsAffectedSites != null)
                    //    {
                    //        if (TefUmtsAffectedSites.Contains(site))
                    //            affectedTechs.Add("3G");
                    //    }
                    //    if (TefLteAffectedSites != null)
                    //    {
                    //        if (TefLteAffectedSites.Contains(site))
                    //            affectedTechs.Add(site);
                    //    }

                    //    if (affectedTechs.Count > 0)
                    //    {
                    //        if (affectedTechs.Count == 2)
                    //        {
                    //            if (TefGsmUmtsLteAffectedSites == null)
                    //                TefGsmUmtsLteAffectedSites = new List<string>();
                    //            TefGsmUmtsLteAffectedSites.Add(site);
                    //        }
                    //        else
                    //        {
                    //            if (affectedTechs[0] == "3G")
                    //            {
                    //                if (TefGsmUmtsAffectedSites == null)
                    //                    TefGsmUmtsAffectedSites = new List<string>();
                    //                TefGsmUmtsAffectedSites.Add(site);
                    //            }
                    //            else
                    //            {
                    //                if (TefGsmLteAffectedSites == null)
                    //                    TefGsmLteAffectedSites = new List<string>();
                    //                TefGsmLteAffectedSites.Add(site);
                    //            }
                    //        }
                    //    }
                    //}
                }
				else
					TefOutage += Environment.NewLine + string.Join(Environment.NewLine, TefSites);
				
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

            Summary = log[VfReportIndex + 1];
            VfOutage = string.Join(Environment.NewLine, log.Slice(VfReportIndex + 1, VfBulkCiIndex));
            VfBulkCI = string.Join(Environment.NewLine, log.Slice(VfBulkCiIndex + 1, (TefReportIndex > -1 ? TefReportIndex : log.Length)));
            if(TefReportIndex > -1)
            {
                TefOutage = string.Join(Environment.NewLine, log.Slice(TefReportIndex + 1, TefBulkCiIndex));
                TefBulkCI = string.Join(Environment.NewLine, log.Slice(TefBulkCiIndex + 1, log.Length));
            }

            if (VfReportIndex > -1) {
				int VfLocationsIndex = Array.FindIndex(log, element => element.StartsWith("Locations", StringComparison.Ordinal));
				int VfSitesListIndex = Array.FindIndex(log, element => element.Equals("Site List", StringComparison.Ordinal));
				int VfGsmCellsIndex = Array.FindIndex(log, element => element.StartsWith("2G Cells", StringComparison.Ordinal));
				int VfUmtsCellsIndex = Array.FindIndex(log, element => element.StartsWith("3G Cells", StringComparison.Ordinal));
				int VfLteCellsIndex = Array.FindIndex(log, element => element.StartsWith("4G Cells", StringComparison.Ordinal));

                VfLocations = log.Slice(VfLocationsIndex + 1, VfSitesListIndex - 1).ToList();

                int endIndex = VfGsmCellsIndex > -1 ? VfGsmCellsIndex - 1 : (VfUmtsCellsIndex > -1 ? VfUmtsCellsIndex - 1 : (VfLteCellsIndex > -1 ? VfLteCellsIndex - 1 : VfBulkCiIndex));
                VfSites = log.Slice(VfSitesListIndex, endIndex)
                    .Select(s => s
                        .Split(new[] { " (" }, StringSplitOptions.None)[0]
                        .Split(strToFind, StringSplitOptions.None)[0])
                    .ToList();

                if (VfGsmCellsIndex > -1)
                {
                    endIndex = VfUmtsCellsIndex > -1 ? VfUmtsCellsIndex - 1 : (VfLteCellsIndex > -1 ? VfLteCellsIndex - 1 : VfBulkCiIndex);
                    VfGsmCells = log.Slice(VfGsmCellsIndex + 1, endIndex)
                        .Select(c => c.Split(strToFind, StringSplitOptions.None)[1])
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList();
                }
                if (VfUmtsCellsIndex > -1)
                {
                    endIndex = VfLteCellsIndex > -1 ? VfLteCellsIndex - 1 : VfBulkCiIndex;
                    VfUmtsCells = log.Slice(VfUmtsCellsIndex + 1, endIndex)
                        .Select(c => c.Split(strToFind, StringSplitOptions.None)[1])
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList();
                }
                if (VfLteCellsIndex > -1)
                    VfLteCells = log.Slice(VfLteCellsIndex + 1, VfBulkCiIndex).ToList();

                try
                {
                    if (VfGsmCellsIndex > -1)
                        VfGsmTime = Convert.ToDateTime(log[VfGsmCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
                } catch { }
                try
                {
                    if (VfUmtsCellsIndex > -1)
                        VfUmtsTime = Convert.ToDateTime(log[VfUmtsCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
                } catch { }
                try
                {
                    if (VfLteCellsIndex > -1)
                        VfLteTime = Convert.ToDateTime(log[VfLteCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
                } catch { }
            }
			
			//if(TefReportIndex == log.Length)
			//	TefReportIndex = -1;
			
			if(TefReportIndex > -1) {
                var tempLog = log.Slice(TefReportIndex, TefBulkCiIndex);
				int TefLocationsIndex = Array.FindLastIndex(tempLog, element => element.StartsWith("Locations", StringComparison.Ordinal));
				int TefSitesListIndex = Array.FindLastIndex(tempLog, element => element.Equals("Site List", StringComparison.Ordinal));
				int TefGsmCellsIndex = Array.FindLastIndex(tempLog, element => element.StartsWith("2G Cells", StringComparison.Ordinal));
				int TefUmtsCellsIndex = Array.FindLastIndex(tempLog, element => element.StartsWith("3G Cells", StringComparison.Ordinal));
				int TefLteCellsIndex = Array.FindLastIndex(tempLog, element => element.StartsWith("4G Cells", StringComparison.Ordinal));

                TefLocations = tempLog.Slice(TefLocationsIndex + 1, TefSitesListIndex - 1).ToList();
                if (TefGsmCellsIndex > -1)
                {
                    int endIndex = TefUmtsCellsIndex > -1 ? TefUmtsCellsIndex - 1 : (TefLteCellsIndex > -1 ? TefLteCellsIndex - 1 : TefBulkCiIndex);
                    TefGsmCells = tempLog.Slice(TefGsmCellsIndex + 1, endIndex)
                        .Select(c => c.Split(strToFind, StringSplitOptions.None)[1])
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList();
                }
                if (TefUmtsCellsIndex > -1)
                {
                    int endIndex = TefLteCellsIndex > -1 ? TefLteCellsIndex - 1 : TefBulkCiIndex;
                    TefUmtsCells = tempLog.Slice(TefUmtsCellsIndex + 1, endIndex)
                        .Select(c => c.Split(strToFind, StringSplitOptions.None)[1])
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList();
                }
                if (TefLteCellsIndex > -1)
                    TefLteCells = tempLog.Slice(TefLteCellsIndex + 1, tempLog.Length).ToList();

                try
                {
                    if (TefGsmCellsIndex > -1)
                        TefGsmTime = Convert.ToDateTime(tempLog[TefGsmCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
                }
                catch { }
                try
                {
                    if (TefUmtsCellsIndex > -1)
                        TefUmtsTime = Convert.ToDateTime(tempLog[TefUmtsCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
                }
                catch { }
                try
                {
                    if (TefLteCellsIndex > -1)
                        TefLteTime = Convert.ToDateTime(tempLog[TefLteCellsIndex].Split(strToFind, StringSplitOptions.None)[1]);
                }
                catch { }
            }
		}

        //void ResolveAffectedSites(string[] log, string @operator)
        //{
        //    var tempSites = @operator == "VF" ? VfSites : TefSites;
        //    for (int c = 0; c < tempSites.Count; c++)
        //        tempSites[c] = Convert.ToInt32(tempSites[c].Split(' ')[0].Replace("RBS", string.Empty)).ToString();
        //    List<Site> sites = DB.SitesDB.getSites(tempSites);
        //    if(@operator == "VF")
        //    {
        //        foreach (Site site in sites)
        //        {
        //            List<Cell> cells = @operator == "VF" ? site.Cells.Filter(Cell.Filters.VF_2G) : site.Cells.Filter(Cell.Filters.TF_2G);
        //            if (cells.Count > 0)
        //            {
        //                if (@operator == "VF")
        //                {
        //                    if (VfGsmAffectedSites == null)
        //                        VfGsmAffectedSites = new List<string>();
        //                }
        //                else
        //                {
        //                    if (TefGsmAffectedSites == null)
        //                        TefGsmAffectedSites = new List<string>();
        //                }
        //            }
        //            foreach (Cell cell in cells)
        //            {
        //                if (string.Join(Environment.NewLine, log).Contains(cell.Name))
        //                {
        //                    VfGsmAffectedSites.Add(site.FullId);
        //                    break;
        //                }
        //            }
        //            cells = site.Cells.Filter(Cell.Filters.VF_3G);
        //            if (cells.Count > 0)
        //            {
        //                if (VfUmtsAffectedSites == null)
        //                    VfUmtsAffectedSites = new List<string>();
        //                foreach (Cell cell in cells)
        //                {
        //                    if (string.Join(Environment.NewLine, log).Contains(cell.Name))
        //                    {
        //                        VfUmtsAffectedSites.Add(site.FullId);
        //                        break;
        //                    }
        //                }
        //            }
        //            cells = site.Cells.Filter(Cell.Filters.VF_4G);
        //            if (cells.Count > 0)
        //            {
        //                if (VfLteAffectedSites == null)
        //                    VfLteAffectedSites = new List<string>();
        //                foreach (Cell cell in cells)
        //                {
        //                    if (string.Join(Environment.NewLine, log).Contains(cell.Name))
        //                    {
        //                        VfLteAffectedSites.Add(site.FullId);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        tempSites = TefSites;
        //        for (int c = 0; c < tempSites.Count; c++)
        //            tempSites[c] = Convert.ToInt32(tempSites[c].Split(' ')[0].Replace("RBS", string.Empty)).ToString();
        //        sites = DB.SitesDB.getSites(tempSites);
        //        foreach (Site site in sites)
        //        {
        //            List<Cell> cells = site.Cells.Filter(Cell.Filters.TF_2G);
        //            if (cells.Count > 0)
        //            {
        //                if (TefGsmAffectedSites == null)
        //                    TefGsmAffectedSites = new List<string>();
        //                foreach (Cell cell in cells)
        //                {
        //                    if (string.Join(Environment.NewLine, log).Contains(cell.Name))
        //                    {
        //                        TefGsmAffectedSites.Add(site.FullId);
        //                        break;
        //                    }
        //                }
        //            }
        //            cells = site.Cells.Filter(Cell.Filters.TF_3G);
        //            if (site.Cells.Filter(Cell.Filters.TF_3G).Count > 0)
        //            {
        //                if (TefUmtsAffectedSites == null)
        //                    TefUmtsAffectedSites = new List<string>();
        //                foreach (Cell cell in cells)
        //                {
        //                    if (string.Join(Environment.NewLine, log).Contains(cell.Name))
        //                    {
        //                        TefUmtsAffectedSites.Add(site.FullId);
        //                        break;
        //                    }
        //                }
        //            }
        //            cells = site.Cells.Filter(Cell.Filters.TF_4G);
        //            if (site.Cells.Filter(Cell.Filters.TF_4G).Count > 0)
        //            {
        //                if (TefLteAffectedSites == null)
        //                    TefLteAffectedSites = new List<string>();
        //                foreach (Cell cell in cells)
        //                {
        //                    if (string.Join(Environment.NewLine, log).Contains(cell.Name))
        //                    {
        //                        TefLteAffectedSites.Add(site.FullId);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

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
				form.Icon = appCore.UI.Resources.app_icon;
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
					AffectedCells.RemoveAll(c => c.Bearer == Bearers.GSM);
				}
				if(cb3G.Checked)
					VfUmtsTime = TefUmtsTime = dtp3G.Value;
				else {
					VfUmtsCells.Clear();
					TefUmtsCells.Clear();
					AffectedCells.RemoveAll(c => c.Bearer == Bearers.UMTS);
				}
				if(cb4G.Checked)
					VfLteTime = TefLteTime = dtp4G.Value;
				else {
					VfLteCells.Clear();
					TefLteCells.Clear();
					AffectedCells.RemoveAll(c => c.Bearer == Bearers.LTE);
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
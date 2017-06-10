/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 08/03/2017
 * Time: 01:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using appCore.SiteFinder;
using FileHelpers;

namespace appCore.DB
{
	/// <summary>
	/// Description of SitesDB.
	/// </summary>
	public static class SitesDB
	{
		static List<Site> SitesCache = new List<Site>();
		
		public static List<Site> List {
			get { return SitesCache; }
		}

		public static void Add(Site site) {
			SitesCache.Add(site);
		}

		public static void AddRange(IEnumerable<Site> sites) {
			SitesCache.AddRange(sites);
		}

		public static void Remove(Site site) {
			if(site.DbIndex > -1)
				SitesCache.RemoveAt(site.DbIndex);
		}
		
		public static void Remove(string siteId) {
			int index = GetSiteIndex(siteId);
			if(index > -1)
				SitesCache.RemoveAt(index);
		}
		
		public static void Clear() {
			SitesCache.Clear();
		}
		
		public static int GetSiteIndex(string siteID) {
//			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//			sw.Start();
//			int index = SitesCache.FindIndex(s => s.Id == site.Id);
//			sw.Stop();
//			var t = sw.Elapsed;
//			sw.Reset();
//			sw.Start();
			int index = -1;
			for(int c = 0; c < SitesCache.Count;c++) {
				if(SitesCache[c].Id == siteID) {
					index = c;
					break;
				}
			}
//			sw.Stop();
//			var t2 = sw.Elapsed;
			return index;
		}
		
		public static int GetSiteIndexWithJVCO(string JVCO) {
			int index = -1;
			for(int c = 0; c < SitesCache.Count;c++) {
				if(SitesCache[c].JVCO_Id == JVCO) {
					index = c;
					break;
				}
			}
			return index;
		}
		
		public static void UpdateSiteData(Site site) {
			if(site != null) {
				int index = site.DbIndex;
				if(index > -1)
					SitesCache[index] = site;
			}
		}
		
		public static Site getSite(string site, bool ignoreCache = false)
		{
			Site foundSite = null;
			if(!ignoreCache) {
				int index = GetSiteIndex(site);
				foundSite = index > -1 ? SitesCache[index] : null;
			}
			
			if(foundSite == null) {
				List<string> sitesList = new List<string>();
				sitesList.Add(site);
				
				List<Site> res = getSites(sitesList, true);
				
				return res.Count > 0 ? res[0] : new Site(site);
			}
			
			return foundSite;
		}
		
		public static Site getSiteWithJVCO(string JVCO, bool ignoreCache = false)
		{
			Site foundSite = null;
			if(!ignoreCache) {
				int index = GetSiteIndexWithJVCO(JVCO);
				foundSite = index > -1 ? SitesCache[index] : null;
			}
			
			if(foundSite == null) {
				List<Site> sites = null;
				try {
					var engine = new FileHelperEngine<Site>();
					engine.BeforeReadRecord += (eng, e) => {
						if(!e.RecordLine.Contains(JVCO))
							e.SkipThisRecord = true;
					};
					engine.AfterReadRecord +=  (eng, e) => {
						if(JVCO != e.Record.JVCO_Id)
							e.SkipThisRecord = true;
						else {
							e.Record.SiteDataTimestamp = DateTime.Now;
//							foundSite = e.Record;
						}
					};
					sites = engine.ReadFileAsList(Databases.all_sites.FullName);
				}
				catch(FileHelpersException e) {
					string f = e.Message;
				}
				
				return sites.Count > 0 ? sites[0] : new Site(JVCO);
			}
			
			return foundSite;
		}
		
		public static List<Site> getSites(IEnumerable<string> sitesToFind, bool ignoreCache = false)
		{
			List<string> findSites = new List<string>();
			findSites.AddRange(sitesToFind);
			List<string> foundSites = new List<string>();
			List<Site> sites = new List<Site>();
			List<Site> totalFoundSites = new List<Site>();
			
			if(!ignoreCache) {
				foreach(string site in findSites) {
					Site foundSite = SitesCache.Find(s => s.Id == site);
					if(foundSite != null) {
                        foundSites.Add(site);
						totalFoundSites.Add(foundSite);
					}
				}
				foreach(string site in foundSites)
					findSites.Remove(site);
				foundSites = new List<string>();
			}
			
			if(findSites.Count > 0) {
				try {
					var engine = new FileHelperEngine<Site>();
					engine.BeforeReadRecord += (eng, e) => {
						if(!findSites.Contains(e.RecordLine.Substring(0, e.RecordLine.IndexOf(','))))
							e.SkipThisRecord = true;
					};
					engine.AfterReadRecord +=  (eng, e) => {
						if(!findSites.Contains(e.Record.Id))
							e.SkipThisRecord = true;
						else {
							e.Record.SiteDataTimestamp = DateTime.Now;
							foundSites.Add(e.Record.Id);
						}
					};
					sites = engine.ReadFileAsList(Databases.all_sites.FullName);
				}
				catch(FileHelpersException e) {
					string f = e.Message;
				}
				
				if(foundSites.Count > 0)
					sites = getCells(foundSites, sites);
				
				totalFoundSites.AddRange(sites);
				
				if(!ignoreCache) {
					foreach(Site site in totalFoundSites) {
						if(site.DbIndex == -1)
							SitesCache.Add(site);
					}
				}
			}
			
			return totalFoundSites;
		}
		
		public static List<Site> getSitesWithJVCO(List<string> JVCOsToFind, bool ignoreCache = false)
		{
			List<string> JVCOs = JVCOsToFind;
			List<string> foundJVCOs = new List<string>();
			List<Site> sites = new List<Site>();
			List<Site> totalFoundSites = new List<Site>();
			
			if(!ignoreCache) {
				foreach(string JVCO in JVCOs) {
					Site foundSite = SitesCache.Find(s => s.JVCO_Id == JVCO);
					if(foundSite != null) {
						foundJVCOs.Add(JVCO);
						totalFoundSites.Add(foundSite);
					}
				}
				foreach(string JVCO in foundJVCOs)
					JVCOs.Remove(JVCO);
				foundJVCOs = new List<string>();
			}
			
			if(JVCOs.Count > 0) {
				try {
					var engine = new FileHelperEngine<Site>();
					engine.BeforeReadRecord += (eng, e) => {
						if(!JVCOs.Contains(e.RecordLine.Substring(0, e.RecordLine.IndexOf(','))))
							e.SkipThisRecord = true;
					};
					engine.AfterReadRecord +=  (eng, e) => {
						if(!JVCOs.Contains(e.Record.JVCO_Id))
							e.SkipThisRecord = true;
						else {
							e.Record.SiteDataTimestamp = DateTime.Now;
							foundJVCOs.Add(e.Record.JVCO_Id);
						}
					};
					sites = engine.ReadFileAsList(Databases.all_sites.FullName);
				}
				catch(FileHelpersException e) {
					string f = e.Message;
				}
				
				if(foundJVCOs.Count > 0)
					sites = getCellsWithJVCO(foundJVCOs, sites);
				
				totalFoundSites.AddRange(sites);
				
				if(!ignoreCache) {
					foreach(Site site in totalFoundSites) {
						if(site.DbIndex == -1)
							SitesCache.Add(site);
					}
				}
			}
			
			return totalFoundSites;
		}

		public static List<Cell> getCells(List<string> cellNames)
		{
			List<Cell> tempCells = null;
			
			try {
				var engine = new FileHelperEngine<Cell>();
				engine.AfterReadRecord +=  (eng, e) => {
					if(!cellNames.Contains(e.Record.Name))
						e.SkipThisRecord = true;
				};
				tempCells = engine.ReadFileAsList(Databases.all_cells.FullName);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return tempCells;
		}

		public static List<Cell> getCells(List<string> sites, Bearers bearers = (Bearers.GSM | Bearers.UMTS | Bearers.LTE))
		{
			List<Cell> foundCells = new List<Cell>();
			try {
				var engine = new FileHelperEngine<Cell>();
				engine.AfterReadRecord +=  (eng, e) => {
					if(!bearers.HasFlag(e.Record.Bearer))
						e.SkipThisRecord = true;
					else {
						if(!sites.Contains(e.Record.ParentSite))
							e.SkipThisRecord = true;
					}
				};
				foundCells = engine.ReadFileAsList(Databases.all_cells.FullName);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			return foundCells;
		}

		public static List<Cell> getCells(string site, Bearers bearers = (Bearers.GSM | Bearers.UMTS | Bearers.LTE))
		{
			List<string> sitesList = new List<string>();
			sitesList.Add(site);
			
			return getCells(sitesList, bearers);
		}
		
		static List<Site> getCells(ICollection<string> sites, List<Site> Sites) {
			try {
				var engine = new FileHelperEngine<Cell>();
				engine.AfterReadRecord +=  (eng, e) => {
					if(!sites.Contains(e.Record.ParentSite))
						e.SkipThisRecord = true;
					else {
						Site tempSite = Sites.Find(s => s.Id == e.Record.ParentSite);
						if(tempSite != null)
							tempSite.Cells.Add(e.Record);
					}
				};
				engine.ReadFileAsList(Databases.all_cells.FullName);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return Sites;
		}
		
		static List<Site> getCellsWithJVCO(ICollection<string> JVCOs, List<Site> Sites) {
			try {
				var engine = new FileHelperEngine<Cell>();
				engine.AfterReadRecord +=  (eng, e) => {
					if(!JVCOs.Contains(e.Record.ParentSite))
						e.SkipThisRecord = true;
					else {
						Site tempSite = Sites.Find(s => s.Id == e.Record.ParentSite);
						if(tempSite != null)
							tempSite.Cells.Add(e.Record);
					}
				};
				engine.ReadFileAsList(Databases.all_cells.FullName);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return Sites;
		}
		
		public static List<Site> queryAllSitesDB(string columnName, string pattern) {
			List<Site> filtered = new List<Site>();
			try {
				var engine = new FileHelperEngine<Site>();
				engine.AfterReadRecord +=  (eng, e) => {
					switch(columnName) {
						case "CELL_ID":
							if(e.Record.Id != pattern)
								e.SkipThisRecord = true;
							else
								e.Record.populateCells();
							break;
					}
				};
				filtered = engine.ReadFileAsList(Databases.all_sites.FullName);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return filtered;
		}
		
		public static List<Cell> queryAllCellsDB(string columnName, string pattern) {
			List<Cell> filtered = new List<Cell>();
			try {
				var engine = new FileHelperEngine<Cell>();
				engine.AfterReadRecord +=  (eng, e) => {
					switch(columnName) {
						case "CELL_ID":
							if(e.Record.Id != pattern)
								e.SkipThisRecord = true;
							break;
					}
				};
				filtered = engine.ReadFileAsList(Databases.all_cells.FullName);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			return filtered;
		}
	}
}

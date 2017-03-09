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
		
//		public static void UpdateSitesOnCache(List<Site> sites) {
//			for(int c = 0;c < sites.Count;c++)
//				UpdateSitesOnCache(sites[c]);
//		}
		
		public static void UpdateSiteOnCache(Site site) {
			if(site != null) {
//				int siteIndex = SitesCache.FindIndex(S => S.Id == site.Id);
				if(site.DbIndex > -1)
					SitesCache[site.DbIndex] = site;
//				else
//					AddToDB(out site);
			}
		}

		public static void AddToDB(Site site) {
			SitesCache.Add(site);
		}
//
//		public static void AddRangeToDB(out List<Site> sites) {
//			int count = ((ICollection<Site>)sites).Count;
//			for(int c = 0;c < count;c++) {
		////				Site site = sites[c];
//				SitesCache.Add(sites[c]);
//				int index = SitesCache.FindIndex(s => s == site);
//				SitesCache[index].DbIndex =
//					sites[c].DbIndex = index;
//			}
//		}
		
		public static int GetSiteIndex(Site site) {
			return SitesCache.FindIndex(s => s.Id == site.Id);
		}
		
		public static Site getSite(string site)
		{
			Site foundSite = SitesCache.Find(s => s.Id == site);
			if(foundSite == null) {
				List<string> sitesList = new List<string>();
				sitesList.Add(site);
				
				List<Site> res = getSites(sitesList, false);
				
//				res[0].DbIndex = SitesCache.Count;
				SitesCache.Add(res[0]);
				
				return res.Count > 0 ? res[0] : new Site();
			}
			
			return foundSite;
		}
		
		public static List<Site> getSites(List<string> sitesToFind, bool UpdateOnCache = true)
		{
			List<string> Sites = sitesToFind;
			List<string> foundSites = new List<string>();
			List<Site> sites = new List<Site>();
			List<Site> totalFoundSites = new List<Site>();
			
			if(UpdateOnCache) {
				foreach(string site in Sites) {
					Site foundSite = SitesCache.Find(s => s.Id == site);
					if(foundSite != null) {
						foundSites.Add(site);
						totalFoundSites.Add(foundSite);
					}
				}
				foreach(string site in foundSites)
					Sites.Remove(site);
				foundSites = new List<string>();
			}
			
			if(Sites.Count > 0) {
				try {
					var engine = new FileHelperEngine<Site>();
					engine.AfterReadRecord +=  (eng, e) => {
						if(!Sites.Contains(e.Record.Id))
							e.SkipThisRecord = true;
						else
							foundSites.Add(e.Record.Id);
					};
					sites = engine.ReadFileAsList(Databases.all_sites.FullName);
				}
				catch(FileHelpersException e) {
					string f = e.Message;
				}
				
				if(foundSites.Count > 0)
					sites = getCells(foundSites, sites);
				
				if(UpdateOnCache) {
					for(int c = 0;c < sites.Count;c++) {
//						sites[c].DbIndex = SitesCache.Count;
						SitesCache.Add(sites[c]);
					}
				}
				
				totalFoundSites.AddRange(sites);
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

		public static List<Cell> getCells(List<string> sites, string bearers = "2G3G4G")
		{
			List<Cell> foundCells = new List<Cell>();
			try {
				var engine = new FileHelperEngine<Cell>();
				engine.AfterReadRecord +=  (eng, e) => {
					if(!bearers.Contains(e.Record.Bearer))
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

		public static List<Cell> getCells(string site, string bearers = "2G3G4G")
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

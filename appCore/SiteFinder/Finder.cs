/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 06-08-2016
 * Time: 18:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using appCore.DB;
using FileHelpers;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Finder.
	/// </summary>
	public static class Finder
	{
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
		
		public static Site getSite(string site)
		{
			List<string> sitesList = new List<string>();
			sitesList.Add(site);
			
			List<Site> res = getSites(sitesList);
			
			return res.Count > 0 ? res[0] : new Site();
		}
		
		public static List<Site> getSites(List<string> Sites)
		{
			List<Site> sites = new List<Site>();
			List<string> foundSites = new List<string>();
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
			
			return sites;
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
	}
}

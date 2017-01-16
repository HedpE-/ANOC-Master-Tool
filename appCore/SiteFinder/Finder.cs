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
using System.Data;
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
				var engine2 = new FileHelperEngine<Cell>();
				var res = engine2.ReadFileAsList(Databases.all_cells.FullName);
				switch(columnName) {
					case "CELL_ID":
						filtered = res.FindAll(s => s.Id == pattern);
						break;
				}
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			return filtered;
		}
		
		public static Site getSite(string Site)
		{
			Site site = null;
			try {
				var engine = new FileHelperEngine<Site>();
				engine.AfterReadRecord +=  (eng, e) => {
					if(e.Record.Id != Site)
						e.SkipThisRecord = true;
					else
						e.Record.populateCells();
				};
				var res = engine.ReadFileAsList(Databases.all_sites.FullName);
				site = res.Count > 0 ? res[0] : new Site();
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return site;
		}
		
		public static List<Site> getSites(List<string> Sites)
		{
			List<Site> sites = new List<Site>();
			try {
				var engine2 = new FileHelperEngine<Site>();
				engine2.AfterReadRecord +=  (eng, e) => {
					if(!Sites.Contains(e.Record.Id))
						e.SkipThisRecord = true;
					else
						e.Record.populateCells();
				};
				sites = engine2.ReadFileAsList(Databases.all_sites.FullName);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return sites;
		}

		public static Cell getCell(string cell)
		{
			if(!cell.IsAllDigits())
				cell = "00000";
			
			Cell tempCell = null;
			
			try {
				var engine2 = new FileHelperEngine<Cell>();
				engine2.AfterReadRecord +=  (eng, e) => {
					if(e.Record.Name != cell)
						e.SkipThisRecord = true;
				};
				var res = engine2.ReadFileAsList(Databases.all_cells.FullName);
				tempCell = res.Count > 0 ? res[0] : null;
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return tempCell;
		}

		public static List<Cell> getCells(List<string> sites, string bearers = "2G3G4G")
		{
			List<Cell> foundCells = new List<Cell>();
			try {
				var engine2 = new FileHelperEngine<Cell>();
				engine2.AfterReadRecord +=  (eng, e) => {
					if(!bearers.Contains(e.Record.Bearer))
						e.SkipThisRecord = true;
					else {
						if(!sites.Contains(e.Record.ParentSite))
							e.SkipThisRecord = true;
					}
				};
				foundCells = engine2.ReadFileAsList(Databases.all_cells.FullName);
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
	}
}

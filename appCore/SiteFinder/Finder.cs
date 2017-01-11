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
				var res = engine.ReadFileAsList(Databases.all_sites.FullName);
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
				var engine2 = new FileHelperEngine<Site>();
				var res = engine2.ReadFileAsList(Databases.all_sites.FullName);
				site = res.Find(s => s.Id == Site);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			if(site == null)
				site = new Site();
			return site;
		}

		public static List<Cell> getCells(string site)
		{
			if(!site.IsAllDigits())
				site = "00000";
			
			List<Cell> foundCells = new List<Cell>();
			
			try {
				var engine2 = new FileHelperEngine<Cell>();
				var res = engine2.ReadFileAsList(Databases.all_cells.FullName);
				foundCells = res.FindAll(s => s.ParentSite == site);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return foundCells;
		}

		public static Cell getCell(string cell)
		{
			if(!cell.IsAllDigits())
				cell = "00000";
			
			Cell tempCell = null;
			
			try {
				var engine2 = new FileHelperEngine<Cell>();
				var res = engine2.ReadFileAsList(Databases.all_cells.FullName);
				tempCell = res.Find(s => s.Name == cell);
			}
			catch(FileHelpersException e) {
				string f = e.Message;
			}
			
			return tempCell;
		}

//		static DataRowView findSite(string site)
//		{
//			if(!site.IsAllDigits())
//				site = "00000";
//			
//			DataView dv = new DataView(Databases.siteDetailsTable);
//			dv.RowFilter = "SITE = '" + site + "'"; // query example = "id = 10"
//			DataRowView dr = null;
//			if(dv.Count == 1)
//				dr = dv[0];
//			return dr;
//		}
//
//		static DataView findCells(string site)
//		{
//			if(!site.IsAllDigits())
//				site = "00000";
//			
//			DataView dv = new DataView(Databases.cellDetailsTable);
//			dv.RowFilter = "SITE = '" + site + "'";
//			DataTable dt = null;
//			if (dv.Count > 0) {
//				dt = dv.ToTable();
//				//clone the source table
//				DataTable filtered = dt.Clone();
//
//				//fill the clone with the filtered rows
//				foreach (DataRowView drv in dt.DefaultView)
//				{
//					filtered.Rows.Add(drv.Row.ItemArray);
//				}
//				dt = filtered;
//			}
//			
//			return new DataView(dt);
//		}
	}
}

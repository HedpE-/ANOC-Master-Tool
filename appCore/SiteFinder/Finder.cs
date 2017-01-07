/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 06-08-2016
 * Time: 18:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Collections.Generic;
using System.Data;
using appCore.DB;

namespace appCore.SiteFinder
{
    /// <summary>
    /// Description of Finder.
    /// </summary>
    public static class Finder
	{
		public static Site queryAllSitesDB(string columnName, string pattern) {
//			Action action = new Action(delegate {
			DataView dv = new DataView(Databases.siteDetailsTable);
			dv.RowFilter = columnName + " = '" + pattern + "'"; // query example = "id = 10"
			DataRowView dr = null;
			if(dv.Count == 1)
				dr = dv[0];
			
			bool siteFound = dv.Count > 0;
			
			Site site = new Site();
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,true,this);
			return site;
		}
		
		public static List<Cell> queryAllCellsDB(string columnName, string pattern) {
			DataView dv = new DataView(Databases.cellDetailsTable);
			dv.RowFilter = columnName + " = '" + pattern + "'"; // query example = "id = 10"
//			DataRowView dr = null;
//			if(dv.Count == 1)
//				dr = dv[0];
			List<Cell> filtered = new List<Cell>();
//			DataTable dt = null;
			if(dv.Count > 0) {
				DataTable dt = dv.ToTable();
				//clone the source table
				//fill the list with the reolved Cell instances from filtered rows
				foreach (DataRowView drv in dt.DefaultView) {
					filtered.Add(new Cell(drv));
				}
//				dt = filtered;
			}
			return filtered;
		}
		
		public static Site getSite(string Site)
		{
//			Action action = new Action(delegate {
			DataRowView siteRow = null;
			DataView cellsRows = null;
			
			while(Site.StartsWith("0"))
				Site = Site.Substring(1);
			
			if(!string.IsNullOrEmpty(Site)) {
				siteRow = findSite(Site);
				cellsRows = findCells(Site);
			}
			else
				return new Site();
			
			Site site = new Site(siteRow, cellsRows);
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,true,this);
			return site;
		}

		public static List<Cell> getCells(string site)
		{
			if(!site.IsAllDigits())
				site = "00000";
			
			DataView dv = new DataView(Databases.cellDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'";
			
			List<Cell> cellsList = new List<Cell>();
			if (dv.Count > 0) {
				foreach(DataRowView tempCell in dv)
					cellsList.Add(new Cell(tempCell));
			}
			
			return cellsList;
		}

		public static Cell getCell(string cell)
		{
			if(!cell.IsAllDigits())
				cell = "00000";
			
			DataView dv = new DataView(Databases.cellDetailsTable);
			dv.RowFilter = "CELL_NAME = '" + cell + "'";
			
			Cell tempCell = new Cell(dv[0]);
			return tempCell;
		}

		static DataRowView findSite(string site)
		{
			if(!site.IsAllDigits())
				site = "00000";
			
			DataView dv = new DataView(Databases.siteDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'"; // query example = "id = 10"
			DataRowView dr = null;
			if(dv.Count == 1)
				dr = dv[0];
			return dr;
		}

		static DataView findCells(string site)
		{
			if(!site.IsAllDigits())
				site = "00000";
			
			DataView dv = new DataView(Databases.cellDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'";
			DataTable dt = null;
			if (dv.Count > 0) {
				dt = dv.ToTable();
				//clone the source table
				DataTable filtered = dt.Clone();

				//fill the clone with the filtered rows
				foreach (DataRowView drv in dt.DefaultView)
				{
					filtered.Rows.Add(drv.Row.ItemArray);
				}
				dt = filtered;
			}
			
			return new DataView(dt);
		}
	}
}

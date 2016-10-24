/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 06-08-2016
 * Time: 18:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Data;
using appCore.DB;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Finder.
	/// </summary>
	public static class Finder
	{
		public static Site getSite(string Site)
		{
//			Action action = new Action(delegate {
			                           	DataRowView siteRow = null;
			                           	DataView cellsRows = null;
			                           	
			                           	if(!string.IsNullOrEmpty(Site)) {
			                           		siteRow = findSite(Site);
			                           		cellsRows = findCells(Site);
			                           	}
			                           	else
			                           		return new Site();
			                           	
			                           	bool siteFound = siteRow != null;
			                           	
			                           	Site site = new Site(siteRow, cellsRows);
			                           	return site;
//			                           	switch (tb.Name) {
//			                           		case "textBox2":
//			                           			break;
//			                           		case "textBox27":
//			                           			if(siteRow != null) {
//			                           				textBox46.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("PRIORITY")].ToString();
//			                           				textBox47.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("VF_REGION")].ToString();
//			                           			}
//			                           			else {
//			                           				textBox47.Text = "No site found";
//			                           				textBox46.Text = string.Empty;
//			                           			}
//			                           			break;
//			                           		case "textBox43":
//			                           			if(siteRow != null) {
//			                           				textBox45.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("VF_REGION")].ToString();
//			                           				textBox48.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("PRIORITY")].ToString();
//			                           				if(siteRow.Row.Table.Columns.IndexOf("POWER_COMPANY") > -1 && siteRow.Row.Table.Columns.IndexOf("POWER_CONTACT") > -1)
//			                           					textBox49.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("POWER_COMPANY")].ToString() + " +" + siteRow[siteRow.Row.Table.Columns.IndexOf("POWER_CONTACT")].ToString();
//			                           			}
//			                           			else {
//			                           				textBox45.Text = "No site found";
//			                           				textBox48.Text = string.Empty;
//			                           				textBox49.Text = string.Empty;
//			                           			}
//			                           			break;
//			                           		case "textBox50":
//			                           			if(siteRow != null) {
//			                           				cellsRows.RowFilter = "VENDOR LIKE 'ERIC*'";
//			                           				if(cellsRows.Count == 0){
//			                           					("Error",String.Format("Site {0} is not E///",tb.Text));
//			                           					return;
//			                           				}
//			                           				cellsRows.RowFilter = string.Empty;
//			                           				eScriptCellsGlobal = cellsRows;
//			                           				textBox51.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("PRIORITY")].ToString();
//			                           				textBox52.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("VF_REGION")].ToString();
//			                           				
//			                           				// Fill Cell Count table
//			                           				
//			                           				label_TotalCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '2G'";
//			                           				label_Total_2GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '2G' AND VENDOR LIKE 'ERIC*'";
//			                           				radioButton6.Enabled |= eScriptCellsGlobal.Count > 0;
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '2G' AND (CELL_NAME NOT LIKE 'T*' AND CELL_NAME NOT LIKE '*W' AND CELL_NAME NOT LIKE '*X' AND CELL_NAME NOT LIKE '*Y')";
//			                           				label_VF_2GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '2G' AND (CELL_NAME LIKE 'T*' OR CELL_NAME LIKE '*W' OR CELL_NAME LIKE '*X' OR CELL_NAME LIKE '*Y')";
//			                           				label_TF_2GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '3G'";
//			                           				label_Total_3GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '3G' AND VENDOR LIKE 'ERIC*'";
//			                           				radioButton7.Enabled |= eScriptCellsGlobal.Count > 0;
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '3G' AND CELL_NAME NOT LIKE 'T*'";
//			                           				label_VF_3GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '3G' AND CELL_NAME LIKE 'T*'";
//			                           				label_TF_3GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '4G'";
//			                           				label_Total_4GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '4G' AND VENDOR LIKE 'ERIC*'";
//			                           				radioButton8.Enabled |= eScriptCellsGlobal.Count > 0;
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '4G' AND CELL_NAME NOT LIKE 'T*'";
//			                           				label_VF_4GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				eScriptCellsGlobal.RowFilter = "BEARER = '4G' AND CELL_NAME LIKE 'T*'";
//			                           				label_TF_4GCells.Text = eScriptCellsGlobal.Count.ToString();
//			                           				
//			                           				eScriptCellsGlobal.RowFilter = string.Empty;
//			                           			}
//			                           			else {
//			                           				textBox51.Text = string.Empty;
//			                           				textBox52.Text = "No site found";
//			                           			}
//			                           			break;
//			                           	}
//			                           	siteFinder_Toggle(true, siteFound, tb.Name);
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,true,this);
		}

		public static DataRowView findSite(string site)
		{
			if(!Toolbox.Tools.IsAllDigits(site))
				site = "00000";
			
			DataView dv = new DataView(Databases.siteDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'"; // query example = "id = 10"
			DataRowView dr = null;
			if(dv.Count == 1)
				dr = dv[0];
			return dr;
		}

		public static DataView findCells(string site)
		{
			if(!Toolbox.Tools.IsAllDigits(site))
				site = "00000";
			
			DataView dv = new DataView(Databases.cellDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'";
			DataTable dt = null;
			if (dv.Count > 0)
			{
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

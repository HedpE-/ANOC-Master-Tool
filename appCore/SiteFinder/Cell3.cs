/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 21-07-2016
 * Time: 19:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using appCore.SiteFinder;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Cell.
	/// </summary>
	public class Cell3
	{
		string SITE = string.Empty;
		public string ParentSite { get { return SITE; } }
		string JVCO_ID = string.Empty;
		public string JVCO { get { return JVCO_ID; } }
		string CELL_ID = string.Empty;
		public string Id { get { return CELL_ID; } }
		string LAC_TAC = string.Empty;
		public string LacTac { get { return LAC_TAC; } }
		string BSC_RNC_ID = string.Empty;
		public string BscRnc_Id { get { return BSC_RNC_ID; } }
		Site.Vendors VENDOR = Site.Vendors.None;
		public Site.Vendors Vendor { get { return VENDOR; } }
		string ENODEB_ID = string.Empty;
		public string ENodeB_Id { get { return ENODEB_ID; } }
		string TF_SITENO = string.Empty;
		public string TEF_SiteId { get { return TF_SITENO; } }
		string CELL_NAME = string.Empty;
		public string Name { get { return CELL_NAME; } }
		string BEARER = string.Empty;
		public string Bearer { get { return BEARER; } }
//		string COOS = string.Empty;
//		string SO_EXCLUSION = string.Empty;
//		string WHITE_LIST = string.Empty;
//		public string WhiteList { get { return WHITE_LIST; } }
//		string NTQ = string.Empty;
//		public string NoticeToQuit { get { return NTQ; } }
		string NOC = string.Empty;
		public string Noc { get { return NOC; } }
		public string WBTS_BCF { get; private set; }
		string celloperator;
		public string Operator {
			get {
				if(string.IsNullOrEmpty(celloperator))
					celloperator = Name.StartsWith("T") || Name.EndsWith("W") || Name.EndsWith("X") || Name.EndsWith("Y") ? "TEF" : "VF";
				return celloperator;
			}
			private set { celloperator = value;}
		}
		public bool Locked { get; set; }
		string IP_2G_I = string.Empty;
		public string InnerIP2G { get { return IP_2G_I; } }
		string IP_2G_E = string.Empty;
		public string OuterIP2G { get { return IP_2G_E; } }
		string IP_3G_I = string.Empty;
		public string InnerIP3G { get { return IP_3G_I; } }
		string IP_3G_E = string.Empty;
		public string OuterIP3G { get { return IP_3G_E; } }
		string IP_4G_I = string.Empty;
		public string InnerIP4G { get { return IP_4G_I; } }
		string IP_4G_E = string.Empty;
		public string OuterIP4G { get { return IP_4G_E; } }
//		Site.Vendors VENDOR_2G = Site.Vendors.None;
//		public Site.Vendors Vendor2G { get { return VENDOR_2G; } }
//		Site.Vendors VENDOR_3G = Site.Vendors.None;
//		public Site.Vendors Vendor3G { get { return VENDOR_3G; } }
//		Site.Vendors VENDOR_4G = Site.Vendors.None;
//		public Site.Vendors Vendor4G { get { return VENDOR_4G; } }
		
		protected DataRowView _cell;
		
		public Cell3(DataRowView cell)
		{
			_cell = cell;
			
			try { SITE = _cell[_cell.Row.Table.Columns.IndexOf("SITE")].ToString(); } catch (Exception) { }
			try { JVCO_ID = _cell[_cell.Row.Table.Columns.IndexOf("JVCO_ID")].ToString(); } catch (Exception) { }
			try { CELL_ID = _cell[_cell.Row.Table.Columns.IndexOf("CELL_ID")].ToString(); } catch (Exception) { }
			try { LAC_TAC = _cell[_cell.Row.Table.Columns.IndexOf("LAC_TAC")].ToString(); } catch (Exception) { }
			try { BSC_RNC_ID = _cell[_cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString(); } catch (Exception) { }
			try { VENDOR = getVendor(_cell[_cell.Row.Table.Columns.IndexOf("VENDOR")].ToString()); } catch (Exception) { }
			try { ENODEB_ID = _cell[_cell.Row.Table.Columns.IndexOf("ENODEB_ID")].ToString(); } catch (Exception) { }
			try { TF_SITENO = _cell[_cell.Row.Table.Columns.IndexOf("TF_SITENO")].ToString(); } catch (Exception) { }
			try { CELL_NAME = _cell[_cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString(); } catch (Exception) { }
			try { BEARER = _cell[_cell.Row.Table.Columns.IndexOf("BEARER")].ToString(); } catch (Exception) { }
//			try { COOS = _cell[_cell.Row.Table.Columns.IndexOf("COOS")].ToString(); } catch (Exception) { }
//			try { SO_EXCLUSION = _cell[_cell.Row.Table.Columns.IndexOf("SO_EXCLUSION")].ToString(); } catch (Exception) { }
//			try { WHITE_LIST = _cell[_cell.Row.Table.Columns.IndexOf("WHITE_LIST")].ToString(); } catch (Exception) { }
//			try { NTQ = _cell[_cell.Row.Table.Columns.IndexOf("NTQ")].ToString(); } catch (Exception) { }
			try { NOC = _cell[_cell.Row.Table.Columns.IndexOf("NOC")].ToString(); } catch (Exception) { }
			try { WBTS_BCF = _cell[_cell.Row.Table.Columns.IndexOf("WBTS_BCF")].ToString(); } catch (Exception) { }
//			try { LOCKED = _cell[_cell.Row.Table.Columns.IndexOf("LOCKED")].ToString(); } catch (Exception) { }
			try { IP_2G_I = _cell[_cell.Row.Table.Columns.IndexOf("IP_2G_I")].ToString(); } catch (Exception) { }
			try { IP_2G_E = _cell[_cell.Row.Table.Columns.IndexOf("IP_2G_E")].ToString(); } catch (Exception) { }
			try { IP_3G_I = _cell[_cell.Row.Table.Columns.IndexOf("IP_3G_I")].ToString(); } catch (Exception) { }
			try { IP_3G_E = _cell[_cell.Row.Table.Columns.IndexOf("IP_3G_E")].ToString(); } catch (Exception) { }
			try { IP_4G_I = _cell[_cell.Row.Table.Columns.IndexOf("IP_4G_I")].ToString(); } catch (Exception) { }
			try { IP_4G_E = _cell[_cell.Row.Table.Columns.IndexOf("IP_4G_E")].ToString(); } catch (Exception) { }
//			try { VENDOR_2G = getVendor(_cell[_cell.Row.Table.Columns.IndexOf("VENDOR_2G")].ToString()); } catch (Exception) { }
//			try { VENDOR_3G = getVendor(_cell[_cell.Row.Table.Columns.IndexOf("VENDOR_3G")].ToString()); } catch (Exception) { }
//			try { VENDOR_4G = getVendor(_cell[_cell.Row.Table.Columns.IndexOf("VENDOR_4G")].ToString()); } catch (Exception) { }
		}
		
		Site.Vendors getVendor(string strVendor) {
			switch (strVendor.ToUpper()) {
				case "ERICSSON":
					return Site.Vendors.Ericsson;
				case "HUAWEI":
					return Site.Vendors.Huawei;
				case "ALU": case "ALCATEL":
					return Site.Vendors.ALU;
				case "NSN":
					return Site.Vendors.NSN;
				default:
					return Site.Vendors.None;
			}
		}
		
		public enum Filters : byte {
			All_2G,
			All_3G,
			All_4G,
			VF_2G,
			VF_3G,
			VF_4G,
			TF_2G,
			TF_3G,
			TF_4G,
			Locked,
			Unlocked
		};
	}
}

public static class CellExtension {
	public static List<Cell> Filter(this List<Cell> toFilter, Cell.Filters filter) {
		List<Cell> list = new List<Cell>();
		switch(filter) {
			case Cell.Filters.All_2G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "2G" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.VF_2G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "2G" && cell.Operator == "VF" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.TF_2G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "2G" && cell.Operator == "TEF" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.All_3G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "3G" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.VF_3G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "3G" && cell.Operator == "VF" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.TF_3G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "3G" && cell.Operator == "TEF" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.All_4G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "4G" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.VF_4G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "4G" && cell.Operator == "VF" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.TF_4G:
				foreach(Cell cell in toFilter) {
					if(cell.Bearer == "4G" && cell.Operator == "TEF" && cell.Noc.Contains("ANOC"))
						list.Add(cell);
				}
				return list;
			case Cell.Filters.Locked:
				foreach(Cell cell in toFilter) {
					if(cell.Locked)
						list.Add(cell);
				}
				return list;
			case Cell.Filters.Unlocked:
				foreach(Cell cell in toFilter) {
					if(!cell.Locked)
						list.Add(cell);
				}
				return list;
		}
		return null;
	}
}

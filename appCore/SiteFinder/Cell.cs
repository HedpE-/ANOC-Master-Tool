/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 11-01-2017
 * Time: 11:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using appCore.SiteFinder;
using FileHelpers;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Cell2.
	/// </summary>
	[DelimitedRecord(","), IgnoreFirst(1)]
	public class Cell
	{
		[FieldOrder(1)]
		string SITE;
		public string ParentSite { get { return SITE; } }
		[FieldOrder(2)]
		string JVCO_ID;
		public string JVCO { get { return JVCO_ID; } }
		[FieldOrder(3)]
		string CELL_ID;
		public string Id { get { return CELL_ID; } }
		[FieldOrder(4)]
		string LAC_TAC;
		public string LacTac { get { return LAC_TAC; } }
		[FieldOrder(5)]
		string BSC_RNC_ID;
		public string BscRnc_Id { get { return BSC_RNC_ID; } }
		[FieldOrder(6)]
		string VENDOR;
		public Site.Vendors Vendor { get { return getVendor(VENDOR); } }
		[FieldOrder(7)]
		string ENODEB_ID;
		public string ENodeB_Id { get { return ENODEB_ID; } }
		[FieldOrder(8)]
		string TF_SITENO;
		public string TEF_SiteId { get { return TF_SITENO; } }
		[FieldOrder(9)]
		string CELL_NAME;
		public string Name { get { return CELL_NAME; } }
		[FieldOrder(10)]
		string BEARER;
		public string Bearer { get { return BEARER; } }
		[FieldOrder(11)]
		string COOS;
		[FieldOrder(12)]
		string SO_EXCLUSION;
		[FieldOrder(13)]
		string WHITE_LIST;
		public string WhiteList { get { return WHITE_LIST; } }
		[FieldOrder(14)]
		string NTQ;
		public string NoticeToQuit { get { return NTQ; } }
		[FieldOrder(15)]
		string NOC;
		public string Noc { get { return NOC; } }
		[FieldOrder(16)]
		public string WBTS_BCF { get; private set; }
		[FieldOrder(17)]
		[FieldConverter(ConverterKind.Boolean, "Y", "")]
		[FieldNullValue(typeof (bool), "false")]
		public bool Locked;
		[FieldOrder(18)]
		string IP_2G_I;
		public string InnerIP2G { get { return IP_2G_I; } }
		[FieldOrder(19)]
		string IP_2G_E;
		public string OuterIP2G { get { return IP_2G_E; } }
		[FieldOrder(20)]
		string IP_3G_I;
		public string InnerIP3G { get { return IP_3G_I; } }
		[FieldOrder(21)]
		string IP_3G_E;
		public string OuterIP3G { get { return IP_3G_E; } }
		[FieldOrder(22)]
		string IP_4G_I;
		public string InnerIP4G { get { return IP_4G_I; } }
		[FieldOrder(23)]
		string IP_4G_E;
		public string OuterIP4G { get { return IP_4G_E; } }
		[FieldOrder(24)]
		string VENDOR_2G;
//		public Site.Vendors Vendor2G { get { return getVendor(VENDOR_2G); } }
		[FieldOrder(25)]
		string VENDOR_3G;
//		public Site.Vendors Vendor3G { get { return getVendor(VENDOR_3G); } }
		[FieldOrder(26)]
		string VENDOR_4G;
//		public Site.Vendors Vendor4G { get { return getVendor(VENDOR_4G); } }
		
		[FieldHidden]
		string celloperator;
		public string Operator {
			get {
				if(string.IsNullOrEmpty(celloperator))
					celloperator = Name.StartsWith("T") || Name.EndsWith("W") || Name.EndsWith("X") || Name.EndsWith("Y") ? "TEF" : "VF";
				return celloperator;
			}
			private set { celloperator = value;}
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

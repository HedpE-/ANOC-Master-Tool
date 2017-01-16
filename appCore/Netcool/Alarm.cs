/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 23-09-2016
 * Time: 23:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using appCore.SiteFinder;
using FileHelpers;

namespace appCore.Netcool
{
	/// <summary>
	/// Description of Alarm.
	/// </summary>
	[DelimitedRecord("\t"), IgnoreFirst(1)]
	public class Alarm
	{
		[FieldOrder(1)]
		string attributes;
		public string[] Attributes { get { return attributes.Split('/'); } private set { } }
		[FieldOrder(2)]
		string serviceImpact;
		public string ServiceImpact { get { return serviceImpact; } private set { } }
		[FieldOrder(3)]
		string vendor;
		public Site.Vendors Vendor { get { return getVendor(vendor); } protected set { } }
		[FieldOrder(4)]
		string lastOccurrence;
		public DateTime LastOccurrence { get { return Convert.ToDateTime(lastOccurrence); } protected set { } }
		[FieldOrder(5)]
		string alarmCount;
		public int AlarmCount { get { return Convert.ToInt32(alarmCount); } protected set { } }
		[FieldOrder(6)]
		string rncBsc;
		public string RncBsc { get { return rncBsc; } protected set { } }
		[FieldOrder(7)]
		string location;
		public string Location { get { return location; } protected set { } }
		[FieldOrder(8)]
		string element;
		public string Element {
			get {
				if(string.IsNullOrEmpty(element))
					element = ResolveCellName();
				return element;
			}
			protected set { }
		}
		[FieldOrder(9)]
		string summary;
		public string Summary { get { return summary; } protected set { } }
		[FieldOrder(10)]
		string triagedBy;
		public string TriagedBy { get { return triagedBy; } protected set { } }
		[FieldOrder(11)]
		string operatorComments;
		public string OperatorComments { get { return operatorComments; } protected set { } }
		[FieldOrder(12)]
		string vfTtNumber;
		public string VfTtNumber { get { return vfTtNumber; } protected set { } }
		[FieldOrder(13)]
		string vfTtPty;
		public string VfTtPty { get { return vfTtPty; } protected set { } }
		[FieldOrder(14)]
		string assignedGroup;
		public string AssignedGroup { get { return assignedGroup; } protected set { } }
		[FieldOrder(15)]
		string ttStatus;
		public string TtStatus { get { return ttStatus; } protected set { } }
		[FieldOrder(16)]
		string rbsDetails;
		public string RbsDetails { get { return rbsDetails; } protected set { } }
		[FieldOrder(17)]
		string town;
		public string Town { get { return town; } protected set { } }
		[FieldOrder(18)]
		string county;
		public string County { get { return county; } protected set { } }
		[FieldOrder(19)]
		string specialEvent;
		public string SpecialEvent { get { return specialEvent; } protected set { } }
		[FieldOrder(20)]
		string siteType;
		public string SiteType { get { return siteType; } protected set { } }
		[FieldOrder(21)]
		string stateChange;
		public string StateChange { get { return stateChange; } protected set { } }
		[FieldOrder(22)]
		string site;
		public string Site { get { return site; } protected set { } }
		[FieldOrder(23)]
		string node;
		public string Node { get { return node; } protected set { } }
		[FieldOrder(24)]
		string nodeAlias;
		public string NodeAlias { get { return nodeAlias; } protected set { } }
		[FieldOrder(25)]
		string identifier;
		public string Identifier { get { return identifier; } protected set { } }
		[FieldOrder(26)]
		string parentNode;
		public string ParentNode { get { return parentNode; } protected set { } }
		[FieldOrder(27)]
		string techDomain6;
		public string TechDomain6 { get { return techDomain6; } protected set { } }
		[FieldOrder(28)]
		string poc;
		public string POC { get { return poc; } protected set { } }
		[FieldOrder(29)]
		string intermittent;
		public string Intermittent { get { return intermittent; } protected set { } }
		[FieldOrder(30)]
		string weightage;
		public string Weightage { get { return weightage; } protected set { } }
		[FieldOrder(31)]
		string _class;
		public string Class { get { return _class; } protected set { } }
		[FieldOrder(32)]
		string opState;
		public string OpState { get { return opState; } protected set { } }
		[FieldOptional]
		[FieldOrder(33)]
		public string[] mDummyField;
		
		[FieldHidden]
		string siteid;
		public string SiteId {
			get {
				if(string.IsNullOrEmpty(siteid)) {
					siteid = Element.Contains("RBS") ? Element : Location;
					if(!string.IsNullOrEmpty(siteid))
						siteid = Convert.ToInt32(siteid.RemoveLetters()).ToString();
				}
				
				return siteid;
			}
			protected set {
				siteid = value;
			}
		}
		[FieldHidden]
		Site _site = null;
		public Site ParentSite {
			get {
				if(_site == null)
					_site = Finder.getSite(SiteId);
				return _site;
			}
			protected set {
				_site = value;
			}
		}
		[FieldHidden]
		bool coos;
		public bool COOS {
			get {
				if(!coos && !onm)
					checkCoosOrOnM();
				return coos;
			}
			protected set { }
		}
		[FieldHidden]
		bool onm;
		public bool OnM {
			get {
				if(!coos && !onm)
					checkCoosOrOnM();
				return onm;
			}
			protected set { }
		}
		[FieldHidden]
		string bearer;
		public string Bearer {
			get {
				if(string.IsNullOrEmpty(bearer))
					try { bearer = resolveAlarmBearer(); } catch {}
				return bearer;
			}
			protected set { }
		}
		
		[FieldHidden]
		string _parsedAlarm;
		public string ParsedAlarm {
			get {
				if(string.IsNullOrEmpty(_parsedAlarm))
					_parsedAlarm = LastOccurrence + " - " + Summary + Environment.NewLine + RncBsc + " > " + Location + " > " + Element + Environment.NewLine + "Alarm count: " + AlarmCount;
				return _parsedAlarm;
			}
			private set { }
		}
		
		[FieldHidden]
		string celloperator;
		public string Operator {
			get {
				if(string.IsNullOrEmpty(celloperator))
					celloperator = Element.StartsWith("T") || Element.EndsWith("W") || Element.EndsWith("X") || Element.EndsWith("Y") ? "TEF" : "VF";
				return celloperator;
			}
			private set { celloperator = value;}
		}
		
		public Alarm() {}
		
		public Alarm(Cell cell, bool alarmCOOS, DateTime alarmTime) {
			rncBsc = cell.BscRnc_Id;
			lastOccurrence = alarmTime.ToString();
			element = cell.Name;
			coos = alarmCOOS;
			bearer = cell.Bearer;
			string temp = cell.ParentSite;
			while(temp.Length < 5)
				temp = "0" + temp;
			location = "RBS" + temp;
			
			vendor = cell.Vendor.ToString();
			
			county = ParentSite.County;
			town = ParentSite.Town;
		}
		
		public Alarm(Cell cell, bool alarmCOOS, Alarm parentAlarm) {
			rncBsc = cell.BscRnc_Id;
			lastOccurrence = parentAlarm.LastOccurrence.ToString();
			element = cell.Name;
			coos = alarmCOOS;
			bearer = cell.Bearer;
			location = parentAlarm.Location;
			
			vendor = cell.Vendor.ToString();
			
			summary = parentAlarm.Summary + "(Auto generated alarm)";
			alarmCount = parentAlarm.AlarmCount.ToString();
			attributes = string.Join("/",parentAlarm.Attributes);
			poc = parentAlarm.POC;
			node = parentAlarm.Node;
			identifier = parentAlarm.Identifier;
			
			county = parentAlarm.County;
			town = parentAlarm.Town;
			ParsedAlarm = LastOccurrence + " - " + Summary + Environment.NewLine + RncBsc + " > " + Location + " > " + Element + Environment.NewLine + "Alarm count: " + AlarmCount;
		}
		
		void checkCoosOrOnM() { // FIXME: O&M alarms for proper filtering
			switch (Vendor) {
				case SiteFinder.Site.Vendors.ALU:
					coos = Summary.Contains("UNDERLYING_RESOURCE_UNAVAILABLE: State change to Disable");
					break;
				case SiteFinder.Site.Vendors.Ericsson:
					if ((Summary.Contains("CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION") && Summary.Contains("BCCH")) || Summary.Contains("UtranCell_ServiceUnavailable") || Summary.Contains("UtranCell_NbapMessageFailure"))
						coos = true;
					else
						onm = Summary.Contains("Heartbeat Failure") || Summary.Contains("OML FAULT");
					break;
				case SiteFinder.Site.Vendors.Huawei:
					if (Summary.Contains("Cell out of Service") || Summary.Contains("Cell Unavailable") || Summary.Contains("Local Cell Unusable"))
						coos = true;
					else
						onm = Summary.Contains("NE Is Disconnected") || Summary.Contains("OML Fault");
					break;
				case SiteFinder.Site.Vendors.NSN:
					if (Summary.Contains("BCCH MISSING") || Summary.Contains("CELL FAULTY") || Summary.Contains("WCDMA CELL OUT OF USE"))
						coos = true;
					else
						onm = Summary.Contains("O&M");
					break;
			}
		}
		
		string resolveAlarmBearer() {
			string tempBearer = string.Empty;
//			if(!string.IsNullOrEmpty(RncBsc)) {
				switch(RncBsc.Substring(0, 1)) {
					case "B":
						tempBearer = "2G";
						break;
					case "R":
						tempBearer = "3G";
						break;
					default:
						if(Element.StartsWith("D") || Element.StartsWith("G") || Element.StartsWith("I") || Element.StartsWith("P") || Element.StartsWith("S") || Element.StartsWith("U") || Element.StartsWith("TD") || Element.StartsWith("TG") || Element.StartsWith("TI") || Element.StartsWith("TP") || Element.StartsWith("TS") || Element.StartsWith("TU"))
							tempBearer = "2G";
						if(Element.StartsWith("A") || Element.StartsWith("B") || Element.StartsWith("C") || Element.StartsWith("H") || Element.StartsWith("M") || (Element.StartsWith("V") && Element.Length < 15) || Element.StartsWith("W") || Element.StartsWith("TA") || Element.StartsWith("TB") || Element.StartsWith("TC") || Element.StartsWith("TH") || Element.StartsWith("TM") || (Element.StartsWith("TV") && Element.Length < 15) || Element.StartsWith("TW"))
							tempBearer = "3G";
						if(Element.StartsWith("N") || Element.StartsWith("Q") || Element.StartsWith("R") || Element.StartsWith("ZE") || Element.StartsWith("ZK") || (Element.StartsWith("V") && Element.Length == 15) || Element.StartsWith("TN") || Element.StartsWith("TQ") || Element.StartsWith("TR") || Element.StartsWith("TZE") || Element.StartsWith("TZK"))
							tempBearer = "4G";
						break;
				}
//			}
//			else
//				if(!string.IsNullOrEmpty(Element))
//					tempBearer = !Element.StartsWith("V") ? Finder.getCell(Element).Bearer : "4G";
			
			return tempBearer;
		}
		
		string parseSummary(string toParse)
		{
			toParse = toParse.Trim(' ');
			
			if ( toParse.Contains("CELL LOGICAL") )
			{
				toParse = toParse.Replace("P1", string.Empty);
				toParse = toParse.Replace("P3", string.Empty);
				toParse = toParse.Replace("RBS", string.Empty);
				toParse = toParse.Replace("NODEB", string.Empty);
				toParse = toParse.Replace("ENODEB", string.Empty);
				toParse = toParse.Replace("SITE", string.Empty);
				toParse = toParse.Replace(":", string.Empty);
				int Pos = toParse.IndexOf("(", StringComparison.Ordinal);
				
				if (Pos != -1)
				{
					string channelType = string.Empty;
					int a = 0;
					foreach (char ch in toParse)
					{
						if(ch == '(')
							a++;
						if(ch != '(')
							if (a == 0)
								continue;
						channelType += ch.ToString();
						if(ch == ')')
							break;
					}
					if ( channelType == "(TCH FR 1)" )
					{
						int onPos = toParse.IndexOf("CELL = ", StringComparison.Ordinal);
						if (onPos != -1)
						{
							string cell = string.Empty;
							foreach (char ch in toParse.Substring(onPos + 6, toParse.IndexOf(" (", StringComparison.Ordinal) - (onPos + 6)))
							{
								if ( ch != ' ' ) cell += ch;
							}
							channelType += Environment.NewLine + "TCH Cell Degraded (" + cell + ")";
						}
					}
					if ( toParse.Contains("on") )
					{
						int onPos = toParse.IndexOf("on", StringComparison.Ordinal);
						
						if (onPos != -1)
						{
							toParse = toParse.Remove(onPos, toParse.Length - onPos);
						}
					}
					toParse = toParse + channelType;
					toParse = toParse.TrimStart(' ');
					toParse = toParse.TrimEnd(' ');
					return toParse;
				}
			}
			
			if ( toParse.Contains("RADIO X-CEIVER ADMINISTRATION MANAGED OBJECT FAULT") )
			{
				toParse = toParse.Replace("P1", string.Empty);
				toParse = toParse.Replace("P3", string.Empty);
				toParse = toParse.Replace("RBS", string.Empty);
				toParse = toParse.Replace("NODEB", string.Empty);
				toParse = toParse.Replace("ENODEB", string.Empty);
				toParse = toParse.Replace("SITE", string.Empty);
				toParse = toParse.Replace(":", string.Empty);
				int Pos = toParse.IndexOf("MO = ", StringComparison.Ordinal);
				
				if (Pos != -1)
				{
					string moType = string.Empty;
					int a = 0;
					foreach (char ch in toParse)
					{
						if(ch == '=')
							a++;
						if(ch != '=')
							if (a == 0)
								continue;
						moType += ch.ToString();
						if(ch == ':')
							break;
					}
					if ( toParse.Contains("on") )
					{
						int onPos = toParse.IndexOf("on", StringComparison.Ordinal);
						
						if (onPos != -1)
						{
							toParse = toParse.Remove(onPos, toParse.Length - onPos);
						}
					}
					moType = moType.Replace("=", "-");
					moType = moType.Remove(0,2);
					toParse = toParse.Replace("BTS", string.Empty);
					toParse = toParse + Environment.NewLine + moType;
					toParse = toParse.TrimStart(' ');
					toParse = toParse.TrimEnd(' ');
					return toParse;
				}
			}
			
			return toParse;
		}
		
		public string ResolveCellName() {
			string cellName = string.Empty;
			switch(Vendor) {
				case SiteFinder.Site.Vendors.Huawei:
					if(Identifier.Contains("Cell Name=")) {
						string temp = Identifier.Substring(Identifier.IndexOf("Cell Name=") + 10);
						cellName = temp.Substring(0, temp.IndexOf(','));
					}
					else
						cellName = Node;
					break;
				case SiteFinder.Site.Vendors.NSN:
					// FIXME: NSN Cell Name resolver
					char[] nodeNSNcellID = Node.Substring(Node.Length - 3).ToCharArray();
					string elementID = null;
					if(nodeNSNcellID[2] == '4')
						elementID = "M";
					else
						elementID = "W";
					elementID += Location.Substring(3);
					
					if(nodeNSNcellID[2] == '1' || nodeNSNcellID[2] == '4')
						elementID += "0";
					else
						elementID += nodeNSNcellID[2];
					elementID += nodeNSNcellID[0] + nodeNSNcellID[1];
					cellName = elementID;
					break;
				case SiteFinder.Site.Vendors.Ericsson:
					switch(RncBsc.Substring(0,1)) {
						case "B":
							string GCellId = Summary.Substring(Summary.IndexOf("CELL =  ") + 8);
							cellName = GCellId.Substring(0, GCellId.IndexOf(" ("));
							break;
						case "R":
							string UCellId = Summary.Substring(Summary.IndexOf("UtranCell=") + 10);
							List<Cell> results = Finder.queryAllCellsDB("CELL_ID", UCellId);
							foreach(Cell cell in results)
								if(cell.BscRnc_Id == RncBsc && cell.Vendor == Vendor)
									cellName = cell.Name;
							break;
					}
					break;
			}
			return cellName;
		}
		
		Site.Vendors getVendor(string strVendor) {
			switch (strVendor.ToUpper()) {
				case "ERICSSON":
					return SiteFinder.Site.Vendors.Ericsson;
				case "HUAWEI":
					return SiteFinder.Site.Vendors.Huawei;
				case "ALU":
					return SiteFinder.Site.Vendors.ALU;
				case "NSN":
					return SiteFinder.Site.Vendors.NSN;
				default:
					return SiteFinder.Site.Vendors.None;
			}
		}
		
		public override string ToString() {
			return ParsedAlarm;
		}
	}

//	public static class AlarmExtension {
//		public static List<Alarm> Filter(this List<Alarm> toFilter, Cell.Filters filter) {
//			switch(filter) {
//				case Cell.Filters.All_2G:
//					return toFilter.Where(s => s.Bearer == "2G" && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.VF_2G:
//					return toFilter.Where(s => s.Bearer == "2G" && (!s.Name.StartsWith("T") && !s.Name.EndsWith("W") && !s.Name.EndsWith("X") && !s.Name.EndsWith("Y")) && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.TF_2G:
//					return toFilter.Where(s => s.Bearer == "2G" && (s.Name.StartsWith("T") || s.Name.EndsWith("W") || s.Name.EndsWith("X") || s.Name.EndsWith("Y")) && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.All_3G:
//					return toFilter.Where(s => s.Bearer == "3G" && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.VF_3G:
//					return toFilter.Where(s => s.Bearer == "3G" && !s.Name.StartsWith("T") && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.TF_3G:
//					return toFilter.Where(s => s.Bearer == "3G" && s.Name.StartsWith("T") && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.All_4G:
//					return toFilter.Where(s => s.Bearer == "4G" && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.VF_4G:
//					return toFilter.Where(s => s.Bearer == "4G" && !s.Name.StartsWith("T") && s.Noc.Contains("ANOC")).ToList();
//				case Cell.Filters.TF_4G:
//					return toFilter.Where(s => s.Bearer == "4G" && s.Name.StartsWith("T") && s.Noc.Contains("ANOC")).ToList();
//			}
//			return null;
//		}
//	}
}

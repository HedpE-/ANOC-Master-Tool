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
	[DelimitedRecord("\t")]
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
		public Site.Vendors Vendor { get { return getVendor(vendor); } private set { } }
		[FieldOrder(4)]
		string lastOccurrence;
		public DateTime LastOccurrence {
			get {
				try {
					return Convert.ToDateTime(lastOccurrence);
				}
				catch {
					if(lastOccurrence.IsAllDigits())
						return new DateTime(Convert.ToInt64(lastOccurrence));
				}
				return new DateTime(2500,1,1);
			}
			private set { }
		}
		[FieldOrder(5)]
		string alarmCount;
		public int AlarmCount { get { return Convert.ToInt32(alarmCount); } private set { } }
		[FieldOrder(6)]
		string rncBsc;
		public string RncBsc { get { return rncBsc; } private set { } }
		[FieldOrder(7)]
		string location;
		public string Location { get { return location; } private set { } }
		[FieldOrder(8)]
		string element;
		public string Element {
			get {
				if(string.IsNullOrEmpty(element)) {
					try {
						switch(Vendor) {
							case SiteFinder.Site.Vendors.Huawei:
								if(Identifier.Contains("Cell Name=")) {
									string temp = Identifier.Substring(Identifier.IndexOf("Cell Name=") + 10);
									element = temp.Substring(0, temp.IndexOf(','));
								}
								else
									element = Node;
								break;
							case SiteFinder.Site.Vendors.NSN:
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
								element = elementID;
								break;
							case SiteFinder.Site.Vendors.Ericsson:
								switch(RncBsc.Substring(0,1)) {
									case "B":
										string GCellId = Summary.Substring(Summary.IndexOf("CELL =  ") + 8);
										element = GCellId.Substring(0, GCellId.IndexOf(" ("));
										break;
									case "R":
										string UCellId = Summary.Substring(Summary.IndexOf("UtranCell=") + 10);
										List<Cell> results = DB.SitesDB.queryAllCellsDB("CELL_ID", UCellId);
										foreach(Cell cell in results)
											if(cell.BscRnc_Id == RncBsc && cell.Vendor == Vendor)
												element = cell.Name;
										break;
								}
								break;
						}
					} catch { }
				}
				return element;
			}
			set { element = value; }
		}
		[FieldOrder(9)]
		string summary;
		public string Summary { get { return summary; } private set { } }
		[FieldOrder(10)]
		string triagedBy;
		public string TriagedBy { get { return triagedBy; } private set { } }
		[FieldOrder(11)]
		string operatorComments;
		public string OperatorComments { get { return operatorComments; } private set { } }
		[FieldOrder(12)]
		string vfTtNumber;
		public string VfTtNumber { get { return vfTtNumber; } private set { } }
		[FieldOrder(13)]
		string vfTtPty;
		public string VfTtPty { get { return vfTtPty; } private set { } }
		[FieldOrder(14)]
		string assignedGroup;
		public string AssignedGroup { get { return assignedGroup; } private set { } }
		[FieldOrder(15)]
		string ttStatus;
		public string TtStatus { get { return ttStatus; } private set { } }
		[FieldOrder(16)]
		string rbsDetails;
		public string RbsDetails { get { return rbsDetails; } private set { } }
		[FieldOrder(17)]
		string town;
		public string Town { get { return town; } private set { } }
		[FieldOrder(18)]
		string county;
		public string County { get { return county; } private set { } }
		[FieldOrder(19)]
		string specialEvent;
		public string SpecialEvent { get { return specialEvent; } private set { } }
		[FieldOrder(20)]
		string siteType;
		public string SiteType { get { return siteType; } private set { } }
		[FieldOrder(21)]
		string stateChange;
		public string StateChange { get { return stateChange; } private set { } }
		[FieldOrder(22)]
		string site;
		public string Site { get { return site; } private set { } }
		[FieldOrder(23)]
		string node;
		public string Node { get { return node; } private set { } }
		[FieldOrder(24)]
		string nodeAlias;
		public string NodeAlias { get { return nodeAlias; } private set { } }
		[FieldOrder(25)]
		string identifier;
		public string Identifier { get { return identifier; } private set { } }
		[FieldOrder(26)]
		string parentNode;
		public string ParentNode { get { return parentNode; } private set { } }
		[FieldOrder(27)]
		string techDomain6;
		public string TechDomain6 { get { return techDomain6; } private set { } }
		[FieldOrder(28)]
		string poc;
		public string POC { get { return poc; } private set { } }
		[FieldOrder(29)]
		string intermittent;
		public string Intermittent { get { return intermittent; } private set { } }
		[FieldOrder(30)]
		string weightage;
		public string Weightage { get { return weightage; } private set { } }
		[FieldOrder(31)]
		string _class;
		public string Class { get { return _class; } private set { } }
		[FieldOrder(32)]
		string opState;
		public string OpState { get { return opState; } private set { } }
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
						try { siteid = Convert.ToInt32(siteid.RemoveLetters()).ToString(); } catch { siteid = string.Empty; }
				}
				
				return siteid;
			}
			private set {
				siteid = value;
			}
		}
		[FieldHidden]
		Site _site = null;
		public Site ParentSite {
			get {
				if(_site == null)
					_site = DB.SitesDB.getSite(SiteId);
				return _site;
			}
			private set {
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
			private set { }
		}
		[FieldHidden]
		bool onm;
		public bool OnM {
			get {
				if(!coos && !onm)
					checkCoosOrOnM();
				return onm;
			}
			private set { }
		}
		[FieldHidden]
		string bearer;
		public string Bearer {
			get {
				if(string.IsNullOrEmpty(bearer)) {
					if(!string.IsNullOrEmpty(RncBsc)) {
						switch(RncBsc.Substring(0, 1)) {
							case "B":
								bearer = "2G";
								break;
							case "R":
								bearer = "3G";
								break;
							case "X":
								bearer = "4G";
								break;
						}
					}
					else{
						try {
							if(Element.StartsWith("D") || Element.StartsWith("G") || Element.StartsWith("I") || Element.StartsWith("P") || Element.StartsWith("S") || Element.StartsWith("U") || Element.StartsWith("TD") || Element.StartsWith("TG") || Element.StartsWith("TI") || Element.StartsWith("TP") || Element.StartsWith("TS") || Element.StartsWith("TU"))
								bearer = "2G";
						} catch { }
						try {
							if(Element.StartsWith("A") || Element.StartsWith("B") || Element.StartsWith("C") || Element.StartsWith("H") || Element.StartsWith("M") || (Element.StartsWith("V") && Element.Length < 15) || Element.StartsWith("W") || Element.StartsWith("TA") || Element.StartsWith("TB") || Element.StartsWith("TC") || Element.StartsWith("TH") || Element.StartsWith("TM") || (Element.StartsWith("TV") && Element.Length < 15) || Element.StartsWith("TW"))
								bearer = "3G";
						} catch { }
						try {
							if(Element.StartsWith("N") || Element.StartsWith("Q") || Element.StartsWith("R") || Element.StartsWith("ZE") || Element.StartsWith("ZK") || (Element.StartsWith("V") && Element.Length == 15) || Element.StartsWith("TN") || Element.StartsWith("TQ") || Element.StartsWith("TR") || Element.StartsWith("TZE") || Element.StartsWith("TZK"))
								bearer = "4G";
						} catch { }
					}
				}
				return bearer;
			}
			private set { }
		}
		
		[FieldHidden]
		string _parsedAlarm;
		public string ParsedAlarm {
			get {
				if(string.IsNullOrEmpty(_parsedAlarm)) {
					_parsedAlarm = LastOccurrence + " - " + parseSummary(Summary) + Environment.NewLine + RncBsc + " > " + Location;
					if(!string.IsNullOrEmpty(Element))
						_parsedAlarm += " > " + Element;
					_parsedAlarm += Environment.NewLine + "Alarm count: " + AlarmCount;
				}
				return _parsedAlarm;
			}
			private set { }
		}
		
		[FieldHidden]
		string celloperator;
		public string Operator {
			get {
				if(string.IsNullOrEmpty(celloperator))
					celloperator = Element.StartsWith("T")
						|| Element.EndsWith("W")
						|| Element.EndsWith("X")
						|| Element.EndsWith("Y")
						? "TEF" : "VF";
				return celloperator;
			}
			private set { celloperator = value;}
		}
		
		public Alarm() {}

		public Alarm(string[] alarmArray, string[] headers) {
			try { attributes = alarmArray[Array.IndexOf(headers, "Attributes")]; } catch {}
			try { serviceImpact = alarmArray[Array.IndexOf(headers, "Service Impact")]; } catch {}
			try { vendor = alarmArray[Array.IndexOf(headers, "Vendor")]; } catch {}
			try { lastOccurrence = alarmArray[Array.IndexOf(headers, "Last Occurrence")]; } catch {}
			try { alarmCount = alarmArray[Array.IndexOf(headers, "Count")]; } catch {}
			try { rncBsc = alarmArray[Array.IndexOf(headers, "RNC/BSC")]; } catch {}
			try { location = alarmArray[Array.IndexOf(headers, "Location")]; } catch {}
			try { element = alarmArray[Array.IndexOf(headers, "Element")]; } catch {}
			try { summary = alarmArray[Array.IndexOf(headers, "Summary")].Trim(); } catch {}
			try { triagedBy = alarmArray[Array.IndexOf(headers, "Triaged By")]; } catch {}
			try { operatorComments = alarmArray[Array.IndexOf(headers, "Operator Comments")]; } catch {}
			try { vfTtNumber = alarmArray[Array.IndexOf(headers, "VF TT Number")]; } catch {}
			try { vfTtPty = alarmArray[Array.IndexOf(headers, "VF TT Pty")]; } catch {}
			try { assignedGroup = alarmArray[Array.IndexOf(headers, "Assigned Group")]; } catch {}
			try { ttStatus = alarmArray[Array.IndexOf(headers, "TT Status")]; } catch {}
			try { rbsDetails = alarmArray[Array.IndexOf(headers, "RBS Details")]; } catch {}
			try { town = alarmArray[Array.IndexOf(headers, "Town")]; } catch {}
			try { county = alarmArray[Array.IndexOf(headers, "County")]; } catch {}
			try { specialEvent = alarmArray[Array.IndexOf(headers, "Special Event")]; } catch {}
			try { siteType = alarmArray[Array.IndexOf(headers, "Site Type")]; } catch {}
			try { stateChange = alarmArray[Array.IndexOf(headers, "StateChange")]; } catch {}
			try { site = alarmArray[Array.IndexOf(headers, "Site")]; } catch {}
			try { node = alarmArray[Array.IndexOf(headers, "Node")]; } catch {}
			try { nodeAlias = alarmArray[Array.IndexOf(headers, "NodeAlias")]; } catch {}
			try { identifier = alarmArray[Array.IndexOf(headers, "Identifier")]; } catch {}
			try { parentNode = alarmArray[Array.IndexOf(headers, "ParentNode")]; } catch {}
			try { techDomain6 = alarmArray[Array.IndexOf(headers, "TechDomain6")]; } catch {}
			try { poc = alarmArray[Array.IndexOf(headers, "POC")]; } catch {}
			try { intermittent = alarmArray[Array.IndexOf(headers, "Intermittent")]; } catch {}
			try { weightage = alarmArray[Array.IndexOf(headers, "Weightage")]; } catch {}
			try { _class = alarmArray[Array.IndexOf(headers, "Class")]; } catch {}
			try { opState = alarmArray[Array.IndexOf(headers, "OpState")]; } catch {}
		}
		
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
		
		void checkCoosOrOnM() {
			switch (Vendor) {
				case SiteFinder.Site.Vendors.ALU:
					coos = Summary.Contains("UNDERLYING_RESOURCE_UNAVAILABLE: State change to Disable") && (!Element.StartsWith("RNC") || !Location.StartsWith("MTX"));
					break;
				case SiteFinder.Site.Vendors.Ericsson:
					if ((Summary.Contains("CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION") && Summary.Contains("BCCH"))
					    || Summary.Contains("UtranCell_ServiceUnavailable")
					    || Summary.Contains("UtranCell_NbapMessageFailure")
					    || Summary.Contains("NbapCommon_Layer3SetupFailure")
					    || Summary.Contains("Service Unavailable")
					   )
						coos = true;
					else
						onm = Summary.Contains("Heartbeat Failure") || Summary.Contains("OML FAULT");
					break;
				case SiteFinder.Site.Vendors.Huawei:
					if (Summary.Contains("Cell out of Service")
					    || Summary.Contains("Cell Unavailable")
					    || Summary.Contains("Local Cell Unusable")
					   )
						coos = true;
					else
						onm = Summary.Contains("NE Is Disconnected") || Summary.Contains("OML Fault");
					break;
				case SiteFinder.Site.Vendors.NSN:
					if (Summary.Contains("BCCH MISSING")
					    || Summary.Contains("CELL FAULTY")
					    || Summary.Contains("WCDMA CELL OUT OF USE"))
						coos = true;
					else
						onm = Summary.Contains("O&M");
					break;
			}
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

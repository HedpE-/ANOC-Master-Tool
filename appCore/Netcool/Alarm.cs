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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		public Vendors Vendor { get { return getVendor(vendor); } private set { } }
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
                        string temp = string.Empty;
                        switch (Vendor) {
							case Vendors.Huawei:
                                if (Identifier.Contains("Cell Name=")) {
                                    temp = Identifier.Substring(Identifier.IndexOf("Cell Name=") + "Cell Name=".Length);
                                    element = temp.Substring(0, temp.IndexOf(','));
                                }
                                else
                                {
                                    if (Identifier.Contains("Local Cell ID="))
                                    {
                                        temp = Identifier.Substring(Identifier.IndexOf("Local Cell ID="));
                                        temp = temp.Substring(0, temp.IndexOf(','));
                                    }
                                    element = Location + string.Format(" - No cell name on Netcool. ({0})", temp);
                                }
                                break;
							case Vendors.NSN:
        //                        Cell cell = null;
        //                        temp = string.Empty;
        //                        switch (RncBsc.Substring(0, 1)) {
        //                            case "B":
								//	    temp = identifier.Substring(identifier.IndexOf("BtsSiteMgr=") + "BtsSiteMgr=".Length);
								//	    string[] arr = temp.Substring(0, temp.IndexOf(' ')).Replace("GsmCell=", string.Empty).Split(',');
        //                                var sector = Convert.ToInt16(arr[1].RemoveLetters()) - Convert.ToInt16(arr[0].RemoveLetters()) + 1;
        //                                Operators op = Operators.Unknown;
        //                                if (sector > 3)
        //                                {
        //                                    op = Operators.Telefonica;
        //                                    sector -= 3;
        //                                }
        //                                else
        //                                    op = Operators.Vodafone;
        //                                cell = ParentSite.Cells.FirstOrDefault(c => c.WBTS_BCF.RemoveLetters() == arr[0].RemoveLetters() && c.Sector == sector && c.Operator == op);
        //                                try { element = cell.Name; } catch { element = Location + " - No cell description on Netcool."; }
        //                                break;
        //                            case "R":
        //                                var wCel = NodeAlias.Substring(nodeAlias.IndexOf("UtranCell=") + "UtranCell=".Length).Split('/');
        //                                var wCelArr = wCel[1].Substring(wCel[1].Length - 3).ToCharArray();
        //                                List<Cell> results = ParentSite.Cells
        //                                    .Filter(Cell.Filters.All_3G)
        //                                    .Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(new string(new[] { wCelArr[0], wCelArr[1] })) && c.Operator == Operators.Vodafone)
        //                                    .ToList();
        //                                if(!results.Any())
        //                                {
        //                                    switch(wCelArr[1])
        //                                    {
        //                                        case '7':
        //                                            results = ParentSite.Cells.Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(wCelArr[0] + "5") && c.Operator == Operators.Telefonica).ToList();
        //                                            break;
        //                                        case '8':
        //                                            results = ParentSite.Cells.Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(wCelArr[0] + "2") && c.Operator == Operators.Telefonica).ToList();
        //                                            break;
        //                                        case '9':
        //                                            results = ParentSite.Cells.Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(wCelArr[0] + "1") && c.Operator == Operators.Telefonica).ToList();
        //                                            break;
        //                                    }
        //                                }

        //                                if (results.Count == 1)
        //                                    element = results[0].Name;

        //                                if(string.IsNullOrEmpty(element))
        //                                    element = Location + " - No cell description on Netcool.";
        //                                break;
        //                            default:
        //                                if(node.StartsWith("LNCEL"))
        //                                    cell = ParentSite.Cells.FirstOrDefault(c => c.Id == Node.RemoveLetters()) ?? ParentSite.Cells.FirstOrDefault(c => c.Id == Node.RemoveLetters() + "0");
        //                                element = cell != null ? cell.Name :
        //                                    Location + " - No cell description on Netcool.";
        //                                break;
								//}
                                element = Location + " - No cell name on Netcool.";
                                break;
							case Vendors.Ericsson:
								switch(RncBsc.Substring(0,1)) {
									case "B":
										string GCellId = Summary.Substring(Summary.IndexOf("CELL =  ") + 8);
										element = GCellId.Substring(0, GCellId.IndexOf(" ("));
										break;
									case "R":
										string UCellId = Summary.Substring(Summary.IndexOf("UtranCell=") + 10);
                                        List<Cell> results = Task.Run(() => DB.SitesDB.QueryAllCellsDB("CELL_ID", UCellId)).Result;
										foreach(Cell c2 in results)
											if(c2.BscRnc_Id == RncBsc && c2.Vendor == Vendor)
												element = c2.Name;
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
					siteid = Location.Contains("RBS") ? Location : string.Empty;
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
                {
                    while (GettingParentSiteFlag) { }

                    GettingParentSiteFlag = true;
                    _site = Task.Run(() => DB.SitesDB.GetSiteAsync(SiteId)).Result;
                    GettingParentSiteFlag = false;
                }
                return _site;
			}
			private set {
				_site = value;
			}
        }
        [FieldHidden]
        bool GettingParentSiteFlag;
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

		public Bearers Bearer {
			get {
				if (!string.IsNullOrEmpty(RncBsc)) {
					switch(RncBsc.Substring(0, 1)) {
						case "B":
							return Bearers.GSM;
						case "R":
							return Bearers.UMTS;
						case "X":
							return Bearers.LTE;
					}
				}

                if(!Element.Contains("No cell"))
                {
				    try {
					    if(Element.StartsWith("D") || Element.StartsWith("G") || Element.StartsWith("I") || Element.StartsWith("P") || Element.StartsWith("S") || Element.StartsWith("U") || Element.StartsWith("TD") || Element.StartsWith("TG") || Element.StartsWith("TI") || Element.StartsWith("TP") || Element.StartsWith("TS") || Element.StartsWith("TU"))
						    return Bearers.GSM;
				    } catch { }
				    try {
					    if(Element.StartsWith("A") || Element.StartsWith("B") || Element.StartsWith("C") || Element.StartsWith("H") || Element.StartsWith("M") || (Element.StartsWith("V") && Element.Length < 15) || Element.StartsWith("W") || Element.StartsWith("TA") || Element.StartsWith("TB") || Element.StartsWith("TC") || Element.StartsWith("TH") || Element.StartsWith("TM") || (Element.StartsWith("TV") && Element.Length < 15) || Element.StartsWith("TW"))
						    return Bearers.UMTS;
				    } catch { }
				    try {
					    if(Element.StartsWith("N") || Element.StartsWith("Q") || Element.StartsWith("R") || Element.StartsWith("ZE") || Element.StartsWith("ZK") || (Element.StartsWith("V") && Element.Length == 15) || Element.StartsWith("TN") || Element.StartsWith("TQ") || Element.StartsWith("TR") || Element.StartsWith("TZE") || Element.StartsWith("TZK"))
						    return Bearers.LTE;
				    } catch { }
                }

                return Bearers.Unknown;
			}
		}
		
		[FieldHidden]
		string _parsedAlarm;
		public string ParsedAlarm {
			get {
                if (string.IsNullOrEmpty(_parsedAlarm))
                    UpdateParsedAlarm();

				return _parsedAlarm;
			}
			private set { }
		}
		
		public Operators Operator {
			get {
				return Element.StartsWith("T")
					|| Element.EndsWith("W")
					|| Element.EndsWith("X")
					|| Element.EndsWith("Y")
					? Operators.Telefonica : Operators.Vodafone;
			}
		}
		
		public Alarm() {}

		public Alarm(string[] alarmArray, string[] headers)
        {
			try
            {
				attributes = alarmArray[Array.IndexOf(headers, "Attributes")];
				serviceImpact = alarmArray[Array.IndexOf(headers, "Service Impact")];
				vendor = alarmArray[Array.IndexOf(headers, "Vendor")];
				lastOccurrence = alarmArray[Array.IndexOf(headers, "Last Occurrence")];
				alarmCount = alarmArray[Array.IndexOf(headers, "Count")];
				rncBsc = alarmArray[Array.IndexOf(headers, "RNC/BSC")];
				location = alarmArray[Array.IndexOf(headers, "Location")];
                element = alarmArray[Array.IndexOf(headers, "Element")];
				summary = alarmArray[Array.IndexOf(headers, "Summary")].Trim();
				triagedBy = alarmArray[Array.IndexOf(headers, "Triaged By")];
				operatorComments = alarmArray[Array.IndexOf(headers, "Operator Comments")];
				vfTtNumber = alarmArray[Array.IndexOf(headers, "VF TT Number")];
				vfTtPty = alarmArray[Array.IndexOf(headers, "VF TT Pty")];
				assignedGroup = alarmArray[Array.IndexOf(headers, "Assigned Group")];
				ttStatus = alarmArray[Array.IndexOf(headers, "TT Status")];
				rbsDetails = alarmArray[Array.IndexOf(headers, "RBS Details")];
				town = alarmArray[Array.IndexOf(headers, "Town")];
				county = alarmArray[Array.IndexOf(headers, "County")].ToUpper().Replace("COUNTY ", string.Empty);
                if (string.IsNullOrEmpty(county) || county.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
                    county = ParentSite.County;
                specialEvent = alarmArray[Array.IndexOf(headers, "Special Event")];
				siteType = alarmArray[Array.IndexOf(headers, "Site Type")];
				stateChange = alarmArray[Array.IndexOf(headers, "StateChange")];
				site = alarmArray[Array.IndexOf(headers, "Site")];
				node = alarmArray[Array.IndexOf(headers, "Node")];
				nodeAlias = alarmArray[Array.IndexOf(headers, "NodeAlias")];
				identifier = alarmArray[Array.IndexOf(headers, "Identifier")];
				parentNode = alarmArray[Array.IndexOf(headers, "ParentNode")];
				techDomain6 = alarmArray[Array.IndexOf(headers, "TechDomain6")];
				poc = alarmArray[Array.IndexOf(headers, "POC")];
				intermittent = alarmArray[Array.IndexOf(headers, "Intermittent")];
				weightage = alarmArray[Array.IndexOf(headers, "Weightage")];
				_class = alarmArray[Array.IndexOf(headers, "Class")];
				opState = alarmArray[Array.IndexOf(headers, "OpState")];
			}
			catch {
				throw new Exception("Required headers not found");
			}
		}
		
		public Alarm(Cell cell, bool alarmCOOS, DateTime alarmTime) {
			rncBsc = cell.BscRnc_Id;
			lastOccurrence = alarmTime.ToString();
			element = cell.Name;
			coos = alarmCOOS;
			//bearer = cell.Bearer;
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
			//bearer = cell.Bearer;
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
				case Vendors.ALU:
					coos = Summary.Contains("UNDERLYING_RESOURCE_UNAVAILABLE: State change to Disable") && (!Element.StartsWith("RNC") || !Location.StartsWith("MTX"));
					break;
				case Vendors.Ericsson:
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
				case Vendors.Huawei:
					if (Summary.Contains("Cell out of Service")
					    || Summary.Contains("Cell Unavailable")
					    || Summary.Contains("Local Cell Unusable")
					   )
						coos = true;
					else
						onm = Summary.Contains("NE Is Disconnected") || Summary.Contains("OML Fault");
					break;
				case Vendors.NSN:
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
			toParse = toParse.Replace("P1", string.Empty)
				.Replace("P3", string.Empty)
				.Replace("RBS", string.Empty)
				.Replace("NODEB", string.Empty)
				.Replace("ENODEB", string.Empty)
				.Replace("SITE:", string.Empty)
				.Trim();
			
			if(toParse.Contains("CELL LOGICAL")) {
				var t = toParse.IndexOf("SUPERVISION", StringComparison.OrdinalIgnoreCase) + "SUPERVISION".Length;
				int channelsStartPos = toParse.IndexOf("(", StringComparison.Ordinal) + 1;
				toParse = toParse.Replace(toParse.Substring(t, (channelsStartPos - 1) - t), " ");
				
				channelsStartPos = toParse.IndexOf("(", StringComparison.Ordinal) + 1;
				int channelsEndPos = toParse.Substring(channelsStartPos).Contains("END") ? toParse.IndexOf("END", StringComparison.Ordinal) : toParse.IndexOf(")", StringComparison.Ordinal);
				/*
				P3 RBS SITE: CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION: CELL =  G10901X (BCCH SDCCH TCH)
				P3 RBS SITE: CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION: CELL =  G12611 (TCH FR 1)
				P1 RBS SITE: CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION: CELL =  G78007Y (CBCH END -ProbableCause(OSS)=SUBSCRIBER)
				 */
				
				var channels = toParse.Substring(channelsStartPos, channelsEndPos - channelsStartPos)
					.Replace("FR", string.Empty)
					.Replace("HR", string.Empty)
					.RemoveDigits()
					.Trim()
					.Split(' ');
				
				if(channels.Length > 0) {
					if(!channels.Contains("BCCH"))
						toParse += Environment.NewLine + string.Join("/", channels) + " Cell Degraded (" + Element + ")";
				}
				return toParse;
			}
			
			if ( toParse.Contains("RADIO X-CEIVER ADMINISTRATION MANAGED OBJECT FAULT") )
			{
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

        public void ResolveNsnElement(Site parentSite)
        {
            ParentSite = parentSite;
            Cell cell = null;
            string temp = string.Empty;
            switch (RncBsc.Substring(0, 1))
            {
                case "B":
                    temp = identifier.Substring(identifier.IndexOf("BtsSiteMgr=") + "BtsSiteMgr=".Length);
                    string[] arr = temp.Substring(0, temp.IndexOf(' ')).Replace("GsmCell=", string.Empty).Split(',');
                    var sector = Convert.ToInt16(arr[1].RemoveLetters()) - Convert.ToInt16(arr[0].RemoveLetters()) + 1;
                    Operators op = Operators.Unknown;
                    if (sector > 3)
                    {
                        op = Operators.Telefonica;
                        sector -= 3;
                    }
                    else
                        op = Operators.Vodafone;
                    cell = ParentSite.Cells.FirstOrDefault(c => c.WBTS_BCF.RemoveLetters() == arr[0].RemoveLetters() && c.Sector == sector && c.Operator == op);
                    try { element = cell.Name; } catch { element = Location + " - No cell description on Netcool."; }
                    break;
                case "R":
                    var wCel = NodeAlias.Substring(nodeAlias.IndexOf("UtranCell=") + "UtranCell=".Length).Split('/');
                    var wCelArr = wCel[1].Substring(wCel[1].Length - 3).ToCharArray();
                    List<Cell> results = ParentSite.Cells
                        .Filter(Cell.Filters.All_3G)
                        .Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(new string(new[] { wCelArr[0], wCelArr[1] })) && c.Operator == Operators.Vodafone)
                        .ToList();
                    if (!results.Any())
                    {
                        switch (wCelArr[1])
                        {
                            case '7':
                                results = ParentSite.Cells.Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(wCelArr[0] + "5") && c.Operator == Operators.Telefonica).ToList();
                                break;
                            case '8':
                                results = ParentSite.Cells.Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(wCelArr[0] + "2") && c.Operator == Operators.Telefonica).ToList();
                                break;
                            case '9':
                                results = ParentSite.Cells.Where(c => c.WBTS_BCF == wCel[0] && c.Name.EndsWith(wCelArr[0] + "1") && c.Operator == Operators.Telefonica).ToList();
                                break;
                        }
                    }

                    if (results.Count == 1)
                        element = results[0].Name;

                    if (string.IsNullOrEmpty(element))
                        element = Location + " - No cell description on Netcool.";
                    break;
                default:
                    if (node.StartsWith("LNCEL"))
                        cell = ParentSite.Cells.FirstOrDefault(c => c.Id == Node.RemoveLetters()) ?? ParentSite.Cells.FirstOrDefault(c => c.Id == Node.RemoveLetters() + "0");
                    element = cell != null ? cell.Name :
                        Location + " - No cell description on Netcool.";
                    break;
            }
            UpdateParsedAlarm();
        }

        public void ResolveHuaweiElement(Site parentSite)
        {
            ParentSite = parentSite;
            Cell cell = null;
            string temp = string.Empty;
            switch (RncBsc.Substring(0, 1))
            {
                case "B":
                    break;
                case "R":
                    break;
                default:
                    break;
            }
            UpdateParsedAlarm();
        }

        void UpdateParsedAlarm()
        {
            _parsedAlarm = LastOccurrence + " - " + parseSummary(Summary) + Environment.NewLine + RncBsc + " > " + Location;
            if (!string.IsNullOrEmpty(Element))
                _parsedAlarm += " > " + Element;
            _parsedAlarm += Environment.NewLine + "Alarm count: " + AlarmCount;
        }

        Vendors getVendor(string strVendor) {
			switch (strVendor.ToUpper()) {
				case "ERICSSON":
					return Vendors.Ericsson;
				case "HUAWEI":
					return Vendors.Huawei;
				case "ALU":
					return Vendors.ALU;
				case "NSN":
					return Vendors.NSN;
				default:
					return Vendors.Unknown;
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

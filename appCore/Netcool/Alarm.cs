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
using System.Linq;
using appCore.SiteFinder;

namespace appCore.Netcool
{
	/// <summary>
	/// Description of Alarm.
	/// </summary>
	public class Alarm
	{
		DateTime lastOccurrence;
		public DateTime LastOccurrence { get { return lastOccurrence; } protected set { lastOccurrence = value; } }
		string summary;
		public string Summary { get { return summary; } protected set { summary = value; } }
		string rncBsc;
		public string RncBsc { get { return rncBsc; } protected set { rncBsc = value; } }
		string location;
		public string Location { get { return location; } protected set { location = value; } }
		string element;
		public string Element { get { return element; } protected set { element = value; } }
		string alarmCount;
		public string AlarmCount { get { return alarmCount; } protected set { alarmCount = value; } }
		Site.Vendors vendor;
		public Site.Vendors Vendor { get { return vendor; } protected set { vendor = value; } }
		string county;
		public string County { get { return county; } protected set { county = value; } }
		string town;
		public string Town { get { return town; } protected set { town = value; } }
		string poc;
		public string POC { get { return poc; } protected set { poc = value; } }
		string node;
		public string Node { get { return node; } protected set { node = value; } }
		string identifier;
		public string Identifier { get { return identifier; } protected set { identifier = value; } }
		string[] attributes;
		public string[] Attributes { get { return attributes; } protected set { attributes = value; } }
		Site siteId;
		public Site SiteId { get { return siteId; } protected set { siteId = value; } }
		public bool COOS { get; protected set; }
		public bool OnM { get; protected set; }
		public string Bearer { get; protected set; }
		
		string _parsedAlarm = string.Empty;
		public string ParsedAlarm {
			get {
				return _parsedAlarm;
			}
			private set {
				_parsedAlarm = value;
			}
		}
		
		public Alarm(DataRow alarm, DataColumnCollection columns) {
			int lastoccurIndex = columns.Contains("Last Occurrence") ? columns["Last Occurrence"].Ordinal : columns["LastOccurrence"].Ordinal; // Encontrar a posição da column Last Occurence
			int switchIndex = columns["RNC/BSC"].Ordinal; // Encontrar a prosição da column RNC/BSC
			int locationIndex = columns["Location"].Ordinal; // Encontrar a prosição da column Location
			int elementIndex = columns["Element"].Ordinal; // Encontrar a prosição da column Element
			int summaryIndex = columns["Summary"].Ordinal; // Encontrar a prosição da column Summary
			int countIndex = columns["Count"].Ordinal; // Encontrar a prosição da column Count
			int attributesIndex = columns["Attributes"].Ordinal; // Encontrar a prosição da column Attributes
			int vendorIndex = columns["Vendor"].Ordinal; // Encontrar a prosição da column Vendor
			int townIndex = columns["Town"].Ordinal; // Encontrar a prosição da column Town
			int countyIndex = columns["County"].Ordinal; // Encontrar a prosição da column County
			int pocIndex = columns["Poc"].Ordinal; // Encontrar a prosição da column Poc
			int nodeIndex = columns["Node"].Ordinal; // Encontrar a prosição da column Node
			int identifierIndex = columns["Identifier"].Ordinal; // Encontrar a prosição da column Identifier
			
			LastOccurrence = Convert.ToDateTime(alarm.ItemArray[lastoccurIndex]);
			Summary = parseSummary(alarm.ItemArray[summaryIndex].ToString());
			RncBsc = alarm.ItemArray[switchIndex].ToString();
			Location = alarm.ItemArray[locationIndex].ToString();
			Element = alarm.ItemArray[elementIndex].ToString();
			AlarmCount = alarm.ItemArray[countIndex].ToString();
			Vendor = getVendor(alarm.ItemArray[vendorIndex].ToString());
			Attributes = alarm.ItemArray[attributesIndex].ToString().Split('/');
			County = alarm.ItemArray[countyIndex].ToString();
			Town = alarm.ItemArray[townIndex].ToString();
			POC = alarm.ItemArray[pocIndex].ToString();
			Node = alarm.ItemArray[nodeIndex].ToString();
			Identifier = alarm.ItemArray[identifierIndex].ToString();
			
			if(string.IsNullOrEmpty(Element))
				ResolveCellName();
			
			Bearer = resolveAlarmBearer();

			checkCoosOrOnM();
			
			ParsedAlarm = LastOccurrence + " - " + Summary + Environment.NewLine + RncBsc + " > " + Location + " > " + Element + Environment.NewLine + "Alarm count: " + AlarmCount;
		}
		
		public Alarm(Cell cell, bool coos, DateTime alarmTime) {
			RncBsc = cell.BscRnc_Id;
			LastOccurrence = alarmTime;
			Element = cell.Name;
			COOS = coos;
			Bearer = cell.Bearer;
			Site site = Finder.getSite(cell.ParentSite);
			string temp = site.Id;
			while(temp.Length < 5)
				temp = "0" + temp;
			Location = "RBS" + temp;
			
			switch (cell.Bearer) {
				case "2G":
					Vendor = cell.Vendor2G;
					break;
				case "3G":
					Vendor = cell.Vendor3G;
					break;
				case "4G":
					Vendor = cell.Vendor4G;
					break;
			}
			
			County = site.County;
			Town = site.Town;
			ParsedAlarm = LastOccurrence + " - " + Summary + Environment.NewLine + RncBsc + " > " + Location + " > " + Element + Environment.NewLine + "Alarm count: " + AlarmCount;
		}
		
		void resolveAddressFromCell(Cell cell) {
			Site site = Finder.getSite(cell.ParentSite);
			County = site.County;
			Town = site.Town;
		}
		
		void checkCoosOrOnM() { // FIXME: O&M alarms for proper filtering
			switch (Vendor) {
				case Site.Vendors.ALU:
					if(Summary.Contains("UNDERLYING_RESOURCE_UNAVAILABLE: State change to Disable"))
						COOS = true;
					break;
				case Site.Vendors.Ericsson:
					if ((Summary.Contains("CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION") && Summary.Contains("BCCH")) || Summary.Contains("UtranCell_ServiceUnavailable"))
						COOS = true;
					else
						OnM = Summary.Contains("4G: Heartbeat Failure");
					break;
				case Site.Vendors.Huawei:
					if (Summary.Contains("Cell out of Service") || Summary.Contains("Cell Unavailable") || Summary.Contains("Local Cell Unusable"))
						COOS = true;
					else
						OnM = Summary.Contains("eNodeB");
					break;
				case Site.Vendors.NSN:
					if (!(Summary.Contains("BCCH MISSING") || Summary.Contains("CELL FAULTY") || Summary.Contains("WCDMA CELL OUT OF USE")))
						COOS = true;
					else
						OnM = Summary.Contains("ENODEB: NE O&M");
					break;
			}
		}
		
		string resolveAlarmBearer() {
			if(!string.IsNullOrEmpty(RncBsc)) {
				switch(RncBsc.Substring(0)) {
					case "B":
						return "2G";
					case "R":
						return "3G";
					case "X":
						if(Location.StartsWith("V"))
							return "4G";
						break;
				}
			}
			else {
				if(!string.IsNullOrEmpty(Element)) {
					Cell tempCell = Finder.getCell(Element);
					return tempCell.Bearer;
				}
			}
			return null;
		}
		
		string parseSummary(string toParse)
		{
//			toParse = toParse.TrimStart(' ');
//			toParse = toParse.TrimEnd(' ');
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
		
		public bool ResolveCellName() {
			if(string.IsNullOrEmpty(Element)) {
				switch(Vendor) {
					case Site.Vendors.Huawei:
						if(Identifier.Contains("Cell Name=")) {
							string[] temp = Identifier.Split(',');
							for(int c = 0;c < temp.Length;c++) {
								if(temp[c].Contains("Cell Name=")) {
									temp[c] = temp[c].Substring(temp[c].IndexOf("=") + 1);
//									temp[c] = temp[c].Remove(temp[c].IndexOf(","));
									Element = temp[c];
									break;
								}
							}
						}
						else
							Element = Node;
						break;
					case Site.Vendors.NSN:
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
						Element = elementID;
						break;
				}
				
				return true;
			}
			
			return false;
		}
		
		Site.Vendors getVendor(string strVendor) {
			switch (strVendor.ToUpper()) {
				case "ERICSSON":
					return Site.Vendors.Ericsson;
				case "HUAWEI":
					return Site.Vendors.Huawei;
				case "ALU":
					return Site.Vendors.ALU;
				case "NSN":
					return Site.Vendors.NSN;
				default:
					return Site.Vendors.None;
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

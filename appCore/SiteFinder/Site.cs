/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 09-01-2017
 * Time: 22:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Data;
using System.IO;
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using appCore.Toolbox;
using appCore.Web;
using HtmlAgilityPack;
using FileHelpers;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Site2.
	/// </summary>
	[DelimitedRecord(","), IgnoreFirst(1)]
	public partial class Site
	{
		[FieldOrder(1)]
		string SITE;
		public string Id { get { return SITE; } private set { SITE = value; } }
		[FieldOrder(2)]
		string JVCO_ID;
		public string JVCO_Id { get { return JVCO_ID; } private set { JVCO_ID = value; } }
		[FieldOrder(3)]
		public string GSM900;
		[FieldOrder(4)]
		public string GSM1800;
		[FieldOrder(5)]
		public string UMTS900;
		[FieldOrder(6)]
		public string UMTS2100;
		[FieldOrder(7)]
		public string LTE800;
		[FieldOrder(8)]
		public string LTE2600;
		[FieldOrder(9)]
		public string LTE2100;
		[FieldOrder(10)]
		double Easting;
		[FieldOrder(11)]
		double Northing;
		[FieldHidden]
		LLPoint coordinates;
		public LLPoint Coordinates {
			get {
				if(coordinates == null)
					coordinates = coordConvert.toLat_Long(new CoordPoint { Easting = Easting, Northing = Northing }, "OSGB36");
				return coordinates;
			}
			private set { }
		}
		[FieldOrder(12)]
		public string Host;
		[FieldOrder(13)]
		public string Priority;
		[FieldOrder(14)]
		string ADDRESS;
		public string Address {
			get {
				return ADDRESS.Replace(';',',');
			}
			private set {
				ADDRESS = value;
				SplitAddress();
			}
		}
		[FieldHidden]
		string postCode;
		public string PostCode {
			get {
				if(string.IsNullOrEmpty(postCode))
					SplitAddress();
				return postCode;
			}
			private set { }
		}
		[FieldHidden]
		string town;
		public string Town {
			get {
				if(string.IsNullOrEmpty(town))
					SplitAddress();
				return town;
			}
			private set { }
		}
		[FieldHidden]
		string county;
		public string County {
			get {
				if(string.IsNullOrEmpty(county))
					SplitAddress();
				return county;
			}
			private set { }
		}
		[FieldOrder(15)]
		public string TellabsAtRisk;
		[FieldOrder(16)]
		public string Area;
		[FieldOrder(17)]
		public string NSN_Status;
		[FieldOrder(18)]
		public string NOC2G;
		[FieldOrder(19)]
		public string NOC3G;
		[FieldOrder(20)]
		public string NOC4G;
		[FieldOrder(21)]
		public string Region;
		[FieldOrder(22)]
		public string Special_Id;
		[FieldOrder(23)]
		public string Special;
		[FieldOrder(24)]
		string SPECIAL_START;
		public DateTime SpecialEvent_StartDate {
			get { return Convert.ToDateTime(SPECIAL_START); }
			private set { SPECIAL_START = value.ToString(); }
		}
		[FieldOrder(25)]
		string SPECIAL_END;
		public DateTime SpecialEvent_EndDate {
			get { return Convert.ToDateTime(SPECIAL_END); }
			private set { SPECIAL_END = value.ToString(); }
		}
		[FieldOrder(26)]
		public string VIP;
		[FieldOrder(27)]
		public string SharedOperator;
		[FieldOrder(28)]
		public string SharedOperatorSiteID;
		[FieldOrder(29)]
		public string Site_Access;
		[FieldOrder(30)]
		public string Site_Type;
		[FieldOrder(31)]
		public string Site_Subtype;
		[FieldOrder(32)]
		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (bool), "false")]
		public bool Paknet_Fitted;
		[FieldOrder(33)]
		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (bool), "false")]
		public bool Vodapage_Fitted;
		[FieldOrder(34)]
		public string DC_STATUS;
		[FieldOrder(35)]
		string dc_Timestamp;
		public DateTime DC_Timestamp {
			get { return Convert.ToDateTime(dc_Timestamp); }
			private set { dc_Timestamp = value.ToString(); }
		}
		[FieldOrder(36)]
		public string Cooling_Status;
		[FieldOrder(37)]
		public string cooling_Timestamp;
		public DateTime Cooling_Timestamp {
			get { return Convert.ToDateTime(cooling_Timestamp); }
			private set { cooling_Timestamp = value.ToString(); }
		}
		[FieldOrder(38)]
		string keyInformation;
		public string KeyInformation {
			get {
				keyInformation = keyInformation.Trim();
				while(keyInformation.Contains("&#")) {
					string character = keyInformation.Substring(keyInformation.IndexOf("&#"), 6);
					keyInformation = keyInformation.Replace(character, ((char)Convert.ToInt16(character.Substring(2, 3))).ToString());
				}
				keyInformation = keyInformation.Replace(';', ',').Replace("   ", Environment.NewLine + Environment.NewLine).Replace("  ", Environment.NewLine);
				
				return keyInformation;
			}
			private set {
				keyInformation = value;
			}
		}
		[FieldOrder(39)]
		string EF_HealthAndSafety;
		public string HealthAndSafety {
			get {
				EF_HealthAndSafety = EF_HealthAndSafety.Trim().Replace("&quot;", '"'.ToString());
				
				return EF_HealthAndSafety;
			}
			private set {
				EF_HealthAndSafety = value;
			}
		}
		[FieldOrder(40)]
		public string Switch2G;
		[FieldOrder(41)]
		public string Switch3G;
		[FieldOrder(42)]
		public string Switch4G;
		[FieldOrder(43)]
		public string DRSwitch2G;
		[FieldOrder(44)]
		public string DRSwitch3G;
		[FieldOrder(45)]
		public string DRSwitch4G;
		[FieldOrder(46)]
		public string MTX2G;
		[FieldOrder(47)]
		public string MTX3G;
		[FieldOrder(48)]
		public string MTX4G;
		[FieldOrder(49)]
		public string IP_2G_I;
		[FieldOrder(50)]
		public string IP_2G_E;
		[FieldOrder(51)]
		public string IP_3G_I;
		[FieldOrder(52)]
		public string IP_3G_E;
		[FieldOrder(53)]
		public string IP_4G_I;
		[FieldOrder(54)]
		public string IP_4G_E;
		[FieldOrder(55)]
		string VENDOR_2G;
		public Site.Vendors Vendor2G {
			get { return resolveVendor(VENDOR_2G); }
			private set { VENDOR_2G = value.ToString(); }
		}
		[FieldOrder(56)]
		string VENDOR_3G;
		public Site.Vendors Vendor3G {
			get { return resolveVendor(VENDOR_3G); }
			private set { VENDOR_3G = value.ToString(); }
		}
		[FieldOrder(57)]
		string VENDOR_4G;
		public Site.Vendors Vendor4G {
			get { return resolveVendor(VENDOR_4G); }
			private set { VENDOR_4G = value.ToString(); }
		}
		[FieldOrder(58)]
		string DATE;
		public DateTime DeploymentDate {
			get { return Convert.ToDateTime(DATE); }
			private set { DATE = value.ToString(); }
		}
		[FieldOrder(59)]
		public string MTX_Related;
		
		public bool Exists {
			get { return !string.IsNullOrEmpty(JVCO_Id); }
			private set { }
		}
		[FieldHidden]
		string POWER_COMPANY;
		[FieldHidden]
		string POWER_CONTACT;
		public string PowerCompany {
			get {
				return (POWER_COMPANY + " " + POWER_CONTACT).Trim();
			}
			private set {
				string[] temp = value.Split(';');
				POWER_COMPANY = temp[0].Trim();
				POWER_CONTACT = temp[1].Trim();
			}
		}
		
		[FieldHidden]
		public DataTable ActiveAlarms;
		[FieldHidden]
		DateTime ActiveAlarmsTimestamp;
		[FieldHidden]
		public DataTable INCs;
		[FieldHidden]
		DateTime INCsTimestamp;
		[FieldHidden]
		public DataTable CRQs;
		[FieldHidden]
		DateTime CRQsTimestamp;
		[FieldHidden]
		public DataTable BookIns;
		[FieldHidden]
		DateTime BookInsTimestamp;
		[FieldHidden]
		public DataTable Availability;
		[FieldHidden]
		DateTime AvailabilityTimestamp;
		[FieldHidden]
		public DataTable LockedCellsDetails;
		[FieldHidden]
		DateTime LockedCellsDetailsTimestamp;
		[FieldHidden]
		string indexPhp;
		[FieldHidden]
		DateTime indexPhpTimestamp;
		[FieldHidden]
		int indexPhpMaxLifetimeMin = 5;
		
		[FieldHidden]
		GMarkerGoogle mapMarker;
		public GMarkerGoogle MapMarker {
			get {
				if(mapMarker == null) {
					mapMarker = new GMarkerGoogle(new PointLatLng(Coordinates.Latitude, Coordinates.Longitude), GMarkerGoogleType.red);
					mapMarker.Tag = Id;
					mapMarker.ToolTip = new GMapBaloonToolTip(MapMarker);
					mapMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
					mapMarker.ToolTip.Fill = new SolidBrush(Color.FromArgb(180, Color.Black));
					mapMarker.ToolTip.Font  = new Font("Courier New", 9, FontStyle.Bold);
					mapMarker.ToolTip.Foreground = new SolidBrush(Color.White);
					mapMarker.ToolTip.Stroke = new Pen(Color.Red);
					mapMarker.ToolTip.Offset.X -= 15;
					mapMarker.ToolTipText = Id;
				}
				return mapMarker;
			}
			private set { }
		}
		
		[FieldHidden]
		List<Cell> cells;
		public List<Cell> Cells {
			get {
				return cells;
			}
			private set {
				cells = value;
			}
		}
		[FieldHidden]
		List<Cell> cellsInOutage;
		public List<Cell> CellsInOutage {
			get {
				return cellsInOutage;
			}
			set {
				cellsInOutage = value;
			}
		}
		
		public Site() {
		}
		
		/// <summary>
		/// Populate with data pulled from OI
		/// </summary>
		/// <param name="dataToRequest">"INC", "CRQ", "Bookins", "Alarms", "Availability", "PWR", "LKULK"</param>
		/// <param name="forceUpdateIndexPhp"></param>
		public void requestOIData(string dataToRequest, bool forceUpdateIndexPhp = false) {
			if(Exists) {
				List<Thread> threads = new List<Thread>();
				int finishedThreadsCount = 0;
				if(dataToRequest.Contains("INC")) {
					Thread thread = new Thread(() => {
					                           	INCs = FetchINCs();
					                           	finishedThreadsCount++;
					                           });
					threads.Add(thread);
				}
				
				if(dataToRequest.Contains("CRQ")) {
					Thread thread = new Thread(() => {
					                           	CRQs = FetchCRQs();
					                           	finishedThreadsCount++;
					                           });
					threads.Add(thread);
				}
				
				if(dataToRequest.Contains("Alarms")) {
					Thread thread = new Thread(() => {
					                           	ActiveAlarms = FetchActiveAlarms();
					                           	finishedThreadsCount++;
					                           });
					threads.Add(thread);
				}
				
				if(dataToRequest.Contains("Bookins")) {
					Thread thread = new Thread(() => {
					                           	BookIns = FetchBookIns();
					                           	finishedThreadsCount++;
					                           });
					threads.Add(thread);
				}
				
				if(dataToRequest.Contains("Availability")) {
					Thread thread = new Thread(() => {
					                           	Availability = FetchAvailability();
					                           	finishedThreadsCount++;
					                           });
					threads.Add(thread);
				}
				
				if(dataToRequest.Contains("PWR")) {
					Thread thread = new Thread(() => {
					                           	if(string.IsNullOrEmpty(indexPhp) || forceUpdateIndexPhp) {
					                           		indexPhp = getOiSiteIndexPage();
					                           	}
					                           	else {
					                           		if((DateTime.Now - indexPhpTimestamp).Minutes > indexPhpMaxLifetimeMin) {
					                           			indexPhp = getOiSiteIndexPage();
					                           		}
					                           	}
					                           	if(string.IsNullOrWhiteSpace(PowerCompany))
					                           		try { PowerCompany = getPowerCompany(); } catch {}
					                           	
					                           	finishedThreadsCount++;
					                           });
					threads.Add(thread);
				}
				
				if(dataToRequest.Contains("LKULK")) {
					Thread thread = new Thread(() => {
					                           	if(string.IsNullOrEmpty(indexPhp) || (forceUpdateIndexPhp && !dataToRequest.Contains("PWR")))
					                           		indexPhp = getOiSiteIndexPage();
					                           	else {
					                           		if((DateTime.Now - indexPhpTimestamp).Minutes > indexPhpMaxLifetimeMin)
					                           			indexPhp = getOiSiteIndexPage();
					                           	}
					                           	
					                           	if(LockedCellsDetails == null)
					                           		getOiCellsLockedState(true);
					                           	
					                           	finishedThreadsCount++;
					                           });
					threads.Add(thread);
				}
				
				foreach(Thread thread in threads) {
					thread.SetApartmentState(ApartmentState.STA);
					thread.Start();
				}
				
				while(finishedThreadsCount < threads.Count) { }
			}
		}
		
		DataTable FetchINCs(FileSystemInfo table_inc = null) {
			DataTable dt = new DataTable();
			string response;
			response = OIConnection.requestPhpOutput("inc", Id);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No open or incidents")) {
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_inc");
				INCsTimestamp = DateTime.Now;
			}
			return dt;
		}
		
		DataTable FetchCRQs(FileSystemInfo table_crq = null) {
			DataTable dt = new DataTable();
			string response;
			response = OIConnection.requestPhpOutput("crq", Id);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No changes in past 90 days")) {
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_crq");
				CRQsTimestamp = DateTime.Now;
			}
			return dt;
		}
		
		DataTable FetchActiveAlarms(FileSystemInfo table_alarms = null) {
			DataTable dt = new DataTable();
			string response;
			response = OIConnection.requestPhpOutput("alarms", Id);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No alarms reported")) {
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_alarms");
				ActiveAlarmsTimestamp = DateTime.Now;
			}
			return dt;
		}
		
		DataTable FetchBookIns(FileSystemInfo table_visits = null) {
			DataTable dt = new DataTable();
			string response;
			response = OIConnection.requestPhpOutput("sitevisit", Id, 90);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No site visits")) {
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_visits");
				BookInsTimestamp = DateTime.Now;
			}
			return dt;
		}
		
		DataTable FetchAvailability(FileSystemInfo table_ca = null) {
			DataTable dt = new DataTable();
			string response;
			response = OIConnection.requestPhpOutput("ca", Id);
			if(!string.IsNullOrEmpty(response)) {
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_ca");
				AvailabilityTimestamp = DateTime.Now;
			}
			return dt;
		}
		
		string getOiSiteIndexPage() {
			string response = OIConnection.requestPhpOutput("index", Id, string.Empty);
			indexPhpTimestamp = DateTime.Now;
			return response;
		}
		
		string getPowerCompany() {
			if(!string.IsNullOrEmpty(indexPhp)) {
				if(indexPhp.Contains(@"<div class=""div_boxes"" id=""div_access""")) {
					HtmlDocument doc = new HtmlDocument();
					doc.Load(new StringReader(indexPhp));
					
					int powerCompanyColumn = 0;
					HtmlNode accessTableNode = doc.DocumentNode.SelectNodes("//div[@id='div_access']").Descendants("table").First();
					IEnumerable<HtmlNode> ATNdescendants = accessTableNode.Descendants("th");
					for(powerCompanyColumn = 0;powerCompanyColumn < ATNdescendants.Count();powerCompanyColumn++) {
						if(ATNdescendants.ElementAt(powerCompanyColumn).Name == "th" && ATNdescendants.ElementAt(powerCompanyColumn).InnerText.Contains("Power Company"))
							break;
					}
					string[] strTofind = { "<br>" };
					
					return accessTableNode.Descendants("td").ElementAt(powerCompanyColumn).InnerHtml.Replace("<br>",";");
				}
			}
			
			return string.Empty;
		}
		
		string getOiLockedCellsPage() {
			string response = OIConnection.requestPhpOutput("cellslocked", Id, null, string.Empty);
			return response.Contains("Site " + Id + "</b><table>") ? response : "No Cells Locked";
		}
		
		void getOiCellsLockedState(bool getLockedCellsPage) {
			if(Exists && Cells.Count > 0) {
//				if(string.IsNullOrEmpty(indexPhp))
//					indexPhp = OIConnection.requestPhpOutput("index", Id, string.Empty);
				
				if(!string.IsNullOrEmpty(indexPhp) && indexPhp.Contains(@"<div class=""div_boxes"" id=""div_access""")) {
					HtmlDocument doc = new HtmlDocument();
					doc.Load(new StringReader(indexPhp));
					
					HtmlNode div_cells = doc.DocumentNode.SelectNodes("//div[@id='div_cells']").First();
					
					foreach (Cell cell in Cells) {
						HtmlNode checkBoxNode = div_cells.Descendants().ToList().Find(x => x.Id.Contains(cell.Name));
						if(checkBoxNode != null) {
							if(checkBoxNode.ParentNode.InnerHtml.Contains("checked"))
								cell.Locked = checkBoxNode.Attributes.ToList().Find(x => x.Name == "checked").Value == "true";
							else
								cell.Locked = false;
						}
					}
					if(getLockedCellsPage) {
						string response = getOiLockedCellsPage();
						if(response != "No Cells Locked") {
							doc = new HtmlDocument();
							doc.Load(new StringReader(response));
							
							HtmlNode table = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/table[1]");
							
							LockedCellsDetails = Tools.ConvertHtmlTabletoDataTable("<table>" + table.InnerHtml + "</table>", string.Empty);
						}
						else
							LockedCellsDetails = new DataTable();
						LockedCellsDetailsTimestamp = DateTime.Now;
					}
					// Content of a locked cell (unlocked cell doesn't have 'checked' attribute)
					// ><td><input type='checkbox' name='G00151' id='checkboxG00151' disabled='disabled' checked='true'></td>
				}
			}
		}
		
		Site.Vendors resolveVendor(string strVendor) {
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
		
		public void populateCells() {
			cells = Finder.getCells(Id);
		}
		
		void SplitAddress() {
			string[] address = ADDRESS.Split(';');
			int addressLastIndex = address.Length - 1;
			try { postCode = address[addressLastIndex].Trim(); } catch (Exception) { }
			try { county = address[addressLastIndex - 1].Trim(); } catch (Exception) { }
			try { town = address[addressLastIndex - 2].Trim(); } catch (Exception) { }
		}
	}
	
	// TODO: SiteExtension class
	public static class SiteExtension {
		public static DataRow[] NotClosed(this DataTable toFilter) {
			return toFilter.TableName == "table_inc" ? toFilter.Select("Status NOT LIKE 'Closed' AND Status NOT LIKE 'Resolved'") : null;
		}
	}
}
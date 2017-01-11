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
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using appCore.Toolbox;
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
		public string Id;
		[FieldOrder(2)]
		public string JVCO_Id;
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
		public double Easting;
		[FieldOrder(11)]
		public double Northing;
		[FieldOrder(12)]
		public string HostedBy;
		[FieldOrder(13)]
		public string Priority;
		[FieldOrder(14)]
		string ADDRESS;
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
		[FieldOrder(25)]
		string SPECIAL_END;
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
		public string DC_Timestamp;
		[FieldOrder(36)]
		public string Cooling_Status;
		[FieldOrder(37)]
		public string Cooling_Timestamp;
		[FieldOrder(38)]
		public string Key_Information;
		[FieldOrder(39)]
		public string EF_HealthAndSafety;
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
		[FieldOrder(56)]
		string VENDOR_3G;
		[FieldOrder(57)]
		string VENDOR_4G;
		[FieldOrder(58)]
		string DATE;
		[FieldOrder(59)]
		public string MTX_Related;
		
//		[FieldHidden]
		public bool Exists { get { return !string.IsNullOrEmpty(JVCO_Id); } private set { } }
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
//		[FieldHidden]
		public string Address {
			get {
				return ADDRESS.Replace(';',',');
			}
			private set {
				ADDRESS = value;
				string[] address = ADDRESS.Split(';');
				int addressLastIndex = address.Length - 1;
				try { PostCode = address[addressLastIndex].Trim(); } catch (Exception) { }
				try { County = address[addressLastIndex - 1].Trim(); } catch (Exception) { }
				try { Town = address[addressLastIndex - 2].Trim(); } catch (Exception) { }
			}
		}
		[FieldHidden]
		public string PostCode;
		[FieldHidden]
		public string Town;
		[FieldHidden]
		public string County;
		[FieldHidden]
		string POWER_COMPANY = string.Empty;
		[FieldHidden]
		string POWER_CONTACT = string.Empty;
//		[FieldHidden]
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
//		[FieldHidden]
		public DateTime SpecialEvent_StartDate { get { return Convert.ToDateTime(SPECIAL_START); } }
//		[FieldHidden]
		public DateTime SpecialEvent_EndDate { get { return Convert.ToDateTime(SPECIAL_END); } }
//		[FieldHidden]
		public Site.Vendors Vendor2G { get { return resolveVendor(VENDOR_2G); } }
//		[FieldHidden]
		public Site.Vendors Vendor3G { get { return resolveVendor(VENDOR_3G); } }
//		[FieldHidden]
		public Site.Vendors Vendor4G { get { return resolveVendor(VENDOR_4G); } }
//		[FieldHidden]
		public DateTime DeploymentDate { get { return Convert.ToDateTime(DATE); } }
		
		[FieldHidden]
		public DataTable ActiveAlarms;
		[FieldHidden]
		public DataTable INCs;
		[FieldHidden]
		public DataTable CRQs;
		[FieldHidden]
		public DataTable BookIns;
		[FieldHidden]
		public DataTable LockedCellsDetails;
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
					mapMarker.ToolTip.Font = new Font("Courier New", 9, FontStyle.Bold);
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
		List<Cell> cells = new List<Cell>();
		public List<Cell> Cells {
			get {
				if(cells.Count == 0 && Exists) {
					try {
						var engine = new FileHelperEngine<Cell>();
						var res = engine.ReadFileAsList(DB.Databases.all_cells.FullName);
						foreach (Cell s in res)
						{
							if(s.ParentSite == Id)
								cells.Add(s);
						}
					}
					catch(FileHelpersException e) {
						string f = e.Message;
					}
				}
				return cells;
			}
			private set { }
		}
		
		public Site() {
//			_site = site;
//			_cells = cells;
			if(Exists) {
//				try { SITE = _site[_site.Row.Table.Columns.IndexOf("SITE")].ToString(); } catch (Exception) { }
//				try { JVCO_ID = _site[_site.Row.Table.Columns.IndexOf("JVCO_ID")].ToString(); } catch (Exception) { }
//				try { GSM900 = _site[_site.Row.Table.Columns.IndexOf("GSM900")].ToString(); } catch (Exception) { }
//				try { GSM1800 = _site[_site.Row.Table.Columns.IndexOf("GSM1800")].ToString(); } catch (Exception) { }
//				try { UMTS900 = _site[_site.Row.Table.Columns.IndexOf("UMTS900")].ToString(); } catch (Exception) { }
//				try { UMTS2100 = _site[_site.Row.Table.Columns.IndexOf("UMTS2100")].ToString(); } catch (Exception) { }
//				try { LTE800 = _site[_site.Row.Table.Columns.IndexOf("LTE800")].ToString(); } catch (Exception) { }
//				try { LTE2600 = _site[_site.Row.Table.Columns.IndexOf("LTE2600")].ToString(); } catch (Exception) { }
//				try { EASTING = Convert.ToDouble(_site[_site.Row.Table.Columns.IndexOf("EASTING")].ToString()); } catch (Exception) { }
//				try { NORTHING = Convert.ToDouble(_site[_site.Row.Table.Columns.IndexOf("NORTHING")].ToString()); } catch (Exception) { }
//				try { HOST = _site[_site.Row.Table.Columns.IndexOf("HOST")].ToString(); } catch (Exception) { }
//				try { PRIORITY = _site[_site.Row.Table.Columns.IndexOf("PRIORITY")].ToString(); } catch (Exception) { }
//				try { Address = _site[_site.Row.Table.Columns.IndexOf("ADDRESS")].ToString(); } catch (Exception) { }
//				try { POWER_COMPANY = _site[_site.Row.Table.Columns.IndexOf("POWER_COMPANY")].ToString(); } catch (Exception) { }
//				try { POWER_CONTACT = _site[_site.Row.Table.Columns.IndexOf("POWER_CONTACT")].ToString(); } catch (Exception) { }
//			try { TELLABSATRISK = _site[_site.Row.Table.Columns.IndexOf("TELLABSATRISK")].ToString(); } catch (Exception) { }
//				try { AREA = _site[_site.Row.Table.Columns.IndexOf("AREA")].ToString(); } catch (Exception) { }
//			try { NSN_STATUS = _site[_site.Row.Table.Columns.IndexOf("NSN_STATUS")].ToString(); } catch (Exception) { }
//				try { NOC2G = _site[_site.Row.Table.Columns.IndexOf("NOC2G")].ToString(); } catch (Exception) { }
//				try { NOC3G = _site[_site.Row.Table.Columns.IndexOf("NOC3G")].ToString(); } catch (Exception) { }
//				try { NOC4G = _site[_site.Row.Table.Columns.IndexOf("NOC4G")].ToString(); } catch (Exception) { }
//				try { VF_REGION = _site[_site.Row.Table.Columns.IndexOf("VF_REGION")].ToString(); } catch (Exception) { }
//				try { SPECIAL_ID = _site[_site.Row.Table.Columns.IndexOf("SPECIAL_ID")].ToString(); } catch (Exception) { }
//				try { SPECIAL = _site[_site.Row.Table.Columns.IndexOf("SPECIAL")].ToString(); } catch (Exception) { }
//				try { SPECIAL_START = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("SPECIAL_START")].ToString()); } catch (Exception) { }
//				try { SPECIAL_END = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("SPECIAL_END")].ToString()); } catch (Exception) { }
//				try { VIP = _site[_site.Row.Table.Columns.IndexOf("VIP")].ToString(); } catch (Exception) { }
//				try { SITE_SHARE_OPERATOR = _site[_site.Row.Table.Columns.IndexOf("SITE_SHARE_OPERATOR")].ToString(); } catch (Exception) { }
//				try { SITE_SHARE_SITE_NO = _site[_site.Row.Table.Columns.IndexOf("SITE_SHARE_SITE_NO")].ToString(); } catch (Exception) { }
//				try { SITE_ACCESS = _site[_site.Row.Table.Columns.IndexOf("SITE_ACCESS")].ToString(); } catch (Exception) { }
//			try { SITE_TYPE = _site[_site.Row.Table.Columns.IndexOf("SITE_TYPE")].ToString(); } catch (Exception) { }
//			try { SITE_SUBTYPE = _site[_site.Row.Table.Columns.IndexOf("SITE_SUBTYPE")].ToString(); } catch (Exception) { }
//			try { PAKNET_FITTED = _site[_site.Row.Table.Columns.IndexOf("PAKNET_FITTED")].ToString() == "Yes"; } catch (Exception) { }
//			try { VODAPAGE_FITTED = _site[_site.Row.Table.Columns.IndexOf("VODAPAGE_FITTED")].ToString() == "Yes"; } catch (Exception) { }
//			try { DC_STATUS = _site[_site.Row.Table.Columns.IndexOf("DC_STATUS")].ToString(); } catch (Exception) { }
//			try { DC_TIMESTAMP = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("DC_TIMESTAMP")].ToString()); } catch (Exception) { }
//			try { COOLING_STATUS = _site[_site.Row.Table.Columns.IndexOf("COOLING_STATUS")].ToString(); } catch (Exception) { }
//			try { COOLING_TIMESTAMP = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("COOLING_TIMESTAMP")].ToString()); } catch (Exception) { }
//			try { KEY_INFORMATION = _site[_site.Row.Table.Columns.IndexOf("KEY_INFORMATION")].ToString(); } catch (Exception) { }
//			try { EF_HEALTHANDSAFETY = _site[_site.Row.Table.Columns.IndexOf("EF_HEALTHANDSAFETY")].ToString(); } catch (Exception) { }
//				try { SWITCH2G = _site[_site.Row.Table.Columns.IndexOf("SWITCH2G")].ToString(); } catch (Exception) { }
//				try { SWITCH3G = _site[_site.Row.Table.Columns.IndexOf("SWITCH3G")].ToString(); } catch (Exception) { }
//				try { SWITCH4G = _site[_site.Row.Table.Columns.IndexOf("SWITCH4G")].ToString(); } catch (Exception) { }
//			try { DRSWITCH2G = _site[_site.Row.Table.Columns.IndexOf("DRSWITCH2G")].ToString(); } catch (Exception) { }
//			try { DRSWITCH3G = _site[_site.Row.Table.Columns.IndexOf("DRSWITCH3G")].ToString(); } catch (Exception) { }
//			try { DRSWITCH4G = _site[_site.Row.Table.Columns.IndexOf("DRSWITCH4G")].ToString(); } catch (Exception) { }
//				try { MTX2G = _site[_site.Row.Table.Columns.IndexOf("MTX2G")].ToString(); } catch (Exception) { }
//				try { MTX3G = _site[_site.Row.Table.Columns.IndexOf("MTX3G")].ToString(); } catch (Exception) { }
//				try { MTX4G = _site[_site.Row.Table.Columns.IndexOf("MTX4G")].ToString(); } catch (Exception) { }
//			try { IP_2G_I = _site[_site.Row.Table.Columns.IndexOf("IP_2G_I")].ToString(); } catch (Exception) { }
//			try { IP_2G_E = _site[_site.Row.Table.Columns.IndexOf("IP_2G_E")].ToString(); } catch (Exception) { }
//			try { IP_3G_I = _site[_site.Row.Table.Columns.IndexOf("IP_3G_I")].ToString(); } catch (Exception) { }
//			try { IP_3G_E = _site[_site.Row.Table.Columns.IndexOf("IP_3G_E")].ToString(); } catch (Exception) { }
//			try { IP_4G_I = _site[_site.Row.Table.Columns.IndexOf("IP_4G_I")].ToString(); } catch (Exception) { }
//			try { IP_4G_E = _site[_site.Row.Table.Columns.IndexOf("IP_4G_E")].ToString(); } catch (Exception) { }
//				try { VENDOR_2G = resolveVendor(_site[_site.Row.Table.Columns.IndexOf("VENDOR_2G")].ToString()); } catch (Exception) { }
//				try { VENDOR_3G = resolveVendor(_site[_site.Row.Table.Columns.IndexOf("VENDOR_3G")].ToString()); } catch (Exception) { }
//				try { VENDOR_4G = resolveVendor(_site[_site.Row.Table.Columns.IndexOf("VENDOR_4G")].ToString()); } catch (Exception) { }
//				try { DATE = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("DATE")].ToString()); } catch (Exception) { }
//			try { MTX_RELATED = _site[_site.Row.Table.Columns.IndexOf("MTX_RELATED")].ToString(); } catch (Exception) { }
				
//				foreach (DataRowView cell in _cells) {
//					Cells.Add(new Cell(cell));
//				}
//				var lengths = from element in Cells
//					orderby element.Bearer ascending
//					orderby element.Name ascending
//					select element;
			}
		}
		
		/// <summary>
		/// Populate with data pulled from OI
		/// </summary>
		/// <param name="dataToRequest">"INC", "CRQ", "Bookins", "Alarms", "PWR"</param>
		public void requestOIData(string dataToRequest) {
			if(Exists) {
				if(dataToRequest.Contains("INC"))
					INCs = FetchINCs();
				
				if(dataToRequest.Contains("CRQ"))
					CRQs = FetchCRQs();
				
				if(dataToRequest.Contains("Alarms"))
					ActiveAlarms = FetchActiveAlarms();
				
				if(dataToRequest.Contains("Bookins"))
					BookIns = FetchBookIns();
				
				if(dataToRequest.Contains("PWR")) {
					if(string.IsNullOrWhiteSpace(PowerCompany))
						try { PowerCompany = getPowerCompany(); } catch {}
				}
//				}
//				else {
//					if(CurrentUser.userName == "Caramelos" && dataToRequest != "PWR") {
//						DriveInfo penRoot = null;
//						foreach (var drive in DriveInfo.GetDrives()) {
//							if(drive.IsReady) {
//								if(drive.VolumeLabel == "PEN") {
//									Int64 totalGb = ((drive.TotalSize / 1024) / 1024) / 1024;
//									if(totalGb > 55 && totalGb < 65) {
//										penRoot = drive;
//										break;
//									}
//								}
//							}
//						}
//						if(penRoot != null) {
//							if(dataToRequest.Contains("INC") || dataToRequest.Contains("AllSite")) {
//								FileSystemInfo table_inc = null;
//								try {
//									table_inc = penRoot.RootDirectory.GetDirectories("ANOC")[0].GetFiles("table_inc.txt")[0];
//								}
//								catch (Exception) {	}
//								if(table_inc != null) {
//									INCs = FetchINCs(table_inc);
//									DataView dv = INCs.DefaultView;
//									dv.Sort = "Incident Ref desc";
//									INCs = dv.ToTable();
//								}
//							}
//
//							if(dataToRequest.Contains("CRQ") || dataToRequest.Contains("AllSite")) {
//								FileSystemInfo table_crq = null;
//								try {
//									table_crq = penRoot.RootDirectory.GetDirectories("ANOC")[0].GetFiles("table_crq.txt")[0];
//								}
//								catch (Exception) { }
//								if(table_crq != null) {
//									CRQs = FetchCRQs(table_crq);
//									DataView dv = CRQs.DefaultView;
//									dv.Sort = "Scheduled Start desc";
//									CRQs = dv.ToTable();
//								}
//							}
//
//							if(dataToRequest.Contains("Alarms")) {
//								FileSystemInfo table_alarms = null;
//								try {
//									table_alarms = penRoot.RootDirectory.GetDirectories("ANOC")[0].GetFiles("table_alarms.txt")[0];
//								}
//								catch (Exception) { }
//								if(table_alarms != null) {
//									ActiveAlarms = FetchActiveAlarms(table_alarms);
//									DataView dv = ActiveAlarms.DefaultView;
//									dv.Sort = "Date/Time desc";
//									ActiveAlarms = dv.ToTable();
//								}
//							}
//
//							if(dataToRequest.Contains("Bookins")) {
//								FileSystemInfo table_visits = null;
//								try {
//									table_visits = penRoot.RootDirectory.GetDirectories("ANOC")[0].GetFiles("table_visits.txt")[0];
//								}
//								catch (Exception) { }
//								if(table_visits != null) {
//									this.BookIns = FetchBookIns(table_visits);
				////									BookIns = FetchBookIns(table_visits);
//									DataView dv = this.BookIns.DefaultView;
				////									DataView dv = BookIns.DefaultView;
//									dv.Sort = "Visit desc";
//									DataTable BookIns = dv.ToTable();
//								}
//							}
//						}
//					}
//				}
			}
		}
		
		DataTable FetchINCs(FileSystemInfo table_inc = null) {
			DataTable dt = new DataTable();
			string response = string.Empty;
			response = Web.OIConnection.requestPhpOutput("inc", Id);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No open or incidents"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_inc");
			return dt;
		}
		
		DataTable FetchCRQs(FileSystemInfo table_crq = null) {
			DataTable dt = new DataTable();
			string response = string.Empty;
			response = Web.OIConnection.requestPhpOutput("crq", Id);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No changes in past 90 days"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_crq");
			return dt;
		}
		
		DataTable FetchActiveAlarms(FileSystemInfo table_alarms = null) {
			DataTable dt = new DataTable();
			string response = string.Empty;
			response = Web.OIConnection.requestPhpOutput("alarms", Id);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No alarms reported"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_alarms");
			return dt;
		}
		
		DataTable FetchBookIns(FileSystemInfo table_visits = null) {
			DataTable dt = new DataTable();
			string response = string.Empty;
			response = Web.OIConnection.requestPhpOutput("sitevisit", Id, 90);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No site visits"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_visits");
			return dt;
		}
		
		string getPowerCompany(string response = "") {
			if(string.IsNullOrEmpty(response))
				response = Web.OIConnection.requestPhpOutput("index", Id, string.Empty);
			if(!string.IsNullOrEmpty(response) && response.Contains(@"<div class=""div_boxes"" id=""div_access""")) {
				HtmlDocument doc = new HtmlDocument();
				doc.Load(new StringReader(response));
				
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
			return string.Empty;
		}
		
		string getOiLockedCellsDetails() {
			string response = Web.OIConnection.requestPhpOutput("cellslocked", Id, null, string.Empty);
			return response.Contains("Site " + Id + "</b><table>") ? response : string.Empty;
		}
		
		string getOiCellsLockedState(bool getLockedDetails) {
			string response = Web.OIConnection.requestPhpOutput("index", Id, string.Empty);
			if(string.IsNullOrWhiteSpace(PowerCompany))
				try { PowerCompany = getPowerCompany(response); } catch {}
			
			if(!string.IsNullOrEmpty(response) && response.Contains(@"<div class=""div_boxes"" id=""div_access""")) {
				HtmlDocument doc = new HtmlDocument();
				doc.Load(new StringReader(response));
				
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
				if(getLockedDetails) {
//					if(Cells.Filter(Cell.Filters.Locked).Any()) {
					string resp = getOiLockedCellsDetails();
					HtmlDocument doc2 = new HtmlDocument();
					doc2.Load(new StringReader(resp));
					
					HtmlNode table = doc2.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/table[1]");
					
					LockedCellsDetails = Tools.ConvertHtmlTabletoDataTable("<table>" + table.InnerHtml + "</table>", string.Empty);
//					}
				}
				// Content of a locked cell (unlocked cell doesn't have 'checked' attribute)
				// ><td><input type='checkbox' name='G00151' id='checkboxG00151' disabled='disabled' checked='true'></td>
				
				return response;
			}
			return string.Empty;
		}
		
		public void UpdateLockedCells(bool getLockedDetails) {
			if(Exists && Cells.Count > 0)
				getOiCellsLockedState(getLockedDetails);
		}
		
		public void LockUnlockCells() {
//			string response = Web.OIConnection.Connection.requestPhpOutput("enterlock", Id, cellsList, ManRef, comments, false);
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
	}
	
	// TODO: SiteExtension class
	public static class SiteExtension {
		public static DataRow[] NotClosed(this DataTable toFilter) {
			return toFilter.TableName == "table_inc" ? toFilter.Select("Status NOT LIKE 'Closed' AND Status NOT LIKE 'Resolved'") : null;
		}
	}
}
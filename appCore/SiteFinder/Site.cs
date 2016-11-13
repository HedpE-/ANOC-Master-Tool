/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 21-07-2016
 * Time: 10:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Data;
using System.Net;
using System.IO;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using appCore.Settings;
using appCore.Toolbox;
using HtmlAgilityPack;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Site.
	/// </summary>
	public partial class Site
	{
		public bool Exists { get { return _site != null; } private set { } }
		string SITE = string.Empty;
		public string Id { get { return SITE; } }
		string JVCO_ID = string.Empty;
		public string JVCO { get { return JVCO_ID; } }
		string GSM900 = string.Empty;
		public string GSM900Operators { get { return GSM900; } }
		string GSM1800 = string.Empty;
		public string GSM1800Operators { get { return GSM1800; } }
		string UMTS900 = string.Empty;
		public string UMTS900Operators { get { return UMTS900; } }
		string UMTS2100 = string.Empty;
		public string UMTS2100Operators { get { return UMTS2100; } }
		string LTE800 = string.Empty;
		public string LTE800Operators { get { return LTE800; } }
		string LTE2600 = string.Empty;
		public string LTE2600Operators { get { return LTE2600; } }
		double EASTING = 0;
		double NORTHING = 0;
		public LLPoint Coordinates { get; private set; }
		string HOST = string.Empty;
		public string HostedBy { get { return HOST; } }
		string PRIORITY = string.Empty;
		public string Priority { get { return PRIORITY; } }
		string ADDRESS = string.Empty;
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
		public string PostCode { get; private set; }
		public string Town { get; private set; }
		public string County { get; private set; }
		string POWER_COMPANY = string.Empty;
		string POWER_CONTACT = string.Empty;
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
//		string TELLABSATRISK = string.Empty;
//		public string TellabsAtRisk { get { return TELLABSATRISK; } }
		string AREA = string.Empty;
		public string Area { get { return AREA; } }
//		string NSN_STATUS = string.Empty;
//		public string NSNStatus { get { return NSN_STATUS; } }
		string NOC2G = string.Empty;
		public string NOC_2G { get { return NOC2G; } }
		string NOC3G = string.Empty;
		public string NOC_3G { get { return NOC3G; } }
		string NOC4G = string.Empty;
		public string NOC_4G { get { return NOC4G; } }
		string VF_REGION = string.Empty;
		public string Region { get { return VF_REGION; } }
		string SPECIAL_ID = string.Empty;
		public string SpecialEventID { get { return SPECIAL_ID; } }
		string SPECIAL = string.Empty;
		public string SpecialEvent { get { return SPECIAL_ID; } }
		DateTime SPECIAL_START = new DateTime(1,1,1);
		public DateTime SpecialEvent_StartDate { get { return SPECIAL_START; } }
		DateTime SPECIAL_END = new DateTime(1,1,1);
		public DateTime SpecialEvent_EndDate { get { return SPECIAL_END; } }
		string VIP = string.Empty;
		public string VipSite { get { return VIP; } }
		string SITE_SHARE_OPERATOR = string.Empty;
		public string SharedOperator { get { return SITE_SHARE_OPERATOR; } }
		string SITE_SHARE_SITE_NO = string.Empty;
		public string SharedOperatorSiteID { get { return SITE_SHARE_SITE_NO; } }
		string SITE_ACCESS = string.Empty;
		public string Access { get { return SITE_ACCESS; } }
//		string SITE_TYPE = string.Empty;
//		public string Type { get { return SITE_TYPE; } }
//		string SITE_SUBTYPE = string.Empty;
//		public string SubType { get { return SITE_SUBTYPE; } }
//		bool PAKNET_FITTED;
//		public string PaknetFitted { get { return PAKNET_FITTED ? "Yes" : "No"; } }
//		bool VODAPAGE_FITTED;
//		public string VodaPageFitted { get { return VODAPAGE_FITTED ? "Yes" : "No"; } }
//		string DC_STATUS = string.Empty;
//		public string OwnDCStatus { get { return DC_STATUS; } }
//		DateTime DC_TIMESTAMP = new DateTime(1,1,1);
//		public DateTime OwnDCStatusChangedDate { get { return DC_TIMESTAMP; } }
//		string COOLING_STATUS = string.Empty;
//		public string CoolingSystemStatus { get { return COOLING_STATUS; } }
//		DateTime COOLING_TIMESTAMP = new DateTime(1,1,1);
//		public DateTime CoolingSystemStatusChangedDate { get { return COOLING_TIMESTAMP; } }
//		readonly string KEY_INFORMATION = string.Empty;
//		public string KeyInformation { get { return KEY_INFORMATION; } }
//		string EF_HEALTHANDSAFETY = string.Empty;
//		public string HealthAndSafety { get { return EF_HEALTHANDSAFETY; } }
		string SWITCH2G = string.Empty;
		public string BSC { get { return SWITCH2G; } }
		string SWITCH3G = string.Empty;
		public string RNC { get { return SWITCH3G; } }
		string SWITCH4G = string.Empty;
		public string Switch4G { get { return SWITCH4G; } }
//		string DRSWITCH2G = string.Empty;
//		public string DRSwitch2G { get { return DRSWITCH2G; } }
//		string DRSWITCH3G = string.Empty;
//		public string DRSwitch3G { get { return DRSWITCH2G; } }
//		string DRSWITCH4G = string.Empty;
//		public string DRSwitch4G { get { return DRSWITCH2G; } }
		string MTX2G = string.Empty;
		public string MTX_2G { get { return MTX2G; } }
		string MTX3G = string.Empty;
		public string MTX_3G { get { return MTX3G; } }
		string MTX4G = string.Empty;
		public string MTX_4G { get { return MTX4G; } }
//		string IP_2G_I = string.Empty;
//		public string InnerIP2G { get { return IP_2G_I; } }
//		string IP_2G_E = string.Empty;
//		public string OuterIP2G { get { return IP_2G_E; } }
//		string IP_3G_I = string.Empty;
//		public string InnerIP3G { get { return IP_3G_I; } }
//		string IP_3G_E = string.Empty;
//		public string OuterIP3G { get { return IP_3G_E; } }
//		string IP_4G_I = string.Empty;
//		public string InnerIP4G { get { return IP_4G_I; } }
//		string IP_4G_E = string.Empty;
//		public string OuterIP4G { get { return IP_4G_E; } }
		Vendors VENDOR_2G = Vendors.None;
		public Vendors Vendor2G { get { return VENDOR_2G; } }
		Vendors VENDOR_3G = Vendors.None;
		public Vendors Vendor3G { get { return VENDOR_3G; } }
		Vendors VENDOR_4G = Vendors.None;
		public Vendors Vendor4G { get { return VENDOR_4G; } }
		DateTime DATE = new DateTime(1,1,1);
		public DateTime DeploymentDate { get { return DATE; } }
//		string MTX_RELATED = string.Empty;
//		public string RelatedMTX { get { return MTX_RELATED; } }
		public DataTable ActiveAlarms { get; private set; }
		public DataTable INCs { get; private set; }
		public DataTable CRQs { get; private set; }
		public DataTable BookIns { get; private set; }
		public GMarkerGoogle MapMarker { get; private set; }
		
		public List<Cell> Cells = new List<Cell>();
		protected DataRowView _site;
		protected DataView _cells;

		public Site() {}

		public Site(DataRowView site, DataView cells) {
			_site = site;
			_cells = cells;
			if(Exists) {
				try { SITE = _site[_site.Row.Table.Columns.IndexOf("SITE")].ToString(); } catch (Exception) { }
				try { JVCO_ID = _site[_site.Row.Table.Columns.IndexOf("JVCO_ID")].ToString(); } catch (Exception) { }
				try { GSM900 = _site[_site.Row.Table.Columns.IndexOf("GSM900")].ToString(); } catch (Exception) { }
				try { GSM1800 = _site[_site.Row.Table.Columns.IndexOf("GSM1800")].ToString(); } catch (Exception) { }
				try { UMTS900 = _site[_site.Row.Table.Columns.IndexOf("UMTS900")].ToString(); } catch (Exception) { }
				try { UMTS2100 = _site[_site.Row.Table.Columns.IndexOf("UMTS2100")].ToString(); } catch (Exception) { }
				try { LTE800 = _site[_site.Row.Table.Columns.IndexOf("LTE800")].ToString(); } catch (Exception) { }
				try { LTE2600 = _site[_site.Row.Table.Columns.IndexOf("LTE2600")].ToString(); } catch (Exception) { }
				try { EASTING = Convert.ToDouble(_site[_site.Row.Table.Columns.IndexOf("EASTING")].ToString()); } catch (Exception) { }
				try { NORTHING = Convert.ToDouble(_site[_site.Row.Table.Columns.IndexOf("NORTHING")].ToString()); } catch (Exception) { }
				try { HOST = _site[_site.Row.Table.Columns.IndexOf("HOST")].ToString(); } catch (Exception) { }
				try { PRIORITY = _site[_site.Row.Table.Columns.IndexOf("PRIORITY")].ToString(); } catch (Exception) { }
				try { Address = _site[_site.Row.Table.Columns.IndexOf("ADDRESS")].ToString(); } catch (Exception) { }
				try { POWER_COMPANY = _site[_site.Row.Table.Columns.IndexOf("POWER_COMPANY")].ToString(); } catch (Exception) { }
				try { POWER_CONTACT = _site[_site.Row.Table.Columns.IndexOf("POWER_CONTACT")].ToString(); } catch (Exception) { }
//			try { TELLABSATRISK = _site[_site.Row.Table.Columns.IndexOf("TELLABSATRISK")].ToString(); } catch (Exception) { }
				try { AREA = _site[_site.Row.Table.Columns.IndexOf("AREA")].ToString(); } catch (Exception) { }
//			try { NSN_STATUS = _site[_site.Row.Table.Columns.IndexOf("NSN_STATUS")].ToString(); } catch (Exception) { }
				try { NOC2G = _site[_site.Row.Table.Columns.IndexOf("NOC2G")].ToString(); } catch (Exception) { }
				try { NOC3G = _site[_site.Row.Table.Columns.IndexOf("NOC3G")].ToString(); } catch (Exception) { }
				try { NOC4G = _site[_site.Row.Table.Columns.IndexOf("NOC4G")].ToString(); } catch (Exception) { }
				try { VF_REGION = _site[_site.Row.Table.Columns.IndexOf("VF_REGION")].ToString(); } catch (Exception) { }
				try { SPECIAL_ID = _site[_site.Row.Table.Columns.IndexOf("SPECIAL_ID")].ToString(); } catch (Exception) { }
				try { SPECIAL = _site[_site.Row.Table.Columns.IndexOf("SPECIAL")].ToString(); } catch (Exception) { }
				try { SPECIAL_START = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("SPECIAL_START")].ToString()); } catch (Exception) { }
				try { SPECIAL_END = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("SPECIAL_END")].ToString()); } catch (Exception) { }
				try { VIP = _site[_site.Row.Table.Columns.IndexOf("VIP")].ToString(); } catch (Exception) { }
				try { SITE_SHARE_OPERATOR = _site[_site.Row.Table.Columns.IndexOf("SITE_SHARE_OPERATOR")].ToString(); } catch (Exception) { }
				try { SITE_SHARE_SITE_NO = _site[_site.Row.Table.Columns.IndexOf("SITE_SHARE_SITE_NO")].ToString(); } catch (Exception) { }
				try { SITE_ACCESS = _site[_site.Row.Table.Columns.IndexOf("SITE_ACCESS")].ToString(); } catch (Exception) { }
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
				try { SWITCH2G = _site[_site.Row.Table.Columns.IndexOf("SWITCH2G")].ToString(); } catch (Exception) { }
				try { SWITCH3G = _site[_site.Row.Table.Columns.IndexOf("SWITCH3G")].ToString(); } catch (Exception) { }
				try { SWITCH4G = _site[_site.Row.Table.Columns.IndexOf("SWITCH4G")].ToString(); } catch (Exception) { }
//			try { DRSWITCH2G = _site[_site.Row.Table.Columns.IndexOf("DRSWITCH2G")].ToString(); } catch (Exception) { }
//			try { DRSWITCH3G = _site[_site.Row.Table.Columns.IndexOf("DRSWITCH3G")].ToString(); } catch (Exception) { }
//			try { DRSWITCH4G = _site[_site.Row.Table.Columns.IndexOf("DRSWITCH4G")].ToString(); } catch (Exception) { }
				try { MTX2G = _site[_site.Row.Table.Columns.IndexOf("MTX2G")].ToString(); } catch (Exception) { }
				try { MTX3G = _site[_site.Row.Table.Columns.IndexOf("MTX3G")].ToString(); } catch (Exception) { }
				try { MTX4G = _site[_site.Row.Table.Columns.IndexOf("MTX4G")].ToString(); } catch (Exception) { }
//			try { IP_2G_I = _site[_site.Row.Table.Columns.IndexOf("IP_2G_I")].ToString(); } catch (Exception) { }
//			try { IP_2G_E = _site[_site.Row.Table.Columns.IndexOf("IP_2G_E")].ToString(); } catch (Exception) { }
//			try { IP_3G_I = _site[_site.Row.Table.Columns.IndexOf("IP_3G_I")].ToString(); } catch (Exception) { }
//			try { IP_3G_E = _site[_site.Row.Table.Columns.IndexOf("IP_3G_E")].ToString(); } catch (Exception) { }
//			try { IP_4G_I = _site[_site.Row.Table.Columns.IndexOf("IP_4G_I")].ToString(); } catch (Exception) { }
//			try { IP_4G_E = _site[_site.Row.Table.Columns.IndexOf("IP_4G_E")].ToString(); } catch (Exception) { }
				try { VENDOR_2G = getVendor(_site[_site.Row.Table.Columns.IndexOf("VENDOR_2G")].ToString()); } catch (Exception) { }
				try { VENDOR_3G = getVendor(_site[_site.Row.Table.Columns.IndexOf("VENDOR_3G")].ToString()); } catch (Exception) { }
				try { VENDOR_4G = getVendor(_site[_site.Row.Table.Columns.IndexOf("VENDOR_4G")].ToString()); } catch (Exception) { }
				try { DATE = Convert.ToDateTime(_site[_site.Row.Table.Columns.IndexOf("DATE")].ToString()); } catch (Exception) { }
//			try { MTX_RELATED = _site[_site.Row.Table.Columns.IndexOf("MTX_RELATED")].ToString(); } catch (Exception) { }
				
				foreach (DataRowView cell in _cells) {
					Cells.Add(new Cell(cell));
				}
				var lengths = from element in Cells
					orderby element.Bearer ascending
					orderby element.Name ascending
					select element;
				
				Coordinates = coordConvert.toLat_Long(new GMap.NET.Point { Easting = EASTING, Northing = NORTHING }, "OSGB36");
				MapMarker = new GMarkerGoogle(new PointLatLng(Coordinates.Latitude, Coordinates.Longitude), GMarkerGoogleType.red);
				MapMarker.Tag = Id;
				MapMarker.ToolTip = new GMapBaloonToolTip(MapMarker);
				MapMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
				MapMarker.ToolTip.Fill = new SolidBrush(Color.FromArgb(180, Color.Black));
				MapMarker.ToolTip.Font = new Font("Courier New", 9, FontStyle.Bold);
				MapMarker.ToolTip.Foreground = new SolidBrush(Color.White);
				MapMarker.ToolTip.Stroke = new Pen(Color.Red);
				MapMarker.ToolTip.Offset.X -= 15;
				MapMarker.ToolTipText = Id;
			}
		}
		
		/// <summary>
		/// Populate with data pulled from OI
		/// </summary>
		/// <param name="dataToRequest">"INC", "CRQ", "Bookins", "Alarms", "PWR"</param>
		public void requestOIData(string dataToRequest) {
			if(Exists) {
//				HttpStatusCode status = HttpStatusCode.NotFound;
//				if(Web.OIConnection.Connection == null)
//					status = Web.OIConnection.EstablishConnection();
//				HttpStatusCode statusCode = Web.OIConnection.Connection.Logon();
//				if(statusCode == HttpStatusCode.OK) {
				if(dataToRequest.Contains("INC"))
					INCs = FetchINCs();
				
				if(dataToRequest.Contains("CRQ"))
					CRQs = FetchCRQs();
				
				if(dataToRequest.Contains("Alarms"))
					ActiveAlarms = FetchActiveAlarms();
				
				if(dataToRequest.Contains("Bookins"))
					BookIns = FetchBookIns();
				
				if(dataToRequest.Contains("PWR")) {
					PowerCompany = getPowerCompany();
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
//			if(table_inc != null) {
//				if(table_inc.Exists)
//					response = File.ReadAllText(table_inc.FullName);
//			}
//			else
			response = Web.OIConnection.requestPhpOutput("inc", Id);
			string parsedResponse = string.Empty;
			if(!response.Contains("No open or incidents"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_inc");
			return dt;
		}
		
		DataTable FetchCRQs(FileSystemInfo table_crq = null) {
			DataTable dt = new DataTable();
			string response = string.Empty;
//			if(table_crq != null) {
//				if(table_crq.Exists)
//					response = File.ReadAllText(table_crq.FullName);
//			}
//			else
			response = Web.OIConnection.requestPhpOutput("crq", Id);
			string parsedResponse = string.Empty;
			if(!response.Contains("No changes in past 90 days"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_crq");
			return dt;
		}
		
		DataTable FetchActiveAlarms(FileSystemInfo table_alarms = null) {
			DataTable dt = new DataTable();
			string response = string.Empty;
//			if(table_alarms != null) {
//				if(table_alarms.Exists)
//					response = File.ReadAllText(table_alarms.FullName);
//			}
//			else
			response = Web.OIConnection.requestPhpOutput("alarms", Id);
			string parsedResponse = string.Empty;
			if(!response.Contains("No alarms reported"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_alarms");
			return dt;
		}
		
		DataTable FetchBookIns(FileSystemInfo table_visits = null) {
			DataTable dt = new DataTable();
			string response = string.Empty;
//			if(table_visits != null) {
//				if(table_visits.Exists)
//					response = File.ReadAllText(table_visits.FullName);
//			}
//			else
			response = Web.OIConnection.requestPhpOutput("sitevisit", Id, 90);
			string parsedResponse = string.Empty;
			if(!response.Contains("No site visits"))
				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_visits");
			return dt;
		}
		
		string getPowerCompany() {
			string response = Web.OIConnection.requestPhpOutput("index", Id, string.Empty);
			if(response.Contains(@"<div class=""div_boxes"" id=""div_access""")) {
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
		
		string getOiCellsLockedState() {
			string response = Web.OIConnection.requestPhpOutput("index", Id, string.Empty);
			
			if(response.Contains(@"<div class=""div_boxes"" id=""div_access""")) {
				HtmlDocument doc = new HtmlDocument();
				doc.Load(new StringReader(response));
				
				HtmlNode div_cells = doc.DocumentNode.SelectNodes("//div[@id='div_cells']").First();
				
				foreach (Cell cell in Cells) {
					HtmlNode checkBoxNode = div_cells.Descendants().ToList().Find(x => x.Id == "checkbox" + cell.Name);
					if(checkBoxNode != null) {
						if(checkBoxNode.ParentNode.InnerHtml.Contains("checked"))
							cell.Locked = checkBoxNode.Attributes.ToList().Find(x => x.Name == "checked").Value == "true";
						else
							cell.Locked = false;
					}
				}
				
				// Content of a locked cell (unlocked cell doesn't have 'checked' attribute)
				// ><td><input type='checkbox' name='G00151' id='checkboxG00151' disabled='disabled' checked='true'></td>
				
				return response;
			}
			return string.Empty;
		}
		
		public void UpdateLockedCells() {
			if(Exists && Cells.Count > 0) {
//				HttpStatusCode status = HttpStatusCode.NotFound;
//				if(Web.OIConnection.Connection == null)
//					status = Web.OIConnection.EstablishConnection();
//				HttpStatusCode statusCode = Web.OIConnection.Connection.Logon();
//				if(statusCode == HttpStatusCode.OK)
					getOiCellsLockedState();
			}
		}
		
		public void LockUnlockCells() {
//			string response = Web.OIConnection.Connection.requestPhpOutput("enterlock", Id, cellsList, ManRef, comments, false);
		}
		
		Vendors getVendor(string strVendor) {
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

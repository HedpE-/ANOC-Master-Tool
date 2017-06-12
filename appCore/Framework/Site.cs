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
using appCore.OI;
using appCore.OI.JSON;
using HtmlAgilityPack;
using FileHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenWeatherAPI;

namespace appCore
{
	/// <summary>
	/// Description of Site.
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
		public string ClusterName;
		[FieldOrder(23)]
		public string Special_Id;
		[FieldOrder(24)]
		public string Special;
		[FieldOrder(25)]
		string SPECIAL_START;
		public DateTime SpecialEvent_StartDate {
			get {
				return string.IsNullOrEmpty(SPECIAL_START) ?
					new DateTime(1, 1, 1, 0, 0, 0) :
					Convert.ToDateTime(SPECIAL_START);
			}
			private set { SPECIAL_START = value.ToString(); }
		}
		[FieldOrder(26)]
		string SPECIAL_END;
		public DateTime SpecialEvent_EndDate {
			get {
				return string.IsNullOrEmpty(SPECIAL_END) ?
					new DateTime(1, 1, 1, 0, 0, 0) :
					Convert.ToDateTime(SPECIAL_END);
			}
			private set { SPECIAL_END = value.ToString(); }
		}
		[FieldOrder(27)]
		public string VIP;
		[FieldOrder(28)]
		public string SharedOperator;
		[FieldOrder(29)]
		public string SharedOperatorSiteID;
		[FieldOrder(30)]
		public string Site_Access;
		[FieldOrder(31)]
		public string Site_Type;
		[FieldOrder(32)]
		public string Site_Subtype;
		[FieldOrder(33)]
		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (bool), "false")]
		public bool Paknet_Fitted;
		[FieldOrder(34)]
		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (bool), "false")]
		public bool Vodapage_Fitted;
		[FieldOrder(35)]
		public string DC_STATUS;
		[FieldOrder(36)]
		string dc_Timestamp;
		public DateTime DC_Timestamp {
			get {
				return string.IsNullOrEmpty(dc_Timestamp) ?
					new DateTime(1, 1, 1, 0, 0, 0) :
					Convert.ToDateTime(dc_Timestamp);
			}
			private set { dc_Timestamp = value.ToString(); }
		}
		[FieldOrder(37)]
		public string Cooling_Status;
		[FieldOrder(38)]
		public string cooling_Timestamp;
		public DateTime Cooling_Timestamp {
			get {
				return string.IsNullOrEmpty(cooling_Timestamp) ?
					new DateTime(1, 1, 1, 0, 0, 0) :
					Convert.ToDateTime(cooling_Timestamp);
			}
			private set { cooling_Timestamp = value.ToString(); }
		}
		[FieldOrder(39)]
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
		[FieldOrder(40)]
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
		[FieldOrder(41)]
		public string Switch2G;
		[FieldOrder(42)]
		public string Switch3G;
		[FieldOrder(43)]
		public string Switch4G;
		[FieldOrder(44)]
		public string DRSwitch2G;
		[FieldOrder(45)]
		public string DRSwitch3G;
		[FieldOrder(46)]
		public string DRSwitch4G;
		[FieldOrder(47)]
		public string MTX2G;
		[FieldOrder(48)]
		public string MTX3G;
		[FieldOrder(49)]
		public string MTX4G;
		[FieldOrder(50)]
		public string IP_2G_I;
		[FieldOrder(51)]
		public string IP_2G_E;
		[FieldOrder(52)]
		public string IP_3G_I;
		[FieldOrder(53)]
		public string IP_3G_E;
		[FieldOrder(54)]
		public string IP_4G_I;
		[FieldOrder(55)]
		public string IP_4G_E;
		[FieldOrder(56)]
		string VENDOR_2G;
		public Vendors Vendor2G {
			get { return resolveVendor(VENDOR_2G); }
			private set { VENDOR_2G = value.ToString(); }
		}
		[FieldOrder(57)]
		string VENDOR_3G;
		public Vendors Vendor3G {
			get { return resolveVendor(VENDOR_3G); }
			private set { VENDOR_3G = value.ToString(); }
		}
		[FieldOrder(58)]
		string VENDOR_4G;
		public Vendors Vendor4G {
			get { return resolveVendor(VENDOR_4G); }
			private set { VENDOR_4G = value.ToString(); }
		}
//		[FieldOrder(58), FieldOptional]
//		string DATE;
//		public DateTime DeploymentDate {
//			get { return Convert.ToDateTime(DATE); }
//			private set { DATE = value.ToString(); }
//		}
		[FieldOrder(59), FieldOptional]
		public string MTX_Related;
		
		public bool Exists {
			get { return !string.IsNullOrEmpty(JVCO_Id); }
			private set { }
		}
		[FieldHidden]
		string fullId;
		public string FullId {
			get {
				if(string.IsNullOrEmpty(fullId)) {
					fullId = Id;
					while(fullId.Length < 5)
						fullId = 0 + fullId;
					fullId = "RBS" + fullId;
				}
				return fullId;
			}
		}
		[FieldHidden]
		string POWER_COMPANY;
		[FieldHidden]
		string POWER_CONTACT;
		public string PowerCompany {
			get {
				return (POWER_COMPANY + " " + POWER_CONTACT).Trim();
			}
			set {
				string[] temp = value.Split(';');
				POWER_COMPANY = temp[0].Trim();
				POWER_CONTACT = temp[1].Trim();
			}
		}
		
		[FieldHidden]
		public List<Alarm> Alarms;
		[FieldHidden]
		public DateTime AlarmsTimestamp;
		[FieldHidden]
		public bool isUpdatingAlarms;
		[FieldHidden]
		public List<Incident> Incidents;
		[FieldHidden]
		public DateTime IncidentsTimestamp;
		[FieldHidden]
		public bool isUpdatingIncidents;
		[FieldHidden]
		public List<Change> Changes;
		[FieldHidden]
		public DateTime ChangesTimestamp;
		[FieldHidden]
		public bool isUpdatingChanges;
		[FieldHidden]
		public List<BookIn> Visits;
		[FieldHidden]
		public DateTime VisitsTimestamp;
		[FieldHidden]
		public bool isUpdatingVisits;
		[FieldHidden]
		public DataTable Availability;
		[FieldHidden]
		public DateTime AvailabilityTimestamp;
		[FieldHidden]
		public bool isUpdatingAvailability;
		[FieldHidden]
		public CramerDetails CramerData;
		[FieldHidden]
		public DateTime CramerDataTimestamp;
		[FieldHidden]
		public bool isUpdatingCramerData;
		
		[FieldHidden]
		public DataTable LockedCellsDetails;
		[FieldHidden]
		public DateTime LockedCellsDetailsTimestamp;
		
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
        private WeatherItem currentWeather;
		public WeatherItem CurrentWeather {
			get {
                //if (currentWeather == null)
                //    //System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
                //    //st.Start();
                //    currentWeather = DB.WeatherCollection.RetrieveWeatherData(Town);
                //    //st.Stop();
                //    //var t = st.Elapsed;
                //else
                //{
                //    if (DateTime.Now - currentWeather.CurrentWeather.DataTimestamp > new TimeSpan(2, 0, 0))
                //        currentWeather = DB.WeatherCollection.RetrieveWeatherData(Town);
                //}

                return currentWeather;
			}
		}
		
		[FieldHidden]
		public List<Cell> Cells = new List<Cell>();
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
		[FieldHidden]
		public DateTime CellsStateTimestamp;
		
		public int DbIndex {
			get { return DB.SitesDB.GetSiteIndex(Id); }
		}
		
		[FieldHidden]
		System.Timers.Timer UpdateTimer;
		
		[FieldHidden]
		DateTime siteDataTimestamp;
		public DateTime SiteDataTimestamp {
			get {
				return siteDataTimestamp;
			}
			set {
				siteDataTimestamp = value;
				if(Exists) {
					UpdateTimer = new System.Timers.Timer(((siteDataTimestamp + new TimeSpan(2, 0, 0)) - DateTime.Now).TotalMilliseconds);
					UpdateTimer.Elapsed += delegate {
						Site newData = DB.SitesDB.getSite(Id, true);
						OnTimerElapsed_UpdateSiteData(newData);
						if(newData.Exists) {
							siteDataTimestamp = DateTime.Now;
							UpdateTimer.Interval = ((siteDataTimestamp + new TimeSpan(2, 0, 0)) - DateTime.Now).TotalMilliseconds;
						}
						else
							DB.SitesDB.Remove(this);
					};
					UpdateTimer.Enabled = true;
				}
			}
		}
		
		public Site() {
		}
		
		public Site(string siteId) {
			Id = siteId;
		}
		
		/// <summary>
		/// Populate with data pulled from OI
		/// </summary>
		/// <param name="dataToRequest">"INC", "CRQ", "Bookins", "Alarms", "Availability", "PWR", "CellsState", "LKULK", "Cramer"</param>
		public void requestOIData(string dataToRequest) {
			if(Exists) {
				bool connected = !OiConnection.LoggedOn ? OiConnection.InitiateOiConnection() : true;
				
				if(connected) {
					List<Thread> threads = new List<Thread>();
					int finishedThreadsCount = 0;
					if(dataToRequest.Contains("INC")) {
						Thread thread = new Thread(() => {
						                           	isUpdatingIncidents = true;
//					                           	INCs = FetchINCs(null);
						                           	Incidents = FetchINCs();
						                           	isUpdatingIncidents = false;
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_INC";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("CRQ")) {
						Thread thread = new Thread(() => {
						                           	isUpdatingChanges = true;
//					                           	CRQs = FetchCRQs(null);
						                           	Changes = FetchCRQs();
						                           	isUpdatingChanges = false;
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_CRQ";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("Alarms")) {
						Thread thread = new Thread(() => {
						                           	isUpdatingAlarms = true;
//					                           	ActiveAlarms = FetchActiveAlarms(null);
						                           	Alarms = FetchActiveAlarms();
						                           	isUpdatingAlarms = false;
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_Alarms";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("Bookins")) {
						Thread thread = new Thread(() => {
						                           	isUpdatingVisits = true;
						                           	Visits = FetchBookIns();
						                           	if(Visits == null)
						                           		Visits = FetchBookIns(null);
						                           	isUpdatingVisits = false;
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_Bookins";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("Availability")) {
						Thread thread = new Thread(() => {
						                           	isUpdatingAvailability = true;
//						                           	Availability avail = FetchAvailability();
//						                           	if(avail != null) {
//						                           		if(avail.message != "Site is Offair") {
//						                           			Availability = avail.ToDataTable();
//						                           			updateCOOS();
//						                           		}
//						                           	}
//						                           	else {
						                           	Availability = FetchAvailability(null);
						                           	updateCOOS();
//						                           	}
						                           	isUpdatingAvailability = false;
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_Availability";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("PWR")) {
						Thread thread = new Thread(() => {
						                           	if(string.IsNullOrWhiteSpace(PowerCompany))
						                           		try { PowerCompany = getPowerCompany(); } catch {}
						                           	
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_PWR";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("CellsState")) {
						Thread thread = new Thread(() => {
						                           	getOiCellsState();
						                           	
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_CellsState";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("LKULK")) {
						Thread thread = new Thread(() => {
						                           	getOiLockedCellsPage();
						                           	
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_LKULK";
						threads.Add(thread);
					}
					
					if(dataToRequest.Contains("Cramer")) {
						Thread thread = new Thread(() => {
						                           	isUpdatingCramerData = true;
						                           	CramerData = FetchCramerData(null);
						                           	isUpdatingCramerData = false;
						                           	finishedThreadsCount++;
						                           });
						thread.Name = "requestOIData_Alarms";
						threads.Add(thread);
					}
					
					foreach(Thread thread in threads) {
						thread.SetApartmentState(ApartmentState.STA);
						thread.Start();
					}
					
					while(finishedThreadsCount < threads.Count) { }
					
					if(Exists) {
						if(DbIndex > -1)
							DB.SitesDB.UpdateSiteData(this);
						else
							DB.SitesDB.Add(this);
					}
				}
			}
		}
		
		List<Incident> FetchINCs() {
			IncidentsTimestamp = DateTime.Now;
			List<Incident> list = new List<Incident>();
			string response = OiConnection.requestApiOutput("incidents", Id);
			try {
				var jSon = JsonConvert.DeserializeObject<RootObject>(response);
				foreach(JObject jObj in jSon.data)
					list.Add(jObj.ToObject<Incident>());
			}
			catch { return null; }
			return list;
		}
		
//		DataTable FetchINCs(FileSystemInfo table_inc) {
//			DataTable dt = new DataTable();
//			string response = OiConnection.requestPhpOutput("inc", Id);
//			if(!string.IsNullOrEmpty(response) && !response.Contains("No open or incidents")) {
//				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_inc");
//				INCsTimestamp = DateTime.Now;
//			}
//			return dt;
//		}
		
		List<Change> FetchCRQs() {
			ChangesTimestamp = DateTime.Now;
			List<Change> list = new List<Change>();
			string response = OiConnection.requestApiOutput("changes", Id);
			try {
				var jSon = JsonConvert.DeserializeObject<RootObject>(response);
				foreach(JObject jObj in jSon.data) {
					Change item = jObj.ToObject<Change>();
					// remove HTML tags from Status
					while(item.Status.Contains(">")) {
						int startIndex = item.Status.IndexOf("<");
						int endIndex = item.Status.IndexOf(">");
						item.Status = item.Status.Remove(startIndex, (endIndex - startIndex) + 1);
					}
					list.Add(item);
				}
			}
			catch { return null; }
			return list;
		}
		
//		DataTable FetchCRQs(FileSystemInfo table_crq) {
//			DataTable dt = new DataTable();
//			string response = OiConnection.requestPhpOutput("crq", Id);
//			if(!string.IsNullOrEmpty(response) && !response.Contains("No changes in past 90 days")) {
//				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_crq");
//				foreach(DataRow row in dt.Rows) {
//					if(row["Scheduled Start"] == DBNull.Value) {
//						if(row["Scheduled End"] == DBNull.Value)
//							row["Scheduled Start"] = new DateTime(2500, 1, 1, 0, 0, 0);
//						else {
//							DateTime schEnd = Convert.ToDateTime(row["Scheduled End"]);
//							row["Scheduled Start"] = new DateTime(schEnd.Year, schEnd.Month, schEnd.Day, 0, 0, 0);
//						}
//					}
//					if(row["Scheduled End"] == DBNull.Value)
//						row["Scheduled End"] = row["Scheduled Start"];
//				}
//				CRQsTimestamp = DateTime.Now;
//			}
//			return dt;
//		}
		
		public static List<Change> BulkFetchCRQs(IEnumerable<string> sites) {
			List<Change> list = new List<Change>();
			string response = OiConnection.requestApiOutput("changes", sites);
			try {
				var jSon = JsonConvert.DeserializeObject<RootObject>(response);
				foreach(JObject jObj in jSon.data) {
					Change item = jObj.ToObject<Change>();
					// remove HTML tags from Status
					while(item.Status.Contains(">")) {
						int startIndex = item.Status.IndexOf("<");
						int endIndex = item.Status.IndexOf(">");
						item.Status = item.Status.Remove(startIndex, (endIndex - startIndex) + 1);
					}
					list.Add(item);
				}
			}
			catch { return null; }
			return list;
		}
		
		List<Alarm> FetchActiveAlarms() {
			AlarmsTimestamp = DateTime.Now;
			List<Alarm> list = new List<Alarm>();
			string response = OiConnection.requestApiOutput("alarms", Id);
			try {
				var jSon = JsonConvert.DeserializeObject<RootObject>(response);
				foreach(JObject jObj in jSon.data)
					list.Add(jObj.ToObject<Alarm>());
			}
			catch { return null; }
			return list;
		}
		
		public static List<Alarm> BulkFetchActiveAlarms(IEnumerable<string> sites) {
			return null;
		}

        public static List<WeatherItem> BulkFetchWeather(IEnumerable<string> locations)
        {
            List<WeatherItem> weatherList = new List<WeatherItem>();
            //List<int> ids = new List<int>();
            //List<string> locList = locations.ToList();
            //for(int c = 0;c < locList.Count;c++)
            //{
            //    var city = DB.Databases.Cities.FindCity(locList[c]);
            //    if (city != null)
            //    {
            //        ids.Add(city.id);
            //        locList.Remove(locList[c--]);
            //    }
            //}
            //if(ids.Any())
            //    weatherList.AddRange(DB.WeatherCollection.RetrieveWeatherData(ids));

            //if(locList.Any())
            //foreach (string location in locations)
            //    weatherList.Add(DB.WeatherCollection.RetrieveWeatherData(location));

            return weatherList;
        }
		
//		DataTable FetchActiveAlarms(FileSystemInfo table_alarms) {
//			DataTable dt = new DataTable();
//			string response = OiConnection.requestPhpOutput("alarms", Id);
//			if(!string.IsNullOrEmpty(response) && !response.Contains("No alarms reported")) {
//				dt = Tools.ConvertHtmlTabletoDataTable(response, "table_alarms");
//				ActiveAlarmsTimestamp = DateTime.Now;
//			}
//			return dt;
//		}
		
		List<BookIn> FetchBookIns() {
			VisitsTimestamp = DateTime.Now;
			List<BookIn> list = new List<BookIn>();
			string response = OiConnection.requestApiOutput("visits", Id);
			try {
				var jSon = JsonConvert.DeserializeObject<RootObject>(response);
				foreach(JObject jObj in jSon.data)
					list.Add(jObj.ToObject<BookIn>());
			}
			catch(Exception e) {
				var m = e.Message;
				return null;
			}
			return list;
		}
		
		List<BookIn> FetchBookIns(FileSystemInfo table_visits) {
			VisitsTimestamp = DateTime.Now;
			List<BookIn> list = new List<BookIn>();
//			string response = OiConnection.requestPhpOutput("sitevisit", Id, 90);
			string response = OiConnection.requestPhpOutput("visits", Id);
			if(!string.IsNullOrEmpty(response) && !response.Contains("No site visits")) {
				DataTable dt = Tools.ConvertHtmlTableToDT(response, "table_visits");
				foreach(DataRow row in dt.Rows) {
					BookIn bookin = new BookIn();
					bookin.Site = row[0].ToString();
					bookin.Visit = row[1].ToString();
					bookin.Company = row[2].ToString();
					bookin.Engineer = row[3].ToString();
					bookin.Mobile = row[4].ToString();
					bookin.Reference = row[5].ToString();
					bookin.Visit_Type = row[6].ToString();
					bookin.Arrived = row[7].ToString();
					bookin.Planned_Finish = row[8].ToString();
					bookin.Time_Taken = row[9].ToString();
					bookin.Time_Remaining = row[10].ToString();
					bookin.Departed_Site = row[11].ToString();
					list.Add(bookin);
				}
			}
			return list;
		}
		
		public static List<BookIn> BulkFetchBookIns(IEnumerable<string> sites) {
			return null;
		}
		
		Availability FetchAvailability() {
			AvailabilityTimestamp = DateTime.Now;
			string response = OiConnection.requestApiOutput("availability", Id);
			try {
				Availability jSon = JsonConvert.DeserializeObject<Availability>(response);

				return jSon;
			}
			catch { return null; }
		}
		
		DataTable FetchAvailability(FileSystemInfo table_ca) {
			AvailabilityTimestamp = DateTime.Now;
			DataTable dt = new DataTable();
			string response = OiConnection.requestPhpOutput("ca", Id);
			if(!string.IsNullOrEmpty(response)) {
				dt = Tools.ConvertHtmlTableToDT(response, "table_ca");
			}
			return dt;
		}
		
		public static DataTable BulkFetchAvailability(IEnumerable<string> sites) {
			string resp = OiConnection.requestApiOutput("availability", sites);
			DataTable dt = null;
			
			try {
				Availability jSon = JsonConvert.DeserializeObject<Availability>(resp);
				dt = jSon.ToDataTable();
			}
			catch { }
			
			if(dt == null) {
				resp = OiConnection.requestPhpOutput("ca", sites);
				
				try {
					dt = Tools.ConvertHtmlTableToDT(resp, "table_ca");
				}
				catch { }
			}
			
			return dt;
		}
		
		CramerDetails FetchCramerData(FileSystemInfo table_ca) {
			CramerDataTimestamp = DateTime.Now;
			DataTable dt = new DataTable();
			string response = OiConnection.requestApiOutput("labels-html", Id);
			if(!string.IsNullOrEmpty(response)) {
//				string str = response.Substring(response.IndexOf("<strong style=" + '"' + "margin-left: 15px;" + '"' + ">Cramer POC List"));
				string str = response.Substring(response.IndexOf("Cramer POC List"));
				if(str.Substring(24).StartsWith("<p style"))
					return null;
				str = str.Insert(str.IndexOf("<table") + 7, "id=" + '"' + "table_cramer" + '"' + " ");
				dt = Tools.ConvertHtmlTableToDT(str, "table_cramer");
			}

            return dt == null ? new CramerDetails(dt.Rows[0]) : null;
		}
		
		public static DataTable BulkFetchCramerData(IEnumerable<string> sites) {
			DataTable cramerDataList = null;
			string response = OiConnection.requestApiOutput("labels-html", sites);
			if(!string.IsNullOrEmpty(response)) {
				string str = response.Substring(response.IndexOf("Cramer POC List"));
				if(!str.Substring(24).StartsWith("<p style")) {
					str = str.Insert(str.IndexOf("<table") + 7, "id=" + '"' + "table_cramer" + '"' + " ");
					cramerDataList = Tools.ConvertHtmlTableToDT(str, "table_cramer");
				}
			}
			return cramerDataList;
		}
		
		string getPowerCompany() {
			string response = OiConnection.requestApiOutput("access", Id);
			try {
				var jSon = JsonConvert.DeserializeObject<RootObject>(response);
				foreach(JObject jObj in jSon.data) {
					AccessInformation item = jObj.ToObject<AccessInformation>();
					if(item.CI_NAME == Id)
						return item.POWER.Replace("<br>",";");
				}
			}
			catch { }
			
			return string.Empty;
		}
		
		public static List<AccessInformation> BulkFetchPowerCompany(IEnumerable<string> sites) {
			List<AccessInformation> powerList = new List<AccessInformation>();
			string response = OiConnection.requestApiOutput("access", sites);
			try {
				var jSon = JsonConvert.DeserializeObject<RootObject>(response);
				foreach(JObject jObj in jSon.data) {
					AccessInformation item = jObj.ToObject<AccessInformation>();
					if(!powerList.Contains(item))
						powerList.Add(item);
				}
			}
			catch { }
			
			return powerList;
		}
		
		void getOiLockedCellsPage() {
			string response = OiConnection.requestPhpOutput("cellslocked", Id, null, string.Empty);
			response = response.Contains("Site " + Id + "</b><table>") ? response : "No Cells Locked";
			
			if(response != "No Cells Locked") {
				HtmlDocument doc = new HtmlDocument();
				doc.Load(new StringReader(response));
				
				HtmlNode table = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/table[1]");
				
				LockedCellsDetails = Tools.ConvertHtmlTableToDT("<table>" + table.InnerHtml + "</table>", string.Empty);
			}
			else
				LockedCellsDetails = new DataTable();
			LockedCellsDetailsTimestamp = DateTime.Now;
		}
		
		void getOiCellsState() {
			CellsStateTimestamp = DateTime.Now;
			if(Exists && Cells.Count > 0) {
				List<OiCell> list = new List<OiCell>();
				
				string resp = OiConnection.requestApiOutput("cells-html", Id);
				DataTable dt = null;
				for(int c = 2;c <= 4;c++) {
					string tableName = "table_checkbox_cells " + c + "G";
					if(resp.Contains("id=" + '"' + tableName + '"')) {
						DataTable tempDT = Tools.ConvertHtmlTableToDT(resp, tableName);
						if(tempDT.Rows.Count > 0) {
							if(dt == null)
								dt = tempDT;
							else {
								foreach(DataRow row in tempDT.Rows)
									dt.Rows.Add(row.ItemArray);
							}
						}
					}
				}
				
				foreach(DataRow row in dt.Rows) {
					OiCell cell = new OiCell();
					cell.SITE = row[0].ToString();
					cell.BEARER = row[1].ToString();
					cell.CELL_NAME = row[2].ToString();
					cell.CELL_ID = row[3].ToString();
					cell.LAC_TAC = row[4].ToString();
					cell.BSC_RNC_ID = row[5].ToString();
					cell.ENODEB_ID = row[6].ToString();
					cell.WBTS_BCF = row[7].ToString();
					cell.VENDOR = row[8].ToString();
					cell.NOC = row[9].ToString();
					cell.COOS = row[10].ToString();
					string[] attr = row[11].ToString().Split('/');
					cell.JVCO_ID = attr.Length > 1 ? attr[1] : string.Empty;
//				    cell.LOCK = Convert.ToInt16(row[12]);
					cell.LOCKED = attr[0];
					list.Add(cell);
				}
				
				foreach (Cell cell in Cells) {
					OiCell oiCell = list.Find(s => s.CELL_NAME == cell.Name);
					if(oiCell != null) {
						cell.Locked = !string.IsNullOrEmpty(oiCell.LOCKED);
						cell.LockedFlagTimestamp = DateTime.Now;
						cell.COOS = !string.IsNullOrEmpty(oiCell.COOS);
						cell.CoosFlagTimestamp = DateTime.Now;
					}
				}
			}
		}
		
		public static List<OiCell> BulkFetchOiCellsState(IEnumerable<string> sites) {
			List<OiCell> list = new List<OiCell>();
			
			string resp = OiConnection.requestApiOutput("cells-html", sites);
			DataTable dt = null;
			for(int c = 2;c <= 4;c++) {
				string tableName = "table_checkbox_cells " + c + "G";
				if(resp.Contains("id=" + '"' + tableName + '"')) {
					DataTable tempDT = Tools.ConvertHtmlTableToDT(resp, tableName);
					if(tempDT.Rows.Count > 0) {
						if(dt == null)
							dt = tempDT;
						else {
							foreach(DataRow row in tempDT.Rows)
								dt.Rows.Add(row.ItemArray);
						}
					}
				}
			}
			
			foreach(DataRow row in dt.Rows) {
				OiCell cell = new OiCell();
				cell.SITE = row[0].ToString();
				cell.BEARER = row[1].ToString();
				cell.CELL_NAME = row[2].ToString();
				cell.CELL_ID = row[3].ToString();
				cell.LAC_TAC = row[4].ToString();
				cell.BSC_RNC_ID = row[5].ToString();
				cell.ENODEB_ID = row[6].ToString();
				cell.WBTS_BCF = row[7].ToString();
				cell.VENDOR = row[8].ToString();
				cell.NOC = row[9].ToString();
				cell.COOS = row[10].ToString();
				string[] attr = row[11].ToString().Split('/');
				cell.JVCO_ID = attr.Length > 1 ? attr[1] : string.Empty;
				cell.LOCKED = attr[0];
				list.Add(cell);
			}
			
			return list;
		}
		
		public void updateCOOS() {
			foreach(DataRow row in Availability.Rows) {
				Cell cell = null;
				try { cell = Cells.Find(s => s.Name == row["Cell"].ToString()); } catch { }
				if(cell != null) {
					int lastHour = string.IsNullOrEmpty(row[3].ToString()) ? 3600 : Convert.ToInt16(row[3].ToString());
					int previousHour = string.IsNullOrEmpty(row[4].ToString()) ? 3600 : Convert.ToInt16(row[4].ToString());
					if(lastHour >= 3599 || (lastHour > 0 && previousHour == 0))
						cell.COOS = true;
					else
						cell.COOS = false;
					cell.CoosFlagTimestamp = DateTime.Now;
				}
			}
		}
		
		Vendors resolveVendor(string strVendor) {
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
		
		public void populateCells() {
			Cells = DB.SitesDB.getCells(Id);
		}
		
		void SplitAddress() {
			string[] address = ADDRESS.Split(';');
			int addressLastIndex = address.Length - 1;
			try { postCode = address[addressLastIndex].Trim(); } catch (Exception) { }
			try { county = address[addressLastIndex - 1].Trim(); } catch (Exception) { }
			try { town = address[addressLastIndex - 2].Trim(); } catch (Exception) { }
		}
		
		public override string ToString() {
			return Exists ? Id : string.Empty;
		}

		void OnTimerElapsed_UpdateSiteData(Site newData) {
			SITE = newData.SITE;
			JVCO_ID = newData.JVCO_ID;
			GSM900 = newData.GSM900;
			GSM1800 = newData.GSM1800;
			UMTS900 = newData.UMTS900;
			UMTS2100 = newData.UMTS2100;
			LTE800 = newData.LTE800;
			LTE2600 = newData.LTE2600;
			LTE2100 = newData.LTE2100;
			Easting = newData.Easting;
			Northing = newData.Northing;
			Host = newData.Host;
			Priority = newData.Priority;
			ADDRESS = newData.ADDRESS;
			POWER_COMPANY = newData.POWER_COMPANY;
			POWER_CONTACT = newData.POWER_CONTACT;
			TellabsAtRisk = newData.TellabsAtRisk;
			Area = newData.Area;
			NSN_Status = newData.NSN_Status;
			NOC2G = newData.NOC2G;
			NOC3G = newData.NOC3G;
			NOC4G = newData.NOC4G;
			Region = newData.Region;
			ClusterName = newData.ClusterName;
			Special_Id = newData.Special_Id;
			Special = newData.Special;
			SPECIAL_START = newData.SPECIAL_START;
			SPECIAL_END = newData.SPECIAL_END;
			VIP = newData.VIP;
			SharedOperator = newData.SharedOperator;
			SharedOperatorSiteID = newData.SharedOperatorSiteID;
			Site_Access = newData.Site_Access;
			Site_Type = newData.Site_Type;
			Site_Subtype = newData.Site_Subtype;
			Paknet_Fitted = newData.Paknet_Fitted;
			Vodapage_Fitted = newData.Vodapage_Fitted;
			DC_STATUS = newData.DC_STATUS;
			dc_Timestamp = newData.dc_Timestamp;
			Cooling_Status = newData.Cooling_Status;
			cooling_Timestamp = newData.cooling_Timestamp;
			keyInformation = newData.keyInformation;
			EF_HealthAndSafety = newData.EF_HealthAndSafety;
			Switch2G = newData.Switch2G;
			Switch3G = newData.Switch3G;
			Switch4G = newData.Switch4G;
			DRSwitch2G = newData.DRSwitch2G;
			DRSwitch3G = newData.DRSwitch3G;
			DRSwitch4G = newData.DRSwitch4G;
			MTX2G = newData.MTX2G;
			MTX3G = newData.MTX3G;
			MTX4G = newData.MTX4G;
			IP_2G_I = newData.IP_2G_I;
			IP_2G_E = newData.IP_2G_E;
			IP_3G_I = newData.IP_3G_I;
			IP_3G_E = newData.IP_3G_E;
			IP_4G_I = newData.IP_4G_I;
			IP_4G_E = newData.IP_4G_E;
			VENDOR_2G = newData.VENDOR_2G;
			VENDOR_3G = newData.VENDOR_3G;
			VENDOR_4G = newData.VENDOR_4G;
			MTX_Related = newData.MTX_Related;
			
			Cells = newData.Cells;
			
			Coordinates = coordConvert.toLat_Long(new CoordPoint { Easting = Easting, Northing = Northing }, "OSGB36");
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
		

		public void Dispose()
		{
			if(Exists) {
				if(Cells.Count == 0)
					populateCells();
				if(Cells.Count > 0) {
					if(DbIndex > -1)
						DB.SitesDB.UpdateSiteData(this);
					else
						DB.SitesDB.Add(this);
				}
			}
		}
		
		~Site() { // destructor
			Dispose();
		}
		
		public class CramerDetails {
			public string PocType { get; private set; }
			public int OnwardSitesCount { get; private set; }
			public string TxMedium { get; private set; }
			public string TxLastMileRef { get; private set; }
			public List<string> OnwardSites { get; private set; }
			List<Site> onwardSitesObjects;
			public List<Site> OnwardSitesObjects {
				get {
					if(onwardSitesObjects == null) {
						List<Site> temp = null;
						if(OnwardSites.Count > 0)
							temp = DB.SitesDB.getSites(OnwardSites);
						onwardSitesObjects = temp ?? new List<Site>();
					}
					return onwardSitesObjects;
				}
				private set {
					onwardSitesObjects = value;
				}
			}
			//public string EvenflowStatus { get; private set; }
			
			public CramerDetails(DataRow details) {
				PocType = details["POC TYPE"].ToString();
				OnwardSitesCount = Convert.ToInt16(details["EFFECTED SITE COUNT"]);
				TxMedium = details["TX MEDIUM"].ToString();
				TxLastMileRef = details["TX LAST MILE REF"].ToString();
                //EvenflowStatus = details["EVENFLOW STATUS"].ToString();
				OnwardSites = new List<string>();
				string sitesList = details["EFFECTED SITE LIST"].ToString();
				if(!string.IsNullOrEmpty(sitesList)) {
					OnwardSites.AddRange(sitesList.Split(','));
					for(int c = 0;c < OnwardSites.Count;c++)
						if(OnwardSites[c].StartsWith("0"))
							OnwardSites[c] = Convert.ToInt16(OnwardSites[c]).ToString();
					OnwardSites = OnwardSites.OrderBy(c => int.Parse(c)).ToList();
				}
				Thread thread = new Thread(() => {
				                           	var t = OnwardSitesObjects;
				                           });
				thread.Name = "Site " + details[0] + " CramerDetails_GetOnwardSites";
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
		}
	}
}

public static class DtTools {
	public static DataTable ToDataTable(this Availability jSon) {
		DataTable dataTable = new DataTable("Availability");

		for(int c = 0;c < 3; c++)
			dataTable.Columns.Add(jSon.title[c]);
		for(int c = jSon.title.Count - 4;c > 2;c--)
			dataTable.Columns.Add(jSon.title[c]);
		
		foreach(AvailabilityRows cell in jSon.data) {
			DataRow row = dataTable.NewRow();
			int rowCol = 0;
			row[rowCol++] = cell.CI_NAME;
			row[rowCol++] = cell.CELL;
			row[rowCol++] = cell.BEARER;
			for(int c = 193; c > 0;c--) {
				var prop = cell.GetType().GetProperty("COL" + c);
				row[rowCol++] = prop.GetValue(cell);
				// remove HTML tags from Status
				while(row[rowCol - 1].ToString().Contains(">")) {
					int startIndex = row[rowCol - 1].ToString().IndexOf("<");
					int endIndex = row[rowCol - 1].ToString().IndexOf(">");
					row[rowCol - 1] = row[rowCol - 1].ToString().Remove(startIndex, (endIndex - startIndex) + 1);
				}
			}
			dataTable.Rows.Add(row);
		}
		return dataTable;
	}
}
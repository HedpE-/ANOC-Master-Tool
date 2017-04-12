/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 04-12-2015
 * Time: 22:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using appCore.UI;
using appCore.Templates.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using appCore.OI.JSON;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of siteDetails.
	/// </summary>
	public partial class siteDetails : Form
	{
		GMapControl myMap;
		GMapOverlay markersOverlay = new GMapOverlay("markers");
		GMapOverlay selectedSiteOverlay = new GMapOverlay("selectedSite");
		GMapOverlay onwardSitesOverlay = new GMapOverlay("onwardSites");
//		List<GMapMarker> markersList = new List<GMapMarker>();
		public Site currentSite;
		public Outage currentOutage;
		
		List<Site> foundSites = new List<Site>(); // for outage and bulk sites lists
//		string[] sites; // for outages site list
//		byte listView2_EventCount = 1;
		
		AMTMenuStrip MainMenu = new AMTMenuStrip(1090);
		ToolStripMenuItem bulkSiteSearchMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem lockUnlockCellsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem lockedCellsPageToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem viewSiteInOiToolStripMenuItem = new ToolStripMenuItem();
		
		public Control parentControl {
			get;
			private set;
		}
		
		string _siteDetails_UIMode;
		/// <summary>
		/// Valid values: "single","single/readonly","multi",multi/readonly","outage"
		/// </summary>
		public string siteDetails_UIMode {
			get {
				return _siteDetails_UIMode;
			}
			set {
				string[] mode = value.Split('/');
				MainMenu.siteFinder_Toggle(false, false);
				MainMenu.MainMenu.DropDownItems.Clear();
				pictureBox1.Left = dataGridView1.Left + ((dataGridView1.Width - pictureBox1.Width) / 2);
				listView2.Width = dataGridView1.Width;
				int filterCheckBoxesStartPosition = dataGridView1.Left + ((dataGridView1.Width - (checkBox1.Width + checkBox2.Width + checkBox3.Width)) / 2);
				checkBox1.Left = filterCheckBoxesStartPosition;
				checkBox2.Left = filterCheckBoxesStartPosition + checkBox1.Width;
				checkBox3.Left = filterCheckBoxesStartPosition + checkBox1.Width + checkBox2.Width;
				checkBox6.Left = dataGridView1.Right - checkBox6.Width;
				checkBox7.Left = checkBox6.Left - checkBox7.Width;
				if(mode[0] == "single" || mode[0] == "multi") {
					if(!value.Contains("readonly")) {
						MainMenu.MainMenu.DropDownItems.Add(bulkSiteSearchMenuItem);
						MainMenu.MainMenu.DropDownItems.Add("-");
						MainMenu.MainMenu.DropDownItems.Add(lockUnlockCellsToolStripMenuItem);
						MainMenu.MainMenu.DropDownItems.Add("-");
						MainMenu.MainMenu.DropDownItems.Add(lockedCellsPageToolStripMenuItem);
					}
					bulkSiteSearchMenuItem.Enabled =
						lockedCellsPageToolStripMenuItem.Enabled = !value.Contains("readonly");
				}
				switch(mode[0]) {
					case "single":
//						if(!value.Contains("readonly")) {
//							MainMenu.MainMenu.DropDownItems.Add(bulkSiteSearchMenuItem);
//							MainMenu.MainMenu.DropDownItems.Add("-");
//							MainMenu.MainMenu.DropDownItems.Add(lockUnlockCellsToolStripMenuItem);
//							MainMenu.MainMenu.DropDownItems.Add("-");
//							MainMenu.MainMenu.DropDownItems.Add(lockedCellsPageToolStripMenuItem);
//						}
//						bulkSiteSearchMenuItem.Enabled =
//							lockedCellsPageToolStripMenuItem.Enabled = !value.Contains("readonly");
						label11.Visible =
							listView2.Visible = false;
						dataGridView1.Top = listView2.Top;
						label12.Top = dataGridView1.Top - label12.Height;
						textBox1.ReadOnly = value.Contains("readonly");
						break;
					case "multi":
						dataGridView1.Top = listView2.Bottom + 23;
						label11.Visible =
							listView2.Visible = true;
						listView2.Items.Clear();
//						if(!value.Contains("readonly"))
//							MainMenu.MainMenu.DropDownItems.Add(bulkSiteSearchMenuItem);
//						bulkSiteSearchMenuItem.Enabled = !value.Contains("readonly");
						textBox1.ReadOnly = value.Contains("readonly");
						break;
					case "outage":
						dataGridView1.Top = listView2.Bottom + 23;
						label11.Visible =
							listView2.Visible = true;
						listView2.Items.Clear();
						label12.Top = 352;
						textBox1.ReadOnly = true;
						break;
				}
				if(!value.Contains("single")) {
					Button overview = new Button();
					overview.Name = "OverviewButton";
					overview.Text = "Back to map overview";
					overview.Width = 130;
					overview.Location = new Point(listView2.Right - overview.Width, listView2.Top - overview.Height - 3);
					overview.Click += delegate {
						try { myMap.Overlays.Remove(selectedSiteOverlay); } catch (Exception) { }
						try { myMap.Overlays.Remove(onwardSitesOverlay); } catch (Exception) { }
						
						listView2.SelectedIndices.Clear();
					};
					Controls.Add(overview);
				}
				else {
					if(Controls["OverviewButton"] != null)
						Controls.Remove(Controls["OverviewButton"]);
				}
				checkBox1.Top =
					checkBox2.Top =
					checkBox3.Top =
					checkBox6.Top =
					checkBox7.Top = dataGridView1.Top - checkBox1.Height + 1;
				label12.Top = dataGridView1.Top - label12.Height;
				dataGridView1.Height = myMap.Bottom - dataGridView1.Top;
				_siteDetails_UIMode = value;
			}
		}
		
		public siteDetails(Site site, Control parent = null)
		{
			currentSite = site;
			
			parentControl = parent;
//			switch(parent.GetType().ToString()) {
//				case "appCore.Templates.UI.TroubleshootControls":
//					currentSite = ((TroubleshootControls)parentControl).currentSite;
//					break;
//				case "appCore.Templates.UI.FailedCRQControls":
//					currentSite = ((FailedCRQControls)parentControl).currentSite;
//					break;
//				case "appCore.Templates.UI.UpdateControls":
//					currentSite = ((UpdateControls)parentControl).currentSite;
//					break;
//			}
			InitializeComponent();
			this.Name =
				this.Text = "Site " + site.Id + " Details";
			
			Controls.Add(MainMenu);
			MainMenu.InitializeTroubleshootMenu(true);
			MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
			MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
			InitializeToolStripMenuItems();
			
			myMap = drawGMap("myMap",false);
			Controls.Add(myMap);
			
			siteDetails_UIMode = "single/readonly";
			
			Shown += populateSingleForm;
		}
		
		public siteDetails(Outage outage, Control parent)
		{
			InitializeComponent();
			
			parentControl = parent;
			currentOutage = outage;
			// TODO: request Cramer Data on opening
			Controls.Add(MainMenu);
			MainMenu.InitializeTroubleshootMenu(true);
			MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
			MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
			InitializeToolStripMenuItems();
			
			Name =
				Text = "Outage Follow-up";
			
			myMap = drawGMap("myMap",true);
			this.Controls.Add(myMap);
			
			if(outage != null) {
				siteDetails_UIMode = "outage";
				
//				sites = (parent.currentOutage.VfBulkCI + parent.currentOutage.TefBulkCI).Split(';');
//				for(int c = 0;c < sites.Length;c++) {
//					if(sites[c].StartsWith("0")) {
//						while(sites[c].StartsWith("0"))
//							sites[c] = sites[c].Substring(1);
//					}
//					else
//						break;
//				}
//				sites = sites.Distinct().ToArray(); // Remover duplicados
//				sites = sites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
//				parent.currentOutage.AffectedSites = DB.SitesDB.getSites(sites.ToList());
//
//				List<string> cells = parent.currentOutage.VfGsmCells;
//				cells.AddRange(parent.currentOutage.VfUmtsCells);
//				cells.AddRange(parent.currentOutage.VfLteCells);
//				cells.AddRange(parent.currentOutage.TefGsmCells);
//				cells.AddRange(parent.currentOutage.TefUmtsCells);
//				cells.AddRange(parent.currentOutage.TefLteCells);
//
//				for(int c = 0;c < cells.Count;c++) {
//					string[] strToFind = { " - " };
//					var tempCell = cells[c].Split(strToFind, StringSplitOptions.None);
//					if(tempCell.Length > 1)
//						cells[c] = tempCell[1];
//				}
//
//				parent.currentOutage.AffectedCells = DB.SitesDB.getCells(cells);
//				foreach(Cell cell in parent.currentOutage.AffectedCells) {
//					Site site = parent.currentOutage.AffectedSites.Find(s => s.Id == cell.ParentSite);
//					int siteIndex = parent.currentOutage.AffectedSites.IndexOf(site);
//					if(siteIndex > -1) {
//						if(parent.currentOutage.AffectedSites[siteIndex].CellsInOutage == null)
//							parent.currentOutage.AffectedSites[siteIndex].CellsInOutage = new List<Cell>();
//						parent.currentOutage.AffectedSites[siteIndex].CellsInOutage.Add(cell);
//					}
//				}
			}
			else
				siteDetails_UIMode = "single";
			
			this.Shown += populateBulkForm;
		}
		
		public siteDetails()
		{
			InitializeComponent();
			this.Name =
				this.Text = "Site Finder";
			
			myMap = drawGMap("myMap",true);
			this.Controls.Add(myMap);
			
			Controls.Add(MainMenu);
			MainMenu.InitializeTroubleshootMenu(true);
			MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
			MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
			InitializeToolStripMenuItems();
			
			siteDetails_UIMode = "single";
			
			lockUnlockCellsToolStripMenuItem.Enabled = false;
			this.Shown += populateBulkForm;
		}
		
		void populateSingleForm(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	try { myMap.Overlays.Remove(markersOverlay); } catch (Exception) { }
			                           	try { markersOverlay.Clear(); } catch (Exception) { }
			                           	try { myMap.Overlays.Remove(selectedSiteOverlay); } catch (Exception) { }
			                           	try { selectedSiteOverlay.Clear(); } catch (Exception) { }
			                           	try { myMap.Overlays.Remove(onwardSitesOverlay); } catch (Exception) { }
			                           	try { onwardSitesOverlay.Clear(); } catch (Exception) { }
			                           	
			                           	initializeListviews();
			                           	
			                           	selectedSiteDetailsPopulate();
			                           });
			
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, action,true, this);
		}
		
		void populateBulkForm(object sender, EventArgs e)
		{
			// TODO: Multi select sites and show only their markers
			// TODO: Show cells for selected sites
			Action action = new Action(delegate {
			                           	initializeListviews();
			                           	if(siteDetails_UIMode.Contains("outage")) {
			                           		
			                           		try { myMap.Overlays.Remove(markersOverlay); } catch (Exception) { }
			                           		try { markersOverlay.Clear(); } catch (Exception) { }
			                           		try { myMap.Overlays.Remove(selectedSiteOverlay); } catch (Exception) { }
			                           		try { selectedSiteOverlay.Clear(); } catch (Exception) { }
			                           		try { myMap.Overlays.Remove(onwardSitesOverlay); } catch (Exception) { }
			                           		try { onwardSitesOverlay.Clear(); } catch (Exception) { }
			                           		
			                           		List<string> sites = new List<string>();
			                           		foreach(Site site in currentOutage.AffectedSites) {
			                           			if(string.IsNullOrEmpty(site.PowerCompany))
			                           				sites.Add(site.Id);
			                           		}
			                           		if(sites.Count > 0) {
			                           			string resp = OI.OiConnection.requestApiOutput("access", sites);
			                           			try {
			                           				var jSon = JsonConvert.DeserializeObject<RootObject>(resp);
			                           				foreach(JObject jObj in jSon.data) {
			                           					AccessInformation item = jObj.ToObject<AccessInformation>();
			                           					int siteIndex = currentOutage.AffectedSites.FindIndex(s => s.Id == item.CI_NAME);
			                           					if(siteIndex > -1) {
			                           						if(string.IsNullOrEmpty(currentOutage.AffectedSites[siteIndex].PowerCompany))
			                           							currentOutage.AffectedSites[siteIndex].PowerCompany = item.POWER.Replace("<br>",";");
			                           					}
			                           				}
			                           			}
			                           			catch { }
			                           		}
			                           		
			                           		if(currentOutage.AffectedSites.Count > 1) {
			                           			searchResultsPopulate();
			                           			myMap.Overlays.Add(markersOverlay);
			                           			listView2.Focus();
			                           			myMap.ZoomAndCenterMarkers(markersOverlay.Id);
			                           		}
			                           		else {
			                           			if(currentOutage.AffectedSites.Count == 1) {
			                           				siteDetails_UIMode = "single";
			                           				currentSite = currentOutage.AffectedSites[0];
			                           				selectedSiteDetailsPopulate();
			                           			}
			                           		}
			                           	}
			                           	else
			                           		textBox1.Select();
			                           });
			
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, action,true,this);
		}
		
		void initializeListviews() {
			listView2.View = View.Details;
			listView2.Columns.Add("Site");
			listView2.Columns.Add("JVCO ID");
			listView2.Columns.Add("Priority");
			listView2.Columns.Add("Site Host");
			listView2.Columns.Add("Post Code");
		}
		
		GMapControl drawGMap(string mapName, bool multi) {
			// TODO: implement weather layer if possible with GMaps
			GMapProvider.TimeoutMs = 20*1000;
			IWebProxy proxy;
			try {
				proxy = Settings.CurrentUser.NetworkDomain == "internal.vodafone.com" ?
					new WebProxy("http://10.74.51.1:80/", true) :
					WebRequest.GetSystemWebProxy();
			}
			catch (Exception) {
				proxy = WebRequest.GetSystemWebProxy();
			}
			
			proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
			GMapProvider.WebProxy = proxy;
			
			GMapControl map = new GMapControl();
			map.Name = mapName;
			map.Location = new Point(dataGridView1.Right + 5, amtTextBox3.Bottom + 5 + amtTextBox3.Height + 5);
			map.Size = new Size(button1.Right - map.Left, dataGridView1.Bottom - map.Top);
			map.MapProvider = GoogleHybridMapProvider.Instance;
			
			map.Manager.Mode = AccessMode.ServerOnly; //get tiles from server only
			map.CanDragMap = true;
			map.ShowCenter = false;
			map.MouseWheelZoomEnabled = true;
			map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
			map.DragButton = MouseButtons.Left;
			map.DisableFocusOnMouseEnter = true;
			map.PolygonsEnabled = false;
			map.RoutesEnabled = false;
			map.MarkersEnabled = true;
			
			map.MinZoom = 4;
			map.MaxZoom = 18;
			map.SetPositionByKeywords("UK");
			map.Zoom = 5;
			map.KeyDown += GMapKeyDown;
			if(multi)
				map.OnMarkerClick += GMapSiteMarkerClick;
			return map;
		}
		
		void selectedSiteDetailsPopulate() {
			if(currentSite.Exists) {
				textBox1.Text = currentSite.Id;
				textBox2.Text = currentSite.PowerCompany;
				textBox3.Text = currentSite.JVCO_Id;
				textBox4.Text = currentSite.Address.Replace(';',',');
				textBox5.Text = currentSite.Area;
				textBox6.Text = currentSite.Region;
				textBox8.Text = currentSite.Host;
				textBox7.Text = currentSite.SharedOperatorSiteID == string.Empty ? textBox1.Text : currentSite.SharedOperatorSiteID;
				textBox9.Text = currentSite.Priority;
				textBox10.Text = currentSite.SharedOperator;
				amtTextBox2.Text = currentSite.Site_Type;
				amtTextBox3.Text = currentSite.Site_Subtype;
				amtTextBox7.Text = currentSite.Site_Access;
				checkBox4.Checked = currentSite.Paknet_Fitted;
				checkBox5.Checked = currentSite.Vodapage_Fitted;
				richTextBox1.Text = currentSite.KeyInformation;
				amtTextBox4.Text = currentSite.CramerData != null ? currentSite.CramerData.PocType : string.Empty;
				amtTextBox5.Text = currentSite.CramerData != null ? currentSite.CramerData.OnwardSitesCount.ToString() : string.Empty;
				button2.Enabled = currentSite.CramerData != null ? currentSite.CramerData.OnwardSitesCount > 0 : false;
				amtTextBox6.Text = currentSite.CramerData != null ? currentSite.CramerData.TxLastMileRef : string.Empty;
				lockUnlockCellsToolStripMenuItem.Enabled = siteDetails_UIMode.Contains("single") && !siteDetails_UIMode.Contains("readonly");
				
				if(currentSite.CramerData == null && currentSite.CramerDataTimestamp < Settings.GlobalProperties.ApplicationStartTime)
					currentSite.requestOIData("Cramer");
				if(currentSite.CramerData != null) {
					if(currentSite.CramerData.OnwardSitesCount > 0) {
						var sites = DB.SitesDB.getSites(currentSite.CramerData.OnwardSites);
						foreach(Site site in sites) {
							GMarkerGoogle tempMarker = new GMarkerGoogle(site.MapMarker.Position, GMarkerGoogleType.blue);
							tempMarker.Tag = site.MapMarker.Tag;
							tempMarker.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapBaloonToolTip(tempMarker);
							tempMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
							tempMarker.ToolTip.Fill = new SolidBrush(Color.FromArgb(180, Color.Black));
							tempMarker.ToolTip.Font  = new Font("Courier New", 9, FontStyle.Bold);
							tempMarker.ToolTip.Foreground = new SolidBrush(Color.White);
							tempMarker.ToolTip.Stroke = new Pen(Color.Red);
							tempMarker.ToolTip.Offset.X -= 15;
							tempMarker.ToolTipText = site.MapMarker.ToolTipText;
							
							onwardSitesOverlay.Markers.Add(tempMarker);
						}
						myMap.Overlays.Add(onwardSitesOverlay);
					}
				}
				selectedSiteOverlay.Markers.Add(currentSite.MapMarker);
				myMap.Overlays.Add(selectedSiteOverlay);
				if(!siteDetails_UIMode.Contains("single")) {
					var t = onwardSitesOverlay.Markers.Count;
					if(myMap.Overlays.Contains(onwardSitesOverlay))
						myMap.ZoomAndCenterMarkers(onwardSitesOverlay.Id);
					else {
						myMap.Position = markersOverlay.Markers.First(m => m.Tag.ToString() == textBox1.Text).Position;
						myMap.Zoom = 14;
					}
				}
				else {
					if(myMap.Overlays.Contains(onwardSitesOverlay))
						myMap.ZoomAndCenterMarkers(onwardSitesOverlay.Id);
					else
						myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
				}
			}
			else {
				foreach(Control ctr in Controls) {
					if(ctr.Name != "textBox1" && !ctr.Name.Contains("label") && !string.IsNullOrEmpty(ctr.Name)) {
						if(ctr.Name != "dataGridView1") {
							TextBoxBase tb = ctr as TextBoxBase;
							if(tb != null)
								tb.Text = string.Empty;
							else {
								if(ctr.Name == "button2") {
									Button btn = ctr as Button;
									btn.Enabled = false;
								}
							}
						}
					}
				}
				textBox4.Text = "Site not found";
				myMap.SetPositionByKeywords("UK");
				myMap.Zoom = 6;
				lockUnlockCellsToolStripMenuItem.Enabled = false;
			}
			
			dataGridView1.DataSource = null;
			dataGridView1.Columns.Clear();
			
			dataGridView1.DataSource = GetCellsDV();
			int c = 0;
			int[] columnWidths = { 35, 80, 50, 56, 70, 65, 55, 46, 45, 50 };
			foreach(DataGridViewColumn dgvc in dataGridView1.Columns) {
				dgvc.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
				dgvc.Width = columnWidths[c++];
				if(dgvc.Name == "Tech" || dgvc.Name == "COOS" || dgvc.Name == "Locked")
					dgvc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			}
			if(siteDetails_UIMode.Contains("outage"))
				addCheckBoxColumn();
			
			MainMenu.siteFinder_Toggle(currentSite.Exists);
			pictureBox1.UpdateCells(currentSite.Cells);
		}
		
		DataView GetCellsDV() {
			DataTable table = new DataTable();
			table.TableName = "Cells";
			table.Columns.Add("Tech");
			table.Columns.Add("Cell Name");
			table.Columns.Add("Cell ID");
			table.Columns.Add("LAC TAC");
			table.Columns.Add("Switch");
			table.Columns.Add("OSS ID");
			table.Columns.Add("Vendor");
			table.Columns.Add("NOC");
			if(!siteDetails_UIMode.Contains("outage"))
				table.Columns.Add("COOS");
			table.Columns.Add("Locked");
			
			foreach(Cell cell in currentSite.Cells) {
				string ossID;
				if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
					ossID = cell.ENodeB_Id;
				else
					ossID = cell.WBTS_BCF;
				var row = table.Rows.Add(cell.Bearer, cell.Name, cell.Id, cell.LacTac, cell.BscRnc_Id, ossID, cell.Vendor.ToString(), cell.Noc);
				if(!siteDetails_UIMode.Contains("outage"))
					row["COOS"] = cell.COOS ? "YES" : "No";
				row["Locked"] = cell.Locked ? "YES" : "No";
			}
			
			return table.DefaultView;
		}
		
		DataGridViewCheckBoxColumn addCheckBoxColumn() {
			DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
			chkColumn.Name = "CheckBoxes";
			chkColumn.HeaderText = "";
			chkColumn.ThreeState = false;
			chkColumn.FalseValue = false;
			chkColumn.TrueValue = true;
			chkColumn.Width = 50;
			chkColumn.ToolTipText = "Cells Included in Outage\n\nSelect/Unselect all";
			dataGridView1.Columns.Insert(0, chkColumn);
			
			Rectangle rect = dataGridView1.GetCellDisplayRectangle(0, -1, true);
			
			CheckBox checkboxHeader = new CheckBox();
			checkboxHeader.BackColor = Color.Transparent;
			checkboxHeader.Name = "checkboxHeader";
			checkboxHeader.Size = new Size(18, 18);
			checkboxHeader.Location = new Point(rect.X + 2 + ((rect.Width - checkboxHeader.Width) / 2),
			                                    rect.Y + 1 + ((rect.Height - checkboxHeader.Height) / 2));
			checkboxHeader.CheckedChanged += datagridViewCheckBoxHeaderCell_OnCheckBoxClicked;
			dataGridView1.Controls.Add(checkboxHeader);
			
			return chkColumn;
		}
		
		void datagridViewCheckBoxHeaderCell_OnCheckBoxClicked(object sender, EventArgs e)
		{
			CheckBox cb = sender as CheckBox;

			foreach(DataGridViewRow dgvr in dataGridView1.Rows) {
				DataGridViewCheckBoxCell cell = dgvr.Cells[0] as DataGridViewCheckBoxCell;
				
				cell.Value = cb.Checked ? cell.TrueValue : cell.FalseValue;
			}
		}
		
		void GMapKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode) {
				case Keys.Add:
					myMap.Zoom += 1;
					return;
				case Keys.Subtract:
					myMap.Zoom -= 1;
					return;
			}
		}
		
		void GMapSiteMarkerClick(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				GMarkerGoogle marker = (GMarkerGoogle)sender;
				listView2.ItemSelectionChanged -= ListView2ItemSelectionChanged;
				int markerIndex = -1;
				foreach (ListViewItem item in listView2.Items) {
					if(item.SubItems[0].Text == marker.Tag.ToString())
						markerIndex = item.Index;
					item.Selected = false;
				}
				listView2.ItemSelectionChanged += ListView2ItemSelectionChanged;
				if(markerIndex > -1)
					listView2.Items[markerIndex].Selected = true;
			}
		}
		
		void dataGridViewFilter_Changed(object sender, EventArgs e)
		{
			bool show2G = checkBox1.Checked;
			bool show3G = checkBox2.Checked;
			bool show4G = checkBox3.Checked;
			DataView dv = dataGridView1.DataSource as DataView;
			if(dv != null) {
				dv.RowFilter = null;
				string rowFilter = string.Empty;
				if(!show2G && !show3G && !show4G)
					rowFilter = "(Tech = ''";
				else {
					if(!show2G || !show3G || !show4G) {
						rowFilter += "(";
						if(show2G)
							rowFilter += "Tech = '2G'";
						if(show3G) {
							if(rowFilter.Length > 1)
								rowFilter += " or ";
							rowFilter += "Tech = '3G'";
						}
						if(show4G) {
							if(rowFilter.Length > 1)
								rowFilter += " or ";
							rowFilter += "Tech = '4G'";
						}
					}
				}
				if(!string.IsNullOrEmpty(rowFilter))
					rowFilter += ")";
				if(rowFilter != "(Tech = '')" && !(checkBox7.Checked && checkBox6.Checked)) {
					rowFilter += string.IsNullOrEmpty(rowFilter) ? "(" : " and (";
					if(checkBox7.Checked)
						rowFilter += "[Cell Name] NOT LIKE 'T*' and [Cell Name] NOT LIKE '*W' and [Cell Name] NOT LIKE '*X' and [Cell Name] NOT LIKE '*Y'";
					else {
						if(checkBox6.Checked)
							rowFilter += "[Cell Name] LIKE 'T*' or [Cell Name] LIKE '*W' or [Cell Name] LIKE '*X' or [Cell Name] LIKE '*Y'";
						else
							rowFilter += "[Cell Name] = ''";
					}
					rowFilter += ")";
				}
				dv.RowFilter = rowFilter;
				dv.Sort = "Tech Asc";
				dataGridView1.DataSource = dv;
			}
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name) {
				case "textBox4":
					btn = (Button)button45;
					break;
				case "richTextBox1":
					btn = (Button)button1;
					break;
			}
			
			btn.Enabled = !string.IsNullOrEmpty(tb.Text);
		}

		void LargeTextButtonsClick(object sender, EventArgs e)
		{
			Button bt = sender as Button;
			Action action = null;
			
			switch(bt.Name) {
				case "button45":
					action = new Action(delegate {
					                    	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox4.Text,label4.Text,true);
					                    	enlarge.StartPosition = FormStartPosition.CenterParent;
					                    	enlarge.ShowDialog();
					                    });
					break;
				case "button1":
					action = new Action(delegate {
					                    	AMTLargeTextForm enlarge = new AMTLargeTextForm(richTextBox1.Text,label16.Text,true);
					                    	enlarge.StartPosition = FormStartPosition.CenterParent;
					                    	enlarge.ShowDialog();
					                    });
					break;
			}
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, action,false,this);
		}
		
		void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
			switch(dataGridView1.Columns[e.ColumnIndex].Name) {
				case "Locked": case "COOS":
					e.CellStyle.BackColor = e.Value.ToString() == "YES" ? Color.OrangeRed : Color.LightGreen;
					break;
				case "Cell Name":
					if(!string.IsNullOrEmpty(e.Value.ToString())) {
						string cellPrefix = e.Value.ToString();
						if(cellPrefix.EndsWith("W") || cellPrefix.EndsWith("X") || cellPrefix.EndsWith("Y"))
							cellPrefix = cellPrefix.Substring(0, cellPrefix.Length - 2);
						cellPrefix = cellPrefix.RemoveDigits();
						if(cellPrefix.StartsWith("T"))
							cellPrefix = cellPrefix.Substring(1);
						string prefixDescription = string.Empty;
						List<string> temp = Resources.Cells_Prefix.Split('\n').ToList();
						string tempStr = temp.Find(s => s.StartsWith(cellPrefix));
						if(!string.IsNullOrEmpty(tempStr)) {
							string[] strTofind = { " - " };
							prefixDescription = tempStr.Split(strTofind, StringSplitOptions.None)[1];
						}
						
						dataGridView1.Rows[e.RowIndex].Cells["Cell Name"].ToolTipText = cellPrefix;
						if(!string.IsNullOrEmpty(prefixDescription))
							dataGridView1.Rows[e.RowIndex].Cells["Cell Name"].ToolTipText += Environment.NewLine + prefixDescription;
					}
					break;
				case "CheckBoxes":
					if(currentOutage.AffectedCells.FindIndex(s => s.Name == dataGridView1.Rows[e.RowIndex].Cells["Cell Name"].Value.ToString()) > -1) {
						e.CellStyle.BackColor = Color.OrangeRed;
						DataGridViewCheckBoxCell cell = dataGridView1[e.ColumnIndex, e.RowIndex] as DataGridViewCheckBoxCell;
						cell.Value = cell.TrueValue;
					}
					break;
			}
		}

//		void ListView1KeyDown(object sender, KeyEventArgs e)
//		{
//			if(e.Control && e.KeyCode != Keys.ControlKey) {
//				switch(e.KeyCode) {
//					case Keys.A:
//						if(listView1.SelectedItems.Count < listView1.Items.Count) {
//							foreach (ListViewItem item in listView1.Items)
//							{
//								item.Selected = true;
//							}
//						}
//						break;
//					case Keys.E:
//						if(listView1.SelectedItems.Count > 0) {
//							foreach (ListViewItem item in listView1.Items)
//							{
//								item.Selected = false;
//							}
//						}
//						break;
//				}
//			}
//		}

		void PaknetVodapageCheckBoxesMouseUp(object sender, MouseEventArgs e) {
			CheckBox cb = sender as CheckBox;
			cb.Checked = !cb.Checked;
		}

		void bulkSiteSearchMenuItemClick(object sender, EventArgs e)
		{
			AMTRichTextBox sitesList_tb = new AMTRichTextBox();
			Form form = new Form();
			Action action = new Action(delegate {
			                           	using (form) {
			                           		// 
			                           		// titleLabel
			                           		// 
			                           		Label titleLabel = new Label();
			                           		titleLabel.Location = new Point(3, 5);
			                           		titleLabel.Name = "label";
			                           		titleLabel.AutoSize = true;
			                           		titleLabel.Text = "Please provide list of sites to search";
			                           		titleLabel.TextAlign = ContentAlignment.MiddleCenter;
			                           		// 
			                           		// sitesList_tb
			                           		//
			                           		sitesList_tb.Location = new Point(3, titleLabel.Top + titleLabel.Height - 2);
			                           		sitesList_tb.Name = "sitesList_tb";
			                           		sitesList_tb.Multiline = true;
			                           		sitesList_tb.Size = new Size(175, 280);
			                           		sitesList_tb.TabIndex = 111;
			                           		// 
			                           		// continueButton
			                           		// 
			                           		Button continueButton = new Button();
			                           		continueButton.Location = new Point(3, (sitesList_tb.Top + sitesList_tb.Height) + 4);
			                           		continueButton.Name = "continueButton";
			                           		continueButton.Size = new Size(86, 23);
			                           		continueButton.TabIndex = 6;
			                           		continueButton.Text = "Continue";
			                           		continueButton.Click += bulkSearchForm_buttonClick;
			                           		// 
			                           		// cancelButton
			                           		// 
			                           		Button cancelButton = new Button();
			                           		cancelButton.Location = new Point((continueButton.Left + continueButton.Width) + 3, (sitesList_tb.Top + sitesList_tb.Height) + 4);
			                           		cancelButton.Name = "cancelButton";
			                           		cancelButton.Size = new Size(86, 23);
			                           		cancelButton.TabIndex = 6;
			                           		cancelButton.Text = "Cancel";
			                           		cancelButton.Click += bulkSearchForm_buttonClick;
			                           		// 
			                           		// Form1
			                           		// 
			                           		form.AutoScaleDimensions = new SizeF(6F, 13F);
			                           		form.AutoScaleMode = AutoScaleMode.Font;
			                           		form.ClientSize = new Size(182, 337);
			                           		form.Icon = Resources.MB_0001_vodafone3;
			                           		form.MaximizeBox = false;
			                           		form.FormBorderStyle = FormBorderStyle.FixedSingle;
			                           		form.Controls.Add(titleLabel);
			                           		form.Controls.Add(sitesList_tb);
			                           		form.Controls.Add(continueButton);
			                           		form.Controls.Add(cancelButton);
			                           		form.Name = "BulkSiteSearch";
			                           		form.Text = "Sites Bulk Search";
			                           		form.StartPosition = FormStartPosition.Manual;
			                           		Point loc = PointToScreen(Point.Empty);
			                           		loc.X = loc.X + ((this.Width - form.Width) / 2);
			                           		loc.Y = loc.Y + ((this.Height - form.Height) / 2);
			                           		form.Location = loc;
			                           		sitesList_tb.Focus();
			                           		form.ShowDialog();
			                           	}
			                           });
			LoadingPanel load = new LoadingPanel();
			load.Show(action, this);
			if(string.IsNullOrEmpty(sitesList_tb.Text))
				return;
			
			string[] input = sitesList_tb.Text.Contains(";") ? sitesList_tb.Text.Split(';') : sitesList_tb.Text.Split('\n');
			
			for(int c=0;c < input.Length;c++) {
				input[c] = input[c].Trim();
				while(input[c].StartsWith("0"))
					input[c] = input[c].Substring(1);
			}
			
			siteFinder(input);
		}

		void searchResultsPopulate() {
			if(currentOutage != null)
				foundSites = currentOutage.AffectedSites;
			
			foreach (Site site in foundSites) {
				listView2.Items.Add(new ListViewItem(new[]{ site.Id, site.JVCO_Id, site.Priority, site.Host, site.PostCode }));
				
				markersOverlay.Markers.Add(site.MapMarker);
			}
			
			foreach (ColumnHeader col in listView2.Columns)
				col.Width = -2;
			listView2.ResumeLayout();
			
			label11.Text = label11.Text.Split('(')[0] + '(' + foundSites.Count + ')';
		}

		void bulkSearchForm_buttonClick(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			Form form = (Form)btn.Parent;
			if(btn.Text == "Cancel")
				form.Controls["sitesList_tb"].Text = string.Empty;
			form.Close();
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			Panel panel = new Panel();
			panel.BackColor = Color.Black;
			panel.Size = new Size(100, 213);
			DataGridView dgv = new DataGridView();
			dgv.Size = panel.Size;
			dgv.Columns.Add("Site", "Site");
			dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dgv.RowHeadersVisible =
				dgv.ColumnHeadersVisible = false;
			dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
			dgv.AllowUserToAddRows =
				dgv.AllowUserToResizeRows =
				dgv.AllowUserToResizeColumns = false;
			
			foreach(string site in currentSite.CramerData.OnwardSites) {
				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(dgv);
				row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				row.Cells[0].Value = site;
				dgv.Rows.Add(row);
			}
			
			panel.Controls.AddRange(new Control[] {
			                        	dgv
			                        });
			PopupHelper popup = new PopupHelper(panel);
			popup.Show(this);
		}

		void siteFinder(object sender, KeyPressEventArgs e)
		{
			string enteredSite = ((TextBoxBase)sender).Text;
			if(Convert.ToInt32(e.KeyChar) == 13 && !((TextBoxBase)sender).ReadOnly) {
				Action actionThreaded = new Action(delegate {
				                                   	try { myMap.Overlays.Remove(markersOverlay); } catch (Exception) { }
				                                   	try { markersOverlay.Clear(); } catch (Exception) { }
				                                   	try { myMap.Overlays.Remove(selectedSiteOverlay); } catch (Exception) { }
				                                   	try { selectedSiteOverlay.Clear(); } catch (Exception) { }
				                                   	try { myMap.Overlays.Remove(onwardSitesOverlay); } catch (Exception) { }
				                                   	try { onwardSitesOverlay.Clear(); } catch (Exception) { }
				                                   	
				                                   	currentSite = DB.SitesDB.getSite(enteredSite);
				                                   	
				                                   	if(currentSite.Exists) {
				                                   		string dataToRequest = "INCCellsState";
				                                   		if((DateTime.Now - currentSite.ChangesTimestamp) > new TimeSpan(0, 30, 0))
				                                   			dataToRequest += "CRQ";
				                                   		if(string.IsNullOrEmpty(currentSite.PowerCompany))
				                                   			dataToRequest += "PWR";
				                                   		if(currentSite.CramerData == null)
				                                   			dataToRequest += "Cramer";
				                                   		currentSite.requestOIData(dataToRequest);
				                                   	}
				                                   });
				
				Action actionNonThreaded = new Action(delegate {
				                                      	if(siteDetails_UIMode.Contains("multi"))
				                                      		siteDetails_UIMode = "single";
				                                      	selectedSiteDetailsPopulate();
				                                      	MainMenu.siteFinder_Toggle(currentSite.Exists);
				                                      });
				
				LoadingPanel load = new LoadingPanel();
				load.ShowAsync(actionThreaded, actionNonThreaded, true, this);
			}
		}

		void siteFinder(string[] src)
		{
			Action action = new Action(delegate {
			                           	try { myMap.Overlays.Remove(markersOverlay); } catch (Exception) { }
			                           	try { markersOverlay.Clear(); } catch (Exception) { }
			                           	try { myMap.Overlays.Remove(selectedSiteOverlay); } catch (Exception) { }
			                           	try { selectedSiteOverlay.Clear(); } catch (Exception) { }
			                           	try { myMap.Overlays.Remove(onwardSitesOverlay); } catch (Exception) { }
			                           	try { onwardSitesOverlay.Clear(); } catch (Exception) { }
			                           	
			                           	foundSites = DB.SitesDB.getSites(src.ToList());
			                           	
			                           	if(foundSites.Count > 1) {
			                           		if(!siteDetails_UIMode.Contains("outage"))
			                           			siteDetails_UIMode = "multi";
			                           		searchResultsPopulate();
			                           		listView2.Items[0].Focused = true;
			                           		listView2.Items[0].Selected = true;
			                           		myMap.Overlays.Add(markersOverlay);
			                           		myMap.ZoomAndCenterMarkers(markersOverlay.Id);
			                           	}
			                           	else {
			                           		if(foundSites.Count == 1) {
			                           			siteDetails_UIMode = "single";
			                           			currentSite = foundSites[0];
			                           			selectedSiteDetailsPopulate();
			                           		}
			                           	}
			                           });
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, action,true,this);
		}

		void ListView2ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try { myMap.Overlays.Remove(selectedSiteOverlay); } catch (Exception) { }
			try { selectedSiteOverlay.Clear(); } catch (Exception) { }
			try { myMap.Overlays.Remove(onwardSitesOverlay); } catch (Exception) { }
			try { onwardSitesOverlay.Clear(); } catch (Exception) { }
			
			if(listView2.SelectedItems.Count > 0) {
				currentSite = foundSites[listView2.SelectedItems[0].Index];
				
				selectedSiteDetailsPopulate();
				
				MainMenu.siteFinder_Toggle(currentSite.Exists);
			}
			else
				myMap.ZoomAndCenterMarkers(markersOverlay.Id);
		}

		void LoadDisplayOiDataTable(object sender, EventArgs e) {
			string dataToShow = ((ToolStripMenuItem)sender).Name.Replace("Button", string.Empty);
			var fc = Application.OpenForms.OfType<OiSiteTablesForm>().Where(f => f.OwnerControl == (Control)this && f.Text.EndsWith(dataToShow)).ToList();
			if(fc.Count > 0) {
				fc[0].Close();
				fc[0].Dispose();
			}
			
			if(currentSite.Exists) {
				DataTable dt = new DataTable();
				switch(dataToShow) {
					case "INCs":
						if(currentSite.Incidents == null) {
							currentSite.requestOIData("INC");
							if(currentSite.Incidents != null) {
								if(currentSite.Incidents.Count > 0) {
									MainMenu.INCsButton.Enabled = true;
									MainMenu.INCsButton.ForeColor = Color.DarkGreen;
									MainMenu.INCsButton.Text = "INCs (" + currentSite.Incidents.Count + ")";
								}
								else {
									MainMenu.INCsButton.Enabled = false;
									MainMenu.INCsButton.Text = "No INC history";
								}
							}
							return;
						}
						break;
					case "CRQs":
						if(currentSite.Changes == null) {
							currentSite.requestOIData("CRQ");
							if(currentSite.Changes != null) {
								if(currentSite.Changes.Count > 0) {
									MainMenu.CRQsButton.Enabled = true;
									MainMenu.CRQsButton.ForeColor = Color.DarkGreen;
									MainMenu.CRQsButton.Text = "CRQs (" + currentSite.Changes.Count + ")";
								}
								else {
									MainMenu.CRQsButton.Enabled = false;
									MainMenu.CRQsButton.Text = "No CRQ history";
								}
							}
							return;
						}
						break;
					case "BookIns":
						if(currentSite.Visits == null) {
							currentSite.requestOIData("Bookins");
							if(currentSite.Visits != null) {
								if(currentSite.Visits.Count > 0) {
									MainMenu.BookInsButton.Enabled = true;
									MainMenu.BookInsButton.ForeColor = Color.DarkGreen;
									MainMenu.BookInsButton.Text = "Book Ins List (" + currentSite.Visits.Count + ")";
								}
								else {
									MainMenu.BookInsButton.Enabled = false;
									MainMenu.BookInsButton.Text = "No Book In history";
								}
							}
							return;
						}
						break;
					case "ActiveAlarms":
						if(currentSite.Alarms == null) {
							currentSite.requestOIData("Alarms");
							if(currentSite.Alarms != null) {
								if(currentSite.Alarms.Count > 0) {
									MainMenu.ActiveAlarmsButton.Enabled = true;
									MainMenu.ActiveAlarmsButton.ForeColor = Color.DarkGreen;
									MainMenu.ActiveAlarmsButton.Text = "Active alarms (" + currentSite.Alarms.Count + ")";
								}
								else {
									MainMenu.ActiveAlarmsButton.Enabled = false;
									MainMenu.ActiveAlarmsButton.Text = "No alarms to display";
								}
								return;
							}
						}
						break;
					case "Availability":
						if(currentSite.Availability == null) {
							currentSite.requestOIData("Availability");
							if(currentSite.Availability != null) {
								if(currentSite.Availability.Rows.Count > 0) {
									MainMenu.AvailabilityButton.Enabled = true;
									MainMenu.AvailabilityButton.ForeColor = Color.DarkGreen;
									MainMenu.AvailabilityButton.Text = "Availability chart";
								}
								else {
									MainMenu.AvailabilityButton.Enabled = false;
									MainMenu.AvailabilityButton.Text = "No availability chart to display";
								}
							}
							return;
						}
						break;
				}
				
				OiSiteTablesForm OiTable = null;
				switch(dataToShow) {
					case "INCs":
						OiTable = new OiSiteTablesForm(currentSite.Incidents, currentSite.Id, this);
						break;
					case "CRQs":
						OiTable = new OiSiteTablesForm(currentSite.Changes, currentSite.Id, this);
						break;
					case "BookIns":
						OiTable = new OiSiteTablesForm(currentSite.Visits, currentSite.Id, this);
						break;
					case "ActiveAlarms":
						OiTable = new OiSiteTablesForm(currentSite.Alarms, currentSite.Id, this);
						break;
					case "Availability":
						OiTable = new OiSiteTablesForm(currentSite.Availability, dataToShow, currentSite.Id, this);
						break;
				}
				OiTable.Show();
			}
		}

		void refreshOiData(object sender, EventArgs e) {
			currentSite.requestOIData("INCCRQBookinsAlarmsAvailability");
			MainMenu.siteFinder_Toggle(true);
		}

		void ViewSiteInOiButtonClick(object sender, EventArgs e) {
			
		}

		void LockUnlockCells(object sender, EventArgs e) {
			Action actionNonThreaded = new Action(delegate {
			                                      	if(currentSite.Exists) {
			                                      		LockUnlockCellsForm lucf = new LockUnlockCellsForm(this);
			                                      		lucf.ShowDialog();
			                                      		selectedSiteDetailsPopulate();
			                                      	}
			                                      });
			
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, actionNonThreaded, false, this);
		}

		void OpenLockedCells(object sender, EventArgs e) {
			var fc = Application.OpenForms.OfType<LockUnlockCellsForm>().ToList();
			
			if(fc.Count > 0) {
				if(fc[0].WindowState == FormWindowState.Minimized)
					fc[0].Invoke(new Action(() => { fc[0].WindowState = FormWindowState.Normal; }));;
				fc[0].Invoke(new MethodInvoker(fc[0].Activate));
				return;
			}
			
			var thread = new System.Threading.Thread(() => {
			                                         	LockUnlockCellsForm lucf = new LockUnlockCellsForm();
			                                         	lucf.ShowDialog();
			                                         });
			thread.Name = "Cells Locked";
			thread.SetApartmentState(System.Threading.ApartmentState.STA);
			thread.Start();
		}

		void InitializeToolStripMenuItems() {
			// 
			// bulkSiteSearchMenuItem
			// 
			bulkSiteSearchMenuItem.Name = "bulkSiteSearchMenuItem";
			bulkSiteSearchMenuItem.Text = "Bulk Site Search";
			bulkSiteSearchMenuItem.Click += bulkSiteSearchMenuItemClick;
			// 
			// viewSiteInOiToolStripMenuItem
			// 
			viewSiteInOiToolStripMenuItem.Name = "viewSiteInOiToolStripMenuItem";
			viewSiteInOiToolStripMenuItem.Text = "View on SiteLopedia";
//			viewSiteInOiToolStripMenuItem.Font = new Font("Arial Unicode MS", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			viewSiteInOiToolStripMenuItem.Click += ViewSiteInOiButtonClick;
			// 
			// generateTaskToolStripMenuItem
			// 
			lockUnlockCellsToolStripMenuItem.Name = "lockUnlockCellsToolStripMenuItem";
			lockUnlockCellsToolStripMenuItem.Text = "Lock/Unlock Cells...";
			lockUnlockCellsToolStripMenuItem.Click += LockUnlockCells;
			// 
			// lockedCellsPageToolStripMenuItem
			// 
			lockedCellsPageToolStripMenuItem.Name = "lockedCellsPageToolStripMenuItem";
			lockedCellsPageToolStripMenuItem.Text = "Locked Cells Page...";
			lockedCellsPageToolStripMenuItem.Click += OpenLockedCells;
		}
//
//		void SiteDetailsFormClosing(object sender, FormClosingEventArgs e)
//		{
//			if(parentControl != null) {
//				switch(parentControl.GetType().ToString()) {
//					case "appCore.Templates.UI.TroubleshootControls":
//						TroubleshootControls parent = parentControl as TroubleshootControls;
//						parent.currentSite = currentSite;
//						parent.MainMenu.siteFinder_Toggle(true);
//						break;
//					case "appCore.Templates.UI.FailedCRQControls":
//						FailedCRQControls par = parentControl as FailedCRQControls;
//						par.currentSite = currentSite;
//						par.MainMenu.siteFinder_Toggle(true);
//						break;
//					case "appCore.Templates.UI.UpdateControls":
//						UpdateControls parnt = parentControl as UpdateControls;
//						parnt.currentSite = currentSite;
//						parnt.MainMenu.siteFinder_Toggle(true);
//						break;
//				}
//			}
//		}
	}
}

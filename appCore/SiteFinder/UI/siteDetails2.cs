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
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using appCore.UI;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of siteDetails.
	/// </summary>
	public partial class siteDetails2 : Form
	{
		string _siteDetails_UIMode = "single/readonly";
		/// <summary>
		/// Valid values: "single","single/readonly","multi",multi/readonly","outage"
		/// </summary>
		public string siteDetails_UIMode {
			get {
				return _siteDetails_UIMode;
			}
			set {
				string[] mode = value.Split('/');
				Controls.Add(MainMenu);
				MainMenu.InitializeTroubleshootMenu();
				MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
				MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
				InitializeToolStripMenuItems();
				MainMenu.siteFinder_Toggle(false, false);
				switch(mode[0]) {
					case "single":
						label11.Visible = false;
						listView2.Visible = false;
						checkBox1.Location = new System.Drawing.Point(220, 230);
						checkBox2.Location = new System.Drawing.Point(258, 230);
						checkBox3.Location = new System.Drawing.Point(296, 230);
						listView1.Location = new System.Drawing.Point(5, 247);
						listView1.Size = new Size(556, 398);
						label12.Location = new System.Drawing.Point(5, 227);
						textBox1.ReadOnly = value.Contains("readonly");
						bulkSiteSearchMenuItem.Enabled = !value.Contains("readonly");
						if(!value.Contains("readonly")) {
							MainMenu.MainMenu.DropDownItems.Add(bulkSiteSearchMenuItem);
							MainMenu.MainMenu.DropDownItems.Add("-");
//							MainMenu.MainMenu.DropDownItems.Add(viewSiteInOiToolStripMenuItem);
							MainMenu.MainMenu.DropDownItems.Add(lockUnlockCellsToolStripMenuItem);
//							MainMenu.MainMenu.DropDownItems.Add("-");
//							MainMenu.MainMenu.DropDownItems.Add(sendBCPToolStripMenuItem);
//							MainMenu.MainMenu.DropDownItems.Add("-");
//							MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
						}
						break;
					case "multi":
						label11.Visible = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Location = new System.Drawing.Point(220, 355);
						checkBox2.Location = new System.Drawing.Point(258, 355);
						checkBox3.Location = new System.Drawing.Point(296, 355);
						listView1.Location = new System.Drawing.Point(5, 372);
						listView1.Size = new Size(556, 274);
						label12.Location = new System.Drawing.Point(5, 352);
						bulkSiteSearchMenuItem.Enabled = !value.Contains("readonly");
						textBox1.ReadOnly = !value.Contains("readonly");
						break;
					case "outage":
						listView1.Location = new System.Drawing.Point(5, 372);
						listView1.Size = new Size(556, 274);
						listView1.CheckBoxes = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Location = new System.Drawing.Point(220, 355);
						checkBox2.Location = new System.Drawing.Point(258, 355);
						checkBox3.Location = new System.Drawing.Point(296, 355);
						label11.Visible = true;
						label12.Location = new System.Drawing.Point(5, 352);
						bulkSiteSearchMenuItem.Enabled = false;
						textBox1.ReadOnly = true;
						break;
				}
				_siteDetails_UIMode = value;
			}
		}
//		DataView cellsList = null;
//		DataTable foundSites = Databases.siteDetailsTable.Clone();
//		DataTable foundCells = Databases.cellDetailsTable.Clone();
		GMapControl myMap;
		GMapOverlay markersOverlay = new GMapOverlay("markers");
		GMapOverlay selectedSiteOverlay = new GMapOverlay("selectedSite");
		List<GMapMarker> markersList = new List<GMapMarker>();
		public Site currentSite;
		List<Site> outageSites = new List<Site>(); // for outages site list
		byte listView2_EventCount = 1;
		AMTMenuStrip MainMenu = new AMTMenuStrip(560);
		ToolStripMenuItem viewSiteInOiToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem bulkSiteSearchMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem lockUnlockCellsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToNewTemplateToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem sendBCPToolStripMenuItem = new ToolStripMenuItem();
		
		public siteDetails2(Site site)
		{
			InitializeComponent();
			currentSite = site;
			myMap = drawGMap("myMap",false);
			Controls.Add(myMap);
			Shown += populateSingleForm;
		}
		
		void populateSingleForm(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	try {
			                           		myMap.Overlays.Remove(markersOverlay);
			                           		myMap.Overlays.Remove(selectedSiteOverlay);
			                           	}
			                           	catch (Exception) {
			                           	}
			                           	initializeListviews();
			                           	
			                           	siteDetails_UIMode = "single/readonly";
			                           	
			                           	selectedSiteDetailsPopulate(currentSite);
			                           	selectedSiteOverlay.Markers.Add(currentSite.MapMarker);
//			                           	setSiteMarker(currentSite.MapMarker,false);
			                           	myMap.Overlays.Add(selectedSiteOverlay);
			                           	myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
			                           });
			
			Toolbox.Tools.darkenBackgroundForm(action,true, this);
		}
		
		public siteDetails2(bool outage, List<Site> sitesList)
		{
			InitializeComponent();
			myMap = drawGMap("myMap",true);
			this.Controls.Add(myMap);
			if(outage) {
				siteDetails_UIMode = "outage";
				outageSites = sitesList;
			}
			else
				siteDetails_UIMode = "single";
			this.Shown += populateBulkForm;
		}
		
		public siteDetails2()
		{
			InitializeComponent();
			myMap = drawGMap("myMap",true);
			this.Controls.Add(myMap);
			siteDetails_UIMode = "single";
			lockUnlockCellsToolStripMenuItem.Enabled = false;
			this.Shown += populateBulkForm;
		}
		
		void populateBulkForm(object sender, EventArgs e)
		{
			// TODO: Multi select sites and show only their markers
			// TODO: Show cells for selected sites
			Action action = new Action(delegate {
			                           	initializeListviews();
			                           	if(siteDetails_UIMode.Contains("outage")) {
			                           		this.Name = "Outage Follow-up";
			                           		this.Text = this.Name;
//			                           		siteFinder(sites);
			                           	}
			                           	else
			                           		textBox1.Select();
			                           });
			
			Toolbox.Tools.darkenBackgroundForm(action,true,this);
		}
		
		void initializeListviews() {
			listView1.View = View.Details;
			listView1.Columns.Add("Tech");
			listView1.Columns.Add("Cell Name");
			listView1.Columns.Add("Cell ID");
			listView1.Columns.Add("LAC TAC");
			listView1.Columns.Add("Switch");
			listView1.Columns.Add("OSS ID");
			listView1.Columns.Add("Vendor");
			listView1.Columns.Add("NOC");
			listView1.Columns.Add("Locked").TextAlign = HorizontalAlignment.Center;
			
			listView2.View = View.Details;
			listView2.Columns.Add("Site");
			listView2.Columns.Add("JVCO ID");
			listView2.Columns.Add("Priority");
			listView2.Columns.Add("Site Host");
			listView2.Columns.Add("Post Code");
		}
		
		GMapControl drawGMap(string mapName,bool multi) {
			// TODO: implement weather layer if possible with GMaps
			GMapProvider.TimeoutMs = 20*1000;
			IWebProxy proxy;
			try {
				proxy = Settings.CurrentUser.networkDomain == "internal.vodafone.com" ?
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
			map.Location = new System.Drawing.Point(567, 6);
			map.Size = new Size(380, 639);
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
		
		void selectedSiteDetailsPopulate(Site site) {
			currentSite = site;
			if(currentSite.Exists) {
				textBox1.Text = currentSite.Id;
				textBox2.Text = currentSite.PowerCompany;
				textBox3.Text = currentSite.JVCO;
				textBox4.Text = currentSite.Address.Replace(';',',');
				TextBox4TextChanged(textBox4,null);
				textBox5.Text = currentSite.Area;
				textBox6.Text = currentSite.Region;
				textBox8.Text = currentSite.HostedBy;
				textBox7.Text = currentSite.SharedOperatorSiteID == string.Empty ? textBox1.Text : currentSite.SharedOperatorSiteID;
				textBox9.Text = currentSite.Priority;
				textBox10.Text = currentSite.SharedOperator;
				if(Settings.CurrentUser.userName == "GONCARJ3")
					lockUnlockCellsToolStripMenuItem.Enabled = true;
				
				listboxFilter_Changed(checkBox1, null);
			}
			else {
				foreach(Control ctr in Controls) {
					if(ctr.Name != "textBox1" && !ctr.Name.Contains("label") && !string.IsNullOrEmpty(ctr.Name)) {
						if(ctr.Name == "listView1")
							listView1.Items.Clear();
						else {
							TextBoxBase tb = ctr as TextBoxBase;
							if(tb != null)
								tb.Text = string.Empty;
						}
					}
				}
				textBox4.Text = "Site not found";
				myMap.SetPositionByKeywords("UK");
				myMap.Zoom = 4;
				lockUnlockCellsToolStripMenuItem.Enabled = false;
			}
			pictureBox1.UpdateCells(currentSite.Cells);
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
				foreach (ListViewItem item in listView2.Items)
				{
					if(item.SubItems[0].Text == marker.Tag.ToString())
						markerIndex = item.Index;
					item.Selected = false;
				}
				listView2.ItemSelectionChanged += ListView2ItemSelectionChanged;
				if(markerIndex > -1)
					listView2.Items[markerIndex].Selected = true;
			}
		}
		
		void listboxFilter_Changed(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)sender;
			if(cb.Checked) {
				string ossID;
				
				bool show2G = checkBox1.Checked;
				bool show3G = checkBox2.Checked;
				bool show4G = checkBox3.Checked;
				
				listView1.Items.Clear();
				listView1.SuspendLayout();
				
				foreach (Cell cell in currentSite.Cells) {
					switch(cell.Bearer) {
						case "2G":
							if(!show2G)
								continue;
							break;
						case "3G":
							if(!show3G)
								continue;
							break;
						case "4G":
							if(!show4G)
								continue;
							break;
					}
					
					if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
						ossID = cell.ENodeB_Id;
					else
						ossID = cell.WBTS_BCF;
					
					listView1.Items.Add(new ListViewItem(
						new[]{ cell.Bearer,
							cell.Name,
							cell.Id,
							cell.LacTac,
							cell.BscRnc_Id,
							ossID,
							cell.Vendor.ToString(),
							cell.Noc,
							cell.Locked ? "YES" : "No"
						}));
				}
				
				foreach (ColumnHeader col in listView1.Columns)
					col.Width = -2;
				listView1.View = View.Details;
				listView1.ResumeLayout();
			}
			else {
				bool show2G = false;
				bool show3G = false;
				bool show4G = false;
				show2G |= checkBox1.Checked;
				show3G |= checkBox2.Checked;
				show4G |= checkBox3.Checked;
				
				listView1.SuspendLayout();
				
				foreach(ListViewItem item in listView1.Items) {
					switch(item.SubItems[0].Text) {
						case "2G":
							if(!show2G)
								listView1.Items[item.Index].Remove();
							break;
						case "3G":
							if(!show3G)
								listView1.Items[item.Index].Remove();
							break;
						case "4G":
							if(!show4G)
								listView1.Items[item.Index].Remove();
							break;
					}
				}
				listView1.ResumeLayout();
			}
		}
		
		void TextBox4TextChanged(object sender, EventArgs e)
		{
			button45.Enabled = textBox4.Text != string.Empty;
		}

		void Button45Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox4.Text,label4.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void ListView1KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Control && e.KeyCode != Keys.ControlKey) {
				switch(e.KeyCode) {
					case Keys.C:
						if(listView1.SelectedItems.Count > 0) {
							string copiedCells = string.Empty;
							foreach (ListViewItem cell in listView1.SelectedItems) {
								if(!string.IsNullOrEmpty(copiedCells))
									copiedCells += Environment.NewLine;
								copiedCells += cell.SubItems[1].Text;
							}
							Clipboard.SetText(copiedCells);
						}
						break;
					case Keys.A:
						if(listView1.SelectedItems.Count < listView1.Items.Count) {
							foreach (ListViewItem item in listView1.Items)
							{
								item.Selected = true;
							}
						}
						break;
					case Keys.E:
						if(listView1.SelectedItems.Count > 0) {
							foreach (ListViewItem item in listView1.Items)
							{
								item.Selected = false;
							}
						}
						break;
				}
			}
		}
		
		void bulkSiteSearchMenuItemClick(object sender, EventArgs e)
		{
			RichTextBox sitesList_tb = new RichTextBox();
			Form form = new Form();
			Action action = new Action(delegate {
			                           	using (form) {
			                           		// 
			                           		// titleLabel
			                           		// 
			                           		Label titleLabel = new Label();
			                           		titleLabel.Location = new System.Drawing.Point(3, 5);
			                           		titleLabel.Name = "label";
			                           		titleLabel.AutoSize = true;
			                           		titleLabel.Text = "Please provide list of sites to search";
			                           		titleLabel.TextAlign = ContentAlignment.MiddleCenter;
			                           		// 
			                           		// sitesList_tb
			                           		//
			                           		sitesList_tb.Location = new System.Drawing.Point(3, titleLabel.Top + titleLabel.Height - 2);
			                           		sitesList_tb.Name = "sitesList_tb";
			                           		sitesList_tb.Multiline = true;
			                           		sitesList_tb.Size = new Size(175, 280);
			                           		sitesList_tb.TabIndex = 111;
			                           		// 
			                           		// continueButton
			                           		// 
			                           		Button continueButton = new Button();
			                           		continueButton.Location = new System.Drawing.Point(3, (sitesList_tb.Top + sitesList_tb.Height) + 4);
			                           		continueButton.Name = "continueButton";
			                           		continueButton.Size = new Size(86, 23);
			                           		continueButton.TabIndex = 6;
			                           		continueButton.Text = "Continue";
			                           		continueButton.Click += bulkSearchForm_buttonClick;
			                           		// 
			                           		// cancelButton
			                           		// 
			                           		Button cancelButton = new Button();
			                           		cancelButton.Location = new System.Drawing.Point((continueButton.Left + continueButton.Width) + 3, (sitesList_tb.Top + sitesList_tb.Height) + 4);
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
			                           		System.Drawing.Point loc = PointToScreen(System.Drawing.Point.Empty);
			                           		loc.X = loc.X + ((this.Width - form.Width) / 2);
			                           		loc.Y = loc.Y + ((this.Height - form.Height) / 2);
			                           		form.Location = loc;
			                           		sitesList_tb.Focus();
			                           		form.ShowDialog();
			                           	}
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
			if(string.IsNullOrEmpty(sitesList_tb.Text))
				return;
			
			string[] input = sitesList_tb.Text.Split('\n');
			if(outageSites != null)
				outageSites.Clear();
			foreach(string site in input) {
				currentSite = Finder.getSite(site);
				if(currentSite.Exists)
					outageSites.Add(currentSite);
			}
			siteFinder(outageSites);
		}
		
		void searchResultsPopulate(List<Site> foundSites) {
			outageSites = foundSites;
			foreach (Site site in foundSites) {
				listView2.Items.Add(new ListViewItem(new[]{ site.Id,site.JVCO,site.Priority,site.HostedBy,site.PostCode }));
				
				foreach (ColumnHeader col in listView2.Columns)
					col.Width = -2;
				listView2.ResumeLayout();
				
				markersOverlay.Markers.Add(site.MapMarker);
			}
			
			foreach (ColumnHeader col in listView2.Columns)
				col.Width = -2;
			listView2.ResumeLayout();
			
			label11.Text = label11.Text.Split('(')[0] + '(' + outageSites.Count + ')';
		}
		
		void bulkSearchForm_buttonClick(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			Form form = (Form)btn.Parent;
			if(btn.Text == "Cancel")
				form.Controls["sitesList_tb"].Text = string.Empty;
			form.Close();
		}
		
		void siteFinder(object sender, KeyPressEventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			if(Convert.ToInt32(e.KeyChar) == 13 && !tb.ReadOnly) {
				Action actionThreaded = new Action(delegate {
				                                   	try {
				                                   		myMap.Overlays.Remove(markersOverlay);
				                                   		myMap.Overlays.Remove(selectedSiteOverlay);
				                                   		markersOverlay.Clear();
				                                   		selectedSiteOverlay.Clear();
				                                   	}
				                                   	catch (Exception) {
				                                   	}
				                                   	
				                                   	currentSite = Finder.getSite(tb.Text);
				                                   	currentSite.UpdateLockedCells(false);
				                                   	
				                                   	if(currentSite.Exists)
				                                   		currentSite.requestOIData("INCCRQPWR");
				                                   });
				
				Action actionNonThreaded = new Action(delegate {
				                                      	if(currentSite.Exists) {
				                                      		selectedSiteDetailsPopulate(currentSite);
				                                      		selectedSiteOverlay.Markers.Add(currentSite.MapMarker);
//				                           					setSiteMarker(site.MapMarker,false);
				                                      		myMap.Overlays.Add(selectedSiteOverlay);
				                                      		myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
				                                      	}
				                                      	else {
				                                      		selectedSiteDetailsPopulate(currentSite);
				                                      		myMap.SetPositionByKeywords("UK");
				                                      		myMap.Zoom = 6;
				                                      	}
				                                      	MainMenu.siteFinder_Toggle(currentSite.Exists);
				                                      });
				
				LoadingPanel load = new LoadingPanel();
				load.Show(actionThreaded, actionNonThreaded, true, this);
			}
		}
		
		void siteFinder(List<Site> sitesList)
		{
			Action action = new Action(delegate {
			                           	try {
			                           		myMap.Overlays.Remove(markersOverlay);
			                           		myMap.Overlays.Remove(selectedSiteOverlay);
			                           		markersOverlay.Clear();
			                           		selectedSiteOverlay.Clear();
			                           	}
			                           	catch (Exception) {
			                           	}
			                           	
			                           	if(sitesList.Count > 1) {
			                           		if(!siteDetails_UIMode.Contains("outage"))
			                           			siteDetails_UIMode = "multi";
			                           		searchResultsPopulate(sitesList);
			                           		listView2.Items[0].Focused = true;
			                           		listView2.Items[0].Selected = true;
			                           		myMap.Overlays.Add(markersOverlay);
			                           		myMap.ZoomAndCenterMarkers(markersOverlay.Id);
			                           	}
			                           	else {
			                           		if(sitesList.Count == 1) {
			                           			siteDetails_UIMode = "single";
			                           			selectedSiteDetailsPopulate(sitesList[0]);
			                           			selectedSiteOverlay.Markers.Add(sitesList[0].MapMarker);
			                           			myMap.Overlays.Add(selectedSiteOverlay);
			                           			myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
			                           		}
			                           	}
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,true,this);
		}
		
		void ListView2ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(listView2_EventCount == 0)
				listView2_EventCount++;
			else {
				if(listView2.SelectedItems.Count > 0) {
					Site site = outageSites[listView2.SelectedItems[0].Index];
					
//					if(!Toolbox.Tools.IsAllDigits(site))
//						site = "00000";
//
//					DataView dv = new DataView(foundSites);
//					dv.RowFilter = "SITE = '" + site + "'"; // query example = "id = 10"
//					DataRowView dr = null;
//					if(dv.Count == 1)
//						dr = dv[0];
//
//					dv = new DataView(foundCells);
//					dv.RowFilter = "SITE = '" + site + "'";
//					DataTable dt = null;
//					if (dv.Count > 0)
//					{
//						dt = dv.ToTable();
//						//clone the source table
//						DataTable filtered = dt.Clone();
//
//						//fill the clone with the filtered rows
//						foreach (DataRowView drv in dt.DefaultView)
//						{
//							filtered.Rows.Add(drv.Row.ItemArray);
//						}
//						dt = filtered;
//					}
					
					selectedSiteDetailsPopulate(site);
					if(listView2_EventCount == 1) {
						foreach(GMapMarker marker in markersOverlay.Markers) {
							if(marker.Tag.ToString() == textBox1.Text) {
								myMap.Position = marker.Position;
								myMap.Zoom = 14;
								break;
							}
						}
					}
				}
				else {
					myMap.ZoomAndCenterMarkers(markersOverlay.Id);
				}
				if(listView2_EventCount == 1)
					listView2_EventCount--;
			}
		}
		
		void LoadDisplayOiDataTable(object sender, EventArgs e) {
			ToolStripMenuItem tsim = sender as ToolStripMenuItem;
//			if(e.Button == MouseButtons.Left) {
			if(currentSite.Exists) {
				DataTable dt = new DataTable();
				string dataToShow = string.Empty;
				switch (tsim.Name) {
					case "INCsButton":
						if(currentSite.INCs == null) {
							currentSite.requestOIData("INC");
							if(currentSite.INCs != null) {
								if(currentSite.INCs.Rows.Count > 0) {
									MainMenu.INCsButton.Enabled = true;
									MainMenu.INCsButton.ForeColor = Color.DarkGreen;
									MainMenu.INCsButton.Text = "INCs (" + currentSite.INCs.Rows.Count + ")";
								}
								else {
									MainMenu.INCsButton.Enabled = false;
									MainMenu.INCsButton.Text = "No INC history";
								}
							}
							return;
						}
						dataToShow = "INCs";
						dt = currentSite.INCs;
						break;
					case "CRQsButton":
						if(currentSite.CRQs == null) {
							currentSite.requestOIData("CRQ");
							if(currentSite.CRQs != null) {
								if(currentSite.CRQs.Rows.Count > 0) {
									MainMenu.CRQsButton.Enabled = true;
									MainMenu.CRQsButton.ForeColor = Color.DarkGreen;
									MainMenu.CRQsButton.Text = "CRQs (" + currentSite.CRQs.Rows.Count + ")";
								}
								else {
									MainMenu.CRQsButton.Enabled = false;
									MainMenu.CRQsButton.Text = "No CRQ history";
								}
							}
							return;
						}
						dataToShow = "CRQs";
						dt = currentSite.CRQs;
						break;
					case "BookInsButton":
						if(currentSite.BookIns == null) {
							currentSite.requestOIData("Bookins");
							if(currentSite.BookIns != null) {
								if(currentSite.BookIns.Rows.Count > 0) {
									MainMenu.BookInsButton.Enabled = true;
									MainMenu.BookInsButton.ForeColor = Color.DarkGreen;
									MainMenu.BookInsButton.Text = "Book Ins List (" + currentSite.BookIns.Rows.Count + ")";
								}
								else {
									MainMenu.BookInsButton.Enabled = false;
									MainMenu.BookInsButton.Text = "No Book In history";
								}
							}
							return;
						}
						dataToShow = "BookIns";
						dt = currentSite.BookIns;
						break;
					case "ActiveAlarmsButton":
						if(currentSite.ActiveAlarms == null) {
							currentSite.requestOIData("Alarms");
							if(currentSite.ActiveAlarms != null) {
								if(currentSite.ActiveAlarms.Rows.Count > 0) {
									MainMenu.ActiveAlarmsButton.Enabled = true;
									MainMenu.ActiveAlarmsButton.ForeColor = Color.DarkGreen;
									MainMenu.ActiveAlarmsButton.Text = "Active alarms (" + currentSite.ActiveAlarms.Rows.Count + ")";
								}
								else {
									MainMenu.ActiveAlarmsButton.Enabled = false;
									MainMenu.ActiveAlarmsButton.Text = "No alarms to display";
								}
							}
							return;
						}
						dataToShow = "ActiveAlarms";
						dt = currentSite.ActiveAlarms;
						break;
				}
				
				var fc = Application.OpenForms.OfType<OiSiteTablesForm>();
				Form openForm = null;
				
				foreach (Form frm in fc)
				{
					if(frm.Name.Contains(dataToShow)) {
						openForm = frm;
						break;
					}
				}
				if(openForm != null)
					openForm.Close();
				OiSiteTablesForm OiTable = new OiSiteTablesForm(dt, dataToShow, currentSite.Id);
				OiTable.Show();
			}
		}
		
		void refreshOiData(object sender, EventArgs e) {
			currentSite.requestOIData("INCCRQBookinsAlarms");
			MainMenu.siteFinder_Toggle(true);
		}
		
		void ViewSiteInOiButtonClick(object sender, EventArgs e) {
			
		}
		
		void LockUnlockCells(object sender, EventArgs e) {
			Action actionNonThreaded = new Action(delegate {
			                                      	if(Settings.CurrentUser.userName == "GONCARJ3") {
			                                      		if(currentSite.Exists) {
			                                      			LockUnlockCellsForm lucf = new LockUnlockCellsForm(currentSite);
			                                      			lucf.ShowDialog();
			                                      		}
			                                      	}
			                                      });
			
			LoadingPanel load = new LoadingPanel();
			load.Show(null, actionNonThreaded, false, this);
			selectedSiteDetailsPopulate(currentSite);
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
			// clearToolStripMenuItem
			// 
			clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			clearToolStripMenuItem.Text = "Clear";
//			clearToolStripMenuItem.Click += ClearCurrentSite;
			// 
			// generateTaskToolStripMenuItem
			// 
			lockUnlockCellsToolStripMenuItem.Name = "lockUnlockCellsToolStripMenuItem";
			lockUnlockCellsToolStripMenuItem.Text = "Lock/Unlock Cells...";
			lockUnlockCellsToolStripMenuItem.Click += LockUnlockCells;
			// 
			// copyToNewTemplateToolStripMenuItem
			// 
			copyToNewTemplateToolStripMenuItem.Name = "copyToNewTemplateToolStripMenuItem";
			copyToNewTemplateToolStripMenuItem.Text = "Copy to new Troubleshoot template";
//			copyToNewTemplateToolStripMenuItem.Click += LoadTemplateFromLog;
			// 
			// sendBCPToolStripMenuItem
			// 
			sendBCPToolStripMenuItem.Name = "sendBCPToolStripMenuItem";
			sendBCPToolStripMenuItem.Text = "Send BCP Form";
//			sendBCPToolStripMenuItem.Click += SendBCPForm;
		}
	}
}

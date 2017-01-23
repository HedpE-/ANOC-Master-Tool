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
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using appCore.Toolbox;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of siteDetails.
	/// </summary>
	public partial class siteDetails : Form
	{
		string _siteDetails_UIMode = "single/readonly";
		List<Site> foundSites;
		GMapControl myMap;
		GMapOverlay markersOverlay = new GMapOverlay("markers");
		GMapOverlay selectedSiteOverlay = new GMapOverlay("selectedSite");
		List<GMapMarker> markersList = new List<GMapMarker>();
		string[] sites; // for outages site list
		byte listView2_EventCount = 1;
		Site currentSite;
		
		public siteDetails(bool outage, string[] outageSites)
		{
			InitializeComponent();
			myMap = drawGMap("myMap",true);
			this.Controls.Add(myMap);
			if(outage) {
				siteDetails_UIMode = "outage";
				sites = outageSites;
			}
			else
				siteDetails_UIMode = "single";
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
			                           		siteFinder(sites);
			                           	}
			                           });
			
			Tools.darkenBackgroundForm(action,true,this);
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
			
			listView2.View = View.Details;
			listView2.Columns.Add("Site");
			listView2.Columns.Add("JVCO ID");
			listView2.Columns.Add("Priority");
			listView2.Columns.Add("Site Host");
			listView2.Columns.Add("Post Code");
		}
		
		GMapControl drawGMap(string mapName, bool multi) {
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
			
			GMapProvider.WebProxy = proxy;
			GMapProvider.Credential = CredentialCache.DefaultNetworkCredentials;
			
			GMapControl map = new GMapControl();
			map.Name = mapName;
			map.Location = new Point(567, 6);
			map.Size = new Size(380, 614);
			map.MapProvider = GoogleHybridMapProvider.Instance;
			
			map.Manager.Mode = AccessMode.ServerOnly; //get tiles from server only
			map.CanDragMap = true;
			map.DragButton = MouseButtons.Left;
			map.ShowCenter = false;
			map.MouseWheelZoomEnabled = true;
			map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
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
				if(siteDetails_UIMode != "outage")
					currentSite.requestOIData("PWR");
				if(currentSite.Cells.Count > 0) {
					textBox1.Text = currentSite.Id;
					textBox2.Text = currentSite.PowerCompany;
					textBox3.Text = currentSite.JVCO_Id;
					textBox4.Text = currentSite.Address.Replace(';',',');
					TextBox4TextChanged(textBox4,null);
					textBox5.Text = currentSite.Area;
					textBox6.Text = currentSite.Region;
					textBox8.Text = currentSite.HostedBy;
					textBox7.Text = currentSite.SharedOperatorSiteID == string.Empty ? textBox1.Text : currentSite.SharedOperatorSiteID;
					textBox9.Text = currentSite.Priority;
					textBox10.Text = currentSite.SharedOperator;
					
					listboxFilter_Changed(checkBox1, null);
				}
				else {
					textBox4.Text = "Site not found";
					foreach(Control ctr in Controls) {
						if(ctr.Name.StartsWith("label_"))
							ctr.Text = "0";
					}
				}
				
				// Fill Cell Count table
				
				label_TotalCells.Text = currentSite.Cells.Count.ToString();
				
//				cellsList.RowFilter = "BEARER = '2G'";
				label_Total_2GCells.Text = currentSite.Cells.Filter(Cell.Filters.All_2G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '2G' AND (CELL_NAME NOT LIKE 'T*' AND CELL_NAME NOT LIKE '*W' AND CELL_NAME NOT LIKE '*X' AND CELL_NAME NOT LIKE '*Y')";
				label_VF_2GCells.Text = currentSite.Cells.Filter(Cell.Filters.VF_2G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '2G' AND (CELL_NAME LIKE 'T*' OR CELL_NAME LIKE '*W' OR CELL_NAME LIKE '*X' OR CELL_NAME LIKE '*Y')";
				label_TF_2GCells.Text = currentSite.Cells.Filter(Cell.Filters.TF_2G).Count.ToString();
				
//				cells.RowFilter = "BEARER = '3G'";
				label_Total_3GCells.Text = currentSite.Cells.Filter(Cell.Filters.All_3G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '3G' AND CELL_NAME NOT LIKE 'T*'";
				label_VF_3GCells.Text = currentSite.Cells.Filter(Cell.Filters.VF_3G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '3G' AND CELL_NAME LIKE 'T*'";
				label_TF_3GCells.Text = currentSite.Cells.Filter(Cell.Filters.TF_3G).Count.ToString();
				
//				cells.RowFilter = "BEARER = '4G'";
				label_Total_4GCells.Text = currentSite.Cells.Filter(Cell.Filters.All_4G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '4G' AND CELL_NAME NOT LIKE 'T*'";
				label_VF_4GCells.Text = currentSite.Cells.Filter(Cell.Filters.VF_4G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '4G' AND CELL_NAME LIKE 'T*'";
				label_TF_4GCells.Text = currentSite.Cells.Filter(Cell.Filters.TF_4G).Count.ToString();
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
		
		void listboxFilter_Changed(object sender, EventArgs e) {
			CheckBox cb = (CheckBox)sender;
			if(cb.Checked) {
				bool show2G = false;
				bool show3G = false;
				bool show4G = false;
				show2G |= checkBox1.Checked;
				show3G |= checkBox2.Checked;
				show4G |= checkBox3.Checked;
				
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
					
					string ossID;
					if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
						ossID = cell.ENodeB_Id;
					else
						ossID = cell.WBTS_BCF;
					
					listView1.Items.Add(new ListViewItem(new[]{cell.Bearer, cell.Name, cell.Id, cell.LacTac, cell.BscRnc_Id, ossID, cell.Vendor.ToString(), cell.Noc}));
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
			                           	appCore.UI.AMTLargeTextForm enlarge = new appCore.UI.AMTLargeTextForm(textBox4.Text,label4.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           });
			Tools.darkenBackgroundForm(action,false,this);
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
		
		void searchResultsPopulate() {
			foreach (Site site in foundSites) {
				listView2.Items.Add(new ListViewItem(new[]{ site.Id, site.JVCO_Id, site.Priority, site.HostedBy, site.PostCode }));
				
				markersOverlay.Markers.Add(site.MapMarker);
			}
			
			foreach (ColumnHeader col in listView2.Columns)
				col.Width = -2;
			listView2.ResumeLayout();
			
			label11.Text = label11.Text.Split('(')[0] + '(' + foundSites.Count + ')';
		}
		
		void siteFinder(string[] src)
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
			                           	foundSites = Finder.getSites(src.ToList());
			                           	
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
			                           			selectedSiteDetailsPopulate(foundSites[0]);
			                           			selectedSiteOverlay.Markers.Add(foundSites[0].MapMarker);
			                           			myMap.Overlays.Add(selectedSiteOverlay);
			                           			myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
			                           		}
			                           	}
			                           });
			Tools.darkenBackgroundForm(action,true,this);
		}
		
		void ListView2ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(listView2_EventCount == 0)
				listView2_EventCount++;
			else {
				if(listView2.SelectedItems.Count > 0) {
					Site site = foundSites.Find(s => s.Id == listView2.SelectedItems[0].SubItems[0].Text);
					
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
		
		/// <summary>
		/// Valid values: "single","single/readonly","multi",multi/readonly","outage"
		/// </summary>
		public string siteDetails_UIMode {
			get {
				return _siteDetails_UIMode;
			}
			set {
				string[] mode = value.Split('/');
				switch(mode[0]) {
					case "single":
						label11.Visible = false;
						listView2.Visible = false;
						checkBox1.Location = new Point(220, 205);
						checkBox2.Location = new Point(258, 205);
						checkBox3.Location = new Point(296, 205);
						listView1.Location = new Point(5, 222);
						listView1.Size = new Size(556, 398);
						label12.Location = new Point(5, 202);
						textBox1.ReadOnly = value.Contains("readonly");
						button1.Visible = !value.Contains("readonly");
						label13.Visible = !value.Contains("readonly");
						break;
					case "multi":
						label11.Visible = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Location = new Point(220, 330);
						checkBox2.Location = new Point(258, 330);
						checkBox3.Location = new Point(296, 330);
						listView1.Location = new Point(5, 347);
						listView1.Size = new Size(556, 274);
						label12.Location = new Point(5, 327);
						button1.Visible = !value.Contains("readonly");
						label13.Visible = !value.Contains("readonly");
						textBox1.ReadOnly = !value.Contains("readonly");
						break;
					case "outage":
						listView1.Location = new Point(5, 347);
						listView1.Size = new Size(556, 274);
						listView1.CheckBoxes = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Location = new Point(220, 330);
						checkBox2.Location = new Point(258, 330);
						checkBox3.Location = new Point(296, 330);
						label11.Visible = true;
						label12.Location = new Point(5, 327);
						label13.Visible = false;
						button1.Visible = false;
						textBox1.ReadOnly = true;
						break;
				}
				_siteDetails_UIMode = value;
			}
		}
	}
}

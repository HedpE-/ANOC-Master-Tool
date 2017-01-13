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
using appCore.DB;
using appCore.Toolbox;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of siteDetails.
	/// </summary>
	public partial class siteDetails : Form
	{
		string _siteDetails_UIMode = "single/readonly";
		DataView cellsList = null;
		DataTable foundSites = Databases.siteDetailsTable.Clone();
		DataTable foundCells = Databases.cellDetailsTable.Clone();
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
		
		GMapControl drawGMap(string mapName,bool multi) {
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
				currentSite.requestOIData("INCCRQPWR");
				if(currentSite.Cells.Count > 0) {
					
//				cellsList = cells;
//				cellsList.Sort = "BEARER Asc, CELL_NAME Asc";
					
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
					
//				cellsList.RowFilter = string.Empty;
					
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
				
				cellsList.RowFilter = string.Empty;
			}
		}
		
		void selectedSiteDetailsPopulate(DataRowView site, DataView cells) {
			if(cells.Count > 0) {
				
				cellsList = cells;
				cellsList.Sort = "BEARER Asc, CELL_NAME Asc";
				
				textBox1.Text = site[site.Row.Table.Columns.IndexOf("SITE")].ToString();
				
				if(site.Row.Table.Columns.IndexOf("POWER_COMPANY") > -1 && site.Row.Table.Columns.IndexOf("POWER_CONTACT") > -1)
					textBox2.Text = site[site.Row.Table.Columns.IndexOf("POWER_COMPANY")].ToString() + " +" + site[site.Row.Table.Columns.IndexOf("POWER_CONTACT")].ToString();
				textBox3.Text = site[site.Row.Table.Columns.IndexOf("JVCO_ID")].ToString();
				textBox4.Text = site[site.Row.Table.Columns.IndexOf("ADDRESS")].ToString().Replace(';',',');
				TextBox4TextChanged(textBox4,null);
				textBox5.Text = site[site.Row.Table.Columns.IndexOf("AREA")].ToString();
				textBox6.Text = site[site.Row.Table.Columns.IndexOf("VF_REGION")].ToString();
				textBox8.Text = site[site.Row.Table.Columns.IndexOf("HOST")].ToString();
				textBox7.Text = site[site.Row.Table.Columns.IndexOf("SITE_SHARE_SITE_NO")].ToString() == string.Empty ? textBox1.Text : site[site.Row.Table.Columns.IndexOf("SITE_SHARE_SITE_NO")].ToString();
				textBox9.Text = site[site.Row.Table.Columns.IndexOf("PRIORITY")].ToString();
				textBox10.Text = site[site.Row.Table.Columns.IndexOf("SITE_SHARE_OPERATOR")].ToString();
				
				// Fill Cell Count table
				
				label_TotalCells.Text = cellsList.Count.ToString();
				
				cellsList.RowFilter = "BEARER = '2G'";
				label_Total_2GCells.Text = cellsList.Count.ToString();
				cellsList.RowFilter = "BEARER = '2G' AND (CELL_NAME NOT LIKE 'T*' AND CELL_NAME NOT LIKE '*W' AND CELL_NAME NOT LIKE '*X' AND CELL_NAME NOT LIKE '*Y')";
				label_VF_2GCells.Text = cellsList.Count.ToString();
				cellsList.RowFilter = "BEARER = '2G' AND (CELL_NAME LIKE 'T*' OR CELL_NAME LIKE '*W' OR CELL_NAME LIKE '*X' OR CELL_NAME LIKE '*Y')";
				label_TF_2GCells.Text = cellsList.Count.ToString();
				
				cells.RowFilter = "BEARER = '3G'";
				label_Total_3GCells.Text = cellsList.Count.ToString();
				cellsList.RowFilter = "BEARER = '3G' AND CELL_NAME NOT LIKE 'T*'";
				label_VF_3GCells.Text = cellsList.Count.ToString();
				cellsList.RowFilter = "BEARER = '3G' AND CELL_NAME LIKE 'T*'";
				label_TF_3GCells.Text = cellsList.Count.ToString();
				
				cells.RowFilter = "BEARER = '4G'";
				label_Total_4GCells.Text = cellsList.Count.ToString();
				cellsList.RowFilter = "BEARER = '4G' AND CELL_NAME NOT LIKE 'T*'";
				label_VF_4GCells.Text = cellsList.Count.ToString();
				cellsList.RowFilter = "BEARER = '4G' AND CELL_NAME LIKE 'T*'";
				label_TF_4GCells.Text = cellsList.Count.ToString();
				
				cellsList.RowFilter = string.Empty;
				
//				TODO: databind listview
//					GridView1.DataSource = DV;
//			 		GridView1.DataBind();
				
				listboxFilter_Changed(checkBox1, null);
			}
			else {
				textBox4.Text = "Site not found";
				foreach(Control ctr in Controls) {
					if(ctr.Name.StartsWith("label_"))
						ctr.Text = "0";
				}
//				label_TotalCells.Text = "0";
//
//				label_Total_2GCells.Text = "0";
//				label_VF_2GCells.Text = "0";
//				label_TF_2GCells.Text = "0";
//
//				label_Total_3GCells.Text = "0";
//				label_VF_3GCells.Text = "0";
//				label_TF_3GCells.Text = "0";
//
//				label_Total_4GCells.Text = "0";
//				label_VF_4GCells.Text = "0";
//				label_TF_4GCells.Text = "0";
			}
		}
		
		void setSiteMarker(DataRowView site, bool multi) {
			double easting = Convert.ToDouble(site[site.Row.Table.Columns.IndexOf("EASTING")].ToString());
			double northing = Convert.ToDouble(site[site.Row.Table.Columns.IndexOf("NORTHING")].ToString());
			LLPoint llp =  coordConvert.toLat_Long(new GMap.NET.CoordPoint { Easting = easting, Northing = northing }, "OSGB36");
			
			GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(llp.Latitude, llp.Longitude), GMarkerGoogleType.red);
			marker.Tag = site[site.Row.Table.Columns.IndexOf("SITE")].ToString();
			marker.ToolTip = new GMapBaloonToolTip(marker);
			marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
			marker.ToolTip.Fill = new SolidBrush(Color.FromArgb(180, Color.Black));
			marker.ToolTip.Font = new Font("Courier New", 9, FontStyle.Bold);
			marker.ToolTip.Foreground = new SolidBrush(Color.White);
			marker.ToolTip.Stroke = new Pen(Color.Red);
			marker.ToolTip.Offset.X -= 15;
			marker.ToolTipText = site[site.Row.Table.Columns.IndexOf("SITE")].ToString();
			if(multi)
				markersOverlay.Markers.Add(marker);
			else
				selectedSiteOverlay.Markers.Add(marker);
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
				string bearer;
				string cellName;
				string cellID;
				string lactac;
				string Switch;
				string ossID;
				string vendor;
				string noc;
				
				bool show2G = false;
				bool show3G = false;
				bool show4G = false;
				show2G |= checkBox1.Checked;
				show3G |= checkBox2.Checked;
				show4G |= checkBox3.Checked;
				
				cellsList.RowFilter = string.Empty;
				
				listView1.Items.Clear();
				listView1.SuspendLayout();
				
				foreach (DataRowView dr in cellsList) {
					switch(dr[dr.Row.Table.Columns.IndexOf("BEARER")].ToString()) {
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
					
					bearer = dr[dr.Row.Table.Columns.IndexOf("BEARER")].ToString();
					cellName = dr[dr.Row.Table.Columns.IndexOf("CELL_NAME")].ToString();
					cellID = dr[dr.Row.Table.Columns.IndexOf("CELL_ID")].ToString();
					lactac = dr[dr.Row.Table.Columns.IndexOf("LAC_TAC")].ToString();
					Switch = dr[dr.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString();
					vendor = dr[dr.Row.Table.Columns.IndexOf("VENDOR")].ToString().ToUpper();
					noc = dr[dr.Row.Table.Columns.IndexOf("NOC")].ToString();
					
					if(vendor == "NSN" && bearer == "4G")
						ossID = dr[dr.Row.Table.Columns.IndexOf("ENODEB_ID")].ToString();
					else
						ossID = dr[dr.Row.Table.Columns.IndexOf("WBTS_BCF")].ToString();
					
					listView1.Items.Add(new ListViewItem(new[]{bearer, cellName,cellID,lactac, Switch,ossID, vendor, noc}));
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
		
		void Button1Click(object sender, EventArgs e)
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
			                           		form.Icon = appCore.UI.Resources.MB_0001_vodafone3;
			                           		form.MaximizeBox = false;
			                           		form.FormBorderStyle = FormBorderStyle.FixedSingle;
			                           		form.Controls.Add(titleLabel);
			                           		form.Controls.Add(sitesList_tb);
			                           		form.Controls.Add(continueButton);
			                           		form.Controls.Add(cancelButton);
			                           		form.Name = "BulkSiteSearch";
			                           		form.Text = "Sites Bulk Search";
			                           		form.StartPosition = FormStartPosition.Manual;
			                           		System.Drawing.Point loc = this.PointToScreen(System.Drawing.Point.Empty);
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
			
			siteFinder(sitesList_tb.Text.Split('\n'));
		}
		
		void searchResultsPopulate(DataTable foundSitesTable) {
			foreach (DataRow site in foundSitesTable.Rows) {
				string siteID = site[site.Table.Columns.IndexOf("SITE")].ToString();
				string JVCO = site[site.Table.Columns.IndexOf("JVCO_ID")].ToString();
				string priority = site[site.Table.Columns.IndexOf("PRIORITY")].ToString();
				string host = site[site.Table.Columns.IndexOf("HOST")].ToString();
				string postCode = site[site.Table.Columns.IndexOf("ADDRESS")].ToString();
				postCode = postCode.Substring(postCode.LastIndexOf(';') + 1).Trim();
				listView2.Items.Add(new ListViewItem(new[]{ siteID,JVCO,priority,host,postCode }));
				
				foreach (ColumnHeader col in listView2.Columns)
					col.Width = -2;
				listView2.ResumeLayout();
				
				setSiteMarker(foundSitesTable.DefaultView[foundSitesTable.Rows.IndexOf(site)],true);
			}
			
			foreach (ColumnHeader col in listView2.Columns)
				col.Width = -2;
			listView2.ResumeLayout();
			
			label11.Text = label11.Text.Split('(')[0] + '(' + foundSitesTable.Rows.Count + ')';
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
				Action action = new Action(delegate {
				                           	try {
				                           		myMap.Overlays.Remove(markersOverlay);
				                           		myMap.Overlays.Remove(selectedSiteOverlay);
				                           		markersOverlay.Clear();
				                           		selectedSiteOverlay.Clear();
				                           	}
				                           	catch (Exception) {
				                           	}
				                           	DataRowView site = null;
				                           	DataView cells = null;
				                           	
				                           	if(!string.IsNullOrEmpty(tb.Text)) {
				                           		site = MainForm.findSite(tb.Text);
				                           		cells = MainForm.findCells(tb.Text);
				                           	}
				                           	else
				                           		return;
				                           	
				                           	selectedSiteDetailsPopulate(site,cells);
				                           	
				                           	if(textBox4.Text != "Site not found") {
				                           		setSiteMarker(site,false);
				                           		myMap.Overlays.Add(selectedSiteOverlay);
				                           		myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
				                           	}
				                           	else {
				                           		myMap.SetPositionByKeywords("UK");
				                           		myMap.Zoom = 6;
				                           	}
				                           });
				Tools.darkenBackgroundForm(action,true,this);
			}
		}
		
		void siteFinder(string[] src)
		{
			Action action = new Action(delegate {
			                           	foundSites.Clear();
			                           	foundCells.Clear();
			                           	try {
			                           		myMap.Overlays.Remove(markersOverlay);
			                           		myMap.Overlays.Remove(selectedSiteOverlay);
			                           		markersOverlay.Clear();
			                           		selectedSiteOverlay.Clear();
			                           	}
			                           	catch (Exception) {
			                           	}
			                           	System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
			                           	st.Start();
			                           	foreach (string site in src) {
			                           		if(!string.IsNullOrEmpty(site)) {
			                           			DataRowView drv = MainForm.findSite(site);
			                           			DataRow tempSite = null;
			                           			if(drv != null)
			                           				tempSite = drv.Row;
			                           			if(tempSite != null) {
			                           				DataTable tempCells = MainForm.findCells(site).ToTable();
			                           				foundSites.ImportRow(tempSite);
			                           				foundCells.Merge(tempCells);
			                           			}
			                           		}
			                           		else
			                           			break;
			                           	}
			                           	st.Stop();
			                           	var t = st.Elapsed;
			                           	st.Reset();
			                           	st.Start();
			                           	List<Site> sites2 = Finder.getSites(src.ToList());
			                           	st.Stop();
			                           	var t2 = st.Elapsed;
			                           	
			                           	if(foundSites.Rows.Count > 1) {
			                           		if(!siteDetails_UIMode.Contains("outage"))
			                           			siteDetails_UIMode = "multi";
			                           		searchResultsPopulate(foundSites);
			                           		listView2.Items[0].Focused = true;
			                           		listView2.Items[0].Selected = true;
			                           		myMap.Overlays.Add(markersOverlay);
			                           		myMap.ZoomAndCenterMarkers(markersOverlay.Id);
			                           	}
			                           	else {
			                           		if(foundSites.Rows.Count == 1) {
			                           			siteDetails_UIMode = "single";
			                           			DataRowView site = foundSites.DefaultView[0];
			                           			selectedSiteDetailsPopulate(site,foundCells.DefaultView);
			                           			setSiteMarker(site,false);
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
					string site = listView2.SelectedItems[0].SubItems[0].Text;
					
					if(!site.IsAllDigits())
						site = "00000";
					
					DataView dv = new DataView(foundSites);
					dv.RowFilter = "SITE = '" + site + "'"; // query example = "id = 10"
					DataRowView dr = null;
					if(dv.Count == 1)
						dr = dv[0];
					
					dv = new DataView(foundCells);
					dv.RowFilter = "SITE = '" + site + "'";
					DataTable dt = null;
					if (dv.Count > 0)
					{
						dt = dv.ToTable();
						//clone the source table
						DataTable filtered = dt.Clone();

						//fill the clone with the filtered rows
						foreach (DataRowView drv in dt.DefaultView)
						{
							filtered.Rows.Add(drv.Row.ItemArray);
						}
						dt = filtered;
					}
					
					selectedSiteDetailsPopulate(dr,new DataView(dt));
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
						checkBox1.Location = new System.Drawing.Point(220, 205);
						checkBox2.Location = new System.Drawing.Point(258, 205);
						checkBox3.Location = new System.Drawing.Point(296, 205);
						listView1.Location = new System.Drawing.Point(5, 222);
						listView1.Size = new Size(556, 398);
						label12.Location = new System.Drawing.Point(5, 202);
						textBox1.ReadOnly = value.Contains("readonly");
						button1.Visible = !value.Contains("readonly");
						label13.Visible = !value.Contains("readonly");
						break;
					case "multi":
						label11.Visible = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Location = new System.Drawing.Point(220, 330);
						checkBox2.Location = new System.Drawing.Point(258, 330);
						checkBox3.Location = new System.Drawing.Point(296, 330);
						listView1.Location = new System.Drawing.Point(5, 347);
						listView1.Size = new Size(556, 274);
						label12.Location = new System.Drawing.Point(5, 327);
						button1.Visible = !value.Contains("readonly");
						label13.Visible = !value.Contains("readonly");
						textBox1.ReadOnly = !value.Contains("readonly");
						break;
					case "outage":
						listView1.Location = new System.Drawing.Point(5, 347);
						listView1.Size = new Size(556, 274);
						listView1.CheckBoxes = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Location = new System.Drawing.Point(220, 330);
						checkBox2.Location = new System.Drawing.Point(258, 330);
						checkBox3.Location = new System.Drawing.Point(296, 330);
						label11.Visible = true;
						label12.Location = new System.Drawing.Point(5, 327);
						label13.Visible = false;
						button1.Visible = false;
						textBox1.ReadOnly = true;
						break;
				}
				_siteDetails_UIMode = value;
			}
		}
		
		static DataTable buildSitesTable() {
			DataTable dt = new DataTable();
			
			dt.Columns.Add(new DataColumn("SITE"));
			dt.Columns.Add(new DataColumn("JVCO_ID"));
			dt.Columns.Add(new DataColumn("GSM900"));
			dt.Columns.Add(new DataColumn("GSM1800"));
			dt.Columns.Add(new DataColumn("UMTS900"));
			dt.Columns.Add(new DataColumn("UMTS2100"));
			dt.Columns.Add(new DataColumn("LTE800"));
			dt.Columns.Add(new DataColumn("LTE2600"));
			dt.Columns.Add(new DataColumn("EASTING"));
			dt.Columns.Add(new DataColumn("NORTHING"));
			dt.Columns.Add(new DataColumn("HOST"));
			dt.Columns.Add(new DataColumn("PRIORITY"));
			dt.Columns.Add(new DataColumn("ADDRESS"));
			dt.Columns.Add(new DataColumn("POWER_COMPANY"));
			dt.Columns.Add(new DataColumn("POWER_CONTACT"));
			dt.Columns.Add(new DataColumn("TELLABSATRISK"));
			dt.Columns.Add(new DataColumn("AREA"));
			dt.Columns.Add(new DataColumn("NSN_STATUS"));
			dt.Columns.Add(new DataColumn("NOC2G"));
			dt.Columns.Add(new DataColumn("NOC3G"));
			dt.Columns.Add(new DataColumn("NOC4G"));
			dt.Columns.Add(new DataColumn("VF_REGION"));
			dt.Columns.Add(new DataColumn("SPECIAL"));
			dt.Columns.Add(new DataColumn("SPECIAL_START"));
			dt.Columns.Add(new DataColumn("SPECIAL_END"));
			dt.Columns.Add(new DataColumn("VIP"));
			dt.Columns.Add(new DataColumn("SITE_SHARE_OPERATOR"));
			dt.Columns.Add(new DataColumn("SITE_SHARE_SITE_NO"));
			dt.Columns.Add(new DataColumn("SITE_ACCESS"));
			dt.Columns.Add(new DataColumn("SITE_TYPE"));
			dt.Columns.Add(new DataColumn("SITE_SUBTYPE"));
			dt.Columns.Add(new DataColumn("PAKNET_FITTED"));
			dt.Columns.Add(new DataColumn("VODAPAGE_FITTED"));
			dt.Columns.Add(new DataColumn("DC_STATUS"));
			dt.Columns.Add(new DataColumn("DC_TIMESTAMP"));
			dt.Columns.Add(new DataColumn("COOLING_STATUS"));
			dt.Columns.Add(new DataColumn("COOLING_TIMESTAMP"));
			dt.Columns.Add(new DataColumn("KEY_INFORMATION"));
			dt.Columns.Add(new DataColumn("EF_HEALTHANDSAFETY"));
			dt.Columns.Add(new DataColumn("SWITCH2G"));
			dt.Columns.Add(new DataColumn("SWITCH3G"));
			dt.Columns.Add(new DataColumn("SWITCH4G"));
			dt.Columns.Add(new DataColumn("DRSWITCH2G"));
			dt.Columns.Add(new DataColumn("DRSWITCH3G"));
			dt.Columns.Add(new DataColumn("DRSWITCH4G"));
			dt.Columns.Add(new DataColumn("MTX2G"));
			dt.Columns.Add(new DataColumn("MTX3G"));
			dt.Columns.Add(new DataColumn("MTX4G"));
			dt.Columns.Add(new DataColumn("MTX_RELATED"));

			return dt;
		}
		
		static DataTable buildCellsTable() {
			DataTable dt = new DataTable();
			
			dt.Columns.Add(new DataColumn("SITE"));
			dt.Columns.Add(new DataColumn("JVCO_ID"));
			dt.Columns.Add(new DataColumn("CELL_ID"));
			dt.Columns.Add(new DataColumn("LAC_TAC"));
			dt.Columns.Add(new DataColumn("BSC_RNC_ID"));
			dt.Columns.Add(new DataColumn("VENDOR"));
			dt.Columns.Add(new DataColumn("ENODEB_ID"));
			dt.Columns.Add(new DataColumn("TF_SITENO"));
			dt.Columns.Add(new DataColumn("CELL_NAME"));
			dt.Columns.Add(new DataColumn("BEARER"));
			dt.Columns.Add(new DataColumn("COOS"));
			dt.Columns.Add(new DataColumn("SO_EXCLUSION"));
			dt.Columns.Add(new DataColumn("WHITE_LIST"));
			dt.Columns.Add(new DataColumn("NTQ"));
			dt.Columns.Add(new DataColumn("NOC"));
			dt.Columns.Add(new DataColumn("WBTS_BCF"));
			dt.Columns.Add(new DataColumn("LOCKED"));
			
//			dt.Rows.Add("35","V0018305S","3726","304","BEASN","Ericsson",null,null,"G00351","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("35","V0018305S","3727","304","BEASN","Ericsson",null,null,"G00352","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("35","V0018305S","3728","304","BEASN","Ericsson",null,null,"G00353","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("35","V0018305S","33067","21481","BEASN","Ericsson",null,null,"G0035W","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("35","V0018305S","23067","21481","BEASN","Ericsson",null,null,"G0035X","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("35","V0018305S","13067","21481","BEASN","Ericsson",null,null,"G0035Y","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("35","V0018305S","63051","8","RNCCY3","Ericsson",null,null,"M00035015","3G",null,null,null,null,"TANOC","WBTS-999",null);
//			dt.Rows.Add("35","V0018305S","63050","8","RNCCY3","Ericsson",null,null,"M00035025","3G",null,null,null,null,"TANOC","WBTS-999",null);
//			dt.Rows.Add("35","V0018305S","63049","8","RNCCY3","Ericsson",null,null,"M00035035","3G",null,null,null,null,"TANOC","WBTS-999",null);
//			dt.Rows.Add("35","V0018305S","17715","21723","RNCCY3","Ericsson",null,null,"TM00035017","3G",null,null,null,null,"TANOC","WBTS-1999",null);
//			dt.Rows.Add("35","V0018305S","27715","21723","RNCCY3","Ericsson",null,null,"TM00035027","3G",null,null,null,null,"TANOC","WBTS-1999",null);
//			dt.Rows.Add("35","V0018305S","37715","21723","RNCCY3","Ericsson",null,null,"TM00035037","3G",null,null,null,null,"TANOC","WBTS-1999",null);
//			dt.Rows.Add("35","V0018305S","10","24615","XCY_CA_01","Ericsson","1489",null,"N00035010","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","20","24615","XCY_CA_01","Ericsson","1489",null,"N00035020","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","30","24615","XCY_CA_01","Ericsson","1489",null,"N00035030","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","110","16784","XCY_CA_01","Ericsson","101489",null,"TN00035110","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","120","16784","XCY_CA_01","Ericsson","101489",null,"TN00035120","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("35","V0018305S","130","16784","XCY_CA_01","Ericsson","101489",null,"TN00035130","4G",null,null,null,null,"TF",null,null);
//			dt.Rows.Add("125","V0018305S","33067","21481","BEASN","Ericsson",null,null,"G0125W","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("125","V0018305S","23067","21481","BEASN","Ericsson",null,null,"G0125X","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("125","V0018305S","13067","21481","BEASN","Ericsson",null,null,"G0125Y","2G",null,null,null,null,"ANOC","BCF-1505",null);
//			dt.Rows.Add("136","V0018305S","3726","304","BEASN","Ericsson",null,null,"G001361","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("136","V0018305S","3727","304","BEASN","Ericsson",null,null,"G001362","2G",null,null,null,null,"ANOC","BCF-505",null);
//			dt.Rows.Add("136","V0018305S","3728","304","BEASN","Ericsson",null,null,"G001363","2G",null,null,null,null,"ANOC","BCF-505",null);
			
			return dt;
		}
	}
}

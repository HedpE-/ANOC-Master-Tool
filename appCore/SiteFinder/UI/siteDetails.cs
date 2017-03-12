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
using appCore.Templates.UI;

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
		List<GMapMarker> markersList = new List<GMapMarker>();
		public Site currentSite;
		List<Site> foundSites = new List<Site>(); // for outage and bulk sites lists
		string[] sites; // for outages site list
		byte listView2_EventCount = 1;
		
		AMTMenuStrip MainMenu = new AMTMenuStrip(1090);
		ToolStripMenuItem bulkSiteSearchMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem lockUnlockCellsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem lockedCellsPageToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem viewSiteInOiToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem sitesPerTechToolStripMenuItem = new ToolStripMenuItem();
		
		public Control parentControl {
			get;
			private set;
		}
		
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
				MainMenu.InitializeTroubleshootMenu(true);
				MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
				MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
				InitializeToolStripMenuItems();
				MainMenu.siteFinder_Toggle(false, false);
				if(mode[0] != "outage") {
					MainMenu.MainMenu.DropDownItems.Add(bulkSiteSearchMenuItem);
					if(Settings.CurrentUser.UserName == "GONCARJ3") {
						MainMenu.MainMenu.DropDownItems.Add("-");
						MainMenu.MainMenu.DropDownItems.Add(lockUnlockCellsToolStripMenuItem);
						MainMenu.MainMenu.DropDownItems.Add("-");
						MainMenu.MainMenu.DropDownItems.Add(lockedCellsPageToolStripMenuItem);
					}
				}
				switch(mode[0]) {
					case "single":
						label11.Visible = false;
						listView2.Visible = false;
						checkBox1.Location = new Point(220, 230);
						checkBox2.Location = new Point(258, 230);
						checkBox3.Location = new Point(296, 230);
						dataGridView1.Location = new Point(5, 247);
						dataGridView1.Size = new Size(555 + SystemInformation.VerticalScrollBarWidth, 488);
						label12.Location = new Point(5, 227);
						textBox1.ReadOnly = value.Contains("readonly");
						bulkSiteSearchMenuItem.Enabled =
							lockedCellsPageToolStripMenuItem.Enabled = !value.Contains("readonly");
						break;
					case "multi":
						label11.Visible = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Top =
							checkBox2.Top =
							checkBox3.Top =
							checkBox6.Top =
							checkBox7.Top = 355;
						dataGridView1.Location = new Point(5, 372);
						dataGridView1.Size = new Size(555 + SystemInformation.VerticalScrollBarWidth, 274);
						label12.Location = new Point(5, 352);
						bulkSiteSearchMenuItem.Enabled =
							lockedCellsPageToolStripMenuItem.Enabled = value.Contains("readonly");
						textBox1.ReadOnly = !value.Contains("readonly");
						break;
					case "outage":
						dataGridView1.Location = new Point(5, 372);
						dataGridView1.Size = new Size(555 + SystemInformation.VerticalScrollBarWidth, 274);
//						dataGridView1.CheckBoxes = true;
						listView2.Visible = true;
						listView2.Items.Clear();
						checkBox1.Top =
							checkBox2.Top =
							checkBox3.Top =
							checkBox6.Top =
							checkBox7.Top = 355;
						label11.Visible = true;
						label12.Top = 352;
						if(Settings.CurrentUser.Role == "Shift Leader")
							MainMenu.MainMenu.DropDownItems.Add(sitesPerTechToolStripMenuItem);
//						bulkSiteSearchMenuItem.Enabled =
//							lockedCellsPageToolStripMenuItem.Enabled = false;
						textBox1.ReadOnly = true;
						break;
				}
				_siteDetails_UIMode = value;
			}
		}
		
		public siteDetails(Site site, Control parent = null)
		{
			currentSite = site;
			
			parentControl = parent;
//			parentControl = parent;
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
			this.Text = "Site " + site.Id + " Details";
			myMap = drawGMap("myMap",false);
			Controls.Add(myMap);
			Shown += populateSingleForm;
		}
		
		public siteDetails(bool isOutage, OutageControls parent)
		{
			InitializeComponent();
			
			parentControl = parent;
			this.Text = "Outage Follow Up";
			
			myMap = drawGMap("myMap",true);
			this.Controls.Add(myMap);
			if(isOutage) {
				siteDetails_UIMode = "outage";
				
				sites = (parent.currentOutage.VfBulkCI + parent.currentOutage.TefBulkCI).Split(';');
				for(int c = 0;c < sites.Length;c++) {
					if(sites[c].StartsWith("0")) {
						while(sites[c].StartsWith("0"))
							sites[c] = sites[c].Substring(1);
					}
					else
						break;
				}
				sites = sites.Distinct().ToArray(); // Remover duplicados
				sites = sites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
				parent.currentOutage.AffectedSites = DB.SitesDB.getSites(sites.ToList());
				
				List<string> cells = parent.currentOutage.VfGsmCells;
				cells.AddRange(parent.currentOutage.VfUmtsCells);
				cells.AddRange(parent.currentOutage.VfLteCells);
				cells.AddRange(parent.currentOutage.TefGsmCells);
				cells.AddRange(parent.currentOutage.TefUmtsCells);
				cells.AddRange(parent.currentOutage.TefLteCells);
				
				for(int c = 0;c < cells.Count;c++) {
					string[] strToFind = { " - " };
					var tempCell = cells[c].Split(strToFind, StringSplitOptions.None);
					if(tempCell.Length > 1)
						cells[c] = tempCell[1];
				}
				
				parent.currentOutage.AffectedCells = DB.SitesDB.getCells(cells);
				foreach(Cell cell in parent.currentOutage.AffectedCells) {
					Site site = parent.currentOutage.AffectedSites.Find(s => s.Id == cell.ParentSite);
					int siteIndex = parent.currentOutage.AffectedSites.IndexOf(site);
					if(siteIndex > -1) {
						if(parent.currentOutage.AffectedSites[siteIndex].CellsInOutage == null)
							parent.currentOutage.AffectedSites[siteIndex].CellsInOutage = new List<Cell>();
						parent.currentOutage.AffectedSites[siteIndex].CellsInOutage.Add(cell);
					}
				}
			}
			else
				siteDetails_UIMode = "single";
			this.Shown += populateBulkForm;
		}
		
		public siteDetails()
		{
			InitializeComponent();
			this.Text = "Site Finder";
			myMap = drawGMap("myMap",true);
			this.Controls.Add(myMap);
			siteDetails_UIMode = "single";
			lockUnlockCellsToolStripMenuItem.Enabled = false;
			this.Shown += populateBulkForm;
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
			                           	
			                           	selectedSiteDetailsPopulate();
			                           	selectedSiteOverlay.Markers.Add(currentSite.MapMarker);
			                           	myMap.Overlays.Add(selectedSiteOverlay);
			                           	myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
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
			                           		this.Name = "Outage Follow-up";
			                           		this.Text = this.Name;
			                           		siteFinder(sites);
			                           	}
			                           	else
			                           		textBox1.Select();
//			                           	if(dataGridView1.Controls.OfType<VScrollBar>().First().Visible)
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
		
		GMapControl drawGMap(string mapName,bool multi) {
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
			map.Location = new Point(dataGridView1.Right + 5, amtTextBox3.Bottom + 5);
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
				lockUnlockCellsToolStripMenuItem.Enabled = siteDetails_UIMode.Contains("single") && !siteDetails_UIMode.Contains("readonly");
				
//				addCheckBoxColumn();
				
//				listboxFilter_Changed(checkBox1, null);
			}
			else {
				foreach(Control ctr in Controls) {
					if(ctr.Name != "textBox1" && !ctr.Name.Contains("label") && !string.IsNullOrEmpty(ctr.Name)) {
						if(ctr.Name != "dataGridView1") {
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
			
//			dataGridView1.DataSource = null;
//			dataGridView1.Columns.Clear();
//
//			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridView1.DataSource = GetCellsDV();
			int c = 0;
			int[] columnWidths = { 35, 80, 50, 56, 70, 65, 55, 46, 45, 50 };
			foreach(DataGridViewColumn dgvc in dataGridView1.Columns) {
				dgvc.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
				dgvc.Width = columnWidths[c++];
				if(dgvc.Name == "Tech" || dgvc.Name == "COOS" || dgvc.Name == "Locked")
					dgvc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			}
			
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
			if(!siteDetails_UIMode.Contains("outage")) {
				table.Columns.Add("COOS");
				table.Columns.Add("Locked");
			}
			
			foreach(Cell cell in currentSite.Cells) {
				string ossID;
				if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
					ossID = cell.ENodeB_Id;
				else
					ossID = cell.WBTS_BCF;
				var row = table.Rows.Add(cell.Bearer, cell.Name, cell.Id, cell.LacTac, cell.BscRnc_Id, ossID, cell.Vendor.ToString(), cell.Noc);
				if(!siteDetails_UIMode.Contains("outage")) {
					row["COOS"] = cell.COOS ? "YES" : "No";
					row["Locked"] = cell.Locked ? "YES" : "No";
				}
			}
			
			return table.DefaultView;
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
			if(dataGridView1.Columns[e.ColumnIndex].Name == "Locked" || dataGridView1.Columns[e.ColumnIndex].Name == "COOS")
				e.CellStyle.BackColor = e.Value.ToString() == "YES" ? Color.OrangeRed : Color.LightGreen;
			
			if(dataGridView1.Columns[e.ColumnIndex].Name == "Cell Name") {
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
			}
		}

//		void ListView1KeyDown(object sender, KeyEventArgs e)
//		{
//			if(e.Control && e.KeyCode != Keys.ControlKey) {
//				switch(e.KeyCode) {
//					case Keys.C:
//						if(listView1.SelectedItems.Count > 0) {
//							string copiedCells = string.Empty;
//							foreach (ListViewItem cell in listView1.SelectedItems) {
//								if(!string.IsNullOrEmpty(copiedCells))
//									copiedCells += Environment.NewLine;
//								copiedCells += cell.SubItems[1].Text;
//							}
//							Clipboard.SetText(copiedCells);
//						}
//						break;
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

		void ShowSitesPerTech(object sender, EventArgs e)
		{
			
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
			foreach (Site site in foundSites) {
				listView2.Items.Add(new ListViewItem(new[]{ site.Id, site.JVCO_Id, site.Priority, site.Host, site.PostCode }));
				
				foreach (ColumnHeader col in listView2.Columns)
					col.Width = -2;
				listView2.ResumeLayout();
				
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

		void siteFinder(object sender, KeyPressEventArgs e)
		{
			string site = ((TextBoxBase)sender).Text;
			if(Convert.ToInt32(e.KeyChar) == 13 && !((TextBoxBase)sender).ReadOnly) {
				Action actionThreaded = new Action(delegate {
				                                   	try {
				                                   		myMap.Overlays.Remove(markersOverlay);
				                                   		myMap.Overlays.Remove(selectedSiteOverlay);
				                                   		markersOverlay.Clear();
				                                   		selectedSiteOverlay.Clear();
				                                   	}
				                                   	catch (Exception) {
				                                   	}
				                                   	
				                                   	currentSite = DB.SitesDB.getSite(site);
				                                   	
				                                   	if(currentSite.Exists) {
				                                   		string dataToRequest = "INCCellsState";
				                                   		if((DateTime.Now - currentSite.ChangesTimestamp) > new TimeSpan(0, 30, 0))
				                                   			dataToRequest += "CRQ";
				                                   		if(string.IsNullOrEmpty(currentSite.PowerCompany))
				                                   			dataToRequest += "PWR";
				                                   		currentSite.requestOIData(dataToRequest);
				                                   	}
				                                   });
				
				Action actionNonThreaded = new Action(delegate {
				                                      	if(currentSite.Exists) {
				                                      		selectedSiteDetailsPopulate();
				                                      		selectedSiteOverlay.Markers.Add(currentSite.MapMarker);
				                                      		myMap.Overlays.Add(selectedSiteOverlay);
				                                      		myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
				                                      	}
				                                      	else {
				                                      		selectedSiteDetailsPopulate();
				                                      		myMap.SetPositionByKeywords("UK");
				                                      		myMap.Zoom = 6;
				                                      	}
				                                      	MainMenu.siteFinder_Toggle(currentSite.Exists);
				                                      });
				
				LoadingPanel load = new LoadingPanel();
				load.ShowAsync(actionThreaded, actionNonThreaded, true, this);
			}
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
			                           			selectedSiteOverlay.Markers.Add(foundSites[0].MapMarker);
			                           			myMap.Overlays.Add(selectedSiteOverlay);
			                           			myMap.ZoomAndCenterMarkers(selectedSiteOverlay.Id);
			                           		}
			                           	}
			                           });
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, action,true,this);
		}

		void ListView2ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(listView2_EventCount == 0)
				listView2_EventCount++;
			else {
				if(listView2.SelectedItems.Count > 0) {
					if(currentSite != null)
						foundSites[foundSites.FindIndex(s => s.Id == currentSite.Id)] = currentSite;
					
					currentSite = foundSites[listView2.SelectedItems[0].Index];
					
					selectedSiteDetailsPopulate();
					if(listView2_EventCount == 1) {
						foreach(GMapMarker marker in markersOverlay.Markers) {
							if(marker.Tag.ToString() == textBox1.Text) {
								myMap.Position = marker.Position;
								myMap.Zoom = 14;
								break;
							}
						}
					}
					MainMenu.siteFinder_Toggle(currentSite.Exists);
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
						if(currentSite.Incidents == null) {
							currentSite.requestOIData("INC");
							if(currentSite.Incidents.Count > 0) {
								MainMenu.INCsButton.Enabled = true;
								MainMenu.INCsButton.ForeColor = Color.DarkGreen;
								MainMenu.INCsButton.Text = "INCs (" + currentSite.Incidents.Count + ")";
							}
							else {
								MainMenu.INCsButton.Enabled = false;
								MainMenu.INCsButton.Text = "No INC history";
							}
							return;
						}
						dataToShow = "INCs";
						break;
					case "CRQsButton":
						if(currentSite.Changes == null) {
							currentSite.requestOIData("CRQ");
							if(currentSite.Changes.Count > 0) {
								MainMenu.CRQsButton.Enabled = true;
								MainMenu.CRQsButton.ForeColor = Color.DarkGreen;
								MainMenu.CRQsButton.Text = "CRQs (" + currentSite.Changes.Count + ")";
							}
							else {
								MainMenu.CRQsButton.Enabled = false;
								MainMenu.CRQsButton.Text = "No CRQ history";
							}
							return;
						}
						dataToShow = "CRQs";
						break;
					case "BookInsButton":
						if(currentSite.Visits == null) {
							currentSite.requestOIData("Bookins");
							if(currentSite.Visits.Count > 0) {
								MainMenu.BookInsButton.Enabled = true;
								MainMenu.BookInsButton.ForeColor = Color.DarkGreen;
								MainMenu.BookInsButton.Text = "Book Ins List (" + currentSite.Visits.Count + ")";
							}
							else {
								MainMenu.BookInsButton.Enabled = false;
								MainMenu.BookInsButton.Text = "No Book In history";
							}
							return;
						}
						dataToShow = "BookIns";
						break;
					case "ActiveAlarmsButton":
						if(currentSite.Alarms == null) {
							currentSite.requestOIData("Alarms");
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
						dataToShow = "ActiveAlarms";
						break;
					case "AvailabilityButton":
						if(currentSite.Availability == null) {
							currentSite.requestOIData("Availability");
							if(currentSite.Availability.Rows.Count > 0) {
								MainMenu.AvailabilityButton.Enabled = true;
								MainMenu.AvailabilityButton.ForeColor = Color.DarkGreen;
								MainMenu.AvailabilityButton.Text = "Availability chart";
							}
							else {
								MainMenu.AvailabilityButton.Enabled = false;
								MainMenu.AvailabilityButton.Text = "No availability chart to display";
							}
							return;
						}
						dataToShow = "Availability";
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
						OiTable = new OiSiteTablesForm(currentSite.Availability, "Availability", currentSite.Id, this);
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

		void LockedCellsPage(object sender, EventArgs e) {
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "Cells Locked") {
					if(frm.WindowState == FormWindowState.Minimized) frm.Invoke(new Action(() => { frm.WindowState = FormWindowState.Normal; }));;
					frm.Invoke(new MethodInvoker(frm.Activate));
					return;
				}
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
			lockedCellsPageToolStripMenuItem.Click += LockedCellsPage;
			// 
			// sitesPerTechToolStripMenuItem
			// 
			sitesPerTechToolStripMenuItem.Name = "sitesPerTechToolStripMenuItem";
			sitesPerTechToolStripMenuItem.Text = "Show Sites Per Tech...";
			sitesPerTechToolStripMenuItem.Click += ShowSitesPerTech;
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

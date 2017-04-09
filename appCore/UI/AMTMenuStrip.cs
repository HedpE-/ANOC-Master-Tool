/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 19-10-2016
 * Time: 20:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using appCore.SiteFinder;
using appCore.SiteFinder.UI;
using appCore.Templates.UI;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTMenuStrip.
	/// </summary>
	public class AMTMenuStrip : MenuStrip
	{
		public ToolStripMenuItem MainMenu = new ToolStripMenuItem();
		
		public ToolStripMenuItem INCsButton = new ToolStripMenuItem();
		public ToolStripMenuItem CRQsButton = new ToolStripMenuItem();
		public ToolStripMenuItem BookInsButton = new ToolStripMenuItem();
		public ToolStripMenuItem ActiveAlarmsButton = new ToolStripMenuItem();
		public ToolStripMenuItem AvailabilityButton = new ToolStripMenuItem();
		
		public ToolStripMenuItem RefreshButton = new ToolStripMenuItem();
		
		bool AvailabilityButtonEnabled;
		
		Site currentSite {
			get {
				Site site = null;
				switch(Parent.GetType().ToString()) {
					case "appCore.Templates.UI.TroubleshootControls":
						site = ((TroubleshootControls)Parent).currentSite;
						break;
					case "appCore.Templates.UI.FailedCRQControls":
						site = ((FailedCRQControls)Parent).currentSite;
						break;
					case "appCore.Templates.UI.UpdateControls":
						site = ((UpdateControls)Parent).currentSite;
						break;
					case "appCore.SiteFinder.UI.siteDetails":
						site = ((siteDetails)Parent).currentSite;
						break;
					case "appCore.SiteFinder.UI.LockUnlockCellsForm":
						site = ((LockUnlockCellsForm)Parent).currentSite;
						break;
				}
				return site;
			}
		}
		
		public EventHandler OiButtonsOnClickDelegate {
			get { return null; }
			set {
				INCsButton.Click += value;
				CRQsButton.Click += value;
				BookInsButton.Click += value;
				ActiveAlarmsButton.Click += value;
				if(AvailabilityButtonEnabled)
					AvailabilityButton.Click += value;
			}
		}
		
		public EventHandler RefreshButtonOnClickDelegate {
			get { return null; }
			set {
				RefreshButton.Click += value;
			}
		}
		
		public AMTMenuStrip(int width = 505, int height = 25) {
			Width = width;
			Height = height;
			MainMenu.Text = "|||";
			Items.Add(MainMenu);
			InitializeComponent();
		}
		
		public void InitializeTroubleshootMenu(bool enableAvailabilityButton = false) {
			
			AvailabilityButtonEnabled = enableAvailabilityButton;
			INCsButton.Text = "INCs";
			CRQsButton.Text = "CRQs";
			BookInsButton.Text = "BookIns";
			ActiveAlarmsButton.Text = "Alarms";
			RefreshButton.Text = '\u21bb'.ToString(); // \u21bb clockwise arrow unicode character
			
			Items.AddRange(new ToolStripItem[]{
			               	INCsButton,
			               	CRQsButton,
			               	BookInsButton,
			               	ActiveAlarmsButton
			               });
			if(AvailabilityButtonEnabled) {
				AvailabilityButton.Text = "Availability Chart";
				Items.Add(AvailabilityButton);
				DynamicButtonsSizes();
			}
			Items.Add(RefreshButton);
		}
		
		void showRightClickContext(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
				if(tsmi != null) {
					if(!tsmi.Text.StartsWith("No ") && !tsmi.Text.StartsWith("Click ")) {
						ContextMenuStrip context = new ContextMenuStrip();
						ToolStripItem item = null;
						string dataToRequest = string.Empty;
						switch(tsmi.Name) {
							case "INCsButton":
								item = context.Items.Add("Refresh INC data");
								dataToRequest = "INC";
								break;
							case "CRQsButton":
								item = context.Items.Add("Refresh CRQ data");
								dataToRequest = "CRQ";
								break;
							case "ActiveAlarmsButton":
								item = context.Items.Add("Refresh Alarms data");
								dataToRequest = "Alarms";
								break;
							case "BookInsButton":
								item = context.Items.Add("Refresh Visits data");
								dataToRequest = "Bookins";
								break;
							case "AvailabilityButton":
								item = context.Items.Add("Refresh Availability data");
								dataToRequest = "Availability";
								break;
						}
						
						item.Click += (s, a) => {
							currentSite.requestOIData(dataToRequest);
							siteFinder_Toggle(true);
						};
						
						context.Show(this, PointToClient(Cursor.Position));
					}
				}
			}
		}

		public void siteFinder_Toggle(bool toggle, bool siteFound = true) {
			foreach (ToolStripMenuItem tsmi in Items) {
				if(tsmi.Name.Contains("Button")) {
					if(currentSite != null) {
						if(currentSite.Exists) {
							switch(tsmi.Name) {
								case "INCsButton":
									if(currentSite.Incidents != null) {
										if(currentSite.Incidents.Count > 0) {
											tsmi.Enabled = true;
											tsmi.ForeColor = Color.DarkGreen;
											tsmi.Text = "INCs (" + currentSite.Incidents.Count + ")";
										}
										else {
											tsmi.Enabled = false;
											tsmi.Text = "No INC history";
										}
									}
									else {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load INCs";
									}
									break;
								case "CRQsButton":
									if(currentSite.Changes != null) {
										if(currentSite.Changes.Count > 0) {
											tsmi.Enabled = true;
											tsmi.ForeColor = Color.DarkGreen;
											tsmi.Text = "CRQs (" + currentSite.Changes.Count + ")";
										}
										else {
											tsmi.Enabled = false;
											tsmi.Text = "No CRQ history";
										}
									}
									else {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load CRQs";
									}
									break;
								case "BookInsButton":
									if(currentSite.Visits != null) {
										if(currentSite.Visits.Count > 0) {
											tsmi.Enabled = true;
											tsmi.ForeColor = Color.DarkGreen;
											tsmi.Text = "Book Ins (" + currentSite.Visits.Count + ")";
										}
										else {
											tsmi.Enabled = false;
											tsmi.Text = "No Book In history";
										}
									}
									else {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load Book Ins";
									}
									break;
								case "ActiveAlarmsButton":
									if(currentSite.Alarms != null) {
										if(currentSite.Alarms.Count > 0) {
											tsmi.Enabled = true;
											tsmi.ForeColor = Color.DarkGreen;
											tsmi.Text = "Active alarms (" + currentSite.Alarms.Count + ")";
										}
										else {
											tsmi.Enabled = false;
											tsmi.Text = "No alarms to display";
										}
									}
									else {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load alarms";
									}
									break;
								case "AvailabilityButton":
									if(currentSite.Availability != null) {
										if(currentSite.Availability.Rows.Count > 0) {
											tsmi.Enabled = true;
											tsmi.ForeColor = Color.DarkGreen;
											tsmi.Text = "Availability chart";
										}
										else {
											tsmi.Enabled = false;
											tsmi.Text = "No availability chart to display";
										}
									}
									else {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load availability";
									}
									break;
								case "RefreshButton":
									tsmi.Enabled = true;
									tsmi.Text = '\u21bb'.ToString();
									break;
							}
						}
						else {
							tsmi.Enabled = false;
							tsmi.Text = string.Empty;
						}
					}
					else {
						INCsButton.Enabled = false;
						INCsButton.Text = string.Empty;
						CRQsButton.Enabled = false;
						CRQsButton.Text = string.Empty;
						ActiveAlarmsButton.Enabled = false;
						ActiveAlarmsButton.Text = string.Empty;
						BookInsButton.Enabled = false;
						BookInsButton.Text = string.Empty;
						RefreshButton.Enabled = false;
						AvailabilityButton.Text = string.Empty;
						AvailabilityButton.Enabled = false;
					}
				}
			}
			
			var fc = Application.OpenForms.OfType<OiSiteTablesForm>().Where(f => f.OwnerControl == this.Parent).ToList();
//			List<OiSiteTablesForm> openForms = new List<OiSiteTablesForm>();
//
//			foreach(OiSiteTablesForm frm in fc) {
//				if(frm.OwnerControl == this.Parent)
//					openForms.Add(frm);
//			}
			
			for(int c = fc.Count - 1;c >= 0;c--)
				fc[c].Close();
		}
		
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			InitializeComponent();
			Invalidate();
		}
		
		void InitializeComponent()
		{
			// 
			// thisMenuStrip
			// 
			Renderer = new AMTToolStripSystemRenderer();
			GripMargin = new Padding(0, 0, 0, 0);
			Padding = new Padding(0, 0, 0, 0);
			Name = "AMTMenuStrip";
			TabIndex = 1;
			// 
			// MainMenuToolStripMenuItem
			// 
			MainMenu.Name = "MainMenu";
			MainMenu.TextAlign = ContentAlignment.MiddleCenter;
//			MainMenu.Font = new Font("Arial Unicode MS", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			((ToolStripDropDownMenu) MainMenu.DropDown).ShowImageMargin = false;
			((ToolStripDropDownMenu) MainMenu.DropDown).ShowCheckMargin = false;
			// 
			// Refresh
			// 
			RefreshButton.Name = "RefreshButton";
			RefreshButton.Font = new Font("Arial Unicode MS", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			RefreshButton.TextAlign = ContentAlignment.TopCenter;
			RefreshButton.AutoSize = false;
			// 
			// AvailabilityButton
			// 
			AvailabilityButton.AutoSize = false;
			AvailabilityButton.Name = "AvailabilityButton";
			AvailabilityButton.TextAlign = ContentAlignment.MiddleCenter;
//			AvailabilityButton.Font = new Font("Segoe UI", 9F);
			AvailabilityButton.MouseDown += showRightClickContext;
			// 
			// ActiveAlarmsButton
			// 
			ActiveAlarmsButton.AutoSize = false;
			ActiveAlarmsButton.Name = "ActiveAlarmsButton";
			ActiveAlarmsButton.TextAlign = ContentAlignment.MiddleCenter;
//			ActiveAlarmsButton.Font = new Font("Segoe UI", 9F);
			ActiveAlarmsButton.MouseDown += showRightClickContext;
			// 
			// BookInsButton
			// 
			BookInsButton.AutoSize = false;
			BookInsButton.Name = "BookInsButton";
			BookInsButton.TextAlign = ContentAlignment.MiddleCenter;
//			BookInsButton.Font = new Font("Segoe UI", 9F);
			BookInsButton.MouseDown += showRightClickContext;
			// 
			// CRQsButton
			// 
			CRQsButton.AutoSize = false;
			CRQsButton.Name = "CRQsButton";
			CRQsButton.TextAlign = ContentAlignment.MiddleCenter;
//			refreshToolStripMenuItem.Font = new Font("Segoe UI", 9F);
			CRQsButton.MouseDown += showRightClickContext;
			// 
			// INCsButton
			// 
			INCsButton.AutoSize = false;
			INCsButton.Name = "INCsButton";
			INCsButton.TextAlign = ContentAlignment.MiddleCenter;
//			INCsButton.Font = new Font("Segoe UI", 9F);
			INCsButton.MouseDown += showRightClickContext;
			
			DynamicButtonsSizes();
		}
		
		void DynamicButtonsSizes() {
//			double widthPercentage = AvailabilityButtonEnabled ? 0.18 : 0.225;
			double widthPercentage = AvailabilityButtonEnabled ? 0.179 : 0.224;
			
			RefreshButton.Size = new Size((int)(Width * 0.05), Height);
			if(AvailabilityButtonEnabled)
				AvailabilityButton.Size = new Size((int)(Width * widthPercentage), Height);
			ActiveAlarmsButton.Size = new Size((int)(Width * widthPercentage), Height);
			BookInsButton.Size = new Size((int)(Width * widthPercentage), Height);
			CRQsButton.Size = new Size((int)(Width * widthPercentage), Height);
			INCsButton.Size = new Size((int)(Width * widthPercentage), Height);
			MainMenu.Size = new Size((int)(Width / 0.05), Height);
		}
	}
}

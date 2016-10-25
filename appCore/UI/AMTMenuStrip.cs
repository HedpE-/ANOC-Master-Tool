/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 19-10-2016
 * Time: 20:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using appCore.UI;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTMenuStrip.
	/// </summary>
	public class AMTMenuStrip : MenuStrip
	{
		public ToolStripMenuItem MainMenuToolStripMenuItem = new ToolStripMenuItem();
		
		public ToolStripMenuItem INCsButton = new ToolStripMenuItem();
		public ToolStripMenuItem CRQsButton = new ToolStripMenuItem();
		public ToolStripMenuItem BookInsButton = new ToolStripMenuItem();
		public ToolStripMenuItem ActiveAlarmsButton = new ToolStripMenuItem();
		
		ToolStripMenuItem refreshToolStripMenuItem = new ToolStripMenuItem();
		
		public EventHandler OiButtonsOnClickDelegate {
			get { return null; }
			set {
				INCsButton.Click += value;
				CRQsButton.Click += value;
				BookInsButton.Click += value;
				ActiveAlarmsButton.Click += value;
			}
		}
		
		public EventHandler RefreshButtonOnClickDelegate {
			get { return null; }
			set {
				refreshToolStripMenuItem.Click += value;
			}
		}
		
		public AMTMenuStrip(int width = 0, int height = 0) {
			if(width != 0)
				Width = width;
			if(height != 0)
				Height = height;
			InitializeComponent();
		}
		
		public void InitializeTroubleshootMenu() {
			Items.Add(INCsButton);
			Items.Add(CRQsButton);
			Items.Add(BookInsButton);
			Items.Add(ActiveAlarmsButton);
			Items.Add(refreshToolStripMenuItem);
		}
		
//		void showRightClickContext(object sender, MouseEventArgs e) {
//			if(e.Button == MouseButtons.Right) {
//				ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
//				switch(tsmi.Name) {
//					case "INCsButton":
//						refreshToolStripMenuItem.Text = "Refresh INC data";
//						break;
//					case "CRQsButton":
//						refreshToolStripMenuItem.Text = "Refresh CRQ data";
//						break;
//					case "ActiveAlarmsButton":
//						refreshToolStripMenuItem.Text = "Refresh Alarms data";
//						break;
//					case "BookInsButton":
//						refreshToolStripMenuItem.Text = "Refresh Visits data";
//						break;
//				}
//
//				context.Show((Control)sender, e.Location);
//			}
//		}

		public void siteFinder_Toggle(bool toggle, bool siteFound = true) {
			appCore.SiteFinder.Site currentSite = null;
			switch(Parent.Name) {
				case "Troubleshoot Template GUI":
					currentSite = ((Templates.UI.TroubleshootControls)Parent).currentSite;
					break;
				case "Failed CRQ Template GUI":
					currentSite = ((Templates.UI.FailedCRQControls)Parent).currentSite;
					break;
				case "Update Template GUI":
					currentSite = ((Templates.UI.UpdateControls)Parent).currentSite;
					break;
			}
			foreach (ToolStripMenuItem tsmi in Items) {
				if(tsmi.Name.Contains("Button")) {
					if(currentSite != null) {
						switch(tsmi.Name) {
							case "INCsButton":
								if((currentSite.INCs != null)) {
									if(currentSite.INCs.Rows.Count > 0) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkGreen;
										tsmi.Text = "INCs (" + currentSite.INCs.Rows.Count + ")";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = "No INC history";
									}
								}
								else {
									if(currentSite.Exists) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load INCs";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = string.Empty;
									}
								}
								break;
							case "CRQsButton":
								if(currentSite.CRQs != null) {
									if(currentSite.CRQs.Rows.Count > 0) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkGreen;
										tsmi.Text = "CRQs (" + currentSite.CRQs.Rows.Count + ")";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = "No CRQ history";
									}
								}
								else {
									if(currentSite.Exists) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load CRQ history";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = string.Empty;
									}
								}
								break;
							case "BookInsButton":
								if(currentSite.BookIns != null) {
									if(currentSite.BookIns.Rows.Count > 0) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkGreen;
										tsmi.Text = "Book Ins List (" + currentSite.BookIns.Rows.Count + ")";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = "No Book In history";
									}
								}
								else {
									if(currentSite.Exists) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load Book Ins";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = string.Empty;
									}
								}
								break;
							case "ActiveAlarmsButton":
								if(currentSite.ActiveAlarms != null) {
									if(currentSite.ActiveAlarms.Rows.Count > 0) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkGreen;
										tsmi.Text = "Active alarms (" + currentSite.ActiveAlarms.Rows.Count + ")";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = "No alarms to display";
									}
								}
								else {
									if(currentSite.Exists) {
										tsmi.Enabled = true;
										tsmi.ForeColor = Color.DarkRed;
										tsmi.Text = "Click to load alarms";
									}
									else {
										tsmi.Enabled = false;
										tsmi.Text = string.Empty;
									}
								}
								break;
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
						refreshToolStripMenuItem.Enabled = false;
					}
				}
				else {
					if(tsmi.Name.Contains("refresh"))
						tsmi.Enabled = siteFound;
					else {
						foreach (object ctr in tsmi.DropDownItems) {
							if(ctr.GetType().ToString() != "System.Windows.Forms.ToolStripSeparator") {
								ToolStripMenuItem tsmi2 = ctr as ToolStripMenuItem;
								if(tsmi2.Text.Contains("Generate") || tsmi2.Text == "Site Details" || tsmi2.Text == "Clear") {
									if(tsmi2.Text == "Site Details") {
										if(!toggle || !siteFound)
											tsmi2.Enabled = false;
										else
											tsmi2.Enabled = true;
										break;
									}
									tsmi2.Enabled = toggle;
								}
							}
						}
					}
				}
			}
		}
		
		void InitializeComponent()
		{
			// 
			// thisMenuStrip
			// 
			Renderer = new AMTToolStripSystemRenderer();
			Items.Add(MainMenuToolStripMenuItem);
			GripMargin = new Padding(0, 0, 0, 0);
			Padding = new Padding(0, 0, 0, 0);
			Name = "AMTMenuStrip";
			TabIndex = 1;
			// 
			// MainMenuToolStripMenuItem
			// 
			MainMenuToolStripMenuItem.Name = "MainMenuToolStripMenuItem";
			MainMenuToolStripMenuItem.Text = "|||";
			MainMenuToolStripMenuItem.TextAlign = ContentAlignment.TopCenter;
//			MainMenuToolStripMenuItem.Font = new Font("Arial Unicode MS", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			((ToolStripDropDownMenu) MainMenuToolStripMenuItem.DropDown).ShowImageMargin = false;
			((ToolStripDropDownMenu) MainMenuToolStripMenuItem.DropDown).ShowCheckMargin = false;
			MainMenuToolStripMenuItem.Size = new Size(24, Height);
			// 
			// refreshToolStripMenuItem
			// 
			refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			refreshToolStripMenuItem.Text = '\u21bb'.ToString(); // \u21bb clockwise arrow unicode character
			refreshToolStripMenuItem.TextAlign = ContentAlignment.TopCenter;
			refreshToolStripMenuItem.AutoSize = false;
			refreshToolStripMenuItem.Size = new Size(24, Height);
			// 
			// ActiveAlarmsButton
			// 
			ActiveAlarmsButton.AutoSize = false;
			ActiveAlarmsButton.Size = new Size(114, Height);
			ActiveAlarmsButton.Name = "ActiveAlarmsButton";
			ActiveAlarmsButton.Text = "Alarms";
			ActiveAlarmsButton.TextAlign = ContentAlignment.MiddleCenter;
//			ActiveAlarmsButton.Font = new Font("Segoe UI", 9F);
//			ActiveAlarmsButton.MouseDown += showRightClickContext;
			// 
			// BookInsButton
			// 
			BookInsButton.AutoSize = false;
			BookInsButton.Size = new Size(114, Height);
			BookInsButton.Name = "BookInsButton";
			BookInsButton.Text = "BookIns";
			BookInsButton.TextAlign = ContentAlignment.MiddleCenter;
//			BookInsButton.Font = new Font("Segoe UI", 9F);
//			BookInsButton.MouseDown += showRightClickContext;
			// 
			// CRQsButton
			// 
			CRQsButton.AutoSize = false;
			CRQsButton.Size = new Size(114, Height);
			CRQsButton.Name = "CRQsButton";
			CRQsButton.Text = "CRQs";
			CRQsButton.TextAlign = ContentAlignment.MiddleCenter;
//			refreshToolStripMenuItem.Font = new Font("Segoe UI", 9F);
//			CRQsButton.MouseDown += showRightClickContext;
			// 
			// INCsButton
			// 
			INCsButton.AutoSize = false;
			INCsButton.Size = new Size(114, Height);
			INCsButton.Name = "INCsButton";
			INCsButton.Text = "INCs";
			INCsButton.TextAlign = ContentAlignment.MiddleCenter;
//			INCsButton.Font = new Font("Segoe UI", 9F);
//			INCsButton.MouseDown += showRightClickContext;
		}
	}
}

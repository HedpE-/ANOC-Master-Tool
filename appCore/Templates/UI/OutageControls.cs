/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 31-07-2016
 * Time: 00:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using appCore.Netcool;
using appCore.SiteFinder;
using appCore.Templates.Types;
using appCore.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of OutageControls.
	/// </summary>
	public class OutageControls : Panel
	{
		public Button Button_OuterRight = new Button();
		public Button Button_OuterLeft = new Button();
		public Button Button_InnerLeft = new Button();
		public Button Button_InnerRight = new Button();
		
		Button button3 = new Button();
		Button button4 = new Button();
		Button button12 = new Button();
		Button button46 = new Button();
		Button button25 = new Button();
		
		Button button23 = new Button(); // BulkCILargeTextButton
		Button button22 = new Button(); // Alarms_ReportLargeTextButton
		TabControl tabControl4 = new TabControl(); // VFTFReportTabControl
		TabPage tabPage15 = new TabPage(); // VFReportTabPage
		TabPage tabPage16 = new TabPage(); // TFReportTabPage
		Label label33 = new Label(); // Alarms_ReportLabel
		Label label32 = new Label(); // BulkCILabel
		AMTRichTextBox textBox11 = new AMTRichTextBox(); // BulkCITextBox
		AMTRichTextBox textBox10 = new AMTRichTextBox(); // Alarms_ReportTextBox
		

		Outage currentOutage;
		int paddingLeftRight = 1;
		public int PaddingLeftRight {
			get { return paddingLeftRight; }
			set { paddingLeftRight = value; }
		}
		
		int paddingTopBottom = 1;
		public int PaddingTopBottom {
			get { return paddingTopBottom; }
			set { paddingTopBottom = value; }
		}
		
//		bool toggle;
//		public bool ToggledState {
//			get {
//				return toggle;
//			}
//			set {
//				if(value != toggle){
//					toggle = value;
//				}
//			}
//		}
		
		Template.UIenum _uiMode;
		Template.UIenum UiMode {
			get { return _uiMode; }
			set {
				_uiMode = value;
				if(value == Template.UIenum.Log) {
					PaddingLeftRight = 7;
					InitializeComponent();
					textBox11.ReadOnly = true;
					// 
					// Button_OuterRight
					// 
					Button_OuterRight.Name = "Button_OuterRight";
					Button_OuterRight.Size = new Size(183, 23);
					Button_OuterRight.TabIndex = 24;
					Button_OuterRight.Text = "Copy Outage";
//					Button_OuterRight.Click += CopyOutageReport;
					// 
					// Button_OuterLeft
					// 
					Button_OuterLeft.Name = "Button_OuterLeft";
					Button_OuterLeft.Size = new Size(112, 23);
					Button_OuterLeft.TabIndex = 22;
					Button_OuterLeft.Text = "Generate sites list";
//					Button_OuterLeft.Click += GenerateSitesList;
					// 
					// Button_InnerLeft
					// 
					Button_InnerLeft.Name = "Button_InnerLeft";
					Button_InnerLeft.Size = new Size(121, 23);
					Button_InnerLeft.TabIndex = 23;
					Button_InnerLeft.Text = "Outage Follow Up";
//					Button_InnerLeft.Click += OutageFollowUp;
				}
				else {
					InitializeComponent();
//					TxTypeComboBox.SelectedIndexChanged += TxTypeComboBoxSelectedIndexChanged;
					// 
					// Button_OuterLeft
					// 
					Button_OuterLeft.Name = "Button_OuterLeft";
					Button_OuterLeft.Size = new Size(97, 23);
					Button_OuterLeft.TabIndex = 22;
					Button_OuterLeft.Text = "Generate Report";
					Button_OuterLeft.Click += GenerateReport;
					// 
					// Button_InnerLeft
					// 
					Button_InnerLeft.Name = "Button_InnerLeft";
					Button_InnerLeft.Size = new Size(133, 23);
					Button_InnerLeft.TabIndex = 23;
					Button_InnerLeft.Text = "Generate from sites list";
					Button_InnerLeft.Click += GenerateFromSitesList;
					// 
					// Button_OuterRight
					// 
					Button_OuterRight.Size = new Size(62, 23);
					Button_OuterRight.Name = "Button_OuterRight";
					Button_OuterRight.TabIndex = 24;
					Button_OuterRight.Text = "Clear";
					Button_OuterRight.Click += ClearAllControls;
					// 
					// Button_InnerRight
					// 
//					Button_InnerRight.Enabled = false;
					Button_InnerRight.Name = "Button_InnerRight";
					Button_InnerRight.Size = new Size(121, 23);
					Button_InnerRight.TabIndex = 72;
					Button_InnerRight.Text = "Copy to Clipboard";
					Button_InnerRight.Visible = false;
//					Button_InnerRight.Click += SendBCPForm;
					Controls.Add(Button_InnerRight);
				}
				Controls.Add(Button_OuterLeft);
				Controls.Add(Button_InnerLeft);
				Controls.Add(Button_OuterRight);
				
				Button_OuterLeft.Location = new Point(PaddingLeftRight, textBox11.Bottom + 5);
				Button_InnerLeft.Location = new Point(Button_OuterLeft.Right + 3, textBox11.Bottom + 5);
				Button_OuterRight.Location = new Point(textBox11.Right - Button_OuterRight.Width, textBox11.Bottom + 5);
				Button_InnerRight.Location = new Point(Button_OuterRight.Left - Button_InnerRight.Width - 3, textBox11.Bottom + 5);
				Size = new Size(textBox11.Right + PaddingLeftRight, Button_OuterLeft.Bottom + PaddingTopBottom);
			}
		}
		
		public OutageControls()
		{
			UiMode = Template.UIenum.Template;
		}

		void GenerateReport(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			if (string.IsNullOrEmpty(textBox10.Text)) {
				MessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			try {
				// TODO: Finish revamped outages
				
				Parser alarms = new Parser(textBox10.Text, false);
				currentOutage = alarms.GenerateOutage();

				if(!string.IsNullOrEmpty(currentOutage.VFoutage) && !string.IsNullOrEmpty(currentOutage.TFoutage)) {
					tabControl4.Visible = true;
					tabControl4.SelectTab(0);
				}
				else {
					if(string.IsNullOrEmpty(currentOutage.VFoutage) && string.IsNullOrEmpty(currentOutage.TFoutage)) {
						MainForm.trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
						textBox10.Text = string.Empty;
						return;
					}
					if(!string.IsNullOrEmpty(currentOutage.VFoutage)) {
						tabControl4.Visible = false;
						tabControl4.SelectTab(0);
					}
					else {
						if(!string.IsNullOrEmpty(currentOutage.TFoutage)) {
							tabControl4.Visible = false;
							tabControl4.SelectTab(1);
						}
					}
				}
				if(!string.IsNullOrEmpty(currentOutage.VFoutage) || !string.IsNullOrEmpty(currentOutage.TFoutage)) {
					VFTFReportTabControlSelectedIndexChanged(null,null);
					button4.Text = "Outage Follow Up"; // HACK: Outage Follow Up button on outage report processing
					button4.Width = 100;
					button46.Visible = false;
					button3.Enabled = true;
					textBox10.ReadOnly = true;
					textBox11.ReadOnly = true;
					button12.Visible = true;
					button25.Visible = true;
					textBox10.Focus();
					label33.Text = "Generated Outage Report";
//					LogOutageReport();
//					if (op.tableRemoved)
//						MessageBox.Show("WARNING!!!" + Environment.NewLine + Environment.NewLine + "One or more tables to parse have been removed." + Environment.NewLine + "All tables must have the same columns and order.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				}
			}
			catch {
				MainForm.trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
				return;
			}
//			}
//			                           	else {
//			                           		string[] outageSites = (VFbulkCI + TFbulkCI).Split(';');
//			                           		outageSites = outageSites.Distinct().ToArray(); // Remover duplicados
//			                           		outageSites = outageSites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
//			                           		Thread thread = new Thread(() => {
//			                           		                           	siteDetails sd = new siteDetails(true,outageSites);
//			                           		                           	sd.Name = "Outage Follow-up";
//			                           		                           	sd.StartPosition = FormStartPosition.CenterParent;
//			                           		                           	sd.ShowDialog();
//			                           		                           });
//
//			                           		thread.SetApartmentState(ApartmentState.STA);
//			                           		thread.Start();
//			                           	}
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,true,this);
		}
		
		void OutageFollowUp() {
			string[] outageSites = (currentOutage.VFbulkCI + currentOutage.TFbulkCI).Split(';');
			outageSites = outageSites.Distinct().ToArray(); // Remover duplicados
			outageSites = outageSites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
			Thread thread = new Thread(() => {
			                           	SiteFinder.UI.siteDetails sd = new SiteFinder.UI.siteDetails(true,outageSites);
			                           	sd.Name = "Outage Follow-up";
			                           	sd.StartPosition = FormStartPosition.CenterParent;
			                           	sd.ShowDialog();
			                           });
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		void VFTFReportTabControlSelectedIndexChanged(object sender, EventArgs e)
		{
			if(tabControl4.SelectedIndex == 0) {
				textBox10.Text = currentOutage.VFoutage;
				textBox11.Text = currentOutage.VFbulkCI;
			}
			else {
				textBox10.Text = currentOutage.TFoutage;
				textBox11.Text = currentOutage.TFbulkCI;
			}
			textBox10.Select(0,0);
			textBox11.Select(0,0);
		}

		void GenerateFromSitesList(object sender, EventArgs e)
		{
//			if (string.IsNullOrEmpty(textBox10.Text)) {
			////				Action action = new Action(delegate {
//				MessageBox.Show("Please insert sites list.\n\nTIP: write 1 site PER LINE", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
			////				                           });
			////				Toolbox.Tools.darkenBackgroundForm(action,false,this);
//				return;
//			}
//
//			Site foundSite = null;
			////			DataView foundCells = null;
//			List<string[]> VFsitesArrayList = new List<string[]>();
//			List<string[]> TFsitesArrayList = new List<string[]>();
//			List<string[]> VFlocationsArrayList = new List<string[]>();
//			List<string[]> TFlocationsArrayList = new List<string[]>();
//			List<string> VFcells2G = new List<string>();
//			List<string> VFcells3G = new List<string>();
//			List<string> VFcells4G = new List<string>();
//			List<string> TFcells2G = new List<string>();
//			List<string> TFcells3G = new List<string>();
//			List<string> TFcells4G = new List<string>();
//
//			sites = textBox10.Text.Split('\n');
//			sites = sites.Where(x => !string.IsNullOrEmpty(x)).ToArray();
//			foreach(string site in sites) {
//				foundSite = Finder.getSite(site);
//
//				List<string> VFcells2Gtemp = new List<string>();
//				List<string> VFcells3Gtemp = new List<string>();
//				List<string> VFcells4Gtemp = new List<string>();
//				List<string> TFcells2Gtemp = new List<string>();
//				List<string> TFcells3Gtemp = new List<string>();
//				List<string> TFcells4Gtemp = new List<string>();
//
//				if(foundCells.Count > 0) {
//					bool siteHasVF2G = false;
//					bool siteHasVF3G = false;
//					bool siteHasVF4G = false;
//					bool siteHasTF2G = false;
//					bool siteHasTF3G = false;
//					bool siteHasTF4G = false;
//					foundCells.RowFilter = "BEARER = '2G' AND (CELL_NAME NOT LIKE 'T*' AND CELL_NAME NOT LIKE '*W' AND CELL_NAME NOT LIKE '*X' AND CELL_NAME NOT LIKE '*Y')";
//					foreach (DataRowView cell in foundCells) {
//						VFcells2Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
//						siteHasVF2G = true;
//					}
//					foundCells.RowFilter = "BEARER = '2G' AND (CELL_NAME LIKE 'T*' OR CELL_NAME LIKE '*W' OR CELL_NAME LIKE '*X' OR CELL_NAME LIKE '*Y')";
//					foreach (DataRowView cell in foundCells) {
//						TFcells2Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
//						siteHasTF2G = true;
//					}
//					foundCells.RowFilter = "BEARER = '3G' AND CELL_NAME NOT LIKE 'T*'";
//					foreach (DataRowView cell in foundCells) {
//						VFcells3Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
//						siteHasVF3G = true;
//					}
//					foundCells.RowFilter = "BEARER = '3G' AND CELL_NAME LIKE 'T*'";
//					foreach (DataRowView cell in foundCells) {
//						TFcells3Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
//						siteHasTF3G = true;
//					}
//					foundCells.RowFilter = "BEARER = '4G' AND CELL_NAME NOT LIKE 'T*'";
//					foreach (DataRowView cell in foundCells) {
//						VFcells4Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
//						siteHasVF4G = true;
//					}
//					foundCells.RowFilter = "BEARER = '4G' AND CELL_NAME LIKE 'T*'";
//					foreach (DataRowView cell in foundCells) {
//						TFcells4Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
//						siteHasTF4G = true;
//					}
//
//					string[] address = foundSite[foundSite.Row.Table.Columns.IndexOf("ADDRESS")].ToString().Split(';');
//					if(VFcells2Gtemp.Count > 0 || VFcells3Gtemp.Count > 0 || VFcells4Gtemp.Count > 0) {
//						VFsitesArrayList.Add(new string[]{foundSite[foundSite.Row.Table.Columns.IndexOf("SITE")].ToString(),siteHasVF2G.ToString(),siteHasVF3G.ToString(),siteHasVF4G.ToString()});
//						VFlocationsArrayList.Add(new string[]{address[address.Length - 2].Trim(' '),siteHasVF2G.ToString(),siteHasVF3G.ToString(),siteHasVF4G.ToString()});
//						VFcells2G.AddRange(VFcells2Gtemp);
//						VFcells3G.AddRange(VFcells3Gtemp);
//						VFcells4G.AddRange(VFcells4Gtemp);
//					}
//					if(TFcells2Gtemp.Count > 0 || TFcells3Gtemp.Count > 0 || TFcells4Gtemp.Count > 0) {
//						TFsitesArrayList.Add(new string[]{foundSite[foundSite.Row.Table.Columns.IndexOf("SITE")].ToString(),siteHasTF2G.ToString(),siteHasTF3G.ToString(),siteHasTF4G.ToString()});
//						TFlocationsArrayList.Add(new string[]{address[address.Length - 2].Trim(' '),siteHasTF2G.ToString(),siteHasTF3G.ToString(),siteHasTF4G.ToString()});
//						TFcells2G.AddRange(TFcells2Gtemp);
//						TFcells3G.AddRange(TFcells3Gtemp);
//						TFcells4G.AddRange(TFcells4Gtemp);
//					}
//				}
//			}
//
//			for(int c = 0;c < VFsitesArrayList.Count;c++) {
//				while (VFsitesArrayList[c][0].Length < 5) {
//					VFsitesArrayList[c][0] = '0' + VFsitesArrayList[c][0];
//				}
//				VFsitesArrayList[c][0] = "RBS" + VFsitesArrayList[c][0];
//			}
//			for(int c = 0;c < TFsitesArrayList.Count;c++) {
//				while (TFsitesArrayList[c][0].Length < 5) {
//					TFsitesArrayList[c][0] = '0' + TFsitesArrayList[c][0];
//				}
//				TFsitesArrayList[c][0] = "RBS" + TFsitesArrayList[c][0];
//			}
//
//			bool show2G = VFcells2G.Count > 0 || TFcells2G.Count > 0;
//			bool show3G = VFcells3G.Count > 0 || TFcells3G.Count > 0;
//			bool show4G = VFcells4G.Count > 0 || TFcells4G.Count > 0;
//
//			List<string[]> includeList = showIncludeListForm(show2G,show3G,show4G);
//
//			// FIXME: empty report on cancel IncludeListForm
//
//			// Get sites and locations list for VF and TF
//
//			List<string> VFsitesList = new List<string>();
//			List<string> TFsitesList = new List<string>();
//			List<string> VFlocationsList = new List<string>();
//			List<string> TFlocationsList = new List<string>();
//
//			foreach (string[] site in VFsitesArrayList) {
//				if(site[1] == "True" && includeList[0][0] == "True") {
//					VFsitesList.Add(site[0]);
//					continue;
//				}
//				if(site[2] == "True" && includeList[1][0] == "True") {
//					VFsitesList.Add(site[0]);
//					continue;
//				}
//				if(site[3] == "True" && includeList[2][0] == "True") {
//					VFsitesList.Add(site[0]);
//				}
//			}
//			foreach (string[] location in VFlocationsArrayList) {
//				if(location[1] == "True" && includeList[0][0] == "True") {
//					VFlocationsList.Add(location[0]);
//					continue;
//				}
//				if(location[2] == "True" && includeList[1][0] == "True") {
//					VFlocationsList.Add(location[0]);
//					continue;
//				}
//				if(location[3] == "True" && includeList[2][0] == "True") {
//					VFlocationsList.Add(location[0]);
//				}
//			}
//			foreach (string[] site in TFsitesArrayList) {
//				if(site[1] == "True" && includeList[0][0] == "True") {
//					TFsitesList.Add(site[0]);
//					continue;
//				}
//				if(site[2] == "True" && includeList[1][0] == "True") {
//					TFsitesList.Add(site[0]);
//					continue;
//				}
//				if(site[3] == "True" && includeList[2][0] == "True") {
//					TFsitesList.Add(site[0]);
//				}
//			}
//			foreach (string[] location in TFlocationsArrayList) {
//				if(location[1] == "True" && includeList[0][0] == "True") {
//					TFlocationsList.Add(location[0]);
//					continue;
//				}
//				if(location[2] == "True" && includeList[1][0] == "True") {
//					TFlocationsList.Add(location[0]);
//					continue;
//				}
//				if(location[3] == "True" && includeList[2][0] == "True") {
//					TFlocationsList.Add(location[0]);
//				}
//			}
//
//			VFsitesArrayList = null;
//			TFsitesArrayList = null;
//			VFlocationsArrayList = null;
//			TFlocationsArrayList = null;
//
//			VFsitesList = VFsitesList.Distinct().ToList();
//			VFsitesList.Sort();
//			TFsitesList = TFsitesList.Distinct().ToList();
//			TFsitesList.Sort();
//			VFlocationsList = VFlocationsList.Distinct().ToList();
//			VFlocationsList.Sort();
//			TFlocationsList = TFlocationsList.Distinct().ToList();
//			TFlocationsList.Sort();
//			VFcells2G = VFcells2G.Distinct().ToList();
//			VFcells2G.Sort();
//			TFcells2G = TFcells2G.Distinct().ToList();
//			TFcells2G.Sort();
//			VFcells3G = VFcells3G.Distinct().ToList();
//			VFcells3G.Sort();
//			TFcells3G = TFcells3G.Distinct().ToList();
//			TFcells3G.Sort();
//			VFcells4G = VFcells4G.Distinct().ToList();
//			VFcells4G.Sort();
//			TFcells4G = TFcells4G.Distinct().ToList();
//			TFcells4G.Sort();
//
//			// VF Outage
//
//			int totalCells = 0;
//			if(includeList[0][0] == "True")
//				totalCells += VFcells2G.Count;
//			if(includeList[1][0] == "True")
//				totalCells += VFcells3G.Count;
//			if(includeList[2][0] == "True")
//				totalCells += VFcells4G.Count;
//
//			VFoutage = totalCells + "x COOS (" + VFsitesList.Count;
//			if(VFsitesList.Count == 1)
//				VFoutage += " Site)";
//			else
//				VFoutage += " Sites)";
//			VFoutage += Environment.NewLine + Environment.NewLine + "Locations (" + VFlocationsList.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine,VFlocationsList.ToArray()) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine,VFsitesList.ToArray());
//
//			if(VFcells2G.Count > 0 && includeList[0][0] == "True") {
//				VFoutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + VFcells2G.Count + ")";
//				if(includeList[0][0] == "True")
//					VFoutage += " Event Time - " + includeList[0][1] + Environment.NewLine + string.Join(Environment.NewLine,VFcells2G.ToArray());
//			}
//			if(VFcells3G.Count > 0 && includeList[1][0] == "True") {
//				VFoutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + VFcells3G.Count + ")";
//				if(includeList[1][0] == "True")
//					VFoutage += " Event Time - " + includeList[1][1] + Environment.NewLine + string.Join(Environment.NewLine,VFcells3G.ToArray());
//			}
//			if(VFcells4G.Count > 0 && includeList[2][0] == "True") {
//				VFoutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + VFcells4G.Count + ")";
//				if(includeList[2][0] == "True")
//					VFoutage += " Event Time - " + includeList[2][1] + Environment.NewLine + string.Join(Environment.NewLine,VFcells4G.ToArray());
//			}
//
//			VFbulkCI = string.Empty;
//			foreach (string site in VFsitesList) {
//				string tempSite = Convert.ToInt32(site.Remove(0,3)).ToString();
//				if(tempSite.Length < 4) {
//					do {
//						tempSite = '0' + tempSite;
//					} while (tempSite.Length < 4);
//				}
//				VFbulkCI += tempSite + ';';
//			}
//
//			// TF Outage
//
//			totalCells = 0;
//			if(includeList[0][0] == "True")
//				totalCells += TFcells2G.Count;
//			if(includeList[1][0] == "True")
//				totalCells += TFcells3G.Count;
//			if(includeList[2][0] == "True")
//				totalCells += TFcells4G.Count;
//
//			TFoutage = totalCells + "x COOS (" + TFsitesList.Count;
//			TFoutage += TFsitesList.Count == 1 ? " Site)" : " Sites)";
//			TFoutage += Environment.NewLine + Environment.NewLine + "Locations (" + TFlocationsList.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine,TFlocationsList.ToArray()) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine,TFsitesList.ToArray());
//
//			if(TFcells2G.Count > 0 && includeList[0][0] == "True") {
//				TFoutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + TFcells2G.Count + ")";
//				if(includeList[0][0] == "True")
//					TFoutage += " Event Time - " + includeList[0][1] + Environment.NewLine + string.Join(Environment.NewLine,TFcells2G.ToArray());
//			}
//			if(TFcells3G.Count > 0 && includeList[1][0] == "True") {
//				TFoutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + TFcells3G.Count + ")";
//				if(includeList[1][0] == "True")
//					TFoutage += " Event Time - " + includeList[1][1] + Environment.NewLine + string.Join(Environment.NewLine,TFcells3G.ToArray());
//			}
//			if(TFcells4G.Count > 0 && includeList[2][0] == "True") {
//				TFoutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + TFcells4G.Count + ")";
//				if(includeList[2][0] == "True")
//					TFoutage += " Event Time - " + includeList[2][1] + Environment.NewLine + string.Join(Environment.NewLine,TFcells4G.ToArray());
//			}
//			TFbulkCI = string.Empty;
//			foreach (string site in TFsitesList) {
//				string tempSite = Convert.ToInt32(site.Remove(0,3)).ToString();
//				if(tempSite.Length < 4) {
//					do {
//						tempSite = '0' + tempSite;
//					} while (tempSite.Length < 4);
//				}
//				TFbulkCI += tempSite + ';';
//			}
//
//			if(!string.IsNullOrEmpty(VFoutage) && !string.IsNullOrEmpty(TFoutage)) {
//				tabControl4.Visible = true;
//				tabControl4.SelectTab(0);
//			}
//			else {
//				if(string.IsNullOrEmpty(VFoutage) && string.IsNullOrEmpty(TFoutage)) {
//					MainForm.trayIcon.showBalloon("Empty report","No cells were found for the given sites");
//					textBox10.Text = string.Empty;
//					return;
//				}
//				if(!string.IsNullOrEmpty(VFoutage)) {
//					tabControl4.Visible = false;
//					tabControl4.SelectTab(0);
//				}
//				else {
//					if(!string.IsNullOrEmpty(TFoutage)) {
//						tabControl4.Visible = false;
//						tabControl4.SelectTab(1);
//					}
//				}
//			}
//			if(!string.IsNullOrEmpty(VFoutage) || !string.IsNullOrEmpty(TFoutage)) {
//				VFTFReportTabControlSelectedIndexChanged(null,null);
//				//button4.Enabled = false;
//				button4.Text = "Outage Follow Up";
//				button4.Width = 100;
//				//button46.Enabled = false;
//				button46.Visible = false;
//				button3.Enabled = true;
//				textBox10.ReadOnly = true;
//				textBox11.ReadOnly = true;
//				button12.Visible = true;
//				button25.Visible = true;
//				textBox10.Focus();
//				label33.Text = "Generated Outage Report";
			////				LogOutageReport();
//			}
		}

		void IncludeListForm_cbCheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)sender;
			Form form = (Form)cb.Parent;
			
			switch(cb.Name) {
				case "cb2G":
					foreach (Control ctrl in form.Controls) {
						if(ctrl.Name == "dtp2G") {
							ctrl.Visible = cb.Checked;
							break;
						}
					}
					break;
				case "cb3G":
					foreach (Control ctrl in form.Controls) {
						if(ctrl.Name == "dtp3G") {
							ctrl.Visible = cb.Checked;
							break;
						}
					}
					break;
				default:
					foreach (Control ctrl in form.Controls) {
						if(ctrl.Name == "dtp4G") {
							ctrl.Visible = cb.Checked;
							break;
						}
					}
					break;
			}
		}

		List<string[]> showIncludeListForm(bool show2G,bool show3G,bool show4G) {
			List<string[]> includeList = new List<string[]>();
			Form form = new Form();
			using (form) {
				// 
				// cb2G
				// 
				CheckBox cb2G = new CheckBox();
				cb2G.Location = new Point(3, 34);
				cb2G.Name = "cb2G";
				cb2G.Size = new Size(42, 20);
				cb2G.TabIndex = 0;
				cb2G.Text = "2G";
				cb2G.Enabled = show2G;
				cb2G.CheckedChanged += (IncludeListForm_cbCheckedChanged);
				// 
				// cb3G
				// 
				CheckBox cb3G = new CheckBox();
				cb3G.Location = new Point(3, 60);
				cb3G.Name = "cb3G";
				cb3G.Size = new Size(42, 20);
				cb3G.TabIndex = 2;
				cb3G.Text = "3G";
				cb3G.Enabled = show3G;
				cb3G.CheckedChanged += (IncludeListForm_cbCheckedChanged);
				// 
				// cb4G
				// 
				CheckBox cb4G = new CheckBox();
				cb4G.Location = new Point(3, 86);
				cb4G.Name = "cb4G";
				cb4G.Size = new Size(42, 20);
				cb4G.TabIndex = 4;
				cb4G.Text = "4G";
				cb4G.Enabled = show4G;
				cb4G.CheckedChanged += (IncludeListForm_cbCheckedChanged);
				// 
				// continueButton
				// 
				Button continueButton = new Button();
				continueButton.Location = new Point(3, 112);
				continueButton.Name = "continueButton";
				continueButton.Size = new Size(221, 23);
				continueButton.TabIndex = 6;
				continueButton.Text = "Continue";
				continueButton.Click += (IncludeListForm_buttonClick);
				// 
				// dtp2G
				// 
				DateTimePicker dtp2G = new DateTimePicker();
				dtp2G.Checked = false;
				dtp2G.CustomFormat = "dd/MM/yyyy HH:mm";
				dtp2G.Format = DateTimePickerFormat.Custom;
				dtp2G.Location = new Point(51, 34);
				dtp2G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp2G.Name = "dtp2G";
				dtp2G.Size = new Size(173, 20);
				dtp2G.TabIndex = 1;
				dtp2G.Value = DateTime.Now;
				dtp2G.Visible = false;
				// 
				// dtp3G
				// 
				DateTimePicker dtp3G = new DateTimePicker();
				dtp3G.Checked = false;
				dtp3G.CustomFormat = "dd/MM/yyyy HH:mm";
				dtp3G.Format = DateTimePickerFormat.Custom;
				dtp3G.Location = new Point(51, 60);
				dtp3G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp3G.Name = "dtp3G";
				dtp3G.Size = new Size(173, 20);
				dtp3G.TabIndex = 3;
				dtp3G.Value = DateTime.Now;
				dtp3G.Visible = false;
				// 
				// dtp4G
				// 
				DateTimePicker dtp4G = new DateTimePicker();
				dtp4G.Checked = false;
				dtp4G.CustomFormat = "dd/MM/yyyy HH:mm";
				dtp4G.Format = DateTimePickerFormat.Custom;
				dtp4G.Location = new Point(51, 86);
				dtp4G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp4G.Name = "dtp4G";
				dtp4G.Size = new Size(173, 20);
				dtp4G.TabIndex = 5;
				dtp4G.Value = DateTime.Now;
				dtp4G.Visible = false;
				// 
				// IncludeListForm_label
				// 
				Label IncludeListForm_label = new Label();
				IncludeListForm_label.Location = new Point(3, 2);
				IncludeListForm_label.Name = "label";
				IncludeListForm_label.Size = new Size(221, 29);
				IncludeListForm_label.Text = "Which Technologies do you wish to include?";
				IncludeListForm_label.TextAlign = ContentAlignment.MiddleCenter;
				// 
				// Form1
				// 
				form.AutoScaleDimensions = new SizeF(6F, 13F);
				form.AutoScaleMode = AutoScaleMode.Font;
				form.ClientSize = new Size(228, 137);
				form.Icon = Resources.MB_0001_vodafone3;
				form.MaximizeBox = false;
				form.FormBorderStyle = FormBorderStyle.FixedSingle;
				form.Controls.Add(IncludeListForm_label);
				form.Controls.Add(dtp4G);
				form.Controls.Add(dtp3G);
				form.Controls.Add(dtp2G);
				form.Controls.Add(continueButton);
				form.Controls.Add(cb4G);
				form.Controls.Add(cb3G);
				form.Controls.Add(cb2G);
				form.Name = "IncludeListForm";
				form.Text = "Generate Outage Report";
				form.ShowDialog();
				
				includeList.Add(new []{ cb2G.Checked.ToString(),dtp2G.Text });
				includeList.Add(new []{ cb3G.Checked.ToString(),dtp3G.Text });
				includeList.Add(new []{ cb4G.Checked.ToString(),dtp4G.Text });
			}
			return includeList;
		}

		void IncludeListForm_buttonClick(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			Form form = (Form)btn.Parent;
			if(btn.Text == "Cancel")
				form.Controls["sitesList_tb"].Text = string.Empty;
			form.Close();
		}

		void ClearAllControls(object sender, EventArgs e)
		{
//			VFoutage = string.Empty;
//			VFbulkCI = string.Empty;
//			TFoutage = string.Empty;
//			TFbulkCI = string.Empty;
			label33.Text = "Copy Outage alarms from Netcool";
//			button4.Enabled = true; // HACK: Outage Follow up button on clear
			button4.Text = "Generate Report";
			button4.Width = 97;
			button46.Visible = true;
			button3.Enabled = false;
			textBox10.ReadOnly = false;
			textBox10.Text = string.Empty;
			textBox11.Text = string.Empty;
			button12.Visible = false;
			button25.Visible = false;
			tabControl4.Visible = false;
			textBox10.Focus();
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name) {
				case "BulkCITextBox":
					btn = (Button)button23;
					break;
				case "Alarms_ReportTextBox":
					btn = (Button)button22;
					break;
			}
			
			btn.Enabled = !string.IsNullOrEmpty(tb.Text);
		}
		
		void LargeTextButtonsClick(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			Button btn = (Button)sender;
			string lbl = string.Empty;
			TextBoxBase tb = null;
			switch(btn.Name) {
				case "BulkCILargeTextButton":
					tb = (TextBoxBase)textBox11;
					lbl = label32.Text;
					break;
				case "Alarms_ReportLargeTextButton":
					tb = (TextBoxBase)textBox10;
					lbl = label33.Text;
					break;
			}
			
			LargeTextForm enlarge = new LargeTextForm(tb.Text,lbl,false);
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
			tb.Text = enlarge.finaltext;
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void InitializeComponent()
		{
			BackColor = SystemColors.Control;
			Name = "Outages GUI";
			Controls.Add(label33);
			Controls.Add(textBox10);
			Controls.Add(button22);
			Controls.Add(tabControl4);
			Controls.Add(label32);
			Controls.Add(textBox11);
			Controls.Add(button23);
//			Controls.Add(button46);
//			Controls.Add(button25);
//			Controls.Add(button12);
//			Controls.Add(button3);
//			Controls.Add(button4);
			// 
			// Alarms_ReportLabel
			// 
			label33.Location = new Point(PaddingLeftRight, PaddingTopBottom);
			label33.Name = "Alarms_ReportLabel";
			label33.Size = new Size(179, 20);
			label33.TabIndex = 30;
			label33.Text = "Copy Outage alarms from Netcool";
			label33.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// Alarms_ReportTextBox
			// 
			textBox10.DetectUrls = false;
			textBox10.Font = new Font("Courier New", 8.25F);
			textBox10.Location = new Point(PaddingLeftRight, label33.Bottom + 4);
			textBox10.Name = "Alarms_ReportTextBox";
			textBox10.Size = new Size(510, 368);
			textBox10.TabIndex = 1;
			textBox10.Text = "";
			textBox10.WordWrap = false;
			textBox10.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// Alarms_ReportLargeTextButton
			// 
			button22.Enabled = false;
			button22.Size = new Size(24, 20);
			button22.Location = new Point(textBox10.Right - button22.Width, PaddingTopBottom);
			button22.Name = "Alarms_ReportLargeTextButton";
			button22.TabIndex = 2;
			button22.Text = "...";
			button22.UseVisualStyleBackColor = true;
			button22.Click += LargeTextButtonsClick;
			// 
			// VFTFReportTabControl
			// 
			tabControl4.Appearance = TabAppearance.Buttons;
			tabControl4.Controls.Add(tabPage15);
			tabControl4.Controls.Add(tabPage16);
			tabControl4.Size = new Size(127, 24);
			tabControl4.Location = new Point(button22.Left - tabControl4.Width - 5, PaddingTopBottom);
			tabControl4.Name = "VFTFReportTabControl";
			tabControl4.SelectedIndex = 0;
			tabControl4.TabIndex = 31;
			tabControl4.Visible = false;
			tabControl4.SelectedIndexChanged += VFTFReportTabControlSelectedIndexChanged;
			// 
			// VFReportTabPage
			// 
			tabPage15.Location = new Point(0, 25); // 4, 25
			tabPage15.Name = "VFReportTabPage";
			tabPage15.Padding = new Padding(3);
			tabPage15.Size = new Size(119, 0);
			tabPage15.TabIndex = 0;
			tabPage15.Text = "VF Report";
			tabPage15.UseVisualStyleBackColor = true;
			// 
			// TFReportTabPage
			// 
			tabPage16.Location = new Point(0, 25); // 4, 25
			tabPage16.Name = "TFReportTabPage";
			tabPage16.Padding = new Padding(3);
			tabPage16.Size = new Size(119, 0);
			tabPage16.TabIndex = 1;
			tabPage16.Text = "TF Report";
			tabPage16.UseVisualStyleBackColor = true;
			// 
			// BulkCILabel
			// 
			label32.Location = new Point(PaddingLeftRight, textBox10.Bottom + 4);
			label32.Name = "BulkCILabel";
			label32.Size = new Size(198, 20);
			label32.Text = "BulkCI (Divided into 50 sites chunks)";
			label32.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// BulkCITextBox
			// 
			textBox11.DetectUrls = false;
			textBox11.Font = new Font("Courier New", 8.25F);
			textBox11.Location = new Point(PaddingLeftRight, label32.Bottom + 4);
			textBox11.Name = "BulkCITextBox";
			textBox11.ReadOnly = true;
			textBox11.Size = new Size(510, 203);
			textBox11.TabIndex = 3;
			textBox11.Text = "";
			textBox11.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// BulkCILargeTextButton
			// 
			button23.Enabled = false;
			button23.Size = new Size(24, 20);
			button23.Location = new Point(textBox11.Right - button23.Width, label32.Top);
			button23.Name = "BulkCILargeTextButton";
			button23.TabIndex = 4;
			button23.Text = "...";
			button23.UseVisualStyleBackColor = true;
			button23.Click += LargeTextButtonsClick;
			// 
			// button12
			// 
//			button12.Location = new Point(354, 630);
//			button12.Name = "button12";
//			button12.Size = new Size(99, 23);
//			button12.TabIndex = 6;
//			button12.Text = "Copy to Clipboard";
//			button12.UseVisualStyleBackColor = true;
//			button12.Visible = false;
//			button12.Click += Button12Click;
			// 
			// button3
			// 
//			button3.Enabled = false;
//			button3.Location = new Point(459, 630);
//			button3.Name = "button3";
//			button3.Size = new Size(62, 23);
//			button3.TabIndex = 8;
//			button3.Text = "Clear";
//			button3.UseVisualStyleBackColor = true;
//			button3.Click += Button3Click;
			// 
			// button4
			// 
//			button4.Location = new Point(8, 630);
//			button4.Name = "button4";
//			button4.Size = new Size(97, 23);
//			button4.TabIndex = 5;
//			button4.Text = "Generate Report";
//			button4.UseVisualStyleBackColor = true;
//			button4.Click += Button4Click;
			// 
			// button46
			// 
//			button46.Location = new Point(111, 630);
//			button46.Name = "button46";
//			button46.Size = new Size(133, 23);
//			button46.TabIndex = 33;
//			button46.Text = "Generate from sites list";
//			button46.UseVisualStyleBackColor = true;
//			button46.Click += Button46Click;
			// 
			// button25
			// 
//			button25.Location = new Point(250, 630);
//			button25.Name = "button25";
//			button25.Size = new Size(98, 23);
//			button25.TabIndex = 32;
//			button25.Text = "Generate sites list";
//			button25.UseVisualStyleBackColor = true;
//			button25.Visible = false;
//			button25.Click += Button25Click;
			// 
			// Button_OuterLeft
			// 
			Button_OuterLeft.UseVisualStyleBackColor = true;
			// 
			// Button_OuterRight
			// 
			Button_OuterRight.UseVisualStyleBackColor = true;
			// 
			// Button_InnerLeft
			// 
			Button_InnerLeft.UseVisualStyleBackColor = true;
			// 
			// Button_InnerRight
			// 
			Button_InnerRight.UseVisualStyleBackColor = true;
		}
	}
}
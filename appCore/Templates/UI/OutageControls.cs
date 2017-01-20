﻿/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 31-07-2016
 * Time: 00:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using appCore.Netcool;
using appCore.SiteFinder;
using appCore.SiteFinder.UI;
using appCore.Templates.Types;
using appCore.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of OutageControls.
	/// </summary>
	public class OutageControls : Panel
	{
		Button button3 = new Button();
		Button button4 = new Button();
		Button button12 = new Button();
		Button button46 = new Button();
		Button button25 = new Button();
		
		Button button23 = new Button(); // BulkCILargeTextButton
		Button button22 = new Button(); // Alarms_ReportLargeTextButton
		RadioButton radioButton1 = new RadioButton(); // VFReportRadioButton
		RadioButton radioButton2 = new RadioButton(); // TFReportRadioButton
		Label label33 = new Label(); // Alarms_ReportLabel
		Label label32 = new Label(); // BulkCILabel
		AMTRichTextBox textBox11 = new AMTRichTextBox(); // BulkCITextBox
		AMTRichTextBox textBox10 = new AMTRichTextBox(); // Alarms_ReportTextBox

		Outage currentOutage;
		
		AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem generateReportToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateSitesListToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem outageFollowUpToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateFromSitesListToolStripMenuItem = new ToolStripMenuItem();
		
		int paddingLeftRight = 1;
		public int PaddingLeftRight {
			get { return paddingLeftRight; }
			set {
				paddingLeftRight = value;
				DynamicControlsSizesLocations();
			}
		}
		
		int paddingTopBottom = 1;
		public int PaddingTopBottom {
			get { return paddingTopBottom; }
			set {
				paddingTopBottom = value;
				DynamicControlsSizesLocations();
			}
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
					
					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
				}
				else {
					InitializeComponent();
					
					MainMenu.MainMenu.DropDownItems.Add(generateReportToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(generateFromSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
				}
			}
		}
		
		public OutageControls()
		{
			UiMode = Template.UIenum.Template;
		}

		void GenerateReport(object sender, EventArgs e)
		{
			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
			st.Start();
//			Action action = new Action(delegate {
			if (string.IsNullOrEmpty(textBox10.Text)) {
				FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			try {
//				Action actionThreaded = new Action(delegate {
				AlarmsParser alarms = new AlarmsParser(textBox10.Text, false, true);
				currentOutage = alarms.GenerateOutage();
//				                                   });
//				LoadingPanel load = new LoadingPanel();
//				load.Show(actionThreaded, actionNonThreaded, true, this);

				if(!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
					radioButton1.Enabled = radioButton2.Enabled = radioButton1.Checked = true;
				else {
					if(string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage)) {
						MainForm.trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
						textBox10.Text = string.Empty;
						radioButton1.Enabled = radioButton2.Enabled = false;
						return;
					}
					if(!string.IsNullOrEmpty(currentOutage.VfOutage)) {
						radioButton1.Enabled = radioButton1.Checked = true;
						radioButton2.Enabled = false;
					}
					else {
						if(!string.IsNullOrEmpty(currentOutage.TefOutage)) {
							radioButton2.Enabled = radioButton2.Checked = true;
							radioButton1.Enabled = false;
						}
					}
				}
				if(!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage)) {
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
					MainForm.logFile.HandleOutageLog(currentOutage);
				}
			}
			catch {
				MainForm.trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
				return;
			}
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,true,this);
			st.Stop();
			var t = st.Elapsed;
		}
		
		void OutageFollowUp(object sender, EventArgs e) {
			string[] outageSites = (currentOutage.VfBulkCI + currentOutage.TefBulkCI).Split(';');
			for(int c = 0;c < outageSites.Length;c++) {
				if(outageSites[c].StartsWith("0")) {
					while(outageSites[c].StartsWith("0"))
						outageSites[c] = outageSites[c].Substring(1);
				}
				else
					break;
			}
			outageSites = outageSites.Distinct().ToArray(); // Remover duplicados
			outageSites = outageSites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
			
			Thread thread = new Thread(() => {
			                           	siteDetails sd = new siteDetails(true,outageSites);
			                           	sd.Name = "Outage Follow-up";
			                           	sd.StartPosition = FormStartPosition.CenterParent;
			                           	sd.ShowDialog();
			                           });
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		
		void VFTFReportRadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
			if(rb.Checked) {
				if(rb.Text == "VF Report") {
					textBox10.Text = currentOutage.VfOutage;
					textBox11.Text = currentOutage.VfBulkCI;
				}
				else {
					textBox10.Text = currentOutage.TefOutage;
					textBox11.Text = currentOutage.TefBulkCI;
				}
				textBox10.Select(0,0);
				textBox11.Select(0,0);
			}
		}

		void GenerateFromSitesList(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox10.Text)) {
//				Action action = new Action(delegate {
				MessageBox.Show("Please insert sites list.\n\nTIP: write 1 site PER LINE", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
//				                           });
//				Toolbox.Tools.darkenBackgroundForm(action,false,this);
				return;
			}
			currentOutage = new Outage(textBox10.Text.Split('\n').ToList());
			
			if(!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
				radioButton1.Enabled = radioButton2.Enabled = radioButton1.Checked = true;
			else {
				if(string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage)) {
					MainForm.trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
					textBox10.Text = string.Empty;
					radioButton1.Enabled = radioButton2.Enabled = false;
					return;
				}
				if(!string.IsNullOrEmpty(currentOutage.VfOutage)) {
					radioButton1.Enabled = radioButton1.Checked = true;
					radioButton2.Enabled = false;
				}
				else {
					if(!string.IsNullOrEmpty(currentOutage.TefOutage)) {
						radioButton2.Enabled = radioButton2.Checked = true;
						radioButton1.Enabled = false;
					}
				}
			}
			if(!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage)) {
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
				MainForm.logFile.HandleOutageLog(currentOutage);
			}
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
			radioButton1.Enabled = radioButton1.Checked = radioButton2.Enabled = radioButton2.Checked = false;
//			tabControl4.Visible = false;
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
			
			AMTLargeTextForm enlarge = new AMTLargeTextForm(tb.Text,lbl,false);
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
			Controls.Add(MainMenu);
			Controls.Add(label33);
			Controls.Add(textBox10);
			Controls.Add(button22);
//			Controls.Add(tabControl4);
			Controls.Add(label32);
			Controls.Add(textBox11);
			Controls.Add(button23);
			Controls.Add(radioButton1);
			Controls.Add(radioButton2);
//			Controls.Add(button46);
//			Controls.Add(button25);
//			Controls.Add(button12);
//			Controls.Add(button3);
//			Controls.Add(button4);
			// 
			// MainMenu
			// 
			MainMenu.Location = new Point(paddingLeftRight, 0);
			// 
			// generateReportToolStripMenuItem
			// 
			generateReportToolStripMenuItem.Name = "generateReportToolStripMenuItem";
			generateReportToolStripMenuItem.Text = "Generate Report";
			generateReportToolStripMenuItem.Click += GenerateReport;
			// 
			// clearToolStripMenuItem
			// 
			clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			clearToolStripMenuItem.Text = "Clear";
			clearToolStripMenuItem.Click += ClearAllControls;
			// 
			// copyToClipboardToolStripMenuItem
			// 
			copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
			copyToClipboardToolStripMenuItem.Text = "Copy to Clipboard";
			copyToClipboardToolStripMenuItem.Click += delegate {
				Clipboard.SetText(textBox10.Text);
			};
			// 
			// generateSitesListToolStripMenuItem
			// 
			generateSitesListToolStripMenuItem.Name = "generateSitesListToolStripMenuItem";
			generateSitesListToolStripMenuItem.Text = "Generate sites list";
			generateSitesListToolStripMenuItem.Click += GenerateFromSitesList;
			// 
			// generateFromSitesListToolStripMenuItem
			// 
			generateFromSitesListToolStripMenuItem.Name = "generateFromSitesListToolStripMenuItem";
			generateFromSitesListToolStripMenuItem.Text = "Generate Report from sites list";
			generateFromSitesListToolStripMenuItem.Click += GenerateFromSitesList;
			// 
			// outageFollowUpToolStripMenuItem
			// 
			outageFollowUpToolStripMenuItem.Name = "outageFollowUpToolStripMenuItem";
			outageFollowUpToolStripMenuItem.Text = "Outage Follow Up";
			outageFollowUpToolStripMenuItem.Click += OutageFollowUp;
			// 
			// Alarms_ReportLabel
			// 
//			label33.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
//			label33.Size = new Size(179, 20);
			label33.Name = "Alarms_ReportLabel";
			label33.TabIndex = 30;
			label33.Text = "Copy Outage alarms from Netcool";
			label33.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// Alarms_ReportTextBox
			// 
			textBox10.DetectUrls = false;
			textBox10.Font = new Font("Courier New", 8.25F);
//			textBox10.Location = new Point(PaddingLeftRight, label33.Bottom + 4);
//			textBox10.Size = new Size(510, 368);
			textBox10.Name = "Alarms_ReportTextBox";
			textBox10.TabIndex = 1;
			textBox10.Text = "";
			textBox10.WordWrap = false;
			textBox10.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// Alarms_ReportLargeTextButton
			// 
			button22.Enabled = false;
//			button22.Size = new Size(24, 20);
//			button22.Location = new Point(textBox10.Right - button22.Width, MainMenu.Bottom + 4);
			button22.Name = "Alarms_ReportLargeTextButton";
			button22.TabIndex = 2;
			button22.Text = "...";
			button22.UseVisualStyleBackColor = true;
			button22.Click += LargeTextButtonsClick;
			// 
			// radioButton1
			// 
			radioButton1.Appearance = Appearance.Button;
			radioButton1.Enabled = false;
			radioButton1.Name = "VFReportRadioButton";
			radioButton1.TabIndex = 34;
			radioButton1.TabStop = true;
			radioButton1.Text = "VF Report";
			radioButton1.TextAlign = ContentAlignment.MiddleCenter;
			radioButton1.UseVisualStyleBackColor = true;
			radioButton1.CheckedChanged += VFTFReportRadioButtonsCheckedChanged;
			// 
			// radioButton2
			// 
			radioButton2.Appearance = Appearance.Button;
			radioButton2.Enabled = false;
			radioButton2.Name = "TFReportRadioButton";
			radioButton2.TabIndex = 35;
			radioButton2.Text = "TF Report";
			radioButton2.TextAlign = ContentAlignment.MiddleCenter;
			radioButton2.UseVisualStyleBackColor = true;
			radioButton2.CheckedChanged += VFTFReportRadioButtonsCheckedChanged;
			// 
			// VFTFReportTabControl
			// 
//			tabControl4.Appearance = TabAppearance.Buttons;
//			tabControl4.Controls.Add(tabPage15);
//			tabControl4.Controls.Add(tabPage16);
			////			tabControl4.Size = new Size(127, 24);
			////			tabControl4.Location = new Point(button22.Left - tabControl4.Width - 5, MainMenu.Bottom + 4);
//			tabControl4.Name = "VFTFReportTabControl";
//			tabControl4.SelectedIndex = 0;
//			tabControl4.TabIndex = 31;
//			tabControl4.Visible = false;
//			tabControl4.SelectedIndexChanged += VFTFReportTabControlSelectedIndexChanged;
//			// 
//			// VFReportTabPage
//			// 
//			tabPage15.Location = new Point(0, 25); // 4, 25
//			tabPage15.Name = "VFReportTabPage";
//			tabPage15.Padding = new Padding(3);
//			tabPage15.Size = new Size(119, 0);
//			tabPage15.TabIndex = 0;
//			tabPage15.Text = "VF Report";
//			tabPage15.UseVisualStyleBackColor = true;
//			// 
//			// TFReportTabPage
//			// 
//			tabPage16.Location = new Point(0, 25); // 4, 25
//			tabPage16.Size = new Size(119, 0);
//			tabPage16.Name = "TFReportTabPage";
//			tabPage16.Padding = new Padding(3);
//			tabPage16.TabIndex = 1;
//			tabPage16.Text = "TF Report";
//			tabPage16.UseVisualStyleBackColor = true;
			// 
			// BulkCILabel
			// 
//			label32.Location = new Point(PaddingLeftRight, textBox10.Bottom + 4);
//			label32.Size = new Size(198, 20);
			label32.Name = "BulkCILabel";
			label32.Text = "BulkCI (Divided into 50 sites chunks)";
			label32.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// BulkCITextBox
			// 
			textBox11.DetectUrls = false;
			textBox11.Font = new Font("Courier New", 8.25F);
//			textBox11.Location = new Point(PaddingLeftRight, label32.Bottom + 4);
//			textBox11.Size = new Size(510, 203);
			textBox11.Name = "BulkCITextBox";
			textBox11.ReadOnly = true;
			textBox11.TabIndex = 3;
			textBox11.Text = "";
			textBox11.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// BulkCILargeTextButton
			// 
			button23.Enabled = false;
//			button23.Size = new Size(24, 20);
//			button23.Location = new Point(textBox11.Right - button23.Width, label32.Top);
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
//			Button_OuterLeft.UseVisualStyleBackColor = true;
//			// 
//			// Button_OuterRight
//			// 
//			Button_OuterRight.UseVisualStyleBackColor = true;
//			// 
//			// Button_InnerLeft
//			// 
//			Button_InnerLeft.UseVisualStyleBackColor = true;
//			// 
//			// Button_InnerRight
//			// 
//			Button_InnerRight.UseVisualStyleBackColor = true;
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			label33.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			label33.Size = new Size(179, 20);
			
			textBox10.Location = new Point(PaddingLeftRight, label33.Bottom + 4);
			textBox10.Size = new Size(510, 368);
			
			button22.Size = new Size(24, 20);
			button22.Location = new Point(textBox10.Right - button22.Width, MainMenu.Bottom + 4);
			
			radioButton2.Size = new Size(64, 21);
			radioButton2.Location = new Point(button22.Left - radioButton2.Width - 5, MainMenu.Bottom + 4);
			
			radioButton1.Size = new Size(64, 21);
			radioButton1.Location = new Point(radioButton2.Left - radioButton1.Width - 5, MainMenu.Bottom + 4);
			
//			tabControl4.Size = new Size(127, 24);
//			tabControl4.Location = new Point(button22.Left - tabControl4.Width - 5, MainMenu.Bottom + 4);
			
			label32.Location = new Point(PaddingLeftRight, textBox10.Bottom + 4);
			label32.Size = new Size(198, 20);
			
			textBox11.Location = new Point(PaddingLeftRight, label32.Bottom + 4);
			textBox11.Size = new Size(510, 203);
			
			button23.Size = new Size(24, 20);
			button23.Location = new Point(textBox11.Right - button23.Width, label32.Top);
			
			Size = new Size(textBox11.Right + PaddingLeftRight, textBox11.Bottom + PaddingTopBottom);
		}
	}
}
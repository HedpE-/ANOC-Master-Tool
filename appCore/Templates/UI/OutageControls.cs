/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 31-07-2016
 * Time: 00:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using appCore.Netcool;
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
		Button BulkCILargeTextButton = new Button(); // BulkCILargeTextButton
		Button Alarms_ReportLargeTextButton = new Button(); // Alarms_ReportLargeTextButton
		RadioButton VFReportRadioButton = new RadioButton(); // VFReportRadioButton
		RadioButton TFReportRadioButton = new RadioButton(); // TFReportRadioButton
		Label Alarms_ReportLabel = new Label(); // Alarms_ReportLabel
		Label BulkCILabel = new Label(); // BulkCILabel
		AMTRichTextBox BulkCITextBox = new AMTRichTextBox(); // BulkCITextBox
		AMTRichTextBox Alarms_ReportTextBox = new AMTRichTextBox(); // Alarms_ReportTextBox

		public Outage currentOutage;
		
		AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem generateReportToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateSitesListToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem outageFollowUpToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateFromSitesListToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem sitesPerTechToolStripMenuItem = new ToolStripMenuItem();
		
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
		
		UiEnum _uiMode;
		UiEnum UiMode {
			get { return _uiMode; }
			set {
				_uiMode = value;
				if(value == UiEnum.Log) {
					PaddingLeftRight = 7;
					InitializeComponent();
					BulkCITextBox.ReadOnly =
						Alarms_ReportTextBox.ReadOnly = true;
					Alarms_ReportLabel.Text = "Generated Outage Report";
					
					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(sitesPerTechToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
				}
				else {
					InitializeComponent();
					Alarms_ReportLabel.Text = "Copy Outage alarms from Netcool";
					
					MainMenu.MainMenu.DropDownItems.Add(generateReportToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(generateFromSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(sitesPerTechToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
					
					generateReportToolStripMenuItem.Enabled =
						generateFromSitesListToolStripMenuItem.Enabled =
						clearToolStripMenuItem.Enabled =
						copyToClipboardToolStripMenuItem.Enabled =
						generateSitesListToolStripMenuItem.Enabled =
						outageFollowUpToolStripMenuItem.Enabled =
						sitesPerTechToolStripMenuItem.Enabled = false;
				}
			}
		}
		
		public OutageControls()
		{
			UiMode = UiEnum.Template;
		}
		
		public OutageControls(Outage outage, UiEnum uimode = UiEnum.Log)
		{
			UiMode = uimode;
			currentOutage = outage;
			
			if(!string.IsNullOrEmpty(currentOutage.VfOutage))
				VFReportRadioButton.Checked = VFReportRadioButton.Enabled = true;
			else
				TFReportRadioButton.Checked = true;
			
			TFReportRadioButton.Enabled = !string.IsNullOrEmpty(currentOutage.TefOutage);
		}

		void GenerateReport(object sender, EventArgs e)
		{
//			if (string.IsNullOrEmpty(Alarms_ReportTextBox.Text)) {
//				Action actionNonThreaded = new Action(delegate {
//				                                      	FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
//				                                      	return;
//				                                      });
//				LoadingPanel load = new LoadingPanel();
//				load.Show(null, actionNonThreaded, false, this.FindForm());
//			}
			
			bool parsingError = false;
			string textBoxContent = Alarms_ReportTextBox.Text;
			Action actionThreaded = new Action(delegate {
			                                   	try {
			                                   		AlarmsParser alarms = new AlarmsParser(textBoxContent, false, true);
			                                   		currentOutage = alarms.GenerateOutage();
			                                   	}
			                                   	catch(Exception ex) {
			                                   		var m = ex.Message;
			                                   		MainForm.trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
			                                   		parsingError = true;
			                                   	}
			                                   });
			
			Action actionNonThreaded = new Action(delegate {
			                                      	if(!parsingError) {
			                                      		if(!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
			                                      			VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
			                                      		else {
			                                      			if(string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage)) {
			                                      				MainForm.trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
			                                      				Alarms_ReportTextBox.Text = string.Empty;
			                                      				VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = false;
			                                      				return;
			                                      			}
			                                      			if(!string.IsNullOrEmpty(currentOutage.VfOutage)) {
			                                      				VFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
			                                      				TFReportRadioButton.Enabled = false;
			                                      			}
			                                      			else {
			                                      				if(!string.IsNullOrEmpty(currentOutage.TefOutage)) {
			                                      					TFReportRadioButton.Enabled = TFReportRadioButton.Checked = true;
			                                      					VFReportRadioButton.Enabled = false;
			                                      				}
			                                      			}
			                                      		}
			                                      		if(!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage)) {
			                                      			generateReportToolStripMenuItem.Enabled =
			                                      				generateFromSitesListToolStripMenuItem.Enabled = false;
			                                      			Alarms_ReportTextBox.ReadOnly =
			                                      				BulkCITextBox.ReadOnly = true;
			                                      			outageFollowUpToolStripMenuItem.Enabled =
			                                      				copyToClipboardToolStripMenuItem.Enabled =
			                                      				sitesPerTechToolStripMenuItem.Enabled =
			                                      				generateSitesListToolStripMenuItem.Enabled = true;
			                                      			Alarms_ReportTextBox.Focus();
			                                      			Alarms_ReportLabel.Text = "Generated Outage Report";
			                                      			MainForm.logFiles.HandleOutageLog(currentOutage);
			                                      		}
			                                      	}
			                                      });
			LoadingPanel loading = new LoadingPanel();
			loading.ShowAsync(actionThreaded, actionNonThreaded, true, this);
		}
		
		void OutageFollowUp(object sender, EventArgs e) {
			Thread thread = new Thread(() => {
			                           	siteDetails sd = new siteDetails(currentOutage, this);
//			                           	sd.Name = "Outage Follow-up";
			                           	sd.StartPosition = FormStartPosition.CenterParent;
			                           	sd.ShowDialog();
			                           });
			thread.Name = "LogEditor_OutageFollowUp";
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		
		void VFTFReportRadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
			if(rb.Checked) {
				if(rb.Text == "VF Report") {
					Alarms_ReportTextBox.Text = currentOutage.VfOutage;
					BulkCITextBox.Text = currentOutage.VfBulkCI;
				}
				else {
					Alarms_ReportTextBox.Text = currentOutage.TefOutage;
					BulkCITextBox.Text = currentOutage.TefBulkCI;
				}
				
				if(UiMode == UiEnum.Template) {
					Alarms_ReportTextBox.Select(0,0);
					BulkCITextBox.Select(0,0);
				}
			}
		}

		void GenerateFromSitesList(object sender, EventArgs e)
		{
//			Action action;
//			if (string.IsNullOrEmpty(Alarms_ReportTextBox.Text)) {
//				action = new Action(delegate {
//				                    	MessageBox.Show("Please insert sites list.\n\nTIP: write 1 site PER LINE", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
//				                    });
//				Toolbox.Tools.darkenBackgroundForm(action, false, Toolbox.Tools.getParentForm(this));
//				return;
//			}
			bool parsingError = false;
			string textBoxContent = Alarms_ReportTextBox.Text;
			Action actionThreaded = new Action(delegate {
			                                   	try {
			                                   		List<string> input = textBoxContent.Contains(";") ? textBoxContent.Split(';').ToList() : textBoxContent.Split('\n').ToList();
			                                   		
			                                   		for(int c=0;c < input.Count;c++) {
			                                   			input[c] = input[c].Trim();
			                                   			while(input[c].StartsWith("0"))
			                                   				input[c] = input[c].Substring(1);
			                                   		}
			                                   		
			                                   		currentOutage = new Outage(input);
			                                   	}
			                                   	catch(Exception ex) {
			                                   		var m = ex.Message;
			                                   		MainForm.trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
			                                   		parsingError = true;
			                                   	}
			                                   });
			
			Action actionNonThreaded = new Action(delegate {
			                                      	if(!parsingError) {
			                                      		if(!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
			                                      			VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
			                                      		else {
			                                      			if(string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage)) {
			                                      				MainForm.trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
			                                      				Alarms_ReportTextBox.Text = string.Empty;
			                                      				VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = false;
			                                      				return;
			                                      			}
			                                      			if(!string.IsNullOrEmpty(currentOutage.VfOutage)) {
			                                      				VFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
			                                      				TFReportRadioButton.Enabled = false;
			                                      			}
			                                      			else {
			                                      				if(!string.IsNullOrEmpty(currentOutage.TefOutage)) {
			                                      					TFReportRadioButton.Enabled = TFReportRadioButton.Checked = true;
			                                      					VFReportRadioButton.Enabled = false;
			                                      				}
			                                      			}
			                                      		}
			                                      		if(!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage)) {
			                                      			generateFromSitesListToolStripMenuItem.Enabled =
			                                      				generateReportToolStripMenuItem.Enabled = false;
			                                      			Alarms_ReportTextBox.ReadOnly =
			                                      				BulkCITextBox.ReadOnly = true;
			                                      			copyToClipboardToolStripMenuItem.Enabled =
			                                      				generateSitesListToolStripMenuItem.Enabled =
			                                      				outageFollowUpToolStripMenuItem.Enabled = true;
			                                      			Alarms_ReportTextBox.Focus();
			                                      			Alarms_ReportLabel.Text = "Generated Outage Report";
			                                      			MainForm.logFiles.HandleOutageLog(currentOutage);
			                                      		}
			                                      	}
			                                      });
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(actionThreaded, actionNonThreaded, true, this);
//			Toolbox.Tools.darkenBackgroundForm(action, true, Toolbox.Tools.getParentForm(this));
		}

		void ShowSitesPerTech(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	Label gsmLabel = new Label {
			                           		Text = "2G Affected Sites",
			                           		TextAlign = ContentAlignment.BottomLeft,
			                           		Width = 100,
			                           		Location = new Point(5, 5)
			                           	};
			                           	Label umtsLabel = new Label {
			                           		Text = "3G Affected Sites",
			                           		TextAlign = ContentAlignment.BottomLeft,
			                           		Width = 100,
			                           		Location = new Point(gsmLabel.Right + 10, 5)
			                           	};
			                           	Label lteLabel = new Label {
			                           		Text = "4G Affected Sites",
			                           		TextAlign = ContentAlignment.BottomLeft,
			                           		Width = 100,
			                           		Location = new Point(umtsLabel.Right + 10, 5)
			                           	};
			                           	AMTRichTextBox gsmAmtRichTextBox = new AMTRichTextBox {
			                           		ReadOnly = true,
			                           		Size = new Size(100, 220),
			                           		Location = new Point(5, gsmLabel.Bottom + 5)
			                           	};
			                           	AMTRichTextBox umtsAmtRichTextBox = new AMTRichTextBox {
			                           		ReadOnly = true,
			                           		Size = new Size(100, 220),
			                           		Location = new Point(gsmAmtRichTextBox.Right + 10, umtsLabel.Bottom + 5)
			                           	};
			                           	AMTRichTextBox lteAmtRichTextBox = new AMTRichTextBox {
			                           		ReadOnly = true,
			                           		Size = new Size(100, 220),
			                           		Location = new Point(umtsAmtRichTextBox.Right + 10, lteLabel.Bottom + 5)
			                           	};
			                           	CheckBox sitesFullNameCheckBox = new CheckBox {
			                           		Text = "View Sites full ID",
			                           		Width = 150,
			                           		Location = new Point(5, gsmAmtRichTextBox.Bottom + 5)
			                           	};
			                           	sitesFullNameCheckBox.CheckedChanged += (s, a) => {
			                           		if(sitesFullNameCheckBox.Checked) {
			                           			if(VFReportRadioButton.Checked) {
			                           				gsmAmtRichTextBox.Text = currentOutage.VfGsmAffectedSites != null ? string.Join(Environment.NewLine, currentOutage.VfGsmAffectedSites) : string.Empty;
			                           				umtsAmtRichTextBox.Text = currentOutage.VfUmtsAffectedSites != null ? string.Join(Environment.NewLine, currentOutage.VfUmtsAffectedSites) : string.Empty;
			                           				lteAmtRichTextBox.Text = currentOutage.VfLteAffectedSites != null ? string.Join(Environment.NewLine, currentOutage.VfLteAffectedSites) : string.Empty;
			                           			}
			                           			else {
			                           				gsmAmtRichTextBox.Text = currentOutage.TefGsmAffectedSites != null ? string.Join(Environment.NewLine, currentOutage.TefGsmAffectedSites) : string.Empty;
			                           				umtsAmtRichTextBox.Text = currentOutage.TefUmtsAffectedSites != null ? string.Join(Environment.NewLine, currentOutage.TefUmtsAffectedSites) : string.Empty;
			                           				lteAmtRichTextBox.Text = currentOutage.TefLteAffectedSites != null ? string.Join(Environment.NewLine, currentOutage.TefLteAffectedSites) : string.Empty;
			                           			}
			                           		}
			                           		else {
			                           			string[] lines = gsmAmtRichTextBox.Lines;
			                           			for(int c = 0;c < lines.Length;c++) {
			                           				lines[c] = lines[c].RemoveLetters();
			                           				while(lines[c].StartsWith("0"))
			                           					lines[c] = lines[c].Substring(1);
			                           			}
			                           			gsmAmtRichTextBox.Text = string.Join(Environment.NewLine, lines);
			                           			lines = umtsAmtRichTextBox.Lines;
			                           			for(int c = 0;c < lines.Length;c++) {
			                           				lines[c] = lines[c].RemoveLetters();
			                           				while(lines[c].StartsWith("0"))
			                           					lines[c] = lines[c].Substring(1);
			                           			}
			                           			umtsAmtRichTextBox.Text = string.Join(Environment.NewLine, lines);
			                           			lines = lteAmtRichTextBox.Lines;
			                           			for(int c = 0;c < lines.Length;c++) {
			                           				lines[c] = lines[c].RemoveLetters();
			                           				while(lines[c].StartsWith("0"))
			                           					lines[c] = lines[c].Substring(1);
			                           			}
			                           			lteAmtRichTextBox.Text = string.Join(Environment.NewLine, lines);
			                           		}
			                           	};
			                           	sitesFullNameCheckBox.Checked = true;
			                           	
			                           	Form showSitesPerTechForm = new Form {
			                           		Text = (VFReportRadioButton.Checked ? "VF" : "TF") + " Affected Sites Per Tech",
			                           		Name = "showSitesPerTechForm",
			                           		Size = new Size(338, 312),
			                           		MaximizeBox = false,
			                           		FormBorderStyle = FormBorderStyle.FixedDialog
			                           	};
			                           	
			                           	showSitesPerTechForm.Controls.AddRange(new Control[]{
			                           	                                       	gsmLabel,
			                           	                                       	umtsLabel,
			                           	                                       	lteLabel,
			                           	                                       	gsmAmtRichTextBox,
			                           	                                       	umtsAmtRichTextBox,
			                           	                                       	lteAmtRichTextBox,
			                           	                                       	sitesFullNameCheckBox
			                           	                                       });
			                           	
			                           	
			                           	showSitesPerTechForm.ShowDialog();
			                           });
			LoadingPanel load = new LoadingPanel();
			load.Show(action, this);
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

		void GenerateSitesList(object sender, EventArgs e)
		{
			Action actionNonThreaded = new Action(delegate {
			                                      	FlexibleMessageBox.Show("The following site list was copied to the Clipboard:" + Environment.NewLine + Environment.NewLine + currentOutage.SitesList + Environment.NewLine + Environment.NewLine + "This list can be used to enter a bulk site search on Site Lopedia.","List generated",MessageBoxButtons.OK,MessageBoxIcon.Information);
			                                      	Clipboard.SetText(currentOutage.SitesList);
			                                      });
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, actionNonThreaded, false, this.FindForm());
		}

		void ClearAllControls(object sender, EventArgs e)
		{
			currentOutage = null;
			Alarms_ReportLabel.Text = "Copy Outage alarms from Netcool";
			generateReportToolStripMenuItem.Enabled =
				generateFromSitesListToolStripMenuItem.Enabled = true;
			Alarms_ReportTextBox.ReadOnly = false;
			Alarms_ReportTextBox.Text =
				BulkCITextBox.Text = string.Empty;
			copyToClipboardToolStripMenuItem.Enabled =
				generateSitesListToolStripMenuItem.Enabled =
				sitesPerTechToolStripMenuItem.Enabled =
				outageFollowUpToolStripMenuItem.Enabled = false;
			VFReportRadioButton.Enabled =
				VFReportRadioButton.Checked =
				TFReportRadioButton.Enabled =
				TFReportRadioButton.Checked = false;
			Alarms_ReportTextBox.Focus();
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name) {
				case "BulkCITextBox":
					btn = (Button)BulkCILargeTextButton;
					break;
				case "Alarms_ReportTextBox":
					btn = (Button)Alarms_ReportLargeTextButton;
					generateReportToolStripMenuItem.Enabled =
						generateFromSitesListToolStripMenuItem.Enabled =
						clearToolStripMenuItem.Enabled = !string.IsNullOrEmpty(tb.Text);
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
					tb = (TextBoxBase)BulkCITextBox;
					lbl = BulkCILabel.Text;
					break;
				case "Alarms_ReportLargeTextButton":
					tb = (TextBoxBase)Alarms_ReportTextBox;
					lbl = Alarms_ReportLabel.Text;
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
			Controls.Add(Alarms_ReportLabel);
			Controls.Add(Alarms_ReportTextBox);
			Controls.Add(Alarms_ReportLargeTextButton);
			Controls.Add(BulkCILabel);
			Controls.Add(BulkCITextBox);
			Controls.Add(BulkCILargeTextButton);
			Controls.Add(VFReportRadioButton);
			Controls.Add(TFReportRadioButton);
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
			// generateFromSitesListToolStripMenuItem
			// 
			generateFromSitesListToolStripMenuItem.Name = "generateFromSitesListToolStripMenuItem";
			generateFromSitesListToolStripMenuItem.Text = "Generate Report from sites list";
			generateFromSitesListToolStripMenuItem.Click += GenerateFromSitesList;
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
				Clipboard.SetText(Alarms_ReportTextBox.Text);
			};
			// 
			// generateSitesListToolStripMenuItem
			// 
			generateSitesListToolStripMenuItem.Name = "generateSitesListToolStripMenuItem";
			generateSitesListToolStripMenuItem.Text = "Generate sites list";
			generateSitesListToolStripMenuItem.Click += GenerateSitesList;
			// 
			// outageFollowUpToolStripMenuItem
			// 
			outageFollowUpToolStripMenuItem.Name = "outageFollowUpToolStripMenuItem";
			outageFollowUpToolStripMenuItem.Text = "Outage Follow Up";
			outageFollowUpToolStripMenuItem.Click += OutageFollowUp;
			// 
			// sitesPerTechToolStripMenuItem
			// 
			sitesPerTechToolStripMenuItem.Name = "sitesPerTechToolStripMenuItem";
			sitesPerTechToolStripMenuItem.Text = "Show Sites Per Tech...";
			sitesPerTechToolStripMenuItem.Click += ShowSitesPerTech;
			// 
			// Alarms_ReportLabel
			// 
//			Alarms_ReportLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
//			Alarms_ReportLabel.Size = new Size(179, 20);
			Alarms_ReportLabel.Name = "Alarms_ReportLabel";
			Alarms_ReportLabel.TabIndex = 30;
			Alarms_ReportLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// Alarms_ReportTextBox
			// 
			Alarms_ReportTextBox.DetectUrls = false;
			Alarms_ReportTextBox.Font = new Font("Courier New", 8.25F);
//			Alarms_ReportTextBox.Location = new Point(PaddingLeftRight, label33.Bottom + 4);
//			Alarms_ReportTextBox.Size = new Size(510, 368);
			Alarms_ReportTextBox.Name = "Alarms_ReportTextBox";
			Alarms_ReportTextBox.TabIndex = 1;
			Alarms_ReportTextBox.Text = "";
			Alarms_ReportTextBox.WordWrap = false;
			Alarms_ReportTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// Alarms_ReportLargeTextButton
			// 
			Alarms_ReportLargeTextButton.Enabled = false;
//			Alarms_ReportLargeTextButton.Size = new Size(24, 20);
//			Alarms_ReportLargeTextButton.Location = new Point(textBox10.Right - button22.Width, MainMenu.Bottom + 4);
			Alarms_ReportLargeTextButton.Name = "Alarms_ReportLargeTextButton";
			Alarms_ReportLargeTextButton.TabIndex = 2;
			Alarms_ReportLargeTextButton.Text = "...";
			Alarms_ReportLargeTextButton.UseVisualStyleBackColor = true;
			Alarms_ReportLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// VFReportRadioButton
			// 
			VFReportRadioButton.Appearance = Appearance.Button;
			VFReportRadioButton.Enabled = false;
			VFReportRadioButton.Name = "VFReportRadioButton";
			VFReportRadioButton.TabIndex = 34;
			VFReportRadioButton.TabStop = true;
			VFReportRadioButton.Text = "VF Report";
			VFReportRadioButton.TextAlign = ContentAlignment.MiddleCenter;
			VFReportRadioButton.UseVisualStyleBackColor = true;
			VFReportRadioButton.CheckedChanged += VFTFReportRadioButtonsCheckedChanged;
			// 
			// TFReportRadioButton
			// 
			TFReportRadioButton.Appearance = Appearance.Button;
			TFReportRadioButton.Enabled = false;
			TFReportRadioButton.Name = "TFReportRadioButton";
			TFReportRadioButton.TabIndex = 35;
			TFReportRadioButton.Text = "TF Report";
			TFReportRadioButton.TextAlign = ContentAlignment.MiddleCenter;
			TFReportRadioButton.UseVisualStyleBackColor = true;
			TFReportRadioButton.CheckedChanged += VFTFReportRadioButtonsCheckedChanged;
			// 
			// BulkCILabel
			// 
//			BulkCILabel.Location = new Point(PaddingLeftRight, textBox10.Bottom + 4);
//			BulkCILabel.Size = new Size(198, 20);
			BulkCILabel.Name = "BulkCILabel";
			BulkCILabel.Text = "BulkCI (Divided into 50 sites chunks)";
			BulkCILabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// BulkCITextBox
			// 
			BulkCITextBox.DetectUrls = false;
			BulkCITextBox.Font = new Font("Courier New", 8.25F);
//			BulkCITextBox.Location = new Point(PaddingLeftRight, label32.Bottom + 4);
//			BulkCITextBox.Size = new Size(510, 203);
			BulkCITextBox.Name = "BulkCITextBox";
			BulkCITextBox.ReadOnly = true;
			BulkCITextBox.TabIndex = 3;
			BulkCITextBox.Text = "";
			BulkCITextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// BulkCILargeTextButton
			// 
			BulkCILargeTextButton.Enabled = false;
//			BulkCILargeTextButton.Size = new Size(24, 20);
//			BulkCILargeTextButton.Location = new Point(textBox11.Right - button23.Width, label32.Top);
			BulkCILargeTextButton.Name = "BulkCILargeTextButton";
			BulkCILargeTextButton.TabIndex = 4;
			BulkCILargeTextButton.Text = "...";
			BulkCILargeTextButton.UseVisualStyleBackColor = true;
			BulkCILargeTextButton.Click += LargeTextButtonsClick;
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			Alarms_ReportLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			Alarms_ReportLabel.Size = new Size(179, 20);
			
			Alarms_ReportTextBox.Location = new Point(PaddingLeftRight, Alarms_ReportLabel.Bottom + 4);
			Alarms_ReportTextBox.Size = new Size(510, 368);
			
			Alarms_ReportLargeTextButton.Size = new Size(24, 20);
			Alarms_ReportLargeTextButton.Location = new Point(Alarms_ReportTextBox.Right - Alarms_ReportLargeTextButton.Width, MainMenu.Bottom + 4);
			
			TFReportRadioButton.Size = new Size(64, 21);
			TFReportRadioButton.Location = new Point(Alarms_ReportLargeTextButton.Left - TFReportRadioButton.Width - 5, MainMenu.Bottom + 4);
			
			VFReportRadioButton.Size = new Size(64, 21);
			VFReportRadioButton.Location = new Point(TFReportRadioButton.Left - VFReportRadioButton.Width - 5, MainMenu.Bottom + 4);
			
			BulkCILabel.Location = new Point(PaddingLeftRight, Alarms_ReportTextBox.Bottom + 4);
			BulkCILabel.Size = new Size(198, 20);
			
			BulkCITextBox.Location = new Point(PaddingLeftRight, BulkCILabel.Bottom + 4);
			BulkCITextBox.Size = new Size(510, 203);
			
			BulkCILargeTextButton.Size = new Size(24, 20);
			BulkCILargeTextButton.Location = new Point(BulkCITextBox.Right - BulkCILargeTextButton.Width, BulkCILabel.Top);
			
			Size = new Size(BulkCITextBox.Right + PaddingLeftRight, BulkCITextBox.Bottom + PaddingTopBottom);
		}
	}
}
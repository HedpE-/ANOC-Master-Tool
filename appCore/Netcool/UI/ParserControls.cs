/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 13-04-2017
 * Time: 22:17
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
using appCore.Templates.RAN.Types;
using appCore.UI;

namespace appCore.Netcool.UI
{
	/// <summary>
	/// Description of ParserControls.
	/// </summary>
	public class ParserControls : Panel
	{
		Button Alarms_ReportLargeTextButton = new Button(); // Alarms_ReportLargeTextButton
		Label Alarms_ReportLabel = new Label(); // Alarms_ReportLabel
		AMTRichTextBox Alarms_ReportTextBox = new AMTRichTextBox(); // Alarms_ReportTextBox

//		public Outage currentOutage;
		
		AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem generateReportToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
		
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
						Alarms_ReportTextBox.ReadOnly = true;
					Alarms_ReportLabel.Text = "Generated Outage Report";
					
//					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
//					MainMenu.MainMenu.DropDownItems.Add(sitesPerTechToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
//					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
				}
				else {
					InitializeComponent();
					Alarms_ReportLabel.Text = "Copy Outage alarms from Netcool";
					
					MainMenu.MainMenu.DropDownItems.Add(generateReportToolStripMenuItem);
//					MainMenu.MainMenu.DropDownItems.Add(generateFromSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
//					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
//					MainMenu.MainMenu.DropDownItems.Add(sitesPerTechToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
//					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
					
					generateReportToolStripMenuItem.Enabled =
//						generateFromSitesListToolStripMenuItem.Enabled =
						clearToolStripMenuItem.Enabled =
						copyToClipboardToolStripMenuItem.Enabled = false;
//						generateSitesListToolStripMenuItem.Enabled =
//						outageFollowUpToolStripMenuItem.Enabled =
//						sitesPerTechToolStripMenuItem.Enabled = false;
				}
			}
		}
		
		public ParserControls()
		{
			UiMode = UiEnum.Template;
		}
		
		public ParserControls(Outage outage, UiEnum uimode = UiEnum.Log)
		{
			UiMode = uimode;
//			currentOutage = outage;
			
//			if(!string.IsNullOrEmpty(currentOutage.VfOutage))
//				VFReportRadioButton.Checked = VFReportRadioButton.Enabled = true;
//			else
//				TFReportRadioButton.Checked = true;
//			
//			TFReportRadioButton.Enabled = !string.IsNullOrEmpty(currentOutage.TefOutage);
		}

		async void GenerateReport(object sender, EventArgs e)
		{
            //			if (string.IsNullOrEmpty(Alarms_ReportTextBox.Text)) {
            //				Action actionNonThreaded = new Action(delegate {
            //				                                      	FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //				                                      	return;
            //				                                      });
            //				LoadingPanel load = new LoadingPanel();
            //				load.Show(null, actionNonThreaded, false, this.FindForm());
            //			}
            LoadingPanel loading = new LoadingPanel();
            loading.Show(false, this);

            bool parsingError = false;
			string textBoxContent = Alarms_ReportTextBox.Text;

            await System.Threading.Tasks.Task.Run(() =>
            {
			    try {
			        AlarmsParser alarms = new AlarmsParser(textBoxContent, AlarmsParser.ParsingMode.Outage, false);
//			                                   		currentOutage = alarms.GenerateOutage();
			    }
			    catch(Exception ex) {
			        var m = ex.Message;
			        parsingError = true;
			    }
			});
			
			if(parsingError)
                MainForm.trayIcon.showBalloon("Error parsing alarms", "An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
            else
            {
                loading.ToggleLoadingSpinner();
//			                                      		if(!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
//			                                      			VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
//			                                      		else {
//			                                      			if(string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage)) {
//			                                      				MainForm.trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
//			                                      				Alarms_ReportTextBox.Text = string.Empty;
//			                                      				VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = false;
//			                                      				return;
//			                                      			}
//			                                      			if(!string.IsNullOrEmpty(currentOutage.VfOutage)) {
//			                                      				VFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
//			                                      				TFReportRadioButton.Enabled = false;
//			                                      			}
//			                                      			else {
//			                                      				if(!string.IsNullOrEmpty(currentOutage.TefOutage)) {
//			                                      					TFReportRadioButton.Enabled = TFReportRadioButton.Checked = true;
//			                                      					VFReportRadioButton.Enabled = false;
//			                                      				}
//			                                      			}
//			                                      		}
//			                                      		if(!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage)) {
//			                                      			generateReportToolStripMenuItem.Enabled = false;
////			                                      				generateFromSitesListToolStripMenuItem.Enabled = false;
//			                                      			Alarms_ReportTextBox.ReadOnly = true;
////			                                      			BulkCITextBox.ReadOnly = true;
////			                                      			outageFollowUpToolStripMenuItem.Enabled =
////			                                      				copyToClipboardToolStripMenuItem.Enabled =
////			                                      				generateSitesListToolStripMenuItem.Enabled = true;
//			                                      			Alarms_ReportTextBox.Focus();
//			                                      			Alarms_ReportLabel.Text = "Generated Outage Report";
//			                                      			MainForm.logFiles.HandleOutageLog(currentOutage);
//			                                      		}
            }

			loading.Close();
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name) {
				case "BulkCITextBox":
//					btn = (Button)BulkCILargeTextButton;
					break;
				case "Alarms_ReportTextBox":
					btn = (Button)Alarms_ReportLargeTextButton;
					generateReportToolStripMenuItem.Enabled =
//						generateFromSitesListToolStripMenuItem.Enabled =
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
//				case "BulkCILargeTextButton":
//					tb = (TextBoxBase)BulkCITextBox;
//					lbl = BulkCILabel.Text;
//					break;
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

		void ClearAllControls(object sender, EventArgs e)
		{
//			currentOutage = null;
			Alarms_ReportLabel.Text = "Copy alarms from Netcool";
			generateReportToolStripMenuItem.Enabled = true;
			Alarms_ReportTextBox.ReadOnly = false;
			Alarms_ReportTextBox.Text =  string.Empty;
			copyToClipboardToolStripMenuItem.Enabled = false;
			Alarms_ReportTextBox.Focus();
		}
		
		void InitializeComponent()
		{
			BackColor = SystemColors.Control;
			Name = "Netcool Parser GUI";
			Controls.Add(MainMenu);
			Controls.Add(Alarms_ReportLabel);
			Controls.Add(Alarms_ReportTextBox);
			Controls.Add(Alarms_ReportLargeTextButton);
//			Controls.Add(BulkCILabel);
//			Controls.Add(BulkCITextBox);
//			Controls.Add(BulkCILargeTextButton);
//			Controls.Add(VFReportRadioButton);
//			Controls.Add(TFReportRadioButton);
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
				Clipboard.SetText(Alarms_ReportTextBox.Text);
			};
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
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			Alarms_ReportLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			Alarms_ReportLabel.Size = new Size(179, 20);
			
			Alarms_ReportTextBox.Location = new Point(PaddingLeftRight, Alarms_ReportLabel.Bottom + 4);
			Alarms_ReportTextBox.Size = new Size(510, 368);
			
			Alarms_ReportLargeTextButton.Size = new Size(24, 20);
			Alarms_ReportLargeTextButton.Location = new Point(Alarms_ReportTextBox.Right - Alarms_ReportLargeTextButton.Width, MainMenu.Bottom + 4);
			
//			Size = new Size(BulkCITextBox.Right + PaddingLeftRight, BulkCITextBox.Bottom + PaddingTopBottom);
		}
	}
}
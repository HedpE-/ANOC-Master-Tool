﻿/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 31-07-2016
 * Time: 00:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using appCore.Templates.RAN.Types;
using appCore.UI;

namespace appCore.Templates.RAN.UI
{
	/// <summary>
	/// Description of TxControls.
	/// </summary>
	public class TxControls : Templates.UI.TemplateControlsBase
	{
		Button SitesLargeTextButton = new Button();
		Button DetailedRanTroubleshootLargeTextButton = new Button();
		Button PerformanceOutageDetailsLargeTextButton = new Button();
		Label IpRanPortConfigLabel = new Label();
		Label TxTypeLabel = new Label();
		Label DetailedRanTroubleshootLabel = new Label();
		Label PerformanceOutageDetailsLabel = new Label();
		Label ServiceAffectedLabel = new Label();
		Label SitesLabel = new Label();
		public AMTRichTextBox SitesTextBox = new AMTRichTextBox();
		public AMTRichTextBox DetailedRanTroubleshootTextBox = new AMTRichTextBox();
		public AMTRichTextBox PerformanceOutageDetailsTextBox = new AMTRichTextBox();
		public AMTTextBox IpRanPortConfigTextBox = new AMTTextBox();
		public CheckBox Repeat_IntermittentCheckBox = new CheckBox();
		public ComboBox ServiceAffectedComboBox = new ComboBox();
		public ComboBox TxTypeComboBox = new ComboBox();

        //ErrorProviderFixed errorProvider = new ErrorProviderFixed();

        //AMTMenuStrip MainMenu = new AMTMenuStrip();
		//ToolStripMenuItem generateTemplateToolStripMenuItem = new ToolStripMenuItem();
		//ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		//ToolStripMenuItem copyToNewTemplateToolStripMenuItem = new ToolStripMenuItem();

        //TX currentTemplate;
        //TX prevTemp = new TX();
        //int paddingLeftRight = 1;
        //public int PaddingLeftRight
        //      {
        //	get
        //          {
        //              return paddingLeftRight;
        //          }
        //	set
        //          {
        //		paddingLeftRight = value;
        //		DynamicControlsSizesLocations();
        //	}
        //}

        //int paddingTopBottom = 1;
        //public int PaddingTopBottom
        //      {
        //	get
        //          {
        //              return paddingTopBottom;
        //          }
        //	set
        //          {
        //		paddingTopBottom = value;
        //		DynamicControlsSizesLocations();
        //	}
        //}

        //UiEnum _uiMode;
        protected override UiEnum UiMode
        {
			get
            {
                return _uiMode;
            }
			set
            {
				_uiMode = value;
				if(value == UiEnum.Log)
                {
					PaddingLeftRight = 7;
					InitializeComponent();
					SitesTextBox.ReadOnly = true;
					DetailedRanTroubleshootTextBox.ReadOnly = true;
					PerformanceOutageDetailsTextBox.ReadOnly = true;
					IpRanPortConfigTextBox.ReadOnly = true;
					Repeat_IntermittentCheckBox.Enabled = false;
					ServiceAffectedComboBox.Enabled = false;
					TxTypeComboBox.Enabled = false;
					
					MainMenu.MainMenu.DropDownItems.AddRange(new ToolStripItem[] {
					                                         	generateTemplateToolStripMenuItem,
					                                         	copyToNewTemplateToolStripMenuItem});
				}
				else {
					InitializeComponent();
                    errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                    errorProvider.SetIconPadding(SitesTextBox, -17);
                    errorProvider.SetIconPadding(TxTypeComboBox, -17);
                    errorProvider.SetIconPadding(IpRanPortConfigTextBox, -17);
                    errorProvider.SetIconPadding(ServiceAffectedComboBox, -17);
                    errorProvider.SetIconPadding(PerformanceOutageDetailsTextBox, -17);
                    errorProvider.SetIconPadding(DetailedRanTroubleshootTextBox, -17);

                    TxTypeComboBox.SelectedIndexChanged += TxTypeComboBoxSelectedIndexChanged;
					
					MainMenu.MainMenu.DropDownItems.Add(generateTemplateToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
				}
			}
		}
		
		public TxControls()
		{
			UiMode = UiEnum.Template;
		}
		
		public TxControls(TX template, UiEnum uimode = UiEnum.Log)
		{
			UiMode = uimode;
			currentTemplate = template;
			
			SitesTextBox.Text = ((TX)currentTemplate).SiteIDs;
			ServiceAffectedComboBox.Text = ((TX)currentTemplate).ServiceAffected;
			Repeat_IntermittentCheckBox.Checked = ((TX)currentTemplate).Repeat_Intermittent;
			TxTypeComboBox.Text = ((TX)currentTemplate).TxType;
			IpRanPortConfigTextBox.Text = ((TX)currentTemplate).IpRanPortConfig;
			PerformanceOutageDetailsTextBox.Text = ((TX)currentTemplate).PerformanceOutageDetails;
			DetailedRanTroubleshootTextBox.Text = ((TX)currentTemplate).DetailedRanTroubleshoot;
		}

        protected override void GenerateTemplate(object sender, EventArgs e)
		{
			if(UiMode == UiEnum.Template)
            {
                errorProvider.SetError(SitesTextBox, string.Empty);
                errorProvider.SetError(TxTypeComboBox, string.Empty);
                errorProvider.SetError(IpRanPortConfigTextBox, string.Empty);
                errorProvider.SetError(ServiceAffectedComboBox, string.Empty);
                errorProvider.SetError(PerformanceOutageDetailsTextBox, string.Empty);
                errorProvider.SetError(DetailedRanTroubleshootTextBox, string.Empty);

                bool error = false;

                if (string.IsNullOrEmpty(SitesTextBox.Text))
                {
                    errorProvider.SetError(SitesTextBox, "Site(s) reference(s) missing");
                    error = true;
                }
				if (string.IsNullOrEmpty(TxTypeComboBox.Text))
                {
                    errorProvider.SetError(TxTypeComboBox, "TX type missing");
                    error = true;
                }
				else
                {
					if (TxTypeComboBox.Text == "IPRAN")
                    {
						if (string.IsNullOrEmpty(IpRanPortConfigTextBox.Text))
                        {
                            errorProvider.SetError(IpRanPortConfigTextBox, "IPRAN port configuration missing");
                            error = true;
                        }
					}
				}
				if (string.IsNullOrEmpty(ServiceAffectedComboBox.Text))
                {
                    errorProvider.SetError(ServiceAffectedComboBox, "Service affected missing");
                    error = true;
                }
				if (string.IsNullOrEmpty(PerformanceOutageDetailsTextBox.Text))
                {
                    errorProvider.SetError(PerformanceOutageDetailsTextBox, "Performance / Outage detailed issue missing");
                    error = true;
                }
				if (string.IsNullOrEmpty(DetailedRanTroubleshootTextBox.Text))
                {
                    errorProvider.SetError(DetailedRanTroubleshootTextBox, "Detailed RAN troubleshooting missing");
                    error = true;
                }
                if (error)
                {
                    MainForm.trayIcon.showBalloon("Template generation errors", "Place the mouse over the error icon(s) for more info");
                    return;
                }
            }
			
			currentTemplate = new TX(Controls);
			
			try
            {
				Clipboard.SetText(currentTemplate.ToString());
			}
			catch
            {
				try
                {
					Clipboard.SetText(currentTemplate.ToString());
				}
				catch
                {
					FlexibleMessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				}
			}
			
			FlexibleMessageBox.Show(currentTemplate.ToString(), "Template copied to Clipboard", MessageBoxButtons.OK);
			
			if(UiMode == UiEnum.Template)
            {
				// Store this template for future warning on no changes
				
				previousTemplate = currentTemplate;
				
				MainForm.logFiles.HandleLog(currentTemplate);
			}
		}

		void TxTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (TxTypeComboBox.SelectedIndex == 0)
				IpRanPortConfigTextBox.ReadOnly = false;
			else
            {
				IpRanPortConfigTextBox.ReadOnly = true;
				IpRanPortConfigTextBox.Text = string.Empty;
			}
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name)
            {
				case "SitesTextBox":
					btn = SitesLargeTextButton;
					break;
				case "DetailedRanTroubleshootTextBox":
					btn = DetailedRanTroubleshootLargeTextButton;
					break;
				case "PerformanceOutageDetailsTextBox":
					btn = PerformanceOutageDetailsLargeTextButton;
					break;
			}
			
			btn.Enabled = !string.IsNullOrEmpty(tb.Text);
		}
		
		void LargeTextButtonsClick(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			string lbl = string.Empty;
			TextBoxBase tb = null;
			switch(btn.Name)
            {
				case "SitesLargeTextButton":
					tb = (TextBoxBase)SitesTextBox;
					lbl = SitesLabel.Text;
					break;
				case "DetailedRanTroubleshootLargeTextButton":
					tb = (TextBoxBase)DetailedRanTroubleshootTextBox;
					lbl = DetailedRanTroubleshootLabel.Text;
					break;
				case "PerformanceOutageDetailsLargeTextButton":
					tb = (TextBoxBase)PerformanceOutageDetailsTextBox;
					lbl = PerformanceOutageDetailsLabel.Text;
					break;
			}
			
			AMTLargeTextForm enlarge = new AMTLargeTextForm(tb.Text,lbl,false);
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
			tb.Text = enlarge.finaltext;
		}

        protected override void ClearAllControls(object sender, EventArgs e)
		{
			SitesTextBox.Text = string.Empty;
			ServiceAffectedComboBox.Text = string.Empty;
			Repeat_IntermittentCheckBox.Checked = false;
			TxTypeComboBox.Text = string.Empty;
			IpRanPortConfigTextBox.Text = string.Empty;
			PerformanceOutageDetailsTextBox.Text = string.Empty;
			DetailedRanTroubleshootTextBox.Text = string.Empty;
			IpRanPortConfigTextBox.ReadOnly = true;
			IpRanPortConfigTextBox.Text = string.Empty;
		}

        protected override void InitializeComponent()
		{
			BackColor = SystemColors.Control;
			Name = "TX Template GUI";
			Controls.Add(MainMenu);
			Controls.Add(SitesLargeTextButton);
			Controls.Add(DetailedRanTroubleshootLargeTextButton);
			Controls.Add(PerformanceOutageDetailsLargeTextButton);
			Controls.Add(SitesTextBox);
			Controls.Add(IpRanPortConfigLabel);
			Controls.Add(IpRanPortConfigTextBox);
			Controls.Add(TxTypeLabel);
			Controls.Add(TxTypeComboBox);
			Controls.Add(DetailedRanTroubleshootTextBox);
			Controls.Add(DetailedRanTroubleshootLabel);
			Controls.Add(PerformanceOutageDetailsTextBox);
			Controls.Add(PerformanceOutageDetailsLabel);
			Controls.Add(ServiceAffectedLabel);
			Controls.Add(SitesLabel);
			Controls.Add(Repeat_IntermittentCheckBox);
			Controls.Add(ServiceAffectedComboBox);
			// 
			// MainMenu
			// 
			MainMenu.Location = new Point(paddingLeftRight, 0);
			// 
			// generateTemplateToolStripMenuItem
			// 
			generateTemplateToolStripMenuItem.Name = "generateTemplateToolStripMenuItem";
			generateTemplateToolStripMenuItem.Text = "Generate Template";
			generateTemplateToolStripMenuItem.Click += GenerateTemplate;
			// 
			// clearToolStripMenuItem
			// 
			clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			clearToolStripMenuItem.Text = "Clear template";
			clearToolStripMenuItem.Click += ClearAllControls;
			// 
			// copyToNewTemplateToolStripMenuItem
			// 
			copyToNewTemplateToolStripMenuItem.Name = "copyToNewTemplateToolStripMenuItem";
			copyToNewTemplateToolStripMenuItem.Text = "Copy to new TX template";
			copyToNewTemplateToolStripMenuItem.Click += LoadTemplateFromLog;
			// 
			// SitesLabel
			// 
//			SitesLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
//			SitesLabel.Size = new Size(245, 20);
			SitesLabel.Name = "SitesLabel";
			SitesLabel.Text = "Site(s) reference(s)";
			SitesLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SitesTextBox
			// 
			SitesTextBox.Font = new Font("Courier New", 8.25F);
//			SitesTextBox.Location = new Point(PaddingLeftRight, SitesLabel.Bottom + 4);
//			SitesTextBox.Size = new Size(300, 60);
			SitesTextBox.Name = "SitesTextBox";
			SitesTextBox.TabIndex = 1;
			SitesTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// SitesLargeTextButton
			// 
			SitesLargeTextButton.Enabled = false;
//			SitesLargeTextButton.Size = new Size(24, 20);
//			SitesLargeTextButton.Location = new Point(SitesTextBox.Right + 2, SitesTextBox.Top);
			SitesLargeTextButton.Name = "SitesLargeTextButton";
			SitesLargeTextButton.TabIndex = 2;
			SitesLargeTextButton.Text = "...";
			SitesLargeTextButton.Click += LargeTextButtonsClick;
			SitesLargeTextButton.UseVisualStyleBackColor = true;
			// 
			// ServiceAffectedLabel
			// 
			ServiceAffectedLabel.Name = "ServiceAffectedLabel";
//			ServiceAffectedLabel.Size = new Size(100, 20);
//			ServiceAffectedLabel.Location = new Point(SitesTextBox.Right + 48, SitesTextBox.Top);
			ServiceAffectedLabel.Text = "Service Affected";
			ServiceAffectedLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ServiceAffectedComboBox
			// 
//			ServiceAffectedComboBox.Size = new Size(58, 20);
//			ServiceAffectedComboBox.Location = new Point(ServiceAffectedLabel.Right + 2, SitesTextBox.Top);
			ServiceAffectedComboBox.FormattingEnabled = true;
			ServiceAffectedComboBox.Items.AddRange(new object[] {
			                                       	"Yes",
			                                       	"No"});
			ServiceAffectedComboBox.Name = "ServiceAffectedComboBox";
			ServiceAffectedComboBox.TabIndex = 2;
			// 
			// Repeat_IntermittentCheckBox
			// 
//			Repeat_IntermittentCheckBox.Size = new Size(163, 24);
//			Repeat_IntermittentCheckBox.Location = new Point(SitesTextBox.Right + 46, SitesTextBox.Bottom - Repeat_IntermittentCheckBox.Height);
			Repeat_IntermittentCheckBox.Name = "Repeat_IntermittentCheckBox";
			Repeat_IntermittentCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			Repeat_IntermittentCheckBox.TabIndex = 3;
			Repeat_IntermittentCheckBox.Text = "Repeat/Intermittent";
			Repeat_IntermittentCheckBox.TextAlign = ContentAlignment.MiddleRight;
			Repeat_IntermittentCheckBox.UseVisualStyleBackColor = true;
			// 
			// TxTypeLabel
			// 
//			TxTypeLabel.Location = new Point(PaddingLeftRight, SitesTextBox.Bottom + 4);
//			TxTypeLabel.Size = new Size(58, 20);
			TxTypeLabel.Name = "TxTypeLabel";
			TxTypeLabel.Text = "TX type";
			TxTypeLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// TxTypeComboBox
			// 
			TxTypeComboBox.FormattingEnabled = true;
			TxTypeComboBox.Items.AddRange(new object[] {
			                              	"IPRAN",
			                              	"TDM"});
//			TxTypeComboBox.Location = new Point(TxTypeLabel.Right + 2, TxTypeLabel.Top);
//			TxTypeComboBox.Size = new Size(90, 21);
			TxTypeComboBox.Name = "TxTypeComboBox";
			TxTypeComboBox.TabIndex = 4;
			// 
			// IpRanPortConfigLabel
			// 
//			IpRanPortConfigLabel.Location = new Point(TxTypeComboBox.Right + 10, TxTypeLabel.Top);
//			IpRanPortConfigLabel.Size = new Size(130, 20);
			IpRanPortConfigLabel.Name = "IpRanPortConfigLabel";
			IpRanPortConfigLabel.Text = "IPRAN port configuration";
			IpRanPortConfigLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// IpRanPortConfigTextBox
			// 
			IpRanPortConfigTextBox.ReadOnly = true;
			IpRanPortConfigTextBox.Font = new Font("Courier New", 8.25F);
//			IpRanPortConfigTextBox.Location = new Point(IpRanPortConfigLabel.Right + 2, IpRanPortConfigLabel.Top);
//			IpRanPortConfigTextBox.Size = new Size(218, 20);
			IpRanPortConfigTextBox.Name = "IpRanPortConfigTextBox";
			IpRanPortConfigTextBox.TabIndex = 5;
			// 
			// PerformanceOutageDetailsLabel
			// 
//			PerformanceOutageDetailsLabel.Location = new Point(PaddingLeftRight, TxTypeLabel.Bottom + 4);
//			PerformanceOutageDetailsLabel.Size = new Size(245, 20);
			PerformanceOutageDetailsLabel.Name = "PerformanceOutageDetailsLabel";
			PerformanceOutageDetailsLabel.Text = "Performance/Outage detailed issue";
			PerformanceOutageDetailsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PerformanceOutageDetailsTextBox
			// 
			PerformanceOutageDetailsTextBox.DetectUrls = false;
			PerformanceOutageDetailsTextBox.Font = new Font("Courier New", 8.25F);
//			PerformanceOutageDetailsTextBox.Location = new Point(PaddingLeftRight, PerformanceOutageDetailsLabel.Bottom + 4);
//			PerformanceOutageDetailsTextBox.Size = new Size(510, 212);
			PerformanceOutageDetailsTextBox.Name = "PerformanceOutageDetailsTextBox";
			PerformanceOutageDetailsTextBox.TabIndex = 6;
			PerformanceOutageDetailsTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// PerformanceOutageDetailsLargeTextButton
			// 
			PerformanceOutageDetailsLargeTextButton.Enabled = false;
//			PerformanceOutageDetailsLargeTextButton.Size = new Size(24, 20);
//			PerformanceOutageDetailsLargeTextButton.Location = new Point(PerformanceOutageDetailsTextBox.Right - PerformanceOutageDetailsLargeTextButton.Width, PerformanceOutageDetailsLabel.Top);
			PerformanceOutageDetailsLargeTextButton.Name = "PerformanceOutageDetailsLargeTextButton";
			PerformanceOutageDetailsLargeTextButton.TabIndex = 7;
			PerformanceOutageDetailsLargeTextButton.Text = "...";
			PerformanceOutageDetailsLargeTextButton.Click += LargeTextButtonsClick;
			PerformanceOutageDetailsLargeTextButton.UseVisualStyleBackColor = true;
			// 
			// DetailedRanTroubleshootLabel
			// 
//			DetailedRanTroubleshootLabel.Location = new Point(PaddingLeftRight, PerformanceOutageDetailsTextBox.Bottom + 4);
//			DetailedRanTroubleshootLabel.Size = new Size(509, 20);
			DetailedRanTroubleshootLabel.Name = "DetailedRanTroubleshootLabel";
			DetailedRanTroubleshootLabel.Text = "Detailed RAN troubleshooting (RAN findings, HW replaced, Field visits troubleshooting)";
			DetailedRanTroubleshootLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// DetailedRanTroubleshootTextBox
			// 
			DetailedRanTroubleshootTextBox.DetectUrls = false;
			DetailedRanTroubleshootTextBox.Font = new Font("Courier New", 8.25F);
//			DetailedRanTroubleshootTextBox.Location = new Point(PaddingLeftRight, DetailedRanTroubleshootLabel.Bottom + 4);
//			DetailedRanTroubleshootTextBox.Size = new Size(510, 212);
			DetailedRanTroubleshootTextBox.Name = "DetailedRanTroubleshootTextBox";
			DetailedRanTroubleshootTextBox.TabIndex = 8;
			DetailedRanTroubleshootTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// DetailedRanTroubleshootLargeTextButton
			// 
			DetailedRanTroubleshootLargeTextButton.Enabled = false;
//			DetailedRanTroubleshootLargeTextButton.Size = new Size(24, 20);
//			DetailedRanTroubleshootLargeTextButton.Location = new Point(DetailedRanTroubleshootTextBox.Right - DetailedRanTroubleshootLargeTextButton.Width, DetailedRanTroubleshootLabel.Top);
			DetailedRanTroubleshootLargeTextButton.Name = "DetailedRanTroubleshootLargeTextButton";
			DetailedRanTroubleshootLargeTextButton.TabIndex = 9;
			DetailedRanTroubleshootLargeTextButton.Text = "...";
			DetailedRanTroubleshootLargeTextButton.Click += LargeTextButtonsClick;
			DetailedRanTroubleshootLargeTextButton.UseVisualStyleBackColor = true;
			
			DynamicControlsSizesLocations();
		}

        protected override void DynamicControlsSizesLocations() {
			SitesLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			SitesLabel.Size = new Size(245, 20);
			
			SitesTextBox.Location = new Point(PaddingLeftRight, SitesLabel.Bottom + 4);
			SitesTextBox.Size = new Size(300, 60);
			
			SitesLargeTextButton.Size = new Size(24, 20);
			SitesLargeTextButton.Location = new Point(SitesTextBox.Right + 2, SitesTextBox.Top);
			
			ServiceAffectedLabel.Size = new Size(100, 20);
			ServiceAffectedLabel.Location = new Point(SitesTextBox.Right + 48, SitesTextBox.Top);
			
			ServiceAffectedComboBox.Size = new Size(58, 20);
			ServiceAffectedComboBox.Location = new Point(ServiceAffectedLabel.Right + 2, SitesTextBox.Top);
			
			Repeat_IntermittentCheckBox.Size = new Size(163, 24);
			Repeat_IntermittentCheckBox.Location = new Point(SitesTextBox.Right + 46, SitesTextBox.Bottom - Repeat_IntermittentCheckBox.Height);
			
			TxTypeLabel.Location = new Point(PaddingLeftRight, SitesTextBox.Bottom + 4);
			TxTypeLabel.Size = new Size(58, 20);
			
			TxTypeComboBox.Location = new Point(TxTypeLabel.Right + 2, TxTypeLabel.Top);
			TxTypeComboBox.Size = new Size(90, 21);
			
			IpRanPortConfigLabel.Location = new Point(TxTypeComboBox.Right + 10, TxTypeLabel.Top);
			IpRanPortConfigLabel.Size = new Size(130, 20);
			
			IpRanPortConfigTextBox.Location = new Point(IpRanPortConfigLabel.Right + 2, IpRanPortConfigLabel.Top);
			IpRanPortConfigTextBox.Size = new Size(218, 20);
			
			PerformanceOutageDetailsLabel.Location = new Point(PaddingLeftRight, TxTypeLabel.Bottom + 4);
			PerformanceOutageDetailsLabel.Size = new Size(245, 20);
			
			PerformanceOutageDetailsTextBox.Location = new Point(PaddingLeftRight, PerformanceOutageDetailsLabel.Bottom + 4);
			PerformanceOutageDetailsTextBox.Size = new Size(510, 212);
			
			PerformanceOutageDetailsLargeTextButton.Size = new Size(24, 20);
			PerformanceOutageDetailsLargeTextButton.Location = new Point(PerformanceOutageDetailsTextBox.Right - PerformanceOutageDetailsLargeTextButton.Width, PerformanceOutageDetailsLabel.Top);
			
			DetailedRanTroubleshootLabel.Location = new Point(PaddingLeftRight, PerformanceOutageDetailsTextBox.Bottom + 4);
			DetailedRanTroubleshootLabel.Size = new Size(509, 20);
			
			DetailedRanTroubleshootTextBox.Location = new Point(PaddingLeftRight, DetailedRanTroubleshootLabel.Bottom + 4);
			DetailedRanTroubleshootTextBox.Size = new Size(510, 212);
			
			DetailedRanTroubleshootLargeTextButton.Size = new Size(24, 20);
			DetailedRanTroubleshootLargeTextButton.Location = new Point(DetailedRanTroubleshootTextBox.Right - DetailedRanTroubleshootLargeTextButton.Width, DetailedRanTroubleshootLabel.Top);
			
			Size = new Size(DetailedRanTroubleshootTextBox.Right + PaddingLeftRight, DetailedRanTroubleshootTextBox.Bottom + PaddingTopBottom);
		}
	}
}

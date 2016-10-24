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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using appCore.Settings;
using appCore.SiteFinder;
using appCore.Templates.Types;
using appCore.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of UpdateControls.
	/// </summary>
	public class UpdateControls : Panel
	{
		public Button Button_OuterRight = new Button();
		public Button Button_OuterLeft = new Button();
		public Button Button_InnerLeft = new Button();
		public Button Button_InnerRight = new Button();
		
		public Button NextActionsLargeTextButton = new Button();
		public Button UpdateLargeTextButton = new Button();
		Label NextActionsLabel = new Label();
		Label UpdateLabel = new Label();
		Label SiteIdLabel = new Label();
		Label INCLabel = new Label();
		Label RegionLabel = new Label();
		Label PriorityLabel = new Label();
		Label PowerCompanyLabel = new Label();
		public TextBox SiteIdTextBox = new TextBox();
		public TextBox INCTextBox = new TextBox();
		public TextBox RegionTextBox = new TextBox();
		public TextBox PriorityTextBox = new TextBox();
		public TextBox PowerCompanyTextBox = new TextBox();
		public AMTRichTextBox NextActionsTextBox = new AMTRichTextBox();
		public AMTRichTextBox UpdateTextBox = new AMTRichTextBox();
		
		public Site currentSite { get; private set; }
		Update currentTemplate;
		Update prevTemp = new Update();
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
		
		bool toggle;
		public bool ToggledState {
			get {
				return toggle;
			}
			set {
				if(value != toggle){
					toggle = value;
				}
			}
		}
		
		Template.UIenum _uiMode;
		Template.UIenum UiMode {
			get { return _uiMode; }
			set {
				_uiMode = value;
				if(value == Template.UIenum.Log) {
					PaddingLeftRight = 7;
					InitializeComponent();
					SiteIdTextBox.ReadOnly = true;
					INCTextBox.ReadOnly = true;
					NextActionsTextBox.ReadOnly = true;
					UpdateTextBox.ReadOnly = true;
					// 
					// Button_OuterLeft
					// 
					Button_OuterLeft.Name = "GenerateTemplateButton";
					Button_OuterLeft.Size = new Size(112, 23);
					Button_OuterLeft.TabIndex = 22;
					Button_OuterLeft.Text = "Generate Template";
					Button_OuterLeft.Click += GenerateTemplate;
					Controls.Add(Button_OuterLeft);
					// 
					// Button_OuterRight
					// 
					Button_OuterRight.Name = "CopyToNewTroubleshoot";
					Button_OuterRight.Size = new Size(183, 23);
					Button_OuterRight.TabIndex = 24;
					Button_OuterRight.Text = "Copy to new Troubleshoot template";
					Button_OuterRight.Click += LoadTemplateFromLog;
					Controls.Add(Button_OuterRight);
				}
				else {
					InitializeComponent();
					SiteIdTextBox.KeyPress += SiteIdTextBoxKeyPress;
					SiteIdTextBox.TextChanged += SiteIdTextBoxTextChanged;
					INCTextBox.KeyPress += INCTextBoxKeyPress;
					SiteIdTextBox.Size = new Size(67, 20);
					// 
					// Button_OuterLeft
					// 
					Button_OuterLeft.Name = "GenerateTemplateButton";
					Button_OuterLeft.Size = new Size(112, 23);
					Button_OuterLeft.TabIndex = 22;
					Button_OuterLeft.Text = "Generate Template";
					Button_OuterLeft.Click += GenerateTemplate;
					Controls.Add(Button_OuterLeft);
					// 
					// Button_OuterRight
					// 
					Button_OuterRight.Size = new Size(75, 23);
					Button_OuterRight.Name = "ClearButton";
					Button_OuterRight.TabIndex = 24;
					Button_OuterRight.Text = "Clear";
					Button_OuterRight.Click += ClearAllControls;
					Controls.Add(Button_OuterRight);
				}
				Button_OuterLeft.Location = new Point(PaddingLeftRight, NextActionsTextBox.Bottom + 5);
				Button_InnerLeft.Location = new Point(Button_OuterLeft.Right + 3, NextActionsTextBox.Bottom + 5);
				Button_OuterRight.Location = new Point(NextActionsTextBox.Right - Button_OuterRight.Width, NextActionsTextBox.Bottom + 5);
				Button_InnerRight.Location = new Point(Button_OuterRight.Left - Button_InnerRight.Width - 3, NextActionsTextBox.Bottom + 5);
				Size = new Size(NextActionsTextBox.Right + PaddingLeftRight, Button_OuterLeft.Bottom + PaddingTopBottom);
			}
		}
		
		public UpdateControls()
		{
			UiMode = Template.UIenum.Template;
			if(GlobalProperties.siteFinder_mainswitch)
				siteFinder_Toggle(false, false);
		}
		
		public UpdateControls(Update template, Template.UIenum uimode = Template.UIenum.Log)
		{
			UiMode = uimode;
			currentTemplate = template;
//			if(GlobalProperties.siteFinder_mainswitch)
//				siteFinder_Toggle(false, false);
			
			SiteIdTextBox.Text = currentTemplate.SiteId;
			if(UiMode == Template.UIenum.Template)
				SiteIdTextBoxKeyPress(SiteIdTextBox,new KeyPressEventArgs((char)Keys.Enter));
			INCTextBox.Text = currentTemplate.INC;
			UpdateTextBox.Text = currentTemplate.update;
			NextActionsTextBox.Text = currentTemplate.NextActions;
//			Dispose();
		}

		void siteFinder_Toggle(bool toggle, bool siteFound)
		{
			foreach (object ctrl in Controls) {
				switch(ctrl.GetType().ToString())
				{
					case "System.Windows.Forms.Button":
						Button btn = ctrl as Button;
						if(btn.Text.Contains("Generate") || btn.Text == "Clear") {
							btn.Enabled = toggle;
						}
						break;
					case "appCore.UI.AMTRichTextBox": case "System.Windows.Forms.TextBox":
						TextBoxBase tb = ctrl as TextBoxBase;
						if(tb.Name != "SiteIdTextBox" && tb.Name != "PowerCompanyTextBox")
							tb.Enabled = toggle;
						break;
					case "System.Windows.Forms.NumericUpDown":
						NumericUpDown nup = ctrl as NumericUpDown;
						if(toggle) {
							if(nup.Maximum < 999 || !siteFound)
								nup.Enabled = true;
							else
								nup.Enabled = false;
						}
						else
							nup.Enabled = toggle;
						break;
					case "System.Windows.Forms.CheckBox":
						CheckBox chb = ctrl as CheckBox;
						chb.Enabled = toggle;
						break;
					case "System.Windows.Forms.ComboBox":
						ComboBox cmb = ctrl as ComboBox;
						cmb.Enabled = toggle;
						break;
				}
			}
		}
		
		void SiteIdTextBoxKeyPress(object sender, KeyPressEventArgs e) {
			// TODO: SiteFinder(s) Performance measurement

			if (Convert.ToInt32(e.KeyChar) == 13)
			{
//				Stopwatch sw = new Stopwatch();
//
//				sw.Start();
				TextBox tb =(TextBox)sender;
				while(tb.Text.StartsWith("0"))
					tb.Text = tb.Text.Substring(1);
				currentSite = Finder.getSite(tb.Text);
				bool siteFound = !string.IsNullOrEmpty(currentSite.Id);
				if(!string.IsNullOrEmpty(currentSite.Id)) {
					PriorityTextBox.Text = currentSite.Priority;
					PowerCompanyTextBox.Text = currentSite.PowerCompany;
					RegionTextBox.Text = currentSite.Region;
				}
				else {
					PriorityTextBox.Text = string.Empty;
					PowerCompanyTextBox.Text = "No site found";
					RegionTextBox.Text = string.Empty;
				}
//				sw.Stop();
//				FlexibleMessageBox.Show("Elapsed=" + sw.Elapsed);
				siteFinder_Toggle(true, siteFound);
			}
		}

		void SiteIdTextBoxTextChanged(object sender, EventArgs e)
		{
			if(GlobalProperties.siteFinder_mainswitch)
				siteFinder_Toggle(false, false);
		}

		void INCTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				if (INCTextBox.Text.Length > 0) {
					string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(INCTextBox.Text, "INC");
					if (CompINC_CRQ != "error") INCTextBox.Text = CompINC_CRQ;
					else {
//						Action action = new Action(delegate {
						FlexibleMessageBox.Show("INC number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
//						                           });
//						Toolbox.Tools.darkenBackgroundForm(action,false,this);
						return;
					}
				}
			}
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name) {
				case "NextActionsTextBox":
					btn = (Button)NextActionsLargeTextButton;
					break;
				case "UpdateTextBox":
					btn = (Button)UpdateLargeTextButton;
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
				case "NextActionsLargeTextButton":
					tb = (TextBoxBase)NextActionsTextBox;
					lbl = NextActionsLabel.Text;
					break;
				case "UpdateLargeTextButton":
					tb = (TextBoxBase)UpdateTextBox;
					lbl = UpdateLabel.Text;
					break;
			}
			
			LargeTextForm enlarge = new LargeTextForm(tb.Text,lbl,false);
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
			tb.Text = enlarge.finaltext;
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void ClearAllControls(object sender, EventArgs e)
		{
			SiteIdTextBox.Text = string.Empty;
			INCTextBox.Text = string.Empty;
			RegionTextBox.Text = string.Empty;
			PriorityTextBox.Text = string.Empty;
			PowerCompanyTextBox.Text = string.Empty;
			NextActionsTextBox.Text = string.Empty;
			UpdateTextBox.Text = string.Empty;
			SiteIdTextBox.Focus();
		}
		
		void LoadTemplateFromLog(object sender, EventArgs e) {
//			MainForm.FillTemplateFromLog(currentTemplate);
//			TabControl tb1 = (TabControl)MainForm.Controls["tabControl1"];
//			TabControl tb2 = (TabControl)MainForm.Controls["tabControl2"];
//			tb1.SelectTab(1);
//			tb2.SelectTab(4);
		}
		
		void GenerateTemplate(object sender, EventArgs e) {
			if(UiMode == Template.UIenum.Template) {
				string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(INCTextBox.Text, "INC");
				if (CompINC_CRQ != "error") INCTextBox.Text = CompINC_CRQ;
				else {
					MessageBox.Show("INC number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				string errmsg = "";
				if (string.IsNullOrEmpty(INCTextBox.Text)) {
					errmsg = "         - INC/Ticket Number missing\n";
				}
				if (string.IsNullOrEmpty(SiteIdTextBox.Text)) {
					errmsg += "         - Site ID missing\n";
				}
				if (string.IsNullOrEmpty(UpdateTextBox.Text)) {
					errmsg += "         - Update text missing\n";
				}
				if (!string.IsNullOrEmpty(errmsg)) {
					FlexibleMessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				errmsg = "";
				
				// No changes since the last template warning
				if(currentTemplate != prevTemp) {
					if (INCTextBox.Text == prevTemp.INC) {
						errmsg = "         - INC\n";
					}
					if (SiteIdTextBox.Text == prevTemp.SiteId) {
						errmsg += "         - Site ID\n";
					}
					if (UpdateTextBox.Text == prevTemp.update) {
						errmsg += "         - Update text\n";
					}
					if (errmsg != "") {
						DialogResult ans = FlexibleMessageBox.Show("You haven't changed the following fields in the template:\n\n" + errmsg + "\nDo you want to continue anyway?","Same INC",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
						if (ans == DialogResult.No)
							return;
					}
				}
			}
			
			currentTemplate = new Update(Controls);
			
			try {
				Clipboard.SetText(currentTemplate.ToString());
			}
			catch (Exception) {
				try {
					Clipboard.SetText(currentTemplate.ToString());
				}
				catch (Exception) {
					FlexibleMessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				}
			}
			
			FlexibleMessageBox.Show(currentTemplate.ToString(), "Template copied to Clipboard", MessageBoxButtons.OK);
			
			if(UiMode == Template.UIenum.Template) {
				// Store this template for future warning on no changes
				
				prevTemp = currentTemplate;
				
				MainForm.logFile.HandleLog(currentTemplate);
			}
		}
		
		public void siteFinderSwitch(string toState) {
			if (toState == "off") {
				INCTextBox.KeyPress -= INCTextBoxKeyPress;
				siteFinder_Toggle(true,false);
			}
			else {
				INCTextBox.KeyPress += INCTextBoxKeyPress;
				siteFinder_Toggle(false,false);
			}
		}
		
		void InitializeComponent()
		{
			BackColor = SystemColors.Control;
			Name = "Update Template GUI";
			Controls.Add(RegionLabel);
			Controls.Add(RegionTextBox);
			Controls.Add(PriorityLabel);
			Controls.Add(PriorityTextBox);
			Controls.Add(PowerCompanyLabel);
			Controls.Add(PowerCompanyTextBox);
			Controls.Add(NextActionsLargeTextButton);
			Controls.Add(UpdateLargeTextButton);
			Controls.Add(NextActionsTextBox);
			Controls.Add(UpdateTextBox);
			Controls.Add(NextActionsLabel);
			Controls.Add(UpdateLabel);
			Controls.Add(SiteIdLabel);
			Controls.Add(INCLabel);
			Controls.Add(SiteIdTextBox);
			Controls.Add(INCTextBox);
			// 
			// SiteIdLabel
			// 
			SiteIdLabel.Location = new Point(PaddingLeftRight, PaddingTopBottom);
			SiteIdLabel.Name = "SiteIdLabel";
			SiteIdLabel.Size = new Size(90, 20);
			SiteIdLabel.Text = "Site ID";
			SiteIdLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteIdTextBox
			// 
			SiteIdTextBox.Font = new Font("Courier New", 8.25F);
			SiteIdTextBox.Location = new Point(SiteIdLabel.Right + 2, PaddingTopBottom);
			SiteIdTextBox.MaxLength = 6;
			SiteIdTextBox.Name = "SiteIdTextBox";
			SiteIdTextBox.Size = new Size(68, 20);
			SiteIdTextBox.TabIndex = 2;
			// 
			// PriorityLabel
			// 
			PriorityLabel.Location = new Point(SiteIdTextBox.Right + 5, PaddingTopBottom);
			PriorityLabel.Name = "PriorityLabel";
			PriorityLabel.Size = new Size(40, 20);
			PriorityLabel.Text = "Priority";
			PriorityLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PriorityTextBox
			// 
			PriorityTextBox.Font = new Font("Courier New", 8.25F);
			PriorityTextBox.Location = new Point(PriorityLabel.Right + 2, PaddingTopBottom);
			PriorityTextBox.Name = "PriorityTextBox";
			PriorityTextBox.ReadOnly = true;
			PriorityTextBox.Size = new Size(73, 20);
			PriorityTextBox.TabIndex = 89;
			// 
			// PowerCompanyLabel
			// 
			PowerCompanyLabel.Location = new Point(PaddingLeftRight, SiteIdLabel.Bottom + 4);
			PowerCompanyLabel.Name = "PowerCompanyLabel";
			PowerCompanyLabel.Size = new Size(90, 20);
			PowerCompanyLabel.Text = "Power Company";
			PowerCompanyLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PowerCompanyTextBox
			// 
			PowerCompanyTextBox.Font = new Font("Courier New", 8.25F);
			PowerCompanyTextBox.Location = new Point(PowerCompanyLabel.Right + 2, SiteIdTextBox.Bottom + 4);
			PowerCompanyTextBox.MaxLength = 5;
			PowerCompanyTextBox.Name = "PowerCompanyTextBox";
			PowerCompanyTextBox.ReadOnly = true;
			PowerCompanyTextBox.Size = new Size(188, 20);
			PowerCompanyTextBox.TabIndex = 87;
			// 
			// INCLabel
			// 
			INCLabel.Location = new Point(PriorityTextBox.Right + 5, PaddingTopBottom);
			INCLabel.Name = "INCLabel";
			INCLabel.Size = new Size(43, 20);
			INCLabel.Text = "INC";
			INCLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// INCTextBox
			// 
			INCTextBox.AcceptsTab = true;
			INCTextBox.Font = new Font("Courier New", 8.25F);
			INCTextBox.Location = new Point(INCLabel.Right + 2, PaddingTopBottom);
			INCTextBox.MaxLength = 15;
			INCTextBox.Name = "INCTextBox";
			INCTextBox.Size = new Size(180, 20);
			INCTextBox.TabIndex = 1;
			// 
			// RegionLabel
			// 
			RegionLabel.Location = new Point(PowerCompanyTextBox.Right + 5, INCLabel.Bottom + 4);
			RegionLabel.Name = "RegionLabel";
			RegionLabel.Size = new Size(43, 20);
			RegionLabel.Text = "Region";
			RegionLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RegionTextBox
			// 
			RegionTextBox.AcceptsTab = true;
			RegionTextBox.Font = new Font("Courier New", 8.25F);
			RegionTextBox.Location = new Point(RegionLabel.Right + 2, INCTextBox.Bottom + 4);
			RegionTextBox.Name = "RegionTextBox";
			RegionTextBox.ReadOnly = true;
			RegionTextBox.Size = new Size(180, 20);
			RegionTextBox.TabIndex = 91;
			// 
			// UpdateLabel
			// 
			UpdateLabel.Location = new Point(PaddingLeftRight, PowerCompanyLabel.Bottom + 4);
			UpdateLabel.Name = "UpdateLabel";
			UpdateLabel.Size = new Size(100, 20);
			UpdateLabel.Text = "Update";
			UpdateLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// UpdateTextBox
			// 
			UpdateTextBox.DetectUrls = false;
			UpdateTextBox.Font = new Font("Courier New", 8.25F);
			UpdateTextBox.Location = new Point(PaddingLeftRight, UpdateLabel.Bottom + 4);
			UpdateTextBox.Name = "UpdateTextBox";
			UpdateTextBox.Size = new Size(510, 253);
			UpdateTextBox.TabIndex = 3;
			UpdateTextBox.Text = "";
			// 
			// UpdateLargeTextButton
			// 
			UpdateLargeTextButton.Enabled = false;
			UpdateLargeTextButton.Size = new Size(24, 20);
			UpdateLargeTextButton.Location = new Point(UpdateTextBox.Right - UpdateLargeTextButton.Width, UpdateLabel.Top);
			UpdateLargeTextButton.Name = "UpdateLargeTextButton";
			UpdateLargeTextButton.TabIndex = 4;
			UpdateLargeTextButton.Text = "...";
			UpdateLargeTextButton.UseVisualStyleBackColor = true;
			UpdateLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// NextActionsLabel
			// 
			NextActionsLabel.Location = new Point(PaddingLeftRight, UpdateTextBox.Bottom + 4);
			NextActionsLabel.Name = "NextActionsLabel";
			NextActionsLabel.Size = new Size(109, 20);
			NextActionsLabel.TabIndex = 86;
			NextActionsLabel.Text = "Next actions";
			NextActionsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// NextActionsTextBox
			// 
			NextActionsTextBox.DetectUrls = false;
			NextActionsTextBox.Font = new Font("Courier New", 8.25F);
			NextActionsTextBox.Location = new Point(PaddingLeftRight, NextActionsLabel.Bottom + 4);
			NextActionsTextBox.Name = "NextActionsTextBox";
			NextActionsTextBox.Size = new Size(510, 235);
			NextActionsTextBox.TabIndex = 5;
			NextActionsTextBox.Text = "";
			// 
			// NextActionsLargeTextButton
			// 
			NextActionsLargeTextButton.Enabled = false;
			NextActionsLargeTextButton.Size = new Size(24, 20);
			NextActionsLargeTextButton.Location = new Point(NextActionsTextBox.Right - NextActionsLargeTextButton.Width, NextActionsLabel.Top);
			NextActionsLargeTextButton.Name = "NextActionsLargeTextButton";
			NextActionsLargeTextButton.TabIndex = 6;
			NextActionsLargeTextButton.Text = "...";
			NextActionsLargeTextButton.UseVisualStyleBackColor = true;
			NextActionsLargeTextButton.Click += LargeTextButtonsClick;
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
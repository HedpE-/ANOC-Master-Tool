/*
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
using appCore.Settings;
using appCore.SiteFinder;
using appCore.SiteFinder.UI;
using appCore.Templates.Types;
using appCore.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of UpdateControls.
	/// </summary>
	public class UpdateControls : Panel
	{
		public Button NextActionsLargeTextButton = new Button();
		public Button UpdateLargeTextButton = new Button();
		Label NextActionsLabel = new Label();
		Label UpdateLabel = new Label();
		Label SiteIdLabel = new Label();
		Label INCLabel = new Label();
		Label RegionLabel = new Label();
		Label PriorityLabel = new Label();
		Label PowerCompanyLabel = new Label();
		public AMTTextBox SiteIdTextBox = new AMTTextBox();
		public AMTTextBox INCTextBox = new AMTTextBox();
		public AMTTextBox RegionTextBox = new AMTTextBox();
		public AMTTextBox PriorityTextBox = new AMTTextBox();
		public AMTTextBox PowerCompanyTextBox = new AMTTextBox();
		public AMTRichTextBox NextActionsTextBox = new AMTRichTextBox();
		public AMTRichTextBox UpdateTextBox = new AMTRichTextBox();
		
		public AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem SiteDetailsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateTemplateToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToNewTemplateToolStripMenuItem = new ToolStripMenuItem();
		
		public static siteDetails SiteDetailsUI;
		
		public Site currentSite { get; set; }
		Update currentTemplate;
		Update prevTemp = new Update();
		
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
		
		Template.UiEnum _uiMode;
		Template.UiEnum UiMode {
			get { return _uiMode; }
			set {
				_uiMode = value;
				if(value == Template.UiEnum.Log) {
					PaddingLeftRight = 7;
					InitializeComponent();
					SiteIdTextBox.ReadOnly = true;
					INCTextBox.ReadOnly = true;
					NextActionsTextBox.ReadOnly = true;
					UpdateTextBox.ReadOnly = true;
					
					MainMenu.MainMenu.DropDownItems.AddRange(new ToolStripItem[] {
					                                         	generateTemplateToolStripMenuItem,
					                                         	copyToNewTemplateToolStripMenuItem});
				}
				else {
					InitializeComponent();
					SiteIdTextBox.KeyPress += SiteIdTextBoxKeyPress;
					SiteIdTextBox.TextChanged += SiteIdTextBoxTextChanged;
					INCTextBox.KeyPress += INCTextBoxKeyPress;
					SiteIdTextBox.Size = new Size(67, 20);
					
					MainMenu.InitializeTroubleshootMenu();
					MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
					MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
					
					MainMenu.MainMenu.DropDownItems.Add(generateTemplateToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(SiteDetailsToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
					
					generateTemplateToolStripMenuItem.Enabled =
						SiteDetailsToolStripMenuItem.Enabled =
						clearToolStripMenuItem.Enabled = false;
				}
			}
		}
		
		public UpdateControls()
		{
			UiMode = Template.UiEnum.Template;
			if(GlobalProperties.siteFinder_mainswitch)
				siteFinder_Toggle(false, false);
		}
		
		public UpdateControls(Update template, Template.UiEnum uimode = Template.UiEnum.Log)
		{
			UiMode = uimode;
			currentTemplate = template;
//			if(GlobalProperties.siteFinder_mainswitch)
//				siteFinder_Toggle(false, false);
			
			SiteIdTextBox.Text = currentTemplate.SiteId;
			if(UiMode == Template.UiEnum.Template)
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
					case "appCore.UI.AMTMenuStrip":
						MainMenu.siteFinder_Toggle(toggle, siteFound);
						break;
					case "System.Windows.Forms.Button":
						Button btn = ctrl as Button;
						if(btn.Text.Contains("Generate") || btn.Text == "Clear") {
							btn.Enabled = toggle;
						}
						break;
					case "appCore.UI.AMTRichTextBox": case "appCore.UI.AMTTextBox":
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
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				TextBox tb =(TextBox)sender;
				while(tb.Text.StartsWith("0"))
					tb.Text = tb.Text.Substring(1);
				Action actionThreaded = new Action(delegate {
				                                   	currentSite = DB.SitesDB.getSite(tb.Text);
				                                   	
				                                   	if(currentSite.Exists) {
				                                   		string dataToRequest = "INC";
				                                   		if((DateTime.Now - currentSite.ChangesTimestamp) > new TimeSpan(0, 30, 0))
				                                   			dataToRequest += "CRQ";
				                                   		if(string.IsNullOrEmpty(currentSite.PowerCompany))
				                                   			dataToRequest += "PWR";
				                                   		currentSite.requestOIData(dataToRequest);
				                                   	}
				                                   });
				
				Action actionNonThreaded = new Action(delegate {
				                                      	if(currentSite.Exists) {
				                                      		PriorityTextBox.Text = currentSite.Priority;
				                                      		PowerCompanyTextBox.Text = currentSite.PowerCompany;
				                                      		RegionTextBox.Text = currentSite.Region;
				                                      		SiteDetailsToolStripMenuItem.Enabled = true;
				                                      	}
				                                      	else {
				                                      		PriorityTextBox.Text = string.Empty;
				                                      		PowerCompanyTextBox.Text = "No site found";
				                                      		RegionTextBox.Text = string.Empty;
				                                      		SiteDetailsToolStripMenuItem.Enabled = false;
				                                      	}
				                                      	generateTemplateToolStripMenuItem.Enabled = true;
				                                      	siteFinder_Toggle(true, currentSite.Exists);
				                                      });
				LoadingPanel load = new LoadingPanel();
				load.ShowAsync(actionThreaded, actionNonThreaded, true, this);
			}
		}

		void SiteIdTextBoxTextChanged(object sender, EventArgs e)
		{
			currentSite = null;
			siteFinder_Toggle(false, false);
			clearToolStripMenuItem.Enabled = !string.IsNullOrEmpty(SiteIdTextBox.Text);
			SiteDetailsToolStripMenuItem.Enabled =
				generateTemplateToolStripMenuItem.Enabled = false;
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
			
			AMTLargeTextForm enlarge = new AMTLargeTextForm(tb.Text,lbl,false);
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
			if(UiMode == Template.UiEnum.Template) {
				string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(INCTextBox.Text, "INC");
				if (CompINC_CRQ != "error") INCTextBox.Text = CompINC_CRQ;
				else {
					FlexibleMessageBox.Show("INC number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			
			if(UiMode == Template.UiEnum.Template) {
				// Store this template for future warning on no changes
				
				prevTemp = currentTemplate;
				
				MainForm.logFiles.HandleLog(currentTemplate);
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

		void SiteDetailsButtonClick(object sender, EventArgs e)
		{
			if(SiteDetailsUI != null) {
				SiteDetailsUI.Close();
				SiteDetailsUI.Dispose();
			}
			SiteDetailsUI = new siteDetails(currentSite);
			SiteDetailsUI.Show();
		}
		
		void LoadDisplayOiDataTable(object sender, EventArgs e) {
//			if(e.Button == MouseButtons.Left) {
			string dataToShow = ((ToolStripMenuItem)sender).Name.Replace("Button", string.Empty);
			var fc = Application.OpenForms.OfType<OiSiteTablesForm>().Where(f => f.OwnerControl == (Control)this && f.Text.EndsWith(dataToShow)).ToList();
			if(fc.Count > 0) {
				fc[0].Close();
				fc[0].Dispose();
			}
			
			if(currentSite.Exists) {
				var dt = new System.Data.DataTable();
				switch(dataToShow) {
					case "INCs":
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
						break;
					case "CRQs":
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
						break;
					case "BookIns":
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
						break;
					case "ActiveAlarms":
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
				}
				OiTable.Show();
			}
		}
		
		void refreshOiData(object sender, EventArgs e) {
			currentSite.requestOIData("INCCRQBookinsAlarms");
			MainMenu.siteFinder_Toggle(true);
		}
		
		void InitializeComponent()
		{
			BackColor = SystemColors.Control;
			Name = "Update Template GUI";
			Controls.Add(MainMenu);
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
			// MainMenu
			// 
			MainMenu.Location = new Point(paddingLeftRight, 0);
			// 
			// SiteDetailsToolStripMenuItem
			// 
			SiteDetailsToolStripMenuItem.Name = "SiteDetailsToolStripMenuItem";
			SiteDetailsToolStripMenuItem.Text = "Site Details";
//			SiteDetailsToolStripMenuItem.Font = new Font("Arial Unicode MS", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			SiteDetailsToolStripMenuItem.Click += SiteDetailsButtonClick;
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
			copyToNewTemplateToolStripMenuItem.Text = "Copy to new Troubleshoot template";
//			copyToNewTemplateToolStripMenuItem.Click += LoadTemplateFromLog;
			// 
			// SiteIdLabel
			// 
//			SiteIdLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
//			SiteIdLabel.Size = new Size(90, 20);
			SiteIdLabel.Name = "SiteIdLabel";
			SiteIdLabel.Text = "Site ID";
			SiteIdLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteIdTextBox
			// 
			SiteIdTextBox.Font = new Font("Courier New", 8.25F);
//			SiteIdTextBox.Location = new Point(SiteIdLabel.Right + 2, MainMenu.Bottom + 4);
//			SiteIdTextBox.Size = new Size(68, 20);
			SiteIdTextBox.MaxLength = 6;
			SiteIdTextBox.Name = "SiteIdTextBox";
			SiteIdTextBox.TabIndex = 2;
			// 
			// PriorityLabel
			// 
//			PriorityLabel.Location = new Point(SiteIdTextBox.Right + 5, MainMenu.Bottom + 4);
//			PriorityLabel.Size = new Size(40, 20);
			PriorityLabel.Name = "PriorityLabel";
			PriorityLabel.Text = "Priority";
			PriorityLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PriorityTextBox
			// 
			PriorityTextBox.Font = new Font("Courier New", 8.25F);
//			PriorityTextBox.Location = new Point(PriorityLabel.Right + 2, MainMenu.Bottom + 4);
//			PriorityTextBox.Size = new Size(73, 20);
			PriorityTextBox.Name = "PriorityTextBox";
			PriorityTextBox.ReadOnly = true;
			PriorityTextBox.TabIndex = 89;
			// 
			// PowerCompanyLabel
			// 
//			PowerCompanyLabel.Location = new Point(PaddingLeftRight, SiteIdLabel.Bottom + 4);
//			PowerCompanyLabel.Size = new Size(90, 20);
			PowerCompanyLabel.Name = "PowerCompanyLabel";
			PowerCompanyLabel.Text = "Power Company";
			PowerCompanyLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PowerCompanyTextBox
			// 
			PowerCompanyTextBox.Font = new Font("Courier New", 8.25F);
//			PowerCompanyTextBox.Location = new Point(PowerCompanyLabel.Right + 2, SiteIdTextBox.Bottom + 4);
//			PowerCompanyTextBox.Size = new Size(188, 20);
			PowerCompanyTextBox.MaxLength = 5;
			PowerCompanyTextBox.Name = "PowerCompanyTextBox";
			PowerCompanyTextBox.ReadOnly = true;
			PowerCompanyTextBox.TabIndex = 87;
			// 
			// INCLabel
			// 
//			INCLabel.Location = new Point(PriorityTextBox.Right + 5, MainMenu.Bottom + 4);
//			INCLabel.Size = new Size(43, 20);
			INCLabel.Name = "INCLabel";
			INCLabel.Text = "INC";
			INCLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// INCTextBox
			// 
			INCTextBox.AcceptsTab = true;
			INCTextBox.Font = new Font("Courier New", 8.25F);
//			INCTextBox.Location = new Point(INCLabel.Right + 2, MainMenu.Bottom + 4);
//			INCTextBox.Size = new Size(180, 20);
			INCTextBox.MaxLength = 15;
			INCTextBox.Name = "INCTextBox";
			INCTextBox.TabIndex = 1;
			// 
			// RegionLabel
			// 
//			RegionLabel.Location = new Point(PowerCompanyTextBox.Right + 5, INCLabel.Bottom + 4);
//			RegionLabel.Size = new Size(43, 20);
			RegionLabel.Name = "RegionLabel";
			RegionLabel.Text = "Region";
			RegionLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RegionTextBox
			// 
			RegionTextBox.AcceptsTab = true;
			RegionTextBox.Font = new Font("Courier New", 8.25F);
//			RegionTextBox.Location = new Point(RegionLabel.Right + 2, INCTextBox.Bottom + 4);
//			RegionTextBox.Size = new Size(180, 20);
			RegionTextBox.Name = "RegionTextBox";
			RegionTextBox.ReadOnly = true;
			RegionTextBox.TabIndex = 91;
			// 
			// UpdateLabel
			// 
//			UpdateLabel.Location = new Point(PaddingLeftRight, PowerCompanyLabel.Bottom + 4);
//			UpdateLabel.Size = new Size(100, 20);
			UpdateLabel.Name = "UpdateLabel";
			UpdateLabel.Text = "Update";
			UpdateLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// UpdateTextBox
			// 
			UpdateTextBox.DetectUrls = false;
			UpdateTextBox.Font = new Font("Courier New", 8.25F);
//			UpdateTextBox.Location = new Point(PaddingLeftRight, UpdateLabel.Bottom + 4);
//			UpdateTextBox.Size = new Size(510, 253);
			UpdateTextBox.Name = "UpdateTextBox";
			UpdateTextBox.TabIndex = 3;
			UpdateTextBox.Text = "";
			UpdateTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// UpdateLargeTextButton
			// 
			UpdateLargeTextButton.Enabled = false;
//			UpdateLargeTextButton.Size = new Size(24, 20);
//			UpdateLargeTextButton.Location = new Point(UpdateTextBox.Right - UpdateLargeTextButton.Width, UpdateLabel.Top);
			UpdateLargeTextButton.Name = "UpdateLargeTextButton";
			UpdateLargeTextButton.TabIndex = 4;
			UpdateLargeTextButton.Text = "...";
			UpdateLargeTextButton.UseVisualStyleBackColor = true;
			UpdateLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// NextActionsLabel
			// 
//			NextActionsLabel.Location = new Point(PaddingLeftRight, UpdateTextBox.Bottom + 4);
//			NextActionsLabel.Size = new Size(109, 20);
			NextActionsLabel.Name = "NextActionsLabel";
			NextActionsLabel.TabIndex = 86;
			NextActionsLabel.Text = "Next actions";
			NextActionsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// NextActionsTextBox
			// 
			NextActionsTextBox.DetectUrls = false;
			NextActionsTextBox.Font = new Font("Courier New", 8.25F);
//			NextActionsTextBox.Location = new Point(PaddingLeftRight, NextActionsLabel.Bottom + 4);
//			NextActionsTextBox.Size = new Size(510, 235);
			NextActionsTextBox.Name = "NextActionsTextBox";
			NextActionsTextBox.TabIndex = 5;
			NextActionsTextBox.Text = "";
			NextActionsTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// NextActionsLargeTextButton
			// 
			NextActionsLargeTextButton.Enabled = false;
//			NextActionsLargeTextButton.Size = new Size(24, 20);
//			NextActionsLargeTextButton.Location = new Point(NextActionsTextBox.Right - NextActionsLargeTextButton.Width, NextActionsLabel.Top);
			NextActionsLargeTextButton.Name = "NextActionsLargeTextButton";
			NextActionsLargeTextButton.TabIndex = 6;
			NextActionsLargeTextButton.Text = "...";
			NextActionsLargeTextButton.UseVisualStyleBackColor = true;
			NextActionsLargeTextButton.Click += LargeTextButtonsClick;
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			SiteIdLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			SiteIdLabel.Size = new Size(90, 20);
			
			SiteIdTextBox.Location = new Point(SiteIdLabel.Right + 2, MainMenu.Bottom + 4);
			SiteIdTextBox.Size = new Size(68, 20);
			
			PriorityLabel.Location = new Point(SiteIdTextBox.Right + 5, MainMenu.Bottom + 4);
			PriorityLabel.Size = new Size(40, 20);
			
			PriorityTextBox.Location = new Point(PriorityLabel.Right + 2, MainMenu.Bottom + 4);
			PriorityTextBox.Size = new Size(73, 20);
			
			PowerCompanyLabel.Location = new Point(PaddingLeftRight, SiteIdLabel.Bottom + 4);
			PowerCompanyLabel.Size = new Size(90, 20);
			
			PowerCompanyTextBox.Location = new Point(PowerCompanyLabel.Right + 2, SiteIdTextBox.Bottom + 4);
			PowerCompanyTextBox.Size = new Size(188, 20);
			
			INCLabel.Location = new Point(PriorityTextBox.Right + 5, MainMenu.Bottom + 4);
			INCLabel.Size = new Size(43, 20);
			
			INCTextBox.Location = new Point(INCLabel.Right + 2, MainMenu.Bottom + 4);
			INCTextBox.Size = new Size(180, 20);
			
			RegionLabel.Location = new Point(PowerCompanyTextBox.Right + 5, INCLabel.Bottom + 4);
			RegionLabel.Size = new Size(43, 20);
			
			RegionTextBox.Location = new Point(RegionLabel.Right + 2, INCTextBox.Bottom + 4);
			RegionTextBox.Size = new Size(180, 20);
			
			UpdateLabel.Location = new Point(PaddingLeftRight, PowerCompanyLabel.Bottom + 4);
			UpdateLabel.Size = new Size(100, 20);
			
			UpdateTextBox.Location = new Point(PaddingLeftRight, UpdateLabel.Bottom + 4);
			UpdateTextBox.Size = new Size(510, 253);
			
			UpdateLargeTextButton.Size = new Size(24, 20);
			UpdateLargeTextButton.Location = new Point(UpdateTextBox.Right - UpdateLargeTextButton.Width, UpdateLabel.Top);
			
			NextActionsLabel.Location = new Point(PaddingLeftRight, UpdateTextBox.Bottom + 4);
			NextActionsLabel.Size = new Size(109, 20);
			
			NextActionsTextBox.Location = new Point(PaddingLeftRight, NextActionsLabel.Bottom + 4);
			NextActionsTextBox.Size = new Size(510, 235);
			
			NextActionsLargeTextButton.Size = new Size(24, 20);
			NextActionsLargeTextButton.Location = new Point(NextActionsTextBox.Right - NextActionsLargeTextButton.Width, NextActionsLabel.Top);
			
			Size = new Size(NextActionsTextBox.Right + PaddingLeftRight, NextActionsTextBox.Bottom + PaddingTopBottom);
		}
	}
}
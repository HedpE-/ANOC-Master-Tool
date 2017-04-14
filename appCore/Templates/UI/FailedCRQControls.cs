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
	/// Description of FailedCRQControls.
	/// </summary>
	public class FailedCRQControls : Panel
	{
		public GroupBox FEBookedInGroupBox = new GroupBox();
		public CheckBox FEBookedIn_CalledANOCCheckBox = new CheckBox();
		Label FEBookedIn_PhoneNumberLabel = new Label();
		Label FEBookedIn_NameLabel = new Label();
		public AMTTextBox FEBookedIn_PhoneNumberTextBox = new AMTTextBox();
		public AMTTextBox FEBookedIn_NameTextBox = new AMTTextBox();
		
		Label CRQContactsLabel = new Label();
		public Button CRQContactsLargeTextButton = new Button();
		public AMTRichTextBox CRQContactsTextBox = new AMTRichTextBox();
		
		public GroupBox ContractorToFixFaultGroupBox = new GroupBox();
		public CheckBox ContractorToFixFault_WillReturnCheckBox = new CheckBox();
		public DateTimePicker ContractorToFixFault_WillReturnDateTimePicker = new DateTimePicker();
		Label ContractorToFixFault_PhoneNumberLabel = new Label();
		Label ContractorToFixFault_NameLabel = new Label();
		public AMTTextBox ContractorToFixFault_PhoneNumberTextBox = new AMTTextBox();
		public AMTTextBox ContractorToFixFault_NameTextBox = new AMTTextBox();
		
		public Button TroubleshootingDoneLargeTextButton = new Button();
		public Button ObservationsLargeTextButton = new Button();
		public Button WorkPerformedByFELargeTextButton = new Button();
		public AMTTextBox INCTextBox = new AMTTextBox();
		public AMTTextBox CRQTextBox = new AMTTextBox();
		public AMTTextBox SiteIdTextBox = new AMTTextBox();
		AMTTextBox PriorityTextBox = new AMTTextBox();
		AMTTextBox RegionTextBox = new AMTTextBox();
		public AMTRichTextBox WorkPerformedByFETextBox = new AMTRichTextBox();
		public AMTRichTextBox TroubleshootingDoneTextBox = new AMTRichTextBox();
		public AMTRichTextBox ObservationsTextBox = new AMTRichTextBox();
		Label CRQLabel = new Label();
		Label INCLabel = new Label();
		Label WorkPerformedByFELabel = new Label();
		Label TroubleshootingDoneLabel = new Label();
		Label ObservationsLabel = new Label();
		Label SiteIdLabel = new Label();
		Label PriorityLabel = new Label();
		Label RegionLabel = new Label();
		
		public AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem SiteDetailsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateTemplateToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToNewTemplateToolStripMenuItem = new ToolStripMenuItem();
		
		public static siteDetails SiteDetailsUI;
		
		public Site currentSite { get; set; }
		FailedCRQ currentTemplate;
		FailedCRQ prevTemp = new FailedCRQ();
		
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
		
		UiEnum _uiMode;
		UiEnum UiMode {
			get { return _uiMode; }
			set {
				_uiMode = value;
				if(value == UiEnum.Log) {
					PaddingLeftRight = 7;
					InitializeComponent();
					SiteIdTextBox.ReadOnly = true;
					INCTextBox.ReadOnly = true;
					FEBookedIn_CalledANOCCheckBox.Enabled = false;
					FEBookedIn_PhoneNumberTextBox.ReadOnly = true; // textBox12;
					FEBookedIn_NameTextBox.ReadOnly = true; // textBox13;
					CRQContactsTextBox.ReadOnly = true;
					ContractorToFixFault_WillReturnCheckBox.Enabled = false; // checkBox7;
					ContractorToFixFault_WillReturnDateTimePicker.Enabled = false;
					ContractorToFixFault_PhoneNumberTextBox.ReadOnly = true; // textBox10
					ContractorToFixFault_NameTextBox.ReadOnly = true;
					CRQTextBox.ReadOnly = true; // textBox18
					WorkPerformedByFETextBox.ReadOnly = true; // richTextBox1
					TroubleshootingDoneTextBox.ReadOnly = true; // richTextBox2
					ObservationsTextBox.ReadOnly = true; // richTextBox3
					
					MainMenu.MainMenu.DropDownItems.AddRange(new ToolStripItem[] {
					                                         	generateTemplateToolStripMenuItem,
					                                         	copyToNewTemplateToolStripMenuItem});
				}
				else {
					InitializeComponent();
					INCTextBox.KeyPress += INCTextBoxKeyPress;
					CRQTextBox.KeyPress += CRQTextBoxKeyPress;
					SiteIdTextBox.TextChanged += SiteIdTextBoxTextChanged;
					CRQContactsTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
					WorkPerformedByFETextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
					TroubleshootingDoneTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
					ObservationsTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
					CRQContactsLargeTextButton.Click += LargeTextButtonsClick;
					WorkPerformedByFELargeTextButton.Click += LargeTextButtonsClick;
					TroubleshootingDoneLargeTextButton.Click += LargeTextButtonsClick;
					ObservationsLargeTextButton.Click += LargeTextButtonsClick;
					ContractorToFixFault_NameTextBox.TextChanged += ContractorToFixFault_NameTextChanged;
					ContractorToFixFault_PhoneNumberTextBox.ReadOnly = true;
					
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
		
		public FailedCRQControls()
		{
			UiMode = UiEnum.Template;
			if(GlobalProperties.siteFinder_mainswitch)
				siteFinder_Toggle(false, false);
		}
		
		public FailedCRQControls(FailedCRQ template, UiEnum uimode = UiEnum.Log)
		{
			UiMode = uimode;
			currentTemplate = template;
			
			SiteIdTextBox.Text = currentTemplate.SiteId;
			if(UiMode == UiEnum.Template)
			SiteIdTextBoxKeyPress(SiteIdTextBox, new KeyPressEventArgs((char)Keys.Enter));
			else
				currentSite = currentTemplate.Site;
			INCTextBox.Text = currentTemplate.INC;
			CRQTextBox.Text = currentTemplate.CRQ;
			CRQContactsTextBox.Text = currentTemplate.CrqContacts;
			WorkPerformedByFETextBox.Text = currentTemplate.WorkPerformed == "N/A" ? string.Empty : currentTemplate.WorkPerformed;
			TroubleshootingDoneTextBox.Text = currentTemplate.TroubleshootingDone == "N/A" ? string.Empty : currentTemplate.TroubleshootingDone;
			ObservationsTextBox.Text = currentTemplate.Observations;
			FEBookedIn_CalledANOCCheckBox.Checked = currentTemplate.FECalledANOC;
			FEBookedIn_PhoneNumberTextBox.Text = currentTemplate.FEBookedInTel;
			FEBookedIn_NameTextBox.Text = currentTemplate.FEBookedInName;
			ContractorToFixFault_WillReturnCheckBox.Checked = currentTemplate.ContractorToFixFault_Date != new DateTime(1753, 1, 1);
			ContractorToFixFault_WillReturnDateTimePicker.Value = currentTemplate.ContractorToFixFault_Date;
			ContractorToFixFault_PhoneNumberTextBox.Text = currentTemplate.ContractorToFixFault_Tel;
			ContractorToFixFault_NameTextBox.Text = currentTemplate.ContractorToFixFault_Name == "None provided" ? string.Empty : currentTemplate.ContractorToFixFault_Name;
		}
		
		void GenerateTemplate(object sender, EventArgs e) {
			if(UiMode == UiEnum.Template) {
				string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(INCTextBox.Text, "INC");
				if (CompINC_CRQ != "error")
					INCTextBox.Text = CompINC_CRQ;
				else {
					FlexibleMessageBox.Show("INC number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(CRQTextBox.Text, "CRQ");
				if (CompINC_CRQ != "error")
					CRQTextBox.Text = CompINC_CRQ;
				else {
					FlexibleMessageBox.Show("CRQ number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				string errmsg = "";
				if (string.IsNullOrEmpty(INCTextBox.Text)) {
					errmsg = "         - INC missing\n";
				}
				if (string.IsNullOrEmpty(CRQTextBox.Text)) {
					errmsg += "         - CRQ missing\n";
				}
				if (string.IsNullOrEmpty(SiteIdTextBox.Text)) {
					errmsg += "         - Site missing\n";
				}
				if (string.IsNullOrEmpty(FEBookedIn_NameTextBox.Text)) {
					errmsg += "         - FE booked in name missing\n";
				}
				if (string.IsNullOrEmpty(FEBookedIn_PhoneNumberTextBox.Text)) {
					errmsg += "         - FE phone number missing\n";
				}
				if (string.IsNullOrEmpty(CRQContactsTextBox.Text)) {
					errmsg += "         - CRQ contacts missing\n";
				}
				if (!string.IsNullOrEmpty(ContractorToFixFault_NameTextBox.Text) || !string.IsNullOrEmpty(ContractorToFixFault_PhoneNumberTextBox.Text) || ContractorToFixFault_WillReturnCheckBox.Checked) {
					if (string.IsNullOrEmpty(ContractorToFixFault_PhoneNumberTextBox.Text)) {
						errmsg += "         - Contractor to fix the fault phone number missing\n";
					}
					if (string.IsNullOrEmpty(ContractorToFixFault_NameTextBox.Text)) {
						errmsg += "         - Contractor to fix the fault name missing\n";
					}
					if (ContractorToFixFault_WillReturnCheckBox.Checked && ContractorToFixFault_WillReturnDateTimePicker.Value < DateTime.Now) {
						errmsg += "         - Invalid date: When will the contractor return to fix the fault\n";
					}
				}
				if (!string.IsNullOrEmpty(errmsg)) {
					FlexibleMessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				errmsg = "";
				
				// No changes since the last template warning
				
				// UNDONE: FailedCRQControls prevTemp check
				
//				if(currentTemplate.fullLog != prevTemp.fullLog) {
//					if (INCTextBox.Text == prevTemp.INC) {
//						errmsg = "         - INC\n";
//					}
//					if (SiteIdTextBox.Text == prevTemp.SiteId) {
//						errmsg += "         - Site ID\n";
//					}
//					if (SiteOwnerComboBox.Text == "TF" && TefSiteTextBox.Text == prevTemp.TefSiteId) {
//						errmsg += "         - TF Site ID\n";
//					}
//					if (AddressTextBox.Text == prevTemp.SiteAddress) {
//						errmsg += "         - Site Address\n";
//					}
//					if (CCTRefTextBox.Text != "" && CCTRefTextBox.Text == prevTemp.CCTReference) {
//						errmsg += "         - CCT reference\n";
//					}
//					if (OtherSitesImpactedCheckBox.Checked && OtherSitesImpactedCheckBox.Checked == prevTemp.OtherSitesImpacted){
//						errmsg += "         - Other sites impacted\n";
//					}
//					if (COOSCheckBox.Checked) {
//						if (COOS2GNumericUpDown.Value == prevTemp.COOS2G){
//							errmsg += "         - 2G COOS count\n";
//						}
//						if (COOS3GNumericUpDown.Value == prevTemp.COOS3G) {
//							errmsg += "         - 3G COOS count\n";
//						}
//						if (COOS4GNumericUpDown.Value == prevTemp.COOS4G) {
//							errmsg += "         - 4G COOS count\n";
//						}
//						if(FullSiteOutageCheckBox.Checked && FullSiteOutageCheckBox.Checked == prevTemp.FullSiteOutage)
//							errmsg += "         - Full Site Outage flag\n";
//					}
//					if (PerformanceIssueCheckBox.Checked && PerformanceIssueCheckBox.Checked == prevTemp.PerformanceIssue) {
//						errmsg += "         - Performance issue\n";
//					}
//					if (IntermittentIssueCheckBox.Checked && IntermittentIssueCheckBox.Checked == prevTemp.IntermittentIssue) {
//						errmsg += "         - Intermittent issue\n";
//					}
//					if (RelatedINC_CRQTextBox.Text != "" && RelatedINC_CRQTextBox.Text == prevTemp.RelatedINC_CRQ) {
//						errmsg += "         - Related INC/CRQ\n";
//					}
//					if (ActiveAlarmsTextBox.Text == prevTemp.ActiveAlarms) {
//						errmsg += "         - Active Alarms\n";
//					}
//					if (AlarmHistoryTextBox.Text != "" && AlarmHistoryTextBox.Text == prevTemp.AlarmHistory) {
//						errmsg += "         - Alarm History\n";
//					}
//					if (TroubleshootTextBox.Text == prevTemp.Troubleshoot) {
//						errmsg += "         - Troubleshoot\n";
//					}
//					if (errmsg != "") {
//						DialogResult ans = FlexibleMessageBox.Show("You haven't changed the following fields in the template:\n\n" + errmsg + "\nDo you want to continue anyway?","Same INC",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
//						if (ans == DialogResult.No)
//							return;
//					}
//				}
			}
			currentTemplate = new FailedCRQ(Controls);
			
			Toolbox.Tools.CreateMailItem("A-NOC-UK1stLineRANSL@internal.vodafone.com", string.Empty, currentTemplate.EmailSubject, currentTemplate.EmailBody, true);
			
			if(UiMode == UiEnum.Template) {
				prevTemp = currentTemplate;
				
				MainForm.logFiles.HandleLog(currentTemplate);
			}
		}
		
		void SiteIdTextBoxKeyPress(object sender, KeyPressEventArgs e) {
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				TextBox tb = (TextBox)sender;
				while(tb.Text.StartsWith("0"))
					tb.Text = tb.Text.Substring(1);
				
				Action actionThreaded = new Action(delegate {
				                                   	currentSite = DB.SitesDB.getSite(tb.Text);
				                                   	
				                                   	if(currentSite.Exists) {
				                                   		string dataToRequest = "INC";
				                                   		if((DateTime.Now - currentSite.ChangesTimestamp) > new TimeSpan(0, 30, 0))
				                                   			dataToRequest += "CRQ";
				                                   		currentSite.requestOIData(dataToRequest);
				                                   	}
				                                   });
				
				Action actionNonThreaded = new Action(delegate {
				                                      	if(currentSite.Exists) {
				                                      		PriorityTextBox.Text = currentSite.Priority;
				                                      		RegionTextBox.Text = currentSite.Region;
				                                      		SiteDetailsToolStripMenuItem.Enabled = true;
				                                      	}
				                                      	else {
				                                      		RegionTextBox.Text = "No site found";
				                                      		PriorityTextBox.Text = string.Empty;
				                                      		SiteDetailsToolStripMenuItem.Enabled = false;
				                                      	}
				                                      	generateTemplateToolStripMenuItem.Enabled = true;
				                                      	siteFinder_Toggle(true, currentSite.Exists);
				                                      });
				if(this.Parent != null) {
					LoadingPanel load = new LoadingPanel();
					load.ShowAsync(actionThreaded, actionNonThreaded, true, this);
				}
				else {
					actionThreaded();
					actionNonThreaded();
				}
			}
		}

		void SiteIdTextBoxTextChanged(object sender, EventArgs e)
		{
			currentSite = null;
			siteFinder_Toggle(false, false);
			SiteDetailsToolStripMenuItem.Enabled = false;
			clearToolStripMenuItem.Enabled = !string.IsNullOrEmpty(SiteIdTextBox.Text);
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
						if(btn.Text.Contains("Generate") || btn.Text == "Clear")
							btn.Enabled = toggle;
						break;
					case "appCore.UI.AMTRichTextBox": case "appCore.UI.AMTTextBox":
						TextBoxBase tb = ctrl as TextBoxBase;
						if(tb.Name != "SiteIdTextBox" && tb.Name != "RegionTextBox" && tb.Name != "PriorityTextBox")
							tb.Enabled = toggle;
						break;
					case "System.Windows.Forms.CheckBox":
						CheckBox chb = ctrl as CheckBox;
						chb.Enabled = toggle;
						break;
					case "System.Windows.Forms.GroupBox":
						GroupBox grb = ctrl as GroupBox;
						foreach(Control ctr in grb.Controls) {
							switch(ctr.GetType().ToString())
							{
								case "appCore.UI.AMTTextBox":
									TextBoxBase txb = ctr as TextBoxBase;
									txb.Enabled = toggle;
									break;
								case "System.Windows.Forms.CheckBox":
									CheckBox chkb = ctr as CheckBox;
									chkb.Enabled = toggle;
									break;
								case "System.Windows.Forms.DateTimePicker":
									DateTimePicker dtp = ctr as DateTimePicker;
									dtp.Enabled = toggle;
									break;
							}
						}
						break;
				}
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

		void INCTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				if (INCTextBox.Text.Length > 0) {
					string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(INCTextBox.Text, "INC");
					if (CompINC_CRQ != "error")
						INCTextBox.Text = CompINC_CRQ;
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

		void CRQTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				if (CRQTextBox.Text.Length > 0) {
					string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(CRQTextBox.Text, "CRQ");
					if (CompINC_CRQ != "error") CRQTextBox.Text = CompINC_CRQ;
					else {
//						Action action = new Action(delegate {
						FlexibleMessageBox.Show("CRQ number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
//						                           });
//						Toolbox.Tools.darkenBackgroundForm(action,false,this);
						return;
					}
				}
			}
		}
		
		void ContractorToFixFault_WillReturnCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			ContractorToFixFault_WillReturnDateTimePicker.Visible = ContractorToFixFault_WillReturnCheckBox.Checked;
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e) {
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name) {
				case "CRQContactsTextBox":
					btn = (Button)CRQContactsLargeTextButton;
					break;
				case "WorkPerformedByFETextBox":
					btn = (Button)WorkPerformedByFELargeTextButton;
					break;
				case "TroubleshootingDoneTextBox":
					btn = (Button)TroubleshootingDoneLargeTextButton;
					break;
				case "ObservationsTextBox":
					btn = (Button)ObservationsLargeTextButton;
					break;
			}
			
			btn.Enabled = !string.IsNullOrEmpty(tb.Text);
		}
		
		void ContractorToFixFault_NameTextChanged(object sender, EventArgs e) {
			ContractorToFixFault_PhoneNumberTextBox.ReadOnly = string.IsNullOrWhiteSpace(ContractorToFixFault_NameTextBox.Text);
		}
		
		void LargeTextButtonsClick(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			Button btn = (Button)sender;
			string lbl = string.Empty;
			TextBoxBase tb = null;
			switch(btn.Name) {
				case "CRQContactsLargeTextButton":
					tb = (TextBoxBase)CRQContactsTextBox;
					lbl = CRQContactsLabel.Text;
					break;
				case "WorkPerformedByFELargeTextButton":
					tb = (TextBoxBase)WorkPerformedByFETextBox;
					lbl = WorkPerformedByFELabel.Text;
					break;
				case "TroubleshootingDoneLargeTextButton":
					tb = (TextBoxBase)TroubleshootingDoneTextBox;
					lbl = TroubleshootingDoneLabel.Text;
					break;
				case "ObservationsLargeTextButton":
					tb = (TextBoxBase)ObservationsTextBox;
					lbl = ObservationsLabel.Text;
					break;
			}
			
			AMTLargeTextForm enlarge = new AMTLargeTextForm(tb.Text,lbl,false);
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
			tb.Text = enlarge.finaltext;
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void LoadTemplateFromLog(object sender, EventArgs e) {
			var form = Application.OpenForms.OfType<MainForm>().First();
			form.Invoke((MethodInvoker)delegate { form.FillTemplateFromLog(currentTemplate); });
		}

		void ClearAllControls(object sender, EventArgs e)
		{
			SiteIdTextBox.Text = string.Empty;
			INCTextBox.Text = string.Empty;
			RegionTextBox.Text = string.Empty;
			PriorityTextBox.Text = string.Empty;
			INCTextBox.Text = string.Empty;
			CRQTextBox.Text = string.Empty;
			SiteIdTextBox.Text = string.Empty;
			RegionTextBox.Text = string.Empty;
			PriorityTextBox.Text = string.Empty;
			CRQContactsTextBox.Text = string.Empty;
			WorkPerformedByFETextBox.Text = string.Empty;
			TroubleshootingDoneTextBox.Text = string.Empty;
			ObservationsTextBox.Text = string.Empty;
			FEBookedIn_CalledANOCCheckBox.Checked = false;
			FEBookedIn_PhoneNumberTextBox.Text = string.Empty;
			FEBookedIn_NameTextBox.Text = string.Empty;
			ContractorToFixFault_WillReturnCheckBox.Checked = false;
			ContractorToFixFault_WillReturnDateTimePicker.Value = DateTime.Now;
			ContractorToFixFault_PhoneNumberTextBox.Text = string.Empty;
			ContractorToFixFault_NameTextBox.Text = string.Empty;
			SiteIdTextBox.Focus();
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
							if(currentSite.Incidents != null) {
								if(currentSite.Incidents.Count > 0) {
									MainMenu.INCsButton.Enabled = true;
									MainMenu.INCsButton.ForeColor = Color.DarkGreen;
									MainMenu.INCsButton.Text = "INCs (" + currentSite.Incidents.Count + ")";
								}
								else {
									MainMenu.INCsButton.Enabled = false;
									MainMenu.INCsButton.Text = "No INC history";
								}
							}
							return;
						}
						break;
					case "CRQs":
						if(currentSite.Changes == null) {
							currentSite.requestOIData("CRQ");
							if(currentSite.Changes != null) {
								if(currentSite.Changes.Count > 0) {
									MainMenu.CRQsButton.Enabled = true;
									MainMenu.CRQsButton.ForeColor = Color.DarkGreen;
									MainMenu.CRQsButton.Text = "CRQs (" + currentSite.Changes.Count + ")";
								}
								else {
									MainMenu.CRQsButton.Enabled = false;
									MainMenu.CRQsButton.Text = "No CRQ history";
								}
							}
							return;
						}
						break;
					case "BookIns":
						if(currentSite.Visits == null) {
							currentSite.requestOIData("Bookins");
							if(currentSite.Visits != null) {
								if(currentSite.Visits.Count > 0) {
									MainMenu.BookInsButton.Enabled = true;
									MainMenu.BookInsButton.ForeColor = Color.DarkGreen;
									MainMenu.BookInsButton.Text = "Book Ins List (" + currentSite.Visits.Count + ")";
								}
								else {
									MainMenu.BookInsButton.Enabled = false;
									MainMenu.BookInsButton.Text = "No Book In history";
								}
							}
							return;
						}
						break;
					case "ActiveAlarms":
						if(currentSite.Alarms == null) {
							currentSite.requestOIData("Alarms");
							if(currentSite.Alarms != null) {
								if(currentSite.Alarms.Count > 0) {
									MainMenu.ActiveAlarmsButton.Enabled = true;
									MainMenu.ActiveAlarmsButton.ForeColor = Color.DarkGreen;
									MainMenu.ActiveAlarmsButton.Text = "Active alarms (" + currentSite.Alarms.Count + ")";
								}
								else {
									MainMenu.ActiveAlarmsButton.Enabled = false;
									MainMenu.ActiveAlarmsButton.Text = "No alarms to display";
								}
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
			Name = "Failed CRQ Template GUI";
			Size = new Size(519, 629);
			Text = "Failed CRQ";
			Controls.Add(MainMenu);
			Controls.Add(INCLabel);
			Controls.Add(INCTextBox);
			Controls.Add(CRQLabel);
			Controls.Add(CRQTextBox);
			Controls.Add(SiteIdLabel);
			Controls.Add(SiteIdTextBox);
			Controls.Add(RegionLabel);
			Controls.Add(RegionTextBox);
			Controls.Add(PriorityLabel);
			Controls.Add(PriorityTextBox);
			Controls.Add(FEBookedInGroupBox);
			Controls.Add(CRQContactsLabel);
			Controls.Add(CRQContactsLargeTextButton);
			Controls.Add(CRQContactsTextBox);
			Controls.Add(WorkPerformedByFELabel);
			Controls.Add(WorkPerformedByFELargeTextButton);
			Controls.Add(WorkPerformedByFETextBox);
			Controls.Add(TroubleshootingDoneLabel);
			Controls.Add(TroubleshootingDoneLargeTextButton);
			Controls.Add(TroubleshootingDoneTextBox);
			Controls.Add(ContractorToFixFaultGroupBox);
			Controls.Add(ObservationsLabel);
			Controls.Add(ObservationsLargeTextButton);
			Controls.Add(ObservationsTextBox);
			ContractorToFixFaultGroupBox.SuspendLayout();
			FEBookedInGroupBox.SuspendLayout();
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
			copyToNewTemplateToolStripMenuItem.Click += LoadTemplateFromLog;
			// 
			// INCLabel
			// 
//			INCLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
//			INCLabel.Size = new Size(50, 20);
			INCLabel.Name = "INCLabel";
			INCLabel.Text = "INC";
			INCLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// INCTextBox
			// 
			INCTextBox.AcceptsTab = true;
			INCTextBox.Font = new Font("Courier New", 8.25F);
//			INCTextBox.Location = new Point(INCLabel.Right + 5, MainMenu.Bottom + 4);
//			INCTextBox.Size = new Size(125, 20);
			INCTextBox.MaxLength = 15;
			INCTextBox.Name = "INCTextBox";
			INCTextBox.TabIndex = 1;
			// 
			// CRQLabel
			// 
//			CRQLabel.Location = new Point(PaddingLeftRight, INCLabel.Bottom + 4);
//			CRQLabel.Size = new Size(50, 20);
			CRQLabel.Name = "CRQLabel";
			CRQLabel.Text = "CRQ";
			CRQLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// CRQTextBox
			// 
			CRQTextBox.Font = new Font("Courier New", 8.25F);
//			CRQTextBox.Location = new Point(CRQLabel.Right + 5, CRQLabel.Top);
//			CRQTextBox.Size = new Size(125, 20);
			CRQTextBox.MaxLength = 15;
			CRQTextBox.Name = "CRQTextBox";
			CRQTextBox.TabIndex = 3;
			// 
			// SiteIdLabel
			// 
//			SiteIdLabel.Location = new Point(INCTextBox.Right + 15, MainMenu.Bottom + 4);
//			SiteIdLabel.Size = new Size(60, 20);
			SiteIdLabel.Name = "SiteIdLabel";
			SiteIdLabel.Size = new Size(60, 20);
			SiteIdLabel.Text = "Site";
			SiteIdLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteIdTextBox
			// 
			SiteIdTextBox.AcceptsTab = true;
			SiteIdTextBox.Font = new Font("Courier New", 8.25F);
//			SiteIdTextBox.Location = new Point(SiteIdLabel.Right + 5, MainMenu.Bottom + 4);
//			SiteIdTextBox.Size = new Size(90, 20);
			SiteIdTextBox.MaxLength = 6;
			SiteIdTextBox.Name = "SiteIdTextBox";
			SiteIdTextBox.TabIndex = 2;
			SiteIdTextBox.KeyPress += SiteIdTextBoxKeyPress;
			// 
			// RegionLabel
			// 
//			RegionLabel.Location = new Point(CRQTextBox.Right + 15, SiteIdLabel.Bottom + 4);
//			RegionLabel.Size = new Size(60, 20);
			RegionLabel.Name = "RegionLabel";
			RegionLabel.Text = "Region";
			RegionLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RegionTextBox
			// 
			RegionTextBox.Font = new Font("Courier New", 8.25F);
//			RegionTextBox.Location = new Point(RegionLabel.Right + 5, RegionLabel.Top);
//			RegionTextBox.Size = new Size(250, 20);
			RegionTextBox.MaxLength = 5;
			RegionTextBox.Name = "RegionTextBox";
			RegionTextBox.ReadOnly = true;
			RegionTextBox.TabIndex = 78;
			// 
			// PriorityLabel
			// 
//			PriorityLabel.Location = new Point(SiteIdTextBox.Right + 10, MainMenu.Bottom + 4);
//			PriorityLabel.Size = new Size(45, 20);
			PriorityLabel.Name = "PriorityLabel";
			PriorityLabel.Text = "Priority";
			PriorityLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PriorityTextBox
			// 
			PriorityTextBox.Font = new Font("Courier New", 8.25F);
//			PriorityTextBox.Location = new Point(PriorityLabel.Right + 10, MainMenu.Bottom + 4);
//			PriorityTextBox.Size = new Size(95, 20);
			PriorityTextBox.Name = "PriorityTextBox";
			PriorityTextBox.ReadOnly = true;
			PriorityTextBox.TabIndex = 80;
			// 
			// FEBookedInGroupBox
			// 
			FEBookedInGroupBox.Controls.Add(FEBookedIn_CalledANOCCheckBox);
			FEBookedInGroupBox.Controls.Add(FEBookedIn_PhoneNumberLabel);
			FEBookedInGroupBox.Controls.Add(FEBookedIn_PhoneNumberTextBox);
			FEBookedInGroupBox.Controls.Add(FEBookedIn_NameLabel);
			FEBookedInGroupBox.Controls.Add(FEBookedIn_NameTextBox);
//			FEBookedInGroupBox.Location = new Point(PaddingLeftRight, CRQLabel.Bottom + 4);
//			FEBookedInGroupBox.Size = new Size(250, 100);
			FEBookedInGroupBox.Name = "FEBookedInGroupBox";
			FEBookedInGroupBox.TabIndex = 4;
			FEBookedInGroupBox.TabStop = false;
			FEBookedInGroupBox.Text = "FE Booked In";
			// 
			// CRQContactsLabel
			// 
//			CRQContactsLabel.Location = new Point(FEBookedInGroupBox.Right + 10, FEBookedInGroupBox.Top);
//			CRQContactsLabel.Size = new Size(210, 20);
			CRQContactsLabel.Name = "CRQContactsLabel";
			CRQContactsLabel.Text = "CRQ Contacts (Name/Phone #/E-mail)";
			CRQContactsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// CRQContactsTextBox
			// 
			CRQContactsTextBox.DetectUrls = false;
			CRQContactsTextBox.Font = new Font("Courier New", 8.25F);
//			CRQContactsTextBox.Location = new Point(FEBookedInGroupBox.Right + 10, CRQContactsLabel.Bottom + 3);
//			CRQContactsTextBox.Size = new Size(250, 77);
			CRQContactsTextBox.Name = "CRQContactsTextBox";
			CRQContactsTextBox.TabIndex = 82;
			CRQContactsTextBox.Text = "";
			// 
			// CRQContactsLargeTextButton
			// 
			CRQContactsLargeTextButton.Enabled = false;
//			CRQContactsLargeTextButton.Size = new Size(24, 20);
//			CRQContactsLargeTextButton.Location = new Point(CRQContactsTextBox.Right - CRQContactsLargeTextButton.Width, CRQContactsLabel.Top);
			CRQContactsLargeTextButton.Name = "CRQContactsLargeTextButton";
			CRQContactsLargeTextButton.TabIndex = 83;
			CRQContactsLargeTextButton.Text = "...";
			CRQContactsLargeTextButton.UseVisualStyleBackColor = true;
			// 
			// WorkPerformedByFELabel
			// 
//			WorkPerformedByFELabel.Location = new Point(PaddingLeftRight, FEBookedInGroupBox.Bottom + 4);
//			WorkPerformedByFELabel.Size = new Size(219, 20);
			WorkPerformedByFELabel.Name = "WorkPerformedByFELabel";
			WorkPerformedByFELabel.Text = "Work performed by FE(as described by eng)";
			WorkPerformedByFELabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// WorkPerformedByFETextBox
			// 
			WorkPerformedByFETextBox.DetectUrls = false;
			WorkPerformedByFETextBox.Font = new Font("Courier New", 8.25F);
//			WorkPerformedByFETextBox.Location = new Point(PaddingLeftRight, WorkPerformedByFELabel.Bottom + 4);
//			WorkPerformedByFETextBox.Size = new Size(250, 158);
			WorkPerformedByFETextBox.Name = "WorkPerformedByFETextBox";
			WorkPerformedByFETextBox.TabIndex = 10;
			WorkPerformedByFETextBox.Text = "";
			// 
			// WorkPerformedByFELargeTextButton
			// 
			WorkPerformedByFELargeTextButton.Enabled = false;
//			WorkPerformedByFELargeTextButton.Size = new Size(24, 20);
//			WorkPerformedByFELargeTextButton.Location = new Point(WorkPerformedByFETextBox.Right - WorkPerformedByFELargeTextButton.Width, WorkPerformedByFELabel.Top);
			WorkPerformedByFELargeTextButton.Name = "WorkPerformedByFELargeTextButton";
			WorkPerformedByFELargeTextButton.TabIndex = 11;
			WorkPerformedByFELargeTextButton.Text = "...";
			WorkPerformedByFELargeTextButton.UseVisualStyleBackColor = true;
			// 
			// TroubleshootingDoneLabel
			// 
//			TroubleshootingDoneLabel.Location = new Point(WorkPerformedByFELargeTextButton.Right + 10, WorkPerformedByFELabel.Top);
//			TroubleshootingDoneLabel.Size = new Size(220, 20);
			TroubleshootingDoneLabel.Name = "TroubleshootingDoneLabel";
			TroubleshootingDoneLabel.Text = "Troubleshooting done with FE on site";
			TroubleshootingDoneLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// TroubleshootingDoneTextBox
			// 
			TroubleshootingDoneTextBox.DetectUrls = false;
			TroubleshootingDoneTextBox.Font = new Font("Courier New", 8.25F);
//			TroubleshootingDoneTextBox.Location = new Point(WorkPerformedByFETextBox.Right + 10, TroubleshootingDoneLabel.Bottom + 4);
//			TroubleshootingDoneTextBox.Size = new Size(250, 158);
			TroubleshootingDoneTextBox.Name = "TroubleshootingDoneTextBox";
			TroubleshootingDoneTextBox.TabIndex = 12;
			TroubleshootingDoneTextBox.Text = "";
			// 
			// TroubleshootingDoneLargeTextButton
			// 
			TroubleshootingDoneLargeTextButton.Enabled = false;
//			TroubleshootingDoneLargeTextButton.Size = new Size(24, 20);
//			TroubleshootingDoneLargeTextButton.Location = new Point(TroubleshootingDoneTextBox.Right - TroubleshootingDoneLargeTextButton.Width, TroubleshootingDoneLabel.Top);
			TroubleshootingDoneLargeTextButton.Name = "TroubleshootingDoneLargeTextButton";
			TroubleshootingDoneLargeTextButton.TabIndex = 13;
			TroubleshootingDoneLargeTextButton.Text = "...";
			TroubleshootingDoneLargeTextButton.UseVisualStyleBackColor = true;
			// 
			// ContractorToFixFaultGroupBox
			// 
			ContractorToFixFaultGroupBox.Controls.Add(ContractorToFixFault_WillReturnCheckBox);
			ContractorToFixFaultGroupBox.Controls.Add(ContractorToFixFault_WillReturnDateTimePicker);
			ContractorToFixFaultGroupBox.Controls.Add(ContractorToFixFault_PhoneNumberLabel);
			ContractorToFixFaultGroupBox.Controls.Add(ContractorToFixFault_PhoneNumberTextBox);
			ContractorToFixFaultGroupBox.Controls.Add(ContractorToFixFault_NameLabel);
			ContractorToFixFaultGroupBox.Controls.Add(ContractorToFixFault_NameTextBox);
//			ContractorToFixFaultGroupBox.Location = new Point(PaddingLeftRight, WorkPerformedByFETextBox.Bottom + 4);
//			ContractorToFixFaultGroupBox.Size = new Size(510, 69);
			ContractorToFixFaultGroupBox.Name = "ContractorToFixFaultGroupBox";
			ContractorToFixFaultGroupBox.TabIndex = 12;
			ContractorToFixFaultGroupBox.TabStop = false;
			ContractorToFixFaultGroupBox.Text = "Contractor to fix the fault";
			// 
			// ObservationsLabel
			// 
//			ObservationsLabel.Location = new Point(PaddingLeftRight, ContractorToFixFaultGroupBox.Bottom + 4);
//			ObservationsLabel.Size = new Size(75, 20);
			ObservationsLabel.Name = "ObservationsLabel";
			ObservationsLabel.Text = "Observations";
			ObservationsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ObservationsTextBox
			// 
			ObservationsTextBox.DetectUrls = false;
			ObservationsTextBox.Font = new Font("Courier New", 8.25F);
//			ObservationsTextBox.Location = new Point(PaddingLeftRight, ObservationsLabel.Bottom + 4);
//			ObservationsTextBox.Size = new Size(510, 153);
			ObservationsTextBox.Name = "ObservationsTextBox";
			ObservationsTextBox.TabIndex = 18;
			ObservationsTextBox.Text = "";
			// 
			// ObservationsLargeTextButton
			// 
			ObservationsLargeTextButton.Enabled = false;
//			ObservationsLargeTextButton.Size = new Size(24, 20);
//			ObservationsLargeTextButton.Location = new Point(ObservationsTextBox.Right - ObservationsLargeTextButton.Width, ObservationsLabel.Top);
			ObservationsLargeTextButton.Name = "ObservationsLargeTextButton";
			ObservationsLargeTextButton.TabIndex = 19;
			ObservationsLargeTextButton.Text = "...";
			ObservationsLargeTextButton.UseVisualStyleBackColor = true;
			// 
			// FEBookedIn_NameLabel
			// 
			FEBookedIn_NameLabel.Location = new Point(6, 16);
			FEBookedIn_NameLabel.Name = "FEBookedIn_NameLabel";
			FEBookedIn_NameLabel.Size = new Size(50, 20);
			FEBookedIn_NameLabel.Text = "Name";
			FEBookedIn_NameLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// FEBookedIn_NameTextBox
			// 
			FEBookedIn_NameTextBox.Font = new Font("Courier New", 8.25F);
			FEBookedIn_NameTextBox.Location = new Point(66, 16);
			FEBookedIn_NameTextBox.Name = "FEBookedIn_NameTextBox";
			FEBookedIn_NameTextBox.Size = new Size(169, 20);
			FEBookedIn_NameTextBox.TabIndex = 5;
			// 
			// FEBookedIn_PhoneNumberLabel
			// 
			FEBookedIn_PhoneNumberLabel.Location = new Point(6, 42);
			FEBookedIn_PhoneNumberLabel.Name = "FEBookedIn_PhoneNumberLabel";
			FEBookedIn_PhoneNumberLabel.Size = new Size(50, 20);
			FEBookedIn_PhoneNumberLabel.Text = "Phone #";
			FEBookedIn_PhoneNumberLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// FEBookedIn_PhoneNumberTextBox
			// 
			FEBookedIn_PhoneNumberTextBox.Font = new Font("Courier New", 8.25F);
			FEBookedIn_PhoneNumberTextBox.Location = new Point(66, 42);
			FEBookedIn_PhoneNumberTextBox.Name = "FEBookedIn_PhoneNumberTextBox";
			FEBookedIn_PhoneNumberTextBox.Size = new Size(169, 20);
			FEBookedIn_PhoneNumberTextBox.TabIndex = 5;
			// 
			// FEBookedIn_CalledANOCCheckBox
			// 
			FEBookedIn_CalledANOCCheckBox.Location = new Point(6, 68);
			FEBookedIn_CalledANOCCheckBox.Name = "FEBookedIn_CalledANOCCheckBox";
			FEBookedIn_CalledANOCCheckBox.RightToLeft = RightToLeft.Yes;
			FEBookedIn_CalledANOCCheckBox.Size = new Size(229, 24);
			FEBookedIn_CalledANOCCheckBox.TabIndex = 6;
			FEBookedIn_CalledANOCCheckBox.Text = "FE called ANOC after the CRQ finished";
			FEBookedIn_CalledANOCCheckBox.TextAlign = ContentAlignment.MiddleRight;
			FEBookedIn_CalledANOCCheckBox.UseVisualStyleBackColor = true;
			// 
			// ContractorToFixFault_NameLabel
			// 
			ContractorToFixFault_NameLabel.Location = new Point(6, 16);
			ContractorToFixFault_NameLabel.Name = "ContractorToFixFault_NameLabel";
			ContractorToFixFault_NameLabel.Size = new Size(54, 20);
			ContractorToFixFault_NameLabel.Text = "Name";
			ContractorToFixFault_NameLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ContractorToFixFault_NameTextBox
			// 
			ContractorToFixFault_NameTextBox.AcceptsTab = true;
			ContractorToFixFault_NameTextBox.Font = new Font("Courier New", 8.25F);
			ContractorToFixFault_NameTextBox.Location = new Point(66, 16);
			ContractorToFixFault_NameTextBox.Name = "ContractorToFixFault_NameTextBox";
			ContractorToFixFault_NameTextBox.Size = new Size(169, 20);
			ContractorToFixFault_NameTextBox.TabIndex = 14;
			// 
			// ContractorToFixFault_PhoneNumberLabel
			// 
			ContractorToFixFault_PhoneNumberLabel.Location = new Point(260, 15);
			ContractorToFixFault_PhoneNumberLabel.Name = "ContractorToFixFault_PhoneNumberLabel";
			ContractorToFixFault_PhoneNumberLabel.Size = new Size(60, 20);
			ContractorToFixFault_PhoneNumberLabel.Text = "Phone #";
			ContractorToFixFault_PhoneNumberLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ContractorToFixFault_PhoneNumberTextBox
			// 
			ContractorToFixFault_PhoneNumberTextBox.AcceptsTab = true;
			ContractorToFixFault_PhoneNumberTextBox.Font = new Font("Courier New", 8.25F);
			ContractorToFixFault_PhoneNumberTextBox.Location = new Point(334, 16);
			ContractorToFixFault_PhoneNumberTextBox.MaxLength = 999999;
			ContractorToFixFault_PhoneNumberTextBox.Name = "ContractorToFixFault_PhoneNumberTextBox";
			ContractorToFixFault_PhoneNumberTextBox.Size = new Size(168, 20);
			ContractorToFixFault_PhoneNumberTextBox.TabIndex = 15;
			// 
			// ContractorToFixFault_WillReturnCheckBox
			// 
			ContractorToFixFault_WillReturnCheckBox.Location = new Point(6, 42);
			ContractorToFixFault_WillReturnCheckBox.Name = "ContractorToFixFault_WillReturnCheckBox";
			ContractorToFixFault_WillReturnCheckBox.RightToLeft = RightToLeft.Yes;
			ContractorToFixFault_WillReturnCheckBox.Size = new Size(318, 20);
			ContractorToFixFault_WillReturnCheckBox.TabIndex = 16;
			ContractorToFixFault_WillReturnCheckBox.Text = "When will the contractor return to fix the fault, provided by FE";
			ContractorToFixFault_WillReturnCheckBox.TextAlign = ContentAlignment.MiddleRight;
			ContractorToFixFault_WillReturnCheckBox.UseVisualStyleBackColor = true;
			ContractorToFixFault_WillReturnCheckBox.CheckedChanged += ContractorToFixFault_WillReturnCheckBoxCheckedChanged;
			// 
			// ContractorToFixFault_WillReturnDateTimePicker
			// 
			ContractorToFixFault_WillReturnDateTimePicker.Checked = false;
			ContractorToFixFault_WillReturnDateTimePicker.CustomFormat = "dd/MM/yyyy HH:mm";
			ContractorToFixFault_WillReturnDateTimePicker.Format = DateTimePickerFormat.Custom;
			ContractorToFixFault_WillReturnDateTimePicker.Location = new Point(334, 42);
			ContractorToFixFault_WillReturnDateTimePicker.Size = new Size(168, 20);
			ContractorToFixFault_WillReturnDateTimePicker.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
			ContractorToFixFault_WillReturnDateTimePicker.Name = "ContractorToFixFault_WillReturnDateTimePicker";
			ContractorToFixFault_WillReturnDateTimePicker.RightToLeft = RightToLeft.No;
			ContractorToFixFault_WillReturnDateTimePicker.TabIndex = 17;
			ContractorToFixFault_WillReturnDateTimePicker.Value = DateTime.Now;
			ContractorToFixFault_WillReturnDateTimePicker.Visible = false;
			ContractorToFixFaultGroupBox.ResumeLayout(false);
			ContractorToFixFaultGroupBox.PerformLayout();
			FEBookedInGroupBox.ResumeLayout(false);
			FEBookedInGroupBox.PerformLayout();
			SuspendLayout();
			ResumeLayout(false);
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			INCLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			INCLabel.Size = new Size(50, 20);
			
			INCTextBox.Location = new Point(INCLabel.Right + 5, MainMenu.Bottom + 4);
			INCTextBox.Size = new Size(125, 20);
			
			CRQLabel.Location = new Point(PaddingLeftRight, INCLabel.Bottom + 4);
			CRQLabel.Size = new Size(50, 20);
			
			CRQTextBox.Location = new Point(CRQLabel.Right + 5, CRQLabel.Top);
			CRQTextBox.Size = new Size(125, 20);
			
			SiteIdLabel.Location = new Point(INCTextBox.Right + 15, MainMenu.Bottom + 4);
			SiteIdLabel.Size = new Size(60, 20);
			
			SiteIdTextBox.Location = new Point(SiteIdLabel.Right + 5, MainMenu.Bottom + 4);
			SiteIdTextBox.Size = new Size(90, 20);
			
			RegionLabel.Location = new Point(CRQTextBox.Right + 15, SiteIdLabel.Bottom + 4);
			RegionLabel.Size = new Size(60, 20);
			
			RegionTextBox.Location = new Point(RegionLabel.Right + 5, RegionLabel.Top);
			RegionTextBox.Size = new Size(250, 20);
			
			PriorityLabel.Location = new Point(SiteIdTextBox.Right + 10, MainMenu.Bottom + 4);
			PriorityLabel.Size = new Size(45, 20);
			
			PriorityTextBox.Location = new Point(PriorityLabel.Right + 10, MainMenu.Bottom + 4);
			PriorityTextBox.Size = new Size(95, 20);
			
			FEBookedInGroupBox.Location = new Point(PaddingLeftRight, CRQLabel.Bottom + 4);
			FEBookedInGroupBox.Size = new Size(250, 100);
			
			CRQContactsLabel.Location = new Point(FEBookedInGroupBox.Right + 10, FEBookedInGroupBox.Top);
			CRQContactsLabel.Size = new Size(210, 20);
			
			CRQContactsTextBox.Location = new Point(FEBookedInGroupBox.Right + 10, CRQContactsLabel.Bottom + 3);
			CRQContactsTextBox.Size = new Size(250, 77);
			
			CRQContactsLargeTextButton.Size = new Size(24, 20);
			CRQContactsLargeTextButton.Location = new Point(CRQContactsTextBox.Right - CRQContactsLargeTextButton.Width, CRQContactsLabel.Top);
			
			WorkPerformedByFELabel.Location = new Point(PaddingLeftRight, FEBookedInGroupBox.Bottom + 4);
			WorkPerformedByFELabel.Size = new Size(219, 20);
			
			WorkPerformedByFETextBox.Location = new Point(PaddingLeftRight, WorkPerformedByFELabel.Bottom + 4);
			WorkPerformedByFETextBox.Size = new Size(250, 158);
			
			WorkPerformedByFELargeTextButton.Size = new Size(24, 20);
			WorkPerformedByFELargeTextButton.Location = new Point(WorkPerformedByFETextBox.Right - WorkPerformedByFELargeTextButton.Width, WorkPerformedByFELabel.Top);
			
			TroubleshootingDoneLabel.Location = new Point(WorkPerformedByFELargeTextButton.Right + 10, WorkPerformedByFELabel.Top);
			TroubleshootingDoneLabel.Size = new Size(220, 20);
			
			TroubleshootingDoneTextBox.Location = new Point(WorkPerformedByFETextBox.Right + 10, TroubleshootingDoneLabel.Bottom + 4);
			TroubleshootingDoneTextBox.Size = new Size(250, 158);
			
			TroubleshootingDoneLargeTextButton.Size = new Size(24, 20);
			TroubleshootingDoneLargeTextButton.Location = new Point(TroubleshootingDoneTextBox.Right - TroubleshootingDoneLargeTextButton.Width, TroubleshootingDoneLabel.Top);
			
			ContractorToFixFaultGroupBox.Location = new Point(PaddingLeftRight, WorkPerformedByFETextBox.Bottom + 4);
			ContractorToFixFaultGroupBox.Size = new Size(510, 69);
			
			ObservationsLabel.Location = new Point(PaddingLeftRight, ContractorToFixFaultGroupBox.Bottom + 4);
			ObservationsLabel.Size = new Size(75, 20);
			
			ObservationsTextBox.Location = new Point(PaddingLeftRight, ObservationsLabel.Bottom + 4);
			ObservationsTextBox.Size = new Size(510, 153);
			
			ObservationsLargeTextButton.Size = new Size(24, 20);
			ObservationsLargeTextButton.Location = new Point(ObservationsTextBox.Right - ObservationsLargeTextButton.Width, ObservationsLabel.Top);
			
			Size = new Size(ObservationsTextBox.Right + PaddingLeftRight, ObservationsTextBox.Bottom + PaddingTopBottom);
		}
	}
}

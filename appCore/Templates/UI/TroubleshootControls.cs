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
using System.Windows.Forms;
using appCore.Settings;
using appCore.SiteFinder;
using appCore.Templates.Types;
using appCore.UI;
using appCore.SiteFinder.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of TroubleshootControls.
	/// </summary>
	public class TroubleshootControls : Panel
	{
		public Button AddressLargeTextButton = new Button();
		public Button MTXAddressButton = new Button();
		public Button TroubleshootLargeTextButton = new Button();
		public Button AlarmHistoryLargeTextButton = new Button();
		public Button ActiveAlarmsLargeTextButton = new Button();
		public CheckBox IntermittentIssueCheckBox = new CheckBox();
		public CheckBox PerformanceIssueCheckBox = new CheckBox();
		public CheckBox COOSCheckBox = new CheckBox();
		public CheckBox OtherSitesImpactedCheckBox = new CheckBox();
		public CheckBox FullSiteOutageCheckBox = new CheckBox();
		public ComboBox SiteOwnerComboBox = new ComboBox();
		public TextBox CCTRefTextBox = new TextBox();
		public TextBox TefSiteTextBox = new TextBox();
		public TextBox SiteIdTextBox = new TextBox();
		public TextBox INCTextBox = new TextBox();
		public TextBox RelatedINC_CRQTextBox = new TextBox();
		public TextBox PowerCompanyTextBox = new TextBox();
		public TextBox RegionTextBox = new TextBox();
		public AMTRichTextBox AddressTextBox = new AMTRichTextBox();
		public AMTRichTextBox ActiveAlarmsTextBox = new AMTRichTextBox();
		public AMTRichTextBox AlarmHistoryTextBox = new AMTRichTextBox();
		public AMTRichTextBox TroubleshootTextBox = new AMTRichTextBox();
		public NumericUpDown COOS4GNumericUpDown = new NumericUpDown();
		public NumericUpDown COOS3GNumericUpDown = new NumericUpDown();
		public NumericUpDown COOS2GNumericUpDown = new NumericUpDown();
		public Label COOS4GLabel = new Label();
		public Label COOS3GLabel = new Label();
		public Label COOS2GLabel = new Label();
		Label TefSiteLabel = new Label();
		Label TroubleshootLabel = new Label();
		Label AlarmHistoryLabel = new Label();
		Label ActiveAlarmsLabel = new Label();
		Label RelatedINC_CRQLabel = new Label();
		Label CCTRefLabel = new Label();
		Label AddressLabel = new Label();
		Label SiteOwnerLabel = new Label();
		Label SiteIdLabel = new Label();
		Label INCLabel = new Label();
		Label PowerCompanyLabel = new Label();
		Label RegionLabel = new Label();
		
		AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem SiteDetailsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateTemplateToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateTaskToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToNewTemplateToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem sendBCPToolStripMenuItem = new ToolStripMenuItem();
		
		public static siteDetails2 SiteDetailsUI;
		
		public Site currentSite { get; private set; }
		TroubleShoot currentTemplate;
		TroubleShoot prevTemp;
		
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
		
		bool toggled;
		public bool ToggledState {
			get {
				return toggled;
			}
			set {
				if(value != toggled){
					toggled = value;
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
					AddressTextBox.ReadOnly = true;
					OtherSitesImpactedCheckBox.Enabled = false;
					COOSCheckBox.Enabled = false;
					FullSiteOutageCheckBox.Enabled = false;
					COOS4GNumericUpDown.ReadOnly = true;
					COOS3GNumericUpDown.ReadOnly = true;
					COOS2GNumericUpDown.ReadOnly = true;
					IntermittentIssueCheckBox.Enabled = false;
					PerformanceIssueCheckBox.Enabled = false;
					SiteOwnerComboBox.Enabled = false;
					TefSiteTextBox.ReadOnly = true;
					CCTRefTextBox.ReadOnly = true;
					RelatedINC_CRQTextBox.Visible = false;
					ActiveAlarmsTextBox.ReadOnly = true;
					AlarmHistoryTextBox.ReadOnly = true;
					TroubleshootTextBox.ReadOnly = true;
					MTXAddressButton.Enabled = false;
					TroubleshootTextBox.Height = 183;
					
					MainMenu.MainMenu.DropDownItems.AddRange(new ToolStripItem[] {
					                                         	generateTemplateToolStripMenuItem,
					                                         	copyToNewTemplateToolStripMenuItem});
				}
				else {
					InitializeComponent();
					FullSiteOutageCheckBox.CheckedChanged += FullSiteOutageCheckedChanged;
					COOS2GNumericUpDown.ValueChanged += NumericUpDownValueChanged;
					COOS3GNumericUpDown.ValueChanged += NumericUpDownValueChanged;
					COOS4GNumericUpDown.ValueChanged += NumericUpDownValueChanged;
					COOS2GLabel.DoubleClick += COOSLabelDoubleClick;
					COOS3GLabel.DoubleClick += COOSLabelDoubleClick;
					COOS4GLabel.DoubleClick += COOSLabelDoubleClick;
					SiteIdTextBox.TextChanged += SiteIdTextBoxTextChanged;
					SiteIdTextBox.KeyPress += SiteIdTextBoxKeyPress;
					INCTextBox.KeyPress += INCTextBoxKeyPress;
					TroubleshootTextBox.Height = 203;
					
					MainMenu.InitializeTroubleshootMenu();
					MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
					MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
					
					MainMenu.MainMenu.DropDownItems.Add(generateTemplateToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(generateTaskToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(sendBCPToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(SiteDetailsToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
				}
				Size = new Size(TroubleshootTextBox.Right + PaddingLeftRight, TroubleshootTextBox.Bottom + PaddingTopBottom);
			}
		}
		
		public TroubleshootControls()
		{
			UiMode = Template.UIenum.Template;
			if(GlobalProperties.siteFinder_mainswitch)
				siteFinder_Toggle(false, false);
		}
		
		public TroubleshootControls(TroubleShoot template, Template.UIenum uimode = Template.UIenum.Log)
		{
			UiMode = uimode;
			currentTemplate = template;
			
			SiteIdTextBox.Text = currentTemplate.SiteId;
			if(UiMode == Template.UIenum.Template)
				SiteIdTextBoxKeyPress(SiteIdTextBox,new KeyPressEventArgs((char)Keys.Enter));
			INCTextBox.Text = currentTemplate.INC;
			AddressTextBox.Text = currentTemplate.SiteAddress;
			OtherSitesImpactedCheckBox.Checked = currentTemplate.OtherSitesImpacted;
			COOSCheckBox.Checked = currentTemplate.COOS;
			FullSiteOutageCheckBox.Checked = currentTemplate.FullSiteOutage;
			COOS4GNumericUpDown.Value = currentTemplate.COOS2G;
			COOS3GNumericUpDown.Value = currentTemplate.COOS3G;
			COOS2GNumericUpDown.Value = currentTemplate.COOS4G;
			IntermittentIssueCheckBox.Checked = currentTemplate.IntermittentIssue;
			PerformanceIssueCheckBox.Checked = currentTemplate.PerformanceIssue;
			SiteOwnerComboBox.Text = currentTemplate.SiteOwner;
			TefSiteTextBox.Text = currentTemplate.TefSiteId;
			CCTRefTextBox.Text = currentTemplate.CCTReference;
			RelatedINC_CRQTextBox.Text = currentTemplate.RelatedINC_CRQ;
			ActiveAlarmsTextBox.Text = currentTemplate.ActiveAlarms;
			AlarmHistoryTextBox.Text = currentTemplate.AlarmHistory;
			TroubleshootTextBox.Text = currentTemplate.Troubleshoot;
		}

		void siteFinder_Toggle(bool toggle, bool siteFound) {
			foreach (object ctrl in Controls) {
				switch(ctrl.GetType().ToString())
				{
					case "appCore.UI.AMTMenuStrip":
						MainMenu.siteFinder_Toggle(toggle, siteFound);
						break;
					case "System.Windows.Forms.Button":
						Button btn = ctrl as Button;
						if(btn.Text == "MTX")
							btn.Enabled = toggle;
						break;
					case "appCore.UI.AMTRichTextBox": case "System.Windows.Forms.TextBox":
						TextBoxBase tb = ctrl as TextBoxBase;
						if(tb.Name != "SiteIdTextBox" && tb.Name != "PowerCompanyTextBox" && tb.Name != "RegionTextBox")
							tb.Enabled = toggle;
						break;
					case "System.Windows.Forms.NumericUpDown":
						NumericUpDown nup = ctrl as NumericUpDown;
						nup.Enabled = toggle ? nup.Maximum < 999 || !siteFound : toggle;
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

			if (Convert.ToInt32(e.KeyChar) == 13) {
//				Stopwatch sw = new Stopwatch();
//
//				sw.Start();
				TextBox tb =(TextBox)sender;
				while(tb.Text.StartsWith("0"))
					tb.Text = tb.Text.Substring(1);
				currentSite = Finder.getSite(tb.Text);
				if(currentSite.Exists) {
					currentSite.requestOIData("INCCRQPWR");
					AddressTextBox.Text = currentSite.Address;
					RegionTextBox.Text = currentSite.Region;
					PowerCompanyTextBox.Text = currentSite.PowerCompany;
					if(currentSite.HostedBy.Contains("TF") || currentSite.HostedBy.Contains("O2")) {
						SiteOwnerComboBox.Text = "TF";
						TefSiteTextBox.Text = currentSite.SharedOperatorSiteID;
					}
					else {
						SiteOwnerComboBox.Text = "VF";
						TefSiteTextBox.Text = string.Empty;
					}
				}
				else {
					AddressTextBox.Text = string.Empty;
					PowerCompanyTextBox.Text = "No site found";
					COOS2GLabel.Text = COOS2GLabel.Text.Split('(')[0];
					COOS3GLabel.Text = COOS3GLabel.Text.Split('(')[0];
					COOS4GLabel.Text = COOS4GLabel.Text.Split('(')[0];
					if(SiteOwnerComboBox.Text == "TF")
						SiteOwnerComboBox.Text = "VF";
					MainMenu.INCsButton.Enabled = false;
				}
				siteFinder_Toggle(true, currentSite.Exists);
				generateTemplateToolStripMenuItem.Enabled = true;
				generateTaskToolStripMenuItem.Enabled = true;
				sendBCPToolStripMenuItem.Enabled = true;
				
				List<Cell> cellsFilter = currentSite.Cells.Filter(Cell.Filters.VF_2G);
				COOS2GLabel.Text = "2G cells(" + cellsFilter.Count() + ")";
				COOS2GNumericUpDown.Maximum = cellsFilter.Any() ? cellsFilter.Count() : 999;
				COOS2GNumericUpDown.Value = 0;
				
				cellsFilter = currentSite.Cells.Filter(Cell.Filters.VF_3G);
				COOS3GLabel.Text = "3G cells(" + cellsFilter.Count() + ")";
				COOS3GNumericUpDown.Maximum = cellsFilter.Any() ? cellsFilter.Count() : 999;
				COOS3GNumericUpDown.Value = 0;

				cellsFilter = currentSite.Cells.Filter(Cell.Filters.VF_4G);
				COOS4GLabel.Text = "4G cells(" + cellsFilter.Count() + ")";
				COOS4GNumericUpDown.Maximum = cellsFilter.Any() ? cellsFilter.Count() : 999;
				COOS4GNumericUpDown.Value = 0;
				COOSCheckBox.Checked = false;
//				sw.Stop();
//				FlexibleMessageBox.Show("Elapsed=" + sw.Elapsed);
			}
		}

		void SiteIdTextBoxTextChanged(object sender, EventArgs e)
		{
			currentSite = null;
			siteFinder_Toggle(false, false);
			PowerCompanyTextBox.Text = string.Empty;
			RegionTextBox.Text = string.Empty;
			clearToolStripMenuItem.Enabled = !string.IsNullOrEmpty(SiteIdTextBox.Text);
		}

		void INCTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13) {
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

		void COOSCheckedChanged(object sender, EventArgs e)
		{
			if (COOSCheckBox.Checked) {
				for(int c = 2;c<5;c++) {
					NumericUpDown nupd = (NumericUpDown)Controls["COOS" + c + "GNumericUpDown"];
					Label lbl = (Label)Controls["COOS" + c + "GLabel"];
					lbl.Visible = true;
					nupd.Visible = true;
					nupd.Enabled = nupd.Maximum < 999;
				}
				FullSiteOutageCheckBox.Visible = true;
			}
			else {
				FullSiteOutageCheckBox.Visible = false;
				FullSiteOutageCheckBox.Checked = false;
				for(int c = 2;c<5;c++) {
					NumericUpDown nupd = (NumericUpDown)Controls["COOS" + c + "GNumericUpDown"];
					Label lbl = (Label)Controls["COOS" + c + "GLabel"];
					lbl.Visible = false;
					nupd.Visible = false;
				}
			}
		}
		
		void NumericUpDownValueChanged(object sender, EventArgs e)
		{
			int nupdTotal = 0;
			int nupdMaxed = 0;
			for(int c = 2;c<5;c++) {
				NumericUpDown nupd = (NumericUpDown)Controls["COOS" + c + "GNumericUpDown"];
				if(nupd.Maximum < 999) {
					nupdTotal++;
					if(nupd.Value == nupd.Maximum)
						nupdMaxed++;
				}
			}
			if(nupdMaxed == nupdTotal)
				if(!FullSiteOutageCheckBox.Checked)
					FullSiteOutageCheckBox.Checked = true;
		}

		void COOSLabelDoubleClick(object sender, EventArgs e)
		{
			Label lbl = (Label)sender;
			NumericUpDown nupd = null;
			switch(lbl.Name) {
				case "COOS2GLabel":
					nupd = COOS2GNumericUpDown;
					break;
				case "COOS3GLabel":
					nupd = COOS3GNumericUpDown;
					break;
				case "COOS4GLabel":
					nupd = COOS4GNumericUpDown;
					break;
			}
			if(nupd.Enabled) {
				int max = Convert.ToInt16(lbl.Text.Split('(')[1].Replace(")",string.Empty));
				if(nupd.Value < max)
					nupd.Value = max;
				else {
					if(nupd.Value == max)
						nupd.Value = 0;
				}
			}
		}

		void SiteOwnerComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (SiteOwnerComboBox.SelectedIndex == 1) {
				TefSiteTextBox.Visible = true;
				TefSiteLabel.Visible = true;
			}
			else {
				TefSiteTextBox.Visible = false;
				TefSiteTextBox.Text = string.Empty;
				TefSiteLabel.Visible = false;
			}
		}

		void OtherSitesImpactedCheckedChanged(object sender, EventArgs e)
		{
			for(int c=2;c<5;c++) {
				NumericUpDown nupd = (NumericUpDown)Controls["COOS" + c + "GNumericUpDown"];
				if(OtherSitesImpactedCheckBox.Checked) {
					Label lbl = (Label)Controls["COOS" + c + "GLabel"];
					lbl.Text = c + "G cells";
					nupd.Maximum = 9999;
					nupd.Enabled = true;
				}
				else {
					int max;
					switch(c) {
						case 2:
							max = currentSite.Cells.Filter(Cell.Filters.VF_2G).Count();
							break;
						case 3:
							max = currentSite.Cells.Filter(Cell.Filters.VF_3G).Count();
							break;
						default:
							max = currentSite.Cells.Filter(Cell.Filters.VF_4G).Count();
							break;
					}
					if(max == 0)
						nupd.Enabled = false;
					else
						nupd.Enabled = true;
					nupd.Maximum = max;
					Label lbl = (Label)Controls["COOS" + c + "GLabel"];
					lbl.Text = c + "G cells(" + max + ")";
				}
			}
		}
		
		void FullSiteOutageCheckedChanged(object sender, EventArgs e) {
			if(!OtherSitesImpactedCheckBox.Checked) {
				if(FullSiteOutageCheckBox.Checked) {
					if(GlobalProperties.siteFinder_mainswitch) {
						for(int c=2;c<5;c++) {
							NumericUpDown nupd = (NumericUpDown)Controls["COOS" + c + "GNumericUpDown"];
							if (nupd.Maximum < 999) {
								nupd.Value = nupd.Maximum;
								nupd.Enabled = false;
							}
						}
					}
				}
				else {
					for(int c=2;c<5;c++) {
						NumericUpDown nupd = (NumericUpDown)Controls["COOS" + c + "GNumericUpDown"];
						nupd.Value = 0;
						if (nupd.Maximum < 999) {
							nupd.Value = 0;
							nupd.Enabled = true;
						}
					}
				}
			}
		}

		void SiteDetailsButtonClick(object sender, EventArgs e)
		{
			if(SiteDetailsUI != null) {
				SiteDetailsUI.Close();
				SiteDetailsUI.Dispose();
			}
			SiteDetailsUI = new siteDetails2(currentSite);
			SiteDetailsUI.Show();
		}

		void TextBoxesTextChanged_LargeTextButtons(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			Button btn = null;
			switch(tb.Name) {
				case "AddressTextBox":
					btn = (Button)AddressLargeTextButton;
					break;
				case "ActiveAlarmsTextBox":
					btn = (Button)ActiveAlarmsLargeTextButton;
					break;
				case "AlarmHistoryTextBox":
					btn = (Button)AlarmHistoryLargeTextButton;
					break;
				case "TroubleshootTextBox":
					btn = (Button)TroubleshootLargeTextButton;
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
				case "AddressLargeTextButton":
					tb = (TextBoxBase)AddressTextBox;
					lbl = AddressLabel.Text;
					break;
				case "ActiveAlarmsLargeTextButton":
					tb = (TextBoxBase)ActiveAlarmsTextBox;
					lbl = ActiveAlarmsLabel.Text;
					break;
				case "AlarmHistoryLargeTextButton":
					tb = (TextBoxBase)AlarmHistoryTextBox;
					lbl = AlarmHistoryLabel.Text;
					break;
				case "TroubleshootLargeTextButton":
					tb = (TextBoxBase)TroubleshootTextBox;
					lbl = TroubleshootLabel.Text;
					break;
			}
			
			LargeTextForm enlarge = new LargeTextForm(tb.Text,lbl,false);
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
			tb.Text = enlarge.finaltext;
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void MTXAddressButtonClick(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			Form form = new Form();
			using (form)
			{
				form.AutoScaleDimensions = new SizeF(6F, 13F);
				form.AutoScaleMode = AutoScaleMode.Font;
				form.ClientSize = new Size(657, 275);
				form.StartPosition = FormStartPosition.CenterParent;
				form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
				form.Name = "MTXsForm";
				form.ShowIcon = false;
				form.Text = "MTX";
				
				ListView listView1 = new ListView();
				form.Controls.Add(listView1);
				
				listView1.Activation = ItemActivation.OneClick;
				listView1.AutoArrange = false;
				listView1.FullRowSelect = true;
				listView1.GridLines = true;
				listView1.HideSelection = false;
				listView1.MultiSelect = false;
				listView1.Name = "listView1";
				listView1.TabIndex = 0;
				listView1.UseCompatibleStateImageBehavior = false;
				listView1.Location = new Point(0, 1);
				listView1.Size = new Size(657, 275);
				listView1.View = View.Details;
				listView1.DoubleClick += MTXListViewDoubleClick;
				listView1.KeyPress += MTXListViewKeyPress;
				
				listView1.Columns.Add("Name").Width = -2;
				listView1.Columns.Add("MTX").Width = -2;
				listView1.Columns.Add("Address").Width = -2;
				
				string[] mtxs = Resources.MTX.Split('\n');
				
				foreach (string str in mtxs) {
					string [] parsed = str.Replace("\r", string.Empty).Replace("\\t","\t").Split('\t');
					listView1.Items.Add(new ListViewItem(new string[]{parsed[0],parsed[1],parsed[2]}));
				}
				form.ShowDialog();
			}
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void MTXListViewDoubleClick(object sender, EventArgs e)
		{
			ListView lv = (ListView)sender;
			ListViewItem lvItem = lv.SelectedItems[0];
			Form frm = lv.Parent as Form;
			
			if(lvItem != null)
			{
				AddressTextBox.Text = lvItem.SubItems[2].Text;
			}
			frm.Close();
		}

		void MTXListViewKeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				MTXListViewDoubleClick(sender, null);
			}
		}

		void ClearAllControls(object sender, EventArgs e)
		{
			SiteIdTextBox.Text = string.Empty;
			INCTextBox.Text = string.Empty;
			SiteOwnerComboBox.SelectedIndex = -1;
			TefSiteTextBox.Text = string.Empty;
			AddressTextBox.Text = string.Empty;
			CCTRefTextBox.Text = string.Empty;
			RelatedINC_CRQTextBox.Text = string.Empty;
			ActiveAlarmsTextBox.Text = string.Empty;
			AlarmHistoryTextBox.Text = string.Empty;
			TroubleshootTextBox.Text = string.Empty;
			OtherSitesImpactedCheckBox.Checked = false;
			COOSCheckBox.Checked = false;
			PerformanceIssueCheckBox.Checked = false;
			IntermittentIssueCheckBox.Checked = false;
			SiteDetailsToolStripMenuItem.Enabled = false;
			generateTemplateToolStripMenuItem.Enabled = false;
			generateTaskToolStripMenuItem.Enabled = false;
			sendBCPToolStripMenuItem.Enabled = false;
			clearToolStripMenuItem.Enabled = false;
			SiteIdTextBox.Focus();
		}
		
		void LoadTemplateFromLog(object sender, EventArgs e) {
			// TODO: LoadTemplateFromLog
//			MainForm.FillTemplateFromLog(currentTemplate);
//			TabControl tb1 = (TabControl)MainForm.Controls["tabControl1"];
//			TabControl tb2 = (TabControl)MainForm.Controls["tabControl2"];
//			tb1.SelectTab(1);
//			tb2.SelectTab(4);
		}

		void GenerateTaskNotes(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "TasksForm") {
					frm.Activate();
					DialogResult ans = MessageBox.Show("Task Notes Generator is already open, in order to open the requested Task Notes Generator, the previous must be closed.\n\nDo you want to close?","Task Notes Generator",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
					if (ans == DialogResult.Yes) frm.Close();
					else return;
					break;
				}
			}
			
			string errmsg = "";
			if (string.IsNullOrEmpty(INCTextBox.Text)) {
				errmsg += "         - INC/Ticket Number missing\n";
			}
			if (string.IsNullOrEmpty(SiteIdTextBox.Text)) {
				errmsg += "         - Site ID missing\n";
			}
			if (SiteOwnerComboBox.SelectedIndex == 1 && string.IsNullOrEmpty(TefSiteTextBox.Text)) {
				errmsg += "          - TF Site ID missing\n";
			}
			if (string.IsNullOrEmpty(AddressTextBox.Text)) {
				errmsg += "         - Site Address missing\n";
			}
			if (!string.IsNullOrEmpty(errmsg)) {
				MessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			TasksForm Tasks = new TasksForm();
			TasksForm.siteID = SiteIdTextBox.Text;
			TasksForm.siteAddress = AddressTextBox.Text;
			TasksForm.powerCompany = PowerCompanyTextBox.Text;
			TasksForm.cct = CCTRefTextBox.Text;
			TasksForm.siteTEF = TefSiteTextBox.Text;
			TasksForm.relatedINC = RelatedINC_CRQTextBox.Text;
			Tasks.StartPosition = FormStartPosition.CenterParent;
			Tasks.Show();
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,true,this);
		}

		void SendBCPForm(object sender, EventArgs e) {
			if(prevTemp != null) {
				if(currentTemplate == prevTemp) {
					SendBCP bcp = new SendBCP(ref currentTemplate);
					bcp.ShowDialog();
					
//				currentTemplate.AddBcpLog(bcp.mailBody);
					
					MainForm.logFile.HandleLog(currentTemplate, true);
				}
				else
					FlexibleMessageBox.Show("You must generate the Template first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
				FlexibleMessageBox.Show("You must generate the Template first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		
		void GenerateTemplate(object sender, EventArgs e) {
			if(UiMode == Template.UIenum.Template) {
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
				if (SiteOwnerComboBox.SelectedIndex == 1 && string.IsNullOrEmpty(TefSiteTextBox.Text)) {
					errmsg += "          - TF Site ID missing\n";
				}
				if (string.IsNullOrEmpty(AddressTextBox.Text)) {
					errmsg += "         - Site Address missing\n";
				}
				if (string.IsNullOrEmpty(ActiveAlarmsTextBox.Text)) {
					errmsg += "         - Active alarms missing\n";
				}
				if (string.IsNullOrEmpty(TroubleshootTextBox.Text)) {
					errmsg += "         - Troubleshoot missing\n";
				}
				if (COOSCheckBox.Checked) {
					if ((COOS2GNumericUpDown.Value == 0) && (COOS3GNumericUpDown.Value == 0) && (COOS4GNumericUpDown.Value == 0)) {
						errmsg += "         - COOS count missing\n";
					}
				}
				if (!string.IsNullOrEmpty(errmsg)) {
					MessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			
			if(currentTemplate != null)
				currentTemplate = null;
			currentTemplate = new TroubleShoot(Controls);
			
			if(UiMode == Template.UIenum.Template && prevTemp != null) {
				// No changes since the last template warning
				string errmsg = "";
				if(currentTemplate.ToString() != prevTemp.ToString()) {
					if (INCTextBox.Text == prevTemp.INC) {
						errmsg = "         - INC\n";
					}
					if (SiteIdTextBox.Text == prevTemp.SiteId) {
						errmsg += "         - Site ID\n";
					}
					if (SiteOwnerComboBox.Text == "TF" && TefSiteTextBox.Text == prevTemp.TefSiteId) {
						errmsg += "         - TF Site ID\n";
					}
					if (AddressTextBox.Text == prevTemp.SiteAddress) {
						errmsg += "         - Site Address\n";
					}
					if (CCTRefTextBox.Text != "" && CCTRefTextBox.Text == prevTemp.CCTReference) {
						errmsg += "         - CCT reference\n";
					}
					if (OtherSitesImpactedCheckBox.Checked && prevTemp.OtherSitesImpacted){
						errmsg += "         - Other sites impacted\n";
					}
					if (COOSCheckBox.Checked) {
						if (COOS2GNumericUpDown.Value > 0 && COOS2GNumericUpDown.Value == prevTemp.COOS2G){
							errmsg += "         - 2G COOS count\n";
						}
						if (COOS3GNumericUpDown.Value > 0 && COOS3GNumericUpDown.Value == prevTemp.COOS3G) {
							errmsg += "         - 3G COOS count\n";
						}
						if (COOS4GNumericUpDown.Value > 0 && COOS4GNumericUpDown.Value == prevTemp.COOS4G) {
							errmsg += "         - 4G COOS count\n";
						}
						if(FullSiteOutageCheckBox.Checked && prevTemp.FullSiteOutage)
							errmsg += "         - Full Site Outage flag\n";
					}
					if (PerformanceIssueCheckBox.Checked && prevTemp.PerformanceIssue) {
						errmsg += "         - Performance issue\n";
					}
					if (IntermittentIssueCheckBox.Checked && prevTemp.IntermittentIssue) {
						errmsg += "         - Intermittent issue\n";
					}
//					if (RelatedINC_CRQTextBox.Text != "" && RelatedINC_CRQTextBox.Text == prevTemp.RelatedINC_CRQ) {
//						errmsg += "         - Related INC/CRQ\n";
//					}
					if (ActiveAlarmsTextBox.Text == prevTemp.ActiveAlarms) {
						errmsg += "         - Active Alarms\n";
					}
					if (AlarmHistoryTextBox.Text != "" && AlarmHistoryTextBox.Text == prevTemp.AlarmHistory) {
						errmsg += "         - Alarm History\n";
					}
					if (TroubleshootTextBox.Text != "" && TroubleshootTextBox.Text == prevTemp.Troubleshoot) {
						errmsg += "         - Troubleshoot\n";
					}
					if (errmsg != "") {
						DialogResult ans = FlexibleMessageBox.Show("You haven't changed the following fields in the template:\n\n" + errmsg + "\nDo you want to continue anyway?","Same INC",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
						if (ans == DialogResult.No)
							return;
					}
				}
			}
			
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
				if(!sendBCPToolStripMenuItem.Enabled)
					sendBCPToolStripMenuItem.Enabled = true;
				
				// Store this template for future warning on no changes
				prevTemp = currentTemplate;
				
				MainForm.logFile.HandleLog(currentTemplate);
			}
		}
		
		public void siteFinderSwitch(string toState) {
			if (toState == "off") {
				SiteIdTextBox.TextChanged -= SiteIdTextBoxTextChanged;
				SiteIdTextBox.KeyPress -= SiteIdTextBoxKeyPress;
				OtherSitesImpactedCheckBox.CheckedChanged -= OtherSitesImpactedCheckedChanged;
				FullSiteOutageCheckBox.CheckedChanged -= FullSiteOutageCheckedChanged;
				COOS2GLabel.DoubleClick -= COOSLabelDoubleClick;
				COOS3GLabel.DoubleClick -= COOSLabelDoubleClick;
				COOS4GLabel.DoubleClick -= COOSLabelDoubleClick;
				siteFinder_Toggle(true,false);
			}
			else {
				INCTextBox.KeyPress += INCTextBoxKeyPress;
				SiteIdTextBox.TextChanged += SiteIdTextBoxTextChanged;
				SiteIdTextBox.KeyPress += SiteIdTextBoxKeyPress;
				OtherSitesImpactedCheckBox.CheckedChanged += OtherSitesImpactedCheckedChanged;
				FullSiteOutageCheckBox.CheckedChanged += FullSiteOutageCheckedChanged;
				COOS2GLabel.DoubleClick += COOSLabelDoubleClick;
				COOS3GLabel.DoubleClick += COOSLabelDoubleClick;
				COOS4GLabel.DoubleClick += COOSLabelDoubleClick;
				siteFinder_Toggle(false,false);
			}
			COOS2GLabel.Text = "2G cells";
			COOS3GLabel.Text = "3G cells";
			COOS4GLabel.Text = "4G cells";
		}
		
		void LoadDisplayOiDataTable(object sender, EventArgs e) {
			ToolStripMenuItem tsim = sender as ToolStripMenuItem;
//			if(e.Button == MouseButtons.Left) {
			if(currentSite.Exists) {
				System.Data.DataTable dt = new System.Data.DataTable();
				string dataToShow = string.Empty;
				switch (tsim.Name) {
					case "INCsButton":
						if(currentSite.INCs == null) {
							currentSite.requestOIData("INC");
							if(currentSite.INCs != null) {
								if(currentSite.INCs.Rows.Count > 0) {
									MainMenu.INCsButton.Enabled = true;
									MainMenu.INCsButton.ForeColor = Color.DarkGreen;
									MainMenu.INCsButton.Text = "INCs (" + currentSite.INCs.Rows.Count + ")";
								}
								else {
									MainMenu.INCsButton.Enabled = false;
									MainMenu.INCsButton.Text = "No INC history";
								}
							}
							return;
						}
						dataToShow = "INCs";
						dt = currentSite.INCs;
						break;
					case "CRQsButton":
						if(currentSite.CRQs == null) {
							currentSite.requestOIData("CRQ");
							if(currentSite.CRQs != null) {
								if(currentSite.CRQs.Rows.Count > 0) {
									MainMenu.CRQsButton.Enabled = true;
									MainMenu.CRQsButton.ForeColor = Color.DarkGreen;
									MainMenu.CRQsButton.Text = "CRQs (" + currentSite.CRQs.Rows.Count + ")";
								}
								else {
									MainMenu.CRQsButton.Enabled = false;
									MainMenu.CRQsButton.Text = "No CRQ history";
								}
							}
							return;
						}
						dataToShow = "CRQs";
						dt = currentSite.CRQs;
						break;
					case "BookInsButton":
						if(currentSite.BookIns == null) {
							currentSite.requestOIData("Bookins");
							if(currentSite.BookIns != null) {
								if(currentSite.BookIns.Rows.Count > 0) {
									MainMenu.BookInsButton.Enabled = true;
									MainMenu.BookInsButton.ForeColor = Color.DarkGreen;
									MainMenu.BookInsButton.Text = "Book Ins List (" + currentSite.BookIns.Rows.Count + ")";
								}
								else {
									MainMenu.BookInsButton.Enabled = false;
									MainMenu.BookInsButton.Text = "No Book In history";
								}
							}
							return;
						}
						dataToShow = "BookIns";
						dt = currentSite.BookIns;
						break;
					case "ActiveAlarmsButton":
						if(currentSite.ActiveAlarms == null) {
							currentSite.requestOIData("Alarms");
							if(currentSite.ActiveAlarms != null) {
								if(currentSite.ActiveAlarms.Rows.Count > 0) {
									MainMenu.ActiveAlarmsButton.Enabled = true;
									MainMenu.ActiveAlarmsButton.ForeColor = Color.DarkGreen;
									MainMenu.ActiveAlarmsButton.Text = "Active alarms (" + currentSite.ActiveAlarms.Rows.Count + ")";
								}
								else {
									MainMenu.ActiveAlarmsButton.Enabled = false;
									MainMenu.ActiveAlarmsButton.Text = "No alarms to display";
								}
							}
							return;
						}
						dataToShow = "ActiveAlarms";
						dt = currentSite.ActiveAlarms;
						break;
				}
				
				var fc = Application.OpenForms.OfType<OiSiteTablesForm>();
				Form openForm = null;
				
				foreach (Form frm in fc)
				{
					if(frm.Name.Contains(dataToShow)) {
						openForm = frm;
						break;
					}
				}
				if(openForm != null)
					openForm.Close();
				OiSiteTablesForm OiTable = new OiSiteTablesForm(dt, dataToShow);
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
			Controls.Add(MainMenu);
			Controls.Add(FullSiteOutageCheckBox);
			Controls.Add(RelatedINC_CRQTextBox);
			Controls.Add(RegionTextBox);
			Controls.Add(PowerCompanyTextBox);
			Controls.Add(AddressTextBox);
			Controls.Add(AddressLargeTextButton);
			Controls.Add(PowerCompanyLabel);
			Controls.Add(MTXAddressButton);
			Controls.Add(TroubleshootLargeTextButton);
			Controls.Add(AlarmHistoryLargeTextButton);
			Controls.Add(ActiveAlarmsLargeTextButton);
			Controls.Add(ActiveAlarmsTextBox);
			Controls.Add(AlarmHistoryTextBox);
			Controls.Add(TroubleshootTextBox);
			Controls.Add(COOS4GNumericUpDown);
			Controls.Add(COOS3GNumericUpDown);
			Controls.Add(COOS2GNumericUpDown);
			Controls.Add(COOS4GLabel);
			Controls.Add(COOS3GLabel);
			Controls.Add(COOS2GLabel);
			Controls.Add(TefSiteLabel);
			Controls.Add(TroubleshootLabel);
			Controls.Add(AlarmHistoryLabel);
			Controls.Add(ActiveAlarmsLabel);
			Controls.Add(RelatedINC_CRQLabel);
			Controls.Add(CCTRefLabel);
			Controls.Add(AddressLabel);
			Controls.Add(SiteOwnerLabel);
			Controls.Add(RegionLabel);
			Controls.Add(SiteIdLabel);
			Controls.Add(INCLabel);
			Controls.Add(IntermittentIssueCheckBox);
			Controls.Add(PerformanceIssueCheckBox);
			Controls.Add(COOSCheckBox);
			Controls.Add(OtherSitesImpactedCheckBox);
			Controls.Add(SiteOwnerComboBox);
			Controls.Add(CCTRefTextBox);
			Controls.Add(TefSiteTextBox);
			Controls.Add(SiteIdTextBox);
			Controls.Add(INCTextBox);
			Name = "Troubleshoot Template GUI";
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
			// generateTaskToolStripMenuItem
			// 
			generateTaskToolStripMenuItem.Name = "generateTaskToolStripMenuItem";
			generateTaskToolStripMenuItem.Text = "Generate Task notes...";
			generateTaskToolStripMenuItem.Click += GenerateTaskNotes;
			// 
			// copyToNewTemplateToolStripMenuItem
			// 
			copyToNewTemplateToolStripMenuItem.Name = "copyToNewTemplateToolStripMenuItem";
			copyToNewTemplateToolStripMenuItem.Text = "Copy to new Troubleshoot template";
			copyToNewTemplateToolStripMenuItem.Click += LoadTemplateFromLog;
			// 
			// sendBCPToolStripMenuItem
			// 
			sendBCPToolStripMenuItem.Name = "sendBCPToolStripMenuItem";
			sendBCPToolStripMenuItem.Text = "Send BCP Email...";
			sendBCPToolStripMenuItem.Click += SendBCPForm;
			// 
			// SiteIdLabel
			// 
			SiteIdLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			SiteIdLabel.Name = "SiteIdLabel";
			SiteIdLabel.Size = new Size(67, 20);
			SiteIdLabel.TabIndex = 56;
			SiteIdLabel.Text = "Site ID";
			SiteIdLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteIdTextBox
			// 
			SiteIdTextBox.Font = new Font("Courier New", 8.25F);
			SiteIdTextBox.Location = new Point(SiteIdLabel.Right + 2, MainMenu.Bottom + 4);
			SiteIdTextBox.Size = new Size(58, 20);
			SiteIdTextBox.MaxLength = 6;
			SiteIdTextBox.Name = "SiteIdTextBox";
			SiteIdTextBox.TabIndex = 2;
			// 
			// RegionLabel
			// 
			RegionLabel.Location = new Point(SiteIdTextBox.Right + 2, MainMenu.Bottom + 4);
			RegionLabel.Name = "RegionLabel";
			RegionLabel.Size = new Size(43, 20);
			RegionLabel.Text = "Region";
			RegionLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RegionTextBox
			// 
			RegionTextBox.Font = new Font("Courier New", 8.25F);
			RegionTextBox.Location = new Point(RegionLabel.Right + 2, MainMenu.Bottom + 4);
			RegionTextBox.MaxLength = 5;
			RegionTextBox.Name = "RegionTextBox";
			RegionTextBox.ReadOnly = true;
			RegionTextBox.Size = new Size(78, 20);
			RegionTextBox.TabIndex = 78;
			// 
			// SiteOwnerLabel
			// 
			SiteOwnerLabel.Location = new Point(PaddingLeftRight, SiteIdLabel.Bottom + 4);
			SiteOwnerLabel.Name = "SiteOwnerLabel";
			SiteOwnerLabel.Size = new Size(67, 20);
			SiteOwnerLabel.TabIndex = 59;
			SiteOwnerLabel.Text = "Site Owner";
			SiteOwnerLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteOwnerComboBox
			// 
			SiteOwnerComboBox.FormattingEnabled = true;
			SiteOwnerComboBox.ItemHeight = 13;
			SiteOwnerComboBox.Items.AddRange(new object[] {
			                                 	"VF",
			                                 	"TF"});
			SiteOwnerComboBox.Location = new Point(SiteOwnerLabel.Right + 2, SiteOwnerLabel.Top);
			SiteOwnerComboBox.Name = "SiteOwnerComboBox";
			SiteOwnerComboBox.Size = new Size(43, 21);
			SiteOwnerComboBox.TabIndex = 3;
			SiteOwnerComboBox.SelectedIndexChanged += SiteOwnerComboBoxSelectedIndexChanged;
			// 
			// TefSiteLabel
			// 
			TefSiteLabel.Location = new Point(SiteOwnerComboBox.Right + 2, SiteOwnerLabel.Top);
			TefSiteLabel.Name = "TefSiteLabel";
			TefSiteLabel.Size = new Size(43, 20);
			TefSiteLabel.TabIndex = 67;
			TefSiteLabel.Text = "TF Site";
			TefSiteLabel.TextAlign = ContentAlignment.MiddleLeft;
			TefSiteLabel.Visible = false;
			// 
			// TefSiteTextBox
			// 
			TefSiteTextBox.Font = new Font("Courier New", 8.25F);
			TefSiteTextBox.Location = new Point(TefSiteLabel.Right + 2, SiteOwnerLabel.Top);
			TefSiteTextBox.Name = "TefSiteTextBox";
			TefSiteTextBox.Size = new Size(91, 20);
			TefSiteTextBox.TabIndex = 4;
			TefSiteTextBox.Visible = false;
			// 
			// AddressLabel
			// 
			AddressLabel.Location = new Point(PaddingLeftRight, SiteOwnerLabel.Bottom + 4);
			AddressLabel.Name = "AddressLabel";
			AddressLabel.Size = new Size(67, 40);
			AddressLabel.TabIndex = 61;
			AddressLabel.Text = "Address";
			AddressLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// AddressTextBox
			// 
			AddressTextBox.DetectUrls = false;
			AddressTextBox.Font = new Font("Courier New", 8.25F);
			AddressTextBox.Location = new Point(AddressLabel.Right + 2, AddressLabel.Top);
			AddressTextBox.Name = "AddressTextBox";
			AddressTextBox.Size = new Size(157, 40);
			AddressTextBox.TabIndex = 78;
			AddressTextBox.Text = "";
			AddressTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// AddressLargeTextButton
			// 
			AddressLargeTextButton.Enabled = false;
			AddressLargeTextButton.Location = new Point(AddressTextBox.Right, AddressTextBox.Top);
			AddressLargeTextButton.Name = "AddressLargeTextButton";
			AddressLargeTextButton.Size = new Size(24, 20);
			AddressLargeTextButton.TabIndex = 75;
			AddressLargeTextButton.Text = "...";
			AddressLargeTextButton.UseVisualStyleBackColor = true;
			AddressLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// MTXAddressButton
			// 
			MTXAddressButton.Font = new Font("Microsoft Sans Serif", 3.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			MTXAddressButton.Location = new Point(AddressTextBox.Right, AddressLargeTextButton.Bottom);
			MTXAddressButton.Name = "MTXAddressButton";
			MTXAddressButton.Size = new Size(24, 20);
			MTXAddressButton.TabIndex = 6;
			MTXAddressButton.Text = "MTX";
			MTXAddressButton.UseVisualStyleBackColor = true;
			MTXAddressButton.Click += MTXAddressButtonClick;
			// 
			// CCTRefLabel
			// 
			CCTRefLabel.Location = new Point(PaddingLeftRight, AddressLabel.Bottom + 4);
			CCTRefLabel.Name = "CCTRefLabel";
			CCTRefLabel.Size = new Size(67, 20);
			CCTRefLabel.TabIndex = 62;
			CCTRefLabel.Text = "CCT ref.";
			CCTRefLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// CCTRefTextBox
			// 
			CCTRefTextBox.Font = new Font("Courier New", 8.25F);
			CCTRefTextBox.Location = new Point(CCTRefLabel.Right + 2, CCTRefLabel.Top);
			CCTRefTextBox.Name = "CCTRefTextBox";
			CCTRefTextBox.Size = new Size(181, 20);
			CCTRefTextBox.TabIndex = 7;
			// 
			// PowerCompanyLabel
			// 
			PowerCompanyLabel.Location = new Point(PaddingLeftRight, CCTRefLabel.Bottom + 4);
			PowerCompanyLabel.Name = "PowerCompanyLabel";
			PowerCompanyLabel.Size = new Size(67, 20);
			PowerCompanyLabel.TabIndex = 74;
			PowerCompanyLabel.Text = "Power Comp";
			PowerCompanyLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PowerCompanyTextBox
			// 
			PowerCompanyTextBox.Font = new Font("Courier New", 8.25F);
			PowerCompanyTextBox.Location = new Point(PowerCompanyLabel.Right + 2, PowerCompanyLabel.Top);
			PowerCompanyTextBox.MaxLength = 5;
			PowerCompanyTextBox.Name = "PowerCompanyTextBox";
			PowerCompanyTextBox.ReadOnly = true;
			PowerCompanyTextBox.Size = new Size(181, 20);
			PowerCompanyTextBox.TabIndex = 73;
			// 
			// INCLabel
			// 
			INCLabel.Location = new Point(PaddingLeftRight + 250 + 10, MainMenu.Bottom + 4);
			INCLabel.Name = "INCLabel";
			INCLabel.Size = new Size(67, 20);
			INCLabel.TabIndex = 54;
			INCLabel.Text = "INC";
			INCLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// INCTextBox
			// 
			INCTextBox.AcceptsTab = true;
			INCTextBox.Font = new Font("Courier New", 8.25F);
			INCTextBox.Location = new Point(INCLabel.Right + 2, MainMenu.Bottom + 4);
			INCTextBox.MaxLength = 15;
			INCTextBox.Name = "INCTextBox";
			INCTextBox.Size = new Size(181, 20);
			INCTextBox.TabIndex = 1;
			// 
			// OtherSitesImpactedCheckBox
			// 
			OtherSitesImpactedCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, INCLabel.Bottom + 2);
			OtherSitesImpactedCheckBox.Name = "OtherSitesImpactedCheckBox";
			OtherSitesImpactedCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			OtherSitesImpactedCheckBox.Size = new Size(123, 23);
			OtherSitesImpactedCheckBox.TabIndex = 8;
			OtherSitesImpactedCheckBox.Text = "Other sites impacted";
			OtherSitesImpactedCheckBox.TextAlign = ContentAlignment.MiddleRight;
			OtherSitesImpactedCheckBox.UseVisualStyleBackColor = true;
			// 
			// COOSCheckBox
			// 
			COOSCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, OtherSitesImpactedCheckBox.Bottom);
			COOSCheckBox.Name = "COOSCheckBox";
			COOSCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			COOSCheckBox.Size = new Size(123, 23);
			COOSCheckBox.TabIndex = 9;
			COOSCheckBox.Text = "COOS";
			COOSCheckBox.TextAlign = ContentAlignment.MiddleRight;
			COOSCheckBox.UseVisualStyleBackColor = true;
			COOSCheckBox.CheckedChanged += COOSCheckedChanged;
			// 
			// FullSiteOutageCheckBox
			// 
			FullSiteOutageCheckBox.CheckAlign = ContentAlignment.MiddleRight;
			FullSiteOutageCheckBox.Location = new Point(OtherSitesImpactedCheckBox.Right + 10, OtherSitesImpactedCheckBox.Top);
			FullSiteOutageCheckBox.Name = "FullSiteOutageCheckBox";
			FullSiteOutageCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			FullSiteOutageCheckBox.Size = new Size(103, 23);
			FullSiteOutageCheckBox.TabIndex = 80;
			FullSiteOutageCheckBox.Text = "Full site outage";
			FullSiteOutageCheckBox.TextAlign = ContentAlignment.MiddleRight;
			FullSiteOutageCheckBox.UseVisualStyleBackColor = true;
			FullSiteOutageCheckBox.Visible = false;
			// 
			// COOS2GLabel
			// 
			COOS2GLabel.Location = new Point(COOSCheckBox.Right + 5, FullSiteOutageCheckBox.Bottom);
			COOS2GLabel.Name = "COOS2GLabel";
			COOS2GLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			COOS2GLabel.Size = new Size(63, 20);
			COOS2GLabel.TabIndex = 68;
			COOS2GLabel.Text = "2G cells";
			COOS2GLabel.TextAlign = ContentAlignment.MiddleLeft;
			COOS2GLabel.Visible = false;
			// 
			// COOS2GNumericUpDown
			// 
			COOS2GNumericUpDown.Location = new Point(COOS2GLabel.Right + 2, COOS2GLabel.Top);
			COOS2GNumericUpDown.Maximum = new decimal(new int[] {
			                                          	999,
			                                          	0,
			                                          	0,
			                                          	0});
			COOS2GNumericUpDown.Name = "COOS2GNumericUpDown";
			COOS2GNumericUpDown.Size = new Size(59, 20);
			COOS2GNumericUpDown.TabIndex = 10;
			COOS2GNumericUpDown.Visible = false;
			// 
			// COOS3GLabel
			// 
			COOS3GLabel.Location = new Point(COOSCheckBox.Right + 5, COOS2GLabel.Bottom + 4);
			COOS3GLabel.Name = "COOS3GLabel";
			COOS3GLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			COOS3GLabel.Size = new Size(63, 20);
			COOS3GLabel.TabIndex = 69;
			COOS3GLabel.Text = "3G cells";
			COOS3GLabel.TextAlign = ContentAlignment.MiddleLeft;
			COOS3GLabel.Visible = false;
			// 
			// COOS3GNumericUpDown
			// 
			COOS3GNumericUpDown.Location = new Point(COOS3GLabel.Right + 2, COOS3GLabel.Top);
			COOS3GNumericUpDown.Maximum = new decimal(new int[] {
			                                          	999,
			                                          	0,
			                                          	0,
			                                          	0});
			COOS3GNumericUpDown.Name = "COOS3GNumericUpDown";
			COOS3GNumericUpDown.Size = new Size(59, 20);
			COOS3GNumericUpDown.TabIndex = 11;
			COOS3GNumericUpDown.Visible = false;
			// 
			// COOS4GLabel
			// 
			COOS4GLabel.Location = new Point(COOSCheckBox.Right + 5, COOS3GLabel.Bottom + 4);
			COOS4GLabel.Name = "COOS4GLabel";
			COOS4GLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			COOS4GLabel.Size = new Size(63, 20);
			COOS4GLabel.TabIndex = 70;
			COOS4GLabel.Text = "4G cells";
			COOS4GLabel.TextAlign = ContentAlignment.MiddleLeft;
			COOS4GLabel.Visible = false;
			// 
			// COOS4GNumericUpDown
			// 
			COOS4GNumericUpDown.Location = new Point(COOS4GLabel.Right + 2, COOS4GLabel.Top);
			COOS4GNumericUpDown.Maximum = new decimal(new int[] {
			                                          	999,
			                                          	0,
			                                          	0,
			                                          	0});
			COOS4GNumericUpDown.Name = "COOS4GNumericUpDown";
			COOS4GNumericUpDown.Size = new Size(59, 20);
			COOS4GNumericUpDown.TabIndex = 12;
			COOS4GNumericUpDown.Visible = false;
			// 
			// PerformanceIssueCheckBox
			// 
			PerformanceIssueCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, COOSCheckBox.Bottom);
			PerformanceIssueCheckBox.Name = "PerformanceIssueCheckBox";
			PerformanceIssueCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			PerformanceIssueCheckBox.Size = new Size(123, 23);
			PerformanceIssueCheckBox.TabIndex = 13;
			PerformanceIssueCheckBox.Text = "Performance Issue";
			PerformanceIssueCheckBox.TextAlign = ContentAlignment.MiddleRight;
			PerformanceIssueCheckBox.UseVisualStyleBackColor = true;
			// 
			// IntermittentIssueCheckBox
			// 
			IntermittentIssueCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, PerformanceIssueCheckBox.Bottom);
			IntermittentIssueCheckBox.Name = "IntermittentIssueCheckBox";
			IntermittentIssueCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			IntermittentIssueCheckBox.Size = new Size(123, 23);
			IntermittentIssueCheckBox.TabIndex = 14;
			IntermittentIssueCheckBox.Text = "Intermittent Issue";
			IntermittentIssueCheckBox.TextAlign = ContentAlignment.MiddleRight;
			IntermittentIssueCheckBox.UseVisualStyleBackColor = true;
			// 
			// RelatedINC_CRQLabel
			// 
			RelatedINC_CRQLabel.Location = new Point(PaddingLeftRight + 250 + 10, IntermittentIssueCheckBox.Bottom + 2);
			RelatedINC_CRQLabel.Name = "RealatedINC_CRQLabel";
			RelatedINC_CRQLabel.Size = new Size(105, 20);
			RelatedINC_CRQLabel.TabIndex = 63;
			RelatedINC_CRQLabel.Text = "Related INC/CRQ";
			RelatedINC_CRQLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RelatedINC_CRQTextBox
			// 
			RelatedINC_CRQTextBox.Font = new Font("Courier New", 8.25F);
			RelatedINC_CRQTextBox.Location = new Point(RelatedINC_CRQLabel.Right + 2, IntermittentIssueCheckBox.Bottom + 2);
			RelatedINC_CRQTextBox.Name = "RelatedINC_CRQTextBox";
			RelatedINC_CRQTextBox.Size = new Size(143, 20);
			RelatedINC_CRQTextBox.TabIndex = 15;
			// 
			// ActiveAlarmsLabel
			// 
			ActiveAlarmsLabel.Location = new Point(PaddingLeftRight, PowerCompanyLabel.Bottom + 4);
			ActiveAlarmsLabel.Name = "ActiveAlarmsLabel";
			ActiveAlarmsLabel.Size = new Size(77, 20);
			ActiveAlarmsLabel.TabIndex = 64;
			ActiveAlarmsLabel.Text = "Active Alarms";
			ActiveAlarmsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ActiveAlarmsTextBox
			// 
			ActiveAlarmsTextBox.DetectUrls = false;
			ActiveAlarmsTextBox.Font = new Font("Courier New", 8.25F);
			ActiveAlarmsTextBox.Location = new Point(PaddingLeftRight, ActiveAlarmsLabel.Bottom + 4);
			ActiveAlarmsTextBox.Name = "ActiveAlarmsTextBox";
			ActiveAlarmsTextBox.Size = new Size(250, 194);
			ActiveAlarmsTextBox.TabIndex = 16;
			ActiveAlarmsTextBox.Text = "";
			ActiveAlarmsTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// ActiveAlarmsLargeTextButton
			// 
			ActiveAlarmsLargeTextButton.Enabled = false;
			ActiveAlarmsLargeTextButton.Size = new Size(24, 20);
			ActiveAlarmsLargeTextButton.Location = new Point(ActiveAlarmsTextBox.Right - ActiveAlarmsLargeTextButton.Width, ActiveAlarmsLabel.Top);
			ActiveAlarmsLargeTextButton.Name = "ActiveAlarmsLargeTextButton";
			ActiveAlarmsLargeTextButton.TabIndex = 17;
			ActiveAlarmsLargeTextButton.Text = "...";
			ActiveAlarmsLargeTextButton.UseVisualStyleBackColor = true;
			ActiveAlarmsLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// AlarmHistoryLabel
			// 
			AlarmHistoryLabel.Location = new Point(ActiveAlarmsTextBox.Right + 10, ActiveAlarmsLabel.Top);
			AlarmHistoryLabel.Name = "AlarmHistoryLabel";
			AlarmHistoryLabel.Size = new Size(109, 20);
			AlarmHistoryLabel.TabIndex = 65;
			AlarmHistoryLabel.Text = "Alarm History";
			AlarmHistoryLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// AlarmHistoryTextBox
			// 
			AlarmHistoryTextBox.DetectUrls = false;
			AlarmHistoryTextBox.Font = new Font("Courier New", 8.25F);
			AlarmHistoryTextBox.Location = new Point(ActiveAlarmsTextBox.Right + 10, AlarmHistoryLabel.Bottom + 3);
			AlarmHistoryTextBox.Name = "AlarmHistoryTextBox";
			AlarmHistoryTextBox.Size = new Size(250, 194);
			AlarmHistoryTextBox.TabIndex = 18;
			AlarmHistoryTextBox.Text = "";
			AlarmHistoryTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// AlarmHistoryLargeTextButton
			// 
			AlarmHistoryLargeTextButton.Enabled = false;
			AlarmHistoryLargeTextButton.Size = new Size(24, 20);
			AlarmHistoryLargeTextButton.Location = new Point(AlarmHistoryTextBox.Right - AlarmHistoryLargeTextButton.Width, AlarmHistoryLabel.Top);
			AlarmHistoryLargeTextButton.Name = "AlarmHistoryLargeTextButton";
			AlarmHistoryLargeTextButton.TabIndex = 19;
			AlarmHistoryLargeTextButton.Text = "...";
			AlarmHistoryLargeTextButton.UseVisualStyleBackColor = true;
			AlarmHistoryLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// TroubleshootLabel
			// 
			TroubleshootLabel.Location = new Point(PaddingLeftRight, ActiveAlarmsTextBox.Bottom + 4);
			TroubleshootLabel.Name = "TroubleshootLabel";
			TroubleshootLabel.Size = new Size(109, 20);
			TroubleshootLabel.TabIndex = 66;
			TroubleshootLabel.Text = "Troubleshoot";
			TroubleshootLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// TroubleshootTextBox
			// 
			TroubleshootTextBox.DetectUrls = false;
			TroubleshootTextBox.Font = new Font("Courier New", 8.25F);
			TroubleshootTextBox.Size = new Size(510, 203);
			TroubleshootTextBox.Location = new Point(PaddingLeftRight, TroubleshootLabel.Bottom + 3);
			TroubleshootTextBox.Name = "TroubleshootTextBox";
			TroubleshootTextBox.TabIndex = 20;
			TroubleshootTextBox.Text = "";
			TroubleshootTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// TroubleshootLargeTextButton
			// 
			TroubleshootLargeTextButton.Enabled = false;
			TroubleshootLargeTextButton.Size = new Size(24, 20);
			TroubleshootLargeTextButton.Location = new Point(TroubleshootTextBox.Right - TroubleshootLargeTextButton.Width, TroubleshootLabel.Top);
			TroubleshootLargeTextButton.Name = "TroubleshootLargeTextButton";
			TroubleshootLargeTextButton.TabIndex = 21;
			TroubleshootLargeTextButton.Text = "...";
			TroubleshootLargeTextButton.UseVisualStyleBackColor = true;
			TroubleshootLargeTextButton.Click += LargeTextButtonsClick;
		}
	}
}
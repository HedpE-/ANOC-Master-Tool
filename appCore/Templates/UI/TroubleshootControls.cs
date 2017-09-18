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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using appCore.Settings;
using appCore.SiteFinder;
using appCore.Templates.Types;
using appCore.UI;
using appCore.SiteFinder.UI;
using appCore.OI.JSON;

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
		public AMTTextBox CCTRefTextBox = new AMTTextBox();
		public AMTTextBox TefSiteTextBox = new AMTTextBox();
		public AMTTextBox SiteIdTextBox = new AMTTextBox();
		public AMTTextBox INCTextBox = new AMTTextBox();
		public AMTTextBox RelatedINC_CRQTextBox = new AMTTextBox();
		public AMTTextBox PowerCompanyTextBox = new AMTTextBox();
		public AMTTextBox RegionTextBox = new AMTTextBox();
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
		
		public AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem SiteDetailsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateTemplateToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateTaskToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToNewTemplateToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem sendBcpToolStripMenuItem = new ToolStripMenuItem();
		
		public static siteDetails SiteDetailsUI;
		
		public Site currentSite;
		Troubleshoot currentTemplate;
		Troubleshoot prevTemp;
		
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

        bool fromLog = false; // flag used to know if constructor was called from LogEditor

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
					//TroubleshootTextBox.Height = 183;
					
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
					//TroubleshootTextBox.Height = 203;
					
					MainMenu.InitializeTroubleshootMenu();
					MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
					MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
					
					MainMenu.MainMenu.DropDownItems.Add(generateTemplateToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(generateTaskToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(sendBcpToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(SiteDetailsToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
					
					generateTemplateToolStripMenuItem.Enabled =
						generateTaskToolStripMenuItem.Enabled =
						sendBcpToolStripMenuItem.Enabled =
						SiteDetailsToolStripMenuItem.Enabled =
						clearToolStripMenuItem.Enabled = false;
				}
			}
		}
		
		public TroubleshootControls()
		{
			UiMode = UiEnum.Template;

            //fromLog = false;

			if(GlobalProperties.siteFinder_mainswitch)
            {
                siteFinder_Toggle(false, false);
                MainMenu.siteFinder_Toggle(false, false);
            }
		}
		
		public TroubleshootControls(Troubleshoot template, UiEnum uimode = UiEnum.Log)
		{
			UiMode = uimode;

            currentTemplate = template;
			
			SiteIdTextBox.Text = currentTemplate.SiteId;

            if (uimode == UiEnum.Template)
            {
                fromLog = true;
				SiteIdTextBoxKeyPress(SiteIdTextBox, new KeyPressEventArgs((char)Keys.Enter));
            }
            else
            {
                currentSite = currentTemplate.Site;
                PowerCompanyTextBox.Text = currentSite.PowerCompany;
                RegionTextBox.Text = currentSite.Region;
            }
			INCTextBox.Text = currentTemplate.INC;
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
			RelatedINC_CRQTextBox.Text = currentTemplate.RelatedINC_CRQ != "None" ? currentTemplate.RelatedINC_CRQ : string.Empty;
			ActiveAlarmsTextBox.Text = currentTemplate.ActiveAlarms;
			AlarmHistoryTextBox.Text = currentTemplate.AlarmHistory != "None related" ? currentTemplate.AlarmHistory : string.Empty;
			TroubleshootTextBox.Text = currentTemplate.TroubleShoot;
            AddressTextBox.Text = currentTemplate.SiteAddress;

            //			currentSite.requestOIData("INCCRQ");
            //			siteFinder_Toggle(true, currentSite.Exists);
        }

		async Task siteFinder_Toggle(bool toggle, bool siteFound)
        {
			foreach (object ctrl in Controls)
            {
				switch(ctrl.GetType().ToString())
				{
					//case "appCore.UI.AMTMenuStrip":
					//	break;
					case "System.Windows.Forms.Button":
						Button btn = ctrl as Button;
						if(btn.Text == "MTX")
							btn.Enabled = toggle;
						break;
					case "appCore.UI.AMTRichTextBox": case "appCore.UI.AMTTextBox":
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

        async void SiteIdTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)// && !tb.ReadOnly)
            {
                TextBox tb = (TextBox)sender;

                while (tb.Text.StartsWith("0"))
                    tb.Text = tb.Text.Substring(1);

                LoadingPanel loading = null;
                if (Parent != null)
                {
                    loading = new LoadingPanel();
                    loading.Show(true, this);
                }

                if (currentSite != null)
                {
                    if (tb.Text == currentSite.Id)
                    {
                        if (currentSite.Exists)
                        {
                            await MainMenu.ShowLoading();
                            await currentSite.requestOIDataAsync("INCCRQBookins");
                            await MainMenu.siteFinder_Toggle(true);
                        }
                        return;
                    }
                }

                tb.ReadOnly = true;

                currentSite = await DB.SitesDB.getSiteAsync(tb.Text);

                SiteDetailsToolStripMenuItem.Enabled = currentSite.Exists;

                if (loading != null)
                    loading.Close();

                if (currentSite.Exists)
                {
                    await MainMenu.ShowLoading();
                    await siteFinder_Toggle(true, true);

                    AddressTextBox.Text = await Task.Run(() => currentSite.Address);
                    RegionTextBox.Text = await Task.Run(() => currentSite.Region);

                    if (currentSite.Host.Contains("TF") || currentSite.Host.Contains("O2"))
                    {
                        SiteOwnerComboBox.Text = "TF";
                        TefSiteTextBox.Text = currentSite.SharedOperatorSiteID;
                    }
                    else
                    {
                        SiteOwnerComboBox.Text = "VF";
                        TefSiteTextBox.Text = string.Empty;
                    }

                    List<Cell> cellsFilter = await Task.Run(() => currentSite.Cells.Filter(Cell.Filters.VF_2G));
                    COOS2GLabel.Text = await Task.Run(() => "2G cells(" + cellsFilter.Count + ")");
                    COOS2GNumericUpDown.Maximum = await Task.Run(() => cellsFilter.Any() ? cellsFilter.Count : 999);
                    COOS2GNumericUpDown.Value = 0;

                    cellsFilter = await Task.Run(() => currentSite.Cells.Filter(Cell.Filters.VF_3G));
                    COOS3GLabel.Text = await Task.Run(() => "3G cells(" + cellsFilter.Count + ")");
                    COOS3GNumericUpDown.Maximum = await Task.Run(() => cellsFilter.Any() ? cellsFilter.Count : 999);
                    COOS3GNumericUpDown.Value = 0;

                    cellsFilter = await Task.Run(() => currentSite.Cells.Filter(Cell.Filters.VF_4G));
                    COOS4GLabel.Text = await Task.Run(() => "4G cells(" + cellsFilter.Count + ")");
                    COOS4GNumericUpDown.Maximum = await Task.Run(() => cellsFilter.Any() ? cellsFilter.Count : 999);
                    COOS4GNumericUpDown.Value = 0;
                    COOSCheckBox.Checked = false;

                    string dataToRequest = "INCBookins";
                    if ((DateTime.Now - currentSite.ChangesTimestamp) > new TimeSpan(0, 30, 0))
                        dataToRequest += "CRQ";
                    if (string.IsNullOrEmpty(currentSite.PowerCompany))
                        dataToRequest += "PWR";
                    if (currentSite.CramerData == null)
                        dataToRequest += "Cramer";
                    await currentSite.requestOIDataAsync(dataToRequest);

                    if (currentSite.CramerData != null)
                        CCTRefTextBox.Text = await Task.Run(() => currentSite.CramerData.TxLastMileRef);

                    //if (string.IsNullOrEmpty(PowerCompanyTextBox.Text))
                    PowerCompanyTextBox.Text = currentSite.PowerCompany;
                }
                else
                {
                    if(!fromLog)
                    {
                        AddressTextBox.Text = string.Empty;
                        SiteOwnerComboBox.Text = "VF";
                    }
                    PowerCompanyTextBox.Text = "No site found";
                    COOS2GLabel.Text = COOS2GLabel.Text.Split('(')[0];
                    COOS3GLabel.Text = COOS3GLabel.Text.Split('(')[0];
                    COOS4GLabel.Text = COOS4GLabel.Text.Split('(')[0];
                }
                await siteFinder_Toggle(true, currentSite.Exists);
                await MainMenu.siteFinder_Toggle(true, currentSite.Exists);

                tb.ReadOnly = false;

                generateTaskToolStripMenuItem.Enabled =
                    sendBcpToolStripMenuItem.Enabled =
                    generateTemplateToolStripMenuItem.Enabled = true;
            }
        }

        async void SiteIdTextBoxTextChanged(object sender, EventArgs e)
		{
            if(GlobalProperties.siteFinder_mainswitch)
            {
                if (currentSite != null)
                {
                    currentSite.Dispose(); // = null;
                    currentSite = null;
                }
			    await siteFinder_Toggle(false, false);
                await MainMenu.siteFinder_Toggle(false, false);
                PowerCompanyTextBox.Text =
				    RegionTextBox.Text =
				    AddressTextBox.Text =
				    CCTRefTextBox.Text = string.Empty;
            }
            else
            {
                generateTemplateToolStripMenuItem.Enabled =
                    generateTaskToolStripMenuItem.Enabled = !string.IsNullOrEmpty(SiteIdTextBox.Text);

            }
			clearToolStripMenuItem.Enabled = !string.IsNullOrEmpty(SiteIdTextBox.Text);
			sendBcpToolStripMenuItem.Enabled =
                fromLog = false;
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
//					nupd.Enabled = nupd.Maximum < 999;
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
			if(currentSite != null) {
				if(currentSite.Exists) {
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
			}
		}

		void COOSLabelDoubleClick(object sender, EventArgs e)
		{
			if(currentSite != null) {
				if(currentSite.Exists) {
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
			if(currentSite != null) {
				if(currentSite.Exists) {
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
			}
		}
		
		void FullSiteOutageCheckedChanged(object sender, EventArgs e) {
			if(currentSite != null) {
				if(currentSite.Exists) {
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

            LoadingPanel loading = new LoadingPanel();
            loading.Show(false, this.FindForm());
			AMTLargeTextForm enlarge = new AMTLargeTextForm(tb.Text,lbl,false);
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
            loading.Close();
			tb.Text = enlarge.finaltext;
		}

		void MTXAddressButtonClick(object sender, EventArgs e)
		{
            LoadingPanel loading = new LoadingPanel();
            loading.Show(false, this.FindForm());

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

            loading.Close();
		}

		void MTXListViewDoubleClick(object sender, EventArgs e)
		{
			ListView lv = (ListView)sender;
			ListViewItem lvItem = lv.SelectedItems[0];
			Form frm = lv.Parent as Form;
			
			if(lvItem != null)
				AddressTextBox.Text = lvItem.SubItems[2].Text;
			frm.Close();
		}

		void MTXListViewKeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
				MTXListViewDoubleClick(sender, null);
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
			sendBcpToolStripMenuItem.Enabled = false;
			clearToolStripMenuItem.Enabled = false;
            fromLog = false;
			SiteIdTextBox.Focus();
		}
		
		void LoadTemplateFromLog(object sender, EventArgs e) {
			var form = Application.OpenForms.OfType<MainForm>().First();
			form.Invoke((MethodInvoker)delegate { form.FillTemplateFromLog(currentTemplate); });
		}

		void GenerateTaskNotes(object sender, EventArgs e)
		{
			var fc = Application.OpenForms.OfType<TasksForm>().ToList();
			
			if(fc.Count > 0) {
				fc[0].Activate();
				DialogResult ans = FlexibleMessageBox.Show("Task Notes Generator is already open, in order to open the requested Task Notes Generator, the previous must be closed.\n\nDo you want to close?","Task Notes Generator",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
				if (ans == DialogResult.Yes)
					fc[0].Close();
				else
					return;
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
				FlexibleMessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			TasksForm Tasks = new TasksForm();
			Tasks.siteID = SiteIdTextBox.Text;
			Tasks.siteAddress = AddressTextBox.Text;
			Tasks.powerCompany = PowerCompanyTextBox.Text;
			Tasks.cct = CCTRefTextBox.Text;
			Tasks.siteTEF = TefSiteTextBox.Text;
			Tasks.relatedINC = RelatedINC_CRQTextBox.Text;
			Tasks.StartPosition = FormStartPosition.CenterParent;
			Tasks.Show();
		}

		void SendBCPForm(object sender, EventArgs e)
        {
            LoadingPanel loading = new LoadingPanel();
            loading.Show(false, FindForm());

			if(prevTemp != null) {
			    if(currentTemplate == prevTemp) {
			        SendBCP bcp = new SendBCP(ref currentTemplate);
			        bcp.ShowDialog();
			                                      			
//					currentTemplate.AddBcpLog(bcp.mailBody);
			                                      			
			        MainForm.logFiles.HandleLog(currentTemplate, true);
			    }
			    else
			        FlexibleMessageBox.Show("You must generate the Template first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			    FlexibleMessageBox.Show("You must generate the Template first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);

			loading.Close();
		}
		
		void GenerateTemplate(object sender, EventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);
            
            if (UiMode == UiEnum.Template)
            {
                string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(INCTextBox.Text, "INC");
                if (CompINC_CRQ != "error")
                    INCTextBox.Text = CompINC_CRQ;
                else
                {
                    FlexibleMessageBox.Show("INC number must only contain digits!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    load.Close();
                    return;
                }
                string errmsg = "";
                if (string.IsNullOrEmpty(INCTextBox.Text))
                {
                    errmsg = "         - INC/Ticket Number missing\n";
                }
                if (string.IsNullOrEmpty(SiteIdTextBox.Text))
                {
                    errmsg += "         - Site ID missing\n";
                }
                if (SiteOwnerComboBox.SelectedIndex == 1 && string.IsNullOrEmpty(TefSiteTextBox.Text))
                {
                    errmsg += "          - TF Site ID missing\n";
                }
                if (string.IsNullOrEmpty(AddressTextBox.Text))
                {
                    errmsg += "         - Site Address missing\n";
                }
                if (string.IsNullOrEmpty(ActiveAlarmsTextBox.Text))
                {
                    errmsg += "         - Active alarms missing\n";
                }
                if (string.IsNullOrEmpty(TroubleshootTextBox.Text))
                {
                    errmsg += "         - Troubleshoot missing\n";
                }
                if (COOSCheckBox.Checked)
                {
                    if ((COOS2GNumericUpDown.Value == 0) && (COOS3GNumericUpDown.Value == 0) && (COOS4GNumericUpDown.Value == 0))
                    {
                        errmsg += "         - COOS count missing\n";
                    }
                }
                if (!string.IsNullOrEmpty(errmsg))
                {
                    FlexibleMessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    load.Close();
                    return;
                }
            }

            string relatedCases = string.Empty;
            if (GlobalProperties.siteFinder_mainswitch)
            {
                if (currentSite.Exists)
                {
                    if(!CheckOngoingCRQs())
                    {
                        load.Close();
                        return;
                    }
                    DataTable OngoingCases = getCurrentCases();
                    if (OngoingCases.Rows.Count > 0)
                    {
                        OiSiteTablesForm relatedCasesForm = new OiSiteTablesForm(OngoingCases, currentSite.Id);
                        relatedCasesForm.StartPosition = FormStartPosition.CenterParent;
                        relatedCasesForm.ShowDialog();
                        if (relatedCasesForm.Cancel)
                        {
                            load.Close();
                            return;
                        }
                        if (relatedCasesForm.selectedCases.Count > 0)
                        {
                            int c = 0;
                            foreach (DataGridViewRow row in relatedCasesForm.selectedCases)
                            {
                                relatedCases += row.Cells["Reference"].Value + " - " + row.Cells["Summary"].Value;
                                if (row.Cells[1].Value.ToString() == "INC")
                                    relatedCases += " - " + row.Cells["Resolution"].Value;
                                relatedCases += " - " + row.Cells["Status"].Value;
                                if (++c < relatedCasesForm.selectedCases.Count)
                                    relatedCases += Environment.NewLine;
                            }
                        }
                    }
                }
            }
            
            currentTemplate = new Troubleshoot(Controls, relatedCases);

            if (UiMode == UiEnum.Template && prevTemp != null)
            {
                // No changes since the last template warning
                string errmsg = "";
                if (currentTemplate.ToString() != prevTemp.ToString())
                {
                    if (INCTextBox.Text == prevTemp.INC)
                    {
                        errmsg = "         - INC\n";
                    }
                    if (SiteIdTextBox.Text == prevTemp.SiteId)
                    {
                        errmsg += "         - Site ID\n";
                    }
                    if (SiteOwnerComboBox.Text == "TF" && TefSiteTextBox.Text == prevTemp.TefSiteId)
                    {
                        errmsg += "         - TF Site ID\n";
                    }
                    if (AddressTextBox.Text == prevTemp.SiteAddress)
                    {
                        errmsg += "         - Site Address\n";
                    }
                    if (CCTRefTextBox.Text != "" && CCTRefTextBox.Text == prevTemp.CCTReference)
                    {
                        errmsg += "         - CCT reference\n";
                    }
                    if (OtherSitesImpactedCheckBox.Checked && prevTemp.OtherSitesImpacted)
                    {
                        errmsg += "         - Other sites impacted\n";
                    }
                    if (COOSCheckBox.Checked)
                    {
                        if (COOS2GNumericUpDown.Value > 0 && COOS2GNumericUpDown.Value == prevTemp.COOS2G)
                        {
                            errmsg += "         - 2G COOS count\n";
                        }
                        if (COOS3GNumericUpDown.Value > 0 && COOS3GNumericUpDown.Value == prevTemp.COOS3G)
                        {
                            errmsg += "         - 3G COOS count\n";
                        }
                        if (COOS4GNumericUpDown.Value > 0 && COOS4GNumericUpDown.Value == prevTemp.COOS4G)
                        {
                            errmsg += "         - 4G COOS count\n";
                        }
                        if (FullSiteOutageCheckBox.Checked && prevTemp.FullSiteOutage)
                            errmsg += "         - Full Site Outage flag\n";
                    }
                    if (PerformanceIssueCheckBox.Checked && prevTemp.PerformanceIssue)
                    {
                        errmsg += "         - Performance issue\n";
                    }
                    if (IntermittentIssueCheckBox.Checked && prevTemp.IntermittentIssue)
                    {
                        errmsg += "         - Intermittent issue\n";
                    }
                    if (RelatedINC_CRQTextBox.Text != "" && RelatedINC_CRQTextBox.Text == prevTemp.RelatedINC_CRQ)
                    {
                        errmsg += "         - Related INC/CRQ\n";
                    }
                    if (ActiveAlarmsTextBox.Text == prevTemp.ActiveAlarms)
                    {
                        errmsg += "         - Active Alarms\n";
                    }
                    if (AlarmHistoryTextBox.Text != "" && AlarmHistoryTextBox.Text == prevTemp.AlarmHistory)
                    {
                        errmsg += "         - Alarm History\n";
                    }
                    if (TroubleshootTextBox.Text != "" && TroubleshootTextBox.Text == prevTemp.TroubleShoot)
                    {
                        errmsg += "         - Troubleshoot\n";
                    }
                    if (errmsg != "")
                    {
                        DialogResult ans = FlexibleMessageBox.Show("You haven't changed the following fields in the template:\n\n" + errmsg + "\nDo you want to continue anyway?", "Same INC", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (ans == DialogResult.No)
                        {
                            load.Close();
                            return;
                        }
                    }
                }
            }

            try
            {
                Clipboard.SetText(currentTemplate.ToString());
            }
            catch (Exception)
            {
                try
                {
                    Clipboard.SetText(currentTemplate.ToString());
                }
                catch (Exception)
                {
                    FlexibleMessageBox.Show("An error occurred while copying template to the clipboard, please try again.", "Clipboard error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            FlexibleMessageBox.Show(currentTemplate.ToString(), "Template copied to Clipboard", MessageBoxButtons.OK);

            if (UiMode == UiEnum.Template)
            {
                if (!sendBcpToolStripMenuItem.Enabled)
                    sendBcpToolStripMenuItem.Enabled = true;

                // Store this template for future warning on no changes
                prevTemp = currentTemplate;

                MainForm.logFiles.HandleLog(currentTemplate);
            }

			load.Close();
		}
		
		bool CheckOngoingCRQs() {
			if(currentSite.Changes != null) {
				List<Change> OngoingCRQs = new List<Change>();
				foreach(Change crq in currentSite.Changes) {
					if((crq.Status == "Scheduled" || crq.Status == "Implementation In Progress")) {
						if((Convert.ToDateTime(crq.Scheduled_Start) <= DateTime.Now && Convert.ToDateTime(crq.Scheduled_End) > DateTime.Now))
							OngoingCRQs.Add(crq);
					}
				}
				if(OngoingCRQs.Count > 0) {
					string OngoingCRQsStr = string.Empty;
					foreach (Change crq in OngoingCRQs) {
						OngoingCRQsStr += crq.Change_Ref + " - " + crq.Summary + " - " + crq.Project + " - " + crq.Status + " - " + crq.Scheduled_Start + " - " + crq.Scheduled_End;
						if(crq != OngoingCRQs.Last())
							OngoingCRQsStr += Environment.NewLine;
					}
					DialogResult ans = MessageBox.Show("Site has Ongoing CRQ(s):" + Environment.NewLine + Environment.NewLine + OngoingCRQsStr + Environment.NewLine + Environment.NewLine + "Generate template anyway?","Ongoing CRQs",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
					if(ans == DialogResult.No)
						return false;
				}
			}

            return true;
		}
		
		//// <summary>
		//// Gets INC's or CRQ's from site class
		//// </summary>
		//// <param name="type">"INC", "CRQ"</param>
		//// <returns></returns>
		DataTable getCurrentCases() {
			List<Incident> filteredINCs = null;
			List<Change> filteredCRQs = null;

            if (currentSite.Incidents.Count > 0)
		        filteredINCs = currentSite.Incidents.FindAll(s => s.Incident_Ref != INCTextBox.Text);

            if (currentSite.Changes.Count > 0) {
				filteredCRQs = currentSite.Changes;
				for(int c = 0;c < filteredCRQs.Count;c++) {
					Change crq = filteredCRQs[c];
					if(!string.IsNullOrEmpty(crq.Scheduled_Start) && !string.IsNullOrEmpty(crq.Scheduled_End)) {
						if (Convert.ToDateTime(crq.Scheduled_Start) > DateTime.Now) { // && Convert.ToDateTime(row["Scheduled End"]) < DateTime.Now) {// && Convert.ToDateTime(row["Scheduled End"]) >= DateTime.Now)) {
							filteredCRQs.RemoveAt(c);
							c--;
						}
					}
				}
			}
			
			DataTable currentCases = new DataTable();
			using(DataTable tempDT = new DataTable()) {
				tempDT.Columns.Add("Type");
				tempDT.Columns.Add("Reference");
				tempDT.Columns.Add("Summary");
				tempDT.Columns.Add("Status");
				tempDT.Columns.Add("Start Date", typeof(DateTime));
				tempDT.Columns.Add("Resolution");
				tempDT.Columns.Add("End Date", typeof(DateTime));
				
				if(filteredINCs != null) {
					foreach (Incident inc in filteredINCs)
						tempDT.Rows.Add("INC", inc.Incident_Ref, inc.Summary, inc.Status, inc.Submit_Date, inc.Resolution, inc.Resolved_Date);
					DataView dv = tempDT.DefaultView;
					dv.Sort = "Start Date desc";
					currentCases = dv.ToTable();
				}
				
				if(filteredCRQs != null) {
					tempDT.Rows.Clear();
					foreach(Change crq in filteredCRQs)
						tempDT.Rows.Add("CRQ", crq.Change_Ref, crq.Summary, crq.Status, crq.Scheduled_Start, string.Empty, crq.Scheduled_End);
					DataView dv = tempDT.DefaultView;
					dv.Sort = "Start Date desc";
					if(currentCases.Columns.Count > 0) {
						DataTable temp = dv.ToTable();
						foreach(DataRow row in temp.Rows)
							currentCases.Rows.Add(row.ItemArray);
					}
					else
						currentCases = dv.ToTable();
				}
			}
			
			return currentCases;
		}
		
		public void siteFinderSwitch(string toState) {
			if (toState == "off") {
				//SiteIdTextBox.TextChanged -= SiteIdTextBoxTextChanged;
				SiteIdTextBox.KeyPress -= SiteIdTextBoxKeyPress;
				OtherSitesImpactedCheckBox.CheckedChanged -= OtherSitesImpactedCheckedChanged;
				FullSiteOutageCheckBox.CheckedChanged -= FullSiteOutageCheckedChanged;
				COOS2GLabel.DoubleClick -= COOSLabelDoubleClick;
				COOS3GLabel.DoubleClick -= COOSLabelDoubleClick;
				COOS4GLabel.DoubleClick -= COOSLabelDoubleClick;
                //if(MainMenu.MainMenu.DropDownItems.Contains(SiteDetailsToolStripMenuItem))
                //{
                //    int siteDetailsToolStripMenuItemIndex = MainMenu.MainMenu.DropDownItems.IndexOf(SiteDetailsToolStripMenuItem);
                //    MainMenu.MainMenu.DropDownItems.RemoveAt(siteDetailsToolStripMenuItemIndex - 1); // Remove separator
                //    MainMenu.MainMenu.DropDownItems.RemoveAt(siteDetailsToolStripMenuItemIndex); // Remove MenuItem
                //}
				siteFinder_Toggle(true,false);
			}
			else {
				INCTextBox.KeyPress += INCTextBoxKeyPress;
				//SiteIdTextBox.TextChanged += SiteIdTextBoxTextChanged;
				SiteIdTextBox.KeyPress += SiteIdTextBoxKeyPress;
				OtherSitesImpactedCheckBox.CheckedChanged += OtherSitesImpactedCheckedChanged;
				FullSiteOutageCheckBox.CheckedChanged += FullSiteOutageCheckedChanged;
				COOS2GLabel.DoubleClick += COOSLabelDoubleClick;
				COOS3GLabel.DoubleClick += COOSLabelDoubleClick;
				COOS4GLabel.DoubleClick += COOSLabelDoubleClick;
                //if (!MainMenu.MainMenu.DropDownItems.Contains(SiteDetailsToolStripMenuItem))
                //{
                //    int sendBcpToolStripMenuItemIndex = MainMenu.MainMenu.DropDownItems.IndexOf(sendBcpToolStripMenuItem);
                //    MainMenu.MainMenu.DropDownItems.Insert(sendBcpToolStripMenuItemIndex + 1, new ToolStripMenuItem("-"));
                //    MainMenu.MainMenu.DropDownItems.Insert(sendBcpToolStripMenuItemIndex + 2, SiteDetailsToolStripMenuItem);
                //}
                siteFinder_Toggle(false,false);
			}
			COOS2GLabel.Text = "2G cells";
			COOS3GLabel.Text = "3G cells";
			COOS4GLabel.Text = "4G cells";
		}
		
		async void LoadDisplayOiDataTable(object sender, EventArgs e) {
//			if(e.Button == MouseButtons.Left) {
			string dataToShow = ((ToolStripMenuItem)sender).Name.Replace("Button", string.Empty);
			var fc = Application.OpenForms.OfType<OiSiteTablesForm>().Where(f => f.OwnerControl == this && f.Text.EndsWith(dataToShow)).ToList();
			if(fc.Count > 0) {
				fc[0].Close();
				fc[0].Dispose();
			}
			
			if(currentSite.Exists) {
				DataTable dt = new DataTable();
				switch(dataToShow) {
					case "INCs":
						if(currentSite.Incidents == null)
                        {
                            MainMenu.INCsButton.Text = "Loading data...";
                            MainMenu.INCsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("INC");
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
                        if (currentSite.Changes == null)
                        {
                            MainMenu.CRQsButton.Text = "Loading data...";
                            MainMenu.CRQsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("CRQ");
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
						if(currentSite.Visits == null)
                        {
                            MainMenu.BookInsButton.Text = "Loading data...";
                            MainMenu.BookInsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("Bookins");
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
						if(currentSite.Alarms == null)
                        {
                            MainMenu.ActiveAlarmsButton.Text = "Loading data...";
                            MainMenu.ActiveAlarmsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("Alarms");
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
		
		async void refreshOiData(object sender, EventArgs e)
        {
            MainMenu.ShowLoading();
            await currentSite.requestOIDataAsync("INCCRQBookinsAlarms");
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
			sendBcpToolStripMenuItem.Name = "sendBCPToolStripMenuItem";
			sendBcpToolStripMenuItem.Text = "Send BCP Email...";
			sendBcpToolStripMenuItem.Click += SendBCPForm;
			// 
			// SiteIdLabel
			// 
			SiteIdLabel.Name = "SiteIdLabel";
			SiteIdLabel.TabIndex = 56;
			SiteIdLabel.Text = "Site ID";
			SiteIdLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteIdTextBox
			// 
			SiteIdTextBox.Font = new Font("Courier New", 8.25F);
			SiteIdTextBox.MaxLength = 6;
			SiteIdTextBox.Name = "SiteIdTextBox";
			SiteIdTextBox.TabIndex = 2;
			// 
			// RegionLabel
			// 
			RegionLabel.Name = "RegionLabel";
			RegionLabel.Text = "Region";
			RegionLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RegionTextBox
			// 
			RegionTextBox.Font = new Font("Courier New", 8.25F);
			RegionTextBox.MaxLength = 5;
			RegionTextBox.Name = "RegionTextBox";
			RegionTextBox.ReadOnly = true;
			RegionTextBox.TabIndex = 78;
			// 
			// SiteOwnerLabel
			// 
			SiteOwnerLabel.Name = "SiteOwnerLabel";
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
			SiteOwnerComboBox.Name = "SiteOwnerComboBox";
			SiteOwnerComboBox.TabIndex = 3;
			SiteOwnerComboBox.SelectedIndexChanged += SiteOwnerComboBoxSelectedIndexChanged;
			// 
			// TefSiteLabel
			// 
			TefSiteLabel.Name = "TefSiteLabel";
			TefSiteLabel.TabIndex = 67;
			TefSiteLabel.Text = "TF Site";
			TefSiteLabel.TextAlign = ContentAlignment.MiddleLeft;
			TefSiteLabel.Visible = false;
			// 
			// TefSiteTextBox
			// 
			TefSiteTextBox.Font = new Font("Courier New", 8.25F);
			TefSiteTextBox.Name = "TefSiteTextBox";
			TefSiteTextBox.TabIndex = 4;
			TefSiteTextBox.Visible = false;
			// 
			// AddressLabel
			// 
			AddressLabel.Name = "AddressLabel";
			AddressLabel.TabIndex = 61;
			AddressLabel.Text = "Address";
			AddressLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// AddressTextBox
			// 
			AddressTextBox.DetectUrls = false;
			AddressTextBox.Font = new Font("Courier New", 8.25F);
			AddressTextBox.Name = "AddressTextBox";
			AddressTextBox.TabIndex = 78;
			AddressTextBox.Text = "";
			AddressTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// AddressLargeTextButton
			// 
			AddressLargeTextButton.Enabled = false;
			AddressLargeTextButton.Name = "AddressLargeTextButton";
			AddressLargeTextButton.TabIndex = 75;
			AddressLargeTextButton.Text = "...";
			AddressLargeTextButton.UseVisualStyleBackColor = true;
			AddressLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// MTXAddressButton
			// 
			MTXAddressButton.Font = new Font("Microsoft Sans Serif", 3.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			MTXAddressButton.Name = "MTXAddressButton";
			MTXAddressButton.TabIndex = 6;
			MTXAddressButton.Text = "MTX";
			MTXAddressButton.UseVisualStyleBackColor = true;
			MTXAddressButton.Click += MTXAddressButtonClick;
			// 
			// CCTRefLabel
			// 
			CCTRefLabel.Name = "CCTRefLabel";
			CCTRefLabel.TabIndex = 62;
			CCTRefLabel.Text = "CCT ref.";
			CCTRefLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// CCTRefTextBox
			// 
			CCTRefTextBox.Font = new Font("Courier New", 8.25F);
			CCTRefTextBox.Name = "CCTRefTextBox";
			CCTRefTextBox.TabIndex = 7;
			// 
			// PowerCompanyLabel
			// 
			PowerCompanyLabel.Name = "PowerCompanyLabel";
			PowerCompanyLabel.TabIndex = 74;
			PowerCompanyLabel.Text = "Power Comp";
			PowerCompanyLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PowerCompanyTextBox
			// 
			PowerCompanyTextBox.Font = new Font("Courier New", 8.25F);
			PowerCompanyTextBox.MaxLength = 5;
			PowerCompanyTextBox.Name = "PowerCompanyTextBox";
			PowerCompanyTextBox.ReadOnly = true;
			PowerCompanyTextBox.TabIndex = 73;
			// 
			// INCLabel
			// 
			INCLabel.Name = "INCLabel";
			INCLabel.TabIndex = 54;
			INCLabel.Text = "INC";
			INCLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// INCTextBox
			// 
			INCTextBox.AcceptsTab = true;
			INCTextBox.Font = new Font("Courier New", 8.25F);
			INCTextBox.MaxLength = 15;
			INCTextBox.Name = "INCTextBox";
			INCTextBox.TabIndex = 1;
			// 
			// OtherSitesImpactedCheckBox
			// 
			OtherSitesImpactedCheckBox.Name = "OtherSitesImpactedCheckBox";
			OtherSitesImpactedCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			OtherSitesImpactedCheckBox.TabIndex = 8;
			OtherSitesImpactedCheckBox.Text = "Other sites impacted";
			OtherSitesImpactedCheckBox.TextAlign = ContentAlignment.MiddleRight;
			OtherSitesImpactedCheckBox.UseVisualStyleBackColor = true;
			// 
			// COOSCheckBox
			// 
			COOSCheckBox.Name = "COOSCheckBox";
			COOSCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			COOSCheckBox.TabIndex = 9;
			COOSCheckBox.Text = "COOS";
			COOSCheckBox.TextAlign = ContentAlignment.MiddleRight;
			COOSCheckBox.UseVisualStyleBackColor = true;
			COOSCheckBox.CheckedChanged += COOSCheckedChanged;
			// 
			// FullSiteOutageCheckBox
			// 
			FullSiteOutageCheckBox.CheckAlign = ContentAlignment.MiddleRight;
			FullSiteOutageCheckBox.Name = "FullSiteOutageCheckBox";
			FullSiteOutageCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			FullSiteOutageCheckBox.TabIndex = 80;
			FullSiteOutageCheckBox.Text = "Full site outage";
			FullSiteOutageCheckBox.TextAlign = ContentAlignment.MiddleRight;
			FullSiteOutageCheckBox.UseVisualStyleBackColor = true;
			FullSiteOutageCheckBox.Visible = false;
			// 
			// COOS2GLabel
			// 
			COOS2GLabel.Name = "COOS2GLabel";
			COOS2GLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			COOS2GLabel.TabIndex = 68;
			COOS2GLabel.Text = "2G cells";
			COOS2GLabel.TextAlign = ContentAlignment.MiddleLeft;
			COOS2GLabel.Visible = false;
			// 
			// COOS2GNumericUpDown
			// 
			COOS2GNumericUpDown.Maximum = new decimal(new int[] {
			                                          	999,
			                                          	0,
			                                          	0,
			                                          	0});
			COOS2GNumericUpDown.Name = "COOS2GNumericUpDown";
			COOS2GNumericUpDown.TabIndex = 10;
			COOS2GNumericUpDown.Visible = false;
			// 
			// COOS3GLabel
			// 
			COOS3GLabel.Name = "COOS3GLabel";
			COOS3GLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			COOS3GLabel.TabIndex = 69;
			COOS3GLabel.Text = "3G cells";
			COOS3GLabel.TextAlign = ContentAlignment.MiddleLeft;
			COOS3GLabel.Visible = false;
			// 
			// COOS3GNumericUpDown
			// 
			COOS3GNumericUpDown.Maximum = new decimal(new int[] {
			                                          	999,
			                                          	0,
			                                          	0,
			                                          	0});
			COOS3GNumericUpDown.Name = "COOS3GNumericUpDown";
			COOS3GNumericUpDown.TabIndex = 11;
			COOS3GNumericUpDown.Visible = false;
			// 
			// COOS4GLabel
			// 
			COOS4GLabel.Name = "COOS4GLabel";
			COOS4GLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			COOS4GLabel.TabIndex = 70;
			COOS4GLabel.Text = "4G cells";
			COOS4GLabel.TextAlign = ContentAlignment.MiddleLeft;
			COOS4GLabel.Visible = false;
			// 
			// COOS4GNumericUpDown
			// 
			COOS4GNumericUpDown.Maximum = new decimal(new int[] {
			                                          	999,
			                                          	0,
			                                          	0,
			                                          	0});
			COOS4GNumericUpDown.Name = "COOS4GNumericUpDown";
			COOS4GNumericUpDown.TabIndex = 12;
			COOS4GNumericUpDown.Visible = false;
			// 
			// PerformanceIssueCheckBox
			// 
			PerformanceIssueCheckBox.Name = "PerformanceIssueCheckBox";
			PerformanceIssueCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			PerformanceIssueCheckBox.TabIndex = 13;
			PerformanceIssueCheckBox.Text = "Performance Issue";
			PerformanceIssueCheckBox.TextAlign = ContentAlignment.MiddleRight;
			PerformanceIssueCheckBox.UseVisualStyleBackColor = true;
			// 
			// IntermittentIssueCheckBox
			// 
			IntermittentIssueCheckBox.Name = "IntermittentIssueCheckBox";
			IntermittentIssueCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			IntermittentIssueCheckBox.TabIndex = 14;
			IntermittentIssueCheckBox.Text = "Intermittent Issue";
			IntermittentIssueCheckBox.TextAlign = ContentAlignment.MiddleRight;
			IntermittentIssueCheckBox.UseVisualStyleBackColor = true;
			// 
			// RelatedINC_CRQLabel
			// 
			RelatedINC_CRQLabel.Name = "RealatedINC_CRQLabel";
			RelatedINC_CRQLabel.TabIndex = 63;
			RelatedINC_CRQLabel.Text = "Related INC/CRQ";
			RelatedINC_CRQLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RelatedINC_CRQTextBox
			// 
			RelatedINC_CRQTextBox.Font = new Font("Courier New", 8.25F);
			RelatedINC_CRQTextBox.Name = "RelatedINC_CRQTextBox";
			RelatedINC_CRQTextBox.Size = new Size(143, 20);
			RelatedINC_CRQTextBox.TabIndex = 15;
			// 
			// ActiveAlarmsLabel
			// 
			ActiveAlarmsLabel.Name = "ActiveAlarmsLabel";
			ActiveAlarmsLabel.TabIndex = 64;
			ActiveAlarmsLabel.Text = "Active Alarms";
			ActiveAlarmsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ActiveAlarmsTextBox
			// 
			ActiveAlarmsTextBox.DetectUrls = false;
			ActiveAlarmsTextBox.Font = new Font("Courier New", 8.25F);
			ActiveAlarmsTextBox.Name = "ActiveAlarmsTextBox";
			ActiveAlarmsTextBox.TabIndex = 16;
			ActiveAlarmsTextBox.Text = "";
			ActiveAlarmsTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// ActiveAlarmsLargeTextButton
			// 
			ActiveAlarmsLargeTextButton.Enabled = false;
			ActiveAlarmsLargeTextButton.Name = "ActiveAlarmsLargeTextButton";
			ActiveAlarmsLargeTextButton.TabIndex = 17;
			ActiveAlarmsLargeTextButton.Text = "...";
			ActiveAlarmsLargeTextButton.UseVisualStyleBackColor = true;
			ActiveAlarmsLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// AlarmHistoryLabel
			// 
			AlarmHistoryLabel.Name = "AlarmHistoryLabel";
			AlarmHistoryLabel.TabIndex = 65;
			AlarmHistoryLabel.Text = "Alarm History";
			AlarmHistoryLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// AlarmHistoryTextBox
			// 
			AlarmHistoryTextBox.DetectUrls = false;
			AlarmHistoryTextBox.Font = new Font("Courier New", 8.25F);
			AlarmHistoryTextBox.Name = "AlarmHistoryTextBox";
			AlarmHistoryTextBox.TabIndex = 18;
			AlarmHistoryTextBox.Text = "";
			AlarmHistoryTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// AlarmHistoryLargeTextButton
			// 
			AlarmHistoryLargeTextButton.Enabled = false;
			AlarmHistoryLargeTextButton.Name = "AlarmHistoryLargeTextButton";
			AlarmHistoryLargeTextButton.TabIndex = 19;
			AlarmHistoryLargeTextButton.Text = "...";
			AlarmHistoryLargeTextButton.UseVisualStyleBackColor = true;
			AlarmHistoryLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// TroubleshootLabel
			// 
			TroubleshootLabel.Name = "TroubleshootLabel";
			TroubleshootLabel.TabIndex = 66;
			TroubleshootLabel.Text = "Troubleshoot";
			TroubleshootLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// TroubleshootTextBox
			// 
			TroubleshootTextBox.DetectUrls = false;
			TroubleshootTextBox.Font = new Font("Courier New", 8.25F);
			TroubleshootTextBox.Name = "TroubleshootTextBox";
			TroubleshootTextBox.TabIndex = 20;
			TroubleshootTextBox.Text = "";
			TroubleshootTextBox.TextChanged += TextBoxesTextChanged_LargeTextButtons;
			// 
			// TroubleshootLargeTextButton
			// 
			TroubleshootLargeTextButton.Enabled = false;
			TroubleshootLargeTextButton.Name = "TroubleshootLargeTextButton";
			TroubleshootLargeTextButton.TabIndex = 21;
			TroubleshootLargeTextButton.Text = "...";
			TroubleshootLargeTextButton.UseVisualStyleBackColor = true;
			TroubleshootLargeTextButton.Click += LargeTextButtonsClick;
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			SiteIdLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			SiteIdLabel.Size = new Size(67, 20);
			
			SiteIdTextBox.Location = new Point(SiteIdLabel.Right + 2, MainMenu.Bottom + 4);
			SiteIdTextBox.Size = new Size(58, 20);
			
			RegionLabel.Location = new Point(SiteIdTextBox.Right + 2, MainMenu.Bottom + 4);
			RegionLabel.Size = new Size(43, 20);
			
			RegionTextBox.Location = new Point(RegionLabel.Right + 2, MainMenu.Bottom + 4);
			RegionTextBox.Size = new Size(78, 20);
			
			SiteOwnerLabel.Location = new Point(PaddingLeftRight, SiteIdLabel.Bottom + 4);
			SiteOwnerLabel.Size = new Size(67, 20);
			
			SiteOwnerComboBox.Location = new Point(SiteOwnerLabel.Right + 2, SiteOwnerLabel.Top);
			SiteOwnerComboBox.Size = new Size(43, 21);
			
			TefSiteLabel.Location = new Point(SiteOwnerComboBox.Right + 2, SiteOwnerLabel.Top);
			TefSiteLabel.Size = new Size(43, 20);
			
			TefSiteTextBox.Location = new Point(TefSiteLabel.Right + 2, SiteOwnerLabel.Top);
			TefSiteTextBox.Size = new Size(91, 20);
			
			AddressLabel.Location = new Point(PaddingLeftRight, SiteOwnerLabel.Bottom + 4);
			AddressLabel.Size = new Size(67, 40);
			
			AddressTextBox.Location = new Point(AddressLabel.Right + 2, AddressLabel.Top);
			AddressTextBox.Size = new Size(157, 40);
			
			AddressLargeTextButton.Location = new Point(AddressTextBox.Right, AddressTextBox.Top);
			AddressLargeTextButton.Size = new Size(24, 20);
			
			MTXAddressButton.Location = new Point(AddressTextBox.Right, AddressLargeTextButton.Bottom);
			MTXAddressButton.Size = new Size(24, 20);
			
			CCTRefLabel.Location = new Point(PaddingLeftRight, AddressLabel.Bottom + 4);
			CCTRefLabel.Size = new Size(67, 20);
			
			CCTRefTextBox.Location = new Point(CCTRefLabel.Right + 2, CCTRefLabel.Top);
			CCTRefTextBox.Size = new Size(181, 20);
			
			PowerCompanyLabel.Location = new Point(PaddingLeftRight, CCTRefLabel.Bottom + 4);
			PowerCompanyLabel.Size = new Size(67, 20);
			
			PowerCompanyTextBox.Location = new Point(PowerCompanyLabel.Right + 2, PowerCompanyLabel.Top);
			PowerCompanyTextBox.Size = new Size(181, 20);
			
			INCLabel.Location = new Point(PaddingLeftRight + 250 + 10, MainMenu.Bottom + 4);
			INCLabel.Size = new Size(67, 20);
			
			INCTextBox.Location = new Point(INCLabel.Right + 2, MainMenu.Bottom + 4);
			INCTextBox.Size = new Size(181, 20);
			
			OtherSitesImpactedCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, INCLabel.Bottom + 2);
			OtherSitesImpactedCheckBox.Size = new Size(123, 23);
			
			COOSCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, OtherSitesImpactedCheckBox.Bottom);
			COOSCheckBox.Size = new Size(123, 23);
			
			FullSiteOutageCheckBox.Location = new Point(OtherSitesImpactedCheckBox.Right + 10, OtherSitesImpactedCheckBox.Top);
			FullSiteOutageCheckBox.Size = new Size(103, 23);
			
			COOS2GLabel.Location = new Point(COOSCheckBox.Right + 5, FullSiteOutageCheckBox.Bottom);
			COOS2GLabel.Size = new Size(63, 20);
			
			COOS2GNumericUpDown.Location = new Point(COOS2GLabel.Right + 2, COOS2GLabel.Top);
			COOS2GNumericUpDown.Size = new Size(59, 20);
			
			COOS3GLabel.Location = new Point(COOSCheckBox.Right + 5, COOS2GLabel.Bottom + 4);
			COOS3GLabel.Size = new Size(63, 20);
			
			COOS3GNumericUpDown.Location = new Point(COOS3GLabel.Right + 2, COOS3GLabel.Top);
			COOS3GNumericUpDown.Size = new Size(59, 20);
			
			COOS4GLabel.Location = new Point(COOSCheckBox.Right + 5, COOS3GLabel.Bottom + 4);
			COOS4GLabel.Size = new Size(63, 20);
			
			COOS4GNumericUpDown.Location = new Point(COOS4GLabel.Right + 2, COOS4GLabel.Top);
			COOS4GNumericUpDown.Size = new Size(59, 20);
			
			PerformanceIssueCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, COOSCheckBox.Bottom);
			PerformanceIssueCheckBox.Size = new Size(123, 23);
			
			IntermittentIssueCheckBox.Location = new Point(PaddingLeftRight + 250 + 10 - 2, PerformanceIssueCheckBox.Bottom);
			IntermittentIssueCheckBox.Size = new Size(123, 23);
			
			RelatedINC_CRQLabel.Location = new Point(PaddingLeftRight + 250 + 10, IntermittentIssueCheckBox.Bottom + 2);
			RelatedINC_CRQLabel.Size = new Size(105, 20);
			
			RelatedINC_CRQTextBox.Location = new Point(RelatedINC_CRQLabel.Right + 2, IntermittentIssueCheckBox.Bottom + 2);
			RelatedINC_CRQTextBox.Size = new Size(143, 20);
			
			ActiveAlarmsLabel.Location = new Point(PaddingLeftRight, PowerCompanyLabel.Bottom + 4);
			ActiveAlarmsLabel.Size = new Size(77, 20);
			
			ActiveAlarmsTextBox.Location = new Point(PaddingLeftRight, ActiveAlarmsLabel.Bottom + 4);
			ActiveAlarmsTextBox.Size = new Size(250, 194);
			
			ActiveAlarmsLargeTextButton.Size = new Size(24, 20);
			ActiveAlarmsLargeTextButton.Location = new Point(ActiveAlarmsTextBox.Right - ActiveAlarmsLargeTextButton.Width, ActiveAlarmsLabel.Top);
			
			AlarmHistoryLabel.Location = new Point(ActiveAlarmsTextBox.Right + 10, ActiveAlarmsLabel.Top);
			AlarmHistoryLabel.Size = new Size(109, 20);
			
			AlarmHistoryTextBox.Location = new Point(ActiveAlarmsTextBox.Right + 10, AlarmHistoryLabel.Bottom + 3);
			AlarmHistoryTextBox.Size = new Size(250, 194);
			
			AlarmHistoryLargeTextButton.Size = new Size(24, 20);
			AlarmHistoryLargeTextButton.Location = new Point(AlarmHistoryTextBox.Right - AlarmHistoryLargeTextButton.Width, AlarmHistoryLabel.Top);
			
			TroubleshootLabel.Location = new Point(PaddingLeftRight, ActiveAlarmsTextBox.Bottom + 4);
			TroubleshootLabel.Size = new Size(109, 20);
			
			TroubleshootTextBox.Size = new Size(510, 203);
			TroubleshootTextBox.Location = new Point(PaddingLeftRight, TroubleshootLabel.Bottom + 3);
			
			TroubleshootLargeTextButton.Size = new Size(24, 20);
			TroubleshootLargeTextButton.Location = new Point(TroubleshootTextBox.Right - TroubleshootLargeTextButton.Width, TroubleshootLabel.Top);
			
			Size = new Size(TroubleshootTextBox.Right + PaddingLeftRight, TroubleshootTextBox.Bottom + PaddingTopBottom);
		}
	}
}
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
        RadioButton FromNetcoolRadioButton = new RadioButton();
        RadioButton FromExistingReportRadioButton = new RadioButton();
        RadioButton FromSitesListRadioButton = new RadioButton();
        Panel GenerationSourceContainer = new Panel();
        Panel VfTfReportsRadioButtonsContainer = new Panel();
		Label Alarms_ReportLabel = new Label(); // Alarms_ReportLabel
		Label BulkCILabel = new Label(); // BulkCILabel
		AMTRichTextBox BulkCITextBox = new AMTRichTextBox(); // BulkCITextBox
		public AMTRichTextBox Alarms_ReportTextBox = new AMTRichTextBox(); // Alarms_ReportTextBox

		public Outage currentOutage;
		
		AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem generateReportToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem generateSitesListToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem outageFollowUpToolStripMenuItem = new ToolStripMenuItem();
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
                    GenerationSourceContainer.Visible = false;
					
					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(sitesPerTechToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
				}
				else {
					InitializeComponent();
                    //Alarms_ReportLabel.Text = "Generate outage";
                    Alarms_ReportLabel.Visible = false;
                    FromNetcoolRadioButton.Checked = true;

                    MainMenu.MainMenu.DropDownItems.Add(generateReportToolStripMenuItem);
					//MainMenu.MainMenu.DropDownItems.Add(generateFromSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(outageFollowUpToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(sitesPerTechToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(generateSitesListToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add(copyToClipboardToolStripMenuItem);
					MainMenu.MainMenu.DropDownItems.Add("-");
					MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
					
					generateReportToolStripMenuItem.Enabled =
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
            if (FromNetcoolRadioButton.Checked)
                GenerateFromAlarms();
            else
            {
                if (FromExistingReportRadioButton.Checked)
                    GenerateFromExistingReport();
                else
                {

                    GenerateFromSitesList();
                }
            }
        }

        async void GenerateFromAlarms()
        {
            LoadingPanel loading = new LoadingPanel();
            loading.Show(true, this);

            bool parsingError = false;
            string textBoxContent = Alarms_ReportTextBox.Text;

            await System.Threading.Tasks.Task.Run(() =>
            {
			    try
                {
			        AlarmsParser alarms = new AlarmsParser(textBoxContent, AlarmsParser.ParsingMode.Outage, false);
			        currentOutage = alarms.GenerateOutage();
			    }
			    catch(Exception ex)
                {
			        MainForm.trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nError message:\n" + ex.Message);
                    parsingError = true;
                    return;
                }
            });

            if (parsingError)
                return;

            loading.ToggleLoadingSpinner();

            if (!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
			    VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
			else
            {
			    if(string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage))
                {
			        MainForm.trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
			        Alarms_ReportTextBox.Text = string.Empty;
			        VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = false;
                    loading.Close();
                    return;
			    }
			    if(!string.IsNullOrEmpty(currentOutage.VfOutage))
                {
			        VFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
			        TFReportRadioButton.Enabled = false;
			    }
			    else
                {
			        if(!string.IsNullOrEmpty(currentOutage.TefOutage))
                    {
			            TFReportRadioButton.Enabled = TFReportRadioButton.Checked = true;
			            VFReportRadioButton.Enabled = false;
			        }
			    }
			}
			if(!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage))
            {
                GenerationSourceContainer.Visible =
                    generateReportToolStripMenuItem.Enabled = false;
			    Alarms_ReportTextBox.ReadOnly =
			        BulkCITextBox.ReadOnly = true;
			    outageFollowUpToolStripMenuItem.Enabled =
			        copyToClipboardToolStripMenuItem.Enabled =
			        sitesPerTechToolStripMenuItem.Enabled =
			        generateSitesListToolStripMenuItem.Enabled = true;
			    Alarms_ReportTextBox.Focus();
                Alarms_ReportLabel.Visible = true;
                GenerationSourceContainer.Visible = false;
			    MainForm.logFiles.HandleOutageLog(currentOutage);
			}

            loading.Close();
		}

        async void GenerateFromExistingReport()
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            bool parsingError = false;
            string[] textBoxContent = Alarms_ReportTextBox.Lines;
            string err = string.Empty;

            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    currentOutage = new Outage(textBoxContent, DateTime.Now);
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    parsingError = true;
                }
            });
            
            if (parsingError)
                MainForm.trayIcon.showBalloon("Error parsing alarms", "An error occurred while parsing the alarms.\nError message:\n" + err);
            else
            {
                load.ToggleLoadingSpinner();

                if (!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
                    VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
                else
                {
                    if (string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage))
                    {
                        MainForm.trayIcon.showBalloon("Empty report", "The alarms inserted have no COOS in its content, output is blank");
                        Alarms_ReportTextBox.Text = string.Empty;
                        VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = false;
                        return;
                    }
                    if (!string.IsNullOrEmpty(currentOutage.VfOutage))
                    {
                        VFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
                        TFReportRadioButton.Enabled = false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentOutage.TefOutage))
                        {
                            TFReportRadioButton.Enabled = TFReportRadioButton.Checked = true;
                            VFReportRadioButton.Enabled = false;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage))
                {
                    GenerationSourceContainer.Visible =
                        generateReportToolStripMenuItem.Enabled =false;
                    Alarms_ReportTextBox.ReadOnly =
                        BulkCITextBox.ReadOnly = true;
                    outageFollowUpToolStripMenuItem.Enabled =
                        copyToClipboardToolStripMenuItem.Enabled =
                        sitesPerTechToolStripMenuItem.Enabled =
                        generateSitesListToolStripMenuItem.Enabled = true;
                    Alarms_ReportTextBox.Focus();
                    Alarms_ReportLabel.Visible = true;
                    GenerationSourceContainer.Visible = false;
                    MainForm.logFiles.HandleOutageLog(currentOutage);
                }
            }
            
            load.Close();
        }

        async void GenerateFromSitesList()
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            bool parsingError = false;
            string[] textBoxContent = Alarms_ReportTextBox.Lines;

            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    List<string> input = textBoxContent.Count(l => l.Contains(";")) > 0 ? string.Join(Environment.NewLine, textBoxContent).Split(';').ToList() : textBoxContent.ToList();

                    for (int c = 0; c < input.Count; c++)
                    {
                        input[c] = input[c].Trim().RemoveLetters();
                        while (input[c].StartsWith("0"))
                            input[c] = input[c].Substring(1);
                    }

                    currentOutage = new Outage(input);
                }
                catch (Exception ex)
                {
                    var m = ex.Message;
                    parsingError = true;
                }
            });
            
            if (parsingError)
                MainForm.trayIcon.showBalloon("Error parsing alarms", "An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
            else
            {
                load.ToggleLoadingSpinner();
                
                if (!string.IsNullOrEmpty(currentOutage.VfOutage) && !string.IsNullOrEmpty(currentOutage.TefOutage))
                    VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
                else
                {
                    if (string.IsNullOrEmpty(currentOutage.VfOutage) && string.IsNullOrEmpty(currentOutage.TefOutage))
                    {
                        MainForm.trayIcon.showBalloon("Empty report", "The alarms inserted have no COOS in its content, output is blank");
                        Alarms_ReportTextBox.Text = string.Empty;
                        VFReportRadioButton.Enabled = TFReportRadioButton.Enabled = false;
                        return;
                    }
                    if (!string.IsNullOrEmpty(currentOutage.VfOutage))
                    {
                        VFReportRadioButton.Enabled = VFReportRadioButton.Checked = true;
                        TFReportRadioButton.Enabled = false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentOutage.TefOutage))
                        {
                            TFReportRadioButton.Enabled = TFReportRadioButton.Checked = true;
                            VFReportRadioButton.Enabled = false;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(currentOutage.VfOutage) || !string.IsNullOrEmpty(currentOutage.TefOutage))
                {
                    generateReportToolStripMenuItem.Enabled = false;
                    Alarms_ReportTextBox.ReadOnly =
                        BulkCITextBox.ReadOnly = true;
                    copyToClipboardToolStripMenuItem.Enabled =
                        generateSitesListToolStripMenuItem.Enabled =
                        outageFollowUpToolStripMenuItem.Enabled = true;
                    Alarms_ReportTextBox.Focus();
                    Alarms_ReportLabel.Visible = true;
                    GenerationSourceContainer.Visible = false;
                    MainForm.logFiles.HandleOutageLog(currentOutage);
                }
            }

            load.Close();
        }

        void OutageFollowUp(object sender, EventArgs e)
        {
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
		
		void VFTFReportRadioButtonsCheckedChanged(object sender, EventArgs e)
        {
			RadioButton rb = sender as RadioButton;
			if(rb.Checked)
            {
				if(rb.Text == "VF Report")
                {
					Alarms_ReportTextBox.Text = currentOutage.VfOutage;
					BulkCITextBox.Text = currentOutage.VfBulkCI;
				}
				else
                {
					Alarms_ReportTextBox.Text = currentOutage.TefOutage;
					BulkCITextBox.Text = currentOutage.TefBulkCI;
				}
				
				if(UiMode == UiEnum.Template)
                {
					Alarms_ReportTextBox.Select(0,0);
					BulkCITextBox.Select(0,0);
				}
			}
		}

		void ShowSitesPerTech(object sender, EventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            //Action action = new Action(delegate {
            Form showSitesPerTechForm = new Form
            {
                Text = (VFReportRadioButton.Checked ? "VF" : "TF") + " Affected Sites Per Tech",
                Icon = Resources.app_icon,
			    Name = "showSitesPerTechForm",
			    Size = new Size(790, 250),
			    MaximizeBox = false,
			    FormBorderStyle = FormBorderStyle.Sizable
			};
            showSitesPerTechForm.MaximumSize = new Size(showSitesPerTechForm.Width, int.MaxValue);
            showSitesPerTechForm.MinimumSize = new Size(showSitesPerTechForm.Width, 200);

            AMTDataGridView dataGridView = new AMTDataGridView();
			dataGridView.AllowUserToAddRows = false;
			dataGridView.AllowUserToDeleteRows = false;
			dataGridView.AllowUserToResizeColumns = false;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dataGridView.AlwaysVisibleVScrollBar = false;
			dataGridView.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
			dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			dataGridView.BackgroundColor = SystemColors.Control;
			dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridView.CellFormatting += (s, a) =>
            {
                    a.CellStyle.BackColor = a.RowIndex % 2 == 0 ? dataGridView.DefaultCellStyle.BackColor : dataGridView.AlternatingRowsDefaultCellStyle.BackColor;
            };
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
			dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = SystemColors.ControlDark;
			dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
			dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView.DoubleBuffer = true;
			dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
			dataGridView.GridColor = SystemColors.ControlDarkDark;
			dataGridView.Location = Point.Empty;
			dataGridView.Name = "dataGridView";
			dataGridView.RowHeadersVisible = false;
			dataGridView.RowTemplate.Resizable = DataGridViewTriState.True;
			dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dataGridView.ShowEditingIcon = false;
			dataGridView.Size = new Size(showSitesPerTechForm.ClientRectangle.Width, 183);
			dataGridView.SelectionChanged += (s, a) => {
			    int[] counts = new int[dataGridView.Columns.Count];
			    foreach(DataGridViewCell cell in dataGridView.SelectedCells)
			    {
                    if(!string.IsNullOrEmpty(cell.Value.ToString()))
			            counts[cell.ColumnIndex]++;
			    }
			    foreach(DataGridViewColumn col in dataGridView.Columns)
                {
                    if (col.HeaderText.IndexOf(" (") > -1)
                        col.HeaderText = col.HeaderText.Substring(0, col.HeaderText.IndexOf(" ("));

			        col.HeaderText += " (" + counts[col.Index] + ")";
			    }
			};

            CheckBox sitesFullNameCheckBox = new CheckBox
            {
                Text = "View Sites full ID",
                Width = 150,
                Location = new Point(5, dataGridView.Bottom + 5),
                Anchor = (AnchorStyles.Bottom | AnchorStyles.Left)
            };

            DataTable dt = new DataTable("Table");
			dt.Columns.AddRange(new DataColumn[] {
			                        new DataColumn("2G Only Sites", typeof(string)),
			                        new DataColumn("2G/3G Sites", typeof(string)),
			                        new DataColumn("2G/4G Sites", typeof(string)),
			                        new DataColumn("2G/3G/4G Sites", typeof(string)),
			                        new DataColumn("3G Only Sites", typeof(string)),
			                        new DataColumn("3G/4G Sites", typeof(string)),
			                        new DataColumn("4G Only Sites", typeof(string))
			                    });

			int[] listcounts = VFReportRadioButton.Checked ?
			    new[] {
                        currentOutage.VfGsmOnlyAffectedSites.Count,
                        currentOutage.VfGsmUmtsAffectedSites.Count,
                        currentOutage.VfGsmLteAffectedSites.Count,
                        currentOutage.VfGsmUmtsLteAffectedSites.Count,
                        currentOutage.VfUmtsOnlyAffectedSites.Count,
                        currentOutage.VfUmtsLteAffectedSites.Count,
                        currentOutage.VfLteOnlyAffectedSites.Count
                    } :
			    new[] {
                        currentOutage.TefGsmOnlyAffectedSites.Count,
                        currentOutage.TefGsmUmtsAffectedSites.Count,
                        currentOutage.TefGsmLteAffectedSites.Count,
                        currentOutage.TefGsmUmtsLteAffectedSites.Count,
                        currentOutage.TefUmtsOnlyAffectedSites.Count,
                        currentOutage.TefUmtsLteAffectedSites.Count,
                        currentOutage.TefLteOnlyAffectedSites.Count
                    };

			int max = listcounts.Max();
			for (int c = 0; c < max; c++)
			{
			    var newRow = dt.NewRow();
			    if (listcounts[0] > c)
			        newRow["2G Only Sites"] = VFReportRadioButton.Checked ? currentOutage.VfGsmOnlyAffectedSites[c] : currentOutage.TefGsmOnlyAffectedSites[c];
                                            
			    if (listcounts[1] > c)
			        newRow["2G/3G Sites"] = VFReportRadioButton.Checked ? currentOutage.VfGsmUmtsAffectedSites[c] : currentOutage.TefGsmUmtsAffectedSites[c];
                                            
			    if (listcounts[2] > c)
			        newRow["2G/4G Sites"] = VFReportRadioButton.Checked ? currentOutage.VfGsmLteAffectedSites[c] : currentOutage.TefGsmLteAffectedSites[c];
                                            
			    if (listcounts[3] > c)
			        newRow["2G/3G/4G Sites"] = VFReportRadioButton.Checked ? currentOutage.VfGsmUmtsLteAffectedSites[c] : currentOutage.TefGsmUmtsLteAffectedSites[c];
                                            
			    if (listcounts[4] > c)
			        newRow["3G Only Sites"] = VFReportRadioButton.Checked ? currentOutage.VfUmtsOnlyAffectedSites[c] : currentOutage.TefUmtsOnlyAffectedSites[c];
                                            
			    if (listcounts[5] > c)
			        newRow["3G/4G Sites"] = VFReportRadioButton.Checked ? currentOutage.VfUmtsLteAffectedSites[c] : currentOutage.TefUmtsLteAffectedSites[c];
                                            
			    if (listcounts[6] > c)
			        newRow["4G Only Sites"] = VFReportRadioButton.Checked ? currentOutage.VfLteOnlyAffectedSites[c] : currentOutage.TefLteOnlyAffectedSites[c];
			    dt.Rows.Add(newRow);
			}
			sitesFullNameCheckBox.CheckedChanged += (s, a) => {
                    var x = currentOutage.VfGsmOnlyAffectedSites;
			    if (sitesFullNameCheckBox.Checked)
			    {
			        dataGridView.DataSource = null;
			        dataGridView.DataSource = dt;

                    for(int c = 0;c < dataGridView.Columns.Count;c++)
                    {
                        dataGridView.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dataGridView.Columns[c].HeaderText += " (0)";
                    }
                    }
			    else
			    {
			        DataTable tempDT = new DataTable(dt.TableName);
                    foreach(DataColumn col in dt.Columns)
                            tempDT.Columns.Add(col.ColumnName, col.DataType);
                                                
                    foreach(DataRow row in dt.Rows)
                    {
                        DataRow newRow = tempDT.NewRow();
                        newRow.ItemArray = row.ItemArray;
                        tempDT.Rows.Add(newRow);
                    }
                    dataGridView.DataSource = null;

                    for (int row = 0;row < tempDT.Rows.Count;row++)
			        {
			            for(int col = 0;col < tempDT.Rows[row].ItemArray.Length;col++)
			            {
			                string temp = tempDT.Rows[row][col].ToString().RemoveLetters();
			                while (temp.StartsWith("0"))
			                    temp = temp.Substring(1);
			                tempDT.Rows[row][col] = temp;
			            }
			        }

			        dataGridView.DataSource = tempDT;

                    for (int c = 0; c < dataGridView.Columns.Count; c++)
                    {
                        dataGridView.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dataGridView.Columns[c].HeaderText += " (0)";
                    }
                }
			};

			showSitesPerTechForm.Controls.AddRange(new Control[]{
			                           	            dataGridView,
			                           	            sitesFullNameCheckBox
			                           	            });
			sitesFullNameCheckBox.Checked = true;

            showSitesPerTechForm.ShowDialog();
			                           //});

			load.Close();
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
				form.Icon = Resources.app_icon;
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
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            FlexibleMessageBox.Show("The following site list was copied to the Clipboard:" + Environment.NewLine + Environment.NewLine + currentOutage.SitesList + Environment.NewLine + Environment.NewLine + "This list can be used to enter a bulk site search on Site Lopedia.","List generated",MessageBoxButtons.OK,MessageBoxIcon.Information);
            Clipboard.SetText(currentOutage.SitesList);

			load.Close();
		}

        void CopyReportToClipboard(object sender, EventArgs e) {
            Clipboard.SetText(Alarms_ReportTextBox.Text);
        }

        void ClearAllControls(object sender, EventArgs e)
		{
			currentOutage = null;
            //Alarms_ReportLabel.Text = "Copy Outage alarms from Netcool";
            Alarms_ReportLabel.Visible = false;
            GenerationSourceContainer.Visible =
			    generateReportToolStripMenuItem.Enabled = true;
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
			Controls.AddRange(new Control[] {
                MainMenu,
                Alarms_ReportLabel,
                GenerationSourceContainer,
                Alarms_ReportTextBox,
                Alarms_ReportLargeTextButton,
                BulkCILabel,
                BulkCITextBox,
                BulkCILargeTextButton,
                VfTfReportsRadioButtonsContainer
            });
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
            copyToClipboardToolStripMenuItem.Click += CopyReportToClipboard;
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
            Alarms_ReportLabel.Text = "Generated Outage Report";
            Alarms_ReportLabel.TabIndex = 30;
			Alarms_ReportLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FromNetcoolRadioButton
            // 
            FromNetcoolRadioButton.Name = "FromNetcoolRadioButton";
            FromNetcoolRadioButton.Text = "Netcool alarms";
            FromNetcoolRadioButton.UseVisualStyleBackColor = true;
            // 
            // FromExistingReportRadioButton
            // 
            FromExistingReportRadioButton.Name = "FromExistingReportRadioButton";
            FromExistingReportRadioButton.Text = "Existing Report";
            FromExistingReportRadioButton.UseVisualStyleBackColor = true;
            // 
            // FromSitesListRadioButton
            // 
            FromSitesListRadioButton.Name = "FromSitesListRadioButton";
            FromSitesListRadioButton.Text = "Sites list/Bulk CI";
            FromSitesListRadioButton.UseVisualStyleBackColor = true;
            // 
            // GenerationSourceContainer
            // 
            GenerationSourceContainer.BackColor = BackColor;
            GenerationSourceContainer.Name = "GenerationSourceContainer";
            GenerationSourceContainer.Controls.AddRange(new[] { FromNetcoolRadioButton, FromExistingReportRadioButton, FromSitesListRadioButton });
            
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
            // VfTfReportsRadioButtonsContainer
            // 
            VfTfReportsRadioButtonsContainer.BackColor = BackColor;
            VfTfReportsRadioButtonsContainer.Name = "VfTfReportsRadioButtonsContainer";
            VfTfReportsRadioButtonsContainer.Controls.AddRange(new[] { VFReportRadioButton, TFReportRadioButton });
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
			Alarms_ReportLabel.Size = new Size(180, 20);

            FromNetcoolRadioButton.Location = Point.Empty;
            FromNetcoolRadioButton.Size = new Size(95, 20);

            FromExistingReportRadioButton.Location = new Point(FromNetcoolRadioButton.Right, 0);
            FromExistingReportRadioButton.Size = new Size(96, 20);

            FromSitesListRadioButton.Location = new Point(FromExistingReportRadioButton.Right, 0);
            FromSitesListRadioButton.Size = new Size(102, 20);

            GenerationSourceContainer.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
            GenerationSourceContainer.Size = new Size(FromNetcoolRadioButton.Width + FromExistingReportRadioButton.Width + FromSitesListRadioButton.Width, FromNetcoolRadioButton.Height);

            Alarms_ReportTextBox.Location = new Point(PaddingLeftRight, Alarms_ReportLabel.Bottom + 4);
			Alarms_ReportTextBox.Size = new Size(510, 368);
			
			Alarms_ReportLargeTextButton.Size = new Size(24, 20);
			Alarms_ReportLargeTextButton.Location = new Point(Alarms_ReportTextBox.Right - Alarms_ReportLargeTextButton.Width, MainMenu.Bottom + 4);

            VFReportRadioButton.Size = new Size(64, 20);
            VFReportRadioButton.Location = Point.Empty;

            TFReportRadioButton.Size = new Size(64, 20);
			TFReportRadioButton.Location = new Point(VFReportRadioButton.Right + 5, VFReportRadioButton.Top);

            VfTfReportsRadioButtonsContainer.Size = new Size(VFReportRadioButton.Width + TFReportRadioButton.Width + 5, VFReportRadioButton.Height);
            VfTfReportsRadioButtonsContainer.Location = new Point(Alarms_ReportLargeTextButton.Left - VfTfReportsRadioButtonsContainer.Width - 5, MainMenu.Bottom + 4);

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
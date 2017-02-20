/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 22/11/2016
 * Time: 15:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.UI;
using appCore.Settings;
using appCore.SiteFinder;
using appCore.SiteFinder.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.OssScripts.UI
{
	/// <summary>
	/// Description of Ericsson.
	/// </summary>
	public class EricssonScriptsControls : Panel
	{
		Label WarningLabel = new Label();
		Button SelectNoneButton = new Button();
		Button SelectAllButton = new Button();
		Label PriorityLabel = new Label();
		AMTTextBox PriorityTextBox = new AMTTextBox();
		Label RegionLabel = new Label();
		AMTTextBox RegionTextBox = new AMTTextBox();
		Label CellsLabel = new Label();
		ListView CellsListView = new ListView();
		ColumnHeader TechColumnHeader = new ColumnHeader();
		ColumnHeader CellNameColumnHeader = new ColumnHeader();
		ColumnHeader CellIdColumnHeader = new ColumnHeader();
		ColumnHeader LacTacColumnHeader = new ColumnHeader();
		ColumnHeader SwitchColumnHeader = new ColumnHeader();
		ColumnHeader NocColumnHeader = new ColumnHeader();
		Label SiteLabel = new Label();
		Button UnlockScriptLargeTextButton = new Button();
		Button LockScriptLargeTextButton = new Button();
		Label UnlockScriptLabel = new Label();
		Label LockScriptLabel = new Label();
		AMTRichTextBox UnlockScriptTextBox = new AMTRichTextBox();
		AMTRichTextBox LockScriptTextBox = new AMTRichTextBox();
		GroupBox TechnologyGroupBox = new GroupBox();
		RadioButton GsmRadioButton = new RadioButton();
		RadioButton UmtsRadioButton = new RadioButton();
		RadioButton LteRadioButton = new RadioButton();
		AMTTextBox SiteTextBox = new AMTTextBox();
		CellDetailsPictureBox CellsSummaryPictureBox = new CellDetailsPictureBox();
		
//		DataView eScriptCellsGlobal = new DataView();
		List<Cell> filteredCells;
		Site currentSite { get; set; }

		AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem generateScriptsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyLockScriptToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyUnlockScriptToolStripMenuItem = new ToolStripMenuItem();
		
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
		
		int SelectedTech {
			get;
			set;
		}
		
		public EricssonScriptsControls()
		{
			InitializeComponent();
		}

		public void siteFinder(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				AMTTextBox tb = (AMTTextBox)sender;
				Action actionThreaded = new Action(delegate {
//				                           	Stopwatch sw = new Stopwatch();
//
//				                           	sw.Start();
				                                   	while(tb.Text.StartsWith("0"))
				                                   		tb.Text = tb.Text.Substring(1);
				                                   	currentSite = Finder.getSite(tb.Text);
				                                   });
				
				Action actionNonThreaded = new Action(delegate {
				                                      	if(currentSite.Exists) {
				                                      		filteredCells = new List<Cell>();
				                                      		foreach(Cell cell in currentSite.Cells) {
				                                      			if(cell.Vendor == SiteFinder.Site.Vendors.Ericsson)
				                                      				filteredCells.Add(cell);
				                                      		}
				                                      		if(filteredCells.Count == 0){
				                                      			MainForm.trayIcon.showBalloon("Error",String.Format("Site {0} is not E///",tb.Text));
				                                      			return;
				                                      		}
				                                      		PriorityTextBox.Text = currentSite.Priority;
				                                      		RegionTextBox.Text = currentSite.Region;
				                                      		
				                                      		CellsSummaryPictureBox.UpdateCells(filteredCells);
				                                      		GsmRadioButton.Enabled = filteredCells.Filter(Cell.Filters.All_2G).Count > 0;
				                                      		UmtsRadioButton.Enabled = filteredCells.Filter(Cell.Filters.All_3G).Count > 0;
				                                      		LteRadioButton.Enabled = filteredCells.Filter(Cell.Filters.All_4G).Count > 0;
				                                      	}
				                                      	else {
				                                      		PriorityTextBox.Text = string.Empty;
				                                      		RegionTextBox.Text = "No site found";
				                                      	}
//				                           			break;
//				                           	}
//				                           	sw.Stop();
//
//				                           	FlexibleMessageBox.Show("Elapsed=" + sw.Elapsed);
				                                      	siteFinder_Toggle(true, currentSite.Exists);
				                                      });
				LoadingPanel load = new LoadingPanel();
				load.Show(actionThreaded, actionNonThreaded, true, this);
			}
		}

		void SiteTextBoxTextChanged(object sender, EventArgs e)
		{
			if(GlobalProperties.siteFinder_mainswitch)
				siteFinder_Toggle(false, false);
			PriorityTextBox.Text = RegionTextBox.Text = string.Empty;
			clearToolStripMenuItem.Enabled = !string.IsNullOrEmpty(SiteTextBox.Text);
		}

		void siteFinder_Toggle(bool toggle, bool siteFound)
		{
			foreach (object ctrl in Controls) {
				switch(ctrl.GetType().ToString())
				{
					case "System.Windows.Forms.ListView":
						ListView lv = ctrl as ListView;
						if(lv.Name == "listView2")
							lv.Enabled = toggle;
						break;
					case "System.Windows.Forms.GroupBox":
						GroupBox grb = ctrl as GroupBox;
						foreach(Control ctr in grb.Controls) {
							if(ctr.GetType().ToString() == "System.Windows.Forms.RadioButton") {
								if(!toggle) {
									RadioButton rb = ctr as RadioButton;
									rb.Enabled = toggle;
									rb.Checked = toggle;
								}
							}
						}
						break;
					case "appCore.UI.AMTTextBox":
						TextBoxBase tb = ctrl as TextBoxBase;
						if(tb.Name != "SiteTextBox")
							tb.Enabled = toggle;
						break;
				}
				CellsListViewItemChecked(null,null);
//				}
			}
		}
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if(rb.Checked) {
				List<Cell> cells = null;
				switch(rb.Name) {
					case "GsmRadioButton":
						cells = filteredCells.Filter(Cell.Filters.All_2G);
						break;
					case "UmtsRadioButton":
						cells = filteredCells.Filter(Cell.Filters.All_3G);
						break;
					case "LteRadioButton":
						cells = filteredCells.Filter(Cell.Filters.All_4G);
						break;
				}
				foreach(Cell cell in cells)
					CellsListView.Items.Add(new ListViewItem(new[]{cell.Bearer, cell.Name, cell.Id, cell.LacTac, cell.BscRnc_Id, cell.Noc}));
				foreach (ColumnHeader col in CellsListView.Columns)
					col.Width = -2;
				SelectAllButton.Enabled = SelectNoneButton.Enabled = CellsListView.Enabled = true;
				if(rb.Name == "UmtsRadioButton") {
					WarningLabel.Text += "\nIt\'s not possible to lock 3G cells via amos on site, log on to the RNC " + CellsListView.Items[0].SubItems[4].Text + " and follow the instructions";
					WarningLabel.Visible = true;
				}
			}
			else {
				CellsListView.Items.Clear();
				if(rb.Name == "UmtsRadioButton") {
					WarningLabel.Visible = false;
					WarningLabel.Text = "WARNING:";
				}
				SelectAllButton.Enabled = SelectNoneButton.Enabled = CellsListView.Enabled = false;
			}
		}
		
		void CellsListViewItemChecked(object sender, ItemCheckedEventArgs e)
		{
			UnlockScriptTextBox.Text = string.Empty;
			LockScriptTextBox.Text = string.Empty;
			SelectAllButton.Enabled = CellsListView.CheckedItems.Count != CellsListView.Items.Count;
			if(CellsListView.CheckedItems.Count > 0) {
				SelectNoneButton.Enabled = true;
				generateScriptsToolStripMenuItem.Enabled = true;
			}
			else {
				SelectNoneButton.Enabled = false;
				generateScriptsToolStripMenuItem.Enabled = false;
			}
		}
		
		void SelectAllButtonClick(object sender, EventArgs e)
		{
			if(CellsListView.CheckedItems.Count < CellsListView.Items.Count) {
				foreach (ListViewItem cell in CellsListView.Items)
					cell.Checked = true;
			}
		}
		
		void SelectNoneButtonClick(object sender, EventArgs e)
		{
			if(CellsListView.CheckedItems.Count > 0) {
				foreach (ListViewItem cell in CellsListView.Items)
					cell.Checked = false;
			}
		}

		void generateScripts(object sender, EventArgs e)
		{
			int radioch = 0;
			foreach (Control radio in TechnologyGroupBox.Controls) {
				RadioButton rb = radio as RadioButton;
				if (rb.Checked) break;
				radioch++;
			}
			string cellLock = string.Empty;
			string cellUnlock = string.Empty;
			switch(radioch) {
				case 0:
					for(int c = 0;c < CellsListView.CheckedItems.Count;c++) {
						cellLock += "RLSTC:CELL=" + CellsListView.CheckedItems[c].SubItems[1].Text + ",STATE=HALTED;";
						cellUnlock += "RLSTC:CELL=" + CellsListView.CheckedItems[c].SubItems[1].Text + ",STATE=ACTIVE;";
						if(c != CellsListView.CheckedItems.Count - 1) {
							cellLock += Environment.NewLine;
							cellUnlock += Environment.NewLine;
						}
					}
					break;
				case 1:
					foreach(ListViewItem cell in CellsListView.CheckedItems) {
						cellLock += "bl UtranCell=" + cell.SubItems[2].Text + "$" + Environment.NewLine;
						cellUnlock += "deb UtranCell=" + cell.SubItems[2].Text + "$" + Environment.NewLine;
					}
					break;
				case 2:
					foreach(ListViewItem cell in CellsListView.CheckedItems) {
						cellLock += "bl ENodeBFunction=1,EUtranCellFDD=" + cell.SubItems[1].Text + "$" + Environment.NewLine;
						cellUnlock += "deb ENodeBFunction=1,EUtranCellFDD=" + cell.SubItems[1].Text + "$" + Environment.NewLine;
					}
					break;
			}
			UnlockScriptTextBox.Text = cellUnlock;
			LockScriptTextBox.Text = cellLock;
		}

		void UnlockScriptTextBoxTextChanged(object sender, EventArgs e)
		{
			UnlockScriptLargeTextButton.Enabled = copyUnlockScriptToolStripMenuItem.Enabled = !string.IsNullOrEmpty(UnlockScriptTextBox.Text);
		}

		void LockScriptTextBoxTextChanged(object sender, EventArgs e)
		{
			LockScriptLargeTextButton.Enabled = copyLockScriptToolStripMenuItem.Enabled = !string.IsNullOrEmpty(LockScriptTextBox.Text);
		}
		
		void LargeTextButtonsClick(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			Button btn = (Button)sender;
			string lbl = string.Empty;
			TextBoxBase tb = null;
			switch(btn.Name) {
				case "LockScriptLargeTextButton":
					tb = (TextBoxBase)LockScriptTextBox;
					lbl = LockScriptLabel.Text;
					break;
				case "UnlockScriptLargeTextButton":
					tb = (TextBoxBase)UnlockScriptTextBox;
					lbl = UnlockScriptLabel.Text;
					break;
			}
			
			AMTLargeTextForm enlarge = new AMTLargeTextForm(tb.Text,lbl,false);
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
			tb.Text = enlarge.finaltext;
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void copyUnlockScript(object sender, EventArgs e)
		{
			Clipboard.SetText(UnlockScriptTextBox.Text);
		}

		void copyLockScript(object sender, EventArgs e)
		{
			Clipboard.SetText(LockScriptTextBox.Text);
		}

		void ClearAllControls(object sender, EventArgs e)
		{
			SiteTextBox.Text = string.Empty;
		}
		
		void InitializeComponent() {
			SuspendLayout();
			BackColor = SystemColors.Control;
			Name = "Ericsson Scripts GUI";
			Controls.Add(MainMenu);
			Controls.Add(CellsSummaryPictureBox);
			Controls.Add(WarningLabel);
			Controls.Add(SelectNoneButton);
			Controls.Add(SelectAllButton);
			Controls.Add(PriorityLabel);
			Controls.Add(PriorityTextBox);
			Controls.Add(RegionLabel);
			Controls.Add(RegionTextBox);
			Controls.Add(CellsLabel);
			Controls.Add(CellsListView);
			Controls.Add(SiteLabel);
			Controls.Add(UnlockScriptLargeTextButton);
			Controls.Add(LockScriptLargeTextButton);
			Controls.Add(UnlockScriptLabel);
			Controls.Add(LockScriptLabel);
			Controls.Add(UnlockScriptTextBox);
			Controls.Add(LockScriptTextBox);
			Controls.Add(TechnologyGroupBox);
			Controls.Add(SiteTextBox);
			TechnologyGroupBox.SuspendLayout();
			// 
			// MainMenu
			// 
			MainMenu.Location = new Point(paddingLeftRight, 0);
			MainMenu.MainMenu.DropDownItems.Add(generateScriptsToolStripMenuItem);
			MainMenu.MainMenu.DropDownItems.Add("-");
			MainMenu.MainMenu.DropDownItems.Add(copyLockScriptToolStripMenuItem);
			MainMenu.MainMenu.DropDownItems.Add(copyUnlockScriptToolStripMenuItem);
			MainMenu.MainMenu.DropDownItems.Add("-");
			MainMenu.MainMenu.DropDownItems.Add(clearToolStripMenuItem);
			// 
			// generateScriptsToolStripMenuItem
			// 
			generateScriptsToolStripMenuItem.Name = "generateScriptsToolStripMenuItem";
			generateScriptsToolStripMenuItem.Text = "Generate Scripts";
			generateScriptsToolStripMenuItem.Enabled = false;
			generateScriptsToolStripMenuItem.Click += generateScripts;
			// 
			// copyLockScriptToolStripMenuItem
			// 
			copyLockScriptToolStripMenuItem.Name = "copyLockScriptToolStripMenuItem";
			copyLockScriptToolStripMenuItem.Text = "Copy lock script";
			copyLockScriptToolStripMenuItem.Enabled = false;
			copyLockScriptToolStripMenuItem.Click += copyLockScript;
			// 
			// copyUnlockScriptToolStripMenuItem
			// 
			copyUnlockScriptToolStripMenuItem.Name = "copyUnlockScriptToolStripMenuItem";
			copyUnlockScriptToolStripMenuItem.Text = "Copy Unlock script";
			copyUnlockScriptToolStripMenuItem.Enabled = false;
			copyUnlockScriptToolStripMenuItem.Click += copyUnlockScript;
			// 
			// clearToolStripMenuItem
			// 
			clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			clearToolStripMenuItem.Text = "Clear template";
			clearToolStripMenuItem.Enabled = false;
			clearToolStripMenuItem.Click += ClearAllControls;
			// 
			// SiteLabel
			// 
			SiteLabel.Name = "SiteLabel";
			SiteLabel.TabIndex = 147;
			SiteLabel.Text = "Site";
			SiteLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteTextBox
			// 
			SiteTextBox.Font = new Font("Courier New", 8.25F);
			SiteTextBox.Name = "SiteTextBox";
			SiteTextBox.TabIndex = 136;
			SiteTextBox.TextChanged += SiteTextBoxTextChanged;
			SiteTextBox.KeyPress += siteFinder;
			// 
			// PriorityLabel
			// 
			PriorityLabel.Name = "PriorityLabel";
			PriorityLabel.TabIndex = 153;
			PriorityLabel.Text = "Priority";
			PriorityLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// PriorityTextBox
			// 
			PriorityTextBox.Font = new Font("Courier New", 8.25F);
			PriorityTextBox.Name = "PriorityTextBox";
			PriorityTextBox.ReadOnly = true;
			PriorityTextBox.TabIndex = 152;
			// 
			// RegionLabel
			// 
			RegionLabel.Name = "RegionLabel";
			RegionLabel.TabIndex = 151;
			RegionLabel.Text = "Region";
			RegionLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// RegionTextBox
			// 
			RegionTextBox.Font = new Font("Courier New", 8.25F);
			RegionTextBox.MaxLength = 5;
			RegionTextBox.Name = "RegionTextBox";
			RegionTextBox.ReadOnly = true;
			RegionTextBox.TabIndex = 150;
			// 
			// CellsSummaryPictureBox
			// 
			CellsSummaryPictureBox.Image = Resources.Cells_Totals;
			CellsSummaryPictureBox.Name = "CellsSummaryPictureBox";
			CellsSummaryPictureBox.TabIndex = 118;
			CellsSummaryPictureBox.TabStop = false;
			// 
			// TechnologyGroupBox
			// 
			TechnologyGroupBox.Controls.Add(GsmRadioButton);
			TechnologyGroupBox.Controls.Add(UmtsRadioButton);
			TechnologyGroupBox.Controls.Add(LteRadioButton);
			TechnologyGroupBox.Name = "TechnologyGroupBox";
			TechnologyGroupBox.TabIndex = 138;
			TechnologyGroupBox.TabStop = false;
			TechnologyGroupBox.Text = "Technology";
			// 
			// CellsLabel
			// 
//			CellsLabel.ImageAlign = ContentAlignment.MiddleLeft;
			CellsLabel.Name = "CellsLabel";
			CellsLabel.TabIndex = 149;
			CellsLabel.Text = "Cells";
			CellsLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// CellsListView
			// 
			CellsListView.AutoArrange = false;
			CellsListView.CheckBoxes = true;
			CellsListView.Columns.AddRange(new ColumnHeader[] {
			                               	TechColumnHeader,
			                               	CellNameColumnHeader,
			                               	CellIdColumnHeader,
			                               	LacTacColumnHeader,
			                               	SwitchColumnHeader,
			                               	NocColumnHeader});
			CellsListView.Enabled = false;
			CellsListView.FullRowSelect = true;
			CellsListView.GridLines = true;
			CellsListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			CellsListView.Name = "CellsListView";
			CellsListView.TabIndex = 148;
			CellsListView.UseCompatibleStateImageBehavior = false;
			CellsListView.View = View.Details;
			CellsListView.ItemChecked += CellsListViewItemChecked;
			// 
			// SelectAllButton
			// 
			SelectAllButton.Enabled = false;
			SelectAllButton.Name = "SelectAllButton";
			SelectAllButton.TabIndex = 154;
			SelectAllButton.Text = "Select all";
			SelectAllButton.UseVisualStyleBackColor = true;
			SelectAllButton.Click += SelectAllButtonClick;
			// 
			// SelectNoneButton
			// 
			SelectNoneButton.Enabled = false;
			SelectNoneButton.Name = "SelectNoneButton";
			SelectNoneButton.TabIndex = 155;
			SelectNoneButton.Text = "Select none";
			SelectNoneButton.UseVisualStyleBackColor = true;
			SelectNoneButton.Click += SelectNoneButtonClick;
			// 
			// WarningLabel
			// 
			WarningLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			WarningLabel.ForeColor = Color.Red;
			WarningLabel.Name = "WarningLabel";
			WarningLabel.TabIndex = 156;
			WarningLabel.Text = "WARNING:";
			WarningLabel.Visible = false;
			// 
			// LockScriptLabel
			// 
			LockScriptLabel.Name = "LockScriptLabel";
			LockScriptLabel.TabIndex = 144;
			LockScriptLabel.Text = "Lock cells";
			LockScriptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// LockScriptTextBox
			// 
			LockScriptTextBox.Font = new Font("Courier New", 8.25F);
			LockScriptTextBox.Name = "LockScriptTextBox";
			LockScriptTextBox.ReadOnly = true;
			LockScriptTextBox.TabIndex = 137;
			LockScriptTextBox.Text = "";
			LockScriptTextBox.TextChanged += LockScriptTextBoxTextChanged;
			// 
			// LockScriptLargeTextButton
			// 
			LockScriptLargeTextButton.Enabled = false;
			LockScriptLargeTextButton.Name = "LockScriptLargeTextButton";
			LockScriptLargeTextButton.TabIndex = 139;
			LockScriptLargeTextButton.Text = "...";
			LockScriptLargeTextButton.UseVisualStyleBackColor = true;
			LockScriptLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// UnlockScriptLabel
			// 
			UnlockScriptLabel.Name = "UnlockScriptLabel";
			UnlockScriptLabel.TabIndex = 146;
			UnlockScriptLabel.Text = "Unlock cells";
			UnlockScriptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// UnlockScriptTextBox
			// 
			UnlockScriptTextBox.Font = new Font("Courier New", 8.25F);
			UnlockScriptTextBox.Name = "UnlockScriptTextBox";
			UnlockScriptTextBox.ReadOnly = true;
			UnlockScriptTextBox.TabIndex = 140;
			UnlockScriptTextBox.Text = "";
			UnlockScriptTextBox.TextChanged += UnlockScriptTextBoxTextChanged;
			// 
			// UnlockScriptLargeTextButton
			// 
			UnlockScriptLargeTextButton.Enabled = false;
			UnlockScriptLargeTextButton.Name = "UnlockScriptLargeTextButton";
			UnlockScriptLargeTextButton.TabIndex = 141;
			UnlockScriptLargeTextButton.Text = "...";
			UnlockScriptLargeTextButton.UseVisualStyleBackColor = true;
			UnlockScriptLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// GsmRadioButton
			// 
			GsmRadioButton.Enabled = false;
			GsmRadioButton.Location = new Point(18, 19);
			GsmRadioButton.Name = "GsmRadioButton";
			GsmRadioButton.Size = new Size(45, 24);
			GsmRadioButton.TabIndex = 0;
			GsmRadioButton.TabStop = true;
			GsmRadioButton.Text = "2G";
			GsmRadioButton.UseVisualStyleBackColor = true;
			GsmRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// UmtsRadioButton
			// 
			UmtsRadioButton.Enabled = false;
			UmtsRadioButton.Location = new Point(18, 49);
			UmtsRadioButton.Name = "UmtsRadioButton";
			UmtsRadioButton.Size = new Size(45, 24);
			UmtsRadioButton.TabIndex = 1;
			UmtsRadioButton.TabStop = true;
			UmtsRadioButton.Text = "3G";
			UmtsRadioButton.UseVisualStyleBackColor = true;
			UmtsRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// LteRadioButton
			// 
			LteRadioButton.Enabled = false;
			LteRadioButton.Location = new Point(18, 79);
			LteRadioButton.Name = "LteRadioButton";
			LteRadioButton.Size = new Size(45, 24);
			LteRadioButton.TabIndex = 2;
			LteRadioButton.TabStop = true;
			LteRadioButton.Text = "4G";
			LteRadioButton.UseVisualStyleBackColor = true;
			LteRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// TechColumnHeader
			// 
			TechColumnHeader.Name = "TechColumnHeader";
			TechColumnHeader.Text = "Tech";
			TechColumnHeader.Width = 43;
			// 
			// CellNameColumnHeader
			// 
			CellNameColumnHeader.Name = "CellNameColumnHeader";
			CellNameColumnHeader.Text = "Cell Name";
			CellNameColumnHeader.Width = 75;
			// 
			// CellIdColumnHeader
			// 
			CellIdColumnHeader.Name = "CellIdColumnHeader";
			CellIdColumnHeader.Text = "Cell ID";
			CellIdColumnHeader.Width = 43;
			// 
			// LacTacColumnHeader
			// 
			LacTacColumnHeader.Name = "LacTacColumnHeader";
			LacTacColumnHeader.Text = "LAC TAC";
			LacTacColumnHeader.Width = 56;
			// 
			// SwitchColumnHeader
			// 
			SwitchColumnHeader.Name = "SwitchColumnHeader";
			SwitchColumnHeader.Text = "Switch";
			SwitchColumnHeader.Width = 71;
			// 
			// NocColumnHeader
			// 
			NocColumnHeader.Name = "NocColumnHeader";
			NocColumnHeader.Text = "NOC";
			NocColumnHeader.Width = 49;
			
			TechnologyGroupBox.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			SiteLabel.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			SiteLabel.Size = new Size(30, 20);
			
			SiteTextBox.Location = new Point(SiteLabel.Right + 2, MainMenu.Bottom + 4);
			SiteTextBox.Size = new Size(80, 20);
			
			PriorityLabel.Location = new Point(SiteTextBox.Right + 10, MainMenu.Bottom + 4);
			PriorityLabel.Size = new Size(41, 20);
			
			PriorityTextBox.Location = new Point(PriorityLabel.Right + 2, MainMenu.Bottom + 4);
			PriorityTextBox.Size = new Size(95, 20);
			
			RegionLabel.Location = new Point(PriorityTextBox.Right + 10, MainMenu.Bottom + 4);
			RegionLabel.Size = new Size(43, 20);
			
			RegionTextBox.Location = new Point(RegionLabel.Right + 2, MainMenu.Bottom + 4);
			RegionTextBox.Size = new Size(195, 20);
			
//			CellsSummaryPictureBox.Size = new Size(403, 75);
			CellsSummaryPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
			CellsSummaryPictureBox.Location = new Point(((510 + (PaddingLeftRight * 2)) - CellsSummaryPictureBox.Width) / 2, SiteTextBox.Bottom + 6);
			
			TechnologyGroupBox.Location = new Point(PaddingLeftRight, CellsSummaryPictureBox.Bottom + 6);
			TechnologyGroupBox.Size = new Size(81, 113);
			
			CellsLabel.Location = new Point(TechnologyGroupBox.Right + 5, CellsSummaryPictureBox.Bottom + 6);
			CellsLabel.Size = new Size(72, 20);
			
			CellsListView.Location = new Point(CellsLabel.Right + 5, CellsSummaryPictureBox.Bottom + 6);
			CellsListView.Size = new Size(347, 185);
			
			SelectAllButton.Location = new Point(TechnologyGroupBox.Right + 5, CellsLabel.Bottom + 4);
			SelectAllButton.Size = new Size(72, 23);
			
			SelectNoneButton.Location = new Point(TechnologyGroupBox.Right + 5, SelectAllButton.Bottom + 2);
			SelectNoneButton.Size = new Size(72, 23);
			
			WarningLabel.Location = new Point(paddingLeftRight, TechnologyGroupBox.Bottom + 2);
			WarningLabel.Size = new Size(159, 70);
			
			LockScriptLabel.Location = new Point(paddingLeftRight, WarningLabel.Bottom + 2);
			LockScriptLabel.Size = new Size(76, 20);
			
			LockScriptTextBox.Location = new Point(paddingLeftRight, LockScriptLabel.Bottom + 2);
			LockScriptTextBox.Size = new Size(250, 272);
			
			LockScriptLargeTextButton.Size = new Size(24, 20);
			LockScriptLargeTextButton.Location = new Point(LockScriptTextBox.Right - LockScriptLargeTextButton.Width, LockScriptLabel.Top);
			
			UnlockScriptLabel.Location = new Point(paddingLeftRight, WarningLabel.Bottom + 2);
			UnlockScriptLabel.Size = new Size(76, 20);
			
			UnlockScriptTextBox.Location = new Point(LockScriptTextBox.Right + 10, UnlockScriptLabel.Bottom + 2);
			UnlockScriptTextBox.Size = new Size(250, 272);
			
			UnlockScriptLargeTextButton.Size = new Size(24, 20);
			UnlockScriptLargeTextButton.Location = new Point(UnlockScriptTextBox.Right - UnlockScriptLargeTextButton.Width, UnlockScriptLabel.Top);
			
			Size = new Size(UnlockScriptTextBox.Right + PaddingLeftRight, UnlockScriptTextBox.Bottom + PaddingTopBottom);
		}
	}
}

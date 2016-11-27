/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 22/11/2016
 * Time: 15:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.OssScripts.UI
{
	/// <summary>
	/// Description of Huawei.
	/// </summary>
	public class HuaweiScriptsControls : Panel
	{
		GroupBox TechnologyGroupBox = new GroupBox();
		RadioButton GsmRadioButton = new RadioButton();
		RadioButton UmtsRadioButton = new RadioButton();
		RadioButton LteRadioButton = new RadioButton();
		ComboBox CabinetComboBox = new ComboBox();
		Label SiteLabel = new Label();
		Label ListCellsLabel = new Label();
		Label CellsListLabel = new Label();
		Label LockScriptLabel = new Label();
		Label UnlockScriptLabel = new Label();
		Label CabinetLabel = new Label();
		Button UnlockScriptLargeTextButton = new Button();
		Button LockScriptLargeTextButton = new Button();
		Button CellsListLargeTextButton = new Button();
		TextBox ListCellsTextBox = new TextBox();
		TextBox SiteTextBox = new TextBox();
		AMTRichTextBox CellsListTextBox = new AMTRichTextBox();
		AMTRichTextBox LockScriptTextBox = new AMTRichTextBox();
		AMTRichTextBox UnlockScriptTextBox = new AMTRichTextBox();
		
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
		
//		int selectedTech;
		int SelectedTech {
			get; // {
//				int radioch = 0;
//				foreach(Control radio in TechnologyGroupBox.Controls) {
//					RadioButton rb = radio as RadioButton;
//					if(rb.Checked)
//						return radioch;
//					radioch++;
//				}
//				return 4;
//			}
			set; // { selectedTech = value; }
		}
		
		public HuaweiScriptsControls()
		{
			InitializeComponent();
		}

		void RadioButtonsCheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if(rb.Checked) {
				switch(rb.Name) {
					case "GsmRadioButton":
						SelectedTech = 0;
						break;
					case "UmtsRadioButton":
						SelectedTech = 1;
						break;
					case "LteRadioButton":
						SelectedTech = 2;
						break;
				}
				clearToolStripMenuItem.Enabled = true;
			}
			CellsListTextBox.Text = string.Empty;
			LockScriptTextBox.Text = string.Empty;
			UnlockScriptTextBox.Text = string.Empty;
			SiteTextBox.Enabled = true;
			CabinetComboBox.Enabled = false;
			SiteTextBoxTextChanged(null, null);
		}

		void CabinetComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			SiteTextBoxTextChanged(null, null);
		}

		void SiteTextBoxTextChanged(object sender, EventArgs e)
		{
			if (SiteTextBox.Text != string.Empty) {
				switch (SelectedTech) {
					case 0:
						ListCellsTextBox.Text = "LST GCELL:IDTYPE=BYNAME,BTSNAME=" + '"' + "GSM" + SiteTextBox.Text + CabinetComboBox.Text + '"' + ";";
						break;
					case 1:
						string site = SiteTextBox.Text;
						while (site.Length < 5) {
							site = '0' + site;
						}
						ListCellsTextBox.Text = "DSP UCELL:DSPT=BYNODEB,NODEBNAME=" + '"' + "UMTS" + site + CabinetComboBox.Text + '"' + ";";
						break;
					case 2:
						ListCellsTextBox.Text = "LST CELL:;";
						break;
				}
				CellsListTextBox.Enabled = true;
				CabinetComboBox.Enabled = true;
			}
			else {
				ListCellsTextBox.Text = string.Empty;
				CellsListTextBox.Enabled = false;
				CabinetComboBox.Enabled = false;
				CabinetComboBox.SelectedIndex = 0;
			}
			CellsListTextBox.Text = string.Empty;
			LockScriptTextBox.Text = string.Empty;
			UnlockScriptTextBox.Text = string.Empty;
		}

		void RichTextBoxesTextChanged(object sender, EventArgs e)
		{
			TextBoxBase tb = (TextBoxBase)sender;
			switch(tb.Name) {
				case "CellsListTextBox":
					LockScriptTextBox.Text = UnlockScriptTextBox.Text = string.Empty;
					CellsListLargeTextButton.Enabled = generateScriptsToolStripMenuItem.Enabled = !string.IsNullOrEmpty(tb.Text);
					break;
				case "LockScriptTextBox":
					LockScriptLargeTextButton.Enabled = copyLockScriptToolStripMenuItem.Enabled = !string.IsNullOrEmpty(tb.Text);
					break;
				case "UnlockScriptTextBox":
					UnlockScriptLargeTextButton.Enabled = copyUnlockScriptToolStripMenuItem.Enabled = !string.IsNullOrEmpty(tb.Text);
					break;
			}
		}

		void generateScripts(object sender, EventArgs e)
		{
			LockScriptTextBox.Text = UnlockScriptTextBox.Text = string.Empty;
			string[] cells = CellsListTextBox.Text.Split('\n');
			switch (SelectedTech) {
				case 0:
					if (CellsListTextBox.Text.Contains("GSM")) {
						foreach (string row in cells) {
							if (!string.IsNullOrEmpty(row)) {
								string cell = string.Empty;
								string row2 = row.TrimStart(' ');
								foreach (char ch in row2) {
									if (!Char.IsDigit(ch))
										break;
									cell += ch;
								}
								if (!string.IsNullOrEmpty(cell)) {
									LockScriptTextBox.Text += "SET GCELLADMSTAT:IDTYPE=BYID,CELLID=" + cell + ",ADMSTAT=LOCK;\r\n";
									UnlockScriptTextBox.Text += "SET GCELLADMSTAT:IDTYPE=BYID,CELLID=" + cell + ",ADMSTAT=UNLOCK;\r\n";
								}
							}
						}
					}
					break;
				case 1:
					foreach (string row in cells) {
						if (!string.IsNullOrEmpty(row)) {
							string cell = string.Empty;
							string row2 = row.TrimStart(' ');
							foreach (char ch in row2) {
								if (!Char.IsDigit(ch)) break;
								cell += ch;
							}
							LockScriptTextBox.Text += "BLK UCELL:CELLID=" + cell + ",PRIORITY=HIGH;\r\n";
							UnlockScriptTextBox.Text += "UBL UCELL:CELLID=" + cell + ";\r\n";
						}
					}
					break;
				case 2:
					foreach (string row in cells) {
						if (!string.IsNullOrEmpty(row)) {
							string cell = string.Empty;
							string row2 = row.TrimStart(' ');
							foreach (char ch in row2) {
								if (!Char.IsDigit(ch)) break;
								cell += ch;
							}

							LockScriptTextBox.Text += "BLK CELL:LOCALCELLID=" + cell + ",CELLADMINSTATE=CELL_HIGH_BLOCK;\r\n";
							UnlockScriptTextBox.Text += "UBL CELL:LOCALCELLID=" + cell + ";\r\n";
						}
					}
					//}
					break;
			}
			if (!string.IsNullOrEmpty(LockScriptTextBox.Text))
				LockScriptTextBox.Text = LockScriptTextBox.Text.Remove(LockScriptTextBox.Text.Length - 1,1);
			if (!string.IsNullOrEmpty(UnlockScriptTextBox.Text))
				UnlockScriptTextBox.Text = UnlockScriptTextBox.Text.Remove(UnlockScriptTextBox.Text.Length - 1,1);
		}

		void copyLockScript(object sender, EventArgs e)
		{
			Clipboard.SetText(LockScriptTextBox.Text);
		}

		void copyUnlockScript(object sender, EventArgs e)
		{
			Clipboard.SetText(UnlockScriptTextBox.Text);
		}
		
		void LargeTextButtonsClick(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			Button btn = (Button)sender;
			string lbl = string.Empty;
			TextBoxBase tb = null;
			switch(btn.Name) {
				case "CellsListLargeTextButton":
					tb = (TextBoxBase)CellsListTextBox;
					lbl = CellsListLabel.Text;
					break;
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

		void ClearAllControls(object sender, EventArgs e)
		{
			SiteTextBox.Text = string.Empty;
			CabinetComboBox.SelectedIndex = 0;
			GsmRadioButton.Checked = UmtsRadioButton.Checked = LteRadioButton.Checked = false;
			SiteTextBox.Enabled = false;
			clearToolStripMenuItem.Enabled = false;
		}
		
		void InitializeComponent()
		{
			SuspendLayout();
			BackColor = SystemColors.Control;
			Name = "Huawei Scripts GUI";
			Controls.Add(MainMenu);
			Controls.Add(UnlockScriptTextBox);
			Controls.Add(LockScriptTextBox);
			Controls.Add(CellsListTextBox);
			Controls.Add(CabinetComboBox);
			Controls.Add(CabinetLabel);
			Controls.Add(UnlockScriptLargeTextButton);
			Controls.Add(LockScriptLargeTextButton);
			Controls.Add(CellsListLargeTextButton);
			Controls.Add(UnlockScriptLabel);
			Controls.Add(LockScriptLabel);
			Controls.Add(CellsListLabel);
			Controls.Add(TechnologyGroupBox);
			Controls.Add(ListCellsLabel);
			Controls.Add(SiteLabel);
			Controls.Add(ListCellsTextBox);
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
			// TechnologyGroupBox
			// 
			TechnologyGroupBox.Controls.Add(GsmRadioButton);
			TechnologyGroupBox.Controls.Add(UmtsRadioButton);
			TechnologyGroupBox.Controls.Add(LteRadioButton);
			TechnologyGroupBox.Name = "TechnologyGroupBox";
//			TechnologyGroupBox.Size = new Size(190, 50);
			TechnologyGroupBox.TabIndex = 21;
			TechnologyGroupBox.TabStop = false;
			TechnologyGroupBox.Text = "Technology";
			// 
			// GsmRadioButton
			// 
			GsmRadioButton.Location = new Point(6, 19);
			GsmRadioButton.Name = "GsmRadioButton";
			GsmRadioButton.Size = new Size(60, 24);
			GsmRadioButton.TabIndex = 0;
			GsmRadioButton.TabStop = true;
			GsmRadioButton.Text = "GSM";
			GsmRadioButton.UseVisualStyleBackColor = true;
			GsmRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// UmtsRadioButton
			// 
			UmtsRadioButton.Location = new Point(72, 19);
			UmtsRadioButton.Name = "UmtsRadioButton";
			UmtsRadioButton.Size = new Size(60, 24);
			UmtsRadioButton.TabIndex = 1;
			UmtsRadioButton.TabStop = true;
			UmtsRadioButton.Text = "UMTS";
			UmtsRadioButton.UseVisualStyleBackColor = true;
			UmtsRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// LteRadioButton
			// 
			LteRadioButton.Location = new Point(138, 19);
			LteRadioButton.Name = "LteRadioButton";
			LteRadioButton.Size = new Size(46, 24);
			LteRadioButton.TabIndex = 2;
			LteRadioButton.TabStop = true;
			LteRadioButton.Text = "LTE";
			LteRadioButton.UseVisualStyleBackColor = true;
			LteRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// SiteLabel
			// 
			SiteLabel.ImageAlign = ContentAlignment.MiddleLeft;
			SiteLabel.Name = "SiteLabel";
//			SiteLabel.Size = new Size(53, 20);
			SiteLabel.TabIndex = 18;
			SiteLabel.Text = "Site";
			SiteLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// SiteTextBox
			// 
			SiteTextBox.Enabled = false;
			SiteTextBox.Font = new Font("Courier New", 8.25F);
			SiteTextBox.Name = "SiteTextBox";
//			SiteTextBox.Size = new Size(100, 20);
			SiteTextBox.TabIndex = 16;
			SiteTextBox.TextChanged += SiteTextBoxTextChanged;
			// 
			// CabinetLabel
			// 
			CabinetLabel.ImageAlign = ContentAlignment.MiddleLeft;
			CabinetLabel.Name = "CabinetLabel";
//			CabinetLabel.Size = new Size(45, 20);
			CabinetLabel.TabIndex = 30;
			CabinetLabel.Text = "Cabinet";
			CabinetLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// CabinetComboBox
			// 
			CabinetComboBox.Enabled = false;
			CabinetComboBox.FormattingEnabled = true;
//			CabinetComboBox.ItemHeight = 13;
			CabinetComboBox.Items.AddRange(new object[] {
			                               	"A",
			                               	"B",
			                               	"C"});
			CabinetComboBox.Name = "CabinetComboBox";
//			CabinetComboBox.Size = new Size(40, 20);
			CabinetComboBox.TabIndex = 31;
			CabinetComboBox.Text = "A";
			CabinetComboBox.SelectedIndexChanged += CabinetComboBoxSelectedIndexChanged;
			// 
			// ListCellsLabel
			// 
			ListCellsLabel.ImageAlign = ContentAlignment.MiddleLeft;
			ListCellsLabel.Name = "ListCellsLabel";
//			ListCellsLabel.Size = new Size(53, 20);
			ListCellsLabel.TabIndex = 20;
			ListCellsLabel.Text = "List cells";
			ListCellsLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ListCellsTextBox
			// 
			ListCellsTextBox.Font = new Font("Courier New", 8.25F);
			ListCellsTextBox.Name = "ListCellsTextBox";
			ListCellsTextBox.ReadOnly = true;
//			ListCellsTextBox.Size = new Size(255, 20);
			ListCellsTextBox.TabIndex = 17;
			// 
			// CellsListLabel
			// 
			CellsListLabel.ImageAlign = ContentAlignment.MiddleLeft;
			CellsListLabel.Name = "CellsListLabel";
//			CellsListLabel.Size = new Size(114, 20);
			CellsListLabel.TabIndex = 25;
			CellsListLabel.Text = "Paste cells list here";
			CellsListLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// CellsListTextBox
			// 
			CellsListTextBox.Enabled = false;
			CellsListTextBox.Name = "CellsListTextBox";
//			CellsListTextBox.Size = new Size(509, 221);
			CellsListTextBox.TabIndex = 32;
			CellsListTextBox.Text = "";
			CellsListTextBox.TextChanged += RichTextBoxesTextChanged;
			// 
			// CellsListLargeTextButton
			// 
			CellsListLargeTextButton.Enabled = false;
//			CellsListLargeTextButton.Size = new Size(24, 20);
			CellsListLargeTextButton.Name = "CellsListLargeTextButton";
			CellsListLargeTextButton.TabIndex = 19;
			CellsListLargeTextButton.Text = "...";
			CellsListLargeTextButton.UseVisualStyleBackColor = true;
			CellsListLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// LockScriptLabel
			// 
			LockScriptLabel.ImageAlign = ContentAlignment.MiddleLeft;
			LockScriptLabel.Name = "LockScriptLabel";
//			LockScriptLabel.Size = new Size(76, 20);
			LockScriptLabel.TabIndex = 27;
			LockScriptLabel.Text = "Lock cells";
			LockScriptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// LockScriptRichTextBox
			// 
			LockScriptTextBox.Enabled = false;
			LockScriptTextBox.Name = "LockScriptTextBox";
//			LockScriptTextBox.Size = new Size(250, 272);
			LockScriptTextBox.TabIndex = 33;
			LockScriptTextBox.Text = "";
			LockScriptTextBox.TextChanged += RichTextBoxesTextChanged;
			// 
			// LockScriptLargeTextButton
			// 
			LockScriptLargeTextButton.Enabled = false;
//			LockScriptLargeTextButton.Size = new Size(24, 20);
			LockScriptLargeTextButton.Name = "LockScriptLargeTextButton";
			LockScriptLargeTextButton.TabIndex = 22;
			LockScriptLargeTextButton.Text = "...";
			LockScriptLargeTextButton.UseVisualStyleBackColor = true;
			LockScriptLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// UnlockScriptLabel
			// 
			UnlockScriptLabel.ImageAlign = ContentAlignment.MiddleLeft;
			UnlockScriptLabel.Name = "UnlockScriptLabel";
//			UnlockScriptLabel.Size = new Size(76, 20);
			UnlockScriptLabel.TabIndex = 29;
			UnlockScriptLabel.Text = "Unlock cells";
			UnlockScriptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// UnlockScriptTextBox
			// 
			UnlockScriptTextBox.Enabled = false;
			UnlockScriptTextBox.Name = "UnlockScriptTextBox";
//			UnlockScriptTextBox.Size = new Size(250, 272);
			UnlockScriptTextBox.TabIndex = 34;
			UnlockScriptTextBox.Text = "";
			UnlockScriptTextBox.TextChanged += RichTextBoxesTextChanged;
			// 
			// UnlockScriptLargeTextButton
			// 
			UnlockScriptLargeTextButton.Enabled = false;
//			UnlockScriptLargeTextButton.Size = new Size(24, 20);
			UnlockScriptLargeTextButton.Name = "UnlockScriptLargeTextButton";
			UnlockScriptLargeTextButton.TabIndex = 23;
			UnlockScriptLargeTextButton.Text = "...";
			UnlockScriptLargeTextButton.UseVisualStyleBackColor = true;
			UnlockScriptLargeTextButton.Click += LargeTextButtonsClick;
			
			DynamicControlsSizesLocations();
			TechnologyGroupBox.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}
		
		void DynamicControlsSizesLocations() {
			TechnologyGroupBox.Location = new Point(paddingLeftRight, MainMenu.Bottom + 4);
			TechnologyGroupBox.Size = new Size(190, 50);
			
			SiteLabel.Location = new Point(TechnologyGroupBox. Right + 10, MainMenu.Bottom + 4);
			SiteLabel.Size = new Size(53, 20);
			
			SiteTextBox.Location = new Point(SiteLabel.Right + 2, MainMenu.Bottom + 4);
			SiteTextBox.Size = new Size(100, 20);
			
			CabinetLabel.Location = new Point(SiteTextBox.Right + 60, MainMenu.Bottom + 4);
			CabinetLabel.Size = new Size(45, 20);
			
			CabinetComboBox.ItemHeight = 13;
			CabinetComboBox.Location = new Point(CabinetLabel.Right + 2, MainMenu.Bottom + 4);
			CabinetComboBox.Size = new Size(48, 20);
			
			ListCellsLabel.Location = new Point(TechnologyGroupBox.Right + 10, SiteLabel.Bottom + 10);
			ListCellsLabel.Size = new Size(53, 20);
			
			ListCellsTextBox.Location = new Point(ListCellsLabel.Right + 2, SiteTextBox.Bottom + 10);
			ListCellsTextBox.Size = new Size(255, 20);
			
			CellsListLabel.Location = new Point(paddingLeftRight, TechnologyGroupBox.Bottom + 2);
			CellsListLabel.Size = new Size(120, 20);
			
			CellsListTextBox.Location = new Point(paddingLeftRight, CellsListLabel.Bottom + 2);
			CellsListTextBox.Size = new Size(510, 221);
			
			CellsListLargeTextButton.Size = new Size(24, 20);
			CellsListLargeTextButton.Location = new Point(CellsListTextBox.Right - CellsListLargeTextButton.Width, CellsListLabel.Top);
			
			LockScriptLabel.Location = new Point(paddingLeftRight, CellsListTextBox.Bottom + 2);
			LockScriptLabel.Size = new Size(80, 20);
			
			LockScriptTextBox.Location = new Point(paddingLeftRight, LockScriptLabel.Bottom + 2);
			LockScriptTextBox.Size = new Size(250, 269);
			
			LockScriptLargeTextButton.Size = new Size(24, 20);
			LockScriptLargeTextButton.Location = new Point(LockScriptTextBox.Right - LockScriptLargeTextButton.Width, LockScriptLabel.Top);
			
			UnlockScriptLabel.Location = new Point(LockScriptLargeTextButton.Right + 10, CellsListTextBox.Bottom + 2);
			UnlockScriptLabel.Size = new Size(80, 20);
			
			UnlockScriptTextBox.Location = new Point(LockScriptTextBox.Right + 10, UnlockScriptLabel.Bottom + 2);
			UnlockScriptTextBox.Size = new Size(250, 269);
			
			UnlockScriptLargeTextButton.Size = new Size(24, 20);
			UnlockScriptLargeTextButton.Location = new Point(UnlockScriptTextBox.Right - UnlockScriptLargeTextButton.Width, UnlockScriptLabel.Top);
			
			Size = new Size(UnlockScriptTextBox.Right + PaddingLeftRight, UnlockScriptTextBox.Bottom + PaddingTopBottom);
		}
	}
}

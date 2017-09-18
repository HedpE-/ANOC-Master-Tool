/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 22/11/2016
 * Time: 15:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.UI;
using appCore.Toolbox;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.OssScripts.UI
{
	/// <summary>
	/// Description of Nokia.
	/// </summary>
	public class NokiaScriptsControls : Panel
	{
		GroupBox EquipmentGroupBox = new GroupBox();
		GroupBox BtsDChannelsGroupBox = new GroupBox();
		CheckBox BtsCheckBox1 = new CheckBox();
		CheckBox BtsCheckBox2 = new CheckBox();
		CheckBox BtsCheckBox3 = new CheckBox();
		CheckBox BtsCheckBox4 = new CheckBox();
		CheckBox BtsCheckBox5 = new CheckBox();
		CheckBox BtsCheckBox6 = new CheckBox();
		CheckBox BtsCheckBox7 = new CheckBox();
		CheckBox BtsCheckBox8 = new CheckBox();
		CheckBox BtsCheckBox9 = new CheckBox();
		CheckBox BtsCheckBox10 = new CheckBox();
		AMTTextBox BtsTextBox1 = new AMTTextBox();
		AMTTextBox BtsTextBox2 = new AMTTextBox();
		AMTTextBox BtsTextBox3 = new AMTTextBox();
		AMTTextBox BtsTextBox4 = new AMTTextBox();
		AMTTextBox BtsTextBox5 = new AMTTextBox();
		AMTTextBox BtsTextBox6 = new AMTTextBox();
		AMTTextBox BtsTextBox7 = new AMTTextBox();
		AMTTextBox BtsTextBox8 = new AMTTextBox();
		AMTTextBox BtsTextBox9 = new AMTTextBox();
		AMTTextBox BtsTextBox10 = new AMTTextBox();
		Button dChannelsLargeTextButton = new Button();
		Button UnlockScriptLargeTextButton = new Button();
		Button LockScriptLargeTextButton = new Button();
		Label UnlockScriptLabel = new Label();
		AMTRichTextBox dChannelsTextBox = new AMTRichTextBox();
		AMTRichTextBox UnlockScriptTextBox = new AMTRichTextBox();
		AMTRichTextBox LockScriptTextBox = new AMTRichTextBox();
		Label LockScriptLabel = new Label();
		Label BtsDChannelsCommandLabel = new Label();
		Label BcfPcmLabel = new Label();
		AMTTextBox BtsDChannelsCommandTextBox = new AMTTextBox();
		public AMTTextBox BcfPcmTextBox = new AMTTextBox();
		RadioButton BcfRadioButton = new RadioButton();
		RadioButton PcmRadioButton = new RadioButton();
		
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
		
		int SelectedEquipment {
			get;
			set;
		}
		
		bool BcfCheckedState {
			get {
				foreach (Control control in BtsDChannelsGroupBox.Controls)
				{
					CheckBox cb = control as CheckBox;
					if(cb != null) {
						if(cb.Checked)
							return true;
					}
				}
				return false;
			}
			set { }
		}
		
		public NokiaScriptsControls()
		{
			InitializeComponent();
		}

		void generateScripts(object sender, EventArgs e)
		{
            LockScriptTextBox.Text = string.Empty;
            UnlockScriptTextBox.Text = string.Empty;

            switch (SelectedEquipment) {
				case 0: // NSN BCF
					foreach(Control control in BtsDChannelsGroupBox.Controls)
					{
						CheckBox cb = control as CheckBox;
						if(cb != null) {
							if(cb.Checked) {
								AMTTextBox tb = (AMTTextBox)BtsDChannelsGroupBox.Controls["BtsTextBox" + cb.Name.Substring("BtsCheckBox".Length)];
								LockScriptTextBox.Text += "ZEQS:BTS=" + tb.Text + ":L:FHO,60;\r\n";
								UnlockScriptTextBox.Text += "ZEQS:BTS=" + tb.Text + ":U;\r\n";
							}
						}
					}
					LockScriptTextBox.Text += "\r\nZEFS:" + BcfPcmTextBox.Text + ":L;";
					UnlockScriptTextBox.Text += "\r\nZEFS:" + BcfPcmTextBox.Text + ":U;";
					break;
				case 1: // NSN PCM
					string [] temp = dChannelsTextBox.Lines;
					foreach(string row in temp) {
						if(row.Contains("WO-") || row.Contains("BL-") || row.Contains("UA-")) {
							string DCH = string.Empty;
							foreach(char ch in row) {
								if(ch != ' ')
									DCH += ch;
								else
									break;
							}
							UnlockScriptTextBox.Text += "ZDTC:" + DCH + ":WO;\r\n";
							LockScriptTextBox.Text += "ZDTC:" + DCH + ":BL;\r\n";
						}
					}
					if(!string.IsNullOrEmpty(UnlockScriptTextBox.Text))
						UnlockScriptTextBox.Text = UnlockScriptTextBox.Text.Substring(0, UnlockScriptTextBox.Text.Length - 1);
					if(!string.IsNullOrEmpty(LockScriptTextBox.Text))
						LockScriptTextBox.Text = LockScriptTextBox.Text.Substring(0, LockScriptTextBox.Text.Length - 1);
					break;
			}
		}

		void RadioButtonsCheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if(rb.Checked) {
				clearToolStripMenuItem.Enabled = true;
				BcfPcmTextBox.Enabled = true;
				BtsDChannelsCommandTextBox.Text = BcfPcmTextBox.Text = string.Empty;
//				dChannelsTextBox.Text = UnlockScriptTextBox.Text = LockScriptTextBox.Text = string.Empty;
				BcfPcmTextBoxTextChanged(null, null);
				if(rb.Name == "BcfRadioButton") {
					SelectedEquipment = 0;
//					LockScriptLabel.Text = "Lock site";
//					UnlockScriptLabel.Text = "Unlock site";
					BtsDChannelsCommandLabel.Text = "List BTS's";
					BcfPcmLabel.Text = "BCF";
					BtsDChannelsGroupBox.Text = "Choose BTSs";
					dChannelsTextBox.Visible = dChannelsLargeTextButton.Visible = false;
					foreach (Control control in BtsDChannelsGroupBox.Controls)
					{
						CheckBox cb = control as CheckBox;
						if (cb != null)
							cb.Visible = true;
						else {
							TextBox tb = control as TextBox;
							if (tb != null)
								tb.Visible = true;
						}
					}
				}
				else {
					SelectedEquipment = 1;
//					LockScriptLabel.Text = "Lock D-CHANNELs";
//					UnlockScriptLabel.Text = "Unlock D-CHANNELs";
					BtsDChannelsCommandLabel.Text = "List D-CHANNELs";
					BcfPcmLabel.Text = "PCM";
					BtsDChannelsGroupBox.Text = "Paste D-CHANNELs printout";
					dChannelsTextBox.Visible = dChannelsLargeTextButton.Visible = true;
					foreach(Control control in BtsDChannelsGroupBox.Controls)
					{
						CheckBox cb = control as CheckBox;
						if(cb != null)
							cb.Visible = false;
						else {
							TextBox tb = control as TextBox;
							if(tb != null) {
								if(tb.Name != dChannelsTextBox.Name)
									tb.Visible = false;
							}
						}
					}
				}
			}
			else {
				if(rb.Name == "BcfRadioButton") {
					foreach (Control control in BtsDChannelsGroupBox.Controls)
					{
						CheckBox cb = control as CheckBox;
						if (cb != null)
							cb.Visible = false;
						else {
							TextBox tb = control as TextBox;
							if (tb != null)
								tb.Visible = false;
						}
					}
				}
				else {
				}
				BtsDChannelsGroupBox.Text = string.Empty;
			}
		}

		void BcfPcmTextBoxTextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(BcfPcmTextBox.Text)) {
				if (!BcfPcmTextBox.Text.IsAllDigits()) {
//					Action action = new Action(delegate {
					FlexibleMessageBox.Show("BCF can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
//					                           });
//					Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				switch (SelectedEquipment) {
					case 0:
						BtsDChannelsCommandTextBox.Text = "ZEEI:BCF=" + BcfPcmTextBox.Text + ";";
						foreach (Control control in BtsDChannelsGroupBox.Controls) {
							CheckBox cb = control as CheckBox;
							if(cb != null)
								cb.Enabled = true;
						}
						BtsTextBox1.Text = Convert.ToInt32(BcfPcmTextBox.Text).ToString();
						break;
					case 1:
						BtsDChannelsCommandTextBox.Text = "ZDTI:::PCM=" + BcfPcmTextBox.Text + ";";
						dChannelsTextBox.Enabled = true;
						break;
				}
			}
			else {
				BtsDChannelsCommandTextBox.Text = string.Empty;
				dChannelsTextBox.Enabled = false;
				foreach(Control control in BtsDChannelsGroupBox.Controls)
				{
					CheckBox cb = control as CheckBox;
					if(cb != null) {
						cb.Enabled = false;
					}
					else {
						TextBox tb = control as TextBox;
						if(tb != null)
							tb.Text = string.Empty;
					}
				}
			}
			dChannelsTextBox.Text = UnlockScriptTextBox.Text = LockScriptTextBox.Text = string.Empty;
			foreach (Control control in BtsDChannelsGroupBox.Controls)	{
				CheckBox cb = control as CheckBox;
				if(cb != null)
					cb.Checked = false;
			}
		}

		void dChannelsTextBoxTextChanged(object sender, EventArgs e)
		{
			generateScriptsToolStripMenuItem.Enabled = dChannelsLargeTextButton.Enabled = !string.IsNullOrEmpty(dChannelsTextBox.Text);
			UnlockScriptTextBox.Text = LockScriptTextBox.Text = string.Empty;
		}

		void UnlockScriptTextBoxTextChanged(object sender, EventArgs e)
		{
			copyUnlockScriptToolStripMenuItem.Enabled = UnlockScriptLargeTextButton.Enabled = !string.IsNullOrEmpty(UnlockScriptTextBox.Text);
		}

		void LockScriptTextBoxTextChanged(object sender, EventArgs e)
		{
			copyLockScriptToolStripMenuItem.Enabled = LockScriptLargeTextButton.Enabled = !string.IsNullOrEmpty(LockScriptTextBox.Text);
		}

		void BtsTextBoxesTextChanged(object sender, EventArgs e)
		{
			TextBoxBase tb = sender as TextBoxBase;
			if (!string.IsNullOrEmpty(tb.Text)) {
				if (!tb.Text.IsAllDigits()) {
//					Action action = new Action(delegate {
					FlexibleMessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
//					                           });
//					Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				if(tb.Name != "BtsTextBox10")
					BtsDChannelsGroupBox.Controls["BtsTextBox" + (Convert.ToInt32(tb.Name.Substring("BtsTextBox".Length)) + 1)].Text = (Convert.ToInt32(tb.Text) + 1).ToString();
			}
			UnlockScriptTextBox.Text = LockScriptTextBox.Text = string.Empty;
		}

		void copyLockScript(object sender, EventArgs e)
		{
			Clipboard.SetText(LockScriptTextBox.Text);
			MainForm.trayIcon.showBalloon(string.Empty, "Lock script copied to clipboard.");
		}

		void copyUnlockScript(object sender, EventArgs e)
		{
			Clipboard.SetText(UnlockScriptTextBox.Text);
			MainForm.trayIcon.showBalloon(string.Empty, "Unlock script copied to clipboard.");
		}

		void BtsCheckBoxesCheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			if(cb.Checked)
				BtsDChannelsGroupBox.Controls["BtsTextBox" + cb.Name.Substring("BtsCheckBox".Length)].Enabled = generateScriptsToolStripMenuItem.Enabled = true;
			else {
				BtsDChannelsGroupBox.Controls["BtsTextBox" + cb.Name.Substring("BtsCheckBox".Length)].Enabled = false;
				generateScriptsToolStripMenuItem.Enabled = BcfCheckedState;
			}
			LockScriptTextBox.Text = UnlockScriptTextBox.Text = string.Empty;
		}
		
		void LargeTextButtonsClick(object sender, EventArgs e)
		{
//			Action action = new Action(delegate {
			Button btn = (Button)sender;
			string lbl = string.Empty;
			TextBoxBase tb = null;
			switch(btn.Name) {
				case "dChannelsLargeTextButton":
					tb = (TextBoxBase)dChannelsTextBox;
					lbl = BtsDChannelsGroupBox.Text;
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
			
			AMTLargeTextForm enlarge = new AMTLargeTextForm(tb.Text,lbl, btn.Name != "dChannelsLargeTextButton");
			enlarge.StartPosition = FormStartPosition.CenterParent;
			enlarge.ShowDialog();
			tb.Text = enlarge.finaltext;
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void ClearAllControls(object sender, EventArgs e)
		{
			BcfPcmTextBox.Text = BtsDChannelsCommandTextBox.Text = string.Empty;
			BcfPcmLabel.Text = BtsDChannelsCommandLabel.Text = string.Empty;
			BcfPcmTextBox.Enabled = false;
//			CabinetComboBox.SelectedIndex = 0;
			BcfRadioButton.Checked = PcmRadioButton.Checked = false;
//			SiteTextBox.Enabled = false;
			clearToolStripMenuItem.Enabled = false;
		}
		
		void InitializeComponent()
		{
			SuspendLayout();
			BackColor = SystemColors.Control;
			Name = "Nokia Scripts GUI";
			Controls.Add(MainMenu);
			Controls.Add(LockScriptLabel);
			Controls.Add(BtsDChannelsGroupBox);
			Controls.Add(UnlockScriptLargeTextButton);
			Controls.Add(LockScriptLargeTextButton);
			Controls.Add(UnlockScriptLabel);
			Controls.Add(UnlockScriptTextBox);
			Controls.Add(LockScriptTextBox);
			Controls.Add(BtsDChannelsCommandLabel);
			Controls.Add(BcfPcmLabel);
			Controls.Add(BtsDChannelsCommandTextBox);
			Controls.Add(BcfPcmTextBox);
			Controls.Add(EquipmentGroupBox);
			
			BtsDChannelsGroupBox.SuspendLayout();
			EquipmentGroupBox.SuspendLayout();
			// 
			// MainMenu
			// 
			MainMenu.Location = new Point(PaddingLeftRight, 0);
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
			// EquipmentGroupBox
			// 
			EquipmentGroupBox.Controls.Add(BcfRadioButton);
			EquipmentGroupBox.Controls.Add(PcmRadioButton);
//			EquipmentGroupBox.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
//			EquipmentGroupBox.Size = new Size(133, 50);
			EquipmentGroupBox.Name = "EquipmentGroupBox";
			EquipmentGroupBox.RightToLeft = RightToLeft.No;
			EquipmentGroupBox.TabIndex = 92;
			EquipmentGroupBox.TabStop = false;
			EquipmentGroupBox.Text = "Equipment";
			// 
			// BcfRadioButton
			// 
			BcfRadioButton.Location = new Point(10, 19);
			BcfRadioButton.Name = "BcfRadioButton";
			BcfRadioButton.RightToLeft = RightToLeft.No;
			BcfRadioButton.Size = new Size(50, 24);
			BcfRadioButton.TabIndex = 0;
			BcfRadioButton.TabStop = true;
			BcfRadioButton.Text = "BCF";
			BcfRadioButton.UseVisualStyleBackColor = true;
			BcfRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// PcmRadioButton
			// 
			PcmRadioButton.Location = new Point(75, 19);
			PcmRadioButton.Name = "PcmRadioButton";
			PcmRadioButton.RightToLeft = RightToLeft.No;
			PcmRadioButton.Size = new Size(50, 24);
			PcmRadioButton.TabIndex = 1;
			PcmRadioButton.TabStop = true;
			PcmRadioButton.Text = "PCM";
			PcmRadioButton.UseVisualStyleBackColor = true;
			PcmRadioButton.CheckedChanged += RadioButtonsCheckedChanged;
			// 
			// BcfPcmLabel
			// 
			BcfPcmLabel.ImageAlign = ContentAlignment.MiddleLeft;
//			BcfPcmLabel.Location = new Point(EquipmentGroupBox.Right + 2, MainMenu.Bottom + 4);
//			BcfPcmLabel.Size = new Size(93, 20);
			BcfPcmLabel.Name = "BcfPcmLabel";
			BcfPcmLabel.RightToLeft = RightToLeft.No;
			BcfPcmLabel.TabIndex = 93;
			BcfPcmLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// BcfPcmTextBox
			// 
			BcfPcmTextBox.Enabled = false;
			BcfPcmTextBox.Font = new Font("Courier New", 8.25F);
//			BcfPcmTextBox.Location = new Point(BcfPcmLabel.Right + 2, MainMenu.Bottom + 4);
//			BcfPcmTextBox.Size = new Size(100, 20);
			BcfPcmTextBox.Name = "BcfPcmTextBox";
			BcfPcmTextBox.RightToLeft = RightToLeft.No;
			BcfPcmTextBox.TabIndex = 90;
			BcfPcmTextBox.TextChanged += BcfPcmTextBoxTextChanged;
			// 
			// BtsDChannelsCommandLabel
			// 
			BtsDChannelsCommandLabel.ImageAlign = ContentAlignment.MiddleLeft;
//			BtsDChannelsCommandLabel.Location = new Point(EquipmentGroupBox.Right + 2, BcfPcmLabel.Bottom + 10);
//			BtsDChannelsCommandLabel.Size = new Size(93, 20);
			BtsDChannelsCommandLabel.Name = "BtsDChannelsCommandLabel";
			BtsDChannelsCommandLabel.RightToLeft = RightToLeft.No;
			BtsDChannelsCommandLabel.TabIndex = 94;
			BtsDChannelsCommandLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// BtsDChannelsCommandTextBox
			// 
			BtsDChannelsCommandTextBox.Font = new Font("Courier New", 8.25F);
//			BtsDChannelsCommandTextBox.Location = new Point(BtsDChannelsCommandLabel.Right + 2, BcfPcmTextBox.Bottom + 10);
//			BtsDChannelsCommandTextBox.Size = new Size(275, 20);
			BtsDChannelsCommandTextBox.Name = "BtsDChannelsCommandTextBox";
			BtsDChannelsCommandTextBox.ReadOnly = true;
			BtsDChannelsCommandTextBox.RightToLeft = RightToLeft.No;
			BtsDChannelsCommandTextBox.TabIndex = 91;
			// 
			// BtsDChannelsGroupBox
			// 
			BtsDChannelsGroupBox.Controls.Add(dChannelsLargeTextButton);
			BtsDChannelsGroupBox.Controls.Add(dChannelsTextBox);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox1);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox2);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox3);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox4);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox5);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox6);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox7);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox8);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox9);
			BtsDChannelsGroupBox.Controls.Add(BtsCheckBox10);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox1);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox10);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox2);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox9);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox3);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox8);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox4);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox7);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox5);
			BtsDChannelsGroupBox.Controls.Add(BtsTextBox6);
//			BtsDChannelsGroupBox.Location = new Point(PaddingLeftRight, EquipmentGroupBox.Bottom + 2);
//			BtsDChannelsGroupBox.Size = new Size(510, 245);
			BtsDChannelsGroupBox.Name = "BtsDChannelsGroupBox";
			BtsDChannelsGroupBox.RightToLeft = RightToLeft.No;
			BtsDChannelsGroupBox.TabIndex = 103;
			BtsDChannelsGroupBox.TabStop = false;
			// 
			// BtsCheckBox1
			// 
			BtsCheckBox1.Enabled = false;
			BtsCheckBox1.Location = new Point(64, 49);
			BtsCheckBox1.Name = "BtsCheckBox1";
			BtsCheckBox1.RightToLeft = RightToLeft.Yes;
			BtsCheckBox1.Size = new Size(53, 24);
			BtsCheckBox1.TabIndex = 4;
			BtsCheckBox1.Text = "BTS";
			BtsCheckBox1.UseVisualStyleBackColor = true;
			BtsCheckBox1.Visible = false;
			BtsCheckBox1.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox2
			// 
			BtsCheckBox2.Enabled = false;
			BtsCheckBox2.Location = new Point(64, 79);
			BtsCheckBox2.Name = "BtsCheckBox2";
			BtsCheckBox2.RightToLeft = RightToLeft.Yes;
			BtsCheckBox2.Size = new Size(53, 24);
			BtsCheckBox2.TabIndex = 6;
			BtsCheckBox2.Text = "BTS";
			BtsCheckBox2.UseVisualStyleBackColor = true;
			BtsCheckBox2.Visible = false;
			BtsCheckBox2.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox3
			// 
			BtsCheckBox3.Enabled = false;
			BtsCheckBox3.Location = new Point(64, 109);
			BtsCheckBox3.Name = "BtsCheckBox3";
			BtsCheckBox3.RightToLeft = RightToLeft.Yes;
			BtsCheckBox3.Size = new Size(53, 24);
			BtsCheckBox3.TabIndex = 8;
			BtsCheckBox3.Text = "BTS";
			BtsCheckBox3.UseVisualStyleBackColor = true;
			BtsCheckBox3.Visible = false;
			BtsCheckBox3.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox4
			// 
			BtsCheckBox4.Enabled = false;
			BtsCheckBox4.Location = new Point(64, 139);
			BtsCheckBox4.Name = "BtsCheckBox4";
			BtsCheckBox4.RightToLeft = RightToLeft.Yes;
			BtsCheckBox4.Size = new Size(53, 24);
			BtsCheckBox4.TabIndex = 10;
			BtsCheckBox4.Text = "BTS";
			BtsCheckBox4.UseVisualStyleBackColor = true;
			BtsCheckBox4.Visible = false;
			BtsCheckBox4.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox5
			// 
			BtsCheckBox5.Enabled = false;
			BtsCheckBox5.Location = new Point(64, 169);
			BtsCheckBox5.Name = "BtsCheckBox5";
			BtsCheckBox5.RightToLeft = RightToLeft.Yes;
			BtsCheckBox5.Size = new Size(53, 24);
			BtsCheckBox5.TabIndex = 12;
			BtsCheckBox5.Text = "BTS";
			BtsCheckBox5.UseVisualStyleBackColor = true;
			BtsCheckBox5.Visible = false;
			BtsCheckBox5.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox6
			// 
			BtsCheckBox6.Enabled = false;
			BtsCheckBox6.Location = new Point(319, 49);
			BtsCheckBox6.Name = "BtsCheckBox6";
			BtsCheckBox6.RightToLeft = RightToLeft.Yes;
			BtsCheckBox6.Size = new Size(53, 24);
			BtsCheckBox6.TabIndex = 14;
			BtsCheckBox6.Text = "BTS";
			BtsCheckBox6.UseVisualStyleBackColor = true;
			BtsCheckBox6.Visible = false;
			BtsCheckBox6.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox7
			// 
			BtsCheckBox7.Enabled = false;
			BtsCheckBox7.Location = new Point(319, 79);
			BtsCheckBox7.Name = "BtsCheckBox7";
			BtsCheckBox7.RightToLeft = RightToLeft.Yes;
			BtsCheckBox7.Size = new Size(53, 24);
			BtsCheckBox7.TabIndex = 16;
			BtsCheckBox7.Text = "BTS";
			BtsCheckBox7.UseVisualStyleBackColor = true;
			BtsCheckBox7.Visible = false;
			BtsCheckBox7.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox8
			// 
			BtsCheckBox8.Enabled = false;
			BtsCheckBox8.Location = new Point(319, 109);
			BtsCheckBox8.Name = "BtsCheckBox8";
			BtsCheckBox8.RightToLeft = RightToLeft.Yes;
			BtsCheckBox8.Size = new Size(53, 24);
			BtsCheckBox8.TabIndex = 18;
			BtsCheckBox8.Text = "BTS";
			BtsCheckBox8.UseVisualStyleBackColor = true;
			BtsCheckBox8.Visible = false;
			BtsCheckBox8.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox9
			// 
			BtsCheckBox9.Enabled = false;
			BtsCheckBox9.Location = new Point(319, 139);
			BtsCheckBox9.Name = "BtsCheckBox9";
			BtsCheckBox9.RightToLeft = RightToLeft.Yes;
			BtsCheckBox9.Size = new Size(53, 24);
			BtsCheckBox9.TabIndex = 20;
			BtsCheckBox9.Text = "BTS";
			BtsCheckBox9.UseVisualStyleBackColor = true;
			BtsCheckBox9.Visible = false;
			BtsCheckBox9.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsCheckBox10
			// 
			BtsCheckBox10.Enabled = false;
			BtsCheckBox10.Location = new Point(319, 169);
			BtsCheckBox10.Name = "BtsCheckBox10";
			BtsCheckBox10.RightToLeft = RightToLeft.Yes;
			BtsCheckBox10.Size = new Size(53, 24);
			BtsCheckBox10.TabIndex = 22;
			BtsCheckBox10.Text = "BTS";
			BtsCheckBox10.UseVisualStyleBackColor = true;
			BtsCheckBox10.Visible = false;
			BtsCheckBox10.CheckedChanged += BtsCheckBoxesCheckedChanged;
			// 
			// BtsTextBox1
			// 
			BtsTextBox1.Enabled = false;
			BtsTextBox1.Font = new Font("Courier New", 8.25F);
			BtsTextBox1.Location = new Point(125, 51);
			BtsTextBox1.Name = "BtsTextBox1";
			BtsTextBox1.Size = new Size(79, 20);
			BtsTextBox1.TabIndex = 5;
			BtsTextBox1.Visible = false;
			BtsTextBox1.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox2
			// 
			BtsTextBox2.Enabled = false;
			BtsTextBox2.Font = new Font("Courier New", 8.25F);
			BtsTextBox2.Location = new Point(125, 79);
			BtsTextBox2.Name = "BtsTextBox2";
			BtsTextBox2.Size = new Size(79, 20);
			BtsTextBox2.TabIndex = 7;
			BtsTextBox2.Visible = false;
			BtsTextBox2.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox3
			// 
			BtsTextBox3.Enabled = false;
			BtsTextBox3.Font = new Font("Courier New", 8.25F);
			BtsTextBox3.Location = new Point(125, 109);
			BtsTextBox3.Name = "BtsTextBox3";
			BtsTextBox3.Size = new Size(79, 20);
			BtsTextBox3.TabIndex = 9;
			BtsTextBox3.Visible = false;
			BtsTextBox3.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox4
			// 
			BtsTextBox4.Enabled = false;
			BtsTextBox4.Font = new Font("Courier New", 8.25F);
			BtsTextBox4.Location = new Point(125, 139);
			BtsTextBox4.Name = "BtsTextBox4";
			BtsTextBox4.Size = new Size(79, 20);
			BtsTextBox4.TabIndex = 11;
			BtsTextBox4.Visible = false;
			BtsTextBox4.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox5
			// 
			BtsTextBox5.Enabled = false;
			BtsTextBox5.Font = new Font("Courier New", 8.25F);
			BtsTextBox5.Location = new Point(125, 169);
			BtsTextBox5.Name = "BtsTextBox5";
			BtsTextBox5.Size = new Size(79, 20);
			BtsTextBox5.TabIndex = 13;
			BtsTextBox5.Visible = false;
			BtsTextBox5.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox6
			// 
			BtsTextBox6.Enabled = false;
			BtsTextBox6.Font = new Font("Courier New", 8.25F);
			BtsTextBox6.Location = new Point(380, 49);
			BtsTextBox6.Name = "BtsTextBox6";
			BtsTextBox6.Size = new Size(79, 20);
			BtsTextBox6.TabIndex = 15;
			BtsTextBox6.Visible = false;
			BtsTextBox6.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox7
			// 
			BtsTextBox7.Enabled = false;
			BtsTextBox7.Font = new Font("Courier New", 8.25F);
			BtsTextBox7.Location = new Point(380, 79);
			BtsTextBox7.Name = "BtsTextBox7";
			BtsTextBox7.Size = new Size(79, 20);
			BtsTextBox7.TabIndex = 17;
			BtsTextBox7.Visible = false;
			BtsTextBox7.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox8
			// 
			BtsTextBox8.Enabled = false;
			BtsTextBox8.Font = new Font("Courier New", 8.25F);
			BtsTextBox8.Location = new Point(380, 109);
			BtsTextBox8.Name = "BtsTextBox8";
			BtsTextBox8.Size = new Size(79, 20);
			BtsTextBox8.TabIndex = 19;
			BtsTextBox8.Visible = false;
			BtsTextBox8.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox9
			// 
			BtsTextBox9.Enabled = false;
			BtsTextBox9.Font = new Font("Courier New", 8.25F);
			BtsTextBox9.Location = new Point(380, 139);
			BtsTextBox9.Name = "BtsTextBox9";
			BtsTextBox9.Size = new Size(79, 20);
			BtsTextBox9.TabIndex = 21;
			BtsTextBox9.Visible = false;
			BtsTextBox9.TextChanged += BtsTextBoxesTextChanged;
			// 
			// BtsTextBox10
			// 
			BtsTextBox10.Enabled = false;
			BtsTextBox10.Font = new Font("Courier New", 8.25F);
			BtsTextBox10.Location = new Point(380, 169);
			BtsTextBox10.Name = "BtsTextBox10";
			BtsTextBox10.Size = new Size(79, 20);
			BtsTextBox10.TabIndex = 23;
			BtsTextBox10.Visible = false;
			BtsTextBox10.TextChanged += BtsTextBoxesTextChanged;
			// 
			// dChannelsTextBox
			// 
			dChannelsTextBox.Enabled = false;
			dChannelsTextBox.Font = new Font("Courier New", 8.25F);
			dChannelsTextBox.Location = new Point(6, 32);
			dChannelsTextBox.Name = "dChannelsTextBox";
			dChannelsTextBox.Size = new Size(499, 205);
			dChannelsTextBox.TabIndex = 24;
			dChannelsTextBox.Text = "";
			dChannelsTextBox.Visible = false;
			dChannelsTextBox.TextChanged += dChannelsTextBoxTextChanged;
			// 
			// dChannelsLargeTextButton
			// 
			dChannelsLargeTextButton.Enabled = false;
			dChannelsLargeTextButton.Location = new Point(481, 10);
			dChannelsLargeTextButton.Name = "dChannelsLargeTextButton";
			dChannelsLargeTextButton.Size = new Size(24, 20);
			dChannelsLargeTextButton.TabIndex = 25;
			dChannelsLargeTextButton.Text = "...";
			dChannelsLargeTextButton.UseVisualStyleBackColor = true;
			dChannelsLargeTextButton.Visible = false;
			dChannelsLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// LockScriptLabel
			// 
			LockScriptLabel.ImageAlign = ContentAlignment.MiddleLeft;
//			LockScriptLabel.Location = new Point(PaddingLeftRight, BtsDChannelsGroupBox.Bottom + 2);
//			LockScriptLabel.Size = new Size(100, 13);
			LockScriptLabel.Name = "LockScriptLabel";
			LockScriptLabel.Text = "Lock Script";
			LockScriptLabel.TabIndex = 104;
			LockScriptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// LockScriptTextBox
			// 
			LockScriptTextBox.Font = new Font("Courier New", 8.25F);
//			LockScriptTextBox.Location = new Point(PaddingLeftRight, LockScriptLabel.Bottom + 2);
//			LockScriptTextBox.Size = new Size(250, 254);
			LockScriptTextBox.Name = "LockScriptTextBox";
			LockScriptTextBox.ReadOnly = true;
			LockScriptTextBox.TabIndex = 95;
			LockScriptTextBox.Text = "";
			LockScriptTextBox.TextChanged += LockScriptTextBoxTextChanged;
			// 
			// LockScriptLargeTextButton
			// 
			LockScriptLargeTextButton.Enabled = false;
//			LockScriptLargeTextButton.Size = new Size(24, 20);
//			LockScriptLargeTextButton.Location = new Point(LockScriptTextBox.Right - LockScriptLargeTextButton.Width, LockScriptLabel.Top);
			LockScriptLargeTextButton.Name = "LockScriptLargeTextButton";
			LockScriptLargeTextButton.TabIndex = 96;
			LockScriptLargeTextButton.Text = "...";
			LockScriptLargeTextButton.UseVisualStyleBackColor = true;
			LockScriptLargeTextButton.Click += LargeTextButtonsClick;
			// 
			// UnlockScriptLabel
			// 
			UnlockScriptLabel.ImageAlign = ContentAlignment.MiddleLeft;
//			UnlockScriptLabel.Location = new Point(LockScriptLargeTextButton.Right + 10, BtsDChannelsGroupBox.Bottom + 2);
//			UnlockScriptLabel.Size = new Size(100, 13);
			UnlockScriptLabel.Name = "UnlockScriptLabel";
			UnlockScriptLabel.Text = "Unlock Script";
			UnlockScriptLabel.TabIndex = 102;
			UnlockScriptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// UnlockScriptTextBox
			// 
			UnlockScriptTextBox.Font = new Font("Courier New", 8.25F);
//			UnlockScriptTextBox.Location = new Point(LockScriptTextBox.Right + 10, UnlockScriptLabel.Bottom + 2);
//			UnlockScriptTextBox.Size = new Size(250, 254);
			UnlockScriptTextBox.Name = "UnlockScriptTextBox";
			UnlockScriptTextBox.ReadOnly = true;
			UnlockScriptTextBox.TabIndex = 97;
			UnlockScriptTextBox.Text = "";
			UnlockScriptTextBox.TextChanged += UnlockScriptTextBoxTextChanged;
			// 
			// UnlockScriptLargeTextButton
			// 
			UnlockScriptLargeTextButton.Enabled = false;
//			UnlockScriptLargeTextButton.Size = new Size(24, 20);
//			UnlockScriptLargeTextButton.Location = new Point(UnlockScriptTextBox.Right - UnlockScriptLargeTextButton.Width, UnlockScriptLabel.Top);
			UnlockScriptLargeTextButton.Name = "UnlockScriptLargeTextButton";
			UnlockScriptLargeTextButton.TabIndex = 98;
			UnlockScriptLargeTextButton.Text = "...";
			UnlockScriptLargeTextButton.UseVisualStyleBackColor = true;
			UnlockScriptLargeTextButton.Click += LargeTextButtonsClick;
			
			BtsDChannelsGroupBox.ResumeLayout(false);
			BtsDChannelsGroupBox.PerformLayout();
			EquipmentGroupBox.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
			
			DynamicControlsSizesLocations();
		}
		
		void DynamicControlsSizesLocations() {
			EquipmentGroupBox.Location = new Point(PaddingLeftRight, MainMenu.Bottom + 4);
			EquipmentGroupBox.Size = new Size(133, 50);
			
			BcfPcmLabel.Location = new Point(EquipmentGroupBox.Right + 2, MainMenu.Bottom + 4);
			BcfPcmLabel.Size = new Size(93, 20);
			
			BcfPcmTextBox.Location = new Point(BcfPcmLabel.Right + 2, MainMenu.Bottom + 4);
			BcfPcmTextBox.Size = new Size(100, 20);
			
			BtsDChannelsCommandLabel.Location = new Point(EquipmentGroupBox.Right + 2, BcfPcmLabel.Bottom + 10);
			BtsDChannelsCommandLabel.Size = new Size(93, 20);
			
			BtsDChannelsCommandTextBox.Location = new Point(BtsDChannelsCommandLabel.Right + 2, BcfPcmTextBox.Bottom + 10);
			BtsDChannelsCommandTextBox.Size = new Size(280, 20);
			
			BtsDChannelsGroupBox.Location = new Point(PaddingLeftRight, EquipmentGroupBox.Bottom + 2);
			BtsDChannelsGroupBox.Size = new Size(510, 245);
			
			LockScriptLabel.Location = new Point(PaddingLeftRight, BtsDChannelsGroupBox.Bottom + 2);
			LockScriptLabel.Size = new Size(150, 20);
			
			LockScriptTextBox.Location = new Point(PaddingLeftRight, LockScriptLabel.Bottom + 2);
			LockScriptTextBox.Size = new Size(250, 267);
			
			LockScriptLargeTextButton.Size = new Size(24, 20);
			LockScriptLargeTextButton.Location = new Point(LockScriptTextBox.Right - LockScriptLargeTextButton.Width, LockScriptLabel.Top);
			
			UnlockScriptLabel.Location = new Point(LockScriptLargeTextButton.Right + 10, BtsDChannelsGroupBox.Bottom + 2);
			UnlockScriptLabel.Size = new Size(150, 20);
			
			UnlockScriptTextBox.Location = new Point(LockScriptTextBox.Right + 10, UnlockScriptLabel.Bottom + 2);
			UnlockScriptTextBox.Size = new Size(250, 267);
			
			UnlockScriptLargeTextButton.Size = new Size(24, 20);
			UnlockScriptLargeTextButton.Location = new Point(UnlockScriptTextBox.Right - UnlockScriptLargeTextButton.Width, UnlockScriptLabel.Top);
			
			Size = new Size(UnlockScriptTextBox.Right + PaddingLeftRight, UnlockScriptTextBox.Bottom + PaddingTopBottom);
		}
	}
}
/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 13-11-2014
 * Time: 17:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using appCore.DB;
using appCore.Logs;
using appCore.Settings;
using appCore.SiteFinder.UI;
using appCore.Templates;
using appCore.Templates.UI;
using appCore.Toolbox;
using appCore.UI;
using appCore.Shifts;
using appCore.OssScripts.UI;

namespace appCore
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	
	public partial class MainForm : Form
	{
		public static string LogFilePath = string.Empty;
		public static string[] sites; // Para o parser guardar os sites para o bulkci
		public static string VFoutage;
		public static string TFoutage;
		public static string VFbulkCI;
		public static string TFbulkCI;
		
		public static TrayIcon trayIcon;
		public static TroubleshootControls TroubleshootUI = new TroubleshootControls();
		public static FailedCRQControls FailedCRQUI = new FailedCRQControls();
		public static UpdateControls UpdateUI = new UpdateControls();
		public static TXControls TXUI = new TXControls();
		public static siteDetails2 SiteDetailsUI;
		public static PictureBox SiteDetailsPictureBox = new PictureBox();
		public static OutageControls OutageUI = new OutageControls();
		public static LogsCollection<Template> logFiles = new LogsCollection<Template>();
		public static ShiftsCalendar shiftsCalendar;
		static Label TicketCountLabel = new Label();
		
		DataView eScriptCellsGlobal = new DataView();
		
		EricssonScriptsControls ericssonScriptsControls = new EricssonScriptsControls();
		NokiaScriptsControls nokiaScriptsControls = new NokiaScriptsControls();
		HuaweiScriptsControls huaweiScriptsControls = new HuaweiScriptsControls();
		
		bool CheckLogFileExists(string logtype)
		{
			GlobalProperties.dt = DateTime.Now;
			LogFilePath = UserFolder.LogsFolder.FullName + "\\" + GlobalProperties.dt.ToString("MMM",GlobalProperties.culture) + "-" + GlobalProperties.dt.ToString("yyyy",GlobalProperties.culture);
			if(!Directory.Exists(LogFilePath)) {
				Directory.CreateDirectory(LogFilePath);
				if (logtype == "outage") {
					Directory.CreateDirectory(LogFilePath + "\\outages");
				}
				return false;
			}
			
			if (logtype == "template") {
				LogFilePath = LogFilePath + "\\" + GlobalProperties.dt.ToString("dd",GlobalProperties.culture) + ".txt";
				
				if (!File.Exists(LogFilePath)){
					return false;
				}
			}
			else {
				if(!Directory.Exists(LogFilePath + "\\outages")) {
					Directory.CreateDirectory(LogFilePath + "\\outages");
					return false;
				}
				if (!File.Exists(LogFilePath + "\\outages\\" + GlobalProperties.dt.ToString("dd",GlobalProperties.culture) + ".txt")) {
					return false;
				}
			}
			
			return true;
		}
		
		int[] CheckLogExists(string src, string LogType)
		{
			List<int> foundIndexes = new List<int>();
			
			if (!CheckLogFileExists("template"))
				return foundIndexes.ToArray();
			
			string[] Logs = appCore.Logs.UI.LogEditor.ParseLogs(LogFilePath);
			string searchPrefix;
			
			for(int c = 0;c < Logs.Length;c++) {
				if(LogType == "TX Template")
					searchPrefix = "Site(s) ref: ";
				else
					searchPrefix = src.Substring(0,3) + ": ";
				
				if (Logs[c].Contains(LogType) && Logs[c].Contains(searchPrefix + src))
					foundIndexes.Add(c);
			}
			return foundIndexes.ToArray();
		}
		
		void LogOutageReport()
		{
			string separator = string.Empty;
			for (int i = 1; i < 301; i++) {
				if (i == 151) separator += Environment.NewLine;
				separator += "*";
			}
			
			string ToLog = string.Empty;
			if(!string.IsNullOrEmpty(VFoutage))
				ToLog = "----------VF Report----------" + Environment.NewLine + VFoutage + Environment.NewLine + "-----BulkCI-----" + Environment.NewLine + VFbulkCI + Environment.NewLine;
			if(!string.IsNullOrEmpty(TFoutage))
				ToLog += "----------TF Report----------" + Environment.NewLine + TFoutage + Environment.NewLine + "-----BulkCI-----" + Environment.NewLine + TFbulkCI + Environment.NewLine;
			ToLog += separator;
			
		Retry:
			
			try
			{
				if (CheckLogFileExists("outage")) File.AppendAllText(LogFilePath + "\\outages\\" + GlobalProperties.dt.ToString("dd",GlobalProperties.culture) + ".txt",Environment.NewLine + GlobalProperties.dt.ToString("HH:mm:ss") + Environment.NewLine + ToLog);
				else File.WriteAllText(LogFilePath + "\\outages\\" + GlobalProperties.dt.ToString("dd",GlobalProperties.culture) + ".txt",GlobalProperties.dt.ToString("HH:mm:ss")  + Environment.NewLine + ToLog);
			}
			catch (IOException)
			{
				Action action = new Action(delegate {
				                           	FlexibleMessageBox.Show("Log file is currently in use, please close it and press OK to retry","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
				                           });
				Tools.darkenBackgroundForm(action,false,this);
				goto Retry;
			}
		}
		
		public void FillTemplateFromLog(Template log)
		{
			switch(log.LogType) {
				case "Troubleshoot":
					tabControl1.SelectTab(1);
					tabControl2.SelectTab(0);
					if(TroubleshootUI != null)
						TroubleshootUI.Dispose();
					TroubleshootUI = new TroubleshootControls(log.ToTroubleShootTemplate(), Template.UIenum.Template);
					break;
				case "Failed CRQ":
					tabControl1.SelectTab(1);
					tabControl2.SelectTab(1);
					if(FailedCRQUI != null)
						FailedCRQUI.Dispose();
					FailedCRQUI = new FailedCRQControls(log.ToFailedCRQTemplate(), Template.UIenum.Template);
					break;
				case "Update":
					tabControl1.SelectTab(1);
					tabControl2.SelectTab(2);
					if(UpdateUI != null)
						UpdateUI.Dispose();
					UpdateUI = new UpdateControls(log.ToUpdateTemplate(), Template.UIenum.Template);
					break;
				case "TX":
					tabControl1.SelectTab(1);
					tabControl2.SelectTab(3);
					if(TXUI != null)
						TXUI.Dispose();
					TXUI = new TXControls(log.ToTXTemplate(), Template.UIenum.Template);
					break;
			}
		}
		
		public void FillTemplateFromLog(string[] log,string LogType)
		{
//			string[] strTofind = { string.Empty };
//			string complete = string.Empty;
//			int c = 0;
			Action action = new Action(delegate {
			                           	string[] strTofind = { string.Empty };
			                           	string complete = string.Empty;
//			                           	int c = 0;
			                           	tabControl1.SelectTab(1);
			                           	switch(LogType) {
			                           		case "Troubleshoot Template":
//			                           			tabControl2.SelectTab(0);
//			                           			textBox1.Text = log[1].Substring("INC: ".Length);
//			                           			textBox2.Text = log[2].Substring("Site ID: ".Length);
//			                           			if(siteFinder_mainswitch)
//			                           				siteFinder(textBox2,new KeyPressEventArgs((char)Keys.Enter));
//			                           			comboBox1.Text = log[3].Substring("Site Owner: ".Length,2);
//			                           			if(comboBox1.Text == "TF") {
//			                           				string[] tempSplitStr = log[3].Split('(');
//			                           				string tempStr = string.Empty;
//			                           				foreach (char ch in tempSplitStr[tempSplitStr.GetUpperBound(0)]) {
//			                           					if(ch != ')')
//			                           						tempStr += ch;
//			                           				}
//			                           				textBox3.Text = tempStr;
//			                           			}
//			                           			else
//			                           				textBox3.Text = string.Empty;
//			                           			textBox4.Text = log[4].Substring("Site Address: ".Length);
//			                           			checkBox1.Checked = log[5].Substring("Other sites impacted: ".Length) != "None";
//			                           			if (log[6].Substring("COOS: ".Length,2) == "No")
//			                           				checkBox2.Checked = false;
//			                           			else {
//			                           				checkBox2.Checked = true;
//			                           				string[] COOS = log[6].Substring("COOS: YES ".Length).Split(' ');
//			                           				numericUpDown1.Text = COOS[1];
//			                           				numericUpDown2.Text = COOS[3];
//			                           				numericUpDown3.Text = COOS[5];
//			                           			}
//			                           			c = 7;
//			                           			if(log[c].StartsWith("Full")) {
//			                           				checkBox18.Checked = log[c].Substring("Full Site Outage: ".Length) != "No";
//			                           				c++;
//			                           			}
//			                           			checkBox3.Checked = log[c].Substring("Performance Issue: ".Length) != "No";
//			                           			c++;
//			                           			checkBox4.Checked = log[c].Substring("Intermittent Issue: ".Length) != "No";
//			                           			c++;
//			                           			textBox5.Text = log[c].Substring("CCT Reference: ".Length) == "None" ? string.Empty : log[c].Substring("CCT Reference: ".Length);
//			                           			c++;
//			                           			textBox6.Text = log[c].Substring("Related INC/CRQ: ".Length) == "None" ? string.Empty : log[c].Substring("Related INC/CRQ: ".Length);
//
//			                           			c = Array.IndexOf(log, "Active Alarms:");
//			                           			for (c++; c < log.Length - 4; c++) {
//			                           				if (log[c] == "") {
//			                           					if (log[c+1] != "Alarm History:")
//			                           						complete += Environment.NewLine;
//			                           					else {
//			                           						c++;
//			                           						break;
//			                           					}
//			                           				}
//			                           				else {
//			                           					if (string.IsNullOrEmpty(complete))
//			                           						complete = log[c];
//			                           					else
//			                           						complete += Environment.NewLine + log[c];
//			                           				}
//			                           			}
//			                           			textBox7.Text = complete;
//
//			                           			complete = string.Empty;
//			                           			for (c++; c < log.Length - 4; c++) {
//			                           				if (log[c] == "None related") {
//			                           					complete = string.Empty;
//			                           					c += 2;
//			                           					break;
//			                           				}
//			                           				if (log[c] == "") {
//			                           					if (log[c+1] != "Troubleshoot:")
//			                           						complete += Environment.NewLine;
//			                           					else {
//			                           						c++;
//			                           						break;
//			                           					}
//			                           				}
//			                           				else {
//			                           					if (string.IsNullOrEmpty(complete))
//			                           						complete = log[c];
//			                           					else
//			                           						complete += Environment.NewLine + log[c];
//			                           				}
//			                           			}
//			                           			textBox8.Text = complete;
//
//			                           			complete = string.Empty;
//			                           			for (c++; c < log.Length - 4; c++) {
//			                           				if (string.IsNullOrEmpty(complete))
//			                           					complete = log[c];
//			                           				else
//			                           					complete += Environment.NewLine + log[c];
//			                           			}
//			                           			textBox9.Text = complete;
//			                           			break;
//			                           		case "Failed CRQ":
//			                           			tabControl2.SelectTab(1);
//			                           			c = 1;
//			                           			textBox16.Text = log[c].Substring("INC raised: ".Length);
//			                           			c++;
//			                           			if (log[c].Contains("Site: ")) {
//			                           				textBox27.Text = log[2].Substring("Site: ".Length);
//			                           				c++;
//			                           			}
//			                           			else
//			                           				textBox27.Text = string.Empty;
//			                           			if(siteFinder_mainswitch)
//			                           				siteFinder(textBox27,new KeyPressEventArgs((char)Keys.Enter));
//			                           			textBox17.Text = log[c].Substring("CRQ: ".Length);
//			                           			c++;
//			                           			if(log[c].Contains("CRQ contacts:")) {
//			                           				complete = string.Empty;
//			                           				complete = log[++c];
//			                           				for (c++; c < log.Length; c++) {
//			                           					if (!log[c].Contains("FE booked in:"))
//			                           						complete += Environment.NewLine + log[c];
//			                           					else break;
//			                           				}
//			                           				richTextBox16.Text = complete;
//			                           			}
//			                           			else {
//			                           				richTextBox16.Text = log[c].Substring("CRQ contact: ".Length);
//			                           				c++;
//			                           			}
//			                           			string[] temp = log[c].Substring("FE booked in: ".Length).Split(strTofind, StringSplitOptions.None);
//			                           			textBox18.Text = temp[0];
//			                           			textBox22.Text = temp[1];
//			                           			c++;
//			                           			checkBox5.Checked = log[c].Substring("Did FE call the ANOC after CRQ: ".Length) != "No";
//			                           			c++;
//
//			                           			complete = string.Empty;
//			                           			if(log[c].Length > "Work performed by FE on site:".Length) {
//			                           				if (log[c].Substring("Work performed by FE on site:".Length) == " N/A")
//			                           					richTextBox1.Text = string.Empty;
//			                           				else
//			                           					complete = log[c].Substring("Work performed by FE on site: ".Length);
//			                           				c++;
//			                           			}
//			                           			else {
//			                           				complete = log[++c];
//			                           				for (c++; c < log.Length; c++) {
//			                           					if (!log[c].Contains("Troubleshooting done with FE on site to recover affected cells:"))
//			                           						complete += Environment.NewLine + log[c];
//			                           					else break;
//			                           				}
//			                           				richTextBox1.Text = complete;
//			                           			}
//
//			                           			complete = string.Empty;
//			                           			if(log[c].Length > "Troubleshooting done with FE on site to recover affected cells:".Length) {
//			                           				if (log[c].Substring("Troubleshooting done with FE on site to recover affected cells:".Length) == " N/A")
//			                           					richTextBox2.Text = string.Empty;
//			                           				else
//			                           					complete = log[c].Substring("Troubleshooting done with FE on site to recover affected cells:".Length);
//			                           				c++;
//			                           			}
//			                           			else {
//			                           				complete = log[++c];
//			                           				for (c++; c < log.Length; c++) {
//			                           					if (!log[c].Contains("Contractor to fix the fault: "))
//			                           						complete += Environment.NewLine + log[c];
//			                           					else break;
//			                           				}
//			                           				richTextBox2.Text = complete;
//			                           			}
//
//			                           			if (log[c].Substring("Contractor to fix the fault: ".Length) == "None provided") {
//			                           				textBox24.Text = string.Empty;
//			                           				textBox23.Text = string.Empty;
//			                           				checkBox6.Checked = false;
//			                           				c++;
//			                           			}
//			                           			else {
//			                           				string[] temp2 = log[c].Substring("Contractor to fix the fault: ".Length).Split(strTofind, StringSplitOptions.None);
//			                           				textBox24.Text = temp2[0];
//			                           				textBox23.Text = temp2[1];
//			                           				c++;
//			                           				if (log[c].Substring("Time to fix the fault: ".Length) == "None provided")
//			                           					checkBox6.Checked = false;
//			                           				else {
//			                           					checkBox6.Checked = true;
//			                           					string[] temp3 = log[c].Substring("Time to fix the fault: ".Length).Split(':');
//			                           					dateTimePicker1.Value = new DateTime(2014, 11, 19, Convert.ToInt32(temp3[0]), Convert.ToInt32(temp3[1]), Convert.ToInt32(temp3[2]), 0);
//			                           				}
//			                           			}
//			                           			c++;
//
//			                           			complete = string.Empty;
//			                           			if(log[c].Length > "Observations:".Length) {
//			                           				if (log[c].Substring("Observations:".Length) == " N/A")
//			                           					richTextBox3.Text = string.Empty;
//			                           			}
//			                           			else {
//			                           				if (c < log.Length) {
//			                           					complete = log[++c];
//			                           					for (c++; c < log.Length; c++)
//			                           						complete += Environment.NewLine + log[c];
//			                           				}
//			                           				richTextBox3.Text = complete;
//			                           			}
//			                           			break;
			                           		case "TX Template":
//			                           			tabControl2.SelectTab(3);
//			                           			complete = log[1].Substring(13,log[1].Length - 13);
//			                           			c = 1;
//			                           			for (c++; c < log.Length; c++) {
//			                           				if (!log[c].Contains("Service affected: ")) complete = complete + Environment.NewLine + log[c];
//			                           				else break;
//			                           			}
//			                           			textBox25.Text = complete;
//			                           			comboBox2.Text = log[c].Substring(18,log[c].Length - 18);
//			                           			c++;
//
//			                           			complete = log[c].Substring(35,log[c].Length - 35);
//			                           			for (c++; c < log.Length; c++) {
//			                           				if (!log[c].Contains("Repeat/Intermittent: ")) complete = complete + Environment.NewLine + log[c];
//			                           				else break;
//			                           			}
//			                           			richTextBox4.Text = complete;
//
//			                           			if (log[c].Substring(21, 2) == "No") checkBox7.Checked = false;
//			                           			else checkBox7.Checked = true;
//			                           			c++;
//
//			                           			comboBox3.Text = log[c].Substring(22, log[c].Length - 22);
//			                           			c++;
//			                           			if (comboBox3.Text == "IPRAN") textBox26.Text = log[c].Substring(26, log[c].Length - 26);
//			                           			else textBox26.Text = string.Empty;
//			                           			c++;
//
//			                           			complete = log[c].Substring(88,log[c].Length - 88);
//			                           			for (c++; c < log.Length - 4; c++) {
//			                           				complete = complete + Environment.NewLine + log[c];
//			                           			}
//			                           			richTextBox5.Text = complete;
			                           			break;
			                           		case "Update Template":
//			                           			tabControl2.SelectTab(2);
//			                           			textBox44.Text = log[1].Substring(5,15);
//			                           			textBox43.Text = log[2].Substring(6,log[2].Length - 6);
//			                           			if(siteFinder_mainswitch)
//			                           				siteFinder(textBox43,new KeyPressEventArgs((char)Keys.Enter));
//			                           			richTextBox13.Text = log[4];
//			                           			if (log[6] == "Next actions:") richTextBox12.Text = log[7];
			                           			break;
			                           	}
			                           });
			if (this.InvokeRequired)
				this.BeginInvoke(new MethodInvoker(() => action()));
			else
				action();
			
		}
		
		public static void UpdateTicketCountLabel(bool ignoreLabelVisibility = false) {
			if(!string.IsNullOrEmpty(TicketCountLabel.Text) || ignoreLabelVisibility)
				TicketCountLabel.Text = logFiles.FilterCounts(Template.Filters.TicketCount).ToString();
		}

		void TicketCountLabelMouseClick(object sender, MouseEventArgs e) {
			switch(e.Button) {
				case MouseButtons.Left:
					if(logFiles.LogFile.Exists) {
						if(string.IsNullOrEmpty(TicketCountLabel.Text)) {
							logFiles.CheckLogFileIntegrity();
							UpdateTicketCountLabel(true);
						}
						else
							TicketCountLabel.Text = string.Empty;
					}
					else {
						TicketCountLabel.Text = string.IsNullOrEmpty(TicketCountLabel.Text) ? 0.ToString() : string.Empty;
					}
					break;
				case MouseButtons.Right:
					TicketCountLabel.ForeColor = TicketCountLabel.ForeColor == Color.Black ? Color.White : Color.Black;
					break;
			}
			
		}
		
		void toolTipDeploy() {
			// Create the ToolTip and associate with the Form container.
			ToolTip toolTip = new ToolTip();

			// Set up the delays for the ToolTip.
			toolTip.AutoPopDelay = 600000;
			toolTip.InitialDelay = 500;
			toolTip.ReshowDelay = 500;
			
			toolTip.ShowAlways = false; // Force the ToolTip text to be displayed whether or not the form is active.
			toolTip.IsBalloon = true;
			toolTip.UseAnimation = true;
			toolTip.UseFading = false;
			toolTip.BackColor = Color.White;
			toolTip.ForeColor = Color.Firebrick;
			
			// Set up the ToolTip text for each object
			
			toolTip.SetToolTip(pictureBox1, "Settings");
			toolTip.SetToolTip(pictureBox2, "AMT Browser");
			toolTip.SetToolTip(pictureBox3, "Notes");
			toolTip.SetToolTip(pictureBox4, "Log Browser");
			toolTip.SetToolTip(SiteDetailsPictureBox, "Site Finder");
			toolTip.SetToolTip(button25, "To paste on Site Lopedia's bulk search");
		}
		
		public MainForm(NotifyIcon tray)
		{
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			GlobalProperties.resolveOfficePath();
			
			SplashForm.ShowSplashScreen();
			trayIcon = new TrayIcon(tray);
			
			EmbeddedAssemblies.Init();
			
			SplashForm.UpdateLabelText("Getting network access");
			
			// Initialize Properties
			
			GlobalProperties.CheckShareAccess();
			
			SplashForm.UpdateLabelText("Setting User Folder");
			
			CurrentUser.InitializeUserProperties();
			
			SplashForm.UpdateLabelText("Setting User Settings");
			
			logFiles.Initialize();
			
			SplashForm.UpdateLabelText("Loading UI");
			
			InitializeComponent();
			panel1.BackColor = CurrentUser.userName == "GONCARJ3" ? Color.FromArgb(150, Color.LightGray) : Color.Transparent;
			
			string img = SettingsFile.BackgroundImage;
			
			if(img != "Default") {
				if(File.Exists(img))
					tabPage1.BackgroundImage = Image.FromFile(img);
				else
					trayIcon.showBalloon("Image file not found", "Background Image file not found, applying default");
			}
			
			panel1.Controls.Add(SiteDetailsPictureBox);
			// 
			// SiteDetailsPictureBox
			// 
			SiteDetailsPictureBox.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			SiteDetailsPictureBox.BackColor = Color.Transparent;
			SiteDetailsPictureBox.Image = Resources.radio_tower;
			SiteDetailsPictureBox.Location = new Point(6, 49);
			SiteDetailsPictureBox.Name = "pictureBox5";
			SiteDetailsPictureBox.Size = new Size(40, 40);
			SiteDetailsPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
			SiteDetailsPictureBox.TabIndex = 8;
			SiteDetailsPictureBox.TabStop = false;
			SiteDetailsPictureBox.Click += PictureBoxesClick;
			SiteDetailsPictureBox.MouseLeave += PictureBoxesMouseLeave;
			SiteDetailsPictureBox.MouseHover += PictureBoxesMouseHover;
			
			tabPage1.Controls.Add(TicketCountLabel);
			// 
			// TicketCountLabel
			// 
			TicketCountLabel.BackColor = Color.Transparent;
			TicketCountLabel.Size = new Size(40, 20);
			TicketCountLabel.Location = new Point(tabPage1.Width - TicketCountLabel.Width - 5, tabPage1.Height - TicketCountLabel.Height - 5);
			TicketCountLabel.Name = "TicketCountLabel";
			TicketCountLabel.TabIndex = 5;
			TicketCountLabel.TextAlign = ContentAlignment.MiddleRight;
			TicketCountLabel.MouseClick += TicketCountLabelMouseClick;
			
			// UNDONE: Developer specific action
			if(CurrentUser.userName == "GONCARJ3" || CurrentUser.userName == "Caramelos" || CurrentUser.userName == "SANTOSS2") {
				Button butt2 = new Button();
				butt2.Name = "butt2";
				butt2.Location = new Point(5, tabPage1.Height - butt2.Height - 5);
				butt2.Text = "Update OI DB Files";
				butt2.AutoSize = true;
				butt2.Click += delegate {
					Thread t = new Thread(() => { Databases.UpdateSourceDBFiles(); });
					t.Start();
				};
				tabPage1.Controls.Add(butt2);
				if(CurrentUser.userName == "GONCARJ3" || CurrentUser.userName == "Caramelos") {
					Button butt = new Button();
					butt.Name = "butt";
					butt.Location = new Point(5, butt2.Top - butt.Height - 5);
					butt.Click += delegate {
//						if(OutageUI != null)
//							OutageUI.Dispose();
//						tabControl1.SelectTab(6);
//						OutageUI = new OutageControls();
//						tabPage17.Controls.Add(OutageUI);
						
//						Remedy.UI.RemedyWebBrowser wb = new appCore.Remedy.UI.RemedyWebBrowser();
//						wb.Show();
						ShiftsSwapForm ss = new ShiftsSwapForm();
						ss.Show();
					};
					tabPage1.Controls.Add(butt);
					
					OutageUI.Location = new Point(1, 2);
					tabPage17.Controls.Add(OutageUI);
				}
			}
			if(CurrentUser.userName != "GONCARJ3" && CurrentUser.userName != "Caramelos") {
				tabControl1.TabPages.Remove(tabPage17); // new outage reports
				tabControl3.TabPages.Remove(tabPage14); // Alcatel scripts tab
			}
			
			TroubleshootUI.Location = new Point(1, 2);
			tabPage8.Controls.Add(TroubleshootUI);
			FailedCRQUI.Location = new Point(1, 2);
			tabPage10.Controls.Add(FailedCRQUI);
			UpdateUI.Location = new Point(1, 2);
			tabPage6.Controls.Add(UpdateUI);
			TXUI.Location = new Point(1, 2);
			tabPage9.Controls.Add(TXUI);
			
			nokiaScriptsControls.Location = new Point(1, 2);
			tabPage12.Controls.Add(nokiaScriptsControls);
			huaweiScriptsControls.Location = new Point(1, 2);
			tabPage11.Controls.Add(huaweiScriptsControls);
			ericssonScriptsControls.Location = new Point(1, 2);
			tabPage13.Controls.Add(ericssonScriptsControls);
			
			SplashForm.UpdateLabelText("Loading Databases");
			
			Databases.PopulateDatabases();
			
			comboBox1.Items.AddRange(new []{ "CBV", CurrentUser.ClosureCode });
			comboBox1.Text = CurrentUser.ClosureCode;
			
			GlobalProperties.siteFinder_mainswitch = false;
			GlobalProperties.siteFinder_mainswitch = Databases.all_sites.Exists || Databases.all_cells.Exists;
			
			if((CurrentUser.department.Contains("1st Line RAN") || CurrentUser.department.Contains("First Line Operations")) && Databases.shiftsFile.Exists) {
				string[] monthShifts = Databases.shiftsFile.GetAllShiftsInMonth(CurrentUser.fullName[1] + " " + CurrentUser.fullName[0], DateTime.Now.Month);
				
				if(monthShifts.Length > 0) {
					pictureBox6.Visible = true;
					shiftsCalendar = new ShiftsCalendar();
					shiftsCalendar.Location = new Point((tabPage1.Width - shiftsCalendar.Width) / 2, 0 - shiftsCalendar.Height);
					tabPage1.Controls.Add(shiftsCalendar);
				}
			}
			
			// TODO: get sites list from alarms
			
			SplashForm.UpdateLabelText("Almost finished");
			
			trayIcon.toggleShareAccess();
			
			toolTipDeploy();
			
			string thisfn = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\appCore.dll";
			
			SplashForm.CloseForm();
			
			if (SettingsFile.LastRunVersion != GlobalProperties.AssemblyFileVersionInfo.FileVersion) {
				SettingsFile.LastRunVersion = GlobalProperties.AssemblyFileVersionInfo.FileVersion;
				FlexibleMessageBox.Show(Resources.Changelog, "Changelog", MessageBoxButtons.OK);
			}
		}

		public static DataRowView findSite(string site)
		{
			if(!site.IsAllDigits())
				site = "00000";
			while(site.StartsWith("0"))
				site = site.Substring(1);
			
			DataView dv = new DataView(Databases.siteDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'"; // query example = "id = 10"
			DataRowView dr = null;
			if(dv.Count == 1)
				dr = dv[0];
			return dr;
		}

		public static DataView findCells(string site)
		{
			while(site.StartsWith("0"))
				site = site.Substring(1);
			if(!site.IsAllDigits())
				site = "00000";
			
			DataView dv = new DataView(Databases.cellDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'";
			DataTable dt = null;
			if (dv.Count > 0)
			{
				dt = dv.ToTable();
				//clone the source table
				DataTable filtered = dt.Clone();

				//fill the clone with the filtered rows
				foreach (DataRowView drv in dt.DefaultView)
				{
					filtered.Rows.Add(drv.Row.ItemArray);
				}
				dt = filtered;
			}
			
			return new DataView(dt);
		}

		void TextBox13TextChanged(object sender, EventArgs e)
		{
			textBox14.Text = "";
			if (textBox13.Text.Length == 15 & comboBox1.Text.Length == 3)
			{
				label30.Visible = false;
				textBox13.TextChanged -= TextBox13TextChanged;
				comboBox1.TextChanged -= ComboBox1TextChanged;
				textBox13.Text = textBox13.Text.ToUpper();
				comboBox1.Text = comboBox1.Text.ToUpper();
				textBox13.TextChanged += TextBox13TextChanged;
				comboBox1.TextChanged += ComboBox1TextChanged;
				if (textBox13.Text.Substring(3,textBox13.Text.Length - 3).IsAllDigits()) {
					int[] rng = new int[12];
					for (int c = 0; c <= 11; c++) {
						rng[c] = Convert.ToInt32(textBox13.Text.Substring(c + 3,1));
					}
					int SumFw = rng[0] * 2 + rng[1] * 3 + rng[2] * 4 + rng[3] * 5 + rng[4] * 6 + rng[5] * 7 + rng[6] * 8 + rng[7] * 9 + rng[8] * 10 + rng[9] * 11 + rng[10] * 12 + rng[11] * 13;
					int SumBw = rng[11] * 2 + rng[10] * 3 + rng[9] * 4 + rng[8] * 5 + rng[7] * 6 + rng[6] * 7 + rng[5] * 8 + rng[4] * 9 + rng[3] * 10 + rng[2] * 11 + rng[1] * 12 + rng[0] * 13;
					string hx = (SumFw * SumBw).ToString("X");
					if (hx.Length < 5) {
						for (int c = 1; c <= 5 - hx.Length; c++) {
							textBox14.Text += "0";
						}
					}
					textBox14.Text += hx + " " + comboBox1.Text;
				}
				else {
					Action action = new Action(delegate {
					                           	FlexibleMessageBox.Show("INC/CRQ can only contain numbers","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           	textBox13.TextChanged -= TextBox13TextChanged;
					                           	textBox13.Text = "";
					                           	textBox13.TextChanged += TextBox13TextChanged;
					                           });
					Tools.darkenBackgroundForm(action,false,this);
				}
			}
			else {
				label30.Visible = true;
				if (textBox13.Text.Length > 0 && textBox13.Text.Length < 15) label30.Text = "Press ENTER key to complete INC number";
				else {
					if (textBox13.Text.Length != 15) label30.Text = "Insert INC/CRQ number";
					else label30.Visible = false;
				}
			}
		}

		void ComboBox1TextChanged(object sender, EventArgs e)
		{
			try{
				if (comboBox1.Text.Length == 3 & textBox13.Text.Length == 15) {
					label31.Visible = false;
					TextBox13TextChanged(sender, e);
				}
				else {
					textBox14.Text = "";
					if (comboBox1.Text.Length < 3) label31.Visible = true;
					else {
						comboBox1.TextChanged -= ComboBox1TextChanged;
						comboBox1.Text = comboBox1.Text.ToUpper();
						comboBox1.TextChanged += ComboBox1TextChanged;
						label31.Visible = false;
					}
				}
			}
			finally {}
		}

		void Button46Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox10.Text)) {
				Action action = new Action(delegate {
				                           	FlexibleMessageBox.Show("Please insert sites list.\n\nTIP: write 1 site PER LINE", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
				                           });
				Tools.darkenBackgroundForm(action,false,this);
				return;
			}
			
			DataRowView foundSite = null;
			DataView foundCells = null;
			List<string[]> VFsitesArrayList = new List<string[]>();
			List<string[]> TFsitesArrayList = new List<string[]>();
			List<string[]> VFlocationsArrayList = new List<string[]>();
			List<string[]> TFlocationsArrayList = new List<string[]>();
			List<string> VFcells2G = new List<string>();
			List<string> VFcells3G = new List<string>();
			List<string> VFcells4G = new List<string>();
			List<string> TFcells2G = new List<string>();
			List<string> TFcells3G = new List<string>();
			List<string> TFcells4G = new List<string>();
			
			sites = textBox10.Text.Split('\n');
			sites = sites.Where(x => !string.IsNullOrEmpty(x)).ToArray();
			foreach(string site in sites) {
				foundSite = findSite(site);
				foundCells = findCells(site);
				
				if(foundSite == null)
					continue;
				
				List<string> VFcells2Gtemp = new List<string>();
				List<string> VFcells3Gtemp = new List<string>();
				List<string> VFcells4Gtemp = new List<string>();
				List<string> TFcells2Gtemp = new List<string>();
				List<string> TFcells3Gtemp = new List<string>();
				List<string> TFcells4Gtemp = new List<string>();
				
				if(foundCells.Count > 0) {
					bool siteHasVF2G = false;
					bool siteHasVF3G = false;
					bool siteHasVF4G = false;
					bool siteHasTF2G = false;
					bool siteHasTF3G = false;
					bool siteHasTF4G = false;
					foundCells.RowFilter = "BEARER = '2G' AND (CELL_NAME NOT LIKE 'T*' AND CELL_NAME NOT LIKE '*W' AND CELL_NAME NOT LIKE '*X' AND CELL_NAME NOT LIKE '*Y')";
					foreach (DataRowView cell in foundCells) {
						VFcells2Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
						siteHasVF2G = true;
					}
					foundCells.RowFilter = "BEARER = '2G' AND (CELL_NAME LIKE 'T*' OR CELL_NAME LIKE '*W' OR CELL_NAME LIKE '*X' OR CELL_NAME LIKE '*Y')";
					foreach (DataRowView cell in foundCells) {
						TFcells2Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
						siteHasTF2G = true;
					}
					foundCells.RowFilter = "BEARER = '3G' AND CELL_NAME NOT LIKE 'T*'";
					foreach (DataRowView cell in foundCells) {
						VFcells3Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
						siteHasVF3G = true;
					}
					foundCells.RowFilter = "BEARER = '3G' AND CELL_NAME LIKE 'T*'";
					foreach (DataRowView cell in foundCells) {
						TFcells3Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString() + " - " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
						siteHasTF3G = true;
					}
					foundCells.RowFilter = "BEARER = '4G' AND CELL_NAME NOT LIKE 'T*'";
					foreach (DataRowView cell in foundCells) {
						VFcells4Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
						siteHasVF4G = true;
					}
					foundCells.RowFilter = "BEARER = '4G' AND CELL_NAME LIKE 'T*'";
					foreach (DataRowView cell in foundCells) {
						TFcells4Gtemp.Add(cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
						siteHasTF4G = true;
					}
					
					string[] address = foundSite[foundSite.Row.Table.Columns.IndexOf("ADDRESS")].ToString().Split(';');
					if(VFcells2Gtemp.Count > 0 || VFcells3Gtemp.Count > 0 || VFcells4Gtemp.Count > 0) {
						VFsitesArrayList.Add(new string[]{foundSite[foundSite.Row.Table.Columns.IndexOf("SITE")].ToString(),siteHasVF2G.ToString(),siteHasVF3G.ToString(),siteHasVF4G.ToString()});
						VFlocationsArrayList.Add(new string[]{address[address.Length - 2].Trim(' '),siteHasVF2G.ToString(),siteHasVF3G.ToString(),siteHasVF4G.ToString()});
						VFcells2G.AddRange(VFcells2Gtemp);
						VFcells3G.AddRange(VFcells3Gtemp);
						VFcells4G.AddRange(VFcells4Gtemp);
					}
					if(TFcells2Gtemp.Count > 0 || TFcells3Gtemp.Count > 0 || TFcells4Gtemp.Count > 0) {
						TFsitesArrayList.Add(new string[]{foundSite[foundSite.Row.Table.Columns.IndexOf("SITE")].ToString(),siteHasTF2G.ToString(),siteHasTF3G.ToString(),siteHasTF4G.ToString()});
						TFlocationsArrayList.Add(new string[]{address[address.Length - 2].Trim(' '),siteHasTF2G.ToString(),siteHasTF3G.ToString(),siteHasTF4G.ToString()});
						TFcells2G.AddRange(TFcells2Gtemp);
						TFcells3G.AddRange(TFcells3Gtemp);
						TFcells4G.AddRange(TFcells4Gtemp);
					}
				}
			}
			
			for(int c = 0;c < VFsitesArrayList.Count;c++) {
				while (VFsitesArrayList[c][0].Length < 5) {
					VFsitesArrayList[c][0] = '0' + VFsitesArrayList[c][0];
				}
				VFsitesArrayList[c][0] = "RBS" + VFsitesArrayList[c][0];
			}
			for(int c = 0;c < TFsitesArrayList.Count;c++) {
				while (TFsitesArrayList[c][0].Length < 5) {
					TFsitesArrayList[c][0] = '0' + TFsitesArrayList[c][0];
				}
				TFsitesArrayList[c][0] = "RBS" + TFsitesArrayList[c][0];
			}
			
			bool show2G = VFcells2G.Count > 0 || TFcells2G.Count > 0;
			bool show3G = VFcells3G.Count > 0 || TFcells3G.Count > 0;
			bool show4G = VFcells4G.Count > 0 || TFcells4G.Count > 0;
			
			List<string[]> includeList = showIncludeListForm(show2G,show3G,show4G);
			
			// FIXME: empty report on cancel IncludeListForm
			
			// Get sites and locations list for VF and TF
			
			List<string> VFsitesList = new List<string>();
			List<string> TFsitesList = new List<string>();
			List<string> VFlocationsList = new List<string>();
			List<string> TFlocationsList = new List<string>();
			
			foreach (string[] site in VFsitesArrayList) {
				if(site[1] == "True" && includeList[0][0] == "True") {
					VFsitesList.Add(site[0]);
					continue;
				}
				if(site[2] == "True" && includeList[1][0] == "True") {
					VFsitesList.Add(site[0]);
					continue;
				}
				if(site[3] == "True" && includeList[2][0] == "True") {
					VFsitesList.Add(site[0]);
				}
			}
			foreach (string[] location in VFlocationsArrayList) {
				if(location[1] == "True" && includeList[0][0] == "True") {
					VFlocationsList.Add(location[0]);
					continue;
				}
				if(location[2] == "True" && includeList[1][0] == "True") {
					VFlocationsList.Add(location[0]);
					continue;
				}
				if(location[3] == "True" && includeList[2][0] == "True") {
					VFlocationsList.Add(location[0]);
				}
			}
			foreach (string[] site in TFsitesArrayList) {
				if(site[1] == "True" && includeList[0][0] == "True") {
					TFsitesList.Add(site[0]);
					continue;
				}
				if(site[2] == "True" && includeList[1][0] == "True") {
					TFsitesList.Add(site[0]);
					continue;
				}
				if(site[3] == "True" && includeList[2][0] == "True") {
					TFsitesList.Add(site[0]);
				}
			}
			foreach (string[] location in TFlocationsArrayList) {
				if(location[1] == "True" && includeList[0][0] == "True") {
					TFlocationsList.Add(location[0]);
					continue;
				}
				if(location[2] == "True" && includeList[1][0] == "True") {
					TFlocationsList.Add(location[0]);
					continue;
				}
				if(location[3] == "True" && includeList[2][0] == "True") {
					TFlocationsList.Add(location[0]);
				}
			}
			
			VFsitesArrayList = null;
			TFsitesArrayList = null;
			VFlocationsArrayList = null;
			TFlocationsArrayList = null;
			
			VFsitesList = VFsitesList.Distinct().ToList();
			VFsitesList.Sort();
			TFsitesList = TFsitesList.Distinct().ToList();
			TFsitesList.Sort();
			VFlocationsList = VFlocationsList.Distinct().ToList();
			VFlocationsList.Sort();
			TFlocationsList = TFlocationsList.Distinct().ToList();
			TFlocationsList.Sort();
			VFcells2G = VFcells2G.Distinct().ToList();
			VFcells2G.Sort();
			TFcells2G = TFcells2G.Distinct().ToList();
			TFcells2G.Sort();
			VFcells3G = VFcells3G.Distinct().ToList();
			VFcells3G.Sort();
			TFcells3G = TFcells3G.Distinct().ToList();
			TFcells3G.Sort();
			VFcells4G = VFcells4G.Distinct().ToList();
			VFcells4G.Sort();
			TFcells4G = TFcells4G.Distinct().ToList();
			TFcells4G.Sort();
			
			// VF Outage
			
			int totalCells = 0;
			if(includeList[0][0] == "True")
				totalCells += VFcells2G.Count;
			if(includeList[1][0] == "True")
				totalCells += VFcells3G.Count;
			if(includeList[2][0] == "True")
				totalCells += VFcells4G.Count;
			
			VFoutage = totalCells + "x COOS (" + VFsitesList.Count;
			if(VFsitesList.Count == 1)
				VFoutage += " Site)";
			else
				VFoutage += " Sites)";
			VFoutage += Environment.NewLine + Environment.NewLine + "Locations (" + VFlocationsList.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine,VFlocationsList.ToArray()) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine,VFsitesList.ToArray());
			
			if(VFcells2G.Count > 0 && includeList[0][0] == "True") {
				VFoutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + VFcells2G.Count + ")";
				if(includeList[0][0] == "True")
					VFoutage += " Event Time - " + includeList[0][1] + Environment.NewLine + string.Join(Environment.NewLine,VFcells2G.ToArray());
			}
			if(VFcells3G.Count > 0 && includeList[1][0] == "True") {
				VFoutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + VFcells3G.Count + ")";
				if(includeList[1][0] == "True")
					VFoutage += " Event Time - " + includeList[1][1] + Environment.NewLine + string.Join(Environment.NewLine,VFcells3G.ToArray());
			}
			if(VFcells4G.Count > 0 && includeList[2][0] == "True") {
				VFoutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + VFcells4G.Count + ")";
				if(includeList[2][0] == "True")
					VFoutage += " Event Time - " + includeList[2][1] + Environment.NewLine + string.Join(Environment.NewLine,VFcells4G.ToArray());
			}
			
			VFbulkCI = string.Empty;
			foreach (string site in VFsitesList) {
				string tempSite = Convert.ToInt32(site.Remove(0,3)).ToString();
				if(tempSite.Length < 4) {
					do {
						tempSite = '0' + tempSite;
					} while (tempSite.Length < 4);
				}
				VFbulkCI += tempSite + ';';
			}
			
			// TF Outage
			
			totalCells = 0;
			if(includeList[0][0] == "True")
				totalCells += TFcells2G.Count;
			if(includeList[1][0] == "True")
				totalCells += TFcells3G.Count;
			if(includeList[2][0] == "True")
				totalCells += TFcells4G.Count;
			
			TFoutage = totalCells + "x COOS (" + TFsitesList.Count;
			TFoutage += TFsitesList.Count == 1 ? " Site)" : " Sites)";
			TFoutage += Environment.NewLine + Environment.NewLine + "Locations (" + TFlocationsList.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine,TFlocationsList.ToArray()) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine,TFsitesList.ToArray());
			
			if(TFcells2G.Count > 0 && includeList[0][0] == "True") {
				TFoutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + TFcells2G.Count + ")";
				if(includeList[0][0] == "True")
					TFoutage += " Event Time - " + includeList[0][1] + Environment.NewLine + string.Join(Environment.NewLine,TFcells2G.ToArray());
			}
			if(TFcells3G.Count > 0 && includeList[1][0] == "True") {
				TFoutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + TFcells3G.Count + ")";
				if(includeList[1][0] == "True")
					TFoutage += " Event Time - " + includeList[1][1] + Environment.NewLine + string.Join(Environment.NewLine,TFcells3G.ToArray());
			}
			if(TFcells4G.Count > 0 && includeList[2][0] == "True") {
				TFoutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + TFcells4G.Count + ")";
				if(includeList[2][0] == "True")
					TFoutage += " Event Time - " + includeList[2][1] + Environment.NewLine + string.Join(Environment.NewLine,TFcells4G.ToArray());
			}
			TFbulkCI = string.Empty;
			foreach (string site in TFsitesList) {
				string tempSite = Convert.ToInt32(site.Remove(0,3)).ToString();
				if(tempSite.Length < 4) {
					do {
						tempSite = '0' + tempSite;
					} while (tempSite.Length < 4);
				}
				TFbulkCI += tempSite + ';';
			}
			
			if(!string.IsNullOrEmpty(VFoutage) && !string.IsNullOrEmpty(TFoutage)) {
				tabControl4.Visible = true;
				tabControl4.SelectTab(0);
			}
			else {
				if(string.IsNullOrEmpty(VFoutage) && string.IsNullOrEmpty(TFoutage)) {
					trayIcon.showBalloon("Empty report","No cells were found for the given sites");
					textBox10.Text = string.Empty;
					return;
				}
				if(!string.IsNullOrEmpty(VFoutage)) {
					tabControl4.Visible = false;
					tabControl4.SelectTab(0);
				}
				else {
					if(!string.IsNullOrEmpty(TFoutage)) {
						tabControl4.Visible = false;
						tabControl4.SelectTab(1);
					}
				}
			}
			if(!string.IsNullOrEmpty(VFoutage) || !string.IsNullOrEmpty(TFoutage)) {
				TabControl4SelectedIndexChanged(null,null);
				//button4.Enabled = false;
				button4.Text = "Outage Follow Up";
				button4.Width = 100;
				//button46.Enabled = false;
				button46.Visible = false;
				button3.Enabled = true;
				textBox10.ReadOnly = true;
				textBox11.ReadOnly = true;
				button12.Visible = true;
				button25.Visible = true;
				textBox10.Focus();
				label33.Text = "Generated Outage Report";
				LogOutageReport();
			}
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
				form.Icon = Resources.MB_0001_vodafone3;
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

		void Button4Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	Stopwatch st = new Stopwatch();
			                           	st.Start();
			                           	if (string.IsNullOrEmpty(textBox10.Text)) {
			                           		FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
			                           		return;
			                           	}
			                           	Button btn = (Button)sender;
			                           	
			                           	if(!btn.Text.Contains("Follow Up")) {
			                           		try {
			                           			DataTable parserTable = new DataTable();
			                           			Netcool.outage_parser op = new Netcool.outage_parser();
			                           			
			                           			parserTable = op.parse(textBox10.Text);
			                           			VFoutage = op.genReport(parserTable,"VF");
			                           			VFbulkCI = op.bulkCi(sites);
			                           			TFoutage = op.genReport(parserTable,"TF");
			                           			TFbulkCI = op.bulkCi(sites);
			                           			
			                           			if(!string.IsNullOrEmpty(VFoutage) && !string.IsNullOrEmpty(TFoutage)) {
			                           				tabControl4.Visible = true;
			                           				tabControl4.SelectTab(0);
			                           			}
			                           			else {
			                           				if(string.IsNullOrEmpty(VFoutage) && string.IsNullOrEmpty(TFoutage)) {
			                           					trayIcon.showBalloon("Empty report","The alarms inserted have no COOS in its content, output is blank");
			                           					textBox10.Text = string.Empty;
			                           					return;
			                           				}
			                           				if(!string.IsNullOrEmpty(VFoutage)) {
			                           					tabControl4.Visible = false;
			                           					tabControl4.SelectTab(0);
			                           				}
			                           				else {
			                           					if(!string.IsNullOrEmpty(TFoutage)) {
			                           						tabControl4.Visible = false;
			                           						tabControl4.SelectTab(1);
			                           					}
			                           				}
			                           			}
			                           			if(!string.IsNullOrEmpty(VFoutage) || !string.IsNullOrEmpty(TFoutage)) {
			                           				TabControl4SelectedIndexChanged(null,null);
//			                           				button4.Enabled = false;
			                           				button4.Text = "Outage Follow Up"; // HACK: Outage Follow Up button on outage report processing
			                           				button4.Width = 100;
			                           				button46.Visible = false;
			                           				button3.Enabled = true;
			                           				textBox10.ReadOnly = true;
			                           				textBox11.ReadOnly = true;
			                           				button12.Visible = true;
			                           				button25.Visible = true;
			                           				textBox10.Focus();
			                           				label33.Text = "Generated Outage Report";
			                           				LogOutageReport();
			                           				if (op.tableRemoved)
			                           					FlexibleMessageBox.Show("WARNING!!!" + Environment.NewLine + Environment.NewLine + "One or more tables to parse have been removed." + Environment.NewLine + "All tables must have the same columns and order.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			                           			}
			                           		}
			                           		catch {
			                           			trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
			                           			return;
			                           		}
			                           	}
			                           	else {
			                           		string[] outageSites = (VFbulkCI + TFbulkCI).Split(';');
			                           		for(int c = 0;c < outageSites.Length;c++) {
			                           			if(outageSites[c].StartsWith("0")) {
			                           				while(outageSites[c].StartsWith("0"))
			                           					outageSites[c] = outageSites[c].Substring(1);
			                           			}
			                           			else
			                           				break;
			                           		}
			                           		outageSites = outageSites.Distinct().ToArray(); // Remover duplicados
			                           		outageSites = outageSites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
			                           		Thread thread = new Thread(() => {
			                           		                           	siteDetails sd = new siteDetails(true,outageSites);
			                           		                           	sd.Name = "Outage Follow-up";
			                           		                           	sd.StartPosition = FormStartPosition.CenterParent;
			                           		                           	sd.ShowDialog();
			                           		                           });
			                           		
			                           		thread.SetApartmentState(ApartmentState.STA);
			                           		thread.Start();
			                           	}
			                           	st.Stop();
			                           	var t = st.Elapsed;
			                           });
			Tools.darkenBackgroundForm(action,true,this);
		}

		void TabControl4SelectedIndexChanged(object sender, EventArgs e)
		{
			if(tabControl4.SelectedIndex == 0) {
				textBox10.Text = VFoutage;
				textBox11.Text = VFbulkCI;
			}
			else {
				textBox10.Text = TFoutage;
				textBox11.Text = TFbulkCI;
			}
			textBox10.Select(0,0);
			textBox11.Select(0,0);
		}

		void Button3Click(object sender, EventArgs e)
		{
			VFoutage = string.Empty;
			VFbulkCI = string.Empty;
			TFoutage = string.Empty;
			TFbulkCI = string.Empty;
			label33.Text = "Copy Outage alarms from Netcool";
//			button4.Enabled = true; // HACK: Outage Follow up button on clear
			button4.Text = "Generate Report";
			button4.Width = 97;
			button46.Visible = true;
			button3.Enabled = false;
			textBox10.ReadOnly = false;
			textBox10.Text = string.Empty;
			textBox11.Text = string.Empty;
			button12.Visible = false;
			button25.Visible = false;
			tabControl4.Visible = false;
			textBox10.Focus();
		}

		void Button25Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	string[] temp = textBox10.Text.Split('\n');
			                           	int c;
			                           	for(c = 0;c < temp.Length;c++) {
			                           		if(temp[c] != "Site List") {
			                           			temp = temp.Where((source, index) => index != c).ToArray();
			                           			c--;
			                           		}
			                           		else {
			                           			temp = temp.Where((source, index) => index != c).ToArray();
			                           			break;
			                           		}
			                           	}
			                           	
			                           	for(c = 0;c < temp.Length;c++) {
			                           		if(string.IsNullOrEmpty(temp[c]))
			                           			break;
			                           	}
			                           	
			                           	for(int i = 0;i < temp.Length;i++) {
			                           		if(temp[i].Length > 8) {
			                           			temp[i] = temp[i].Substring(0,8);
			                           			i--;
			                           		}
			                           	}
			                           	
			                           	List<string> temp2 = temp.ToList();
			                           	temp2.RemoveRange(c,temp.Length - c);
			                           	temp = temp2.ToArray();
			                           	
			                           	for(c = 0;c < temp.Length;c++) {
			                           		temp[c] = Convert.ToInt32(temp[c].Replace("RBS",string.Empty)).ToString();
			                           	}
			                           	
			                           	FlexibleMessageBox.Show("The following site list was copied to the Clipboard:" + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine,temp) + Environment.NewLine + Environment.NewLine + "This list can be used to enter a bulk site search on Site Lopedia.","List generated",MessageBoxButtons.OK,MessageBoxIcon.Information);
			                           	Clipboard.SetText(string.Join(Environment.NewLine,temp));
			                           });
			Tools.darkenBackgroundForm(action,false,this);
		}


		void Button6Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	if (string.IsNullOrEmpty(textBox12.Text)) {
			                           		FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
			                           		return;
			                           	}
			                           	try {
//			                           		Netcool.AlarmsParser2 netcool2 = new Netcool.AlarmsParser2(textBox12.Text);
//			                           		textBox12.Text = netcool2.ToString();
			                           		
			                           		Netcool.AlarmsParser netcool = new Netcool.AlarmsParser(textBox12.Text);
			                           		textBox12.Text = netcool.ToString();
			                           		
			                           		textBox12.Select(0,0);
			                           		button6.Enabled = false;
			                           		button5.Enabled = true;
			                           		button13.Visible = true;
			                           		textBox12.ReadOnly = true;
			                           		label34.Text = "Parsed alarms";
			                           	}
			                           	catch {
			                           		trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
			                           		return;
			                           	}
			                           });
			Tools.darkenBackgroundForm(action,true,this);
		}

		void Button5Click(object sender, EventArgs e)
		{
			label34.Text = "Alarms to parse";
			button6.Enabled = true;
			button5.Enabled = false;
			textBox12.ReadOnly = false;
			textBox12.Text = "";
			button13.Visible = false;
			textBox12.Focus();
		}

		void TabControl1SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl1.SelectedIndex)
			{
				case 1:
					TabControl2SelectedIndexChanged(null, null);
					break;
				case 2:
					textBox13.Focus();
					break;
				case 3:
					textBox10.Focus();
					break;
				case 4:
					textBox12.Focus();
					break;
			}
		}

		void TabControl2SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl2.SelectedIndex)
			{
				case 0:
					TroubleshootUI.SiteIdTextBox.Focus();
					break;
				case 1:
					FailedCRQUI.SiteIdTextBox.Focus();
					break;
				case 2:
					UpdateUI.SiteIdTextBox.Focus();
					break;
				case 3:
					TXUI.SitesTextBox.Focus();
					break;
			}
		}

		void TextBox13KeyPress(object sender, KeyPressEventArgs e)
		{
			
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				if (textBox13.Text.Length > 0) {
					string CompINC_CRQ = Tools.CompleteINC_CRQ_TAS(textBox13.Text, "INC");
					if (CompINC_CRQ != "error") textBox13.Text = CompINC_CRQ;
					else {
						Action action = new Action(delegate {
						                           	FlexibleMessageBox.Show("INC number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
						                           });
						Tools.darkenBackgroundForm(action,false,this);
						return;
					}
				}
			}
		}

		void TabPage1MouseClick(object sender, MouseEventArgs e)
		{
			//FIXME:			wholeShiftsPanelDispose();
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				contextMenuStrip1.Show(PointToScreen(e.Location));
		}

		void ToolStripMenuItem2Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	OpenFileDialog fileBrowser = new OpenFileDialog();
			                           	fileBrowser.Filter = "All image files(*.bmp,*.gif,*.jpg,*.jpeg,*.png)|*.bmp;*.gif;*.jpg;*.jpeg;*.png";
			                           	fileBrowser.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			                           	fileBrowser.Title = "Please select a new background image";
			                           	if (fileBrowser.ShowDialog() == DialogResult.OK)
			                           	{
			                           		SettingsFile.BackgroundImage = fileBrowser.FileName;
			                           		tabPage1.BackgroundImage = Image.FromFile(fileBrowser.FileName);
			                           	}
			                           });
			Tools.darkenBackgroundForm(action,false,this);
		}

		void ToolStripMenuItem3Click(object sender, EventArgs e)
		{
			SettingsFile.BackgroundImage = "Default";
			tabPage1.BackgroundImage = Resources.zoozoo_wallpaper_15;
		}

		void Button12Click(object sender, EventArgs e)
		{
			try {
				Clipboard.SetText(textBox10.Text);
			}
			catch (Exception) {
				try {
					Clipboard.SetText(textBox10.Text);
				}
				catch (Exception) {
					Action action = new Action(delegate {
					                           	FlexibleMessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Tools.darkenBackgroundForm(action,false,this);
				}
			}
		}

		void Button13Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	TroubleshootUI.ActiveAlarmsTextBox.Text = textBox12.Text;
			                           	tabControl1.SelectTab(1);
			                           	tabControl2.SelectTab(0);
			                           });
			Tools.darkenBackgroundForm(action,false,this);
		}

		void TextBox10TextChanged(object sender, EventArgs e)
		{
			if (textBox10.Text != string.Empty) button22.Enabled = true;
			else button22.Enabled = false;
		}

		void Button22Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox10.Text,label33.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox10.Text = enlarge.finaltext;
			                           });
			Tools.darkenBackgroundForm(action,false,this);
		}

		void TextBox11TextChanged(object sender, EventArgs e)
		{
			if (textBox11.Text != string.Empty) button23.Enabled = true;
			else button23.Enabled = false;
		}

		void Button23Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox11.Text,label32.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox11.Text = enlarge.finaltext;
			                           });
			Tools.darkenBackgroundForm(action,false,this);
		}

		void TextBox12TextChanged(object sender, EventArgs e)
		{
			if (textBox12.Text != string.Empty) button24.Enabled = true;
			else button24.Enabled = false;
		}

		void Button24Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox12.Text,label34.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox12.Text = enlarge.finaltext;
			                           });
			Tools.darkenBackgroundForm(action,false,this);
		}

		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			DialogResult ans = DialogResult.No;
			Action action = new Action(delegate {
			                           	ans = FlexibleMessageBox.Show("Are you sure you want to quit ANOC Master Tool?","Quitting",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
			                           });
			Tools.darkenBackgroundForm(action,false,this);
			if(ans == DialogResult.Yes) {
				UserFolder.ClearTempFolder();
				FormCollection fc = Application.OpenForms;
				foreach (Form frm in fc)
				{
					if(frm.Name == "BrowserView") {
						frm.Invoke(new MethodInvoker(frm.Close));
						break;
					}
				}
			}
			else
				e.Cancel = true;
		}

		void MainFormActivate(object sender, EventArgs e)
		{
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "MainForm") {
					frm.Activate();
					frm.WindowState = FormWindowState.Normal;
					break;
				}
			}
		}
		
		public static void openSettings() {
//			Action action = new Action(delegate {
			Settings.UI.SettingsForm settings = new Settings.UI.SettingsForm(GlobalProperties.siteFinder_mainswitch);
			settings.StartPosition = FormStartPosition.CenterParent;
			settings.ShowDialog();
			
			if(settings.siteFinder_newSwitch != GlobalProperties.siteFinder_mainswitch)
				GlobalProperties.siteFinder_mainswitch = settings.siteFinder_newSwitch;
			
//			                           	SetUserFolder(false);
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		public static void openAMTBrowser() {
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "BrowserView") {
					if(frm.WindowState == FormWindowState.Minimized) frm.Invoke(new Action(() => { frm.WindowState = FormWindowState.Normal; }));;
					frm.Invoke(new MethodInvoker(frm.Activate));
					return;
				}
			}
			
			Thread thread = new Thread(() => {
			                           	Web.UI.BrowserView brwsr = new Web.UI.BrowserView();
			                           	brwsr.StartPosition = FormStartPosition.CenterParent;
			                           	brwsr.ShowDialog();
			                           });
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		
		public static void openNotes() {
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "NotesForm") {
					frm.Activate();
					return;
				}
			}
			
			NotesForm notes = new NotesForm();
			notes.StartPosition = FormStartPosition.CenterParent;
			notes.Show();
		}
		
		public static void openLogBrowser() {
			FormCollection fc = Application.OpenForms;
			
			MainForm _this = null;
			
			foreach (Form frm in fc)
			{
				if(frm.Name == "MainForm")
					_this = (MainForm)frm;
				if (frm.Name == "LogBrowser") {
					if(frm.WindowState == FormWindowState.Minimized) frm.Invoke(new Action(() => { frm.WindowState = FormWindowState.Normal; }));
					frm.Invoke(new MethodInvoker(frm.Activate));
				}
			}
			
			Thread thread = new Thread(() => {
			                           	Logs.UI.LogBrowser LogView = new Logs.UI.LogBrowser(_this);
			                           	LogView.StartPosition = FormStartPosition.CenterParent;
			                           	LogView.ShowDialog();
			                           });
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		
		public static void openSiteFinder() {
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "siteDetails") {
					if(frm.WindowState == FormWindowState.Minimized) frm.Invoke(new Action(() => { frm.WindowState = FormWindowState.Normal; }));;
					frm.Invoke(new MethodInvoker(frm.Activate));
					return;
				}
			}
			
			Thread thread = new Thread(() => {
			                           	siteDetails2 sd = new siteDetails2();
			                           	sd.StartPosition = FormStartPosition.CenterParent;
			                           	sd.ShowDialog();
			                           });
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		
		void PictureBoxesClick(object sender, EventArgs e) {
			PictureBox pic = (PictureBox)sender;
			switch(pic.Name) {
				case "pictureBox1":
//					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					Action action = new Action(delegate {
					                           	openSettings();
					                           });
					Tools.darkenBackgroundForm(action,false,this);
					break;
				case "pictureBox2":
//					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openAMTBrowser();
					break;
				case "pictureBox3":
//					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openNotes();
					break;
				case "pictureBox4":
//					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openLogBrowser();
					break;
				case "pictureBox5":
//					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openSiteFinder();
					break;
				case "pictureBox6":
					shiftsCalendar.toggleShiftsPanel();
					break;
			}
		}
		
		void PictureBoxesMouseLeave(object sender, EventArgs e) {
			PictureBox pic = (PictureBox)sender;
			switch(pic.Name) {
				case "pictureBox1":
					pic.Image = Resources.Settings_normal;
					break;
				case "pictureBox2":
					pic.Image = Resources.globe;
					break;
				case "pictureBox3":
					pic.Image = Resources.Book_512;
					break;
				case "pictureBox4":
					break;
				case "pictureBox5":
					pic.Image = Resources.radio_tower;
					break;
				case "pictureBox6":
					break;
			}
		}
		
		void PictureBoxesMouseHover(object sender, EventArgs e) {
			PictureBox pic = (PictureBox)sender;
			switch(pic.Name) {
				case "pictureBox1":
					pic.Image = Resources.Settings_hover;
					break;
				case "pictureBox2":
					pic.Image = Resources.globe_hover;
					break;
				case "pictureBox3":
					break;
				case "pictureBox4":
					break;
				case "pictureBox5":
					pic.Image = Resources.radio_tower_hover;
					break;
				case "pictureBox6":
					break;
			}
		}
	}
}
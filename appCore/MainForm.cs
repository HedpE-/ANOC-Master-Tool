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
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Transitions;
using appCore.DB;
using appCore.Logs;
using appCore.Settings;
using appCore.SiteFinder;
using appCore.SiteFinder.UI;
using appCore.Templates;
using appCore.Templates.UI;
using appCore.Toolbox;
using appCore.UI;
using appCore.Shifts;

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
		
		public static ShiftsPanel shiftsPanel;
		public static List<Rectangle> rectCollection = new List<Rectangle>();
		public static DataRow[] foundRows;
		public static DateTime shiftsChosenDate = DateTime.Now;
		public static Bitmap shiftsBodySnap;
		public static WholeShiftsPanel wholeShiftsPanel = new WholeShiftsPanel();
		public static Bitmap wholeShiftSnap;
		static string wholeShiftString;
		
		DataView eScriptCellsGlobal = new DataView();
		
		public static TrayIcon trayIcon;
		public static TroubleshootControls TroubleshootUI = new TroubleshootControls();
		public static FailedCRQControls FailedCRQUI = new FailedCRQControls();
		public static UpdateControls UpdateUI = new UpdateControls();
		public static TXControls TXUI = new TXControls();
		public static siteDetails2 SiteDetailsUI;
		public static PictureBox SiteDetailsPictureBox = new PictureBox();
		public static OutageControls OutageUI = new OutageControls();
		public static LogsCollection<Template> logFile = new LogsCollection<Template>();
		static Label TicketCountLabel = new Label();
		
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
				                           	MessageBox.Show("Log file is currently in use, please close it and press OK to retry","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
				                           });
				Tools.darkenBackgroundForm(action,false,this);
				goto Retry;
			}
		}
		
//		void UpdateLogFile(string ToLog, string src, string LogType)
//		{
//			string separator = string.Empty;
//			for (int i = 1; i < 301; i++) {
//				if (i == 151) separator = separator + Environment.NewLine;
//				separator = separator + "*";
//			}
//
		////			TODO: BCP Template - search on logs
		////				>1 per INC
		////				Find all INCs and compare contents
//
//			int[] LogIndex = CheckLogExists(src,LogType);
//
//			if(LogIndex.Length > 0) {
//				string[] Logs = appCore.Logs.UI.LogEditor.ParseLogs(LogFilePath);
//				switch(LogType) {
//					case "Troubleshoot Template": case "Failed CRQ":
//						string existingLog = Logs[LogIndex[0]].Substring(Logs[LogIndex[0]].IndexOf("\r\n", StringComparison.Ordinal) + 2,Logs[LogIndex[0]].Length - (Logs[LogIndex[0]].IndexOf("\r\n", StringComparison.Ordinal) + 2));
//						if (existingLog != ToLog){
//							string[] strTofind = { "\r\n" };
//							string[] log = Logs[LogIndex[0]].Split(strTofind, StringSplitOptions.None);
//
//							ScrollableMessageBox msgBox = new ScrollableMessageBox();
//							Action action = new Action(delegate {
//							                           	msgBox.StartPosition = FormStartPosition.CenterParent;
//							                           	msgBox.Show(Logs[LogIndex[0]], "Existing log found", MessageBoxButtons.YesNo, "Overwrite existing log?",false);
//							                           });
//							Toolbox.Tools.darkenBackgroundForm(action,false,this);
//							if (msgBox.DialogResult == DialogResult.Yes) {
//								Logs = Logs.Where((source, index) => index != LogIndex[0]).ToArray();
//								List<string> LogsList = Logs.ToList<string>();
//								LogsList.Add(dt.ToString("HH:mm:ss") + " - " + LogType + Environment.NewLine + ToLog);
//								Logs = LogsList.ToArray();
//								File.Delete(LogFilePath);
//								foreach (string str in Logs) {
//									if (str == Logs[0])	File.AppendAllText(LogFilePath, str + Environment.NewLine + separator);
//									else File.AppendAllText(LogFilePath, Environment.NewLine + str + Environment.NewLine + separator);
//								}
//							}
//							else return;
//						}
//						break;
//					case "TX Template": case "Update Template":
//						foreach(int index in LogIndex) {
//							string tempLog = Logs[index].Substring(Logs[index].IndexOf("\r\n", StringComparison.Ordinal) + 2,Logs[index].Length - (Logs[index].IndexOf("\r\n", StringComparison.Ordinal) + 2));
//							if(tempLog == ToLog)
//								return;
//						}
//						ToLog += Environment.NewLine + separator;
//						addLog(ToLog,LogType);
//						break;
//					case "BCP":
//						break;
//				}
//
//			}
//			else {
//				ToLog += Environment.NewLine + separator;
//				addLog(ToLog,LogType);
//			}
//		}
//
//		void addLog(string ToLog, string LogType)
//		{
//
//		Retry:
//
//			try
//			{
//				if (CheckLogFileExists("template")) File.AppendAllText(LogFilePath,Environment.NewLine + dt.ToString("HH:mm:ss") + " - " + LogType + Environment.NewLine + ToLog);
//				else File.WriteAllText(LogFilePath,dt.ToString("HH:mm:ss") + " - " + LogType  + Environment.NewLine + ToLog);
//			}
//			catch (IOException)
//			{
//				Action action = new Action(delegate {
//				                           	MessageBox.Show("Log file is currently in use, please close it and press OK to retry","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
//				                           });
//				Toolbox.Tools.darkenBackgroundForm(action,false,this);
//				goto Retry;
//			}
//
//		}
		
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
				TicketCountLabel.Text = logFile.FilterCounts(Template.Filters.TicketCount).ToString();
		}

		void TicketCountLabelClick(object sender, EventArgs e) {
			if(logFile.Exists) {
				if(string.IsNullOrEmpty(TicketCountLabel.Text)) {
					logFile.CheckLogFileIntegrity();
					UpdateTicketCountLabel(true);
				}
				else
					TicketCountLabel.Text = string.Empty;
			}
			else
				if(string.IsNullOrEmpty(TicketCountLabel.Text))
					TicketCountLabel.Text = 0.ToString();
				else
					TicketCountLabel.Text = string.Empty;
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
		
//		void trayIconPopulate()
//		{
//			// Documents on constants for easier filename change
//			string ProcessesDoc = "\\\\vf-pt\\fs\\ANOC-UK\\ANOC-UK 1st LINE\\1. RAN 1st LINE\\Processes";
//			const string TeamContactsDoc = "\\\\vf-pt\\fs\\ANOC-UK\\ANOC-UK 1st LINE\\Contactos\\ANOC Team contact numbers.xlsx";
//			const string VFcontactsDoc = "\\\\vf-pt\\fs\\ANOC-UK\\ANOC-UK 1st LINE\\Contactos\\25-09-2013 USEFUL CONTACTS.xlsx";
//
//			// Add menu items to shortcut menu.
//			MenuItem Documents = new MenuItem("ANOC Documents");
//			Documents.MenuItems.Add("Shifts", (s, e) => Process.Start("excel.exe", '"' + ShiftsDoc + '"'));
//			Documents.MenuItems.Add("ANOC Contacts", (s, e) => Process.Start("excel.exe", '"' + TeamContactsDoc + '"'));
//			Documents.MenuItems.Add("-");
//			Documents.MenuItems.Add("Useful Contacts", (s, e) => Process.Start("excel.exe", '"' + VFcontactsDoc + '"'));
//			Documents.MenuItems.Add("Processes", (s, e) => Process.Start("winword.exe", '"' + ProcessesDoc + '"'));
//
//			MenuItem Links = new MenuItem("Links");
//			Links.MenuItems.Add("ST Internal Citrix", (s, e) => Process.Start("https://st.internal.vodafone.co.uk/"));
//			Links.MenuItems.Add("ANOC-UK Network Share", (s, e) => Process.Start("explorer.exe", '"' + "\\\\vf-pt\\fs\\ANOC-UK" + '"'));
//			Links.MenuItems.Add("Energy Networks", (s, e) => Process.Start("http://www.energynetworks.org/info/faqs/electricity-distribution-map.html"));
//			Links.MenuItems.Add("BT Wholesale", (s, e) => Process.Start("https://www.btwholesale.com/portalzone/portalzone/homeLogin.do"));
//			Links.MenuItems.Add("ALEX", (s, e) => Process.Start("http://oprweb/alex"));
//			Links.MenuItems.Add("Google Translate", (s, e) => Process.Start("http://translate.google.com/"));
//
//			TrayIcon.ContextMenu = new ContextMenu(new MenuItem[] {
//			                                       	Documents,
//			                                       	Links,
//			                                       	new MenuItem("-"),
//			                                       	new MenuItem("Settings", (s, e) => PictureBox1Click(null, null)),
//			                                       	new MenuItem("AMT Browser", (s, e) => PictureBox2Click(null, null)),
//			                                       	new MenuItem("Notes", (s, e) => PictureBox3Click(null, null)),
//			                                       	new MenuItem("Log Browser", (s, e) => PictureBox4Click(null, null)),
//			                                       	new MenuItem("Site Finder", (s, e) => PictureBox5Click(null, null)),
//			                                       	new MenuItem("-"),
//			                                       	new MenuItem("Check for Updates..."),
//			                                       	new MenuItem("Exit AMT", (s, e) => Application.Exit()),
//			                                       });
//			//trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);
//			trayIcon.MouseUp += trayIcon_MouseUp;
//
//			if(GlobalProperties.shareAccess) {
//				string[] ProcessesDocFiles = Directory.GetFiles(ProcessesDoc);
//
//				foreach (string file in ProcessesDocFiles) {
//					if(file.Contains("ANOC UK 1st Line Processes") && file.Contains(".docx")) {
//						ProcessesDoc = file;
//						break;
//					}
//				}
//
//				DirectoryInfo ShiftsDir = new DirectoryInfo(Path.GetDirectoryName(ShiftsDoc));
//				FileInfo[] shiftFiles = ShiftsDir.GetFiles("*shift*.xlsx");
//
//				if(shiftFiles.Length > 0) {
//					DateTime newestDate = Convert.ToDateTime("01/01/1900 00:00:00");
//					foreach (FileInfo file in shiftFiles) {
//						if(file.CreationTime > newestDate && !file.Attributes.ToString().Contains("Hidden")) {
//							newestDate = file.CreationTime;
//							ShiftsDoc = file.FullName;
//						}
//					}
//				}
//
//				if(Toolbox.Tools.GetUserDetails("Username") == "GONCARJ3") {
//					string errmsg = string.Empty;
//					if(!File.Exists(ProcessesDoc))
//						errmsg += "Processes Document";
//					if(!File.Exists(ShiftsDoc)) {
//						if(!string.IsNullOrEmpty(errmsg))
//							errmsg += Environment.NewLine;
//						errmsg += "Shifts Document";
//					}
//					if(!File.Exists(TeamContactsDoc)) {
//						if(!string.IsNullOrEmpty(errmsg))
//							errmsg += Environment.NewLine;
//						errmsg += "Team Contacts Document";
//					}
//					if(!File.Exists(VFcontactsDoc)) {
//						if(!string.IsNullOrEmpty(errmsg))
//							errmsg += Environment.NewLine;
//						errmsg += "Useful Contacts Document";
//					}
//					if (!string.IsNullOrEmpty(errmsg))
//						showBalloon("The following documents were not found",errmsg);
//				}
//			}
//			else {
//				FileInfo[] shiftFiles = UserProperties.userFolder.GetFiles("*shift*.xlsx");
//
//				if(shiftFiles.Length > 0) {
//					DateTime newestDate = Convert.ToDateTime("01/01/1900 00:00:00");
//					foreach (FileInfo file in shiftFiles) {
//						if(file.CreationTime > newestDate && !file.Attributes.ToString().Contains("Hidden")) {
//							newestDate = file.CreationTime;
//							ShiftsDoc = file.FullName;
//						}
//					}
//				}
//
//				foreach (MenuItem item in Documents.MenuItems) {
//					if(item.Name == "Shifts" && File.Exists(ShiftsDoc))
//						item.Enabled = true;
//					else
//						item.Enabled = false;
//				}
//				Links.MenuItems[1].Enabled = false;
//				TrayIcon.ContextMenu.MenuItems[9].Enabled = false;
//			}
//		}
//
//		void trayIcon_MouseUp(object sender, MouseEventArgs e)
//		{
//			if (e.Button == MouseButtons.Left) {
//				MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
//				mi.Invoke(trayIcon, null);
//			}
//		}
		
		void buttClick(object sender, EventArgs e) {
			Button bt = (Button)sender;
			if(bt.Name == "butt") {
				if(OutageUI != null)
					OutageUI.Dispose();
				tabControl1.SelectTab(6);
				OutageUI = new OutageControls();
				tabPage17.Controls.Add(OutageUI);

			}
			else {
				Thread t = new Thread(Databases.UpdateSourceDBFiles);
				t.Start();
			}
		}
		
		public MainForm(NotifyIcon tray)
		{
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			GlobalProperties.resolveOfficePath();
			
			SplashForm.ShowSplashScreen();
			trayIcon = new TrayIcon(tray);
//			trayIc = tray;
//			tray.Dispose();
			
			Tools.EmbeddedAssembliesInit();
			
			SplashForm.UpdateLabelText("Getting network access");
			
			// Initialize Properties
			
			GlobalProperties.CheckShareAccess();
			
			SplashForm.UpdateLabelText("Setting User Folder");
			
			CurrentUser.InitializeUserProperties();
			
			SplashForm.UpdateLabelText("Setting User Settings");
			
			logFile.Initialize();
			
			SplashForm.UpdateLabelText("Loading UI");
			
			InitializeComponent();
			
			string img = SettingsFile.BackgroundImage;
			
			if (img != "Default") {
				if (File.Exists(img))
					tabPage1.BackgroundImage = Image.FromFile(img);
				else
					trayIcon.showBalloon("Image file not found", "Background Image file not found, applying default");
			}
			
			tabPage1.Controls.Add(SiteDetailsPictureBox);
			// 
			// pictureBox5
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
//			TicketCountLabel.Location = new Point(480, 631);
			TicketCountLabel.Location = new Point(tabPage1.Width - TicketCountLabel.Width - 5, tabPage1.Height - TicketCountLabel.Height - 5);
			TicketCountLabel.Name = "TicketCountLabel";
			TicketCountLabel.TabIndex = 5;
			TicketCountLabel.TextAlign = ContentAlignment.MiddleRight;
			TicketCountLabel.Click += TicketCountLabelClick;
			
			// UNDONE: Developer specific action
			if(CurrentUser.userName == "GONCARJ3" || CurrentUser.userName == "Caramelos" || CurrentUser.userName == "SANTOSS2") {
				Button butt2 = new Button();
				butt2.Name = "butt2";
				butt2.Location = new Point(5, tabPage1.Height - butt2.Height - 5);
				butt2.Text = "Update OI DB Files";
				butt2.AutoSize = true;
				butt2.Click += buttClick;
				tabPage1.Controls.Add(butt2);
				
				Button butt = new Button();
				butt.Name = "butt";
				butt.Location = new Point(5, butt2.Top - butt.Height - 5);
				butt.Click += buttClick;
				tabPage1.Controls.Add(butt);
				if(CurrentUser.userName == "GONCARJ3" || CurrentUser.userName == "Caramelos") {
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
			
			SplashForm.UpdateLabelText("Loading Databases");
			
			Databases.PopulateDatabases();
			
			GlobalProperties.siteFinder_mainswitch = false;
			GlobalProperties.siteFinder_mainswitch = Databases.siteDetailsTable != null || Databases.cellDetailsTable != null;
			
			if((CurrentUser.department.Contains("1st Line RAN") || CurrentUser.department.Contains("First Line Operations")) && Databases.shiftsFile.Exists) {
				foundRows = Databases.shiftsFile.monthTables[DateTime.Now.Month - 1].Select("AbsName Like '" + Tools.RemoveDiacritics(CurrentUser.fullName[1]).ToUpper() + "%' AND AbsName Like '%" + Tools.RemoveDiacritics(CurrentUser.fullName[0]).ToUpper() + "'");
				
				if(foundRows.Length < 1) {
					pictureBox6.Visible = false;
					goto noPanel;
				}
				
				shiftsPanel = new ShiftsPanel();
				shiftsPanel.DoubleBufferActive = true;
				shiftsPanel.BorderColor = Color.Black;
				shiftsPanel.BackColor = Color.Gray;
				shiftsPanel.Size = new Size(220, 26);
				shiftsPanel.BorderWidth = 1.25f;
				shiftsPanel.BordersToDraw = ShiftsPanel.Borders.Left | ShiftsPanel.Borders.Bottom | ShiftsPanel.Borders.Right;
//					shiftsPanel.BordersToDraw = ShiftsPanel.Borders.None;
				shiftsPanel.CornersToRound = ShiftsPanel.Corners.BottomLeft | ShiftsPanel.Corners.BottomRight;
				shiftsPanel.MouseClick += shiftsPanel_MouseClick;
				PictureBox shiftsPanel_icon = new PictureBox();
				((System.ComponentModel.ISupportInitialize)(shiftsPanel_icon)).BeginInit();
				shiftsPanel_icon.BackColor = Color.Gray;
				shiftsPanel_icon.Name = "shiftsPanel_icon";
				shiftsPanel_icon.Size = new Size(16, 16);
				shiftsPanel_icon.Image = Resources.Business_Planner_icon;
				shiftsPanel_icon.SizeMode = PictureBoxSizeMode.StretchImage;
				shiftsPanel_icon.Parent = shiftsPanel;
				shiftsPanel_icon.Location = new Point(5, 5);
				shiftsPanel.Controls.Add(shiftsPanel_icon);
				PictureBox shiftsPanel_refresh = new PictureBox();
				((System.ComponentModel.ISupportInitialize)(shiftsPanel_refresh)).BeginInit();
				shiftsPanel_refresh.BackColor = Color.Gray;
				shiftsPanel_refresh.Name = "shiftsPanel_refresh";
				shiftsPanel_refresh.Size = new Size(16, 16);
				shiftsPanel_refresh.Image = Resources.Replace_64;
				shiftsPanel_refresh.SizeMode = PictureBoxSizeMode.StretchImage;
				shiftsPanel_refresh.Parent = shiftsPanel;
				shiftsPanel_refresh.Location = new Point(shiftsPanel.Width - 21, 5);
				shiftsPanel_refresh.Click += shiftsPanel_refreshClick;
//					shiftsPanel_refresh.MouseLeave += (this.PictureBox5MouseLeave);
//					shiftsPanel_refresh.MouseHover += (this.PictureBox5MouseHover);
				shiftsPanel.Controls.Add(shiftsPanel_refresh);
				shiftsPanel.Name = "shiftsPanel";
//					shiftsPanel.MouseMove += shiftsPanel_MouseMove;
//					http://www.codeproject.com/Articles/38436/Extended-Graphics-Rounded-rectangles-Font-metrics
				shiftsBodySnap = Toolbox.Tools.loadShifts(shiftsBodySnap, shiftsChosenDate);
				shiftsPanel.Location = new Point((tabPage1.Width - shiftsPanel.Width) / 2, 0 - shiftsPanel.Height);
				shiftsPanel.Paint += shiftsPanelPaint;
				shiftsPanel.LocationChanged += shiftsPanel_LocationChanged;
				
				tabPage1.Controls.Add(shiftsPanel);
			}
			else
				pictureBox6.Visible = false;
			
			noPanel:;
			
			// TODO: get sites list from alarms
			
			SplashForm.UpdateLabelText("Almost finished");
			
			trayIcon.toggleShareAccess();
			
			toolTipDeploy();
			
			richTextBox9.Height = 206;
			richTextBox9.Width = 499;
			richTextBox9.Left = 6;
			richTextBox9.Top = 32;
			
			string thisfn = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\appCore.dll";
			
			if (SettingsFile.LastRunVersion != FileVersionInfo.GetVersionInfo(thisfn).FileVersion) {
				SettingsFile.LastRunVersion = FileVersionInfo.GetVersionInfo(thisfn).FileVersion;
				ScrollableMessageBox msgBox = new ScrollableMessageBox();
				msgBox.StartPosition = FormStartPosition.CenterParent;
				msgBox.Show(Resources.Changelog, "Changelog", MessageBoxButtons.OK, "New Version Changelog",false);
			}
			SplashForm.CloseForm();
		}

		void shiftsPanel_LocationChanged(object sender, EventArgs e)
		{
			if(shiftsPanel.Location.Y == 0)
				shiftsPanel.Refresh();
		}

		void shiftsPanel_MouseClick(object sender, MouseEventArgs e)
		{
			if(wholeShiftsPanel.Parent != null) {
				wholeShiftsPanelDispose();
				return;
			}
			
			bool loopFinished = true;
			for(int c = 0;c < rectCollection.Count;c++)
			{
				if(rectCollection[c].Contains(e.Location)) {
					shiftsChosenDate = new DateTime(shiftsChosenDate.Year, shiftsChosenDate.Month, c + 1);
					loopFinished = false;
					break;
				}
			}
			if(loopFinished)
				return;
			
			string shift = Databases.shiftsFile.monthTables[shiftsChosenDate.Month - 1].Select("AbsName Like '" + Toolbox.Tools.RemoveDiacritics(CurrentUser.fullName[1]).ToUpper() + "%' AND AbsName Like '%" + Toolbox.Tools.RemoveDiacritics(CurrentUser.fullName[0]).ToUpper() + "'")[0]["Day" + shiftsChosenDate.Day].ToString();
			if(string.IsNullOrEmpty(shift))
				return;
			
			DataRow[] sameShiftRows = Tools.getWholeShift(shiftsChosenDate);
			
			if(sameShiftRows == null)
				return;
			if(sameShiftRows.Length == 0)
				return;
			
			wholeShiftsPanel = new WholeShiftsPanel();
			
			List<DataRow> SL = new List<DataRow>();
			List<DataRow> Agents = new List<DataRow>();
			FieldInfo _rowID;
			foreach(DataRow dr in sameShiftRows) {
				_rowID = typeof(DataRow).GetField("_rowID", BindingFlags.NonPublic | BindingFlags.Instance);
				int rowID = (int)Convert.ToInt64(_rowID.GetValue(dr));
				if(rowID > 3 && rowID < 12)
					SL.Add(dr);
				else
					Agents.Add(dr);
			}
			
			// Draw panel
			// FIXME: improve wholeShiftsPanel performance(generate a cache for individual shifts on separate thread)
			const int RectHeight = 20;
			const int nameRectWidth = 145;
			const int shiftRectWidth = 35;
			const int headerSpacing = 10;
			const int paddingVertical = 7;
			const int paddingHorizontal = 7;
			int num_lines = SL.Count + Agents.Count + 3; // + 3 for Title, SL & Agents headers
			
			int panelHeight = (int)(2 * paddingVertical) + (2 * headerSpacing) + (num_lines * RectHeight);
			int panelWidth = (int)(2 * paddingHorizontal) + nameRectWidth + shiftRectWidth;
			
			wholeShiftString = string.Empty;
			wholeShiftSnap = new Bitmap(panelWidth, panelHeight);
			
			using (Graphics g = Graphics.FromImage(wholeShiftSnap)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality; // Set format of string.
				g.FillRectangle(new SolidBrush(Color.Black), 0, 0, wholeShiftSnap.Width, wholeShiftSnap.Height);
				
				StringFormat drawStringFormat = new StringFormat();
				drawStringFormat.Alignment = StringAlignment.Center;
				drawStringFormat.LineAlignment = StringAlignment.Center;
				Font titlesFont = new Font("Tahoma",11, FontStyle.Bold);
				Font stringFont = new Font("Tahoma",8, FontStyle.Bold);
				
				string tempText = String.Format("{0:dd/MM/yyyy}", shiftsChosenDate) + " - " + shift + " Shift";
				wholeShiftString += tempText + Environment.NewLine;
				Rectangle rectangle = new Rectangle(new Point(paddingHorizontal, paddingVertical), new Size(wholeShiftSnap.Width - (paddingHorizontal * 2), RectHeight));
				g.DrawString(tempText, titlesFont, Brushes.Gray, rectangle, drawStringFormat);
				
				int previousRectBottomCoord = rectangle.Bottom;
				
				tempText = "Shift Leaders:";
				wholeShiftString += Environment.NewLine + tempText + Environment.NewLine;
				drawStringFormat.Alignment = StringAlignment.Near;
				rectangle = new Rectangle(new Point(7, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
				g.DrawString(tempText, titlesFont, Brushes.Red, rectangle, drawStringFormat);
				previousRectBottomCoord = rectangle.Bottom;
				
				foreach(DataRow dr in SL) {
					rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
					tempText = dr["Column3"].ToString();
					wholeShiftString += tempText + '\t';
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					drawStringFormat.Alignment = StringAlignment.Far;
					rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
					tempText = dr["Day" + shiftsChosenDate.Day].ToString();
					wholeShiftString += tempText + Environment.NewLine;
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					previousRectBottomCoord = rectangle.Bottom;
					drawStringFormat.Alignment = StringAlignment.Near;
				}
				
				tempText = "Agents:";
				wholeShiftString += Environment.NewLine + tempText + Environment.NewLine;
				rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
				g.DrawString(tempText, titlesFont, Brushes.Red, rectangle, drawStringFormat);
				previousRectBottomCoord = rectangle.Bottom;
				
				for(int c = 1;c <= Agents.Count;c++) {
					rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
					tempText = Agents[c - 1]["Column3"].ToString();
					wholeShiftString += tempText + '\t';
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					drawStringFormat.Alignment = StringAlignment.Far;
					rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
					tempText = Agents[c - 1]["Day" + shiftsChosenDate.Day].ToString();
					wholeShiftString += tempText;
					if(c < Agents.Count)
						wholeShiftString +=  Environment.NewLine;
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					previousRectBottomCoord = rectangle.Bottom;
					drawStringFormat.Alignment = StringAlignment.Near;
				}
//				if(Tools.GetUserDetails("Username") == "Caramelos")
//					wholeShiftSnap.Save(UserFolderPath + @"\wholeShiftSnap.png");
//				else
//					wholeShiftSnap.Save(@"\\vf-pt\fs\ANOC-UK\ANOC-UK 1st LINE\1. RAN 1st LINE\ANOC Master Tool\ANOC Master Tool\wholeShiftSnap.png");
			}
			PictureBox shiftsPictureBox = new PictureBox();
			shiftsPictureBox.Parent = this;
			shiftsPictureBox.Size = new Size(wholeShiftSnap.Width, wholeShiftSnap.Height);
			shiftsPictureBox.Location = new Point(0, 0);
			shiftsPictureBox.Image = wholeShiftSnap;
			shiftsPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
			Button shiftsCopyButton = new Button();
			shiftsCopyButton.BackColor = Color.Black;
			shiftsCopyButton.FlatStyle = FlatStyle.Flat;
			shiftsCopyButton.ForeColor = Color.Gray;
			shiftsCopyButton.Text = "Copy to Clipboard";
			shiftsCopyButton.Size = new Size(shiftsPictureBox.Width, 23);
			shiftsCopyButton.Location = new Point(0, shiftsPictureBox.Bottom);
			shiftsCopyButton.Click += shiftsCopyButtonClick;
			wholeShiftsPanel.AutoScroll = true;
//			wholeShiftsPanel.BordersToDraw = ShapedPanel.Borders.None;
//			wholeShiftsPanel.CornersToRound = ShapedPanel.Corners.None;
			wholeShiftsPanel.Location = e.Location;
			wholeShiftsPanel.BackColor = Color.Black;
			wholeShiftsPanel.Capture = true;
//			wholeShiftsPanel.MouseWheel += wholeShiftsPanelMouseWheel;
			wholeShiftsPanel.MouseDown += wholeShiftsPanelMouseDown;
			wholeShiftsPanel.MouseEnter += wholeShiftsPanelMouseEnter;
//			wholeShiftsPanel.MouseLeave += wholeShiftsPanelMouseLeave;
			const int wholeshiftsPanelMaxHeight = 277;
			if(wholeShiftSnap.Height + shiftsCopyButton.Height > wholeshiftsPanelMaxHeight) {
				wholeShiftsPanel.Size = new Size(wholeShiftSnap.Width + SystemInformation.VerticalScrollBarWidth, wholeshiftsPanelMaxHeight);
//				wholeShiftsPanel.Size = new Size(wholeShiftSnap.Width, wholeshiftsPanelMaxHeight);
//				wholeShiftsPanel.AutoScrollPosition = new Point(0, 0);
//				wholeShiftsPanel.VerticalScroll.Maximum = (wholeShiftSnap.Height + shiftsCopyButton.Height) - wholeshiftsPanelMaxHeight;
//				wholeShiftsPanel.VerticalScroll.Minimum = 0;
			}
			else
				wholeShiftsPanel.Size = new Size(wholeShiftSnap.Width, wholeShiftSnap.Height + shiftsCopyButton.Height);
			wholeShiftsPanel.Controls.Add(shiftsPictureBox);
			wholeShiftsPanel.Controls.Add(shiftsCopyButton);
			this.Controls.Add(wholeShiftsPanel);
			wholeShiftsPanel.BringToFront();
		}
		
		void wholeShiftsPanelDispose() {
			if(wholeShiftsPanel.Parent != null) {
				wholeShiftsPanel.Dispose();
				shiftsPanel.Invalidate(true);
				return;
			}
		}
		
		void wholeShiftsPanelMouseWheel(object sender, MouseEventArgs e)
		{
			wholeShiftsPanel.SuspendLayout();
			if (e.Delta == 120)
			{
				if (wholeShiftsPanel.VerticalScroll.Value - 2 >= wholeShiftsPanel.VerticalScroll.Minimum)
					wholeShiftsPanel.VerticalScroll.Value -= 2;
				else
					wholeShiftsPanel.VerticalScroll.Value = wholeShiftsPanel.VerticalScroll.Minimum;
			}
			if (e.Delta == -120)
			{
				if (wholeShiftsPanel.VerticalScroll.Value + 2 <= wholeShiftsPanel.VerticalScroll.Maximum)
					wholeShiftsPanel.VerticalScroll.Value += 2;
				else
					wholeShiftsPanel.VerticalScroll.Value = wholeShiftsPanel.VerticalScroll.Maximum;
			}
			wholeShiftsPanel.ResumeLayout();
		}
		
		void wholeShiftsPanelMouseEnter(object sender, EventArgs e) {
			wholeShiftsPanel.Focus();
		}
		
		void wholeShiftsPanelMouseLeave(object sender, EventArgs e) {
			wholeShiftsPanel.Focus();
		}
		
		void wholeShiftsPanelMouseDown(object sender, MouseEventArgs e) {
			Rectangle wholePanelArea = Rectangle.Union(wholeShiftsPanel.ClientRectangle,new Rectangle(wholeShiftsPanel.Right, wholeShiftsPanel.Top, 23, wholeShiftsPanel.Height));
			if(!wholePanelArea.Contains(e.Location)) {
				if(wholeShiftsPanel.Parent != null) {
					wholeShiftsPanel.Dispose();
					shiftsPanel.Invalidate(true);
				}
			}
		}
		
		void shiftsCopyButtonClick(object sender, EventArgs e) {
			Clipboard.SetText(wholeShiftString);
			trayIcon.showBalloon("Copied to clipboard","Copied to clipboard");
		}
		
		void shiftsPanel_refreshClick(object sender, EventArgs e)
		{
			Thread t = new Thread(() => {
			                      	shiftsBodySnap = Toolbox.Tools.loadShifts(shiftsBodySnap, shiftsChosenDate);
			                      	shiftsPanel.Invalidate(true);
			                      });
			t.Start();
		}
		
//		bool siteFinder_mainswitch {
//			get {
//				return _siteFinder_mainswitch;
//			}
//			set {
//				if(_siteFinder_mainswitch == value)
//					return;
//				_siteFinder_mainswitch = value;
//				if (!_siteFinder_mainswitch) {
		////					textBox27.KeyPress -= siteFinder;
		////					textBox43.KeyPress -= siteFinder;
//					textBox50.KeyPress -= siteFinder;
//					pictureBox5.Visible = false;
//					siteFinder_Toggle(true,false,"All");
//				}
//				else {
		////					textBox27.KeyPress += siteFinder;
		////					textBox43.KeyPress += siteFinder;
//					textBox50.KeyPress += siteFinder;
//					pictureBox5.Visible = true;
//					siteFinder_Toggle(false,false,"All");
//				}
//			}
//		}
//
//		public static bool siteFinderMainswitch {
//			get {
//				return _siteFinder_mainswitch;
//			}
//		}

		public void siteFinder(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				Action action = new Action(delegate {
//				                           	Stopwatch sw = new Stopwatch();
//
//				                           	sw.Start();
				                           	TextBoxBase tb = (TextBoxBase)sender;
				                           	DataRowView siteRow = null;
				                           	DataView cellsRows = null;
				                           	
				                           	if(!string.IsNullOrEmpty(tb.Text)) {
				                           		siteRow = findSite(tb.Text);
				                           		cellsRows = findCells(tb.Text);
				                           	}
				                           	else
				                           		return;
				                           	
				                           	bool siteFound = siteRow != null;
				                           	
				                           	Site site = new Site(siteRow, cellsRows);
				                           	switch (tb.Name) {
				                           		case "textBox50":
				                           			if(siteRow != null) {
				                           				cellsRows.RowFilter = "VENDOR LIKE 'ERIC*'";
				                           				if(cellsRows.Count == 0){
				                           					trayIcon.showBalloon("Error",String.Format("Site {0} is not E///",tb.Text));
				                           					return;
				                           				}
				                           				cellsRows.RowFilter = string.Empty;
				                           				eScriptCellsGlobal = cellsRows;
				                           				textBox51.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("PRIORITY")].ToString();
				                           				textBox52.Text = siteRow[siteRow.Row.Table.Columns.IndexOf("VF_REGION")].ToString();
				                           				
				                           				pictureBox7.UpdateCells(eScriptCellsGlobal);
				                           				eScriptCellsGlobal.RowFilter = "BEARER = '2G' AND VENDOR LIKE 'ERIC*'";
				                           				radioButton6.Enabled |= eScriptCellsGlobal.Count > 0;
				                           				eScriptCellsGlobal.RowFilter = "BEARER = '3G' AND VENDOR LIKE 'ERIC*'";
				                           				radioButton7.Enabled |= eScriptCellsGlobal.Count > 0;
				                           				eScriptCellsGlobal.RowFilter = "BEARER = '4G' AND VENDOR LIKE 'ERIC*'";
				                           				radioButton8.Enabled |= eScriptCellsGlobal.Count > 0;
				                           				
				                           				eScriptCellsGlobal.RowFilter = string.Empty;
				                           			}
				                           			else {
				                           				textBox51.Text = string.Empty;
				                           				textBox52.Text = "No site found";
				                           			}
				                           			break;
				                           	}
//				                           	sw.Stop();
//
//				                           	FlexibleMessageBox.Show("Elapsed=" + sw.Elapsed);
				                           	siteFinder_Toggle(true, siteFound, tb.Name);
				                           });
				Toolbox.Tools.darkenBackgroundForm(action,true,this);
			}
		}

		public static DataRowView findSite(string site)
		{
			if(!Tools.IsAllDigits(site))
				site = "00000";
			
			DataView dv = new DataView(Databases.siteDetailsTable);
			dv.RowFilter = "SITE = '" + site + "'"; // query example = "id = 10"
			DataRowView dr = null;
			if(dv.Count == 1)
				dr = dv[0];
			return dr;
		}

		public static DataView findCells(string site)
		{
			if(!Tools.IsAllDigits(site))
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

		void MainFormShown(object sender, EventArgs e)
		{
			this.BeginInvoke(new Action(delegate {
			                            	if(GlobalProperties.siteFinder_mainswitch)
			                            		siteFinder_Toggle(false, false, "All");
			                            	else
			                            		siteFinder_Toggle(true, false, "All");
			                            }));
			
			this.Shown -= MainFormShown;
		}

		void siteFinder_Toggle(bool toggle, bool siteFound, string sender)
		{
			if(sender == "textBox50") {
				List<Control> parentControls = new List<Control>();
				if(sender == "textBox43" || sender == "All")
					parentControls.AddRange(tabPage6.Controls.OfType<Control>());
				if(sender == "textBox50" || sender == "All")
					parentControls.AddRange(tabPage13.Controls.OfType<Control>());
				foreach (object ctrl in parentControls) {
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
						case "System.Windows.Forms.RichTextBox": case "System.Windows.Forms.TextBox":
							TextBoxBase tb = ctrl as TextBoxBase;
							if(tb.Name != "textBox43" && tb.Name != "textBox45" && !(Convert.ToInt16(tb.Name.Substring(tb.Name.Length - 2)) >= 48 && Convert.ToInt16(tb.Name.Substring(tb.Name.Length - 2)) <= 52))
								tb.Enabled = toggle;
							break;
					}
					ListView2ItemChecked(null,null);
				}
			}
		}

		void TextBox13TextChanged(object sender, EventArgs e)
		{
			textBox14.Text = "";
			if (textBox13.Text.Length == 15 & textBox15.Text.Length == 3)
			{
				label30.Visible = false;
				textBox13.TextChanged -= TextBox13TextChanged;
				textBox15.TextChanged -= TextBox15TextChanged;
				textBox13.Text = textBox13.Text.ToUpper();
				textBox15.Text = textBox15.Text.ToUpper();
				textBox13.TextChanged += TextBox13TextChanged;
				textBox15.TextChanged += TextBox15TextChanged;
				if (Toolbox.Tools.IsAllDigits(textBox13.Text.Substring(3,textBox13.Text.Length - 3))) {
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
					textBox14.Text += hx + " " + textBox15.Text;
				}
				else {
					Action action = new Action(delegate {
					                           	MessageBox.Show("INC/CRQ can only contain numbers","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           	textBox13.TextChanged -= TextBox13TextChanged;
					                           	textBox13.Text = "";
					                           	textBox13.TextChanged += TextBox13TextChanged;
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
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

		void TextBox15TextChanged(object sender, EventArgs e)
		{
			try{
				if (textBox15.Text.Length == 3 & textBox13.Text.Length == 15) {
					label31.Visible = false;
					TextBox13TextChanged(sender, e);
				}
				else {
					textBox14.Text = "";
					if (textBox15.Text.Length < 3) label31.Visible = true;
					else {
						textBox15.TextChanged -= TextBox15TextChanged;
						textBox15.Text = textBox15.Text.ToUpper();
						textBox15.TextChanged += TextBox15TextChanged;
						label31.Visible = false;
					}
				}
			}
			finally {}
		}

//		void Button1Click(object sender, EventArgs e)
//		{
//			Action action = new Action(delegate {
//			                           	string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(textBox1.Text, "INC");
//			                           	if (CompINC_CRQ != "error") textBox1.Text = CompINC_CRQ;
//			                           	else {
//			                           		MessageBox.Show("INC number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
//			                           		return;
//			                           	}
//			                           	string errmsg = "";
//			                           	if (string.IsNullOrEmpty(textBox1.Text)) {
//			                           		errmsg = "         - INC/Ticket Number missing\n";
//			                           	}
//			                           	if (string.IsNullOrEmpty(textBox2.Text)) {
//			                           		errmsg += "         - Site ID missing\n";
//			                           	}
//			                           	if (comboBox1.SelectedIndex == 1 && string.IsNullOrEmpty(textBox3.Text)) {
//			                           		errmsg += "          - TF Site ID missing\n";
//			                           	}
//			                           	if (string.IsNullOrEmpty(textBox4.Text)) {
//			                           		errmsg += "         - Site Address missing\n";
//			                           	}
//			                           	if (string.IsNullOrEmpty(textBox7.Text)) {
//			                           		errmsg += "         - Active alarms missing\n";
//			                           	}
//			                           	if (string.IsNullOrEmpty(textBox9.Text)) {
//			                           		errmsg += "         - Troubleshoot missing\n";
//			                           	}
//			                           	if (checkBox2.Checked) {
//			                           		if ((numericUpDown1.Value == 0) && (numericUpDown2.Value == 0) && (numericUpDown3.Value == 0)) {
//			                           			errmsg += "         - COOS count missing\n";
//			                           		}
//			                           	}
//			                           	if (!string.IsNullOrEmpty(errmsg)) {
//			                           		MessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
//			                           		return;
//			                           	}
//			                           	errmsg = "";
//
//			                           	// No changes since the last template warning
//
//			                           	if (prevTemp[0] != "-") {
//			                           		if (textBox1.Text == prevTemp[0]) {
//			                           			errmsg = "         - INC\n";
//			                           		}
//			                           		if (textBox2.Text == prevTemp[1]) {
//			                           			errmsg += "         - Site ID\n";
//			                           		}
//			                           		if (comboBox1.Text == "TF" && textBox3.Text == prevTemp[2]) {
//			                           			errmsg += "         - TF Site ID\n";
//			                           		}
//			                           		if (textBox4.Text == prevTemp[3]) {
//			                           			errmsg += "         - Site Address\n";
//			                           		}
//			                           		if (textBox5.Text != "" && textBox5.Text == prevTemp[4]) {
//			                           			errmsg += "         - CCT reference\n";
//			                           		}
//			                           		if (checkBox1.Checked && checkBox1.Checked.ToString() == prevTemp[5]){
//			                           			errmsg += "         - Other sites impacted\n";
//			                           		}
//			                           		if (checkBox2.Checked) {
//			                           			if(numericUpDown1.Text == prevTemp[6] && numericUpDown2.Text == prevTemp[7] && numericUpDown3.Text == prevTemp[8]) {
//			                           				if (checkBox2.Checked && numericUpDown1.Text == prevTemp[6]){
//			                           					errmsg += "         - 2G COOS count\n";
//			                           				}
//			                           				if (checkBox2.Checked && numericUpDown2.Text == prevTemp[7]) {
//			                           					errmsg += "         - 3G COOS count\n";
//			                           				}
//			                           				if (checkBox2.Checked && numericUpDown3.Text == prevTemp[8]) {
//			                           					errmsg += "         - 4G COOS count\n";
//			                           				}
//			                           			}
//			                           			if(checkBox18.Checked && checkBox18.Checked.ToString() == prevTemp[9])
//			                           				errmsg += "         - Full Site Outage flag\n";
//			                           		}
//			                           		if (checkBox3.Checked && checkBox3.Checked.ToString() == prevTemp[10]) {
//			                           			errmsg += "         - Performance issue\n";
//			                           		}
//			                           		if (checkBox4.Checked && checkBox4.Checked.ToString() == prevTemp[11]) {
//			                           			errmsg += "         - Intermittent issue\n";
//			                           		}
//			                           		if (textBox6.Text != "" && textBox6.Text == prevTemp[12]) {
//			                           			errmsg += "         - Related INC/CRQ\n";
//			                           		}
//			                           		if (textBox7.Text == prevTemp[13]) {
//			                           			errmsg += "         - Active Alarms\n";
//			                           		}
//			                           		if (textBox8.Text != "" && textBox8.Text == prevTemp[14]) {
//			                           			errmsg += "         - Alarm History\n";
//			                           		}
//			                           		if (textBox9.Text == prevTemp[15]) {
//			                           			errmsg += "         - Troubleshoot\n";
//			                           		}
//			                           		if (errmsg != "") {
//			                           			DialogResult ans = MessageBox.Show("You haven't changed the following fields in the template:\n\n" + errmsg + "\nDo you want to continue anyway?","Same INC",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
//			                           			if (ans == DialogResult.No)	return;
//			                           		}
//			                           	}
//			                           	Template test = new Templates.Types.TroubleShoot(tabPage8.Controls);
//			                           	string template = buildTST();
//
//			                           	try {
//			                           		Clipboard.SetText(template);
//			                           	}
//			                           	catch (Exception) {
//			                           		try {
//			                           			Clipboard.SetText(template);
//			                           		}
//			                           		catch (Exception) {
//			                           			MessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
//			                           		}
//			                           	}
//
//			                           	ScrollableMessageBox msgBox = new ScrollableMessageBox();
//			                           	msgBox.StartPosition = FormStartPosition.CenterParent;
//			                           	msgBox.Show(template, "Success", MessageBoxButtons.OK, "Template copied to Clipboard",true);
//
//			                           	// Store this template for future warning on no changes
//
//			                           	prevTemp[0] = textBox1.Text;
//			                           	prevTemp[1] = textBox2.Text;
//			                           	prevTemp[2] = textBox3.Text;
//			                           	prevTemp[3] = textBox4.Text;
//			                           	prevTemp[4] = textBox5.Text;
//			                           	prevTemp[5] = checkBox1.Checked.ToString();
//			                           	prevTemp[6] = numericUpDown1.Text;
//			                           	prevTemp[7] = numericUpDown2.Text;
//			                           	prevTemp[8] = numericUpDown3.Text;
//			                           	prevTemp[9] = checkBox18.Checked.ToString();
//			                           	prevTemp[10] = checkBox3.Checked.ToString();
//			                           	prevTemp[11] = checkBox4.Checked.ToString();
//			                           	prevTemp[12] = textBox6.Text;
//			                           	prevTemp[13] = textBox7.Text;
//			                           	prevTemp[14] = textBox8.Text;
//			                           	prevTemp[15] = textBox9.Text;
//
//			                           	UpdateLogFile(template, textBox1.Text, "Troubleshoot Template"); // store template in logfile
//			                           	if (label40.Text != string.Empty) Updatelabel40();
//			                           	button44.Enabled = true;
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,false,this);
//		}

//		string buildTST()
//		{
//			string template = "INC: " + textBox1.Text + Environment.NewLine;
//			template += "Site ID: " + textBox2.Text + Environment.NewLine;
//			template += "Site Owner: ";
//			if (comboBox1.SelectedIndex == -1) {
//				comboBox1.SelectedIndex = 0;
//			}
//			else {
//				if (comboBox1.SelectedIndex == 1) {
//					comboBox1.Text = "TF (" + textBox3.Text + ")";
//				}
//			}
//			template += comboBox1.Text + Environment.NewLine;
//			template += "Site Address: " + textBox4.Text + Environment.NewLine;
//			template += "Other sites impacted: ";
//			if (checkBox1.Checked){
//				template += "YES * more info on the INC" + Environment.NewLine;
//			}
//			else {
//				template += "None" + Environment.NewLine;
//			}
//			template += "COOS: ";
//			if (checkBox2.Checked) {
//				template += "YES 2G: " + numericUpDown1.Value + " 3G: " + numericUpDown2.Value + " 4G: " + numericUpDown3.Value + Environment.NewLine;
//			}
//			else
//				template += "No" + Environment.NewLine;
//			template += "Full Site Outage: ";
//			if(checkBox18.Checked)
//				template += "YES" + Environment.NewLine;
//			else
//				template += "No" + Environment.NewLine;
//			template += "Performance Issue: ";
//			if (checkBox3.Checked) {
//				template += "YES" + Environment.NewLine;
//			}
//			else {
//				template += "No" + Environment.NewLine;
//			}
//			template += "Intermittent Issue: ";
//			if (checkBox4.Checked) {
//				template += "YES" + Environment.NewLine;
//			}
//			else {
//				template += "No" + Environment.NewLine;
//			}
//			if (!string.IsNullOrEmpty(textBox5.Text)) {
//				template += "CCT Reference: " + textBox5.Text + Environment.NewLine;
//			}
//			else {
//				template += "CCT Reference: None" + Environment.NewLine;
//			}
//			if (!string.IsNullOrEmpty(textBox6.Text)) {
//				template += "Related INC/CRQ: " + textBox6.Text + Environment.NewLine;
//			}
//			else {
//				template += "Related INC/CRQ: None" + Environment.NewLine + Environment.NewLine;
//			}
//			template += "Active Alarms:" + Environment.NewLine;
//			template += textBox7.Text + Environment.NewLine + Environment.NewLine;
//			template += "Alarm History:" + Environment.NewLine;
//			if (string.IsNullOrEmpty(textBox8.Text)) {
//				template += "None related" + Environment.NewLine + Environment.NewLine;
//			}
//			else {
//				template += textBox8.Text + Environment.NewLine + Environment.NewLine;
//			}
//			template += "Troubleshoot:" + Environment.NewLine;
//			template += textBox9.Text;
//			comboBox1.Text = comboBox1.Text.Substring(0,2);
//
//			string[] name = Toolbox.Tools.GetUserDetails("Name").Split(' ');
//			string dept = Toolbox.Tools.GetUserDetails("Department");
//			dept = dept.Contains("2nd Line RAN") ? "2nd Line RAN Support" : "1st Line RAN Support";
//
//			template += Environment.NewLine + Environment.NewLine + name[1].Replace(",",string.Empty) + " " + name[0].Replace(",",string.Empty) + Environment.NewLine + dept + Environment.NewLine + "ANOC Number: +44 163 569 206";
//			template += dept == "1st Line RAN Support" ? "7" : "9";
//			return template;
//		}

//		void TSTChanged(object sender, EventArgs e)
//		{
//			if(siteFinder_mainswitch && sender.GetHashCode() == textBox2.GetHashCode()) {
//				siteFinder_Toggle(false, false, "textBox2");
//				textBox42.Text = string.Empty;
//			}
//
//			if(button44.Enabled) {
//				if(sender is TextBox || sender is RichTextBox) {
//					TextBoxBase tb = (TextBoxBase)sender;
//					int controlNum = Convert.ToInt16(tb.Name.Substring(tb.Name.Length - 1,1));
//					switch(controlNum) {
//						case 1: case 2: case 3: case 4: case 5:
//							if(tb.Text != prevTemp[controlNum - 1])
//								button44.Enabled = false;
//							break;
//						default:
//							if(tb.Text != prevTemp[controlNum + 5])
//								button44.Enabled = false;
//							break;
//					}
//				}
//				else if(sender is CheckBox) {
//					CheckBox chkb = (CheckBox)sender;
//					int controlNum = Convert.ToInt16(chkb.Name.Remove(0,"checkBox".Length));
//					switch(controlNum) {
//						case 1:
//							button44.Enabled = chkb.Checked.ToString() == prevTemp[5];
//							break;
//						case 2:
//							for(int c=1;c<4;c++){
//								if(tabPage8.Controls["numericUpDown" + c].Text != prevTemp[c + 5]) {
//									button44.Enabled = false;
//									break;
//								}
//							}
//							break;
//						case 18:
//							button44.Enabled = chkb.Checked.ToString() == prevTemp[9];
//							break;
//						default:
//							if(chkb.Checked.ToString() != prevTemp[controlNum + 7])
//								button44.Enabled = false;
//							break;
//					}
//				}
//				else if(sender is ComboBox) {
//					ComboBox cmbb = (ComboBox)sender;
//					if(textBox3.Text != prevTemp[2])
//						button44.Enabled = false;
//				}
//				else if(sender is NumericUpDown) {
//					NumericUpDown nud = (NumericUpDown)sender;
//					int controlNum = Convert.ToInt16(nud.Name.Substring(nud.Name.Length - 1,1));
//					if(nud.Text != prevTemp[controlNum + 5])
//						button44.Enabled = false;
//				}
//			}
//		}

//		void Button44Click(object sender, EventArgs e)
//		{
//			Templates.UI.SendBCP bcp = new Templates.UI.SendBCP(textBox2.Text,buildTST());
//			bcp.ShowDialog();
//		}

		void Button46Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox10.Text)) {
				Action action = new Action(delegate {
				                           	MessageBox.Show("Please insert sites list.\n\nTIP: write 1 site PER LINE", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
				                           });
				Toolbox.Tools.darkenBackgroundForm(action,false,this);
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
			                           	if (string.IsNullOrEmpty(textBox10.Text)) {
			                           		MessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			                           					MessageBox.Show("WARNING!!!" + Environment.NewLine + Environment.NewLine + "One or more tables to parse have been removed." + Environment.NewLine + "All tables must have the same columns and order.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			                           			}
			                           		}
			                           		catch {
			                           			trayIcon.showBalloon("Error parsing alarms","An error occurred while parsing the alarms.\nMake sure you're pasting alarms from Netcool");
			                           			return;
			                           		}
			                           	}
			                           	else {
			                           		string[] outageSites = (VFbulkCI + TFbulkCI).Split(';');
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
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,true,this);
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
			                           	
			                           	MessageBox.Show("The following site list was copied to the Clipboard:" + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine,temp) + Environment.NewLine + Environment.NewLine + "This list can be used to enter a bulk site search on Site Lopedia.","List generated",MessageBoxButtons.OK,MessageBoxIcon.Information);
			                           	Clipboard.SetText(string.Join(Environment.NewLine,temp));
			                           });
			Tools.darkenBackgroundForm(action,false,this);
		}


		void Button6Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	if (string.IsNullOrEmpty(textBox12.Text)) {
			                           		MessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
			                           		return;
			                           	}
			                           	try {
			                           		Netcool.Parser netcool = new Netcool.Parser(textBox12.Text);
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

//		void Button7Click(object sender, EventArgs e)
//		{
//			Action action = new Action(delegate {
//			                           	FormCollection fc = Application.OpenForms;
//
//			                           	foreach (Form frm in fc)
//			                           	{
//			                           		if (frm.Name == "TasksForm") {
//			                           			frm.Activate();
//			                           			DialogResult ans = MessageBox.Show("Task Notes Generator is already open, in order to open the requested Task Notes Generator, the previous must be closed.\n\nDo you want to close?","Task Notes Generator",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
//			                           			if (ans == DialogResult.Yes) frm.Close();
//			                           			else return;
//			                           			break;
//			                           		}
//			                           	}
//
//			                           	string errmsg = "";
//			                           	if (string.IsNullOrEmpty(textBox1.Text)) {
//			                           		errmsg += "         - INC/Ticket Number missing\n";
//			                           	}
//			                           	if (string.IsNullOrEmpty(textBox2.Text)) {
//			                           		errmsg += "         - Site ID missing\n";
//			                           	}
//			                           	if (comboBox1.SelectedIndex == 1 && string.IsNullOrEmpty(textBox3.Text)) {
//			                           		errmsg += "          - TF Site ID missing\n";
//			                           	}
//			                           	if (string.IsNullOrEmpty(textBox4.Text)) {
//			                           		errmsg += "         - Site Address missing\n";
//			                           	}
//			                           	if (!string.IsNullOrEmpty(errmsg)) {
//			                           		MessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
//			                           		return;
//			                           	}
//			                           	Templates.UI.TasksForm Tasks = new Templates.UI.TasksForm();
//			                           	Templates.UI.TasksForm.siteID = textBox2.Text;
//			                           	Templates.UI.TasksForm.siteAddress = textBox4.Text;
//			                           	Templates.UI.TasksForm.powerCompany = textBox42.Text;
//			                           	Templates.UI.TasksForm.cct = textBox5.Text;
//			                           	Templates.UI.TasksForm.siteTEF = textBox3.Text;
//			                           	Templates.UI.TasksForm.relatedINC = textBox6.Text;
//			                           	Tasks.StartPosition = FormStartPosition.CenterParent;
//			                           	Tasks.Show();
//			                           });
//			Toolbox.Tools.darkenBackgroundForm(action,true,this);
//		}

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
					string CompINC_CRQ = Toolbox.Tools.CompleteINC_CRQ_TAS(textBox13.Text, "INC");
					if (CompINC_CRQ != "error") textBox13.Text = CompINC_CRQ;
					else {
						Action action = new Action(delegate {
						                           	MessageBox.Show("INC number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
						                           });
						Toolbox.Tools.darkenBackgroundForm(action,false,this);
						return;
					}
				}
			}
		}

		void TabPage1MouseClick(object sender, MouseEventArgs e)
		{
			wholeShiftsPanelDispose();
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
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
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
					                           	MessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
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
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void RadioButton1CheckedChanged(object sender, EventArgs e)
		{
			richTextBox6.Text = string.Empty;
			richTextBox7.Text = string.Empty;
			richTextBox8.Text = string.Empty;
			textBox28.Enabled = true;
			comboBox4.Enabled = true;
			TextBox28TextChanged(null, null);
		}

		void RadioButton2CheckedChanged(object sender, EventArgs e)
		{
			richTextBox6.Text = string.Empty;
			richTextBox7.Text = string.Empty;
			richTextBox8.Text = string.Empty;
			textBox28.Enabled = true;
			comboBox4.Enabled = true;
			TextBox28TextChanged(null, null);
		}

		void RadioButton3CheckedChanged(object sender, EventArgs e)
		{
			richTextBox6.Text = string.Empty;
			richTextBox7.Text = string.Empty;
			richTextBox8.Text = string.Empty;
			textBox28.Enabled = true;
			comboBox4.Enabled = false;
			TextBox28TextChanged(null, null);
		}

		void TextBox29Click(object sender, EventArgs e)
		{
			textBox29.SelectAll();
		}

		void TextBox10TextChanged(object sender, EventArgs e)
		{
			if (textBox10.Text != string.Empty) button22.Enabled = true;
			else button22.Enabled = false;
		}

		void Button22Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(textBox10.Text,label33.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox10.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void TextBox11TextChanged(object sender, EventArgs e)
		{
			if (textBox11.Text != string.Empty) button23.Enabled = true;
			else button23.Enabled = false;
		}

		void Button23Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(textBox11.Text,label32.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox11.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void TextBox12TextChanged(object sender, EventArgs e)
		{
			if (textBox12.Text != string.Empty) button24.Enabled = true;
			else button24.Enabled = false;
		}

		void Button24Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(textBox12.Text,label34.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox12.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void RichTextBox6TextChanged(object sender, EventArgs e)
		{
			if (richTextBox6.Text != string.Empty) {
				button26.Enabled = true;
				button29.Enabled = true;
			}
			else {
				button26.Enabled = false;
				button29.Enabled = false;
			}
			richTextBox7.Text = string.Empty;
			richTextBox8.Text = string.Empty;
		}

		void Button26Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox6.Text,label44.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox6.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void RichTextBox7TextChanged(object sender, EventArgs e)
		{
			if (richTextBox7.Text != string.Empty) {
				button27.Enabled = true;
				button30.Visible = true;
			}
			else {
				button27.Enabled = false;
				button30.Visible = false;
			}
		}

		void Button27Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox7.Text,label45.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox7.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void RichTextBox8TextChanged(object sender, EventArgs e)
		{
			if (richTextBox8.Text != string.Empty) {
				button28.Enabled = true;
				button31.Visible = true;
			}
			else {
				button28.Enabled = false;
				button31.Visible = false;
			}
		}

		void Button28Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox8.Text,label46.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox8.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void TextBox28TextChanged(object sender, EventArgs e)
		{
			if (textBox28.Text != string.Empty) {
				int radioch = 0;
				foreach (Control radio in groupBox4.Controls) {
					RadioButton rb = radio as RadioButton;
					if (rb.Checked) break;
					radioch++;
				}
				switch (radioch) {
					case 0:
						textBox29.Text = "LST GCELL:IDTYPE=BYNAME,BTSNAME=" + '"' + "GSM" + textBox28.Text + comboBox4.Text + '"' + ";";
						break;
					case 1:
						string site = textBox28.Text;
						while (site.Length < 5) {
							site = '0' + site;
						}
						textBox29.Text = "DSP UCELL:DSPT=BYNODEB,NODEBNAME=" + '"' + "UMTS" + site + comboBox4.Text + '"' + ";";
						break;
					case 2:
						textBox29.Text = "LST CELL:;";
						break;
				}
				richTextBox6.Enabled = true;
			}
			else {
				textBox29.Text = string.Empty;
				richTextBox6.Enabled = false;
				comboBox4.SelectedIndex = 0;
			}
			richTextBox6.Text = string.Empty;
			richTextBox7.Text = string.Empty;
			richTextBox8.Text = string.Empty;
		}

		void Button29Click(object sender, EventArgs e)
		{
			int radioch = 0;
			foreach (Control radio in groupBox4.Controls) {
				RadioButton rb = radio as RadioButton;
				if (rb.Checked) break;
				radioch++;
			}
			
			richTextBox7.Text = string.Empty;
			richTextBox8.Text = string.Empty;
			string[] cells = richTextBox6.Text.Split('\n');
			switch (radioch) {
				case 0:
					if (richTextBox6.Text.Contains("GSM")) {
						foreach (string row in cells) {
							if (!string.IsNullOrEmpty(row)) {
								string cell = string.Empty;
								string row2 = row.TrimStart(' ');
								foreach (char ch in row2) {
									if (!Char.IsDigit(ch)) break;
									cell += ch;
								}
								if (!string.IsNullOrEmpty(cell)) {
									richTextBox7.Text += "SET GCELLADMSTAT:IDTYPE=BYID,CELLID=" + cell + ",ADMSTAT=LOCK;\r\n";
									richTextBox8.Text += "SET GCELLADMSTAT:IDTYPE=BYID,CELLID=" + cell + ",ADMSTAT=UNLOCK;\r\n";
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
							richTextBox7.Text += "BLK UCELL:CELLID=" + cell + ",PRIORITY=HIGH;\r\n";
							richTextBox8.Text += "UBL UCELL:CELLID=" + cell + ";\r\n";
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

							richTextBox7.Text += "BLK CELL:LOCALCELLID=" + cell + ",CELLADMINSTATE=CELL_HIGH_BLOCK;\r\n";
							richTextBox8.Text += "UBL CELL:LOCALCELLID=" + cell + ";\r\n";
						}
					}
					//}
					break;
			}
			if (!string.IsNullOrEmpty(richTextBox7.Text)) richTextBox7.Text = richTextBox7.Text.Remove(richTextBox7.Text.Length - 1,1);
			if (!string.IsNullOrEmpty(richTextBox8.Text)) richTextBox8.Text = richTextBox8.Text.Remove(richTextBox8.Text.Length - 1,1);
		}

		void Button30Click(object sender, EventArgs e)
		{
			Clipboard.SetText(richTextBox7.Text);
		}

		void Button31Click(object sender, EventArgs e)
		{
			Clipboard.SetText(richTextBox8.Text);
		}

		void RadioButton4CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton4.Checked) {
				label50.Text = "Lock site";
				label49.Text = "Unlock site";
				label47.Text = "List BTS's";
				label48.Text = "BCF";
				groupBox6.Text = "Choose BTSs";
				richTextBox9.Visible = false;
				button37.Visible = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null)	cb.Visible = true;
					else {
						TextBox tb = control as TextBox;
						if (tb != null) tb.Visible = true;
					}
				}
			}
			richTextBox9.Text = string.Empty;
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
			textBox31.Enabled = true;
			textBox31.Text = string.Empty;
			textBox30.Text = string.Empty;
			
			TextBox31TextChanged(null, null);
		}

		void RadioButton5CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton5.Checked) {
				label50.Text = "Lock D-CHANNELs";
				label49.Text = "Unlock D-CHANNELs";
				label47.Text = "List D-CHANNELs";
				label48.Text = "PCM";
				groupBox6.Text = "Paste D-CHANNELs printout";
				richTextBox9.Visible = true;
				button37.Visible = true;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null)	cb.Visible = false;
					else {
						TextBox tb = control as TextBox;
						if (tb != null) tb.Visible = false;
					}
				}
			}
			richTextBox9.Text = string.Empty;
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
			textBox31.Enabled = true;
			textBox31.Text = string.Empty;
			textBox30.Text = string.Empty;
			
			TextBox31TextChanged(null, null);
		}

		void TextBox31TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox31.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox31.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BCF can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				int radioch = 0;
				foreach (Control radio in groupBox5.Controls) {
					RadioButton rb = radio as RadioButton;
					if (rb.Checked) break;
					radioch++;
				}
				
				switch (radioch) {
					case 0:
						textBox30.Text = "ZEEI:BCF=" + textBox31.Text + ";";
						foreach (Control control in groupBox6.Controls)
						{
							CheckBox cb = control as CheckBox;
							if (cb != null)
								cb.Enabled = true;
						}
						textBox32.Text = Convert.ToInt32(textBox31.Text).ToString();
						break;
					case 1:
						textBox30.Text = "ZDTI:::PCM=" + textBox31.Text + ";";
						richTextBox9.Enabled = true;
						break;
				}
			}
			else {
				textBox30.Text = string.Empty;
				richTextBox9.Enabled = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						cb.Enabled = false;
					}
					else {
						TextBox tb = control as TextBox;
						if (tb != null) tb.Text = string.Empty;
					}
				}
			}
			richTextBox9.Text = string.Empty;
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
			foreach (Control control in groupBox6.Controls)	{
				CheckBox cb = control as CheckBox;
				if (cb != null)
					cb.Checked = false;
			}
		}

		void CheckBox8CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox8.Checked) {
				textBox32.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox32.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox9CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox9.Checked) {
				textBox33.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox33.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox10CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox10.Checked) {
				textBox34.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox34.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox11CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox11.Checked) {
				textBox35.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox35.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox12CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox12.Checked) {
				textBox36.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox36.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox13CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox13.Checked) {
				textBox37.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox37.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox14CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox14.Checked) {
				textBox38.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox38.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox15CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox15.Checked) {
				textBox39.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox39.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox16CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox16.Checked) {
				textBox40.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox40.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void CheckBox17CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox17.Checked) {
				textBox41.Enabled = true;
				button34.Enabled = true;
			}
			else {
				textBox41.Enabled = false;
				bool anych = false;
				foreach (Control control in groupBox6.Controls)
				{
					CheckBox cb = control as CheckBox;
					if (cb != null) {
						if (cb.Checked) {
							anych = true;
							break;
						}
					}
				}
				if (!anych) button34.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox32TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox32.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox32.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox33.Text = (Convert.ToInt32(textBox32.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox33TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox33.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox33.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox34.Text = (Convert.ToInt32(textBox33.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox34TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox34.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox34.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox35.Text = (Convert.ToInt32(textBox34.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox35TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox35.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox35.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox36.Text = (Convert.ToInt32(textBox35.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox36TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox32.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox36.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox37.Text = (Convert.ToInt32(textBox36.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox37TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox37.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox37.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox38.Text = (Convert.ToInt32(textBox37.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox38TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox38.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox38.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox39.Text = (Convert.ToInt32(textBox38.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox39TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox39.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox39.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox40.Text = (Convert.ToInt32(textBox39.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void TextBox40TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBox40.Text)) {
				if (!Toolbox.Tools.IsAllDigits(textBox40.Text)) {
					Action action = new Action(delegate {
					                           	MessageBox.Show("BTS can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					return;
				}
				textBox41.Text = (Convert.ToInt32(textBox40.Text) + 1).ToString();
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void Button34Click(object sender, EventArgs e)
		{
			int radioch = 0;
			foreach (Control radio in groupBox5.Controls) {
				RadioButton rb = radio as RadioButton;
				if (rb.Checked) break;
				radioch++;
			}
			switch (radioch) {
				case 0: // NSN BCF
					foreach (Control control in groupBox6.Controls)
					{
						CheckBox cb = control as CheckBox;
						if (cb != null) {
							if (cb.Checked) {
								int cbnum;
								if (Toolbox.Tools.IsAllDigits(cb.Name.Substring(cb.Name.Length - 2,2)))
									cbnum = Convert.ToInt32(cb.Name.Substring(cb.Name.Length - 2,2));
								else
									cbnum = Convert.ToInt32(cb.Name.Substring(cb.Name.Length - 1,1));
								TextBox tb = (TextBox)groupBox6.Controls["textBox" + (cbnum + 24).ToString()];
								richTextBox11.Text += "ZEQS:BTS=" + tb.Text + ":L:FHO,30;\r\n";
								richTextBox10.Text += "ZEQS:BTS=" + tb.Text + ":U;\r\n";
							}
						}
					}
					richTextBox11.Text += "\r\nZEFS:" + textBox31.Text + ":L;";
					richTextBox10.Text += "\r\nZEFS:" + textBox31.Text + ":U;";
					break;
				case 1: // NSN PCM
					string [] temp = richTextBox9.Text.Split('\n');
					int a = 0;
					foreach (string row in temp) {
						if (row.Contains("WO-") || row.Contains("BL-") || row.Contains("UA-")) {
							string DCH = string.Empty;
							foreach (char ch in row) {
								if (ch != ' ') DCH += ch;
								else break;
							}
							richTextBox10.Text += "ZDTC:" + DCH + ":WO;\r\n";
							richTextBox11.Text += "ZDTC:" + DCH + ":BL;\r\n";
						}
						a++;
					}
					richTextBox10.Text = richTextBox10.Text.Substring(0,richTextBox10.Text.Length - 1);
					richTextBox11.Text = richTextBox11.Text.Substring(0,richTextBox11.Text.Length - 1);
					break;
			}
		}

		void RichTextBox9TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(richTextBox9.Text)) {
				button34.Enabled = true;
				button37.Enabled = true;
			}
			else {
				button34.Enabled = false;
				button37.Enabled = false;
			}
			richTextBox10.Text = string.Empty;
			richTextBox11.Text = string.Empty;
		}

		void Button37Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox9.Text,groupBox6.Text,false);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox9.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void RichTextBox11TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(richTextBox11.Text)) {
				button36.Enabled = true;
				button33.Visible = true;
			}
			else {
				button36.Enabled = false;
				button33.Visible = false;
			}
		}

		void Button36Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox11.Text,label50.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox11.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void RichTextBox10TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(richTextBox10.Text)) {
				button35.Enabled = true;
				button32.Visible = true;
			}
			else {
				button35.Enabled = false;
				button32.Visible = false;
			}
		}

		void Button35Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox10.Text,label49.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox10.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void Button32Click(object sender, EventArgs e)
		{
			Clipboard.SetText(richTextBox10.Text);
		}

		void Button33Click(object sender, EventArgs e)
		{
			Clipboard.SetText(richTextBox11.Text);
		}

		void ComboBox4SelectedIndexChanged(object sender, EventArgs e)
		{
			TextBox28TextChanged(null, null);
		}

		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			wholeShiftsPanelDispose();
			DialogResult ans = DialogResult.No;
			Action action = new Action(delegate {
			                           	ans = MessageBox.Show("Are you sure you want to quit ANOC Master Tool?","Quitting",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
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

//		public static void showBalloon(string title, string body)
//		{
//			if (title != null)
//				trayIcon.BalloonTipTitle = title;
//
//			if (body != null)
//				trayIcon.BalloonTipText = body;
//
//			trayIcon.ShowBalloonTip(10000);
//		}

//		void Button43Click(object sender, EventArgs e)
//		{
//			DataRowView site = null;
//			DataView cells = null;
//			if(!string.IsNullOrEmpty(textBox2.Text)) {
//				site = findSite(textBox2.Text);
//				cells = findCells(textBox2.Text);
//			}
//			else
//				return;
//
//			FormCollection fc = Application.OpenForms;
//
//			foreach (Form frm in fc)
//			{
//				if (frm.Name == "siteDetails") {
//					frm.Close();
//					break;
//				}
//			}
//
//			siteDetails sd = new siteDetails(site,cells);
//			sd.Show();
//		}

		void TextBox50TextChanged(object sender, EventArgs e)
		{
			if(GlobalProperties.siteFinder_mainswitch)
				siteFinder_Toggle(false, false, "textBox50");
		}

//		void CheckBox1CheckedChanged(object sender, EventArgs e)
//		{
//			if(checkBox1.Checked) {
//				for(int c=1;c<4;c++) {
//					NumericUpDown nupd = (NumericUpDown)tabPage8.Controls["numericUpDown" + c];
//					nupd.Maximum = 9999;
//					nupd.Enabled = true;
//				}
//			}
//			else {
//				for(int c=1;c<4;c++) {
//					NumericUpDown nupd = (NumericUpDown)tabPage8.Controls["numericUpDown" + c];
//					string max;
//					string temp = tabPage8.Controls["label" + (c + 14)].Text.Split('(')[1];
//					max = temp.Substring(0,temp.Length - 1);
//					if(max == "0")
//						nupd.Enabled = false;
//					else {
//						nupd.Maximum = Convert.ToInt16(max);
//						nupd.Enabled = true;
//					}
//				}
//			}
//			TSTChanged(sender,e);
//		}

//		void Label15DoubleClick(object sender, EventArgs e)
//		{
//			if(numericUpDown1.Enabled) {
//				int max = Convert.ToInt16(label15.Text.Split('(')[1].Replace(")",string.Empty));
//				if(numericUpDown1.Value < max)
//					numericUpDown1.Value = max;
//				else {
//					if(numericUpDown1.Value == max)
//						numericUpDown1.Value = 0;
//				}
//			}
//		}
//
//		void Label16DoubleClick(object sender, EventArgs e)
//		{
//			if(numericUpDown2.Enabled) {
//				int max = Convert.ToInt16(label16.Text.Split('(')[1].Replace(")",string.Empty));
//				if(numericUpDown2.Value < max)
//					numericUpDown2.Value = max;
//				else {
//					if(numericUpDown2.Value == max)
//						numericUpDown2.Value = 0;
//				}
//			}
//		}
//
//		void Label17DoubleClick(object sender, EventArgs e)
//		{
//			if(numericUpDown3.Enabled) {
//				int max = Convert.ToInt16(label17.Text.Split('(')[1].Replace(")",string.Empty));
//				if(numericUpDown3.Value < max)
//					numericUpDown3.Value = max;
//				else {
//					if(numericUpDown3.Value == max)
//						numericUpDown3.Value = 0;
//				}
//			}
//		}
		
//		void CheckBox18CheckedChanged(object sender, EventArgs e) {
//			if(!checkBox1.Checked) {
//				if(checkBox18.Checked) {
//					if(siteFinder_mainswitch) {
//						for(int c=1;c<4;c++) {
//							NumericUpDown nupd = (NumericUpDown)tabPage8.Controls["numericUpDown" + c];
//							if (nupd.Maximum < 999) {
//								nupd.Value = nupd.Maximum;
//								nupd.Enabled = false;
//							}
//						}
//					}
//				}
//				else {
//					for(int c=1;c<4;c++) {
//						NumericUpDown nupd = (NumericUpDown)tabPage8.Controls["numericUpDown" + c];
//						nupd.Value = 0;
//						if (nupd.Maximum < 999) {
//							nupd.Value = 0;
//							nupd.Enabled = true;
//						}
//					}
//				}
//			}
//			TSTChanged(sender,e);
//		}
		
//		void NumericUpDownValueChanged(object sender, EventArgs e)
//		{
//			int nupdTotal = 0;
//			int nupdMaxed = 0;
//			for(int c = 1;c<4;c++) {
//				NumericUpDown nupd = (NumericUpDown)tabPage8.Controls["numericUpDown" + c];
//				if(nupd.Maximum < 999) {
//					nupdTotal++;
//					if(nupd.Value == nupd.Maximum)
//						nupdMaxed++;
//				}
//			}
//			if(nupdMaxed == nupdTotal)
//				if(!checkBox18.Checked)
//					checkBox18.Checked = true;
//			TSTChanged(sender, e);
//		}
		
		public static void shiftsPanelPaint(object sender, PaintEventArgs e)
		{
			MainForm.shiftsPanel.Invalidate(true);
//			shiftsSnap.Save(UserFolderPath + @"\bmp.png");
			MainForm.shiftsPanel.Size = new Size(MainForm.shiftsBodySnap.Width, MainForm.shiftsBodySnap.Height);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.DrawImageUnscaled(MainForm.shiftsBodySnap, Point.Empty);
		}
		
		void RadioButton6CheckedChanged(object sender, EventArgs e)
		{
			if(radioButton6.Checked) {
				string bearer;
				string cellName;
				string cellID;
				string lactac;
				string Switch;
				string noc;
				
				eScriptCellsGlobal.RowFilter = "BEARER = '2G' AND VENDOR LIKE 'ERIC*'";
				foreach(DataRowView dr in eScriptCellsGlobal) {
					bearer = dr[dr.Row.Table.Columns.IndexOf("BEARER")].ToString();
					cellName = dr[dr.Row.Table.Columns.IndexOf("CELL_NAME")].ToString();
					cellID = dr[dr.Row.Table.Columns.IndexOf("CELL_ID")].ToString();
					lactac = dr[dr.Row.Table.Columns.IndexOf("LAC_TAC")].ToString();
					Switch = dr[dr.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString();
					noc = dr[dr.Row.Table.Columns.IndexOf("NOC")].ToString();
					
					listView2.Items.Add(new ListViewItem(new[]{bearer, cellName, cellID, lactac, Switch, noc}));
				}
				foreach (ColumnHeader col in listView2.Columns)
					col.Width = -2;
				
				eScriptCellsGlobal.RowFilter = string.Empty;
			}
			else
				listView2.Items.Clear();
		}
		
		void RadioButton7CheckedChanged(object sender, EventArgs e)
		{
			if(radioButton7.Checked) {
				string bearer;
				string cellName;
				string cellID;
				string lactac;
				string Switch;
				string noc;
				
				eScriptCellsGlobal.RowFilter = "BEARER = '3G' AND VENDOR LIKE 'ERIC*'";
				foreach(DataRowView dr in eScriptCellsGlobal) {
					bearer = dr[dr.Row.Table.Columns.IndexOf("BEARER")].ToString();
					cellName = dr[dr.Row.Table.Columns.IndexOf("CELL_NAME")].ToString();
					cellID = dr[dr.Row.Table.Columns.IndexOf("CELL_ID")].ToString();
					lactac = dr[dr.Row.Table.Columns.IndexOf("LAC_TAC")].ToString();
					Switch = dr[dr.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString();
					noc = dr[dr.Row.Table.Columns.IndexOf("NOC")].ToString();
					
					listView2.Items.Add(new ListViewItem(new[]{bearer, cellName,cellID,lactac, Switch, noc}));
				}
				foreach (ColumnHeader col in listView2.Columns)
					col.Width = -2;
				
				eScriptCellsGlobal.RowFilter = string.Empty;
				label68.Text += "\nIt\'s not possible to lock 3G cells via amos on site, log on to the RNC " + listView2.Items[0].SubItems[4].Text + " and follow the instructions";
				label68.Visible = true;
			}
			else {
				label68.Visible = false;
				label68.Text = "WARNING:";
				listView2.Items.Clear();
			}
		}
		
		void RadioButton8CheckedChanged(object sender, EventArgs e)
		{
			if(radioButton8.Checked) {
				string bearer;
				string cellName;
				string cellID;
				string lactac;
				string Switch;
				string noc;
				
				eScriptCellsGlobal.RowFilter = "BEARER = '4G' AND VENDOR LIKE 'ERIC*'";
				foreach(DataRowView dr in eScriptCellsGlobal) {
					bearer = dr[dr.Row.Table.Columns.IndexOf("BEARER")].ToString();
					cellName = dr[dr.Row.Table.Columns.IndexOf("CELL_NAME")].ToString();
					cellID = dr[dr.Row.Table.Columns.IndexOf("CELL_ID")].ToString();
					lactac = dr[dr.Row.Table.Columns.IndexOf("LAC_TAC")].ToString();
					Switch = dr[dr.Row.Table.Columns.IndexOf("BSC_RNC_ID")].ToString();
					noc = dr[dr.Row.Table.Columns.IndexOf("NOC")].ToString();
					
					listView2.Items.Add(new ListViewItem(new[]{bearer, cellName,cellID,lactac, Switch, noc}));
				}
				foreach (ColumnHeader col in listView2.Columns)
					col.Width = -2;
				
				eScriptCellsGlobal.RowFilter = string.Empty;
			}
			else
				listView2.Items.Clear();
		}
		
		void ListView2ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			richTextBox14.Text = string.Empty;
			richTextBox15.Text = string.Empty;
			button52.Enabled = listView2.CheckedItems.Count != listView2.Items.Count;
			if(listView2.CheckedItems.Count > 0) {
				button53.Enabled = true;
				button49.Enabled = true;
			}
			else {
				button53.Enabled = false;
				button49.Enabled = false;
			}
		}
		
		void Button52Click(object sender, EventArgs e)
		{
			if(listView2.CheckedItems.Count < listView2.Items.Count) {
				foreach (ListViewItem cell in listView2.Items)
					cell.Checked = true;
			}
		}
		
		void Button53Click(object sender, EventArgs e)
		{
			if(listView2.CheckedItems.Count > 0) {
				foreach (ListViewItem cell in listView2.Items)
					cell.Checked = false;
			}
		}

		void Button49Click(object sender, EventArgs e)
		{
			int radioch = 0;
			foreach (Control radio in groupBox7.Controls) {
				RadioButton rb = radio as RadioButton;
				if (rb.Checked) break;
				radioch++;
			}
			string cellLock = string.Empty;
			string cellUnlock = string.Empty;
			switch(radioch) {
				case 0:
					for(int c = 0;c < listView2.CheckedItems.Count;c++) {
						cellLock += "RLSTC:CELL=" + listView2.CheckedItems[c].SubItems[1].Text + ",STATE=HALTED;";
						cellUnlock += "RLSTC:CELL=" + listView2.CheckedItems[c].SubItems[1].Text + ",STATE=ACTIVE;";
						if(c != listView2.CheckedItems.Count - 1) {
							cellLock += Environment.NewLine;
							cellUnlock += Environment.NewLine;
						}
					}
					
//					foreach(ListViewItem cell in listView2.CheckedItems) {
//						cellLock += "RLSTC:CELL=" + cell.SubItems[1].Text + ",STATE=HALTED;" + Environment.NewLine;
//						cellUnlock += "RLSTC:CELL=" + cell.SubItems[1].Text + ",STATE=ACTIVE;" + Environment.NewLine;
//					}
					break;
				case 1:
					foreach(ListViewItem cell in listView2.CheckedItems) {
						cellLock += "bl UtranCell=" + cell.SubItems[2].Text + Environment.NewLine;
						cellUnlock += "deb UtranCell=" + cell.SubItems[2].Text + Environment.NewLine;
					}
					break;
				case 2:
					foreach(ListViewItem cell in listView2.CheckedItems) {
						cellLock += "bl ENodeBFunction=1,EUtranCellFDD=" + cell.SubItems[1].Text + Environment.NewLine;
						cellUnlock += "deb ENodeBFunction=1,EUtranCellFDD=" + cell.SubItems[1].Text + Environment.NewLine;
					}
					break;
			}
			richTextBox14.Text = cellUnlock;
			richTextBox15.Text = cellLock;
		}

		void RichTextBox14TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(richTextBox14.Text)) {
				button50.Enabled = true;
				button47.Visible = true;
			}
			else {
				button50.Enabled = false;
				button47.Visible = false;
			}
		}

		void RichTextBox15TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(richTextBox15.Text)) {
				button51.Enabled = true;
				button48.Visible = true;
			}
			else {
				button51.Enabled = false;
				button48.Visible = false;
			}
		}

		void Button50Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox14.Text,label62.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox14.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void Button51Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	UI.LargeTextForm enlarge = new UI.LargeTextForm(richTextBox15.Text,label63.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox15.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}

		void Button47Click(object sender, EventArgs e)
		{
			Clipboard.SetText(richTextBox14.Text);
		}

		void Button48Click(object sender, EventArgs e)
		{
			Clipboard.SetText(richTextBox15.Text);
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
			                           	if(CurrentUser.GetUserDetails("Username") == "GONCARJ3") {
			                           		Web.UI.BrowserView brwsr = new Web.UI.BrowserView();
			                           		brwsr.StartPosition = FormStartPosition.CenterParent;
			                           		brwsr.ShowDialog();
			                           	}
			                           	else {
			                           		BrowserView brwsr = new BrowserView();
			                           		brwsr.StartPosition = FormStartPosition.CenterParent;
			                           		brwsr.ShowDialog();
			                           	}
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
		
		public static void toggleShiftsPanel() {
			// FIXME: UI glitch on shiftsPanel objects
			// FIXME: If shiftFile doesn't exist and share access is denied, app crashes
			if(shiftsPanel.Location.Y == 0) {
				if(wholeShiftsPanel.Parent != null) {
					wholeShiftsPanel.Dispose();
					shiftsPanel.Invalidate(true);
				}
				Transition t = new Transition(new TransitionType_EaseInEaseOut(500));
				t.add(shiftsPanel, "Top", 0 - shiftsPanel.Height);
				t.run();
			}
			else {
				Transition t = new Transition(new TransitionType_EaseInEaseOut(500));
				t.add(shiftsPanel, "Top", 0);
				t.run();
			}
		}
		
		void PictureBoxesClick(object sender, EventArgs e) {
			PictureBox pic = (PictureBox)sender;
			switch(pic.Name) {
				case "pictureBox1":
					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					Action action = new Action(delegate {
					                           	openSettings();
					                           });
					Tools.darkenBackgroundForm(action,false,this);
					break;
				case "pictureBox2":
					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openAMTBrowser();
					break;
				case "pictureBox3":
					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openNotes();
					break;
				case "pictureBox4":
					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openLogBrowser();
					break;
				case "pictureBox5":
					wholeShiftsPanelDispose();
					MainFormActivate(null,null);
					openSiteFinder();
					break;
				case "pictureBox6":
					toggleShiftsPanel();
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
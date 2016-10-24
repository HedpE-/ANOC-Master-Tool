/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 20/03/2015
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using appCore.Templates;
using appCore.Templates.UI;
using appCore.Templates.Types;

namespace appCore.Logs.UI
{
	/// <summary>
	/// Description of LogEditor.
	/// </summary>
	public sealed partial class LogEditor2 : Form
	{
		public string[] globalLogs;
		public string GlobalLogType;
		List<string> LTEsites = new List<string>();
		public static string VFoutage;
		public static string TFoutage;
		public static string VFbulkCI;
		public static string TFbulkCI;
		public static LogsCollection<Template> Logs;
		public static TroubleshootControls TroubleshootUI;
		public static FailedCRQControls FailedCRQUI;
		public static UpdateControls UpdateUI;
		public static TXControls TXUI;
		public static OutageControls OutageUI;
		MainForm myFormControl1;
		
//		public static string[] ParseLogs(string LogFile)
//		{
//			string separator = string.Empty;
//			for (int c = 1; c < 301; c++) {
//				if (c == 151) separator += "\r\n";
//				separator += "*";
//			}
//			string[] strTofind = { "\r\n" + separator + "\r\n" }; // build logs separator
//
//			string[] temp = File.ReadAllText(LogFile).Split(strTofind, StringSplitOptions.None); // parse all logs using previously built separator
//
//			strTofind[0] = strTofind[0].Substring(0,strTofind[0].Length - 2); // remove last line feed for last log removal
//
//			if (temp[temp.Length - 1].Contains(strTofind[0])) temp[temp.Length - 1] = temp[temp.Length - 1].Substring(0, temp[temp.Length - 1].Length - strTofind[0].Length); // remove separator from last log since strTofind needs to change to be removed from last
//
//			return temp;
//		}
		
		public LogEditor2(LogsCollection<Template> logs, string LogType, MainForm myForm)
		{
			InitializeComponent();
			
			myFormControl1 = myForm;
			Logs = logs;
			
			string WindowTitle = Logs.logFileDate.Day.ToString();
			switch (Logs.logFileDate.Day) {
				case 1: case 21: case 31:
					WindowTitle += "st of ";
					break;
				case 2: case 22:
					WindowTitle += "nd of ";
					break;
				case 3: case 23:
					WindowTitle += "rd of ";
					break;
				default:
					WindowTitle += "th of ";
					break;
			}
			WindowTitle += Logs.logFileDate.ToString("MMMM, yyyy",CultureInfo.GetCultureInfo("en-GB"));// + ", " + Logs.logFileDate.Year;
			
			this.Height = 152;
			this.Text = "Log Editor - " + WindowTitle + " - " + LogType + " logs";
			
			GlobalLogType = LogType;
//			Logs = ParseLogs(LogFile); // Parse logs on log file
			////			List<Log> logs = new List<Log>();
//			if (Logs[Logs.Length - 1].Substring(Logs[Logs.Length - 1].Length - 3,3) == "***") {
//				Logs[Logs.Length - 1] = Logs[Logs.Length - 1].Substring(0,Logs[Logs.Length - 1].Length - 304);
//			}
			
			
			listView1.View = View.Details;
			
			switch (LogType) {
				case "Templates":
					// Populate ListView1 with logs

					listView1.Columns.Add("Log Type").Width = -2;
					listView1.Columns.Add("INC").Width = -2;
					listView1.Columns.Add("Target").Width = -2;
					listView1.Columns.Add("Timestamp").Width = -2;
					
					for (int c = 0; c < Logs.Count; c++) {
//						string[] strTofind = { "\r\n" };
//						string[] log = Logs[c].Split(strTofind, StringSplitOptions.None);
//						strTofind[0] = " - ";
//
						switch (Logs[c].LogType) {
							case "Troubleshoot":
								TroubleShoot TSlog = new TroubleShoot();
								Toolbox.Tools.CopyProperties(TSlog, Logs[c]);
								listView1.Items.Add(new ListViewItem(new string[]{"Troubleshoot Template",TSlog.INC,TSlog.SiteId,TSlog.GenerationDate.ToString("HH:mm:ss")}));
								break;
							case "Failed CRQ":
								FailedCRQ FCRQlog = new FailedCRQ();
								Toolbox.Tools.CopyProperties(FCRQlog, Logs[c]);
								listView1.Items.Add(new ListViewItem(new string[]{"Failed CRQ",FCRQlog.INC,FCRQlog.SiteId,FCRQlog.GenerationDate.ToString("HH:mm:ss")}));
								break;
							case "TX":
								TX TXlog = new TX();
								Toolbox.Tools.CopyProperties(TXlog, Logs[c]);
								listView1.Items.Add(new ListViewItem(new string[]{"TX Template","-",TXlog.SiteIDs,TXlog.GenerationDate.ToString("HH:mm:ss")}));
								break;
							case "Update":
								Update UPDlog = new Update();
								Toolbox.Tools.CopyProperties(UPDlog, Logs[c]);
								listView1.Items.Add(new ListViewItem(new string[]{"Update Template",UPDlog.INC,UPDlog.SiteId,UPDlog.GenerationDate.ToString("HH:mm:ss")}));
								break;
						}
					}
					break;
				case "Outages":
					listView1.Columns.Add("Timestamp").Width = -2;
					listView1.Columns.Add("Summary").Width = -2;
					listView1.Columns.Add("2G").Width = -2;
					listView1.Columns.Add("3G").Width = -2;
					listView1.Columns.Add("4G").Width = -2;
					listView1.Columns.Add("Event Time").Width = -2;
					listView1.Columns.Add("VF Report").Width = -2;
					listView1.Columns[listView1.Columns.Count -1].TextAlign = HorizontalAlignment.Center;
					listView1.Columns.Add("TF Report").Width = -2;
					listView1.Columns[listView1.Columns.Count -1].TextAlign = HorizontalAlignment.Center;
					
					for (int c = 0; c < globalLogs.Length; c++) {
						string[] strTofind = { "\r\n" };
						string[] log = globalLogs[c].Split(strTofind, StringSplitOptions.None);
						
						string GSMcells = string.Empty;
						string UMTScells = string.Empty;
						string LTEcells = string.Empty;
						
						ArrayList eventTimeList = new ArrayList();
						CultureInfo culture = new CultureInfo("pt-PT");
						
						strTofind[0] = "G Cells (";
						string eventTime = string.Empty;
						
						// Manipulate log array to make it compatible with VF/TF new logs
						if(Array.FindIndex(log,element => element.Contains("F Report----------")) == -1) {
							List<string> log2 = log.ToList(); // Create new List with log array values
							string Report = log2[1]; // Store outage report to string
							log2.RemoveAt(1); // Remove outage report previously stored on Report string
							string[] SplitReport = Report.Split('\n'); // Split Report string to new array
							log2.Insert(1,"----------VF Report----------"); // Insert VF Report header to match code checks
							log2.InsertRange(2,SplitReport); // Insert SplitReport array into list after header
							log = log2.ToArray(); // Replace original log array with new generated List values
						}
						
						char VFReportExists;
						VFReportExists = Array.FindIndex(log, element => element.Equals("----------VF Report----------")) > -1 ? '\u2714' : '\u2718';

						char TFReportExists;
						TFReportExists = Array.FindIndex(log, element => element.Equals("----------TF Report----------")) > -1 ? '\u2714' : '\u2718';
						
						if(!string.IsNullOrEmpty(Array.Find(log, element => element.StartsWith("2G Cells", StringComparison.Ordinal)))) {
							string[] tmp = Array.Find(log, element => element.StartsWith("2G Cells", StringComparison.Ordinal)).Split(strTofind, StringSplitOptions.None);
							GSMcells = tmp[1].Substring(0, tmp[1].IndexOf(")", StringComparison.Ordinal));
							eventTime  = tmp[1].Substring(tmp[1].IndexOf("- ", StringComparison.Ordinal) + 2,16);
							eventTimeList.Add(eventTime);
						}
						else
							GSMcells = "0";
						
						if(!string.IsNullOrEmpty(Array.Find(log, element => element.StartsWith("3G Cells", StringComparison.Ordinal)))) {
							string[] tmp = Array.Find(log, element => element.StartsWith("3G Cells", StringComparison.Ordinal)).Split(strTofind, StringSplitOptions.None);
							UMTScells = tmp[1].Substring(0, tmp[1].IndexOf(")", StringComparison.Ordinal));
							eventTime  = tmp[1].Substring(tmp[1].IndexOf("- ", StringComparison.Ordinal) + 2,16);
							eventTimeList.Add(eventTime);
						}
						else
							UMTScells = "0";
						
						if(!string.IsNullOrEmpty(Array.Find(log, element => element.StartsWith("4G Cells", StringComparison.Ordinal)))) {
							string[] tmp = Array.Find(log, element => element.StartsWith("4G Cells", StringComparison.Ordinal)).Split(strTofind, StringSplitOptions.None);
							LTEcells = tmp[1].Substring(0, tmp[1].IndexOf(")", StringComparison.Ordinal));
							eventTime = tmp[1].Substring(tmp[1].IndexOf("- ", StringComparison.Ordinal) + 2,16);
							eventTimeList.Add(eventTime);
						}
						else
							LTEcells = "0";
						
						eventTimeList.Sort();
						
						if(eventTimeList.Count == 0)
							eventTimeList.Add("-"); // Adiciona 1
						
						listView1.Items.Add(new ListViewItem(new string[]{log[0], log[2], GSMcells, UMTScells, LTEcells, eventTimeList[0].ToString(), VFReportExists.ToString(), TFReportExists.ToString()}));
					}
					break;
			}
			
		}
		
		public void LoadOutages()
		{
			int c = 0;
			VFoutage = string.Empty;
			TFoutage = string.Empty;
			VFbulkCI = string.Empty;
			TFbulkCI = string.Empty;
			string[] strTofind = { "\r\n" };
			string[] log = globalLogs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
			
			if(string.IsNullOrEmpty(log[log.Length - 1])) {
				log = log.Where((source, index) => index != log.Length - 1).ToArray();
			}
			
			// Manipulate log array to make it compatible with VF/TF new logs
			if(Array.FindIndex(log,element => element.Contains("F Report----------")) == -1) {
				List<string> log2 = log.ToList(); // Create new List with log array values
				string Report = log2[1]; // Store outage report to string
				log2.RemoveAt(1); // Remove outage report previously stored on Report string
				string[] SplitReport = Report.Split('\n'); // Split Report string to new array
				log2.Insert(1,"----------VF Report----------"); // Insert VF Report header to match code checks
				log2.InsertRange(2,SplitReport); // Insert SplitReport array into list after header
				log = log2.ToArray(); // Replace original log array with new generated List values
				if(Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal)) > -1) // Check if log contains LTE sites
					log[Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal))] = "----------LTE sites----------"; // Convert header to match code checks
			}
			
			int VFreportIndex = Array.FindIndex(log, element => element.Equals("----------VF Report----------", StringComparison.Ordinal));
			int VFbulkciIndex = Array.FindIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
			int TFreportIndex = Array.FindIndex(log, element => element.Equals("----------TF Report----------", StringComparison.Ordinal));
			int TFbulkciIndex = Array.FindLastIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
			
			if(VFreportIndex > -1) {
				for(c = VFreportIndex + 1;c < VFbulkciIndex;c++) {
					VFoutage += log[c];
					if(c < VFbulkciIndex - 1)
						VFoutage += Environment.NewLine;
				}
				
				if(TFreportIndex == -1) {
					TFreportIndex = log.Length;
				}
				
				for(c = VFbulkciIndex + 1;c < TFreportIndex;c++) {
					VFbulkCI += log[c];
					if(c < TFreportIndex - 1)
						VFbulkCI += Environment.NewLine;
				}
			}
			
			if(TFreportIndex == log.Length)
				TFreportIndex--;
			
			if(log[TFreportIndex].Equals("----------TF Report----------")) {
				for(c = TFreportIndex + 1;c < TFbulkciIndex;c++) {
					TFoutage += log[c];
					if(c < TFbulkciIndex - 1)
						TFoutage += Environment.NewLine;
				}
				
				for(c = TFbulkciIndex + 1;c < log.Length;c++) {
					TFbulkCI += log[c];
					if(c < log.Length - 1)
						VFbulkCI += Environment.NewLine;
				}
			}
			
			if(!string.IsNullOrEmpty(VFoutage) && !string.IsNullOrEmpty(TFoutage)) {
				tabControl1.Visible = true;
				tabControl1.SelectTab(0);
			}
			else {
				if(!string.IsNullOrEmpty(VFoutage)) {
					tabControl1.Visible = false;
					tabControl1.SelectTab(0);
				}
				else {
					if(!string.IsNullOrEmpty(TFoutage)) {
						tabControl1.Visible = false;
						tabControl1.SelectTab(1);
					}
				}
			}
			TabControl4SelectedIndexChanged(null,null);
		}
		
		void ListView1SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SuspendLayout();
			if (listView1.SelectedItems.Count > 0) {
				switch (GlobalLogType) {
					case "Templates":
						switch (listView1.SelectedItems[0].Text) {
							case "Troubleshoot Template":
								if(TroubleshootUI != null)
									TroubleshootUI.Dispose();
								
								TroubleshootUI = new TroubleshootControls(Logs[listView1.SelectedItems[0].Index].ToTroubleShootTemplate());
								TroubleshootUI.Location = new System.Drawing.Point(0, listView1.Bottom + 10);
								this.Controls.Add(TroubleshootUI);
								
								this.Size = new System.Drawing.Size(TroubleshootUI.Right + 6, TroubleshootUI.Bottom + 29);
								break;
							case "Failed CRQ":
								if(FailedCRQUI != null)
									FailedCRQUI.Dispose();
								
								FailedCRQUI = new FailedCRQControls(Logs[listView1.SelectedItems[0].Index].ToFailedCRQTemplate());
								FailedCRQUI.Location = new System.Drawing.Point(0, listView1.Bottom + 10);
								this.Controls.Add(FailedCRQUI);
								this.Size = new System.Drawing.Size(FailedCRQUI.Right + 6, FailedCRQUI.Bottom + 29);
								break;
							case "TX Template":
								if(TXUI != null)
									TXUI.Dispose();
								
								TXUI = new TXControls(Logs[listView1.SelectedItems[0].Index].ToTXTemplate());
								TXUI.Location = new System.Drawing.Point(0, listView1.Bottom + 10);
								this.Controls.Add(TXUI);
								this.Size = new System.Drawing.Size(TXUI.Right + 6, TXUI.Bottom + 29);
								break;
							case "Update Template":
								if(UpdateUI != null)
									UpdateUI.Dispose();
								
								UpdateUI = new UpdateControls(Logs[listView1.SelectedItems[0].Index].ToUpdateTemplate());
								UpdateUI.Location = new System.Drawing.Point(0, listView1.Bottom + 10);
								this.Controls.Add(UpdateUI);
								this.Size = new System.Drawing.Size(UpdateUI.Right + 6, UpdateUI.Bottom + 29);
								break;
						}
						groupBox7.Visible = false;
						button10.Visible = false;
						button11.Visible = false;
						button12.Visible = false;
						break;
					case "Outages":
						groupBox7.Visible = true;
						button10.Visible = true;
						button12.Visible = true;
						button11.Visible = true;
						button11.Text = "Generate sites list";
						button11.Size = new System.Drawing.Size(99, 23);
						button11.Location = new System.Drawing.Point(434, 707);
						LoadOutages();
						button10.Text = "Copy Outage";
						break;
				}
			}
			else {
				if(TroubleshootUI != null)
					TroubleshootUI.Dispose();
				if(FailedCRQUI != null)
					FailedCRQUI.Dispose();
				if(UpdateUI != null)
					UpdateUI.Dispose();
				if(TXUI != null)
					TXUI.Dispose();
				if(OutageUI != null)
					OutageUI.Dispose();
//				this.Height = listView1.Bottom + 7;
				button10.Visible = false;
				button11.Visible = false;
			}
			this.ResumeLayout();
		}
		
		void LogEditorFormClosing(object sender, FormClosingEventArgs e)
		{
			FormCollection fc = Application.OpenForms;

			foreach (Form frm in fc)
			{
				if (frm.Name == "LogBrowser") {
					frm.Activate();
					return;
				}
			}
		}
		
		void TextBox10TextChanged(object sender, EventArgs e)
		{
			button14.Enabled = !string.IsNullOrEmpty(textBox10.Text);
		}
		
		void Button14Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(textBox10.Text,label33.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox10.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void TextBox11TextChanged(object sender, EventArgs e)
		{
			button1.Enabled = textBox11.Text != string.Empty;
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(textBox11.Text,label32.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox11.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void Button10Click(object sender, EventArgs e)
		{
			Toolbox.ScrollableMessageBox msgBox;
			switch(button10.Text) {
				case "Copy Outage":
//					string[] strTofind = { "\r\n" };
//					string[] log = Logs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
//					int outageStartIndex;
//					outageStartIndex = tabControl1.SelectedTab == tabControl1.TabPages[0] ? Array.IndexOf(log, "----------VF Report----------") : Array.IndexOf(log, "----------TF Report----------");
//
//					log = log.Where((source, index) => index > outageStartIndex).ToArray();
//
//					int outageEndIndex = Array.IndexOf(log,"-----BulkCI-----");
//					log = log.Where((source, index) => index < outageEndIndex).ToArray();
//
//					string template = string.Join(Environment.NewLine, log);
					
					try {
						Clipboard.SetText(textBox10.Text);
					}
					catch (Exception) {
						try {
							Clipboard.SetText(textBox10.Text);
						}
						catch (Exception) {
							MessageBox.Show("An error occurred while copying the outage report to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
						}
					}
					
					msgBox = new Toolbox.ScrollableMessageBox();
					msgBox.StartPosition = FormStartPosition.CenterParent;
					msgBox.Show(textBox10.Text, "Success", MessageBoxButtons.OK, "Outage report copied to Clipboard",true);
					break;
				case "Copy Template":
					////					string[] strTofind = { "\r\n" };
					////					string log = Logs[listView1.SelectedItems[0].Index];
//					string[] logArr = globalLogs[listView1.SelectedItems[0].Index].Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();
//					string log = string.Join("\n\n", globalLogs[listView1.SelectedItems[0].Index].Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray());
//					string template = "INC: " + textBox1.Text + Environment.NewLine;
//					template += "Site ID: " + textBox2.Text + Environment.NewLine;
//					template += "Site Owner: ";
//					template += comboBox1.SelectedIndex == 1 ? "TF (" + textBox3.Text + ")" : "VF";
//					template += Environment.NewLine;
//					template += "Site Address: " + textBox4.Text + Environment.NewLine;
//					template += "Other sites impacted: ";
//					template += checkBox1.Checked ? "YES * more info on the INC" : "None";
//					template += Environment.NewLine;
//					template += "COOS: ";
//					template += checkBox2.Checked ? "YES 2G: " + numericUpDown1.Value + " 3G: " + numericUpDown2.Value + " 4G: " + numericUpDown3.Value : "No";
//					template += Environment.NewLine;
//					template += "Full Site Outage: ";
//					template += checkBox18.Checked ? "YES" : "No";
//					template += Environment.NewLine;
//					template += "Performance Issue: ";
//					template += checkBox3.Checked ? "YES" : "No";
//					template += Environment.NewLine;
//					template += "Intermittent Issue: ";
//					template += checkBox4.Checked ? "YES" : "No";
//					template += Environment.NewLine;
//					template += "CCT Reference: ";
//					template += !string.IsNullOrEmpty(textBox5.Text) ? textBox5.Text : "None";
//					template += Environment.NewLine;
//					template += "Related INC/CRQ: ";
//					template += !string.IsNullOrEmpty(textBox6.Text) ? textBox6.Text : "None";
//					template += Environment.NewLine + Environment.NewLine;
//					template += "Active Alarms:" + Environment.NewLine;
//					template += textBox7.Text + Environment.NewLine + Environment.NewLine;
//					template += "Alarm History:" + Environment.NewLine;
//					if (string.IsNullOrEmpty(textBox8.Text)) {
//						template += "None related" + Environment.NewLine + Environment.NewLine;
//					}
//					else {
//						template += textBox8.Text + Environment.NewLine + Environment.NewLine;
//					}
//					template += "Troubleshoot:" + Environment.NewLine;
//					template += textBox9.Text;
//
//					string[] name = Toolbox.Tools.GetUserDetails("Name").Split(' ');
//					string dept = Toolbox.Tools.GetUserDetails("Department");
//					dept = dept.Contains("2nd Line RAN") ? "2nd Line RAN Support" : "1st Line RAN Support";
//
//					template += Environment.NewLine + Environment.NewLine + name[1].Replace(",",string.Empty) + " " + name[0].Replace(",",string.Empty) + Environment.NewLine + dept + Environment.NewLine + "ANOC Number: +44 163 569 206";
//					template += dept == "1st Line RAN Support" ? "7" : "9";
//
//					msgBox = new Toolbox.ScrollableMessageBox();
//					msgBox.StartPosition = FormStartPosition.CenterParent;
//					msgBox.Show(template, "Success", MessageBoxButtons.OK, "Template copied to Clipboard",true);
//
//					try {
//						Clipboard.SetText(template);
//					}
//					catch (Exception) {
//						try {
//							Clipboard.SetText(template);
//						}
//						catch (Exception) {
//							MessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
//						}
//					}
//
					break;
			}
		}
		
		void Button11Click(object sender, EventArgs e)
		{
			switch(button11.Text) {
				case "Copy to Clipboard":
					Action action = new Action(delegate {
					                           	// Generate outage sites list
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
					                           	try {
					                           		Clipboard.SetText(string.Join(Environment.NewLine,temp));
					                           	}
					                           	catch (Exception) {
					                           		try {
					                           			Clipboard.SetText(string.Join(Environment.NewLine,temp));
					                           		}
					                           		catch (Exception) {
					                           			MessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					                           		}
					                           	}
					                           });
					Toolbox.Tools.darkenBackgroundForm(action,false,this);
					break;
				default:
					string[] strTofind = { "\r\n" };
					string[] log = globalLogs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
					
					myFormControl1.FillTemplateFromLog(log,listView1.SelectedItems[0].Text);
					
					
					FormCollection fc = Application.OpenForms;
					foreach (Form frm in fc)
					{
						if (frm.Name == "MainForm") {
							frm.Invoke(new MethodInvoker(frm.Activate));
							return;
						}
					}
					break;
			}
		}
		
		void TabControl4SelectedIndexChanged(object sender, EventArgs e)
		{
			if(tabControl1.SelectedIndex == 0) {
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
		
		void LogEditorActivated(object sender, EventArgs e)
		{
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "appCore.UI.LargeTextForm" || frm.Name == "ScrollableMessageBox") {
					frm.Activate();
					return;
				}
			}
		}
		
//		void RichTextBox_CtrlVFix(object sender, KeyEventArgs e)
//		{
//			if (e.Control && e.KeyCode == Keys.V)
//			{
//				RichTextBox rtb = (RichTextBox)sender;
//				if(!rtb.ReadOnly) {
//					// suspend layout to avoid blinking
//					rtb.SuspendLayout();
//
//					// get insertion point
//					if (rtb.SelectionLength > 1) {
//						rtb.SelectedText = ""; // clear selected text before paste
//					}
//					int insPt = rtb.SelectionStart;
//
//					// preserve text from after insertion pont to end of RTF content
//					string postRTFContent = rtb.Text.Substring(insPt);
//
//					// remove the content after the insertion point
//					rtb.Text = rtb.Text.Substring(0, insPt);
//
//					// add the clipboard content and then the preserved postRTF content
//					rtb.Text += (string)Clipboard.GetData("Text") + postRTFContent;
//
//					// adjust the insertion point to just after the inserted text
//					rtb.SelectionStart = rtb.TextLength - postRTFContent.Length;
//
//					// restore layout
//					rtb.ResumeLayout();
//
//					// cancel the paste
//					e.Handled = true;
//				}
//			}
//		}
		
		void Form_Resize(object sender, EventArgs e)
		{
			// Fire Resize event to check if the window was minimized and minimize LogBrowser as well
			if (WindowState == FormWindowState.Minimized)
			{
//				Form frm = (Form)this.Parent;
				Form frm = this.Owner;
				// FIXME: App crashes when minimizin both windows
//				frm.WindowState = FormWindowState.Minimized;
				frm.Invoke((MethodInvoker)delegate
				           {
				           	frm.WindowState = FormWindowState.Minimized;
				           });
//				Invoke(new Delegate(WindowState = FormWindowState.Minimized;
			}
		}
		
		void Button12Click(object sender, EventArgs e)
		{
			string[] outageSites = (VFbulkCI + TFbulkCI).Split(';');
			outageSites = outageSites.Distinct().ToArray(); // Remover duplicados
			outageSites = outageSites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
			Thread thread = new Thread(() => {
			                           	SiteFinder.siteDetails sd = new SiteFinder.siteDetails(true,outageSites);
			                           	sd.Name = "Outage Follow-up";
			                           	sd.StartPosition = FormStartPosition.CenterParent;
			                           	sd.ShowDialog();
			                           });
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
	}
}

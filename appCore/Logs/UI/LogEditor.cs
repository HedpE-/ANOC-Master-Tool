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
using appCore.Settings;

namespace appCore.Logs.UI
{
	/// <summary>
	/// Description of LogEditor.
	/// </summary>
	public sealed partial class LogEditor : Form
	{
		public string[] Logs;
		public string GlobalLogType;
		List<string> LTEsites = new List<string>();
		public static string VFoutage;
		public static string TFoutage;
		public static string VFbulkCI;
		public static string TFbulkCI;
		MainForm myFormControl1;
		
		public static string[] ParseLogs(string LogFile)
		{
			string separator = string.Empty;
			for (int c = 1; c < 301; c++) {
				if (c == 151) separator += "\r\n";
				separator += "*";
			}
			string[] strTofind = { "\r\n" + separator + "\r\n" }; // build logs separator
			
			string[] temp = File.ReadAllText(LogFile).Split(strTofind, StringSplitOptions.None); // parse all logs using previously built separator
			
			strTofind[0] = strTofind[0].Substring(0,strTofind[0].Length - 2); // remove last line feed for last log removal
			
			if (temp[temp.Length - 1].Contains(strTofind[0])) temp[temp.Length - 1] = temp[temp.Length - 1].Substring(0, temp[temp.Length - 1].Length - strTofind[0].Length); // remove separator from last log since strTofind needs to change to be removed from last
			
			return temp;
		}
		
		public LogEditor(string LogFile, string LogType, string WindowTitle, MainForm myForm)
		{
			InitializeComponent();
			
			myFormControl1 = myForm;
			
			this.Height = 152;
			this.Text = "Log Editor - " + WindowTitle + " - " + LogType + " logs";
			
			GlobalLogType = LogType;
			Logs = ParseLogs(LogFile); // Parse logs on log file
//			List<Log> logs = new List<Log>();
			if (Logs[Logs.Length - 1].Substring(Logs[Logs.Length - 1].Length - 3,3) == "***") {
				Logs[Logs.Length - 1] = Logs[Logs.Length - 1].Substring(0,Logs[Logs.Length - 1].Length - 304);
			}
			
			
			listView1.View = View.Details;
			
			switch (LogType) {
				case "Templates":
					// Populate ListView1 with logs

					listView1.Columns.Add("Log Type").Width = -2;
					listView1.Columns.Add("INC").Width = -2;
					listView1.Columns.Add("Target").Width = -2;
					listView1.Columns.Add("Timestamp").Width = -2;
					
					for (int c = 0; c < Logs.Length; c++) {
						string[] strTofind = { "\r\n" };
						string[] log = Logs[c].Split(strTofind, StringSplitOptions.None);
						strTofind[0] = " - ";
						
						switch (log[0].Split(strTofind, StringSplitOptions.None)[1]) {
							case "Troubleshoot Template":
								listView1.Items.Add(new ListViewItem(new string[]{"Troubleshoot Template",log[1].Substring(5,15),log[2].Substring(9,log[2].Length - 9),log[0].Split(strTofind, StringSplitOptions.None)[0]}));
								break;
							case "Failed CRQ":
								if (log[2].Contains("Site: "))
									listView1.Items.Add(new ListViewItem(new string[]{"Failed CRQ",log[1].Substring(12,log[1].Length - 12),log[2].Substring(6,log[2].Length - 6) + " - " + log[3].Substring(5,15), log[0].Split(strTofind, StringSplitOptions.None)[0]}));
								else
									listView1.Items.Add(new ListViewItem(new string[]{"Failed CRQ",log[1].Substring(12,log[1].Length - 12),log[2].Substring(5,15), log[0].Split(strTofind, StringSplitOptions.None)[0]}));
								break;
							case "TX Template":
								listView1.Items.Add(new ListViewItem(new string[]{"TX Template","-",log[1].Substring(13,log[1].Length - 13),log[0].Split(strTofind, StringSplitOptions.None)[0]}));
								break;
							case "Update Template":
								listView1.Items.Add(new ListViewItem(new string[]{"Update Template",log[1].Substring(5,15),log[2].Substring(6,log[2].Length - 6),log[0].Split(strTofind, StringSplitOptions.None)[0]}));
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
					
					for (int c = 0; c < Logs.Length; c++) {
						string[] strTofind = { "\r\n" };
						string[] log = Logs[c].Split(strTofind, StringSplitOptions.None);
						
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
				case "Updates":
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
			string[] log = Logs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
			
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
		
		public void LoadTST()
		{
			string[] strTofind = { "\r\n" };
			string[] log = Logs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
			do {
				if(log[log.Length - 1] == string.Empty) log = log.Where((source, index) => index != log.Length - 1).ToArray();
			} while (log[log.Length - 1] == string.Empty);
			
			textBox1.Text = log[1].Substring("INC: ".Length);
			textBox2.Text = log[2].Substring("Site ID: ".Length);
			comboBox1.Text = log[3].Substring("Site Owner: ".Length,2);
			if(comboBox1.Text == "TF") {
				string[] temp = log[3].Split('(');
				string tempStr = string.Empty;
				foreach (char c in temp[temp.GetUpperBound(0)]) {
					if(c != ')')
						tempStr += c;
				}
				textBox3.Text = tempStr;
			}
			else
				textBox3.Text = string.Empty;
			textBox4.Text = log[4].Substring("Site Address: ".Length);
			checkBox1.Checked = log[5].Substring("Other sites impacted: ".Length) != "None";
			if (log[6].Substring("COOS: ".Length,2) == "No")
				checkBox2.Checked = false;
			else {
				checkBox2.Checked = true;
				string[] COOS = log[6].Substring("COOS: YES ".Length).Split(' ');
				numericUpDown1.Text = COOS[1];
				numericUpDown2.Text = COOS[3];
				numericUpDown3.Text = COOS[5];
			}
			int ind = 7;
			if(log[ind].StartsWith("Full")) {
				checkBox18.Checked = log[ind].Substring("Full Site Outage: ".Length) != "No";
				ind++;
			}
			else {
				checkBox18.Checked = false;
				checkBox18.Visible = false;
			}
			checkBox3.Checked = log[ind].Substring("Performance Issue: ".Length) != "No";
			ind++;
			checkBox4.Checked = log[ind].Substring("Intermittent Issue: ".Length) != "No";
			ind++;
			textBox5.Text = log[ind].Substring("CCT Reference: ".Length) == "None" ? string.Empty : log[ind].Substring("CCT Reference: ".Length);
			ind++;
			textBox6.Text = log[ind].Substring("Related INC/CRQ: ".Length) == "None" ? string.Empty : log[ind].Substring("Related INC/CRQ: ".Length);
			
			string complete = string.Empty;
			ind = Array.IndexOf(log, "Active Alarms:");
			for (ind++; ind < log.Length - 4; ind++) {
				if (log[ind] == "") {
					if (log[ind+1] != "Alarm History:")
						complete += Environment.NewLine;
					else {
						ind++;
						break;
					}
				}
				else {
					if (string.IsNullOrEmpty(complete)) complete = log[ind];
					else complete += Environment.NewLine + log[ind];
				}
			}
			textBox7.Text = complete;
			
			complete = string.Empty;
			for (ind++; ind < log.Length - 4; ind++) {
				if (log[ind] == "None related") {
					complete = string.Empty;
					ind += 2;
					break;
				}
				if (log[ind] == "") {
					if (log[ind+1] != "Troubleshoot:") complete = complete + Environment.NewLine;
					else {
						ind++;
						break;
					}
				}
				else {
					if (string.IsNullOrEmpty(complete))
						complete = log[ind];
					else
						complete += Environment.NewLine + log[ind];
				}
			}
			textBox8.Text = complete;
			
			complete = string.Empty;
			ind = Array.IndexOf(log, "Troubleshoot:");
			for (ind++; ind < log.Length - 4; ind++) {
				if (complete == string.Empty) complete = log[ind];
				else complete += Environment.NewLine + log[ind];
			}
			textBox9.Text = complete;
		}
		
		public void LoadFCRQ()
		{
			string complete = string.Empty;
			string[] strTofind = { "\r\n" };
			string[] log = Logs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
			int c = 1;
			textBox16.Text = log[c].Substring("INC raised: ".Length);
			c++;
			if (log[c].Contains("Site: ")) {
				textBox27.Text = log[c].Substring("Site: ".Length);
				c++;
			}
			else
				textBox27.Text = string.Empty;
			textBox17.Text = log[c].Substring("CRQ: ".Length);
			c++;
			
			if(log[c].Contains("CRQ contacts:")) {
				complete = log[++c];
				for (c++; c < log.Length; c++) {
					if (!log[c].Contains("FE booked in:"))
						complete += Environment.NewLine + log[c];
					else break;
				}
				richTextBox16.Text = complete;
			}
			else {
				richTextBox16.Text = log[c].Substring("CRQ contact: ".Length);
				c++;
			}
			string[] temp = log[c].Substring("FE booked in: ".Length).Split(',');
			textBox18.Text = temp[0];
			textBox22.Text = temp[1].Substring(1);
			c++;
			checkBox5.Checked = log[c].Substring("Did FE call the ANOC after CRQ: ".Length) != "No";
			c++;
			
			complete = string.Empty;
			if(log[c].Length > "Work performed by FE on site:".Length) {
				if (log[c].Substring("Work performed by FE on site:".Length) == " N/A")
					richTextBox1.Text = string.Empty;
				else
					complete = log[c].Substring("Work performed by FE on site: ".Length);
				c++;
			}
			else {
				complete = log[++c];
				for (c++; c < log.Length; c++) {
					if (!log[c].Contains("Troubleshooting done with FE on site to recover affected cells:"))
						complete += Environment.NewLine + log[c];
					else break;
				}
				richTextBox1.Text = complete;
			}
			
			complete = string.Empty;
			if(log[c].Length > "Troubleshooting done with FE on site to recover affected cells:".Length) {
				if (log[c].Substring("Troubleshooting done with FE on site to recover affected cells:".Length) == " N/A")
					richTextBox2.Text = string.Empty;
				else
					complete = log[c].Substring("Troubleshooting done with FE on site to recover affected cells:".Length);
				c++;
			}
			else {
				complete = log[++c];
				for (c++; c < log.Length; c++) {
					if (!log[c].Contains("Contractor to fix the fault: "))
						complete += Environment.NewLine + log[c];
					else break;
				}
				richTextBox2.Text = complete;
			}
			
			if (log[c].Substring("Contractor to fix the fault: ".Length) == "None provided") {
				textBox24.Text = string.Empty;
				textBox23.Text = string.Empty;
				checkBox6.Checked = false;
				c++;
			}
			else {
				string[] temp2 = log[c].Substring("Contractor to fix the fault: ".Length).Split(strTofind, StringSplitOptions.None);
				textBox24.Text = temp2[0];
				textBox23.Text = temp2[1];
				c++;
				if (log[c].Substring("Time to fix the fault: ".Length) == "None provided")
					checkBox6.Checked = false;
				else {
					checkBox6.Checked = true;
					string[] temp3 = log[c].Substring("Time to fix the fault: ".Length).Split(':');
					dateTimePicker1.Value = new DateTime(2014, 11, 19, Convert.ToInt32(temp3[0]), Convert.ToInt32(temp3[1]), Convert.ToInt32(temp3[2]), 0);
				}
			}
			c++;
			
			complete = string.Empty;
			if(log[c].Length > "Observations:".Length) {
				if (log[c].Substring("Observations:".Length) == " N/A")
					richTextBox3.Text = string.Empty;
				else
					complete = log[c].Substring("Troubleshooting done with FE on site to recover affected cells:".Length);
				c++;
			}
			else {
				if (c < log.Length) {
					complete += log[++c];
					for (c++; c < log.Length; c++)
						complete += Environment.NewLine + log[c];
				}
				richTextBox3.Text = complete;
			}
		}
		
		public void LoadTXT()
		{
			string[] strTofind = { "\r\n" };
			string[] log = Logs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
			
			string complete = log[1].Substring(13,log[1].Length - 13);
			int c = 1;
			for (c++; c < log.Length; c++) {
				if (!log[c].Contains("Service affected: ")) complete = complete + Environment.NewLine + log[c];
				else break;
			}
			textBox25.Text = complete;
			comboBox2.Text = log[c].Substring(18,log[c].Length - 18);
			c++;
			
			complete = log[c].Substring(35,log[c].Length - 35);
			for (c++; c < log.Length; c++) {
				if (!log[c].Contains("Repeat/Intermittent: ")) complete = complete + Environment.NewLine + log[c];
				else break;
			}
			richTextBox4.Text = complete;
			
			checkBox7.Checked = log[c].Substring(21, 2) == "No" ? false : true;
			c++;
			
			comboBox3.Text = log[c].Substring(22, log[c].Length - 22);
			c++;
			textBox26.Text = comboBox3.Text == "IPRAN" ? log[c].Substring(26, log[c].Length - 26) : string.Empty;
			c++;
			
			complete = log[c].Substring(88,log[c].Length - 88);
			for (c++; c < log.Length - 4; c++) {
				complete = complete + Environment.NewLine + log[c];
			}
			richTextBox5.Text = complete;
		}
		
		public void LoadUPT()
		{
			string[] strTofind = { "\r\n" };
			string[] log = Logs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
			
			textBox44.Text = log[1].Substring(5,15);
			textBox43.Text = log[2].Substring(6,log[2].Length - 6);
			richTextBox13.Text = log[4];
			if (log[6] == "Next actions:") richTextBox12.Text = log[7];
		}
		
		void ListView1SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0) {
				switch (GlobalLogType) {
					case "Templates":
						switch (listView1.SelectedItems[0].Text) {
							case "Troubleshoot Template":
								groupBox1.Visible = true;
								groupBox2.Visible = false;
								groupBox3.Visible = false;
								groupBox7.Visible = false;
								groupBox8.Visible = false;
								button11.Visible = true;
								button12.Visible = false;
								LoadTST();
								break;
							case "Failed CRQ":
								groupBox1.Visible = false;
								groupBox2.Visible = true;
								groupBox3.Visible = false;
								groupBox7.Visible = false;
								groupBox8.Visible = false;
								button11.Visible = true;
								button12.Visible = false;
								LoadFCRQ();
								break;
							case "TX Template":
								groupBox1.Visible = false;
								groupBox2.Visible = false;
								groupBox3.Visible = true;
								groupBox7.Visible = false;
								groupBox8.Visible = false;
								button11.Visible = true;
								button12.Visible = false;
								LoadTXT();
								break;
							case "Update Template":
								groupBox1.Visible = false;
								groupBox2.Visible = false;
								groupBox3.Visible = false;
								groupBox7.Visible = false;
								groupBox8.Visible = true;
								button11.Visible = false;
								button12.Visible = false;
								LoadUPT();
								break;
						}
						button11.Text = "Copy to new Troubleshoot template";
						button11.Size = new System.Drawing.Size(183, 23);
						button11.Location = new System.Drawing.Point(350, 707);
						button10.Text = "Copy Template";
						break;
					case "Outages":
						groupBox1.Visible = false;
						groupBox2.Visible = false;
						groupBox3.Visible = false;
						groupBox7.Visible = true;
						groupBox8.Visible = false;
						button12.Visible = true;
						button11.Visible = true;
						button11.Text = "Generate sites list";
						button11.Size = new System.Drawing.Size(99, 23);
						button11.Location = new System.Drawing.Point(434, 707);
						LoadOutages();
						button10.Text = "Copy Outage";
						break;
				}
				this.Height = 765;
				button10.Visible = true;
			}
			else {
				this.Height = 153;
				button10.Visible = false;
				button11.Visible = false;
			}
		}
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.Text == "TF") {
				textBox3.Visible = true;
				label14.Visible = true;
			}
			else {
				textBox3.Visible = false;
				textBox3.Text = string.Empty;
				label14.Visible = false;
			}
		}
		
		void CheckBox2CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox2.Checked) {
				checkBox18.Visible = true;
				numericUpDown1.Visible = true;
				numericUpDown2.Visible = true;
				numericUpDown3.Visible = true;
				label15.Visible = true;
				label16.Visible = true;
				label17.Visible = true;
			}
			else {
				checkBox18.Visible = false;
				checkBox18.Checked = false;
				numericUpDown1.Visible = false;
				numericUpDown1.Value = 0;
				numericUpDown2.Visible = false;
				numericUpDown2.Value = 0;
				numericUpDown3.Visible = false;
				numericUpDown3.Value = 0;
				label15.Visible = false;
				label16.Visible = false;
				label17.Visible = false;
			}
		}
		
		void CheckBox6CheckedChanged(object sender, EventArgs e)
		{
			dateTimePicker1.Visible = checkBox6.Checked;
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
		
		void TextBox7TextChanged(object sender, EventArgs e)
		{
			button7.Enabled = textBox7.Text != string.Empty;
		}
		
		void Button7Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(textBox7.Text,label11.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox7.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void TextBox8TextChanged(object sender, EventArgs e)
		{
			button8.Enabled = textBox8.Text != string.Empty;
		}
		
		void Button8Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(textBox8.Text,label12.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox8.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void TextBox9TextChanged(object sender, EventArgs e)
		{
			button9.Enabled = textBox9.Text != string.Empty;
		}
		
		void Button9Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(textBox9.Text,label13.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox9.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void RichTextBox1TextChanged(object sender, EventArgs e)
		{
			button4.Enabled = richTextBox1.Text != string.Empty;
		}
		
		void Button4Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox1.Text,label25.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox1.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void RichTextBox2TextChanged(object sender, EventArgs e)
		{
			button5.Enabled = richTextBox2.Text != string.Empty;
		}
		
		void Button5Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox2.Text,label26.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox2.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void Button54Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox16.Text,groupBox5.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox16.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void RichTextBox16TextChanged(object sender, EventArgs e)
		{
			button54.Enabled = richTextBox16.Text != string.Empty;
		}
		
		void RichTextBox3TextChanged(object sender, EventArgs e)
		{
			button6.Enabled = richTextBox3.Text != string.Empty;
		}
		
		void Button6Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox3.Text,label27.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox3.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void RichTextBox4TextChanged(object sender, EventArgs e)
		{
			button2.Enabled = richTextBox4.Text != string.Empty;
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox4.Text,label6.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox4.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void RichTextBox5TextChanged(object sender, EventArgs e)
		{
			button3.Enabled = richTextBox5.Text != string.Empty;
		}
		
		void Button3Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox5.Text,label37.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox5.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
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
		
		void RichTextBox13TextChanged(object sender, EventArgs e)
		{
			button39.Enabled = richTextBox13.Text != string.Empty;
		}
		
		void Button39Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox13.Text,label52.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox13.Text = enlarge.finaltext;
			                           });
			Toolbox.Tools.darkenBackgroundForm(action,false,this);
		}
		
		void RichTextBox12TextChanged(object sender, EventArgs e)
		{
			button38.Enabled = richTextBox12.Text != string.Empty;
		}
		
		void Button38Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	appCore.UI.LargeTextForm enlarge = new appCore.UI.LargeTextForm(richTextBox12.Text,label51.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	richTextBox12.Text = enlarge.finaltext;
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
//					string[] strTofind = { "\r\n" };
//					string log = Logs[listView1.SelectedItems[0].Index];
					string[] logArr = Logs[listView1.SelectedItems[0].Index].Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();
					string log = string.Join("\n\n", Logs[listView1.SelectedItems[0].Index].Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray());
					string template = "INC: " + textBox1.Text + Environment.NewLine;
					template += "Site ID: " + textBox2.Text + Environment.NewLine;
					template += "Site Owner: ";
					template += comboBox1.SelectedIndex == 1 ? "TF (" + textBox3.Text + ")" : "VF";
					template += Environment.NewLine;
					template += "Site Address: " + textBox4.Text + Environment.NewLine;
					template += "Other sites impacted: ";
					template += checkBox1.Checked ? "YES * more info on the INC" : "None";
					template += Environment.NewLine;
					template += "COOS: ";
					template += checkBox2.Checked ? "YES 2G: " + numericUpDown1.Value + " 3G: " + numericUpDown2.Value + " 4G: " + numericUpDown3.Value : "No";
					template += Environment.NewLine;
					template += "Full Site Outage: ";
					template += checkBox18.Checked ? "YES" : "No";
					template += Environment.NewLine;
					template += "Performance Issue: ";
					template += checkBox3.Checked ? "YES" : "No";
					template += Environment.NewLine;
					template += "Intermittent Issue: ";
					template += checkBox4.Checked ? "YES" : "No";
					template += Environment.NewLine;
					template += "CCT Reference: ";
					template += !string.IsNullOrEmpty(textBox5.Text) ? textBox5.Text : "None";
					template += Environment.NewLine;
					template += "Related INC/CRQ: ";
					template += !string.IsNullOrEmpty(textBox6.Text) ? textBox6.Text : "None";
					template += Environment.NewLine + Environment.NewLine;
					template += "Active Alarms:" + Environment.NewLine;
					template += textBox7.Text + Environment.NewLine + Environment.NewLine;
					template += "Alarm History:" + Environment.NewLine;
					if (string.IsNullOrEmpty(textBox8.Text)) {
						template += "None related" + Environment.NewLine + Environment.NewLine;
					}
					else {
						template += textBox8.Text + Environment.NewLine + Environment.NewLine;
					}
					template += "Troubleshoot:" + Environment.NewLine;
					template += textBox9.Text;
					
					template = CurrentUser.fullName[1] + " " + CurrentUser.fullName[0] + Environment.NewLine + CurrentUser.department + Environment.NewLine + "ANOC Number: +44 163 569 206";
					template += CurrentUser.department == "1st Line RAN Support" ? "7" : "9";
					
					msgBox = new Toolbox.ScrollableMessageBox();
					msgBox.StartPosition = FormStartPosition.CenterParent;
					msgBox.Show(template, "Success", MessageBoxButtons.OK, "Template copied to Clipboard",true);
					
					try {
						Clipboard.SetText(template);
					}
					catch (Exception) {
						try {
							Clipboard.SetText(template);
						}
						catch (Exception) {
							MessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
						}
					}
					
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
					string[] log = Logs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
					
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
		
		void RichTextBox_CtrlVFix(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.V)
			{
				RichTextBox rtb = (RichTextBox)sender;
				if(!rtb.ReadOnly) {
					// suspend layout to avoid blinking
					rtb.SuspendLayout();

					// get insertion point
					if (rtb.SelectionLength > 1) {
						rtb.SelectedText = ""; // clear selected text before paste
					}
					int insPt = rtb.SelectionStart;

					// preserve text from after insertion pont to end of RTF content
					string postRTFContent = rtb.Text.Substring(insPt);

					// remove the content after the insertion point
					rtb.Text = rtb.Text.Substring(0, insPt);

					// add the clipboard content and then the preserved postRTF content
					rtb.Text += (string)Clipboard.GetData("Text") + postRTFContent;

					// adjust the insertion point to just after the inserted text
					rtb.SelectionStart = rtb.TextLength - postRTFContent.Length;

					// restore layout
					rtb.ResumeLayout();

					// cancel the paste
					e.Handled = true;
				}
			}
		}
		
		void Form_Resize(object sender, EventArgs e)
		{
			// Fire Resize event to check if the window was minimized and minimize LogBrowser as well
			if (WindowState == FormWindowState.Minimized)
			{
				Form frm = (Form)this.Parent;
				frm.WindowState = FormWindowState.Minimized;
			}
		}
		void Button12Click(object sender, EventArgs e)
		{
			string[] outageSites = (VFbulkCI + TFbulkCI).Split(';');
			outageSites = outageSites.Distinct().ToArray(); // Remover duplicados
			outageSites = outageSites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
			Thread thread = new Thread(() => {
			                           	SiteFinder.UI.siteDetails sd = new SiteFinder.UI.siteDetails(true,outageSites);
			                           	sd.Name = "Outage Follow-up";
			                           	sd.StartPosition = FormStartPosition.CenterParent;
			                           	sd.ShowDialog();
			                           });
			
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
	}
}

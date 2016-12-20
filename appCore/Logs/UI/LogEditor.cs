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
using appCore.UI;

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
		
		void ListView1SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0) {
				switch (GlobalLogType) {
					case "Outages":
						groupBox7.Visible = true;
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
			Action actionNonThreaded = new Action(delegate {
			                                      	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox10.Text,label33.Text,true);
			                                      	enlarge.StartPosition = FormStartPosition.CenterParent;
			                                      	enlarge.ShowDialog();
			                                      	textBox10.Text = enlarge.finaltext;
			                                      });
			LoadingPanel load = new LoadingPanel();
			load.Show(null, actionNonThreaded,false,this);
		}
		
		void TextBox11TextChanged(object sender, EventArgs e)
		{
			button1.Enabled = textBox11.Text != string.Empty;
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			Action actionNonThreaded = new Action(delegate {
			                           	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox11.Text,label32.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox11.Text = enlarge.finaltext;
			                           });
			LoadingPanel load = new LoadingPanel();
			load.Show(null, actionNonThreaded,false,this);
		}
		
		void Button10Click(object sender, EventArgs e)
		{
			if(button10.Text == "Copy Outage") {
				try {
					Clipboard.SetText(textBox10.Text);
				}
				catch (Exception) {
					try {
						Clipboard.SetText(textBox10.Text);
					}
					catch (Exception) {
						FlexibleMessageBox.Show("An error occurred while copying the outage report to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					}
				}
				
				FlexibleMessageBox.Show(textBox10.Text, "Success", MessageBoxButtons.OK);
			}
		}
		
		void Button11Click(object sender, EventArgs e)
		{
			if(button11.Text == "Copy to Clipboard") {
				Action actionThreaded = new Action(delegate {
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
				                           	
				                           	FlexibleMessageBox.Show("The following site list was copied to the Clipboard:" + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine,temp) + Environment.NewLine + Environment.NewLine + "This list can be used to enter a bulk site search on Site Lopedia.","List generated",MessageBoxButtons.OK,MessageBoxIcon.Information);
				                           	try {
				                           		Clipboard.SetText(string.Join(Environment.NewLine,temp));
				                           	}
				                           	catch (Exception) {
				                           		try {
				                           			Clipboard.SetText(string.Join(Environment.NewLine,temp));
				                           		}
				                           		catch (Exception) {
				                           			FlexibleMessageBox.Show("An error occurred while copying template to the clipboard, please try again.","Clipboard error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				                           		}
				                           	}
				                           });
				LoadingPanel load = new LoadingPanel();
				load.Show(actionThreaded, null, false, this);
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

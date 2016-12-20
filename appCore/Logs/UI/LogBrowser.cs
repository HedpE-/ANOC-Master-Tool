/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 20/03/2015
 * Time: 02:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using appCore.UI;

namespace appCore.Logs.UI
{
    /// <summary>
    /// Description of LogBrowser.
    /// </summary>
    public partial class LogBrowser : Form
	{
		public string chkrb;
		MainForm myFormControl1;
		
		public string LogCount(string logfile)
		{
			string strTofind = string.Empty;
			for (int c = 1; c < 301; c++) {
				if (c == 151) strTofind = strTofind + Environment.NewLine;
				strTofind = strTofind + "*";
			}
			
			return Toolbox.Tools.CountStringOccurrences(File.ReadAllText(logfile), strTofind).ToString();
		}
		
		public string WindowTitle()
		{
			string temp = listBox3.Text;
			switch (temp) {
				case "1": case "21": case "31":
					temp += "st of ";
					break;
				case "2": case "22":
					temp += "nd of ";
					break;
				case "3": case "23":
					temp += "rd of ";
					break;
				default:
					temp += "th of ";
					break;
			}
			return temp + DateTime.ParseExact(listBox2.Text,"MMM",CultureInfo.GetCultureInfo("pt-PT")).ToString("MMMM",CultureInfo.GetCultureInfo("en-GB")) + ", " + listBox1.Text;
		}
		
		public LogBrowser(MainForm myForm)
		{
			InitializeComponent();
			
			myFormControl1 = myForm;
			
			radioButton1.Checked = true;
		}
		
		void ListBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			listBox2.Items.Clear();
			listBox3.Items.Clear();
			label1.Text = string.Empty;
			
			if (listBox1.SelectedIndex != -1) {
				string month = string.Empty;
				ArrayList monthsList = new ArrayList();
				CultureInfo culture = new CultureInfo("pt-PT");
				
				
				switch (chkrb) {
					case "Templates": // get available months list for templates
						foreach (var folder in Settings.UserFolder.LogsFolder.GetDirectories("*-" + listBox1.Text)) {
							if (folder.GetFiles("*.txt").Length > 0) {
								month = folder.Name.Substring(0,3);
								monthsList.Add(DateTime.ParseExact(month,"MMM",culture));
							}
						}
						break;
					case "Outages": // get available months list for outages
						foreach (var folder in Settings.UserFolder.LogsFolder.GetDirectories("*-" + listBox1.Text)) {
							DirectoryInfo tempdir = new DirectoryInfo(Settings.UserFolder.LogsFolder.FullName + "\\" + folder.Name);
							foreach (var tempfolder in tempdir.GetDirectories()) {
								if (tempfolder.Name == "outages") {
									month = folder.Name.Substring(0,3);
									monthsList.Add(DateTime.ParseExact(month,"MMM",culture));
								}
							}
						}
						break;
					case "Updates": // get available months list for outages
						break;
				}
				monthsList.Sort();
				foreach(DateTime obj in monthsList)
				{
					listBox2.Items.Add(obj.ToString("MMM",culture));
				}
			}
		}
		
		void ListBox2SelectedIndexChanged(object sender, EventArgs e)
		{
			listBox3.Items.Clear();
			label1.Text = string.Empty;
			if (listBox2.SelectedIndex != -1) {
				if (chkrb == "Templates") {
					DirectoryInfo monthdir = new DirectoryInfo(Settings.UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text);
					foreach (var file in monthdir.GetFiles("*.txt")) {
						listBox3.Items.Add(file.Name.Substring(0,2));
					}
				}
				else {
					if (chkrb == "Outages") {
						DirectoryInfo monthdir = new DirectoryInfo(Settings.UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text + "\\outages");
						foreach (var file in monthdir.GetFiles("*.txt")) {
							listBox3.Items.Add(file.Name.Substring(0,2));
						}
					}
					else {
						if (radioButton3.Checked) {}
					}
				}
			}
		}
		
		void ListBox3SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox3.SelectedIndex != -1) {
				string logfile = Settings.UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text + "\\";
				if (radioButton2.Checked)
					logfile += "outages\\";
				logfile += listBox3.Text + ".txt";
				label1.Text = "Total logs on selected log file: " + LogCount(logfile);
				
			}
		}
		
		void ListBox3DoubleClick(object sender, EventArgs e)
		{
			Action actionThreaded = new Action(delegate {
			                           	if (listBox3.SelectedIndex != -1) {
			                           		string LogFile;
			                           		if (chkrb == "Templates") {
			                           			string separator = string.Empty;
			                           			for (int c = 1; c < 301; c++) {
			                           				if (c == 151) separator += "\r\n";
			                           				separator += "*";
			                           			}
			                           			LogFile = Settings.UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text + "\\" + listBox3.Text + ".txt";
			                           			string[] strTofind = { "\r\n" };
			                           			string[] Logs = File.ReadAllText(LogFile).Split(strTofind, StringSplitOptions.None);
			                           			
			                           			if (Logs[0].Contains(" - ")) {
			                           				LogsCollection<Templates.Template> logs = new LogsCollection<Templates.Template>();
			                           				logs = logs.ImportLogFile(new FileInfo(LogFile));
			                           				LogEditor2 LogEdit = new LogEditor2(logs, "Templates", myFormControl1);
			                           				LogEdit.StartPosition = FormStartPosition.CenterParent;
			                           				LogEdit.ShowDialog(this);
			                           			}
			                           			else {
			                           				DialogResult ans = FlexibleMessageBox.Show("This Log file isn't compatible with the built-in viewer.\n\nDo you want to open with Notepad?","Can't open log file",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
			                           				if (ans == DialogResult.No)	return;
			                           				System.Diagnostics.Process.Start("notepad.exe", LogFile);
			                           			}
			                           		}
			                           		else if (chkrb == "Outages") {
			                           			string separator = string.Empty;
			                           			for (int i = 1; i < 301; i++) {
			                           				if (i == 151) separator = separator + Environment.NewLine;
			                           				separator = separator + "*";
			                           			}
			                           			
			                           			LogFile = Settings.UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text + "\\outages\\" + listBox3.Text + ".txt";
			                           			
			                           			string[] strTofind = { separator + "\r\n" };
			                           			string[] Logs = File.ReadAllText(LogFile).Split(strTofind, StringSplitOptions.None);
			                           			
			                           			LogEditor LogEdit = new LogEditor(LogFile, "Outages", WindowTitle(),myFormControl1);
			                           			LogEdit.StartPosition = FormStartPosition.CenterParent;
			                           			LogEdit.ShowDialog();
			                           		}
			                           		else
			                           			if(chkrb == "Updates") {}
			                           	}
			                           });
			LoadingPanel load = new LoadingPanel();
			load.Show(actionThreaded, null, false, this);
		}
		
		void ListBox3KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				ListBox3DoubleClick(sender, e);
			}
		}
		
		void LogBrowserActivated(object sender, EventArgs e)
		{
			FormCollection fc = Application.OpenForms;
			
			foreach (Form frm in fc)
			{
				if (frm.Name == "LogEditor") {
					frm.Activate();
					return;
				}
			}
		}
		
		void RadioButton1CheckedChanged(object sender, EventArgs e)
		{
			listBox1.Items.Clear();
			listBox2.Items.Clear();
			listBox3.Items.Clear();
			foreach (var folder in Settings.UserFolder.LogsFolder.GetDirectories()) {
				if (folder.GetFiles("*.txt").Length > 0) {
					string year = folder.Name.Substring(4,4);
					if (!listBox1.Items.Contains(year))
						listBox1.Items.Add(year);
				}
			}
			label1.Text = string.Empty;
			chkrb = radioButton1.Text;
		}
		
		void RadioButton2CheckedChanged(object sender, EventArgs e)
		{
			listBox1.Items.Clear();
			listBox2.Items.Clear();
			listBox3.Items.Clear();
			foreach (var folder in Settings.UserFolder.LogsFolder.GetDirectories()) {
				string year = folder.Name.Substring(4,4);
				DirectoryInfo tempdir = new DirectoryInfo(Settings.UserFolder.LogsFolder.FullName + "\\" + folder.Name);
				foreach (var tempfolder in tempdir.GetDirectories()) {
					if (tempfolder.Name == "outages") {
						if (!listBox1.Items.Contains(year))
							listBox1.Items.Add(year);
					}
				}
				
			}
			
			label1.Text = string.Empty;
			chkrb = radioButton2.Text;
		}
		
		void RadioButton3CheckedChanged(object sender, EventArgs e)
		{
			label1.Text = string.Empty;
			chkrb = radioButton3.Text;
		}
	}
}

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
using appCore.Settings;
using appCore.Templates;
using appCore.Templates.Types;

namespace appCore.Logs.UI
{
	/// <summary>
	/// Description of LogBrowser.
	/// </summary>
	public partial class LogBrowser : Form
	{
		string chkrb {
			get {
				if(radioButton1.Checked)
					return radioButton1.Text;
				return radioButton2.Checked ? radioButton2.Text : string.Empty;
			}
		}
		MainForm myFormControl1;
		
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
				
				switch(chkrb) {
					case "Templates": // get available months list for templates
						foreach (var folder in UserFolder.LogsFolder.GetDirectories("*-" + listBox1.Text)) {
							if (folder.GetFiles("*.txt").Length > 0) {
								month = folder.Name.Substring(0,3);
								monthsList.Add(DateTime.ParseExact(month,"MMM",culture));
							}
						}
						break;
					case "Outages": // get available months list for outages
						foreach (var folder in UserFolder.LogsFolder.GetDirectories("*-" + listBox1.Text)) {
							DirectoryInfo tempdir = new DirectoryInfo(UserFolder.LogsFolder.FullName + "\\" + folder.Name);
							foreach (var tempfolder in tempdir.GetDirectories()) {
								if (tempfolder.Name == "outages") {
									month = folder.Name.Substring(0,3);
									monthsList.Add(DateTime.ParseExact(month,"MMM",culture));
								}
							}
						}
						break;
				}
				monthsList.Sort();
				foreach(DateTime obj in monthsList)
					listBox2.Items.Add(obj.ToString("MMM", culture));
			}
		}
		
		void ListBox2SelectedIndexChanged(object sender, EventArgs e)
		{
			listBox3.Items.Clear();
			label1.Text = string.Empty;
			if (listBox2.SelectedIndex != -1) {
				if (chkrb == "Templates") {
					DirectoryInfo monthdir = new DirectoryInfo(UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text);
					foreach (var file in monthdir.GetFiles("*.txt"))
						listBox3.Items.Add(file.Name.Substring(0,2));
				}
				else {
					DirectoryInfo monthdir = new DirectoryInfo(UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text + "\\outages");
					foreach (var file in monthdir.GetFiles("*.txt"))
						listBox3.Items.Add(file.Name.Substring(0,2));
				}
			}
		}
		
		void ListBox3SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox3.SelectedIndex != -1) {
				string logfile = UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text + "\\";
				if (radioButton2.Checked)
					logfile += "outages\\";
				logfile += listBox3.Text + ".txt";
				label1.Text = "Total logs on selected log file: " + LogCount(logfile);
				
			}
		}
		
		void ListBox3DoubleClick(object sender, EventArgs e)
		{
//			Action actionNonThreaded = null;
			Action actionThreaded = new Action(delegate {
			                                   	if (listBox3.SelectedIndex != -1) {
			                                   		string separator = string.Empty;
			                                   		for (int c = 1; c < 301; c++) {
			                                   			if (c == 151) separator += "\r\n";
			                                   			separator += "*";
			                                   		}
			                                   		string LogFile = UserFolder.LogsFolder.FullName + "\\" + listBox2.Text + "-" + listBox1.Text + "\\";
			                                   		string[] strTofind = { "\r\n" };
			                                   		string[] Logs;
			                                   		
			                                   		if (chkrb == "Templates") {
			                                   			LogFile += listBox3.Text + ".txt";
			                                   			Logs = File.ReadAllText(LogFile).Split(strTofind, StringSplitOptions.None);
			                                   			if (Logs[0].Contains(" - ")) {
			                                   				LogsCollection<Template> logs = new LogsCollection<Template>();
//			                                   				actionNonThreaded = delegate {
			                                   				logs = logs.ImportLogFile(new FileInfo(LogFile));
			                                   				LogEditor LogEdit = new LogEditor(logs, myFormControl1);
			                                   				LogEdit.StartPosition = FormStartPosition.CenterParent;
			                                   				LogEdit.ShowDialog(this);
//			                                   				};
			                                   			}
			                                   			else {
			                                   				DialogResult ans = FlexibleMessageBox.Show("This Log file isn't compatible with the built-in viewer.\n\nDo you want to open with Notepad?","Can't open log file",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
			                                   				if(ans == DialogResult.No)
			                                   					return;
			                                   				System.Diagnostics.Process.Start("notepad.exe", LogFile);
			                                   			}
			                                   		}
			                                   		else {
			                                   			LogFile += "outages\\" + listBox3.Text + ".txt";
			                                   			
			                                   			Logs = File.ReadAllText(LogFile).Split(strTofind, StringSplitOptions.None);
			                                   			LogsCollection<Outage> logs = new LogsCollection<Outage>();
//			                                   				actionNonThreaded = delegate {
			                                   			logs = logs.ImportOutagesLogFile(new FileInfo(LogFile));
//			                                   			actionNonThreaded = delegate {
			                                   			LogEditor LogEdit = new LogEditor(logs, myFormControl1);
			                                   			LogEdit.StartPosition = FormStartPosition.CenterParent;
			                                   			LogEdit.ShowDialog();
//			                                   			};
			                                   		}
			                                   	}
			                                   });
			Toolbox.Tools.darkenBackgroundForm(actionThreaded, false, this);
//			LoadingPanel load = new LoadingPanel();
//			load.Show(actionThreaded, actionNonThreaded, false, this);
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
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
			listBox1.Items.Clear();
			listBox2.Items.Clear();
			listBox3.Items.Clear();
			ArrayList items = new ArrayList();
			
			switch(rb.Name) {
				case "radioButton1":
					foreach (var folder in UserFolder.LogsFolder.GetDirectories()) {
						if (folder.GetFiles("*.txt").Length > 0) {
							string year = folder.Name.Substring(4,4);
							if (!items.Contains(year))
								items.Add(year);
						}
					}
					break;
				case "radioButton2":
					foreach (var folder in UserFolder.LogsFolder.GetDirectories()) {
						string year = folder.Name.Substring(4,4);
						DirectoryInfo tempdir = new DirectoryInfo(UserFolder.LogsFolder.FullName + "\\" + folder.Name);
						foreach (var tempfolder in tempdir.GetDirectories()) {
							if (tempfolder.Name == "outages") {
								if (!items.Contains(year))
									items.Add(year);
							}
						}
						
					}
					break;
			}
			
			items.Sort();
			foreach(string item in items)
				listBox1.Items.Add(item);
			
			label1.Text = string.Empty;
		}
		
		public string LogCount(string logfile)
		{
			string strTofind = string.Empty;
			for (int c = 1; c < 301; c++) {
				if (c == 151) strTofind = strTofind + Environment.NewLine;
				strTofind = strTofind + "*";
			}
			
			return File.ReadAllText(logfile).CountStringOccurrences(strTofind).ToString();
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
	}
}

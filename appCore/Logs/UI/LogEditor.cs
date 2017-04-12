/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 20/03/2015
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using appCore.Templates;
using appCore.Templates.Types;
using appCore.Templates.UI;

namespace appCore.Logs.UI
{
	/// <summary>
	/// Description of LogEditor.
	/// </summary>
	public sealed partial class LogEditor : Form
	{
		MainForm myFormControl1;
		string GlobalLogType {
			get;
			set;
		}
		
		public static LogsCollection<Template> Logs;
		public static LogsCollection<Outage> OutageLogs;
		
		public static TroubleshootControls TroubleshootUI;
		public static FailedCRQControls FailedCRQUI;
		public static UpdateControls UpdateUI;
		public static TXControls TXUI;
		public static OutageControls OutageUI;
		
		public LogEditor(LogsCollection<Template> logs, MainForm myForm)
		{
			InitializeComponent();
			
//			myFormControl1 = myForm;
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
			
			GlobalLogType = "Templates";
			
			this.Height = 152;
			this.Text = "Log Editor - " + WindowTitle + " - " + GlobalLogType + " logs";
			
			listView1.View = View.Details;
			
			// Populate ListView1 with logs

			listView1.Columns.Add("Log Type").Width = -2;
			listView1.Columns.Add("INC").Width = -2;
			listView1.Columns.Add("Target").Width = -2;
			listView1.Columns.Add("Timestamp").Width = -2;
			
			var emptyList = new List<dynamic>();
			
			for(int c = 0; c < Logs.Count; c++) {
				switch (Logs[c].LogType) {
					case "Troubleshoot":
						Troubleshoot TSlog = new Troubleshoot();
						Toolbox.Tools.CopyProperties(TSlog, Logs[c]);
						emptyList.Add(new {
						              	LogType = "Troubleshoot Template",
						              	INC = TSlog.INC,
						              	Target = TSlog.SiteId,
						              	Timestamp = TSlog.GenerationDate.ToString("HH:mm:ss")
						              });
						listView1.Items.Add(new ListViewItem(
							new []{
								"Troubleshoot Template",
								TSlog.INC,
								TSlog.SiteId,
								TSlog.GenerationDate.ToString("HH:mm:ss")
							}
						));
						break;
					case "Failed CRQ":
						FailedCRQ FCRQlog = new FailedCRQ();
						Toolbox.Tools.CopyProperties(FCRQlog, Logs[c]);
						emptyList.Add(new {
						              	LogType = "Failed CRQ",
						              	INC = FCRQlog.INC,
						              	Target = FCRQlog.SiteId,
						              	Timestamp = FCRQlog.GenerationDate.ToString("HH:mm:ss")
						              });
						listView1.Items.Add(new ListViewItem(
							new []{
								"Failed CRQ",
								FCRQlog.INC,
								FCRQlog.SiteId,
								FCRQlog.GenerationDate.ToString("HH:mm:ss")
							}
						));
						break;
					case "TX":
						TX TXlog = new TX();
						Toolbox.Tools.CopyProperties(TXlog, Logs[c]);
						emptyList.Add(new {
						              	LogType = "TX Template",
						              	INC = "-",
						              	Target = TXlog.SiteIDs,
						              	Timestamp = TXlog.GenerationDate.ToString("HH:mm:ss")
						              });
						listView1.Items.Add(new ListViewItem(
							new []{
								"TX Template",
								"-",
								TXlog.SiteIDs,
								TXlog.GenerationDate.ToString("HH:mm:ss")
							}
						));
						break;
					case "Update":
						Update UPDlog = new Update();
						Toolbox.Tools.CopyProperties(UPDlog, Logs[c]);
						emptyList.Add(new {
						              	LogType = "Update Template",
						              	INC = UPDlog.INC,
						              	Target = UPDlog.SiteId,
						              	Timestamp = UPDlog.GenerationDate.ToString("HH:mm:ss")
						              });
						listView1.Items.Add(new ListViewItem(
							new []{
								"Update Template",
								UPDlog.INC,
								UPDlog.SiteId,
								UPDlog.GenerationDate.ToString("HH:mm:ss")
							}
						));
						break;
				}
				
				dataGridView1.DataSource = emptyList;
				dataGridView1.Columns["LogType"].HeaderText = "Log Type";
			}
		}
		
		public LogEditor(LogsCollection<Outage> logs, MainForm myForm)
		{
			InitializeComponent();
			
//			myFormControl1 = myForm;
			OutageLogs = logs;
			
			string WindowTitle = OutageLogs.logFileDate.Day.ToString();
			switch (OutageLogs.logFileDate.Day) {
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
			WindowTitle += OutageLogs.logFileDate.ToString("MMMM, yyyy",CultureInfo.GetCultureInfo("en-GB"));
			
			GlobalLogType = "Outages";
			
			this.Height = 152;
			this.Text = "Log Editor - " + WindowTitle + " - " + GlobalLogType + " logs";
			
			listView1.View = View.Details;
			
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
			
			var emptyList = new List<dynamic>();
			
			for (int c = 0; c < OutageLogs.Count; c++) {
				char VfReportExists = !string.IsNullOrEmpty(OutageLogs[c].VfOutage) ? '\u2714' : '\u2718';
				char TefReportExists = !string.IsNullOrEmpty(OutageLogs[c].TefOutage) ? '\u2714' : '\u2718';
				emptyList.Add(new {
				              	Timestamp = OutageLogs[c].GenerationDate.ToString(),
				              	Summary = OutageLogs[c].Summary,
				              	Gsm = OutageLogs[c].GsmCells.ToString(),
				              	Umts = OutageLogs[c].UmtsCells.ToString(),
				              	Lte = OutageLogs[c].LteCells.ToString(),
				              	EventTime = OutageLogs[c].EventTime.ToString(),
				              	VfReport = VfReportExists.ToString(),
				              	TfReport = TefReportExists.ToString()
				              });
				listView1.Items.Add(new ListViewItem(
					new []{
						OutageLogs[c].GenerationDate.ToString(),
						OutageLogs[c].Summary,
						OutageLogs[c].GsmCells.ToString(),
						OutageLogs[c].UmtsCells.ToString(),
						OutageLogs[c].LteCells.ToString(),
						OutageLogs[c].EventTime.ToString(),
						VfReportExists.ToString(),
						TefReportExists.ToString()
					}
				));
			}
			
			dataGridView1.DataSource = emptyList;
			dataGridView1.Columns["Gsm"].HeaderText = "2G";
			dataGridView1.Columns["Umts"].HeaderText = "3G";
			dataGridView1.Columns["Lte"].HeaderText = "4G";
			dataGridView1.Columns["EventTime"].HeaderText = "Event Time";
			dataGridView1.Columns["VfReport"].HeaderText = "VF Report";
			dataGridView1.Columns["TfReport"].HeaderText = "TF Report";
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
						break;
					case "Outages":
						if(OutageUI != null)
							OutageUI.Dispose();
						
						OutageUI = new OutageControls(OutageLogs[listView1.SelectedItems[0].Index]);
						OutageUI.Location = new System.Drawing.Point(0, listView1.Bottom + 10);
						this.Controls.Add(OutageUI);
						this.Size = new System.Drawing.Size(OutageUI.Right + 6, OutageUI.Bottom + 29);
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
			}
			this.ResumeLayout();
		}
		
		void LogEditorFormClosing(object sender, FormClosingEventArgs e)
		{
			var fc = Application.OpenForms.OfType<LogBrowser>().ToList();
			
			if(fc.Count > 0) {
				if(fc[0].WindowState == FormWindowState.Minimized)
					fc[0].WindowState = FormWindowState.Normal;
				fc[0].Activate();
			}
		}
		
		void LogEditorActivated(object sender, EventArgs e)
		{
//			FormCollection fc = Application.OpenForms;
//
//			foreach (Form frm in fc)
//			{
//				if (frm.Name == "appCore.UI.LargeTextForm" || frm.Name == "ScrollableMessageBox") {
//					frm.Activate();
//					return;
//				}
//			}
		}
		
		void DataGridView1SelectionChanged(object sender, EventArgs e)
		{
			this.SuspendLayout();
			List<DataGridViewRow> selectedRowsList = new List<DataGridViewRow>();
			foreach(DataGridViewCell cell in dataGridView1.SelectedCells) {
				if(!selectedRowsList.Contains(cell.OwningRow))
					selectedRowsList.Add(cell.OwningRow);
			}
			
			if(selectedRowsList.Count == 1) {
				switch(GlobalLogType) {
					case "Templates":
						switch (selectedRowsList[0].Cells["LogType"].Value.ToString()) {
							case "Troubleshoot Template":
								if(TroubleshootUI != null)
									TroubleshootUI.Dispose();
								
								TroubleshootUI = new TroubleshootControls(Logs[selectedRowsList[0].Index].ToTroubleShootTemplate());
								TroubleshootUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
								this.Controls.Add(TroubleshootUI);
								
								this.Size = new System.Drawing.Size(TroubleshootUI.Right + 6, TroubleshootUI.Bottom + 29);
								break;
							case "Failed CRQ":
								if(FailedCRQUI != null)
									FailedCRQUI.Dispose();
								
								FailedCRQUI = new FailedCRQControls(Logs[selectedRowsList[0].Index].ToFailedCRQTemplate());
								FailedCRQUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
								this.Controls.Add(FailedCRQUI);
								this.Size = new System.Drawing.Size(FailedCRQUI.Right + 6, FailedCRQUI.Bottom + 29);
								break;
							case "TX Template":
								if(TXUI != null)
									TXUI.Dispose();
								
								TXUI = new TXControls(Logs[selectedRowsList[0].Index].ToTXTemplate());
								TXUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
								this.Controls.Add(TXUI);
								this.Size = new System.Drawing.Size(TXUI.Right + 6, TXUI.Bottom + 29);
								break;
							case "Update Template":
								if(UpdateUI != null)
									UpdateUI.Dispose();
								
								UpdateUI = new UpdateControls(Logs[selectedRowsList[0].Index].ToUpdateTemplate());
								UpdateUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
								this.Controls.Add(UpdateUI);
								this.Size = new System.Drawing.Size(UpdateUI.Right + 6, UpdateUI.Bottom + 29);
								break;
						}
						break;
					case "Outages":
						if(OutageUI != null)
							OutageUI.Dispose();
						
						OutageUI = new OutageControls(OutageLogs[selectedRowsList[0].Index]);
						OutageUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
						this.Controls.Add(OutageUI);
						this.Size = new System.Drawing.Size(OutageUI.Right + 6, OutageUI.Bottom + 29);
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
			}
			this.ResumeLayout();
		}
		
		void Form_Resize(object sender, EventArgs e)
		{
			// Fire Resize event to check if the window was minimized and minimize LogBrowser as well
//			if (WindowState == FormWindowState.Minimized)
//			{
			////				Form frm = (Form)this.Parent;
//				Form frm = this.Owner;
//				// FIXME: App crashes when minimizing both windows
//				frm.WindowState = FormWindowState.Minimized;
//				frm.Invoke((MethodInvoker)delegate
//				           {
//				           	frm.WindowState = FormWindowState.Minimized;
//				           });
			////				Invoke(new Delegate(WindowState = FormWindowState.Minimized;
//			}
		}
	}
}

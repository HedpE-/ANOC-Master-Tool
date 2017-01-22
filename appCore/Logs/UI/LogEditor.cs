/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 20/03/2015
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Threading;
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
		public static LogsCollection<Outage> OutageLogs;
		public static TroubleshootControls TroubleshootUI;
		public static FailedCRQControls FailedCRQUI;
		public static UpdateControls UpdateUI;
		public static TXControls TXUI;
		public static OutageControls OutageUI;
		MainForm myFormControl1;
		
		public LogEditor2(LogsCollection<Template> logs, MainForm myForm)
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
			
			GlobalLogType = "Templates";
			
			this.Height = 152;
			this.Text = "Log Editor - " + WindowTitle + " - " + GlobalLogType + " logs";
			
			listView1.View = View.Details;
			
			// Populate ListView1 with logs

			listView1.Columns.Add("Log Type").Width = -2;
			listView1.Columns.Add("INC").Width = -2;
			listView1.Columns.Add("Target").Width = -2;
			listView1.Columns.Add("Timestamp").Width = -2;
			
			for (int c = 0; c < Logs.Count; c++) {
				switch (Logs[c].LogType) {
					case "Troubleshoot":
						TroubleShoot TSlog = new TroubleShoot();
						Toolbox.Tools.CopyProperties(TSlog, Logs[c]);
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
			}
		}
		
		public LogEditor2(LogsCollection<Outage> logs, MainForm myForm)
		{
			InitializeComponent();
			
			myFormControl1 = myForm;
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
			
			for (int c = 0; c < OutageLogs.Count; c++) {
				char VfReportExists = !string.IsNullOrEmpty(OutageLogs[c].VfOutage) ? '\u2714' : '\u2718';
				char TefReportExists = !string.IsNullOrEmpty(OutageLogs[c].TefOutage) ? '\u2714' : '\u2718';
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
			FormCollection fc = Application.OpenForms;

			foreach (Form frm in fc)
			{
				if (frm.Name == "LogBrowser") {
					frm.Activate();
					return;
				}
			}
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

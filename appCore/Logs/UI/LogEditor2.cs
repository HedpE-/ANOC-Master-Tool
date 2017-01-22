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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using appCore.Templates;
using appCore.Templates.UI;
using appCore.Templates.Types;
using appCore.UI;

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
//				string[] strTofind = { "\r\n" };
//				string[] log = Logs[c].Split(strTofind, StringSplitOptions.None);
//				strTofind[0] = " - ";
//
				switch (Logs[c].LogType) {
					case "Troubleshoot":
						TroubleShoot TSlog = new TroubleShoot();
						Toolbox.Tools.CopyProperties(TSlog, Logs[c]);
						listView1.Items.Add(new ListViewItem(new []{"Troubleshoot Template",TSlog.INC,TSlog.SiteId,TSlog.GenerationDate.ToString("HH:mm:ss")}));
						break;
					case "Failed CRQ":
						FailedCRQ FCRQlog = new FailedCRQ();
						Toolbox.Tools.CopyProperties(FCRQlog, Logs[c]);
						listView1.Items.Add(new ListViewItem(new []{"Failed CRQ",FCRQlog.INC,FCRQlog.SiteId,FCRQlog.GenerationDate.ToString("HH:mm:ss")}));
						break;
					case "TX":
						TX TXlog = new TX();
						Toolbox.Tools.CopyProperties(TXlog, Logs[c]);
						listView1.Items.Add(new ListViewItem(new []{"TX Template","-",TXlog.SiteIDs,TXlog.GenerationDate.ToString("HH:mm:ss")}));
						break;
					case "Update":
						Update UPDlog = new Update();
						Toolbox.Tools.CopyProperties(UPDlog, Logs[c]);
						listView1.Items.Add(new ListViewItem(new []{"Update Template",UPDlog.INC,UPDlog.SiteId,UPDlog.GenerationDate.ToString("HH:mm:ss")}));
						break;
				}
			}
		}
		
		public LogEditor2(LogsCollection<Outage> logs, MainForm myForm)
		{
			InitializeComponent();
			
			myFormControl1 = myForm;
			OutageLogs = logs;
			
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
			
			for (int c = 0; c < Logs.Count; c++) {
				Outage OutageLog = new Outage();
				Toolbox.Tools.CopyProperties(OutageLog, Logs[c]);
				char VfReportExists = string.IsNullOrEmpty(OutageLog.VfOutage) ? '\u2714' : '\u2718';
				char TefReportExists = string.IsNullOrEmpty(OutageLog.VfOutage) ? '\u2714' : '\u2718';
				listView1.Items.Add(new ListViewItem(new []{OutageLog.GenerationDate.ToString(), OutageLog.Summary, OutageLog.GsmCells.ToString(), OutageLog.UmtsCells.ToString(), OutageLog.LteCells.ToString(), OutageLog.EventTime.ToString(), VfReportExists.ToString(), TefReportExists.ToString()}));
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
//						button10.Visible = false;
//						button11.Visible = false;
//						button12.Visible = false;
						break;
					case "Outages":
						if(OutageUI != null)
							OutageUI.Dispose();
						
						OutageUI = new OutageControls(OutageLogs[listView1.SelectedItems[0].Index]);
						OutageUI.Location = new System.Drawing.Point(0, listView1.Bottom + 10);
						this.Controls.Add(UpdateUI);
						this.Size = new System.Drawing.Size(UpdateUI.Right + 6, UpdateUI.Bottom + 29);
//						
//						button10.Visible = false;
//						button12.Visible = false;
//						button11.Visible = false;
//						button11.Text = "Generate sites list";
//						button11.Size = new System.Drawing.Size(99, 23);
//						button11.Location = new System.Drawing.Point(434, 707);
//						LoadOutages();
//						button10.Text = "Copy Outage";
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
//				button10.Visible = false;
//				button11.Visible = false;
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

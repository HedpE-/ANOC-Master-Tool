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
using appCore.Templates.RAN.Types;
using appCore.Templates.RAN.UI;

namespace appCore.Logs.UI
{
	/// <summary>
	/// Description of LogEditor.
	/// </summary>
	public sealed partial class LogEditor : Form
	{
		string GlobalLogType { get; set; }
		
		public static LogsCollection<Template> Logs;
		public static LogsCollection<Outage> OutageLogs;
		
		public static TroubleshootControls TroubleshootUI;
		public static FailedCRQControls FailedCRQUI;
		public static UpdateControls UpdateUI;
		public static TxControls TXUI;
		public static OutageControls OutageUI;
		
		public LogEditor(LogsCollection<Template> logs)
		{
			InitializeComponent();
			
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
			WindowTitle += Logs.logFileDate.ToString("MMMM, yyyy", CultureInfo.GetCultureInfo("en-GB"));// + ", " + Logs.logFileDate.Year;
			
			GlobalLogType = "Templates";
			
			this.Height = 164;
			this.Text = "Log Editor - " + WindowTitle + " - " + GlobalLogType + " logs";
			


			var logsList = new List<dynamic>();
			
			for(int c = 0; c < Logs.Count; c++)
            {
				switch (Logs[c].LogType)
                {
					case TemplateTypes.Troubleshoot:
						Troubleshoot TSlog = new Troubleshoot();
						Toolbox.Tools.CopyProperties(TSlog, Logs[c]);
						logsList.Add(new {
						             	LogType = Logs[c].LogType.GetDescription(),
						             	INC = TSlog.INC,
						             	Target = TSlog.SiteId,
						             	Timestamp = TSlog.GenerationDate.ToString("HH:mm:ss")
						             });
						break;
					case TemplateTypes.FailedCRQ:
						FailedCRQ FCRQlog = new FailedCRQ();
						Toolbox.Tools.CopyProperties(FCRQlog, Logs[c]);
						logsList.Add(new {
						             	LogType = Logs[c].LogType.GetDescription(),
						             	INC = FCRQlog.INC,
						             	Target = FCRQlog.SiteId,
						             	Timestamp = FCRQlog.GenerationDate.ToString("HH:mm:ss")
						             });
						break;
					case TemplateTypes.TX:
						TX TXlog = new TX();
						Toolbox.Tools.CopyProperties(TXlog, Logs[c]);
						logsList.Add(new {
						             	LogType = Logs[c].LogType.GetDescription(),
						             	INC = "-",
						             	Target = TXlog.SiteIDs,
						             	Timestamp = TXlog.GenerationDate.ToString("HH:mm:ss")
						             });
						break;
					case TemplateTypes.Update:
						Update UPDlog = new Update();
						Toolbox.Tools.CopyProperties(UPDlog, Logs[c]);
						logsList.Add(new {
						             	LogType = Logs[c].LogType.GetDescription(),
						             	INC = UPDlog.INC,
						             	Target = UPDlog.SiteId,
						             	Timestamp = UPDlog.GenerationDate.ToString("HH:mm:ss")
						             });
						break;
				}
				
				dataGridView1.DataSource = logsList;
				dataGridView1.Columns["LogType"].HeaderText = "Log Type";
			}
		}
		
		public LogEditor(LogsCollection<Outage> logs)
		{
			InitializeComponent();
			
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
			
			this.Height = 164;
			this.Text = "Log Editor - " + WindowTitle + " - " + GlobalLogType + " logs";
			
			var logsList = new List<dynamic>();
			
			for (int c = 0; c < OutageLogs.Count; c++)
            {
				char VfReportExists = !string.IsNullOrEmpty(OutageLogs[c].VfOutage) ? '\u2714' : '\u2718';
				char TefReportExists = !string.IsNullOrEmpty(OutageLogs[c].TefOutage) ? '\u2714' : '\u2718';
				logsList.Add(new {
				             	Timestamp = OutageLogs[c].GenerationDate.ToString("HH:mm:ss"),
				             	Summary = OutageLogs[c].Summary,
				             	Gsm = OutageLogs[c].GsmCells.ToString(),
				             	Umts = OutageLogs[c].UmtsCells.ToString(),
				             	Lte = OutageLogs[c].LteCells.ToString(),
				             	EventTime = OutageLogs[c].EventTime.ToString(),
				             	VfReport = VfReportExists.ToString(),
				             	TfReport = TefReportExists.ToString()
				             });
			}
			
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			dataGridView1.DataSource = logsList;
			
			for(int c = 0;c < dataGridView1.Columns.Count;c++)
            {
				switch(dataGridView1.Columns[c].Name)
                {
					case "Timestamp":
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns[c].Width = 70;
						break;
					case "Gsm":
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns[c].HeaderText = "2G";
						dataGridView1.Columns[c].Width = 32;
						break;
					case "Umts":
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns[c].HeaderText = "3G";
						dataGridView1.Columns[c].Width = 32;
						break;
					case "Lte":
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns[c].HeaderText = "4G";
						dataGridView1.Columns[c].Width = 32;
						break;
					case "EventTime":
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
						dataGridView1.Columns[c].HeaderText = "Event Time";
						break;
					case "VfReport":
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
						dataGridView1.Columns[c].HeaderText = "VF Report";
						dataGridView1.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						break;
					case "TfReport":
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
						dataGridView1.Columns[c].HeaderText = "TF Report";
						dataGridView1.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						break;
					default:
						dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
						break;
						
				}
			}
        }

        private async void LogEditor_Shown(object sender, EventArgs e)
        {

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
			if(dataGridView1.SelectedRows.Count > 0)
            {
				if(dataGridView1.SelectedRows.Count == 1)
                {
                    appCore.UI.LoadingPanel loading = new appCore.UI.LoadingPanel();
                    loading.Show(false, this);

                    if (TroubleshootUI != null)
					    TroubleshootUI.Dispose();
					if(FailedCRQUI != null)
					    FailedCRQUI.Dispose();
					if(UpdateUI != null)
					    UpdateUI.Dispose();
					if(TXUI != null)
					    TXUI.Dispose();
					if(OutageUI != null)
					    OutageUI.Dispose();

					switch(GlobalLogType) {
					    case "Templates":
					        switch ((EnumExtensions.Parse(typeof(TemplateTypes), dataGridView1.SelectedRows[0].Cells["LogType"].Value.ToString()))) {
					            case TemplateTypes.Troubleshoot:
					                TroubleshootUI = new TroubleshootControls(Logs[dataGridView1.SelectedRows[0].Index].ToTroubleshootTemplate());
					                TroubleshootUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
					                this.Controls.Add(TroubleshootUI);					                                      					
					                //this.Height = TroubleshootUI.Bottom + 29;
					                break;
					            case TemplateTypes.FailedCRQ:
					                FailedCRQUI = new FailedCRQControls(Logs[dataGridView1.SelectedRows[0].Index].ToFailedCRQTemplate());
					                FailedCRQUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
					                this.Controls.Add(FailedCRQUI);
					                //this.Height = FailedCRQUI.Bottom + 29;
					                break;
					            case TemplateTypes.TX:
					                TXUI = new TxControls(Logs[dataGridView1.SelectedRows[0].Index].ToTxTemplate());
					                TXUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
					                this.Controls.Add(TXUI);
					                //this.Height = TXUI.Bottom + 29;
					                break;
					            case TemplateTypes.Update:
					                UpdateUI = new UpdateControls(Logs[dataGridView1.SelectedRows[0].Index].ToUpdateTemplate());
					                UpdateUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
					                this.Controls.Add(UpdateUI);
                                    //this.Height = UpdateUI.Bottom + 29;
					                break;
					        }
					        break;
					    case "Outages":
					        OutageUI = new OutageControls(OutageLogs[dataGridView1.SelectedRows[0].Index]);
					        OutageUI.Location = new System.Drawing.Point(0, dataGridView1.Bottom + 10);
					        this.Controls.Add(OutageUI);
                            //this.Height = OutageUI.Bottom + 29;
                            break;
                    }
                    var ct = Controls.Cast<Control>().FirstOrDefault(c => c.Name.EndsWith("GUI"));
                    this.Height = ct.Bottom + 45;
					loading.Close();
				}
			}
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

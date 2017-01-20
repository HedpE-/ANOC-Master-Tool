/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 15:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using appCore.Netcool;
using appCore.SiteFinder;
using FileHelpers;

namespace appCore.Templates.Types
{
	/// <summary>
	/// Description of Outage.
	/// </summary>
	public class Outage : Template
	{
		public string VfOutage;
		public string VfBulkCI;
		public string TefOutage;
		public string TefBulkCI;
		
		List<string> VfSites = new List<string>();
		List<string> VfGsmCells = new List<string>();
		List<string> VfUmtsCells = new List<string>();
		List<string> VfLteCells = new List<string>();
		List<string> VfLocations = new List<string>();
		DateTime VfGsmTime = new DateTime(2500,1,1);
		DateTime VfUmtsTime = new DateTime(2500,1,1);
		DateTime VfLteTime = new DateTime(2500,1,1);
		List<string> TefSites = new List<string>();
		List<string> TefGsmCells = new List<string>();
		List<string> TefUmtsCells = new List<string>();
		List<string> TefLteCells = new List<string>();
		List<string> TefLocations = new List<string>();
		DateTime TefGsmTime = new DateTime(2500,1,1);
		DateTime TefUmtsTime = new DateTime(2500,1,1);
		DateTime TefLteTime = new DateTime(2500,1,1);
		
		List<Alarm> OutageAlarms;
		
//		List<Alarm> VfAlarms {
//			get {
//				return OutageAlarms.FindAll(s => s.Operator == "VF");
//			}
//		}
//		List<Alarm> TefAlarms {
//			get {
//				return OutageAlarms.FindAll(s => s.Operator == "TEF");
//			}
//		}
		
		public Outage(AlarmsParser alarms) {
//			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
//			st.Start();
			OutageAlarms = alarms.AlarmsList;
			
			List<Cell> LTEcells = new List<Cell>();
			
//			TimeSpan t2;
			if(alarms.lteSitesOnM.Count > 0) {
				LTEcells = Finder.getCells(alarms.lteSitesOnM, "4G");
//				System.Diagnostics.Stopwatch st2 = new System.Diagnostics.Stopwatch();
//				st2.Start();
				
				foreach(Cell cell in LTEcells) {
					Alarm temp = OutageAlarms.Find(a => a.SiteId == cell.ParentSite);
					OutageAlarms.Add(new Alarm(cell, true, temp));
				}
//				st2.Stop();
//				t2 = st2.Elapsed;
			}
			string toparse = string.Empty;
			try {
				var engine = new FileHelperEngine<Alarm>();

				engine.BeforeWriteRecord += (eng, e) => {
					if(e.Record.OnM && !e.Record.COOS)
						e.SkipThisRecord = true;
				};

				toparse = engine.WriteString(OutageAlarms);
			}
			catch(FileHelpersException e) {
				var m = e.Message;
			}
			
			try {
				var engine = new FileHelperEngine<Alarm>();
				engine.AfterReadRecord += (eng, e) => {
					string temp;
					if(e.Record.Bearer == "4G")
						temp = e.Record.Element;
					else
						temp = e.Record.RncBsc + " - " + e.Record.Element;
					string tempSite = string.IsNullOrEmpty(e.Record.POC) ? e.Record.Location : e.Record.Location + " - " + e.Record.POC;
					switch (e.Record.Operator) {
						case "VF":
							if(string.IsNullOrEmpty(e.Record.County)) {
								if(string.IsNullOrEmpty(e.Record.Town)) {
									if(!VfLocations.Contains(e.Record.ParentSite.County))
										VfLocations.Add(e.Record.ParentSite.County);
								}
								else
									if(!VfLocations.Contains(e.Record.Town))
										VfLocations.Add(e.Record.Town);
							}
							else
								if(!VfLocations.Contains(e.Record.County))
									VfLocations.Add(e.Record.County);
							if(!VfSites.Contains(tempSite))
								VfSites.Add(tempSite);
							switch(e.Record.Bearer) {
								case "2G":
									if(!VfGsmCells.Contains(temp))
										VfGsmCells.Add(temp);
									if(e.Record.LastOccurrence < VfGsmTime)
										VfGsmTime = e.Record.LastOccurrence;
									break;
								case "3G":
									if(!VfUmtsCells.Contains(temp))
										VfUmtsCells.Add(temp);
									if(e.Record.LastOccurrence < VfUmtsTime)
										VfUmtsTime = e.Record.LastOccurrence;
									break;
								case "4G":
									if(!VfLteCells.Contains(temp))
										VfLteCells.Add(temp);
									if(e.Record.LastOccurrence < VfLteTime)
										VfLteTime = e.Record.LastOccurrence;
									break;
							}
							break;
						case "TEF":
							if(string.IsNullOrEmpty(e.Record.County)) {
								if(string.IsNullOrEmpty(e.Record.Town)) {
									if(!TefLocations.Contains(e.Record.ParentSite.County))
										TefLocations.Add(e.Record.ParentSite.County);
								}
								else
									if(!TefLocations.Contains(e.Record.Town))
										TefLocations.Add(e.Record.Town);
							}
							else
								if(!TefLocations.Contains(e.Record.County))
									TefLocations.Add(e.Record.County);
							if(!TefSites.Contains(tempSite))
								TefSites.Add(tempSite);
							switch(e.Record.Bearer) {
								case "2G":
									if(!TefGsmCells.Contains(temp))
										TefGsmCells.Add(temp);
									if(e.Record.LastOccurrence < TefGsmTime)
										TefGsmTime = e.Record.LastOccurrence;
									break;
								case "3G":
									if(!TefUmtsCells.Contains(temp))
										TefUmtsCells.Add(temp);
									if(e.Record.LastOccurrence < TefUmtsTime)
										TefUmtsTime = e.Record.LastOccurrence;
									break;
								case "4G":
									if(!TefLteCells.Contains(temp))
										TefLteCells.Add(temp);
									if(e.Record.LastOccurrence < TefLteTime)
										TefLteTime = e.Record.LastOccurrence;
									break;
							}
							break;
					}
				};
				
				OutageAlarms = engine.ReadStringAsList(toparse);
			}
			catch(FileHelpersException e) {
				var m = e.Message;
			}
			
//			st.Stop();
//			var t = st.Elapsed;
			
			generateReports();
			
			LogType = "Outage";
			fullLog = generateFullLog();
		}
		
		public Outage(List<string> Sites) {
			List<Site> sites = Finder.getSites(Sites);
			List<Cell> cells = Finder.getCells(Sites);
			foreach (Cell cell in cells) {
				Site tempSite = sites.Find(s => s.Id == cell.ParentSite);
				string cellString;
				string tempSiteString = cell.ParentSite;
				while(tempSiteString.Length < 5)
					tempSiteString = "0" + tempSiteString;
				tempSiteString = "RBS" + tempSiteString;
				if(cell.Bearer == "4G")
					cellString = cell.Name;
				else
					cellString = cell.BscRnc_Id + " - " + cell.Name;
				switch(cell.Operator) {
					case "VF":
						if(!VfSites.Contains(tempSiteString))
							VfSites.Add(tempSiteString);
						switch(cell.Bearer) {
							case "2G":
								if(!VfGsmCells.Contains(cellString))
									VfGsmCells.Add(cellString);
								break;
							case "3G":
								if(!VfUmtsCells.Contains(cellString))
									VfUmtsCells.Add(cellString);
								break;
							case "4G":
								if(!VfLteCells.Contains(cellString))
									VfLteCells.Add(cellString);
								break;
						}
						if(!VfLocations.Contains(tempSite.Town))
							VfLocations.Add(tempSite.Town);
						break;
					case "TEF":
						if(!TefSites.Contains(cell.ParentSite))
							TefSites.Add(tempSiteString);
						switch(cell.Bearer) {
							case "2G":
								if(!TefGsmCells.Contains(cellString))
									TefGsmCells.Add(cellString);
								break;
							case "3G":
								if(!TefUmtsCells.Contains(cellString))
									TefUmtsCells.Add(cellString);
								break;
							case "4G":
								if(!TefLteCells.Contains(cellString))
									TefLteCells.Add(cellString);
								break;
						}
						if(!TefLocations.Contains(tempSite.Town))
							TefLocations.Add(tempSite.Town);
						break;
				}
			}
			showIncludeListForm();
			generateReports();
			
			LogType = "Outage";
			fullLog = generateFullLog();
		}
		
		public Outage(Outage existingOutage) {
			Toolbox.Tools.CopyProperties(this, existingOutage);
			fullLog = generateFullLog();
			LogType = "Outage";
		}
		
		public Outage(string[] log, DateTime date) {
			LoadOutageReport(log);
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = "Outage";
		}
		
		void generateReports() {
			int cellTotal = VfGsmCells.Count + VfUmtsCells.Count + VfLteCells.Count;
			if(cellTotal > 0) {
				VfLocations.Sort();
				VfSites.Sort();
				VfGsmCells.Sort();
				VfUmtsCells.Sort();
				VfLteCells.Sort();
				VfOutage = cellTotal + "x COOS (" + VfSites.Count;
				VfOutage += VfSites.Count == 1 ? " Site)" : " Sites)";
				VfOutage += Environment.NewLine + Environment.NewLine + "Locations (" + VfLocations.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine, VfLocations) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine, VfSites);
				
				if ( VfGsmCells.Count > 0 )
					VfOutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + VfGsmCells.Count + ") Event Time - " + VfGsmTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfGsmCells);
				if ( VfUmtsCells.Count > 0)
					VfOutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + VfUmtsCells.Count + ") Event Time - " + VfUmtsTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfUmtsCells);
				if ( VfLteCells.Count > 0)
					VfOutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + VfLteCells.Count + ") Event Time - " + VfLteTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, VfLteCells);
				
				for(int c = 0;c < VfSites.Count;c++) {
					string[] strToFind = { " - " };
					string tempSite = VfSites[c].Split(strToFind, StringSplitOptions.None)[0];
					tempSite = Convert.ToInt32(tempSite.RemoveLetters()).ToString();
					while(tempSite.Length < 4)
						tempSite = "0" + tempSite;
					VfBulkCI += tempSite + ";";
					if(c > 0 && c % 50 == 0)
						VfBulkCI += Environment.NewLine + Environment.NewLine;
				}
			}
			
			cellTotal = TefGsmCells.Count + TefUmtsCells.Count + TefLteCells.Count;
			if(cellTotal > 0) {
				TefLocations.Sort();
				TefSites.Sort();
				TefGsmCells.Sort();
				TefUmtsCells.Sort();
				TefLteCells.Sort();
				TefOutage = cellTotal + "x COOS (" + TefSites.Count;
				TefOutage += TefSites.Count == 1 ? " Site)" : " Sites)";
				TefOutage += Environment.NewLine + Environment.NewLine + "Locations (" + TefLocations.Count + ")" + Environment.NewLine + string.Join(Environment.NewLine, TefLocations) + Environment.NewLine + Environment.NewLine + "Site List" + Environment.NewLine + string.Join(Environment.NewLine, TefSites);
				
				if ( TefGsmCells.Count > 0 )
					TefOutage += Environment.NewLine + Environment.NewLine + "2G Cells (" + TefGsmCells.Count + ") Event Time - " + TefGsmTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, TefGsmCells);
				if ( TefUmtsCells.Count > 0)
					TefOutage += Environment.NewLine + Environment.NewLine + "3G Cells (" + TefUmtsCells.Count + ") Event Time - " + TefUmtsTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, TefUmtsCells);
				if ( TefLteCells.Count > 0)
					TefOutage += Environment.NewLine + Environment.NewLine + "4G Cells (" + TefLteCells.Count + ") Event Time - " + TefLteTime.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + string.Join(Environment.NewLine, TefLteCells);
				
				for(int c = 0;c < TefSites.Count;c++) {
					string[] strToFind = { " - " };
					string tempSite = TefSites[c].Split(strToFind, StringSplitOptions.None)[0];
					tempSite = Convert.ToInt32(tempSite.RemoveLetters()).ToString();
					while(tempSite.Length < 4)
						tempSite = "0" + tempSite;
					TefBulkCI += tempSite + ";";
					if(c > 0 && ((c + 1) % 50) == 0)
						TefBulkCI += Environment.NewLine + Environment.NewLine;
				}
			}
		}
		
		string generateFullLog() {
			fullLog += DateTime.Now.ToString("HH:mm:ss");
			if(!string.IsNullOrEmpty(VfOutage)) {
				fullLog += Environment.NewLine + "----------VF Report----------" + Environment.NewLine;
				fullLog += VfOutage + Environment.NewLine;
				fullLog += "-----BulkCI-----" + Environment.NewLine;
				fullLog += VfBulkCI;
			}
			if(!string.IsNullOrEmpty(TefOutage)) {
				fullLog += Environment.NewLine + "----------TF Report----------" + Environment.NewLine;
				fullLog += TefOutage + Environment.NewLine;
				fullLog += "-----BulkCI-----" + Environment.NewLine;
				fullLog += TefBulkCI;
			}
			
			return string.Empty;
		}
		
		public void LoadOutageReport(string[] log)
		{
//			int c = 0;
//			VFoutage = string.Empty;
//			TFoutage = string.Empty;
//			VFbulkCI = string.Empty;
//			TFbulkCI = string.Empty;
//			string[] strTofind = { "\r\n" };
//			string[] log = globalLogs[listView1.SelectedItems[0].Index].Split(strTofind, StringSplitOptions.None);
//
//			if(string.IsNullOrEmpty(log[log.Length - 1])) {
//				log = log.Where((source, index) => index != log.Length - 1).ToArray();
//			}
//
//			// Manipulate log array to make it compatible with VF/TF new logs
//			if(Array.FindIndex(log,element => element.Contains("F Report----------")) == -1) {
//				List<string> log2 = log.ToList(); // Create new List with log array values
//				string Report = log2[1]; // Store outage report to string
//				log2.RemoveAt(1); // Remove outage report previously stored on Report string
//				string[] SplitReport = Report.Split('\n'); // Split Report string to new array
//				log2.Insert(1,"----------VF Report----------"); // Insert VF Report header to match code checks
//				log2.InsertRange(2,SplitReport); // Insert SplitReport array into list after header
//				log = log2.ToArray(); // Replace original log array with new generated List values
//				if(Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal)) > -1) // Check if log contains LTE sites
//					log[Array.FindIndex(log, element => element.Equals("-----LTE sites-----", StringComparison.Ordinal))] = "----------LTE sites----------"; // Convert header to match code checks
//			}
//
//			int VFreportIndex = Array.FindIndex(log, element => element.Equals("----------VF Report----------", StringComparison.Ordinal));
//			int VFbulkciIndex = Array.FindIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
//			int TFreportIndex = Array.FindIndex(log, element => element.Equals("----------TF Report----------", StringComparison.Ordinal));
//			int TFbulkciIndex = Array.FindLastIndex(log, element => element.Equals("-----BulkCI-----", StringComparison.Ordinal));
//
//			if(VFreportIndex > -1) {
//				for(c = VFreportIndex + 1;c < VFbulkciIndex;c++) {
//					VFoutage += log[c];
//					if(c < VFbulkciIndex - 1)
//						VFoutage += Environment.NewLine;
//				}
//
//				if(TFreportIndex == -1) {
//					TFreportIndex = log.Length;
//				}
//
//				for(c = VFbulkciIndex + 1;c < TFreportIndex;c++) {
//					VFbulkCI += log[c];
//					if(c < TFreportIndex - 1)
//						VFbulkCI += Environment.NewLine;
//				}
//			}
//
//			if(TFreportIndex == log.Length)
//				TFreportIndex--;
//
//			if(log[TFreportIndex].Equals("----------TF Report----------")) {
//				for(c = TFreportIndex + 1;c < TFbulkciIndex;c++) {
//					TFoutage += log[c];
//					if(c < TFbulkciIndex - 1)
//						TFoutage += Environment.NewLine;
//				}
//
//				for(c = TFbulkciIndex + 1;c < log.Length;c++) {
//					TFbulkCI += log[c];
//					if(c < log.Length - 1)
//						VFbulkCI += Environment.NewLine;
//				}
//			}
//
//			if(!string.IsNullOrEmpty(VFoutage) && !string.IsNullOrEmpty(TFoutage)) {
//				tabControl1.Visible = true;
//				tabControl1.SelectTab(0);
//			}
//			else {
//				if(!string.IsNullOrEmpty(VFoutage)) {
//					tabControl1.Visible = false;
//					tabControl1.SelectTab(0);
//				}
//				else {
//					if(!string.IsNullOrEmpty(TFoutage)) {
//						tabControl1.Visible = false;
//						tabControl1.SelectTab(1);
//					}
//				}
//			}
//			TabControl4SelectedIndexChanged(null,null);
		}

		void showIncludeListForm() {
			List<string[]> includeList = new List<string[]>();
			Form form = new Form();
			using (form) {
				// 
				// cb2G
				// 
				CheckBox cb2G = new CheckBox();
				cb2G.Location = new System.Drawing.Point(3, 34);
				cb2G.Name = "cb2G";
				cb2G.Size = new System.Drawing.Size(42, 20);
				cb2G.TabIndex = 0;
				cb2G.Text = "2G";
				cb2G.Enabled = VfGsmCells.Any() || TefGsmCells.Any();
				cb2G.CheckedChanged += IncludeListForm_cbCheckedChanged;
				// 
				// cb3G
				// 
				CheckBox cb3G = new CheckBox();
				cb3G.Location = new System.Drawing.Point(3, 60);
				cb3G.Name = "cb3G";
				cb3G.Size = new System.Drawing.Size(42, 20);
				cb3G.TabIndex = 2;
				cb3G.Text = "3G";
				cb3G.Enabled = VfUmtsCells.Any() || TefUmtsCells.Any();
				cb3G.CheckedChanged += IncludeListForm_cbCheckedChanged;
				// 
				// cb4G
				// 
				CheckBox cb4G = new CheckBox();
				cb4G.Location = new System.Drawing.Point(3, 86);
				cb4G.Name = "cb4G";
				cb4G.Size = new System.Drawing.Size(42, 20);
				cb4G.TabIndex = 4;
				cb4G.Text = "4G";
				cb4G.Enabled = VfLteCells.Any() || TefLteCells.Any();
				cb4G.CheckedChanged += IncludeListForm_cbCheckedChanged;
				// 
				// continueButton
				// 
				Button continueButton = new Button();
				continueButton.Location = new System.Drawing.Point(3, 112);
				continueButton.Name = "continueButton";
				continueButton.Size = new System.Drawing.Size(221, 23);
				continueButton.TabIndex = 6;
				continueButton.Text = "Continue";
				continueButton.Click += IncludeListForm_buttonClick;
				// 
				// dtp2G
				// 
				DateTimePicker dtp2G = new DateTimePicker();
				dtp2G.Checked = false;
				dtp2G.CustomFormat = "dd/MM/yyyy HH:mm";
				dtp2G.Format = DateTimePickerFormat.Custom;
				dtp2G.Location = new System.Drawing.Point(51, 34);
				dtp2G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp2G.Name = "dtp2G";
				dtp2G.Size = new System.Drawing.Size(173, 20);
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
				dtp3G.Location = new System.Drawing.Point(51, 60);
				dtp3G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp3G.Name = "dtp3G";
				dtp3G.Size = new System.Drawing.Size(173, 20);
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
				dtp4G.Location = new System.Drawing.Point(51, 86);
				dtp4G.MinDate = new DateTime(2010, 1, 1, 0, 0, 0, 0);
				dtp4G.Name = "dtp4G";
				dtp4G.Size = new System.Drawing.Size(173, 20);
				dtp4G.TabIndex = 5;
				dtp4G.Value = DateTime.Now;
				dtp4G.Visible = false;
				// 
				// IncludeListForm_label
				// 
				Label IncludeListForm_label = new Label();
				IncludeListForm_label.Location = new System.Drawing.Point(3, 2);
				IncludeListForm_label.Name = "label";
				IncludeListForm_label.Size = new System.Drawing.Size(221, 29);
				IncludeListForm_label.Text = "Which Technologies do you wish to include?";
				IncludeListForm_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
				// 
				// Form1
				// 
				form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				form.AutoScaleMode = AutoScaleMode.Font;
				form.ClientSize = new System.Drawing.Size(228, 137);
				form.Icon = appCore.UI.Resources.MB_0001_vodafone3;
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
				
				if(cb2G.Checked)
					VfGsmTime = TefGsmTime = dtp2G.Value;
				if(cb3G.Checked)
					VfUmtsTime = TefUmtsTime = dtp3G.Value;
				if(cb4G.Checked)
					VfLteTime = TefLteTime = dtp4G.Value;
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

		void IncludeListForm_buttonClick(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			Form form = (Form)btn.Parent;
			if(btn.Text == "Cancel")
				form.Controls["sitesList_tb"].Text = string.Empty;
			form.Close();
		}
	}
}
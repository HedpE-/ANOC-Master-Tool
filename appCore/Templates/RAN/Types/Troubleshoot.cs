﻿/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 15:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace appCore.Templates.RAN.Types
{
	/// <summary>
	/// Description of Troubleshoot.
	/// </summary>
	[Serializable]
	public class Troubleshoot : Template
	{
		string inc = string.Empty;
		public string INC { get { return inc; } protected set { inc = value; } }
		string siteId = string.Empty;
		public string SiteId { get { return siteId; } protected set { siteId = value; } }
		string owner = string.Empty;
		public string SiteOwner { get { return owner; } protected set { owner = value; } }
		string tefSite = string.Empty;
		public string TefSiteId { get { return tefSite; } protected set { tefSite = value; } }
		string address = string.Empty;
		public string SiteAddress { get { return address; } protected set { address = value; } }
		bool otherSitesImpacted;
		public bool OtherSitesImpacted { get { return otherSitesImpacted; } protected set { otherSitesImpacted = value; } }
		bool coos;
		public bool COOS { get { return coos; } protected set { coos = value; } }
		int coos2g;
		public int COOS2G { get { return coos2g; } protected set { coos2g = value; } }
		int coos3g;
		public int COOS3G { get { return coos3g; } protected set { coos3g = value; } }
		int coos4g;
		public int COOS4G { get { return coos4g; } protected set { coos4g = value; } }
		bool fullSiteOutage;
		public bool FullSiteOutage { get { return fullSiteOutage; } protected set { fullSiteOutage = value; } }
		bool performanceIssue;
		public bool PerformanceIssue { get { return performanceIssue; } protected set { performanceIssue = value; } }
		bool intermittentIssue;
		public bool IntermittentIssue { get { return intermittentIssue; } protected set { intermittentIssue = value; } }
		string cctReference = string.Empty;
		public string CCTReference { get { return cctReference; } protected set { cctReference = value; } }
		string relatedINC_CRQ = string.Empty;
		public string RelatedINC_CRQ { get { return relatedINC_CRQ; } protected set { relatedINC_CRQ = value; } }
		string activeAlarms = string.Empty;
		public string ActiveAlarms { get { return activeAlarms; } protected set { activeAlarms = value; } }
		string alarmHistory = string.Empty;
		public string AlarmHistory { get { return alarmHistory; } protected set { alarmHistory = value; } }
		string troubleshoot = string.Empty;
		public string TroubleShoot { get { return troubleshoot; } protected set { troubleshoot = value; } }
		string bcpForm = string.Empty;
		public string BcpForm { get { return bcpForm; } protected set { bcpForm = value; } }
		string OngoingINCs;
		string OngoingCRQs;
		
		Site site;
		public Site Site {
			get {
				if(site == null)
					site = Task.Run(() => DB.SitesDB.GetSiteAsync(SiteId)).Result;
				return site;
			}
		}
		
		public Troubleshoot() {
			LogType = TemplateTypes.Troubleshoot;
		}
		
		public Troubleshoot(Control.ControlCollection controlsCollection, string relatedCases)
		{
			try { INC = controlsCollection["INCTextBox"].Text; } catch (Exception) { }
			try { SiteId = controlsCollection["SiteIdTextBox"].Text; } catch (Exception) { }
			try {
				ComboBox cb = (ComboBox)controlsCollection["SiteOwnerComboBox"];
				if (cb.SelectedIndex == -1)
					cb.SelectedIndex = 0;
				SiteOwner = cb.Text;
			} catch (Exception) { }
			try { TefSiteId = controlsCollection["TefSiteTextBox"].Text; } catch (Exception) { }
			try { SiteAddress = controlsCollection["AddressTextBox"].Text; } catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["OtherSitesImpactedCheckBox"];
				OtherSitesImpacted = cb.Checked;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["COOSCheckBox"];
				COOS = cb.Checked;
			} catch (Exception) { }
			try {
				NumericUpDown nud = (NumericUpDown)controlsCollection["COOS2GNumericUpDown"];
				COOS2G = (int)nud.Value;
			} catch (Exception) { }
			try {
				NumericUpDown nud = (NumericUpDown)controlsCollection["COOS3GNumericUpDown"];
				COOS3G = (int)nud.Value;
			} catch (Exception) { }
			try {
				NumericUpDown nud = (NumericUpDown)controlsCollection["COOS4GNumericUpDown"];
				COOS4G = (int)nud.Value;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["FullSiteOutageCheckBox"];
				FullSiteOutage = cb.Checked;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["PerformanceIssueCheckBox"];
				PerformanceIssue = cb.Checked;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["IntermittentIssueCheckBox"];
				IntermittentIssue = cb.Checked;
			} catch (Exception) { }
			try { CCTReference = string.IsNullOrEmpty(controlsCollection["CCTRefTextBox"].Text) ? "None" : controlsCollection["CCTRefTextBox"].Text; } catch (Exception) { }
			try {
				if(!string.IsNullOrEmpty(relatedCases))
					RelatedINC_CRQ = relatedCases;
				else
					RelatedINC_CRQ = string.IsNullOrEmpty(controlsCollection["RelatedINC_CRQTextBox"].Text) ? "None" : controlsCollection["RelatedINC_CRQTextBox"].Text;
			}
			catch (Exception) { }
			try { ActiveAlarms = controlsCollection["ActiveAlarmsTextBox"].Text; } catch (Exception) { }
			try { AlarmHistory = string.IsNullOrEmpty(controlsCollection["AlarmHistoryTextBox"].Text) ? "None related" : controlsCollection["AlarmHistoryTextBox"].Text; } catch (Exception) { }
			try { TroubleShoot = controlsCollection["TroubleshootTextBox"].Text; } catch (Exception) { }
//			try {
//				Signature = CurrentUser.fullName[1] + " " + CurrentUser.fullName[0] + Environment.NewLine + CurrentUser.department + Environment.NewLine + "ANOC Number: +44 163 569 206";
//				Signature += CurrentUser.department == "1st Line RAN Support" ? "7" : "9";
//			} catch (Exception) { }
			fullLog = generateFullLog();
			LogType = TemplateTypes.Troubleshoot;
		}
		
		public Troubleshoot(Troubleshoot template) {
			Toolbox.Tools.CopyProperties(this, template);
			fullLog = generateFullLog();
			LogType = TemplateTypes.Troubleshoot;
		}
		
		public Troubleshoot(string[] log, DateTime date) {
			LoadTST(log);
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = TemplateTypes.Troubleshoot;
		}
		
		public void LoadTST(string[] log)
		{
			INC = log[1].Substring("INC: ".Length);
			SiteId = log[2].Substring("Site ID: ".Length);
			SiteOwner = log[3].Substring("Site Owner: ".Length,2);
			if(SiteOwner == "TF") {
				string[] temp = log[3].Split('(');
				string tempStr = string.Empty;
				foreach (char c in temp[temp.GetUpperBound(0)]) {
					if(c != ')')
						tempStr += c;
				}
				TefSiteId = tempStr;
			}
			SiteAddress = log[4].Substring("Site Address: ".Length);
			OtherSitesImpacted = log[5].Substring("Other sites impacted: ".Length) != "None";
			if (log[6].Substring("COOS: ".Length,2) == "No")
				COOS = false;
			else {
				COOS = true;
				string[] COOSvalues = log[6].Substring("COOS: YES ".Length).Split(' ');
				COOS2G = Convert.ToInt16(COOSvalues[1]);
				COOS3G = Convert.ToInt16(COOSvalues[3]);
				COOS4G = Convert.ToInt16(COOSvalues[5]);
			}
			int ind = 7;
			if(log[ind].StartsWith("Full"))
				FullSiteOutage = log[ind++].Substring("Full Site Outage: ".Length) != "No";
			PerformanceIssue = log[ind++].Substring("Performance Issue: ".Length) != "No";
			IntermittentIssue = log[ind++].Substring("Intermittent Issue: ".Length) != "No";
			CCTReference = log[ind].Substring("CCT Reference: ".Length) == "None" ? string.Empty : log[ind].Substring("CCT Reference: ".Length);
			ind++;
			
			string complete = string.Empty;
			if(log[ind].StartsWith("Related")) {
				if(log[ind].Substring("Related INC/CRQ:".Length) == " None")
					RelatedINC_CRQ = string.Empty;
				else {
					if("Related INC/CRQ:".Length < log[ind].Length && log[ind+1] == string.Empty)
						RelatedINC_CRQ = log[ind].Substring("Related INC/CRQ: ".Length);
					else {
						for (ind++; ind < log.Length - 4; ind++) {
							if(log[ind] == "") {
								if(log[ind+1].StartsWith("Active Alarms:"))
									break;
							}
							else {
								if(string.IsNullOrEmpty(complete))
									complete += log[ind];
								else
									complete += Environment.NewLine + log[ind];
							}
						}
						RelatedINC_CRQ = complete;
					}
				}
			}
			else {
				if(log[++ind].Substring("Ongoing INCs:".Length) == " None")
					OngoingINCs = log[ind++].Substring("Ongoing INCs:".Length);
				else {
//					complete = Environment.NewLine;
					for (ind++; ind < log.Length - 4; ind++) {
						if (log[ind] == "") {
							if (log[ind+1].StartsWith("Ongoing CRQs:"))
								break;
						}
						else
							complete += log[ind] + Environment.NewLine;
					}
					OngoingINCs = string.IsNullOrWhiteSpace(complete) ? " None" : complete;
				}
				
				if(log[++ind].Substring("Ongoing CRQs:".Length) == " None")
					OngoingCRQs = log[ind++].Substring("Ongoing CRQs:".Length);
				else {
//					complete = Environment.NewLine;
					for (ind++; ind < log.Length - 4; ind++) {
						if (log[ind] == "") {
							if (log[ind+1].StartsWith("Active Alarms:")) {
								ind++;
								break;
							}
						}
						else
							complete += log[ind] + Environment.NewLine;
					}
					OngoingCRQs = string.IsNullOrWhiteSpace(complete) ? " None" : complete;
				}
			}
			
			complete = string.Empty;
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
					if(string.IsNullOrEmpty(complete))
						complete = log[ind];
					else
						complete += Environment.NewLine + log[ind];
				}
			}
			ActiveAlarms = complete;
			
			complete = string.Empty;
			for (ind++; ind < log.Length - 4; ind++) {
				if (log[ind] == "None related") {
					complete = string.Empty;
					ind += 2;
					break;
				}
				if (log[ind] == "") {
					if (log[ind+1] != "Troubleshoot:")
						complete += Environment.NewLine;
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
			AlarmHistory = complete;
			
			complete = string.Empty;
			ind = Array.IndexOf(log, "Troubleshoot:");
			for (ind++; ind < log.Length - 4; ind++) {
				if(complete == string.Empty)
					complete = log[ind];
				else
					complete += Environment.NewLine + log[ind];
			}
			TroubleShoot = complete;
			
			for(ind++; ind < log.Length; ind++) {
				Signature += log[ind];
				if(ind != log.Length - 1)
					Signature += Environment.NewLine;
			}
			fullLog = string.Join("\r\n", log.Where((val, idx) => idx != 0).ToArray());
		}
		
		public void AddBcpLog(string BcpForm) {
			
		}
		
		string generateFullLog() {
			string template = "INC: " + INC + Environment.NewLine;
			template += "Site ID: " + SiteId + Environment.NewLine;
			template += "Site Owner: " + SiteOwner;
			if (SiteOwner == "TF")
				template += " (" + TefSiteId + ")";
			template += Environment.NewLine;
			template += "Site Address: " + SiteAddress + Environment.NewLine;
			template += "Other sites impacted: ";
			template += OtherSitesImpacted ? "YES * more info on the INC" : "None";
			template += Environment.NewLine;
			template += "COOS: ";
			if (COOS)
				template += "YES 2G: " + COOS2G + " 3G: " + COOS3G + " 4G: " + COOS4G;
			else
				template += "No";
			template += Environment.NewLine;
			template += "Full Site Outage: ";
			template += FullSiteOutage ? "YES" : "No";
			template += Environment.NewLine;
			template += "Performance Issue: ";
			template += PerformanceIssue ? "YES" : "No";
			template += Environment.NewLine;
			template += "Intermittent Issue: ";
			template += IntermittentIssue ? "YES" : "No";
			template += Environment.NewLine;
			template += "CCT Reference: " + CCTReference + Environment.NewLine;
//			template += "Related INC/CRQ: " + RelatedINC_CRQ + Environment.NewLine + Environment.NewLine;
			template += "Related INC/CRQ:";
			if(RelatedINC_CRQ == "None")
				template += " " + RelatedINC_CRQ;
			else
				template += Environment.NewLine + RelatedINC_CRQ;
			template += Environment.NewLine + Environment.NewLine;
			
//			template += Environment.NewLine;
//			template += "Ongoing INCs:" + getCurrentCases("INC", true) + Environment.NewLine;
//			template += "Ongoing CRQs:" + getCurrentCases("CRQ", true) + Environment.NewLine;
			
			template += "Active Alarms:" + Environment.NewLine + ActiveAlarms + Environment.NewLine + Environment.NewLine;
			template += "Alarm History:" + Environment.NewLine + AlarmHistory + Environment.NewLine + Environment.NewLine;
			template += "Troubleshoot:" + Environment.NewLine + TroubleShoot;
			template += Environment.NewLine + Environment.NewLine + Signature;
			
			return template;
		}
		
//		/// <summary>
//		/// Gets INC's or CRQ's from site class
//		/// </summary>
//		/// <param name="type">"INC", "CRQ"</param>
//		/// <param name="update">Forces data update</param>
//		/// <returns></returns>
//		string getCurrentCases(string type, bool update = false) {
//			string temp = string.Empty;
//			temp = type == "INC" ? OngoingINCs : OngoingCRQs;
//			if(string.IsNullOrWhiteSpace(temp) || update) {
//				temp = string.Empty;
//				if(MainForm.TroubleshootUI.currentSite != null) {
//					DataTable cases;
//					cases = type == "INC" ? MainForm.TroubleshootUI.currentSite.INCs : MainForm.TroubleshootUI.currentSite.CRQs;
//					if(cases == null) {
//						MainForm.TroubleshootUI.currentSite.requestOIData(type);
//						cases = type == "INC" ? MainForm.TroubleshootUI.currentSite.INCs : MainForm.TroubleshootUI.currentSite.CRQs;
//					}
//					if(cases != null) {
//						if(cases.Rows.Count > 0) {
//							temp += Environment.NewLine;
//							string query = type == "INC" ? "[Incident Ref] NOT LIKE '" + INC + "' AND Status NOT LIKE 'Closed' AND Status NOT LIKE 'Resolved'" :
//								"Status = 'Scheduled' OR Status = 'Implementation in Progress'"; // "Status NOT LIKE 'Closed' AND 'Scheduled Start' >= #" + Convert.ToString(DateTime.Now.Date) +"#"; // .ToString("dd-MM-yyyy HH:mm:ss")
//							List<DataRow> filteredCases = cases.Select(query).ToList();
//							if(type == "CRQ" && filteredCases.Count > 0) {
//								for(int c = 0;c < filteredCases.Count;c++) {
//									DataRow row = filteredCases[c];
//									if(!(row["Scheduled Start"] is DBNull) && !(row["Scheduled End"] is DBNull)) {
//										if (Convert.ToDateTime(row["Scheduled Start"]) < DateTime.Now && Convert.ToDateTime(row["Scheduled End"]) < DateTime.Now) {// && Convert.ToDateTime(row["Scheduled End"]) >= DateTime.Now)) {
//											filteredCases.RemoveAt(c);
//											c--;
//										}
//									}
//								}
//							}
//							foreach(DataRow row in filteredCases) {
//								string rowString = string.Empty;
//								if(type == "INC")
//									rowString = row["Incident Ref"] + " - " + row["Summary"] + " - " + ((DateTime)row["Submit Date"]).ToString("dd-MM-yyyy HH:mm") + Environment.NewLine;
//								else {
//									string startDate = string.Empty;
//									string endDate = string.Empty;
//									try { startDate = Convert.ToDateTime(row["Scheduled Start"]).ToString("dd-MM-yyyy HH:mm"); } catch(Exception) {}
//									try { endDate = Convert.ToDateTime(row["Scheduled End"]).ToString("dd-MM-yyyy HH:mm"); } catch(Exception) {}
//									rowString = row["Change Ref"] + " - " + row["Status"] + " - " + row["Summary"] + " - " + startDate + " until " + endDate + Environment.NewLine;
//								}
//								temp += rowString;
//							}
//						}
//					}
//				}
//			}
//			if(string.IsNullOrWhiteSpace(temp))
//				temp = " None" + Environment.NewLine;
//			return temp;
//		}
		
		public override string ToString()
		{
			if(string.IsNullOrEmpty(fullLog))
				fullLog = generateFullLog();
			
			return fullLog;
		}
		
		public override Troubleshoot ToTroubleshootTemplate() {
			return new Troubleshoot(this);
		}
	}
}
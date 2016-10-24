/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 30-07-2016
 * Time: 15:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using appCore.Logs;
using System.Linq;
using System.Reflection;

namespace appCore.Logs.Types
{
	/// <summary>
	/// Description of TroubleShoot.
	/// </summary>
	[Serializable]
	public class TroubleShoot : Log
	{
		string inc = string.Empty;
		public string INC { get { return inc; } protected set { inc = value; } }
		string site = string.Empty;
		public string SiteId { get { return site; } protected set { site = value; } }
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
		public string Troubleshoot { get { return troubleshoot; } protected set { troubleshoot = value; } }
		string signature = string.Empty;
		public string Signature { get { return signature; } protected set { signature = value; } }
		
		public TroubleShoot() { }
		
		public TroubleShoot(Control.ControlCollection controlsCollection)
		{
			try { INC = controlsCollection["textBox1"].Text; } catch (Exception) { }
			try { SiteId = controlsCollection["textBox2"].Text; } catch (Exception) { }
			try {
				ComboBox cb = (ComboBox)controlsCollection["comboBox1"];
				if (cb.SelectedIndex == -1) {
					cb.SelectedIndex = 0;
				}
				SiteOwner = cb.Text;
			} catch (Exception) { }
			try { TefSiteId = controlsCollection["textBox3"].Text; } catch (Exception) { }
			try { SiteAddress = controlsCollection["textBox4"].Text; } catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["checkBox1"];
				OtherSitesImpacted = cb.Checked;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["checkBox2"];
				COOS = cb.Checked;
			} catch (Exception) { }
			try {
				NumericUpDown nud = (NumericUpDown)controlsCollection["numericUpDown1"];
				COOS2G = (int)nud.Value;
			} catch (Exception) { }
			try {
				NumericUpDown nud = (NumericUpDown)controlsCollection["numericUpDown2"];
				COOS3G = (int)nud.Value;
			} catch (Exception) { }
			try {
				NumericUpDown nud = (NumericUpDown)controlsCollection["numericUpDown3"];
				COOS4G = (int)nud.Value;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["checkBox18"];
				FullSiteOutage = cb.Checked;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["checkBox3"];
				PerformanceIssue = cb.Checked;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["checkBox4"];
				IntermittentIssue = cb.Checked;
			} catch (Exception) { }
			try { CCTReference = string.IsNullOrEmpty(controlsCollection["textBox5"].Text) ? "None" : controlsCollection["textBox5"].Text; } catch (Exception) { }
			try { RelatedINC_CRQ = string.IsNullOrEmpty(controlsCollection["textBox6"].Text) ? "None" : controlsCollection["textBox6"].Text; } catch (Exception) { }
			try { ActiveAlarms = controlsCollection["textBox7"].Text; } catch (Exception) { }
			try { AlarmHistory = string.IsNullOrEmpty(controlsCollection["textBox8"].Text) ? "None related" : controlsCollection["textBox8"].Text; } catch (Exception) { }
			try { Troubleshoot = controlsCollection["textBox9"].Text; } catch (Exception) { }
			try {
				string[] name = Toolbox.Tools.GetUserDetails("Name").Split(' ');
				string dept = Toolbox.Tools.GetUserDetails("Department");
				dept = dept.Contains("2nd Line RAN") ? "2nd Line RAN Support" : "1st Line RAN Support";
				
				Signature = name[1].Replace(",",string.Empty) + " " + name[0].Replace(",",string.Empty) + Environment.NewLine + dept + Environment.NewLine + "ANOC Number: +44 163 569 206";
				Signature += dept == "1st Line RAN Support" ? "7" : "9";
			} catch (Exception) { }
		}
		
		public TroubleShoot(appCore.Templates.Types.TroubleShoot template) {
			Log log = new TroubleShoot();
			Toolbox.Tools.CopyProperties(log, template);
			generateFullLog();
		}
		
		public TroubleShoot(string[] log, DateTime date) {
			LoadTST(log);
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = "Troubleshoot";
//			GenerationDate = Convert.ToDateTime(date + " " + log[0].Split(strTofind, StringSplitOptions.None)[0]);
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
			if(log[ind].StartsWith("Full")) {
				FullSiteOutage = log[ind].Substring("Full Site Outage: ".Length) != "No";
				ind++;
			}
			PerformanceIssue = log[ind].Substring("Performance Issue: ".Length) != "No";
			ind++;
			IntermittentIssue = log[ind].Substring("Intermittent Issue: ".Length) != "No";
			ind++;
			CCTReference = log[ind].Substring("CCT Reference: ".Length) == "None" ? string.Empty : log[ind].Substring("CCT Reference: ".Length);
			ind++;
			RelatedINC_CRQ = log[ind].Substring("Related INC/CRQ: ".Length) == "None" ? string.Empty : log[ind].Substring("Related INC/CRQ: ".Length);
			
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
			ActiveAlarms = complete;
			
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
			AlarmHistory = complete;
			
			complete = string.Empty;
			ind = Array.IndexOf(log, "Troubleshoot:");
			for (ind++; ind < log.Length - 4; ind++) {
				if (complete == string.Empty) complete = log[ind];
				else complete += Environment.NewLine + log[ind];
			}
			Troubleshoot = complete;
			for(ind++; ind < log.Length; ind++) {
				Signature += log[ind];
				if(ind != log.Length - 1)
					Signature += Environment.NewLine;
			}
			fullLog = string.Join("\r\n", log.Where((val, idx) => idx != 0).ToArray());
		}
		
		public string generateFullLog() {
			fullLog = "INC: " + INC + Environment.NewLine;
			fullLog += "Site ID: " + SiteId + Environment.NewLine;
			fullLog += "Site Owner: " + SiteOwner;
			if (SiteOwner == "TF")
				fullLog += " (" + TefSiteId + ")";
			fullLog += Environment.NewLine;
			fullLog += "Site Owner: " + SiteOwner + Environment.NewLine;
			fullLog += "Site Address: " + SiteAddress + Environment.NewLine;
			fullLog += "Other sites impacted: ";
			fullLog += OtherSitesImpacted ? "YES * more info on the INC" : "None";
			fullLog += Environment.NewLine;
			fullLog += "COOS: ";
			if (COOS)
				fullLog += "YES 2G: " + COOS2G + " 3G: " + COOS3G + " 4G: " + COOS4G;
			else
				fullLog += "No";
			fullLog += Environment.NewLine;
			fullLog += "Full Site Outage: ";
			fullLog += FullSiteOutage ? "YES" : "No";
			fullLog += Environment.NewLine;
			fullLog += "Performance Issue: ";
			fullLog += PerformanceIssue ? "YES" : "No";
			fullLog += Environment.NewLine;
			fullLog += "Intermittent Issue: ";
			fullLog += IntermittentIssue ? "YES" : "No";
			fullLog += Environment.NewLine;
			fullLog += "CCT Reference: " + CCTReference + Environment.NewLine;
			fullLog += "Related INC/CRQ: " + RelatedINC_CRQ + Environment.NewLine + Environment.NewLine;
			fullLog += "Active Alarms:" + Environment.NewLine + ActiveAlarms + Environment.NewLine + Environment.NewLine;
			fullLog += "Alarm History:" + Environment.NewLine + AlarmHistory + Environment.NewLine + Environment.NewLine;
			fullLog += "Troubleshoot:" + Environment.NewLine + Troubleshoot;
			fullLog += Environment.NewLine + Environment.NewLine + Signature;
			return fullLog;
		}
		
		public override TroubleShoot ToTroubleShootTemplate() {
			return new TroubleShoot(this);
		}
	}
}
/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 30-07-2016
 * Time: 15:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using appCore.Logs;
using System.Reflection;

namespace appCore.Logs.Types
{
	/// <summary>
	/// Description of TX.
	/// </summary>
	public class TX : Log
	{
		public string INC { get; set; }
		public string SiteId { get; set; }
		public string SiteOwner { get; set; }
		public string TefSiteId { get; set; }
		public string SiteAddress { get; set; }
		public bool OtherSitesImpacted { get; set; }
		public bool COOS { get; set; }
		public int COOS2G { get; set; }
		public int COOS3G { get; set; }
		public int COOS4G { get; set; }
		public bool FullSiteOutage { get; set; }
		public bool PerformanceIssue { get; set; }
		public bool IntermittentIssue { get; set; }
		public string CCTReference { get; set; }
		public string RelatedINC_CRQ { get; set; }
		public string ActiveAlarms { get; set; }
		public string AlarmHistory { get; set; }
		public string Troubleshoot { get; set; }
		public string Signature { get; set; }
		
		protected TX() { }
		
		public TX(Control.ControlCollection controlsCollection)
		{
//			logType = thisLogType;
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
		
		public TX(appCore.Templates.Types.TX template) {
			Log log = new TX();
			CopyProperties(log, template);
			generateFullLog();
		}
		
		public static void CopyProperties(object dst, object src)
		{
			PropertyInfo[] srcProperties = src.GetType().GetProperties();
			dynamic dstType = dst.GetType();

			if (srcProperties == null | dstType.GetProperties() == null) {
				return;
			}

			foreach (PropertyInfo srcProperty in srcProperties) {
				PropertyInfo dstProperty = dstType.GetProperty(srcProperty.Name);

				if (dstProperty != null) {
					if(dstProperty.CanWrite) {
						if (dstProperty.CanWrite && dstProperty.PropertyType.IsAssignableFrom(srcProperty.PropertyType) == true) {
							dstProperty.SetValue(dst, srcProperty.GetValue(src, null), null);
						}
					}
				}
			}
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
	}
}
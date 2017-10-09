/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 15:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows.Forms;

namespace appCore.Templates.RAN.Types
{
	/// <summary>
	/// Description of TX.
	/// </summary>
//	[Serializable]
	public class TX : Template
	{		
		string siteids = string.Empty;
		public string SiteIDs { get { return siteids; } protected set { siteids = value; } }
		string serviceaffected = string.Empty;
		public string ServiceAffected { get { return serviceaffected; } protected set { serviceaffected = value; } }
		bool repeat_intermittent;
		public bool Repeat_Intermittent{ get { return repeat_intermittent; } protected set { repeat_intermittent = value; } }
		string txtype = string.Empty;
		public string TxType { get { return txtype; } protected set { txtype = value; } }
		string ipranportconfig = string.Empty;
		public string IpRanPortConfig { get { return ipranportconfig; } protected set { ipranportconfig = value; } }
		string performanceoutagedetails = string.Empty;
		public string PerformanceOutageDetails { get { return performanceoutagedetails; } protected set { performanceoutagedetails = value; } }
		string detailedrantroubleshoot = string.Empty;
		public string DetailedRanTroubleshoot { get { return detailedrantroubleshoot; } protected set { detailedrantroubleshoot = value; } }
		
		public TX() {
			LogType = TemplateTypes.TX;
		}
		
		public TX(Control.ControlCollection controlsCollection)
		{
			try { SiteIDs = controlsCollection["SitesTextBox"].Text; } catch (Exception) { }
			try {
				ComboBox cb = (ComboBox)controlsCollection["ServiceAffectedComboBox"];
				if (cb.SelectedIndex == -1) {
					cb.SelectedIndex = 0;
				}
				ServiceAffected = cb.Text;
			} catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["Repeat_IntermittentCheckBox"];
				Repeat_Intermittent = cb.Checked;
			} catch (Exception) { }
			try {
				ComboBox cb = (ComboBox)controlsCollection["TxTypeComboBox"];
				if (cb.SelectedIndex == -1) {
					cb.SelectedIndex = 0;
				}
				TxType = cb.Text;
			} catch (Exception) { }
			try { IpRanPortConfig = TxType == "IPRAN" ? controlsCollection["IpRanPortConfigTextBox"].Text : "N/A"; } catch (Exception) { }
			try { PerformanceOutageDetails = controlsCollection["PerformanceOutageDetailsTextBox"].Text; } catch (Exception) { }
			try { DetailedRanTroubleshoot = controlsCollection["DetailedRanTroubleshootTextBox"].Text; } catch (Exception) { }
			fullLog = generateFullLog();
			LogType = TemplateTypes.TX;
		}
		
		public TX(TX template) {
			Toolbox.Tools.CopyProperties(this, template);
			fullLog = generateFullLog();
			LogType = TemplateTypes.TX;
		}
		
		public TX(string[] log, DateTime date) {
			LoadTXT(log);
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = TemplateTypes.TX;
		}
		
		public void LoadTXT(string[] log)
		{
			int c = 1;
			string complete = log[c].Substring("Site(s) ref: ".Length);
			for (c++; c < log.Length; c++) {
				if (!log[c].Contains("Service affected: "))
					complete = complete + Environment.NewLine + log[c];
				else
					break;
			}
			SiteIDs = complete;
			ServiceAffected = log[c++].Substring("Service affected: ".Length);
			
			complete = log[c].Substring("Performance/Outage detailed issue: ".Length);
			for (c++; c < log.Length; c++) {
				if (!log[c].Contains("Repeat/Intermittent: "))
					complete += Environment.NewLine + log[c];
				else
					break;
			}
			PerformanceOutageDetails = complete;
			
			Repeat_Intermittent = log[c++].Substring("Repeat/Intermittent: ".Length) != "No";
			
			TxType = log[c++].Substring("TX type (IPRAN, TDM): ".Length);
			IpRanPortConfig = TxType == "IPRAN" ? log[c].Substring("IPRAN port configuration: ".Length) : string.Empty;
			
			complete = log[++c].Substring("Detailed RAN troubleshooting (RAN findings, HW replaced, Field visits troubleshooting): ".Length);
			for (c++; c < log.Length - 4; c++) {
				if(string.IsNullOrEmpty(log[c]))
					if(log[c + 1] == Settings.CurrentUser.FullName[1] + " " + Settings.CurrentUser.FullName[0])
						break;
				complete += Environment.NewLine + log[c];
			}
			DetailedRanTroubleshoot = complete;
			
			fullLog = string.Join("\r\n", log.Where((val, idx) => idx != 0).ToArray());
		}
		
		string generateFullLog() {
			string template = "Site(s) ref: " + SiteIDs + Environment.NewLine;
			template += "Service affected: " + ServiceAffected + Environment.NewLine;
			template += "Performance/Outage detailed issue: " + PerformanceOutageDetails + Environment.NewLine;
			template += "Repeat/Intermittent: " + (Repeat_Intermittent ? "YES" : "No") + Environment.NewLine;
			template += "TX type (IPRAN, TDM): " + TxType + Environment.NewLine;
			template += "IPRAN port configuration: " + IpRanPortConfig + Environment.NewLine;
			template += "Detailed RAN troubleshooting (RAN findings, HW replaced, Field visits troubleshooting): " + DetailedRanTroubleshoot;
			
			template += Environment.NewLine + Environment.NewLine + Signature;
			
			return template;
		}
		
		public override string ToString()
		{
			if(string.IsNullOrEmpty(fullLog))
				generateFullLog();
			
			return fullLog;
		}
		
		public override TX ToTxTemplate() {
			return new TX(this);
		}
	}
}
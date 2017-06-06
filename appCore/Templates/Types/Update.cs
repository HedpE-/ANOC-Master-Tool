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

namespace appCore.Templates.Types
{
	/// <summary>
	/// Description of Update.
	/// </summary>
//	[Serializable]
	public class Update : Template
	{
		string inc = string.Empty;
		public string INC { get { return inc; } protected set { inc = value; } }
		string siteid = string.Empty;
		public string SiteId { get { return siteid; } protected set { siteid = value; } }
		string nextactions = string.Empty;
		public string NextActions { get { return nextactions; } protected set { nextactions = value; } }
		string _update = string.Empty;
		public string update { get { return _update; } protected set { _update = value; } }
		
		SiteFinder.Site site;
		public SiteFinder.Site Site {
			get {
				if(site == null)
					site = DB.SitesDB.getSite(SiteId);
				return site;
			}
		}
		
		public Update() {
			LogType = TemplateTypes.Update;
		}
		
		public Update(Control.ControlCollection controlsCollection)
		{
			try { INC = controlsCollection["INCTextBox"].Text; } catch (Exception) { }
			try { SiteId = controlsCollection["SiteIdTextBox"].Text; } catch (Exception) { }
			try { update = controlsCollection["UpdateTextBox"].Text; } catch (Exception) { }
			try { NextActions = string.IsNullOrWhiteSpace(controlsCollection["NextActionsTextBox"].Text) ? string.Empty : controlsCollection["NextActionsTextBox"].Text; } catch (Exception) { }
			fullLog = generateFullLog();
			LogType = TemplateTypes.Update;
		}
		
		public Update(Update template) {
			Toolbox.Tools.CopyProperties(this, template);
			fullLog = generateFullLog();
			LogType = TemplateTypes.Update;
		}
		
		public Update(string[] log, DateTime date) {
			LoadUpdate(log);
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = TemplateTypes.Update;
		}
		
		public void LoadUpdate(string[] log)
		{
			INC = log[1].Substring(5,15);
			SiteId = log[2].Substring(6,log[2].Length - 6);
			update = log[4];
			if(log[6] == "Next actions:")
				NextActions = log[7];
			fullLog = string.Join("\r\n", log.Where((val, idx) => idx != 0).ToArray());
		}
		
		string generateFullLog() {
			string template = "INC: " + INC + Environment.NewLine;
			template += "Site: " + SiteId + Environment.NewLine;
			template += "Update:" + Environment.NewLine + update + Environment.NewLine + Environment.NewLine;
			if (!string.IsNullOrEmpty(NextActions))
				template += "Next actions:" + Environment.NewLine + NextActions + Environment.NewLine + Environment.NewLine;
			template += Signature;
			
			return template;
		}
		
		public override string ToString()
		{
			if(string.IsNullOrEmpty(fullLog))
				generateFullLog();
			
			return fullLog;
		}
		
		public override Update ToUpdateTemplate() {
			return new Update(this);
		}
	}
}
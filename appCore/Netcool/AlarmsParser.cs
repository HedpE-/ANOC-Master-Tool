/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 23-09-2016
 * Time: 23:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using appCore.Templates.Types;
using FileHelpers;

namespace appCore.Netcool
{
	/// <summary>
	/// Description of AlarmsParser.
	/// </summary>
	public class AlarmsParser
	{
		string parsedOutput;
		public List<Alarm> AlarmsList = new List<Alarm>();
		public List<string> lteSitesOnM = new List<string>();
		
		public AlarmsParser(string netcoolAlarms, bool generateOutput = true, bool outage = false) {
			while(netcoolAlarms.Contains("\n-ProbableCause"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\n-ProbableCause", StringComparison.Ordinal), 1);
			
			try {
				var engine = new FileHelperEngine<Alarm>();
				
				engine.BeforeReadRecord += (eng, e) => {
					var tx = e.RecordLine;
					if(e.RecordLine.StartsWith("Attributes\tService Impact\t"))
						e.SkipThisRecord = true;
					else {
						if(e.RecordLine.Contains("\t-ProbableCause")) {
							while(e.RecordLine.Contains("\t-ProbableCause"))
								e.RecordLine = e.RecordLine.Remove(e.RecordLine.IndexOf("\t-ProbableCause", StringComparison.Ordinal), 1);
						}
					}
				};
				
				if(outage) {
					engine.AfterReadRecord += (eng, e) => {
						if(string.IsNullOrEmpty(e.Record.Element))
							e.Record.Element = e.Record.ResolveCellName();
						if(e.Record.Bearer == "4G" && !e.Record.COOS)
							lteSitesOnM.Add(e.Record.SiteId);
					};
				}
				else {
					if(generateOutput) {
						engine.AfterReadRecord +=  (eng, e) => {
							Alarm t = e.Record;
							parsedOutput += e.Record + Environment.NewLine + Environment.NewLine;
						};
					}
				}
				
//				AlarmsTable = engine.ReadStringAsDT(netcoolAlarms);
				AlarmsList = engine.ReadStringAsList(netcoolAlarms);
			}
			catch(FileHelpersException e) {
				var m = e.Message;
			}
			
			if(lteSitesOnM.Any())
				lteSitesOnM = lteSitesOnM.Distinct().ToList();
			if(generateOutput)
				parsedOutput = parsedOutput.Substring(0, parsedOutput.Length - 4);
		}
		
		public Outage GenerateOutage() {
			return new Outage(this);
		}
		
		public override string ToString()
		{
			return parsedOutput;
		}
	}
}

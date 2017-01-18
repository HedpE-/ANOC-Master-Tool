/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 23-09-2016
 * Time: 23:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using appCore.Templates.Types;
using FileHelpers;
using FileHelpers.Dynamic;

namespace appCore.Netcool
{
	/// <summary>
	/// Description of AlarmsParser.
	/// </summary>
	public class AlarmsParser2
	{
		string parsedOutput;
//		public List<Alarm2> AlarmsList = new List<Alarm2>();
		public List<string> lteSites = new List<string>();
		
		public AlarmsParser2(string netcoolAlarms, bool generateOutput = true, bool outage = false) {
			while(netcoolAlarms.Contains("\n-ProbableCause"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\n-ProbableCause", StringComparison.Ordinal), 1);
			
			try {
				var cb = new DelimitedClassBuilder("Alarm2", "\t") { IgnoreFirstLines = 0, IgnoreEmptyLines = true, Delimiter = "\t"  };
				var headerArray = netcoolAlarms.Split('\n')[0].Split('\t');
				foreach (var header in headerArray)
				{
					var fieldName = header.Replace("/", "").Replace("\"", "").Replace(" ", "");
					cb.AddField(fieldName, typeof(string));
				}
				var cbc = cb.CreateRecordClass();
				var engine = new FileHelperEngine(cbc);
				
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
//					engine.AfterReadRecord += (eng, e) => {
//						if(e.Record.Bearer == "4G")
//							lteSites.Add(e.Record.SiteId);
//					};
				}
				else {
					if(generateOutput) {
						engine.AfterReadRecord +=  (eng, e) => {
							var t = e.Record;
							parsedOutput += e.Record + Environment.NewLine + Environment.NewLine;
						};
					}
				}
				
//				AlarmsTable = engine.ReadStringAsDT(netcoolAlarms);
				var AlarmsList = engine.ReadStringAsList(netcoolAlarms);
			}
			catch(FileHelpersException e) {
				var m = e.Message;
			}
			
			if(lteSites.Any())
				lteSites = lteSites.Distinct().ToList();
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

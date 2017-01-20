﻿/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 23-09-2016
 * Time: 23:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
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
		public DataTable AlarmsTable = new DataTable();
		public List<string> lteSitesOnM = new List<string>();
		
		public AlarmsParser(string netcoolAlarms, bool generateOutput = true, bool outage = false) {
			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
			
			while(netcoolAlarms.Contains("\n-ProbableCause"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\n-ProbableCause", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("\t-ProbableCause"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\t-ProbableCause", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("\n*** FMX Info Begin ***"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\n*** FMX Info Begin ***", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("*** FMX Info Begin ***\n"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("*** FMX Info Begin ***\n", StringComparison.Ordinal) + "*** FMX Info Begin ***".Length, 1);
			while(netcoolAlarms.Contains("\nhas generated"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\nhas generated", StringComparison.Ordinal), 1);
			while(netcoolAlarms.EndsWith("\n"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.Length - 1);
//			st.Start();
//			try {
//				var engine = new FileHelperEngine<Alarm>();
//
//				engine.BeforeReadRecord += (eng, e) => {
//					var tx = e.RecordLine;
//					if(e.RecordLine.StartsWith("Attributes\tService Impact\t"))
//						e.SkipThisRecord = true;
//				};
//
//				if(outage) {
//					engine.AfterReadRecord += (eng, e) => {
//						if(string.IsNullOrEmpty(e.Record.Element))
//							e.Record.Element = e.Record.ResolveCellName();
//						if(e.Record.Bearer == "4G" && !e.Record.COOS)
//							lteSitesOnM.Add(e.Record.SiteId);
//					};
//				}
//				else {
//					if(generateOutput) {
//						engine.AfterReadRecord +=  (eng, e) => {
//							parsedOutput += e.Record + Environment.NewLine + Environment.NewLine;
//						};
//					}
//				}
//
//				AlarmsList = engine.ReadStringAsList(netcoolAlarms);
//			}
//			catch(FileHelpersException e) {
//				var m = e.Message;
//			}
//			st.Stop();
//			var t = st.Elapsed;
//			AlarmsList.Clear();
//			st.Start();
			string[] temparr = netcoolAlarms.Split('\n');
			string[] headers = null;
			foreach (string line in temparr) {
				if(line.StartsWith("Attributes\tService Impact\t")) {
					headers = line.Split('\t');
					continue;
				}
				if(string.IsNullOrWhiteSpace(line))
					continue;
				
				Alarm al = new Alarm(line.Split('\t'), headers);
				AlarmsList.Add(al);
				if(outage) {
					if(al.Bearer == "4G" && !al.COOS)
						lteSitesOnM.Add(al.SiteId);
				}
				else {
					if(generateOutput) {
						try {
							parsedOutput += al;
							if(line != temparr.Last())
								parsedOutput += Environment.NewLine + Environment.NewLine;
						}
						catch {
							continue;
						}
					}
				}
			}
			
			if(lteSitesOnM.Any())
				lteSitesOnM = lteSitesOnM.Distinct().ToList();
//			if(generateOutput)
//				parsedOutput = parsedOutput.Substring(0, parsedOutput.Length - 4);
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

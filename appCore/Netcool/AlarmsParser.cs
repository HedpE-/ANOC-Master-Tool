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
        string ParsedOutput
        {
            get
            {
                if (string.IsNullOrEmpty(parsedOutput))
                {
                    foreach(Alarm al in AlarmsList)
                    {
                        try
                        {
                            parsedOutput += al;
                            if (al != AlarmsList.Last())
                                parsedOutput += Environment.NewLine + Environment.NewLine;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                return parsedOutput;
            }
        }
		public List<Alarm> AlarmsList = new List<Alarm>();
		public DataTable AlarmsTable = new DataTable();
		public List<string> lteSitesOnM = new List<string>();
		
		public AlarmsParser(string netcoolAlarms, bool generateOutput = true, bool outage = false) {
//			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
//			st.Start();
			while(netcoolAlarms.Contains("\n-ProbableCause"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\n-ProbableCause", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("\n-EventType"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\n-EventType", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("\nOther"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\nOther", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("\nLook"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\nLook", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("\t-ProbableCause"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\t-ProbableCause", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("\n*** FMX Info Begin ***"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\n*** FMX Info Begin ***", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("*** FMX Info Begin ***\n"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("*** FMX Info Begin ***\n", StringComparison.Ordinal) + "*** FMX Info Begin ***".Length, 1);
			while(netcoolAlarms.Contains("\nhas"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("\nhas", StringComparison.Ordinal), 1);
			while(netcoolAlarms.Contains("ExternalAlarm\n"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("ExternalAlarm\n", StringComparison.Ordinal) + "ExternalAlarm".Length, 1);
			while(netcoolAlarms.Contains("PacketFrequencySyncRef=1 \n"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("PacketFrequencySyncRef=1 \n", StringComparison.Ordinal) + "PacketFrequencySyncRef=1 ".Length, 1);
			while(netcoolAlarms.Contains("PacketFrequencySyncRef=2 \n"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.IndexOf("PacketFrequencySyncRef=2 \n", StringComparison.Ordinal) + "PacketFrequencySyncRef=2 ".Length, 1);
			while(netcoolAlarms.EndsWith("\n"))
				netcoolAlarms = netcoolAlarms.Remove(netcoolAlarms.Length - 1);

            if (netcoolAlarms.Contains("Attributes") && netcoolAlarms.Contains("Summary") && netcoolAlarms.Contains("Element"))
            {
                if (netcoolAlarms.IndexOf('\t') == -1)
                {
                    if(netcoolAlarms.IndexOf("           ") > -1)
                        netcoolAlarms.Replace("           ", "\t");
                    else
                        throw new Exception("Netcool column headers not found.");
                }
            }
            string[] temparr = netcoolAlarms.Split('\n');
			string[] headers = null;
			foreach (string line in temparr) {
				if(line.Contains("Attributes") && line.Contains("Summary") && line.Contains("Element")) {
					headers = line.Split('\t');           
					continue;
				}
				if(string.IsNullOrWhiteSpace(line))
					continue;
				Alarm al = null;
				if(headers == null)
					throw new Exception("Netcool column headers not found.");
				try {
					al = new Alarm(line.Split('\t'), headers);
				}
				catch {
					continue;
				}
				
				AlarmsList.Add(al);
				if(outage) {
					if(al.Bearer == Bearers.LTE && !al.COOS && al.OnM)
						lteSitesOnM.Add(al.SiteId);
				}
			}

            var alarmsWithoutElement = AlarmsList.FindAll(a => a.Element.Contains("No cell description")).Select(a => AlarmsList.IndexOf(a)).ToList();
            if(alarmsWithoutElement.Any())
            {
                //System.Windows.Forms.DialogResult ans = appCore.UI.FlexibleMessageBox.Show(alarmsWithoutElement.Count + " alarms without cell description found.\n\nIt's possible to try resolving the correct cell names but the operation might take a while.\nDo you want to proceed with the operation?", "Alarms without cell description", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                //if(ans == System.Windows.Forms.DialogResult.Yes)
                //{
                    System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
                    st.Start();
                    var sites = AlarmsList.Where(a => alarmsWithoutElement.Contains(AlarmsList.IndexOf(a))).Select(a => a.SiteId).ToList().Distinct();
                    var allSites = DB.SitesDB.getSites(sites);
                    foreach(int index in alarmsWithoutElement)
                        AlarmsList[index].ResolveNsnElement(allSites.FirstOrDefault(s => s.Id == AlarmsList[index].SiteId));
                    st.Stop();
                    var t = st.Elapsed;
                //}
            }
			
			if(lteSitesOnM.Any())
				lteSitesOnM = lteSitesOnM.Distinct().ToList();
			
			if(AlarmsList.Count == 0)
				throw new Exception("No COOS alarms");
//			st.Stop();
//			var t = st.Elapsed;
		}
		
		public Outage GenerateOutage() {
			return new Outage(this);
		}
		
		public override string ToString()
		{
			return ParsedOutput;
		}
	}
}

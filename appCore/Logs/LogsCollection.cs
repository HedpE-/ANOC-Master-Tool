/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 17:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using appCore.Templates;
using appCore.Templates.Types;
using appCore.UI;
using System.Timers;

namespace appCore.Logs
{
	/// <summary>
	/// Description of LogsCollection. http://www.codeproject.com/Articles/21241/Implementing-C-Generic-Collections-using-ICollecti
	/// </summary>
	public class LogsCollection<T> : CollectionBase, ILog<T> where T : Template
	{
		FileInfo _logFile;
		public FileInfo LogFile {
			get {
				return _logFile;
			}
			set {
				string[] month_year = value.Directory.Name.Split('-');
				string day = value.Name.Remove(value.Name.Length - 4);
				month_year[0] = DateTime.ParseExact(month_year[0],"MMM",CultureInfo.GetCultureInfo("pt-PT")).ToString("MM",CultureInfo.GetCultureInfo("en-GB"));
				logFileDate = new DateTime(Convert.ToInt16(month_year[1]), Convert.ToInt16(month_year[0]), Convert.ToInt16(day));
				_logFile = value;
			}
		}
		
		new public int Count {
			get { return this.List.Count; }
			private set { }
		}
		
		FileInfo _outagesLogFile;
		public FileInfo OutagesLogFile {
			get { return _outagesLogFile; }
			set { _outagesLogFile = value; }
		}
		
		new public int OutagesCount {
			get { return this.List.Count; }
			private set { }
		}
		
		bool ForceOverwriteLog;
		System.Timers.Timer timer;
		public DateTime logFileDate = DateTime.Now;
		string logSeparator = getLogSeparator();
		
//		public static bool IsFileLocked(FileInfo file)
//		{
//			FileStream stream = null;
//
//			try
//			{
//				stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
//			}
//			catch (IOException)
//			{
//				//the file is unavailable because it is:
//				//still being written to
//				//or being processed by another thread
//				//or does not exist (has already been processed)
//				return true;
//			}
//			finally
//			{
//				if (stream != null)
//					stream.Close();
//			}
//
//			//file is not locked
//			return false;
//		}
		
		public void Initialize() {
//			currentLogsCollection = true;
			
			DateTime midnight = new DateTime(DateTime.Now.AddDays(1).Year,DateTime.Now.AddDays(1).Month,DateTime.Now.AddDays(1).Day,0,0,1);
			TimeSpan timeSpanToMidnight = midnight - DateTime.Now;
			int msToMidnight = (int)timeSpanToMidnight.TotalMilliseconds;
			timer = new System.Timers.Timer(msToMidnight);
			timer.Elapsed += midnightTimer_Elapsed;
			timer.Enabled = true;

			getLogFile();
		}

		void midnightTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (LogFile.Name.Split('.')[0] != DateTime.Now.Day.ToString("dd")) {
				getLogFile();
				
				DateTime midnight = new DateTime(DateTime.Now.AddDays(1).Year,DateTime.Now.AddDays(1).Month,DateTime.Now.AddDays(1).Day,0,0,1);
				TimeSpan timeSpanToMidnight = midnight - DateTime.Now;
				timer.Interval = (int)timeSpanToMidnight.TotalMilliseconds;
			}
		}
		
		void getLogFile() {
			LogFile = new FileInfo(Settings.UserFolder.LogsFolder.FullName + "\\" + DateTime.Now.ToString("MMM-yyyy") + "\\" + DateTime.Now.ToString("dd") + ".txt");
			if(LogFile.Exists)
				ImportLogFile();
			else {
				if(!LogFile.Directory.Exists)
					LogFile.Directory.Create();
			}
		}
		
		public void CheckLogFileIntegrity(bool outage = false) {
			if(outage)
				OutagesLogFile = new FileInfo(Settings.UserFolder.LogsFolder.FullName + "\\" + DateTime.Now.ToString("MMM-yyyy") + @"\outages\"  + DateTime.Now.ToString("dd") + ".txt");
			else
				LogFile = new FileInfo(Settings.UserFolder.LogsFolder.FullName + "\\" + DateTime.Now.ToString("MMM-yyyy") + "\\" + DateTime.Now.ToString("dd") + ".txt");
			
			string logfile = string.Empty;
			
			if(LogFile.Exists) {
//				logfile = LogFile.OpenText().ReadToEnd();
				using (StreamReader reader = new StreamReader(LogFile.FullName))
					logfile = reader.ReadToEnd();
			}
			
			int logsCount = logfile.CountStringOccurrences(logSeparator);
			if(logsCount != this.List.Count) {
				this.List.Clear();
				if(logsCount > 0)
					ImportLogFile();
				MainForm.UpdateTicketCountLabel();
			}
		}

		void ImportLogFile() {
			List<string> temp = ReadLogFile();

			foreach (string logStr in temp) {
				string[] strTofind = { "\r\n" };
				string[] logArray = logStr.Split(strTofind, StringSplitOptions.None);
				string[] hour = logArray[0].Split('-')[0].Substring(0,8).Split(':');
				DateTime logtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt16(hour[0]), Convert.ToInt16(hour[1]), Convert.ToInt16(hour[2]));
				Template log = null;

				switch(logArray[0].Split('-')[1].Substring(1)) {
					case "Troubleshoot Template":
						log = new TroubleShoot(logArray, logtime);
						break;
					case "Failed CRQ":
						log = new FailedCRQ(logArray, logtime);
						break;
					case "Update Template":
						log = new Update(logArray, logtime);
						break;
					case "TX Template":
						log = new TX(logArray, logtime);
						break;
				}
				if(log != null)
					this.List.Add(log);
			}
		}

		public LogsCollection<Template> ImportLogFile(FileInfo logfile) {
			LogsCollection<Template> list = new LogsCollection<Template>();
			list.LogFile = logfile;
			List<string> temp = ReadLogFile(logfile);

			foreach (string logStr in temp) {
				string[] strTofind = { "\r\n" };
				string[] logArray = logStr.Split(strTofind, StringSplitOptions.None);
				Template log = null;

				switch(logArray[0].Split('-')[1].Substring(1)) {
					case "Troubleshoot Template":
						log = new TroubleShoot(logArray, list.logFileDate);
						break;
					case "Failed CRQ":
						log = new FailedCRQ(logArray, list.logFileDate);
						break;
					case "Update Template":
						log = new Update(logArray, list.logFileDate);
						break;
					case "TX Template":
						log = new TX(logArray, list.logFileDate);
						break;
				}
				if(log != null)
					list.Add(log);
			}

			return list;
		}
		
		List<string> ReadLogFile() {
			List<string> list = new List<string>();
			string[] strTofind = { "\r\n" };
			using (StreamReader sr = LogFile.OpenText())
			{
				string s = "";
				string tempLog = string.Empty;
				string sepline = logSeparator.Split(strTofind, StringSplitOptions.None)[0]; // get separator 1st line
				while ((s = sr.ReadLine()) != null) {
					if(s == sepline) {
						if(!string.IsNullOrEmpty(tempLog)) {
							list.Add(tempLog.Substring(0, tempLog.Length - Environment.NewLine.Length));
							tempLog = string.Empty;
						}
					}
					else
						tempLog += s + Environment.NewLine;
				}
			}
			return list;
		}
		
		List<string> ReadLogFile(FileInfo logFile) {
			List<string> list = new List<string>();
			string[] strTofind = { "\r\n" };
			using (StreamReader sr = logFile.OpenText())
			{
				string s = "";
				string tempLog = string.Empty;
				string sepline = logSeparator.Split(strTofind, StringSplitOptions.None)[0]; // get separator 1st line
				while ((s = sr.ReadLine()) != null) {
					if(s == sepline) {
						if(!string.IsNullOrEmpty(tempLog)) {
							list.Add(tempLog.Substring(0, tempLog.Length - 1));
							tempLog = string.Empty;
						}
					}
					else
						tempLog += s + Environment.NewLine;
				}
			}
			return list;
		}
		
		int CheckLogExists(T n)
		{
			int existingLog = -1;
			string searchKey = string.Empty;
			
			switch(n.LogType) {
				case "Troubleshoot":
					searchKey = n.fullLog.Substring(n.fullLog.IndexOf("INC: "), 20); //.Substring(5);
					break;
				case "Failed CRQ":
					searchKey = n.fullLog.Substring(n.fullLog.IndexOf("CRQ: "), 20);
					break;
				case "Update":
					searchKey = n.fullLog.Substring(n.fullLog.IndexOf("INC: "), 20);
					break;
				case "TX Template":
					searchKey = "Site(s) ref: ";
					break;
			}
			foreach(T log in this.List){
				if(log.LogType == n.LogType) {
					if(log.fullLog.Contains(searchKey)) {
						existingLog = this.List.IndexOf(log);
						break;
					}
				}
			}
//			var test = this.List.Cast<Template>().Select(x => x.LogType == n.LogType && x.fullLog.Contains(searchKey)).Count();
			return existingLog;
		}
		
		void UpdateLogFile(T n, int existingLogIndex)
		{
//			TODO: BCP Template - search on logs
//				>1 per INC
//				Find all INCs and compare contents
			
			switch(n.LogType) {
				case "Troubleshoot": case "Failed CRQ": case "Update":
					T existingLog = (T)List[existingLogIndex];
					if (n.fullLog != existingLog.fullLog) {
						//						Toolbox.ScrollableMessageBox msgBox = new Toolbox.ScrollableMessageBox();
						//						Action action = new Action(delegate {
						//						msgBox.StartPosition = FormStartPosition.CenterParent;
						//						msgBox.Show(existingLog.fullLog, "Existing log found", MessageBoxButtons.YesNo, "Overwrite existing log?",false);
						DialogResult res = DialogResult.No;
//						Action actionNonThreaded = new Action(delegate {
						if (!ForceOverwriteLog)
							res = FlexibleMessageBox.Show("Overwrite existing log?" + Environment.NewLine + Environment.NewLine + existingLog.fullLog, "Existing log found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
//						                           });
//						LoadingPanel load = new LoadingPanel();
//						load.Show(actionNonThreaded, false, this);
//						Toolbox.Tools.darkenBackgroundForm(action,false,this);
						if (res == DialogResult.Yes || ForceOverwriteLog) {
							RemoveLog(existingLogIndex);
							WriteLog(n);
						}
						else return;
					}
					break;
				case "TX":
//						foreach(int index in LogIndex) {
//							string tempLog = Logs[index].Substring(Logs[index].IndexOf("\r\n", StringComparison.Ordinal) + 2,Logs[index].Length - (Logs[index].IndexOf("\r\n", StringComparison.Ordinal) + 2));
//							if(tempLog == ToLog)
//								return;
//						}
//						ToLog += Environment.NewLine + separator;
//						addLog(ToLog,LogType);
					break;
				case "BCP":
					break;
			}
		}

		public void HandleLog(T n, bool overwrite = false) {
			CheckLogFileIntegrity();
			ForceOverwriteLog = overwrite;
			int existingLogIndex = CheckLogExists(n);
			if(existingLogIndex == -1)
				WriteLog(n);
			else
				UpdateLogFile(n, existingLogIndex);
		}

		public void HandleOutageLog(T n) {
			CheckLogFileIntegrity();
			int existingLogIndex = CheckLogExists(n);
			if(existingLogIndex == -1)
				WriteLog(n);
			else
				UpdateLogFile(n, existingLogIndex);
		}

		void WriteLog(T n) {
//			FileInfo fi = new FileInfo(@"c:\MyTest.txt");
//
//			// This text is added only once to the file.
//			if (!fi.Exists)
//			{
//				//Create a file to write to.
//				using (StreamWriter sw = fi.CreateText())
//				{
//					sw.WriteLine("Hello");
//					sw.WriteLine("And");
//					sw.WriteLine("Welcome");
//				}
//			}
//
//			// This text will always be added, making the file longer over time
//			// if it is not deleted.
//			using (StreamWriter sw = fi.AppendText())
//			{
//				sw.WriteLine("This");
//				sw.WriteLine("is Extra");
//				sw.WriteLine("Text");
//			}
//
//			//Open the file to read from.
//			using (StreamReader sr = fi.OpenText())
//			{
//				string s = "";
//				while ((s = sr.ReadLine()) != null)
//				{
//					Console.WriteLine(s);
//				}
//			}
//		Retry:
//
//			try
//			{
//				if(List.Count > 0 && !LogFile.Exists)
//					LogFile = new FileInfo(LogFile.FullName);
			
			if (LogFile.Exists) {
			Retry:
				try {
					using (StreamWriter sw = LogFile.AppendText())
					{
						sw.WriteLine();
						string logtype = n.LogType;
						if(logtype != "Failed CRQ")
							logtype += " Template";
						sw.WriteLine(n.GenerationDate.ToString("HH:mm:ss") + " - " + logtype);
						sw.WriteLine(n.fullLog);
//						if(logtype == "Troubleshoot") {
//							TroubleShoot temp = (TroubleShoot)n;
//						}
						sw.Write(logSeparator);
					}
				}
				catch (Exception e) {
					int errorCode = (int)(e.HResult & 0x0000FFFF);
					DialogResult ans;
					switch(errorCode) {
						case 32:
							ans = ErrorHandling.showFileInUseDuringLogFileOperation;
							break;
						case 112:
							ans = ErrorHandling.showLowSpaceWarningDuringLogFileOperation;
							break;
						default:
							ans = DialogResult.None;
							break;
					}
					if(ans == DialogResult.Retry)
						goto Retry;
				}
			}
			else {
			Retry2:
				try {
					using (StreamWriter sw = LogFile.CreateText())
					{
						string logtype = n.LogType;
						if(logtype != "Failed CRQ")
							logtype += " Template";
						sw.WriteLine(n.GenerationDate.ToString("HH:mm:ss") + " - " + logtype);
						sw.WriteLine(n.fullLog);
						sw.Write(logSeparator);
					}
					LogFile = new FileInfo(LogFile.FullName);
				}
				catch (Exception e) {
					int errorCode = (int)(e.HResult & 0x0000FFFF);
					DialogResult ans;
					switch(errorCode) {
						case 32:
							ans = ErrorHandling.showFileInUseDuringLogFileOperation;
							break;
						case 112:
							ans = ErrorHandling.showLowSpaceWarningDuringLogFileOperation;
							break;
						default:
							ans = DialogResult.None;
							break;
					}
					if(ans == DialogResult.Retry)
						goto Retry2;
				}
			}
//			}
//			catch (IOException)
//			{
			////				Action action = new Action(delegate {
//				FlexibleMessageBox.Show("Log file is currently in use, please close it and press OK to retry","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
			////				                           });
			////				Toolbox.Tools.darkenBackgroundForm(action,false,this);
//				goto Retry;
//			}
			this.List.Add(n);
			MainForm.UpdateTicketCountLabel();
		}
		
		void RemoveLog(int index) {
			string content = string.Empty;
			string[] strTofind = { "\r\n" };
			string sepline = logSeparator.Split(strTofind, StringSplitOptions.None)[0]; // get separator 1st line
			
			List<string> logsList = ReadLogFile();
			
			logsList.RemoveAt(index);
			
			if(logsList.Count > 0) {
				using(StreamWriter sw = new StreamWriter(LogFile.FullName, false)) {
					foreach (string logStr in logsList) {
						sw.WriteLine(logStr);
						if(logsList.IndexOf(logStr) == logsList.Count - 1)
							sw.Write(logSeparator);
						else
							sw.WriteLine(logSeparator);
					}
				}
			}
			else {
				LogFile.Delete();
				LogFile = new FileInfo(LogFile.FullName);
			}
			
			List.RemoveAt(index);
		}

		static string getLogSeparator() {
			string sep = string.Empty;
			for (int c = 1; c < 301; c++) {
				if (c == 151) sep += "\r\n";
				sep += "*";
			}
			return sep;
		}

		public T this[int index]
		{
			get { return (T)this.List[index]; }
			set { this.List[index] = value; }
		}

		public int IndexOf(T item)
		{
			return this.List.IndexOf(item);
		}

		public bool Contains(T item)
		{
			return this.List.Contains(item);
		}

		public int Add(T item)
		{
			return this.List.Add(item);
		}
		
		public int FilterCounts(Template.Filters filter) {
			int count = 0;
			switch(filter) {
				case Template.Filters.Troubleshoot:
					foreach(Template t in this.List) {
						if(t.LogType == "Troubleshoot")
							count++;
					}
					return count;
				case Template.Filters.FailedCRQ:
					foreach(Template t in this.List) {
						if(t.LogType == "Failed CRQ")
							count++;
					}
					return count;
				case Template.Filters.TX:
					foreach(Template t in this.List) {
						if(t.LogType == "TX")
							count++;
					}
					return count;
				case Template.Filters.Update:
					foreach(Template t in this.List) {
						if(t.LogType == "Update")
							count++;
					}
					return count;
				case Template.Filters.Outage:
					foreach(Template t in this.List) {
						if(t.LogType == "Outage")
							count++;
					}
					return count;
				case Template.Filters.TicketCount:
					foreach(Template t in this.List) {
						if(t.LogType == "Troubleshoot" || t.LogType == "Failed CRQ" || t.LogType == "TX")
							count++;
					}
					return count;
			}
			return -1;
		}
		
//		void LogOutageReport()
//		{
//			string separator = string.Empty;
//			for (int i = 1; i < 301; i++) {
//				if (i == 151) separator += Environment.NewLine;
//				separator += "*";
//			}
//
//			string ToLog = string.Empty;
//			if(!string.IsNullOrEmpty(VFoutage))
//				ToLog = "----------VF Report----------" + Environment.NewLine + VFoutage + Environment.NewLine + "-----BulkCI-----" + Environment.NewLine + VFbulkCI + Environment.NewLine;
//			if(!string.IsNullOrEmpty(TFoutage))
//				ToLog += "----------TF Report----------" + Environment.NewLine + TFoutage + Environment.NewLine + "-----BulkCI-----" + Environment.NewLine + TFbulkCI + Environment.NewLine;
//			ToLog += separator;
//
//		Retry:
//
//			try
//			{
//				if (CheckLogFileExists("outage")) File.AppendAllText(LogFilePath + "\\outages\\" + dt.ToString("dd",culture) + ".txt",Environment.NewLine + dt.ToString("HH:mm:ss") + Environment.NewLine + ToLog);
//				else File.WriteAllText(LogFilePath + "\\outages\\" + dt.ToString("dd",culture) + ".txt",dt.ToString("HH:mm:ss")  + Environment.NewLine + ToLog);
//			}
//			catch (IOException)
//			{
//				Action action = new Action(delegate {
//				                           	MessageBox.Show("Log file is currently in use, please close it and press OK to retry","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
//				                           });
//				Toolbox.Tools.darkenBackgroundForm(action,false,this);
//				goto Retry;
//			}
//		}
//
//		public void Remove(T item)
//		{
//			this.List.Remove(item);
//		}
//
//		public void CopyTo(Array array, int index)
//		{
//			this.List.CopyTo(array, index);
//		}
//
//		public void AddRange(LogsCollection<T> collection)
//		{
//			for (int i = 0; i < collection.Count; i++)
//			{
//				this.List.Add(collection[i]);
//			}
//		}
//
//		public void AddRange(T[] collection)
//		{
//			this.AddRange(collection);
//		}

//		public void Insert(int index, T item)
//		{
//			this.List.Insert(index, item);
//		}
	}
}
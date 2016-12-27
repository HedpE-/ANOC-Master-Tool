namespace appCore.Toolbox
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
//	using System.DirectoryServices;
//	using System.DirectoryServices.AccountManagement;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Security.Cryptography;
	using System.Text;
	using System.Windows.Forms;
//	using Excel;
//	using msExcel = Microsoft.Office.Interop.Excel;
	using Outlook = Microsoft.Office.Interop.Outlook;
	using appCore.UI;
	using appCore.Settings;
	using appCore.DB;
	using HtmlAgilityPack;
	
	public static class Tools
	{
		public enum Months : byte {
			January,
			February,
			March,
			April,
			May,
			June,
			July,
			August,
			September,
			October,
			November,
			December
		};
		
		public static string[] DirSearch(string sDir)
		{
			List<string> result = new List<string>();
			try
			{
				foreach (string d in Directory.GetDirectories(sDir))
				{
					foreach (string f in Directory.GetFiles(d, "pattern"))
					{
						result.Add(f);
					}
					DirSearch(d);
				}
			}
			catch (Exception excpt)
			{
				Console.WriteLine(excpt.Message);
			}
			
			return result.ToArray();
		}
		// TODO: Implement MD5 Hash check
		public static string CalculateMD5Hash(FileInfo filename) {
			MD5 md5 = MD5.Create();
			FileStream stream = File.OpenRead(filename.FullName);
			byte[] hash = md5.ComputeHash(stream);

			return convertByteArrayToHex(hash);
		}
		
		public static string CalculateMD5Hash(string input) {
			// step 1, calculate MD5 hash from input

			MD5 md5 = MD5.Create();
			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			return convertByteArrayToHex(hash);
		}
		
		public static Form getParentForm(Control control) {
			Form parentForm = null;
			if((Form)control == null) {
				while(parentForm == null) {
					control = control.Parent;
					if(control is Form)
						parentForm = control as Form;
				}
			}
			else
				parentForm = (Form)control;
			return parentForm;
		}
		
		public static string convertByteArrayToHex(byte[] hash) {
			// step 2, convert byte array to hex string

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++) {
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}

		public static void CreateMailItem(string sendTo, string sendCC, string mailSubject, string mailBody, bool HTML)
		{
			Type officeType = Type.GetTypeFromProgID("Outlook.Application");

			if (officeType == null) {
				// Outlook is not installed.
				Clipboard.SetText(mailBody);
				MainForm.trayIcon.showBalloon("Outlook is not installed", "Email content copied to the clipboard");
				return;
			}
			// Outlook is installed.
			if (Process.GetProcessesByName("OUTLOOK").Length == 0) {
				Process process = new Process {
					StartInfo = new ProcessStartInfo {
						FileName = "outlook.exe"
					}
				};
				process.Start();
				while (!process.WaitForInputIdle()) {}
				MainForm.trayIcon.showBalloon("Starting Outlook","Please try again once Outlook's ready");
				return;
			}
			
			Outlook.Application application = new Outlook.Application();
			Outlook.Folder defaultFolder = application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderDrafts) as Outlook.Folder;
//			UserFolder.ReadyAMTFailedCRQTempFile();
			FileInfo path = UserFolder.ReadyAMTFailedCRQTempFile(); // create Outlook email template file on userfolder
			
			Outlook.MailItem item = (Outlook.MailItem) application.CreateItemFromTemplate(path.FullName, defaultFolder);
			if(mailSubject == "BCP") item.SentOnBehalfOfName = "1stLineANOCUKRAN@vodafone.com";
			item.To = sendTo;
			item.CC = sendCC;
			item.Subject = mailSubject;
			if (HTML)
				item.HTMLBody = mailBody;
			else
				item.Body = mailBody;
			item.Display(false);
			
			UserFolder.ClearTempFolder(); // delete Outlook email template file on userfolder
		}

		public static string CompleteINC_CRQ_TAS(string num, string prefix)
		{
			if ((num.Length < 1) || (num.Length >= 15))
				return num;
			
			num = num.Replace(prefix, string.Empty);
			if (!IsAllDigits(num))
				return "error";
			
			if (num.Length < 13) {
				int num2 = 15 - (num.Length + 3);
				for (int i = 1; i <= num2; i++) {
					num = "0" + num;
				}
			}
			return (prefix + num);
		}
		
		public static void darkenBackgroundForm(Action action, bool showLoading, Control control)
		{
			// Resolve Parent Form if needed
			Form parentForm = control is Form ? control as Form : getParentForm(control);
			// take a screenshot of the form and darken it
			Bitmap bmp = new Bitmap(parentForm.ClientRectangle.Width, parentForm.ClientRectangle.Height);
			using (Graphics G = Graphics.FromImage(bmp))
			{
				G.CompositingMode = CompositingMode.SourceOver;
				G.CopyFromScreen(parentForm.PointToScreen(new Point(0, 0)), new Point(0, 0), parentForm.ClientRectangle.Size);
				const double percent = 0.40;
				Color darken = Color.FromArgb((int)(255 * percent), Color.Black);
				using (Brush brsh = new SolidBrush(darken))
				{
					G.FillRectangle(brsh, parentForm.ClientRectangle);
				}
			}

			// put the darkened screenshot into a Panel and bring it to the front:
			using (Panel p = new Panel())
			{
				p.Location = new Point(0, 0);
				p.Size = parentForm.ClientRectangle.Size;
				p.BackgroundImage = bmp;
				parentForm.Controls.Add(p);
				p.BringToFront();
				
				// display your dialog somehow:
				if(showLoading) {
					Point loc = p.PointToScreen(Point.Empty);
					loc.X = loc.X + ((p.Width - 32) / 2);
					loc.Y = loc.Y + ((p.Height - 32) / 2);
//					PictureBox loadingBox = new PictureBox();
//					loadingBox.Image = Resources.spinner1;
//					loadingBox.Location = loc;
//					p.Controls.Add(loadingBox);
					Loading.ShowLoadingForm(loc, parentForm);
				}
				
				action();
				
				if(showLoading)
					Loading.CloseLoadingForm();
			} // panel will be disposed and the form will "lighten" again...
		}
		
		/// <summary>
		/// Valid action values: "Enc","Dec"
		/// </summary>
		public static string EncryptDecryptText(string action, string text)
		{
			if(!string.IsNullOrEmpty(text)) {
				string str = string.Empty;
				switch(action) {
					case "Enc":
						foreach (char ch in text)
							str += Convert.ToInt32(ch).ToString("x");
						text = str;
						break;
					case "Dec":
						text = text.Replace(" ", "");
						byte[] bytes = new byte[text.Length / 2];
						for (int i = 0; i < bytes.Length; i++)
							bytes[i] = Convert.ToByte(text.Substring(i * 2, 2), 0x10);
						
						text = Encoding.ASCII.GetString(bytes);
						break;
				}
			}
			return text;
		}
		
		public static bool IsAllDigits(string s)
		{
			foreach (char ch in s)
				if (!char.IsDigit(ch))
					return false;
			
			return true;
		}
		
//		static string ReadSignature()
//		{
//			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Signatures";
//			string str = string.Empty;
//			DirectoryInfo info = new DirectoryInfo(path);
//			if (info.Exists)
//			{
//				FileInfo[] files = info.GetFiles("*.htm");
//				if (files.Length < 1)
//					return str;
//				str = new StreamReader(files[0].FullName, Encoding.Default).ReadToEnd();
//				if (!string.IsNullOrEmpty(str)) {
//					string str2 = files[0].Name.Replace(files[0].Extension, string.Empty);
//					str = str.Replace(str2 + "_files/", path + "/" + str2 + "_files/");
//				}
//			}
//			return str;
//		}
		
		//        public const int GW_HWNDNEXT = 2;
		//        public const int GW_HWNDPREV = 3;

		//        [return: MarshalAs(UnmanagedType.Bool)]
		//        [DllImport("user32.dll")]
		//        private static extern bool IsWindowVisible(IntPtr hWnd);
		
		//        [DllImport("user32.dll", EntryPoint="GetWindow", CharSet=CharSet.Auto, SetLastError=true)]
		//        public static extern IntPtr GetNextWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int wFlag);
		//        public static Form GetTopMostWindow(IntPtr hWnd_mainFrm)
		//        {
		//        	Form form = null;
		//        	IntPtr topWindow = GetTopWindow(IntPtr.Zero);
		//        	if (!(topWindow != IntPtr.Zero))
		//        	{
		//        		return form;
		//        	}
		//        	Label_0045:;
		//        	if ((!IsWindowVisible(topWindow) || (form == null)) && (topWindow != hWnd_mainFrm))
		//        	{
		//        		topWindow = GetNextWindow(topWindow, 2);
		//        		try
		//        		{
		//        			form = (Form) Control.FromHandle(topWindow);
		//        		}
		//        		catch
		//        		{
		//        		}
		//        		goto Label_0045;
		//        	}
		//        	return form;
		//        }
//
		//        [DllImport("user32.dll")]
		//        private static extern IntPtr GetTopWindow(IntPtr hWnd);public static string StripTagsCharArray(string source)
		
//		public static string StripTagsCharArray(string source) {
//			char[] array = new char[source.Length];
//			int arrayIndex = 0;
//			bool inside = false;
//
//			for (int i = 0; i < source.Length; i++)
//			{
//				char let = source[i];
//				if (let == '<')
//				{
//					inside = true;
//					continue;
//				}
//				if (let == '>')
//				{
//					inside = false;
//					continue;
//				}
//				if (!inside)
//				{
//					array[arrayIndex] = let;
//					arrayIndex++;
//				}
//			}
//			return new string(array, 0, arrayIndex);
//		}
		
		public static void EmbeddedAssembliesInit()
		{
			// http://www.codeproject.com/Articles/528178/Load-DLL-From-Embedded-Resource
			
			EmbeddedAssembly.Load("appCore.GMap.NET.Lib.GMap.NET.Core.dll", "GMap.NET.Core.dll");
			EmbeddedAssembly.Load("appCore.GMap.NET.Lib.GMap.NET.WindowsForms.dll", "GMap.NET.WindowsForms.dll");
			EmbeddedAssembly.Load("appCore.Extensions.Transitions.dll", "Transitions.dll");
			EmbeddedAssembly.Load("appCore.Extensions.RestSharp.dll", "RestSharp.dll");
			EmbeddedAssembly.Load("appCore.Extensions.Excel.dll", "Excel.dll");
			EmbeddedAssembly.Load("appCore.Extensions.Outlook.dll", "Outlook.dll");
			EmbeddedAssembly.Load("appCore.Extensions.ICSharpCode.SharpZipLib.dll", "ICSharpCode.SharpZipLib.dll");
			EmbeddedAssembly.Load("appCore.Extensions.BMC.ARSystem.dll", "BMC.ARSystem.dll");
			
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}
		
		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return EmbeddedAssembly.Get(args.Name);
		}

		public static int CountStringOccurrences(string text, string pattern)
		{
			int num = 0;
			int startIndex = 0;
			while ((startIndex = text.IndexOf(pattern, startIndex, StringComparison.Ordinal)) != -1) {
				startIndex += pattern.Length;
				num++;
			}
			return num;
		}
		
		public static string RemoveDiacritics(string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}
		
		public static void CopyProperties(object dst, object src) {
			PropertyInfo[] srcProperties = src.GetType().GetProperties();
			dynamic dstType = dst.GetType();

			if (srcProperties == null || dstType.GetProperties() == null) {
				return;
			}

			foreach (PropertyInfo srcProperty in srcProperties) {
				PropertyInfo dstProperty = dstType.GetProperty(srcProperty.Name);

				if (dstProperty != null) {
					if(dstProperty.CanWrite) {
						if(dstProperty.PropertyType.IsAssignableFrom(srcProperty.PropertyType)) {
							dstProperty.SetValue(dst, srcProperty.GetValue(src, null), null);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Parse HTML tables from OI to DataTable
		/// </summary>
		/// <param name="html">HTML returned from OI</param>
		/// <param name="tableName">Table Name. Valid values: "table_inc", "table_crq", "table_alarms", "table_visits"</param>
		public static DataTable ConvertHtmlTabletoDataTable(string html, string tableName) {
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.Load(new StringReader(html));
			DataTable dt = new DataTable();
			dt.TableName = tableName;
			string csv = string.Empty;
			
			// Build DataTable Headers ("table_visits" has headers inside <tr> tag, unlike the other tables)
			foreach (HtmlNode th in doc.DocumentNode.SelectNodes("//table[@id='" + tableName + "']").Descendants("th")) {
				if(th.InnerText.Contains("Date") || th.InnerText.Contains("Scheduled") || th.InnerText == "Arrived" || th.InnerText == "Planned Finish" || th.InnerText == "Departed Site")
					dt.Columns.Add(th.InnerText, typeof(DateTime));
				else
					dt.Columns.Add(th.InnerText);
			}
			
			// Build DataTable
			List<string> tableRow = new List<string>();
			foreach (HtmlNode tr in doc.DocumentNode.SelectNodes("//table[@id='" + tableName + "']").Descendants("tr")) {
				tableRow.Clear();
				if(tr.Name != "#text") {
					foreach(var node in tr.ChildNodes) {
						if(node.Name != "td") // && node.Name != "th")
							continue;
						
						tableRow.Add(node.InnerText);
					}
				}
				
				if(tableRow.Count > 0) {
					DataRow dataRow = dt.NewRow();
					for(int c = 0;c < tableRow.Count;c++) {
						if(dt.Columns[c].DataType == typeof(DateTime)) {
							if(!string.IsNullOrWhiteSpace(tableRow[c]))
								dataRow[c] = Convert.ToDateTime(tableRow[c]);
						}
						else
							dataRow[c] = tableRow[c];
					}
					dt.Rows.Add(dataRow);
				}
			}
			
			return dt;
		}
		
		/// <summary>
		/// Parse HTML tables from OI to CSV format
		/// </summary>
		/// <param name="html">HTML returned from OI</param>
		/// <param name="tableName">Table Name. Valid values: "table_inc", "table_crq", "table_alarms", "table_visits"</param>
		public static string ConvertHtmlTabletoCSV(string html, string tableName) {
			if(tableName == "table_visits") {
				html = html.Substring(html.IndexOf("<table"));
				html = html.Substring(0, html.IndexOf("</table>") + 8);
			}
			
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.Load(new StringReader(html));
			string csv = string.Empty;
			
			// Build Headers CSV string ("table_visits" has headers inside <tr> tag, unlike the other tables)
			if(tableName != "table_visits") {
				foreach (HtmlNode th in doc.DocumentNode.SelectNodes("//table[@id='" + tableName + "']").Descendants("th"))
					csv += th.InnerText+ ',';
				if(csv.Length > 0) {
					csv = csv.Substring(0, csv.Length - 1);
					csv += Environment.NewLine;
				}
			}
			
			// Build Table CSV string
			foreach (HtmlNode tr in doc.DocumentNode.SelectNodes("//table[@id='" + tableName + "']").Descendants("tr"))
			{
				if(tr.Name != "#text") {
					foreach(var node in tr.ChildNodes)
					{
						if(node.Name != "td" && node.Name != "th")
							continue;
						
						csv += node.InnerText.Replace(',',';').Replace("\r\n", " ").Replace('\n', ' ') + ',';
					}
					if(csv.Length > 0) {
						csv = csv.Substring(0, csv.Length - 1);
						csv += Environment.NewLine;
					}
				}
			}
			csv = csv.Substring(0, csv.Length - 2); // Delete last Environment.NewLine \r\n
			
			return csv;
		}
		
		public static DataTable GetDataTableFromCsv(FileInfo CsvFile, bool isFirstRowHeader) {
			DataTable dataTable = new DataTable();
			
			using (var stream = CsvFile.OpenRead())
				using (var reader = new StreamReader(stream))
			{
				dataTable = ParseCsv(reader, isFirstRowHeader);
			}
			
			return dataTable;
		}
		
		public static DataTable GetDataTableFromCsv(string CsvString, bool isFirstRowHeader) {
			DataTable dataTable = ParseCsv(new StringReader(CsvString), isFirstRowHeader);
			
			return dataTable;
		}
		
		static DataTable ParseCsv(TextReader reader, bool isFirstRowHeader) {
			DataTable dataTable = new DataTable();
			
			string header = isFirstRowHeader ? "YES" : "NO";
			
			var data = CsvParser.ParseHeadAndTail(reader, ',', '"');

			var headers = data.Item1;
			
			foreach (var head in headers) {
				DataColumn dataColumn = new DataColumn(head);
				dataTable.Columns.Add(dataColumn);
			}
			
			var lines = data.Item2;

			foreach (var line in lines)
			{
				DataRow dataRow = dataTable.NewRow();
				for(int c=0;c < line.Count();c++) {
					dataRow[c] = line[c];
				}
				dataTable.Rows.Add(dataRow);
			}
			
			return dataTable;
		}
	}
}

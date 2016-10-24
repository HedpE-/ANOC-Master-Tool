namespace appCore.Toolbox
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.DirectoryServices;
	using System.DirectoryServices.AccountManagement;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading;
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
			
			UserFolder.ReleaseAMTFailedCRQTempFile(); // delete Outlook email template file on userfolder
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
		
		public static void darkenBackgroundForm(Action action, bool showLoading, Form parentForm )
		{
			// FIXME: darkenBackgroundForm on new templates
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
		
		public const int shiftsRectWidth = 30; //the width of the rectangle
		public const int shiftsRectHeight = 20; //the height of the rectangle
		
		public static Bitmap loadShifts(Bitmap shiftsSnap, DateTime date) {
			// FIXME: pictureBoxes UI glitches on shiftsPanel
			shiftsSnap = null;
			
			MainForm.rectCollection.Clear();
			byte first_weekday = (byte)(new DateTime(date.Year,date.Month,1).DayOfWeek);
			if(first_weekday == 0) first_weekday = 6; // if month starts on Sunday, 6 because Sunday is actually weekday 0
			else first_weekday--;
			int num_days = DateTime.DaysInMonth(date.Year, date.Month); // find how many days are in the selected month
			int num_lines = (int)Math.Ceiling((double)(first_weekday + num_days) / (double)7);
			int panelHeaderWidth = MainForm.shiftsPanel.Controls["shiftsPanel_refresh"].Left - MainForm.shiftsPanel.Controls["shiftsPanel_icon"].Right;
			int panelBodyHeight = (int)((shiftsRectHeight * 2) + 3 + ((num_lines * 2) * shiftsRectHeight) + 10); // (height * 2) + 3 for title and weedays headers; + 10 for 5 padding on top&bottom
			MainForm.foundRows = Databases.shiftsFile.monthTables[date.Month - 1].Select("AbsName Like '" + RemoveDiacritics(CurrentUser.fullName[1]).ToUpper() + "%' AND AbsName Like '%" + RemoveDiacritics(CurrentUser.fullName[0]).ToUpper() + "'");
			
//			shiftsHeaderSnap = new Bitmap(MainForm.shiftsPanel.Controls["shiftsPanel_refresh"].Right + MainForm.shiftsPanel.Controls["shiftsPanel_icon"].Right, shiftsRectHeight);
			shiftsSnap = new Bitmap(224, panelBodyHeight);
			
			using (Graphics g = Graphics.FromImage(shiftsSnap)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;    // Set format of string.
				g.FillRectangle(new SolidBrush(MainForm.shiftsPanel.BackColor), 0, 0, shiftsSnap.Width, shiftsSnap.Height);
				
				StringFormat drawStringFormat = new StringFormat();
				drawStringFormat.Alignment = StringAlignment.Center;
				drawStringFormat.LineAlignment = StringAlignment.Far;
				using (Pen pen = new Pen(Color.Black, 1)) {
					string title = Enum.GetName(typeof(Months),date.Month - 1) + " " + date.Year;
					Rectangle rectangle = new Rectangle(new Point(7, 0), new Size(shiftsRectWidth * 7, shiftsRectHeight + 3));
					g.DrawString(title, new Font("Tahoma",12, FontStyle.Bold), Brushes.Black, rectangle, drawStringFormat);
					drawStringFormat.LineAlignment = StringAlignment.Center;
					Font stringFont = new Font("Tahoma",8, FontStyle.Bold);
					Brush normalRectFill = new SolidBrush(Color.FromArgb((int)(255 * 0.3), Color.Black));
					Brush curDayFill = new SolidBrush(Color.FromArgb((int)(255 * 0.6), Color.Black));
					
					for (int index = 0; index < 7; index++)
					{
						Brush weekdaysBrush = new SolidBrush(Color.FromArgb((int)(255 * 0.8), Color.Black));
						Rectangle rect = new Rectangle(new Point(7 + shiftsRectWidth * index, 25 + shiftsRectHeight * 0), new Size(shiftsRectWidth, shiftsRectHeight));
						string text = Enum.GetName(typeof(DayOfWeek),(index + 1) % 7).Substring(0,3);
						
						g.FillRectangle(weekdaysBrush, rect);
						g.DrawRectangle(pen, rect);
						g.DrawString(text, stringFont, Brushes.LightGray, rect, drawStringFormat);
					}
					
					int curDay = 1 - first_weekday; // will get negative value if not monday, loop below will only write anything when this value is over 0
					
					for(int line = 1;line <= num_lines * 2;line+=2) {
						for(int col = 0;col < 7;col++) {
							// Draw 1st box for day number
							string text = string.Empty;
							Rectangle rect1 = new Rectangle(new Point(7 + shiftsRectWidth * col, 25 + shiftsRectHeight * (line)), new Size(shiftsRectWidth, shiftsRectHeight));
							
							if(curDay > 0 && curDay <= num_days) {
								text = curDay.ToString();
								g.FillRectangle(curDayFill, rect1);
							}
							else
								g.FillRectangle(normalRectFill, rect1);
							g.DrawRectangle(pen, rect1);
							g.DrawString(text, stringFont, Brushes.DarkGray, rect1, drawStringFormat);
							
							// Draw 2nd box for shift text
							text = string.Empty;
							Rectangle rect2 = new Rectangle(new Point(7 + shiftsRectWidth * col, 25 + shiftsRectHeight * (line + 1)), new Size(shiftsRectWidth, shiftsRectHeight));
							if(curDay == DateTime.Now.Day)
								g.FillRectangle(curDayFill, rect2);
							else
								g.FillRectangle(normalRectFill, rect2);
							Brush drawBrush = Brushes.LightGray;
							if(curDay > 0 && curDay <= num_days) {
								text = MainForm.foundRows[0]["Day" + curDay].ToString();
								switch(text) {
									case "M":
										drawBrush = Brushes.ForestGreen;
										break;
									case "A":
										drawBrush = Brushes.OrangeRed;
										break;
									case "N":
										drawBrush = Brushes.Red;
										break;
								}
							}
							
							g.DrawRectangle(pen, rect2);
							g.DrawString(text, stringFont, drawBrush, rect2, drawStringFormat);
							if(curDay > 0 && curDay <= num_days)
								MainForm.rectCollection.Add(rect2);
//								MainForm.rectCollection.Add(Rectangle.Union(rect1,rect2));
							curDay++;
						}
					}
				}
//				shiftsSnap.Save(MainForm.UserFolderPath + @"\shiftsSnap.png");
			}
			return shiftsSnap;
		}
		
		public static DataRow[] getWholeShift(DateTime date) {
			string shift = Databases.shiftsFile.monthTables[date.Month - 1].Select("AbsName Like '" + RemoveDiacritics(CurrentUser.fullName[1]).ToUpper() + "%' AND AbsName Like '%" + RemoveDiacritics(CurrentUser.fullName[0]).ToUpper() + "'")[0]["Day" + date.Day].ToString();
			if(shift.StartsWith("H"))
				return null;
//			string dayColumnToFilter = MainForm.shiftsTable[date.Month - 1].Columns[date.Day + 3].ColumnName;
			string filter = string.Empty;
			switch (shift) {
				case "M":
					filter = "AbsName <> '' AND Day" + date.Day + " <> 'A' AND Day" + date.Day + " <> 'N' AND Day" + date.Day + " <> 'H' AND Day" + date.Day + " <> 'HA' AND Day" + date.Day + " <> 'L'";
					break;
				case "A":
					filter = "AbsName <> '' AND Day" + date.Day + " <> 'M' AND Day" + date.Day + " <> 'N' AND Day" + date.Day + " <> 'H' AND Day" + date.Day + " <> 'HA' AND Day" + date.Day + " <> 'L'";
					break;
				case "N":
					filter = "AbsName <> '' AND Day" + date.Day + " = '" + shift + "'";
					break;
				default:
					filter = "AbsName <> '' AND Day" + date.Day + " <> 'N' AND Day" + date.Day + " <> 'H' AND Day" + date.Day + " <> 'HA' AND Day" + date.Day + " <> 'L'";
					break;
			}
			
			DataRow[] foundRows = Databases.shiftsFile.monthTables[date.Month - 1].Select(filter);
			
			return foundRows;
		}
		
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
			
			EmbeddedAssembly.Load("appCore.GMap.NET.Lib.GMap.NET.Core.dll", "GMap.NET.WindowsForms.dll");
			EmbeddedAssembly.Load("appCore.GMap.NET.Lib.GMap.NET.WindowsForms.dll", "GMap.NET.Core.dll");
			EmbeddedAssembly.Load("appCore.Extensions.Transitions.dll", "Transitions.dll");
			EmbeddedAssembly.Load("appCore.Extensions.RestSharp.dll", "RestSharp.dll");
			EmbeddedAssembly.Load("appCore.Extensions.Excel.dll", "Excel.dll");
			EmbeddedAssembly.Load("appCore.Extensions.ICSharpCode.SharpZipLib.dll", "ICSharpCode.SharpZipLib.dll");
			EmbeddedAssembly.Load("appCore.Extensions.log4net.dll", "log4net.dll");
			EmbeddedAssembly.Load("appCore.Extensions.HtmlAgilityPack.dll", "HtmlAgilityPack.dll");
			
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
//			if(tableName == "table_visits") {
//				html = html.Substring(html.IndexOf("<table"));
//				html = html.Substring(0, html.IndexOf("</table>") + 8);
//			}
			
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.Load(new StringReader(html));
			DataTable dt = new DataTable();
			string csv = string.Empty;
			
			// Build DataTable Headers ("table_visits" has headers inside <tr> tag, unlike the other tables)
			string test;
			string test2;
//			if(tableName != "table_visits") {
			foreach (HtmlNode th in doc.DocumentNode.SelectNodes("//table[@id='" + tableName + "']").Descendants("th")) {
				dt.Columns.Add(th.InnerText);
			}
			
			// Build DataTable
			List<string> tableRow = new List<string>();
			foreach (HtmlNode tr in doc.DocumentNode.SelectNodes("//table[@id='" + tableName + "']").Descendants("tr")) {
				tableRow.Clear();
				if(tr.Name != "#text") {
					foreach(var node in tr.ChildNodes) {
						test = node.Name;
						test2 = node.InnerText;
						if(node.Name != "td") // && node.Name != "th")
							continue;
						
						tableRow.Add(node.InnerText);
					}
				}
				
				if(tableRow.Count > 0) {
					DataRow dataRow = dt.NewRow();
					for(int c = 0;c < tableRow.Count;c++) {
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

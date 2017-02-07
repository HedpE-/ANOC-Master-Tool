using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using appCore.UI;
using appCore.Settings;
using HtmlAgilityPack;

namespace appCore.Toolbox
{
	public static class Tools
	{
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
			if(control is Form)
				parentForm = (Form)control;
			else {
				while(parentForm == null) {
					try {
						control = control.Parent;
						if(control is Form)
							parentForm = (Form)control;
					}
					catch {}
				}
			}
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
			if (!num.IsAllDigits())
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
		
		public static void CopyProperties(object dst, object src) {
			PropertyInfo[] srcProperties = src.GetType().GetProperties();
			dynamic dstType = dst.GetType();

			if (srcProperties == null || dstType.GetProperties() == null)
				return;

			foreach (PropertyInfo srcProperty in srcProperties) {
				PropertyInfo dstProperty = dstType.GetProperty(srcProperty.Name);

				if (dstProperty != null) {
					if(dstProperty.CanWrite) {
						if(dstProperty.PropertyType.IsAssignableFrom(srcProperty.PropertyType))
							dstProperty.SetValue(dst, srcProperty.GetValue(src, null), null);
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
			
			// Build DataTable Headers ("table_visits" has headers inside <tr> tag, unlike the other tables)
			HtmlNode table = tableName == string.Empty ? doc.DocumentNode.SelectSingleNode("//table") : doc.DocumentNode.SelectSingleNode("//table[@id='" + tableName + "']");
			var descendantNodes = table.Descendants("th");
			foreach (HtmlNode th in descendantNodes) {
				if(th.InnerText.Contains("Date") || th.InnerText.Contains("Scheduled") || th.InnerText == "Arrived" || th.InnerText == "Planned Finish" || th.InnerText == "Departed Site" || th.InnerText == "Time")
					dt.Columns.Add(th.InnerText, typeof(DateTime));
				else {
					if(!dt.Columns.Contains(th.InnerText))
						dt.Columns.Add(th.InnerText);
					else
						dt.Columns.Add(th.InnerText + "2");
				}
			}
			
			// Build DataTable
			descendantNodes = table.Descendants("tr");
			foreach (HtmlNode tr in descendantNodes) {
//				csv += Environment.NewLine;
				List<string> tableRow = new List<string>();
				if(tr.Name != "#text") {
					var childNodes = tr.ChildNodes;
					foreach(var node in childNodes) {
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
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.Load(new StringReader(html));
			string csv = string.Empty;
			
			// Build DataTable Headers ("table_visits" has headers inside <tr> tag, unlike the other tables)
			HtmlNode table = tableName == string.Empty ? doc.DocumentNode.SelectSingleNode("//table") : doc.DocumentNode.SelectSingleNode("//table[@id='" + tableName + "']");
			var descendantNodes = table.Descendants("th");
			foreach (HtmlNode th in descendantNodes) {
				csv += th.InnerText;
				if(th != descendantNodes.Last())
					csv += ',';
			}
			
			// Build DataTable
			descendantNodes = table.Descendants("tr");
			foreach (HtmlNode tr in descendantNodes) {
				csv += Environment.NewLine;
				if(tr.Name != "#text") {
					var childNodes = tr.ChildNodes;
					foreach(var node in childNodes) {
						if(node.Name != "td") // && node.Name != "th")
							continue;
						
						csv += node.InnerText.Replace(',',';').Replace("\n","<<lb>>");
						if(node != childNodes.Last())
							csv += ',';
					}
				}
			}
			
			return csv;
		}
	}
//
//	[DelimitedRecord(","), IgnoreFirst(1)]
//	public class INC {
//		public string Site;
//		public string IncidentRef;
//		public string Summary;
//		public string Priority;
//		public string Status;
//		public string AssigneeGroup;
//		string submitDate;
//		public DateTime SubmitDate {
//			get {
//				return Convert.ToDateTime(submitDate);
//			}
//			private set {}
//		}
//		public string ResolutionCategory2;
//		public string ResolutionCategory3;
//		public string Resolution;
//		string resolvedDate;
//		public DateTime ResolvedDate {
//			get {
//				return Convert.ToDateTime(resolvedDate);
//			}
//			private set {}
//		}
//
//		public INC() {}
//	}
//
//	[DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines]
//	public class BookIns {
//		public string Site;
//		public string Visit;
//		public string Company;
//		public string Engineer;
//		public string Mobile;
//		public string Reference;
//		public string VisitType;
//		public string Arrived;
	////		public DateTime Arrived {
	////			get {
	////				return Convert.ToDateTime(arrived);
	////			}
	////			private set {}
	////		}
//		public string PlannedFinish;
	////		public DateTime PlannedFinish {
	////			get {
	////				return Convert.ToDateTime(plannedFinish);
	////			}
	////			private set {}
	////		}
//		public string TimeTaken;
	////		public DateTime TimeTaken {
	////			get {
	////				return Convert.ToDateTime(timeTaken);
	////			}
	////			private set {}
	////		}
//		public string TimeRemaining;
	////		public DateTime TimeRemaining {
	////			get {
	////				return Convert.ToDateTime(timeRemaining);
	////			}
	////			private set {}
	////		}
//		public string DepartedSite;
	////		public DateTime DepartedSite {
	////			get {
	////				return Convert.ToDateTime(departedSite);
	////			}
	////			private set {}
	////		}
//
//		public BookIns() {}
//	}
}

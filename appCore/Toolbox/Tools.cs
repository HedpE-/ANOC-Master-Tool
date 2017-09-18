using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
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
		
		static string convertByteArrayToHex(byte[] hash) {
			// step 2, convert byte array to hex string

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++) {
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}
		
        public static Color GetContrastForeground(Control control)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Bitmap orig = new Bitmap(control.ClientRectangle.Width, control.ClientRectangle.Height);
            //using (Graphics g = Graphics.FromImage(orig))
            //{

            //}
            control.DrawToBitmap(orig, control.ClientRectangle);
            //				if(CurrentUser.UserName == "GONCARJ3")
            //					orig.Save(UserFolder.FullName + @"\orig.png");

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // updated: the Interpolation mode needs to be set to
                // HighQualityBilinear or HighQualityBicubic or this method
                // doesn't work at all.  With either setting, the results are
                // slightly different from the averaging method.
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(orig, new Rectangle(0, 0, 1, 1));
            }
            Color pixel = bmp.GetPixel(0, 0);
            // pixel will contain average values for entire orig Bitmap
            return Tools.ContrastColor(Color.FromArgb(255, pixel.R, pixel.G, pixel.B));
        }

		static Color ContrastColor(Color color)
		{
			int d = 0;

			// Counting the perceptive luminance - human eye favors green color...
			double a = 1 - ( 0.299 * color.R + 0.587 * color.G + 0.114 * color.B)/255;

			d = a < 0.5 ? 0 : 255; // dark colors - white font

			return  Color.FromArgb(d, d, d);
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

        public static Control GetControl(string controlName, Type parentType)
        {
            List<Form> foundForms = Application.OpenForms.Cast<Form>().ToList();

            for (int c = 0; c < foundForms.Count; c++)
            {
                if (foundForms[c].GetType() == parentType)
                {
                    return findControl(foundForms[c].Controls, controlName);
                }
            }

            return null;
        }

        private static Control findControl(Control.ControlCollection controls, string controlName)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl.Name == controlName)
                    return ctrl;
                else
                {
                    if (ctrl.Controls != null)
                        return findControl(ctrl.Controls, controlName);
                }
            }

            return null;
        }

        public static List<string> FindAllControls(Control.ControlCollection controls)
        {
            List<string> foundControls = new List<string>();
            foreach (Control ctrl in controls)
            {
                foundControls.Add(ctrl.Name + "," + ctrl.Parent.Name);
                if (ctrl.Controls != null)
                {
                    foundControls.AddRange(FindAllControls(ctrl.Controls));
                    if(ctrl is UI.AMTMenuStrip)
                    {
                        foreach (var tsmi in ((UI.AMTMenuStrip)ctrl).Items.Cast<ToolStripMenuItem>().First().DropDownItems.OfType<ToolStripMenuItem>())
                        {
                            foundControls.Add(tsmi.Name + "," + ctrl.Name);
                        }
                    }

                }
            }

            return foundControls;
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
		/// <param name="tableName">Table Name. Valid values: "div_access", "table_inc", "table_crq", "table_alarms", "table_visits", "table_checkbox_availability"</param>
		public static DataTable ConvertHtmlTableToDT(string html, string tableName) {
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.Load(new StringReader(html));
			DataTable dt = new DataTable();
			
			dt.TableName = tableName;
			
			// Build DataTable Headers ("table_visits" has headers inside <tr> tag, unlike the other tables)
			HtmlNode table = tableName == string.Empty ? doc.DocumentNode.SelectSingleNode("//table") : tableName == "div_access" ? doc.DocumentNode.SelectSingleNode("//div[@id='" + tableName + "']").ChildNodes.FirstOrDefault(n => n.Name == "table") : doc.DocumentNode.SelectSingleNode("//table[@id='" + tableName + "']");
			IEnumerable<HtmlNode> descendantNodes = null;
			switch(tableName) {
				case "table_checkbox_availability":
					descendantNodes = table.SelectSingleNode("/table[1]/thead[1]/tr[1]").ChildNodes; // "/table[1]/thead[1]/tr[1]"
					descendantNodes = descendantNodes.Where(item => item.Name == "td");
					break;
				case "table_ca": case "table_cramer": // ca -> 204 th cramer -> // 6 th
                    descendantNodes = table.Descendants("thead").First().Descendants("th");
					break;
				case "table_checkbox_cells 2G": case "table_checkbox_cells 3G": case "table_checkbox_cells 4G": // 12 th
					descendantNodes = table.Descendants("thead").First().Descendants("tr").First().Descendants("th");
					break;
				default:
					descendantNodes = table.Descendants("th");
					break;
			}

            if(descendantNodes.Count() > 1)
            {
			    foreach (HtmlNode node in descendantNodes) {
                    string nodeText = node.InnerText.Replace("&amp;", "&");
                    if (nodeText.Contains("Date") || nodeText.Contains("Scheduled") || nodeText == "Arrived" || nodeText == "Planned Finish" || nodeText == "Departed Site" || nodeText == "Time")
					    dt.Columns.Add(nodeText, typeof(DateTime));
				    else {
					    if(!dt.Columns.Contains(nodeText))
						    dt.Columns.Add(nodeText);
				    }
			    }
			
			    // Build DataTable
			
			    descendantNodes = tableName.Contains("_cells ") || tableName.Contains("_cramer") ? table.Descendants("tbody").First().Descendants("tr") : table.Descendants("tr");
			    descendantNodes = descendantNodes.Where(dn => dn.Name != "#text" && dn.ParentNode.Name != "thead");
			    foreach(HtmlNode tr in descendantNodes) {
				    List<string> tableRow = new List<string>();
				    if(tr.Name != "#text" && tr.ParentNode.Name != "thead") {
					    var childNodes = tr.ChildNodes.Where(cn => cn.Name != "#text");
					    foreach(var node in childNodes) {
						    if(node.Name != "td")
							    continue;
						
						    if(node.InnerText == "&nbsp;")
							    tableRow.Add(string.Empty);
						    else {
							    if(tableName.Contains("_cells ") && tableRow.Count == dt.Columns["&nbsp;"].Ordinal) {
								    HtmlNode input = node.FirstChild;
								    if(input != null) {
									    var checkd = input.Attributes.Contains("checked") ? "Y" : "";
									    var jvco = input.Attributes["data-jvco"].Value;
									    tableRow.Add(checkd + '/' + jvco);
								    }
								    else
									    tableRow.Add(string.Empty);
							    }
							    else
                                {
                                    if(tableName == "div_access")
                                        tableRow.Add(node.InnerHtml.Replace("<br>", ";"));
                                    else
                                        tableRow.Add(node.InnerText);
                                }
						    }
					    }
				    }
				
				    if(tableRow.Count > 0) {
					    DataRow dataRow = dt.NewRow();
					    for(int i = 0;i < tableRow.Count;i++) {
						    if(i < dataRow.ItemArray.Count()) {
							    if(dt.Columns[i].DataType == typeof(DateTime)) {
								    if(!string.IsNullOrWhiteSpace(tableRow[i]))
									    dataRow[i] = Convert.ToDateTime(tableRow[i]);
							    }
							    else {
								    dataRow[i] = tableRow[i];
							    }
						    }
					    }
					    dt.Rows.Add(dataRow);
				    }
			    }

                //if(dt.TableName.Contains("availability") || dt.TableName == "table_ca") {
                if (dt.TableName == "table_ca")
                {
                    for (int c = 3;c < dt.Columns.Count;c++) {
					    string colName = dt.Columns[3].ColumnName;
					    dt.Columns[colName].SetOrdinal((dt.Columns.Count - 1) - (c - 3));
				    }
			    }
            }
            return dt.Rows.Count > 0 ? dt : null;
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
}

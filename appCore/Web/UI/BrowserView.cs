/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-02-2015
 * Time: 18:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.UI;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
//using System.Threading;
//using mshtml;
//using RestSharp;

namespace appCore.Web.UI
{
	/// <summary>
	/// Description of BrowserView.
	/// </summary>
	public partial class BrowserView : Form
	{
		bool _lockedCellsPanelEnabled = false;
		string OIUsername = string.Empty;
		string OIPassword = string.Empty;
		string SettingsFile = string.Empty;
		List<HtmlElement> cells2GCheckboxes_wb2 = new List<HtmlElement>();
		List<HtmlElement> cells3GCheckboxes_wb2 = new List<HtmlElement>();
		List<HtmlElement> cells4GCheckboxes_wb2 = new List<HtmlElement>();
		List<HtmlElement> cells2GCheckboxes_wb3 = new List<HtmlElement>();
		List<HtmlElement> cells3GCheckboxes_wb3 = new List<HtmlElement>();
		List<HtmlElement> cells4GCheckboxes_wb3 = new List<HtmlElement>();
		List<HtmlElement> lockCellsCheckboxes = new List<HtmlElement>();
		bool[] wb2LockCellsPanel = null;
		bool[] wb3LockCellsPanel = null;
		Uri CitrixHome = new Uri("https://st.internal.vodafone.co.uk/");
		Uri OILogonScreen = new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/sso/index.php?url=%2F");
		Uri OldOILogonScreen = new Uri("http://195.233.194.118/sso/index.php?url=%2F");
		List<UriItem> UrisList = new List<UriItem> {
			new UriItem("SITE Lopedia", new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/site/")), // operationalintelligence.vf-uk.corp.vodafone.com / 195.233.194.118
			new UriItem("SITE Lopedia (Old)", new Uri("http://195.233.194.118/site_old/index.php")),
			new UriItem("Locked Cells", new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/site/cellslocked.php")),
			new UriItem("Locked Cells (Old)", new Uri("http://195.233.194.118/site/cellslocked.php")),
			new UriItem("Sites Off Air", new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/site/offair.php")),
			new UriItem("Vendor Override", new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/RANOps/admin/beaconit.php")),
			new UriItem("COOS - No Unavailability", new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/ranops/coos-tickets.php")),
			new UriItem("Bulk Uploader", new Uri("http://operationalintelligence.vf-uk.corp.vodafone.com/ukoim/bulk_uploader.php")),
			new UriItem("ANOC Site Management Diary (Book Ins)", new Uri("https://smsproxy.vavs.vodafone.com/anoc/"))
		};
		
		public BrowserView()
		{
			InitializeComponent();
			
//			OIUsername = Settings.SettingsFile.OIUsername;
//			OIPassword = Settings.SettingsFile.OIPassword;
//
			tabControl1.SelectTab(1);
			webBrowser1.Navigate(CitrixHome);
//
//			string postData = string.Format("username={0}&password={1}", OIUsername, Toolbox.Tools.EncryptDecryptText("Dec", OIPassword));
//			ASCIIEncoding enc = new ASCIIEncoding();
//			webBrowser2.Navigate(OILogonScreen, "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
//			webBrowser3.Navigate(OILogonScreen, "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
//			Thread.Sleep(1000);
			OIConnection.InitiateOiConnection();
//			webBrowser2.Navigate(homeAddress(webBrowser2));
//			webBrowser3.Navigate(homeAddress(webBrowser3));
			webBrowser2.ResumeSession(homeAddress(webBrowser2), OIConnection.OICookieContainer);
			webBrowser3.ResumeSession(homeAddress(webBrowser3), OIConnection.OICookieContainer);
			
			comboBox1.SelectedIndex = 0;
			comboBox1.SelectedIndexChanged += ComboBox1SelectedIndexChanged;
//			HttpStatusCode statusCode = OIConnection.EstablishConnection();
//			if(statusCode == HttpStatusCode.OK) {
//				if(!string.IsNullOrEmpty(OIConnection.Connection.LoggedOnUsername) && !OIConnection.Connection.OICookieContainer.ToList()[0].Expired) {
//					webBrowser2.Navigate(UrisList[0].URI);
//					webBrowser3.Navigate(homeAddress(webBrowser3));
			////					webBrowser2.ResumeSession(UrisList[0].URI, OIConnection.Connection.OICookieContainer);
			////					webBrowser3.ResumeSession(homeAddress(webBrowser3), OIConnection.Connection.OICookieContainer);
//				}
//				else {
//					statusCode = OIConnection.Connection.Logon();
//
//					// TODO: Handle errors
//					if(statusCode == HttpStatusCode.OK) {
//						webBrowser2.Navigate(UrisList[0].URI);
//						webBrowser3.Navigate(homeAddress(webBrowser3));
			////						webBrowser2.ResumeSession(UrisList[0].URI, OIConnection.Connection.OICookieContainer);
			////						webBrowser3.ResumeSession(homeAddress(webBrowser3), OIConnection.Connection.OICookieContainer);
//					}
//				}
//			}
		}
		
		void WebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AMTBrowser wb = (AMTBrowser)sender;
			if(wb.Name == "webBrowser2" || wb.Name == "webBrowser3") {
				if(wb.Name == "webBrowser2") {
					wb2LockCellsPanel = null;
					cells2GCheckboxes_wb2.Clear();
					cells3GCheckboxes_wb2.Clear();
					cells4GCheckboxes_wb2.Clear();
				}
				else {
					wb3LockCellsPanel = null;
					cells2GCheckboxes_wb3.Clear();
					cells3GCheckboxes_wb3.Clear();
					cells4GCheckboxes_wb3.Clear();
					lockCellsCheckboxes.Clear();
				}
				
				groupBox1.Visible &= !lockedCellsPanelEnabled;
				if(wb.Url.ToString().Contains("operationalintelligence.vf-uk.corp.vodafone.com") || wb.Url.ToString().Contains("195.233.194.118")) {
//					if(!checkOILogin(wb)) {
					////						OIConnection.EstablishConnection();
					////						OIConnection.Connection.Logon();
					////						wb.ResumeSession(homeAddress(wb), OIConnection.OICookieContainer);
//						string postData = string.Format("username={0}&password={1}", OIUsername, Toolbox.Tools.EncryptDecryptText("Dec", OIPassword));
//						ASCIIEncoding enc = new ASCIIEncoding();
//						if(wb.Url.ToString().Contains("operationalintelligence.vf-uk.corp.vodafone.com"))
//							wb.Navigate(OILogonScreen, "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
//						else
//							wb.Navigate(OldOILogonScreen, "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
//						Thread.Sleep(1000);
//						wb.Navigate(homeAddress(wb));
//					}
					if(wb.Url.ToString().Contains(@"://195.233.194.118/site_old") || wb.Url.ToString().Contains(@"://operationalintelligence.vf-uk.corp.vodafone.com/site/")) {
						HtmlElement divCells = wb.Document.GetElementById("div_cells");
						if(divCells != null) {
							groupBox1.Visible |= lockedCellsPanelEnabled;
							checkBox1.Enabled = false;
							checkBox2.Enabled = false;
							checkBox3.Enabled = false;
							checkBox4.Enabled = false;
							
							if(divCells.InnerHtml.Contains("<TD>2G</TD>")) {
								string[] strTofind = { "<TD>2G</TD>" };
								string[] temp = divCells.InnerHtml.Substring(divCells.InnerHtml.IndexOf("<TD>2G</TD>", StringComparison.Ordinal)).Split(strTofind, StringSplitOptions.None);
								temp =  temp.Where(x => !string.IsNullOrEmpty(x)).ToArray();
								foreach(string str in temp) {
									string tempCell = string.Empty;
									foreach (char chr in str.Substring(str.IndexOf("INPUT id=checkbox", StringComparison.Ordinal) + 9)) {
										if(chr != ' ')
											tempCell += chr;
										else
											break;
									}
									HtmlElement tempCellElement = (HtmlElement)wb.Document.GetElementById(tempCell);
									if(tempCellElement.GetAttribute("disabled") == "False") {
										if(wb.Name == "webBrowser2")
											cells2GCheckboxes_wb2.Add(tempCellElement);
										else
											cells2GCheckboxes_wb3.Add(tempCellElement);
									}
								}
							}
							if(divCells.InnerHtml.Contains("<TD>3G</TD>")) {
								string[] strTofind = { "<TD>3G</TD>" };
								string[] temp = divCells.InnerHtml.Substring(divCells.InnerHtml.IndexOf("<TD>3G</TD>", StringComparison.Ordinal)).Split(strTofind, StringSplitOptions.None);
								temp =  temp.Where(x => !string.IsNullOrEmpty(x)).ToArray();
								foreach(string str in temp) {
									string tempCell = string.Empty;
									foreach (char chr in str.Substring(str.IndexOf("INPUT id=checkbox", StringComparison.Ordinal) + 9)) {
										if(chr != ' ')
											tempCell += chr;
										else
											break;
									}
									HtmlElement tempCellElement = (HtmlElement)wb.Document.GetElementById(tempCell);
									if(tempCellElement.GetAttribute("disabled") == "False") {
										if(wb.Name == "webBrowser2")
											cells3GCheckboxes_wb2.Add(tempCellElement);
										else
											cells3GCheckboxes_wb3.Add(tempCellElement);
									}
								}
							}
							if(divCells.InnerHtml.Contains("<TD>4G</TD>")) {
								string[] strTofind = { "<TD>4G</TD>" };
								string[] temp = divCells.InnerHtml.Substring(divCells.InnerHtml.IndexOf("<TD>4G</TD>", StringComparison.Ordinal)).Split(strTofind, StringSplitOptions.None);
								temp =  temp.Where(x => !string.IsNullOrEmpty(x)).ToArray();
								foreach(string str in temp) {
									string tempCell = string.Empty;
									foreach (char chr in str.Substring(str.IndexOf("INPUT id=checkbox", StringComparison.Ordinal) + 9)) {
										if(chr != ' ')
											tempCell += chr;
										else
											break;
									}
									HtmlElement tempCellElement = (HtmlElement)wb.Document.GetElementById(tempCell);
									if(tempCellElement.GetAttribute("disabled") == "False") {
										if(wb.Name == "webBrowser2")
											cells4GCheckboxes_wb2.Add(tempCellElement);
										else
											cells4GCheckboxes_wb3.Add(tempCellElement);
									}
								}
							}
							
							if(wb.Name == "webBrowser2") {
								checkBox1.Enabled |= cells2GCheckboxes_wb2.Any();
								checkBox2.Enabled |= cells3GCheckboxes_wb2.Any();
								checkBox3.Enabled |= cells4GCheckboxes_wb2.Any();
							}
							else {
								checkBox1.Enabled |= cells2GCheckboxes_wb3.Any();
								checkBox2.Enabled |= cells3GCheckboxes_wb3.Any();
								checkBox3.Enabled |= cells4GCheckboxes_wb3.Any();
							}
							
							int count = 0;
							foreach (CheckBox cb in groupBox1.Controls) {
								if(cb.Enabled)
									count++;
							}
							checkBox4.Enabled |= count > 1;
							foreach (CheckBox cb in groupBox1.Controls) {
								cb.Checked = false;
							}
							if(wb.Name == "webBrowser2")
								wb2LockCellsPanel = new[]{ checkBox1.Enabled, checkBox2.Enabled, checkBox3.Enabled, checkBox4.Enabled };
							else
								wb3LockCellsPanel = new[]{ checkBox1.Enabled, checkBox2.Enabled, checkBox3.Enabled, checkBox4.Enabled };
						}
						
					}
				}
			}
		}
		
//		bool checkOILogin(AMTBrowser wb) {
//			var elements = wb.Document.GetElementsByTagName("form");
//			bool loginform = false;
//			HtmlElement loginfrm = null;
//			foreach (HtmlElement elem in elements) {
//				if(elem.GetAttribute("name") == "loginform") {
//					loginform = true;
//					loginfrm = elem;
//					break;
//				}
//			}
//			return !loginform;
//		}
		
		string homeAddress(AMTBrowser wb)
		{
			switch(wb.Name) {
				case "webBrowser1":
					return CitrixHome.AbsoluteUri;
				case "webBrowser2":
					return UrisList[0].URI.AbsoluteUri;
				case "webBrowser3":
					if(comboBox1.SelectedIndex == -1)
						return UrisList[0].URI.AbsoluteUri;
					
					return UrisList[comboBox1.SelectedIndex].URI.AbsoluteUri;
			}
			return string.Empty;
		}
		
		void PictureBoxesMouseHover(object sender, EventArgs e)
		{
			PictureBox pb = (PictureBox)sender;
			pb.BorderStyle = BorderStyle.Fixed3D;
		}
		
		void PictureBoxesMouseLeave(object sender, EventArgs e)
		{
			PictureBox pb = (PictureBox)sender;
			pb.BorderStyle = BorderStyle.None;
		}
		
		void PictureBox1Click(object sender, EventArgs e)
		{
			switch (tabControl1.SelectedIndex)
			{
				case 0:
					webBrowser1.GoBack();
					break;
				case 1:
					webBrowser2.GoBack();
					break;
				case 2:
					webBrowser3.GoBack();
					break;
			}
		}
		void PictureBox2Click(object sender, EventArgs e)
		{
			switch (tabControl1.SelectedIndex)
			{
				case 0:
					webBrowser1.GoForward();
					break;
				case 1:
					webBrowser2.GoForward();
					break;
				case 2:
					webBrowser3.GoForward();
					break;
			}
		}
		void PictureBox3Click(object sender, EventArgs e)
		{
			switch (tabControl1.SelectedIndex)
			{
				case 0:
					webBrowser1.Refresh();
					break;
				case 1:
					webBrowser2.Refresh();
					break;
				case 2:
					webBrowser3.Refresh();
					break;
			}
		}
		void PictureBox4Click(object sender, EventArgs e)
		{
			switch (tabControl1.SelectedIndex)
			{
				case 0:
					webBrowser1.Navigate(CitrixHome);
					break;
				case 1:
					OIConnection.InitiateOiConnection();
					webBrowser2.ResumeSession(homeAddress(webBrowser2), OIConnection.OICookieContainer);
					break;
				case 2:
					OIConnection.InitiateOiConnection(comboBox1.Text.Contains("(Old)"));
					CookieContainer cookies = comboBox1.Text.Contains("(Old)") ? OIConnection.OldOICookieContainer : OIConnection.OICookieContainer;
					webBrowser3.ResumeSession(UrisList[comboBox1.SelectedIndex].URI, cookies);
					break;
			}
		}
		
		void checkBoxesCheckedState(AMTBrowser wb) {
			List<HtmlElement> cells2GCheckboxes = new List<HtmlElement>();
			List<HtmlElement> cells3GCheckboxes = new List<HtmlElement>();
			List<HtmlElement> cells4GCheckboxes = new List<HtmlElement>();
			switch(wb.Name) {
				case "webBrowser2":
					cells2GCheckboxes = cells2GCheckboxes_wb2;
					cells3GCheckboxes = cells3GCheckboxes_wb2;
					cells4GCheckboxes = cells4GCheckboxes_wb2;
					break;
				case "webBrowser3":
					cells2GCheckboxes = cells2GCheckboxes_wb3;
					cells3GCheckboxes = cells3GCheckboxes_wb3;
					cells4GCheckboxes = cells4GCheckboxes_wb3;
					break;
			}
			
			for(int cb = 1;cb < 5;cb++) {
				switch(cb) {
					case 1:
						foreach(HtmlElement cbx in cells2GCheckboxes) {
							if(cbx.GetAttribute("checked") == "True")
								cbx.InvokeMember("CLICK");
						}
						break;
					case 2:
						foreach(HtmlElement cbx in cells3GCheckboxes) {
							if(cbx.GetAttribute("checked") == "True")
								cbx.InvokeMember("CLICK");
						}
						break;
					case 3:
						foreach(HtmlElement cbx in cells4GCheckboxes) {
							if(cbx.GetAttribute("checked") == "True")
								cbx.InvokeMember("CLICK");
						}
						break;
				}
			}
			
			if(wb.Name == "webBrowser3" && wb.Url.ToString().Contains(@"/site/cellslocked.php")) {
				foreach(HtmlElement cbx in lockCellsCheckboxes) {
					if(cbx.GetAttribute("checked") == "True")
						cbx.InvokeMember("CLICK");
				}
			}
			
			foreach (CheckBox cb in groupBox1.Controls) {
				cb.Checked = false;
			}
		}
		
		void TabControl1SelectedIndexChanged(object sender, EventArgs e)
		{
			comboBox1.Visible = tabControl1.SelectedIndex == 2;
			
			this.Text = "AMT Browser - " + tabControl1.SelectedTab.Text;
			checkBox1.CheckedChanged -= (CheckBox1CheckedChanged);
			checkBox2.CheckedChanged -= (CheckBox2CheckedChanged);
			checkBox3.CheckedChanged -= (CheckBox3CheckedChanged);
			checkBox4.CheckedChanged -= (CheckBox4CheckedChanged);
			if(tabControl1.SelectedIndex == 1) {
				if (wb2LockCellsPanel != null) {
					groupBox1.Visible |= lockedCellsPanelEnabled;
					for (int cb = 1; cb < 5; cb++) {
						groupBox1.Controls["checkBox" + cb].Enabled = wb2LockCellsPanel[cb - 1];
					}
					checkBoxesCheckedState(webBrowser2);
				}
				else
					groupBox1.Visible &= !lockedCellsPanelEnabled;
			}
			else {
				if(tabControl1.SelectedIndex == 2) {
					if (wb3LockCellsPanel != null) {
						groupBox1.Visible |= lockedCellsPanelEnabled;
						if (wb3LockCellsPanel.Length > 1) {
							for (int cb = 1; cb < 5; cb++) {
								groupBox1.Controls["checkBox" + cb].Enabled = wb3LockCellsPanel[cb - 1];
							}
							checkBoxesCheckedState(webBrowser3);
						} else {
							for (int cb = 1; cb < 5; cb++) {
								if (cb != 4)
									groupBox1.Controls["checkBox" + cb].Enabled = false;
								else
									checkBox4.Enabled = wb3LockCellsPanel[0];
							}
							checkBoxesCheckedState(webBrowser3);
						}
					} else
						groupBox1.Visible &= !lockedCellsPanelEnabled;
				}
			}
			checkBox1.CheckedChanged += (CheckBox1CheckedChanged);
			checkBox2.CheckedChanged += (CheckBox2CheckedChanged);
			checkBox3.CheckedChanged += (CheckBox3CheckedChanged);
			checkBox4.CheckedChanged += (CheckBox4CheckedChanged);
			//WebBrowserHelper.ClearCache();
		}
		
		void WebBrowsersNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			// FIXME: Textbox not updating address correctly sometimes
			//WebBrowserHelper.ClearCache();
			AMTBrowser wb = (AMTBrowser)sender;
//			if(e.Url.AbsoluteUri.Contains("/sso/index.php")) {
//				int status = (int)OIConnection.Connection.Logon();
//				e.Cancel = true;
//				if(wb.Name == "webBrowser2")
//					wb.ResumeSession(UrisList[0].URI, OIConnection.Connection.OICookieContainer);
//				else
//					wb.ResumeSession(UrisList[comboBox1.SelectedIndex].URI, OIConnection.Connection.OICookieContainer);
//				return;
//			}
			TabPage tp = (TabPage)tabControl1.Controls[Convert.ToInt16(wb.Name.Substring(wb.Name.IndexOf('s') + 3)) - 1];  // webBrowserx, index of 's' + 3 to get number
			tp.Controls["textBox" + wb.Name.Substring(wb.Name.IndexOf('s') + 3)].Text = e.Url.AbsoluteUri;
		}
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			OIConnection.InitiateOiConnection(comboBox1.Text.Contains("(Old)"));
			CookieContainer cookies = comboBox1.Text.Contains("(Old)") ? OIConnection.OldOICookieContainer : OIConnection.OICookieContainer;
			webBrowser3.ResumeSession(UrisList[comboBox1.SelectedIndex].URI, cookies);
			tabPage3.Text = comboBox1.Text;
			this.Text = "AMT Browser - " + comboBox1.Text;
		}
		
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			List<HtmlElement> tempList = new List<HtmlElement>();
			tempList = tabControl1.SelectedIndex == 1 ? cells2GCheckboxes_wb2 : cells2GCheckboxes_wb3;
			foreach(HtmlElement element in tempList) {
				element.InvokeMember("CLICK");
			}
		}
		
		void CheckBox2CheckedChanged(object sender, EventArgs e)
		{
			List<HtmlElement> tempList = new List<HtmlElement>();
			tempList = tabControl1.SelectedIndex == 1 ? cells3GCheckboxes_wb2 : cells3GCheckboxes_wb3;
			foreach(HtmlElement element in tempList) {
				element.InvokeMember("CLICK");
			}
		}
		
		void CheckBox3CheckedChanged(object sender, EventArgs e)
		{
			List<HtmlElement> tempList = new List<HtmlElement>();
			tempList = tabControl1.SelectedIndex == 1 ? cells4GCheckboxes_wb2 : cells4GCheckboxes_wb3;
			foreach(HtmlElement element in tempList) {
				element.InvokeMember("CLICK");
			}
		}
		
		void CheckBox4CheckedChanged(object sender, EventArgs e)
		{
			if(tabControl1.SelectedIndex == 1 || (tabControl1.SelectedIndex == 2 && webBrowser3.Url.ToString().Contains(@"/site/index.php"))) {
				CheckBox1CheckedChanged(null,null);
				CheckBox2CheckedChanged(null,null);
				CheckBox3CheckedChanged(null,null);
			}
			else {
				if(tabControl1.SelectedIndex == 2 && webBrowser3.Url.ToString().Contains(@"/site/cellslocked.php")) {
					foreach(HtmlElement element in lockCellsCheckboxes) {
						element.InvokeMember("CLICK");
					}
				}
			}
		}

		/// <summary>
		/// lockedCellsPanel property to toggle the panel's visibility
		/// </summary>
		public bool lockedCellsPanelEnabled {
			get {
				return _lockedCellsPanelEnabled;
			}
			set {
				_lockedCellsPanelEnabled = value;
				if(value) {
					switch(tabControl1.SelectedIndex) {
						case 1:
							groupBox1.Visible |= wb2LockCellsPanel != null;
							break;
						case 2:
							groupBox1.Visible |= wb3LockCellsPanel != null;
							break;
					}
				}
				else {
					switch(tabControl1.SelectedIndex) {
						case 1:
							groupBox1.Visible = false;
							break;
						case 2:
							groupBox1.Visible = false;
							break;
					}
				}
			}
		}
		
		void PictureBox5Click(object sender, EventArgs e)
		{
			MouseEventArgs me = (MouseEventArgs) e;
			if(me.Button == MouseButtons.Right) {
				if(Control.ModifierKeys.ToString().Contains("Shift") && Control.ModifierKeys.ToString().Contains("Control")) {
					lockedCellsPanelEnabled = !lockedCellsPanelEnabled;
					return;
				}
			}
			
			lockedCellsPanelEnabled = false;
		}
	}

	/**
	 * Modified from code originally found here: http://support.microsoft.com/kb/326201
	 **/

	public static class WebBrowserHelper
	{
		#region Definitions/DLL Imports
		/// <summary>
		/// For PInvoke: Contains information about an entry in the Internet cache
		/// </summary>
		[StructLayout(LayoutKind.Explicit, Size = 80)]
		public struct INTERNET_CACHE_ENTRY_INFOA
		{
			[FieldOffset(0)]
			public uint dwStructSize;
			[FieldOffset(4)]
			public IntPtr lpszSourceUrlName;
			[FieldOffset(8)]
			public IntPtr lpszLocalFileName;
			[FieldOffset(12)]
			public uint CacheEntryType;
			[FieldOffset(16)]
			public uint dwUseCount;
			[FieldOffset(20)]
			public uint dwHitRate;
			[FieldOffset(24)]
			public uint dwSizeLow;
			[FieldOffset(28)]
			public uint dwSizeHigh;
			[FieldOffset(32)]
			public System.Runtime.InteropServices.ComTypes.FILETIME LastModifiedTime;
			[FieldOffset(40)]
			public System.Runtime.InteropServices.ComTypes.FILETIME ExpireTime;
			[FieldOffset(48)]
			public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
			[FieldOffset(56)]
			public System.Runtime.InteropServices.ComTypes.FILETIME LastSyncTime;
			[FieldOffset(64)]
			public IntPtr lpHeaderInfo;
			[FieldOffset(68)]
			public uint dwHeaderInfoSize;
			[FieldOffset(72)]
			public IntPtr lpszFileExtension;
			[FieldOffset(76)]
			public uint dwReserved;
			[FieldOffset(76)]
			public uint dwExemptDelta;
		}
		
		// For PInvoke: Initiates the enumeration of the cache groups in the Internet cache
		[DllImport(@"wininet",
		           SetLastError = true,
		           CharSet = CharSet.Auto,
		           EntryPoint = "FindFirstUrlCacheGroup",
		           CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr FindFirstUrlCacheGroup(
			int dwFlags,
			int dwFilter,
			IntPtr lpSearchCondition,
			int dwSearchCondition,
			ref long lpGroupId,
			IntPtr lpReserved);
		
		// For PInvoke: Retrieves the next cache group in a cache group enumeration
		[DllImport(@"wininet",
		           SetLastError = true,
		           CharSet = CharSet.Auto,
		           EntryPoint = "FindNextUrlCacheGroup",
		           CallingConvention = CallingConvention.StdCall)]
		public static extern bool FindNextUrlCacheGroup(
			IntPtr hFind,
			ref long lpGroupId,
			IntPtr lpReserved);
		
		// For PInvoke: Releases the specified GROUPID and any associated state in the cache index file
		[DllImport(@"wininet",
		           SetLastError = true,
		           CharSet = CharSet.Auto,
		           EntryPoint = "DeleteUrlCacheGroup",
		           CallingConvention = CallingConvention.StdCall)]
		public static extern bool DeleteUrlCacheGroup(
			long GroupId,
			int dwFlags,
			IntPtr lpReserved);
		
		// For PInvoke: Begins the enumeration of the Internet cache
		[DllImport(@"wininet",
		           SetLastError = true,
		           CharSet = CharSet.Auto,
		           EntryPoint = "FindFirstUrlCacheEntryA",
		           CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr FindFirstUrlCacheEntry(
			[MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern,
			IntPtr lpFirstCacheEntryInfo,
			ref int lpdwFirstCacheEntryInfoBufferSize);
		
		// For PInvoke: Retrieves the next entry in the Internet cache
		[DllImport(@"wininet",
		           SetLastError = true,
		           CharSet = CharSet.Auto,
		           EntryPoint = "FindNextUrlCacheEntryA",
		           CallingConvention = CallingConvention.StdCall)]
		public static extern bool FindNextUrlCacheEntry(
			IntPtr hFind,
			IntPtr lpNextCacheEntryInfo,
			ref int lpdwNextCacheEntryInfoBufferSize);
		
		// For PInvoke: Removes the file that is associated with the source name from the cache, if the file exists
		[DllImport(@"wininet",
		           SetLastError = true,
		           CharSet = CharSet.Auto,
		           EntryPoint = "DeleteUrlCacheEntryA",
		           CallingConvention = CallingConvention.StdCall)]
		public static extern bool DeleteUrlCacheEntry(
			IntPtr lpszUrlName);
		#endregion
		
		#region Public Static Functions
		
		/// <summary>
		/// Clears the cache of the web browser
		/// </summary>
		public static void ClearCache()
		{
			// Indicates that all of the cache groups in the user's system should be enumerated
			const int CACHEGROUP_SEARCH_ALL = 0x0;
			// Indicates that all the cache entries that are associated with the cache group
			// should be deleted, unless the entry belongs to another cache group.
			const int CACHEGROUP_FLAG_FLUSHURL_ONDELETE = 0x2;
			// File not found.
			const int ERROR_FILE_NOT_FOUND = 0x2;
			// No more items have been found.
			const int ERROR_NO_MORE_ITEMS = 259;
			// Pointer to a GROUPID variable
			long groupId = 0;
			
			// Local variables
			int cacheEntryInfoBufferSizeInitial = 0;
			int cacheEntryInfoBufferSize = 0;
			IntPtr cacheEntryInfoBuffer = IntPtr.Zero;
			INTERNET_CACHE_ENTRY_INFOA internetCacheEntry;
			IntPtr enumHandle = IntPtr.Zero;
			bool returnValue = false;
			
			// Delete the groups first.
			// Groups may not always exist on the system.
			// For more information, visit the following Microsoft Web site:
			// http://msdn.microsoft.com/library/?url=/workshop/networking/wininet/overview/cache.asp
			// By default, a URL does not belong to any group. Therefore, that cache may become
			// empty even when the CacheGroup APIs are not used because the existing URL does not belong to any group.
			enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
			// If there are no items in the Cache, you are finished.
			if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
				return;
			
			// Loop through Cache Group, and then delete entries.
			while (true)
			{
				if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error()) { break; }
				// Delete a particular Cache Group.
				returnValue = DeleteUrlCacheGroup(groupId, CACHEGROUP_FLAG_FLUSHURL_ONDELETE, IntPtr.Zero);
				if (!returnValue && ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error())
				{
					returnValue = FindNextUrlCacheGroup(enumHandle, ref groupId, IntPtr.Zero);
				}
				
				if (!returnValue && (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error()))
					break;
			}
			
			// Start to delete URLs that do not belong to any group.
			enumHandle = FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);
			if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
				return;
			
			cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
			cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
			enumHandle = FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
			
			while (true)
			{
				internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof(INTERNET_CACHE_ENTRY_INFOA));
				if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error()) { break; }
				
				cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;
				returnValue = DeleteUrlCacheEntry(internetCacheEntry.lpszLocalFileName);
				if (!returnValue)
				{
					returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
				}
				if (!returnValue && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
				{
					break;
				}
				if (!returnValue && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
				{
					cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
					cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, (IntPtr)cacheEntryInfoBufferSize);
					returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
				}
			}
			Marshal.FreeHGlobal(cacheEntryInfoBuffer);
		}
		#endregion
		
		const string url = "http://195.233.194.118/SSO/?action=login";
		static string username = string.Empty; //Tools.SettingsFileHandler("OIUsername","read",null,BrowserView.SettingsFile);
		static string password = string.Empty; //Tools.EncryptDecryptText("Dec",Tools.SettingsFileHandler("OIPassword","read",null,BrowserView.SettingsFile));
		const string commit = "Sign+In"; //this matches the data from Tamper Data

		public static void Login()
		{
			StringBuilder postData = new StringBuilder();
			postData.Append("/SSO/index.asp?url=http%3A%2F%2F195%2E233%2E194%2E118%2F username=" + username + "&password=" + password);

			ASCIIEncoding ascii = new ASCIIEncoding();
			byte[] postBytes = ascii.GetBytes(postData.ToString());
			
			WebRequest request = WebRequest.Create(url);
			request.Credentials = CredentialCache.DefaultCredentials;
			request.Method = "POST";
			request.ContentLength = postBytes.Length;
			Stream dataStream = request.GetRequestStream ();
			dataStream.Write(postBytes, 0, postBytes.Length);
			WebResponse response = request.GetResponse();
			
			string a = (((HttpWebResponse)response).StatusDescription);
			
			//WebBrowser b = new WebBrowser();
			//b.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted);
			//b.Navigate(url);
		}

		static void b_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AMTBrowser b = sender as AMTBrowser;
			//try
			//{
			//	HTMLDocument pass = new HTMLDocument();
			//	pass = (HTMLDocument)Web_V1.Document;
			//	HTMLInputElement passBox = (HTMLInputElement)pass.all.item("password", 0);
			//	passBox.value = password;
			//	HTMLDocument log = new HTMLDocument();
			//	log = (HTMLDocument)Web_V1.Document;
			//	HTMLInputElement logBox = (HTMLInputElement)log.all.item("username", 0);
			//	logBox.value = username;
			//	HTMLInputElement submit = (HTMLInputElement)pass.all.item("SubmitButtonIDFromPageSource", 0);
			//	submit.click();
			//}
			//catch { }
			
			string response = b.DocumentText;

			// looks in the page source to find the authenticity token.
			// could also use regular exp<b></b>ressions here.

			int index = response.IndexOf("authenticity_token", StringComparison.Ordinal);
			int startIndex = index + 41;
			string authenticityToken = response.Substring(startIndex, 40);

			// unregisters the first event handler
			// adds a second event handler

			b.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted2);
			b.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted);
			b.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted2);

			// format our data that we are going to post to the server
			// this will include our post parameters.  They do not need to be in a specific
			// order, as long as they are concatenated together using an ampersand ( & )

			string postData = string.Format("authenticity_token={2}&session[username_or_email]={0}&session[password]={1}&commit={3}", username, password, authenticityToken, commit);

			ASCIIEncoding enc = new ASCIIEncoding();

			//  we are encoding the postData to a byte array

			b.Navigate("https://twitter.com/sessions", "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
		}

		static void b_DocumentCompleted2(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AMTBrowser b = sender as AMTBrowser;
			string response = b.DocumentText;

			if (response.Contains("Sign out"))
			{
				MessageBox.Show("Login Successful");
			}
		}
	}
}



/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-08-2016
 * Time: 14:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using appCore.DB;
using appCore.Settings;
//using appCore.Toolbox;

namespace appCore.UI
{
	/// <summary>
	/// Description of trayIcon.
	/// </summary>
	public class TrayIcon
	{
		NotifyIcon _notifyIcon;
		public NotifyIcon InternalNotifyIcon {
			get {
				return _notifyIcon;
			}
			private set {
				_notifyIcon = value;
				_notifyIcon.ContextMenu = value.ContextMenu;
				_notifyIcon.ContextMenuStrip = value.ContextMenuStrip;
				Populate();
				if(UserFolder.FullName != null)
					toggleShareAccess();
			}
		}
		
		[DefaultValue(10000)]
		public int BalloonToolTipTimeoutMs { get; set; }
		
		public ContextMenu ContextMenu {
			get {
				return InternalNotifyIcon.ContextMenu;
			}
			set {
				InternalNotifyIcon.ContextMenu = value;
			}
		}
		
		public ContextMenuStrip ContextMenuStrip {
			get {
				return InternalNotifyIcon.ContextMenuStrip;
			}
			set {
				InternalNotifyIcon.ContextMenuStrip = value;
			}
		}
		
		// Documents on constants for easier filename change
		FileInfo ProcessesDoc;
		FileInfo TeamContactsDoc;
		FileInfo VFcontactsDoc;
		MenuItem Documents;
		MenuItem Links;

		public TrayIcon(NotifyIcon tray) {
			InternalNotifyIcon = tray;
			InternalNotifyIcon.Icon = Resources.MB_0001_vodafone3;
			InternalNotifyIcon.Text = "ANOC Master Tool";
			InternalNotifyIcon.Visible = true;
			
			InternalNotifyIcon.MouseUp += thisNotifyIcon_MouseUp;
//			trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);
		}
		
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetForegroundWindow();
		
		void thisNotifyIcon_MouseUp(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				FieldInfo notifyIconNativeWindowInfo = typeof(NotifyIcon).GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
				NativeWindow notifyIconNativeWindow = (NativeWindow)notifyIconNativeWindowInfo.GetValue(InternalNotifyIcon);

				bool visible = notifyIconNativeWindow.Handle == GetForegroundWindow();
				if(!visible) {
					MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
					mi.Invoke(InternalNotifyIcon, null);
				}
//				else {
//					InternalNotifyIcon.ContextMenu.
//				}
				
//				Control ctr = new Control();
//				ctr.CreateControl();
//				InternalNotifyIcon.ContextMenu.Show(ctr,Cursor.Position);
				
//				InternalNotifyIcon.ContextMenuStrip.Show(e.Location);
			}
		}
		
		public void showBalloon(string title, string body) {
			if (title != null)
				InternalNotifyIcon.BalloonTipTitle = title;
			if (body != null)
				InternalNotifyIcon.BalloonTipText = body;

//			MainForm.trayIc.ShowBalloonTip(BalloonToolTipTimeoutMs);
			InternalNotifyIcon.ShowBalloonTip(BalloonToolTipTimeoutMs);
		}
		
		public void Populate() {
			// Add menu items to shortcut menu.
			Documents = new MenuItem("ANOC Documents");
			Documents.MenuItems.Add("Shifts", (s, e) => Process.Start(GlobalProperties.OfficePath + "\\excel.exe", '"' + Databases.shiftsFile2.FullName + '"'));
			Documents.MenuItems.Add("ANOC Contacts", (s, e) => Process.Start(GlobalProperties.OfficePath + "\\excel.exe", '"' + TeamContactsDoc.FullName + '"'));
			Documents.MenuItems.Add("-");
			Documents.MenuItems.Add("Useful Contacts", (s, e) => Process.Start(GlobalProperties.OfficePath + "\\excel.exe", '"' + VFcontactsDoc.FullName + '"'));
			Documents.MenuItems.Add("Processes", (s, e) => Process.Start(GlobalProperties.OfficePath + "\\winword.exe", '"' + ProcessesDoc.FullName + '"'));
			
			Links = new MenuItem("Links");
			Links.MenuItems.Add("ST Internal Citrix", (s, e) => Process.Start("https://st.internal.vodafone.co.uk/"));
			Links.MenuItems.Add("Vodafone Application Portal", (s, e) => Process.Start("https://dealer.vodafone.co.uk/"));
			Links.MenuItems.Add("ANOC-UK Network Share", (s, e) => Process.Start("explorer.exe", '"' + "\\\\vf-pt\\fs\\ANOC-UK" + '"'));
			Links.MenuItems.Add("Energy Networks", (s, e) => Process.Start("http://www.energynetworks.org/info/faqs/electricity-distribution-map.html"));
			Links.MenuItems.Add("BT Wholesale", (s, e) => Process.Start("https://www.btwholesale.com/portalzone/portalzone/homeLogin.do"));
			Links.MenuItems.Add("ALEX", (s, e) => Process.Start("http://oprweb/alex"));
			Links.MenuItems.Add("Google Translate", (s, e) => Process.Start("http://translate.google.com/"));
			
			InternalNotifyIcon.ContextMenu = new ContextMenu(new MenuItem[] {
//			MainForm.trayIc.ContextMenu = new ContextMenu(new MenuItem[] {
			                                                 	Documents,
			                                                 	Links,
			                                                 	new MenuItem("-"),
			                                                 	
			                                                 	new MenuItem("Settings", (s, e) => MainForm.openSettings()),
			                                                 	new MenuItem("AMT Browser", (s, e) => MainForm.openAMTBrowser()),
			                                                 	new MenuItem("Notes", (s, e) => MainForm.openNotes()),
			                                                 	new MenuItem("Log Browser", (s, e) => MainForm.openLogBrowser()),
			                                                 	new MenuItem("Site Finder", (s, e) => MainForm.openSiteFinder()),
//
			                                                 	new MenuItem("-"),
			                                                 	new MenuItem("Check for Updates..."),
			                                                 	new MenuItem("Exit AMT", (s, e) => Application.Exit()),
			                                                 });
			
		}
		
		public void toggleShareAccess() {
			if(GlobalProperties.shareAccess) {
				DirectoryInfo SearchInFolder = new DirectoryInfo(GlobalProperties.ShareRootDir.Parent.FullName + @"\Processes");
				FileInfo[] FoundFiles = SearchInFolder.GetFiles("ANOC UK 1st Line Processes*.docx");
				
				if(FoundFiles.Length > 1) {
					DateTime newestDate = Convert.ToDateTime("01/01/1900 00:00:00");
					foreach (FileInfo file in FoundFiles) {
						if(file.CreationTime > newestDate && !file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$")) {
							newestDate = file.CreationTime;
							ProcessesDoc = file;
						}
					}
				}
				else
					if(FoundFiles.Length == 1)
						ProcessesDoc = FoundFiles[0];

				SearchInFolder = new DirectoryInfo(GlobalProperties.ShareRootDir.Parent.Parent.FullName + @"\Contactos");
				FoundFiles = SearchInFolder.GetFiles("ANOC Team contact numbers.xlsx");
				
				if(FoundFiles.Length > 1) {
					DateTime newestDate = Convert.ToDateTime("01/01/1900 00:00:00");
					foreach (FileInfo file in FoundFiles) {
						if(file.CreationTime > newestDate && !file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$")) {
							newestDate = file.CreationTime;
							TeamContactsDoc = file;
						}
					}
				}
				else
					if(FoundFiles.Length == 1)
						TeamContactsDoc = FoundFiles[0];
				
				FoundFiles = SearchInFolder.GetFiles("*USEFUL CONTACTS*.xlsx");
				
				if(FoundFiles.Length > 1) {
					DateTime newestDate = Convert.ToDateTime("01/01/1900 00:00:00");
					foreach (FileInfo file in FoundFiles) {
						if(file.CreationTime > newestDate && !file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$")) {
							newestDate = file.CreationTime;
							VFcontactsDoc = file;
						}
					}
				}
				else
					if(FoundFiles.Length == 1)
						VFcontactsDoc = FoundFiles[0];
				
				if(CurrentUser.userName == "GONCARJ3") {
					string errmsg = string.Empty;
					if(!ProcessesDoc.Exists)
						errmsg += "Processes Document";
					if(!Databases.shiftsFile.Exists) {
						if(!string.IsNullOrEmpty(errmsg))
							errmsg += Environment.NewLine;
						errmsg += "Shifts Document";
					}
					if(!TeamContactsDoc.Exists) {
						if(!string.IsNullOrEmpty(errmsg))
							errmsg += Environment.NewLine;
						errmsg += "Team Contacts Document";
					}
					if(!VFcontactsDoc.Exists) {
						if(!string.IsNullOrEmpty(errmsg))
							errmsg += Environment.NewLine;
						errmsg += "Useful Contacts Document";
					}
					if (!string.IsNullOrEmpty(errmsg))
						showBalloon("The following documents were not found",errmsg);
				}
			}
			else {
//				FileInfo[] shiftFiles = UserFolder.GetFiles("*shift*.xlsx");
//
//				if(shiftFiles.Length > 0) {
//					DateTime newestDate = Convert.ToDateTime("01/01/1900 00:00:00");
//					foreach (FileInfo file in shiftFiles) {
//						if(file.CreationTime > newestDate && !file.Attributes.ToString().Contains("Hidden")) {
//							newestDate = file.CreationTime;
//							Databases.ShiftsDoc = file;
//						}
//					}
//				}
				
				Documents.MenuItems[0].Enabled = Databases.shiftsFile.Exists;
				Documents.MenuItems[1].Enabled = false;
				Documents.MenuItems[3].Enabled = false;
				Documents.MenuItems[4].Enabled = false;
				Links.MenuItems[1].Enabled = false;
				InternalNotifyIcon.ContextMenu.MenuItems[9].Enabled = false;
//				MainForm.trayIc.ContextMenu.MenuItems[9].Enabled = false;
			}
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 13-11-2014
 * Time: 17:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Data.SQLite;
using appCore.DB;
using appCore.Logs;
using appCore.Settings;
using appCore.SiteFinder.UI;
using appCore.Templates;
using appCore.Templates.RAN.UI;
using appCore.Toolbox;
using appCore.UI;
using appCore.Shifts;
using appCore.OssScripts.UI;
using appCore.OI.JSON;
using Newtonsoft.Json;
using Microsoft.Exchange.WebServices.Data;
using appCore.Toolbox.TipOfTheDay;
using appCore.Toolbox.Notifications;

namespace appCore
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>

    public partial class MainForm : Form
    {
        public Bitmap ScreenshotBeforeMinimize;
        public static TrayIcon trayIcon;
        public static TroubleshootControls TroubleshootUI = new TroubleshootControls();
        public static FailedCRQControls FailedCRQUI = new FailedCRQControls();
        public static UpdateControls UpdateUI = new UpdateControls();
        public static TxControls TxUI = new TxControls();
        public static siteDetails SiteDetailsUI;
        public static PictureBox SiteDetailsPictureBox = new PictureBox();
        public static OutageControls OutageUI = new OutageControls();
        public static LogsCollection<Template> logFiles = new LogsCollection<Template>();
        public static ShiftsCalendar shiftsCalendar;
        public static Label TicketCountLabel = new Label();

        EricssonScriptsControls ericssonScriptsControls = new EricssonScriptsControls();
        NokiaScriptsControls nokiaScriptsControls = new NokiaScriptsControls();
        HuaweiScriptsControls huaweiScriptsControls = new HuaweiScriptsControls();

        public static NotificationsCenter notificationsCenter;

        ErrorProviderFixed errorProvider = new ErrorProviderFixed();

        System.Timers.Timer OiDbFilesLastUpdatedTimer;

        public MainForm(NotifyIcon tray, string[] args)
        {
            GlobalProperties.ApplicationStartTime = DateTime.Now;
            //args = new[] { "-otherUser", "DALEMN" }; // HACK: force login with another user
            //args = new[] { "-otherUser", "SILVABT" }; // HACK: force login with another user
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            GlobalProperties.ResolveOfficePath();

            SplashForm.ShowSplashScreen(true);
            trayIcon = new TrayIcon(tray);

            //GlobalProperties.DeployExternalAssemblies();
            //EmbeddedAssemblies.Init();

            // Initialize Properties

            SplashForm.UpdateLabelText("Getting network access");

            GlobalProperties.CheckShareAccess();

            SplashForm.UpdateLabelText("Loading User Profile");

            string otherUser = string.Empty;
            if (args.Contains("-otherUser"))
                try { otherUser = args[Array.FindIndex(args, str => str.Equals("-otherUser")) + 1].ToUpper(); } catch { }

            try
            {
                CurrentUser.InitializeUserProperties(otherUser);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + Environment.NewLine + "This means you don't have access to the Vodafone network, which you must have in order to use this tool.", "Quitting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            logFiles.Initialize();

            SplashForm.UpdateLabelText("Loading UI");

            InitializeComponent();

            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.SetIconPadding(textBox13, -17);

            StartMenuPanel.BackColor = CurrentUser.UserName == "GONCARJ3" ? Color.FromArgb(150, Color.LightGray) : Color.Transparent;
            if (!string.IsNullOrEmpty(otherUser))
            {
                Label otherUserLabel = new Label()
                {
                    Name = "otherUserLabel",
                    Text = "Logged on as " + otherUser,
                    Font = new Font("Courier New", 12F, FontStyle.Bold),
                    AutoSize = true
                };
                otherUserLabel.Location = new Point(0, (StartTabPage.Height + StartTabPage.Bounds.X) - otherUserLabel.Height);
                otherUserLabel.BackColor = Color.FromArgb(150, Color.LightGray);
                otherUserLabel.ForeColor = Color.Red;
                StartTabPage.Controls.Add(otherUserLabel);
            }

            //			ToolsMenu toolsMenu = new ToolsMenu();
            //			toolsMenu.Location = new Point(tabPage1.Right - toolsMenu.Width, 0);
            //			tabPage1.Controls.Add(toolsMenu);

            StartMenuPanel.Controls.Add(SiteDetailsPictureBox);
            // 
            // SiteDetailsPictureBox
            // 
            SiteDetailsPictureBox.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            SiteDetailsPictureBox.BackColor = Color.Transparent;
            SiteDetailsPictureBox.Image = Resources.radio_tower;
            SiteDetailsPictureBox.Location = new Point(6, 49);
            SiteDetailsPictureBox.Name = "SiteDetailsPictureBox";
            SiteDetailsPictureBox.Size = new Size(40, 40);
            SiteDetailsPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            SiteDetailsPictureBox.TabIndex = 8;
            SiteDetailsPictureBox.TabStop = false;
            SiteDetailsPictureBox.Click += PictureBoxesClick;
            SiteDetailsPictureBox.MouseLeave += PictureBoxesMouseLeave;
            SiteDetailsPictureBox.MouseHover += PictureBoxesMouseHover;

            StartTabPage.Controls.Add(TicketCountLabel);
            // 
            // TicketCountLabel
            // 
            TicketCountLabel.BackColor = Color.Transparent;
            TicketCountLabel.Size = new Size(40, 20);
            TicketCountLabel.Location = new Point(StartTabPage.Width - TicketCountLabel.Width - 5, StartTabPage.Height - TicketCountLabel.Height - 5);
            TicketCountLabel.Name = "TicketCountLabel";
            TicketCountLabel.TabIndex = 5;
            TicketCountLabel.TextAlign = ContentAlignment.MiddleRight;
            TicketCountLabel.MouseClick += TicketCountLabelMouseClick;

            string img = SettingsFile.BackgroundImage;

            if (img != string.Empty)
            {
                if (File.Exists(img))
                    StartTabPage.BackgroundImage = Image.FromFile(img);
                else
                    trayIcon.showBalloon("Image file not found", "Background Image file not found, applying default");
            }

            if (CurrentUser.UserName != "GONCARJ3")
            {
                MainTabControl.TabPages.Remove(TestTabPage);
                RAN_ScriptsTabControl.TabPages.Remove(AlcatelTabPage); // Alcatel scripts tab
                BreaksPictureBox.Visible = false;
            }

            if (CurrentUser.Department == Departments.RanTier1 || CurrentUser.Department == Departments.RanTier2)
            {
                TroubleshootUI.Location = new Point(1, 2);
                TroubleshootTabPage.Controls.Add(TroubleshootUI);
                FailedCRQUI.Location = new Point(1, 2);
                FailedCRQsTabPage.Controls.Add(FailedCRQUI);
                UpdateUI.Location = new Point(1, 2);
                UpdatesTabPage.Controls.Add(UpdateUI);
                TxUI.Location = new Point(1, 2);
                TxTabPage.Controls.Add(TxUI);

                nokiaScriptsControls.Location = new Point(1, 2);
                NokiaTabPage.Controls.Add(nokiaScriptsControls);
                huaweiScriptsControls.Location = new Point(1, 2);
                HuaweiTabPage.Controls.Add(huaweiScriptsControls);
                ericssonScriptsControls.Location = new Point(1, 2);
                EricsonScriptTabPage.Controls.Add(ericssonScriptsControls);

                OutageUI.Location = new Point(1, 2);
                RAN_OutagesTabPage.Controls.Add(OutageUI);
            }

            SplashForm.UpdateLabelText("Loading Databases");

            Databases.PopulateDatabases();

            string closureCode = CurrentUser.ClosureCode;
            if (!string.IsNullOrEmpty(closureCode))
                comboBox1.Items.Add(closureCode);
            comboBox1.Items.Add("CBV");
            comboBox1.Text = closureCode;

            GlobalProperties.SiteFinderMainswitch = false;
            bool OiAvailable = System.Threading.Tasks.Task.Run(OI.OiConnection.Available).Result;
            GlobalProperties.SiteFinderMainswitch = (Databases.all_sites.Exists && Databases.all_cells.Exists) && OiAvailable; // OI.OiConnection.Available().GetAwaiter().GetResult();

            if (CurrentUser.Department == Departments.RanTier1 && Databases.shiftsFile.Exists)
            {
                string[] monthShifts = Databases.shiftsFile.GetAllShiftsInMonth(CurrentUser.FullName[1] + " " + CurrentUser.FullName[0], DateTime.Now.Month);
                
                CalendarPictureBox.Visible = true;
                shiftsCalendar = new ShiftsCalendar();
                shiftsCalendar.Location = new Point((StartTabPage.Width - shiftsCalendar.Width) / 2, 0 - shiftsCalendar.Height);
                StartTabPage.Controls.Add(shiftsCalendar);
                //				}
            }

            // TODO: get sites list from alarms

            SplashForm.UpdateLabelText("Almost finished");

            // HACK: Developer specific action
            if (CurrentUser.UserName == "GONCARJ3" || CurrentUser.UserName == "SANTOSS2" || CurrentUser.Role == Roles.ShiftLeader)
            {
                Button UpdateOiFilesButton = new Button();
                UpdateOiFilesButton.Name = "UpdateOiFilesButton";
                UpdateOiFilesButton.Text = "Update OI DB Files";
                UpdateOiFilesButton.Size = new Size(110, 23);
                UpdateOiFilesButton.Click += UpdateDbFilesButtonClick;
                StartTabPage.Controls.Add(UpdateOiFilesButton);
                UpdateOiFilesButton.Location = new Point(5, StartTabPage.Height - UpdateOiFilesButton.Height - 5);
                OiDbFilesLastUpdatedTimer = new System.Timers.Timer(60 * 1000); // fires every 60 sec.
                OiDbFilesLastUpdatedTimer.Elapsed += OiDbFilesLastUpdatedTimer_Elapsed;

                allCellsLabel.Location = new Point(5, StartTabPage.Controls["UpdateOiFilesButton"].Top - 5 - allCellsLabel.Height);
                allSitesLabel.Location = new Point(5, allCellsLabel.Top - allSitesLabel.Height);
                allCellsLabel.Visible = true;
                allSitesLabel.Visible = true;

                allCellsLabel.Text = "all_cells last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");
                allSitesLabel.Text = "all_sites last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");

                if (CurrentUser.UserName == "GONCARJ3")
                {
                    Button butt = new Button();
                    butt.Name = "butt";
                    butt.Text = "Clear SitesDB";
                    butt.AutoSize = true;
                    butt.Location = new Point(UpdateOiFilesButton.Right + 10, UpdateOiFilesButton.Top);
                    butt.Click += delegate
                    {
                        SitesDB.Clear();

                        List<string> list = new List<string>();
                        while (list.Count < 1000000)
                            list.Add("str");

                        //Stopwatch st = new Stopwatch();
                        //st.Start();
                        bool result = list.Contains("and", StringComparison.OrdinalIgnoreCase);
                        //st.Stop();
                        //var t = st.Elapsed;

                        //var sites = SitesDB.getSites(new[] { "15", "3792", "4467", "1190", "46788" });
                        //// horizontal
                        //double maxLongitude = sites.Max(s => s.Coordinates.Longitude);
                        //double minLongitude = sites.Min(s => s.Coordinates.Longitude);

                        //// vertical
                        //double maxLatitude = sites.Max(s => s.Coordinates.Latitude);
                        //double minLatitude = sites.Min(s => s.Coordinates.Latitude);

                        //var t = openWeatherAPI.queryZipCode(sites[0].PostCode);
                        //var cities = openWeatherAPI.query(minLongitude, maxLongitude, minLatitude, maxLatitude);
                        //						InputBoxDialog ib = new InputBoxDialog();
                        //						ib.FormPrompt = "Site to remove?\n\n(Leave blank and click ok to clear whole DB";
                        //						DialogResult ans = ib.ShowDialog();
                        //						if(ans != DialogResult.Cancel) {
                        //							string input = ib.InputResponse;
                        ////						string input = Microsoft.VisualBasic.Interaction.InputBox("Site to remove?\n\n(Leave blank and click ok to clear whole DB");
                        //							if(string.IsNullOrEmpty(input))
                        //								SitesDB.Clear();
                        //							else
                        //								SitesDB.Remove(input);
                        //						}

                        //						Remedy.UI.RemedyWebBrowser wb = new appCore.Remedy.UI.RemedyWebBrowser();
                        //						wb.Show();
                    };
                    StartTabPage.Controls.Add(butt);

                    Button butt3 = new Button();
                    butt3.Name = "butt3";
                    butt3.Text = "Show SitesDB";
                    butt3.AutoSize = true;
                    butt3.Location = new Point(butt.Right + 10, butt.Top);
                    butt3.Click += delegate
                    {
                        ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                        service.UseDefaultCredentials = true;
                        // Set the URL.
                        service.Url = new Uri("https://outlook-north.vodafone.com/ews/exchange.asmx");

                        ExpandGroupResults myGroupMembers = service.ExpandGroup("anoccsuk@internal.vodafone.com");
                        
                        List<dynamic> t = new List<dynamic>();
                        foreach(var m in myGroupMembers.Members)
                            t.Add(new { user = m.Name, email = m.Address, dep = CurrentUser.GetUser("", "", m.Address, m.Name, "Vodafone Portugal").Department });

                        //myGroupMembers = service.ExpandGroup("anocuk2ndlinehuawei_nsn@internal.vodafone.com");
                        //foreach (var m in myGroupMembers.Members)
                        //    t.Add(new { user = m.Name, email = m.Address, dep = CurrentUser.GetUserDetails(m.Name, m.Address, "Department") });

                        //System.Collections.Generic.List<string> deps = new System.Collections.Generic.List<string>();
                        for (int c = 0; c < t.Count; c++)
                            t[c] = new { user = t[c].user, email = t[c].email, strDep = t[c].dep, resDep = CurrentUser.GetUser("", "", t[c].email, t[c].user, "Vodafone Portugal").Department };

                        string departs = string.Join(Environment.NewLine, t.Select(u => u.user + "," + u.email + "," + u.strDep + "," + u.resDep));

                        //var t2 = myGroupMembers.Members.Select(m => m.);

                        //Form form = new Form();
                        //form.Size = new Size((int)(Screen.FromControl(this).Bounds.Width * 0.85), (int)(Screen.FromControl(this).Bounds.Height * 0.6));
                        //form.Text = SitesDB.List.Count.ToString();
                        //form.Text += SitesDB.List.Count > 1 ? " sites listed" : " site listed";

                        //DataGridView dgv = new DataGridView();
                        //dgv.Dock = DockStyle.Fill;
                        //dgv.RowHeadersVisible = false;
                        //dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        //dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        //dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                        //dgv.DataSource = SitesDB.List;
                        ////						dgv.DataSource = dt;

                        //foreach (DataGridViewColumn col in dgv.Columns)
                        //{
                        //    if (col.Name == "KeyInformation" || col.Name == "HealthAndSafety" || col.Name == "Address")
                        //    {
                        //        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        //        col.Width = col.Name == "Address" ? 100 : 300;
                        //        col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        //    }
                        //}

                        //form.Controls.Add(dgv);
                        //form.ShowDialog();
                    };
                    StartTabPage.Controls.Add(butt3);

                    //					OutageUI.Location = new Point(1, 2);
                    //					tabPage17.Controls.Add(OutageUI);
                }
            }

            trayIcon.toggleShareAccess();

            toolTipDeploy();
            
            ProcessControlPermissions();

            notificationsCenter = new NotificationsCenter();
            UpdateNotificationsIcon(NotificationsPictureBox);

            //SQLiteConnection.CreateFile(GlobalProperties.AppDataRootDir + "\\MyDatabase.sqlite");

            //SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + GlobalProperties.AppDataRootDir + "\\MyDatabase.sqlite;Version=3;");
            //m_dbConnection.Open();
            //string sql = "CREATE TABLE highscores (name VARCHAR(20), score INT)";
            //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();

            //sql = "insert into highscores (name, score) values ('Me', 3000)";
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //sql = "insert into highscores (name, score) values ('Myself', 6000)";
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //sql = "insert into highscores (name, score) values ('And I', 9001)";
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();

            //SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + GlobalProperties.AppDataRootDir + "\\MyDatabase.sqlite;Version=3;");
            //m_dbConnection.Open();

            //string sql = "select * from highscores order by score desc";
            //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            //SQLiteDataReader reader = command.ExecuteReader();
            //List<string> list2 = new List<string>();
            //while (reader.Read())
            //   list2.Add("Name: " + reader["name"] + "\tScore: " + reader["score"]);

            SplashForm.CloseForm();

            if (SettingsFile.LastRunVersion != GlobalProperties.AssemblyFileVersionInfo.FileVersion)
            {
                SettingsFile.LastRunVersion = GlobalProperties.AssemblyFileVersionInfo.FileVersion;
                FlexibleMessageBox.Show(Resources.Changelog, "Changelog", MessageBoxButtons.OK);
            }
            
            //LoadUiForDepartment();
        }

        void ProcessControlPermissions()
        {
            if (CurrentUser.Department != Departments.RanTier1 && CurrentUser.Department != Departments.RanTier2)
            {
                StartMenuPanel.Controls.Remove(AMTBrowserPictureBox);
                MainTabControl.Controls.Remove(RAN_CellsDownTabPage);
                MainTabControl.Controls.Remove(RAN_ClosureCodeTabPage);
                MainTabControl.Controls.Remove(RAN_OutagesTabPage);
                MainTabControl.Controls.Remove(ScriptsTabPage);
                //ScriptsTabPage.Controls.Remove(RAN_ScriptsTabControl);
                MainTabControl.Controls.Remove(TemplatesTabPage);
                //TemplatesTabPage.Controls.Remove(Ran_TemplatesTabControl);
                StartTabPage.Controls.Remove(TicketCountLabel);
            }
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            LoadingPanel loading = null;

            try
            {
                if (notificationsCenter.HasUnreadNotifications)
                {
                    loading = new LoadingPanel();
                    await System.Threading.Tasks.Task.Run(() => Thread.Sleep(150));
                    loading.Show(false, this);

                    notificationsCenter.OpenGUI(new Point(StartTabPage.PointToScreen(Point.Empty).X + ((StartTabPage.Width - NotificationsCenter.GuiSize.Width) / 2), StartTabPage.PointToScreen(Point.Empty).Y));
                }
            }
            catch (Exception ex)
            {
                var m = ex.Message;
            }

            if (TipOfTheDayDialog.ShowTipsOnStartUp)
            {
                if (loading != null)
                {
                    loading = new LoadingPanel();
                    await System.Threading.Tasks.Task.Run(() => Thread.Sleep(150));
                    loading.Show(false, this);
                }

                TipOfTheDayDialog dlg = new TipOfTheDayDialog();
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = new Point(StartTabPage.PointToScreen(Point.Empty).X + ((StartTabPage.Width - dlg.Width) / 2), StartTabPage.PointToScreen(Point.Empty).Y);
                dlg.ShowDialog();
            }

            if (OiDbFilesLastUpdatedTimer != null)
                OiDbFilesLastUpdatedTimer.Enabled = true;

            if (loading != null)
                loading.Close();
        }

        public static void UpdateNotificationsIcon(PictureBox pictureBox = null)
        {
            try
            {
                if(pictureBox == null)
                    pictureBox = Application.OpenForms.Cast<Form>().First(f => f.GetType() == typeof(MainForm)).Controls["MainTabControl"].Controls["StartTabPage"].Controls["NotificationsPictureBox"] as PictureBox;

                if (notificationsCenter.TotalNotificationsCount > 0)
                {
                    if (notificationsCenter.UnreadNotificationsCount > 0)
                        pictureBox.Image = Resources.notifications_unread;
                    else
                        pictureBox.Image = Resources.notifications_read;
                }
                else
                    pictureBox.Image = Resources.notifications;
            }
            catch { }            
        }

        public static async void OpenNotificationsGUI_Delegate()
        {
            MainForm mf = Application.OpenForms.Cast<Form>().First(f => f.GetType() == typeof(MainForm)) as MainForm;
            Control startTabPage = mf.Controls["MainTabControl"].Controls["StartTabPage"];
            mf.MainFormActivate(null, null);
            //await System.Threading.Tasks.Task.Run(() => Thread.Sleep(150));

            LoadingPanel loading = null;
            mf.Invoke((MethodInvoker)delegate
            {
                loading = new LoadingPanel();
                loading.Show(false, mf);

                notificationsCenter.OpenGUI(new Point(startTabPage.PointToScreen(Point.Empty).X + ((startTabPage.Width - NotificationsCenter.GuiSize.Width) / 2), startTabPage.PointToScreen(Point.Empty).Y));
                
                loading.Close();
            });
        }

        //void LoadUiForDepartment()
        //{
        //    // TODO: Hide/Unhide UI components depending on user Department
        //    List<Control> controls = null;
        //    string t = string.Join(Environment.NewLine, Tools.FindAllControls(Controls));
        //    switch (CurrentUser.Department)
        //    {
        //        case Departments.RanTier1:
        //        case Departments.RanTier2:
        //            controls = MainTabControl.Controls.Cast<Control>().Where(c => c.Name.StartsWith("TX_") || c.Name.StartsWith("CORE_")).ToList();

        //            foreach (Control c in controls)
        //                c.Visible = false;
        //            break;
        //        case Departments.TxTier1:
        //        case Departments.TxTier2:
        //            controls = MainTabControl.Controls.Cast<Control>().Where(c => c.Name.StartsWith("RAN_") || c.Name.StartsWith("CORE_")).ToList();
        //            foreach (Control c in controls)
        //                c.Parent.Controls.Remove(c);
        //            break;
        //        case Departments.CoreTier1:
        //        case Departments.CoreTier2:
        //            controls = MainTabControl.Controls.Cast<Control>().Where(c => c.Name.StartsWith("RAN_") || c.Name.StartsWith("TX_")).ToList();
        //            foreach (Control c in controls)
        //                c.Visible = false;
        //            break;
        //    }
        //}

        void OiDbFilesLastUpdatedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var allCellsLastUpdated = DateTime.Now - new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv").LastWriteTime;
            var allSitesLastUpdated = DateTime.Now - new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv").LastWriteTime;

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    allCellsLabel.Text = "all_cells last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");
                    allSitesLabel.Text = "all_sites last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");
                });
            }
            else
            {
                allCellsLabel.Text = "all_cells last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");
                allSitesLabel.Text = "all_sites last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");
            }

            var UpdateWarningTimeSpan = new TimeSpan(8, 0, 0);

            var msgBoxForm = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.Text == "OI DB Files out of date");

            if (allCellsLastUpdated > UpdateWarningTimeSpan || allSitesLastUpdated > UpdateWarningTimeSpan)
            {
                FileInfo updating = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\Updating");
                if (!updating.Exists)
                {
                    if (msgBoxForm == null)
                    {
                        MainFormActivate(null, null);

                        var ans = FlexibleMessageBox.Show("At least one OI DB File is more than 8h old, please consider updating the OI DB Files" + Environment.NewLine + Environment.NewLine + "Click Yes to begin updating.", "OI DB Files out of date", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (ans == DialogResult.Yes)
                            UpdateDbFilesButtonClick(StartTabPage.Controls["UpdateOiFilesButton"], null);
                    }
                }
            }
            else
            {
                if(msgBoxForm != null)
                    msgBoxForm.Invoke(new Action(() => { msgBoxForm.Close(); }));
            }
        }

        public async System.Threading.Tasks.Task FillTemplateFromLog(Template log)
        {
            LoadingPanel loading = new LoadingPanel();
            
            //Invoke((MethodInvoker)async delegate
            //{
                MainTabControl.SelectTab(1);
                switch (log.LogType)
                {
                    case TemplateTypes.Troubleshoot:
                        Ran_TemplatesTabControl.SelectTab(0);

                        await System.Threading.Tasks.Task.Run(() => Thread.Sleep(150));

                        loading.Show(false, TroubleshootTabPage);

                        if (TroubleshootUI != null)
                            TroubleshootUI.Dispose();
                        TroubleshootUI = new TroubleshootControls(log.ToTroubleshootTemplate(), UiEnum.Template);
                        TroubleshootTabPage.Controls.Add(TroubleshootUI);
                        break;
                    case TemplateTypes.FailedCRQ:
                        Ran_TemplatesTabControl.SelectTab(1);

                        await System.Threading.Tasks.Task.Run(() => Thread.Sleep(50));

                        loading.Show(false, FailedCRQsTabPage);

                        if (FailedCRQUI != null)
                            FailedCRQUI.Dispose();
                        FailedCRQUI = new FailedCRQControls(log.ToFailedCRQTemplate(), UiEnum.Template);
                        FailedCRQsTabPage.Controls.Add(FailedCRQUI);
                        break;
                    case TemplateTypes.Update:
                        Ran_TemplatesTabControl.SelectTab(2);

                        await System.Threading.Tasks.Task.Run(() => Thread.Sleep(50));

                        loading.Show(false, UpdatesTabPage);

                        if (UpdateUI != null)
                            UpdateUI.Dispose();
                        UpdateUI = new UpdateControls(log.ToUpdateTemplate(), UiEnum.Template);
                        UpdatesTabPage.Controls.Add(UpdateUI);
                        break;
                    case TemplateTypes.TX:
                        Ran_TemplatesTabControl.SelectTab(3);

                        await System.Threading.Tasks.Task.Run(() => Thread.Sleep(50));

                        loading.Show(false, TxTabPage);

                        if (TxUI != null)
                            TxUI.Dispose();
                        TxUI = new TxControls(log.ToTxTemplate(), UiEnum.Template);
                        TxTabPage.Controls.Add(TxUI);
                        break;
                }

                loading.Close();
                MainFormActivate(null, null);
            //});            
        }

        public static void UpdateTicketCountLabel(bool ignoreLabelVisibility = false)
        {
            if (!string.IsNullOrEmpty(TicketCountLabel.Text) || ignoreLabelVisibility)
                TicketCountLabel.Text = logFiles.FilterCounts(TemplateTypes.Troubleshoot | TemplateTypes.FailedCRQ).ToString();
        }

        void TicketCountLabelMouseClick(object sender, MouseEventArgs e)
        {
            if (shiftsCalendar.isVisible)
                shiftsCalendar.toggleShiftsPanel();

            if(e.Button == MouseButtons.Left)
            {
                if (logFiles.LogFile.Exists)
                {
                    if (string.IsNullOrEmpty(TicketCountLabel.Text))
                    {
                        logFiles.CheckLogFileIntegrity();
                        UpdateTicketCountLabel(true);
                    }
                    else
                        TicketCountLabel.Text = string.Empty;
                }
                else
                {
                    TicketCountLabel.Text = string.IsNullOrEmpty(TicketCountLabel.Text) ? 0.ToString() : string.Empty;
                }
            }
        }

        void TabPage1BackgroundImageChanged(object sender, EventArgs e)
        {
            Control[] controls = new[] { TicketCountLabel, allSitesLabel, allCellsLabel };
            for(int c = 0; c < controls.Length;c++)
            {
                Control ctrl = controls[c];
                if(ctrl != null)
                {
                    string text = ctrl.Text;
                    ctrl.Text = string.Empty;
                    //Color back = ctrl.BackColor;
                    ctrl.BackColor = Color.Transparent;
                    
                    ctrl.ForeColor = Tools.GetContrastForeground(ctrl);

                    ctrl.Text = text;
                }
            }
        }

        void allSitesCellsLabelsForeColorChanged(object sender, EventArgs e)
        {
            Control lbl = sender as Label;
            if (lbl.ForeColor.R == Color.White.R && lbl.ForeColor.G == Color.White.G && lbl.ForeColor.B == Color.White.B)
                lbl.BackColor = Color.FromArgb(100, Color.Black);
            else
                lbl.BackColor = Color.FromArgb(100, Color.White);
        }

        void toolTipDeploy()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 600000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;

            toolTip.ShowAlways = false; // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.IsBalloon = true;
            toolTip.UseAnimation = true;
            toolTip.UseFading = false;
            toolTip.BackColor = Color.White;
            toolTip.ForeColor = Color.Firebrick;

            // Set up the ToolTip text for each object

            toolTip.SetToolTip(SettingsPictureBox, "Settings");
            toolTip.SetToolTip(AMTBrowserPictureBox, "AMT Browser");
            toolTip.SetToolTip(NotesPictureBox, "Notes");
            toolTip.SetToolTip(LogsPictureBox, "Log Browser");
            toolTip.SetToolTip(SiteDetailsPictureBox, "Site Finder");
            toolTip.SetToolTip(CalendarPictureBox, "Shifts Calendar");
            toolTip.SetToolTip(QuestionMarkPictureBox, "Tips of the Day");
            toolTip.SetToolTip(NotificationsPictureBox, "Notifications Center");

            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip2 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip2.AutoPopDelay = 600000;
            toolTip2.InitialDelay = 500;
            toolTip2.ReshowDelay = 500;
            toolTip2.ReshowDelay = 500;
            toolTip2.ReshowDelay = 500;

            toolTip2.ShowAlways = false; // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip2.IsBalloon = true;
            toolTip2.UseAnimation = true;
            toolTip2.UseFading = false;
            toolTip2.BackColor = Color.White;
            toolTip2.ForeColor = Color.Firebrick;

            // Set up the ToolTip text for each object

            toolTip2.SetToolTip(BreaksPictureBox, "Breaks Planner");
        }

        void TextBox13TextChanged(object sender, EventArgs e)
        {
            textBox14.Text = "";
            if (textBox13.Text.Length == 15 & comboBox1.Text.Length == 3)
            {
                label30.Visible = false;
                textBox13.TextChanged -= TextBox13TextChanged;
                comboBox1.TextChanged -= ComboBox1TextChanged;
                textBox13.Text = textBox13.Text.ToUpper();
                comboBox1.Text = comboBox1.Text.ToUpper();
                textBox13.TextChanged += TextBox13TextChanged;
                comboBox1.TextChanged += ComboBox1TextChanged;
                if (textBox13.Text.Substring(3, textBox13.Text.Length - 3).IsAllDigits())
                {
                    int[] rng = new int[12];
                    for (int c = 0; c <= 11; c++)
                    {
                        rng[c] = Convert.ToInt32(textBox13.Text.Substring(c + 3, 1));
                    }
                    int SumFw = rng[0] * 2 + rng[1] * 3 + rng[2] * 4 + rng[3] * 5 + rng[4] * 6 + rng[5] * 7 + rng[6] * 8 + rng[7] * 9 + rng[8] * 10 + rng[9] * 11 + rng[10] * 12 + rng[11] * 13;
                    int SumBw = rng[11] * 2 + rng[10] * 3 + rng[9] * 4 + rng[8] * 5 + rng[7] * 6 + rng[6] * 7 + rng[5] * 8 + rng[4] * 9 + rng[3] * 10 + rng[2] * 11 + rng[1] * 12 + rng[0] * 13;
                    string hx = (SumFw * SumBw).ToString("X");
                    while (hx.Length < 5)
                        hx = "0" + hx;
                    textBox14.Text += hx + " " + comboBox1.Text;
                }
                else
                {
                    LoadingPanel load = new LoadingPanel();
                    load.Show(false, this);

                    FlexibleMessageBox.Show("INC/CRQ can only contain numbers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox13.TextChanged -= TextBox13TextChanged;
                        textBox13.Text = "";
                        textBox13.TextChanged += TextBox13TextChanged;
                    
                    load.Close();
                }
            }
            else
            {
                label30.Visible = true;
                if (textBox13.Text.Length > 0 && textBox13.Text.Length < 15)
                    label30.Text = "Press ENTER key to complete INC number";
                else
                {
                    if (textBox13.Text.Length != 15)
                        label30.Text = "Insert INC/CRQ number";
                    else
                        label30.Visible = false;
                }
            }
        }

        void ComboBox1TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text.Length == 3 & textBox13.Text.Length == 15)
                {
                    label31.Visible = false;
                    TextBox13TextChanged(sender, e);
                }
                else
                {
                    textBox14.Text = "";
                    if (comboBox1.Text.Length < 3)
                        label31.Visible = true;
                    else
                    {
                        comboBox1.TextChanged -= ComboBox1TextChanged;
                        comboBox1.Text = comboBox1.Text.ToUpper();
                        comboBox1.TextChanged += ComboBox1TextChanged;
                        label31.Visible = false;
                    }
                }
            }
            finally { }
        }

        void UpdateDbFilesButtonClick(object sender, EventArgs e)
        {
            Thread t = new Thread(() =>
            {
                Button butt2 = sender as Button;

                butt2.Invoke((MethodInvoker)delegate
                {
                    butt2.Enabled = false;
                });

                Databases.UpdateSourceDBFiles();

                butt2.Invoke((MethodInvoker)delegate
                {
                    butt2.Enabled = true;
                    allSitesLabel.Text = "all_sites last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");
                    allCellsLabel.Text = "all_cells last update time: " + new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv").LastWriteTime.ToString("dd/MM/yyyy HH:mm");
                });
            });
            t.Name = "UpdateSourceDBFiles_MainFormButton";
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        async void Button4Click(object sender, EventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, RAN_CellsDownTabPage);

            if (string.IsNullOrEmpty(amtRichTextBox1.Text))
            {
                FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);

                load.Close();
                return;
            }

            load.ToggleLoadingSpinner();

            bool parsingError = false;
            string alarms = amtRichTextBox1.Text;
            Netcool.AlarmsParser netcool = null;

            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    //var st = new Stopwatch();
                    //st.Start();
                    netcool = new Netcool.AlarmsParser(alarms, Netcool.AlarmsParser.ParsingMode.CoosReport, false);
                    //st.Stop();
                    //var t = st.Elapsed;
                }
                catch (Exception ex)
                {
                    trayIcon.showBalloon("Error parsing alarms", "An error occurred while parsing the alarms.\n\nError message:\n" + ex.Message);
                    parsingError = true;
                }
            });
            
            if (!parsingError)
            {
                amtRichTextBox1.Text = netcool.GenerateCoosReport();

                amtRichTextBox1.Select(0, 0);
                button4.Enabled = false;
                button3.Enabled = true;
                amtRichTextBox1.ReadOnly = true;
                label1.Text = String.Format("Cells down report ({0})", netcool.AlarmsList.Count);
            }
                
            load.Close();
        }

        async void Button6Click(object sender, EventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, NetcoolParserTabPage);

            if (string.IsNullOrEmpty(textBox12.Text))
            {
                FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);

                load.Close();
                return;
            }

            load.ToggleLoadingSpinner();

            bool parsingError = false;
            string alarms = textBox12.Text;
            Netcool.AlarmsParser netcool = null;

            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    netcool = new Netcool.AlarmsParser(alarms, Netcool.AlarmsParser.ParsingMode.ParseAllAlarms);
                }
                catch (Exception ex)
                {
                    trayIcon.showBalloon("Error parsing alarms", "An error occurred while parsing the alarms.\n\nError message:\n" + ex.Message);
                    parsingError = true;
                }
            });
            
            if (!parsingError)
            {
                textBox12.Text = netcool.ToString();

                textBox12.Select(0, 0);
                button6.Enabled = false;
                button5.Enabled = true;
                button13.Visible = true;
                textBox12.ReadOnly = true;
                label34.Text = String.Format("Parsed alarms ({0})", netcool.AlarmsList.Count);
            }
                
            load.Close();
        }

        void Button5Click(object sender, EventArgs e)
        {
            label34.Text = "Alarms to parse";
            button6.Enabled = true;
            button5.Enabled = false;
            textBox12.ReadOnly = false;
            textBox12.Text = "";
            button13.Visible = false;
            textBox12.Focus();
        }

        void Button3Click(object sender, EventArgs e)
        {
            label1.Text = "Paste all active alarms from Netcool";
            button4.Enabled = true;
            amtRichTextBox1.ReadOnly = false;
            amtRichTextBox1.Text = "";
            amtRichTextBox1.Focus();
        }

        void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MainTabControl.SelectedIndex)
            {
                case 1:
                    TabControl2SelectedIndexChanged(null, null);
                    break;
                case 2:
                    RAN_ScriptsTabControlSelectedIndexChanged(null, null);
                    break;
                case 3:
                    textBox13.Focus();
                    break;
                case 4:
                    OutageUI.Alarms_ReportTextBox.Focus();
                    break;
                case 5:
                    textBox12.Focus();
                    break;
                case 6:
                    amtRichTextBox1.Focus();
                    break;
            }
        }

        void TabControl2SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Ran_TemplatesTabControl.SelectedIndex)
            {
                case 0:
                    TroubleshootUI.SiteIdTextBox.Focus();
                    break;
                case 1:
                    FailedCRQUI.SiteIdTextBox.Focus();
                    break;
                case 2:
                    UpdateUI.SiteIdTextBox.Focus();
                    break;
                case 3:
                    TxUI.SitesTextBox.Focus();
                    break;
            }
        }

        void RAN_ScriptsTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (RAN_ScriptsTabControl.SelectedIndex)
            {
                case 0:
                    ericssonScriptsControls.SiteTextBox.Focus();
                    break;
                case 1:
                    nokiaScriptsControls.BcfPcmTextBox.Focus();
                    break;
                case 2:
                    huaweiScriptsControls.SiteTextBox.Focus();
                    break;
                //case 3:
                //    TxUI.SitesTextBox.Focus();
                //    break;
            }
        }

        void TextBox13KeyPress(object sender, KeyPressEventArgs e)
        {

            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                errorProvider.SetError(textBox13, string.Empty);
                try
                {
                    textBox13.Text = Tools.CompleteRemedyReference(textBox13.Text, "INC/CRQ");
                }
                catch (Exception ex)
                {
                    errorProvider.SetError(textBox13, ex.Message);
                    return;
                }
            }
        }

        void TabPage1MouseClick(object sender, MouseEventArgs e)
        {
            //FIXME:			wholeShiftsPanelDispose();
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show(PointToScreen(e.Location));
            if(shiftsCalendar != null)
            {
                if (shiftsCalendar.isVisible)
                    shiftsCalendar.toggleShiftsPanel();
            }
        }

        void ToolStripMenuItem2Click(object sender, EventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            OpenFileDialog fileBrowser = new OpenFileDialog();
            fileBrowser.Filter = "All image files(*.bmp,*.gif,*.jpg,*.jpeg,*.png)|*.bmp;*.gif;*.jpg;*.jpeg;*.png";
            fileBrowser.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileBrowser.Title = "Please select a new background image";
            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                SettingsFile.BackgroundImage = fileBrowser.FileName;
                StartTabPage.BackgroundImage = Image.FromFile(fileBrowser.FileName);
            }

            load.Close();
        }

        void ToolStripMenuItem3Click(object sender, EventArgs e)
        {
            SettingsFile.BackgroundImage = string.Empty;
            StartTabPage.BackgroundImage = Resources.zoozoo_wallpaper_15;
        }

        void Button13Click(object sender, EventArgs e)
        {
            //			Action action = new Action(delegate {
            TroubleshootUI.ActiveAlarmsTextBox.Text = textBox12.Text;
            MainTabControl.SelectTab(1);
            Ran_TemplatesTabControl.SelectTab(0);
            //			                           });
            //			LoadingPanel load = new LoadingPanel();
            //			load.Show(null, action, false, this);
        }

        void TextBox12TextChanged(object sender, EventArgs e)
        {
            if (textBox12.Text != string.Empty) button24.Enabled = true;
            else button24.Enabled = false;
        }

        void AmtRichTextBox1TextChanged(object sender, EventArgs e)
        {
            button24.Enabled =
                button3.Enabled = !string.IsNullOrEmpty(amtRichTextBox1.Text);
        }

        void Button24Click(object sender, EventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox12.Text, label34.Text, false);
                enlarge.StartPosition = FormStartPosition.CenterParent;
                enlarge.ShowDialog();
                textBox12.Text = enlarge.finaltext;

            load.Close();
        }

        void Button1Click(object sender, EventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            AMTLargeTextForm enlarge = new AMTLargeTextForm(amtRichTextBox1.Text, label1.Text, false);
                enlarge.StartPosition = FormStartPosition.CenterParent;
                enlarge.ShowDialog();
                amtRichTextBox1.Text = enlarge.finaltext;

            load.Close();
        }

        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            LoadingPanel load = new LoadingPanel();
            load.Show(false, this);

            DialogResult ans = FlexibleMessageBox.Show("Are you sure you want to quit ANOC Master Tool?", "Quitting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans == DialogResult.Yes)
            {
                UserFolder.ClearTempFolder();

                var openForms = Application.OpenForms.Cast<Form>().ToList();
                if (openForms.Count > 0)
                {
                    for (int c = 0; c < openForms.Count; c++)
                    {
                        Form form = openForms[c];
                        if (form != this)
                        {
                            if (form.InvokeRequired)
                            {
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.Close();
                                    form.Dispose();
                                }));
                            }
                            else
                            {
                                form.Close();
                                form.Dispose();
                            }
                        }
                    }
                }
            }
            else
                e.Cancel = true;

            load.Close();
        }

        void MainFormActivate(object sender, EventArgs e)
        {
            var form = Application.OpenForms.OfType<MainForm>().First();

            if(form.InvokeRequired)
            {
                form.Invoke((MethodInvoker)delegate
                {
                    form.Activate();
                    form.WindowState = FormWindowState.Normal;
                });
            }
            else
            {
                form.Activate();
                form.WindowState = FormWindowState.Normal;
            }
        }

        public static void openSettings(Control callerControl, bool fromTrayIcon = false)
        {
            Settings.UI.SettingsForm settings = new Settings.UI.SettingsForm();
            settings.StartPosition = FormStartPosition.CenterParent;
            settings.ShowDialog();
        }

        public static void openAMTBrowser()
        {
            var fc = Application.OpenForms.OfType<Web.UI.BrowserView>().ToList();

            if (fc.Count > 0)
            {
                if (fc[0].WindowState == FormWindowState.Minimized)
                    fc[0].Invoke(new Action(() => { fc[0].WindowState = FormWindowState.Normal; }));
                fc[0].Invoke(new MethodInvoker(fc[0].Activate));
                return;
            }

            Thread thread = new Thread(() =>
            {
                Web.UI.BrowserView brwsr = new Web.UI.BrowserView();
                brwsr.StartPosition = FormStartPosition.CenterParent;
                brwsr.ShowDialog();
            });
            thread.Name = "AMTBrowser";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void openNotes()
        {
            var fc = Application.OpenForms.OfType<NotesForm>().ToList();

            if (fc.Count > 0)
            {
                fc[0].Activate();
                return;
            }

            NotesForm notes = new NotesForm();
            notes.StartPosition = FormStartPosition.CenterParent;
            notes.Show();
        }

        public static void openLogBrowser()
        {
            var fc = Application.OpenForms.Cast<Form>().ToList();

            var lbForm = (Logs.UI.LogBrowser)fc.Find(f => f is Logs.UI.LogBrowser);

            if (lbForm != null)
            {
                if (lbForm.WindowState == FormWindowState.Minimized)
                    lbForm.Invoke(new Action(() => { lbForm.WindowState = FormWindowState.Normal; }));
                lbForm.Invoke(new MethodInvoker(lbForm.Activate));
                return;
            }

            MainForm _this = (MainForm)fc.Find(f => f is MainForm);

            //			foreach (Form frm in fc) {
            //				if(frm.Name == "MainForm")
            //					_this = (MainForm)frm;
            //				if (frm.Name == "LogBrowser") {
            //					if(frm.WindowState == FormWindowState.Minimized) frm.Invoke(new Action(() => { frm.WindowState = FormWindowState.Normal; }));
            //					frm.Invoke(new MethodInvoker(frm.Activate));
            //					return;
            //				}
            //			}

            Thread thread = new Thread(() =>
            {
                Logs.UI.LogBrowser LogView = new Logs.UI.LogBrowser(_this);
                LogView.StartPosition = FormStartPosition.CenterParent;
                LogView.ShowDialog();
            });
            thread.Name = "LogBrowser";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void openSiteFinder()
        {
            var fc = Application.OpenForms.OfType<siteDetails>().Where(f => f.Text == "Site Finder").ToArray();

            if (fc.Length > 0)
            {
                if (fc[0].WindowState == FormWindowState.Minimized)
                    fc[0].Invoke(new Action(() => { fc[0].WindowState = FormWindowState.Normal; }));
                fc[0].Invoke(new MethodInvoker(fc[0].Activate));
                return;
            }

            Thread thread = new Thread(() =>
            {
                siteDetails sd = new siteDetails();
                sd.StartPosition = FormStartPosition.CenterParent;
                sd.ShowDialog();
            });
            thread.Name = "siteFinder";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        void PictureBoxesClick(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            if (pic.Name != "CalendarPictureBox")
            {
                if(shiftsCalendar != null)
                {
                    if (shiftsCalendar.isVisible)
                        shiftsCalendar.toggleShiftsPanel();
                }
            }
            LoadingPanel loading;
            switch (pic.Name)
            {
                case "SettingsPictureBox":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);

                    loading = new LoadingPanel();
                    loading.Show(false, this);

                    openSettings(this);

                    loading.Close();
                    break;
                case "AMTBrowserPictureBox":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openAMTBrowser();
                    break;
                case "NotesPictureBox":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openNotes();
                    break;
                case "LogsPictureBox":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openLogBrowser();
                    break;
                case "SiteDetailsPictureBox":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openSiteFinder();
                    break;
                case "CalendarPictureBox":
                    shiftsCalendar.toggleShiftsPanel();
                    break;
                case "QuestionMarkPictureBox":
                    loading = new LoadingPanel();
                    loading.Show(false, this);

                    TipOfTheDayDialog dlg = new TipOfTheDayDialog();
                    dlg.StartPosition = FormStartPosition.Manual;
                    dlg.Location = new Point(StartTabPage.PointToScreen(Point.Empty).X + ((StartTabPage.Width - dlg.Width) / 2), StartTabPage.PointToScreen(Point.Empty).Y);
                    dlg.ShowDialog();

                    loading.Close();
                    break;
                case "NotificationsPictureBox":
                    loading = new LoadingPanel();
                    loading.Show(false, this);

                    notificationsCenter.OpenGUI(new Point(StartTabPage.PointToScreen(Point.Empty).X + ((StartTabPage.Width - NotificationsCenter.GuiSize.Width) / 2), StartTabPage.PointToScreen(Point.Empty).Y));

                    loading.Close();
                    break;
                case "BreaksPictureBox":
                    //loading = new LoadingPanel();
                    //loading.Show(false, this);


                    
                    //loading.Close();
                    break;
            }
        }

        void PictureBoxesMouseLeave(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            switch (pic.Name)
            {
                case "SettingsPictureBox":
                    pic.Image = Resources.Settings_normal;
                    break;
                case "AMTBrowserPictureBox":
                    pic.Image = Resources.globe;
                    break;
                case "LogsPictureBox":
                    pic.Image = Resources.book;
                    break;
                case "NotesPictureBox":
                    break;
                case "SiteDetailsPictureBox":
                    pic.Image = Resources.radio_tower;
                    break;
                case "CalendarPictureBox":
                    pic.Image = Resources.calendar2;
                    break;
            }
        }

        void PictureBoxesMouseHover(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            switch (pic.Name)
            {
                case "SettingsPictureBox":
                    pic.Image = Resources.Settings_hover;
                    break;
                case "AMTBrowserPictureBox":
                    pic.Image = Resources.globe_hover;
                    break;
                case "LogsPictureBox":
                    break;
                case "NotesPictureBox":
                    break;
                case "SiteDetailsPictureBox":
                    pic.Image = Resources.radio_tower_hover;
                    break;
                case "CalendarPictureBox":
                    pic.Image = Resources.calendar2_hover;
                    break;
            }
        }

        void TabControl1MouseDown(object sender, MouseEventArgs e)
        {
            if (shiftsCalendar != null)
            {
                if (shiftsCalendar.isVisible)
                    shiftsCalendar.toggleShiftsPanel();
            }
        }

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MINIMIZE = 0xF020;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MINIMIZE)
                        ScreenshotBeforeMinimize = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                    break;
            }
            base.WndProc(ref m);
        }
    }
}

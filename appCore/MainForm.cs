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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using appCore.DB;
using appCore.Logs;
using appCore.Settings;
using appCore.SiteFinder.UI;
using appCore.Templates;
using appCore.Templates.UI;
using appCore.Toolbox;
using appCore.UI;
using appCore.Shifts;
using appCore.OssScripts.UI;
using appCore.OI.JSON;
using Newtonsoft.Json;

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

        public MainForm(NotifyIcon tray, string[] args)
        {
            GlobalProperties.ApplicationStartTime = DateTime.Now;
            //args = new[] { "-otherUser", "SANTOSS2" }; // HACK: force login with another user
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            GlobalProperties.resolveOfficePath();

            SplashForm.ShowSplashScreen(true);
            trayIcon = new TrayIcon(tray);

            EmbeddedAssemblies.Init();

            SplashForm.UpdateLabelText("Getting network access");

            // Initialize Properties

            GlobalProperties.CheckShareAccess();

            SplashForm.UpdateLabelText("Setting User Folder");

            string otherUser = string.Empty;
            if (args.Contains("-otherUser"))
                try { otherUser = args[Array.FindIndex(args, str => str.Equals("-otherUser")) + 1].ToUpper(); } catch { }

            CurrentUser.InitializeUserProperties(otherUser);

            SplashForm.UpdateLabelText("Setting User Settings");

            logFiles.Initialize();

            SplashForm.UpdateLabelText("Loading UI");

            InitializeComponent();
            panel1.BackColor = CurrentUser.UserName == "GONCARJ3" ? Color.FromArgb(150, Color.LightGray) : Color.Transparent;
            if (!string.IsNullOrEmpty(otherUser))
            {
                Label otherUserLabel = new Label()
                {
                    Name = "otherUserLabel",
                    Text = "Logged on as " + otherUser,
                    Font = new Font("Courier New", 12F, FontStyle.Bold),
                    AutoSize = true
                };
                otherUserLabel.Location = new Point(0, (tabPage1.Height + tabPage1.Bounds.X) - otherUserLabel.Height);
                otherUserLabel.BackColor = Color.FromArgb(150, Color.LightGray);
                otherUserLabel.ForeColor = Color.Red;
                tabPage1.Controls.Add(otherUserLabel);
            }

            //			ToolsMenu toolsMenu = new ToolsMenu();
            //			toolsMenu.Location = new Point(tabPage1.Right - toolsMenu.Width, 0);
            //			tabPage1.Controls.Add(toolsMenu);

            panel1.Controls.Add(SiteDetailsPictureBox);
            // 
            // SiteDetailsPictureBox
            // 
            SiteDetailsPictureBox.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            SiteDetailsPictureBox.BackColor = Color.Transparent;
            SiteDetailsPictureBox.Image = Resources.radio_tower;
            SiteDetailsPictureBox.Location = new Point(6, 49);
            SiteDetailsPictureBox.Name = "pictureBox5";
            SiteDetailsPictureBox.Size = new Size(40, 40);
            SiteDetailsPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            SiteDetailsPictureBox.TabIndex = 8;
            SiteDetailsPictureBox.TabStop = false;
            SiteDetailsPictureBox.Click += PictureBoxesClick;
            SiteDetailsPictureBox.MouseLeave += PictureBoxesMouseLeave;
            SiteDetailsPictureBox.MouseHover += PictureBoxesMouseHover;

            tabPage1.Controls.Add(TicketCountLabel);
            // 
            // TicketCountLabel
            // 
            TicketCountLabel.BackColor = Color.Transparent;
            TicketCountLabel.Size = new Size(40, 20);
            TicketCountLabel.Location = new Point(tabPage1.Width - TicketCountLabel.Width - 5, tabPage1.Height - TicketCountLabel.Height - 5);
            TicketCountLabel.Name = "TicketCountLabel";
            TicketCountLabel.TabIndex = 5;
            TicketCountLabel.TextAlign = ContentAlignment.MiddleRight;
            TicketCountLabel.MouseClick += TicketCountLabelMouseClick;

            string img = SettingsFile.BackgroundImage;

            if (img != "Default")
            {
                if (File.Exists(img))
                    tabPage1.BackgroundImage = Image.FromFile(img);
                else
                    trayIcon.showBalloon("Image file not found", "Background Image file not found, applying default");
            }

            if (CurrentUser.UserName != "GONCARJ3")
            {
                tabControl1.TabPages.Remove(tabPage17); // new outage reports
                tabControl3.TabPages.Remove(tabPage14); // Alcatel scripts tab
            }

            TroubleshootUI.Location = new Point(1, 2);
            tabPage8.Controls.Add(TroubleshootUI);
            FailedCRQUI.Location = new Point(1, 2);
            tabPage10.Controls.Add(FailedCRQUI);
            UpdateUI.Location = new Point(1, 2);
            tabPage6.Controls.Add(UpdateUI);
            TxUI.Location = new Point(1, 2);
            tabPage9.Controls.Add(TxUI);

            nokiaScriptsControls.Location = new Point(1, 2);
            tabPage12.Controls.Add(nokiaScriptsControls);
            huaweiScriptsControls.Location = new Point(1, 2);
            tabPage11.Controls.Add(huaweiScriptsControls);
            ericssonScriptsControls.Location = new Point(1, 2);
            tabPage13.Controls.Add(ericssonScriptsControls);

            OutageUI.Location = new Point(1, 2);
            tabPage4.Controls.Add(OutageUI);

            SplashForm.UpdateLabelText("Loading Databases");

            Databases.PopulateDatabases();

            string closureCode = CurrentUser.ClosureCode;
            if (!string.IsNullOrEmpty(closureCode))
                comboBox1.Items.Add(closureCode);
            comboBox1.Items.Add("CBV");
            comboBox1.Text = closureCode;

            GlobalProperties.siteFinder_mainswitch = false;
            GlobalProperties.siteFinder_mainswitch = Databases.all_sites.Exists || Databases.all_cells.Exists;

            if ((CurrentUser.Department.Contains("1st Line RAN") || CurrentUser.Department.Contains("First Line Operations")) && Databases.shiftsFile.Exists)
            {
                string[] monthShifts = Databases.shiftsFile.GetAllShiftsInMonth(CurrentUser.FullName[1] + " " + CurrentUser.FullName[0], DateTime.Now.Month);
                
                pictureBox6.Visible = true;
                shiftsCalendar = new ShiftsCalendar();
                shiftsCalendar.Location = new Point((tabPage1.Width - shiftsCalendar.Width) / 2, 0 - shiftsCalendar.Height);
                tabPage1.Controls.Add(shiftsCalendar);
                //				}
            }

            // TODO: get sites list from alarms

            SplashForm.UpdateLabelText("Almost finished");

            // HACK: Developer specific action
            //if(CurrentUser.UserName == "GONCARJ3" || CurrentUser.UserName == "SANTOSS2") {
            if (CurrentUser.UserName == "GONCARJ3" || CurrentUser.Role == Roles.ShiftLeader)
            {
                Button butt2 = new Button();
                butt2.Name = "butt2";
                butt2.Text = "Update OI DB Files";
                butt2.Size = new Size(110, 23);
                butt2.Click += UpdateDbFilesButtonClick;
                tabPage1.Controls.Add(butt2);
                butt2.Location = new Point(5, tabPage1.Height - butt2.Height - 5);
                System.Timers.Timer OiDbFilesLastUpdatedTimer = new System.Timers.Timer(60 * 1000); // fires every 60 sec.
                OiDbFilesLastUpdatedTimer.Elapsed += OiDbFilesLastUpdatedTimer_Elapsed;

                OiDbFilesLastUpdatedTimer.Enabled = true;

                allCellsLabel.Location = new Point(5, tabPage1.Controls["butt2"].Top - 5 - allCellsLabel.Height);
                allSitesLabel.Location = new Point(5, allCellsLabel.Top - allSitesLabel.Height);
                allCellsLabel.Visible = true;
                allSitesLabel.Visible = true;

                OiDbFilesLastUpdatedTimer_Elapsed(null, null);

                if (CurrentUser.UserName == "GONCARJ3")
                {
                    Button butt = new Button();
                    butt.Name = "butt";
                    butt.Text = "Clear SitesDB";
                    butt.AutoSize = true;
                    butt.Location = new Point(butt2.Right + 10, butt2.Top);
                    butt.Click += delegate
                    {
                        SitesDB.Clear();

                        System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
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
                    tabPage1.Controls.Add(butt);

                    Button butt3 = new Button();
                    butt3.Name = "butt3";
                    butt3.Text = "Show SitesDB";
                    butt3.AutoSize = true;
                    butt3.Location = new Point(butt.Right + 10, butt.Top);
                    butt3.Click += delegate
                    {
                        Form form = new Form();
                        form.Size = new Size((int)(Screen.FromControl(this).Bounds.Width * 0.85), (int)(Screen.FromControl(this).Bounds.Height * 0.6));
                        form.Text = SitesDB.List.Count.ToString();
                        form.Text += SitesDB.List.Count > 1 ? " sites listed" : " site listed";
                        //						string str = null;
                        //						try {
                        //							var engine = new FileHelpers.FileHelperEngine<SiteFinder.Site>();
                        //							str = engine.WriteString(SitesDB.List);
                        //						}
                        //						catch(FileHelpers.FileHelpersException e) {
                        //							string f = e.Message;
                        //						}
                        //						System.Data.DataTable dt = null;
                        //						try {
                        //							var engine = new FileHelpers.FileHelperEngine<SiteFinder.Site>();
                        //							dt = engine.ReadStringAsDT(str);
                        //						}
                        //						catch(FileHelpers.FileHelpersException e) {
                        //							string f = e.Message;
                        //						}

                        DataGridView dgv = new DataGridView();
                        dgv.Dock = DockStyle.Fill;
                        dgv.RowHeadersVisible = false;
                        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                        dgv.DataSource = SitesDB.List;
                        //						dgv.DataSource = dt;

                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            if (col.Name == "KeyInformation" || col.Name == "HealthAndSafety" || col.Name == "Address")
                            {
                                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                                col.Width = col.Name == "Address" ? 100 : 300;
                                col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                            }
                        }

                        form.Controls.Add(dgv);
                        form.ShowDialog();
                    };
                    tabPage1.Controls.Add(butt3);

                    //					OutageUI.Location = new Point(1, 2);
                    //					tabPage17.Controls.Add(OutageUI);
                }
            }

            trayIcon.toggleShareAccess();

            toolTipDeploy();

            string thisfn = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\appCore.dll";

            SplashForm.CloseForm();

            if (SettingsFile.LastRunVersion != GlobalProperties.AssemblyFileVersionInfo.FileVersion)
            {
                SettingsFile.LastRunVersion = GlobalProperties.AssemblyFileVersionInfo.FileVersion;
                FlexibleMessageBox.Show(Resources.Changelog, "Changelog", MessageBoxButtons.OK);
            }
        }

        void OiDbFilesLastUpdatedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(InvokeRequired)
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
        }

        public void FillTemplateFromLog(Template log)
        {
            //			if(tabControl1.InvokeRequired)
            //				tabControl1.Invoke((MethodInvoker)delegate {
            //				                   	tabControl1.SelectTab(1);
            //				                   });
            //			else
            tabControl1.SelectTab(1);
            Action action = null;
            Control parent = null;
            switch (log.LogType)
            {
                case TemplateTypes.Troubleshoot:
                    tabControl2.SelectTab(0);
                    parent = tabPage8;
                    action = new Action(delegate
                    {
                        if (TroubleshootUI != null)
                            TroubleshootUI.Dispose();
                        TroubleshootUI = new TroubleshootControls(log.ToTroubleshootTemplate(), UiEnum.Template);
                        tabPage8.Controls.Add(TroubleshootUI);
                    });
                    break;
                case TemplateTypes.FailedCRQ:
                    tabControl2.SelectTab(1);
                    parent = tabPage10;
                    action = new Action(delegate
                    {
                        if (FailedCRQUI != null)
                            FailedCRQUI.Dispose();
                        FailedCRQUI = new FailedCRQControls(log.ToFailedCRQTemplate(), UiEnum.Template);
                        tabPage10.Controls.Add(FailedCRQUI);
                    });
                    break;
                case TemplateTypes.Update:
                    tabControl2.SelectTab(2);
                    parent = tabPage6;
                    action = new Action(delegate
                    {
                        if (UpdateUI != null)
                            UpdateUI.Dispose();
                        UpdateUI = new UpdateControls(log.ToUpdateTemplate(), UiEnum.Template);
                        tabPage6.Controls.Add(UpdateUI);
                    });
                    break;
                case TemplateTypes.TX:
                    tabControl2.SelectTab(3);
                    parent = tabPage9;
                    action = new Action(delegate
                    {
                        if (TxUI != null)
                            TxUI.Dispose();
                        TxUI = new TxControls(log.ToTxTemplate(), UiEnum.Template);
                        tabPage9.Controls.Add(TxUI);
                    });
                    break;
            }
            LoadingPanel loading = new LoadingPanel();
            loading.Show(action, parent);
            MainFormActivate(null, null);
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
            //			switch(e.Button) {
            //				case MouseButtons.Left:
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
            //					break;
            //				case MouseButtons.Right:
            //					TicketCountLabel.ForeColor = TicketCountLabel.ForeColor == Color.Black ? Color.White : Color.Black;
            //					break;
            //			}

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
                    Color back = ctrl.BackColor;
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

            toolTip.SetToolTip(pictureBox1, "Settings");
            toolTip.SetToolTip(pictureBox2, "AMT Browser");
            toolTip.SetToolTip(pictureBox3, "Notes");
            toolTip.SetToolTip(pictureBox4, "Log Browser");
            toolTip.SetToolTip(SiteDetailsPictureBox, "Site Finder");
            toolTip.SetToolTip(pictureBox6, "Shifts Calendar");
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
                    Action action = new Action(delegate
                    {
                        FlexibleMessageBox.Show("INC/CRQ can only contain numbers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox13.TextChanged -= TextBox13TextChanged;
                        textBox13.Text = "";
                        textBox13.TextChanged += TextBox13TextChanged;
                    });
                    LoadingPanel load = new LoadingPanel();
                    load.ShowAsync(null, action, false, this);
                }
            }
            else
            {
                label30.Visible = true;
                if (textBox13.Text.Length > 0 && textBox13.Text.Length < 15) label30.Text = "Press ENTER key to complete INC number";
                else
                {
                    if (textBox13.Text.Length != 15) label30.Text = "Insert INC/CRQ number";
                    else label30.Visible = false;
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
                    if (comboBox1.Text.Length < 3) label31.Visible = true;
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

        void Button6Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox12.Text))
            {
                Action action = new Action(delegate
                {
                    FlexibleMessageBox.Show("Please copy alarms from Netcool!", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
                LoadingPanel darken = new LoadingPanel();
                darken.Show(action, this);
                return;
            }

            bool parsingError = false;
            string alarms = textBox12.Text;
            Netcool.AlarmsParser netcool = null;
            Action actionThreaded = new Action(delegate
            {
                try
                {
                    var st = new Stopwatch();
                    st.Start();
                    netcool = new Netcool.AlarmsParser(alarms);
                    st.Stop();
                    var t = st.Elapsed;
                }
                catch (Exception ex)
                {
                    trayIcon.showBalloon("Error parsing alarms", "An error occurred while parsing the alarms.\n\nError message:\n" + ex.Message);
                    parsingError = true;
                }
            });

            Action actionNonThreaded = new Action(delegate
            {
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
            });
            LoadingPanel load = new LoadingPanel();
            load.ShowAsync(actionThreaded, actionNonThreaded, true, tabPage5);
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

        void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    TabControl2SelectedIndexChanged(null, null);
                    break;
                case 2:
                    textBox13.Focus();
                    break;
                case 3:
                    //					textBox10.Focus();
                    break;
                case 4:
                    textBox12.Focus();
                    break;
            }
        }

        void TabControl2SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl2.SelectedIndex)
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

        void TextBox13KeyPress(object sender, KeyPressEventArgs e)
        {

            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                if (textBox13.Text.Length > 0)
                {
                    string CompINC_CRQ = Tools.CompleteINC_CRQ_TAS(textBox13.Text, "INC");
                    if (CompINC_CRQ != "error") textBox13.Text = CompINC_CRQ;
                    else
                    {
                        Action action = new Action(delegate
                        {
                            FlexibleMessageBox.Show("INC number must only contain digits!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        });
                        LoadingPanel load = new LoadingPanel();
                        load.ShowAsync(null, action, false, this);
                    }
                }
            }
        }

        void TabPage1MouseClick(object sender, MouseEventArgs e)
        {
            //FIXME:			wholeShiftsPanelDispose();
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                contextMenuStrip1.Show(PointToScreen(e.Location));
            if (shiftsCalendar.isVisible)
                shiftsCalendar.toggleShiftsPanel();
        }

        void ToolStripMenuItem2Click(object sender, EventArgs e)
        {
            Action action = new Action(delegate
            {
                OpenFileDialog fileBrowser = new OpenFileDialog();
                fileBrowser.Filter = "All image files(*.bmp,*.gif,*.jpg,*.jpeg,*.png)|*.bmp;*.gif;*.jpg;*.jpeg;*.png";
                fileBrowser.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                fileBrowser.Title = "Please select a new background image";
                if (fileBrowser.ShowDialog() == DialogResult.OK)
                {
                    SettingsFile.BackgroundImage = fileBrowser.FileName;
                    tabPage1.BackgroundImage = Image.FromFile(fileBrowser.FileName);
                }
            });
            LoadingPanel load = new LoadingPanel();
            load.ShowAsync(null, action, false, this);
        }

        void ToolStripMenuItem3Click(object sender, EventArgs e)
        {
            SettingsFile.BackgroundImage = "Default";
            tabPage1.BackgroundImage = Resources.zoozoo_wallpaper_15;
        }

        void Button13Click(object sender, EventArgs e)
        {
            //			Action action = new Action(delegate {
            TroubleshootUI.ActiveAlarmsTextBox.Text = textBox12.Text;
            tabControl1.SelectTab(1);
            tabControl2.SelectTab(0);
            //			                           });
            //			LoadingPanel load = new LoadingPanel();
            //			load.Show(null, action, false, this);
        }

        void TextBox12TextChanged(object sender, EventArgs e)
        {
            if (textBox12.Text != string.Empty) button24.Enabled = true;
            else button24.Enabled = false;
        }

        void Button24Click(object sender, EventArgs e)
        {
            Action action = new Action(delegate
            {
                AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox12.Text, label34.Text, false);
                enlarge.StartPosition = FormStartPosition.CenterParent;
                enlarge.ShowDialog();
                textBox12.Text = enlarge.finaltext;
            });
            LoadingPanel load = new LoadingPanel();
            load.ShowAsync(null, action, false, this);
        }

        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult ans = DialogResult.No;
            Action action = new Action(delegate
            {
                ans = FlexibleMessageBox.Show("Are you sure you want to quit ANOC Master Tool?", "Quitting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            });
            LoadingPanel load = new LoadingPanel();
            load.Show(action, this);
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
        }

        void MainFormActivate(object sender, EventArgs e)
        {
            var form = Application.OpenForms.OfType<MainForm>().First();

            form.Activate();
            form.WindowState = FormWindowState.Normal;
        }

        public static void openSettings(Control callerControl, bool fromTrayIcon = false)
        {
            Action action = new Action(delegate
            {
                Settings.UI.SettingsForm settings = new Settings.UI.SettingsForm(GlobalProperties.siteFinder_mainswitch);
                settings.StartPosition = FormStartPosition.CenterParent;
                settings.ShowDialog();

                if (settings.siteFinder_newSwitch != GlobalProperties.siteFinder_mainswitch)
                    GlobalProperties.siteFinder_mainswitch = settings.siteFinder_newSwitch;

                //			                           	SetUserFolder(false);
            });
            if (!fromTrayIcon)
            {
                LoadingPanel load = new LoadingPanel();
                load.ShowAsync(null, action, false, callerControl);
            }
            else
                action();
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
            if (pic.Name != "pictureBox6")
            {
                if (shiftsCalendar.isVisible)
                    shiftsCalendar.toggleShiftsPanel();
            }
            switch (pic.Name)
            {
                case "pictureBox1":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    Action action = new Action(delegate
                    {
                        openSettings(this);
                    });
                    LoadingPanel load = new LoadingPanel();
                    load.ShowAsync(null, action, false, this);
                    break;
                case "pictureBox2":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openAMTBrowser();
                    break;
                case "pictureBox3":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openNotes();
                    break;
                case "pictureBox4":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openLogBrowser();
                    break;
                case "pictureBox5":
                    //					wholeShiftsPanelDispose();
                    MainFormActivate(null, null);
                    openSiteFinder();
                    break;
                case "pictureBox6":
                    shiftsCalendar.toggleShiftsPanel();
                    break;
            }
        }

        void PictureBoxesMouseLeave(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            switch (pic.Name)
            {
                case "pictureBox1":
                    pic.Image = Resources.Settings_normal;
                    break;
                case "pictureBox2":
                    pic.Image = Resources.globe;
                    break;
                case "pictureBox3":
                    pic.Image = Resources.Book_512;
                    break;
                case "pictureBox4":
                    break;
                case "pictureBox5":
                    pic.Image = Resources.radio_tower;
                    break;
                case "pictureBox6":
                    break;
            }
        }

        void PictureBoxesMouseHover(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            switch (pic.Name)
            {
                case "pictureBox1":
                    pic.Image = Resources.Settings_hover;
                    break;
                case "pictureBox2":
                    pic.Image = Resources.globe_hover;
                    break;
                case "pictureBox3":
                    break;
                case "pictureBox4":
                    break;
                case "pictureBox5":
                    pic.Image = Resources.radio_tower_hover;
                    break;
                case "pictureBox6":
                    break;
            }
        }

        void TabControl1MouseDown(object sender, MouseEventArgs e)
        {
            if (shiftsCalendar.isVisible)
                shiftsCalendar.toggleShiftsPanel();
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
/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 26-01-2015
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using appCore.UI;
using appCore.DB;
using appCore.permChecker;

namespace appCore.Settings.UI
{
    /// <summary>
    /// Description of SettingsForm.
    /// </summary>
    public partial class SettingsForm : Form
    {
        public bool siteFinder_newSwitch;

        public SettingsForm(bool siteFinder_mainswitch)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            // Permission check for user administration
            var permCheck = new permCheck();
            switch (permCheck.getUserPerm())
            {
                case 1: case 2: case 3: userAdminTab.Enabled = true; userAdminPanel.Visible = true; noPermPanel.Visible = true; break;
                default: userAdminTab.Enabled = false; userAdminPanel.Visible = false; noPermPanel.Visible = true; break;
            }


            // get appCore.dll file path

            var fnv = FileVersionInfo.GetVersionInfo(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\appCore.dll");

            label3.Text = "Version: " + fnv.FileVersion;
            label4.Text = "Release date: " + File.GetLastWriteTime(fnv.FileName).ToString("dd-MM-yyyy");

            textBox1.Text = SettingsFile.UserFolderPath.FullName;


            if (!CurrentUser.hasOICredentials)
            {
                DialogResult ans = MessageBox.Show("No OI credentials stored, do you want to store your logon credentials now?", "OI Login", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans == DialogResult.Yes)
                    Button4Click(null, null);
            }
            else label6.Text = SettingsFile.OIUsername;

            comboBox1.Text = siteFinder_mainswitch ? "Enabled" : "Disabled";
        }

        void Button1Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        void Button2Click(object sender, EventArgs e)
        {
            Action action = new Action(delegate
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && textBox1.Text != SettingsFile.UserFolderPath.FullName)
                {
                    DialogResult ans = FlexibleMessageBox.Show("User Folder will be changed to the following location:" + Environment.NewLine + Environment.NewLine + textBox1.Text + Environment.NewLine + Environment.NewLine + "Are you sure you want to apply this change?", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ans == DialogResult.Yes)
                    {
                        UserFolder.Change(new DirectoryInfo(textBox1.Text));
                        MainForm.UpdateTicketCountLabel();
                    }
                    this.Close();
                }
            });
            Toolbox.Tools.darkenBackgroundForm(action, true, this);
        }

        void Button3Click(object sender, EventArgs e)
        {
            Action action = new Action(delegate
            {
                Toolbox.ScrollableMessageBox msgBox = new Toolbox.ScrollableMessageBox();
                msgBox.StartPosition = FormStartPosition.CenterParent;
                msgBox.Show(Resources.Changelog, "Changelog", MessageBoxButtons.OK, "Changelog", false);
            });
            Toolbox.Tools.darkenBackgroundForm(action, false, this);
        }

        void Button4Click(object sender, EventArgs e)
        {
            Action action = new Action(delegate
            {
                Web.UI.AuthForm auth = new Web.UI.AuthForm("OI");
                auth.StartPosition = FormStartPosition.CenterParent;
                auth.ShowDialog();
                if (!string.IsNullOrEmpty(auth.Username) && !string.IsNullOrEmpty(auth.Password))
                {
                    SettingsFile.OIUsername = auth.Username;
                    SettingsFile.OIPassword = auth.Password;
                    label6.Text = auth.Username;
                }
            });
            Toolbox.Tools.darkenBackgroundForm(action, false, this);
        }

        void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (!string.IsNullOrEmpty(cb.Text))
                siteFinder_newSwitch = cb.Text == "Enabled";
        }

        void Button5Click(object sender, EventArgs e)
        {
            Action action = new Action(delegate
            {
                Databases.LoadDBFiles(null, null);
            });
            Toolbox.Tools.darkenBackgroundForm(action, true, this);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}

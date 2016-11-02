﻿/*
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
        private permCheck permCheck = new permCheck();
        public SettingsForm(bool siteFinder_mainswitch)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            // Permission check for user administration
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
            var users = permCheck.getUsers();
            for (int a = 0; a < permCheck.currMaxUser(); a++)
            {
                if (users[a, 0] != null)
                {
                    string[] newRow = new string[] { users[a, 0], users[a, 1], users[a, 2] };
                    switch (newRow[2])
                    {
                        case "o": newRow[2] = permCheck.getPermName(0); break;
                        case "1": newRow[2] = permCheck.getPermName(1); break;
                        case "2": newRow[2] = permCheck.getPermName(2); break;
                        case "3": newRow[2] = permCheck.getPermName(3); break;
                        case "4": newRow[2] = permCheck.getPermName(4); break;
                        case "5": newRow[2] = permCheck.getPermName(5); break;
                    }
                    dataGridView1.Rows.Add(newRow);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int perm = 0;
            switch (comboBox2.Text)
            {
                case "Shiftleader": perm = 3; break;
                case "1st Line": perm = 4; break;
                case "2nd Line": perm = 5; break;
            }
            permCheck.addUser(textBox2.Text, textBox3.Text, perm.ToString());
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            var users = permCheck.getUsers();
            for (int a = 0; a < permCheck.currMaxUser(); a++)
            {
                if (users[a, 0] != null)
                {
                    string[] newRow = new string[] { users[a, 0], users[a, 1], users[a, 2] };
                    switch (newRow[2])
                    {
                        case "o": newRow[2] = permCheck.getPermName(0); break;
                        case "1": newRow[2] = permCheck.getPermName(1); break;
                        case "2": newRow[2] = permCheck.getPermName(2); break;
                        case "3": newRow[2] = permCheck.getPermName(3); break;
                        case "4": newRow[2] = permCheck.getPermName(4); break;
                        case "5": newRow[2] = permCheck.getPermName(5); break;
                    }
                    dataGridView1.Rows.Add(newRow);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            if (!row.Cells[2].Value.ToString().Equals("root", StringComparison.InvariantCultureIgnoreCase))
            {
                textBox4.Text = row.Cells[0].Value.ToString() ?? string.Empty;
                textBox5.Text = row.Cells[1].Value.ToString() ?? string.Empty;
                switch (row.Cells[2].Value.ToString() ?? string.Empty)
                {
                    case "No permission": comboBox3.Text = permCheck.getPermName(0); break;
                    case "root": comboBox3.Text = permCheck.getPermName(1); break;
                    case "Manager": comboBox3.Text = permCheck.getPermName(2); break;
                    case "Shiftleader": comboBox3.Text = permCheck.getPermName(3); break;
                    case "1st Line": comboBox3.Text = permCheck.getPermName(4); break;
                    case "2nd Line": comboBox3.Text = permCheck.getPermName(5); break;
                    default: comboBox3.Text = "No permission"; break;
                }
            }
            else
            {
                MessageBox.Show("root user cannot be modified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Mod user in xml
            //..
            //..
            //..
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            var users = permCheck.getUsers();
            for (int a = 0; a < permCheck.currMaxUser(); a++)
            {
                if (users[a, 0] != null)
                {
                    string[] newRow = new string[] { users[a, 0], users[a, 1], users[a, 2] };
                    switch (newRow[2])
                    {
                        case "o": newRow[2] = permCheck.getPermName(0); break;
                        case "1": newRow[2] = permCheck.getPermName(1); break;
                        case "2": newRow[2] = permCheck.getPermName(2); break;
                        case "3": newRow[2] = permCheck.getPermName(3); break;
                        case "4": newRow[2] = permCheck.getPermName(4); break;
                        case "5": newRow[2] = permCheck.getPermName(5); break;
                    }
                    dataGridView1.Rows.Add(newRow);
                }
            }
        }
    }
}

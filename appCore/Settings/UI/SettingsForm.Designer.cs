/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 26-01-2015
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace appCore.Settings.UI
{
	partial class SettingsForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private appCore.UI.AMTTextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage GeneralTabPage;
		private System.Windows.Forms.TabPage AboutTabPage;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button button5;
        private JCS.ToggleSwitch toggleSwitch2;
        private System.Windows.Forms.Label label17;
        private JCS.ToggleSwitch toggleSwitch1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label7;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.textBox1 = new appCore.UI.AMTTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.GeneralTabPage = new System.Windows.Forms.TabPage();
            this.toggleSwitch3 = new JCS.ToggleSwitch();
            this.label9 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.toggleSwitch2 = new JCS.ToggleSwitch();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.toggleSwitch1 = new JCS.ToggleSwitch();
            this.button5 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.PermissionsTabPage = new System.Windows.Forms.TabPage();
            this.AboutTabPage = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.GeneralTabPage.SuspendLayout();
            this.AboutTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(98, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(348, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "User Folder Path";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.GeneralTabPage);
            this.tabControl1.Controls.Add(this.PermissionsTabPage);
            this.tabControl1.Controls.Add(this.AboutTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(496, 317);
            this.tabControl1.TabIndex = 0;
            // 
            // GeneralTabPage
            // 
            this.GeneralTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.GeneralTabPage.Controls.Add(this.toggleSwitch3);
            this.GeneralTabPage.Controls.Add(this.label9);
            this.GeneralTabPage.Controls.Add(this.button1);
            this.GeneralTabPage.Controls.Add(this.toggleSwitch2);
            this.GeneralTabPage.Controls.Add(this.label17);
            this.GeneralTabPage.Controls.Add(this.label16);
            this.GeneralTabPage.Controls.Add(this.label7);
            this.GeneralTabPage.Controls.Add(this.toggleSwitch1);
            this.GeneralTabPage.Controls.Add(this.button5);
            this.GeneralTabPage.Controls.Add(this.label8);
            this.GeneralTabPage.Controls.Add(this.label6);
            this.GeneralTabPage.Controls.Add(this.label5);
            this.GeneralTabPage.Controls.Add(this.button4);
            this.GeneralTabPage.Controls.Add(this.label1);
            this.GeneralTabPage.Controls.Add(this.textBox1);
            this.GeneralTabPage.Location = new System.Drawing.Point(4, 22);
            this.GeneralTabPage.Name = "GeneralTabPage";
            this.GeneralTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralTabPage.Size = new System.Drawing.Size(488, 291);
            this.GeneralTabPage.TabIndex = 0;
            this.GeneralTabPage.Text = "General";
            // 
            // toggleSwitch3
            // 
            this.toggleSwitch3.Location = new System.Drawing.Point(135, 162);
            this.toggleSwitch3.Name = "toggleSwitch3";
            this.toggleSwitch3.OffFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleSwitch3.OnFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleSwitch3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toggleSwitch3.Size = new System.Drawing.Size(50, 19);
            this.toggleSwitch3.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Fancy;
            this.toggleSwitch3.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(3, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(123, 15);
            this.label9.TabIndex = 18;
            this.label9.Text = "Show tips on Startup";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(452, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(29, 20);
            this.button1.TabIndex = 17;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // toggleSwitch2
            // 
            this.toggleSwitch2.Location = new System.Drawing.Point(135, 137);
            this.toggleSwitch2.Name = "toggleSwitch2";
            this.toggleSwitch2.OffFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleSwitch2.OnFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleSwitch2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toggleSwitch2.Size = new System.Drawing.Size(50, 19);
            this.toggleSwitch2.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Fancy;
            this.toggleSwitch2.TabIndex = 16;
            this.toggleSwitch2.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.ToggleSwitchesCheckedChanged);
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(3, 141);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(123, 15);
            this.label17.TabIndex = 15;
            this.label17.Text = "Weather Service";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Cursor = System.Windows.Forms.Cursors.Default;
            this.label16.Location = new System.Drawing.Point(3, 85);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(322, 15);
            this.label16.TabIndex = 14;
            this.label16.Text = "Current all_cells file last updated on: ";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Cursor = System.Windows.Forms.Cursors.Default;
            this.label7.Location = new System.Drawing.Point(3, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(322, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Current all_sites file last updated on: ";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toggleSwitch1
            // 
            this.toggleSwitch1.Location = new System.Drawing.Point(135, 112);
            this.toggleSwitch1.Name = "toggleSwitch1";
            this.toggleSwitch1.OffFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleSwitch1.OnFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleSwitch1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toggleSwitch1.Size = new System.Drawing.Size(50, 19);
            this.toggleSwitch1.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Fancy;
            this.toggleSwitch1.TabIndex = 12;
            this.toggleSwitch1.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.ToggleSwitchesCheckedChanged);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(383, 77);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(98, 23);
            this.button5.TabIndex = 11;
            this.button5.Text = "Reload DB Files";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.Button5Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(3, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(123, 15);
            this.label8.TabIndex = 9;
            this.label8.Text = "Site Finder";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(132, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 20);
            this.label6.TabIndex = 7;
            this.label6.Text = "Not configured";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "SiteLopedia Credentials";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(383, 32);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(98, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "Set/Change login";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4Click);
            // 
            // PermissionsTabPage
            // 
            this.PermissionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.PermissionsTabPage.Name = "PermissionsTabPage";
            this.PermissionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.PermissionsTabPage.Size = new System.Drawing.Size(488, 291);
            this.PermissionsTabPage.TabIndex = 2;
            this.PermissionsTabPage.Text = "Permissions Manager";
            this.PermissionsTabPage.UseVisualStyleBackColor = true;
            // 
            // AboutTabPage
            // 
            this.AboutTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.AboutTabPage.Controls.Add(this.button3);
            this.AboutTabPage.Controls.Add(this.label4);
            this.AboutTabPage.Controls.Add(this.label3);
            this.AboutTabPage.Controls.Add(this.label2);
            this.AboutTabPage.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.AboutTabPage.Location = new System.Drawing.Point(4, 22);
            this.AboutTabPage.Name = "AboutTabPage";
            this.AboutTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.AboutTabPage.Size = new System.Drawing.Size(488, 291);
            this.AboutTabPage.TabIndex = 1;
            this.AboutTabPage.Text = "About";
            // 
            // button3
            // 
            this.button3.AutoSize = true;
            this.button3.Location = new System.Drawing.Point(164, 244);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(195, 39);
            this.button3.TabIndex = 1;
            this.button3.Text = "View changelog";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Release date: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Version: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Pristina", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(80, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(349, 63);
            this.label2.TabIndex = 0;
            this.label2.Text = "ANOC Master Tool";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 317);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::appCore.UI.Resources.app_icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.tabControl1.ResumeLayout(false);
            this.GeneralTabPage.ResumeLayout(false);
            this.GeneralTabPage.PerformLayout();
            this.AboutTabPage.ResumeLayout(false);
            this.AboutTabPage.PerformLayout();
            this.ResumeLayout(false);

		}

        private System.Windows.Forms.TabPage PermissionsTabPage;
        private System.Windows.Forms.Button button1;
        private JCS.ToggleSwitch toggleSwitch3;
        private System.Windows.Forms.Label label9;
    }
}

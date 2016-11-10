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
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button button5;
		
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.button5 = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.button4 = new System.Windows.Forms.Button();
			this.userAdminTab = new System.Windows.Forms.TabPage();
			this.noPermPanel = new System.Windows.Forms.Panel();
			this.userAdminPanel = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.button7 = new System.Windows.Forms.Button();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button6 = new System.Windows.Forms.Button();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.AgentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Username = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Permission = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label9 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.button3 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.userAdminTab.SuspendLayout();
			this.noPermPanel.SuspendLayout();
			this.userAdminPanel.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.tabPage2.SuspendLayout();
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
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(452, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(29, 20);
			this.button1.TabIndex = 2;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
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
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(395, 98);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(86, 23);
			this.button2.TabIndex = 3;
			this.button2.Text = "Save Changes";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.Button2Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(514, 317);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage1.Controls.Add(this.button5);
			this.tabPage1.Controls.Add(this.comboBox1);
			this.tabPage1.Controls.Add(this.label8);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.button4);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.button2);
			this.tabPage1.Controls.Add(this.textBox1);
			this.tabPage1.Controls.Add(this.button1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage1.Size = new System.Drawing.Size(506, 291);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(383, 63);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(98, 23);
			this.button5.TabIndex = 11;
			this.button5.Text = "Reload DB Files";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.Button5Click);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
			"Enabled",
			"Disabled"});
			this.comboBox1.Location = new System.Drawing.Point(68, 61);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(118, 21);
			this.comboBox1.TabIndex = 10;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 63);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(59, 15);
			this.label8.TabIndex = 9;
			this.label8.Text = "Site Finder";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(210, 32);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(167, 20);
			this.label7.TabIndex = 8;
			this.label7.Text = "User Folder Path";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(106, 32);
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
			this.label5.Size = new System.Drawing.Size(97, 20);
			this.label5.TabIndex = 6;
			this.label5.Text = "SiteLopedia Login";
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
			// userAdminTab
			// 
			this.userAdminTab.Controls.Add(this.noPermPanel);
			this.userAdminTab.Location = new System.Drawing.Point(4, 22);
			this.userAdminTab.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.userAdminTab.Name = "userAdminTab";
			this.userAdminTab.Size = new System.Drawing.Size(506, 291);
			this.userAdminTab.TabIndex = 2;
			this.userAdminTab.Text = "User Administration";
			this.userAdminTab.UseVisualStyleBackColor = true;
			// 
			// noPermPanel
			// 
			this.noPermPanel.Controls.Add(this.userAdminPanel);
			this.noPermPanel.Controls.Add(this.label9);
			this.noPermPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.noPermPanel.Location = new System.Drawing.Point(0, 0);
			this.noPermPanel.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.noPermPanel.Name = "noPermPanel";
			this.noPermPanel.Size = new System.Drawing.Size(506, 291);
			this.noPermPanel.TabIndex = 0;
			this.noPermPanel.Visible = false;
			// 
			// userAdminPanel
			// 
			this.userAdminPanel.Controls.Add(this.groupBox2);
			this.userAdminPanel.Controls.Add(this.groupBox1);
			this.userAdminPanel.Controls.Add(this.dataGridView1);
			this.userAdminPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userAdminPanel.Location = new System.Drawing.Point(0, 0);
			this.userAdminPanel.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.userAdminPanel.Name = "userAdminPanel";
			this.userAdminPanel.Size = new System.Drawing.Size(506, 291);
			this.userAdminPanel.TabIndex = 1;
			this.userAdminPanel.Visible = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.button7);
			this.groupBox2.Controls.Add(this.comboBox3);
			this.groupBox2.Controls.Add(this.textBox4);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.label15);
			this.groupBox2.Controls.Add(this.textBox5);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBox2.Location = new System.Drawing.Point(227, 142);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.groupBox2.Size = new System.Drawing.Size(279, 149);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Modify User";
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(210, 22);
			this.button7.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(51, 64);
			this.button7.TabIndex = 13;
			this.button7.Text = "Modify";
			this.button7.UseVisualStyleBackColor = true;
			// 
			// comboBox3
			// 
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[] {
			"Shiftleader",
			"1st Line",
			"2nd Line"});
			this.comboBox3.Location = new System.Drawing.Point(68, 70);
			this.comboBox3.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(138, 21);
			this.comboBox3.TabIndex = 12;
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(68, 22);
			this.textBox4.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(138, 20);
			this.textBox4.TabIndex = 11;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(7, 71);
			this.label13.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(63, 13);
			this.label13.TabIndex = 10;
			this.label13.Text = "Permission: ";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(7, 47);
			this.label14.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(58, 13);
			this.label14.TabIndex = 9;
			this.label14.Text = "Username:";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(7, 23);
			this.label15.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(38, 13);
			this.label15.TabIndex = 8;
			this.label15.Text = "Name:";
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(68, 45);
			this.textBox5.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(138, 20);
			this.textBox5.TabIndex = 7;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button6);
			this.groupBox1.Controls.Add(this.comboBox2);
			this.groupBox1.Controls.Add(this.textBox3);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.textBox2);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox1.Location = new System.Drawing.Point(0, 142);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.groupBox1.Size = new System.Drawing.Size(275, 149);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Add User";
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(210, 21);
			this.button6.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(51, 64);
			this.button6.TabIndex = 6;
			this.button6.Text = "Add";
			this.button6.UseVisualStyleBackColor = true;
			// 
			// comboBox2
			// 
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
			"Shiftleader",
			"1st Line",
			"2nd Line"});
			this.comboBox2.Location = new System.Drawing.Point(72, 68);
			this.comboBox2.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(138, 21);
			this.comboBox2.TabIndex = 5;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(72, 45);
			this.textBox3.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(138, 20);
			this.textBox3.TabIndex = 4;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(7, 70);
			this.label12.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(63, 13);
			this.label12.TabIndex = 3;
			this.label12.Text = "Permission: ";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(7, 46);
			this.label11.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(58, 13);
			this.label11.TabIndex = 2;
			this.label11.Text = "Username:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(7, 22);
			this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(38, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "Name:";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(72, 21);
			this.textBox2.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(138, 20);
			this.textBox2.TabIndex = 0;
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
			this.AgentName,
			this.Username,
			this.Permission});
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
			this.dataGridView1.Location = new System.Drawing.Point(0, 0);
			this.dataGridView1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 37;
			this.dataGridView1.Size = new System.Drawing.Size(506, 142);
			this.dataGridView1.TabIndex = 0;
			// 
			// AgentName
			// 
			this.AgentName.HeaderText = "Name";
			this.AgentName.Name = "Name";
			this.AgentName.ReadOnly = true;
			this.AgentName.Width = 150;
			// 
			// Username
			// 
			this.Username.HeaderText = "Username";
			this.Username.Name = "Username";
			this.Username.ReadOnly = true;
			this.Username.Width = 150;
			// 
			// Permission
			// 
			this.Permission.HeaderText = "Permission";
			this.Permission.Name = "Permission";
			this.Permission.Width = 150;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.ForeColor = System.Drawing.Color.Red;
			this.label9.Location = new System.Drawing.Point(7, 6);
			this.label9.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(99, 13);
			this.label9.TabIndex = 0;
			this.label9.Text = "User not authorized";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage2.Controls.Add(this.button3);
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage2.Size = new System.Drawing.Size(556, 363);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "About";
			// 
			// button3
			// 
			this.button3.AutoSize = true;
			this.button3.Location = new System.Drawing.Point(388, 128);
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
			this.label2.Location = new System.Drawing.Point(132, 3);
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
			this.ClientSize = new System.Drawing.Size(514, 317);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = global::appCore.UI.Resources.MB_0001_vodafone3;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.Text = "Settings";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.userAdminTab.ResumeLayout(false);
			this.noPermPanel.ResumeLayout(false);
			this.noPermPanel.PerformLayout();
			this.userAdminPanel.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);

		}

        private System.Windows.Forms.TabPage userAdminTab;
        private System.Windows.Forms.Panel noPermPanel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel userAdminPanel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgentName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Username;
        private System.Windows.Forms.DataGridViewTextBoxColumn Permission;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox5;
    }
}

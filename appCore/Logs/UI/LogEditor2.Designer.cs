/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 20/03/2015
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace appCore.Logs.UI
{
	partial class LogEditor2
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Label label32;
		private appCore.UI.AMTRichTextBox textBox11;
		private appCore.UI.AMTRichTextBox textBox10;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button14;
		private System.Windows.Forms.Button button10;
		private System.Windows.Forms.Button button11;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button button12;
		
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
			this.listView1 = new System.Windows.Forms.ListView();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.button1 = new System.Windows.Forms.Button();
			this.button14 = new System.Windows.Forms.Button();
			this.label33 = new System.Windows.Forms.Label();
			this.label32 = new System.Windows.Forms.Label();
			this.textBox11 = new appCore.UI.AMTRichTextBox();
			this.textBox10 = new appCore.UI.AMTRichTextBox();
			this.button10 = new System.Windows.Forms.Button();
			this.button11 = new System.Windows.Forms.Button();
			this.button12 = new System.Windows.Forms.Button();
			this.groupBox7.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.listView1.AutoArrange = false;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(7, 7);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(510, 112);
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.ListView1SelectedIndexChanged);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.tabControl1);
			this.groupBox7.Controls.Add(this.button1);
			this.groupBox7.Controls.Add(this.button14);
			this.groupBox7.Controls.Add(this.label33);
			this.groupBox7.Controls.Add(this.label32);
			this.groupBox7.Controls.Add(this.textBox11);
			this.groupBox7.Controls.Add(this.textBox10);
			this.groupBox7.Location = new System.Drawing.Point(7, 127);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(526, 574);
			this.groupBox7.TabIndex = 73;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Outages Logs";
			this.groupBox7.Visible = false;
			// 
			// tabControl1
			// 
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(359, 15);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(127, 22);
			this.tabControl1.TabIndex = 36;
			this.tabControl1.Visible = false;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl4SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(119, 0);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "VF Report";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(119, 0);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "TF Report";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(492, 386);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(24, 20);
			this.button1.TabIndex = 4;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// button14
			// 
			this.button14.Enabled = false;
			this.button14.Location = new System.Drawing.Point(492, 15);
			this.button14.Name = "button14";
			this.button14.Size = new System.Drawing.Size(24, 21);
			this.button14.TabIndex = 2;
			this.button14.Text = "...";
			this.button14.UseVisualStyleBackColor = true;
			this.button14.Click += new System.EventHandler(this.Button14Click);
			// 
			// label33
			// 
			this.label33.Location = new System.Drawing.Point(8, 15);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(179, 20);
			this.label33.TabIndex = 34;
			this.label33.Text = "Outage Report";
			this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label32
			// 
			this.label32.Location = new System.Drawing.Point(8, 385);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(198, 20);
			this.label32.TabIndex = 33;
			this.label32.Text = "BulkCI (Divided into 50 sites chunks)";
			this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox11
			// 
			this.textBox11.DetectUrls = false;
			this.textBox11.Location = new System.Drawing.Point(8, 408);
			this.textBox11.Name = "textBox11";
			this.textBox11.ReadOnly = true;
			this.textBox11.Size = new System.Drawing.Size(508, 160);
			this.textBox11.TabIndex = 3;
			this.textBox11.Text = "";
			this.textBox11.TextChanged += new System.EventHandler(this.TextBox11TextChanged);
			// 
			// textBox10
			// 
			this.textBox10.DetectUrls = false;
			this.textBox10.Location = new System.Drawing.Point(8, 38);
			this.textBox10.Name = "textBox10";
			this.textBox10.ReadOnly = true;
			this.textBox10.Size = new System.Drawing.Size(508, 342);
			this.textBox10.TabIndex = 1;
			this.textBox10.Text = "";
			this.textBox10.WordWrap = false;
			this.textBox10.TextChanged += new System.EventHandler(this.TextBox10TextChanged);
			// 
			// button10
			// 
			this.button10.Location = new System.Drawing.Point(7, 707);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(112, 23);
			this.button10.TabIndex = 78;
			this.button10.Text = "Copy template";
			this.button10.UseVisualStyleBackColor = true;
			this.button10.Visible = false;
			this.button10.Click += new System.EventHandler(this.Button10Click);
			// 
			// button11
			// 
			this.button11.Location = new System.Drawing.Point(350, 707);
			this.button11.Name = "button11";
			this.button11.Size = new System.Drawing.Size(183, 23);
			this.button11.TabIndex = 79;
			this.button11.Text = "Copy to new Troubleshoot template";
			this.button11.UseVisualStyleBackColor = true;
			this.button11.Visible = false;
			this.button11.Click += new System.EventHandler(this.Button11Click);
			// 
			// button12
			// 
			this.button12.Location = new System.Drawing.Point(125, 707);
			this.button12.Name = "button12";
			this.button12.Size = new System.Drawing.Size(112, 23);
			this.button12.TabIndex = 79;
			this.button12.Text = "Outage Follow Up";
			this.button12.UseVisualStyleBackColor = true;
			this.button12.Visible = false;
			this.button12.Click += new System.EventHandler(this.Button12Click);
			// 
			// LogEditor2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(524, 733);
			this.Controls.Add(this.button12);
			this.Controls.Add(this.button11);
			this.Controls.Add(this.button10);
			this.Controls.Add(this.groupBox7);
			this.Controls.Add(this.listView1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = global::appCore.UI.Resources.MB_0001_vodafone3;
			this.MaximizeBox = false;
			this.Name = "LogEditor2";
			this.Text = "LogEditor";
			this.Activated += new System.EventHandler(this.LogEditorActivated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogEditorFormClosing);
			this.Resize += new System.EventHandler(this.Form_Resize);
			this.groupBox7.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}


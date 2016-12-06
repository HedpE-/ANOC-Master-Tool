﻿/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 03/12/2016
 * Time: 17:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace appCore.SiteFinder.UI
{
	partial class LockUnlockCellsForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader Tech;
		private System.Windows.Forms.ColumnHeader CellName;
		private System.Windows.Forms.ColumnHeader OssId;
		private System.Windows.Forms.ColumnHeader Switch;
		private System.Windows.Forms.ColumnHeader Vendor;
		private System.Windows.Forms.ColumnHeader NOC;
		private System.Windows.Forms.ColumnHeader Locked;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.Label label1;
		private appCore.UI.AMTRichTextBox amtRichTextBox1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		
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
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.listView1 = new System.Windows.Forms.ListView();
			this.Tech = new System.Windows.Forms.ColumnHeader();
			this.CellName = new System.Windows.Forms.ColumnHeader();
			this.Switch = new System.Windows.Forms.ColumnHeader();
			this.OssId = new System.Windows.Forms.ColumnHeader();
			this.Vendor = new System.Windows.Forms.ColumnHeader();
			this.NOC = new System.Windows.Forms.ColumnHeader();
			this.Locked = new System.Windows.Forms.ColumnHeader();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.amtRichTextBox1 = new appCore.UI.AMTRichTextBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// radioButton2
			// 
			this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton2.Location = new System.Drawing.Point(88, 2);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(76, 24);
			this.radioButton2.TabIndex = 34;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "Unlock Cells";
			this.radioButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.RadioButtonsCheckedChanged);
			// 
			// radioButton1
			// 
			this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton1.Location = new System.Drawing.Point(6, 2);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(76, 24);
			this.radioButton1.TabIndex = 35;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Lock Cells";
			this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.RadioButtonsCheckedChanged);
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.Tech,
			this.CellName,
			this.Switch,
			this.OssId,
			this.Vendor,
			this.NOC,
			this.Locked});
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(6, 32);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(437, 364);
			this.listView1.TabIndex = 36;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView1_ItemCheck);
			this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListView1ItemChecked);
			// 
			// Tech
			// 
			this.Tech.Text = "Tech";
			this.Tech.Width = 38;
			// 
			// CellName
			// 
			this.CellName.Text = "Cell Name";
			// 
			// Switch
			// 
			this.Switch.Text = "Switch";
			this.Switch.Width = 45;
			// 
			// OssId
			// 
			this.OssId.Text = "OSS ID";
			this.OssId.Width = 51;
			// 
			// Vendor
			// 
			this.Vendor.Text = "Vendor";
			this.Vendor.Width = 49;
			// 
			// NOC
			// 
			this.NOC.Text = "NOC";
			this.NOC.Width = 39;
			// 
			// Locked
			// 
			this.Locked.Text = "Locked";
			this.Locked.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Locked.Width = 49;
			// 
			// checkBox1
			// 
			this.checkBox1.Enabled = false;
			this.checkBox1.Location = new System.Drawing.Point(449, 53);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(67, 24);
			this.checkBox1.TabIndex = 37;
			this.checkBox1.Text = "2G";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBoxesCheckedChanged);
			// 
			// checkBox2
			// 
			this.checkBox2.Enabled = false;
			this.checkBox2.Location = new System.Drawing.Point(449, 83);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(67, 24);
			this.checkBox2.TabIndex = 38;
			this.checkBox2.Text = "3G";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBoxesCheckedChanged);
			// 
			// checkBox3
			// 
			this.checkBox3.Enabled = false;
			this.checkBox3.Location = new System.Drawing.Point(449, 113);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(67, 24);
			this.checkBox3.TabIndex = 39;
			this.checkBox3.Text = "4G";
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.CheckBoxesCheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(449, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 18);
			this.label1.TabIndex = 40;
			this.label1.Text = "Select Cells";
			// 
			// amtRichTextBox1
			// 
			this.amtRichTextBox1.Enabled = false;
			this.amtRichTextBox1.Location = new System.Drawing.Point(522, 113);
			this.amtRichTextBox1.Name = "amtRichTextBox1";
			this.amtRichTextBox1.Size = new System.Drawing.Size(254, 283);
			this.amtRichTextBox1.TabIndex = 41;
			this.amtRichTextBox1.Text = "";
			this.amtRichTextBox1.EnabledChanged += new System.EventHandler(this.AmtRichTextBox1EnabledChanged);
			this.amtRichTextBox1.TextChanged += new System.EventHandler(this.AmtRichTextBox1TextChanged);
			// 
			// comboBox1
			// 
			this.comboBox1.Enabled = false;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(522, 53);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(254, 21);
			this.comboBox1.TabIndex = 42;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1TextUpdate);
			this.comboBox1.TextUpdate += new System.EventHandler(this.ComboBox1TextUpdate);
			this.comboBox1.EnabledChanged += new System.EventHandler(this.ComboBox1EnabledChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(522, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 18);
			this.label2.TabIndex = 43;
			this.label2.Text = "Reference";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(522, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 18);
			this.label3.TabIndex = 44;
			this.label3.Text = "Observations";
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(449, 143);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(67, 51);
			this.button1.TabIndex = 45;
			this.button1.Text = "Unlock Cells";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// LockUnlockCellsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(783, 403);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.amtRichTextBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBox3);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.radioButton2);
			this.Name = "LockUnlockCellsForm";
			this.Text = "LockUnlockCellsForm";
			this.ResumeLayout(false);

		}
	}
}

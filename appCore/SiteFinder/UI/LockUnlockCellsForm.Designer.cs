/*
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
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.Label label1;
		private appCore.UI.AMTRichTextBox amtRichTextBox1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label label4;
		
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.amtRichTextBox1 = new appCore.UI.AMTRichTextBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
			// checkBox1
			// 
			this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox1.Enabled = false;
			this.checkBox1.Location = new System.Drawing.Point(807, 53);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(67, 24);
			this.checkBox1.TabIndex = 37;
			this.checkBox1.Text = "2G";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBoxesCheckedChanged);
			// 
			// checkBox2
			// 
			this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox2.Enabled = false;
			this.checkBox2.Location = new System.Drawing.Point(807, 83);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(67, 24);
			this.checkBox2.TabIndex = 38;
			this.checkBox2.Text = "3G";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBoxesCheckedChanged);
			// 
			// checkBox3
			// 
			this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox3.Enabled = false;
			this.checkBox3.Location = new System.Drawing.Point(807, 113);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(67, 24);
			this.checkBox3.TabIndex = 39;
			this.checkBox3.Text = "4G";
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.CheckBoxesCheckedChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(807, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 18);
			this.label1.TabIndex = 40;
			this.label1.Text = "Select Cells";
			// 
			// amtRichTextBox1
			// 
			this.amtRichTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.amtRichTextBox1.Enabled = false;
			this.amtRichTextBox1.Location = new System.Drawing.Point(880, 113);
			this.amtRichTextBox1.Name = "amtRichTextBox1";
			this.amtRichTextBox1.Size = new System.Drawing.Size(262, 296);
			this.amtRichTextBox1.TabIndex = 41;
			this.amtRichTextBox1.Text = "";
			this.amtRichTextBox1.EnabledChanged += new System.EventHandler(this.AmtRichTextBox1EnabledChanged);
			this.amtRichTextBox1.TextChanged += new System.EventHandler(this.AmtRichTextBox1TextChanged);
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.Enabled = false;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(880, 53);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(262, 21);
			this.comboBox1.TabIndex = 42;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1TextUpdate);
			this.comboBox1.TextUpdate += new System.EventHandler(this.ComboBox1TextUpdate);
			this.comboBox1.EnabledChanged += new System.EventHandler(this.ComboBox1EnabledChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(880, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 18);
			this.label2.TabIndex = 43;
			this.label2.Text = "Reference";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(880, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 18);
			this.label3.TabIndex = 44;
			this.label3.Text = "Observations";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(807, 143);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(67, 51);
			this.button1.TabIndex = 45;
			this.button1.Text = "Unlock Cells";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// radioButton3
			// 
			this.radioButton3.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton3.Location = new System.Drawing.Point(170, 2);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(76, 24);
			this.radioButton3.TabIndex = 46;
			this.radioButton3.TabStop = true;
			this.radioButton3.Text = "History";
			this.radioButton3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.CheckedChanged += new System.EventHandler(this.RadioButtonsCheckedChanged);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeColumns = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.dataGridView1.Location = new System.Drawing.Point(6, 32);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridView1.Size = new System.Drawing.Size(795, 377);
			this.dataGridView1.TabIndex = 47;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1CellContentClick);
			this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
			this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1CellValueChanged);
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.Red;
			this.label4.Location = new System.Drawing.Point(252, 3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(549, 23);
			this.label4.TabIndex = 48;
			this.label4.Text = "CAUTION!! No valid Book In";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.label4.Visible = false;
			// 
			// LockUnlockCellsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1148, 415);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.radioButton3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.amtRichTextBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBox3);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.radioButton2);
			this.Icon = global::appCore.UI.Resources.MB_0001_vodafone3;
			this.MinimumSize = new System.Drawing.Size(1164, 442);
			this.Name = "LockUnlockCellsForm";
			this.Text = "LockUnlockCellsForm";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);

		}
	}
}

/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 04-12-2015
 * Time: 22:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace appCore.SiteFinder.UI
{
	partial class siteDetails2
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.RichTextBox textBox4;
		private System.Windows.Forms.Button button45;
		private System.Windows.Forms.Label label56;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBox8;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private appCore.SiteFinder.UI.CellDetailsPictureBox pictureBox1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBox9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textBox10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.Label label12;
		
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(siteDetails2));
			this.textBox4 = new System.Windows.Forms.RichTextBox();
			this.button45 = new System.Windows.Forms.Button();
			this.label56 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBox8 = new System.Windows.Forms.TextBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox7 = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new appCore.SiteFinder.UI.CellDetailsPictureBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textBox9 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textBox10 = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.listView2 = new System.Windows.Forms.ListView();
			this.label12 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox4
			// 
			this.textBox4.DetectUrls = false;
			this.textBox4.Location = new System.Drawing.Point(87, 57);
			this.textBox4.Name = "textBox4";
			this.textBox4.ReadOnly = true;
			this.textBox4.Size = new System.Drawing.Size(250, 46);
			this.textBox4.TabIndex = 85;
			this.textBox4.Text = "";
			this.textBox4.TextChanged += new System.EventHandler(this.TextBox4TextChanged);
			// 
			// button45
			// 
			this.button45.Enabled = false;
			this.button45.Location = new System.Drawing.Point(343, 57);
			this.button45.Name = "button45";
			this.button45.Size = new System.Drawing.Size(24, 20);
			this.button45.TabIndex = 84;
			this.button45.Text = "...";
			this.button45.UseVisualStyleBackColor = true;
			this.button45.Click += new System.EventHandler(this.Button45Click);
			// 
			// label56
			// 
			this.label56.Location = new System.Drawing.Point(5, 107);
			this.label56.Name = "label56";
			this.label56.Size = new System.Drawing.Size(84, 20);
			this.label56.TabIndex = 83;
			this.label56.Text = "Power Company";
			this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(86, 107);
			this.textBox2.MaxLength = 5;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(280, 20);
			this.textBox2.TabIndex = 82;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 57);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 45);
			this.label4.TabIndex = 81;
			this.label4.Text = "Address";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 20);
			this.label2.TabIndex = 80;
			this.label2.Text = "Site ID";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(87, 30);
			this.textBox1.MaxLength = 6;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(53, 20);
			this.textBox1.TabIndex = 79;
			this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.siteFinder);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(146, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 20);
			this.label1.TabIndex = 87;
			this.label1.Text = "JVCO ID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(199, 30);
			this.textBox3.MaxLength = 6;
			this.textBox3.Name = "textBox3";
			this.textBox3.ReadOnly = true;
			this.textBox3.Size = new System.Drawing.Size(70, 20);
			this.textBox3.TabIndex = 86;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(373, 30);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(42, 20);
			this.label6.TabIndex = 90;
			this.label6.Text = "Area";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(421, 30);
			this.textBox5.MaxLength = 5;
			this.textBox5.Name = "textBox5";
			this.textBox5.ReadOnly = true;
			this.textBox5.Size = new System.Drawing.Size(140, 20);
			this.textBox5.TabIndex = 89;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(373, 82);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(42, 20);
			this.label7.TabIndex = 92;
			this.label7.Text = "Host";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox8
			// 
			this.textBox8.Location = new System.Drawing.Point(421, 82);
			this.textBox8.MaxLength = 5;
			this.textBox8.Name = "textBox8";
			this.textBox8.ReadOnly = true;
			this.textBox8.Size = new System.Drawing.Size(34, 20);
			this.textBox8.TabIndex = 91;
			// 
			// listView1
			// 
			this.listView1.AutoArrange = false;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.Location = new System.Drawing.Point(5, 247);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(556, 398);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView1KeyDown);
			// 
			// checkBox1
			// 
			this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox1.Location = new System.Drawing.Point(220, 230);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(38, 18);
			this.checkBox1.TabIndex = 88;
			this.checkBox1.Text = "2G";
			this.checkBox1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.listboxFilter_Changed);
			// 
			// checkBox2
			// 
			this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox2.Location = new System.Drawing.Point(258, 230);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(38, 18);
			this.checkBox2.TabIndex = 89;
			this.checkBox2.Text = "3G";
			this.checkBox2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.listboxFilter_Changed);
			// 
			// checkBox3
			// 
			this.checkBox3.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox3.Checked = true;
			this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox3.Location = new System.Drawing.Point(296, 230);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(38, 18);
			this.checkBox3.TabIndex = 90;
			this.checkBox3.Text = "4G";
			this.checkBox3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.listboxFilter_Changed);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(373, 57);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(42, 20);
			this.label3.TabIndex = 94;
			this.label3.Text = "Region";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox6
			// 
			this.textBox6.Location = new System.Drawing.Point(421, 57);
			this.textBox6.MaxLength = 5;
			this.textBox6.Name = "textBox6";
			this.textBox6.ReadOnly = true;
			this.textBox6.Size = new System.Drawing.Size(140, 20);
			this.textBox6.TabIndex = 93;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(457, 82);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 20);
			this.label5.TabIndex = 96;
			this.label5.Text = "Host Site";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox7
			// 
			this.textBox7.Location = new System.Drawing.Point(508, 82);
			this.textBox7.MaxLength = 5;
			this.textBox7.Name = "textBox7";
			this.textBox7.ReadOnly = true;
			this.textBox7.Size = new System.Drawing.Size(53, 20);
			this.textBox7.TabIndex = 95;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(84, 141);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(403, 75);
			this.pictureBox1.TabIndex = 98;
			this.pictureBox1.TabStop = false;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(275, 30);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(38, 20);
			this.label9.TabIndex = 110;
			this.label9.Text = "Priority";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox9
			// 
			this.textBox9.Location = new System.Drawing.Point(319, 30);
			this.textBox9.MaxLength = 6;
			this.textBox9.Name = "textBox9";
			this.textBox9.ReadOnly = true;
			this.textBox9.Size = new System.Drawing.Size(48, 20);
			this.textBox9.TabIndex = 109;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(374, 107);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(41, 20);
			this.label10.TabIndex = 112;
			this.label10.Text = "Shared";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox10
			// 
			this.textBox10.Location = new System.Drawing.Point(421, 107);
			this.textBox10.MaxLength = 5;
			this.textBox10.Name = "textBox10";
			this.textBox10.ReadOnly = true;
			this.textBox10.Size = new System.Drawing.Size(140, 20);
			this.textBox10.TabIndex = 113;
			// 
			// label11
			// 
			this.label11.BackColor = System.Drawing.SystemColors.Control;
			this.label11.Location = new System.Drawing.Point(5, 227);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(158, 20);
			this.label11.TabIndex = 116;
			this.label11.Text = "Sites Information";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label11.Visible = false;
			// 
			// listView2
			// 
			this.listView2.AutoArrange = false;
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView2.Location = new System.Drawing.Point(5, 247);
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(556, 102);
			this.listView2.TabIndex = 115;
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.Visible = false;
			this.listView2.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView2ItemSelectionChanged);
			// 
			// label12
			// 
			this.label12.BackColor = System.Drawing.SystemColors.Control;
			this.label12.Location = new System.Drawing.Point(5, 227);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(85, 20);
			this.label12.TabIndex = 117;
			this.label12.Text = "Cell Information";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// siteDetails2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(952, 651);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.listView2);
			this.Controls.Add(this.textBox7);
			this.Controls.Add(this.textBox8);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textBox10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textBox9);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBox6);
			this.Controls.Add(this.checkBox3);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textBox5);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.button45);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label56);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label12);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::appCore.UI.Resources.MB_0001_vodafone3;
			this.MaximizeBox = false;
			this.Name = "siteDetails2";
			this.Text = "Site Details";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}

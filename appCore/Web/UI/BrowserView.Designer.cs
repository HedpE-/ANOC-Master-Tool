/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-02-2015
 * Time: 18:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace appCore.Web.UI
{
	partial class BrowserView
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.WebBrowser webBrowser1;
		private AMTBrowser webBrowser2;
		private AMTBrowser webBrowser3;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.PictureBox pictureBox5;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label3;
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.webBrowser2 = new appCore.Web.AMTBrowser();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.webBrowser3 = new appCore.Web.AMTBrowser();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.pictureBox4 = new System.Windows.Forms.PictureBox();
			this.pictureBox5 = new System.Windows.Forms.PictureBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(0, 68);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(996, 564);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.textBox1);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.webBrowser1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(988, 538);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Citrix";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(35, 3);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(950, 20);
			this.textBox1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "URL";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// webBrowser1
			// 
			this.webBrowser1.AllowWebBrowserDrop = false;
			this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser1.Location = new System.Drawing.Point(3, 26);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.ScriptErrorsSuppressed = true;
			this.webBrowser1.Size = new System.Drawing.Size(982, 509);
			this.webBrowser1.TabIndex = 0;
			this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowsersNavigating);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.textBox2);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Controls.Add(this.webBrowser2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(988, 538);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "SITE Lopedia";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.Location = new System.Drawing.Point(35, 3);
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(950, 20);
			this.textBox2.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 20);
			this.label2.TabIndex = 4;
			this.label2.Text = "URL";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// webBrowser2
			// 
			this.webBrowser2.AllowWebBrowserDrop = false;
			this.webBrowser2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser2.Location = new System.Drawing.Point(3, 26);
			this.webBrowser2.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser2.Name = "webBrowser2";
			this.webBrowser2.ScriptErrorsSuppressed = true;
			this.webBrowser2.Size = new System.Drawing.Size(982, 509);
			this.webBrowser2.TabIndex = 0;
			this.webBrowser2.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserDocumentCompleted);
			this.webBrowser2.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowsersNavigating);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.textBox3);
			this.tabPage3.Controls.Add(this.label3);
			this.tabPage3.Controls.Add(this.webBrowser3);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(988, 538);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "SITE Lopedia";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// textBox3
			// 
			this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox3.Location = new System.Drawing.Point(35, 3);
			this.textBox3.Name = "textBox3";
			this.textBox3.ReadOnly = true;
			this.textBox3.Size = new System.Drawing.Size(950, 20);
			this.textBox3.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 20);
			this.label3.TabIndex = 6;
			this.label3.Text = "URL";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// webBrowser3
			// 
			this.webBrowser3.AllowWebBrowserDrop = false;
			this.webBrowser3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser3.Location = new System.Drawing.Point(3, 26);
			this.webBrowser3.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser3.Name = "webBrowser3";
			this.webBrowser3.ScriptErrorsSuppressed = true;
			this.webBrowser3.Size = new System.Drawing.Size(982, 509);
			this.webBrowser3.TabIndex = 1;
			this.webBrowser3.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserDocumentCompleted);
			this.webBrowser3.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowsersNavigating);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::appCore.UI.Resources.arrow_left;
			this.pictureBox1.Location = new System.Drawing.Point(4, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(40, 40);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.PictureBox1Click);
			this.pictureBox1.MouseLeave += new System.EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox1.MouseHover += new System.EventHandler(this.PictureBoxesMouseHover);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::appCore.UI.Resources.arrow_right;
			this.pictureBox2.Location = new System.Drawing.Point(50, 12);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(40, 40);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.PictureBox2Click);
			this.pictureBox2.MouseLeave += new System.EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox2.MouseHover += new System.EventHandler(this.PictureBoxesMouseHover);
			// 
			// pictureBox3
			// 
			this.pictureBox3.Image = global::appCore.UI.Resources.refresh;
			this.pictureBox3.Location = new System.Drawing.Point(96, 12);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(40, 40);
			this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox3.TabIndex = 3;
			this.pictureBox3.TabStop = false;
			this.pictureBox3.Click += new System.EventHandler(this.PictureBox3Click);
			this.pictureBox3.MouseLeave += new System.EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox3.MouseHover += new System.EventHandler(this.PictureBoxesMouseHover);
			// 
			// pictureBox4
			// 
			this.pictureBox4.Image = global::appCore.UI.Resources.home;
			this.pictureBox4.Location = new System.Drawing.Point(142, 12);
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.Size = new System.Drawing.Size(40, 40);
			this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox4.TabIndex = 4;
			this.pictureBox4.TabStop = false;
			this.pictureBox4.Click += new System.EventHandler(this.PictureBox4Click);
			this.pictureBox4.MouseLeave += new System.EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox4.MouseHover += new System.EventHandler(this.PictureBoxesMouseHover);
			// 
			// pictureBox5
			// 
			this.pictureBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox5.Image = global::appCore.UI.Resources.Badass_browser_1;
			this.pictureBox5.Location = new System.Drawing.Point(912, 4);
			this.pictureBox5.Name = "pictureBox5";
			this.pictureBox5.Size = new System.Drawing.Size(80, 80);
			this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox5.TabIndex = 5;
			this.pictureBox5.TabStop = false;
			this.pictureBox5.Click += new System.EventHandler(this.PictureBox5Click);
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
			"SITE Lopedia",
			"SITE Lopedia (Old)",
			"Locked Cells",
			"Locked Cells (Old)",
			"Sites Off Air",
			"Vendor Override",
			"COOS - No Unavailability",
			"Bulk Uploader",
			"ANOC Site Management Diary (Book Ins)"});
			this.comboBox1.Location = new System.Drawing.Point(675, 4);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(231, 21);
			this.comboBox1.TabIndex = 6;
			this.comboBox1.Visible = false;
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(6, 19);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(40, 15);
			this.checkBox1.TabIndex = 7;
			this.checkBox1.Text = "2G";
			this.checkBox1.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkBox4);
			this.groupBox1.Controls.Add(this.checkBox3);
			this.groupBox1.Controls.Add(this.checkBox2);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Location = new System.Drawing.Point(537, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(132, 79);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Select cells";
			this.groupBox1.Visible = false;
			// 
			// checkBox4
			// 
			this.checkBox4.Location = new System.Drawing.Point(65, 38);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(61, 15);
			this.checkBox4.TabIndex = 10;
			this.checkBox4.Text = "All cells";
			this.checkBox4.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox4.UseVisualStyleBackColor = true;
			this.checkBox4.CheckedChanged += new System.EventHandler(this.CheckBox4CheckedChanged);
			// 
			// checkBox3
			// 
			this.checkBox3.Location = new System.Drawing.Point(6, 58);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(40, 15);
			this.checkBox3.TabIndex = 9;
			this.checkBox3.Text = "4G";
			this.checkBox3.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.CheckBox3CheckedChanged);
			// 
			// checkBox2
			// 
			this.checkBox2.Location = new System.Drawing.Point(6, 38);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(40, 15);
			this.checkBox2.TabIndex = 8;
			this.checkBox2.Text = "3G";
			this.checkBox2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBox2CheckedChanged);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Font = new System.Drawing.Font("Stencil", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.DarkRed;
			this.label4.Location = new System.Drawing.Point(675, 28);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(231, 56);
			this.label4.TabIndex = 9;
			this.label4.Text = "UNDER\r\nDEVELOPMENT";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BrowserView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(992, 628);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.pictureBox5);
			this.Controls.Add(this.pictureBox4);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.tabControl1);
			this.Icon = global::appCore.UI.Resources.Badass_browser;
			this.MinimumSize = new System.Drawing.Size(1000, 655);
			this.Name = "BrowserView";
			this.Text = "AMT Browser - Citrix";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		
	}
}

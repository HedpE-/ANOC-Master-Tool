/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 05-05-2015
 * Time: 04:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTLargeTextForm.
	/// </summary>
	public sealed class AMTLargeTextForm : Form
	{
		AMTRichTextBox richTextBox1 = new AMTRichTextBox();
		Button button1 = new Button();
		Button button2 = new Button();
		public string finaltext = string.Empty;
		
		public AMTLargeTextForm(string content, string title, bool ReadOnly)
		{
			InitializeComponent();
			
			if(ReadOnly) {
				button1.Visible = false;
				button2.Visible = false;
				richTextBox1.Size = new Size(579, 375);
			}
			
			this.Text = title;
			finaltext = content;
			richTextBox1.Text = finaltext;
			
			richTextBox1.ReadOnly = ReadOnly;
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			finaltext = richTextBox1.Text;
			Close();
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			richTextBox1.Text = finaltext;
			Close();
		}
		
		void InitializeComponent()
		{
			SuspendLayout();
			// 
			// richTextBox1
			// 
			richTextBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
			| AnchorStyles.Left) 
			| AnchorStyles.Right)));
			richTextBox1.Font = new Font("Courier New", 8.25F);
			richTextBox1.Location = new Point(0, 2);
			richTextBox1.Name = "richTextBox1";
			richTextBox1.Size = new Size(579, 346);
			richTextBox1.TabIndex = 0;
			richTextBox1.Text = "";
			// 
			// button1
			// 
			button1.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
			button1.Location = new Point(0, 354);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 1;
			button1.Text = "Continue";
			button1.UseVisualStyleBackColor = true;
			button1.Click += Button1Click;
			// 
			// button2
			// 
			button2.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
			button2.Location = new Point(504, 354);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 2;
			button2.Text = "Cancel";
			button2.UseVisualStyleBackColor = true;
			button2.Click += Button2Click;
			// 
			// LargeTextForm
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(579, 378);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(richTextBox1);
			Icon = Resources.app_icon;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(200, 200);
			Name = "LargeTextForm";
			Text = "LargeTextForm";
			ResumeLayout(false);
		}
	}
}

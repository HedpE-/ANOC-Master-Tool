/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 16/04/2017
 * Time: 19:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of ToolsMenu.
	/// </summary>
	public class ToolsMenu : Panel
	{
		PictureBox pictureBox1 = new PictureBox();
		PictureBox pictureBox2 = new PictureBox();
		PictureBox pictureBox3 = new PictureBox();
		PictureBox pictureBox4 = new PictureBox();
		PictureBox SiteDetailsPictureBox = new PictureBox();
		PictureBox pictureBox6 = new PictureBox();
		
		public ToolsMenu() {
			InitializeComponent();
		}
		
		void PictureBoxesMouseLeave(object sender, EventArgs e) {
			PictureBox pic = (PictureBox)sender;
			switch(pic.Name) {
				case "pictureBox1":
					pic.Image = Toolbox.SVGParser.GetBitmapFromSvgFile(@"C:\Users\goncarj3\Desktop\ANOC-Master-Tool\cogwheel-outline.svg", new Size(40, 40));
					break;
				case "pictureBox2":
					pic.Image = Resources.globe;
					break;
				case "pictureBox3":
					pic.Image = Resources.Book_512;
					break;
				case "pictureBox4":
					break;
				case "SiteDetailsPictureBox":
					pic.Image = Resources.radio_tower;
					break;
				case "pictureBox6":
					break;
			}
		}
		
		void PictureBoxesMouseHover(object sender, EventArgs e) {
			PictureBox pic = (PictureBox)sender;
			switch(pic.Name) {
				case "pictureBox1":
					pic.Image = Resources.Settings_hover;
					break;
				case "pictureBox2":
					pic.Image = Resources.globe_hover;
					break;
				case "pictureBox3":
					break;
				case "pictureBox4":
					break;
				case "SiteDetailsPictureBox":
					pic.Image = Resources.radio_tower_hover;
					break;
				case "pictureBox6":
					break;
			}
		}
		
		void InitializeComponent()
		{
			BackColor = Color.Transparent;
			Controls.Add(pictureBox2);
			Controls.Add(pictureBox6);
			Controls.Add(pictureBox1);
			Controls.Add(pictureBox4);
			Controls.Add(pictureBox3);
			Controls.Add(SiteDetailsPictureBox);
			Name = "panel1";
			Size = new Size(98, 138);
			// 
			// pictureBox2
			// 
			pictureBox2.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			pictureBox2.BackColor = Color.Transparent;
			pictureBox2.Image = Resources.globe;
			pictureBox2.Location = new Point(6, 3);
			pictureBox2.Name = "pictureBox2";
			pictureBox2.Size = new Size(40, 40);
			pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox2.TabIndex = 3;
			pictureBox2.TabStop = false;
//			pictureBox2.Click += PictureBoxesClick;
			pictureBox2.MouseLeave += PictureBoxesMouseLeave;
			pictureBox2.MouseHover += PictureBoxesMouseHover;
			// 
			// pictureBox6
			// 
			pictureBox6.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			pictureBox6.BackColor = Color.Transparent;
			pictureBox6.Image = Resources.Business_Planner_icon;
			pictureBox6.Location = new Point(52, 95);
			pictureBox6.Name = "pictureBox6";
			pictureBox6.Size = new Size(40, 40);
			pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox6.TabIndex = 9;
			pictureBox6.TabStop = false;
			pictureBox6.Visible = false;
//			pictureBox6.Click += PictureBoxesClick;
			pictureBox6.MouseLeave += PictureBoxesMouseLeave;
			pictureBox6.MouseHover += PictureBoxesMouseHover;
			// 
			// pictureBox1
			// 
			pictureBox1.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			pictureBox1.BackColor = Color.Transparent;
//			pictureBox1.Image = Resources.Settings_normal;
			pictureBox1.Image = Toolbox.SVGParser.GetBitmapFromSvgFile(@"C:\Users\goncarj3\Desktop\ANOC-Master-Tool\cogwheel-outline.svg", new Size(40, 40));
			pictureBox1.Location = new Point(6, 95);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(40, 40);
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox1.TabIndex = 1;
			pictureBox1.TabStop = false;
//			pictureBox1.Click += PictureBoxesClick;
			pictureBox1.MouseLeave += PictureBoxesMouseLeave;
			pictureBox1.MouseHover += PictureBoxesMouseHover;
			// 
			// pictureBox4
			// 
			pictureBox4.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			pictureBox4.BackColor = Color.Transparent;
			pictureBox4.Image = Resources._lock;
			pictureBox4.Location = new Point(52, 49);
			pictureBox4.Name = "pictureBox4";
			pictureBox4.Size = new Size(40, 40);
			pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox4.TabIndex = 7;
			pictureBox4.TabStop = false;
//			pictureBox4.Click += PictureBoxesClick;
			// 
			// pictureBox3
			// 
			pictureBox3.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			pictureBox3.BackColor = Color.Transparent;
			pictureBox3.Image = Resources.Book_512;
			pictureBox3.Location = new Point(52, 3);
			pictureBox3.Name = "pictureBox3";
			pictureBox3.Size = new Size(40, 40);
			pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox3.TabIndex = 4;
			pictureBox3.TabStop = false;
//			pictureBox3.Click += PictureBoxesClick;
			pictureBox3.MouseLeave += PictureBoxesMouseLeave;
			// 
			// SiteDetailsPictureBox
			// 
			SiteDetailsPictureBox.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			SiteDetailsPictureBox.BackColor = Color.Transparent;
			SiteDetailsPictureBox.Image = Resources.radio_tower;
			SiteDetailsPictureBox.Location = new Point(6, 49);
			SiteDetailsPictureBox.Name = "SiteDetailsPictureBox";
			SiteDetailsPictureBox.Size = new Size(40, 40);
			SiteDetailsPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
			SiteDetailsPictureBox.TabIndex = 8;
			SiteDetailsPictureBox.TabStop = false;
//			SiteDetailsPictureBox.Click += PictureBoxesClick;
			SiteDetailsPictureBox.MouseLeave += PictureBoxesMouseLeave;
			SiteDetailsPictureBox.MouseHover += PictureBoxesMouseHover;
		}
	}
}

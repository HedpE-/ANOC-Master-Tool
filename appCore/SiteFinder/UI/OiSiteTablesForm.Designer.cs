/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 14-10-2016
 * Time: 01:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace appCore.SiteFinder.UI
{
	partial class OiSiteTablesForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ListView listView1;
		
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
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HideSelection = false;
			this.listView1.HoverSelection = true;
			this.listView1.Location = new System.Drawing.Point(6, 6);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(884, 221);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.DoubleClick += new System.EventHandler(this.ListView1DoubleClick);
			this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView1KeyDown);
			// 
			// OiSiteTablesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(896, 236);
			this.Controls.Add(this.listView1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = global::appCore.UI.Resources.MB_0001_vodafone3;
			this.Name = "OiSiteTablesForm";
			this.Text = "OiSiteTablesForm";
			this.ResumeLayout(false);

		}
	}
}

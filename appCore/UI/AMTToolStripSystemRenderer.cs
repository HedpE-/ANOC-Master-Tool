/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 17-10-2016
 * Time: 19:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTToolStripSystemRenderer.
	/// https://msdn.microsoft.com/en-us/library/system.windows.forms.toolstriprenderer(v=vs.110).aspx
	/// </summary>
	public class AMTToolStripSystemRenderer : ToolStripSystemRenderer
	{
		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
			if(e.Item.OwnerItem == null) {
				if(!e.Item.Enabled) {
					if(!e.Item.Name.Contains("Refresh")) {
						Color prevColor = e.TextColor;
						e.TextFont = new Font("Segoe UI", 7F);
						e.TextColor = prevColor;
						base.OnRenderItemText(e);
					}
//					else {
//	//					double factor = (double) e.Item.Bounds.Height / Resources.refresh.Height;
//	//					var rect = new Rectangle( e.Item.Bounds.X, e.Item.Bounds.Y,
//	//					                         (int) ( Resources.refresh.Width * factor ),
//	//					                         (int) ( Resources.refresh.Height * factor ) );
//	//					e.Graphics.DrawImage(Resources.refresh, rect);
//					}
				}
				else {
					if(e.Item.Name.Contains("Button")) {
						e.TextFont = e.Text.StartsWith("Click") ? new Font("Segoe UI", 7F) : new Font("Segoe UI", 9F);
//					base.OnRenderItemText(e);
					}
//				else {
//					double factor = (double) e.Item.Bounds.Height / Resources.refresh.Height;
//					var rect = new Rectangle( e.Item.Bounds.X, e.Item.Bounds.Y,
//					                         (int) ( Resources.refresh.Width * factor ),
//					                         (int) ( Resources.refresh.Height * factor ) );
//					e.Graphics.DrawImage(Resources.refresh, rect);
//				}
					base.OnRenderItemText(e);
				}
			}
			else
				base.OnRenderItemText(e);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			if(e.ToolStrip.GetType() != typeof(AMTMenuStrip))
				base.OnRenderToolStripBorder(e); // render border on items that are not of type AMTMenuStrip
//			else {
//				// skip render border
//				e.Graphics.FillRectangle(Brushes.Black, e.ConnectedArea);
//				base.OnRenderToolStripBorder(e);
//			}
		}
		
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
			if(e.Item.OwnerItem == null) { //Name.Contains("ToolStripMenuItem")) {
				if(e.Item.Name.Contains("Refresh")) {
					if(e.Item.Enabled)
						base.OnRenderMenuItemBackground(e);
				}
				else {
					if (!e.Item.Selected)
						base.OnRenderMenuItemBackground(e);
					else {
						if (!string.IsNullOrEmpty(e.Item.Text) && e.Item.Enabled) {
							base.OnRenderMenuItemBackground(e);
						}
					}
				}
			}
			else
				base.OnRenderMenuItemBackground(e);
		}
		
//		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
//			if(!e.Item.Name.Contains("refresh"))
//				base.OnRenderItemText(e);
//		}
		

//		private class MyColors : ProfessionalColorTable {
//			public override Color MenuItemSelected {
//				get { return Color.Yellow; }
//			}
//			public override Color MenuItemSelectedGradientBegin {
//				get { return Color.Orange; }
//			}
//			public override Color MenuItemSelectedGradientEnd {
//				get { return Color.Yellow; }
//			}
//		}
	}
}

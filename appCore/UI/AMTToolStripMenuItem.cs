/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 20-10-2016
 * Time: 19:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTToolStripMenuItem.
	/// </summary>
	public class AMTToolStripMenuItem : ToolStripMenuItem
	{
		//        private ContextMenuStrip secondaryContextMenu;
//
		//        public ContextMenuStrip SecondaryContextMenu
		//        {
		//            get
		//            {
		//                return secondaryContextMenu;
		//            }
		//            set
		//            {
		//                secondaryContextMenu = value;
		//            }
		//        }
//
		//        public AMTToolStripMenuItem(string text)
		//            : base(text)
		//        { }
//
		public AMTToolStripMenuItem()
		{ }
		
//		protected override void OnPaint(PaintEventArgs e) {
//			double factor = (double) e.Bounds.Height / Resources.refresh.Height;
//			var rect = new Rectangle( e.Bounds.X, e.Bounds.Y,
//			                         (int) ( Resources.refresh.Width * factor ),
//			                         (int) ( Resources.refresh.Height * factor ) );
//			e.Graphics.DrawImage(Resources.refresh, rect);
//		}
//
		//        protected override void Dispose(bool disposing)
		//        {
		//            if (disposing)
		//            {
		//                if (secondaryContextMenu != null)
		//                {
		//                    secondaryContextMenu.Dispose();
		//                    secondaryContextMenu = null;
		//                }
		//            }
//
		//            base.Dispose(disposing);
		//        }
//
		//        protected override void OnMouseDown(MouseEventArgs e)
		//        {
		//        	if (SecondaryContextMenu != null && e.Button == MouseButtons.Right) {
		//        		AMTContextMenuStrip cm = (AMTContextMenuStrip)Parent;
		//        		cm.Show();
		//            }
		//        	else
		//                base.OnMouseDown(e);
		//        }
	}
}

/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 25/04/2017
 * Time: 10:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.GeoAPIs.UI
{
	/// <summary>
	/// Description of WeatherPanel.
	/// </summary>
	public class WeatherPanel : Panel
	{
		public WeatherPanel()
		{
		}
		
//		protected void TickHandler(object sender, EventArgs e)
//		{
//			this.InvalidateEx();
//		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;

				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT

				return cp;
			}
		}
//
//		protected void InvalidateEx()
//		{
//			if (Parent == null)
//			{
//				return;
//			}
//
//			Rectangle rc = new Rectangle(this.Location, this.Size);
//
//			Parent.Invalidate(rc, true);
//		}
//
//		protected override void OnPaintBackground(PaintEventArgs pevent)
//		{
//
//		}
//
//		private Random r = new Random();

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Black)), this.ClientRectangle);
		}
		
//		private Point previousLocation;
//		
//		protected override void OnMouseDown(MouseEventArgs e)
//		{
//			previousLocation = e.Location;
//		}
//
//		protected override void OnMouseMove(MouseEventArgs e)
//		{
////			if (activeControl == null || activeControl != sender)
////				return;
//
//			var location = Parent.Location;
//			location.Offset(e.Location.X - previousLocation.X, e.Location.Y - previousLocation.Y);
//			Parent.Location = location;
//		}
	}
}

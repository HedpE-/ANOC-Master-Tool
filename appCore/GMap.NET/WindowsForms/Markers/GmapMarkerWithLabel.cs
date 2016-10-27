/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 04-01-2016
 * Time: 00:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace GMap.NET.WindowsForms.Markers
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public class GmapMarkerWithLabel : GMapMarker, ISerializable
	{
		Font font;
		GMarkerGoogle innerMarker;

		public string Caption;

		public GmapMarkerWithLabel(PointLatLng p, string caption, GMarkerGoogleType type)
			: base(p)
		{
			font = new Font("Courier New", 8.25F, FontStyle.Bold);
			
			innerMarker = new GMarkerGoogle(p, type);

			Caption = caption;
		}

		public override void OnRender(Graphics g)
		{
			if (innerMarker != null)
			{
				innerMarker.OnRender(g);
			}

			g.DrawString(Caption, font, Brushes.Red, new PointF(0.0f, innerMarker.Size.Height));
		}

		public override void Dispose()
		{
			if(innerMarker != null)
			{
				innerMarker.Dispose();
				innerMarker = null;
			}

			base.Dispose();
		}

		#region ISerializable Members

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		protected GmapMarkerWithLabel(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}

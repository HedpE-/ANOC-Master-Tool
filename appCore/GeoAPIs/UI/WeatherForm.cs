/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 30/04/2017
 * Time: 15:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenWeatherAPI;

namespace appCore.GeoAPIs.UI
{
	/// <summary>
	/// Description of WeatherForm.
	/// </summary>
	public class WeatherForm : Form
	{
		WeatherPanel weatherPanel;
		
		public int WeatherPanelOpacity {
			get {
				return weatherPanel != null ? weatherPanel.Opacity : -1;
			}
			set {
				if(weatherPanel != null)
					weatherPanel.Opacity = value;
				else
					throw new Exception();
			}
		}
		
		[StructLayout(LayoutKind.Sequential)]
		public struct MARGINS
		{
			public int Left;
			public int Right;
			public int Top;
			public int Bottom;
		}

		[DllImport("dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);
		
		public WeatherForm(Query weather)
		{
			MARGINS marg = new MARGINS() { Left = -1, Right = -1, Top = -1, Bottom = -1 };
			DwmExtendFrameIntoClientArea(this.Handle, ref marg);
			
			weatherPanel = new WeatherPanel(weather);
			weatherPanel.Location = Point.Empty;
			weatherPanel.CornersToRound = appCore.UI.AMTRoundCornersPanel.Corners.BottomLeft;
			weatherPanel.CornerSize = 25;
			weatherPanel.BordersToDraw = appCore.UI.AMTRoundCornersPanel.Borders.None;
			
			Controls.Add(weatherPanel);
			
			ControlBox = false;
//			ShowInTaskbar = false;
			Size = weatherPanel.Size;
			FormBorderStyle = FormBorderStyle.None;
			Text = "Weather Conditions: " + weather.Name;
			Icon = global::appCore.UI.Resources.app_icon;
		}
		
//		public const int WM_NCLBUTTONDOWN = 0xA1;
//		public const int HT_CAPTION = 0x2;
//
//		[DllImportAttribute("user32.dll")]
//		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
//		[DllImportAttribute("user32.dll")]
//		public static extern bool ReleaseCapture();
//
//		protected override void OnMouseDown(MouseEventArgs e)
//		{
//			base.OnMouseDown(e);
//			if (e.Button == MouseButtons.Left)
//			{
//				ReleaseCapture();
//				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
//			}
//		}
		
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(weatherPanel != null)
				weatherPanel.Refresh();
		}
	}
}

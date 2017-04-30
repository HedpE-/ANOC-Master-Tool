/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 30/04/2017
 * Time: 15:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace appCore.GeoAPIs.UI
{
	/// <summary>
	/// Description of Weather2.
	/// </summary>
	public class Weather2 : Form
	{
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
		
		public Weather2()
		{
			MARGINS marg = new MARGINS() { Left = -1, Right = -1, Top = -1, Bottom = -1 };
			DwmExtendFrameIntoClientArea(this.Handle, ref marg);
		}
	}
}

/***************************************************
 * Glacial List v1.30
 * 
 * Written By Allen Anderson
 * http://www.glacialcomponents.com
 * 
 * February 24th, 2004
 * 
 * You may redistribute this control in binary and modified binary form as you please.  You may
 * use this control in commercial applications without need for external credit royalty free.
 * 
 * However, you are restricted from releasing the source code in any modified fashion
 * whatsoever.
 * 
 * I MAKE NO PROMISES OR WARRANTIES ON THIS CODE/CONTROL.  IF ANY DAMAGE OR PROBLEMS HAPPEN FROM ITS USE
 * THEN YOU ARE RESPONSIBLE.
 * 
 */

using System;
using System.Runtime.InteropServices;

namespace GlacialComponents.Controls
{
	/// <summary>
	/// Summary description for Win32.
	/// </summary>
	internal class Win32
	{
      [DllImport("user32.dll", ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
      static public extern IntPtr BeginDeferWindowPos(int nNumWindows);

      [DllImport("user32.dll", ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
      static public extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

      [DllImport("user32.dll", ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
      static public extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, HandleRef hwnd, HandleRef hwndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

      [DllImport("user32.dll", ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
      static public extern bool SetWindowPos(HandleRef hwnd, HandleRef hwndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

      public const int SWP_NOSIZE = 0x0001,
         SWP_NOMOVE = 0x0002,
         SWP_NOZORDER = 0x0004,
         SWP_NOACTIVATE = 0x0010,
         SWP_FRAMECHANGED = 0x0020,
         SWP_DRAWFRAME = SWP_FRAMECHANGED,
         SWP_SHOWWINDOW = 0x0040,
         SWP_HIDEWINDOW = 0x0080,
         SWP_NOCOPYBITS = 0x0100,
         SWP_NOOWNERZORDER = 0x0200,
         SWP_NOREPOSITION = SWP_NOOWNERZORDER,
         SWP_NOSENDCHANGING = 0x0400,
         SWP_DEFERERASE = 0x2000,
         SWP_ASYNCWINDOWPOS = 0x4000;

      public static HandleRef HWND_TOP = new HandleRef(null, (IntPtr)0);
      public static HandleRef HWND_BOTTOM = new HandleRef(null, (IntPtr)1);
      public static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
      public static HandleRef HWND_NOTOPMOST = new HandleRef(null, new IntPtr(-2));
      public static HandleRef HWND_NULL = new HandleRef(null, (IntPtr)0);
	}
}

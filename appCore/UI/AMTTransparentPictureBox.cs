﻿/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 27/04/2017
 * Time: 19:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTTransparentPictureBox.
	/// </summary>
	public class AMTTransparentPictureBox : PictureBox
	{
		public AMTTransparentPictureBox()
		{
		}

		/// <summary>
		/// Gets the creation parameters.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x20;
				return cp;
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs e)
			// Paint background with underlying graphics from other controls
		{
			base.OnPaintBackground(e);
			Graphics g = e.Graphics;

			if (Parent != null)
			{
				// Take each control in turn
				int index = Parent.Controls.GetChildIndex(this);
				for (int i = Parent.Controls.Count - 1; i > index; i--)
				{
					Control c = Parent.Controls[i];

					// Check it's visible and overlaps this control
					if (c.Bounds.IntersectsWith(Bounds) && c.Visible)
					{
						// Load appearance of underlying control and redraw it on this background
						Bitmap bmp = new Bitmap(c.Width, c.Height, g);
						c.DrawToBitmap(bmp, c.ClientRectangle);
						g.TranslateTransform(c.Left - Left, c.Top - Top);
						g.DrawImageUnscaled(bmp, Point.Empty);
						g.TranslateTransform(Left - c.Left, Top - c.Top);
						bmp.Dispose();
					}
				}
			}
		}
	}
}
/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 29/04/2017
 * Time: 15:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

[ToolboxBitmap(typeof(System.Windows.Forms.Panel))]
public class Panel : System.Windows.Forms.Panel
{

	private bool bMouseIsHover = false;

	private Bitmap btMousePointer = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
	#region "Generic component"
	public Panel() : base()
	{
		MouseMove += Panel_MouseMove;
		MouseLeave += Panel_MouseLeave;
		MouseEnter += Panel_MouseEnter;
		Paint += Panel_Paint;
		SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		SetStyle(ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Opaque, false);
		SetStyle(ControlStyles.EnableNotifyMessage, true);
		SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		BackColor = Color.Transparent;
	}

	[Browsable(false)]
	public bool MouseIsHover {
		get { return bMouseIsHover; }
	}

	private void Panel_Paint(object sender, PaintEventArgs e)
	{
		PaintBackgoundSurface(this, e);
		PaintBorder(this, e);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	#endregion

	#region "Mouse Events"

	private void Panel_MouseEnter(object sender, System.EventArgs e)
	{
		bMouseIsHover = true;
		CaptureMousePointerImage();
	}

	private void Panel_MouseLeave(object sender, System.EventArgs e)
	{
		bMouseIsHover = false;
		Invalidate();
	}

	private void Panel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		Invalidate();
		//New Rectangle(e.X - 32, e.Y - 32, 64, 64)
	}

	#endregion

	#region "Properties"

	private bool bMouseReflection = true;
	[Category("Glass"), Description("Should the mouse cursor reflects on the glass surface.")]
	public bool MouseReflection {
		get { return bMouseReflection; }
		set {
			bMouseReflection = value;
			Invalidate();
		}
	}

	private int iOpacity = 25;
	[Category("Glass"), Description("Sets the opacity of the glass (between 0 [transparent] and 255 [opaque])")]
	public int Opacity {
		get { return iOpacity; }
		set {
			if (value > 255)
				value = 255;
			if (value < 0)
				value = 0;
			iOpacity = value;
			Invalidate();
		}
	}

	private Color clGlassColor = Color.WhiteSmoke;
	[Category("Glass"), Description("Defines the color of the glass. Among good choices (WhiteSmoke, AliceBlue, MistyRose, AntiqueWhite, Ivory, HoneyDew, Lavender")]
	public Color GlassColor {
		get { return clGlassColor; }
		set {
			clGlassColor = value;
			Invalidate();
		}
	}

	#endregion

	#region "Private Properties"
	[Browsable(false)]
	public RectangleF EffectiveBounds {
		get { return new RectangleF(ClientRectangle.X + 2, ClientRectangle.Y + 2, ClientRectangle.Width - 4, ClientRectangle.Height - 4); }
	}

	[Browsable(false)]
	public GraphicsPath RoundSurface {
		get { return RoundCorners(EffectiveBounds, Radius); }
	}

	[Browsable(false)]
	public GraphicsPath RoundSurfaceInner {
		get {
			RectangleF rect = EffectiveBounds;
			rect.Inflate(-1, -1);
			return RoundCorners(rect, Radius);
		}
	}

	[Browsable(false)]
	public Region InnerRegion {
		get {
			Region rgInnerRegion = new Region(RoundSurface);
			return rgInnerRegion;
		}
	}

	[Browsable(false)]
	public Region OuterRegion {
		get {
			Region rgOuterRegion = new Region(RoundSurface);
			rgOuterRegion.Xor(ClientRectangle);
			return rgOuterRegion;
		}
	}

	private float snRadius = 5;
	[Browsable(false)]
	public float Radius {
		get { return snRadius; }
		set { snRadius = value; }
	}

	#endregion

	#region "Paintings"
	public void PaintBackgoundSurface(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		var _with1 = e.Graphics;
		_with1.SmoothingMode = SmoothingMode.HighQuality;
		_with1.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias;
		_with1.CompositingQuality = CompositingQuality.HighQuality;
		SolidBrush brGlass = new SolidBrush(Color.FromArgb(Check(Opacity), GlassColor.R, GlassColor.G, GlassColor.B));
		e.Graphics.FillPath(brGlass, RoundSurface);
		PaintHorizontalSurface(sender, e);
		PaintGlowSurface(sender, e);
		PaintReflectiveBands(sender, e);
		PaintLightSource(sender, e);
		if (bMouseIsHover)
			PaintMouseCursorReflection(sender, e);
	}

	public void PaintReflectiveBands(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		SolidBrush brGlassReflect = new SolidBrush(Color.FromArgb(Check(Opacity * 0.5), GlassColor.R, GlassColor.G, GlassColor.B));
		GraphicsPath grpBand1 = CreateReflectiveBand(0.1f, 0.5f, 0.15f);
		GraphicsPath grpBand2 = CreateReflectiveBand(0.4f, 0.8f, 0.1f);
		e.Graphics.FillPath(brGlassReflect, grpBand1);
		e.Graphics.FillPath(brGlassReflect, grpBand2);
	}

	public void PaintHorizontalSurface(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		SolidBrush brGlassDark = new SolidBrush(Color.FromArgb(Opacity * 2.5, DeductMinZero(GlassColor.R, 50), DeductMinZero(GlassColor.G, 50), DeductMinZero(GlassColor.B, 50)));
		e.Graphics.ExcludeClip(new Rectangle(int.Parse(EffectiveBounds.Left), EffectiveBounds.Top, EffectiveBounds.Width, EffectiveBounds.Height * 0.66f));
		e.Graphics.FillPath(brGlassDark, RoundSurface);
		e.Graphics.ResetClip();
	}

	public void PaintGlowSurface(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		LinearGradientBrush brGlassDarkLinear = new LinearGradientBrush(ClientRectangle, Color.FromArgb(0, DeductMinZero(GlassColor.R, 50), DeductMinZero(GlassColor.G, 50), DeductMinZero(GlassColor.B, 50)), Color.FromArgb(Check(Opacity * 2.5), GlassColor.R, GlassColor.G, GlassColor.B), LinearGradientMode.Vertical);
		e.Graphics.FillPath(brGlassDarkLinear, RoundSurfaceInner);
	}

	public GraphicsPath CreateReflectiveBand(float LeftFactor, float RightFactor, float SizeFactor)
	{
		GraphicsPath grpBand = new GraphicsPath();
		var _with2 = grpBand;
		_with2.StartFigure();
		_with2.AddLine(2 + (EffectiveBounds.Width * LeftFactor), 2, 2 + (EffectiveBounds.Width * LeftFactor) + (EffectiveBounds.Width * SizeFactor), 2);
		_with2.AddLine((2 + (EffectiveBounds.Width * LeftFactor)) + (EffectiveBounds.Width * SizeFactor), 2, (2 + (EffectiveBounds.Width * RightFactor)) + (EffectiveBounds.Width * SizeFactor), EffectiveBounds.Top + EffectiveBounds.Height);
		_with2.AddLine((2 + (EffectiveBounds.Width * RightFactor) + (EffectiveBounds.Width * SizeFactor)), 2 + EffectiveBounds.Height, EffectiveBounds.Left + (EffectiveBounds.Width * RightFactor), EffectiveBounds.Top + EffectiveBounds.Height);
		_with2.AddLine(2 + (EffectiveBounds.Width * RightFactor), 2 + EffectiveBounds.Height, 2 + (EffectiveBounds.Width * LeftFactor), 2);
		_with2.CloseFigure();
		return grpBand;
	}

	public void PaintLightSource(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		RectangleF rcLight = GetLightBounds(0.75);
		GraphicsPath grpLight = new GraphicsPath();
		grpLight.StartFigure();
		grpLight.AddEllipse(rcLight);

		PathGradientBrush brLight = new PathGradientBrush(grpLight);
		brLight.CenterColor = Color.FromArgb(Check(Opacity * 3), 255, 255, 255);
		brLight.SurroundColors = new Color[] { Color.FromArgb(0, 255, 255, 255) };

		e.Graphics.ExcludeClip(OuterRegion);
		e.Graphics.FillEllipse(brLight, rcLight);
		e.Graphics.ResetClip();
	}

	public RectangleF GetLightBounds(float Size)
	{
		return new RectangleF(2 - ((EffectiveBounds.Height * Size) / 2), 2 - ((EffectiveBounds.Height * Size) / 2), EffectiveBounds.Height * Size, EffectiveBounds.Height * Size);
	}

	public void PaintBorder(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		e.Graphics.DrawPath(new Pen(Color.FromArgb(200, 255, 255, 255), 0.5f), RoundSurface);
		e.Graphics.DrawPath(new Pen(Color.FromArgb(255, 0, 0, 0), 0.5f), RoundSurfaceInner);
	}

	public void PaintMouseCursorReflection(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		Point ptMouseLocation = this.PointToClient(Cursor.Position);
		ptMouseLocation.Offset(-30, -30);

		ColorMatrix clrMatrix = default(ColorMatrix);
		clrMatrix = new ColorMatrix(new float[][] {
			new float[] {
				1,
				0,
				0,
				0,
				0
			},
			new float[] {
				0,
				1,
				0,
				0,
				0
			},
			new float[] {
				0,
				0,
				1,
				0,
				0
			},
			new float[] {
				0,
				0,
				0,
				0.1f,
				0
			},
			new float[] {
				0,
				0,
				0,
				0,
				1
			}
		});

		ImageAttributes imgAttributes = new ImageAttributes();
		imgAttributes.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

		e.Graphics.ExcludeClip(OuterRegion);
		e.Graphics.DrawImage(btMousePointer, new Rectangle(ptMouseLocation.X, ptMouseLocation.Y, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, imgAttributes);
		e.Graphics.ResetClip();
	}

	public void CaptureMousePointerImage()
	{
		Graphics grPointer = Graphics.FromImage(btMousePointer);
		grPointer.Clear(Color.Transparent);
		Cursor.Current.Draw(grPointer, new Rectangle(0, 0, 32, 32));
	}

	#endregion

}
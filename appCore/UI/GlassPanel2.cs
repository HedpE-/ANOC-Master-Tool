/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 29/04/2017
 * Time: 15:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
#region "Import"

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

#endregion

#region "MBPanel Class"

/// <summary>
/// MBPanel Classs © 2011 By Manoj Kishor Bhoir
/// </summary>
/// <remarks>Version 1.0.1096.2286</remarks>
[ToolboxItem(true), ToolboxBitmap(typeof(MBPanel), "MBPanel.MBPanel.bmp"), ToolboxItemFilter("System.Windows.Forms"), Description("MBPanel Enables You to Group Collection Of Controls.")]
[DefaultEvent("Click"), DefaultProperty("Text")]
public class MBPanel : System.Windows.Forms.Panel
{
	#region "Generic component"
	/// <summary>
	/// Initialize Components for MBPanel
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	private void InitializeComponent()
	{
		this.SuspendLayout();
		//
		//MBGlassStylePanel
		//
		this.ClientSize = new System.Drawing.Size(200, 100);
		this.BorderStyle = MBBorderStyle.None;
		this.Name = "MBPanel";
		this.Text = "MBPanel";
		this.ResumeLayout(false);

	}
	/// <summary>
	/// Constructor for MBPanel
	/// </summary>
	public MBPanel() : base()
	{
		Paint += Panel_Paint;
		InitializeComponent();
		SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		SetStyle(ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Opaque, false);
		SetStyle(ControlStyles.EnableNotifyMessage, true);
		SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		BackColor = Color.Transparent;
	}
	/// <summary>
	/// Paint BackgroundSurface and Border for MBPanel
	/// </summary>
	/// <param name="sender">ByVal sender As Object</param>
	/// <param name="e">e As System.Windows.Forms.PaintEventArgs</param>
	private void Panel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		PaintBackgoundSurface(this, e);
		PaintBorder(this, e);
	}
	/// <summary>
	/// WndProc for MBPanel
	/// </summary>
	/// <param name="m">ByRef m As System.Windows.Forms.Message</param>
	protected override void WndProc(ref System.Windows.Forms.Message m)
	{
		base.WndProc(ref m);
	}

	#endregion

	#region "Enumerations"

	public enum MBBorderStyle
	{
		None,
		Rounded
	}

	[Flags()]
	public enum Corner
	{
		None = 0,
		TopLeft = 1,
		TopRight = 2,
		BottomLeft = 4,
		BottomRight = 8,
		All = TopLeft | TopRight | BottomLeft | BottomRight,
		AllTop = TopLeft | TopRight,
		AllLeft = TopLeft | BottomLeft,
		AllRight = TopRight | BottomRight,
		AllBottom = BottomLeft | BottomRight
	}

	#endregion

	#region "Properties"
	/// <summary>
	/// Get or Set Opacity of MBPanel
	/// </summary>
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
	/// <summary>
	/// Get or Set Glass Color for MBPanel
	/// </summary>
	private Color clGlassColor = Color.Black;
	[Category("Glass"), Description("Defines the color of the glass. Among good choices (WhiteSmoke, AliceBlue, MistyRose, AntiqueWhite, Ivory, HoneyDew, Lavender")]
	public Color GlassColor {
		get { return clGlassColor; }
		set {
			clGlassColor = value;
			Invalidate();
		}
	}
	/// <summary>
	/// Get or Set Border Style for MBPanel
	/// </summary>
	private MBBorderStyle _BorderStyle = MBBorderStyle.None;
	public MBBorderStyle BorderStyle {
		get { return _BorderStyle; }
		set {
			_BorderStyle = value;
			this.Invalidate();
		}
	}

	#endregion

	#region "Private Properties"
	/// <summary>
	/// Get Effective Bounds Of MBPanel
	/// </summary>
	/// <returns>Returns Value As RectangleF</returns>
	[Browsable(false)]
	public RectangleF EffectiveBounds {
		get { return new RectangleF(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2); }
	}
	/// <summary>
	/// Get RoundedSurface Of MBPanel
	/// </summary>
	[Browsable(false)]
	public GraphicsPath RoundSurface {
		get { return RoundCorners(EffectiveBounds, Radius); }
	}
	/// <summary>
	/// Get Rounded Surface Inner of MBPanel
	/// </summary>
	[Browsable(false)]
	public GraphicsPath RoundSurfaceInner {
		get {
			RectangleF rect = EffectiveBounds;
			rect.Inflate(-1, -1);
			return RoundCorners(rect, Radius);
		}
	}
	/// <summary>
	/// Get InnerRefion of MBPanel
	/// </summary>
	[Browsable(false)]
	public Region InnerRegion {
		get {
			Region rgInnerRegion = new Region(RoundSurface);
			return rgInnerRegion;
		}
	}
	/// <summary>
	/// Get Outer Region of MBPanel
	/// </summary>
	[Browsable(false)]
	public Region OuterRegion {
		get {
			Region rgOuterRegion = new Region(RoundSurface);
			rgOuterRegion.Xor(ClientRectangle);
			return rgOuterRegion;
		}
	}
	/// <summary>
	/// Get or Set Corner Radius Of MBPanel
	/// </summary>
	private float snRadius = 5;
	[Browsable(true)]
	public float Radius {
		get { return snRadius; }
		set {
			if (value > 0) {
				snRadius = value;
			} else {
				snRadius = 1;
			}
		}
	}

	#endregion

	#region "Private Methods"
	/// <summary>
	/// Paint Backround Surface Of MBPanel
	/// </summary>
	private void PaintBackgoundSurface(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		var _with1 = e.Graphics;
		_with1.SmoothingMode = SmoothingMode.HighQuality;
		_with1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
		_with1.CompositingQuality = CompositingQuality.HighQuality;
		SolidBrush _GlassBrush = new SolidBrush(Color.FromArgb(CheckOpacity(Opacity), GlassColor.R, GlassColor.G, GlassColor.B));
		e.Graphics.FillPath(_GlassBrush, RoundSurface);
		PaintHorizontalSurface(sender, e);
		PaintGlowSurface(sender, e);
		PaintReflectiveBands(sender, e);
		PaintLightSource(sender, e);
	}
	/// <summary>
	/// Paint ReflectiveBands for MBPanel
	/// </summary>
	private void PaintReflectiveBands(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		SolidBrush _GlassReflectBrush = new SolidBrush(Color.FromArgb(CheckOpacity(int.Parse((Opacity * 0.5).ToString())), GlassColor.R, GlassColor.G, GlassColor.B));
		GraphicsPath BandGraphicsPath1 = CreateReflectiveBand(0.1f, 0.5f, 0.15f);
		GraphicsPath BandGraphicsPath2 = CreateReflectiveBand(0.4f, 0.8f, 0.1f);
		e.Graphics.FillPath(_GlassReflectBrush, BandGraphicsPath1);
		e.Graphics.FillPath(_GlassReflectBrush, BandGraphicsPath2);
	}
	/// <summary>
	/// Paint HorizontalSurface for MBPanel
	/// </summary>
	private void PaintHorizontalSurface(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		SolidBrush _GlassDarkBrush = new SolidBrush(Color.FromArgb(int.Parse((Opacity * 0.5).ToString()), DeductMinZero(GlassColor.R, 50), DeductMinZero(GlassColor.G, 50), DeductMinZero(GlassColor.B, 50)));
		e.Graphics.ExcludeClip(new Rectangle(int.Parse(EffectiveBounds.Left.ToString()), int.Parse(EffectiveBounds.Top.ToString()), int.Parse(EffectiveBounds.Width.ToString()), int.Parse((EffectiveBounds.Height * 0.66f).ToString())));
		e.Graphics.FillPath(_GlassDarkBrush, RoundSurface);
		e.Graphics.ResetClip();
	}
	/// <summary>
	/// Paint Glow Surface for MBPanel
	/// </summary>
	private void PaintGlowSurface(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		LinearGradientBrush _GlassDarkLinearBrush = new LinearGradientBrush(ClientRectangle, Color.FromArgb(0, DeductMinZero(GlassColor.R, 50), DeductMinZero(GlassColor.G, 50), DeductMinZero(GlassColor.B, 50)), Color.FromArgb(CheckOpacity(int.Parse((Opacity * 2.5).ToString())), GlassColor.R, GlassColor.G, GlassColor.B), LinearGradientMode.Vertical);
		e.Graphics.FillPath(_GlassDarkLinearBrush, RoundSurfaceInner);
	}
	/// <summary>
	/// CreateReflective Bands for MBPanel
	/// </summary>
	private GraphicsPath CreateReflectiveBand(float LeftFactor, float RightFactor, float SizeFactor)
	{
		GraphicsPath BandGraphicsPath = new GraphicsPath();
		var _with2 = BandGraphicsPath;
		_with2.StartFigure();
		_with2.AddLine(2 + (EffectiveBounds.Width * LeftFactor), 2, 2 + (EffectiveBounds.Width * LeftFactor) + (EffectiveBounds.Width * SizeFactor), 2);
		_with2.AddLine((2 + (EffectiveBounds.Width * LeftFactor)) + (EffectiveBounds.Width * SizeFactor), 2, (2 + (EffectiveBounds.Width * RightFactor)) + (EffectiveBounds.Width * SizeFactor), EffectiveBounds.Top + EffectiveBounds.Height);
		_with2.AddLine((2 + (EffectiveBounds.Width * RightFactor) + (EffectiveBounds.Width * SizeFactor)), 2 + EffectiveBounds.Height, EffectiveBounds.Left + (EffectiveBounds.Width * RightFactor), EffectiveBounds.Top + EffectiveBounds.Height);
		_with2.AddLine(2 + (EffectiveBounds.Width * RightFactor), 2 + EffectiveBounds.Height, 2 + (EffectiveBounds.Width * LeftFactor), 2);
		_with2.CloseFigure();
		return BandGraphicsPath;
	}
	/// <summary>
	/// Paint Light Source for MBPanel
	/// </summary>
	private void PaintLightSource(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		RectangleF _RectangleLight = GetLightBounds(float.Parse((0.75).ToString()));
		GraphicsPath _LightGraphicsPath = new GraphicsPath();
		_LightGraphicsPath.StartFigure();
		_LightGraphicsPath.AddEllipse(_RectangleLight);
		PathGradientBrush brLight = new PathGradientBrush(_LightGraphicsPath);
		brLight.CenterColor = Color.FromArgb(CheckOpacity(Opacity * 3), 255, 255, 255);
		brLight.SurroundColors = new Color[] { Color.FromArgb(0, 255, 255, 255) };
		e.Graphics.ExcludeClip(OuterRegion);
		e.Graphics.FillEllipse(brLight, _RectangleLight);
		e.Graphics.ResetClip();
	}
	/// <summary>
	/// Returns Light Band for MBPanel
	/// </summary>
	private RectangleF GetLightBounds(float Size)
	{
		return new RectangleF(2 - ((EffectiveBounds.Height * Size) / 2), 2 - ((EffectiveBounds.Height * Size) / 2), EffectiveBounds.Height * Size, EffectiveBounds.Height * Size);
	}
	/// <summary>
	/// Paint Border of MBPanel
	/// </summary>
	private void PaintBorder(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		if (_BorderStyle == MBBorderStyle.Rounded) {
			e.Graphics.DrawPath(new Pen(Color.FromArgb(200, 255, 255, 255), 0.5f), RoundSurface);
			e.Graphics.DrawPath(new Pen(Color.FromArgb(255, 0, 0, 0), 0.5f), RoundSurfaceInner);
		}
	}

	#endregion

	#region "Public Functions"

	public int CheckOpacity(int Value)
	{
		if (Value > 255) {
			return 255;
		} else if (Value < 0) {
			return 0;
		} else {
			return Value;
		}
	}

	public int DeductMinZero(int Value, int Deduction)
	{
		if (Value - Deduction < 0) {
			return 0;
		} else {
			return Value - Deduction;
		}
	}

	public GraphicsPath RoundCorners(RectangleF Rectangle, float Radius = 5, Corner Corners = Corner.All)
	{
		GraphicsPath p = new GraphicsPath();
		float x = Rectangle.X;
		float y = Rectangle.Y;
		float w = Rectangle.Width;
		float h = Rectangle.Height;
		float r = Radius;
		p.StartFigure();
		if (Convert.ToBoolean(Corners & Corner.TopLeft)) {
			p.AddArc(new RectangleF(x, y, 2 * r, 2 * r), 180, 90);
		} else {
			p.AddLine(new PointF(x, y + r), new PointF(x, y));
			p.AddLine(new PointF(x, y), new PointF(x + r, y));
		}
		p.AddLine(new PointF(x + r, y), new PointF(x + w - r, y));
		if (Convert.ToBoolean(Corners & Corner.TopRight)) {
			p.AddArc(new RectangleF(x + w - 2 * r, y, 2 * r, 2 * r), 270, 90);
		} else {
			p.AddLine(new PointF(x + w - r, y), new PointF(x + w, y));
			p.AddLine(new PointF(x + w, y), new PointF(x + w, y + r));
		}
		p.AddLine(new PointF(x + w, y + r), new PointF(x + w, y + h - r));
		if (Convert.ToBoolean(Corners & Corner.BottomRight)) {
			p.AddArc(new RectangleF(x + w - 2 * r, y + h - 2 * r, 2 * r, 2 * r), 0, 90);
		} else {
			p.AddLine(new PointF(x + w, y + h - r), new PointF(x + w, y + h));
			p.AddLine(new PointF(x + w, y + h), new PointF(x + w - r, y + h));
		}
		p.AddLine(new PointF(x + w - r, y + h), new PointF(x + r, y + h));
		if (Convert.ToBoolean(Corners & Corner.BottomLeft)) {
			p.AddArc(new RectangleF(x, y + h - 2 * r, 2 * r, 2 * r), 90, 90);
		} else {
			p.AddLine(new PointF(x + r, y + h), new PointF(x, y + h));
			p.AddLine(new PointF(x, y + h), new PointF(x, y + h - r));
		}
		p.AddLine(new PointF(x, y + h - r), new PointF(x, y + r));
		p.CloseFigure();
		return p;
	}

	#endregion

}

#endregion
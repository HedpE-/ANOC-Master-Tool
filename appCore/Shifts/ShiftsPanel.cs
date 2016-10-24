/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 20-06-2016
 * Time: 22:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Collections.Generic;
using appCore.UI;
//using RoundedRectangles;

namespace appCore.Shifts
{
	public class ShiftsPanel : Panel
	{
		[Flags]
		public enum Borders
		{
			Top = 1,
			Left = 2,
			Right = 4,
			Bottom = 8,
			None = 16,
			All = Top | Left | Right | Bottom
		}

		[Flags]
		public enum Corners
		{
			TopLeft = 1,
			TopRight = 2,
			BottomLeft = 4,
			BottomRight = 8,
			None = 16,
			All = TopLeft | TopRight | BottomLeft | BottomRight
		}

		static int _edge = 25;
		[Browsable(true)]
		public int CornerSize {
			get { return _edge; }
			set {
				_edge = value;
				Invalidate();
			}
		}
		
		static int edgeTopLeft;
		static int edgeTopRight;
		static int edgeBottomLeft;
		static int edgeBottomRight;
		
		static Corners _roundCorners = Corners.TopLeft | Corners.TopRight | Corners.BottomRight | Corners.BottomLeft;
		[Browsable(true)]
		public virtual Corners CornersToRound {
			get {
				return _roundCorners;
			}
			set {
				_roundCorners = value;
				Invalidate();
			}
		}

		static Borders _drawBorders = Borders.Top | Borders.Right | Borders.Bottom | Borders.Left;
		[Browsable(true)]
		public virtual Borders BordersToDraw {
			get {
				return _drawBorders;
			}
			set {
				_drawBorders = value;
				Invalidate();
			}
		}

		static float _penWidth = 2f;
		[Browsable(true)]
		public float BorderWidth {
			get { return _penWidth; }
			set {
				_penWidth = value;
				pen = new Pen(_borderColor, _penWidth);
				Invalidate();
			}
		}

		static Color _borderColor = Color.White;
		[Browsable(true)]
		public Color BorderColor {
			get { return _borderColor; }
			set {
				_borderColor = value;
				pen = new Pen(_borderColor, _penWidth);
				Invalidate();
			}
		}

		Pen pen = new Pen(_borderColor, _penWidth);

		public bool DoubleBufferActive {
			get { return DoubleBuffered; }
			set {
				DoubleBuffered = value;
				Invalidate();
			}
		}
		
		GraphicsPath path;
		
		public ShiftsPanel()
		{
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			calculate4Corners();
			ExtendedDraw(e);
			DrawBorder(e.Graphics);
			base.OnPaint(e);
		}
		
//	protected override void OnPaint(PaintEventArgs e)
//	{
//		base.OnPaint(e);
//
//		int strokeOffset = Convert.ToInt32(Math.Ceiling(BorderWidth));
//		GraphicsPath borderPath = RoundedRectangle.Create(0, 0, Width + strokeOffset, Height + strokeOffset, CornerSize/2, RoundedRectangle.RectangleCorners.BottomLeft | RoundedRectangle.RectangleCorners.BottomRight );
//		Region = new Region(borderPath);
//		e.Graphics.DrawPath(Pens.Black, borderPath);
//		GraphicsPath containerPath = RoundedRectangle.Create(ClientRectangle, CornerSize/2, RoundedRectangle.RectangleCorners.BottomLeft | RoundedRectangle.RectangleCorners.BottomRight );
//		e.Graphics.FillPath(new SolidBrush(BackColor), containerPath);
//	}
		
		void calculate4Corners() {
			edgeTopLeft = 0;
			edgeTopRight = 0;
			edgeBottomLeft = 0;
			edgeBottomRight = 0;
			if ((Corners.None & CornersToRound) != Corners.None) {
				List<Corners> selCorners = new List<Corners>();
				foreach (Corners x in Enum.GetValues(typeof(Corners))) {
					if ((x & CornersToRound) == x)
						selCorners.Add(x);
				}
				foreach (Corners element in selCorners) {
					switch (element) {
						case Corners.TopLeft:
							edgeTopLeft = CornerSize;
							break;
						case Corners.TopRight:
							edgeTopRight = CornerSize;
							break;
						case Corners.BottomLeft:
							edgeBottomLeft = CornerSize;
							break;
						case Corners.BottomRight:
							edgeBottomRight = CornerSize;
							break;
					}
				}
			}
		}

		Rectangle GetLeftUpper(int e)
		{
			return new Rectangle(0, 0, e, e);
		}

		Rectangle GetRightUpper(int e)
		{
			return new Rectangle(Width - e, 0, e, e);
		}

		Rectangle GetRightLower(int e)
		{
			return new Rectangle(Width - e, Height - e, e, e);
		}

		Rectangle GetLeftLower(int e)
		{

			return new Rectangle(0, Height - e, e, e);
		}

		void ExtendedDraw(PaintEventArgs e)
		{
			
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			path = e.Graphics.GenerateRoundedRectangle(ClientRectangle,CornerSize/2,RectangleEdgeFilter.BottomLeft | RectangleEdgeFilter.BottomRight);
//		DrawBorder(e.Graphics);
//		path = new GraphicsPath();
//		path.StartFigure();
//
//		if(CornersToRound.HasFlag(Corners.TopLeft))
//			path.AddArc(GetLeftUpper(edgeTopLeft), 180, 90);
//
			////		Borders.Top
//		path.AddLine(edgeTopLeft, 0, Width - (edgeTopLeft + edgeTopRight/2), 0);
//
//		if(CornersToRound.HasFlag(Corners.TopRight))
//			path.AddArc(GetRightUpper(edgeTopRight), 270, 90);
//
			////		Borders.Right
//		path.AddLine(Width, edgeTopRight, Width, Height - (edgeTopRight + edgeBottomRight));
//
//		if(CornersToRound.HasFlag(Corners.BottomRight))
//			path.AddArc(GetRightLower(edgeBottomRight), 0, 90);
//
			////		Borders.Bottom
//		path.AddLine(Width - edgeBottomRight, Height, edgeBottomLeft, Height);
//
//		if(CornersToRound.HasFlag(Corners.BottomLeft))
//			path.AddArc(GetLeftLower(edgeBottomLeft), 90, 90);
//
			////		Borders.Left
//		path.AddLine(0, Height - edgeBottomLeft, 0, 0);
//
//		path.CloseFigure();
			Region = new Region(path);
		}

		void DrawBorder(Graphics graphics)
		{
			if((Borders.None & BordersToDraw) != Borders.None) {
				int strokeOffset = Convert.ToInt32(Math.Ceiling(BorderWidth));
//			Bounds = Rectangle.Inflate(ClientRectangle, -strokeOffset, -strokeOffset);

				// Set a new rectangle to the same size as the button's
				// ClientRectangle property.
//			RectangleF newRectangle = path.GetBounds();
//
//			// Decrease the size of the rectangle.
//			newRectangle.Inflate(-strokeOffset, -strokeOffset);
//
//			// Draw the button's border.
//			graphics.DrawRectangle(pen, Rectangle.Round(newRectangle));
//
//			// Increase the size of the rectangle to include the border.
//			newRectangle.Inflate(strokeOffset, strokeOffset);
//
//			// Set the button's Region property to the newly created
//			// circle region.
//			path.Dispose();
//			path.AddRectangle(newRectangle);
//			Region = new Region(path);
				
				pen.EndCap = pen.StartCap = LineCap.Round;

				SmoothingMode old = graphics.SmoothingMode;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				
				if ((Corners.TopLeft & CornersToRound) == Corners.TopLeft && ((Borders.Top & BordersToDraw) == Borders.Top || (Borders.Left & BordersToDraw) == Borders.Left))
					graphics.DrawArc(pen, new Rectangle(0, 0, edgeTopLeft, edgeTopLeft), 180, 90);
				if ((Corners.TopRight & CornersToRound) == Corners.TopLeft && ((Borders.Top & BordersToDraw) == Borders.Top || (Borders.Right & BordersToDraw) == Borders.Right))
					graphics.DrawArc(pen, new RectangleF(Width - edgeTopRight, 0, edgeTopRight, edgeTopRight), 270, 90);
				if ((Corners.BottomRight & CornersToRound) == Corners.BottomRight && ((Borders.Bottom & BordersToDraw) == Borders.Bottom || (Borders.Right & BordersToDraw) == Borders.Right))
					graphics.DrawArc(pen, new RectangleF(Width - edgeBottomRight, Height - edgeBottomRight - BorderWidth, edgeBottomRight, edgeBottomRight), 0, 90);
				if ((Corners.BottomLeft & CornersToRound) == Corners.BottomLeft && ((Borders.Bottom & BordersToDraw) == Borders.Bottom || (Borders.Left & BordersToDraw) == Borders.Left))
					graphics.DrawArc(pen, new RectangleF(0, Height - edgeBottomLeft - BorderWidth, edgeBottomLeft, edgeBottomLeft), 90, 90);
				if((Borders.Top & BordersToDraw) == Borders.Top)
					graphics.DrawLine(pen,new PointF(edgeTopLeft/2,0),new PointF(Width - (edgeTopRight/2),0));
				if((Borders.Right & BordersToDraw) == Borders.Right)
					graphics.DrawLine(pen,new PointF(Width - BorderWidth, (edgeTopRight/2)),new PointF(Width - BorderWidth, Height - (edgeBottomRight/2)));
				if((Borders.Bottom & BordersToDraw) == Borders.Bottom)
					graphics.DrawLine(pen, new PointF(edgeBottomLeft/2, Height - BorderWidth), new PointF(Width - (edgeBottomRight/2), Height - BorderWidth));
				if((Borders.Left & BordersToDraw) == Borders.Left)
					graphics.DrawLine(pen, new PointF(0, edgeTopLeft/2), new PointF(0, Height - ((edgeBottomLeft/2)/2)));
//			graphics.DrawPath(pen, path);
				graphics.SmoothingMode = old;
			}
		}
		
//	protected override CreateParams CreateParams {
//		get {
//			var parms = base.CreateParams;
//			parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
//			return parms;
//		}
//	}
	}
}
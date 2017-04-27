using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace appCore.UI
{
    [Designer(typeof(TranspControlDesigner))]
    public class TranspControl: UserControl
    {
        public bool drag = false;
        private Image backImage = null;
        private Color fillColor = Color.White;
        private Color backColor = Color.Transparent;
        private Color transpKey = Color.White;
		private int opacity = 100;
        private int lineWidth = 2;
        private int alpha;
        private bool glassMode = true;
		
		public TranspControl()
		{
            //Set style for double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint, true);
        }


        [Browsable(false)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        public Image BackImage
        {
            get { return this.backImage; }
            set
            {
                this.backImage = value;
                this.Invalidate();
            }
        }

        public Color TranspKey
        {
            get { return this.transpKey; }
            set
            {
                this.transpKey = value;
                this.Invalidate();
            }
        }
		
        public Color GlassColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value; 
                this.Invalidate();
		    }
		}

        public bool GlassMode
        {
            get { return this.glassMode; }
            set
            {
                this.glassMode = value;
                this.Invalidate();
            }
        }

		public Color FillColor
		{
			get { return this.fillColor; }
			set
			{
				this.fillColor = value;
                this.Invalidate();
			}
		}

        public int LineWidth
        {
            get { return this.lineWidth; }
            set
            {
                this.lineWidth = value;
                this.Invalidate();
            }
        }

		public int Opacity
		{
			get
			{
				if (opacity > 100) {opacity = 100;}
				else if (opacity < 1) {opacity = 0;}
				return this.opacity;
			}
			set
			{
				this.opacity = value;
                this.Invalidate();
			}
		}
      
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;

            if (Parent != null && !drag)
            {
                BackColor = Color.Transparent;
                int index = Parent.Controls.GetChildIndex(this);

                for (int i = Parent.Controls.Count - 1; i > index; i--)
                {
                    Control c = Parent.Controls[i];
                    if (c.Bounds.IntersectsWith(Bounds) && c.Visible)
                    {
                        Bitmap bmp = new Bitmap(c.Width, c.Height, g);
                        c.DrawToBitmap(bmp, c.ClientRectangle);

                        g.TranslateTransform(c.Left - Left, c.Top - Top);
                        g.DrawImageUnscaled(bmp, Point.Empty);
                        g.TranslateTransform(Left - c.Left, Top - c.Top);
                        bmp.Dispose();
                    }
                }
            }
            else
            {
                g.Clear(Parent.BackColor);
                g.FillRectangle(new SolidBrush(Color.FromArgb(opacity * 255 / 100, GlassColor)),
                                               this.ClientRectangle);
            }

            if (BackImage != null && GlassMode)
            {
                Bitmap image = new Bitmap(BackImage);
                image.MakeTransparent(TranspKey);

                float a = (float)opacity / 100.0f;

                float[][] mtxItens = {
                new float[] {1,0,0,0,0},
                new float[] {0,1,0,0,0},
                new float[] {0,0,1,0,0},
                new float[] {0,0,0,a,0},
                new float[] {0,0,0,0,1}};
                ColorMatrix colorMatrix = new ColorMatrix(mtxItens);

                ImageAttributes imgAtb = new ImageAttributes();
                imgAtb.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);

                g.DrawImage(
                        image,
                        ClientRectangle,
                        0.0f,
                        0.0f,
                        image.Width,
                        image.Height,
                        GraphicsUnit.Pixel,
                        imgAtb);
            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            Rectangle pBounds = this.Bounds;
            pBounds.Inflate(pBounds.Width/2, pBounds.Height/2);
            this.Invalidate();
            if (this.Parent != null) this.Parent.Invalidate(pBounds, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Rectangle pBounds = this.Bounds;
            pBounds.Inflate(pBounds.Width/2, pBounds.Height/2);
            this.Invalidate();
            if (this.Parent != null) this.Parent.Invalidate(pBounds, true);
        }
		
		protected override void OnPaint(PaintEventArgs e)
		{
            base.OnPaint(e);
        
            ///////////////////////////////
            //         SETTINGS          //
            ///////////////////////////////

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.GammaCorrected;

            RectangleF bounds = this.ClientRectangle;
            alpha = (opacity * 255) / 100;
         
            float penWidth = (float)LineWidth;
            Pen pen = new Pen(Color.FromArgb(alpha, ForeColor), penWidth);
            pen.Alignment = PenAlignment.Center;

            Brush brushColor = new SolidBrush(Color.FromArgb(alpha, FillColor));
            Brush bckColor = new SolidBrush(Color.FromArgb(alpha, GlassColor));


            ///////////////////////////////
            //    DRAW YOUR SHAPE HERE   //
            ///////////////////////////////

            GraphicsPath shape = new GraphicsPath();
            GraphicsPath regionShape = new GraphicsPath();
            GraphicsPath innerShape = new GraphicsPath();
            
            // Create a shape region for non glass mode
            regionShape.AddEllipse(bounds);
            Region region = new Region(regionShape);

            // Create the inner region for non glass mode
            RectangleF inner = bounds;
            inner.Inflate(-penWidth, -penWidth);
            inner.Inflate(-2.0f, -2.0f);
            innerShape.AddEllipse(inner);
            Region innerRegion = new Region(innerShape);
         
            // Fill the region background
            if (GlassMode)
            {
                Region = new Region();
                if (GlassColor != Color.Transparent && Opacity > 0)
                {
                    g.FillRegion(bckColor, Region);
                }
            }
            else
            {             
                // Make a hole inside the shape if FillColor is transparent
                if (FillColor == Color.Transparent || Opacity == 0)
                {
                    region.Exclude(innerRegion);       
                }
                Region = region;
            }

            // Add a shape to the path
            bounds.Inflate(-1.0f, -1.0f); //fit the ellipse inside the region
            shape.AddEllipse(bounds);

            // Fill the shape with a color
            if (FillColor != Color.Transparent && Opacity > 0)
            {
                g.FillPath(brushColor, shape);
            }

            // Draw the shape outline
            bounds.Inflate(-penWidth / 2.0f, -penWidth / 2.0f);
            if (ForeColor != Color.Transparent && Opacity > 0)
            {
                g.DrawEllipse(pen, bounds);
            }

            ///////////////////////////////
            //       FREES MEMORY        //
            ///////////////////////////////

            brushColor.Dispose();
            bckColor.Dispose();
			pen.Dispose();
            regionShape.Dispose();
            innerShape.Dispose();
            shape.Dispose();
            innerRegion.Dispose();
            region.Dispose();
		}
    }

    internal class TranspControlDesigner : ControlDesigner
    {
        private TranspControl control;


        protected override void OnMouseDragBegin(int x, int y)
        {
            base.OnMouseDragBegin(x, y);
            control = (TranspControl)(this.Control);
            control.drag = true;
           
        }
      
        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            control = (TranspControl)(this.Control);
            control.drag = false;
           
      }
   
   }

}

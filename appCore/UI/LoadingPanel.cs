/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 16/12/2016
 * Time: 19:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of LoadingPanel.
	/// </summary>
	public class LoadingPanel
	{
        Panel innerPanel;

        PictureBox loadingBox;

        Control ParentControl;

        public int SpinnerSize
        {
            get;
            set;
        } = 32;
        
		public double Opacity
		{
            get;
            set;
        } = 0.4;

        public bool Visible
        {
            get
            {
                return innerPanel != null;
            }
        }

        public bool LoadingSpinnerVisible
        {
            get
            {
                return innerPanel.Controls.Contains(loadingBox);
            }
        }

        /// <summary>
        /// Show LoadingPanel with both Threaded and Non Threaded instructions.
        /// For Threaded or Non Threaded action only, pass the other action argument as null
        /// </summary>
        /// <param name="showLoading"></param>
        /// <param name="parentControl"></param>
        public void Show(bool showLoading, Control parentControl)
        {
            if (parentControl.Parent != null)
            {
                if (parentControl.Parent.Controls.Cast<Control>().Any(c => c.GetType() == typeof(LoadingPanel)))
                    throw new Exception("Another Loading Panel already exists in Parent.");
            }
            else
            {
                if (parentControl.Controls.Cast<Control>().Any(c => c.GetType() == typeof(LoadingPanel)))
                    throw new Exception("Another Loading Panel already exists in Control.");
            }

            ParentControl = parentControl;

            innerPanel = new Panel();
			darkenBackgroundForm(showLoading);
        }

        public void Close()
        {
            if (innerPanel != null)
            {
                if (innerPanel.Parent != null)
                {
                    innerPanel.Parent.Controls.Remove(innerPanel);
                    innerPanel.Dispose();
                }
            }
        }

        void darkenBackgroundForm(bool showLoading)
        {
            Form parentForm = ParentControl.FindForm();
            //if(!(parentControl is Form) && !(parentControl is TabPage))
            if (!ParentControl.HasChildren)
            {
                if (ParentControl is AMTMenuStrip)
                {
                    parentForm.Controls.Add(innerPanel);
                    innerPanel.Location = parentForm.PointToClient(ParentControl.Parent.PointToScreen(ParentControl.Location));
                }
                else
                {
                    ParentControl.Parent.Controls.Add(innerPanel);
                    innerPanel.Location = ParentControl.Location;
                }
            }
            else
            {
                if (ParentControl.InvokeRequired)
                    ParentControl.Invoke((MethodInvoker)delegate
                    {
                        ParentControl.Controls.Add(innerPanel);
                    });
                else
                    ParentControl.Controls.Add(innerPanel);

                innerPanel.Location = Point.Empty;
            }
            // take a screenshot of the form and darken it
            Bitmap bmp;
            if (ParentControl is MainForm && parentForm.WindowState == FormWindowState.Minimized)
                bmp = ((MainForm)parentForm).ScreenshotBeforeMinimize;
            else
                bmp = new Bitmap(ParentControl.ClientRectangle.Width, ParentControl.ClientRectangle.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                if(ParentControl.InvokeRequired)
                    ParentControl.Invoke((MethodInvoker)delegate
                    {
                        g.CopyFromScreen(ParentControl.PointToScreen(new Point(0, 0)), new Point(0, 0), bmp.Size);
                    });
                else
                    g.CopyFromScreen(ParentControl.PointToScreen(new Point(0, 0)), new Point(0, 0), bmp.Size);

                Color darken = Color.FromArgb((int)(255 * Opacity), Color.Black);
                using (Brush brsh = new SolidBrush(darken))
                    g.FillRectangle(brsh, ParentControl.ClientRectangle);
            }
            //bmp.Save(Settings.GlobalProperties.AppDataRootDir + "\\bmp.jpg");
            innerPanel.Size = ParentControl.ClientRectangle.Size;
            innerPanel.BackgroundImage = bmp;

            innerPanel.BringToFront();
            if (showLoading)
                ToggleLoadingSpinner();
        }

        public void ToggleLoadingSpinner()
        {
            if (LoadingSpinnerVisible)
            {
                innerPanel.Controls.Remove(loadingBox);
                loadingBox.Dispose();
            }
            else
            {
                Point loc = innerPanel.PointToScreen(Point.Empty);
                loc.X = (ParentControl.Width - SpinnerSize) / 2;
                loc.Y = (ParentControl.Height - SpinnerSize) / 2;

                PictureBox loadingBox = new PictureBox();
                loadingBox.BackColor = Color.Transparent;
                loadingBox.Image = loadingBox.InitialImage = Resources.spinner1;
                loadingBox.Size = new Size(SpinnerSize, SpinnerSize);
                loadingBox.SizeMode = PictureBoxSizeMode.StretchImage;
                loadingBox.Location = loc;
                innerPanel.Controls.Add(loadingBox);
                loadingBox.BringToFront();
            }
        }
    }
}

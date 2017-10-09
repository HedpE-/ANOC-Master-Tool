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
using Transitions;

namespace appCore.UI
{
	/// <summary>
	/// Description of LoadingPanel.
	/// </summary>
	public class LoadingPanel : Panel
	{
        //Panel innerPanel;

        //PictureBox loadingBox;
        CircularProgressBar.CircularProgressBar loadingBox;

        Control ParentControl;

        static System.Windows.Forms.Timer tOut = new System.Windows.Forms.Timer();

        public double Opacity
		{
            get;
            set;
        } = 0.8;

        public bool IsVisible
        {
            get
            {
                return Visible;
            }
        }

        public bool IsLoadingSpinnerVisible
        {
            get
            {
                return Controls.Contains(loadingBox);
            }
        }

        public bool ShowLoadingSpinner
        {
            get;
            set;
        }

        public int LoadingSpinnerSize
        {
            get;
            set;
        } = 96;

        ProgressBarStyle loadingSpinnerStyle = ProgressBarStyle.Marquee;
        public ProgressBarStyle LoadingSpinnerStyle
        {
            get
            {
                return loadingSpinnerStyle;
            }
            set
            {
                loadingSpinnerStyle = value;
                if (loadingBox != null)
                    loadingBox.Style = loadingSpinnerStyle;
            }
        }

        int progressValue;
        public int LoadingSpinnerProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
                if (loadingBox != null)
                {
                    if (loadingBox.InvokeRequired)
                        loadingBox.Invoke((MethodInvoker)delegate
                        {
                            loadingBox.Value = progressValue;
                            loadingBox.Text = progressValue.ToString();
                        });
                    else
                    {
                        loadingBox.Value = progressValue;
                        loadingBox.Text = progressValue.ToString();
                    }
                }
            }
        }

        public LoadingPanel()
        {
            tOut.Enabled = false;
            tOut.Interval = 25;
            tOut.Tick += FadeOutTimerTick;
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
                if (parentControl.Parent.Controls.OfType<LoadingPanel>().Any())
                    throw new Exception("Another Loading Panel already exists in Parent.");
            }
            else
            {
                if (parentControl.Controls.OfType<LoadingPanel>().Any())
                    throw new Exception("Another Loading Panel already exists in Control.");
            }

            ParentControl = parentControl;
            
			darkenBackgroundForm(showLoading);
        }

        public void Close()
        {
            if (Parent != null)
            {
                //FadeOut();

                Parent.Controls.Remove(this);
                Dispose();
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
                    parentForm.Controls.Add(this);
                    Location = parentForm.PointToClient(ParentControl.Parent.PointToScreen(ParentControl.Location));
                }
                else
                {
                    ParentControl.Parent.Controls.Add(this);
                    Location = ParentControl.Location;
                }
            }
            else
            {
                if (ParentControl.InvokeRequired)
                    ParentControl.Invoke((MethodInvoker)delegate
                    {
                        ParentControl.Controls.Add(this);
                    });
                else
                    ParentControl.Controls.Add(this);
                
                Location = Point.Empty;
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
            if (ParentControl.InvokeRequired)
                ParentControl.Invoke((MethodInvoker)delegate
                {
                    Size = ParentControl.ClientRectangle.Size;
                });
            else
                Size = ParentControl.ClientRectangle.Size;
            BackgroundImage = bmp;

            BringToFront();
            if (showLoading)
                ToggleLoadingSpinner();
        }

        public void ToggleLoadingSpinner()
        {
            if (IsLoadingSpinnerVisible)
            {
                Controls.Remove(loadingBox);
                loadingBox.Dispose();
            }
            else
            {
                Point loc = PointToScreen(Point.Empty);
                loc.X = (ParentControl.Width - LoadingSpinnerSize) / 2;
                loc.Y = (ParentControl.Height - LoadingSpinnerSize) / 2;

                loadingBox = new CircularProgressBar.CircularProgressBar();

                loadingBox.Size = new Size(LoadingSpinnerSize, LoadingSpinnerSize);
                loadingBox.Location = loc;
                loadingBox.Style = LoadingSpinnerStyle;
                loadingBox.Value = loadingSpinnerStyle == ProgressBarStyle.Marquee ? 15 : LoadingSpinnerProgressValue;

                loadingBox.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
                loadingBox.Anchor = AnchorStyles.None;
                loadingBox.AnimationSpeed = 500;
                loadingBox.BackColor = Color.Transparent;
                loadingBox.ForeColor = Color.White;
                loadingBox.InnerColor = Color.White;
                loadingBox.InnerMargin = 0;
                loadingBox.InnerWidth = 0;
                loadingBox.MarqueeAnimationSpeed = 2000;
                loadingBox.OuterColor = Color.Black;
                loadingBox.OuterMargin = -12;
                loadingBox.OuterWidth = 15;
                if(Settings.CurrentUser.UserName == "GONCARJ3")
                    loadingBox.ProgressColor = Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(151)))), ((int)(((byte)(218)))));
                else
                    loadingBox.ProgressColor = Color.DarkRed;
                loadingBox.ProgressWidth = 12;
                loadingBox.SecondaryFont = new Font("Microsoft Sans Serif", 11.125F);
                loadingBox.StartAngle = 270;
                //loadingBox.SubscriptColor = Color.Gray;
                //loadingBox.SubscriptMargin = new Padding(0);
                loadingBox.SubscriptText = "";
                if (loadingSpinnerStyle != ProgressBarStyle.Marquee)
                {
                    loadingBox.SuperscriptColor = Color.LightGray;
                    loadingBox.SuperscriptMargin = new Padding(3, 22, 0, 0);
                    loadingBox.SuperscriptText = "%";
                }
                else
                    loadingBox.SuperscriptText = "";
                loadingBox.Font = new Font("Arial", ResolveLoadingSpinnerTextSize(), FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                loadingBox.Text = loadingSpinnerStyle == ProgressBarStyle.Marquee ? "PLEASE WAIT" : LoadingSpinnerProgressValue.ToString();
                loadingBox.TextMargin = loadingSpinnerStyle == ProgressBarStyle.Marquee ? new Padding(0, 2, 0, 0) : new Padding(0, 2, 0, 0);
                
                Controls.Add(loadingBox);

                //CircularProgressBar.CircularProgressBar circularProgressBar1 = null;
                //if (loadingSpinnerStyle != ProgressBarStyle.Marquee)
                //{
                //    circularProgressBar1 = new CircularProgressBar.CircularProgressBar();
                //    circularProgressBar1.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
                //    circularProgressBar1.AnimationSpeed = 500;
                //    circularProgressBar1.BackColor = System.Drawing.Color.Transparent;
                //    circularProgressBar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                //    circularProgressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                //    circularProgressBar1.InnerColor = System.Drawing.Color.Transparent;
                //    circularProgressBar1.InnerMargin = 2;
                //    circularProgressBar1.InnerWidth = -1;
                //    circularProgressBar1.Location = loc;
                //    circularProgressBar1.MarqueeAnimationSpeed = 2000;
                //    circularProgressBar1.Name = "circularProgressBar1";
                //    circularProgressBar1.OuterColor = System.Drawing.Color.Black;
                //    circularProgressBar1.OuterMargin = -10;
                //    circularProgressBar1.OuterWidth = 5;
                //    circularProgressBar1.ProgressColor = System.Drawing.Color.DarkRed;
                //    circularProgressBar1.ProgressWidth = 18;
                //    circularProgressBar1.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
                //    circularProgressBar1.Size = new System.Drawing.Size(LoadingSpinnerSize, LoadingSpinnerSize);
                //    circularProgressBar1.StartAngle = 270;
                //    circularProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
                //    circularProgressBar1.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
                //    circularProgressBar1.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
                //    circularProgressBar1.SubscriptText = "";
                //    circularProgressBar1.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
                //    circularProgressBar1.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
                //    circularProgressBar1.SuperscriptText = "";
                //    circularProgressBar1.TabIndex = 16;
                //    circularProgressBar1.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
                //    circularProgressBar1.Value = 2;

                //    Controls.Add(circularProgressBar1);
                //}

                loadingBox.BringToFront();
                //if (loadingSpinnerStyle != ProgressBarStyle.Marquee)
                //    circularProgressBar1.BringToFront();
            }
        }

        float ResolveLoadingSpinnerTextSize()
        {
            if(loadingBox!=null)
            {
                float size = 0;
                if (loadingBox.Style == ProgressBarStyle.Marquee)
                    return 5.75F;
                else
                    return 17.75F;
            }

            return 0;
        }

        void FadeOutTimerTick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
                this.Opacity -= 0.075;
            else
                tOut.Enabled = false;
        }

        void FadeOut()
        {
            tOut.Enabled = true;

            //while (tOut.Enabled) { }
        }
    }
}

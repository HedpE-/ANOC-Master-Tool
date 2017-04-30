/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 25/04/2017
 * Time: 10:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using OpenWeatherAPI;

namespace appCore.GeoAPIs.UI
{
	/// <summary>
	/// Description of WeatherPanel.
	/// </summary>
	public class WeatherPanel : Panel // appCore.UI.AMTRoundCornersPanel
	{
		appCore.UI.AMTTransparentLabel town = new appCore.UI.AMTTransparentLabel();
		appCore.UI.AMTTransparentLabel currentTemperature = new appCore.UI.AMTTransparentLabel();
		appCore.UI.AMTTransparentLabel maxMinTemperature = new appCore.UI.AMTTransparentLabel();
		appCore.UI.AMTTransparentLabel weatherCondition = new appCore.UI.AMTTransparentLabel();
		appCore.UI.AMTTransparentLabel weatherDescription = new appCore.UI.AMTTransparentLabel();
		
		Rectangle weatherPictureRectangle = new Rectangle(3, 36, 90, 90);
		
		Image weatherPicture;
		Image WeatherPicture {
			get { return weatherPicture; }
			set {
				weatherPicture = value;
				
				this.Refresh();
			}
		}
		
		private int opacity = 75;
		[System.ComponentModel.DefaultValue(75)]
		public int Opacity
		{
			get
			{
				return this.opacity;
			}
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentException("value must be between 0 and 100");
				this.opacity = value;
				this.Invalidate();
			}
		}
		
		Query currentWeatherQuery;
		
		public WeatherPanel()
		{
			SetStyle(ControlStyles.Opaque, true);
			InitializeComponent();
			// Resolve PEN root path
			System.IO.DriveInfo pen = System.IO.DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == System.IO.DriveType.Removable && d.VolumeLabel == "PEN");
			if(pen != null)
				WeatherPicture = Image.FromFile(pen.Name + @"Weather\01d.png");
			else {
				using(System.Net.WebClient wc = new System.Net.WebClient()) {
					System.Net.IWebProxy proxy = System.Net.WebRequest.GetSystemWebProxy();
					proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
					wc.Proxy = proxy;
					byte[] bytes = wc.DownloadData("http://openweathermap.org/img/w/01d.png");
					System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
					WeatherPicture = Image.FromStream(ms);
				}
			}
		}
		
		public WeatherPanel(Query weatherQuery)
		{
			SetStyle(ControlStyles.Opaque, true);
			InitializeComponent();
			currentWeatherQuery = weatherQuery;
			
			// Resolve PEN root path
			System.IO.DriveInfo pen = System.IO.DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == System.IO.DriveType.Removable && d.VolumeLabel == "PEN");
			if(pen != null)
				WeatherPicture = Image.FromFile(pen.Name + @"Weather\" + weatherQuery.Weathers[0].Icon + ".png");
			else {
				using(System.Net.WebClient wc = new System.Net.WebClient()) {
					System.Net.IWebProxy proxy = System.Net.WebRequest.GetSystemWebProxy();
					proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
					wc.Proxy = proxy;
					byte[] bytes = wc.DownloadData("http://openweathermap.org/img/w/" + weatherQuery.Weathers[0].Icon + ".png");
					System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
					WeatherPicture = Image.FromStream(ms);
				}
			}
			
//			town.Text = weatherQuery.Name;
			////			town.BackColor = Color.FromArgb(0, 0, 0, 0);
//			currentTemperature.Text = Math.Round(weatherQuery.Main.Temperature.CelsiusCurrent, 1, MidpointRounding.AwayFromZero) + "°C";
			////			currentTemperature.BackColor = Color.FromArgb(0, 0, 0, 0);
//			maxMinTemperature.Text = "Max: " + Math.Round(weatherQuery.Main.Temperature.CelsiusMaximum, 0, MidpointRounding.AwayFromZero) + "°C" + " Min: " + Math.Round(weatherQuery.Main.Temperature.CelsiusMinimum, 0, MidpointRounding.AwayFromZero) + "°C";
			////			maxMinTemperature.BackColor = Color.FromArgb(0, 0, 0, 0);
//			weatherCondition.Text = weatherQuery.Weathers.FirstOrDefault().Main.CapitalizeWords();
			////			weatherCondition.BackColor = Color.FromArgb(0, 0, 0, 0);
//			weatherDescription.Text = weatherQuery.Weathers.FirstOrDefault().Description.CapitalizeWords();
			////			weatherDescription.BackColor = Color.FromArgb(0, 0, 0, 0);
		}
		
//		protected void TickHandler(object sender, EventArgs e)
//		{
//			this.InvalidateEx();
//		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;

				cp.ExStyle |= 0x00000020 | 0x02000000; //WS_EX_TRANSPARENT | Turn on WS_EX_COMPOSITED
				return cp;
			}
		}
		
		void DrawWeatherPicture(PaintEventArgs e) {
//			g.FillRectangle(new SolidBrush( Color.FromArgb( 0, Color.Black ) ), weatherPictureRectangle );

			Point[] points = new Point[3];

			points[0] = new Point( weatherPictureRectangle.X, weatherPictureRectangle.Y );
			points[1] = new Point( weatherPictureRectangle.X, weatherPictureRectangle.Height );
//			points[2] = new Point( weatherPictureRectangle.Width, weatherPictureRectangle.Height);
			points[2] = new Point( weatherPictureRectangle.Width, weatherPictureRectangle.Y );

//			Brush brush = new SolidBrush( Color.DarkGreen );

			e.Graphics.DrawImage(WeatherPicture, points);
		}
		
		protected override void OnLayout(LayoutEventArgs e)
		{
			BringToFront();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Bitmap weatherSnap = GetBitmap();
//			Size = new Size(shiftsBodySnap.Width, shiftsBodySnap.Height);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.DrawImageUnscaled(weatherSnap, Point.Empty);
//			if(weatherPicture != null)
//				DrawWeatherPicture(e);
			BringToFront();
		}
		
		Bitmap GetBitmap() {
			Bitmap bm = new Bitmap(Width, Height);
			
			using (Graphics g = Graphics.FromImage(bm)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;    // Set format of string.
				g.FillRectangle(new SolidBrush(Color.FromArgb(Opacity * 255 / 100, BackColor)), ClientRectangle);
				
				StringFormat drawStringFormat = new StringFormat();
				drawStringFormat.Alignment = StringAlignment.Center;
				drawStringFormat.LineAlignment = StringAlignment.Far;
				using (Pen pen = new Pen(Color.Black, 1)) {
					// 
					// Town
					// 
					g.DrawString(
						currentWeatherQuery != null ? currentWeatherQuery.Name : "Town",
						new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold),
						Brushes.White,
						new Rectangle(new Point(0, 3), new Size(Width, 30)),
						drawStringFormat
					);
					// 
					// CurrentTemperature
					// 
					drawStringFormat.Alignment = StringAlignment.Near;
					drawStringFormat.LineAlignment = StringAlignment.Far;
					g.DrawString(
						currentWeatherQuery != null ? Math.Round(currentWeatherQuery.Main.Temperature.CelsiusCurrent, 1, MidpointRounding.AwayFromZero) + "°C" : "CurrentTemperature",
						new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(100, 35), new Size(Width - 100, 30)),
						drawStringFormat
					);
					// 
					// MaxMinTemperature
					// 
					drawStringFormat.LineAlignment = StringAlignment.Far;
					g.DrawString(
						currentWeatherQuery != null ? "Max: " + Math.Round(currentWeatherQuery.Main.Temperature.CelsiusMaximum, 0, MidpointRounding.AwayFromZero) + "°C" + " Min: " + Math.Round(currentWeatherQuery.Main.Temperature.CelsiusMinimum, 0, MidpointRounding.AwayFromZero) + "°C" : "MaxMinTemperature",
						new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(100, 65), new Size(Width - 100, 15)),
						drawStringFormat
					);
					// 
					// WeatherCondition
					// 
					drawStringFormat.LineAlignment = StringAlignment.Near;
					g.DrawString(
						currentWeatherQuery != null ? currentWeatherQuery.Weathers.FirstOrDefault().Main.CapitalizeWords() : "WeatherCondition",
						new Font("Microsoft Sans Serif", 12F, FontStyle.Bold),
						Brushes.White,
						new Rectangle(new Point(100, 85), new Size(Width - 100, 20)),
						drawStringFormat
					);
					// 
					// WeatherDescription
					// 
					g.DrawString(
						currentWeatherQuery != null ? currentWeatherQuery.Weathers.FirstOrDefault().Description.CapitalizeWords() : "WeatherDescription",
						new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(100, 110), new Size(Width - 100, 18)),
						drawStringFormat
					);

					if(WeatherPicture != null) {
						Point[] points = new Point[3];

						points[0] = new Point( weatherPictureRectangle.X, weatherPictureRectangle.Y );
						points[1] = new Point( weatherPictureRectangle.X, weatherPictureRectangle.Height );
						points[2] = new Point( weatherPictureRectangle.Width, weatherPictureRectangle.Y );

						g.DrawImage(WeatherPicture, points);
					}
				}
				if(Settings.CurrentUser.UserName == "GONCARJ3")
					bm.Save(Settings.UserFolder.FullName + @"\weatherSnap.png");
			}
			return bm;
		}
		
//		private Point previousLocation;
//
//		protected override void OnMouseDown(MouseEventArgs e)
//		{
//			previousLocation = e.Location;
//		}
//
//		protected override void OnMouseMove(MouseEventArgs e)
//		{
		////			if (activeControl == null || activeControl != sender)
		////				return;
//
//			var location = Parent.Location;
//			location.Offset(e.Location.X - previousLocation.X, e.Location.Y - previousLocation.Y);
//			Parent.Location = location;
//		}
		
		void InitializeComponent() {
//			// 
//			// town
//			// 
//			town.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
//			town.ForeColor = Color.White;
//			town.Location = new Point(3, 3);
//			town.Name = "town";
//			town.Text = "Town";
//			town.Size = new Size(242, 31);
//			town.TextAlign = ContentAlignment.TopCenter;
//			// 
//			// currentTemperature
//			// 
//			currentTemperature.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
//			currentTemperature.ForeColor = Color.White;
//			currentTemperature.Location = new Point(99, 36);
//			currentTemperature.Name = "currentTemperature";
//			currentTemperature.Text = "CurrentTemperature";
//			currentTemperature.Size = new Size(146, 25);
//			// 
//			// maxMinTemperature
//			// 
//			maxMinTemperature.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
//			maxMinTemperature.ForeColor = Color.White;
//			maxMinTemperature.Location = new Point(99, 63);
//			maxMinTemperature.Name = "maxMinTemperature";
//			maxMinTemperature.Text = "MaxMinTemperature";
//			maxMinTemperature.Size = new Size(146, 15);
//			// 
//			// weatherCondition
//			// 
//			weatherCondition.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
//			weatherCondition.ForeColor = Color.White;
//			weatherCondition.TextAlign = ContentAlignment.BottomLeft;
//			weatherCondition.Location = new Point(99, 85);
//			weatherCondition.Name = "weatherCondition";
//			weatherCondition.Text = "WeatherCondition";
//			weatherCondition.Size = new Size(146, 20);
//			// 
//			// weatherDescription
//			// 
//			weatherDescription.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
//			weatherDescription.ForeColor = Color.White;
//			weatherDescription.TextAlign = ContentAlignment.BottomLeft;
//			weatherDescription.Location = new Point(99, 108);
//			weatherDescription.Name = "weatherDescription";
//			weatherDescription.Text = "WeatherDescription";
//			weatherDescription.Size = new Size(146, 18);
			
//			Controls.AddRange(new Control[]{
//			                  	town,
//			                  	currentTemperature,
//			                  	maxMinTemperature,
//			                  	weatherCondition,
//			                  	weatherDescription
//			                  });
			Name = "WeatherPanel";
			Size = new Size(248, 178);
			BackColor = Color.Black;
		}
	}
}

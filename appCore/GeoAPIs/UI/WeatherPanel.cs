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
using appCore.UI;
using OpenWeatherAPI;

namespace appCore.GeoAPIs.UI
{
	/// <summary>
	/// Description of WeatherPanel.
	/// </summary>
	public class WeatherPanel : AMTRoundCornersPanel
	{
		Rectangle weatherPictureRectangle = new Rectangle(0, 35, 100, 100);
		
		Image weatherPicture;
		Image WeatherPicture {
			get { return weatherPicture; }
			set {
				weatherPicture = value;
				
				this.Refresh();
			}
        }

        public Image HumidityPicture;
        public Image TemperaturePicture;
        public Image CloudinessPicture;

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
		
		WeatherItem weatherQuery;
		
		public WeatherPanel()
		{
			SetStyle(ControlStyles.Opaque, true);
			InitializeComponent();
            if (Settings.GlobalProperties.ExternalResourceFilesLocation.Exists)
            {
                WeatherPicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"01d.png");
                if (System.IO.File.Exists(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\hygrometer.png"))
                    HumidityPicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\hygrometer.png");
                if (System.IO.File.Exists(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\thermometer.png"))
                    TemperaturePicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\thermometer.png");
                if (System.IO.File.Exists(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\cloudiness.png"))
                    CloudinessPicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\cloudiness.png");
            }
            else
            {
                // Resolve PEN root path
                System.IO.DriveInfo pen = System.IO.DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == System.IO.DriveType.Removable && d.VolumeLabel == "PEN");
                if (pen != null)
                    WeatherPicture = Image.FromFile(pen.Name + @"Weather\01d.png");
                else
                {
                    using (System.Net.WebClient wc = new System.Net.WebClient())
                    {
                        System.Net.IWebProxy proxy = System.Net.WebRequest.GetSystemWebProxy();
                        proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                        wc.Proxy = proxy;
                        byte[] bytes = wc.DownloadData("http://openweathermap.org/img/w/01d.png");
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
                        WeatherPicture = Image.FromStream(ms);
                    }
                }
            }
		}
		
		public WeatherPanel(WeatherItem weatherItem)
		{
			SetStyle(ControlStyles.Opaque, true);
			InitializeComponent();

            weatherQuery = weatherItem;

            if (Settings.GlobalProperties.ExternalResourceFilesLocation.Exists)
            {
                WeatherPicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + "\\" + weatherItem.CurrentWeather.weather[0].icon + ".png");
                if (System.IO.File.Exists(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\hygrometer.png"))
                    HumidityPicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\hygrometer.png");
                if (System.IO.File.Exists(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\thermometer.png"))
                    TemperaturePicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\thermometer.png");
                if (System.IO.File.Exists(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\cloudiness.png"))
                    CloudinessPicture = Image.FromFile(Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\cloudiness.png");
            }
            else
            {
                // Resolve PEN root path
                System.IO.DriveInfo pen = System.IO.DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == System.IO.DriveType.Removable && d.VolumeLabel == "PEN");
                if (pen != null)
                    WeatherPicture = Image.FromFile(pen.Name + @"Weather\" + weatherItem.CurrentWeather.weather[0].icon + ".png");
                else
                {
                    using (System.Net.WebClient wc = new System.Net.WebClient())
                    {
                        System.Net.IWebProxy proxy = System.Net.WebRequest.GetSystemWebProxy();
                        proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                        wc.Proxy = proxy;
                        byte[] bytes = wc.DownloadData("http://openweathermap.org/img/w/" + weatherItem.CurrentWeather.weather[0].icon + ".png");
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
                        WeatherPicture = Image.FromStream(ms);
                    }
                }
            }
        }

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
			Point[] points = new Point[4];

			points[0] = new Point( weatherPictureRectangle.X, weatherPictureRectangle.Y );
			points[1] = new Point( weatherPictureRectangle.X, weatherPictureRectangle.Height );
			points[2] = new Point( weatherPictureRectangle.Width, weatherPictureRectangle.Height);
			points[3] = new Point( weatherPictureRectangle.Width, weatherPictureRectangle.Y );

			e.Graphics.DrawImageUnscaled(WeatherPicture, points[0]);
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
            //base.OnPaint(e);
            Bitmap weatherSnap = GetBitmap();
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.DrawImageUnscaled(weatherSnap, Point.Empty);
//			if(Settings.CurrentUser.UserName == "GONCARJ3")
//				weatherSnap.Save(Settings.UserFolder.FullName + @"\weatherSnap.png");
			BringToFront();
		}
		
		Bitmap GetBitmap() {
			Bitmap bm = new Bitmap(Width, Height);
			
			using (Graphics g = Graphics.FromImage(bm)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;    // Set format of string.
				g.FillRectangle(new SolidBrush(Color.FromArgb(Opacity * 255 / 100, BackColor)), ClientRectangle);
				
				using (Pen pen = new Pen(Color.Black, 1)) {
                    StringFormat drawStringFormat = new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    FontFamily font = FontFamily.Families.FirstOrDefault(f => f.Name == "Calibri") ??
						FontFamily.Families.FirstOrDefault(f => f.Name == "Microsoft Sans Serif");
					// 
					// Town
					// 
					var rect = new Rectangle(Point.Empty, new Size(Width, 30));
					g.FillRectangle(new SolidBrush(Color.FromArgb(Opacity * 255 / 100, Color.DarkRed)), rect);
                    string text = weatherQuery != null ? weatherQuery.CurrentWeather.name : "Town";
                    g.DrawString(
						text,
						new Font(font, text.Length > 20 ? 13F : 15.75F, FontStyle.Bold),
						Brushes.White,
						rect,
						drawStringFormat
					);
                    // 
                    // TemperaturePicture
                    // 
                    if (TemperaturePicture != null)
                        g.DrawImage(TemperaturePicture, new Rectangle(new Point(102, 37), new Size(18, 18)));
                    // 
                    // CurrentTemperature
                    // 
                    drawStringFormat.Alignment = StringAlignment.Near;
					g.DrawString(
						weatherQuery != null ? Math.Round(weatherQuery.CurrentWeather.main.temperature.CelsiusCurrent, MidpointRounding.AwayFromZero) + "°C" : "CurrentTemperature",
						new Font(font, 15.75F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(120, 35), new Size(50, 25)),
						drawStringFormat
					);
					// 
					// MaxTemperature
					// 
					drawStringFormat.LineAlignment = StringAlignment.Near;
					g.DrawString(
						weatherQuery != null ? "Max: " + Math.Round(weatherQuery.Forecast5D3H.temp_min_max_5days[0].CelsiusMaximum, 0, MidpointRounding.AwayFromZero) + "°C" : "MaxTemperature",
						new Font(font, 8F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(170, 35), new Size(Width - 170, 12)),
						drawStringFormat
					);
					// 
					// MinTemperature
					// 
					drawStringFormat.LineAlignment = StringAlignment.Far;
					g.DrawString(
						weatherQuery != null ? "Min: " + Math.Round(weatherQuery.Forecast5D3H.temp_min_max_5days[0].CelsiusMinimum, 0, MidpointRounding.AwayFromZero) + "°C" : "MinTemperature",
						new Font(font, 8F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(170, 48), new Size(Width - 170, 12)),
						drawStringFormat
					);
                    // 
                    // WeatherDescription
                    // 
                    drawStringFormat.LineAlignment = StringAlignment.Center;
                    text = weatherQuery != null ? weatherQuery.CurrentWeather.weather.FirstOrDefault().description.CapitalizeWords() : "WeatherDescription";
                    g.DrawString(
						text,
						new Font(font, text.Length > 15 ? 9.5F : 11F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(100, text.Length > 12 ? 63 : 60), new Size(Width - 100, 18)),
						drawStringFormat
					);
                    // 
                    // HumidityPicture
                    // 
                    if (HumidityPicture != null)
                        g.DrawImage(HumidityPicture, new Rectangle(new Point(103, 86), new Size(16, 16)));
                    // 
                    // Humidity
                    // 
                    drawStringFormat.LineAlignment = StringAlignment.Near;
                    g.DrawString(
                        weatherQuery != null ? weatherQuery.CurrentWeather.main.humidity + "%" : "Humidity",
                        new Font(font, 12F, FontStyle.Regular),
                        Brushes.White,
                        new Rectangle(new Point(120, 85), new Size(43, 18)),
                        drawStringFormat
                    );
                    // 
                    // CloudsPicture
                    // 
                    if (CloudinessPicture != null)
                        g.DrawImage(CloudinessPicture, new Rectangle(new Point(162, 86), new Size(16, 16)));
                    // 
                    // Cloudiness
                    // 
                    g.DrawString(
                        weatherQuery != null ? weatherQuery.CurrentWeather.clouds.all + "%" : "Cloudiness",
                        new Font(font, 12F, FontStyle.Regular),
                        Brushes.White,
                        new Rectangle(new Point(182, 85), new Size(Width - 182, 18)),
                        drawStringFormat
                    );
                    // 
                    // WindDirectionPicture
                    // 
                    if (weatherQuery.CurrentWeather.wind.DirectionPicture != null)
                        g.DrawImage(weatherQuery.CurrentWeather.wind.DirectionPicture, new Rectangle(new Point(100, 110), new Size(18, 18)));
                    // 
                    // WindSpeed
                    // 
                    drawStringFormat.LineAlignment = StringAlignment.Near;
                    g.DrawString(
						weatherQuery != null ? weatherQuery.CurrentWeather.wind.SpeedMetersPerSecond + " m/s" : "WindSpeed",
						new Font(font, 12F, FontStyle.Regular),
						Brushes.White,
						new Rectangle(new Point(120, 110), new Size(Width - 120, 18)),
						drawStringFormat
					);

                    if (WeatherPicture != null)
						g.DrawImage(WeatherPicture, weatherPictureRectangle);
                    // 
                    // WeatherTimestamp
                    // 
                    drawStringFormat.LineAlignment = StringAlignment.Near;
                    drawStringFormat.Alignment = StringAlignment.Far;
                    g.DrawString(
                        weatherQuery != null ? "(" + weatherQuery.CurrentWeather.DataTimestamp.ToString("dd-MM-yyyy HH:mm:ss") + ")" : "",
                        new Font(font, 7.5F, FontStyle.Regular),
                        Brushes.LightGray,
                        new Rectangle(new Point(Width - 190, 131), new Size(170, 12)),
                        drawStringFormat
                    );
                }
//				if(Settings.CurrentUser.UserName == "GONCARJ3")
//					bm.Save(Settings.UserFolder.FullName + @"\weatherSnap.png");
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
			Size = new Size(240, 165);
			BackColor = Color.Black;
            CornersToRound = Corners.BottomLeft;
            BordersToDraw = Borders.None;
            DoubleBuffered = true;
		}
	}
}

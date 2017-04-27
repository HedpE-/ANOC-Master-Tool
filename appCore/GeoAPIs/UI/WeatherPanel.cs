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
		Label town = new Label();
		Label currentTemperature = new Label();
		Label maxMinTemperature = new Label();
		Label weatherCondition = new Label();
		Label weatherDescription = new Label();
		PictureBox weatherPicture = new PictureBox();
		
		private int opacity = 50;
		[System.ComponentModel.DefaultValue(50)]
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
			}
		}
		
		public WeatherPanel(Query weatherQuery)
		{
			SetStyle(ControlStyles.Opaque, true);
			InitializeComponent();
			BackColor = Color.Black;
			Opacity = 75;
			
			// Resolve PEN root path
			System.IO.DriveInfo pen = System.IO.DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == System.IO.DriveType.Removable && d.VolumeLabel == "PEN");
			if(pen != null)
				weatherPicture.Image = Image.FromFile(pen.Name + @"Weather\" + weatherQuery.Weathers[0].Icon + ".png");
			else
				weatherPicture.Load("http://openweathermap.org/img/w/" + weatherQuery.Weathers[0].Icon + ".png");
			
			weatherPicture.BackColor = Color.FromArgb(opacity * 255 / 100, BackColor);
			town.Text = weatherQuery.Name;
			town.BackColor = Color.FromArgb(opacity * 255 / 100, BackColor);
			currentTemperature.Text = Math.Round(weatherQuery.Main.Temperature.CelsiusCurrent, 1, MidpointRounding.AwayFromZero) + "°C";
			currentTemperature.BackColor = Color.FromArgb(opacity * 255 / 100, BackColor);
			maxMinTemperature.Text = "Max: " + Math.Round(weatherQuery.Main.Temperature.CelsiusMaximum, 0, MidpointRounding.AwayFromZero) + "°C" + " Min: " + Math.Round(weatherQuery.Main.Temperature.CelsiusMinimum, 0, MidpointRounding.AwayFromZero) + "°C";
			maxMinTemperature.BackColor = Color.FromArgb(opacity * 255 / 100, BackColor);
			weatherCondition.Text = weatherQuery.Weathers.FirstOrDefault().Main.CapitalizeWords();
			weatherCondition.BackColor = Color.FromArgb(opacity * 255 / 100, BackColor);
			weatherDescription.Text = weatherQuery.Weathers.FirstOrDefault().Description.CapitalizeWords();
			weatherDescription.BackColor = Color.FromArgb(opacity * 255 / 100, BackColor);
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

				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT

				return cp;
			}
		}
//
//		protected void InvalidateEx()
//		{
//			if (Parent == null)
//			{
//				return;
//			}
//
//			Rectangle rc = new Rectangle(this.Location, this.Size);
//
//			Parent.Invalidate(rc, true);
//		}
//
//		protected override void OnPaintBackground(PaintEventArgs pevent)
//		{
//
//		}
//
//		private Random r = new Random();

		protected override void OnPaint(PaintEventArgs e)
		{
//			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Black)), this.ClientRectangle);
			using (var brush = new SolidBrush(Color.FromArgb(opacity * 255 / 100, BackColor)))
			{
				e.Graphics.FillRectangle(brush, ClientRectangle);
			}
			base.OnPaint(e);
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
			// 
			// weatherPicture
			// 
			weatherPicture.Location = new Point(3, 36);
			weatherPicture.Name = "weatherPicture";
			weatherPicture.Size = new Size(90, 90);
			weatherPicture.SizeMode = PictureBoxSizeMode.Zoom;
			// 
			// town
			// 
			town.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			town.ForeColor = Color.White;
			town.Location = new Point(3, 3);
			town.Name = "town";
			town.Size = new Size(242, 31);
			town.TextAlign = ContentAlignment.TopCenter;
			// 
			// currentTemperature
			// 
			currentTemperature.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			currentTemperature.ForeColor = Color.White;
			currentTemperature.Location = new Point(99, 36);
			currentTemperature.Name = "currentTemperature";
			currentTemperature.Size = new Size(146, 25);
			// 
			// maxMinTemperature
			// 
			maxMinTemperature.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			maxMinTemperature.ForeColor = Color.White;
			maxMinTemperature.Location = new Point(99, 63);
			maxMinTemperature.Name = "maxMinTemperature";
			maxMinTemperature.Size = new Size(146, 15);
			// 
			// weatherCondition
			// 
			weatherCondition.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			weatherCondition.ForeColor = Color.White;
			weatherCondition.ImageAlign = ContentAlignment.BottomLeft;
			weatherCondition.Location = new Point(99, 85);
			weatherCondition.Name = "weatherCondition";
			weatherCondition.Size = new Size(146, 20);
			// 
			// weatherDescription
			// 
			weatherDescription.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			weatherDescription.ForeColor = Color.White;
			weatherDescription.ImageAlign = ContentAlignment.BottomLeft;
			weatherDescription.Location = new Point(99, 108);
			weatherDescription.Name = "label23";
			weatherDescription.Size = new Size(146, 18);
			
			Controls.AddRange(new Control[]{
			                  	town,
			                  	currentTemperature,
			                  	maxMinTemperature,
			                  	weatherCondition,
			                  	weatherDescription,
			                  	weatherPicture
			                  });
			Name = "WeatherPanel";
			Size = new Size(248, 178);
		}
	}
}

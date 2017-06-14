/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 21/11/2016
 * Time: 00:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using appCore.UI;
using appCore.DB;
using appCore.Settings;
using Transitions;

namespace appCore.Shifts
{
	/// <summary>
	/// Description of ShiftsCalendar.
	/// </summary>
	public class ShiftsCalendar : AMTRoundCornersPanel
	{
		System.Timers.Timer midnightTimer;
		
		Bitmap shiftsBodySnap;
		Panel wholeShiftsPanel;
		PopupHelper popup;
		
		List<Rectangle> rectCollection = new List<Rectangle>();
		
		DateTime shiftsChosenDate = DateTime.Now;
		public int ShownMonth {
			get { return shiftsChosenDate.Month; }
			private set { }
		}
		public int ShownYear {
			get { return shiftsChosenDate.Year; }
			private set { }
		}
		
		bool daysSelection;
		DateTime shiftsEndSelectDate;
		
		public string PersonName {
			get;
			private set;
		}
		
		public Size CalendarSize {
			get { return shiftsBodySnap.Size; }
			private set { }
		}
		
		public bool isVisible {
			get { return Location.X >= 0 && Location.Y >= 0; }
		}
		
		const int shiftsRectWidth = 30; //the width of the rectangle
		const int shiftsRectHeight = 20; //the height of the rectangle
		
		public ShiftsCalendar()
		{
			DoubleBufferActive = true;
			BorderColor = Color.Black;
			BackColor = Color.Gray;
			BorderWidth = 1.25f;
			BordersToDraw = AMTRoundCornersPanel.Borders.Left | AMTRoundCornersPanel.Borders.Bottom | AMTRoundCornersPanel.Borders.Right;
//					this.BordersToDraw = this.Borders.None;
			CornersToRound = AMTRoundCornersPanel.Corners.BottomLeft | AMTRoundCornersPanel.Corners.BottomRight;
			MouseClick += shiftsPanel_MouseClick;
			PersonName = CurrentUser.FullName[1] + " " + CurrentUser.FullName[0];
			shiftsBodySnap = loadShifts(true);
//			shiftsBodySnap.Save(UserFolder.FullName + @"\shiftsSnap.png");
			Size = shiftsBodySnap.Size;
			
			PictureBox shiftsPanel_icon = new PictureBox();
			((System.ComponentModel.ISupportInitialize)(shiftsPanel_icon)).BeginInit();
			shiftsPanel_icon.BackColor = Color.Gray;
			shiftsPanel_icon.Name = "shiftsPanel_icon";
			shiftsPanel_icon.Size = new Size(16, 16);
			shiftsPanel_icon.Image = Resources.Business_Planner_icon;
			shiftsPanel_icon.SizeMode = PictureBoxSizeMode.StretchImage;
			shiftsPanel_icon.Parent = this;
			shiftsPanel_icon.Location = new Point(5, 5);
			Controls.Add(shiftsPanel_icon);
			
			PictureBox shiftsPanel_swapShifts = new PictureBox();
			((System.ComponentModel.ISupportInitialize)(shiftsPanel_swapShifts)).BeginInit();
			shiftsPanel_swapShifts.BackColor = Color.Gray;
			shiftsPanel_swapShifts.Name = "shiftsPanel_swapShifts";
			shiftsPanel_swapShifts.Size = new Size(16, 16);
			shiftsPanel_swapShifts.Image = Resources.Replace_64;
			shiftsPanel_swapShifts.SizeMode = PictureBoxSizeMode.StretchImage;
			shiftsPanel_swapShifts.Parent = this;
			shiftsPanel_swapShifts.Location = new Point(this.Width - 21, 5);
			shiftsPanel_swapShifts.Click += shiftsPanel_swapShiftsClick;
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 600000;
			toolTip.InitialDelay = 500;
			toolTip.ReshowDelay = 500;
			toolTip.ShowAlways = false; // Force the ToolTip text to be displayed whether or not the form is active.
			toolTip.IsBalloon = true;
			toolTip.UseAnimation = true;
			toolTip.UseFading = false;
			toolTip.BackColor = Color.White;
			toolTip.ForeColor = Color.Firebrick;
			toolTip.SetToolTip(shiftsPanel_swapShifts, "Request Shifts Swap");
//			shiftsPanel_refresh.MouseLeave += (this.PictureBox5MouseLeave);
//			shiftsPanel_refresh.MouseHover += (this.PictureBox5MouseHover);
			Controls.Add(shiftsPanel_swapShifts);
			Name = "ShiftsCalendar";
//			this.MouseMove += shiftsPanel_MouseMove;
//			http://www.codeproject.com/Articles/38436/Extended-Graphics-Rounded-rectangles-Font-metrics
//			Initiate(true);
			Paint += shiftsPanelPaint;
			deployMidnightTimer();
		}
		
		public ShiftsCalendar(string name, DateTime startSelectDate, DateTime endSelectDate) {
			DoubleBufferActive = true;
			shiftsChosenDate = startSelectDate;
			shiftsEndSelectDate = endSelectDate;
			daysSelection = shiftsEndSelectDate > shiftsChosenDate;
			PersonName = name;
//			Initiate(false);
			shiftsBodySnap = loadShifts(false);
			Size = shiftsBodySnap.Size;
			Paint += shiftsPanelPaint;
//			Invalidate();
		}
		
//		void Initiate(bool showIcons) {
		////			LocationChanged += shiftsPanel_LocationChanged;
//		}
		
		public void RedrawCalendar(string name, DateTime startSelectDate, DateTime endSelectDate) {
			shiftsChosenDate = startSelectDate;
			shiftsEndSelectDate = endSelectDate;
			daysSelection = shiftsEndSelectDate >= shiftsChosenDate;
			PersonName = name;
			shiftsBodySnap = loadShifts(false);
			Size = shiftsBodySnap.Size;
			Invalidate();
//			Initiate(false);
		}
		
		void deployMidnightTimer() {
			DateTime midnight = new DateTime(DateTime.Now.AddDays(1).Year,DateTime.Now.AddDays(1).Month,DateTime.Now.AddDays(1).Day,0,0,1);
			TimeSpan timeSpanToMidnight = midnight - DateTime.Now;
			int msToMidnight = (int)timeSpanToMidnight.TotalMilliseconds;
			midnightTimer = new System.Timers.Timer(msToMidnight);
			midnightTimer.Elapsed += midnightTimer_Elapsed;
			midnightTimer.Enabled = true;
		}

		void midnightTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(DateTime.Now > shiftsChosenDate) {
				shiftsChosenDate = DateTime.Now;
				shiftsBodySnap = loadShifts(true);
//				Initiate(true);
				
				DateTime midnight = new DateTime(DateTime.Now.AddDays(1).Year,DateTime.Now.AddDays(1).Month,DateTime.Now.AddDays(1).Day,0,0,1);
				TimeSpan timeSpanToMidnight = midnight - DateTime.Now;
				midnightTimer.Interval = (int)timeSpanToMidnight.TotalMilliseconds;
			}
		}
		
		void shiftsPanelPaint(object sender, PaintEventArgs e)
		{
//			Invalidate(true);
//			shiftsSnap.Save(UserFolderPath + @"\bmp.png");
			Size = new Size(shiftsBodySnap.Width, shiftsBodySnap.Height);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.DrawImageUnscaled(shiftsBodySnap, Point.Empty);
		}

		void shiftsPanel_LocationChanged(object sender, EventArgs e)
		{
			if(this.Location.Y == 0)
				this.Refresh();
		}

		void shiftsPanel_MouseClick(object sender, MouseEventArgs e)
		{
			bool loopFinished = true;
			for(int c = 0;c < rectCollection.Count;c++)
			{
				if(rectCollection[c].Contains(e.Location)) {
					shiftsChosenDate = new DateTime(shiftsChosenDate.Year, shiftsChosenDate.Month, c + 1);
					loopFinished = false;
					break;
				}
			}
			if(loopFinished)
				return;
			
			string shift = Databases.shiftsFile.GetShift(PersonName, shiftsChosenDate);
			if(string.IsNullOrEmpty(shift) || shift.StartsWith("H") || shift == "F") {
				if(DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 8)
					shift = "N";
				else {
					if(DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 16)
						shift = "M";
					else
						shift = "A";
				}
			}
//			if(string.IsNullOrEmpty(shift) || shift.StartsWith("H") || shift == "F")
//				return;
			
			List<SingleShift> sameShiftRows = Databases.shiftsFile.GetWholeShift(shift, shiftsChosenDate);
			
			if(sameShiftRows.Count == 0)
				return;
			
			wholeShiftsPanel = new WholeShiftsPanel(shiftsChosenDate, sameShiftRows, shift);
			popup = new PopupHelper(wholeShiftsPanel);
			popup.Show(this, Cursor.Position);
		}
		
		void shiftsPanel_swapShiftsClick(object sender, EventArgs e)
		{
			if(CurrentUser.UserName == "GONCARJ3") {
				var fc = Application.OpenForms.OfType<ShiftsSwapForm>().ToList();
				
				if(fc.Any()) {
					fc[0].Activate();
					return;
				}
				
				ShiftsSwapForm ss = new ShiftsSwapForm();
				ss.Show();
			}
			else
				FlexibleMessageBox.Show("New feature coming soon...", "Request Shifts Swap", MessageBoxButtons.OK);
			// FIXME: shiftsPanel_refreshClick disabled due to System.InvalidOperationException on this.Invalidate(true)
			// System.InvalidOperationException: Cross-thread operation not valid: Control 'shiftsPanel' accessed from a thread other than the thread it was created on.

//			Thread t = new Thread(() => {
//			                      	shiftsBodySnap = Tools.loadShifts(shiftsBodySnap, shiftsChosenDate);
//			                      	this.Invalidate(true);
//			                      });
//			t.Start();
		}
		
		Bitmap loadShifts(bool showIcons) {
			rectCollection.Clear();
			byte first_weekday = (byte)(new DateTime(shiftsChosenDate.Year,shiftsChosenDate.Month,1).DayOfWeek);
			if(first_weekday == 0)
				first_weekday = 6; // if month starts on Sunday, 6 because Sunday is actually weekday 0
			else
				first_weekday--;
			int num_days = DateTime.DaysInMonth(shiftsChosenDate.Year, shiftsChosenDate.Month); // find how many days are in the selected month
			int num_lines = (int)Math.Ceiling((double)(first_weekday + num_days) / (double)7);
//			int panelHeaderWidth = 0;
//			if(showIcons)
//			panelHeaderWidth = this.Controls["shiftsPanel_refresh"].Left - this.Controls["shiftsPanel_icon"].Right;
			int panelBodyHeight = (int)((shiftsRectHeight * 2) + 3 + ((num_lines * 2) * shiftsRectHeight) + 10); // (height * 2) + 3 for title and weedays headers; + 10 for 5 padding on top&bottom
//			DataRow[] foundRows = Databases.shiftsFile.monthTables[date.Month - 1].Select("AbsName Like '" + CurrentUser.fullName[1].RemoveDiacritics().ToUpper() + "%' AND AbsName Like '%" + CurrentUser.fullName[0].RemoveDiacritics().ToUpper() + "'");
			string[] foundRows = null;
			if(!string.IsNullOrEmpty(PersonName))
				foundRows = Databases.shiftsFile.GetAllShiftsInMonth(PersonName, shiftsChosenDate.Month);
			
//			shiftsHeaderSnap = new Bitmap(MainForm.shiftsPanel.Controls["shiftsPanel_refresh"].Right + MainForm.shiftsPanel.Controls["shiftsPanel_icon"].Right, shiftsRectHeight);
			shiftsBodySnap = new Bitmap(224, panelBodyHeight);
			
			using (Graphics g = Graphics.FromImage(shiftsBodySnap)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;    // Set format of string.
				g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, shiftsBodySnap.Width, shiftsBodySnap.Height);
				
				StringFormat drawStringFormat = new StringFormat();
				drawStringFormat.Alignment = StringAlignment.Center;
				drawStringFormat.LineAlignment = StringAlignment.Far;
				using (Pen pen = new Pen(Color.Black, 1)) {
					string title = Enum.GetName(typeof(Months),shiftsChosenDate.Month - 1) + " " + shiftsChosenDate.Year;
					Rectangle rectangle = new Rectangle(new Point(7, 0), new Size(shiftsRectWidth * 7, shiftsRectHeight + 3));
					g.DrawString(title, new Font("Tahoma",12, FontStyle.Bold), Brushes.Black, rectangle, drawStringFormat);
					drawStringFormat.LineAlignment = StringAlignment.Center;
					Font stringFont = new Font("Tahoma",8, FontStyle.Bold);
					Brush normalRectFill = new SolidBrush(Color.FromArgb((int)(255 * 0.3), Color.Black));
					Brush curDayFill = new SolidBrush(Color.FromArgb((int)(255 * 0.6), Color.Black));
					Brush selectedDayFill = new SolidBrush(Color.FromArgb((int)(255 * 0.6), Color.DarkCyan));
					Brush selectedCurDayFill = new SolidBrush(Color.FromArgb((int)(255 * 0.6), Color.Cyan));
					
					for (int index = 0; index < 7; index++)
					{
						Brush weekdaysBrush = new SolidBrush(Color.FromArgb((int)(255 * 0.8), Color.Black));
						Rectangle rect = new Rectangle(new Point(7 + shiftsRectWidth * index, 25 + shiftsRectHeight * 0), new Size(shiftsRectWidth, shiftsRectHeight));
						string text = System.Enum.GetName(typeof(DayOfWeek),(index + 1) % 7).Substring(0,3);
						
						g.FillRectangle(weekdaysBrush, rect);
						g.DrawRectangle(pen, rect);
						g.DrawString(text, stringFont, Brushes.LightGray, rect, drawStringFormat);
					}
					
					//
					int curDay = 1 - first_weekday; // will get negative value if not monday, loop below will only write anything when this value is over 0
					
					for(int line = 1;line <= num_lines * 2;line+=2) {
						for(int col = 0;col < 7;col++) {
							// Draw 1st box for day number
							string text = string.Empty;
							Rectangle rect1 = new Rectangle(new Point(7 + shiftsRectWidth * col, 25 + shiftsRectHeight * (line)), new Size(shiftsRectWidth, shiftsRectHeight));
							
							if(curDay > 0 && curDay <= num_days) {
								text = curDay.ToString();
								g.FillRectangle(curDayFill, rect1);
							}
							else
								g.FillRectangle(normalRectFill, rect1);
							g.DrawRectangle(pen, rect1);
							g.DrawString(text, stringFont, Brushes.DarkGray, rect1, drawStringFormat);
							
							// Draw 2nd box for shift text
							text = string.Empty;
							Rectangle rect2 = new Rectangle(new Point(7 + shiftsRectWidth * col, 25 + shiftsRectHeight * (line + 1)), new Size(shiftsRectWidth, shiftsRectHeight));
							if(curDay == DateTime.Now.Day && ShownMonth == DateTime.Now.Month && ShownYear == DateTime.Now.Year) {
								if(daysSelection) {
									if(curDay >= shiftsChosenDate.Day && curDay <= shiftsEndSelectDate.Day)
										g.FillRectangle(selectedCurDayFill,rect2);
									else
										g.FillRectangle(curDayFill, rect2);
								}
								else
									g.FillRectangle(curDayFill, rect2);
							}
							else {
								if(daysSelection) {
									if(curDay >= shiftsChosenDate.Day && curDay <= shiftsEndSelectDate.Day)
										g.FillRectangle(selectedDayFill,rect2);
									else
										g.FillRectangle(normalRectFill, rect2);
								}
								else
									g.FillRectangle(normalRectFill, rect2);
							}
							Brush drawBrush = Brushes.LightGray;
							if(curDay > 0 && curDay <= num_days) {
								if(foundRows != null) {
									text = foundRows[curDay - 1];
									switch(text) {
										case "M":
											drawBrush = Brushes.ForestGreen;
											break;
										case "A":
											drawBrush = Brushes.OrangeRed;
											break;
										case "N":
											drawBrush = Brushes.Red;
											break;
									}
								}
							}
							
							g.DrawRectangle(pen, rect2);
							g.DrawString(text, stringFont, drawBrush, rect2, drawStringFormat);
							if(curDay > 0 && curDay <= num_days)
								rectCollection.Add(rect2);
							curDay++;
						}
					}
				}
//				shiftsBodySnap.Save(UserFolder.FullName + @"\shiftsSnap.png");
			}
			return shiftsBodySnap;
		}
		
		public void toggleShiftsPanel() {
			// FIXME: UI glitch on shiftsPanel objects
			if(this.Location.Y == 0) {
				Transition t = new Transition(new TransitionType_EaseInEaseOut(500));
				t.add(this, "Top", 0 - this.Height);
				t.run();
			}
			else {
				Transition t = new Transition(new TransitionType_EaseInEaseOut(500));
				t.add(this, "Top", 0);
				t.run();
			}
		}
	}
}

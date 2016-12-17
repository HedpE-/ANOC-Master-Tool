/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 21/11/2016
 * Time: 00:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using appCore.UI;
using appCore.Toolbox;
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
		List<Rectangle> rectCollection = new List<Rectangle>();
		DateTime shiftsChosenDate = DateTime.Now;
		Bitmap shiftsBodySnap;
		Panel wholeShiftsPanel;
		PopupHelper popup;
		
		public const int shiftsRectWidth = 30; //the width of the rectangle
		public const int shiftsRectHeight = 20; //the height of the rectangle
		
		public ShiftsCalendar()
		{
			DoubleBufferActive = true;
			BorderColor = Color.Black;
			BackColor = Color.Gray;
			Size = new Size(220, 26);
			BorderWidth = 1.25f;
			BordersToDraw = AMTRoundCornersPanel.Borders.Left | AMTRoundCornersPanel.Borders.Bottom | AMTRoundCornersPanel.Borders.Right;
//					this.BordersToDraw = this.Borders.None;
			CornersToRound = AMTRoundCornersPanel.Corners.BottomLeft | AMTRoundCornersPanel.Corners.BottomRight;
			MouseClick += shiftsPanel_MouseClick;
			
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
			
			PictureBox shiftsPanel_refresh = new PictureBox();
			((System.ComponentModel.ISupportInitialize)(shiftsPanel_refresh)).BeginInit();
			shiftsPanel_refresh.BackColor = Color.Gray;
			shiftsPanel_refresh.Name = "shiftsPanel_refresh";
			shiftsPanel_refresh.Size = new Size(16, 16);
			shiftsPanel_refresh.Image = Resources.Replace_64;
			shiftsPanel_refresh.SizeMode = PictureBoxSizeMode.StretchImage;
			shiftsPanel_refresh.Parent = this;
			shiftsPanel_refresh.Location = new Point(this.Width - 21, 5);
			shiftsPanel_refresh.Click += shiftsPanel_refreshClick;
//			shiftsPanel_refresh.MouseLeave += (this.PictureBox5MouseLeave);
//			shiftsPanel_refresh.MouseHover += (this.PictureBox5MouseHover);
			Controls.Add(shiftsPanel_refresh);
			Name = "ShiftsCalendar";
//			this.MouseMove += shiftsPanel_MouseMove;
//			http://www.codeproject.com/Articles/38436/Extended-Graphics-Rounded-rectangles-Font-metrics
			shiftsBodySnap = loadShifts(shiftsChosenDate);
			Paint += shiftsPanelPaint;
			LocationChanged += shiftsPanel_LocationChanged;
		}
		
		void shiftsPanelPaint(object sender, PaintEventArgs e)
		{
			Invalidate(true);
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
			
			string shift = Databases.shiftsFile.monthTables[shiftsChosenDate.Month - 1].Select("AbsName Like '" + Tools.RemoveDiacritics(CurrentUser.fullName[1]).ToUpper() + "%' AND AbsName Like '%" + Tools.RemoveDiacritics(CurrentUser.fullName[0]).ToUpper() + "'")[0]["Day" + shiftsChosenDate.Day].ToString();
			if(string.IsNullOrEmpty(shift))
				return;
			
			DataRow[] sameShiftRows = getWholeShift(shiftsChosenDate);
			
			if(sameShiftRows == null)
				return;
			if(sameShiftRows.Length == 0)
				return;
			
			wholeShiftsPanel = new WholeShiftsPanel(shiftsChosenDate, sameShiftRows, shift);
			popup = new PopupHelper(wholeShiftsPanel);
			popup.Show(this);
		}
		
		void shiftsPanel_refreshClick(object sender, EventArgs e)
		{
			// FIXME: shiftsPanel_refreshClick disabled due to System.InvalidOperationException on this.Invalidate(true)
			// System.InvalidOperationException: Cross-thread operation not valid: Control 'shiftsPanel' accessed from a thread other than the thread it was created on.

//			Thread t = new Thread(() => {
//			                      	shiftsBodySnap = Tools.loadShifts(shiftsBodySnap, shiftsChosenDate);
//			                      	this.Invalidate(true);
//			                      });
//			t.Start();
		}
		
		Bitmap loadShifts(DateTime date) {
			rectCollection.Clear();
			byte first_weekday = (byte)(new DateTime(date.Year,date.Month,1).DayOfWeek);
			if(first_weekday == 0) first_weekday = 6; // if month starts on Sunday, 6 because Sunday is actually weekday 0
			else first_weekday--;
			int num_days = DateTime.DaysInMonth(date.Year, date.Month); // find how many days are in the selected month
			int num_lines = (int)Math.Ceiling((double)(first_weekday + num_days) / (double)7);
			int panelHeaderWidth = this.Controls["shiftsPanel_refresh"].Left - this.Controls["shiftsPanel_icon"].Right;
			int panelBodyHeight = (int)((shiftsRectHeight * 2) + 3 + ((num_lines * 2) * shiftsRectHeight) + 10); // (height * 2) + 3 for title and weedays headers; + 10 for 5 padding on top&bottom
			DataRow[] foundRows = Databases.shiftsFile.monthTables[date.Month - 1].Select("AbsName Like '" + Tools.RemoveDiacritics(CurrentUser.fullName[1]).ToUpper() + "%' AND AbsName Like '%" + Tools.RemoveDiacritics(CurrentUser.fullName[0]).ToUpper() + "'");
			
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
					string title = Enum.GetName(typeof(Tools.Months),date.Month - 1) + " " + date.Year;
					Rectangle rectangle = new Rectangle(new Point(7, 0), new Size(shiftsRectWidth * 7, shiftsRectHeight + 3));
					g.DrawString(title, new Font("Tahoma",12, FontStyle.Bold), Brushes.Black, rectangle, drawStringFormat);
					drawStringFormat.LineAlignment = StringAlignment.Center;
					Font stringFont = new Font("Tahoma",8, FontStyle.Bold);
					Brush normalRectFill = new SolidBrush(Color.FromArgb((int)(255 * 0.3), Color.Black));
					Brush curDayFill = new SolidBrush(Color.FromArgb((int)(255 * 0.6), Color.Black));
					
					for (int index = 0; index < 7; index++)
					{
						Brush weekdaysBrush = new SolidBrush(Color.FromArgb((int)(255 * 0.8), Color.Black));
						Rectangle rect = new Rectangle(new Point(7 + shiftsRectWidth * index, 25 + shiftsRectHeight * 0), new Size(shiftsRectWidth, shiftsRectHeight));
						string text = Enum.GetName(typeof(DayOfWeek),(index + 1) % 7).Substring(0,3);
						
						g.FillRectangle(weekdaysBrush, rect);
						g.DrawRectangle(pen, rect);
						g.DrawString(text, stringFont, Brushes.LightGray, rect, drawStringFormat);
					}
					
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
							if(curDay == DateTime.Now.Day)
								g.FillRectangle(curDayFill, rect2);
							else
								g.FillRectangle(normalRectFill, rect2);
							Brush drawBrush = Brushes.LightGray;
							if(curDay > 0 && curDay <= num_days) {
								text = foundRows[0]["Day" + curDay].ToString();
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
							
							g.DrawRectangle(pen, rect2);
							g.DrawString(text, stringFont, drawBrush, rect2, drawStringFormat);
							if(curDay > 0 && curDay <= num_days)
								rectCollection.Add(rect2);
//								MainForm.rectCollection.Add(Rectangle.Union(rect1,rect2));
							curDay++;
						}
					}
				}
//				shiftsSnap.Save(MainForm.UserFolderPath + @"\shiftsSnap.png");
			}
			return shiftsBodySnap;
		}
		
		public DataRow[] getWholeShift(DateTime date) {
			string shift = Databases.shiftsFile.monthTables[date.Month - 1].Select("AbsName Like '" + Tools.RemoveDiacritics(CurrentUser.fullName[1]).ToUpper() + "%' AND AbsName Like '%" + Tools.RemoveDiacritics(CurrentUser.fullName[0]).ToUpper() + "'")[0]["Day" + date.Day].ToString();
			if(shift.StartsWith("H"))
				return null;
//			string dayColumnToFilter = MainForm.shiftsTable[date.Month - 1].Columns[date.Day + 3].ColumnName;
			string filter = string.Empty;
			switch (shift) {
				case "M":
					filter = "AbsName <> '' AND Day" + date.Day + " <> 'A' AND Day" + date.Day + " <> 'N' AND Day" + date.Day + " <> 'H' AND Day" + date.Day + " <> 'HA' AND Day" + date.Day + " <> 'L'";
					break;
				case "A":
					filter = "AbsName <> '' AND Day" + date.Day + " <> 'M' AND Day" + date.Day + " <> 'N' AND Day" + date.Day + " <> 'H' AND Day" + date.Day + " <> 'HA' AND Day" + date.Day + " <> 'L'";
					break;
				case "N":
					filter = "AbsName <> '' AND Day" + date.Day + " = '" + shift + "'";
					break;
				default:
					filter = "AbsName <> '' AND Day" + date.Day + " <> 'N' AND Day" + date.Day + " <> 'H' AND Day" + date.Day + " <> 'HA' AND Day" + date.Day + " <> 'L'";
					break;
			}
			
			DataRow[] foundRows = Databases.shiftsFile.monthTables[date.Month - 1].Select(filter);
			
			return foundRows;
		}
		
		public void toggleShiftsPanel() {
			// FIXME: UI glitch on shiftsPanel objects
			// FIXME: If shiftFile doesn't exist and share access is denied, app crashes
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

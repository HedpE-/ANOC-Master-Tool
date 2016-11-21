/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 01-07-2016
 * Time: 02:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.Settings;
using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace appCore.Shifts
{
	public class WholeShiftsPanel : Panel
	{
		string wholeShiftString;
		
		public WholeShiftsPanel(DateTime shiftsChosenDate, DataRow[] sameShiftRows, string shift)
		{
			this.SetStyle(ControlStyles.Selectable, true);
			
			List<DataRow> SL = new List<DataRow>();
			List<DataRow> Agents = new List<DataRow>();
			FieldInfo _rowID;
			foreach(DataRow dr in sameShiftRows) {
				_rowID = typeof(DataRow).GetField("_rowID", BindingFlags.NonPublic | BindingFlags.Instance);
				int rowID = (int)Convert.ToInt64(_rowID.GetValue(dr));
				if(rowID > 3 && rowID < 12)
					SL.Add(dr);
				else
					Agents.Add(dr);
			}
			
			// Draw panel
			// FIXME: improve wholeShiftsPanel performance(generate a cache for individual shifts on separate thread)
			const int RectHeight = 20;
			const int nameRectWidth = 145;
			const int shiftRectWidth = 35;
			const int headerSpacing = 10;
			const int paddingVertical = 7;
			const int paddingHorizontal = 7;
			int num_lines = SL.Count + Agents.Count + 3; // + 3 for Title, SL & Agents headers
			
			int panelHeight = (int)(2 * paddingVertical) + (2 * headerSpacing) + (num_lines * RectHeight);
			int panelWidth = (int)(2 * paddingHorizontal) + nameRectWidth + shiftRectWidth;
			
			wholeShiftString = string.Empty;
			Bitmap wholeShiftSnap = new Bitmap(panelWidth, panelHeight);
			
			using (Graphics g = Graphics.FromImage(wholeShiftSnap)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality; // Set format of string.
				g.FillRectangle(new SolidBrush(Color.Black), 0, 0, wholeShiftSnap.Width, wholeShiftSnap.Height);
				
				StringFormat drawStringFormat = new StringFormat();
				drawStringFormat.Alignment = StringAlignment.Center;
				drawStringFormat.LineAlignment = StringAlignment.Center;
				Font titlesFont = new Font("Tahoma",11, FontStyle.Bold);
				Font stringFont = new Font("Tahoma",8, FontStyle.Bold);
				
				string tempText = String.Format("{0:dd/MM/yyyy}", shiftsChosenDate) + " - " + shift + " Shift";
				wholeShiftString += tempText + Environment.NewLine;
				Rectangle rectangle = new Rectangle(new Point(paddingHorizontal, paddingVertical), new Size(wholeShiftSnap.Width - (paddingHorizontal * 2), RectHeight));
				g.DrawString(tempText, titlesFont, Brushes.Gray, rectangle, drawStringFormat);
				
				int previousRectBottomCoord = rectangle.Bottom;
				
				tempText = "Shift Leaders:";
				wholeShiftString += Environment.NewLine + tempText + Environment.NewLine;
				drawStringFormat.Alignment = StringAlignment.Near;
				rectangle = new Rectangle(new Point(7, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
				g.DrawString(tempText, titlesFont, Brushes.Red, rectangle, drawStringFormat);
				previousRectBottomCoord = rectangle.Bottom;
				
				foreach(DataRow dr in SL) {
					rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
					tempText = dr["Column3"].ToString();
					wholeShiftString += tempText + '\t';
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					drawStringFormat.Alignment = StringAlignment.Far;
					rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
					tempText = dr["Day" + shiftsChosenDate.Day].ToString();
					wholeShiftString += tempText + Environment.NewLine;
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					previousRectBottomCoord = rectangle.Bottom;
					drawStringFormat.Alignment = StringAlignment.Near;
				}
				
				tempText = "Agents:";
				wholeShiftString += Environment.NewLine + tempText + Environment.NewLine;
				rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
				g.DrawString(tempText, titlesFont, Brushes.Red, rectangle, drawStringFormat);
				previousRectBottomCoord = rectangle.Bottom;
				
				for(int c = 1;c <= Agents.Count;c++) {
					rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
					tempText = Agents[c - 1]["Column3"].ToString();
					wholeShiftString += tempText + '\t';
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					drawStringFormat.Alignment = StringAlignment.Far;
					rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
					tempText = Agents[c - 1]["Day" + shiftsChosenDate.Day].ToString();
					wholeShiftString += tempText;
					if(c < Agents.Count)
						wholeShiftString +=  Environment.NewLine;
					g.DrawString(tempText, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					previousRectBottomCoord = rectangle.Bottom;
					drawStringFormat.Alignment = StringAlignment.Near;
				}
				if(CurrentUser.userName == "GONCARJ3")
					wholeShiftSnap.Save(UserFolder.FullName + @"\wholeShiftSnap.png");
			}
			PictureBox shiftsPictureBox = new PictureBox();
			shiftsPictureBox.Parent = this;
			shiftsPictureBox.Size = new Size(wholeShiftSnap.Width, wholeShiftSnap.Height);
			shiftsPictureBox.Location = new Point(0, 0);
			shiftsPictureBox.Image = wholeShiftSnap;
			shiftsPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
			Button shiftsCopyButton = new Button();
			shiftsCopyButton.BackColor = Color.Black;
			shiftsCopyButton.FlatStyle = FlatStyle.Flat;
			shiftsCopyButton.ForeColor = Color.Gray;
			shiftsCopyButton.Text = "Copy to Clipboard";
			shiftsCopyButton.Size = new Size(shiftsPictureBox.Width, 23);
			shiftsCopyButton.Location = new Point(0, shiftsPictureBox.Bottom);
			shiftsCopyButton.Click += shiftsCopyButtonClick;
			
			AutoScroll = true;
			BackColor = Color.Black;
			Capture = true;
			
			const int wholeshiftsPanelMaxHeight = 277;
			if(wholeShiftSnap.Height + shiftsCopyButton.Height > wholeshiftsPanelMaxHeight)
				Size = new Size(wholeShiftSnap.Width + SystemInformation.VerticalScrollBarWidth, wholeshiftsPanelMaxHeight);
			else
				Size = new Size(wholeShiftSnap.Width, wholeShiftSnap.Height + shiftsCopyButton.Height);
			Controls.Add(shiftsPictureBox);
			Controls.Add(shiftsCopyButton);
		}
		
//		void wholeShiftsPanelMouseWheel(object sender, MouseEventArgs e)
//		{
//			wholeShiftsPanel.SuspendLayout();
//			if (e.Delta == 120)
//			{
//				if (wholeShiftsPanel.VerticalScroll.Value - 2 >= wholeShiftsPanel.VerticalScroll.Minimum)
//					wholeShiftsPanel.VerticalScroll.Value -= 2;
//				else
//					wholeShiftsPanel.VerticalScroll.Value = wholeShiftsPanel.VerticalScroll.Minimum;
//			}
//			if (e.Delta == -120)
//			{
//				if (wholeShiftsPanel.VerticalScroll.Value + 2 <= wholeShiftsPanel.VerticalScroll.Maximum)
//					wholeShiftsPanel.VerticalScroll.Value += 2;
//				else
//					wholeShiftsPanel.VerticalScroll.Value = wholeShiftsPanel.VerticalScroll.Maximum;
//			}
//			wholeShiftsPanel.ResumeLayout();
//		}
//		
//		void wholeShiftsPanelMouseEnter(object sender, EventArgs e) {
//			wholeShiftsPanel.Focus();
//		}
//		
//		void wholeShiftsPanelMouseLeave(object sender, EventArgs e) {
//			wholeShiftsPanel.Focus();
//		}
//		
//		void wholeShiftsPanelMouseDown(object sender, MouseEventArgs e) {
//			Rectangle wholePanelArea = Rectangle.Union(wholeShiftsPanel.ClientRectangle,new Rectangle(wholeShiftsPanel.Right, wholeShiftsPanel.Top, 23, wholeShiftsPanel.Height));
//			if(!wholePanelArea.Contains(e.Location)) {
//				if(wholeShiftsPanel.Parent != null) {
//					wholeShiftsPanel.Dispose();
//					this.Invalidate(true);
//				}
//			}
//		}
		
		void shiftsCopyButtonClick(object sender, EventArgs e) {
			Clipboard.SetText(wholeShiftString);
			MainForm.trayIcon.showBalloon("Copied to clipboard","Copied to clipboard");
		}
	}
}
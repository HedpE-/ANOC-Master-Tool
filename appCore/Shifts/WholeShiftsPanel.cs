/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 01-07-2016
 * Time: 02:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace appCore.Shifts
{
	public class WholeShiftsPanel : Panel
	{
		string wholeShiftString;
		
		public WholeShiftsPanel(DateTime shiftsChosenDate, List<SingleShift> sameShiftRows, string shift)
		{
			this.SetStyle(ControlStyles.Selectable, true);
			
			// Draw panel
			// FIXME: improve wholeShiftsPanel performance(generate a cache for individual shifts on separate thread)
			const int RectHeight = 20;
			const int nameRectWidth = 145;
			const int shiftRectWidth = 35;
			const int headerSpacing = 10;
			const int paddingVertical = 7;
			const int paddingHorizontal = 7;

            var userCountPerDisplayedTeam = sameShiftRows.GroupBy(s => s.Role).Select(group => new { Role = group.Key, Count = group.Count() });

			int num_lines = sameShiftRows.Count + 1 + userCountPerDisplayedTeam.Count(); // + 1 for Title + 1 for each team header(SL, Agents, APT, TEF, ExternalAlarms & Reporting)
			
			int panelHeight = (int)(2 * paddingVertical) + (userCountPerDisplayedTeam.Count() * headerSpacing) + (num_lines * RectHeight);
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
				
                if(userCountPerDisplayedTeam.Any(t => t.Role == Roles.ShiftLeader))
                {
                    List<SingleShift> filteredList = sameShiftRows.Where(s => s.Role == Roles.ShiftLeader).ToList();
                    wholeShiftString += Environment.NewLine + "Shift Leaders:" + Environment.NewLine;
                    drawStringFormat.Alignment = StringAlignment.Near;
                    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
                    g.DrawString("Shift Leaders:", titlesFont, Brushes.Red, rectangle, drawStringFormat);
                    previousRectBottomCoord = rectangle.Bottom;
                    foreach (SingleShift sh in filteredList) {
					    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
					    wholeShiftString += sh.Name + '\t';
					    g.DrawString(sh.Name, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					    drawStringFormat.Alignment = StringAlignment.Far;
					    rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
					    wholeShiftString += sh.Shift + Environment.NewLine;
					    g.DrawString(sh.Shift, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					    previousRectBottomCoord = rectangle.Bottom;
					    drawStringFormat.Alignment = StringAlignment.Near;
                    }
                }

                if (userCountPerDisplayedTeam.Any(t => t.Role == Roles.APT))
                {
                    List<SingleShift> filteredList = sameShiftRows.Where(s => s.Role == Roles.APT).ToList();
                    wholeShiftString += Environment.NewLine + "FSO - APT:" + Environment.NewLine;
                    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
                    g.DrawString("FSO - APT:", titlesFont, Brushes.Red, rectangle, drawStringFormat);
                    previousRectBottomCoord = rectangle.Bottom;
                    for (int c = 1; c <= filteredList.Count; c++)
                    {
                        rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Name + '\t';
                        g.DrawString(filteredList[c - 1].Name, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        drawStringFormat.Alignment = StringAlignment.Far;
                        rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Shift;
                        if (c < filteredList.Count)
                            wholeShiftString += Environment.NewLine;
                        g.DrawString(filteredList[c - 1].Shift, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        previousRectBottomCoord = rectangle.Bottom;
                        drawStringFormat.Alignment = StringAlignment.Near;
                    }
                }

                if (userCountPerDisplayedTeam.Any(t => t.Role == Roles.TEF))
                {
                    List<SingleShift> filteredList = sameShiftRows.Where(s => s.Role == Roles.TEF).ToList();
                    wholeShiftString += Environment.NewLine + Environment.NewLine + "TEF Customer:" + Environment.NewLine;
                    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
                    g.DrawString("TEF Customer:", titlesFont, Brushes.Red, rectangle, drawStringFormat);
                    previousRectBottomCoord = rectangle.Bottom;
                    for (int c = 1; c <= filteredList.Count; c++)
                    {
                        rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Name + '\t';
                        g.DrawString(filteredList[c - 1].Name, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        drawStringFormat.Alignment = StringAlignment.Far;
                        rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Shift;
                        if (c < filteredList.Count)
                            wholeShiftString += Environment.NewLine;
                        g.DrawString(filteredList[c - 1].Shift, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        previousRectBottomCoord = rectangle.Bottom;
                        drawStringFormat.Alignment = StringAlignment.Near;
                    }
                }

                if (userCountPerDisplayedTeam.Any(t => t.Role == Roles.ExternalAlarms))
                {
                    List<SingleShift> filteredList = sameShiftRows.Where(s => s.Role == Roles.ExternalAlarms).ToList();
                    wholeShiftString += Environment.NewLine + Environment.NewLine + "External Alarms:" + Environment.NewLine;
                    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
                    g.DrawString("External Alarms:", titlesFont, Brushes.Red, rectangle, drawStringFormat);
                    previousRectBottomCoord = rectangle.Bottom;
                    for (int c = 1; c <= filteredList.Count; c++)
                    {
                        rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Name + '\t';
                        g.DrawString(filteredList[c - 1].Name, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        drawStringFormat.Alignment = StringAlignment.Far;
                        rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Shift;
                        if (c < filteredList.Count)
                            wholeShiftString += Environment.NewLine;
                        g.DrawString(filteredList[c - 1].Shift, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        previousRectBottomCoord = rectangle.Bottom;
                        drawStringFormat.Alignment = StringAlignment.Near;
                    }
                }

                if (userCountPerDisplayedTeam.Any(t => t.Role == Roles.Reporting))
                {
                    List<SingleShift> filteredList = sameShiftRows.Where(s => s.Role == Roles.Reporting).ToList();
                    wholeShiftString += Environment.NewLine + Environment.NewLine + "Reporting:" + Environment.NewLine;
                    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
                    g.DrawString("Reporting:", titlesFont, Brushes.Red, rectangle, drawStringFormat);
                    previousRectBottomCoord = rectangle.Bottom;
                    for (int c = 1; c <= filteredList.Count; c++)
                    {
                        rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Name + '\t';
                        g.DrawString(filteredList[c - 1].Name, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        drawStringFormat.Alignment = StringAlignment.Far;
                        rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
                        wholeShiftString += filteredList[c - 1].Shift;
                        if (c < filteredList.Count)
                            wholeShiftString += Environment.NewLine;
                        g.DrawString(filteredList[c - 1].Shift, stringFont, Brushes.Gray, rectangle, drawStringFormat);
                        previousRectBottomCoord = rectangle.Bottom;
                        drawStringFormat.Alignment = StringAlignment.Near;
                    }
                }

                if(userCountPerDisplayedTeam.Any(t => t.Role == Roles.RAN))
                {
                    List<SingleShift> filteredList = sameShiftRows.Where(s => s.Role == Roles.RAN).ToList();
                    wholeShiftString += Environment.NewLine + Environment.NewLine + "RAN Agents:" + Environment.NewLine;
				    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord + headerSpacing), new Size(nameRectWidth, RectHeight));
				    g.DrawString("RAN Agents:", titlesFont, Brushes.Red, rectangle, drawStringFormat);
				    previousRectBottomCoord = rectangle.Bottom;
				    for(int c = 1;c <= filteredList.Count;c++) {
					    rectangle = new Rectangle(new Point(paddingHorizontal, previousRectBottomCoord), new Size(nameRectWidth, RectHeight));
					    wholeShiftString += filteredList[c - 1].Name + '\t';
					    g.DrawString(filteredList[c - 1].Name, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					    drawStringFormat.Alignment = StringAlignment.Far;
					    rectangle = new Rectangle(new Point(paddingHorizontal + nameRectWidth, previousRectBottomCoord), new Size(shiftRectWidth, RectHeight));
					    wholeShiftString += filteredList[c - 1].Shift;
					    if(c < filteredList.Count)
						    wholeShiftString += Environment.NewLine;
					    g.DrawString(filteredList[c - 1].Shift, stringFont, Brushes.Gray, rectangle, drawStringFormat);
					    previousRectBottomCoord = rectangle.Bottom;
					    drawStringFormat.Alignment = StringAlignment.Near;
                    }
                }
                //if (Settings.CurrentUser.UserName == "GONCARJ3")
                //    wholeShiftSnap.Save(Settings.GlobalProperties.AppDataRootDir + @"\wholeShiftSnap.png");
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
			shiftsCopyButton.Size = new Size(shiftsPictureBox.Width, 25);
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
		
		void shiftsCopyButtonClick(object sender, EventArgs e) {
			Clipboard.SetText(wholeShiftString);
			MainForm.trayIcon.showBalloon("Copied to clipboard","Copied to clipboard");
		}
	}
}
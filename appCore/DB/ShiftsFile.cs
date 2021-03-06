/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 18-08-2016
 * Time: 06:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using appCore.Settings;
using appCore.Shifts;
using OfficeOpenXml;

namespace appCore.DB
{
	/// <summary>
	/// Description of ShiftsFile.
	/// </summary>
	public class ShiftsFile
	{
		FileInfo shiftsFile;
		ExcelPackage package;
		
		ArrayList monthRanges;

		public string FullName {
			get {
				return shiftsFile.FullName;
			}
			protected set { }
		}

		public bool Exists {
			get {
				if(shiftsFile != null)
					return shiftsFile.Exists;
				return false;
				
			}
			protected set { }
		}

		public Months LastMonthAvailable {
			get {
				int lastMonth = DateTime.Now.Month - 1;
				if(shiftsFile != null) {
					var nextMonthHeaderRange = package.Workbook.Worksheets[1].Cells[monthRanges[DateTime.Now.Month].ToString()];
					var nextMonthShifts = package.Workbook.Worksheets[1].Cells[4, nextMonthHeaderRange.Start.Column, 65, nextMonthHeaderRange.End.Column];
					foreach(var cell in nextMonthShifts) {
						if(cell.Value.ToString() == "M" || cell.Value.ToString() == "A" || cell.Value.ToString() == "N") {
							lastMonth++;
							break;
						}
					}
				}
				
				return (Months)lastMonth;
			}
			protected set { }
		}

		public int Year
        {
			get;
			private set;
		}
		
		public List<string> ShiftLeaders
        {
			get;
			private set;
		}		
		public List<string> RAN
        {
			get;
			private set;
		}		
		public List<string> TEF
        {
			get;
			private set;
		}		
		public List<string> ExternalAlarms
        {
			get;
			private set;
		}
        public List<string> Reporting
        {
            get;
            private set;
        }
        public List<string> APT
        {
            get;
            private set;
        }

        int LastRow { get; set; }
		
		public ShiftsFile(int year) {
			shiftsFile = UserFolder.getDBFile("shift*" + year + "*.xlsx");
			
			try {
				package = new ExcelPackage(shiftsFile);
			}
			catch(Exception e) {
                var t = e.Message;
				if(shiftsFile != null) {
					UserFolder.CreateTempFolder();
					FileInfo tempShiftsFile = shiftsFile.CopyTo(GlobalProperties.TempFolder + "\\" + shiftsFile.Name, true);
					package = new ExcelPackage(tempShiftsFile);
				}
				
			}
			
			if (package.File != null) {
				var allMergedCells = package.Workbook.Worksheets[1].MergedCells;
				monthRanges = new ArrayList();
				foreach(string address in allMergedCells) {
					string[] temp = address.Split(':');
					if(temp[0].RemoveLetters() != "1")
						continue;
					if(temp[1].RemoveLetters() != "1")
						continue;
					monthRanges.Add(address);
				}
				
				SortAlphabetLength alphaLen = new SortAlphabetLength();
				monthRanges.Sort(alphaLen);

                var currentMonthHeaderRange = package.Workbook.Worksheets[1].Cells[monthRanges[DateTime.Now.Month - 1].ToString()];
                var currentCell = package.Workbook.Worksheets[1].Cells["a:a"].FirstOrDefault(c => c.Text == "Closure Code").Offset(2, 0);

				while(!string.IsNullOrEmpty(currentCell.Text))
                {
                    var currentMonthShifts = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row, currentMonthHeaderRange.Start.Column, currentCell.Start.Row, currentMonthHeaderRange.End.Column];
                    int shiftsCount = CalculateShiftsInRange(currentMonthShifts);

                    if (shiftsCount > 0)
                    {
                        if (ShiftLeaders == null)
                            ShiftLeaders = new List<string>();
                        ShiftLeaders.Add(currentCell.Offset(0, 2).Text);
                    }
					currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
				}
				while(string.IsNullOrEmpty(currentCell.Text))
					currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];

				while(!string.IsNullOrEmpty(currentCell.Text))
                {
                    var currentMonthShifts = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row, currentMonthHeaderRange.Start.Column, currentCell.Start.Row, currentMonthHeaderRange.End.Column];
                    int shiftsCount = CalculateShiftsInRange(currentMonthShifts);

                    if (shiftsCount > 0)
                    {
					    if(APT == null)
						    APT = new List<string>();
					    APT.Add(currentCell.Offset(0, 2).Text);
                    }
                    currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
				}
				currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
                while (!string.IsNullOrEmpty(currentCell.Text))
                {
                    var currentMonthShifts = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row, currentMonthHeaderRange.Start.Column, currentCell.Start.Row, currentMonthHeaderRange.End.Column];
                    int shiftsCount = CalculateShiftsInRange(currentMonthShifts);

                    if (shiftsCount > 0)
                    {
                        if (TEF == null)
                            TEF = new List<string>();
                        TEF.Add(currentCell.Offset(0, 2).Text);
                    }
                    currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
                }
                currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
                while (!string.IsNullOrEmpty(currentCell.Text))
                {
                    var currentMonthShifts = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row, currentMonthHeaderRange.Start.Column, currentCell.Start.Row, currentMonthHeaderRange.End.Column];
                    int shiftsCount = CalculateShiftsInRange(currentMonthShifts);

                    if (shiftsCount > 0)
                    {
                        if (ExternalAlarms == null)
                            ExternalAlarms = new List<string>();
                        ExternalAlarms.Add(currentCell.Offset(0, 2).Text);
                    }
					currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
				}
				currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
				while(!string.IsNullOrEmpty(currentCell.Text))
                {
                    var currentMonthShifts = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row, currentMonthHeaderRange.Start.Column, currentCell.Start.Row, currentMonthHeaderRange.End.Column];
                    int shiftsCount = CalculateShiftsInRange(currentMonthShifts);

                    if (shiftsCount > 0)
                    {
                        if (Reporting == null)
                            Reporting = new List<string>();
                        Reporting.Add(currentCell.Offset(0, 2).Text);
                    }
					currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
                }
                currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
                while (!string.IsNullOrEmpty(currentCell.Text))
                {
                    if(!ShiftLeaders.Contains(currentCell.Offset(0, 2).Text) && !TEF.Contains(currentCell.Offset(0, 2).Text) && !ExternalAlarms.Contains(currentCell.Offset(0, 2).Text) && !Reporting.Contains(currentCell.Offset(0, 2).Text))
                    {
                        if (RAN == null)
                            RAN = new List<string>();
                        RAN.Add(currentCell.Offset(0, 2).Text);
                    }
                    currentCell = package.Workbook.Worksheets[1].Cells[currentCell.Start.Row + 1, 1];
                }
                //				foreach(var cell in package.Workbook.Worksheets[1].Cells["a:a"]) {
                //					if(cell.Value != null) {
                //						if(cell.Text != "Closure Code") {
                //							if(!slListEnd) {
                //								if(ShiftLeaders == null)
                //									ShiftLeaders = new List<string>();
                //								ShiftLeaders.Add(cell.Offset(0, 2).Text);
                //							}
                //							else {
                //								if(RAN == null)
                //									RAN = new List<string>();
                //								RAN.Add(cell.Offset(0, 2).Text);
                //								LastRow = cell.Start.Row;
                //							}
                //						}
                //					}
                //					else {
                //						if(!slListEnd && cell.Offset(0, 2).Value != null)
                //							slListEnd = cell.Offset(0, 2).Text == "Morning";
                //					}
                //				}
            }
			Year = year;
			if(package.File.FullName.Contains(GlobalProperties.TempFolder.FullName))
				UserFolder.ClearTempFolder();
		}

        int CalculateShiftsInRange(ExcelRangeBase range)
        {
            int shiftsCount = 0;
            foreach (var cell in range)
            {
                var t = cell.Style.Fill.BackgroundColor;
                if (!string.IsNullOrEmpty(cell.Style.Fill.BackgroundColor.Theme))
                {
                    if (!(cell.Style.Fill.BackgroundColor.Theme == "1" && cell.Style.Fill.BackgroundColor.Tint == 0 && cell.Style.Fill.BackgroundColor.Indexed == 10) && cell.Value != null)
                        shiftsCount++;
                }
                else
                    shiftsCount++;
            }

            return shiftsCount;
        }
		
		public string GetShift(string name, DateTime date) {
			int personRow = FindPersonRow(name);
			if(personRow > 0) {
				foreach(var cell in package.Workbook.Worksheets[1].Cells[monthRanges[date.Month - 1].ToString().Replace('1','3')]) {
					if(cell.Value != null) {
						if(cell.Text == date.Day.ToString())
							return package.Workbook.Worksheets[1].Cells[personRow, cell.Start.Column].Text;
					}
				}
			}
			return string.Empty;
		}
		
		public string[] GetShiftsRange(string name, DateTime startDate, DateTime endDate) {
			int personRow = FindPersonRow(name);
			var shiftsRange = package.Workbook.Worksheets[1].Cells[FindDayColumn(startDate) + personRow + ":" +
			                                                       FindDayColumn(endDate) + personRow];
			
			List<string> shifts = new List<string>();
			
			foreach(var cell in shiftsRange) {
				if(cell.Value != null)
					shifts.Add(cell.Text);
			}
			
			return shifts.ToArray();
		}
		
		public string[] GetAllShiftsInMonth(string name, int month) {
			List<string> list = null;
			int personRow = FindPersonRow(name);
			if(personRow > 0) {
				list = new List<string>();
				var personRange = package.Workbook.Worksheets[1].Cells[monthRanges[month - 1].ToString().Replace("1",personRow.ToString())];
				for(int c = personRange.Start.Column;c <= personRange.End.Column;c++) {
					var cell = package.Workbook.Worksheets[1].Cells[personRow, c];
					if(cell.Value == null)
						list.Add(string.Empty);
					else
						list.Add(cell.Text);
				}
			}
			return list == null ? null : list.ToArray();
		}
		
		public List<SingleShift> GetWholeShift(string shift, DateTime date) {
			if(shift.StartsWith("H") || shift == "B")
				return null;
			
			string dayColumn = FindDayColumn(date);
			var columnRange = package.Workbook.Worksheets[1].Cells[dayColumn + "4:" + dayColumn + LastRow];
			List<SingleShift> foundRows = new List<SingleShift>();
			switch (shift) {
				case "M": case "MT": case "QM":
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text != "A" && cell.Text != "N" && !cell.Text.StartsWith("H") && cell.Text != "HA" && cell.Text != "L" && cell.Text != "B" && !cell.Text.IsAllDigits())
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					break;
				case "A":
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text != "M" && cell.Text != "MT" && cell.Text != "QM" && cell.Text != "N" && !cell.Text.StartsWith("H") && cell.Text != "HA" && cell.Text != "L" && cell.Text != "B" && !cell.Text.IsAllDigits())
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					break;
				case "N":
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text == "N")
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					break;
				default:
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text != "N" && !cell.Text.StartsWith("H") && cell.Text != "HA" && cell.Text != "L" && cell.Text != "B" && !cell.Text.IsAllDigits())
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					
					break;
			}
			
			return foundRows;
		}
		
		int FindPersonRow(string name) {
			foreach(var cell in package.Workbook.Worksheets[1].Cells["c:c"]) {
				if(cell.Value != null) {
					string[] nameArr = name.ToUpper().Split(' ');
					if(cell.Text.ToUpper().RemoveDiacritics().Contains(nameArr[0].ToUpper().RemoveDiacritics()) &&
					   cell.Text.ToUpper().RemoveDiacritics().Contains(nameArr[1].ToUpper().RemoveDiacritics()))
						return cell.Start.Row;
				}
			}
			return 0;
		}
		
		public Roles GetRole(string name) {
			if(ShiftLeaders.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
				return Roles.ShiftLeader;
			
			if(TEF.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
				return Roles.TEF;
			
			if(ExternalAlarms.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
				return Roles.ExternalAlarms;
			
			if(RAN.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
				return Roles.RAN;
			
			return Roles.Unknown;
		}
		
		public string GetClosureCode(string name) {
			int personRow = FindPersonRow(name);
			try { return package.Workbook.Worksheets[1].Cells[personRow, 3].Offset(0, -2).Text; }
			catch { return string.Empty; }
		}
		
		public string[] GetAllClosureCodes() {
			List<string> list = new List<string>();
			foreach(var cell in package.Workbook.Worksheets[1].Cells["a:a"]) {
				if(cell.Value != null) {
					if(cell.Value.ToString() != "Closure Code")
						list.Add(cell.Text);
				}
			}
			return list.ToArray();
		}
		
		string FindDayColumn(DateTime date) {
			foreach(var cell in package.Workbook.Worksheets[1].Cells[monthRanges[date.Month - 1].ToString().Replace("1","3")]) {
				if(cell.Value != null) {
					if(cell.Text == date.Day.ToString()) {
						return cell.Address.RemoveDigits();
					}
				}
			}
			return string.Empty;
		}
	}
}
